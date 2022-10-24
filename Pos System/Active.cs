using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace Pos_System
{
    public partial class Active : Form
    {
        public Active()
        {
            InitializeComponent();
        }
      
        //public static string path = @"C:\Windows\System\SystemProtectionMicroV3.dll";

        //private void Btn_Active_Click(object sender, EventArgs e)
        //{
        //    if(textSerial.Text.Trim() == "active")
        //    {
        //        MessageBox.Show("تم التفعيل بنجاح", "نجحت العملية", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, 0, MessageBoxOptions.RtlReading);

        //        if(!File.Exists(path))
        //        File.WriteAllText(path, "SystemProtection Definder V3.0");
            

        //        this.Hide();
        //        LoginForm login = new LoginForm();
        //        login.Show();
        //    }
        //    else
        //        MessageBox.Show("السريال غير صحيح !", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error, 0, MessageBoxOptions.RtlReading);

        //}

        private void Active_Load(object sender, EventArgs e)
        {
            this.Icon = Icon.ExtractAssociatedIcon(AppDomain.CurrentDomain.FriendlyName);
        }

        private void ImageButtonFacebook_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.facebook.com/KING231MONTSER");

        }

        private void Active_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.ExitThread();
        }
    }
}
