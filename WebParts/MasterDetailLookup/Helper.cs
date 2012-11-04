using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint;
using System.Web.UI.WebControls;
using System.Runtime.InteropServices;
using System.Web.UI;
using System.Globalization;
using System.Web.UI.HtmlControls;
using System.Web;
using CustomLookupField.CONTROLTEMPLATES;
using System.Xml;

namespace CustomLookupField
{
    internal sealed class Helper
    {
        internal static void get_matched_items(CustomDropDownList field, string sel_value, string linked_column, ref List<ListItem> item_list)
        {
            SPListItemCollection matched_items = null;
            string viewId = string.Empty;
            bool use_view_order = false;
            
            if (Convert.ToString(field.GetCustomProperty(CustomDropDownList.VIEW)) != string.Empty)
            {
                viewId = Convert.ToString(field.GetCustomProperty(CustomDropDownList.VIEW));
                viewId = viewId.Substring(0, viewId.IndexOf('|'));
                use_view_order = Convert.ToBoolean(field.GetCustomProperty(CustomDropDownList.SORT_BY_VIEW));
            }

            using (SPWeb w = SPContext.Current.Site.OpenWeb(((SPFieldLookup)field).LookupWebId))
            {

                SPList list1 = w.Lists[new Guid(((SPFieldLookup)field).LookupList)];
                SPField link_field = list1.Fields[new Guid(linked_column)];
             
                SPQuery query = new SPQuery();
                if (string.IsNullOrEmpty(viewId))
                {
                    query.Query = "<Where><Eq><FieldRef Name='" + link_field.InternalName + "' LookupId='TRUE'/><Value Type='Lookup'>" + sel_value + "</Value></Eq></Where>";
                }
                else
                {
                    SPView view = list1.GetView(new Guid(viewId));
                    string view_query = view.Query;
                    string item_query = "<Eq><FieldRef Name='" + link_field.InternalName + "' LookupId='TRUE'/><Value Type='Lookup'>" + sel_value + "</Value></Eq></And>";
                    if (!string.IsNullOrEmpty(view_query) && view_query.Contains("<Where>") && view_query.Contains("</Where>"))
                    {
                        int start_index = view_query.IndexOf("<Where>") + "<Where>".Length;
                        int length = view_query.IndexOf("</Where>") - start_index;
                        view_query = view_query.Substring(start_index, length);
                        view_query = "<Where><And>" + view_query + item_query + "</Where>";
                        string view_order_query = string.Empty;
                        if (use_view_order)
                        {
                            string xml = string.Format("<Query>{0}</Query>", view.Query);
                            XmlDocument document = new XmlDocument();
                            document.LoadXml(xml);
                            XmlNode node = document.DocumentElement.SelectSingleNode("OrderBy");
                            if (node == null || string.IsNullOrEmpty(node.InnerXml))
                            {
                                //do nothing
                            }
                            else
                            {
                                view_order_query = node.InnerXml;
                                view_order_query = string.Format("<OrderBy>{0}</OrderBy>", view_order_query);
                            }
                            view_query += view_order_query;
                        }
                        query.Query = view_query;
                    }
                    else
                    {
                        query.Query = "<Where><Eq><FieldRef Name='" + link_field.InternalName + "' LookupId='TRUE'/><Value Type='Lookup'>" + sel_value + "</Value></Eq></Where>";
                    }
                }
                matched_items = list1.GetItems(query);
               
            }
            foreach (SPListItem item in matched_items)
            {
                ListItem newItem = new ListItem(Convert.ToString(item.Fields[new Guid(((SPFieldLookup)field).LookupField)].GetFieldValueAsText(item[new Guid(((SPFieldLookup)field).LookupField)])), item.ID.ToString());
                if (!item_list.Contains(newItem))
                {
                    item_list.Add(newItem);
                }
            }

        }
    }
}
