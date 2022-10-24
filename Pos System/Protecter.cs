using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Protection;
using System.IO;

namespace Pos_System
{
    public class Protecter
    {
        
        public static bool CheckDevice()
        {
            
            Serial check = new Serial();
            string serial = check.GetHardDriveSerial() + check.GetProcesserSerial();
            Dictionary<string, string> serials = new Dictionary<string, string>();
            serials.Add("54L5SKP6SBFEBFBFF000306A9","My");
            serials.Add("50026B727800D855BFEBFBFF000506C9","محمد المعز");
            serials.Add("WD-WXU1E13SWLU6BFEBFBFF000406E3", "Ahmed Osman");
            serials.Add("RBF50A151S251PBFEBFBFF000306D4", "Wad Ahmed الترابي");
            serials.Add("46Q7P1RLTBFEBFBFF000206A7", "من قروب المبرمجين");
            serials.Add("5636414442324c52202020202020202020202020BFEBFBFF00020655", "قودزيلا");

            bool SerialExist = serials.ContainsKey(serial);
            return SerialExist;
        }

        public static bool IsSerialActive()
        {
            Serial check = new Serial();
            string fullserial = check.GetHardDriveSerial() + check.GetProcesserSerial();
            Dictionary<string, string> activeserials = new Dictionary<string, string>();
            activeserials.Add("54L5SKP6SBFEBFBFF000306A9", "My");
            activeserials.Add("46Q7P1RLTBFEBFBFF000206A7", "من قروب المبرمجين");
            activeserials.Add("RBF50A151S251PBFEBFBFF000306D4", "Wad Ahmed الترابي");
            activeserials.Add("5636414442324c52202020202020202020202020BFEBFBFF00020655", "قودزيلا");


            bool equal = activeserials.ContainsKey(fullserial);
            return equal;

        }

       
    }
}
