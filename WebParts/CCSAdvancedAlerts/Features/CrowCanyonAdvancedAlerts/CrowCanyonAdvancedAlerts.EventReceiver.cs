using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using Microsoft.SharePoint.Administration;

namespace CCSAdvancedAlerts.Features.CrowCanyonAdvancedAlerts
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("f7906c36-2dbf-48a5-9c91-835be7529985")]
    public class CrowCanyonAdvancedAlertsEventReceiver : SPFeatureReceiver
    {

        #region Global Variables
        private string jobTitle = "CrowCanyon Advanced Alert Timer Job For ";
        public const string WebSiteURL_KeyName = "WebSiteURL";
        #endregion

        /// <summary>
        /// Invoke on Feature Activation
        /// </summary>
        /// <param name="properties"></param>
        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {

            try
            {
                SPSite site = properties.Feature.Parent as SPSite;
                jobTitle = jobTitle + site.Url.ToString();

                // Delete Existing Timer Job If Installed 
                foreach (SPJobDefinition job in site.WebApplication.JobDefinitions)
                {

                    if (job.Name.Equals(jobTitle, StringComparison.InvariantCultureIgnoreCase))
                    {
                        job.Delete();
                    }
                }


                AlertJobdefinition objArchivalJob = new AlertJobdefinition(jobTitle, site.WebApplication);

                SPMinuteSchedule schedule = new SPMinuteSchedule();

                if (schedule != null)
                {
                    schedule.BeginSecond = 0;
                    schedule.EndSecond = 59;
                    schedule.Interval = 30;

                    objArchivalJob.Properties.Add(WebSiteURL_KeyName, site.Url);
                    objArchivalJob.Schedule = schedule;
                    objArchivalJob.Update();
                }
            }
            catch (System.Exception Ex)
            {

            }
        }

        /// <summary>
        /// Invoke On Feature Deactivation
        /// </summary>
        /// <param name="properties"></param>
        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {

            try
            {

                SPSite site = properties.Feature.Parent as SPSite;
                jobTitle = jobTitle + site.Url.ToString();

                // Delete Existing Timer Job If Installed 
                foreach (SPJobDefinition job in site.WebApplication.JobDefinitions)
                {

                    if (job.Name.Equals(jobTitle, StringComparison.InvariantCultureIgnoreCase))
                    {
                        job.Delete();
                    }
                }


            }
            catch (System.Exception Ex)
            {

            }
        }



        // Uncomment the method below to handle the event raised after a feature has been installed.

        //public override void FeatureInstalled(SPFeatureReceiverProperties properties)
        //{
        //}


        // Uncomment the method below to handle the event raised before a feature is uninstalled.

        //public override void FeatureUninstalling(SPFeatureReceiverProperties properties)
        //{
        //}

        // Uncomment the method below to handle the event raised when a feature is upgrading.

        //public override void FeatureUpgrading(SPFeatureReceiverProperties properties, string upgradeActionName, System.Collections.Generic.IDictionary<string, string> parameters)
        //{
        //}
    }
}
