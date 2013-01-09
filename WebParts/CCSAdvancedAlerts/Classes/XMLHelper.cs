using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace CCSAdvancedAlerts
{
    class XMLHelper
    {
        


        internal static XmlNode CreateNode(XmlDocument xDoc,  string Name, string InnerText)
        {
            XmlNode xNode = xDoc.CreateElement(Name);
            xNode.InnerText = InnerText;
            return xNode;
        }

        internal static XmlAttribute AppendAttribute(XmlDocument xDoc, string Name, string value)
        {
            XmlAttribute xAttribute = xDoc.CreateAttribute(Name);
            xAttribute.Value = value;
            return xAttribute;

        }

        internal static string GetChildValue(XmlDocument xmlDoc, string nodeName)
        {
            string strValue = string.Empty;
            if(xmlDoc.DocumentElement.SelectSingleNode(nodeName) != null)
            {
                strValue = xmlDoc.DocumentElement.SelectSingleNode(nodeName).InnerText;
            }
            return strValue;
        }

    }
}
