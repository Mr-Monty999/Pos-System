using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;
using System.Data.SQLite;

namespace Pos_System
{
    public partial class SalesReport : Form
    {
        public SalesReport()
        {
            InitializeComponent();
        }

        private void SalesReport_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'posdataDataSet.sales' table. You can move, or remove it, as needed.
            //this.salesTableAdapter.Fill(this.posdataDataSet.sales);

           

            this.reportViewer1.RefreshReport();
          
        }
    }
}
