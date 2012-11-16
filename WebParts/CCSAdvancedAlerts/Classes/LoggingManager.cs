using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.Threading;
using Microsoft.Win32;

namespace CCSAdvancedAlerts
{
    public class Utils
    {
        static ILogging logger;

        public static ILogging LogManager
        {
            get
            {
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
        private string Location = null;
        private bool logValid = false;
        private bool bInitialized = false;
        private string TraceLevel = "error";
        private DateTime FixedDate = DateTime.Now;

        public bool IsInitialized()
        {

            return bInitialized;
        }

        public void Init()
        {
            try
            {
                bInitialized = true;

            }
            catch (System.Exception ex)
            {

            }
        }

        public void Configure()
        {
            try
            {
                //RegistryKey MainKey;
                //if (ProcessingPage.Is64BitOs())
                //{
                //    MainKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Crow Canyon Systems\Help Desk\Setup");
                //}
                //else
                //{
                //    MainKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Crow Canyon Systems\Help Desk\Setup");
                //}
                ////RegistryKey MainKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Crow Canyon Systems\Help Desk\Setup");
                //string DirectoryLocation = MainKey.GetValue("TraceLog Folder Path").ToString().Trim();
                //TraceLevel = MainKey.GetValue("TraceLog Level (Information/Error)").ToString().Trim().ToLower();
                //if (!((string)MainKey.GetValue("EnableTraceLog")).Trim().Equals("true", StringComparison.InvariantCultureIgnoreCase))
                //{
                //    logValid = false;
                //    return;
                //}

                //If Dir Location is not present
                string DirectoryLocation = string.Empty;
                if (DirectoryLocation == "")
                {
                    DirectoryLocation = GetAssemblyLocation();
                }

                if (DirectoryLocation.LastIndexOf('\\') == (DirectoryLocation.Length - 1) || DirectoryLocation.LastIndexOf('/') == (DirectoryLocation.Length - 1))
                {
                    DirectoryLocation = DirectoryLocation.Substring(0, DirectoryLocation.Length - 1);
                }
                String FileLocation = DirectoryLocation + "\\CCSAdvancedAlertsLog.txt";


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
                catch (System.Exception ex)
                {
                    logValid = false;
                }
            }
            catch (Exception ex)
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
                    lock (this)
                    {
                        //if (traceLevel.ToLower() == TraceLevel.ToLower() || traceLevel.ToLower() == "error")
                        {
                            if (traceLevel.ToLower() == "error")
                                File.WriteAllText(Location, File.ReadAllText(Location) + "\r\n{ThreadID:: " + Thread.CurrentThread.ManagedThreadId + " [" + DateTime.Now + "]} [ERROR] " + logText);
                            else
                                File.WriteAllText(Location, File.ReadAllText(Location) + "\r\n{ThreadID:: " + Thread.CurrentThread.ManagedThreadId + " [" + DateTime.Now + "]} [INFO] " + logText);
                        }
                    }
                }
                catch (System.Exception ex)
                {

                }
            }
        }

        public void write(string logText)
        {
            write(logText, "information");
        }

        private string GetAssemblyLocation()
        {
            string CodeBase = Assembly.GetExecutingAssembly().Location;
            if (CodeBase.Contains("\\"))
            {
                CodeBase = CodeBase.Substring(0, CodeBase.LastIndexOf("\\"));
            }
            return CodeBase;
        }
    }
}
