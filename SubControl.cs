using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using EasyModbus;
using SubControl.Barrel;
using SubControl.Ro;
using SubControl.Facility;

namespace SubControl
{
    public partial class SubControl : Form
    {
        private readonly Dictionary<string, RoScheduleForm> OpenRoForms = new Dictionary<string, RoScheduleForm>();

        private FacilityMapControl Facility;
        public BarrelControl Barrel;
        public RoControl Ro;

        private readonly System.Windows.Forms.Timer RunningTimer = new System.Windows.Forms.Timer();
        private int Running = 0; // 0=idle, 1=running (중복 실행 방지)

        public SubControl()
        {
            InitializeComponent();
            Barrel = new BarrelControl(P1IpAddress_txt.Text, Int32.Parse(P1Port_txt.Text));     // IP 및 포트 필요 시 수정

            Ro = new RoControl();
            Ro.Disconnected += Ro_Disconnected;

            // === Main Layout UI (Facility Map) ===
            Facility = new FacilityMapControl();

            // 우선 폼에 직접 붙이기 (TabControl이 없으므로). 원하면 디자이너에서 Panel 만들어 바꿀 수 있음.
            Facility.ShowLayoutUx = false; // ✅ 메인 화면은 운영용(편집 UX 숨김)
            Facility.Dock = DockStyle.Fill;
            Facility.BringToFront();
            // 별도 컨테이너가 없으므로, 최하단에 크게 붙이기: 기존 컨트롤을 유지하기 위해 SplitContainer 대체 없이 추가만 함.
            // 레이아웃 충돌이 있을 경우, 디자이너에서 Panel(LayoutHostPanel)을 하나 만들고 아래 두 줄로 교체:
            // this.LayoutHostPanel.Controls.Add(_facility);
            // _facility.Dock = DockStyle.Fill;
            this.Controls.Add(Facility);
            Facility.SendToBack(); // 배경처럼 사용 (바닥 도면)
            Facility.ConnectRequested += Facility_ConnectRequested;
            Facility.DeviceClicked += Facility_DeviceClicked;

            RunningTimer.Interval = 5000; // 5초
            RunningTimer.Tick += RunningTimer_Tick;
            RunningTimer.Start();

            // 샘플 설비 3개
            var layoutPath = Facility.AutoSavePath;

            if (!System.IO.File.Exists(layoutPath))
            {
                var d1 = Facility.AddDevice(new DeviceModel
                {
                    Name = "프린터 #1",
                    Type = DeviceType.Printer,
                    Location = new System.Drawing.Point(60, 60)
                });
                var d2 = Facility.AddDevice(new DeviceModel
                {
                    Name = "소결로 #1",
                    Type = DeviceType.Furnace,
                    Location = new System.Drawing.Point(260, 60)
                });
                var d3 = Facility.AddDevice(new DeviceModel
                {
                    Name = "연마기 #1",
                    Type = DeviceType.Polisher,
                    Location = new System.Drawing.Point(460, 60)
                });

                // 기본 샘플을 첫 실행에만 저장
                try { Facility.SaveLayout(layoutPath); } catch { }
            }
        }
        private void Ro_Disconnected(string ip)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => Ro_Disconnected(ip)));
                return;
            }

            CloseRoScheduleFormIfOpen(ip, "장비 연결이 끊겨 화면을 종료합니다.");
        }

        private async void RunningTimer_Tick(object sender, EventArgs e)
        {
            // 이전 Tick의 체크가 아직 끝나지 않았으면 스킵
            if (Interlocked.Exchange(ref Running, 1) == 1)
                return;

            try
            {
                // 네트워크 작업이므로 UI 스레드 블로킹 방지
                await Task.Run(() => CheckConnectionStatus());
            }
            catch (Exception ex)
            {
                Console.WriteLine("RunningTimer_Tick Error: " + ex.Message);
            }
            finally
            {
                Interlocked.Exchange(ref Running, 0);
            }
        }
        private void CheckConnectionStatus()
        {
            foreach (var s in Ro.GetServers())
            {
                string ip = s.Item1;
                int port = s.Item2; // 현재 RoControl.IsConnected가 ip만 받더라도, 서버 목록은 유지용

                bool connected = false;
                try
                {
                    // ✅ 여기서 절대 재연결/재검증 호출 금지
                    connected = Ro.IsConnected(ip); // 현재 프로젝트 기준
                }
                catch
                {
                    connected = false;
                }

                // UI 업데이트 (반드시 UI 스레드로)
                SafeUpdateDevice(ip, connected, connected ? "Connected" : "Disconnected");
                
                if (!connected)
                    CloseRoScheduleFormIfOpen(ip, "장비 연결이 끊겨 화면을 종료합니다.");
            }
        }

        private void SafeUpdateDevice(string ip, bool isConnected, string status)
        {
            try
            {
                if (Facility == null) return;

                if (Facility.InvokeRequired)
                {
                    Facility.BeginInvoke(new Action(() =>
                        Facility.UpdateDeviceStatusByIp(ip, isConnected, status)));
                }
                else
                {
                    Facility.UpdateDeviceStatusByIp(ip, isConnected, status);
                }
            }
            catch
            {
                // 폼 닫히는 타이밍의 ObjectDisposed 예외 방지
            }
        }


        private void OpenLayoutEditor()
        {
            using (var f = new LayoutEditorForm())
            {
                f.Show(this);
            }

            // ✅ 편집창에서 저장된 레이아웃을 메인 FacilityMap에 즉시 반영
            try
            {
                Facility.LoadLayout(Facility.AutoSavePath);
                Facility.Invalidate(); // 화면 재그리기
            }
            catch (Exception ex)
            {
                MessageBox.Show("레이아웃 반영 실패: " + ex.Message,
                    "LoadLayout Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private bool ServerExists(string ip, int port)
        {
            return Ro.GetServers().Any(s => s.Item1 == ip && s.Item2 == port);
        }

        private async void Facility_ConnectRequested(DeviceModel m)
        {
            if (m == null) return;

            // 1) 소결로만 RoControl로 붙일 거면 타입 체크
            if (m.Type != DeviceType.Furnace)
            {
                return;
            }

            // 2) IP 유효성(메뉴에서도 체크하지만, 방어적으로 한 번 더)
            var ip = (m.ServerIp ?? "").Trim();
            if (string.IsNullOrWhiteSpace(ip))
            {
                MessageBox.Show("IP가 비어있습니다. 먼저 IP 설정을 해주세요.");
                return;
            }

            // 3) 포트 결정 (현재 UI에 Port_txt가 있으니 우선 그 값을 사용, 실패 시 5000)
            int port = (m.ServerPort > 0) ? m.ServerPort : 5000;

            try
            {
                // 4) RoControl 서버 목록에 없으면 추가
                if (!ServerExists(ip, port))
                    Ro.AddRo(ip, port);

                Facility.UpdateDeviceStatusByIp(ip, false, "Verifying...");
                bool ok = await Ro.ConnectAndVerifyAsync(ip, port);

                if (!ok)
                {
                    Facility.UpdateDeviceStatusByIp(ip, false, "Verify Failed");
                    MessageBox.Show("장비 응답 검증에 실패했습니다.\n우클릭 > 연결하기로 다시 시도하세요.");
                    return;
                }

                Facility.UpdateDeviceStatusByIp(ip, true, "Connected");
            }
            catch (Exception ex)
            {
                Facility.UpdateDeviceStatusByIp(ip, false, "Connect Failed");
                MessageBox.Show($"소결로 연결 실패\n\n{ip}:{port}\n\n{ex.Message}", "Connect Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private async void Facility_DeviceClicked(DeviceModel m)
        {
            if (m == null) return;
            if (m.Type != DeviceType.Furnace) return;

            var ip = (m.ServerIp ?? "").Trim();
            if (string.IsNullOrWhiteSpace(ip))
            {
                MessageBox.Show("IP가 설정되어 있지 않습니다.\n설정창 > IP 설정을 먼저 해주세요.");
                return;
            }

            int port = (m.ServerPort > 0) ? m.ServerPort : 5000;

            try
            {
                // 0) 서버 목록에 없으면 등록 (현 흐름 유지)
                if (!ServerExists(ip, port))
                    Ro.AddRo(ip, port);

                // 1) "최소 1회 검증 연결 이력"이 없으면 좌클릭에서 절대 연결/재연결 금지
                if (!Ro.WasEverVerified(ip))
                {
                    MessageBox.Show("먼저 우클릭 > 연결하기로 연결을 1회 이상 완료해야 합니다.");
                    return;
                }

                // 2) 이력이 있으면: 끊겨있을 때만 '재연결(검증 없음)' 수행
                if (!Ro.IsConnected(ip))
                {
                    Facility.UpdateDeviceStatusByIp(ip, false, "Connecting...");

                    // ✅ 검증 없이 TCP 재연결만
                    await Ro.ConnectAsync(ip, port);

                    // 재연결 결과를 즉시 UI에 반영
                    bool nowConnected = Ro.IsConnected(ip);
                    Facility.UpdateDeviceStatusByIp(ip, nowConnected, nowConnected ? "Connected" : "Disconnected");

                    // 연결이 실패했는데도 폼을 띄우면 혼란이므로 차단(권장)
                    if (!nowConnected)
                    {
                        MessageBox.Show("현재 연결에 실패했습니다.\n우클릭 > 연결하기로 다시 시도하세요.");
                        return;
                    }
                }
                else
                {
                    // 이미 연결되어 있으면 상태를 한번 정리(선택)
                    Facility.UpdateDeviceStatusByIp(ip, true, "Connected");
                }

                // 3) 기존처럼 폼 오픈
                OpenRoScheduleForm(ip);
            }
            catch (Exception ex)
            {
                Facility.UpdateDeviceStatusByIp(ip, false, "Disconnected");
                MessageBox.Show($"소결로 연결 오류\n\nIP: {ip}:{port}\n\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private async void OpenRoScheduleForm(string ip)
        {
            // 이미 열려 있고 유효한 폼이 있으면 포커스만
            if (OpenRoForms.TryGetValue(ip, out var existing))
            {
                if (existing == null || existing.IsDisposed)
                {
                    OpenRoForms.Remove(ip);
                }
                else
                {
                    // ✅ 핵심: 통신이 살아있을 때만 기존 폼을 다시 보여준다 (Hide 복원)
                    if (Ro.IsConnected(ip))
                    {
                        try
                        {
                            existing.BeginInvoke(new Action(() =>
                            {
                                if (!existing.IsDisposed)
                                {
                                    existing.Show(this); // Hide였다면 다시 보여줌
                                    existing.WindowState = FormWindowState.Normal;
                                    existing.Activate();
                                    existing.BringToFront();
                                }
                            }));
                        }
                        catch
                        {
                            if (OpenRoForms.ContainsKey(ip))
                                OpenRoForms.Remove(ip);
                        }
                        return; // ✅ A 케이스는 여기서 끝 (재통신 없음)
                    }
                    else
                    {
                        // ✅ 통신이 끊긴 상태면: 기존 폼을 절대 띄우지 않고 새 폼을 만들게 한다
                        //    (기존 폼이 Hide로 남아있을 수 있으니 정리)
                        try { existing.Close(); } catch { }
                        OpenRoForms.Remove(ip);
                        // return 하지 않음 → 아래 새 폼 생성 로직으로 진행
                    }
                }
            }

            RoScheduleForm f = null;

            try
            {
                // 1. 폼 생성
                f = new RoScheduleForm(Ro, ip);

                // 2. 데이터 로드 시도 (여기서 1단계에서 수정한 throw가 발생하면 catch로 이동)
                // 타임아웃이 나면 여기서 대기하다가 예외가 발생합니다.
                await f.LoadFirstPageDataAsync();

                // 3. 로딩 중 닫힘 방어
                if (f.IsDisposed) return;

                // 4. 성공 시 등록 및 표시
                OpenRoForms[ip] = f;
                f.FormClosed += (s, e) => { OpenRoForms.Remove(ip); };

                f.Shown += async (s, e) =>
                {
                    try { await f.LoadRemainingPagesInBackgroundAsync(); }
                    catch { /* 백그라운드 로딩 에러는 무시하거나 로그만 */ }
                };

                f.Show(this);
            }
            catch (Exception ex)
            {
                // ★★★ 핵심 해결책 ★★★

                // 1. 사용자에게 알림 (선택 사항, 너무 자주 뜨면 제거 가능)
                // MessageBox.Show($"통신 오류로 창을 닫습니다.\n{ex.Message}", "연결 실패");

                // 2. 생성하다 만 폼이 있다면 강제로 닫고 폐기
                if (f != null && !f.IsDisposed)
                {
                    f.Close();
                    f.Dispose();
                }

                // 목록에서도 확실히 제거
                if (OpenRoForms.ContainsKey(ip))
                    OpenRoForms.Remove(ip);

                // 3. ★ 가장 중요: 꼬인 통신 풀기 (강제 연결 끊기) ★
                // 여기서 Disconnect를 호출해야 다음 번 클릭 시 'ConnectAsync'가 
                // 기존 소켓을 버리고 '새로운 소켓'을 생성합니다.
                Ro.Disconnect(ip);

                // 4. 상태 UI 업데이트 (Disconnected로 표시)
                try { Facility.UpdateDeviceStatusByIp(ip, false, "Error/Reset"); } catch { }
            }
        }
        private void CloseRoScheduleFormIfOpen(string ip, string reason = null)
        {
            if (!OpenRoForms.TryGetValue(ip, out var f) || f == null || f.IsDisposed)
                return;

            try
            {
                // UI 스레드에서 닫기
                if (f.InvokeRequired)
                {
                    f.BeginInvoke(new Action(() =>
                    {
                        try
                        {
                            if (!f.IsDisposed)
                            {
                                // 필요하면 종료 사유를 표시하고 닫기 (선택)
                                // MessageBox.Show(reason ?? "장비 연결이 끊겨 화면을 종료합니다.");

                                f.Close();
                            }
                        }
                        catch { }
                        finally
                        {
                        }
                    }));
                }
                else
                {
                    try
                    {
                        f.Close();
                    }
                    finally
                    {
                    }
                }
            }
            catch 
            {
            }
        }

        private void P1Connect_bt_Click(object sender, EventArgs e)
        {
            string ip = P1IpAddress_txt.Text.Trim();
            string portText = P1Port_txt.Text.Trim();

            if (!int.TryParse(portText, out int port))
            {
                MessageBox.Show("포트 번호가 올바르지 않습니다.");
                return;
            }

            Barrel = new BarrelControl(ip, port);                               // 사용자가 입력한 IP와 포트로 새로운 BarrelControl 객체 생성

            if (Barrel.Connect())
            {
                Status_label.Text = "Connected";
                Status_label.ForeColor = System.Drawing.Color.Green;
            
                try { Facility?.UpdateDeviceStatusByIp(P1IpAddress_txt.Text.Trim(), true, "Connected"); } catch { }
            }
            else
            {
                Status_label.Text = "Connect Failed";
                Status_label.ForeColor = System.Drawing.Color.Red;
            }
        }
        private void P1Disconnect_bt_Click(object sender, EventArgs e)
        {
            Barrel.Disconnect();
            Status_label.Text = "Disconnected";
            Status_label.ForeColor = System.Drawing.Color.Gray;
        
            try { Facility?.UpdateDeviceStatusByIp(P1IpAddress_txt.Text.Trim(), false, "Disconnected"); } catch { }
        }
        private void P1Read_bt_Click(object sender, EventArgs e)
        {
            try
            {
                int addr = int.Parse(P1ReadRegister_txt.Text);
                int[] result = Barrel.ReadRegisters(addr, 1); // 한 개 읽기
                P1Read_label.Text = result[0].ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Read Error: " + ex.Message);
            }
        }
        private void P1ReadBit_bt_Click(object sender, EventArgs e)
        {
            try
            {
                int coilAddress = int.Parse(P1ReadRegister_txt.Text);  // Coil 주소 직접 입력
                bool bitValue = Barrel.ReadBitFromCoil(coilAddress);   // Coil에서 읽기
                P1ReadBit_label.Text = bitValue ? "1" : "0";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bit Read Error: " + ex.Message);
            }
        }
        private void P1Write_bt_Click(object sender, EventArgs e)
        {
            try
            {
                int addr = int.Parse(P1WriteRegister_txt.Text);
                int val = int.Parse(P1Value_txt.Text);

                Barrel.WriteRegister(addr, val);

                // ✅ 결과 라벨에 성공 메시지 출력
                P1Write_label.Text = $"[{addr}] ← {val}";
                P1Write_label.ForeColor = Color.Green;  // 성공 시 초록색
            }
            catch (Exception ex)
            {
                // ✅ 예외 메시지 출력
                P1Write_label.Text = "Write Error: " + ex.Message;
                P1Write_label.ForeColor = Color.Red;  // 실패 시 빨간색

                MessageBox.Show("Write Error: " + ex.Message);
            }
        }
        private async void P1WriteBit_bt_Click(object sender, EventArgs e)
        {
            try
            {
                int coilAddress = int.Parse(P1WriteRegister_txt.Text);       // Coil 주소
                string input = P1BitTureFalse_txt.Text.Trim();               // 입력값: 1 또는 0
                bool value = input == "1" ? true :
                             input == "0" ? false :
                             throw new Exception("비트 값은 1 또는 0만 입력하세요.");

                if (P1PulseMode_bt.Text == "Pulse")
                {
                    // ✅ Pulse 모드: 1 쓰고 일정 시간 후 0으로 자동 리셋
                    await Task.Run(() => Barrel.WriteBitToCoil(coilAddress, true));
                    await Task.Delay(200); // 200ms 유지
                    await Task.Run(() => Barrel.WriteBitToCoil(coilAddress, false));

                    P1WriteBit_label.Text = $"[Coil {coilAddress}] PULSE: 1 → 0";
                    P1WriteBit_label.ForeColor = Color.DarkOrange;
                }
                else
                {
                    // ✅ 일반 비트 쓰기
                    await Task.Run(() => Barrel.WriteBitToCoil(coilAddress, value));
                    P1WriteBit_label.Text = $"[Coil {coilAddress}] ← {(value ? 1 : 0)}";
                    P1WriteBit_label.ForeColor = Color.Green;
                }
            }
            catch (Exception ex)
            {
                P1WriteBit_label.Text = "Write Error: " + ex.Message;
                P1WriteBit_label.ForeColor = Color.Red;
                MessageBox.Show("Write Error: " + ex.Message);
            }
        }
        private void P1PulseMode_bt_Click(object sender, EventArgs e)
        {
            if (P1PulseMode_bt.Text == "Pulse")
                P1PulseMode_bt.Text = "Nomal";
            else
                P1PulseMode_bt.Text = "Pulse";
        }

        private void Edit_bt_Click(object sender, EventArgs e)
        {
            OpenLayoutEditor();
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            try
            {
                RunningTimer.Stop();
                RunningTimer.Tick -= RunningTimer_Tick;
            }
            catch { }
            base.OnFormClosing(e);
        }

    }
}