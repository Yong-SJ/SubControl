using System;
using System.Drawing;
using System.Windows.Forms;

namespace SubControl.Facility
{
    /// <summary>
    /// 미니멀한 다크 스타일의 컨텍스트 메뉴 렌더러.
    /// - 항목 하이라이트는 사각형으로, 항목 내부만 칠합니다.
    /// - 텍스트는 수직 중앙 정렬.
    /// 사용법) cms.Renderer = new FlatMenuRenderer();
    /// </summary>
    public class FlatMenuRenderer : ToolStripProfessionalRenderer
    {
        public Color BackColor { get; set; } = Color.FromArgb(44, 48, 52);   // 전체 배경
        public Color HoverColor { get; set; } = Color.FromArgb(64, 70, 76);   // 항목 호버
        public Color TextColor { get; set; } = Color.White;                  // 텍스트
        public Color SepColor { get; set; } = Color.FromArgb(90, 120, 125, 130); // 구분선

        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            using (var b = new SolidBrush(BackColor))
                e.Graphics.FillRectangle(b, e.AffectedBounds);
        }

        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            // 항목 내부 좌표계(0,0) 기준으로 자기 영역만 칠한다
            Rectangle r = new Rectangle(Point.Empty, e.Item.Bounds.Size);

            if (e.Item.Selected && e.Item.Enabled)
            {
                using (var b = new SolidBrush(HoverColor))
                    e.Graphics.FillRectangle(b, r);
            }
            // 선택되지 않은 경우는 기본(투명)
        }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            // 좌우 패딩 적용 후, 세로 중앙 정렬로 텍스트 그리기
            const int padX = 12;
            Rectangle itemRect = new Rectangle(Point.Empty, e.Item.Bounds.Size);
            Rectangle textRect = new Rectangle(
                padX, 0,
                Math.Max(1, itemRect.Width - padX * 2),
                itemRect.Height
            );

            var flags = TextFormatFlags.Left
                        | TextFormatFlags.VerticalCenter
                        | TextFormatFlags.NoPrefix
                        | TextFormatFlags.NoPadding;

            TextRenderer.DrawText(
                e.Graphics,
                e.Text ?? string.Empty,
                e.TextFont,
                textRect,
                TextColor,
                Color.Transparent,
                flags
            );
        }

        protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
        {
            // 좌우 여백을 둔 가는 구분선
            int y = e.Item.Bounds.Height / 2;
            int left = 10;
            int right = e.Item.Bounds.Width - 10;

            using (var p = new Pen(SepColor))
                e.Graphics.DrawLine(p, left, y, right, y);
        }

        // 선택적으로 체크박스/이미지 여백 제거
        protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
        {
            // 이미지 마진 배경을 칠하지 않음(=완전 플랫)
            // base 호출 생략
        }
    }
}
