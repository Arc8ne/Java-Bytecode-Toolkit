using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Java_Bytecode_Toolkit
{
    public class Logger
    {
        public bool isEnabled = false;

        private void WriteDateAndTimeBeforeLogMessage()
        {
            this.Write(
                "[" + DateTime.Now.Day + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year + " " + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second + "] ",
                false
            );
        }

        public Logger()
        {
            if (Directory.Exists(App.Current.LOGS_DIR_FILE_PATH) == false)
            {
                Directory.CreateDirectory(App.Current.LOGS_DIR_FILE_PATH);
            }

            if (File.Exists(App.Current.LOG_FILE_PATH) == false)
            {
                File.Create(App.Current.LOG_FILE_PATH).Close();
            }

            this.DeleteAllLogs();
        }

        public void Write(string text, bool logDateAndTime = true)
        {
            if (this.isEnabled == false)
            {
                return;
            }

            if (logDateAndTime == true)
            {
                this.WriteDateAndTimeBeforeLogMessage();
            }

            File.AppendAllText(App.Current.LOG_FILE_PATH, text);
        }

        public void WriteLine(string text, bool logDateAndTime = true)
        {
            if (this.isEnabled == false)
            {
                return;
            }

            if (logDateAndTime == true)
            {
                this.WriteDateAndTimeBeforeLogMessage();
            }

            File.AppendAllText(App.Current.LOG_FILE_PATH, text);
        }

        public void DeleteAllLogs()
        {
            File.WriteAllText(App.Current.LOG_FILE_PATH, "");
        }
    }
}
