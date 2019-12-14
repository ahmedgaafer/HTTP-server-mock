using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{
    class Logger
    {
        static StreamWriter sw;
        public static void LogException(Exception ex)
        {
            // TODO: Create log file named log.txt to log exception details in it
            //Datetime:
            //message:
            // for each exception write its details associated with datetime
            using (sw = new StreamWriter("log.txt", true))
            {
                sw.WriteLine($"[ {DateTime.Now.ToString()} ] EXEPTION: {ex.ToString()}");

            }
        }
    }
}
