using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;

namespace SubControl.Facility
{
    public class FacilityMapControl : Panel
    {
        private static readonly string DefaultLayoutDir = @"C:\Farm_Setting\Layout";
        private static readonly string DefaultLayoutFile = "facility_layout.json";


        public Image FloorImage { get; private set; }
        private string FloorImagePath;

        public bool ShowGrid { get; set; } = true;
        public int GridSize { get; set; } = 40;
        public bool ShowLayoutUx { get; set; } = true;
        private Dictionary<DeviceModel, Rectangle> DragOccCache;
        private int DragInfoH;
        private bool DragIsOverlapping;

        public string AutoSavePath { get; set; }

        // ── Info 패널 레이아웃 상수 ───────────────────────────
        private const int InfoPadX = 8;     // 좌우 안쪽 여백
        private const int InfoPadY = 6;     // 상하 안쪽 여백
        private const int LineGap = 1;       // 줄 간격
        private const int LedDiameter = 10;  // LED 지름
        private const int PanelMargin = 6;   // 장비 사각형과 패널 사이 바깥 여백
        private const int ImagePanelGap = 10;// 이미지와 패널 사이 간격
        private const int LedTextGap = 14;

        // ★ 모든 장비 공통 '고정' 패널 너비
        private const int InfoFixedW = 200;  // 필요시 180~240으로 조정

        // 무효화 여유(패널이 장비 영역을 넘을 수 있으므로)
        private const int InfoInflateX = 120; // 패널 좌우 여유(Invalidate용)
        private const int InfoInflateY = 90;  // 패널 아래 여유(Invalidate용)

        // 장비 이동 시 무효화 여유(LED 글로우 포함)
        private const int DragInflate = 24;

        // 장비(컨트롤 없이 직접 그리기)
        private readonly List<DeviceModel> Devices = new List<DeviceModel>();

        // 드래그 상태
        private DeviceModel DragSel;
        private Point DragOffset;
        private Point LastDragPos;

        // 배경 캐시
        private Bitmap BgCache;
        private bool BgDirty;

        // 우클릭 위치
        private Point LastContextPoint;

        // 활성 메뉴(Dispose X)
        private ContextMenuStrip ActiveMenu;

        // 폰트(고정)
        private static readonly Font NameFont = new Font("Y이드스트릿체 L", 8.5f, FontStyle.Bold);
        private static readonly Font MetaFont = new Font("Y이드스트릿체 L", 9.0f, FontStyle.Regular);

        // 캐시
        private readonly Dictionary<DeviceModel, Bitmap> DeviceCache = new Dictionary<DeviceModel, Bitmap>();
        private bool InvalidateDeviceCache = false;
        private static readonly FlatMenuRenderer s_menuRenderer = new FlatMenuRenderer();
        public event Action<DeviceModel> ConnectRequested;
        public event Action<DeviceModel> DeviceClicked;

        public FacilityMapControl()
        {
            DoubleBuffered = true;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
            BackColor = Color.FromArgb(230, 230, 230);

            MouseClick += OnMouseClickLocal;
            MouseDown += OnMouseDownLocal;
            MouseMove += OnMouseMoveLocal;
            MouseUp += OnMouseUpLocal;

            try { Directory.CreateDirectory(DefaultLayoutDir); } catch { }
            AutoSavePath = Path.Combine(DefaultLayoutDir, DefaultLayoutFile);
            if (File.Exists(AutoSavePath)) { LoadLayout(AutoSavePath); }
        }

        // ===== DTO =====
        public class LayoutDto
        {
            public string FloorImagePath { get; set; }
            public List<DeviceModel> Devices { get; set; }
        }
        private Point SnapToGrid(Point p)
        {
            int g = GridSize;
            if (g <= 1) return p;

            int x = (int)Math.Round(p.X / (double)g) * g;
            int y = (int)Math.Round(p.Y / (double)g) * g;

            return new Point(x, y);
        }
        // ===== 장비 추가(외부 호환) =====
        public DeviceModel AddDevice(DeviceModel model)
        {
            if (model == null) return null;

            if (model.Size.Width <= 0 || model.Size.Height <= 0)
                model.Size = CalcDefaultSize(model.Type);

            var loc = model.Location;
            if (loc == Point.Empty) loc = new Point(20, 20);
            loc = new Point(
                Math.Max(0, Math.Min(Width - model.Size.Width, loc.X)),
                Math.Max(0, Math.Min(Height - model.Size.Height, loc.Y))
            );
            model.Location = loc;

            Devices.Add(model);

            InvalidateDeviceCache = true;
            QueueAutoSave();
            Invalidate(GetInvalidateWithInfo(new Rectangle(model.Location, model.Size)));
            return model;
        }

        // ===== 저장/불러오기 =====
        public void SaveLayout(string filePath)
        {
            try
            {
                var dto = new LayoutDto { FloorImagePath = FloorImagePath, Devices = Devices };
                var json = JsonSerializer.Serialize(dto, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, json, Encoding.UTF8);
            }
            catch { }
        }

        public void LoadLayout(string filePath)
        {
            try
            {
                if (!File.Exists(filePath)) return;
                var json = File.ReadAllText(filePath, Encoding.UTF8);
                var dto = JsonSerializer.Deserialize<LayoutDto>(json);

                Devices.Clear();

                if (dto != null && dto.Devices != null) Devices.AddRange(dto.Devices);
                if (dto != null && !string.IsNullOrEmpty(dto.FloorImagePath) && File.Exists(dto.FloorImagePath))
                    SetFloorImage(dto.FloorImagePath);

                InvalidateDeviceCache = true;
                Invalidate();
            }
            catch { }
        }

        private void QueueAutoSave()
        {
            try
            {
                if (string.IsNullOrEmpty(AutoSavePath)) return;
                var dto = new LayoutDto { FloorImagePath = FloorImagePath, Devices = Devices };
                var json = JsonSerializer.Serialize(dto, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(AutoSavePath, json, Encoding.UTF8);
            }
            catch { }
        }

        public void SetFloorImage(string path)
        {
            if (!File.Exists(path)) return;
            FloorImage?.Dispose();
            FloorImage = Image.FromFile(path);
            FloorImagePath = path;
            BgDirty = true;
            Invalidate();
            QueueAutoSave();
        }

        // ===== 이름 자동/기본 크기 =====
        private string NextName(DeviceType t)
        {
            string baseName =
                (t == DeviceType.Printer) ? "프린터" :
                (t == DeviceType.Cleaner) ? "세척기" :
                (t == DeviceType.Furnace) ? "소결로" :
                (t == DeviceType.Polisher) ? "연마기" : "장비";

            int n = 0; foreach (var d in Devices) if (d.Type == t) n++;
            return baseName + " #" + (n + 1);
        }

        private Size CalcDefaultSize(DeviceType type)
        {
            const double PX_PER_MM = 0.30;
            Size SizeFromMmXZ(int xMm, int zMm)
            {
                int w = (int)Math.Round(xMm * PX_PER_MM);
                int h = (int)Math.Round(zMm * PX_PER_MM);
                if (w < 48) w = 48;
                if (h < 56) h = 56;
                return new Size(w, h);
            }

            switch (type)
            {
                case DeviceType.Printer: { var s = SizeFromMmXZ(515, 1655); return new Size((int)(s.Width * 0.70), (int)(s.Height * 0.70)); }
                case DeviceType.Cleaner: { var s = SizeFromMmXZ(660, 1210); return new Size((int)(s.Width * 0.65), (int)(s.Height * 0.65)); }
                case DeviceType.Furnace: { var s = SizeFromMmXZ(324, 527); return new Size((int)(s.Width * 1.35), (int)(s.Height * 1.35)); }
                case DeviceType.Polisher: { var s = SizeFromMmXZ(1514, 1654); return new Size((int)(s.Width * 0.50), (int)(s.Height * 0.48)); }
            }
            return new Size(80, 80);
        }

        // === (추가) 주어진 텍스트로 정보패널 높이 정확히 계산 ===
        private int MeasureInfoHeight(Graphics g, string name, string ip, string status, int textW)
        {
            int nameH = MeasureLineHeight(g, NameFont);
            if (string.IsNullOrEmpty(ip)) ip = "-";
            if (string.IsNullOrEmpty(status)) status = "Idle";
            int ipH = MeasureWrappedHeight(g, ip, MetaFont, textW);
            int stateH = MeasureWrappedHeight(g, status, MetaFont, textW);
            return Math.Max(LedDiameter, nameH + LineGap + ipH + LineGap + stateH) + (InfoPadY * 2);
        }

        // === (추가) 해당 장비 사각형에서 실제 그려질 infoRect 계산 ===
        private Rectangle GetInfoRect(Rectangle deviceRect, int infoH)
        {
            return new Rectangle(
                deviceRect.Left + (deviceRect.Width - InfoFixedW) / 2,
                deviceRect.Bottom - infoH - PanelMargin,
                InfoFixedW,
                infoH
            );
        }

        // === (추가) 점유영역(장비 ∪ 정보패널) ===
        private Rectangle GetOccupiedRect(DeviceModel d, Graphics g)
        {
            var rect = new Rectangle(d.Location, d.Size);
            int textW = Math.Max(1, InfoFixedW - (InfoPadX * 2) - LedDiameter - 8);
            int infoH = MeasureInfoHeight(g, d.Name ?? "", d.ServerIp ?? "-", string.IsNullOrEmpty(d.Status) ? "Idle" : d.Status, textW);
            var infoRect = GetInfoRect(rect, infoH);
            return Rectangle.Union(rect, infoRect);
        }

        // === (추가) 후보 위치용 점유영역(추가하려는 이름/IP/상태 기준) ===
        private Rectangle GetOccupiedRect(Rectangle deviceRect, string name, string ip, string status, Graphics g)
        {
            int textW = Math.Max(1, InfoFixedW - (InfoPadX * 2) - LedDiameter - 8);
            int infoH = MeasureInfoHeight(g, name ?? "", string.IsNullOrEmpty(ip) ? "-" : ip, string.IsNullOrEmpty(status) ? "Idle" : status, textW);
            var infoRect = GetInfoRect(deviceRect, infoH);
            return Rectangle.Union(deviceRect, infoRect);
        }

        // === (추가) 겹치지 않는 위치 찾기(정보패널까지 고려) ===
        private Point FindNonOverlappingLocation(Size sz, Point preferredTopLeft, string name, string ip, string status, int margin = 6)
        {
            int ClampX(int x) => Math.Max(0, Math.Min(Width - sz.Width, x));
            int ClampY(int y) => Math.Max(0, Math.Min(Height - sz.Height, y));

            using (var g = CreateGraphics())
            {
                bool IntersectsAny(Rectangle r)
                {
                    // 후보 점유영역
                    var candOcc = GetOccupiedRect(r, name, ip, status, g);
                    candOcc.Inflate(margin, margin);

                    for (int i = 0; i < Devices.Count; i++)
                    {
                        var occ = GetOccupiedRect(Devices[i], g);
                        if (candOcc.IntersectsWith(occ)) return true;
                    }
                    return false;
                }

                var start = new Point(ClampX(preferredTopLeft.X), ClampY(preferredTopLeft.Y));
                var testRect = new Rectangle(start, sz);
                if (!IntersectsAny(testRect))
                    return start;

                const int step = 8;                   // 촘촘히 탐색 → 덜 멀어지게
                int maxRadius = Math.Max(Width, Height);

                for (int r = step; r <= maxRadius; r += step)
                {
                    for (int dx = -r; dx <= r; dx += step)
                    {
                        var p1 = new Point(ClampX(preferredTopLeft.X + dx), ClampY(preferredTopLeft.Y - r));
                        if (!IntersectsAny(new Rectangle(p1, sz))) return p1;

                        var p2 = new Point(ClampX(preferredTopLeft.X + dx), ClampY(preferredTopLeft.Y + r));
                        if (!IntersectsAny(new Rectangle(p2, sz))) return p2;
                    }
                    for (int dy = -r + step; dy <= r - step; dy += step)
                    {
                        var p3 = new Point(ClampX(preferredTopLeft.X - r), ClampY(preferredTopLeft.Y + dy));
                        if (!IntersectsAny(new Rectangle(p3, sz))) return p3;

                        var p4 = new Point(ClampX(preferredTopLeft.X + r), ClampY(preferredTopLeft.Y + dy));
                        if (!IntersectsAny(new Rectangle(p4, sz))) return p4;
                    }
                }
            }

            return new Point(0, 0);
        }

        // ★ AddAt: 겹침 체크만 새 오버로드 사용 (다른 기능 그대로)
        private void AddAt(DeviceType type, Point pt)
        {
            var sz = CalcDefaultSize(type);

            // 우클릭 점을 기준(중앙 배치)으로 선호 좌상단 계산
            var preferredTopLeft = new Point(
                pt.X - sz.Width / 2,
                pt.Y - sz.Height / 2
            );

            string name = NextName(type);
            string ip = "-";
            string status = "Idle";

            var loc = FindNonOverlappingLocation(sz, preferredTopLeft, name, ip, status, margin: 6);

            var d = new DeviceModel { Name = name, Type = type, Location = loc, Size = sz, Status = status };
            Devices.Add(d);

            InvalidateDeviceCache = true;
            QueueAutoSave();
            Invalidate(GetInvalidateWithInfo(new Rectangle(loc, sz)));
        }

        public void UpdateDeviceStatusByIp(string ip, bool connected, string status)
        {
            if (string.IsNullOrWhiteSpace(ip)) return;

            // ✅ IpToId 제거: Devices 전체에서 IP 매칭되는 모든 장비 업데이트
            for (int i = 0; i < Devices.Count; i++)
            {
                var d = Devices[i];
                if (d == null) continue;

                var dip = (d.ServerIp ?? "").Trim();
                if (dip.Length == 0) continue;

                if (string.Equals(dip, ip.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    d.Connected = connected;
                    if (!string.IsNullOrWhiteSpace(status)) d.Status = status;

                    if (DeviceCache.ContainsKey(d))
                    {
                        DeviceCache[d].Dispose();
                        DeviceCache.Remove(d);
                    }
                    Invalidate(GetInvalidateWithInfo(new Rectangle(d.Location, d.Size)));
                }
            }
        }

        private void BuildDragCacheForRealtimeCollision(DeviceModel moving)
        {
            DragOccCache = new Dictionary<DeviceModel, Rectangle>(Devices.Count);

            using (var g = CreateGraphics())
            {
                for (int i = 0; i < Devices.Count; i++)
                {
                    var d = Devices[i];
                    if (d == null) continue;
                    if (ReferenceEquals(d, moving)) continue;

                    DragOccCache[d] = GetOccupiedRect(d, g); // 장비+패널 점유영역
                }

                int textW = Math.Max(1, InfoFixedW - (InfoPadX * 2) - LedDiameter - 8);
                DragInfoH = MeasureInfoHeight(
                    g,
                    moving.Name ?? "",
                    moving.ServerIp ?? "-",
                    string.IsNullOrEmpty(moving.Status) ? "Idle" : moving.Status,
                    textW
                );
            }
        }
        private Rectangle GetOccupiedRectFast(DeviceModel moving, Point newTopLeft)
        {
            var deviceRect = new Rectangle(newTopLeft, moving.Size);
            var infoRect = GetInfoRect(deviceRect, DragInfoH);
            return Rectangle.Union(deviceRect, infoRect);
        }

        private bool WouldOverlapRealtime(DeviceModel moving, Point newTopLeft, int margin = 6)
        {
            if (DragOccCache == null) return false;

            var occ = GetOccupiedRectFast(moving, newTopLeft);
            occ.Inflate(margin, margin);

            foreach (var kv in DragOccCache)
            {
                if (occ.IntersectsWith(kv.Value))
                    return true;
            }
            return false;
        }
        private Point FindNearestLocation(DeviceModel moving, Point preferredTopLeft, int margin = 6)
        {
            if (moving == null) return preferredTopLeft;

            // 그리드 스냅 고려
            Point ApplySnap(Point p)
            {
                if (ShowGrid && GridSize > 1) p = SnapToGrid(p);
                int x = Math.Max(0, Math.Min(Width - moving.Size.Width, p.X));
                int y = Math.Max(0, Math.Min(Height - moving.Size.Height, p.Y));
                return new Point(x, y);
            }

            bool IntersectsAny(Point topLeft)
            {
                // DragOccCache는 "다른 장비들 점유영역"만 들어있음
                if (DragOccCache == null) return false;

                var occ = GetOccupiedRectFast(moving, topLeft);
                occ.Inflate(margin, margin);

                foreach (var kv in DragOccCache)
                {
                    if (occ.IntersectsWith(kv.Value))
                        return true;
                }
                return false;
            }

            // 1) 현재 위치를 먼저 검사
            preferredTopLeft = ApplySnap(preferredTopLeft);
            if (!IntersectsAny(preferredTopLeft))
                return preferredTopLeft;

            // 2) 주변을 원형(사각 링)으로 탐색: 가까운 곳부터
            // step은 GridSize가 켜져 있으면 GridSize, 아니면 8px 정도
            int step = (ShowGrid && GridSize > 1) ? GridSize : 8;
            int maxRadius = Math.Max(Width, Height);

            for (int r = step; r <= maxRadius; r += step)
            {
                // 상/하 라인
                for (int dx = -r; dx <= r; dx += step)
                {
                    var p1 = ApplySnap(new Point(preferredTopLeft.X + dx, preferredTopLeft.Y - r));
                    if (!IntersectsAny(p1)) return p1;

                    var p2 = ApplySnap(new Point(preferredTopLeft.X + dx, preferredTopLeft.Y + r));
                    if (!IntersectsAny(p2)) return p2;
                }

                // 좌/우 라인
                for (int dy = -r + step; dy <= r - step; dy += step)
                {
                    var p3 = ApplySnap(new Point(preferredTopLeft.X - r, preferredTopLeft.Y + dy));
                    if (!IntersectsAny(p3)) return p3;

                    var p4 = ApplySnap(new Point(preferredTopLeft.X + r, preferredTopLeft.Y + dy));
                    if (!IntersectsAny(p4)) return p4;
                }
            }

            // 못 찾으면 원위치(최후 수단)
            return preferredTopLeft;
        }
        private void OnMouseClickLocal(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            // ✅ 편집모드(ShowLayoutUx=true)에서는 기존 드래그/편집 흐름 유지
            // 운영모드(ShowLayoutUx=false)에서만 "클릭 선택"을 사용
            if (ShowLayoutUx) return;

            var hit = HitTest(e.Location);
            if (hit == null) return;

            DeviceClicked?.Invoke(hit);
        }

        // ===== 마우스/메뉴 =====
        private void OnMouseDownLocal(object s, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                LastContextPoint = e.Location;
                return;
            }
            if (e.Button != MouseButtons.Left)
                return;

            if (!ShowLayoutUx)
                return;

            for (int i = Devices.Count - 1; i >= 0; i--)
            {
                var d = Devices[i];
                var r = new Rectangle(d.Location, d.Size);
                if (r.Contains(e.Location))
                {
                    DragSel = d;
                    BuildDragCacheForRealtimeCollision(d);
                    DragIsOverlapping = false;
                    DragOffset = new Point(e.X - d.Location.X, e.Y - d.Location.Y);
                    LastDragPos = d.Location;
                    return;
                }
            }
        }

        private void OnMouseMoveLocal(object s, MouseEventArgs e)
        {
            if (DragSel == null) return;

            int nx = Math.Max(0, Math.Min(Width - DragSel.Size.Width, e.X - DragOffset.X));
            int ny = Math.Max(0, Math.Min(Height - DragSel.Size.Height, e.Y - DragOffset.Y));
            var newPos = new Point(nx, ny);

            if (ShowGrid && GridSize > 1)
            {
                newPos = SnapToGrid(newPos);

                int sx = Math.Max(0, Math.Min(Width - DragSel.Size.Width, newPos.X));
                int sy = Math.Max(0, Math.Min(Height - DragSel.Size.Height, newPos.Y));
                newPos = new Point(sx, sy);
            }

            // ✅ 그리드 ON/OFF 관계없이 항상 충돌 체크
            bool overlapNow = WouldOverlapRealtime(DragSel, newPos, margin: 0);
            if (overlapNow != DragIsOverlapping)
            {
                DragIsOverlapping = overlapNow;
                Invalidate(GetInvalidateWithInfo(GetOccupiedRectFast(DragSel, DragSel.Location)));
            }

            if (newPos != DragSel.Location)
            {
                var oldOcc = GetOccupiedRectFast(DragSel, LastDragPos); // rect+infoRect union
                var newOcc = GetOccupiedRectFast(DragSel, newPos);
                var inv = Rectangle.Union(GetInvalidateWithInfo(oldOcc), GetInvalidateWithInfo(newOcc));

                DragSel.Location = newPos;
                LastDragPos = newPos;
                Invalidate(inv);
            }
        }

        private void OnMouseUpLocal(object s, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (ActiveMenu != null)
                {
                    try { ActiveMenu.Close(ToolStripDropDownCloseReason.AppClicked); } catch { }
                    ActiveMenu = null;
                }

                var hit = HitTest(e.Location);

                ContextMenuStrip menu;
                if (hit != null)
                {
                    menu = BuildDeviceMenu(hit); // 장비 우클릭 메뉴(통신 기능 포함)
                }
                else
                {
                    // ✅ 운영 화면에서는 '장비 추가' UI 자체를 표시하지 않음
                    if (!ShowLayoutUx) return;
                    menu = BuildBackgroundMenu(); // 빈 공간 우클릭: 장비 추가 메뉴
                }

                ActiveMenu = menu;
                ActiveMenu.Closed += (sender2, args2) => { ActiveMenu = null; };

                BeginInvoke(new Action(() =>
                {
                    if (!IsDisposed && ActiveMenu != null)
                        ActiveMenu.Show(this, e.Location);
                }));
                return;
            }

            if (DragSel != null)
            {
                // ✅ 겹침 상태면 현재 위치 근처로 자동 배치
                if (DragIsOverlapping)
                {
                    var oldPos = DragSel.Location;

                    // 드래그 중 last 위치 기준(현재 위치)에서 탐색
                    var newPos = FindNearestLocation(DragSel, DragSel.Location, margin: 0);

                    // 위치가 바뀌면 반영 + 화면 무효화 범위 크게
                    if (newPos != oldPos)
                    {
                        var oldOcc = GetOccupiedRectFast(DragSel, oldPos);
                        var newOcc = GetOccupiedRectFast(DragSel, newPos);
                        var inv = Rectangle.Union(GetInvalidateWithInfo(oldOcc), GetInvalidateWithInfo(newOcc));

                        DragSel.Location = newPos;
                        LastDragPos = newPos; // 일관성 유지
                        Invalidate(inv);
                    }
                    else
                    {
                        // 위치가 그대로면 최소한 현재 영역 갱신
                        Invalidate(GetInvalidateWithInfo(GetOccupiedRectFast(DragSel, DragSel.Location)));
                    }
                }

                QueueAutoSave();

                // 드래그 종료 후 최종 화면 정리
                Invalidate(GetInvalidateWithInfo(GetOccupiedRectFast(DragSel, DragSel.Location)));

                DragSel = null;
            }

            DragOccCache = null;
            DragInfoH = 0;
            DragIsOverlapping = false;
        }

        private Rectangle GetInvalidateWithInfo(Rectangle deviceRect)
        {
            var r = deviceRect;
            r.Inflate(Math.Max(DragInflate, InfoInflateX), Math.Max(DragInflate, InfoInflateY));
            return r;
        }

        private DeviceModel HitTest(Point p)
        {
            for (int i = Devices.Count - 1; i >= 0; i--)
            {
                var d = Devices[i];
                if (new Rectangle(d.Location, d.Size).Contains(p)) return d;
            }
            return null;
        }

        private ContextMenuStrip BuildBackgroundMenu()
        {
            var cms = new ContextMenuStrip
            {
                Renderer = s_menuRenderer,
                ShowImageMargin = false,
                Font = new Font("Y이드스트릿체 L", 10f, FontStyle.Regular),
                ForeColor = Color.WhiteSmoke,
                BackColor = s_menuRenderer.BackColor
            };

            cms.Items.Add("프린터 추가", null, (s, e) => AddAt(DeviceType.Printer, LastContextPoint));
            cms.Items.Add("세척기 추가", null, (s, e) => AddAt(DeviceType.Cleaner, LastContextPoint));
            cms.Items.Add("소결로 추가", null, (s, e) => AddAt(DeviceType.Furnace, LastContextPoint));
            cms.Items.Add("연마기 추가", null, (s, e) => AddAt(DeviceType.Polisher, LastContextPoint));
            cms.Items.Add(new ToolStripSeparator());
            cms.Items.Add("모두 삭제", null, (s, e) =>
            {
                Devices.Clear();
                ClearDeviceCache();
                QueueAutoSave();
                Invalidate();
            });
            return cms;
        }

        private ContextMenuStrip BuildDeviceMenu(DeviceModel m)
        {
            var cms = new ContextMenuStrip
            {
                Renderer = s_menuRenderer,
                ShowImageMargin = false,
                Font = new Font("Y이드스트릿체 L", 10f, FontStyle.Regular),
                ForeColor = Color.WhiteSmoke,
                BackColor = s_menuRenderer.BackColor
            };

            if (!ShowLayoutUx && m.Type != DeviceType.Cleaner)
            {
                cms.Items.Add("연결하기", null, (s, e) =>
                {
                    // ✅ 이미 연결된 경우 차단
                    if (m.Connected)
                    {
                        MessageBox.Show(
                            "이미 연결된 장비입니다.",
                            "연결됨",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                        return;
                    }

                    // 1) IP 체크
                    if (string.IsNullOrWhiteSpace(m.ServerIp))
                    {
                        MessageBox.Show(
                            "장비 IP가 설정되어 있지 않습니다.\n우클릭 > IP 설정에서 IP를 먼저 입력하세요.",
                            "IP 미설정",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                        return;
                    }

                    // 2) 연결 요청 이벤트 발생
                    ConnectRequested?.Invoke(m);
                });
            }

            if (ShowLayoutUx)
            {
                cms.Items.Add("이름 변경", null, (s, e) =>
                {
                    using (var input = new InputBox("이름 입력", m.Name))
                    {
                        var owner = FindForm();
                        var dr = (owner != null) ? input.ShowDialog(owner) : input.ShowDialog();
                        if (dr == DialogResult.OK)
                        {
                            m.Name = input.Value;
                            if (DeviceCache.ContainsKey(m)) { DeviceCache[m].Dispose(); DeviceCache.Remove(m); }
                            QueueAutoSave();
                            Invalidate(GetInvalidateWithInfo(new Rectangle(m.Location, m.Size)));
                        }
                    }
                });
            }

            if (ShowLayoutUx && m.Type != DeviceType.Cleaner)
            {
                cms.Items.Add("IP 설정", null, (s, e) =>
                {
                    using (var input = new InputBox("Server IP 입력", m.ServerIp ?? ""))
                    {
                        var owner = FindForm();
                        var dr = (owner != null) ? input.ShowDialog(owner) : input.ShowDialog();
                        if (dr == DialogResult.OK)
                        {
                            string newIp = (input.Value ?? "").Trim();

                            // ✅ 공백 포함 모든 잘못된 입력 차단
                            if (!IsValidIPv4(newIp))
                            {
                                MessageBox.Show(
                                    "IP 형식이 올바르지 않습니다.\n\n예) 192.168.0.10",
                                    "IP 입력 오류",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                                return; // ❗ 저장 중단
                            }

                            if (string.Equals((m.ServerIp ?? "").Trim(), newIp, StringComparison.OrdinalIgnoreCase))
                            {
                                MessageBox.Show(
                                    "현재 설정된 IP와 동일합니다.\n변경할 IP를 입력하세요.",
                                    "변경 없음",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                                return;
                            }

                            if (DuplicateIpCheck(newIp, m))
                            {
                                MessageBox.Show(
                                    $"이미 다른 장비에 설정된 IP입니다.\n\nIP: {newIp}\n\n다른 IP를 입력하세요.",
                                    "IP 중복",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                                return;
                            }

                            // 신규 IP 저장
                            m.ServerIp = newIp;

                            if (DeviceCache.ContainsKey(m))
                            {
                                DeviceCache[m].Dispose();
                                DeviceCache.Remove(m);
                            }

                            QueueAutoSave();
                            Invalidate(GetInvalidateWithInfo(new Rectangle(m.Location, m.Size)));
                        }
                    }
                });
            }

            if (ShowLayoutUx)
            {
                cms.Items.Add(new ToolStripSeparator());
                cms.Items.Add("삭제", null, (s, e) =>
                {
                    var r = GetInvalidateWithInfo(new Rectangle(m.Location, m.Size));
                    Devices.Remove(m);
                    if (DeviceCache.ContainsKey(m)) { DeviceCache[m].Dispose(); DeviceCache.Remove(m); }
                    QueueAutoSave();
                    Invalidate(r);
                });
            }

            return cms;
        }
        private bool IsValidIPv4(string ip)
        {
            if (string.IsNullOrWhiteSpace(ip)) return false;

            // 공백 제거
            ip = ip.Trim();

            // IPv4 파싱 + IPv4인지 확인
            if (!IPAddress.TryParse(ip, out var addr)) return false;
            if (addr.AddressFamily != AddressFamily.InterNetwork) return false;

            // "1.2.3.4 " 같은 입력이 TryParse에 통과하는 경우가 있어 ToString 비교로 형태 보정
            // (선행 0 허용 여부 등은 정책에 따라 다르지만, 여기서는 일반 형태 강제)
            return addr.ToString() == ip;
        }
        private bool DuplicateIpCheck(string ip, DeviceModel self)
        {
            if (string.IsNullOrWhiteSpace(ip)) return false;
            ip = ip.Trim();

            for (int i = 0; i < Devices.Count; i++)
            {
                var d = Devices[i];
                if (d == null) continue;

                // 자기 자신은 제외 (기존 IP 그대로 재입력은 허용)
                if (ReferenceEquals(d, self)) continue;

                var dip = (d.ServerIp ?? "").Trim();
                if (dip.Length == 0) continue;

                if (string.Equals(dip, ip, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        // ===== 배경 캐시 =====
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            BgDirty = true;
            ClearDeviceCache();
            Invalidate();
        }

        private void RebuildBackgroundCache()
        {
            if (ClientSize.Width <= 0 || ClientSize.Height <= 0) return;

            BgCache?.Dispose();
            BgCache = new Bitmap(ClientSize.Width, ClientSize.Height);

            using (var g = Graphics.FromImage(BgCache))
            {
                g.Clear(BackColor);
                if (FloorImage != null)
                {
                    g.InterpolationMode = InterpolationMode.Bilinear;
                    g.DrawImage(FloorImage, ClientRectangle);
                }

                if (ShowGrid)
                {
                    using (var p = new Pen(Color.FromArgb(30, Color.Black)))
                    {
                        for (int x = 0; x < Width; x += GridSize) g.DrawLine(p, x, 0, x, Height);
                        for (int y = 0; y < Height; y += GridSize) g.DrawLine(p, 0, y, Width, y);
                    }
                }
            }

            BgDirty = false;
        }

        private void ClearDeviceCache()
        {
            foreach (var kvp in DeviceCache) kvp.Value.Dispose();
            DeviceCache.Clear();
        }

        // ===== 페인트 =====
        protected override void OnPaint(PaintEventArgs e)
        {
            if (BgCache == null || BgDirty || BgCache.Size != ClientSize) RebuildBackgroundCache();
            if (BgCache != null) e.Graphics.DrawImageUnscaled(BgCache, 0, 0);

            if (InvalidateDeviceCache)
            {
                ClearDeviceCache();
                InvalidateDeviceCache = false;
            }

            e.Graphics.CompositingMode = CompositingMode.SourceOver;
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.None;
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            var clipRect = e.ClipRectangle;
            bool isDragging = (DragSel != null);

            for (int i = 0; i < Devices.Count; i++)
            {
                var d = Devices[i];
                var rect = new Rectangle(d.Location, d.Size);
                if (!clipRect.IntersectsWith(GetInvalidateWithInfo(rect))) continue;

                DrawDevice(e.Graphics, d, isDragging && d == DragSel);
            }
        }

        // === 패널은 항상 고정폭, 장비 캐시는 "이미지"만 ===
        private void DrawDevice(Graphics g, DeviceModel m, bool isDraggingSel)
        {
            var rect = new Rectangle(m.Location, m.Size);

            // ── (1) 먼저 패널 텍스트 레이아웃을 계산 → infoH 산출
            int textW = Math.Max(1, InfoFixedW - (InfoPadX * 2) - LedDiameter - 8);

            string nameText = m.Name ?? "";
            string ipText = m.ServerIp ?? "-";
            string statusText = string.IsNullOrEmpty(m.Status) ? "Idle" : m.Status;

            int nameH = MeasureLineHeight(g, NameFont);
            int ipH = MeasureWrappedHeight(g, ipText, MetaFont, textW);
            int stateH = MeasureWrappedHeight(g, statusText, MetaFont, textW);
            int infoH = Math.Max(LedDiameter, nameH + LineGap + ipH + LineGap + stateH) + (InfoPadY * 2);

            // ── (2) 장비 이미지(캐시)는 infoH + ImagePanelGap만큼 아래 공간을 남기고 그림
            if (!DeviceCache.ContainsKey(m) || DeviceCache[m] == null)
            {
                var bmp = new Bitmap(rect.Width, rect.Height);
                using (var gg = Graphics.FromImage(bmp))
                {
                    gg.CompositingMode = CompositingMode.SourceOver;
                    gg.CompositingQuality = CompositingQuality.HighQuality;
                    gg.SmoothingMode = SmoothingMode.AntiAlias;
                    gg.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    gg.PixelOffsetMode = PixelOffsetMode.None;

                    Image img = null;
                    try
                    {
                        switch (m.Type)
                        {
                            case DeviceType.Printer: img = Properties.Resources.Printer; break;
                            case DeviceType.Furnace: img = Properties.Resources.Furnace; break;
                            case DeviceType.Polisher: img = Properties.Resources.Polisher; break;
                            case DeviceType.Cleaner: img = Properties.Resources.Cleaner; break;
                        }
                    }
                    catch { }

                    int pad = 6;
                    int availW = Math.Max(1, rect.Width - pad * 2);
                    int availH = Math.Max(1, rect.Height - (infoH + ImagePanelGap) - pad * 2); // ★ 패널 높이만큼 제외!

                    if (img != null)
                    {
                        float s = Math.Min((float)availW / img.Width, (float)availH / img.Height);
                        int dw = Math.Max(1, (int)Math.Round(img.Width * s));
                        int dh = Math.Max(1, (int)Math.Round(img.Height * s));
                        int dx = (rect.Width - dw) / 2;
                        int dy = pad; // TOP
                        gg.DrawImage(img, new Rectangle(dx, dy, dw, dh));
                    }
                }
                DeviceCache[m] = bmp;
            }

            g.DrawImageUnscaled(DeviceCache[m], rect.Location);

            // ── (3) 정보 패널(고정폭)은 g(메인) 위에 그린다(겹침 없음)
            var infoRect = new Rectangle(
                rect.Left + (rect.Width - InfoFixedW) / 2,
                rect.Bottom - infoH - PanelMargin,
                InfoFixedW,
                infoH
            );

            using (var path = Rounded(infoRect, 10))
            using (var panelBrush = new SolidBrush(Color.FromArgb(200, 20, 20, 20)))
            using (var pen = new Pen(Color.FromArgb(60, 255, 255, 255), 1f))
            {
                g.FillPath(panelBrush, path);
                g.DrawPath(pen, path);
            }

            // LED
            var ledRect = new Rectangle(
                infoRect.Left + InfoPadX,
                infoRect.Top + (infoRect.Height - LedDiameter) / 2,
                LedDiameter, LedDiameter
            );
            bool isOn = m.Connected;
            Color core = isOn ? Color.FromArgb(0x34, 0xC7, 0x59) : Color.FromArgb(120, 120, 120);
            Color glow = isOn ? Color.FromArgb(0x6E, 0xE1, 0x7A) : Color.FromArgb(100, 100, 100);

            if (isDraggingSel) { using (var b = new SolidBrush(core)) g.FillEllipse(b, ledRect); }
            else { DrawGlowDot(g, ledRect, glow, core, 6); }

            // 텍스트
            int textLeft = ledRect.Right + LedTextGap;
            var textArea = new Rectangle(
                textLeft,
                infoRect.Top + InfoPadY,
                Math.Max(1, infoRect.Right - textLeft - InfoPadX + 2),
                infoRect.Height - (InfoPadY * 2)
            );

            var nameRect = new Rectangle(textArea.Left, textArea.Top, textArea.Width, nameH);
            TextRenderer.DrawText(g, nameText, NameFont, nameRect,
                Color.White, Color.Transparent,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter |
                TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix);

            var ipRect = new Rectangle(textArea.Left, nameRect.Bottom + LineGap, textArea.Width, textArea.Height);
            TextRenderer.DrawText(g, ipText, MetaFont, ipRect,
                Color.White, Color.Transparent,
                TextFormatFlags.Left | TextFormatFlags.Top | TextFormatFlags.WordBreak |
                TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix);

            var ipSize = TextRenderer.MeasureText(g, ipText, MetaFont,
                            new Size(textArea.Width, int.MaxValue),
                            TextFormatFlags.WordBreak | TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix);
            var stRect = new Rectangle(textArea.Left, nameRect.Bottom + LineGap + ipSize.Height + LineGap,
                                       textArea.Width, textArea.Height);
            TextRenderer.DrawText(g, statusText, MetaFont, stRect,
                Color.FromArgb(240, 240, 240), Color.Transparent,
                TextFormatFlags.Left | TextFormatFlags.Top | TextFormatFlags.WordBreak |
                TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix);

            if (isDraggingSel && DragIsOverlapping)
            {
                using (var p = new Pen(Color.Red, 2f))
                {
                    p.Alignment = PenAlignment.Inset;

                    if (isDraggingSel && DragIsOverlapping)
                    {
                        var union = Rectangle.Union(rect, infoRect);

                        using (var u = new Pen(Color.Red, 2f))
                        {
                            u.Alignment = PenAlignment.Inset;
                            g.DrawRectangle(u, union);
                        }
                    }
                }
            }
        }

        private static GraphicsPath Rounded(Rectangle r, int radius)
        {
            var p = new GraphicsPath();
            int d = radius * 2;
            p.AddArc(r.X, r.Y, d, d, 180, 90);
            p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            p.CloseFigure();
            return p;
        }

        private static void DrawGlowDot(Graphics g, Rectangle coreRect, Color glow, Color core, int glowSize)
        {
            for (int i = glowSize; i >= 1; i--)
            {
                var r = Rectangle.Inflate(coreRect, i * 2, i * 2);
                int a = (int)(22 + 18.0 * (glowSize - i));
                if (a > 90) a = 90;
                using (var b = new SolidBrush(Color.FromArgb(a, glow)))
                    g.FillEllipse(b, r);
            }
            using (var b = new SolidBrush(core))
                g.FillEllipse(b, coreRect);

            var hi = new Rectangle(coreRect.X + 2, coreRect.Y + 2, coreRect.Width - 4, coreRect.Height - 4);
            using (var wb = new SolidBrush(Color.FromArgb(100, Color.White)))
                g.FillEllipse(wb, hi);
        }

        private static int MeasureLineHeight(Graphics g, Font font)
        {
            var size = g.MeasureString("Ag", font);
            return (int)Math.Ceiling(size.Height);
        }

        private static int MeasureWrappedHeight(Graphics g, string text, Font font, int width)
        {
            if (string.IsNullOrEmpty(text)) text = " ";
            var size = g.MeasureString(text, font, width);
            return (int)Math.Ceiling(size.Height);
        }

        public void DrawCachedBackground(Graphics g, Rectangle dest, Rectangle src)
        {
            if (BgCache == null || BgDirty) RebuildBackgroundCache();
            if (BgCache != null) g.DrawImage(BgCache, dest, src, GraphicsUnit.Pixel);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ClearDeviceCache();
                BgCache?.Dispose();
                BgCache = null;
                FloorImage?.Dispose();
                FloorImage = null;
            }
            base.Dispose(disposing);
        }

        private class InputBox : Form
        {
            public string Value { get { return _tb.Text; } }
            private System.Windows.Forms.TextBox _tb;

            public InputBox(string title, string initial)
            {
                Text = title;
                FormBorderStyle = FormBorderStyle.FixedDialog;
                StartPosition = FormStartPosition.CenterParent;
                MinimizeBox = false; MaximizeBox = false;
                Width = 360; Height = 150;

                _tb = new System.Windows.Forms.TextBox { Left = 12, Top = 12, Width = 320, Text = initial };
                var ok = new System.Windows.Forms.Button { Text = "OK", Left = 170, Width = 70, Top = 50, DialogResult = DialogResult.OK };
                var cancel = new System.Windows.Forms.Button { Text = "Cancel", Left = 255, Width = 70, Top = 50, DialogResult = DialogResult.Cancel };
                AcceptButton = ok; CancelButton = cancel;

                Controls.Add(_tb); Controls.Add(ok); Controls.Add(cancel);
            }
        }
    }
}
