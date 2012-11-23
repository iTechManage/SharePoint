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
        
        //General Information
        internal string webId;
        internal string listId;

        
        //Address Fields
        internal  string toAddress;
        internal string fromAdderss;
        internal string ccAddress;
        internal string bccAddress;
        internal string blockedUsers;

        
        //Date Column fields
        //TODO later make seperate class for this.
        internal string dateColumn;
        internal PeriodType periodType;
        internal PeriodType PeriodPosition;
        internal PeriodType RepeatType;
        internal bool Repeat;
        internal int RepeatInterval;
        internal int RepeatCount;


        //WhenToSend
        internal SendType sendType;         
        internal bool combineAlerts;
        internal string businessDays;
        internal int businessStartHour;
        internal int businessendtHour;
        internal bool summaryMode ;

        
        IList<Condition> conditions;


        public Alert(SPListItem listItem)
        {
            this.webId = Convert.ToString(listItem[ListAndFieldNames.settingsListWebIdFieldName]);
            this.listId  =Convert.ToString( listItem[ListAndFieldNames.settingsListListIdFieldName]);
            //this.mailBody =Convert.ToString( listItem[ListAndFieldNames.settingsListMailBpdyFieldName]);
            //this.mailSubject = Convert.ToString(listItem[ListAndFieldNames.settingsListSubjectFieldName]);
            //this.toAddress = Convert.ToString(listItem[ListAndFieldNames.settingsListToAddressFieldName]);
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
