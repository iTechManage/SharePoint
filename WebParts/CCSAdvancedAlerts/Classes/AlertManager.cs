using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using System.Collections;
using System.Xml;

namespace CCSAdvancedAlerts
{
    class AlertManager
    {
        private SPList alertList;
        private SPList delayedAlertList;

        //public string rootwebSiteURL;
        //public string listID;

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


        internal IList<Alert> GetAlertForList(SPListItem listItem, AlertEventType eventType, MailTemplateManager mTManager)
        {
            IList<Alert> alerts = new List<Alert>();
            try
            {
                if (alertList != null)
                {
                    //TOD: write a caml query to get the alerts based eventtype
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.Append("<Where>");
                    stringBuilder.AppendFormat(
                        "<And>"+
                            "<And>"+
                                "<And>"+
                                    "<Eq>"+
                                        "<FieldRef Name=\"{0}\"/>"+
                                        "<Value Type=\"Text\">{1}</Value>"+
                                   "</Eq>"+
                                   "<Eq>"+
                                        "<FieldRef Name=\"{2}\"/>"+
                                        "<Value Type=\"Text\">{3}</Value>"+
                                   "</Eq>"+
                                "</And>"+
                                "<Contains>"+
                                    "<FieldRef Name=\"{4}\"/>"+
                                    "<Value Type=\"Choice\">{5}</Value>"+
                               "</Contains>"+
                            "</And>"+
                            "<Eq>"+
                                "<FieldRef Name=\"{6}\"/>"+
                                "<Value Type=\"Text\">{7}</Value>"+
                            "</Eq>"+ 
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


        //internal static Alert CreateAlertFromItem(SPListItem item)
        //{
        //    return (new Alert(item,new MailTemplateManager()));
        //}

        
        private SPList GetCCSAlertList(SPWeb rootWebSite)
        {
            return rootWebSite.Lists[ListAndFieldNames.settingsListName];
        }


        private void EnsureCCSAlertList(SPWeb rootWebSite)
        {
            //We have to check wether ccs alert list is exist or not.
            //if exist then no need to do anything
            //if not exist the need to create new one
        }

        #region Save Alert to hidden Alert Listing List

        /// <summary>
        /// This method take alert object and create item in alert listing list
        /// if Alert succesfully added to Alert list it will return true
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        /// 
        internal static bool AddAlert(SPWeb rootweb, Alert alert)
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
            if (settingslist != null)
            {
                SPListItem listItem = settingslist.AddItem();
                listItem["Title"] = alert.Title;
                listItem[ListAndFieldNames.settingsListWebIdFieldName] = alert.WebId;
                listItem[ListAndFieldNames.settingsListListIdFieldName] = alert.listId;
                
                //Event Type Registered
                foreach(AlertEventType aType in   alert.AlertType )
                {
                    listItem[ListAndFieldNames.settingsListEventTypeFieldName] += aType + ";#";
                }

                //Send type
                listItem[ListAndFieldNames.settingsListWhenToSendFieldName] = alert.SendType;




                //Other information in xml format
                listItem[ListAndFieldNames.settingsListDetailInfoFieldName] = SerializeAlertMetaData(alert);

                listItem.Update();

            }
            return true;
        }
                
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

                //Create Conditions
                XmlNode xConditions = rootNode.AppendChild(xmlDoc.CreateElement(XMLElementNames.ConditionsRootNodeName));
                foreach (Condition condition in alert.Conditions)
                {
                    XmlNode xCondition = xConditions.AppendChild(xmlDoc.CreateElement(XMLElementNames.ConditionChildNodeName));
                    xCondition.Attributes.Append(XMLHelper.AppendAttribute(xmlDoc, XMLElementNames.ConditionFieldTagName, condition.FieldName));
                    xCondition.Attributes.Append(XMLHelper.AppendAttribute(xmlDoc, XMLElementNames.ConditionOperatorTagName, Convert.ToString(condition.ComparisionOperator)));
                    xCondition.Attributes.Append(XMLHelper.AppendAttribute(xmlDoc, XMLElementNames.ConditionValueTagName, condition.StrValue));
                }


                //XmlNode userNode = xmlDoc.CreateElement("To").InnerText="";
                //userNode.InnerText = "krishna@itechmanage.com";
                //rootNode.AppendChild(userNode);

                //userNode = xmlDoc.CreateElement("user");
                //attribute = xmlDoc.CreateAttribute("age");
                //attribute.Value = "39";
                //userNode.Attributes.Append(attribute);
                //userNode.InnerText = "Jane Doe";
                //rootNode.AppendChild(userNode);
                //XmlAttribute attribute = xmlDoc.CreateAttribute("age");
                //attribute.Value = "42";
                //userNode.Attributes.Append(attribute);
          

            }
            catch { }
            return xmlDoc.InnerXml;
        }

        internal void AddDelayedAlert(DelayedAlert dAlert)
        {
            try
            {
                SPListItem item = delayedAlertList.AddItem();
                item[ListAndFieldNames.DelayedSubjectFieldName] = dAlert.Subject;
                item[ListAndFieldNames.DelayedBodyFieldName] = dAlert.Body;
                item[ListAndFieldNames.DelayedEventTypeFieldName] = dAlert.AlertType;
                item[ListAndFieldNames.DelayedAlertLookupFieldName] = dAlert.ParentAlertID + ";#" + dAlert.ParentAlertID;
                
                item.Update();
            }
            catch { }
        }




        #endregion
    }
}
