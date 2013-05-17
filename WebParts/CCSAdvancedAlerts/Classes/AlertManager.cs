using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using System.Collections;
using System.Xml;
using System.Net.Mail;

namespace CCSAdvancedAlerts
{
    class AlertManager
    {
        private SPList alertList;
        private SPList delayedAlertList;

        #region Constructor

        public AlertManager(string siteCollectionURL)
        {
            using (SPSite site = new SPSite(siteCollectionURL))
            {
                using (SPWeb web = site.RootWeb)
                {
                    CheckForExistanceOfAlertList(web);
                }
            }
            
        }

        public AlertManager(SPWeb web)
        {
            CheckForExistanceOfAlertList(web);
        }

        #endregion


        #region Alert Related
        public string siteCollectionURL;
        string body = string.Empty;
        string toAddress =string.Empty;
        string ccAddress =string.Empty;
        string fromAddress = string.Empty;
        string subject = string.Empty;
        string smtpSName= string.Empty;
      
        /// <summary>
        /// Check Alerts list is existed in the site collection or not
        /// </summary>
        /// <param name="web"></param>
        public void CheckForExistanceOfAlertList(SPWeb web)
        {
            try
            {
                //Get the MailTemplate list from web if exists
                if (web != null)
                {
                    alertList = web.Lists.TryGetList(ListAndFieldNames.settingsListName);
                    if (alertList == null)
                    {
                        //Create new list if not exists
                    }

                    delayedAlertList = web.Lists.TryGetList(ListAndFieldNames.DelayedListName);
                    if (delayedAlertList == null)
                    {
                        //Create Delayed alert list
                    }
                }
            }
            catch
            { }
        }

        /// <summary>
        /// Get the alerts based on event type
        /// </summary>
        /// <param name="listItem"></param>
        /// <param name="eventType"></param>
        /// <param name="mTManager"></param>
        /// <returns></returns>
        internal IList<Alert> GetAlertForList(SPListItem listItem, AlertEventType eventType, MailTemplateManager mTManager)
        {
            IList<Alert> alerts = new List<Alert>();
            try
            {
                if (alertList != null)
                {
                    //TODO: write a caml query to get the alerts based eventtype
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.Append("<Where>");
                    stringBuilder.AppendFormat(
                       "<And>" +
                            "<And>" +
                                "<And>" +
                                    "<Eq>" +
                                        "<FieldRef Name=\"{0}\"/>" +
                                        "<Value Type=\"Text\">{1}</Value>" +
                                   "</Eq>" +
                                   "<Eq>" +
                                        "<FieldRef Name=\"{2}\"/>" +
                                        "<Value Type=\"Text\">{3}</Value>" +
                                   "</Eq>" +
                                "</And>" +
                                "<Contains>" +
                                    "<FieldRef Name=\"{4}\"/>" +
                                    "<Value Type=\"Choice\">{5}</Value>" +
                               "</Contains>" +
                            "</And>" +
                             "<Or>" +
                            "<Eq>" +
                                "<FieldRef Name=\"{6}\"/>" +
                                "<Value Type=\"Text\">0</Value>" + 
                            "</Eq>" +
                             "<Eq>" +
                             "<FieldRef Name=\"{6}\"/>" +
                             "<Value Type=\"Text\">{7}</Value>" +
                             "</Eq>" +
                             "</Or>" + 
                        "</And>", new object[] { "WebID", listItem.ParentList.ParentWeb.ID, "ListID", listItem.ParentList.ID, "EventType", eventType, "ItemID", "0" });
                    stringBuilder.Append("</Where>");

                    SPQuery query = new SPQuery();
                    query.Query = stringBuilder.ToString();
                    
                    SPListItemCollection  lItemCollection = alertList.GetItems(query);

                    foreach (SPListItem item in lItemCollection) 
                    {
                        alerts.Add(new Alert(item, mTManager));
                    }
                }
            }
            catch
            {

            }
            return alerts;
        }
        
        /// <summary>
        /// This method take alert object and create item in alert listing list
        /// if Alert succesfully added to Alert list it will return true
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        /// 
        internal static int AddAlert(SPWeb rootweb, Alert alert)
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

            SPList settingslist = rootweb.Lists.TryGetList(ListAndFieldNames.settingsListName);
            int alertID = 0;
            if (settingslist != null)
            {
                SPListItem listItem = null;

                if (alert.Id != "0")
                {
                    listItem = settingslist.GetItemById(Convert.ToInt32(alert.Id));
                    
                }
                if(listItem ==null)
                {
                    listItem = settingslist.AddItem();
                }
                
                listItem["Title"] = alert.Title;
                listItem[ListAndFieldNames.settingsListWebIdFieldName] = alert.WebId;
                listItem[ListAndFieldNames.settingsListListIdFieldName] = alert.ListId;
                listItem[ListAndFieldNames.settingsListItemIdFieldName] = alert.ItemID;
                
                //Event Type Registered  
                string eventType = string.Empty;
                foreach(AlertEventType aType in   alert.AlertType )
                {
                    eventType += aType + ";#";
                }
                listItem[ListAndFieldNames.settingsListEventTypeFieldName] = eventType;

                //Send type
                listItem[ListAndFieldNames.settingsListWhenToSendFieldName] = alert.SendType;

               //Alert owner
                listItem[ListAndFieldNames.settingsListOwner] = alert.Owner;


                //Other information in xml format
                listItem[ListAndFieldNames.settingsListDetailInfoFieldName] = SerializeAlertMetaData(alert);

                listItem.Update();

                alertID = listItem.ID;

            }
            return alertID;
        }
        
        /// <summary>
        /// Serialize Alert information into XML format
        /// </summary>
        /// <param name="alert"></param>
        /// <returns></returns>
        private static string SerializeAlertMetaData(Alert alert)
        {
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                XmlNode rootNode = xmlDoc.CreateElement("AlertInformation");
                xmlDoc.AppendChild(rootNode);

                
                //General Properties
                rootNode.AppendChild(XMLHelper.CreateNode(xmlDoc, XMLElementNames.ToAddress, alert.ToAddress));
                rootNode.AppendChild(XMLHelper.CreateNode(xmlDoc, XMLElementNames.FromAddress, alert.FromAdderss));
                rootNode.AppendChild(XMLHelper.CreateNode(xmlDoc, XMLElementNames.CcAddress, alert.CcAddress));
                rootNode.AppendChild(XMLHelper.CreateNode(xmlDoc, XMLElementNames.BccAddress, alert.BccAddress));

                // Create evaluation criteria
                XmlNode xEvaluation = rootNode.AppendChild(xmlDoc.CreateElement(XMLElementNames.EvaluationCriteria));
                xEvaluation.Attributes.Append(XMLHelper.AppendAttribute(xmlDoc, XMLElementNames.EvaluationCustom, alert.CustomEvaluation ? "true" : "false"));
                if (alert.CustomEvaluation)
                {
                    XmlNode xCustomData = xEvaluation.AppendChild(xmlDoc.CreateElement(XMLElementNames.CustomEvaluationData));
                    xCustomData.InnerXml = alert.CustomEvaluationData;
                }
                else
                {
                    //Create Conditions
                    XmlNode xConditions = xEvaluation.AppendChild(xmlDoc.CreateElement(XMLElementNames.ConditionsRootNodeName));
                    foreach (Condition condition in alert.Conditions)
                    {
                        XmlNode xCondition = xConditions.AppendChild(xmlDoc.CreateElement(XMLElementNames.ConditionChildNodeName));
                        xCondition.Attributes.Append(XMLHelper.AppendAttribute(xmlDoc, XMLElementNames.ConditionFieldTagName, condition.FieldName));
                        xCondition.Attributes.Append(XMLHelper.AppendAttribute(xmlDoc, XMLElementNames.ConditionOperatorTagName, Convert.ToString(condition.ComparisionOperator)));
                        xCondition.Attributes.Append(XMLHelper.AppendAttribute(xmlDoc, XMLElementNames.ConditionValueTagName, condition.StrValue));
                        xCondition.Attributes.Append(XMLHelper.AppendAttribute(xmlDoc, XMLElementNames.ConditionsComparisionType, Convert.ToString(condition.ComparisionType)));
                    }
                }

                //General Information
                rootNode.AppendChild(XMLHelper.CreateNode(xmlDoc, XMLElementNames.BlockedUsers, alert.BlockedUsers));//
                rootNode.AppendChild(XMLHelper.CreateNode(xmlDoc, XMLElementNames.DateColumnName, alert.DateColumnName));//
                rootNode.AppendChild(XMLHelper.CreateNode(xmlDoc, XMLElementNames.PType, alert.PeriodType.ToString()));//
                rootNode.AppendChild(XMLHelper.CreateNode(xmlDoc, XMLElementNames.PPosition, alert.PeriodPosition.ToString()));//
                rootNode.AppendChild(XMLHelper.CreateNode(xmlDoc, XMLElementNames.Repeat, alert.Repeat.ToString()));//
                rootNode.AppendChild(XMLHelper.CreateNode(xmlDoc, XMLElementNames.RInterval, alert.RepeatInterval.ToString()));//
                rootNode.AppendChild(XMLHelper.CreateNode(xmlDoc, XMLElementNames.RType, alert.RepeatType.ToString()));//
                rootNode.AppendChild(XMLHelper.CreateNode(xmlDoc, XMLElementNames.RCount, alert.RepeatCount.ToString()));//
                rootNode.AppendChild(XMLHelper.CreateNode(xmlDoc, XMLElementNames.CombineAlerts, alert.CombineAlerts.ToString())); //
                    XmlNode xSendType = rootNode.AppendChild(xmlDoc.CreateElement(XMLElementNames.SendType));
                    xSendType.Attributes.Append(XMLHelper.AppendAttribute(xmlDoc, XMLElementNames.Type, alert.SendType.ToString()));
                    if (alert.ImmediateDays)
                    {
                        XmlNode xImmediate = xSendType.AppendChild(xmlDoc.CreateElement(XMLElementNames.SendTypeDetails));
                        xImmediate.AppendChild(XMLHelper.CreateNode(xmlDoc, XMLElementNames.ImmediateBusinessDays, ConvertDaysToString(alert.ImmediateBusinessDays)));//
                        xImmediate.AppendChild(XMLHelper.CreateNode(xmlDoc, XMLElementNames.ImmediateBusinessHoursStart, alert.BusinessStartHour.ToString()));//
                        xImmediate.AppendChild(XMLHelper.CreateNode(xmlDoc, XMLElementNames.ImmediateBusinessHoursFinish, alert.BusinessendtHour.ToString()));//
                        xImmediate.AppendChild(XMLHelper.CreateNode(xmlDoc, XMLElementNames.SendAsSingleMessage, alert.SendAsSingleMessage.ToString()));
                    }
                    else if (alert.SendType==SendType.Daily)
                    {
                        XmlNode xBusinessdays = xSendType.AppendChild(xmlDoc.CreateElement(XMLElementNames.SendTypeDetails));
                        xBusinessdays.AppendChild(XMLHelper.CreateNode(xmlDoc, XMLElementNames.DailyBusinessDays, ConvertDaysToString(alert.DailyBusinessDays)));
                        xBusinessdays.AppendChild(XMLHelper.CreateNode(xmlDoc, XMLElementNames.SendHour, alert.SendHour.ToString()));
                        xBusinessdays.AppendChild(XMLHelper.CreateNode(xmlDoc, XMLElementNames.SendAsSingleMessage, alert.SendAsSingleMessage.ToString()));
                    }
                    else if (alert.SendType == SendType.Weekly)
                    {
                        XmlNode xWeekely = xSendType.AppendChild(xmlDoc.CreateElement(XMLElementNames.SendTypeDetails));
                        xWeekely.AppendChild(XMLHelper.CreateNode(xmlDoc, XMLElementNames.SendDay, alert.SendDay.ToString()));//
                        xWeekely.AppendChild(XMLHelper.CreateNode(xmlDoc, XMLElementNames.SendHour, alert.SendHour.ToString()));
                        xWeekely.AppendChild(XMLHelper.CreateNode(xmlDoc, XMLElementNames.SendAsSingleMessage, alert.SendAsSingleMessage.ToString()));
                    }

                //rootNode.AppendChild(XMLHelper.CreateNode(xmlDoc, XMLElementNames.ImmediateAlways, alert.ImmidiateAlways.ToString()));
                //rootNode.AppendChild(XMLHelper.CreateNode(xmlDoc, XMLElementNames.ImmediateDays, alert.ImmediateDays.ToString()));
                //rootNode.AppendChild(XMLHelper.CreateNode(xmlDoc, XMLElementNames.ImmediateBusinessDays, ConvertDaysToString(alert.ImmediateBusinessDays)));//
                //rootNode.AppendChild(XMLHelper.CreateNode(xmlDoc, XMLElementNames.ImmediateBusinessHoursStart, alert.BusinessStartHour.ToString()));//
                //rootNode.AppendChild(XMLHelper.CreateNode(xmlDoc, XMLElementNames.ImmediateBusinessHoursFinish, alert.BusinessendtHour.ToString()));//
                //rootNode.AppendChild(XMLHelper.CreateNode(xmlDoc, XMLElementNames.DailyBusinessDays, ConvertDaysToString(alert.DailyBusinessDays)));//
                    rootNode.AppendChild(XMLHelper.CreateNode(xmlDoc, XMLElementNames.SummaryMode, alert.SummaryMode.ToString())); //
                //rootNode.AppendChild(XMLHelper.CreateNode(xmlDoc, XMLElementNames.SendDay, alert.SendDay.ToString()));//
                //rootNode.AppendChild(XMLHelper.CreateNode(xmlDoc, XMLElementNames.SendHour, alert.SendHour.ToString()));
                //rootNode.AppendChild(XMLHelper.CreateNode(xmlDoc, XMLElementNames.SendAsSingleMessage, alert.SendAsSingleMessage.ToString()));
            
                //if (alert.PeriodQty > 0)
                {
                    rootNode.AppendChild(XMLHelper.CreateNode(xmlDoc, XMLElementNames.PQty, alert.PeriodQty.ToString())); //
                }
         
            }
            catch { }
            return xmlDoc.InnerXml;
        }

        private static string ConvertDaysToString(List<WeekDays> days)
        {
            string strdays = string.Empty;

            foreach (WeekDays day in days)
            {
                if(string.IsNullOrEmpty(strdays))
                {
                    strdays = day.ToString();
                }
                else
                {
                    strdays = strdays + ";" + day.ToString();
                }
            }
            return strdays;
        }

        /// <summary>
        /// This will call if we need to get all the alerts in the site collection
        /// </summary>
        /// <returns></returns>
        internal Dictionary<int, Alert> GetAllAlerts()
        {
            try
            {
                return GetAlertsChangesSince(DateTime.MinValue);
            }
            catch
            { }
            return new Dictionary<int, Alert>();
        }

        /// <summary>
        /// Get the alerts changed after last sync
        /// </summary>
        /// <param name="since"></param>
        /// <returns></returns>
        internal Dictionary<int, Alert> GetAlertsChangesSince(DateTime since)
        {
            Dictionary<int, Alert> modifiedAlerts =   new Dictionary<int, Alert>();
            try
            {
                if (since < alertList.Created || since < DateTime.UtcNow.AddDays(-60))
                {
                    //By default, the change log retains data for 60 days. You can configure the retention period by setting the ChangeLogRetentionPeriod property.
                    since =   alertList.Created;

                    //since = DateTime.UtcNow.AddDays(-30);
                }

                //SPChangeToken startToken = new SPChangeToken(SPChangeCollection.CollectionScope.List, list.ID, DateTime.UtcNow);

                //SPChangeToken endToken = new SPChangeToken(SPChangeCollection.CollectionScope.List,list.ID, new DateTime(2008, 10, 18));



                SPChangeToken token = new SPChangeToken(SPChangeCollection.CollectionScope.List, this.alertList.ID, since.ToUniversalTime());
                //Dictionary<int,Alert> modifiedAlerts = new Dictionary<int,Alert>();
                foreach (SPChange change in alertList.GetChanges(token))
                {
                    if (!(change is SPChangeItem))
                    {
                        continue;
                    }
                    SPChangeItem item = change as SPChangeItem;
                    if (!modifiedAlerts.ContainsKey(item.Id))
                    {
                        Alert alert = null;
                        //if(item.ChangeType  != 3)
                        try
                        {
                            alert = new Alert(alertList.GetItemById(item.Id), new MailTemplateManager(alertList.ParentWeb));
                           
                        }
                        catch
                        {
                           //item has been deleted

                        }

                        //if (alert != null && !alert.ImmidiateAlways)
                        //{
                        //    alert = null;
                        //}

                        modifiedAlerts.Add(item.Id, alert);
                    }
                    //Check if the alert is not immediate and all the stuff which are not eligible for timer based alerts
                }
            }
            catch { }
            return modifiedAlerts;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dAlert"></param>
        internal void DeleteAlerts(string alertID, MailTemplateManager mtManager)
        {
            try
            {
                SPListItem alertItem = null;
                //Delete the alert 
                if(!string.IsNullOrEmpty(alertID))
                {
                    alertItem = this.alertList.GetItemById(Convert.ToInt32(alertID));
                    if (alertItem != null)
                    {
                        try
                        {
                            alertItem.Delete();
                            //Delte the template objects for the alerts
                            mtManager.DeleteTemplateUsageObjects(alertID);

                        }
                        catch
                        {
                            //error occured while deleting alert
                        }
                    }
                }
                else
                {
                    //Alert Id is null or empty
                }
                
            }
            catch 
            {
                //Error occured while deleting alert
            }
        }

        /// <summary>
        /// Get Alert from its item id
        /// </summary>
        /// <param name="alertId"></param>
        /// <param name="mtManager"></param>
        /// <returns></returns>
        internal Alert GetAlertFromID(string alertId,MailTemplateManager mtManager)
        {
            Alert alert = null;
            try
            {
               SPListItem item = this.alertList.GetItemById(Convert.ToInt32(alertId));
               alert = new Alert(item, mtManager);
            }
            catch 
            { 
                //error occured while entering
            }
            return alert;
        }


        #endregion


        #region Delayed Alert related
        

        internal void AddDelayedAlert(DelayedAlert dAlert)
        {
            try
            {
                SPListItem item = delayedAlertList.AddItem();
                item[ListAndFieldNames.DelayedSubjectFieldName] = dAlert.Subject;
                item[ListAndFieldNames.DelayedBodyFieldName] = dAlert.Body;
                item[ListAndFieldNames.DelayedEventTypeFieldName] = dAlert.AlertType;
                item[ListAndFieldNames.DelayedAlertLookupFieldName] = dAlert.ParentAlertID + ";#" + dAlert.ParentAlertID;
                item[ListAndFieldNames.DelayedParentItemID] = dAlert.ParentItemID;
                item.Update();
            }
            catch { }
        }

        internal void ExecuteDelayedMessages(Alert alert)
        {
            try
            {
                SPQuery query = new SPQuery();
                query.Query  = string.Format("<Where><Eq><FieldRef Name=\"{0}\" LookupId=\"TRUE\"/><Value Type=\"Lookup\">{1}</Value></Eq></Where>", "Alert", alert.Id);
                SPListItemCollection items = this.delayedAlertList.GetItems(query);
                if (items.Count > 0)
                {
                    foreach (SPListItem item in items)
                    {
                        try
                        {
                            try
                            {
                                if (alert.SendAsSingleMessage)
                                {
                                    DelayedAlert delayedAlert = new DelayedAlert(item);
                                    Notifications notificationSender = new Notifications();
                                    SendDelayedMessage2(delayedAlert, alert,item);                          
                                }
                                else
                                {
                                    DelayedAlert delayedAlert = new DelayedAlert(item);
                                    Notifications notificationSender = new Notifications();
                                    notificationSender.SendDelayedMessage(delayedAlert, alert,item);                            
                                }
                            }
                            catch 
                            {
                            }
                            continue;
                        }
                        finally
                        {
                            try
                            {
                                //Delete the delayed alert after completion
                            }
                            catch { }
                        }
                    }
                    if (alert.SendAsSingleMessage)
                    {
                        SendMail(smtpSName, toAddress, fromAddress,ccAddress,subject,body,null);
                    }
                }
                else
                {
                    //No delayed alert found in the Delayed alert list
                }
            }
            catch
            {  }
        }

        #endregion


        #region Common methods

        internal static bool SendMail(string SmtpServer, string To, string From, string CC, string Subject, string Body, List<Attachment> Attachments)
        {
            bool succes = false;
            try
            {
                if (string.IsNullOrEmpty(To) && string.IsNullOrEmpty(CC))
                    return false;

                SmtpClient smtp = new SmtpClient(SmtpServer);
                Utilities.LogManager.write("smtp client created ");

                MailMessage msg = new MailMessage();
                msg.IsBodyHtml = true;
                msg.To.Add(To);
                msg.From = new MailAddress(From);
                if (!string.IsNullOrEmpty(CC))
                {
                    msg.CC.Add(CC);
                }
                if (!string.IsNullOrEmpty(Subject))
                {
                    msg.Subject = Subject;
                }
                if (!string.IsNullOrEmpty(Body))
                {
                    msg.Body = Body;
                }
                if (Attachments != null)
                {
                    if (Attachments.Count > 0)
                    {
                        foreach (Attachment attach in Attachments)
                        {
                            msg.Attachments.Add(attach);
                        }
                    }
                }

                smtp.Send(msg);
                succes = true;
            }
            catch
            {
                succes = false;
            }
            return succes;
        }
        public void SendDelayedMessage2(DelayedAlert delayedAlert, Alert alert, SPListItem item)
        {
            
            Notifications notificationSender = new Notifications();
            try
            {
                
                MailTemplateUsageObject mtObject = alert.GetMailTemplateUsageObjectForEventType(delayedAlert.AlertType);
                toAddress = notificationSender.GetRecipientEmailAddresses(alert.ToAddress, item);
                ccAddress = notificationSender.GetRecipientEmailAddresses(alert.CcAddress, item);
                fromAddress = notificationSender.GetRecipientEmailAddresses(alert.FromAdderss, item);
                subject = delayedAlert.Subject;
                body += delayedAlert.Body;
                smtpSName = notificationSender.GetSMTPServer(item);
                //SendMail(smtpSName,
                //         toAddress,
                //         fromAddress,
                //         ccAddress,
                //         subject,
                //         body,
                //         null);
            }
            catch { }
        }

        internal Dictionary<string,string> GetAlertOwners()
        {
            Dictionary<string, string> allOwners = new Dictionary<string, string>();
            try
            {
                //Iterate througu all the alerts for the owners
                foreach (SPListItem item in alertList.Items)
                {
                    //Push them to Dict
                    if (item["Owner"] != null)
                    {
                        SPUser user = new SPFieldUserValue(SPContext.Current.Web, item["Owner"].ToString()).User;
                        if(!allOwners.ContainsKey(user.ID.ToString()))
                        {
                            allOwners.Add(user.ID.ToString(), user.Name);
                        }
                    }
                }
            }
            catch
            {
               //Error occured while getting all the owners of the alerts
            }
            return allOwners;
        }


        #endregion
    }
}
