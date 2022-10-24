using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Data;
using System.Data.SQLite;


namespace Pos_System
{
   public class Wallet
    {
        protected SQLiteCommand cmd;
        protected SQLiteConnection connect;
        protected SQLiteDataReader read;

        public string user_name
        { get; set; }

        public double cash
        { get; set; }

        public string process_type
        { get; set; }

        public string process_date
        { get; set; }

        public Wallet(SQLiteConnection connect)
        {
            this.connect = connect;
        }

        public void Add()
        {
           //MainForm.trans = connect.BeginTransaction();
            using (cmd = new SQLiteCommand("insert into [wallet] ([user_name],[cash],[process_type],[process_date]) values('"+ user_name + "',"+ cash + ",'"+ process_type + "','"+process_date+"')", connect))
            {
                //cmd.Parameters.AddWithValue("@user",user_name);
                //cmd.Parameters.AddWithValue("@cash", cash);
                //cmd.Parameters.AddWithValue("@process_type", process_type);
                //cmd.Parameters.AddWithValue("@process_date", process_date);
                cmd.ExecuteNonQuery();
            }
            //MainForm.trans.Commit();
        }

        public double GetAllAmount()
        {
            using (cmd = new SQLiteCommand("select sum(cash) from [wallet]", connect))
            {
                read = cmd.ExecuteReader();
                read.Read();

                if (!read.IsDBNull(0))
                {
                    double amount = double.Parse(read[0].ToString());
                    read.Close();

                    return amount;
                }
                else
                {
                    read.Close();
                    return 0;
                }
            }
        }

        public double GetAllAmountByDate(string date)
        {
            using (cmd = new SQLiteCommand("select sum(cash) from [wallet] where [process_date] like'%"+date+"%'", connect))
            {
                read = cmd.ExecuteReader();
                read.Read();

                if (!read.IsDBNull(0))
                {
                    double amount = double.Parse(read[0].ToString());
                    read.Close();

                    return amount;
                }
                else
                {
                    read.Close();
                    return 0;
                }
            }
        }
        public int GetAllWalletCount()
        {
            using (cmd = new SQLiteCommand("select count(*) from [wallet]", connect))
            {
                read = cmd.ExecuteReader();
                read.Read();
                int amount = 0;
                if (!read.IsDBNull(0))
                     amount = int.Parse(read[0].ToString());

                read.Close();

                return amount;
            }
        }

    }
}
