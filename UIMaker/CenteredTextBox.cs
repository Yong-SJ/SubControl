using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

public class CenteredTextBox : TextBox
{
    private const int EM_SETRECT = 0x00B3;

    [DllImport("user32.dll")]
    private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, ref RECT lp);
    public bool TreatAsSingleLine { get; set; } = true;
    public int TextPaddingLeft { get; set; } = 2;
    public int TextPaddingRight { get; set; } = 2;

    [StructLayout(LayoutKind.Sequential)]
    private struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    public CenteredTextBox()
    {
        this.Multiline = true; // 중앙 정렬을 위해 멀티라인 활성화
    }

    protected override void OnCreateControl()
    {
        base.OnCreateControl();
        AdjustTextAlignment(); // 컨트롤 생성 후 초기 정렬 적용
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        AdjustTextAlignment();
    }

    protected override void OnTextChanged(EventArgs e)
    {
        base.OnTextChanged(e);

        if (TreatAsSingleLine)
        {
            string t = this.Text;
            if (t.IndexOf('\r') >= 0 || t.IndexOf('\n') >= 0)
            {
                int sel = this.SelectionStart;
                this.Text = t.Replace("\r", "").Replace("\n", "");
                this.SelectionStart = Math.Min(sel, this.TextLength);
            }
        }

        AdjustTextAlignment();
    }

    protected override void OnFontChanged(EventArgs e)
    {
        base.OnFontChanged(e);
        AdjustTextAlignment();
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        AdjustTextAlignment(); // 핸들 생성 후 정렬 적용
    }

    private void AdjustTextAlignment()
    {
        if (this.Multiline && this.IsHandleCreated)
        {
            RECT rect = new RECT
            {
                Left = TextPaddingLeft,
                Top = (this.ClientSize.Height - this.Font.Height) / 2, // 세로 중앙 정렬
                Right = this.ClientSize.Width - TextPaddingRight,
                Bottom = this.ClientSize.Height
            };

            // 텍스트 영역 업데이트
            SendMessage(this.Handle, EM_SETRECT, IntPtr.Zero, ref rect);

            // 텍스트 강제 재렌더링
            this.Invalidate();
        }
    }
}
