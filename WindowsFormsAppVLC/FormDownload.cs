using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsAppVLC
{
    public partial class FormDownload : Form
    {
        public FormDownload()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
        }
        readonly string jsonPath = Application.ExecutablePath + ".json";

        class CustomMessageBox : Form
        {
            public CustomMessageBox(string message)
            {
                // 设置窗体属性
                Text = "提示";
                StartPosition = FormStartPosition.CenterParent;
                FormBorderStyle = FormBorderStyle.FixedDialog;
                MaximizeBox = false;
                MinimizeBox = false;
                Padding = new Padding(10);

                // 创建消息文本标签
                Label label = new Label
                {
                    Text = message,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Fill,
                };

                // 创建确定按钮
                Button okButton = new Button
                {
                    Text = "确定",
                    DialogResult = DialogResult.OK,
                    Dock = DockStyle.Bottom,
                };
                okButton.Click += (sender, e) => Close();

                // 添加控件到窗体
                Controls.Add(label);
                Controls.Add(okButton);

                // 设置窗体大小
                Size = new Size(200, 150);
            }
        }

        readonly WebClient webClient = new WebClient { Encoding = Encoding.UTF8 };
        private void Header(string key, string val)
        {
            webClient.Headers[key] = val;
        }
        private async Task 下载配置文件()
        {
            toolStripStatusLabel1.Text = "正在下载配置文件...";

            string version = Assembly.GetEntryAssembly().GetName().Version.ToString();
            Header("version", version);
            string sJson;
            try
            {
                byte[] Result = await webClient.DownloadDataTaskAsync(comboBox1.Text);
                sJson = Encoding.UTF8.GetString(Result);
            }
            catch (Exception ex)
            {
                toolStripStatusLabel1.Text = "文件下载失败: " + ex.ToString();
                return;
            }
            toolStripStatusLabel1.Text = "配置文件获取成功,正在解析JSON...";
            Firadio.Response response;
            try
            {
                response = Firadio.HttpUtils.JSON.Parse<Firadio.Response>(sJson);
                Console.WriteLine(response);
            }
            catch (Exception ex)
            {
                toolStripStatusLabel1.Text = "JSON解析报错: " + ex.ToString();
                return;
            }
            File.WriteAllText(jsonPath, sJson);
            toolStripStatusLabel1.Text = "配置文件下载完成.";
            new CustomMessageBox(response.Message).ShowDialog();
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            _ = 下载配置文件();
            button1.Enabled = true;
        }

        private void FormDownload_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (File.Exists(jsonPath))
            {
                DialogResult = DialogResult.OK;
            }
        }
    }
}
