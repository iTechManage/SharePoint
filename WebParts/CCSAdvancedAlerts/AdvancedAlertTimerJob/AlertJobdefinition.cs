using System;
using System.Collections.Generic;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;


namespace CCSAdvancedAlerts
{
    class AlertJobdefinition: SPJobDefinition
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
                if(site!=null)
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
                                    using (SPWeb web = site.OpenWeb(webId))
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
                                                    list = web.Lists[alert.ListId];
                                                }
                                                catch
                                                { continue;  }

                                                this.ExecuteTimerAlert( web, list, alert);
                                            }

                                            //2. Handling Delayed alerts for daily bu specific time and send as single message
                                            //if (alert.AlertType != SendType.Immediate)
                                            if(alert.SendType != SendType.Immediate)
                                            {
                                               //if (((info2.SendHour == time2.Hour) && (time2.Minute < 30)) && (((info2.Timing == SendTiming.Daily) && info2.DailyBusinessDays.Contains(time2.DayOfWeek)) || (info2.SendWeekday == time2.DayOfWeek)))
                                               //  {
                                                
                                                //if((alert.SendHour == dtWebTime.Hour) && ( 
                                                // based on time
                                                if (alertManager == null)
                                                {
                                                    alertManager = new AlertManager(site.Url);
                                                }
                                                alertManager.SendDelayedMessages(alert);



                                            }

                                            //3. Handling Delayed alerts based on weekdays and all the stuff
                                            //if(!alert.ImmidiateAlways)
                                            //{
                                            //    //Based on week days

                                            //}

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

      

        private void ExecuteTimerAlert(SPWeb web,SPList list, Alert alert)
        {
            try
            {
                if (web != null && list != null)
                {
                    DateTime executionTime = DateTime.Now.AddMinutes(-30) ;
                    DateTime currentTime = DateTime.Now;
                    //We need to get all alerts which are fall
                    SPQuery query = new SPQuery();
                    //query.Query(string.Format("<Where>\r\n <And>\r\n        <Gt>\r\n            <FieldRef Name=\"{0}\" />\r\n            <Value Type=\"DateTime\" IncludeTimeValue=\"TRUE\">{1}</Value>\r\n        </Gt>\r\n        <Leq>\r\n            <FieldRef Name=\"{0}\" />\r\n            <Value Type=\"DateTime\" IncludeTimeValue=\"TRUE\">{2}</Value>\r\n        </Leq>\r\n    </And>\r\n</Where>", new object[] { alert.DateColumnName, SPUtility.CreateISO8601DateTimeFromSystemDateTime(executionTime), SPUtility.CreateISO8601DateTimeFromSystemDateTime(currentTime)}));
                    SPListItemCollection items = list.GetItems(query);
                    if (items.Count> 0)
                    {
                        foreach (SPListItem item in items)
                        {
                            if (alert.IsValid(item,AlertEventType.DateColumn))
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
                    {
                        //No items returned as part of Query
                    }
                }
            }
            catch
            {
               //Error occured while executing timer alerts
            }
        }

        private void ExecuteDelayedAlert()
        {
            try
            {

            }
            catch 
            { 
                //error occured while executing delayed alerts
            }
        }
        #endregion


    }
}
