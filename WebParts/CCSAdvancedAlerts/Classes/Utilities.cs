using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SharePoint;

namespace CCSAdvancedAlerts
{
   
    class Utilities
    {
        internal static LoggingManager LogManager = new LoggingManager();
        //public static LoggingManager LogManager
        //{
        //    get
        //    {
        //        if (logManager != null)
        //        {
        //            return logManager;
        //        }
        //        else
        //        {
        //            logManager = new LoggingManager();
        //            return logManager;
        //        }
        //    }
        //}


        internal static bool IsAdvancedAlertFeatureEnabledForsite(SPSite site)
        {
            try
            {
                // get all the features if Advanced Alert feature is activated then return true
                SPFeature feature = site.Features[new Guid("041d4cb3-e31e-4859-bd3d-51375fb89af4")];
                if (feature != null)
                {
                    return true;
                }
            }
            catch
            { }
            return false;
        }
        
        internal static bool ParseToBool(string value)
        {
            bool boolTemp = false;
            Boolean.TryParse(value, out boolTemp);
            return boolTemp;
        }

        internal static int ParseToInt(string value)
        {
            int intTemp = 0;
            int.TryParse(value, out intTemp);
            return intTemp;
        }
        
        internal static bool ContainsDay(List<WeekDays> days, int day)
        {
            bool isValid = false;
            foreach (WeekDays weekDay in days)
            {
                if (weekDay == WeekDays.sun && day == 0)
                {
                    return true;
                }
                else if (weekDay == WeekDays.mon && day == 1)
                {
                    return true;
                }
                else if (weekDay == WeekDays.tue && day == 2)
                {
                    return true;
                }
                else if (weekDay == WeekDays.wed && day == 3)
                {
                    return true;
                }
                else if (weekDay == WeekDays.thu && day == 4)
                {
                    return true;
                }
                else if (weekDay == WeekDays.fri && day == 5)
                {
                    return true;
                }
                else if (weekDay == WeekDays.sat && day == 6)
                {
                    return true;
                }

            }


            return isValid;

        }

        internal static bool CreateOrUpdate_CCSAdvancedAlertsList(SPWeb rootWebSite)
        {
            try
            {
                SPList alertLst = null;
                if (alertLst == null)
                {
                    rootWebSite.AllowUnsafeUpdates = true;
                    Guid guid = rootWebSite.Lists.Add("CCSAdvancedAlertsList", "CCS Advanced Alerts", SPListTemplateType.GenericList);

                    alertLst = rootWebSite.Lists[guid];  
                    
                    //List setting
                    alertLst.Hidden = true;
                    alertLst.OnQuickLaunch = false;
                    alertLst.NoCrawl = true;

                    //Adding fields
                    alertLst.Fields.Add("WebID",  SPFieldType.Text, false);  
                    alertLst.Fields.Add("ListID", SPFieldType.Text, false);
                    alertLst.Fields.Add("ItemID", SPFieldType.Text, false);
                    alertLst.Fields.Add("Owner",  SPFieldType.User , true);
                    //alertLst.Fields.Add("Contents", 3, true);


                    string str = alertLst.Fields.Add("ChangeTypes", SPFieldType.MultiChoice, true);
                    SPFieldMultiChoice choice = (SPFieldMultiChoice) alertLst.Fields[str];   
                    choice.Choices.Add(AlertEventType.ItemAdded.ToString());
                    choice.Choices.Add(AlertEventType.ItemUpdated.ToString());
                    choice.Choices.Add(AlertEventType.ItemDeleted.ToString());
                    choice.Choices.Add(AlertEventType.DateColumn.ToString());
                    choice.Update();

                    string str2 = alertLst.Fields.Add("Timing", SPFieldType.Choice, true);
                    SPFieldChoice choice2 = (SPFieldChoice)alertLst.Fields[str2]; 
                    choice2.Choices.Add(SendType.Immediate.ToString());
                    choice2.Choices.Add(SendType.Daily.ToString());
                    choice2.Choices.Add(SendType.Weekely.ToString());
                    choice2.Update();


                    //string str3 = alertLst.get_Fields().Add("SendDay", 6, true);
                    //SPFieldChoice choice3 = alertLst.get_Fields().get_Item(str3);
                    //for (int i = 1; i < 8; i++)
                    //{
                    //    choice3.get_Choices().Add(i.ToString());
                    //}
                    //choice3.Update();
                    //string str4 = alertLst.get_Fields().Add("SendHour", 6, true);
                    //SPFieldChoice choice4 = alertLst.get_Fields().get_Item(str4);
                    //for (int j = 0; j < 0x17; j++)
                    //{
                    //    choice4.get_Choices().Add(j.ToString());
                    //}
                    //choice4.Update();

                    alertLst.Update();
                    rootWebSite.AllowUnsafeUpdates = false;
                }
     
            }
            catch 
            {return false;}
            return true;
        }

        internal static bool CreateOrUpdate_CCSAdvancedAlertsMailTemplates(SPWeb rootWeb)
        {
            try
            {
                SPList templateList = null;
                rootWeb.AllowUnsafeUpdates = true;

                //Creating delayed alert list
                Guid guid = rootWeb.Lists.Add("CCSAdvancedAlertsMailTemplates", "CCS Advanced Alerts Mail Templates", SPListTemplateType.GenericList);
                templateList = rootWeb.Lists[guid];

                //set the list settings
                templateList.Hidden = true;
                templateList.NoCrawl = true;
                templateList.OnQuickLaunch = false;

                //Add fields
                templateList.Fields.Add("InsertUpdatedFields", SPFieldType.Boolean, false);
                templateList.Fields.Add("HighLightUpdatedFields", SPFieldType.Boolean, false);
                templateList.Fields.Add("InsertAttachments", SPFieldType.Boolean, false);
                templateList.Fields.Add("Owner", SPFieldType.User, false);
                templateList.Fields.Add("Subject", SPFieldType.Text, false);
                templateList.Fields.Add("Body", SPFieldType.Text, false);

                templateList.Update();
                rootWeb.AllowUnsafeUpdates = false;


            }
            catch
            { return false; }
            return true;
        }

        internal static bool CreateOrUpdate_CCSAdvancedDelayedAlertsList(SPWeb rootWeb)
        {
            try
            {
                SPList alertList = rootWeb.Lists.TryGetList("CCSAdvancedAlertsList");
               

                SPList delayedAlertList = null;
                rootWeb.AllowUnsafeUpdates = true;
                
                //Creating delayed alert list
                Guid guid = rootWeb.Lists.Add("CCSAdvancedDelayedAlerts", "CCS Advanced Delayed Alerts", SPListTemplateType.GenericList);
                delayedAlertList = rootWeb.Lists[guid];
                
                //set the list settings
                delayedAlertList.Hidden = true;
                delayedAlertList.NoCrawl = true;
                delayedAlertList.OnQuickLaunch = false;
                
                //Add fields
                delayedAlertList.Fields.Add("Subject ", SPFieldType.Text, false);
                delayedAlertList.Fields.Add("Body ", SPFieldType.Text, false);
                
                delayedAlertList.Fields.AddLookup("Alert ", alertList.ID, false);
                delayedAlertList.Fields.Add("ItemID ", SPFieldType.Text, false);

                string fieldName =  delayedAlertList.Fields.Add("EventType ", SPFieldType.MultiChoice, false);
                SPFieldMultiChoice multiChoices = (SPFieldMultiChoice) delayedAlertList.Fields[fieldName];

                multiChoices.Choices.Add(AlertEventType.ItemAdded.ToString());
                multiChoices.Choices.Add(AlertEventType.ItemAdded.ToString());
                multiChoices.Choices.Add(AlertEventType.ItemAdded.ToString());
                multiChoices.Choices.Add(AlertEventType.ItemAdded.ToString());

                multiChoices.Update();


                delayedAlertList.Update();

                rootWeb.AllowUnsafeUpdates = false;
            }
            catch
            { return false; }
            return true;
        }

        internal static bool CreateOrUpdate_CCSAdvancedAlertsForTemplateList(SPWeb rootWeb)
        {
            try
            {

                rootWeb.AllowUnsafeUpdates = true;

                //Get dependent lists
                SPList templatelist = rootWeb.Lists.TryGetList("CCSAdvancedAlertsMailTemplates");
                SPList alertList = rootWeb.Lists.TryGetList("CCSAdvancedAlertsList");
                SPList templateForAlertList = null;

                //Creating delayed alert list
                Guid guid = rootWeb.Lists.Add("CCSAdvancedTemplateForAlert", "CCS Advanced Template For Alert", SPListTemplateType.GenericList);
                templateForAlertList = rootWeb.Lists[guid];

                //set the list settings
                templateForAlertList.Hidden = true;
                templateForAlertList.NoCrawl = true;
                templateForAlertList.OnQuickLaunch = false;

                //Add fields
                templateForAlertList.Fields.AddLookup("Template ", templatelist.ID, false);
                templateForAlertList.Fields.AddLookup("Alert ", alertList.ID, false);

                string fieldName =  templateForAlertList.Fields.Add("EventType ", SPFieldType.MultiChoice, false);
                SPFieldMultiChoice multiChoices = (SPFieldMultiChoice)templateForAlertList.Fields[fieldName];

                multiChoices.Choices.Add(AlertEventType.ItemAdded.ToString());
                multiChoices.Choices.Add(AlertEventType.ItemAdded.ToString());
                multiChoices.Choices.Add(AlertEventType.ItemAdded.ToString());
                multiChoices.Choices.Add(AlertEventType.ItemAdded.ToString());

                multiChoices.Update();

                templateForAlertList.Fields.Add("InsertUpdatedFields ", SPFieldType.Boolean, false);
                templateForAlertList.Fields.Add("HighLightUpdatedFields ", SPFieldType.Boolean, false);
                templateForAlertList.Fields.Add("InsertAttachments ", SPFieldType.Boolean, false);

                templateForAlertList.Update();
                rootWeb.AllowUnsafeUpdates = false;
            }
            catch
            {return false;}
            return true;
        }


    }
}
