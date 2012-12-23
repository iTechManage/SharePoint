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


        

    }
}
