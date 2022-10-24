using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;
using System.IO;
using Bunifu.UI.WinForm;
using Bunifu.UI.WinForms;
using Bunifu.UI.Winforms;
using Bunifu.UI.WinForms.BunifuButton;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Data.SQLite;

namespace Pos_System
{
   public class Items
    {
        protected string _item_name;
        protected string _item_image;
        protected SQLiteConnection connect;
        private SQLiteCommand cmd;
        private SQLiteDataReader read;

        public string item_name
        {
            get { return _item_name; }
            set { _item_name = value; }
        }

        public string item_image
        {
            get { return _item_image; }
            set { _item_image = value; }
        }

        public Items(SQLiteConnection connect)
        {
            this.connect = connect;
        }

        public void Insert()
        {
            //MainForm.trans = connect.BeginTransaction();
            if (_item_image.Trim() != "")
            {
                using (cmd = new SQLiteCommand("insert into items values('" + _item_name + "',@image)", connect))
                {

                    byte[] byt = File.ReadAllBytes(_item_image);
                    cmd.Parameters.AddWithValue("@image", byt);
                    cmd.ExecuteNonQuery();
                }
            }
            else
            {
                using (cmd = new SQLiteCommand("insert into items (item_name) values('" + _item_name + "')", connect))
                {
                    cmd.ExecuteNonQuery();
                }
             
            }
            //MainForm.trans.Commit();
        }

        public void Update()
        {
            if (_item_image.Trim() != "")
            {
                byte[] byt = File.ReadAllBytes(_item_image);

                using (cmd = new SQLiteCommand("update items set item_image = @image where item_name = '" + _item_name + "'", connect))
                {
                    cmd.Parameters.AddWithValue("@image", byt);
                    cmd.ExecuteNonQuery();
                }
            }
            else
            {
                using (cmd = new SQLiteCommand("update items set item_image = null where item_name = '" + _item_name + "'", connect))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete()
        {
            using (cmd = new SQLiteCommand("delete from items where item_name = '" + _item_name + "'", connect))
            {
                cmd.ExecuteNonQuery();
            }
        }

        public bool CheckItemExist()
        {
            using (cmd = new SQLiteCommand("select * from items where item_name = '" + _item_name + "'", connect))
            {
                read = cmd.ExecuteReader();
                bool check = read.HasRows;
                read.Close();

                return check;
            }
            
        }

    }
}
