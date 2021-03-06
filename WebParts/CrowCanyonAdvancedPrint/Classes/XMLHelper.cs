﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace CrowCanyonAdvancedPrint.Classes
{
    class XMLHelper
    {
        internal static XmlNode CreateNode(XmlDocument xDoc, string Name, string InnerText)
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
    }
}
