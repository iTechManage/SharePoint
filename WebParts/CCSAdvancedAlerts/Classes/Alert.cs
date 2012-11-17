using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;

namespace CCSAdvancedAlerts
{
    class Alert
    {
        internal ReceivedEventType alertType;
        internal  string toAddress;
        internal string fromAdderss;
        internal string ccAddress;
        internal string bccAddress;
        internal string mailBody;
        internal string mailSubject;
        internal string webId;
        internal string listId;

        public Alert(SPListItem listItem)
        {
            this.webId = Convert.ToString(listItem[ListAndFieldNames.settingsListWebIdFieldName]);
            this.listId  =Convert.ToString( listItem[ListAndFieldNames.settingsListListIdFieldName]);
            this.mailBody =Convert.ToString( listItem[ListAndFieldNames.settingsListMailBpdyFieldName]);
            this.mailSubject = Convert.ToString(listItem[ListAndFieldNames.settingsListSubjectFieldName]);
            this.toAddress = Convert.ToString(listItem[ListAndFieldNames.settingsListToAddressFieldName]);
            this.fromAdderss  = Convert.ToString(listItem[ListAndFieldNames.settingsListFromAddressFieldName]);
            this.alertType = GetAlertType(Convert.ToString(listItem[ListAndFieldNames.settingsListAlertTypeFieldName]));
            
        }

        private ReceivedEventType GetAlertType(string alertType)
        {
            if (alertType == "itemadded")
            {
                return ReceivedEventType.ItemAdded;
            }
            else if (alertType == "itemdeleted")
            {
                return ReceivedEventType.ItemDeleted;
            }
            else if (alertType == "itemupdated")
            {
                return ReceivedEventType.ItemUpdated;
            }
            else
            {
                return ReceivedEventType.Custom;
            }
        }


    
    }

   

}
