using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uAuth2
{
    class Logger
    {
        string _file_path = null;
        string _app_name = null;

        public Logger(string filePath, string appName)
        {
            _file_path = filePath;
            _app_name = appName;
        }


        private void writeLog(Exception e)
        {
            DateTime dt = DateTime.Now;
            string filePath = _file_path + _app_name + ".log";

            using (StreamWriter ErrorStreamWriter = new StreamWriter(new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.Write)))
            {
                string StrToWrite = string.Format("Date: {0}\r\nError Number: {1}\r\nError Message: {2}\r\n", dt, e.HResult, e.Message);
                ErrorStreamWriter.WriteLine(StrToWrite);
            }
        }
    }
}
