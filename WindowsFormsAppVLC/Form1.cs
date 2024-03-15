using System;
using System.Windows.Forms;

namespace WindowsFormsAppVLC
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            Form2 form2 = new Form2();
            form2.Text = "频道001-016";
            form2.Show();
            button1.Enabled = true;
        }
    }
}
