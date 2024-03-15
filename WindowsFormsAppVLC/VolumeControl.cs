using System.Windows.Forms;

namespace WindowsFormsAppVLC
{
    public partial class VolumeControl : UserControl
    {

        readonly VerticalProgressBar verticalProgressBar;
        public int Value
        {
            get
            {
                return verticalProgressBar.Value;
            }
            set
            {
                verticalProgressBar.Value = value;
            }
        }
        public VolumeControl()
        {
            InitializeComponent();
            verticalProgressBar = new VerticalProgressBar();
            verticalProgressBar.Margin = new Padding(0);
            verticalProgressBar.Dock = DockStyle.Fill;
            Controls.Add(verticalProgressBar);
            verticalProgressBar.Minimum = 0;
            verticalProgressBar.Maximum = 255;
            //verticalProgressBar.Value = 0;
        }


        public class VerticalProgressBar : ProgressBar
        {
            protected override CreateParams CreateParams
            {
                get
                {
                    CreateParams cp = base.CreateParams;
                    cp.Style |= 0x04;
                    return cp;
                }
            }
        }
    }
}
