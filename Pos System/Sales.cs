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
    public class Sales
    {

        protected SQLiteConnection connect;
        protected SQLiteCommand cmd;
        protected SQLiteDataReader read;

        public int invoice_number
        {
            get;
            set;
        }
        
        public string sold_products
        {
            get;
            set;
        }
        public double total_price
        {
            get;set;
        }
        public string extract_date
        {
            get;set;
        }

        public string  user_name
        {
            get;set;
        }


        public Sales(SQLiteConnection connect)
        {

            this.connect = connect;
        }

        public void Insert()
        {
            //MainForm.trans = connect.BeginTransaction();
            using (cmd = new SQLiteCommand("insert into sales values(" + invoice_number + ",'" + sold_products + "'," + total_price + ",'" + extract_date + "','"+user_name+"')", connect))
            {
                cmd.ExecuteNonQuery();
            }
            //MainForm.trans.Commit();
        }

        public string GetFullTotalByDate(string date)
        {
            using (cmd = new SQLiteCommand("select sum(total_price) from sales where extract_date like '%" + date + "%'", connect))
            {
                read = cmd.ExecuteReader();
                read.Read();
                double total = 0;
                if (!read.IsDBNull(0))
                 total = double.Parse(read[0].ToString());
                read.Close();

                return total.ToString();
            }
        }

        public string GetAllTotal()
        {

            using (cmd = new SQLiteCommand("select sum(total_price) from sales", connect))
            {
                read = cmd.ExecuteReader();
                read.Read();
                double total = 0;
                if (!read.IsDBNull(0))
                    total = double.Parse(read[0].ToString());
                read.Close();
                return total.ToString();
            }
        }

        public double GetAllProfits()
        {

            using (SQLiteCommand cmd1 = new SQLiteCommand("select sum(total_price) from sales", connect))
            using (SQLiteCommand cmd2 = new SQLiteCommand("select sum(purchase_price) from purchases", connect))
            {

               SQLiteDataReader read1 = cmd1.ExecuteReader();
               SQLiteDataReader read2 = cmd2.ExecuteReader();
                read1.Read();
                read2.Read();
                double total = 0;
                double purchase = 0;
                double all = 0;
                if (!read2.IsDBNull(0))
                    purchase = double.Parse(read2[0].ToString());
                if (!read1.IsDBNull(0))
                    total = double.Parse(read1[0].ToString());

                all = total - purchase;
                read1.Close();
                read2.Close();

                return all;
            }
        }

        public double GetProfitsByDate(string date)
        {
            using (SQLiteCommand cmd1 = new SQLiteCommand("select sum(total_price) from sales where extract_date like '%" + date + "%'", connect))
            using (SQLiteCommand cmd2 = new SQLiteCommand("select sum(purchase_price) from purchases ", connect))
            {
                SQLiteDataReader read1 = cmd1.ExecuteReader();
                SQLiteDataReader read2 = cmd2.ExecuteReader();
                read1.Read();
                read2.Read();
                double total = 0;
                double purchase = 0;
                double all = 0;
                if (read2.HasRows)
                    if (!read2.IsDBNull(0))
                        purchase = double.Parse(read2[0] + "");

                if (read1.HasRows)
                    if (!read1.IsDBNull(0))
                        total = double.Parse(read1[0] + "");

                all = total - purchase;
                read1.Close();
                read2.Close();

                return all;
            }
        }

        public double GetMonthlyProfits(string date)
        {
            using (SQLiteCommand cmd1 = new SQLiteCommand("select sum(total_price) from sales where extract_date like '%" + date + "%'", connect))
            using (SQLiteCommand cmd2 = new SQLiteCommand("select sum(purchase_price) from purchases", connect))
            {
                SQLiteDataReader read1 = cmd1.ExecuteReader();
                SQLiteDataReader read2 = cmd2.ExecuteReader();
                read1.Read();
                read2.Read();
                double total = 0;
                double purchase = 0;
                double all = 0;
                if (!read2.IsDBNull(0))
                    purchase = double.Parse(read2[0].ToString());
                if (!read1.IsDBNull(0))
                    total = double.Parse(read1[0].ToString());

                all = total - purchase;
                read1.Close();
                read2.Close();

                return all;
            }
        }


        public int GetLastInvoiceNumber()
        {
            using (cmd = new SQLiteCommand("select max(invoice_number) from sales", connect))
            {
                int last = 0;

                read = cmd.ExecuteReader();
                read.Read();

                if (!read.IsDBNull(0)) 
                 last = int.Parse(read[0].ToString());

                read.Close();
                return last;
            }
        }

        public int GetAllSalesCount()
        {
            using (cmd = new SQLiteCommand("select count(invoice_number) from sales", connect))
            {
                read = cmd.ExecuteReader();
                read.Read();
                int count = int.Parse(read[0].ToString());
                read.Close();
                return count;
            }
        }



    }
}
