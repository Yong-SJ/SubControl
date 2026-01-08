using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

public class RoundedTextBoxOverlay : UserControl
{
    private readonly TextBox _tb = new TextBox();

    public TextBox InnerTextBox => _tb;

    public int CornerRadius { get; set; } = 20;
    public int BorderThickness { get; set; } = 1;
    public Color BorderColor { get; set; } = Color.Silver;
    public int PaddingX { get; set; } = 12;

    // ===== Win32 중앙정렬용 =====
    private const int EM_SETRECT = 0x00B3;
    private const int EM_SETRECTNP = 0x00B4;
    private const int WM_SETREDRAW = 0x000B;

    [DllImport("user32.dll")]
    private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, ref RECT lp);

    [DllImport("user32.dll")]
    private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);

    [StructLayout(LayoutKind.Sequential)]
    private struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }
    // ==========================

    public RoundedTextBoxOverlay()
    {
        SetStyle(ControlStyles.UserPaint |
                 ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.ResizeRedraw, true);

        BackColor = Color.White;

        _tb.BorderStyle = BorderStyle.None;
        _tb.Multiline = true;          // ★ 필수
        _tb.ScrollBars = ScrollBars.None;
        _tb.WordWrap = true;
        _tb.BackColor = BackColor;

        Controls.Add(_tb);
        _tb.HandleCreated += (s, e) => ApplyTextCentering();

        this.Click += (s, e) => _tb.Focus();
    }

    protected override void OnLayout(LayoutEventArgs e)
    {
        base.OnLayout(e);

        int padX = PaddingX;

        // TextBox 자체를 좌우 패딩만큼 줄여서 올려둠
        _tb.SetBounds(
            padX,
            0,
            Math.Max(10, Width - (padX * 2)),
            Height
        );

        // HandleCreated 이후에도 자주 호출되므로 그대로 호출
        ApplyTextCentering();

    }

    protected override void OnSizeChanged(EventArgs e)
    {
        base.OnSizeChanged(e);
        ApplyRoundRegion();
        ApplyTextCentering();
    }

    protected override void OnFontChanged(EventArgs e)
    {
        base.OnFontChanged(e);
        _tb.Font = this.Font;
        ApplyTextCentering();
    }

    protected override void OnBackColorChanged(EventArgs e)
    {
        base.OnBackColorChanged(e);
        _tb.BackColor = BackColor;
        Invalidate();
    }

    private void ApplyTextCentering()
    {
        if (!_tb.IsHandleCreated)
            return;

        int textHeight = TextRenderer.MeasureText("A", _tb.Font).Height;

        int top = (_tb.ClientSize.Height - textHeight) / 2;
        if (top < 0) top = 0;

        RECT rect = new RECT
        {
            Left = 0,
            Right = _tb.ClientSize.Width,
            Top = top,
            Bottom = top + textHeight + 2
        };

        SendMessage(_tb.Handle, WM_SETREDRAW, IntPtr.Zero, IntPtr.Zero);
        SendMessage(_tb.Handle, EM_SETRECTNP, IntPtr.Zero, ref rect);
        SendMessage(_tb.Handle, WM_SETREDRAW, new IntPtr(1), IntPtr.Zero);

        _tb.Invalidate();
        _tb.Update();
    }


    private void ApplyRoundRegion()
    {
        int r = Math.Max(1, CornerRadius);
        Rectangle rect = new Rectangle(0, 0, Width, Height);

        using (GraphicsPath path = CreateRoundPath(rect, r))
            this.Region = new Region(path);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

        Rectangle rect = new Rectangle(0, 0, Width - 1, Height - 1);
        using (GraphicsPath path = CreateRoundPath(rect, CornerRadius))
        using (Pen pen = new Pen(BorderColor, BorderThickness))
        {
            e.Graphics.DrawPath(pen, path);
        }
    }

    private static GraphicsPath CreateRoundPath(Rectangle rect, int radius)
    {
        int d = radius * 2;
        GraphicsPath path = new GraphicsPath();

        path.AddArc(rect.X, rect.Y, d, d, 180, 90);
        path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
        path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
        path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
        path.CloseFigure();

        return path;
    }
}
