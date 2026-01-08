using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace SubControl.Ro
{
    public class RoControl
    {
        private readonly List<Tuple<string, int>> Ro_List = new List<Tuple<string, int>>();                      // Ro 목록
        private readonly Dictionary<string, TcpClient> ClientDict = new Dictionary<string, TcpClient>();         // Ro별 TcpClient 관리
        private readonly Dictionary<string, NetworkStream> StreamDict = new Dictionary<string, NetworkStream>(); // Ro별 NetworkStream 관리
        private readonly Dictionary<string, bool> VerifiedDict = new Dictionary<string, bool>();
        private readonly ConcurrentDictionary<string, SemaphoreSlim> IpLocks  = new ConcurrentDictionary<string, SemaphoreSlim>();
        private readonly HashSet<string> VerifiedOnce = new HashSet<string>();
        private ushort _transactionId = 0; // Transaction ID 카운터
        public event Action<string, string, string, string> OnDataUpdate;                                        // (function, type, message, serverIp)
        public event Action<string> Disconnected;
        public IEnumerable<Tuple<string, int>> GetServers() => Ro_List;

        private SemaphoreSlim GetIpLock(string ip)
        {
            return IpLocks.GetOrAdd(ip, _ => new SemaphoreSlim(1, 1));
        }

        private ushort GetNextTransactionId()
        {
            return _transactionId++;
        }

        public class RoReadResult
        {
            public string DisplayText { get; set; } = "";      // 라벨에 그대로 표시할 문자열
            public bool IsLikelyText { get; set; }             // 문자로 판단되면 true
            public ushort[] UShortValues { get; set; } = Array.Empty<ushort>(); // 숫자값(항상 제공)
        }

        public void AddRo(string ip, int port)
        {
            if (Ro_List.Exists(x => x.Item1 == ip && x.Item2 == port))
                throw new Exception("This server is already added.");

            Ro_List.Add(new Tuple<string, int>(ip, port));
        }

        public async Task StartCommunicationAsync()
        {
            foreach (var (ip, port) in Ro_List)
            {
                try { await ConnectAsync(ip, port); }
                catch { continue; }
            }
        }

        public async Task ConnectAsync(string ip, int port)
        {
            // 1) 기존에 연결된 기록이 있는지 확인
            if (ClientDict.TryGetValue(ip, out var existing))
            {
                // ★ 중요: 살아있는지 확인해보고, 죽었거나 에러나면 "Disconnect" 호출해서 치워버림
                if (!IsSocketConnected(existing))
                {
                    Log($"Found dead connection for {ip}. Cleaning up...");
                    Disconnect(ip); // 딕셔너리에서 제거하고 소켓 파괴
                }
                else
                {
                    // 진짜 살아있다면 재사용 (로그 남기지 않고 리턴)
                    return;
                }
            }

            // 2) 여기까지 왔다는 건 '없거나' '죽어서 치웠거나' 둘 중 하나임.
            //    이제 깨끗한 상태에서 새 연결 시도
            var client = new TcpClient();

            try
            {
                // 타임아웃 1.5초 설정 (너무 길면 UI가 답답해짐)
                var t = client.ConnectAsync(ip, port);
                var completed = await Task.WhenAny(t, Task.Delay(1000));

                if (completed != t)
                {
                    // 시간 초과
                    try { client.Close(); } catch { }
                    throw new TimeoutException("Connect timeout (1.5s)");
                }

                // 연결 시도 자체에서 에러가 났는지 확인 (await t)
                await t;

                if (!client.Connected)
                {
                    throw new Exception("Socket not connected after task completion.");
                }
            }
            catch (Exception ex)
            {
                // 연결 실패 시 즉시 정리
                try { client.Close(); } catch { }
                throw new Exception($"Connection failed: {ex.Message}");
            }

            // 3) 성공 시 등록
            ClientDict[ip] = client;
            StreamDict[ip] = client.GetStream();

            Log($"Connected to {ip}:{port}");
        }

        // ✅ "우클릭 연결하기"에서 사용: TCP 연결 + (간단) 검증 통과 시에만 true
        public async Task<bool> ConnectAndVerifyAsync(string serverIp, int port)
        {
            try
            {
                await ConnectAsync(serverIp, port);

                bool ok = IsConnected(serverIp);
                VerifiedDict[serverIp] = ok;

                if (ok)
                {
                    VerifiedOnce.Add(serverIp); // ✅ 검증 성공 이력 기록(좌클릭 복구 허용 조건)
                    Log($"[RO] Verified Connected: {serverIp}:{port}");
                }
                else
                {
                    Log($"[RO] Verify failed: {serverIp}:{port}");
                }

                return ok;
            }
            catch (Exception ex)
            {
                VerifiedDict[serverIp] = false;
                Log($"[RO] ConnectAndVerify 실패: {serverIp}:{port} - {ex.Message}");
                return false;
            }
        }

        public bool IsConnected(string serverIp)
        {
            if (string.IsNullOrWhiteSpace(serverIp)) 
                return false;
            if (!ClientDict.ContainsKey(serverIp)) 
                return false;

            var c = ClientDict[serverIp];
            return IsSocketConnected(c);
        }
        public bool WasEverVerified(string serverIp)
        {
            if (string.IsNullOrWhiteSpace(serverIp)) return false;
            return VerifiedOnce.Contains(serverIp);
        }
        public bool IsVerifiedConnected(string serverIp)
        {
            if (string.IsNullOrWhiteSpace(serverIp)) return false;
            return VerifiedDict.TryGetValue(serverIp, out var v) && v && IsConnected(serverIp);
        }
        private bool IsSocketConnected(TcpClient client)
        {
            try
            {
                // 1. 객체가 없거나 null이면 끊긴 것
                if (client == null) return false;

                // 2. 이미 Dispose된 객체를 client.Connected로 접근하면 예외가 발생함
                // 따라서 여기서 예외가 나면 "끊긴 것"으로 처리해야 함.
                if (!client.Connected) return false;

                // 3. 소켓 정밀 검사 (Poll)
                // 연결된 것처럼 보여도 실제 데이터 전송이 불가능한 상태인지 확인
                if (client.Client.Poll(0, SelectMode.SelectRead) && client.Client.Available == 0)
                    return false;

                return true;
            }
            catch (ObjectDisposedException)
            {
                // ★ 삭제된 개체 에러가 나면 -> "연결 끊김"으로 간주 (재연결 시도하게 유도)
                return false;
            }
            catch (Exception)
            {
                // 기타 에러도 끊김으로 간주
                return false;
            }
        }

        private async Task<bool> EnsureConnectedAsync(string serverIp, int port)                                        // 소켓 연결 확인후 재연결
        {
            if (!ClientDict.ContainsKey(serverIp) || !IsSocketConnected(ClientDict[serverIp]))
            {
                Log($"Connection lost. Reconnecting to {serverIp}:{port}...");
                if (ClientDict.ContainsKey(serverIp))
                {
                    try { StreamDict[serverIp]?.Close(); } catch { }
                    ClientDict.Remove(serverIp);
                    StreamDict.Remove(serverIp);
                }
                await ConnectAsync(serverIp, port);
            }
            return ClientDict.ContainsKey(serverIp) && IsSocketConnected(ClientDict[serverIp]);
        }

        public async Task<RoReadResult> ReadAsync(string ip, string idHex, string registerHex, string countHex)
        {
            if (string.IsNullOrWhiteSpace(ip))
                throw new ArgumentException("IP is empty");

            if (!StreamDict.TryGetValue(ip, out var stream) || stream == null)
                throw new Exception("Not connected");

            ushort id = ConvertHexToUShortWithException(idHex);
            ushort startReg = ConvertHexToUShortWithException(registerHex);
            ushort count = ConvertHexToUShortWithException(countHex);

            byte[] request = ReadRegisters(id, startReg, count);

            // ✅ 세마포어 추가
            var sem = GetIpLock(ip);
            await sem.WaitAsync();

            try
            {
                await stream.WriteAsync(request, 0, request.Length);

                byte[] response = new byte[512];

                // ★ 타임아웃 적용 (예: 1500ms ~ 3000ms 사이 권장)
                var readTask = stream.ReadAsync(response, 0, response.Length);
                var completed = await Task.WhenAny(readTask, Task.Delay(2000));

                if (completed != readTask)
                {
                    // ★ 1. 통신 강제 종료
                    Disconnect(ip);

                    // ★ 2. 로그(선택)
                    OnDataUpdate?.Invoke("Read", "Timeout", $"Modbus read timeout. Connection closed. IP={ip}", ip);

                    // ★ 3. 예외 전달
                    throw new Exception("Modbus read timeout (connection closed)");
                }

                int bytesRead = readTask.Result;

                if (bytesRead < 9) throw new Exception("Invalid Modbus response (too short)");

                byte functionCode = response[7];
                if (functionCode == 0x83)
                    throw new Exception("Modbus exception: 0x" + response[8].ToString("X2"));
                if (functionCode != 0x03)
                    throw new Exception("Unexpected function code: 0x" + functionCode.ToString("X2"));

                int byteCount = response[8];
                int dataStart = 9;
                if (byteCount <= 0) return new RoReadResult();

                if (bytesRead < dataStart + byteCount)
                    throw new Exception("Invalid Modbus response (byteCount mismatch)");
                if (byteCount % 2 != 0)
                    throw new Exception("Invalid Modbus payload length");

                // 1) raw data 복사
                byte[] data = new byte[byteCount];
                Buffer.BlockCopy(response, dataStart, data, 0, byteCount);

                // 2) 숫자 파싱(항상 수행)
                ushort[] values = ToUShortArrayBigEndian(data);

                // 3) 문자 후보 디코딩(High/Low 둘 다 시도해서 더 그럴듯한 쪽 선택)
                string textHigh = DecodeByteStreamCp949(TakeEvery2Bytes(data, takeHigh: true));
                string textLow = DecodeByteStreamCp949(TakeEvery2Bytes(data, takeHigh: false));

                // 4) 어떤 게 문자에 가까운지 점수 비교
                int scoreHigh = ScoreAsText(textHigh);
                int scoreLow = ScoreAsText(textLow);

                string bestText = scoreHigh >= scoreLow ? textHigh : textLow;
                int bestScore = Math.Max(scoreHigh, scoreLow);

                // 5) 최종 판정
                bool likelyText = bestScore >= 5; // 임계값(필요 시 조정)

                // 6) DisplayText 결정
                string display;
                if (likelyText)
                {
                    display = bestText;
                }
                else
                {
                    // 숫자값을 UI에 그냥 보여주기 좋은 형태로
                    // 예: "6" 또는 "6, 0, 12"
                    display = string.Join(", ", values.Select(v => v.ToString()));
                }

                return new RoReadResult
                {
                    DisplayText = display,
                    IsLikelyText = likelyText,
                    UShortValues = values
                };
            }
            finally
            {
                sem.Release(); // ✅ 반드시 해제
            }
        }
        private static ushort[] ToUShortArrayBigEndian(byte[] data)
        {
            int n = data.Length / 2;
            ushort[] r = new ushort[n];
            for (int i = 0; i < n; i++)
            {
                int p = i * 2;
                r[i] = (ushort)((data[p] << 8) | data[p + 1]);
            }
            return r;
        }

        private static byte[] TakeEvery2Bytes(byte[] data, bool takeHigh)
        {
            int n = data.Length / 2;
            byte[] r = new byte[n];
            int idx = 0;
            for (int i = 0; i + 1 < data.Length; i += 2)
                r[idx++] = takeHigh ? data[i] : data[i + 1];
            return r;
        }

        private static string DecodeByteStreamCp949(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) return "";

            int end = Array.IndexOf(bytes, (byte)0x00);
            if (end < 0) end = bytes.Length;
            if (end == 0) return "";

            try
            {
                return Encoding.GetEncoding(949).GetString(bytes, 0, end).Trim();
            }
            catch
            {
                return "";
            }
        }

        private static int ScoreAsText(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return 0;

            int score = 0;

            // 제어문자/깨짐 패널티
            if (s.IndexOf('\uFFFD') >= 0) score -= 10;
            foreach (char c in s)
            {
                if (c == '\r' || c == '\n' || c == '\t') { score += 1; continue; }
                if (char.IsControl(c)) score -= 2;
                else score += 1;
            }

            // 한글/영문/숫자 가산
            if (s.Any(ch => ch >= 0xAC00 && ch <= 0xD7A3)) score += 10; // 한글
            if (s.Any(ch => (ch >= 'A' && ch <= 'Z') || (ch >= 'a' && ch <= 'z'))) score += 5;
            if (s.Any(ch => ch >= '0' && ch <= '9')) score += 2;

            return score;
        }

        public async Task SingleWriteAsync(string ip, string idHex, string registerHex, string inputText)
        {
            if (string.IsNullOrWhiteSpace(ip))
                throw new ArgumentException("IP is empty");

            ushort id = ConvertHexToUShortWithException(idHex);
            ushort reg = ConvertHexToUShortWithException(registerHex);

            ushort value = 0;

            if (!string.IsNullOrWhiteSpace(inputText))
            {
                // 문자 영역
                if (reg >= 0x001D && reg <= 0x002C)
                {
                    if (inputText.Length == 1)
                    {
                        value = (ushort)inputText[0];
                    }
                    else
                    {
                        ushort parsed = 0;
                        foreach (char c in inputText)
                        {
                            parsed = (ushort)((parsed * 10) + (c - '0'));
                        }
                        value = parsed;
                    }
                }
                else
                {
                    ushort.TryParse(inputText, out value);
                }
            }

            byte[] request = WriteSingleRegisters(id, reg, value);

            await CommunicateAsync(ip, "SingleWrite", request);
        }
        public async Task MultiWriteAsync(string ip, string idHex, string registerHex, string inputText)
        {
            if (string.IsNullOrWhiteSpace(ip))
                throw new ArgumentException("IP is empty");

            ushort id = ConvertHexToUShortWithException(idHex);
            ushort reg = ConvertHexToUShortWithException(registerHex);

            ushort[] values;

            if (string.IsNullOrWhiteSpace(inputText))
            {
                values = new ushort[16];
                for (int i = 0; i < values.Length; i++)
                    values[i] = 0;
            }
            else
            {
                if (reg >= 0x001D && reg <= 0x002C)
                {
                    values = new ushort[inputText.Length];
                    for (int i = 0; i < inputText.Length; i++)
                        values[i] = (ushort)inputText[i];
                }
                else
                {
                    values = ConvertStringToUShortArray(inputText);
                }
            }

            byte[] request = WriteMultiRegisters(id, reg, values);

            await CommunicateAsync(ip, "MultiWrite", request);
        }

        public async Task CommunicateAsync(string serverIp, string function, byte[] request)
        {
            try
            {
                if (!ClientDict.ContainsKey(serverIp))
                {
                    Log($"Not connected to {serverIp}");
                    return;
                }

                int port = Ro_List.Find(x => x.Item1 == serverIp).Item2;

                if (!await EnsureConnectedAsync(serverIp, port))
                {
                    OnDataUpdate?.Invoke(function, "Receive", $"Failed to reconnect to {serverIp}.", serverIp);
                    return;
                }

                var stream = StreamDict[serverIp];

                var sem = GetIpLock(serverIp);
                await sem.WaitAsync();

                try
                {
                    await stream.WriteAsync(request, 0, request.Length);
                    OnDataUpdate?.Invoke(function, "Send", $"{BitConverter.ToString(request).Replace("-", " ")}", serverIp);

                    byte[] response = new byte[256];
                    var readTask = stream.ReadAsync(response, 0, response.Length);

                    if (await Task.WhenAny(readTask, Task.Delay(3000)) == readTask)
                    {
                        int bytesRead = readTask.Result;
                        if (bytesRead > 0)
                        {
                            string responseHex = BitConverter.ToString(response, 0, bytesRead).Replace("-", " ");
                            OnDataUpdate?.Invoke(function, "Receive", responseHex, serverIp);
                        }
                        else
                        {
                            OnDataUpdate?.Invoke(function, "Receive", "No response received.", serverIp);
                        }
                    }
                    else
                    {
                        OnDataUpdate?.Invoke(function, "Receive", "Timeout: No response received within 3 seconds.", serverIp);
                    }
                }
                finally
                {
                    sem.Release();
                }
            }
            catch (Exception ex)
            {
                OnDataUpdate?.Invoke(function, "Receive", $"Error during communication: {ex.Message}", serverIp);
                Log($"[Exception Detail] {ex}");
            }
        }
        // [RoControl.cs]

        public void Disconnect(string serverIp)
        {
            if (string.IsNullOrWhiteSpace(serverIp)) return;

            SemaphoreSlim sem = null;
            bool lockTaken = false;

            // 1. 락(세마포어) 가져오기
            IpLocks.TryGetValue(serverIp, out sem);

            try
            {
                // 2. 락 대기 시도 (기존 코드 유지)
                if (sem != null)
                {
                    lockTaken = sem.Wait(100); // 0.1초 대기
                    if (!lockTaken)
                    {
                        // 중요: 여기서 return을 하지 않고 아래로 내려가야 강제 종료가 됩니다.
                        Log($"Disconnect wait timeout (Forcing close): {serverIp}");
                    }
                }
            }
            catch { } // 락 에러 무시하고 계속 진행

            try
            {
                // 1) 스트림 정리 및 목록 제거
                if (StreamDict.TryGetValue(serverIp, out var s))
                {
                    try { s.Close(); s.Dispose(); } catch { }
                    StreamDict.Remove(serverIp); // ★ 목록에서 확실히 제거
                }

                // 2) 클라이언트(소켓) 정리 및 목록 제거
                if (ClientDict.TryGetValue(serverIp, out var c))
                {
                    try { c.Close(); c.Dispose(); } catch { }
                    ClientDict.Remove(serverIp); // ★ 목록에서 확실히 제거
                }

                Log($"Disconnected & Cleaned: {serverIp}");
            }
            catch (Exception ex)
            {
                Log($"Error during cleaning: {ex.Message}");
            }

            // 4. 마무리 (락 해제 및 이벤트 알림)
            try
            {
                if (lockTaken && sem != null)
                {
                    sem.Release();
                }
            }
            catch { }

            try
            {
                Disconnected?.Invoke(serverIp);
            }
            catch { }
        }

        public byte[] ReadRegisters(ushort ID, ushort registerAddress, ushort numberOfRegisters)
        {
            byte[] request = new byte[12];

            // Transaction Identifier (2 bytes)
            ushort transactionId = GetNextTransactionId();
            request[0] = (byte)(transactionId >> 8); // High byte
            request[1] = (byte)(transactionId & 0xFF); // Low byte

            // Protocol Identifier (2 bytes)
            request[2] = 0x00; // High byte
            request[3] = 0x00; // Low byte

            // Length (2 bytes)
            request[4] = 0x00; // High byte
            request[5] = 0x06; // Low byte (6 bytes follow: Unit ID, Function Code, and Data)

            // Furnace ID (1 byte)
            request[6] = (byte)ID;

            // Function Code (1 byte)
            request[7] = 0x03;                        // Request

            // Start Address (2 bytes)
            request[8] = (byte)(registerAddress >> 8);   // High byte
            request[9] = (byte)(registerAddress & 0xFF); // Low byte

            // Number of Registers (2 bytes)
            request[10] = (byte)(numberOfRegisters >> 8);   // High byte
            request[11] = (byte)(numberOfRegisters & 0xFF); // Low byte

            return request;
        }

        public byte[] WriteSingleRegisters(ushort ID, ushort registerAddress, ushort registerValues)
        {
            byte[] request = new byte[12];

            // Transaction Identifier (2 bytes)
            ushort transactionId = GetNextTransactionId();
            request[0] = (byte)(transactionId >> 8); // High byte
            request[1] = (byte)(transactionId & 0xFF); // Low byte

            // Protocol Identifier (2 bytes)
            request[2] = 0x00; // High byte
            request[3] = 0x00; // Low byte

            // Length (2 bytes)
            request[4] = 0x00; // High byte
            request[5] = 0x06; // Low byte (6 bytes follow: Unit ID, Function Code, and Data)

            // Furnace ID (1 byte)
            request[6] = (byte)ID;

            // Function Code (1 byte)
            request[7] = 0x06;                        // Request

            // Start Address (2 bytes)
            request[8] = (byte)(registerAddress >> 8);   // High byte
            request[9] = (byte)(registerAddress & 0xFF); // Low byte

            // Number of Registers (2 bytes)
            request[10] = (byte)(registerValues >> 8);   // High byte
            request[11] = (byte)(registerValues & 0xFF); // Low byte

            return request;
        }

        public byte[] WriteMultiRegisters(ushort ID, ushort startAddress, params ushort[] registerValues)
        {
            // 요청 크기 계산 (고정된 헤더 크기 + 레지스터 데이터 크기)
            int headerSize = 13; // 고정 헤더 크기 (Transaction, Protocol, Length, Unit ID, Function Code, Address, Register Count)
            int totalSize = headerSize + registerValues.Length * 2;
            byte[] request = new byte[totalSize];

            // Transaction Identifier (2 bytes)
            ushort transactionId = GetNextTransactionId();
            request[0] = (byte)(transactionId >> 8); // High byte
            request[1] = (byte)(transactionId & 0xFF); // Low byte

            // Protocol Identifier (2 bytes)
            request[2] = 0x00; // High byte
            request[3] = 0x00; // Low byte

            // Length (2 bytes) → 전체 요청 크기 설정
            request[4] = (byte)(totalSize >> 8); // High byte
            request[5] = (byte)(totalSize & 0xFF); // Low byte (배열 크기와 동일)

            // Furnace ID (1 byte)
            request[6] = (byte)ID;

            // Function Code (1 byte)
            request[7] = 0x10; // Request

            // Start Address (2 bytes)
            request[8] = (byte)(startAddress >> 8); // High byte
            request[9] = (byte)(startAddress & 0xFF); // Low byte

            // Number of Registers (2 bytes)
            request[10] = (byte)(registerValues.Length >> 8); // High byte
            request[11] = (byte)(registerValues.Length & 0xFF); // Low byte

            // Byte count (registerValues.Length * 2)
            request[12] = (byte)(registerValues.Length * 2);

            // Register Values (Data)
            for (int i = 0; i < registerValues.Length; i++)
            {
                request[13 + (i * 2)] = (byte)(registerValues[i] >> 8);   // High byte
                request[13 + (i * 2) + 1] = (byte)(registerValues[i] & 0xFF); // Low byte
            }

            return request;
        }

        // 문자열을 ushort 배열로 변환하는 메서드
        public static ushort[] ConvertStringToUShortArray(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new FormatException("Input cannot be null or whitespace.");
            }

            // 공백을 기준으로 문자열을 나누고, 각 요소를 ushort로 변환
            string[] parts = input.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            ushort[] result = new ushort[parts.Length];

            for (int i = 0; i < parts.Length; i++)
            {
                if (ushort.TryParse(parts[i], out ushort value))
                {
                    result[i] = value; // 숫자는 그대로 변환
                }
                else if (parts[i].Length == 1) // 단일 문자일 경우
                {
                    result[i] = (ushort)parts[i][0]; // 문자 → ASCII 코드 변환
                }
                else
                {
                    throw new FormatException($"Invalid number or character format: {parts[i]}");
                }
            }

            return result;
        }
        public static ushort ConvertHexToUShortWithException(string input)
        {
            try
            {
                return Convert.ToUInt16(input, 16); // 16진수 문자열을 ushort로 변환
            }
            catch (FormatException)
            {
                Console.WriteLine("텍스트가 올바른 16진수 형식이 아닙니다.");
                throw; // 변환할 수 없는 형식
            }
            catch (OverflowException)
            {
                Console.WriteLine("텍스트 숫자가 ushort 범위를 초과합니다.");
                throw; // 변환된 숫자가 ushort 범위를 초과하는 경우
            }
        }
        private void Log(string msg)
        {
            try
            {
                // 시간 포함해서 콘솔 출력
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {msg}");
            }
            catch
            {
                // 콘솔 로그는 절대 죽으면 안 됨
            }
        }
    }
}
