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
    public partial class AmountForm : Form
    {
        public AmountForm()
        {
            InitializeComponent();
        }
        public double amount = 1;
        public string name;
        public double price;
        public double fulltotal;

        private void Numbers_Load(object sender, EventArgs e)
        {
            TextView.Text = amount.ToString();
        }

        private void Btn_0_Click(object sender, EventArgs e)
        {
            TextView.Text += "0";
            amount = double.Parse(TextView.Text);
          

        }

        private void Btn_1_Click(object sender, EventArgs e)
        {
            TextView.Text += "1";
            amount = double.Parse(TextView.Text);

        }

        private void Btn_2_Click(object sender, EventArgs e)
        {
            TextView.Text += "2";
            amount = double.Parse(TextView.Text);

        }

        private void Btn_3_Click(object sender, EventArgs e)
        {
            TextView.Text += "3";
            amount = double.Parse(TextView.Text);

        }

        private void Btn_4_Click(object sender, EventArgs e)
        {
            TextView.Text += "4";
            amount = double.Parse(TextView.Text);

        }

        private void Btn_5_Click(object sender, EventArgs e)
        {
            TextView.Text += "5";
            amount = double.Parse(TextView.Text);

        }

        private void Btn_6_Click(object sender, EventArgs e)
        {
            TextView.Text += "6";
            amount = double.Parse(TextView.Text);

        }

        private void Btn_7_Click(object sender, EventArgs e)
        {
            TextView.Text += "7";
            amount = double.Parse(TextView.Text);

        }

        private void Btn_8_Click(object sender, EventArgs e)
        {
            TextView.Text += "8";
            amount = double.Parse(TextView.Text);

        }

        private void Btn_9_Click(object sender, EventArgs e)
        {
            TextView.Text += "9";
            amount = double.Parse(TextView.Text);

        }

        private void Btn_OK_Click(object sender, EventArgs e)
        {
            if (TextView.Text.Trim() == "")
                amount = 0;

            if (!MainForm.order.Rows.Contains(name))
                {
                    object[] data = { name, amount, price, (price * amount) };
                    MainForm.order.Rows.Add(data);
                }
                else
                {
                    double amountsum = amount + double.Parse(MainForm.order.Rows.Find(name)[1].ToString());
                    double editedprice = price * amountsum;
                    MainForm.order.Rows.Find(name)[1] = amountsum;
                    MainForm.order.Rows.Find(name)[3] = editedprice;

                }

                fulltotal += price * amount;

                MainForm.Label_FullTotal.Text = string.Format("{0:N0}", fulltotal);

                amount = 0;


            this.Close();


        }

        private void Btn_Clear_Click(object sender, EventArgs e)
        {

            
            TextView.Text = "";
            amount = 0;
            this.Focus();
            
        }

        private void Numbers_FormClosing(object sender, FormClosingEventArgs e)
        {
            amount = 1;
            TextView.Text = "";

        }
        private void Numbers_KeyPress(object sender, KeyPressEventArgs e)
        {

            if (e.KeyChar >= '0' && e.KeyChar <= '9')
            {
                TextView.Text += e.KeyChar;
                amount = double.Parse(TextView.Text);
            }



        }

        private void AmountForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Back)
            {
                string text = "";
                for (int x = 0; x < TextView.TextLength - 1; x++)
                    text += TextView.Text[x];

                TextView.Text = text;

                if (TextView.Text.Trim() != "")
                    amount = double.Parse(text);
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
                amount = double.Parse(text);


        }

      
    }
}
