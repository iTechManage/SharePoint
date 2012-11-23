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
        public string rootwebSiteURL;
        

        public  AlertManager( string WebSiteURL)
        {
            rootwebSiteURL = WebSiteURL;
        }


        internal static IList<Alert> GetAlertForList(SPWeb rootWeb, ReceivedEventType eventType)
        {
            IList<Alert> alerts = new List<Alert>();
            try
            {
                SPList list = rootWeb.Lists.TryGetList(ListAndFieldNames.settingsListName);
                if (list != null)
                {
                    //TOD: write a caml query to get the alerts based onconditions

                    foreach (SPListItem listItem in list.Items)
                    {
                        alerts.Add(new Alert(listItem));
                    }

                }
            }
            catch
            {

            }
            return alerts;
        }


        internal static Alert CreateAlertFromItem(SPListItem item)
        {
            return (new Alert(item));
        }

        internal static Alert CreateItemFromalert(SPListItem item)
        {
            return (new Alert(item));
        }

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

        private static XmlDocument SerializeAlertInfo(Alert alert)
        {
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                XmlNode rootNode = xmlDoc.CreateElement("AlertInformation");
                xmlDoc.AppendChild(rootNode);
                
                //General Properties
                rootNode.AppendChild(XMLHelper.CreateNode(xmlDoc, XMLElementNames.ToAddress, alert.toAddress));
                rootNode.AppendChild(XMLHelper.CreateNode(xmlDoc, XMLElementNames.FromAddress, alert.fromAdderss));
                rootNode.AppendChild(XMLHelper.CreateNode(xmlDoc, XMLElementNames.CcAddress, alert.ccAddress));
                rootNode.AppendChild(XMLHelper.CreateNode(xmlDoc, XMLElementNames.BccAddress, alert.bccAddress));
                rootNode.AppendChild(XMLHelper.CreateNode(xmlDoc, XMLElementNames.CombineAlerts , alert.combineAlerts.ToString()));
                //rootNode.AppendChild(XMLHelper.CreateNode(xmlDoc, XMLElementNames.ToAddress, alert.toAddress));
                //rootNode.AppendChild(XMLHelper.CreateNode(xmlDoc, XMLElementNames.ToAddress, alert.toAddress));
                //rootNode.AppendChild(XMLHelper.CreateNode(xmlDoc, XMLElementNames.ToAddress, alert.toAddress));
                //rootNode.AppendChild(XMLHelper.CreateNode(xmlDoc, XMLElementNames.ToAddress, alert.toAddress));
                //rootNode.AppendChild(XMLHelper.CreateNode(xmlDoc, XMLElementNames.ToAddress, alert.toAddress));

               //Create Conditions
                   


               //

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
            return xmlDoc;
        }


        private static Alert DeSerializeAlertInfo(XmlDocument xmlDoc)
        {
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                XmlNode rootNode = xmlDoc.CreateElement("AlertInformation");
                xmlDoc.AppendChild(rootNode);

                XmlNode userNode = xmlDoc.CreateElement("To");
                userNode.InnerText = "krishna@itechmanage.com";
                rootNode.AppendChild(userNode);

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
            return xmlDoc;
        }

    

    }
}
