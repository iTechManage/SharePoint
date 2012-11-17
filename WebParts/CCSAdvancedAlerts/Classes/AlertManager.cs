using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using System.Collections;

namespace CCSAdvancedAlerts
{
    class AlertManager
    {
        public string rootwebSiteURL;
        

        public  AlertManager( string WebSiteURL)
        {
            rootwebSiteURL = WebSiteURL;
        }


        internal void GetAlertForList(SPWeb rootWeb)
        {
            try
            {
                

            }
            catch
            {

            }

        }


        internal static Alert CreateAlertFromItem(SPListItem item)
        {
            return (new Alert(item));
        }

        private SPList GetCCSAlertList(SPWeb rootWebSite)
        {
            return rootWebSite.Lists[ListAndFieldNames.settingsListName];
        }


        private void EnsureCCSAlertList(SPWeb rootWebSite)
        {
            //We have to check wether ccs alert list is exist or not.
            //if exist then no need to do anything
            //if not exist the need to create new one
        }
    

    }
}
