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
                //Creating Hidden List1
                SPSite site = properties.Feature.Parent as SPSite;
                SPWeb web = site.OpenWeb();
                SPListCollection lists = web.Lists;
                lists.Add("CCSAdvancedAlertsList", "CrowCanyon Advanced Alerts List", SPListTemplateType.GenericList);
                SPList newList = web.Lists["CCSAdvancedAlertsList"];
                newList.Fields.Add("WebID", SPFieldType.Text, false);
                newList.Fields.Add("ListID", SPFieldType.Text, false);
                newList.Fields.Add("ItemID", SPFieldType.Text, false);
                newList.Fields.Add("WhenToSend", SPFieldType.Choice, false);

                SPFieldChoice WhenToSendChoiceCol = (SPFieldChoice)newList.Fields["WhenToSend"];
                string[] strdata = new string[3];
                strdata[0] = "Immediate";
                strdata[1] = "Daily";
                strdata[2] = "Weekely";
                WhenToSendChoiceCol.Choices.Add(strdata[0]);
                WhenToSendChoiceCol.Choices.Add(strdata[1]);
                WhenToSendChoiceCol.Choices.Add(strdata[2]);
                WhenToSendChoiceCol.Update();
                newList.Fields.Add("DetailInfo", SPFieldType.Note, false);
                newList.Fields.Add("Owner", SPFieldType.User, false);
                newList.Fields.Add("EventType", SPFieldType.Choice, false);

                SPFieldChoice EventTypeChoiceCol = (SPFieldChoice)newList.Fields["EventType"];
                string[] strdata1 = new string[4];
                strdata1[0] = "ItemAdded";
                strdata1[1] = "ItemUpdated";
                strdata1[2] = "ItemDeleted";
                strdata1[3] = "DateColumn";
                EventTypeChoiceCol.Choices.Add(strdata1[0]);
                EventTypeChoiceCol.Choices.Add(strdata1[1]);
                EventTypeChoiceCol.Choices.Add(strdata1[2]);
                EventTypeChoiceCol.Choices.Add(strdata1[3]);
                EventTypeChoiceCol.Update();
                newList.Fields.Add("SendDay", SPFieldType.Choice, false);

                SPFieldChoice SendDayChoiceCol = (SPFieldChoice)newList.Fields["SendDay"];
                string[] strdata2 = new string[7];
                strdata2[0] = "1";
                strdata2[1] = "2";
                strdata2[2] = "3";
                strdata2[3] = "4";
                strdata2[4] = "5";
                strdata2[5] = "6";
                strdata2[6] = "7";
                SendDayChoiceCol.Choices.Add(strdata2[0]);
                SendDayChoiceCol.Choices.Add(strdata2[1]);
                SendDayChoiceCol.Choices.Add(strdata2[2]);
                SendDayChoiceCol.Choices.Add(strdata2[3]);
                SendDayChoiceCol.Choices.Add(strdata2[4]);
                SendDayChoiceCol.Choices.Add(strdata2[5]);
                SendDayChoiceCol.Choices.Add(strdata2[6]);
                SendDayChoiceCol.Update();
                newList.Fields.Add("SendHour", SPFieldType.Choice, false);

                SPFieldChoice SendHourChoiceCol = (SPFieldChoice)newList.Fields["SendHour"];
                string[] strdata3 = new string[24];
                strdata3[0] = "0";
                strdata3[1] = "1";
                strdata3[2] = "2";
                strdata3[3] = "3";
                strdata3[4] = "4";
                strdata3[5] = "5";
                strdata3[6] = "6";
                strdata3[7] = "7";
                strdata3[8] = "8";
                strdata3[9] = "9";
                strdata3[10] = "10";
                strdata3[11] = "11";
                strdata3[12] = "12";
                strdata3[13] = "13";
                strdata3[14] = "14";
                strdata3[15] = "15";
                strdata3[16] = "16";
                strdata3[17] = "17";
                strdata3[18] = "18";
                strdata3[19] = "19";
                strdata3[20] = "20";
                strdata3[21] = "21";
                strdata3[22] = "22";
                strdata3[23] = "23";
                SendHourChoiceCol.Choices.Add(strdata3[0]);
                SendHourChoiceCol.Choices.Add(strdata3[1]);
                SendHourChoiceCol.Choices.Add(strdata3[2]);
                SendHourChoiceCol.Choices.Add(strdata3[3]);
                SendHourChoiceCol.Choices.Add(strdata3[4]);
                SendHourChoiceCol.Choices.Add(strdata3[5]);
                SendHourChoiceCol.Choices.Add(strdata3[6]);
                SendHourChoiceCol.Choices.Add(strdata3[7]);
                SendHourChoiceCol.Choices.Add(strdata3[8]);
                SendHourChoiceCol.Choices.Add(strdata3[9]);
                SendHourChoiceCol.Choices.Add(strdata3[10]);
                SendHourChoiceCol.Choices.Add(strdata3[11]);
                SendHourChoiceCol.Choices.Add(strdata3[12]);
                SendHourChoiceCol.Choices.Add(strdata3[13]);
                SendHourChoiceCol.Choices.Add(strdata3[14]);
                SendHourChoiceCol.Choices.Add(strdata3[15]);
                SendHourChoiceCol.Choices.Add(strdata3[16]);
                SendHourChoiceCol.Choices.Add(strdata3[17]);
                SendHourChoiceCol.Choices.Add(strdata3[18]);
                SendHourChoiceCol.Choices.Add(strdata3[19]);
                SendHourChoiceCol.Choices.Add(strdata3[20]);
                SendHourChoiceCol.Choices.Add(strdata3[21]);
                SendHourChoiceCol.Choices.Add(strdata3[22]);
                SendHourChoiceCol.Choices.Add(strdata3[23]);
                SendHourChoiceCol.Update();
                SPView view = newList.DefaultView;
                view.ViewFields.Add("WebID");
                view.ViewFields.Add("ListID");
                view.ViewFields.Add("ItemID");
                view.ViewFields.Add("WhenToSend");
                view.ViewFields.Add("DetailInfo");
                view.ViewFields.Add("Owner");
                view.ViewFields.Add("EventType");
                view.ViewFields.Add("SendDay");
                view.ViewFields.Add("SendHour");
                view.Update();
                newList.Hidden = true;
                newList.Update();

                //Creating Hidden List2
                lists.Add("CCSAdvancedAlertsMailTemplates", "CrowCanyon Advanced Alerts Mail Templates", SPListTemplateType.GenericList);
                SPList newList2 = web.Lists["CCSAdvancedAlertsMailTemplates"];
                newList2.Fields.Add("InsertUpdatedFields", SPFieldType.Boolean, false);
                newList2.Fields.Add("HighLightUpdatedFields", SPFieldType.Boolean, false);
                newList2.Fields.Add("InsertAttachments", SPFieldType.Boolean, false);
                newList2.Fields.Add("Owner", SPFieldType.User, false);
                newList2.Fields.Add("Subject", SPFieldType.Text, false);
                newList2.Fields.Add("Body", SPFieldType.Note, false);
                SPView view2 = newList2.DefaultView;
                view2.ViewFields.Add("InsertUpdatedFields");
                view2.ViewFields.Add("HighLightUpdatedFields");
                view2.ViewFields.Add("InsertAttachments");
                view2.ViewFields.Add("Owner");
                view2.ViewFields.Add("Subject");
                view2.ViewFields.Add("Body");
                view2.Update();
                newList2.Hidden = true;
                newList2.Update();

                // Creating Hidden List3
                lists.Add("CCSAdvancedTemplateForAlert", "CrowCanyon Advanced Template for alert", SPListTemplateType.GenericList);
                SPList newList3 = web.Lists["CCSAdvancedTemplateForAlert"];
                newList3.Fields.AddLookup("Template", newList2.ID, false);
                SPFieldLookup lkp = (SPFieldLookup)newList3.Fields["Template"];
                lkp.LookupField = newList2.Fields["Title"].InternalName;
                newList3.Fields.Add("EventType", SPFieldType.Choice, false);

                SPFieldChoice EventTypeChoiceCol2 = (SPFieldChoice)newList3.Fields["EventType"];
                string[] strdata4 = new string[4];
                strdata4[0] = "ItemAdded";
                strdata4[1] = "ItemUpdated";
                strdata4[2] = "ItemDeleted";
                strdata4[3] = "DateColumn";
                EventTypeChoiceCol2.Choices.Add(strdata4[0]);
                EventTypeChoiceCol2.Choices.Add(strdata4[1]);
                EventTypeChoiceCol2.Choices.Add(strdata4[2]);
                EventTypeChoiceCol2.Choices.Add(strdata4[3]);
                EventTypeChoiceCol2.Update();
                newList3.Fields.Add("InsertUpdatedFields", SPFieldType.Boolean, false);
                newList3.Fields.Add("HighLightUpdatedFields", SPFieldType.Boolean, false);
                newList3.Fields.Add("InsertAttachments", SPFieldType.Boolean, false);
                newList3.Fields.AddLookup("Alert", newList.ID, false);
                SPFieldLookup lkp2 = (SPFieldLookup)newList3.Fields["Alert"];
                lkp2.LookupField = newList.Fields["Title"].InternalName;
                newList3.Update();
                SPView view3 = newList3.DefaultView;
                view3.ViewFields.Add("Template");
                view3.ViewFields.Add("EventType");
                view3.ViewFields.Add("InsertUpdatedFields");
                view3.ViewFields.Add("HighLightUpdatedFields");
                view3.ViewFields.Add("InsertAttachments");
                view3.ViewFields.Add("Alert");
                view3.Update();
                newList3.Hidden = true;
                newList3.Update();

                //Creating Hidden List 4
                lists.Add("CCSAdvancedDelayedAlerts", "CrowCanyon Advanced Delayed Alerts", SPListTemplateType.GenericList);
                SPList newList4 = web.Lists["CCSAdvancedDelayedAlerts"];
                newList4.Fields.Add("Subject", SPFieldType.Text, false);
                newList4.Fields.Add("Body", SPFieldType.Note, false);
                newList4.Fields.Add("EventType", SPFieldType.Choice, false);

                SPFieldChoice EventTypeChoiceCol3 = (SPFieldChoice)newList4.Fields["EventType"];
                string[] strdata5 = new string[4];
                strdata5[0] = "ItemAdded";
                strdata5[1] = "ItemUpdated";
                strdata5[2] = "ItemDeleted";
                strdata5[3] = "DateColumn";
                EventTypeChoiceCol3.Choices.Add(strdata5[0]);
                EventTypeChoiceCol3.Choices.Add(strdata5[1]);
                EventTypeChoiceCol3.Choices.Add(strdata5[2]);
                EventTypeChoiceCol3.Choices.Add(strdata5[3]);
                EventTypeChoiceCol3.Update();
                newList4.Fields.AddLookup("Alert", newList.ID, false);
                SPFieldLookup lkp3 = (SPFieldLookup)newList4.Fields["Alert"];
                lkp3.LookupField = newList.Fields["Title"].InternalName;
                newList4.Fields.Add("ItemID", SPFieldType.Text, false);
                newList4.Update();
                SPView view4 = newList4.DefaultView;
                view4.ViewFields.Add("Subject");
                view4.ViewFields.Add("Body");
                view4.ViewFields.Add("EventType");
                view4.ViewFields.Add("Alert");
                view4.ViewFields.Add("ItemID");
                view4.Update();
                newList4.Hidden = true;
                newList4.Update();

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
