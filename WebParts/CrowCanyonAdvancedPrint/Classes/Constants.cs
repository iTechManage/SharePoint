using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrowCanyonAdvancedPrint.Classes
{
    class Constants
    {
        public static class ConfigFile
        {
            public const string PrintSettingsFile = "CCSPrintSettings.xml";
            public const string Print2 = "Print2.xml";
        }

        public static class ActionField
        {
            public const string printID = "PrintID";
            public const string printTitle = "Title";
            public const string printHeader = "Header";
            public const string printFooter = "Footer";
            public const string printType = "Type";
            public const string printExpressions = "Fields";
          
        }

        public static class Field
        {
            public const string fldNodeName = "Field";
            public const string fldFieldName = "FieldName";
            
        }
    }
}
