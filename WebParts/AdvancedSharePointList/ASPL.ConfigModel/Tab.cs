using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using ASPL.Blocks;
using Microsoft.SharePoint.Utilities;

namespace ASPL.ConfigModel
{
    public class Tab
    {
        internal Tab(XmlNode tabNode, int tabIndex)
        {
            string parentXPath = string.Format("/tabs/tab[{0}]", tabIndex);

            this.Index = Convert.ToUInt16(tabNode.SelectSingleNode(parentXPath + "/index").InnerText);
            this.Title = Helper.HtmlDecode(tabNode.SelectSingleNode(parentXPath + "/title").InnerText);
            this.Description = Helper.HtmlDecode(tabNode.SelectSingleNode(parentXPath + "/description").InnerText);
            this.IsSelected = Helper.ConvertToBool(tabNode.SelectSingleNode(parentXPath + "/isselected").InnerText);

            XmlNodeList fieldNode = tabNode.SelectNodes(parentXPath + "/fields/field");
            this.Fields = new Fields();

            int fieldIndex = 1;
            foreach (XmlNode node in fieldNode)
            {
                Field f = new Field(this, node, tabIndex, fieldIndex);
                this.Fields.Add(f);
                fieldIndex++;
            }

            XmlNodeList tabPermission = tabNode.SelectNodes(parentXPath + "/permissions/permission");

            this.Permissions = new TabPermissions();

            int permIndex = 1;
            foreach (XmlNode node in tabPermission)
            {
                TabPermission tp = new TabPermission(node, tabIndex, permIndex);
                this.Permissions.Add(tp);
                permIndex++;
            }
        }

        public Tab(ushort index, string title, string description)
        {
            this.Index = index;
            this.Title = title;
            this.Description = description;
            this.Fields = new Fields();
            this.Permissions = new TabPermissions();
        }

        public string ToHiddenFldValue()
        {
            string value = SPHttpUtility.UrlPathEncode(this.Title, false) + "=";
            foreach (Field f in this.Fields)
            {
                value += f.SPName + "~Show|";
            }

            return value;
        }

        public string CommaSeperatedFields
        {
            get
            {
                string strCommaSeperatedFields = "";
                foreach (Field field in this.Fields)
                {
                    strCommaSeperatedFields += field.SPName + Constants.FieldToStringSeparator;
                }

                return strCommaSeperatedFields;
            }
            set
            {
                foreach (string field in
                    value.Split(Constants.FieldToStringSeparator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                {
                    if (!string.IsNullOrEmpty(field))
                    {
                        this.Fields.Add(new Field(field));
                    }
                }
            }
        }

        public bool IsFirst { get; set; }
        public bool IsLast { get; set; }
        public ushort Index { get; set; }
        public string Title { get; set; }
        public bool IsSelected { get; set; }
        public string Description { get; set; }
        public Fields Fields { get; set; }
        public TabPermissions Permissions { get; set; }

        public override string ToString()
        {
            return string.Format(
                "<tab><index>{0}</index><isselected>{1}</isselected><title>{2}</title><description>{3}</description>{4}{5}</tab>",
                this.Index.ToString(),
                this.IsSelected.ToString(),
                Helper.HtmlEncode(this.Title),
                Helper.HtmlEncode(this.Description),
                this.Fields.ToString(),
                this.Permissions.ToString()
                );
        }
    }

    public class Tabs : List<Tab>
    {
        public string Theme { get; set; }
        public bool HideEmptyTabs { get; set; }

        public Tabs()
        {
            this.Theme = "default";
            this.HideEmptyTabs = false;
        }

        public override string ToString()
        {
            string str = string.Empty;
            foreach (Tab item in this)
            {
                str += item.ToString();
            }

            return string.Format("<tabs><theme>{0}</theme><hideemptytags>{1}</hideemptytags>{2}</tabs>",
                this.Theme.ToString(), this.HideEmptyTabs.ToString(), str);
        }

        public static Tabs LoadTabs(XmlDocument xmlTabSettings)
        {
            if (xmlTabSettings == null) return null;

            Tabs tabsSettings = new Tabs();
            tabsSettings.Theme = xmlTabSettings.SelectSingleNode("/tabs/theme").InnerText;

            tabsSettings.HideEmptyTabs =
                Helper.ConvertToBool(xmlTabSettings.SelectSingleNode("/tabs/hideemptytags").InnerText);

            Tab t2 = null;
            XmlNodeList tabNodes = xmlTabSettings.SelectNodes("/tabs/tab");
            int index = 1;
            foreach (XmlNode node in tabNodes)
            {
                Tab t = new Tab(node, index);
                t2 = t;
                tabsSettings.Add(t);
                index++;
            }

            if (t2 != null)
            {
                t2.IsLast = true;
            }

            return tabsSettings;
        }

        public List<string> GetAllUniqueFields()
        {
            List<string> allUniqueFields = new List<string>();
            foreach (Tab t in this)
            {
                foreach (Field f in t.Fields)
                {
                    if (!allUniqueFields.Contains(f.SPName))
                    {
                        allUniqueFields.Add(f.SPName);
                    }
                }
            }

            return allUniqueFields;
        }

        public string ToHiddenFldValue()
        {
            string currentTab = "currenttab=";
            const string separator = "&";
            string tabFields = "";
            foreach (Tab t in this)
            {
                if (t.IsSelected)
                {
                    currentTab += SPHttpUtility.UrlPathEncode(t.Title, false);
                    break;
                }
            }

            foreach (Tab t in this)
            {
                tabFields += t.ToHiddenFldValue() + separator;
            }

            return currentTab + separator + tabFields.Trim(separator.ToCharArray());
        }

        public Tab GetSelectedTab()
        {
            foreach (Tab t in this)
            {
                if (t.IsSelected) return t;
            }

            return this[0];
        }

        public string GetTabNameOfField(string spInternalName)
        {
            foreach (Tab t in this)
            {
                if (t.Fields.Any<Field>(f => f.SPName.Equals(spInternalName, StringComparison.InvariantCultureIgnoreCase)))
                {
                    return t.Title;
                }
            }

            return null;
        }
    }
}
