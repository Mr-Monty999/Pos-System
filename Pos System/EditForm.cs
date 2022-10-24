using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pos_System
{
    public partial class EditForm : Form
    {
        public EditForm()
        {
            InitializeComponent();
        }

        public double number;
        public bool OkPressed;
        private void EditForm_Load(object sender, EventArgs e)
        {

        }
        private void Btn_0_Click(object sender, EventArgs e)
        {
            TextView.Text += "0";
            number = double.Parse(TextView.Text);


        }

        private void Btn_1_Click(object sender, EventArgs e)
        {
            TextView.Text += "1";
            number = double.Parse(TextView.Text);

        }

        private void Btn_2_Click(object sender, EventArgs e)
        {
            TextView.Text += "2";
            number = double.Parse(TextView.Text);

        }

        private void Btn_3_Click(object sender, EventArgs e)
        {
            TextView.Text += "3";
            number = double.Parse(TextView.Text);

        }

        private void Btn_4_Click(object sender, EventArgs e)
        {
            TextView.Text += "4";
            number = double.Parse(TextView.Text);

        }

        private void Btn_5_Click(object sender, EventArgs e)
        {
            TextView.Text += "5";
            number = double.Parse(TextView.Text);

        }

        private void Btn_6_Click(object sender, EventArgs e)
        {
            TextView.Text += "6";
            number = double.Parse(TextView.Text);

        }

        private void Btn_7_Click(object sender, EventArgs e)
        {
            TextView.Text += "7";
            number = double.Parse(TextView.Text);

        }

        private void Btn_8_Click(object sender, EventArgs e)
        {
            TextView.Text += "8";
            number = double.Parse(TextView.Text);

        }

        private void Btn_9_Click(object sender, EventArgs e)
        {
            TextView.Text += "9";
            number = double.Parse(TextView.Text);

        }

        private void Btn_OK_Click(object sender, EventArgs e)
        {

            if (TextView.Text.Trim() == "")
                number = 0;

            OkPressed = true;
            this.Close();


        }

        private void Btn_Clear_Click(object sender, EventArgs e)
        {

            number = 0;
            TextView.Text = "";
            this.Focus();

        }

        private void EditForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar >= '0' && e.KeyChar <= '9')
            {
                TextView.Text += e.KeyChar;
                number = double.Parse(TextView.Text);
            }

        }

        private void EditForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Back)
            {
                string text = "";
               for(int x= 0; x<TextView.TextLength-1;x++)
                    text += TextView.Text[x];

                TextView.Text = text;

                if(TextView.Text.Trim() != "")
                number = double.Parse(text);
            }

            if (e.KeyCode == Keys.C || e.KeyCode == Keys.Delete)
                Btn_Clear.PerformClick();

        }

        private void Btn_Backspace_Click(object sender, EventArgs e)
        {
            
            string text = "";
            for (int x = 0; x < TextView.TextLength-1; x++)
                text += TextView.Text[x];

            TextView.Text = text;

            if (TextView.Text.Trim() != "")
                number = double.Parse(text);
          


        }
    }
}
