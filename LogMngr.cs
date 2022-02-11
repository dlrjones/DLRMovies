using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Threading;

namespace DLRMovies
{
    class LogMngr
    {
        #region class variables
        private static NameValueCollection ConfigData = null;
        private static string logFilePath = "";
        private static string logFile = "";
        private static string corp = "";
        private static string attachmentPath = "";
        private bool debug = false;
        private ArrayList outgoingData;
        private string TAB = "        ";             //Convert.ToChar(9);
        private static LogMngr outMngr = null;

        public ArrayList OutgoingData
        {
            set { outgoingData = value; }
        }
        public bool Debug
        {
            set { debug = value; }
        }
        public string LogFilePath
        {
            set { logFilePath = value; }
        }
        public string AttachmentPath
        {
            get {return attachmentPath;}
            set {attachmentPath = value; }
        }
        #endregion

        private LogMngr()
        {
            // this constructor is private to force the calling program to use GetInstance()
           // use it as necessary
        }

     
        public static LogMngr GetInstance()
        {
            if (outMngr == null)
            {
                CreateInstance();
            }
            return outMngr;
        }

        private static void CreateInstance()
        {
            Mutex configMutex = new Mutex();
            configMutex.WaitOne();
            outMngr = new LogMngr();
            configMutex.ReleaseMutex();
        }

        private static void InitLogMngr()
        {  //this is called when a class wants to write to the log without first having to go through ConfigData to get  
            //and pass in the log file name and path. It DOES require that an app.config file exists with the 
            //proper keys, though
            ConfigData = (NameValueCollection)ConfigurationManager.GetSection("MovieCollection");
            logFilePath = ConfigData.Get("logFilePath") + ConfigData.Get("logFile");
        }

        public void Write()
        {
            string writeText = "";

            //this next "if" allows a class to write to the log without first having to go through ConfigData to get  
            //and pass in the log file name and path. It DOES require that an app.config file exists with the 
            //proper keys, though
            if (logFilePath.Length == 0)
                InitLogMngr();
            try
            {
                if(debug)
                    LogEntry("OutputMngr/Write:  " + "OutputMngr.Write()");
                foreach (string item in outgoingData)
                {
                    writeText += item.Trim() + TAB;
                }
                LogEntry(writeText);
            }
            catch (Exception ex)
            {
                LogEntry("OutputMngr/Write:  " + ex.Message);
            }
        }

        public void LogEntry(string logText)
        {
           //this next "if" allows a class to write to the log without first having to go through ConfigData to get and 
            //and pass in the log file name and path. It DOES require that an app.config file exists with the 
            //proper keys, though
            if (logFilePath.Length == 0)
                InitLogMngr();

            if (!File.Exists(logFilePath))
                File.WriteAllText(logFilePath, "ExpiredPasswordNotification ERROR LOG" + Environment.NewLine);
            if (logText.Length > 0)
                File.AppendAllText(logFilePath, DateTime.Now + TAB.ToString() + logText + Environment.NewLine);
        }
    }
}
