using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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


        internal static IList<Alert> GetAlertForList(SPWeb rootWeb, ReceivedEventType eventType)
        {
            IList<Alert> alerts = new List<Alert>();
            try
            {
                SPList list = rootWeb.Lists.TryGetList(ListAndFieldNames.settingsListName);
                if (list != null)
                {
                    //TOD: write a caml query to get the alerts based onconditions

                    foreach (SPListItem listItem in list.Items)
                    {
                        alerts.Add(new Alert(listItem));
                    }

                }
            }
            catch
            {

            }
            return alerts;
        }


        internal static Alert CreateAlertFromItem(SPListItem item)
        {
            return (new Alert(item));
        }

        internal static Alert CreateItemFromalert(SPListItem item)
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
