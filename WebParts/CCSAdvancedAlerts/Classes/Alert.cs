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
            this.webId =  listItem[ListAndFieldNames.settingsListWebIdFieldName].ToString();
            this.listId  = listItem[ListAndFieldNames.settingsListListIdFieldName].ToString();
            this.mailBody = listItem[ListAndFieldNames.settingsListMailBpdyFieldName].ToString();
            this.mailSubject = listItem[ListAndFieldNames.settingsListSubjectFieldName].ToString();
            this.toAddress = listItem[ListAndFieldNames.settingsListToAddressFieldName].ToString();
            this.fromAdderss  = listItem[ListAndFieldNames.settingsListFromAddressFieldName].ToString();

            
        }


    
    }

   

}
