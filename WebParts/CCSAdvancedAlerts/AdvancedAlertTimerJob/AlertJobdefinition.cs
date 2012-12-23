using System;
using System.Collections.Generic;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint;


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
                                            //if (alert.AlertType!= SendType.Immediate)
                                            {

                                            }

                                            //3. Handling Delayed alerts based on weekdays and all the stuff
                                            {

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

      

        private void ExecuteTimerAlert(SPWeb web,SPList list, Alert alert)
        {
            try
            {
                if (web != null && list != null)
                {
                    DateTime executionTime;
                    DateTime currentTime;
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
