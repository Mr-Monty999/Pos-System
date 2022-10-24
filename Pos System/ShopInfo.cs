using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Data.SQLite;

namespace Pos_System
{
    public class ShopInfo
    {
        protected SQLiteConnection connect;
        protected SQLiteCommand cmd;
        protected SQLiteDataReader read;
         
        public string shop_name
        { get; set; }

        public string shop_address
        { get; set; }

        public string shop_tellphone
        { get; set; }

        public string shop_tellphone2
        { get; set; }

        public string logo_path
        { get; set; }


        public ShopInfo(SQLiteConnection connect)
        {
            this.connect = connect;
        }

        public bool HasRows()
        {

            using (cmd = new SQLiteCommand("select * from shop_info", connect))
            {
                read = cmd.ExecuteReader();
                bool check = read.HasRows;
                read.Close();
                return check;
            }
        }

        public void Save()
        {
            //MainForm.trans = connect.BeginTransaction();
            if (!HasRows())
            {
                if (logo_path.Trim() != "")
                {
                    using (cmd = new SQLiteCommand("insert into shop_info values(@name,@address,@phone,@phone2,@image)", connect))
                    {
                        cmd.Parameters.AddWithValue("@name", shop_name);
                        cmd.Parameters.AddWithValue("@address", shop_address);
                        cmd.Parameters.AddWithValue("@phone", shop_tellphone);
                        cmd.Parameters.AddWithValue("@phone2", shop_tellphone2);
                        byte[] byt = File.ReadAllBytes(logo_path);
                        cmd.Parameters.AddWithValue("@image", byt);

                        cmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    using (cmd = new SQLiteCommand("insert into shop_info (shop_name,shop_address,shop_tellphone,shop_tellphone2) values(@name,@address,@phone,@phone2)", connect))
                    {
                        cmd.Parameters.AddWithValue("@name", shop_name);
                        cmd.Parameters.AddWithValue("@address", shop_address);
                        cmd.Parameters.AddWithValue("@phone", shop_tellphone);
                        cmd.Parameters.AddWithValue("@phone2", shop_tellphone2);


                        cmd.ExecuteNonQuery();
                    }
                }

            }
            else
            {
                if (logo_path.Trim() != "")
                {
                    using (cmd = new SQLiteCommand("update shop_info set shop_name = @name, shop_address = @address, shop_tellphone = @phone, shop_tellphone2 = @phone2, shop_logo = @image", connect))
                    {
                        cmd.Parameters.AddWithValue("@name", shop_name);
                        cmd.Parameters.AddWithValue("@address", shop_address);
                        cmd.Parameters.AddWithValue("@phone", shop_tellphone);
                        cmd.Parameters.AddWithValue("@phone2", shop_tellphone2);
                        byte[] byt = File.ReadAllBytes(logo_path);
                        cmd.Parameters.AddWithValue("@image", byt);

                        cmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    using (cmd = new SQLiteCommand("update shop_info set shop_name = @name, shop_address = @address, shop_tellphone = @phone, shop_tellphone2 = @phone2,shop_logo = null", connect))
                    {
                        cmd.Parameters.AddWithValue("@name", shop_name);
                        cmd.Parameters.AddWithValue("@address", shop_address);
                        cmd.Parameters.AddWithValue("@phone", shop_tellphone);
                        cmd.Parameters.AddWithValue("@phone2", shop_tellphone2);


                        cmd.ExecuteNonQuery();
                    }
                }

            }

            //MainForm.trans.Commit();
        }

       
        
    }
}
