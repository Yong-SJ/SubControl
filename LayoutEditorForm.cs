using System.Windows.Forms;
using SubControl.Facility;

namespace SubControl
{
    public partial class LayoutEditorForm : Form
    {
        private FacilityMapControl _facility;

        public LayoutEditorForm()
        {
            InitializeComponent();

            _facility = new FacilityMapControl();
            _facility.Dock = DockStyle.Fill;
            _facility.ShowLayoutUx = true; // ✅ 편집 창에서는 편집 UX 표시
            Controls.Add(_facility);
        }
    }
}
