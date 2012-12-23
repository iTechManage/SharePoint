using System;
using System.Collections.Generic;

namespace CCSAdvancedAlerts
{
    /// <summary>
    /// This is static class for cache all the alerts insted querying every time for get all alerts
    /// </summary>
    static class AlertsCache
    {

        private static Dictionary<string, Dictionary<int, Alert>> bufferedAlerts = new Dictionary<string, Dictionary<int, Alert>>();
        private static Dictionary<string, DateTime> bufferedLastExecution = new Dictionary<string, DateTime>();

        /// <summary>
        /// return all buffered alerts for the site collection
        /// </summary>
        /// <param name="siteUrl"></param>
        /// <returns></returns>
        internal static Dictionary<int, Alert> GetAlertsForSiteCollection(string siteUrl)
        {
            try
            {
                foreach (string key in bufferedAlerts.Keys)
                {
                    if (key.Equals(siteUrl))
                    {
                        return bufferedAlerts[key];
                    }
                }
                //if (bufferedAlerts.ContainsKey(siteUrl))
                //{
                //    return bufferedAlerts[siteUrl];
                //}
            }
            catch { }
            return new Dictionary<int, Alert>();
        }

        /// <summary>
        /// Get the last execution time for timer job
        /// </summary>
        /// <param name="JobName"></param>
        /// <returns></returns>
        internal static DateTime GetLastExecutionTimeForTimerJob(string JobName)
        {
            try
            {
                if (bufferedLastExecution.ContainsKey(JobName))
                {
                    return bufferedLastExecution[JobName];
                }
            }
            catch { }
            return DateTime.MinValue;
        }


        /// <summary>
        /// Update last runtime of timer job once execution is started or completed
        /// </summary>
        /// <param name="jobName"></param>
        /// <param name="lastRunTime"></param>
        internal static void UpdateLastExecutionTime(string jobName,DateTime lastRunTime )
        {
            try
            {
                if (bufferedLastExecution.ContainsKey(jobName))
                {
                    bufferedLastExecution[jobName] = lastRunTime;
                }
            }
            catch { }
        }



        /// <summary>
        /// update the buffered alerts to latest if any modifications are done
        /// </summary>
        /// <param name="siteUrl"></param>
        internal static void SynchroniseAlerts(string siteUrl,string jobName)
        {
            try
            {
               //Create alert manager object
                AlertManager aManager = new AlertManager(siteUrl);
                if (!bufferedAlerts.ContainsKey(siteUrl))
                {
                    //Directly add all Alerts
                    bufferedAlerts.Add(siteUrl, aManager.GetAllAlerts());
                }
                else
                {
                    //update old alerts
                    DateTime lastExecutionDate = GetLastExecutionTimeForTimerJob(jobName);
                    Dictionary<int, Alert> modifiedAlerts = aManager.GetAlertsChangesSince(lastExecutionDate);
                    Dictionary<int, Alert> AllAlerts = bufferedAlerts[siteUrl];
                    foreach (int id in modifiedAlerts.Keys)
                    {
                        if (AllAlerts.ContainsKey(id))
                        {
                            if (modifiedAlerts[id] != null)
                            {
                                //Alert is updated
                                AllAlerts[id] = modifiedAlerts[id];
                            }
                            else
                            {
                                //Remove alert it has been deleted from the alert settings
                                AllAlerts.Remove(id);
                            }
                            continue;
                        }
                        if (modifiedAlerts[id] != null)
                        {
                            // New alert is added
                            AllAlerts.Add(id, modifiedAlerts[id]);
                        }
                     }
                }
            }
            catch { }
        }

    }

}
