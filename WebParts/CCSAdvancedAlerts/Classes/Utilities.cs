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

        //internal static bool CreateOrUpdate_CCSAdvancedAlertsList(SPWeb web)
        //{
        //    try
        //    {
        //        //SPList list = web.Lists.Add("CCSAdvancedAlertsList","CCS Advanced Alerts",

        //    }
        //    catch 
        //    {return false;}
        //    return true;
        //}

        //internal static bool CreateOrUpdate_CCSAdvancedAlertsList(SPWeb web)
        //{
        //    try
        //    {
        //        //SPList list = web.Lists.Add("CCSAdvancedAlertsList","CCS Advanced Alerts",

        //    }
        //    catch
        //    { return false; }
        //    return true;
        //}

        //internal static bool CreateOrUpdate_CCSAdvancedAlertsList(SPWeb web)
        //{
        //    try
        //    {
        //        //SPList list = web.Lists.Add("CCSAdvancedAlertsList","CCS Advanced Alerts",
        //    }
        //    catch
        //    { return false; }
        //    return true;
        //}

        //internal static bool CreateOrUpdate_CCSAdvancedAlertsList(SPWeb web)
        //{
        //    try
        //    {
        //        //SPList list = web.Lists.Add("CCSAdvancedAlertsList","CCS Advanced Alerts",
        //    }
        //    catch
        //    {return false;}
        //    return true;
        //}


    }
}
