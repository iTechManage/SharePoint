using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using System.Xml;


namespace CCSAdvancedAlerts
{
    class Alert
    {
        private List<AlertEventType> alertType = new List<AlertEventType>();
        internal List<AlertEventType> AlertType
        {
            get { return alertType; }
            set { alertType = value; }
        }
        
        //General Information
        private string title;
        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        private string id;
        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        private string webId;
        internal string WebId
        {
            get { return webId; }
            set { webId = value; }
        }

        private string listId;
        internal string ListId
        {
            get { return listId; }
            set { listId = value; }
        }

        
        //Address Fields
        private string toAddress;
        internal string ToAddress
        {
            get { return toAddress; }
            set { toAddress = value; }
        }
        private string fromAdderss;
        internal string FromAdderss
        {
            get { return fromAdderss; }
            set { fromAdderss = value; }
        }
        private string ccAddress;
        internal string CcAddress
        {
            get { return ccAddress; }
            set { ccAddress = value; }
        }
        private string bccAddress;
        internal string BccAddress
        {
            get { return bccAddress; }
            set { bccAddress = value; }
        }
        private string blockedUsers;
        internal string BlockedUsers
        {
            get { return blockedUsers; }
            set { blockedUsers = value; }
        }

        private string dateColumnName;
        internal string DateColumnName
        {
            get { return dateColumnName; }
            set { dateColumnName = value; }
        }

        private int periodQty;
        public int PeriodQty
        {
            get { return periodQty; }
            set { periodQty = value; }
        }

        private PeriodType periodType;
        internal PeriodType PeriodType
        {
            get { return periodType; }
            set { periodType = value; }
        }

        private PeriodPosition periodPosition;
        internal PeriodPosition PeriodPosition
        {
            get { return periodPosition; }
            set { periodPosition = value; }
        }
    
        private RepeatType repeatType;
        internal RepeatType RepeatType
        {
            get { return repeatType; }
            set { repeatType = value; }
        }

        private bool repeat;
        internal bool Repeat
        {
            get { return repeat; }
            set { repeat = value; }
        }

        private int repeatInterval;
        internal int RepeatInterval
        {
            get { return repeatInterval; }
            set { repeatInterval = value; }
        }

        private int repeatCount;
        internal int RepeatCount
        {
            get { return repeatCount; }
            set { repeatCount = value; }
        }


        //WhenToSend
        private SendType sendType;
        internal SendType SendType
        {
            get { return sendType; }
            set { sendType = value; }
        }

        private bool combineAlerts;
        internal bool CombineAlerts
        {
            get { return combineAlerts; }
            set { combineAlerts = value; }
        }

         private int businessStartHour;
        internal int BusinessStartHour
        {
            get { return businessStartHour; }
            set { businessStartHour = value; }
        }

        private int businessendtHour;
        internal int BusinessendtHour
        {
            get { return businessendtHour; }
            set { businessendtHour = value; }
        }

        private bool summaryMode;
        internal bool SummaryMode
        {
            get { return summaryMode; }
            set { summaryMode = value; }
        }
        
        private MailTemplateManager templateManager;
        internal MailTemplateManager TemplateManager
        {
            get { return templateManager; }
            set { templateManager = value; }
        }


        private bool immidiateAlways;
        public bool ImmidiateAlways
        {
            get { return immidiateAlways; }
            set { immidiateAlways = value; }
        }

        private List<WeekDays> immediateBusinessDays;
        public List<WeekDays> ImmediateBusinessDays
        {
            get { return immediateBusinessDays; }
            set { immediateBusinessDays = value; }
        }

        private List<WeekDays> dailyBusinessDays;
        public List<WeekDays> DailyBusinessDays
        {
            get { return dailyBusinessDays; }
            set { dailyBusinessDays = value; }
        }
        

        private IList<Condition> conditions;
        internal IList<Condition> Conditions
        {
            get
            {
                if (conditions == null)
                {
                    conditions = new List<Condition>();
                }
                return conditions;
            }
            set
            {
                conditions = value;
            }
        }

        private IList<MailTemplateUsageObject> templateObjects;
        internal IList<MailTemplateUsageObject> TemplateObjects
        {
            get
            {
                if (templateObjects == null)
                {
                    templateObjects = new List<MailTemplateUsageObject>();
                }
                return templateObjects;
            }
            set
            {
                templateObjects = value;
            }
        }
    
        public Alert()
        {
        }

        public Alert(SPListItem listItem,MailTemplateManager mailTemplateManager)
        {
            try
            {
                ///Basic information we are saving for Alert in Alert listing List
                //Title  Single line of text  
                //WebID  Single line of text  
                //ListID  Single line of text  
                //ItemID  Single line of text  
                //WhenToSend  Choice  
                //DetailInfo  Multiple lines of text  
                //Owner  Person or Group  
                //EventType  Choice 
                if (mailTemplateManager == null)
                {
                    mailTemplateManager = new MailTemplateManager(listItem.ParentList.ParentWeb);
                }
                else
                {
                    this.templateManager = mailTemplateManager;
                }

                this.id = Convert.ToString(listItem.ID);
                this.title = Convert.ToString(listItem["Title"]);
                this.webId = Convert.ToString(listItem[ListAndFieldNames.settingsListWebIdFieldName]);
                this.listId = Convert.ToString(listItem[ListAndFieldNames.settingsListListIdFieldName]);


                //Event Type Registered
                string stralerttype = Convert.ToString(listItem[ListAndFieldNames.settingsListEventTypeFieldName]);
                if (stralerttype.Contains(AlertEventType.ItemAdded.ToString()))
                    this.AlertType.Add(AlertEventType.ItemAdded);
                if (stralerttype.Contains(AlertEventType.ItemDeleted.ToString()))
                    this.AlertType.Add(AlertEventType.ItemDeleted);
                if (stralerttype.Contains(AlertEventType.ItemUpdated.ToString()))
                    this.AlertType.Add(AlertEventType.ItemUpdated);
                if (stralerttype.Contains(AlertEventType.DateColumn.ToString()))
                    this.AlertType.Add(AlertEventType.DateColumn);

                               
                if (mailTemplateManager != null)
                {
                    //Assign Mailtemplate Manager
                    //this.templateManager = mailTemplateManager;
                    this.templateObjects = templateManager.GetTemplateUsageObjects(this.id);
                }

                //Get the general aleret info and Conditions
                string metaXML = Convert.ToString(listItem[ListAndFieldNames.settingsListDetailInfoFieldName]);
                DeSerializeMetaData(metaXML);

                this.sendType = SendType.Immediate;

                //Send type
                //if (Convert.ToString(listItem[ListAndFieldNames.settingsListWhenToSendFieldName] )== Convert.ToString(SendType.Immediate))
                //{

                //}

            }
            catch
            { }
        }

  
        private  void DeSerializeMetaData(string xmlMetaData)
        {

            try
            {
                XmlDocument xmlDoc = new XmlDocument();

                //Get General Information
                xmlDoc.LoadXml(xmlMetaData);
                this.toAddress = xmlDoc.DocumentElement.SelectSingleNode(XMLElementNames.ToAddress).InnerText;
                this.ccAddress = xmlDoc.DocumentElement.SelectSingleNode(XMLElementNames.CcAddress).InnerText;
                this.bccAddress = xmlDoc.DocumentElement.SelectSingleNode(XMLElementNames.BccAddress).InnerText;
                this.fromAdderss = xmlDoc.DocumentElement.SelectSingleNode(XMLElementNames.FromAddress).InnerText;

                

               //Get the conditions
                XmlNodeList xNodes = xmlDoc.DocumentElement.SelectNodes("Conditions/*");
                this.conditions = new List<Condition>();
                foreach (XmlNode xNode in xNodes)
                {
                    this.conditions.Add(new Condition(xNode));
                }

                //General information
                this.BlockedUsers = xmlDoc.DocumentElement.SelectSingleNode(XMLElementNames.BlockedUsers).InnerText;

                this.DateColumnName = xmlDoc.DocumentElement.SelectSingleNode(XMLElementNames.DateColumnName).InnerText;

                this.PeriodType = (PeriodType)Enum.Parse(typeof(PeriodType), xmlDoc.DocumentElement.SelectSingleNode(XMLElementNames.PType).InnerText);

                this.PeriodPosition = (PeriodPosition)Enum.Parse(typeof(PeriodPosition), xmlDoc.DocumentElement.SelectSingleNode(XMLElementNames.PPosition).InnerText); ;
               
                this.Repeat =Convert.ToBoolean( xmlDoc.DocumentElement.SelectSingleNode(XMLElementNames.Repeat).InnerText);

                this.RepeatInterval =Convert.ToInt32( xmlDoc.DocumentElement.SelectSingleNode(XMLElementNames.RInterval).InnerText);

                this.RepeatType = (RepeatType)Enum.Parse(typeof(RepeatType), xmlDoc.DocumentElement.SelectSingleNode(XMLElementNames.RType).InnerText);

                this.RepeatCount =Convert.ToInt32( xmlDoc.DocumentElement.SelectSingleNode(XMLElementNames.RCount).InnerText);

                this.CombineAlerts = Convert.ToBoolean(xmlDoc.DocumentElement.SelectSingleNode(XMLElementNames.CombineAlerts).InnerText);

                this.ImmidiateAlways = Convert.ToBoolean( xmlDoc.DocumentElement.SelectSingleNode(XMLElementNames.ImmediateAlways).InnerText);

                this.immediateBusinessDays = DesrializeDays(xmlDoc.DocumentElement.SelectSingleNode(XMLElementNames.ImmediateBusinessDays).InnerText);

                if(!string.IsNullOrEmpty(xmlDoc.DocumentElement.SelectSingleNode(XMLElementNames.ImmediateBusinessHoursStart).InnerText))
                {
                this.BusinessStartHour =  Convert.ToInt32(xmlDoc.DocumentElement.SelectSingleNode(XMLElementNames.ImmediateBusinessHoursStart).InnerText);
                }

                if (!string.IsNullOrEmpty(xmlDoc.DocumentElement.SelectSingleNode(XMLElementNames.ImmediateBusinessHoursFinish).InnerText))
                {
                    this.BusinessendtHour = Convert.ToInt32(xmlDoc.DocumentElement.SelectSingleNode(XMLElementNames.ImmediateBusinessHoursFinish).InnerText);
                }

                this.DailyBusinessDays = DesrializeDays( xmlDoc.DocumentElement.SelectSingleNode(XMLElementNames.DailyBusinessDays).InnerText);

                this.SummaryMode = Convert.ToBoolean(  xmlDoc.DocumentElement.SelectSingleNode(XMLElementNames.SummaryMode).InnerText);

                if (!string.IsNullOrEmpty(xmlDoc.DocumentElement.SelectSingleNode(XMLElementNames.PQty).InnerText))
                {
                    this.PeriodQty = Convert.ToInt32(xmlDoc.DocumentElement.SelectSingleNode(XMLElementNames.PQty).InnerText);
                }


            }
            catch { }

        }

        private List<WeekDays> DesrializeDays(string serializedDays)
        {
            List<WeekDays> days = new List<WeekDays>();
            if(!string.IsNullOrEmpty(serializedDays))
            {
                string[] strdays = serializedDays.Split(new char[] { ';'},StringSplitOptions.RemoveEmptyEntries) ;
                foreach (string strday in strdays)
                {
                  days.Add((WeekDays)Enum.Parse(typeof(WeekDays), strday));
                }
            }
            return days;
        }


        internal bool IsValid(SPListItem item, AlertEventType eventType)
        {
            foreach(Condition condition in this.conditions)
            {
                if (condition != null)
                {
                    if (!condition.isValid(item, eventType))
                    {
                        return false;
                    }
                }
            }
            return true;
        }


        public MailTemplateUsageObject GetMailTemplateUsageObjectForEventType( AlertEventType eventType)
        {
            return this.templateManager.GetTemplateUsageObjectForAlert(this.Id, eventType);
        }

    
    }
}
