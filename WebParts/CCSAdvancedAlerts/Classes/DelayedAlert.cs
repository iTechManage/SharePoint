using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace CCSAdvancedAlerts
{
    class DelayedAlert
    {
        //Subject  Single line of text  
        //Body  Multiple lines of text  
        //EventType  Choice  
        //Alert  
        private string subject;
        public string Subject
        {
            get { return subject; }
            set { subject = value; }
        }

        private string body;
        public string Body
        {
            get { return body; }
            set { body = value; }
        }

        AlertEventType alertType;
        public AlertEventType AlertType
        {
            get { return alertType; }
            set { alertType = value; }
        }

        string parentAlertID;
        public string ParentAlertID
        {
            get { return parentAlertID; }
            set { parentAlertID = value; }
        }

        private List<SPFile> files;
        internal List<SPFile> Files
        {
            get
            {
                return this.files;
            }
        }

        private string id;
        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        private string parentItemID;
        public string ParentItemID
        {
            get { return parentItemID; }
            set { parentItemID = value; }
        }

        private SPListItem item;
        public SPListItem Item
        {
            get { return item; }
            set { item = value; }
        }



        public DelayedAlert(string subject,string body,string parentAlertId,string parentItemId, AlertEventType eventType)
        {
            this.subject = subject;
            this.body = body;
            this.parentAlertID = parentAlertId;
            this.alertType = eventType;
            this.parentItemID = parentItemId;
        }

        public DelayedAlert(SPListItem item)
        {
            try
            {
                this.subject = Convert.ToString(item[ListAndFieldNames.DelayedSubjectFieldName]);
                this.body = Convert.ToString(item[ListAndFieldNames.DelayedBodyFieldName]);
                SPFieldLookupValue lookupValue = new SPFieldLookupValue(item[ListAndFieldNames.DelayedAlertLookupFieldName].ToString());
                this.parentAlertID = Convert.ToString(lookupValue.LookupId);
                this.alertType = (AlertEventType)Enum.Parse(typeof(AlertEventType), Convert.ToString(item[ListAndFieldNames.DelayedEventTypeFieldName]));
                this.item = item;
                this.id = Convert.ToString(item.ID);
                this.parentItemID = Convert.ToString(item[ListAndFieldNames.DelayedParentItemID]);
            }
            catch { }
        }


    }
}
