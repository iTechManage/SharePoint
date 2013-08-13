using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.Threading;
using Microsoft.Win32;
using System.Diagnostics;

namespace CrowCanyon.CascadedLookup
{
    public class EnterExitLogger : IDisposable
    {
        string Message = "";
        public EnterExitLogger(string message)
        {
            Utils.LogManager = null;
            this.Message = (string.IsNullOrEmpty(message) ? "" : message);

            Utils.LogManager.write("Enter " +  Message);            
        }

        public void Dispose()
        {
            Utils.LogManager.write ("Exiting " +  Message);
            Utils.LogManager = null;
        }
    }

    public class Utils
    {
        static ILogging logger = null;

        public static ILogging LogManager
        {
            get
            {
                if (logger == null)
                {
                    logger = new LoggingManager();
                }

                return logger;
            }

            set
            {
                logger = value as ILogging;
            }
        }
    }

    public interface ILogging
    {
        void write(string logText);
        void write(string logText, string traceLevel);
    }

    public class LoggingManager : ILogging
    {
        const string LogFileName = "CCSCascadeLookupLogs";
        private string Location = null;
        private bool logValid = false;
        private bool bInitialized = false;
        private string TraceLevel = "error";
        private DateTime FixedDate = DateTime.Now;
        public bool IsInitialized()
        {

            return bInitialized;
        }

        public LoggingManager()
        {
            Init();
            Configure();
        }

        public void Init()
        {
            try
            {
                RegistryKey MainKey;
                MainKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Crow Canyon\Sharepoint\Custom Fields");
                if (MainKey == null)
                {
                    MainKey = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Crow Canyon\Sharepoint\Custom Fields", RegistryKeyPermissionCheck.ReadWriteSubTree);
                }

                if (MainKey.GetValue("EnableTraceLog", null) == null)
                {
                    MainKey.SetValue("EnableTraceLog", "true");
                }
        
                if (MainKey.GetValue("TraceLog Folder Path", null) == null)
                {
                    MainKey.SetValue("TraceLog Folder Path", this.GetFolderPath());
                }
                
                if (MainKey.GetValue("TraceLog Level (Information/Error)", null) == null)
                {
                    MainKey.SetValue("TraceLog Level (Information/Error)", "Error");
                }
                
                bInitialized = true;

            }
            catch (System.Exception)
            {

            }
        }


        public void Configure()
        {
            try
            {
                RegistryKey MainKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Crow Canyon\Sharepoint\Custom Fields");

                string DirectoryLocation = MainKey.GetValue("TraceLog Folder Path").ToString().Trim();
                TraceLevel = MainKey.GetValue("TraceLog Level (Information/Error)").ToString().Trim().ToLower();
                
                if (!((string)MainKey.GetValue("EnableTraceLog")).Trim().Equals("true", StringComparison.InvariantCultureIgnoreCase))
                {
                    logValid = false;
                    return;
                }

                if (string.IsNullOrEmpty(DirectoryLocation ))
                {
                    DirectoryLocation = this.GetFolderPath();
                }

                String FileLocation = Path.Combine(DirectoryLocation, LogFileName + ".txt"); // +FixedDate.ToString("-mm-dd-yyyy h-m-s") + ".txt";

                if (File.Exists(FileLocation))
                {
                    Location = FileLocation;
                    logValid = true;
                    return;
                }

                try
                {
                    lock (this)
                    {
                        if (!Directory.Exists(Location))
                        {
                            Directory.CreateDirectory(DirectoryLocation);
                        }
                        if (!File.Exists(FileLocation))
                        {
                            FileStream logFile = File.Create(FileLocation);
                            logFile.Close();
                        }
                        Location = FileLocation;
                        logValid = true;
                    }

                }
                catch (System.Exception)
                {
                    logValid = false;
                }
            }
            catch (System.Exception)
            {
                logValid = false;

            }
        }

        public void write(string logText, string traceLevel)
        {
            if (logValid)
            {
                try
                {
                    CheckFileSize();

                    lock (this)
                    {
                        if (traceLevel.ToLower() == TraceLevel.ToLower() || traceLevel.ToLower() == "error")
                        {
                            if (traceLevel.ToLower() == "error")
                            {
                                //File.WriteAllText(Location, File.ReadAllText(Location) + "\r\n{ThreadID:: " + Thread.CurrentThread.ManagedThreadId + " [" + DateTime.Now + "]} [ERROR] " + logText);
                                WriteLogs("{ThreadID:: " + Thread.CurrentThread.ManagedThreadId + " [" + DateTime.Now + "]} [ERROR] " + logText);
                            }
                            else
                            {
                                //File.WriteAllText(Location, File.ReadAllText(Location) + "\r\n{ThreadID:: " + Thread.CurrentThread.ManagedThreadId + " [" + DateTime.Now + "]} [INFO] " + logText);
                                WriteLogs("{ThreadID:: " + Thread.CurrentThread.ManagedThreadId + " [" + DateTime.Now + "]} [INFO] " + logText);
                            }
                        }
                    }
                }
                catch (System.Exception)
                {
                    if (traceLevel.ToLower() == "error")
                    {
                        try
                        {
                            string EventName = "CCSCascadedLookupLogs";
                            if (!EventLog.SourceExists(EventName))
                            {
                                EventLog.CreateEventSource(EventName, EventName);
                            }

                            EventLog MyLog = new EventLog();
                            MyLog.Source = EventName;
                            MyLog.WriteEntry("{ThreadID:: " + Thread.CurrentThread.ManagedThreadId + " [" + DateTime.Now + "]} [ERROR] " + logText, EventLogEntryType.Error);
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
        }

        void WriteLogs(string Message)
        {
            using (TextWriter tw = new StreamWriter(Location, true))
            {
                tw.WriteLine(Message);
                tw.Flush();
                tw.Close();
            }
        }

        public void write(string logText)
        {
            write(logText, "information");
        }

        private string GetFolderPath()
        {
            try
            {
                return Path.Combine(Path.GetPathRoot(Environment.SystemDirectory), "CrowCanyon Logs");
            }
            catch (Exception)
            {
            }

            return String.Empty;
        }

        private void CheckFileSize()
        {
            FileInfo fi = new FileInfo(Location);
            if (fi.Length > (1024 * 500))
            {
                GetUniqueFileName();
            }
        }

        void GetUniqueFileName()
        {
            int i =1;
            while (File.Exists(Location))
            {
                string tempLocation = Path.Combine(Path.GetDirectoryName(Location), LogFileName + "_" + i + ".txt");
                if (!File.Exists(tempLocation) && (new FileInfo(Location)).Length < (1024 * 500))
                {
                    break;
                }

                Location = tempLocation;
                i++;
            }

            try
            {
                lock (this)
                {
                    if (!File.Exists(Location))
                    {
                        FileStream logFile = File.Create(Location);
                        logFile.Close();
                    }

                    logValid = true;
                }

            }
            catch (System.Exception)
            {
                logValid = false;
            }
        }
    }
}
