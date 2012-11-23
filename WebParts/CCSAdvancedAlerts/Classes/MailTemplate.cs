using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCSAdvancedAlerts
{
    class MailTemplate
    {
        //Id
        string iD;
        public string ID
        {
            get { return iD; }
            set { iD = value; }
        }

        string body;
        public string Body
        {
            get { return body; }
            set { body = value; }
        }

        string subject;
        public string Subject
        {
            get { return subject; }
            set { subject = value; }
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



    }
}
