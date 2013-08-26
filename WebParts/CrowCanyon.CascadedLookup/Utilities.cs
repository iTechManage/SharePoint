using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml;
using System.Web.UI;
using System.Web.UI.WebControls;

using Microsoft.SharePoint;

namespace CrowCanyon.CascadedLookup
{
    class Utilities
    {
        #region Excluded Fields
        static readonly string[] EXCLUDED_FIELDS = new string[]{
              "_Author","_Category", "_CheckinComment", "_Comments", "_Contributor", "_Coverage", "_DCDateCreated",
              "_DCDateModified", "_EditMenuTableEnd", "_EditMenuTableStart", "_EndDate", "_Format",
              "_HasCopyDestinations", "_IsCurrentVersion", "_LastPrinted", "_Level", "_ModerationComments",
              "_ModerationStatus", "_Photo", "_Publisher", "_Relation", "_ResourceType", "_Revision",
              "_RightsManagement", "_SharedFileIndex", "_Source", "_SourceUrl", "_Status", "ActualWork",
              "AdminTaskAction", "AdminTaskDescription", "AdminTaskOrder", "AssignedTo", "Attachments",
              "AttendeeStatus",/* "Author",*/ "BaseAssociationGuid", "BaseName", "Birthday", "Body",
              "BodyAndMore", "BodyWasExpanded", "Categories", "CheckoutUser", "Comment", "Comments", "Completed",
              /*"Created",*/ "Created_x0020_By", "Created_x0020_Date", "DateCompleted", "DiscussionLastUpdated",
              "DiscussionTitle", "DocIcon", "DueDate",/* "Editor", */"EmailBody", "EmailCalendarDateStamp",
              "EmailCalendarSequence", "EmailCalendarUid", "EndDate", "EventType",
              "ExtendedProperties", "fAllDayEvent", "File_x0020_Size", "File_x0020_Type", "FileDirRef",
              "FileLeafRef", "FileRef", "FileSizeDisplay", "FileType", "FormData", "FormURN", "fRecurrence",
              "FSObjType", "FullBody", "Group", "GUID", "HasCustomEmailBody", "Hobbies", "HTML_x0020_File_x0020_Type",
              "IMAddress", "ImageCreateDate", "ImageHeight", "ImageSize", "ImageWidth", "Indentation", "IndentLevel",
              "InstanceID", "IsActive", "IsSiteAdmin", "ItemChildCount", "Keywords", "Last_x0020_Modified","LessLink",
              "LimitedBody", "LinkDiscussionTitle", "LinkDiscussionTitleNoMenu", "LinkFilename", "LinkFilenameNoMenu",
              "LinkIssueIDNoMenu", "LinkTitle", "LinkTitleNoMenu","MasterSeriesItemID", "MessageBody", "MessageId",
              "MetaInfo",/* "Modified", */"Modified_x0020_By","MoreLink", "Notes", "Occurred", "ol_Department",
              "ol_EventAddress", "owshiddenversion", "ParentFolderId", "ParentLeafName", "ParentVersionString",
              "PendingModTime", "PercentComplete", "PermMask", "PersonViewMinimal", "Picture", "PostCategory",
              "Priority", "ProgId", "PublishedDate", "QuotedTextWasExpanded", "RecurrenceData", "RecurrenceID",
              "RelatedIssues", "RelevantMessages", "RepairDocument", "ReplyNoGif", "RulesUrl", "ScopeId", "SelectedFlag",
              "SelectFilename", "ShortestThreadIndex", "ShortestThreadIndexId", "ShortestThreadIndexIdLookup",
              "ShowCombineView", "ShowRepairView", "StartDate", "StatusBar", "SystemTask", "TaskCompanies",
              "TaskDueDate", "TaskGroup", "TaskStatus", "TaskType", "TemplateUrl", "ThreadIndex", "Threading",
              "ThreadingControls", "ThreadTopic", "Thumbnail", "TimeZone", "ToggleQuotedText", "TotalWork",
              "TrimmedBody", "UniqueId", "VirusStatus", "WebPage", "WorkAddress", "WorkflowAssociation",
              "WorkflowInstance", "WorkflowInstanceID", "WorkflowItemId", "WorkflowListId", "WorkflowVersion",
              "xd_ProgID", "xd_Signature", "XMLTZone", "XomlUrl","FolderChildCount"
        };
        #endregion

        public static bool IsLookupType(SPField field)
        {
            bool flag = (field != null && (field.Type == SPFieldType.Lookup || field.TypeAsString.Equals("Lookup", StringComparison.InvariantCultureIgnoreCase) || field.TypeAsString.Equals("CCSCascadedLookup", StringComparison.InvariantCultureIgnoreCase)));
            if (!flag)
            {
                SPFieldLookup f = field as SPFieldLookup;
                if (f != null)
                {
                    return true;
                }
            }

            return flag;
            
        }

        public static bool IsDisplayField(SPField field)
        {
            using (new EnterExitLogger("Utilities:IsDisplayField function"))
            {
                bool display = false;

                if (field != null && !field.Hidden && (Array.IndexOf<string>(EXCLUDED_FIELDS, field.InternalName) < 0))
                {
                    switch (field.Type)
                    {
                        case SPFieldType.Computed:
                            if (((SPFieldComputed)field).EnableLookup) { display = true; }
                            break;
                        case SPFieldType.Calculated:
                            if (((SPFieldCalculated)field).OutputType == SPFieldType.Text) { display = true; }
                            break;
                        default:
                            display = true;
                            break;
                    }
                }

                return display;
            }
        }

        public static bool GeneralFields(SPField f)
        {
            using (new EnterExitLogger("Utilities:GeneralFields function"))
            {
                Utils.LogManager.write("Field Internal Name: " + f.InternalName);
                if (f.InternalName.Equals("ID") || f.InternalName.Equals("Created") || f.InternalName.Equals("Author") || f.InternalName.Equals("Modified") ||
                f.InternalName.Equals("Editor") || f.InternalName.Equals("_UIVersionString") || f.InternalName.Equals("Title"))
                {
                    Utils.LogManager.write("GeneralFields: True");
                    return true;
                }
                else if (IsDisplayField(f))
                {
                    Utils.LogManager.write("GeneralFields : True");
                    return true;
                }
                else
                {
                    Utils.LogManager.write("GeneralFields : false");
                    return false;
                }
            }
        }

        public static void FetchMatchedValuesFromList(CCSCascadedLookupField field, string parentFieldValue, ref List<ListItem> itemList)
        {
            using (new EnterExitLogger("Utilities:FetchMatchedValuesFromList function"))
            {
                SPListItemCollection matchedItemList = null;

                SPSecurity.RunWithElevatedPrivileges(delegate
                    {
                        using (SPWeb LookupWeb = SPContext.Current.Site.OpenWeb(((SPFieldLookup)field).LookupWebId))
                        {
                            SPList LookupList = LookupWeb.Lists[new Guid(field.LookupFieldListName)];

                            SPField ParentLinkedField = LookupList.Fields[new Guid(field.GetParentLinkedColumnId())];

                            SPQuery query = new SPQuery();

                            string fetchItemConditionString = "<Eq><FieldRef Name='" + ParentLinkedField.InternalName + "' LookupId='TRUE'/><Value Type='Lookup'>" + parentFieldValue + "</Value></Eq>";

                            if (string.IsNullOrEmpty(field.View))
                            {
                                query.Query = "<Where>" + fetchItemConditionString + "</Where>";
                            }
                            else
                            {
                                string viewQueryWhereString = "";
                                string viewQueryOrderByString = "";

                                SPView view = LookupList.GetView(new Guid(field.View));

                                if (!String.IsNullOrEmpty(view.Query))
                                {
                                    string viewQueryXML = string.Format("<Query>{0}</Query>", view.Query);
                                    XmlDocument viewQueryXMLDoc = new XmlDocument();
                                    viewQueryXMLDoc.LoadXml(viewQueryXML);
                                    XmlNode whereNode = viewQueryXMLDoc.DocumentElement.SelectSingleNode("Where");
                                    if (whereNode != null && !string.IsNullOrEmpty(whereNode.InnerXml))
                                    {
                                        viewQueryWhereString = whereNode.InnerXml;
                                    }

                                    XmlNode orderByNode = viewQueryXMLDoc.DocumentElement.SelectSingleNode("OrderBy");
                                    if (orderByNode != null && !string.IsNullOrEmpty(orderByNode.InnerXml))
                                    {
                                        viewQueryOrderByString = orderByNode.InnerXml;
                                    }

                                    viewQueryOrderByString = string.Format("<OrderBy>{0}</OrderBy>", viewQueryOrderByString);
                                }

                                if (!String.IsNullOrEmpty(viewQueryWhereString))
                                {
                                    query.Query = "<Where><And>" + viewQueryWhereString + fetchItemConditionString + "</And></Where>" + viewQueryOrderByString;
                                }
                                else
                                {
                                    query.Query = "<Where>" + fetchItemConditionString + "</Where>";
                                }
                            }

                            Utils.LogManager.write("Item fetch Query: " + query.Query);

                            matchedItemList = LookupList.GetItems(query);
                            Utils.LogManager.write("matchedItemList item Count: " + matchedItemList.Count);
                        }
                    });
                foreach (SPListItem item in matchedItemList)
                {
                    //ListItem newItem = new ListItem(Convert.ToString(item.Fields[new Guid(field.LookupFieldName)].GetFieldValueAsText(item[new Guid(field.LookupFieldName)])), item.ID.ToString());
                    ListItem newItem = new ListItem(Convert.ToString(item.Fields.GetFieldByInternalName(field.LookupFieldName).GetFieldValueAsText(item[item.Fields.GetFieldByInternalName(field.LookupFieldName).Id])), item.ID.ToString());
                    if (!itemList.Contains(newItem))
                    {
                        itemList.Add(newItem);
                    }
                }
                Utils.LogManager.write("itemList item Count: " + itemList.Count);
            }
        }

        public static void FetchAllValuesFromList(CCSCascadedLookupField field, ref List<ListItem> itemList)
        {
            using (new EnterExitLogger("Utilities:FetchAllValuesFromList function"))
            {
                SPListItemCollection matchedItemList = null;

                SPSecurity.RunWithElevatedPrivileges(delegate
                    {
                        using (SPWeb LookupWeb = SPContext.Current.Site.OpenWeb(((SPFieldLookup)field).LookupWebId))
                        {
                            SPList LookupList = LookupWeb.Lists[new Guid(field.LookupFieldListName)];

                            SPQuery query = new SPQuery();
                            query.ViewAttributes = "Scope=\"RecursiveAll\"";

                            if (!string.IsNullOrEmpty(field.View))
                            {
                                string viewQueryWhereString = "";
                                string viewQueryOrderByString = "";

                                SPView view = LookupList.GetView(new Guid(field.View));

                                if (!String.IsNullOrEmpty(view.Query))
                                {
                                    string viewQueryXML = string.Format("<Query>{0}</Query>", view.Query);
                                    XmlDocument viewQueryXMLDoc = new XmlDocument();
                                    viewQueryXMLDoc.LoadXml(viewQueryXML);
                                    XmlNode whereNode = viewQueryXMLDoc.DocumentElement.SelectSingleNode("Where");
                                    if (whereNode != null && !string.IsNullOrEmpty(whereNode.InnerXml))
                                    {
                                        viewQueryWhereString = whereNode.InnerXml;
                                    }

                                    XmlNode orderByNode = viewQueryXMLDoc.DocumentElement.SelectSingleNode("OrderBy");
                                    if (orderByNode != null && !string.IsNullOrEmpty(orderByNode.InnerXml))
                                    {
                                        viewQueryOrderByString = orderByNode.InnerXml;
                                    }

                                    viewQueryOrderByString = string.Format("<OrderBy>{0}</OrderBy>", viewQueryOrderByString);
                                }

                                if (!String.IsNullOrEmpty(viewQueryWhereString))
                                {
                                    query.Query = "<Where>" + viewQueryWhereString + "</Where>" + viewQueryOrderByString;
                                }
                                else
                                {
                                    query.Query = viewQueryOrderByString;
                                }

                                Utils.LogManager.write("Item fetch Query: " + query.Query);

                                matchedItemList = LookupList.GetItems(query);
                            }
                            else
                            {
                                matchedItemList = LookupList.Items;
                            }

                        }
                    });

                Utils.LogManager.write("matchedItemList item Count: " + matchedItemList.Count);
                itemList = new List<ListItem>();
                foreach (SPListItem item in matchedItemList)
                {
                    //ListItem newItem = new ListItem(Convert.ToString(item.Fields[new Guid(field.LookupFieldName)].GetFieldValueAsText(item[new Guid(field.LookupFieldName)])), item.ID.ToString());
                    ListItem newItem = new ListItem(Convert.ToString(item.Fields.GetFieldByInternalName(field.LookupFieldName).GetFieldValueAsText(item[item.Fields.GetFieldByInternalName(field.LookupFieldName).Id])), item.ID.ToString());
                    if (!itemList.Contains(newItem))
                    {
                        itemList.Add(newItem);
                    }
                }

                Utils.LogManager.write("itemList item Count: " + itemList.Count);
            }
        }

        public static void FindControlRecursive(Control Root, Type type, ref List<Control> collect)
        {
            if (Root.GetType() == type) { collect.Add(Root); }

            if (Root != null && Root.Controls != null)
            {
                for (int i = 0; i < Root.Controls.Count; i++)
                {
                    //foreach (Control ctrl in Root.Controls)
                    //{
                    Control ctrl = Root.Controls[i];
                    if (ctrl != null)
                        FindControlRecursive(ctrl, type, ref collect);
                }
            }
        }

        public static void GetParametersValue(CCSCascadedLookupField field, out string WebUrl, out string LookupListName, out string FieldParentLinkedFieldName, out string FieldLookupFieldName, out string FieldViewWhereString, out string FieldViewOrderString)
        {
            using (new EnterExitLogger("Utilities:GetParametersValue function"))
            {
                WebUrl = ""; 
                LookupListName= "";
                FieldParentLinkedFieldName= "";
                FieldLookupFieldName= "";
                FieldViewWhereString= "";
                FieldViewOrderString = "";

                string webUrl = "";
                string lookupListName = "";
                string LookupFieldName = "";
                string ParentLinkedFieldName = "";
                string ViewWhereString = "";
                string ViewOrderString = "";
                Utils.LogManager.write("Field : " + field.Title);
                SPSecurity.RunWithElevatedPrivileges(delegate
                    {
                        using (SPWeb LookupWeb = SPContext.Current.Site.OpenWeb(((SPFieldLookup)field).LookupWebId))
                        {
                            SPList LookupList = LookupWeb.Lists[new Guid(field.LookupFieldListName)];
                            webUrl = LookupWeb.Url;
                            Utils.LogManager.write("webUrl : " + webUrl);

                            lookupListName = LookupList.Title;
                            Utils.LogManager.write("lookupListName : " + lookupListName);

                            LookupFieldName = field.LookupFieldName;
                            Utils.LogManager.write("LookupFieldName : " + LookupFieldName);


                            if (!string.IsNullOrEmpty(field.GetParentLinkedColumnId()))
                            {
                                //string linked_column = field.GetProperty(CustomDropDownList.LINK_COLUMN);
                                SPField ParentLinkedField = LookupList.Fields[new Guid(field.GetParentLinkedColumnId())];
                                if (ParentLinkedField != null)
                                {
                                    ParentLinkedFieldName = ParentLinkedField.InternalName;
                                    Utils.LogManager.write("ParentLinkedFieldName : " + ParentLinkedFieldName);
                                }
                            }

                            if (!string.IsNullOrEmpty(field.View))
                            {
                                SPView view = LookupList.GetView(new Guid(field.View));

                                if (!String.IsNullOrEmpty(view.Query))
                                {
                                    string viewQueryXML = string.Format("<Query>{0}</Query>", view.Query);
                                    XmlDocument viewQueryXMLDoc = new XmlDocument();
                                    viewQueryXMLDoc.LoadXml(viewQueryXML);
                                    XmlNode whereNode = viewQueryXMLDoc.DocumentElement.SelectSingleNode("Where");
                                    if (whereNode != null && !string.IsNullOrEmpty(whereNode.InnerXml))
                                    {
                                        ViewWhereString = whereNode.InnerXml;
                                        Utils.LogManager.write("ViewWhereString : " + ViewWhereString);
                                    }

                                    XmlNode orderByNode = viewQueryXMLDoc.DocumentElement.SelectSingleNode("OrderBy");
                                    if (orderByNode != null && !string.IsNullOrEmpty(orderByNode.InnerXml))
                                    {
                                        ViewOrderString = orderByNode.InnerXml;
                                    }

                                    ViewOrderString = string.Format("<OrderBy>{0}</OrderBy>", ViewOrderString);
                                    Utils.LogManager.write("ViewOrderString : " + ViewOrderString);
                                }
                            }
                        }
                    });

                WebUrl = webUrl;
                LookupListName = lookupListName;
                FieldParentLinkedFieldName = ParentLinkedFieldName;
                FieldLookupFieldName = LookupFieldName;
                FieldViewWhereString = ViewWhereString;
                FieldViewOrderString = ViewOrderString;
            }
        }

    }
}
