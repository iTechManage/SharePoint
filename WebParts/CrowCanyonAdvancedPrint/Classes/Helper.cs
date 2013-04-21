using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using System.Xml;
using Microsoft.SharePoint.Utilities;

namespace CrowCanyonAdvancedPrint.Classes
{
    class Helper
    {
        internal static void AddTemplate(CCSTemplate ccsTemplate, SPList list)
        {
            XmlDocument xmlDoc = GetConfigFile(list, Constants.ConfigFile.PrintSettingsFile);
            CreateConfigFile(list, Constants.ConfigFile.PrintSettingsFile, SerializeActionToXML(ccsTemplate, xmlDoc));
        }
        internal static void UpdateTemplate(CCSTemplate ccsTemplate, SPList list)
        {
            DeleteAction(ccsTemplate.Id, list);
            AddTemplate(ccsTemplate, list);
        }

        internal static void DeleteAction(string ccsTemplateID, SPList list)
        {
            XmlDocument xmlDoc = GetConfigFile(list, Constants.ConfigFile.PrintSettingsFile);

            if (xmlDoc != null)
            {
                XmlNode rootNode = xmlDoc.DocumentElement;
                xmlDoc.AppendChild(rootNode);

                foreach (XmlNode actionNode in rootNode.ChildNodes)
                {
                    if (actionNode.Attributes[Constants.ActionField.printID].Value == ccsTemplateID)
                    {
                        actionNode.RemoveAll();
                        rootNode.RemoveChild(actionNode);
                        break;
                    }
                }
                
            }

            CreateConfigFile(list, Constants.ConfigFile.PrintSettingsFile, xmlDoc.InnerXml);
        }
        private static string SerializeActionToXML(CCSTemplate ccsTemplate, XmlDocument xmlSourceDoc)
        {
            XmlDocument xmlDoc = xmlSourceDoc;
            if (xmlSourceDoc == null)
            {
                xmlDoc = new XmlDocument();
            }

            try
            {
                XmlNode rootNode = null;
                if (xmlSourceDoc == null)
                {
                    rootNode = xmlDoc.CreateElement("Templates");
                    xmlDoc.AppendChild(rootNode);
                }
                else
                {
                    rootNode = xmlDoc.DocumentElement;
                }

                XmlNode actionNode;
                actionNode = xmlDoc.CreateElement("Template");
                actionNode.Attributes.Append(XMLHelper.AppendAttribute(xmlDoc, Constants.ActionField.printID, ccsTemplate.Id));
                actionNode.Attributes.Append(XMLHelper.AppendAttribute(xmlDoc, Constants.ActionField.printTitle, ccsTemplate.Title));
                actionNode.Attributes.Append(XMLHelper.AppendAttribute(xmlDoc, Constants.ActionField.printHeader, ccsTemplate.Header));
                actionNode.Attributes.Append(XMLHelper.AppendAttribute(xmlDoc, Constants.ActionField.printFooter, ccsTemplate.Footer));
                rootNode.AppendChild(actionNode);

                XmlNode expressionsNode = xmlDoc.CreateElement(Constants.ActionField.printExpressions);
                actionNode.AppendChild(expressionsNode);

                foreach (Field expr in ccsTemplate.Fields)
                {
                    XmlNode expressionNode = XMLHelper.CreateNode(xmlDoc, Constants.Field.fldNodeName, string.Empty);
                    expressionNode.Attributes.Append(XMLHelper.AppendAttribute(xmlDoc, Constants.Field.fldFieldName, expr.FieldName));
                    expressionsNode.AppendChild(expressionNode);
                }
            }
            catch { }
            return xmlDoc.InnerXml;
        }
        internal static List<CCSTemplate> DeSerializeActionFromXML(XmlDocument xmlSourceDoc)
        {
            List<CCSTemplate> actions = new List<CCSTemplate>();
            XmlDocument xmlDoc = xmlSourceDoc;
            if (xmlDoc == null)
            {
                return null;
            }

            try
            {
                XmlNode rootNode = null;
                if (xmlSourceDoc == null)
                {
                    rootNode = xmlDoc.CreateElement("Templates");
                    xmlDoc.AppendChild(rootNode);
                }
                else
                {
                    rootNode = xmlDoc.DocumentElement;
                }

                foreach (XmlNode actionNode in rootNode.ChildNodes)
                {
                    string actionID = actionNode.Attributes[Constants.ActionField.printID].Value;
                    string actionTitle = actionNode.Attributes[Constants.ActionField.printTitle].Value;
                    string printHeader = actionNode.Attributes[Constants.ActionField.printHeader].Value;
                    string printFooter = actionNode.Attributes[Constants.ActionField.printFooter].Value;
                    if (!string.IsNullOrEmpty(actionID) && !string.IsNullOrEmpty(actionTitle))
                    {
                        CCSTemplate action = new CCSTemplate();
                        action.Id = actionID;
                        action.Title = actionTitle;
                        action.Header = printHeader;
                        action.Footer = printFooter;
                        XmlNode expressionsNode = actionNode.FirstChild;
                        foreach (XmlNode expressionNode in expressionsNode.ChildNodes)
                        {
                            if (expressionNode.Name == Constants.Field.fldNodeName)
                            {
                                string fieldName = expressionNode.Attributes[Constants.Field.fldFieldName].Value;
                                Field expression = new Field();
                                expression.FieldName = fieldName;
                                action.Fields.Add(expression);
                            }
                        }
                        actions.Add(action);
                    }
                }
            }
            catch { }
            if (actions.Count <= 0)
            {
                actions = null;
            }
            return actions;
        }
        public static bool CreateConfigFile(SPList list, string filename, string xmlData)
        {
            try
            {
                string fileURL = SPUtility.GetFullUrl(list.ParentWeb.Site, list.RootFolder.ServerRelativeUrl.TrimEnd('/') + "/" + filename);
                Byte[] contentArray = Encoding.ASCII.GetBytes(xmlData);
                SPFile file = list.ParentWeb.Files.Add(fileURL, contentArray, true);
                file.Update();
                return true;
            }
            catch (Exception exp)
            {
            }

            return false;
        }
        public static XmlDocument GetConfigFile(SPList list, string filename)
        {
            try
            {
                SPFile file = list.ParentWeb.GetFile(SPUtility.GetFullUrl(list.ParentWeb.Site, list.RootFolder.ServerRelativeUrl.TrimEnd('/') + "/" + filename));
                if (file.Exists)
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(file.OpenBinaryStream());
                    return doc;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception exp)
            {
            }

            return null;
        }

    }

}
