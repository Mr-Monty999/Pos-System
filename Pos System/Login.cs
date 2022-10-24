using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;
using System.Windows.Forms;
using System.Data.SQLite;

namespace Pos_System
{
    public class Login
    {
        protected SQLiteConnection connect;
        protected SQLiteCommand cmd;
        protected SQLiteDataReader read;

        public string username
        { get; set; }

        public string password
        { get; set; }

        public string permission
        { get; set; }

        public Login(SQLiteConnection connect)
        {
            this.connect = connect;
        }

        public bool HasRows()
        {
            bool hasrows = false;

            try
            {
                using (cmd = new SQLiteCommand("select * from login", connect))
                {
                    read = cmd.ExecuteReader();
                     hasrows = read.HasRows;
                    read.Close();

                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error, 0, MessageBoxOptions.RtlReading);
            }
            return hasrows;

        }

        public bool UserNameExist(string username)
        {
            bool exist = false;
            try
            {
                using (cmd = new SQLiteCommand("select * from login where username = @user", connect))
                {
                    cmd.Parameters.AddWithValue("@user", username);
                    read = cmd.ExecuteReader();
                    read.Read();
                    exist = read.HasRows;
                    read.Close();
                    

                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error, 0, MessageBoxOptions.RtlReading);
            }
            return exist;
        }

        public void Register()
        {
            try
            {
                using (cmd = new SQLiteCommand("insert into login values(@user,@pass,@perm)", connect))
                {
                    cmd.Parameters.AddWithValue("@user", username);
                    cmd.Parameters.AddWithValue("@pass", password);
                    cmd.Parameters.AddWithValue("@perm", permission);
                    cmd.ExecuteNonQuery();

                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error, 0, MessageBoxOptions.RtlReading);
            }
        }

        public bool CheckLogin()
        {
            bool check = false;
            try
            {
                using (cmd = new SQLiteCommand("select * from login where username = @user and password = @pass", connect))
                {
                    cmd.Parameters.AddWithValue("@user", username);
                    cmd.Parameters.AddWithValue("@pass", password);
                    read = cmd.ExecuteReader();
                    check = read.HasRows;
                    read.Close();
                    

                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error, 0, MessageBoxOptions.RtlReading);
            }

            return check;
        }

        public bool IsAdmin(string username)
        {
            bool check = false;

            try
            {
                using (cmd = new SQLiteCommand("select * from login where username = @user", connect))
                {
                
                    cmd.Parameters.AddWithValue("@user", username);
                    read = cmd.ExecuteReader();
                    read.Read();
                    if (read.HasRows)
                        check = read[2].ToString() == "admin";
                    else
                        check = false;
                    read.Close();
                }
                
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error, 0, MessageBoxOptions.RtlReading);
            }
            return check;


        }

        public void Update(string existname)
        {
            try
            {
                if (UserNameExist(existname))
            {
                using (cmd = new SQLiteCommand("update [login] set [username] = @user , [password] = @pass  where [username] = @exist", connect))
                {
                    cmd.Parameters.AddWithValue("@user", username);
                    cmd.Parameters.AddWithValue("@pass", password);
                    cmd.Parameters.AddWithValue("@exist", existname);

                    cmd.ExecuteNonQuery();

                }
            }
        }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error, 0, MessageBoxOptions.RtlReading);
            }
}

        public void Delete()
        {
            try
            {
                using (cmd = new SQLiteCommand("delete from login where username = @user", connect))
                {
                    cmd.Parameters.AddWithValue("@user", username);

                    cmd.ExecuteNonQuery();

                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error, 0, MessageBoxOptions.RtlReading);
            }
        }

        
    }
}
