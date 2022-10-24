using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;
using System.IO;
using System.Data.SQLite;

namespace Pos_System
{
    public partial class LoginForm : Form 
    {
        public LoginForm()
        {
            InitializeComponent();
        }
       Login login;
       public static string username = "";
       public static string password = "";
       //public static SqlConnection connect = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|database\posdata.mdf;Integrated Security=True;MultipleActiveResultSets=True");
       public  bool IsAdmin;
       public  bool logged;
      
        public void LoadInfo()
        {
          
            using (SQLiteCommand cmd = new SQLiteCommand("select * from shop_info limit 1",MainForm.connect))
            {
                
                SQLiteDataReader read = cmd.ExecuteReader();
                read.Read();
                
                if (read.HasRows)
                {
                    LabelTitle.Text = this.Text = read[0].ToString();

                    if (!read.IsDBNull(4))
                    {
                        byte[] byt = (byte[])read[4];
                        MemoryStream stream = new MemoryStream(byt);
                        PictureBoxLogo.Image = Image.FromStream(stream);
                        stream.Dispose();
                    }

                }
                read.Close();
            }
        }
       private void LoginForm_Load(object sender, EventArgs e)
        {
            if (MainForm.connect.State == ConnectionState.Closed)
                MainForm.connect.Open();

                this.Icon = Icon.ExtractAssociatedIcon(AppDomain.CurrentDomain.FriendlyName);


            LoadInfo();

            textUserName.Text = "g";
            textPassword.Text = "g";
            textUserName.Reset();
            textPassword.Reset();
            textUserName.Focus();



        }

        private void CheckBoxShowPassword_CheckedChanged(object sender, Bunifu.UI.WinForms.BunifuCheckBox.CheckedChangedEventArgs e)
        {
            if (CheckBoxShowPassword.Checked == true)
            {
                textPassword.UseSystemPasswordChar = false;
                textPassword.PasswordChar = '\0';
            }
            else
                textPassword.UseSystemPasswordChar = true;
        }

        private void LoginForm_FormClosing(object sender, FormClosingEventArgs e)
        {
           

            if (!logged)
            {
                MainForm.trans.Commit();
                if (MainForm.connect.State == ConnectionState.Open)
                    MainForm.connect.Close();

               

                Application.ExitThread();
            }

        }

        private void Btn_Login_Click(object sender, EventArgs e)
        {
            IsAdmin = false;
            login = new Login(MainForm.connect);
            login.username = textUserName.Text;
            login.password = textPassword.Text;

            if (login.HasRows())
            {
                if (login.CheckLogin())
                {
                    IsAdmin = login.IsAdmin(login.username);
                    logged = true;

                    username = login.username;
                    password = login.password;

                    textUserName.Text = "";
                    textPassword.Text = "";

                    if (MainForm.accountstable.Columns.Count <= 0)
                    {
                        using (SQLiteDataAdapter adapt = new SQLiteDataAdapter("select * from login", MainForm.connect))
                        {
                            
                            adapt.Fill(MainForm.accountstable);
                            adapt.FillSchema(MainForm.accountstable, SchemaType.Mapped);

                        }
                    }

                    MessageBox.Show("مرحبا يا " + login.username, "مرحبا", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, 0, MessageBoxOptions.RtlReading);



                    MainForm.ActiveForm.Enabled = true;

                    /*
                     if (!login.IsAdmin())
                     {
                         foreach (Control c in form.Panel_Menu6.Controls)
                         {
                             if (c is Bunifu.UI.WinForms.BunifuButton.BunifuButton)
                             {
                                 c.Hide();
                             }
                         }
                         form.Btn_EditAccount.Show();
                     }
                     else
                     {
                         //form.textUserName.DataSource = accountstable;
                         //form.textUserName.DisplayMember = "username";

                         foreach (Control c in form.Panel_Menu6.Controls)
                         {
                             if (c is Bunifu.UI.WinForms.BunifuButton.BunifuButton)
                                 c.Show();
                         }

                     }
                     */
                    this.Hide();
                }
                else
                    MessageBox.Show("الرجاء التحقق من الاسم او كلمة المرور !", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error, 0, MessageBoxOptions.RtlReading);

            }
            else
                MessageBox.Show("عفوا لايوجد اي حساب مسجل !", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error, 0, MessageBoxOptions.RtlReading);




        }

        private void Btn_SignUp_Click(object sender, EventArgs e)
        {
            login = new Login(MainForm.connect);
            login.username = textUserName.Text;
            login.password = textPassword.Text;
            login.permission = "admin";
            if (textUserName.Text.Trim() != "" && textPassword.Text.Trim() != "")
            {
                if (!login.HasRows())
                {
                    if (!login.UserNameExist(login.username))
                    {
                        login.Register();
                        MessageBox.Show("تم التسجيل بنجاح", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, 0, MessageBoxOptions.RtlReading);

                    }
                    else
                        MessageBox.Show("هذا الاسم موجود بالفعل !", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error, 0, MessageBoxOptions.RtlReading);

                }
                else
                    MessageBox.Show("عفوا لايمكنك التسجيل مجددا !", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error, 0, MessageBoxOptions.RtlReading);
            }
            else
                MessageBox.Show("الرجاء تعبة جميع الحقول !", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error, 0, MessageBoxOptions.RtlReading);


        }

        private void LoginForm_Shown(object sender, EventArgs e)
        {
            this.textUserName.Focus();

        }

        private void LoginForm_FormClosed(object sender, FormClosedEventArgs e)
        {

        }
    }
}
