using LibVLCSharp.Shared;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace WindowsFormsAppVLC
{
    public partial class FormVideoTable : Form
    {
        private readonly List<VideoControl> videoControls = new List<VideoControl>();
        private LibVLC _libVLC;
        private string labelMsgText;
        private readonly Label labelMessage = new Label
        {
            TextAlign = ContentAlignment.MiddleCenter,
            Dock = DockStyle.Fill,
            Font = new Font("微软雅黑", 30),
            ForeColor = Color.Blue,
        };

        private void InitLibVLC()
        {
            labelMsgText = "正在初始化视频控件...";
            if (_libVLC == null)
            {
                //_libVLC = new LibVLC(enableDebugLogs: true);
                _libVLC = new LibVLC();
            }
            //Invoke(new Action(() =>
            //{
            //    ShowMessageLabel("请在菜单栏选择要查看的视频");
            //}));
            labelMsgText = "请在菜单栏选择要查看的视频";
        }

        public FormVideoTable()
        {
            InitializeComponent();
            new Thread(new ThreadStart(InitLibVLC)).Start();
            FormClosing += delegate (object _sender, FormClosingEventArgs _e)
            {
                ClearVideoControl();
                _libVLC.Dispose();
            };
            加载配置文件();
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


        private void ClearVideoControl()
        {
            foreach (VideoControl videoControl in videoControls)
            {
                videoControl.Stop();
            }
            foreach (VideoControl videoControl in videoControls)
            {
                videoControl.Dispose();
            }
            videoControls.Clear();
        }

        private void ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem toolStripMenuItem = (ToolStripMenuItem)sender;
            Firadio.Response.MenuItem menuItem = (Firadio.Response.MenuItem)toolStripMenuItem.Tag;
            if (menuItem.Name == "下载配置文件")
            {
                new FormDownload().ShowDialog();
                加载配置文件();
                return;
            }
            labelMsgText = "";

            int cols = menuItem.Cols;
            int rows = menuItem.Rows;
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


            ClearVideoControl();
            for (int i = menuItem.Start; i <= menuItem.End; i++)
            {
                videoControls.Add(new VideoControl(_libVLC, JsonConfig.Channels[i].Liveurl, JsonConfig.Channels[i].Title));
            }
            //videoControls.Add(new VideoControl(_libVLC, "http://5.28.32.50:10122/TV0001", "test5"));
            //videoControls.Add(new VideoControl(_libVLC, "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4", "test6"));
            tableLayoutPanel1.Controls.Clear();
            foreach (VideoControl videoControl in videoControls)
            {
                videoControl.MouseDoubleClick += VideoControl_MouseDoubleClick;
                tableLayoutPanel1.Controls.Add(videoControl);
            }
        }

        private void ControlsInit(Control control)
        {
            Controls.Clear();
            Controls.Add(control);
            Controls.Add(menuStrip1);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (labelMsgText != labelMessage.Text)
            {
                labelMessage.Text = labelMsgText;
                if (labelMsgText == "")
                {
                    ControlsInit(tableLayoutPanel1);
                }
                else
                {
                    ControlsInit(labelMessage);
                }
            }
        }


        private ToolStripMenuItem ToToolStripMenuItem(Firadio.Response.MenuItem menuItem)
        {
            ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItem.Tag = menuItem;
            toolStripMenuItem.Text = menuItem.Name;
            toolStripMenuItem.Click += ToolStripMenuItem_Click;
            return toolStripMenuItem;
        }

        Firadio.Response JsonConfig;

        private void 加载配置文件()
        {
            menuStrip1.Items.Clear();
            // 首先添加一个下载配置文件的按钮，方便随时点击下载
            menuStrip1.Items.Add(ToToolStripMenuItem(new Firadio.Response.MenuItem
            {
                Name = "下载配置文件"
            }));

            string jsonPath = Application.ExecutablePath + ".json";
            if (!File.Exists(jsonPath))
            {
                // 如果配置文件不存在，自动打开下载框
                new FormDownload().ShowDialog();
                return;
            }
            try
            {
                string sJson = File.ReadAllText(jsonPath);
                JsonConfig = Firadio.HttpUtils.JSON.Parse<Firadio.Response>(sJson);
                Console.WriteLine(JsonConfig);
            }
            catch (Exception ex)
            {
                MessageBox.Show("配置文件JSON解析失败: " + ex.ToString());
                return;
            }

            foreach (Firadio.Response.Menu menu in JsonConfig.Menus)
            {
                ToolStripMenuItem menu1 = new ToolStripMenuItem();
                menu1.Text = menu.Name;
                foreach (Firadio.Response.MenuItem menuItem in menu.Items)
                {
                    menuItem.Title = menu.Name + " (" + menuItem.Name + ")";
                    menu1.DropDownItems.Add(ToToolStripMenuItem(menuItem));
                }
                menuStrip1.Items.Add(menu1);
            }
        }

    }
}
