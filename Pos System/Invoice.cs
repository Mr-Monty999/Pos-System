using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Windows.Forms;
using System.Data.SQLite;

namespace Pos_System
{
   public class Invoice
    {


        protected int _invoice_number;
        protected string _product_name;
        protected int _product_amount;
        protected double _product_price;
        protected double _total_price;
        protected string _extract_date;
        protected SQLiteConnection connect;
        protected SQLiteCommand cmd;
        protected SQLiteDataReader read;


        public int invoice_number
        {
            get { return _invoice_number; }
            set { _invoice_number = value; }
        }

        public string product_name
        {
            get { return _product_name; }
            set { _product_name = value; }
        }

        public int product_amount
        {
            get { return _product_amount; }
            set { _product_amount = value; }
        }

        public double product_price
        {
            get { return _product_price; }
            set { _product_price = value; }
        }

        public double total_price
        {
            get { return _total_price; }
            set { _total_price = value; }
        }

        public string extract_date
        {
            get { return _extract_date; }
            set { _extract_date = value; }
        }

        public Invoice(SQLiteConnection connect)
        {
            this.connect = connect;
        }
        public int GetLastInvoiceNumber()
        {
            using (cmd = new SQLiteCommand("select max(invoice_number) from invoice", connect))
            {
                int invnumber = 0;
                read = cmd.ExecuteReader();
                read.Read();
                if (!read.IsDBNull(0))
                    invnumber = int.Parse(read[0].ToString());
                
                read.Close();
                return invnumber;
            }
        }
        public bool HasRows()
        {
            using (cmd = new SQLiteCommand("select invoice_number from invoice ", connect))
            {
                read = cmd.ExecuteReader();
                bool check = read.HasRows;
                read.Close();
                return check;
            }
            
        }
        public void Insert()
        {
            //MainForm.trans = connect.BeginTransaction();
            using (cmd = new SQLiteCommand("insert into invoice (invoice_number,product_name,product_amount,product_price,total_price,extract_date) values (" + _invoice_number + ",'" + _product_name + "'," + _product_amount + "" +
                "," + _product_price + "," + _total_price + ",'" + _extract_date + "')", connect))
            {
                cmd.ExecuteNonQuery();
            }
            //MainForm.trans.Commit();
        }

        public double GetInvoiceTotal(int InvoiceNumber)
        {
            using (cmd = new SQLiteCommand("select max(total_price) from [invoice] where [invoice_number] = @invnum", connect))
            {
                cmd.Parameters.AddWithValue("@invnum",InvoiceNumber);

                read = cmd.ExecuteReader();
                read.Read();
                double total = 0;
                if (!read.IsDBNull(0))
                 total = double.Parse(read[0].ToString());

                read.Close();

                return total;
            }
        }
    }
}
