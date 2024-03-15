using LibVLCSharp.Shared;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsAppVLC
{
    public partial class FormVideoTable : Form
    {
        private readonly List<VideoControl> videoControls = new List<VideoControl>();
        private LibVLC _libVLC;
        private readonly Label labelMessage = new Label
        {
            TextAlign = ContentAlignment.MiddleCenter,
            Dock = DockStyle.Fill,
            Font = new Font("微软雅黑", 30),
            ForeColor = Color.Blue,
        };

        private void InitLibVLC()
        {
            if (_libVLC == null)
            {
                //_libVLC = new LibVLC(enableDebugLogs: true);
                _libVLC = new LibVLC();
            }
            Invoke(new Action(() =>
            {
                ShowMessageLabel("请在菜单栏选择要查看的视频");
            }));
        }

        public FormVideoTable()
        {
            InitializeComponent();
            ShowMessageLabel("正在初始化视频控件...");
            new Thread(new ThreadStart(InitLibVLC)).Start();

            加载配置文件();

            int cols = 4;
            int rows = 4;
            tableLayoutPanel1.ColumnCount = cols;
            tableLayoutPanel1.ColumnStyles.Clear();
            for (int i = 1; i <= cols; i++)
            {
                tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            }
            tableLayoutPanel1.RowCount = rows;
            tableLayoutPanel1.RowStyles.Clear();
            for (int i = 1; i <= rows; i++)
            {
                tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            }
        }

        private void VideoControl_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // 双击全屏功能
            VideoControl sender1 = (VideoControl)sender;
            if (e.Button == MouseButtons.Right)
            {
                return;
            }
            tableLayoutPanel1.Visible = !tableLayoutPanel1.Visible;
            ControlsInit(sender1);
            if (tableLayoutPanel1.Visible)
            {
                Controls.Add(tableLayoutPanel1);
                tableLayoutPanel1.Controls.Clear();
                foreach (VideoControl videoControl in videoControls)
                {
                    tableLayoutPanel1.Controls.Add(videoControl);
                }
            }
            else
            {
                // 全屏化
                Controls.Add(sender1);
            }
        }

        private void ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EndMessageLabel();
            if (_libVLC == null)
            {
                //_libVLC = new LibVLC(enableDebugLogs: true);
                _libVLC = new LibVLC();
            }
            FormClosing += delegate (object _sender, FormClosingEventArgs _e)
            {
                foreach (VideoControl videoControl in videoControls)
                {
                    videoControl.Stop();
                }
                foreach (VideoControl videoControl in videoControls)
                {
                    videoControl.Dispose();
                }
                _libVLC.Dispose();
            };
            videoControls.Add(new VideoControl(_libVLC, "http://5.28.32.50:10122/TV0001", "test1"));
            videoControls.Add(new VideoControl(_libVLC, "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4", "test2"));
            videoControls.Add(new VideoControl(_libVLC, "http://5.28.32.50:10122/TV0001", "test3"));
            videoControls.Add(new VideoControl(_libVLC, "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4", "test4"));
            videoControls.Add(new VideoControl(_libVLC, "http://5.28.32.50:10122/TV0001", "test5"));
            videoControls.Add(new VideoControl(_libVLC, "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4", "test6"));
            foreach (VideoControl videoControl in videoControls)
            {
                videoControl.MouseDoubleClick += VideoControl_MouseDoubleClick;
                tableLayoutPanel1.Controls.Add(videoControl);
            }
        }
        private void ShowMessageLabel(string LabelText)
        {
            ControlsInit(labelMessage);
            labelMessage.Text = LabelText;
        }

        private void EndMessageLabel()
        {
            ControlsInit(tableLayoutPanel1);
        }

        private async Task 获取分屏列表()
        {
            ShowMessageLabel("正在加载频道列表...");

            await Task.Delay(5000);
            string urlPre_api = @"http://5.28.32.50:10128/xixi";
            Dictionary<string, string> postData = new Dictionary<string, string>();
            Firadio.HttpUtils httpUtils = new Firadio.HttpUtils(urlPre_api);
            try
            {
                Firadio.Response response = await httpUtils.POST数据(@"/iptv.php", postData);
                Console.WriteLine(response.Table.Rows);
                //iptv_channels = response.Table.Rows;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\r\n" + httpUtils.MessageShow.Text, httpUtils.MessageShow.Caption);
            }
            EndMessageLabel();
        }

        private void ControlsInit(Control control)
        {
            Controls.Clear();
            Controls.Add(menuStrip1);
            Controls.Add(control);
        }

        private void 加载配置文件()
        {
            ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItem.Text = "下载配置";
            toolStripMenuItem.Click += ToolStripMenuItem_Click;
            menuStrip1.Items.Add(toolStripMenuItem);

            ToolStripMenuItem toolStripMenuItem2 = new ToolStripMenuItem();
            toolStripMenuItem2.Text = "4分屏";
            toolStripMenuItem2.Checked = true;

            ToolStripMenuItem toolStripMenuItem3 = new ToolStripMenuItem();
            toolStripMenuItem3.Text = "444分屏";
            toolStripMenuItem3.Checked = true;
            toolStripMenuItem2.DropDownItems.Add(toolStripMenuItem3);
            menuStrip1.Items.Add(toolStripMenuItem2);
        }
    }
}
