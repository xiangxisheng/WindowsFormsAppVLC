using LibVLCSharp.Shared;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace WindowsFormsAppVLC
{
    public partial class Form2 : Form
    {
        public List<VideoControl> videoControls = new List<VideoControl>();
        public readonly LibVLC _libVLC;
        public Form2()
        {
            InitializeComponent();

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

            //_libVLC = new LibVLC(enableDebugLogs: true);
            _libVLC = new LibVLC();
            Load += Form1_Load;
            FormClosed += Form1_FormClosed;
            FormClosing += Form2_FormClosing;
            videoControls.Add(new VideoControl(_libVLC, "http://5.28.32.50:10122/TV0001", "test1"));
            videoControls.Add(new VideoControl(_libVLC, "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4", "test2"));
            videoControls.Add(new VideoControl(_libVLC, "http://5.28.32.50:10122/TV0001", "test3"));
            videoControls.Add(new VideoControl(_libVLC, "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4", "test4"));
            videoControls.Add(new VideoControl(_libVLC, "http://5.28.32.50:10122/TV0001", "test5"));
            videoControls.Add(new VideoControl(_libVLC, "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4", "test6"));
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
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
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            foreach (VideoControl videoControl in videoControls)
            {
                tableLayoutPanel1.Controls.Add(videoControl);
            }
        }
    }
}
