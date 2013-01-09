using System;
using System.Collections.Generic;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;


namespace CCSAdvancedAlerts
{
    class AlertJobdefinition : SPJobDefinition
    {
        #region Constructors( SpjobDefinition Implements 3 Constructors)

        public AlertJobdefinition()
            : base()
        { }

        public AlertJobdefinition(string jobName, SPService service, SPServer server, SPJobLockType targetType)
            : base(jobName, service, server, targetType)
        { }

        public AlertJobdefinition(string jobName, SPWebApplication webApplication)
            : base(jobName, webApplication, null, SPJobLockType.Job)
        { this.Title = jobName; }

        #endregion


        #region Override methods
        public override void Execute(Guid targetInstanceId)
        {
            //Get the webapplication on which it is activated
            //Get all the site collections in web application
            foreach (SPSite site in base.WebApplication.Sites)
            {
                //Get all the root level sites in the site collections
                if (site != null)
                {
                    //check do we have Alert pro active for that site collection by checking hidden list if possible feature
                    if (Utilities.IsAdvancedAlertFeatureEnabledForsite(site))
                    {
                        //Sync the alerts for the site collection
                        AlertsCache.SynchroniseAlerts(site.Url, this.Title);

                        //Get all the alerts for the site collection from buffer if not exist add to buffer
                        Dictionary<int, Alert> siteAlerts = AlertsCache.GetAlertsForSiteCollection(site.Url);

                        //Group alerts by web so that no need to create web object again and again
                        Dictionary<string, List<Alert>> siteAlertByWeb = TimerJobHelper.GroupAlertsByWeb(siteAlerts);

                        // get the current time
                        DateTime dtNow = DateTime.Now;
                        AlertManager alertManager = null;

                        try
                        {
                            // if we get any alerts then validate and prepare for sending email.
                            foreach (string webId in siteAlertByWeb.Keys)
                            {
                                try
                                {
                                    //create web object 
                                    using (SPWeb web = site.OpenWeb(new Guid(webId)))
                                    {
                                        //iterate all the alerts for this web
                                        foreach (Alert alert in siteAlertByWeb[webId])
                                        {
                                            DateTime dtWebTime = web.RegionalSettings.TimeZone.UTCToLocalTime(dtNow.ToUniversalTime());
                                            //1. Handling Timer based alerts
                                            if (alert.AlertType.Contains(AlertEventType.DateColumn))
                                            {
                                                // Calling ExecuteTimerAlert
                                                SPList list = null;
                                                try
                                                {
                                                    list = web.Lists[new Guid(alert.ListId)];
                                                }
                                                catch
                                                { continue; }

                                                this.ExecuteTimerAlert(web, list, alert);
                                            }

                                            //2. Handling Delayed alerts for daily bu specific time and send as single message
                                            //if (alert.AlertType != SendType.Immediate)
                                            if (alert.SendType != SendType.Immediate)
                                            {
                                                //if (((info2.SendHour == time2.Hour) && (time2.Minute < 30)) && (((info2.Timing == SendTiming.Daily) && info2.DailyBusinessDays.Contains(time2.DayOfWeek)) || (info2.SendWeekday == time2.DayOfWeek)))
                                                //  {

                                                //if ((alert.SendHour == dtWebTime.Hour) && (((alert.SendType == SendType.Daily) && alert.DailyBusinessDays.Contains(dtWebTime.DayOfWeek)) || (alert.SendDay == dtWebTime.DayOfWeek)))
                                                if ((alert.SendHour == dtWebTime.Hour) && (((alert.SendType == SendType.Daily) && Utilities.ContainsDay(alert.DailyBusinessDays, Convert.ToInt32(dtWebTime.DayOfWeek))) || (alert.SendDay == Convert.ToInt32(dtWebTime.DayOfWeek))))
                                                {
                                                    if (alertManager == null)
                                                    {
                                                        alertManager = new AlertManager(site.Url);
                                                    }
                                                    alertManager.ExecuteDelayedMessages(alert);
                                                }


                                            }

                                            //3. Handling Delayed alerts based on weekdays and all the stuff
                                            else if (!alert.ImmidiateAlways)
                                            {
                                                //Based on week days
                                                //if ((alert.ImmediateBusinessDays.Contains(web.RegionalSettings.TimeZone.UTCToLocalTime(DateTime.UtcNow.DayOfWeek)) && (alert.BusinessStartHour <= web.RegionalSettings.TimeZone.UTCToLocalTime(DateTime.UtcNow).Hour)) && (alert.BusinessendtHour > web.RegionalSettings.TimeZone.UTCToLocalTime(DateTime.UtcNow).Hour))
                                                if ((Utilities.ContainsDay(alert.ImmediateBusinessDays, Convert.ToInt32(web.RegionalSettings.TimeZone.UTCToLocalTime(DateTime.UtcNow).DayOfWeek))) && (alert.BusinessStartHour <= web.RegionalSettings.TimeZone.UTCToLocalTime(DateTime.UtcNow).Hour) && (alert.BusinessendtHour > web.RegionalSettings.TimeZone.UTCToLocalTime(DateTime.UtcNow).Hour))
                                                {
                                                    if (alertManager == null)
                                                    {
                                                        alertManager = new AlertManager(site.Url);
                                                    }
                                                    alertManager.ExecuteDelayedMessages(alert);

                                                }
                                            }

                                        }
                                    }
                                }
                                catch
                                {
                                    //Error occured while creating web application etc
                                }
                            }
                        }
                        catch
                        {
                            //Error occured while processing 
                        }
                    }
                    else
                    {
                        //Feature is not activated for the site collection
                    }
                }
            }
        }



        #endregion


        #region private methods



        private void ExecuteTimerAlert(SPWeb web, SPList list, Alert alert)
        {
            try
            {
                if (web != null && list != null)
                {


                    DateTime startTime = web.RegionalSettings.TimeZone.UTCToLocalTime(DateTime.UtcNow).AddMinutes(-30.0);
                    DateTime endTime = web.RegionalSettings.TimeZone.UTCToLocalTime(DateTime.UtcNow);
                    TimerJobHelper.CaliculateExecutionTime(ref startTime, alert, false);
                    TimerJobHelper.CaliculateExecutionTime(ref endTime, alert, false);

                    int num = !alert.Repeat ? 1 : (alert.RepeatCount + 1);
                    int num2 = 1;
                    while (num2 <= num)
                    {

                        //We need to get all alerts which are fall
                        SPQuery query = new SPQuery();
                        query.Query = string.Format("<Where>" +
                                                      "<And>" +
                                                       "<Gt>" +
                                                        "<FieldRef Name=\"{0}\" />" +
                                                          "<Value Type=\"DateTime\" IncludeTimeValue=\"TRUE\">{1}</Value>" +
                                                        "</Gt>" +
                                                        "<Leq>" +
                                                          "<FieldRef Name=\"{0}\" />" +
                                                          "<Value Type=\"DateTime\" IncludeTimeValue=\"TRUE\">{2}</Value>" +
                                                        "</Leq>" +
                                                       "</And>" +
                                                       "</Where>",
                                                        new object[] { alert.DateColumnName, SPUtility.CreateISO8601DateTimeFromSystemDateTime(startTime), SPUtility.CreateISO8601DateTimeFromSystemDateTime(endTime) });
                        SPListItemCollection items = list.GetItems(query);
                        if (items.Count > 0)
                        {
                            foreach (SPListItem item in items)
                            {
                                if (alert.IsValid(item, AlertEventType.DateColumn, null))
                                {
                                    Notifications mailSender = new Notifications();
                                    //mailSender.SendAlert(alert, ChangeTypes.DateColumn, item2, null);
                                    mailSender.SendMail(alert, AlertEventType.DateColumn, item);

                                }
                                else
                                {
                                    //Some conditions are not passthrough
                                }
                            }
                        }
                        else
                        { //No items returned as part of Query
                        }
                        if (num2 < num)
                        {
                            TimerJobHelper.CaliculateExecutionTime(ref startTime, alert, true);
                            TimerJobHelper.CaliculateExecutionTime(ref endTime, alert, true);
                        }
                        num2++;
                    }
                }
            }
            catch
            {
                //Error occured while executing timer alerts
            }
        }
        #endregion


    }
}
