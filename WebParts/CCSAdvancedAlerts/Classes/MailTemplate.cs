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

        string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }


        string body;
        public string Body
        {
            get { return body; }
            set { body = value; }
        }

        string created;
        public string Created
        {
            get { return created; }
            set { created = value; }
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
        bool shareTemplateWithAllUsers;
        public bool ShareTemplateWithAllUsers
        {
            get { return shareTemplateWithAllUsers; }
            set { shareTemplateWithAllUsers = value; }
        }

    }
}
