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
        private string sendtypes = string.Empty;
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

        private string itemId;
        public string ItemID
        {
            get { return itemId; }
            set { itemId = value; }
        }

        private SPUser owner;
        public SPUser Owner
        {
            get { return owner; }
            set { owner = value; }
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

        private PeriodType repeatType;
        internal PeriodType RepeatType
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
        private bool immediateDays;
        public bool ImmediateDays
        {
            get { return immediateDays; }
            set { immediateDays = value; }
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

        private int sendDay;
        public int SendDay
        {
            get { return sendDay; }
            set { sendDay = value; }
        }

        private int sendHour;
        public int SendHour
        {
            get { return sendHour; }
            set { sendHour = value; }
        }
        bool sendAsSingleMessage;
        public bool SendAsSingleMessage
        {
            get { return sendAsSingleMessage; }
            set { sendAsSingleMessage = value; }
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

        private bool custom_evaluation = false;
        internal bool CustomEvaluation
        {
            get { return custom_evaluation; }
            set { custom_evaluation = value; }
        }

        private string custom_evaluation_data;
        internal string CustomEvaluationData
        {
            get { return custom_evaluation_data; }
            set { custom_evaluation_data = value; }
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

        public Alert(SPListItem listItem, MailTemplateManager mailTemplateManager)
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
                this.itemId = Convert.ToString(listItem[ListAndFieldNames.settingsListItemIdFieldName]);
                

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


                CaliculateSendType();
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
                XmlNode xEval = xmlDoc.DocumentElement.SelectSingleNode(XMLElementNames.EvaluationCriteria);
                this.CustomEvaluation = Convert.ToBoolean(xEval.Attributes[XMLElementNames.EvaluationCustom].Value);
                if (this.CustomEvaluation)
                {
                    XmlNode xData = xEval.SelectSingleNode(XMLElementNames.CustomEvaluationData);
                    this.CustomEvaluationData = xData.InnerXml;
                }
                else
                {
                    XmlNodeList xNodes = xEval.SelectNodes("Conditions/*");
                    this.conditions = new List<Condition>();
                    foreach (XmlNode xNode in xNodes)
                    {
                        this.conditions.Add(new Condition(xNode));
                    }
                }

                //General information
                this.BlockedUsers =  XMLHelper.GetChildValue(xmlDoc , XMLElementNames.BlockedUsers);
                
                this.DateColumnName =  XMLHelper.GetChildValue(xmlDoc, XMLElementNames.DateColumnName);

                if(!string.IsNullOrEmpty(XMLHelper.GetChildValue(xmlDoc, XMLElementNames.PType)))
                this.PeriodType = (PeriodType)Enum.Parse(typeof(PeriodType), XMLHelper.GetChildValue(xmlDoc, XMLElementNames.PType));

                if (!string.IsNullOrEmpty(XMLHelper.GetChildValue(xmlDoc, XMLElementNames.PPosition)))
                this.PeriodPosition = (PeriodPosition)Enum.Parse(typeof(PeriodPosition), XMLHelper.GetChildValue(xmlDoc, XMLElementNames.PPosition));
               
                this.Repeat = Utilities.ParseToBool(XMLHelper.GetChildValue(xmlDoc, XMLElementNames.Repeat));
              
                this.RepeatInterval = Utilities.ParseToInt(XMLHelper.GetChildValue(xmlDoc, XMLElementNames.RInterval));

                if (!string.IsNullOrEmpty(XMLHelper.GetChildValue(xmlDoc, XMLElementNames.RType)))
                    this.RepeatType = (PeriodType)Enum.Parse(typeof(PeriodType), XMLHelper.GetChildValue(xmlDoc, XMLElementNames.RType));

                this.RepeatCount = Utilities.ParseToInt(XMLHelper.GetChildValue(xmlDoc, XMLElementNames.RCount));
                
                this.CombineAlerts =  Utilities.ParseToBool( XMLHelper.GetChildValue(xmlDoc, XMLElementNames.CombineAlerts));

                XmlNode xSendType = xmlDoc.DocumentElement.SelectSingleNode(XMLElementNames.SendType);

                sendtypes = xSendType.Attributes[XMLElementNames.Type].Value.ToString();

                if (sendtypes == "ImmediateBusinessDays")
                {
                    XmlNode xSendTyeDetails = xSendType.SelectSingleNode(XMLElementNames.SendTypeDetails);

                    this.immediateBusinessDays = DesrializeDays(XMLHelper.GetChildValue2(xSendTyeDetails, XMLElementNames.ImmediateBusinessDays));

                    this.BusinessStartHour = Utilities.ParseToInt(XMLHelper.GetChildValue2(xSendTyeDetails, XMLElementNames.ImmediateBusinessHoursStart));

                    this.BusinessendtHour = Utilities.ParseToInt(XMLHelper.GetChildValue2(xSendTyeDetails, XMLElementNames.ImmediateBusinessHoursFinish));

                    this.SendAsSingleMessage = Utilities.ParseToBool(XMLHelper.GetChildValue2(xSendTyeDetails, XMLElementNames.SendAsSingleMessage));
                }
                else if (sendtypes == "Daily")
                {
                    XmlNode xSendTyeDetails = xSendType.SelectSingleNode(XMLElementNames.SendTypeDetails);

                    this.DailyBusinessDays = DesrializeDays(XMLHelper.GetChildValue2(xSendTyeDetails, XMLElementNames.DailyBusinessDays));

                    this.SendHour = Utilities.ParseToInt(XMLHelper.GetChildValue2(xSendTyeDetails, XMLElementNames.SendHour));

                    this.SendAsSingleMessage = Utilities.ParseToBool(XMLHelper.GetChildValue2(xSendTyeDetails, XMLElementNames.SendAsSingleMessage));
                }
                else if (sendtypes == "Weekly")
                {
                    XmlNode xSendTyeDetails = xSendType.SelectSingleNode(XMLElementNames.SendTypeDetails);

                    this.sendDay = Utilities.ParseToInt(XMLHelper.GetChildValue2(xSendTyeDetails, XMLElementNames.SendDay));

                    this.SendHour = Utilities.ParseToInt(XMLHelper.GetChildValue2(xSendTyeDetails, XMLElementNames.SendHour));

                    this.SendAsSingleMessage = Utilities.ParseToBool(XMLHelper.GetChildValue2(xSendTyeDetails, XMLElementNames.SendAsSingleMessage));
                }


                //this.ImmidiateAlways = Utilities.ParseToBool(XMLHelper.GetChildValue(xmlDoc, XMLElementNames.ImmediateAlways));

                //this.immediateDays = Utilities.ParseToBool(XMLHelper.GetChildValue(xmlDoc, XMLElementNames.ImmediateBusinessDays));

                //this.immediateBusinessDays = DesrializeDays(XMLHelper.GetChildValue(xmlDoc,  XMLElementNames.ImmediateBusinessDays));

                //this.BusinessStartHour = Utilities.ParseToInt(XMLHelper.GetChildValue(xmlDoc, XMLElementNames.ImmediateBusinessHoursStart));

                //this.BusinessendtHour = Utilities.ParseToInt(XMLHelper.GetChildValue(xmlDoc, XMLElementNames.ImmediateBusinessHoursFinish));

                //this.DailyBusinessDays = DesrializeDays(XMLHelper.GetChildValue(xmlDoc, XMLElementNames.DailyBusinessDays));

                this.SummaryMode = Convert.ToBoolean( XMLHelper.GetChildValue(xmlDoc,  XMLElementNames.SummaryMode));

                this.PeriodQty = Utilities.ParseToInt(XMLHelper.GetChildValue(xmlDoc, XMLElementNames.PQty));

                //this.sendDay = Utilities.ParseToInt(XMLHelper.GetChildValue(xmlDoc, XMLElementNames.SendDay));

                //this.SendHour = Utilities.ParseToInt(XMLHelper.GetChildValue(xmlDoc, XMLElementNames.SendHour));

                //this.SendAsSingleMessage = Utilities.ParseToBool(XMLHelper.GetChildValue(xmlDoc, XMLElementNames.SendAsSingleMessage));
            }
            catch 
            {
                //Error occured while DeSerializeMetaData
            }

        }

        private void CaliculateSendType()
        {
            try
            {

                if (sendtypes == "ImmediateBusinessDays")
                {
                    this.sendType = SendType.ImmediateBusinessDays;
                }
                else if (sendtypes == "Daily")
                {
                    this.sendType = SendType.Daily;
                }
                else if (sendtypes == "Weekly")
                {
                    this.sendType = SendType.Weekly;
                }
                else
                {
                    this.sendType = SendType.ImmediateAlways;
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


        internal bool IsValid(SPListItem item, AlertEventType eventType, SPItemEventProperties properties)
        {
            if (!this.CustomEvaluation)
            {
                if (this.conditions != null)
                {
                    foreach (Condition condition in this.conditions)
                    {
                        if (condition != null)
                        {
                            if (!condition.isValid(item, eventType, properties))
                            {
                                return false;
                            }
                        }
                    }
                }

                return true;
            }
            else
            {
                ConditionGroup group = new ConditionGroup(this.CustomEvaluationData);
                return group.isValid(item, eventType, properties);
            }
        }

    
        public MailTemplateUsageObject GetMailTemplateUsageObjectForEventType( AlertEventType eventType)
        {
            return this.templateManager.GetTemplateUsageObjectForAlert(this.Id, eventType);
        }

    
    }
}
