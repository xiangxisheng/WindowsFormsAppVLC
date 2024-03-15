using System;
using System.Windows.Forms;

namespace WindowsFormsAppVLC
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
            int cols = 2;
            int rows = 2;
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

            for (int i = 1; i <= 4; i++)
            {
                Button button1 = new Button();
                button1.Dock = DockStyle.Fill;
                button1.Text = "频道001-016";
                button1.Click += delegate (object sender, EventArgs e)
                {
                    button1.Enabled = false;
                    Form2 form2 = new Form2();
                    form2.Text = "频道001-016";
                    form2.Width = 1250;
                    form2.Height = 720;
                    form2.Show();
                    button1.Enabled = true;
                };
                tableLayoutPanel1.Controls.Add(button1);
            }
        }

    }
}
