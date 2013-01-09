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

        internal static DateTime CaliculatePeriod(DateTime date, PeriodType periodUnit, int amountOfTime)
        {
            try
            {
                switch (periodUnit)
                {
                    case PeriodType.Minutes:
                        return date.AddMinutes((double)amountOfTime);

                    case PeriodType.Hours:
                        return date.AddHours((double)amountOfTime);

                    case PeriodType.Days:
                        return date.AddDays((double)amountOfTime);

                    case PeriodType.Weeks:
                        return date.AddDays((double)(amountOfTime * 7));

                    case PeriodType.Months:
                        return date.AddMonths(amountOfTime);

                    case PeriodType.Years:
                        return date.AddYears(amountOfTime);
                }
            }
            catch { }
            return DateTime.MaxValue;
        }

        internal static void CaliculateExecutionTime(ref DateTime executionTime, Alert alert, bool isRepeat)
        {
            try
            {
                    int num = (isRepeat || (alert.PeriodPosition == PeriodPosition.After)) ? -1 : 1;
                    int periodAmount = num * (isRepeat ? alert.RepeatInterval : alert.PeriodQty);
                    PeriodType periodUnit = isRepeat ? alert.RepeatType : alert.PeriodType;
                    executionTime = CaliculatePeriod(executionTime, periodUnit, periodAmount);
            }
            catch { }
        }
    }
}
