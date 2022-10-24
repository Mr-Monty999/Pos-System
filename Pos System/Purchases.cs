using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Data.SQLite;

namespace Pos_System
{
  public class Purchases
    {

        protected SQLiteCommand cmd;
        protected SQLiteConnection connect;
        protected SQLiteDataReader read;

        public string purchase_name
        { get; set; }

        public string purchase_amount
        { get; set; }

        public double purchase_price
        { get; set; }

        public string purchase_date
        { get; set; }

        public Purchases(SQLiteConnection connect)
        {
            this.connect = connect;
        }

        public double GetTotalPurchase()
        {
            using (cmd = new SQLiteCommand("select sum([purchase_price]) from [purchases] ", connect))
            {
                read = cmd.ExecuteReader();
                read.Read();
                double total = 0;
                if (read.HasRows)
                    if(!read.IsDBNull(0))
                 total = double.Parse(read[0].ToString());

                read.Close();
                return total;
            }
        }
        public void AddData()
        {
            //MainForm.trans = connect.BeginTransaction();
            using (cmd = new SQLiteCommand("insert into [purchases] values(@name,@amount,@price,@date)",connect))
            {
                cmd.Parameters.AddWithValue("@name",purchase_name);
                cmd.Parameters.AddWithValue("@amount", purchase_amount);
                cmd.Parameters.AddWithValue("@price", purchase_price);
                cmd.Parameters.AddWithValue("@date", purchase_date);
                cmd.ExecuteNonQuery();
            }
            //MainForm.trans.Commit();

        }

        public void EditByName()
        {
            using (cmd = new SQLiteCommand("update [purchases] set [purchase_name] ='"+purchase_name+"',[purchase_amount]='" + purchase_amount + "',[purchase_price] = "+ purchase_price + ", [purchase_date] = '"+ purchase_date + "' where [purchase_name] = '"+purchase_name+"'", connect))
            {
                //cmd.Parameters.AddWithValue("@name", purchase_name);
                //cmd.Parameters.AddWithValue("@amount", purchase_amount);
                //cmd.Parameters.AddWithValue("@price", purchase_price);
                //cmd.Parameters.AddWithValue("@date", purchase_date);
                cmd.ExecuteNonQuery();
            }

        }

        public void DeleteByName()
        {
            using (cmd = new SQLiteCommand("delete from [purchases] where [purchase_name] = @name", connect))
            {
                cmd.Parameters.AddWithValue("@name", purchase_name);
                cmd.ExecuteNonQuery();
            }
        }

        public bool PurchaseNameExist()
        {
            using (cmd = new SQLiteCommand("select * from [purchases] where [purchase_name] = @name", connect))
            {
                cmd.Parameters.AddWithValue("@name", purchase_name);
                read = cmd.ExecuteReader();
                read.Read();
                bool exist = read.HasRows;
                read.Close();
                return exist;
            }
        }


    }
}
