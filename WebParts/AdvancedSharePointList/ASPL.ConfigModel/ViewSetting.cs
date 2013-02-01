using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace ASPL.ConfigModel
{
    public class ViewSetting
    {
        internal ViewSetting(XmlNode viewNode, int viewIndex)
        {

            string parentXPath = string.Format("/ListViewsSettings/ViewSettings[{0}]", viewIndex);

            this.ID = viewNode.SelectSingleNode(parentXPath + "/Id").InnerText;
            this.Index = Convert.ToUInt16(viewNode.SelectSingleNode(parentXPath + "/Index").InnerText);
            this.SPVName = viewNode.SelectSingleNode(parentXPath + "/ViewName").InnerText;
            this.UserGroup = viewNode.SelectSingleNode(parentXPath + "/UserGroup").InnerText;
            this.Permission = viewNode.SelectSingleNode(parentXPath + "/Permission").InnerText;

            this.HideActionsMenu = Convert.ToBoolean(viewNode.SelectSingleNode(parentXPath + "/HideActionsMenu").InnerText);
            this.HideAccessItem = Convert.ToBoolean(viewNode.SelectSingleNode(parentXPath + "/HideAccessItem").InnerText);
            this.HideRSSItem = Convert.ToBoolean(viewNode.SelectSingleNode(parentXPath + "/HideRSSItem").InnerText);
            this.HideAlertItem = Convert.ToBoolean(viewNode.SelectSingleNode(parentXPath + "/HideAlertItem").InnerText);
        }


        public int Index { get; set; }
        public string SPVName { get; set; }
        public string Permission { get; set; }
        public string UserGroup { get; set; }
        public string ID { get; set; }


        public bool HideActionsMenu { get; set; }
        public bool HideAccessItem { get; set; }
        public bool HideRSSItem { get; set; }
        public bool HideAlertItem { get; set; }


        public ViewSetting(string viewName)
        {
            this.SPVName = viewName;
        }

        public ViewSetting(string id, int index, string viewName, string userGroup, string viewpermission, bool hideActionsMenu, bool hideAccessItem, bool hideRSSItem, bool hideAlertItem)
        {
            this.ID = id;
            this.Index = index;
            this.SPVName = viewName;
            this.Permission = viewpermission;
            this.UserGroup = userGroup;


            this.HideActionsMenu = hideActionsMenu;
            this.HideAccessItem = hideAccessItem;
            this.HideRSSItem = hideRSSItem;
            this.HideAlertItem = hideAlertItem;
        }
        public override string ToString()
        {
            return string.Format("<ViewSettings><Id>{0}</Id><Index>{1}</Index><ViewName>{2}</ViewName><UserGroup>{3}</UserGroup><Permission>{4}</Permission><HideActionsMenu>{5}</HideActionsMenu><HideAccessItem>{6}</HideAccessItem><HideRSSItem>{7}</HideRSSItem><HideAlertItem>{8}</HideAlertItem></ViewSettings>", this.ID, this.Index.ToString(), this.SPVName, this.UserGroup, this.Permission.ToString(),this.HideActionsMenu,this.HideAccessItem,this.HideRSSItem,this.HideAlertItem);
        }
    }
    public class ViewRibbonPermission
    {
        internal ViewRibbonPermission(XmlNode viewNode, int viewIndex)
        {
            string conditionPath = string.Format("/ListViewsSettings[{0}]", viewIndex);
            this.HideActionsMenu = Convert.ToBoolean(viewNode.SelectSingleNode(conditionPath + "/HideActionsMenu").InnerText);
            this.HideActionsMenu = Convert.ToBoolean(viewNode.SelectSingleNode(conditionPath + "/HideActionsMenu").InnerText);
            this.HideActionsMenu = Convert.ToBoolean(viewNode.SelectSingleNode(conditionPath + "/HideActionsMenu").InnerText);
            this.HideActionsMenu = Convert.ToBoolean(viewNode.SelectSingleNode(conditionPath + "/HideActionsMenu").InnerText);

        }
        public bool HideActionsMenu { get; set; }
        public bool HideAccessItem { get; set; }
        public bool HideRSSItem { get; set; }
        public bool HideAlertItem { get; set; }

        public ViewSetting view { get; set; }


        public ViewRibbonPermission(bool hideactionsMenu, bool hideaccessItem, bool hideRSSItem, bool hideAlertItem)
        {

            this.HideActionsMenu = hideactionsMenu;
            this.HideAccessItem = hideaccessItem;
            this.HideRSSItem = hideRSSItem;
            this.HideAlertItem = hideAlertItem;
        }
        public override string ToString()
        {
            return string.Format("<ListViewsSettings><HideActionsMenu>{0}</HideActionsMenu><HideAccessItem>{1}</HideAccessItem><HideRSSItem>{2}</HideRSSItem><HideAlertItem>{3}</HideAlertItem>{4}</ListViewsSettings>", this.HideActionsMenu, this.HideAccessItem, this.HideRSSItem, this.HideAlertItem, view);
        }

    }
    public class Views : List<ViewSetting>
    {
        public bool UseRedirectPage { get; set; }
        public string ViewUnavailableText { get; set; }
        public string AllViewsUnavailableText { get; set; }
        public string NextViewButtonCaption { get; set; }
        public string GotoHomepageButtonCaption { get; set; }


        public Views()
        {
            this.UseRedirectPage = true;
            this.ViewUnavailableText = "According to current view permissions this view is not available. What would you like to do?";
            this.AllViewsUnavailableText = "According to current view permissions no list views are available.";
            this.NextViewButtonCaption = "Go to next available view";
            this.GotoHomepageButtonCaption = "Return to homepage";
        }

        public override string ToString()
        {
            string str = string.Empty;
            //  ViewPermission 
            foreach (ViewSetting item in this)
            {
                str += item.ToString();
            }
            return string.Format("<ListViewsSettings><UseRedirectPage>{0}</UseRedirectPage><ViewUnavailableText>{1}</ViewUnavailableText><AllViewsUnavailableText>{2}</AllViewsUnavailableText><NextViewButtonCaption>{3}</NextViewButtonCaption><GotoHomepageButtonCaption>{4}</GotoHomepageButtonCaption>{5}</ListViewsSettings>", this.UseRedirectPage, this.ViewUnavailableText, this.AllViewsUnavailableText, NextViewButtonCaption, GotoHomepageButtonCaption, str);
        }

        public static Views LoadViews(XmlDocument xmlViewSettings)
        {
            if (xmlViewSettings == null) return null;

            Views viewsSettings = new Views();

            viewsSettings.UseRedirectPage = Convert.ToBoolean(xmlViewSettings.SelectSingleNode("/ListViewsSettings/UseRedirectPage").InnerText);
            viewsSettings.ViewUnavailableText = xmlViewSettings.SelectSingleNode("/ListViewsSettings/ViewUnavailableText").InnerText;
            viewsSettings.AllViewsUnavailableText = xmlViewSettings.SelectSingleNode("/ListViewsSettings/AllViewsUnavailableText").InnerText;
            viewsSettings.NextViewButtonCaption = xmlViewSettings.SelectSingleNode("/ListViewsSettings/NextViewButtonCaption").InnerText;
            viewsSettings.GotoHomepageButtonCaption = xmlViewSettings.SelectSingleNode("/ListViewsSettings/GotoHomepageButtonCaption").InnerText;

            //bool hideActionsMenu = Convert.ToBoolean(xmlViewSettings.SelectSingleNode("/ListViewsSettings/UseRedirectPage").InnerText);
            //bool hideAccessItem = Convert.ToBoolean(xmlViewSettings.SelectSingleNode("/ListViewsSettings/UseRedirectPage").InnerText);
            //bool hideRSSItem = Convert.ToBoolean(xmlViewSettings.SelectSingleNode("/ListViewsSettings/UseRedirectPage").InnerText);
            //bool hideAlertItem = Convert.ToBoolean(xmlViewSettings.SelectSingleNode("/ListViewsSettings/UseRedirectPage").InnerText);


            //ViewRibbonPermission viewRibbonPermission = new ViewRibbonPermission(hideActionsMenu, hideAccessItem, hideRSSItem, hideAlertItem);

            //viewsSettings.Add(viewRibbonPermission);

            // View lstView = null;
            XmlNodeList viewNodes = xmlViewSettings.SelectNodes("/ListViewsSettings/ViewSettings");
            int index = 1;
            foreach (XmlNode node in viewNodes)
            {
                ViewSetting v = new ViewSetting(node, index);

                viewsSettings.Add(v);
                index++;
            }



            //Tab t2 = null;
            //XmlNodeList tabNodes = xmlViewSettings.SelectNodes("/tabs/tab");
            //int index = 1;
            //foreach (XmlNode node in tabNodes)
            //{
            //    Tab t = new Tab(node, index);
            //    t2 = t;
            //    tabsSettings.Add(t);
            //    index++;
            //}
            //if (t2 != null) t2.IsLast = true;


            return viewsSettings;

        }

    }
}
