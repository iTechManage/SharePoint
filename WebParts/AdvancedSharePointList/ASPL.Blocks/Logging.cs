using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.SharePoint.Administration;

namespace ASPL.Blocks
{
    public class Logging
    {
        const string loggingSource = "ASPL-Logs";
        const string log = "Application";

        public static void Log(Exception exp)
        {

            try
            {
                if (!EventLog.SourceExists(loggingSource))
                    EventLog.CreateEventSource(loggingSource, log);

                EventLog.WriteEntry(loggingSource, exp.ToString(), EventLogEntryType.Error);
            }
            catch
            {
                try { LogToULS(exp); }
                catch { }

            }
        }

        private static void LogToULS(Exception exp)
        {
            SPDiagnosticsService diagSvc = SPDiagnosticsService.Local; 

            diagSvc.WriteTrace( 0, 
                new SPDiagnosticsCategory(loggingSource,  TraceSeverity.Unexpected, EventSeverity.Error),
                    TraceSeverity.Unexpected,
                    exp.ToString()
                    );
        }
    }
}
