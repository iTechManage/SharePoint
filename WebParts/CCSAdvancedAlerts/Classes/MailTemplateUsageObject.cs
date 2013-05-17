using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCSAdvancedAlerts
{
    class MailTemplateUsageObject
    {

        string iD;
        public string ID
        {
            get { return iD; }
            set { iD = value; }
        }


        MailTemplate template;
        internal MailTemplate Template
        {
            get { return template; }
            set { template = value; }
        }

        private List<AlertEventType> alertType = new List<AlertEventType>();
        internal List<AlertEventType> AlertType
        {
            get { return alertType; }
            set { alertType = value; }
        }
        string created;
        public string Created
        {
            get { return created; }
            set { created = value; }
        }
        bool highLightUpdatedFields;
        public bool HighLightUpdatedFields
        {
            get { return highLightUpdatedFields; }
            set { highLightUpdatedFields = value; }
        }

        bool insertAttachments;
        public bool InsertAttachments
        {
            get { return insertAttachments; }
            set { insertAttachments = value; }
        }

        bool insertUpdatedFields;
        public bool InsertUpdatedFields
        {
            get { return insertUpdatedFields; }
            set { insertUpdatedFields = value; }
        }

        bool shareTemplateWithAllUsers;
        public bool ShareTemplateWithAllUsers
        {
            get { return shareTemplateWithAllUsers; }
            set { shareTemplateWithAllUsers = value; }
        }



    }
}
