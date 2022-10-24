using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;
using System.Data.SQLite;

namespace Pos_System
{
   public class Settings
    {
        protected SQLiteConnection connect;
        protected SQLiteCommand cmd;
        protected SQLiteDataReader read;

        public Settings(SQLiteConnection connect)
        {
            this.connect = connect;
        }
        public void DeleteInvoicesAndSales()
        {
            using (cmd = new SQLiteCommand("delete from sales", connect))
            {
                cmd.ExecuteNonQuery();
                cmd = cmd = new SQLiteCommand("delete from invoice", connect);
                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteItems()
        {
            using (cmd = new SQLiteCommand("delete from items", connect))
            {
                cmd.ExecuteNonQuery();

            }
        }

        public void DeleteProducts()
        {
            using (cmd = new SQLiteCommand("delete from products", connect))
            {
                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteRestaurantInfo()
        {
            using (cmd = new SQLiteCommand("delete from shop_info", connect))
            {
                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteWallet()
        {
            using (cmd = new SQLiteCommand("delete from wallet", connect))
            {
                cmd.ExecuteNonQuery();
            }
        }

        public void DeletePurchases()
        {
            using (cmd = new SQLiteCommand("delete from purchases", connect))
            {
                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteAllDataAndReset()
        {
            using (cmd = new SQLiteCommand("delete from shop_info", connect))
            {
                cmd.ExecuteNonQuery();

                cmd = new SQLiteCommand("delete from invoice", connect);
                cmd.ExecuteNonQuery();
                cmd = cmd = new SQLiteCommand("delete from items", connect);
                cmd.ExecuteNonQuery();
                cmd = cmd = new SQLiteCommand("delete from sales", connect);
                cmd.ExecuteNonQuery();
                cmd = cmd = new SQLiteCommand("delete from products", connect);
                cmd.ExecuteNonQuery();
                cmd = cmd = new SQLiteCommand("delete from invoice", connect);
                cmd.ExecuteNonQuery();
                cmd = cmd = new SQLiteCommand(" delete from login", connect);
                cmd.ExecuteNonQuery();
                cmd = cmd = new SQLiteCommand(" delete from wallet", connect);
                cmd.ExecuteNonQuery();
                cmd = cmd = new SQLiteCommand(" delete from purchases", connect);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
