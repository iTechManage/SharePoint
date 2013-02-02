using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.SharePoint.Utilities;
using ASPL.Blocks;

namespace ASPL.ConfigModel
{
    public class Field
    {
        internal Field(Tab parent, XmlNode fieldNode, int tabIndex, int fieldIndex)
        {
            this.Parent = parent;

            string parentXPath = string.Format("/tabs/tab[{0}]/fields/field[{1}]", tabIndex, fieldIndex);
            this.Index = Convert.ToUInt16(fieldNode.SelectSingleNode(parentXPath + "/index").InnerText);
            this.SPName = fieldNode.SelectSingleNode(parentXPath + "/spname").InnerText;
            this.SPDisplayName = 
                Helper.HtmlDecode(fieldNode.SelectSingleNode(parentXPath + "/spdisplayname").InnerText);
        }

        public Field(string fieldName)
        {
            this.SPName = fieldName;
        }

        public Field(ushort index, string fieldName)
            : this(fieldName)
        {
            this.Index = index;
        }

        public Field(string fieldName, string fieldDisplayName)
            : this(fieldName)
        {
            this.Index = 0;
            this.SPDisplayName = fieldDisplayName;
        }

        public Tab Parent { get; set; }
        public ushort Index { get; set; }
        public string SPName { get; set; }
        public string SPDisplayName { get; set; }

        public override string ToString()
        {
            return string.Format("<field><index>{0}</index><spname>{1}</spname><spdisplayname>{2}</spdisplayname></field>",
                this.Index.ToString(), this.SPName, Helper.HtmlEncode(this.SPDisplayName));
        }
    }

    public class Fields : List<Field>
    {
        public override string ToString()
        {
            string str = string.Empty;
            foreach (Field item in this)
            {
                str += item.ToString();
            }

            return string.Format("<fields>{0}</fields>", str);
        }
    }
}
