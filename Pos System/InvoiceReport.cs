using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data;
using System.Data.OleDb;
using Microsoft.Reporting.WinForms;
using System.Data.SQLite;
using System.Drawing.Printing;

namespace Pos_System
{
    public partial class InvoiceReport : Form
    {
        public InvoiceReport()
        {
            InitializeComponent();
        }


        private void InvoiceReport_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'posdataDataSet.invoice' table. You can move, or remove it, as needed.
            //this.invoiceTableAdapter.Fill(this.posdataDataSet.invoice);



            Invoice inv = new Invoice(MainForm.connect);

            if (MainForm.invoice_number.Trim() == "")
                MainForm.invoice_number = "0";

            ReportParameter[] parm = new ReportParameter[7];
            parm[0] = new ReportParameter("res_name", MainForm.shop_name);
            parm[1] = new ReportParameter("address", MainForm.shop_address);
            parm[2] = new ReportParameter("phone",MainForm.shop_phone);
            parm[3] = new ReportParameter("phone2", MainForm.shop_phone2);
            parm[4] = new ReportParameter("invoice_number", MainForm.invoice_number);
            parm[5] = new ReportParameter("total_price", inv.GetInvoiceTotal(int.Parse(MainForm.invoice_number)).ToString());
            parm[6] = new ReportParameter("invoice_extract_date", MainForm.invoice_extract_date);

            reportViewer1.LocalReport.SetParameters(parm);

            this.reportViewer1.RefreshReport();
            



        }

        private void reportViewer1_RenderingComplete(object sender, RenderingCompleteEventArgs e)
        {
            reportViewer1.PrintDialog();
            this.Close();

        }
    }
}
