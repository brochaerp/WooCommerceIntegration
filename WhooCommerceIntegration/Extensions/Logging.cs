using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extensions
{
    public static class DebugLogger
    {
        public static ILogProvider LogProvider { get; set; }

        public static string DefaultGroupHeader { get; set; }
        public static bool LogToFile { get; set; }

        const string mFileName = "WooComServiceLog";
 
        private static string logFilePath;
        public static string LogFilePath
        {
            get
            {
                if (string.IsNullOrEmpty(logFilePath) || !File.Exists(logFilePath))
                {
                    string assemblyFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

                    Directory.CreateDirectory(Path.Combine(assemblyFolder, "WooCom Logs"));

                    logFilePath = string.Format(@"WooCom Logs\{0}_{1:yyyy'-'MM'-'dd}.txt", mFileName, DateTime.Today);
                    logFilePath = Path.Combine(assemblyFolder, logFilePath);
                }

                return logFilePath;
            }
            set
            {
                logFilePath = value;
            }
        }

        public static void WriteLine(MessageSeverity severity, string message, params object[] args)
        {
            string fullMessage = string.Format(message, args);

            if (LogProvider == null || LogToFile)
                fullMessage = string.Format("{0}: [{1}]\t{2}{3}", DateTime.Now, severity, fullMessage, Environment.NewLine);

            Write(fullMessage, severity);
        }

        public static void Write(string message, MessageSeverity severity, string logGroupHeader = null)
        {
            try
            {
                Debug.WriteLine(message);

                if (LogProvider == null || LogToFile)
                    WriteToFile(message);
                else
                {
                    ServiceLog log = new ServiceLog()
                    {
                        MessageText = message,
                        ProfileSettingId = LogProvider.ProfileSettingId,
                        LogGroup = logGroupHeader ?? DefaultGroupHeader
                    };
                    log.Category = (byte)severity;

                    LogProvider.InsertServiceLog(log);
                }
            }
            catch (Exception ex)
            {
                string logLocation = (LogProvider == null) ? LogFilePath : LogProvider.GetType().Name;

                var error = new Exception(string.Format("Error writing [{0}] log to \"{1}\" !", message, logLocation), ex);

                WriteToFile(error);
            }
        }

        #region Log To File

        public static void WriteLineToFile(string message, params object[] args)
        {
            string fullMessage = string.Format(message, args);
            string txtToWrite = string.Format("{0}:\t{1}{2}", DateTime.Now, fullMessage, Environment.NewLine);

            WriteToFile(txtToWrite);
        }

        public static void WriteToFile(string message)
        {
            try
            {
#if DEBUG
                Debug.Write(message);
#else
                File.AppendAllText(LogFilePath, message);
#endif
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error writing [{0}] log to \"{1}\" !", message, LogFilePath), ex);
            }
        }

        public static void WriteToFile(Exception ex)
        {
            WriteLineToFile(ex.ToString());
            WriteToFile(new string('-', 20) + Environment.NewLine);
        }
         
        public static void Log(String lines)
        {

            // Write the string to a file.append mode is enabled so that the log
            // lines get appended to  test.txt than wiping content and writing the log
            if (Debugger.IsAttached)
                Debug.WriteLine(lines);
            else
            {
                //if (LogFilePath.IsNullOrEmpty())
                //    return;

                System.IO.StreamWriter file = new System.IO.StreamWriter(LogFilePath, true);
                file.WriteLine(string.Format("{0}\t\t{1}", DateTime.Now, lines));

                file.Close();
            }

        }

        public static void Log(String lines, string caption)
        {

            // Write the string to a file.append mode is enabled so that the log
            // lines get appended to  test.txt than wiping content and writing the log
            if (Debugger.IsAttached)
                Debug.WriteLine(lines);
            else
            {
                //if (LogFilePath.IsNullOrEmpty())
                //    return;


                System.IO.StreamWriter file = new System.IO.StreamWriter(LogFilePath, true);
                file.WriteLine(string.Format("{0}\t[{1}]\t{2}", DateTime.Now, caption, lines));

                file.Close();
            }
        }

        #endregion


    }
}
