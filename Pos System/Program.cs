using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Protection;
using System.IO;
using System.Security.AccessControl;
using System.Data.SQLite;

namespace Pos_System
{
   public static class Program
    {
        public static string trialfile = @"C:\Windows\System32\SystemDoscmd.dll";//Trial File Path
        public static int trialtime = 1000;//Trial Time &&  Day Time = 86400 
        public static bool IsActive = false;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]

        static void Main()
        {


            GiveDatabasePermission();


            if (Protecter.CheckDevice())//Device Suporrted
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                //Check If this Device Has Bought  Active Serial 
                if (Protecter.IsSerialActive())
                {
                    IsActive = true;
                    Application.Run(new MainForm());
                }

                else
                {
                    ///Trial Checker
                    if (File.Exists(trialfile))
                    {
                        //Trial Time File Exist
                        int count = int.Parse(File.ReadAllText(trialfile));
                        if (count >= trialtime)
                        {
                            //Trail Time Expaired
                            Application.Run(new Active());

                        }
                        else
                        {

                            //Trail Time Not Expaired
                            Application.Run(new MainForm());

                        }

                    }
                    else
                    {
                        //Trial Time File Not Exist

                        File.WriteAllText(trialfile, "1");
                        Application.Run(new MainForm());

                    }
                }

                

            }
            else
                MessageBox.Show("عفوا جهازك غير مرخص", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error, 0, MessageBoxOptions.RtlReading);
        }

        static void GiveDatabasePermission()
        {
            FileSystemAccessRule role = new FileSystemAccessRule(Environment.UserName,FileSystemRights.FullControl,AccessControlType.Allow);
         
            FileSecurity secuirty1 = new FileSecurity("database/posdata.db", AccessControlSections.Access);
            secuirty1.SetAccessRule(role);
            //FileSecurity secuirty2 = new FileSecurity("database/posdata_log.ldf", AccessControlSections.Access);
            //secuirty2.SetAccessRule(role);


            File.SetAccessControl("database/posdata.db", secuirty1);
            //File.SetAccessControl("database/posdata_log.ldf", secuirty2);
        }
    }
}
