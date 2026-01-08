using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SubControl.Ro
{
    public partial class RoScheduleForm : Form
    {
        private Dictionary<int, string> ScheduleData = new Dictionary<int, string>();   // 전체 데이터 저장
        private DateTime LastSuccessTime = DateTime.MinValue;                           // 통신 성공 시각
        private System.Windows.Forms.Timer PageRefreshTimer;                            // 페이지 자동 새로고침을 위한 타이머
        private bool LoadingCurrentPage = false;
        private Label[] ScheduleLabels = new Label[10];                                 // 기존에 그려진 레이블들을 참조할 배열
        
        private CancellationTokenSource Cts = new CancellationTokenSource();
        private volatile bool closing = false;
        private volatile bool Disconnected = false;                                     // ✅ 연결 끊김 상태 플래그

        private int CurrentPage = 1;
        private int TotalPages = 5;
        private readonly RoControl Ro;
        private readonly string Ip;

        // [Move 모드용 변수]
        private bool MoveMode = false;
        private int MoveIndex = -1;                                                     // 이동할 데이터 인덱스
        private int MoveDisplayIndex = -1;                                              // 이동할 라벨 화면 인덱스

        // [Copy 모드용 변수]
        private bool CopyMode = false;
        private int CopyIndex = -1;                                                     // -1이면 선택 안됨, 0이상이면 원본 선택됨
        private int CopyDisplayIndex = -1;                                              // 원본 라벨의 화면 위치 인덱스 (0~9) - 색상 복구용

        // [Change 모드용 변수]
        private bool ChangeMode = false;
        private int ChangeDisplayIndex = -1;                                           // 0~9, 선택된 라벨(현재 페이지에서의 위치)

        public RoScheduleForm(RoControl ro, string ip)
        {
            try
            {
                InitializeComponent();
                InitButtonStyle(Change_bt);                 // Change 버튼 초기화
                InitButtonStyle(Copy_bt);                   // Copy 버튼 초기화
                InitButtonStyle(Move_bt);                   // Move 버튼 초기화

                if (ro == null)
                    throw new ArgumentNullException(nameof(ro));
                if (string.IsNullOrWhiteSpace(ip))
                    throw new ArgumentException("IP cannot be null or empty", nameof(ip));

                Ro = ro;
                Ip = ip;

                Ro.Disconnected += Ro_Disconnected;

                Text = $"RO Schedule - {Ip}";

                ScheduleLabels[0] = Schedule1_label;
                ScheduleLabels[1] = Schedule2_label;
                ScheduleLabels[2] = Schedule3_label;
                ScheduleLabels[3] = Schedule4_label;
                ScheduleLabels[4] = Schedule5_label;
                ScheduleLabels[5] = Schedule6_label;
                ScheduleLabels[6] = Schedule7_label;
                ScheduleLabels[7] = Schedule8_label;
                ScheduleLabels[8] = Schedule9_label;
                ScheduleLabels[9] = Schedule10_label;

                for (int i = 0; i < 10; i++)
                {
                    if (ScheduleLabels[i] == null)
                        throw new NullReferenceException($"ScheduleLabels[{i}] is null. Check Designer file.");
                    
                    int itemIndex = i;
                    ScheduleLabels[i].Click += (s, e) => ScheduleItem_Click(itemIndex);
                    ScheduleLabels[i].Cursor = Cursors.Hand;
                }
                
                // ✅ 화면 업데이트
                UpdatePageDisplay();

                // 페이지 자동 새로고침 타이머 초기화
                PageRefreshTimer = new System.Windows.Forms.Timer();
                PageRefreshTimer.Interval = 1000; // 1초마다 체크
                PageRefreshTimer.Tick += PageRefreshTimer_Tick;
                PageRefreshTimer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"RoScheduleForm 생성 오류:\n\n{ex.Message}\n\n{ex.StackTrace}", 
                    "초기화 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw; // 예외를 다시 던져서 호출자에게 알림
            }
        }
        private void Ro_Disconnected(string disconnectedIp)
        {
            if (disconnectedIp != Ip)
                return;

            if (Disconnected) 
                return;   // ✅ 중복 처리 방지
            Disconnected = true;        // ✅ 반드시 필요
            closing = true;               // 🔸 선택(원하면 유지)

            // 1) 모든 백그라운드 작업 중단
            try { Cts.Cancel(); } catch { }

            // 2) 타이머 중단
            if (PageRefreshTimer != null)
            {
                try
                {
                    PageRefreshTimer.Stop();
                    PageRefreshTimer.Tick -= PageRefreshTimer_Tick;
                    PageRefreshTimer.Dispose();
                    PageRefreshTimer = null;
                }
                catch { }
            }

            // 3) UI 알림 (Invoke 보장)
            if (IsHandleCreated)
            {
                BeginInvoke(new Action(() =>
                {
                    ChangeMode = false;
                    Disconnected = true;  
                    this.Close();           
                }));
            }
        }
        private bool IsCommTimeoutOrClosed(Exception ex)
        {
            var msg = ex.ToString();
            return msg.IndexOf("timeout", StringComparison.OrdinalIgnoreCase) >= 0
                || msg.IndexOf("connection closed", StringComparison.OrdinalIgnoreCase) >= 0
                || ex is ObjectDisposedException
                || ex is System.IO.IOException
                || ex is System.Net.Sockets.SocketException;
        }

        private void StopFurtherCommDueToFatal()
        {
            if (Disconnected) return; // 이미 처리했으면 패스
            Disconnected = true;
            closing = true;

            // 1. 작업 취소 토큰 발동
            try { Cts.Cancel(); } catch { }

            // 2. 타이머 정지
            try
            {
                if (PageRefreshTimer != null)
                {
                    PageRefreshTimer.Stop();
                    PageRefreshTimer.Tick -= PageRefreshTimer_Tick; // 이벤트 연결 해제
                    PageRefreshTimer.Dispose();
                    PageRefreshTimer = null;
                }
            }
            catch { }

            // ★★★ 핵심 수정: UI 스레드에서 폼을 강제로 닫아버림 ★★★
            if (!this.IsDisposed && this.IsHandleCreated)
            {
                this.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        // 사용자에게 알림 (선택 사항)
                        // MessageBox.Show("통신 상태가 불안정하여 창을 닫습니다.");
                        this.Close();
                    }
                    catch { }
                }));
            }
        }

        public async Task LoadFirstPageDataAsync()
        {
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    if (Disconnected) throw new Exception("Disconnected state"); // 연결 끊김 상태면 즉시 중단

                    await Ro.SingleWriteAsync(Ip, "1", "1C", i.ToString());
                    await Task.Delay(10);
                    var Result = await Ro.ReadAsync(Ip, "1", "1D", "16");

                    if (!string.IsNullOrWhiteSpace(Result.DisplayText))
                    {
                        ScheduleData[i] = Result.DisplayText;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to read index {i}: {ex.Message}");

                    // ★ 핵심 수정: 타임아웃이나 치명적 에러면 예외를 밖으로 던짐 ★
                    if (IsCommTimeoutOrClosed(ex))
                    {
                        StopFurtherCommDueToFatal();
                        throw; // 메인 폼(SubControl)의 catch로 점프하게 만듦
                    }
                }
            }
            // 화면 업데이트
            UpdatePageDisplay();
        }

        // 나머지 페이지 백그라운드 로드
        public async Task LoadRemainingPagesInBackgroundAsync()
        {
            for (int actualIndex = 10; actualIndex < 50; actualIndex++)
            {
                if (Disconnected) return;

                try
                {
                    await Ro.SingleWriteAsync(Ip, "1", "1C", actualIndex.ToString());
                    await Task.Delay(10);

                    var Result = await Ro.ReadAsync(Ip, "1", "1D", "16");

                    if (!string.IsNullOrWhiteSpace(Result.DisplayText))
                    {
                        ScheduleData[actualIndex] = Result.DisplayText;
                        LastSuccessTime = DateTime.Now;
                    }

                    // 현재 보이는 페이지가 로드된 페이지면 UI 업데이트
                    int loadedPage = (actualIndex / 10) + 1;
                    if (loadedPage == CurrentPage)
                    {
                        if (InvokeRequired)
                            Invoke(new Action(() => UpdatePageDisplay()));
                        else
                            UpdatePageDisplay();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to read index {actualIndex}: {ex.Message}");

                    if (IsCommTimeoutOrClosed(ex))
                    {
                        // 이 함수가 호출되면 위에서 수정한대로 폼이 닫힙니다.
                        StopFurtherCommDueToFatal();
                        return;
                    }
                }
            }
        }

        private async Task LoadCurrentPageDataAsync()
        {
            int startIndex = (CurrentPage - 1) * 10;

            for (int i = 0; i < 10; i++)
            {
                if (Cts.IsCancellationRequested || closing) return;
                if (Disconnected) return;

                int actualIndex = startIndex + i;

                if (ScheduleData.ContainsKey(actualIndex))
                    continue;

                try
                {
                    await Ro.SingleWriteAsync(Ip, "1", "1C", actualIndex.ToString());
                    await Task.Delay(10, Cts.Token);

                    var Result = await Ro.ReadAsync(Ip, "1", "1D", "16");

                    if (!string.IsNullOrWhiteSpace(Result.DisplayText))
                        ScheduleData[actualIndex] = Result.DisplayText;
                }
                catch (OperationCanceledException)
                {
                    return;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to read index {actualIndex}: {ex.Message}");

                    if (IsCommTimeoutOrClosed(ex))
                    {
                        StopFurtherCommDueToFatal();
                        return;
                    }
                }
            }

            // UI 업데이트는 폼 유효할 때만
            if (closing || IsDisposed) return;
            if (!IsHandleCreated) return;

            if (InvokeRequired)
                BeginInvoke(new Action(() => { if (!closing && !IsDisposed) UpdatePageDisplay(); }));
            else
                UpdatePageDisplay();
        }
        public bool NeedsReconnect(TimeSpan maxIdle)
        {
            if (Disconnected) return true;
            if (LastSuccessTime == DateTime.MinValue) return true;

            return (DateTime.Now - LastSuccessTime) > maxIdle;
        }

        // 타이머 Tick 이벤트: 현재 페이지 데이터가 없으면 주기적으로 로드
        private async void PageRefreshTimer_Tick(object sender, EventArgs e)
        {
            if (Disconnected) return;   
            if (closing || IsDisposed) return;
            if (Cts.IsCancellationRequested) return;

            // 현재 페이지 데이터가 모두 있는지 확인
            bool needLoad = false;
            int startIndex = (CurrentPage - 1) * 10;

            for (int i = 0; i < 10; i++)
            {
                int actualIndex = startIndex + i;
                if (!ScheduleData.ContainsKey(actualIndex))
                {
                    needLoad = true;
                    break;
                }
            }

            // 데이터가 없고, 현재 로딩 중이 아니면 로드
            if (needLoad && !LoadingCurrentPage)
            {
                LoadingCurrentPage = true;
                try
                {
                    await LoadCurrentPageDataAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Page refresh error: {ex.Message}");
                }
                finally
                {
                    LoadingCurrentPage = false;
                }
            }
        }
        private async void PerformScheduleCopy(int srcIndex, int tgtIndex)
        {
            try
            {
                // 1. 메모리상 데이터 복사 (이름)
                string srcName = "";
                if (ScheduleData.ContainsKey(srcIndex))
                    srcName = ScheduleData[srcIndex];

                ScheduleData[tgtIndex] = srcName;
                UpdatePageDisplay();

                // 2. 장비에 이름 쓰기 (기존 NameChange 로직과 동일하게 CP949 패딩 적용)
                var enc = System.Text.Encoding.GetEncoding(949);
                byte[] raw = enc.GetBytes(srcName);
                byte[] buf = new byte[32]; // 16워드
                for (int i = 0; i < buf.Length; i++) buf[i] = 0x20; // 공백 채움
                int copyLen = raw.Length > 32 ? 32 : raw.Length;
                Buffer.BlockCopy(raw, 0, buf, 0, copyLen);
                string paddedValue = enc.GetString(buf);

                // 이름 쓰기 통신
                await Ro.SingleWriteAsync(Ip, "1", "1C", tgtIndex.ToString()); // 인덱스 선택
                await Task.Delay(10);
                await Ro.MultiWriteAsync(Ip, "1", "1D", paddedValue); // 이름 쓰기

                // 3. 스케줄 내용(레지스터) 복사
                // TODO: 나중에 레지스터 주소를 알려주시면 여기에 추가하면 됩니다.
                // 예: await CopyRegistersAsync(srcIndex, tgtIndex);
            }
            catch (Exception ex)
            {
                // 5. 통신 실패 시 롤백 (선택 사항)
                // 실패했다면 사용자에게 알리고 원래대로 돌려놓거나, 에러 메시지만 띄웁니다.
                MessageBox.Show("스케줄 복사 통신 실패: " + ex.Message);

                // (옵션) 화면을 다시 원래대로 돌리고 싶다면 여기서 다시 로드하거나 롤백
                await LoadCurrentPageDataAsync(); 
                UpdatePageDisplay();
            }
        }
        private async void ScheduleItem_Click(int displayIndex)
        {
            ChangeDisplayIndex = displayIndex;   // ★ 추가: 어떤 라벨을 눌렀는지 저장
            
            // Move Mode 처리
            if (MoveMode)
            {
                int ActualIndex = GetActualIndex(displayIndex);

                if (MoveIndex == -1)
                {
                    // 첫 번째 선택
                    MoveIndex = ActualIndex;
                    MoveDisplayIndex = displayIndex;
                    ScheduleLabels[displayIndex].BackColor = Color.FromArgb(229, 135, 160);
                }
                else
                {
                    // 두 번째 선택
                    int targetIndex = ActualIndex;
                    ScheduleLabels[displayIndex].BackColor = Color.FromArgb(229, 135, 160);
                    await Task.Delay(100);

                    // 색상 복구
                    if (MoveDisplayIndex >= 0 && MoveDisplayIndex < 10 && ScheduleLabels[MoveDisplayIndex] != null)
                        ScheduleLabels[MoveDisplayIndex].BackColor = Color.Transparent;
                    ScheduleLabels[displayIndex].BackColor = Color.Transparent;

                    if (MoveIndex != targetIndex)
                    {
                        PerformScheduleSwap(MoveIndex, targetIndex);
                    }

                    ResetAllModes();
                }
                return;
            }

            // Copy Mded 처리
            if (CopyMode)
            {
                int ActualIndex = GetActualIndex(displayIndex);

                if (CopyIndex == -1)
                {
                    // [1단계] 원본 선택
                    CopyIndex = ActualIndex;
                    CopyDisplayIndex = displayIndex; // 화면상 위치 저장 (나중에 색 뺄 때 사용)

                    // 원본 라벨 색상 변경 (선택됨 표시)
                    ScheduleLabels[displayIndex].BackColor = Color.FromArgb(229, 135, 160);
                }
                else
                {
                    // [2단계] 대상 선택
                    int targetIndex = ActualIndex;

                    // 대상 라벨도 잠시 색상 변경 (눌렸다는 피드백)
                    ScheduleLabels[displayIndex].BackColor = Color.FromArgb(229, 135, 160);

                    // [추가] 0.2초 동안 대기 (사용자가 색이 바뀐걸 인지할 시간)
                    await Task.Delay(100);

                    // --- 색상 초기화 (반복문 없이 딱 2개만) ---
                    // 1. 원본 라벨 색상 복구
                    if (CopyDisplayIndex >= 0 && CopyDisplayIndex < 10)
                        ScheduleLabels[CopyDisplayIndex].BackColor = Color.Transparent;
                    // 2. 지금 누른 대상 라벨 색상 복구
                    ScheduleLabels[displayIndex].BackColor = Color.Transparent;
                    // ----------------------------------------

                    if (CopyIndex != targetIndex)
                        PerformScheduleCopy(CopyIndex, targetIndex);

                    ResetAllModes();
                }
                return;
            }

            // Change Mode 처리
            if (ChangeMode)
            {
                int actualIndex = GetActualIndex(displayIndex);
                string currentName = "";
                if (ScheduleData.ContainsKey(actualIndex)) currentName = ScheduleData[actualIndex];

                CenteredTextBox txtInput = new CenteredTextBox
                {
                    BorderStyle = BorderStyle.None,
                    BackColor = ScheduleLabels[displayIndex].Parent.BackColor,
                    Font = ScheduleLabels[displayIndex].Font,
                    Text = currentName,
                    TreatAsSingleLine = true,
                    Parent = ScheduleLabels[displayIndex].Parent,
                    Location = ScheduleLabels[displayIndex].Location,
                    Size = ScheduleLabels[displayIndex].Size,
                    TextAlign = HorizontalAlignment.Center
                };

                ScheduleLabels[displayIndex].Visible = false;
                txtInput.BringToFront();
                txtInput.Focus();
                txtInput.SelectAll();

                txtInput.KeyDown += async (s, e) =>
                {
                    if (e.KeyCode == Keys.Enter)
                    {
                        e.Handled = true; e.SuppressKeyPress = true;
                        string newVal = txtInput.Text ?? "";

                        ScheduleData[actualIndex] = newVal;
                        UpdatePageDisplay();

                        ScheduleLabels[displayIndex].Visible = true;
                        txtInput.Dispose();

                        try
                        {
                            var enc = System.Text.Encoding.GetEncoding(949);
                            await WriteNameToDevice(actualIndex, newVal, enc);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("저장 실패: " + ex.Message);
                        }

                        ResetAllModes();
                        this.ActiveControl = null;
                    }
                };

                txtInput.LostFocus += (s, e) =>
                {
                    string newVal = txtInput.Text ?? "";
                    ScheduleData[actualIndex] = newVal;
                    UpdatePageDisplay();
                    ScheduleLabels[displayIndex].Visible = true;
                    txtInput.Dispose();
                };
            }
        }

        private async void Read_label_Click(object sender, EventArgs e)
        {
            try
            {
                await Ro.SingleWriteAsync(Ip, "1", "1C", "0");

                await Task.Delay(10);

                var Result = await Ro.ReadAsync(Ip, "1", "1D", "16");
                Schedule1_label.Text = Result.DisplayText;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Read failed: " + ex.Message);
            }
        }
        private int GetActualIndex(int displayIndex)
        {
            return (CurrentPage - 1) * 10 + displayIndex;
        }
        private async void BeforePage_Click(object sender, EventArgs e)
        {
            if (ChangeMode) 
                return;

            if (CurrentPage > 1)
                CurrentPage--;
            else
                CurrentPage = TotalPages;   // 1 -> 5

            UpdatePageDisplay();
            await TryLoadCurrentPageIfNeeded();
        }

        // 다음 페이지 버튼 클릭
        private async void AfterPage_Click(object sender, EventArgs e)
        {
            if (ChangeMode)
                return;

            if (CurrentPage < TotalPages)
                CurrentPage++;
            else
                CurrentPage = 1;            // 5 -> 1

            UpdatePageDisplay();
            await TryLoadCurrentPageIfNeeded();
        }

        // 현재 페이지 데이터가 없으면 즉시 로드 시도
        private async Task TryLoadCurrentPageIfNeeded()
        {
            // 현재 로딩 중이면 스킵
            if (LoadingCurrentPage)
                return;

            // 현재 페이지 데이터가 모두 있는지 확인
            int startIndex = (CurrentPage - 1) * 10;
            bool needLoad = false;

            for (int i = 0; i < 10; i++)
            {
                int actualIndex = startIndex + i;
                if (!ScheduleData.ContainsKey(actualIndex))
                {
                    needLoad = true;
                    break;
                }
            }

            // 데이터가 없으면 즉시 로드
            if (needLoad)
            {
                LoadingCurrentPage = true;
                try
                {
                    await LoadCurrentPageDataAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Try load current page error: {ex.Message}");
                }
                finally
                {
                    LoadingCurrentPage = false;
                }
            }
        }
        private void UpdatePageDisplay()
        {
            SetPage_label.Text = $"{CurrentPage}";

            for (int i = 0; i < 10; i++)
            {
                int actualIndex = GetActualIndex(i);
                int displayNumber = actualIndex + 1;

                if (ScheduleData.ContainsKey(actualIndex))
                {
                    ScheduleLabels[i].Text = $"{displayNumber}. {ScheduleData[actualIndex]}";
                }
                else
                {
                    ScheduleLabels[i].Text = $"{displayNumber}.";
                }
            }
        }
        private async void PerformScheduleSwap(int index1, int index2)
        {
            try
            {
                // 1. 메모리상 데이터 가져오기
                string name1 = ScheduleData.ContainsKey(index1) ? ScheduleData[index1] : "";
                string name2 = ScheduleData.ContainsKey(index2) ? ScheduleData[index2] : "";

                // 2. 메모리상 데이터 교환 (Swap)
                ScheduleData[index1] = name2;
                ScheduleData[index2] = name1;

                // 3. 화면 즉시 갱신 (반응성 향상)
                UpdatePageDisplay();

                // 4. 장비에 쓰기 (두 번 Write - 백그라운드)
                var enc = System.Text.Encoding.GetEncoding(949);

                // (1) Index1에 Name2 쓰기
                await WriteNameToDevice(index1, name2, enc);

                // (2) Index2에 Name1 쓰기
                await WriteNameToDevice(index2, name1, enc);
            }
            catch (Exception ex)
            {
                MessageBox.Show("이동 실패: " + ex.Message);
                // 실패 시 원복을 위해 다시 로드
                await LoadCurrentPageDataAsync();
                UpdatePageDisplay();
            }
        }
        private async Task WriteNameToDevice(int index, string name, System.Text.Encoding enc)
        {
            byte[] raw = enc.GetBytes(name ?? "");
            byte[] buf = new byte[32]; // 16워드 공백 패딩
            for (int i = 0; i < buf.Length; i++) buf[i] = 0x20;

            int len = raw.Length > 32 ? 32 : raw.Length;
            Buffer.BlockCopy(raw, 0, buf, 0, len);
            string paddedValue = enc.GetString(buf);

            await Ro.SingleWriteAsync(Ip, "1", "1C", index.ToString());
            await Task.Delay(10);
            await Ro.MultiWriteAsync(Ip, "1", "1D", paddedValue);
            await Task.Delay(10); // 연속 쓰기 안정성 확보
        }
        private void Move_bt_Click(object sender, EventArgs e)
        {
            if (MoveMode)
            {
                ResetAllModes();
            }
            else
            {
                ResetAllModes();
                MoveMode = true;
                MoveIndex = -1;
                MoveDisplayIndex = -1;

                // [색상 통일] 붉은색 적용
                Move_bt.BackColor = Color.FromArgb(239, 72, 54);
                Move_bt.ForeColor = Color.White;
            }
            this.ActiveControl = null;
        }
        private void Copy_bt_Click(object sender, EventArgs e)
        {
            if (CopyMode)
            {
                ResetAllModes();
            }
            else
            {
                ResetAllModes();
                CopyMode = true;
                CopyIndex = -1;
                CopyDisplayIndex = -1;

                // [색상 통일] 붉은색 적용
                Copy_bt.BackColor = Color.FromArgb(239, 72, 54);
                Copy_bt.ForeColor = Color.White;
            }
            this.ActiveControl = null;
        }
        private async void Change_bt_Click(object sender, EventArgs e)
        {
            // Case 1: 이미 켜져 있는 경우 -> 저장하고 끄기 (Reset)
            if (ChangeMode)
            {
                // 선택된 항목이 있다면 저장 로직 수행
                if (ChangeDisplayIndex >= 0)
                {
                    try
                    {
                        int ActualIndex = GetActualIndex(ChangeDisplayIndex); // 0 ~ 49

                        // 데이터 가져오기 (없으면 빈 값)
                        string Data = "";
                        if (ScheduleData.ContainsKey(ActualIndex))
                            Data = ScheduleData[ActualIndex];
                        if (Data == null) Data = "";

                        // --- [CP949 32바이트 패딩 로직] ---
                        var enc = System.Text.Encoding.GetEncoding(949);
                        byte[] raw = enc.GetBytes(Data);
                        byte[] buf = new byte[32];
                        for (int i = 0; i < buf.Length; i++) buf[i] = 0x20; // 공백 채움

                        int copyLen = raw.Length > 32 ? 32 : raw.Length;
                        Buffer.BlockCopy(raw, 0, buf, 0, copyLen);

                        string paddedValue = enc.GetString(buf);
                        // --------------------------------

                        // 실제 쓰기 통신
                        await Ro.SingleWriteAsync(Ip, "1", "1C", ActualIndex.ToString());
                        await Task.Delay(10);
                        await Ro.MultiWriteAsync(Ip, "1", "1D", paddedValue);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("저장 실패: " + ex.Message);
                    }
                }

                // 모든 모드 및 UI 초기화 (이름 변경 모드도 여기서 꺼짐)
                ResetAllModes();
            }
            // Case 2: 꺼져 있는 경우 -> 다른 모드 끄고 켜기
            else
            {
                // ★ 다른 모드(Copy 등)가 켜져있을 수 있으므로 싹 정리
                ResetAllModes();

                // 이름 변경 모드 활성화
                ChangeMode = true;
                Change_bt.BackColor = Color.FromArgb(239, 72, 54); // 켜짐 색상
            }
            this.ActiveControl = null;
        }

        private void ResetAllModes()
        {
            // 1. Naming(Change) 모드 초기화
            if (ChangeMode)
            {
                ChangeMode = false;
                Change_bt.BackColor = SystemColors.Control;
                Change_bt.UseVisualStyleBackColor = true;
                ChangeDisplayIndex = -1;
                InitButtonStyle(Change_bt);
            }

            // 2. Copy 모드 초기화
            if (CopyMode)
            {
                CopyMode = false;
                Copy_bt.BackColor = SystemColors.Control;
                Copy_bt.UseVisualStyleBackColor = true;

                // 색상 복구 (원본으로 선택된 라벨이 있다면)
                if (CopyDisplayIndex >= 0 && CopyDisplayIndex < 10)
                {
                    if (ScheduleLabels[CopyDisplayIndex] != null)
                        ScheduleLabels[CopyDisplayIndex].BackColor = Color.Transparent;
                }

                CopyIndex = -1;
                CopyDisplayIndex = -1;
                InitButtonStyle(Copy_bt);
            }

            // 3. Move 모드 초기화
            if (MoveMode)
            {
                MoveMode = false;
                // 선택된 라벨 색상 복구
                if (MoveDisplayIndex >= 0 && MoveDisplayIndex < 10 && ScheduleLabels[MoveDisplayIndex] != null)
                    ScheduleLabels[MoveDisplayIndex].BackColor = Color.Transparent;

                MoveIndex = -1;
                MoveDisplayIndex = -1;
                InitButtonStyle(Move_bt);
            }
        }
        private void InitButtonStyle(Button btn)
        {
            if (btn == null) 
                return;
            
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 1;                  // 테두리 두께 1
            btn.FlatAppearance.BorderColor = Color.DarkGray;    // 테두리 색상 (회색)
            btn.BackColor = SystemColors.Control;               // 배경색 (기본)
            btn.ForeColor = Color.Black;                        // 글자색 (검정)
            btn.TabStop = false;                                // 포커스 점선 제거 시 주석 해제
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // [수정] 종료 사유(CloseReason)를 확인하여 처리 방식을 나눔

            // 1. 진짜로 닫아야 하는 경우 (부모 폼 종료, 앱 종료, 작업 관리자 종료 등)
            //    또는 이미 연결이 끊겨서(isDisconnected) 닫아야 하는 경우
            if (e.CloseReason == CloseReason.FormOwnerClosing ||
                e.CloseReason == CloseReason.MdiFormClosing ||
                e.CloseReason == CloseReason.ApplicationExitCall ||
                e.CloseReason == CloseReason.WindowsShutDown ||
                e.CloseReason == CloseReason.TaskManagerClosing ||
                Disconnected)
            {
                // 여기서는 e.Cancel을 하지 않고 그대로 아래의 "진짜 종료 로직"으로 흘려보냄
                closing = true;
            }
            // 2. 사용자가 X 버튼을 누른 경우 (단순 화면 숨김)
            else
            {
                e.Cancel = true;   // 닫기 취소 (메모리에서 제거 안 함)
                closing = false;   // 닫힌 상태 아님

                try
                {
                    // 숨겨져 있을 때는 굳이 타이머를 돌릴 필요 없으므로 정지
                    if (PageRefreshTimer != null)
                        PageRefreshTimer.Stop();
                }
                catch { }

                this.Hide();       // 화면에서만 숨김
                return;            // 리턴하여 아래의 Dispose 로직 실행 방지
            }

            // --- [진짜 종료 로직] (자원 해제) ---
            try { Cts.Cancel(); } catch { }
            try { Ro.Disconnected -= Ro_Disconnected; } catch { }

            try
            {
                if (PageRefreshTimer != null)
                {
                    PageRefreshTimer.Stop();
                    PageRefreshTimer.Tick -= PageRefreshTimer_Tick;
                    PageRefreshTimer.Dispose();
                    PageRefreshTimer = null;
                }
            }
            catch { }

            base.OnFormClosing(e);
        }
    }
}
