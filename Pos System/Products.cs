using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Data.SQLite;


namespace Pos_System
{
   public class Products
    {
        protected string _product_name;
        protected string _product_price;
        protected string _product_image;
        protected string _item_name;
        protected SQLiteConnection connect;
        protected SQLiteCommand cmd;
        protected SQLiteDataReader read;

        public string product_name
        {
            get { return _product_name; }
            set { _product_name = value; }
        }

        public string product_price
        {
            get { return _product_price; }
            set { _product_price = value; }
        }

        /// <summary>
        /// Path Of the product Image
        /// </summary>
        public string product_image
        {
            get { return _product_image; }
            set { _product_image = value; }
        }

        public string item_name
        {
            get { return _item_name; }
            set { _item_name = value; }
        }

        public Products(SQLiteConnection connect)
        {
            this.connect = connect;
        }

        public void Insert()
        {
            //MainForm.trans = connect.BeginTransaction();

            if (_product_image.Trim() != "")
            {
                byte[] byt = File.ReadAllBytes(_product_image);
                using (cmd = new SQLiteCommand("insert into products values('" + _product_name + "','" + _product_price + "',@image,'" + _item_name + "')", connect))
                {
                    cmd.Parameters.AddWithValue("@image", byt);
                    cmd.ExecuteNonQuery();
                }
            }
            else
            {


                using (cmd = new SQLiteCommand("insert into products (product_name,product_price,item_name) values('" + _product_name + "','" + _product_price + "','" + _item_name + "')", connect))
                {
                    cmd.ExecuteNonQuery();
                }
                
            }
            //MainForm.trans.Commit();
        }


        public void Update()
        {
            if (_product_image.Trim() != "")
            {
                byte[] byt = File.ReadAllBytes(_product_image);

                using (cmd = new SQLiteCommand("update products set product_price = '" + _product_price + "', product_image = @image , item_name = '" + _item_name + "' where product_name = '" + _product_name + "'", connect))
                {
                    cmd.Parameters.AddWithValue("@image", byt);
                    cmd.ExecuteNonQuery();
                }
            }
            else
            {
                using (cmd = new SQLiteCommand("update products set product_price = '" + _product_price + "',product_image = null, item_name = '" + _item_name + "' where product_name = '" + _product_name + "'", connect))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete()
        {
            using (cmd = new SQLiteCommand("delete from products where product_name = '" + _product_name + "'", connect))
            {
                cmd.ExecuteNonQuery();
            }
        }

        public bool CheckItemExist()
        {
            using (cmd = new SQLiteCommand("select * from products where product_name = '" + _product_name + "'", connect))
            {
                read = cmd.ExecuteReader();
                bool check = read.HasRows;
                read.Close();

                return check;
            }
        }
    }
}
