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
    }
}
