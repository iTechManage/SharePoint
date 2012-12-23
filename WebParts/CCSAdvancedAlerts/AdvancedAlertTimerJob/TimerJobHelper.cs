using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SharePoint;

namespace CCSAdvancedAlerts
{
    class TimerJobHelper
    {
        /// <summary>
        /// it will group all the alerts for sub site so that we no need to create object for that site again and again
        /// </summary>
        /// <param name="siteAlerts"></param>
        internal static Dictionary<string, List<Alert>> GroupAlertsByWeb(Dictionary<int, Alert> siteAlerts)
        {
            Dictionary<string, List<Alert>> filteredAlerts = new Dictionary<string, List<Alert>>();
            try
            {
                foreach (Alert alert in siteAlerts.Values)
                {
                    if (!filteredAlerts.ContainsKey(alert.WebId))
                    {
                        filteredAlerts.Add(alert.WebId, new List<Alert>());
                    }
                    filteredAlerts[alert.WebId].Add(alert);
                }
            }
            catch { }
            return filteredAlerts;
        }
    }
}
