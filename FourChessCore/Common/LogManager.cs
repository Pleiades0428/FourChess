using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FourChessCore
{
    public class LogManager
    {
        static bool doLog = false;

        public static void WriteLog(string msg)
        {
            if (doLog)
            {
                File.AppendAllText("log.txt", msg);
            }
        }
    }
}
