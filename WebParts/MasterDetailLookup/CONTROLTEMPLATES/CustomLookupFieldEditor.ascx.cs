using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using Microsoft.SharePoint.WebControls;
using System.Web.UI.WebControls;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using System.Globalization;
using System.Web.UI.HtmlControls;
using System.Collections;
using System.Runtime.InteropServices;

namespace CustomLookupField.CONTROLTEMPLATES
{
    public partial class CustomLookupFieldEditor : UserControl, IFieldEditor
    {
        readonly string[] EXCLUDED_FIELDS = new string[]{
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
      "EmailCalendarSequence", "EmailCalendarUid", "EndDate", "EventType", "Expires",
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
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void SelectedTargetWebChanged(Object sender, EventArgs args)
        {
            if (listTargetWeb.SelectedIndex > -1)
            {
                SetTargetList(listTargetWeb.SelectedItem.Value, true);
                InitialParentColumnValues_with_true();
                Page.SetFocus(listTargetList);
            }
        }

        protected void SelectedTargetListChanged(Object sender, EventArgs args)
        {
            if (listTargetList.SelectedIndex > -1)
            {
                string webId = string.Empty;
                if (listTargetWeb.Items.Count > 0)
                {
                    webId = listTargetWeb.SelectedItem.Value;
                }
                else if (!string.IsNullOrEmpty(TargetWebId)) { webId = TargetWebId; }

                
                  SetTargetColumn(webId, listTargetList.SelectedItem.Value);
               // SetTargetListView(webId, listTargetList.SelectedItem.Value);
                  Page.SetFocus(listTargetColumn);
                  if (listTargetList.SelectedValue == SPContext.Current.ListId.ToString())
                  {
                      cbxLinkParent.Checked = false;
                      cbxLinkParent.Enabled = false;
                  }
            }
           // InitialParentColumnValues_with_true();
        }

        protected void SelectedParentColumnChanged(Object sender, EventArgs args)
        {
            string webId = listTargetWeb.SelectedItem.Value;
            string targetListId = listTargetList.SelectedItem.Value;
            SPSite _site = SPControl.GetContextSite(this.Context);
            SPWeb _web = _site.OpenWeb(new Guid(webId));
            SPList list = _web.Lists[new Guid(targetListId)];
            SPList curList = SPContext.Current.List;
            SPFieldCollection fields = list.Fields;
            SPFieldCollection curFields = curList.Fields;
            string parent_column_lookup_id = ((SPFieldLookup)curFields[listParentColumn.SelectedItem.Text]).LookupList;

            List<ListItem> str2 = new List<ListItem>();
            str2.Clear();

            foreach (SPField f in fields)
            {
                if (CanFieldBeDisplayed(f))
                {
                    if (parent_column_lookup_id != null)
                    {
                        string typename = f.TypeDisplayName;
                        bool temp = Convert.ToString(f.GetCustomProperty(CustomDropDownList.LINK)) == Boolean.TrueString;

                        if ((typename.Equals("CustomDropDownList") && !temp) || f.Type == SPFieldType.Lookup)
                        {
                            if (parent_column_lookup_id.ToLower() == ((SPFieldLookup)f).LookupList.ToLower())
                            {
                                str2.Add(new ListItem(string.Format(CultureInfo.InvariantCulture, "{0}", f.Title), f.Id.ToString()));
                            }
                        }
                    }
                }
            }

            if (str2.Count > 0)
            {
                str2.Sort(delegate(ListItem item1, ListItem item2)
                {
                    return item1.Text.CompareTo(item2.Text);
                });

                listLinkColumn.Items.Clear();
                listLinkColumn.Items.AddRange(str2.ToArray());

                ListItem bitem = null;
                if (!string.IsNullOrEmpty(TargetLinkColumnId)) { bitem = listLinkColumn.Items.FindByValue(TargetLinkColumnId); }
                if (bitem != null) { listLinkColumn.SelectedIndex = listLinkColumn.Items.IndexOf(bitem); }
                else { listLinkColumn.SelectedIndex = 0; } 
            }


        }

        protected void LinkParentColumnChanged(Object sender, EventArgs args)
        {
            if (cbxLinkParent.Checked == false)
            {
                InitialParentColumnValues_with_false();
            }
            else
            {
                InitialParentColumnValues_with_true();
            }
        }

        private void InitializeFieldAdvanceSettings(string webId,string listId)
        {
            SPSite _site = SPControl.GetContextSite(this.Context);
            SPWeb _web = _site.OpenWeb(new Guid(webId));
            SPList list = _web.Lists[new Guid(listId)];
            SPFieldCollection fields = list.Fields;
            foreach (SPField f in fields)
            {
                if (CanFieldBeDisplayed(f))
                {
                    if (f.Type == SPFieldType.Lookup || f.TypeAsString == "CustomDropDownList")
                    {
                        if (f.ShowInViewForms != true)
                        {
                          //  cblAdditionalFilters.Items.Add(new ListItem(string.Format(CultureInfo.InvariantCulture, "{0}", f.Title), f.Id.ToString()));
                        }
                    }
                    else
                    {
                        if (basic_field(f))
                        {
                            cblAdditionalFields.Items.Add(new ListItem(string.Format(CultureInfo.InvariantCulture, "{0}", f.Title), f.Id.ToString()));
                        }
                    }
                }
            }

            if (cblAdditionalFilters.Items.Count > 0)
            {
                lbAdditionalFilters.Visible = true;
                cblAdditionalFilters.Visible = true;
            }
            else
            {
                lbAdditionalFilters.Visible = false;
                cblAdditionalFilters.Visible = false;
            }
            cblAdditionalFields.Visible = true;
            chkSortByView.Visible = true;
            chkAddingNewValues.Visible = true;
            chkUseNewForm.Visible = true;
            pnlConvertFromRegular.Visible = false;
        }

        protected void OptedforAdvanceSettings(Object sender, EventArgs args)
        {
            cblAdditionalFields.Items.Clear();
            cblAdditionalFilters.Items.Clear();
            if (cbxAdvanceSettings.Checked)
            {
                InitializeFieldAdvanceSettings(listTargetWeb.SelectedItem.Value, listTargetList.SelectedItem.Value);
                InitializeFieldViews(null, listTargetWeb.SelectedItem.Value, listTargetList.SelectedItem.Value);
            }
            else
            {
                cblAdditionalFields.Visible = false;
                lbAdditionalFilters.Visible = false;
                cblAdditionalFilters.Visible = false;
                chkSortByView.Visible = false;
                pnlConvertFromRegular.Visible = false;
                ddlView.Visible = false;
                lbView.Visible = false;
                chkAddingNewValues.Visible = false;
                chkUseNewForm.Visible = false;
            }
        }

        protected void SelectedViewChanged(object sender, EventArgs args)
        {
            if (ddlView.SelectedIndex > 0)
            {
                chkSortByView.Enabled = true;
            }
            else
            {
                chkSortByView.Enabled = false;
            }
        }

        protected void SelectedAddNewValues(object sender, EventArgs args)
        {
            if (chkAddingNewValues.Checked)
            {
                chkUseNewForm.Enabled = true;
            }
            else
            {
                chkUseNewForm.Enabled = false;
            }
        }

        protected void Relationship_behavior_changed(object sender, EventArgs args)
        {
            if (cbxRelationshipBehavior.Checked)
            {
                rdbRestrictDelete.Enabled = true;
                rdbCascadeDelete.Enabled = true;
            }
            else
            {
                rdbRestrictDelete.Enabled = false;
                rdbCascadeDelete.Enabled = false;
            }
        }

        protected void Restrict_delete_behavior_changed(object sender, EventArgs args)
        {
            if (rdbRestrictDelete.Checked)
            {
                rdbCascadeDelete.Checked = false;
            }
        }

        protected void Cascade_delete_behavior_changed(object sender, EventArgs args)
        {
            if (rdbCascadeDelete.Checked)
            {
                rdbRestrictDelete.Checked = false;
            }
        }

        protected void Allow_multiple_values_changed(object sender, EventArgs args)
        {
            if (cbxMultipleValues.Checked)
            {
                cbxRelationshipBehavior.Enabled = false;
                rdbRestrictDelete.Enabled = false;
                rdbCascadeDelete.Enabled = false;
            }
            else
            {
                cbxRelationshipBehavior.Enabled = true;
            }
        }

        private void SetTargetColumn(string webId, string selectedListId)
        {
            listTargetColumn.Items.Clear();
            listParentColumn.Items.Clear();
            listLinkColumn.Items.Clear();
            if (!string.IsNullOrEmpty(webId) && !string.IsNullOrEmpty(selectedListId))
            {
                SPSite _site = SPControl.GetContextSite(this.Context);
                SPWeb _web = _site.OpenWeb(new Guid(webId));
                SPList list = _web.Lists[new Guid(selectedListId)];
                SPList curList = SPContext.Current.List;
                SPFieldCollection fields = list.Fields;
                SPFieldCollection curFields = curList.Fields;

                List<ListItem> str = new List<ListItem>();
                List<ListItem> str1 = new List<ListItem>();
                List<ListItem> str2 = new List<ListItem>();
                str1.Clear();
                str2.Clear();

                foreach (SPField f in curFields)
                {
                    if (CanFieldBeDisplayed(f))
                    {
                        bool found = false;
                        foreach (SPField f1 in fields)
                        {
                            string typename = f.TypeDisplayName;
                            bool temp = Convert.ToString(f.GetCustomProperty(CustomDropDownList.LINK)) == Boolean.TrueString;

                            if (((typename.Equals("CustomDropDownList") && !temp) || f.Type == SPFieldType.Lookup) &&
                            (f1.Type == SPFieldType.Lookup || f1.TypeAsString == "Lookup" ||f1.TypeAsString == "CustomDropDownList"))
                            {
                                string left = ((SPFieldLookup)f1).LookupList.ToString();
                                left = left.TrimEnd('}');
                                left = left.TrimStart('{').ToLower();
                                
                                string right = ((SPFieldLookup)f).LookupList.ToString();
                                right = right.TrimEnd('}');
                                right = right.TrimStart('{').ToLower();
                               
                                if( left.Equals(right))
                                {
                                    found = true;
                                    break;
                                }
                            }
                        }
                        if (found)
                        {
                           str1.Add(new ListItem(string.Format(CultureInfo.InvariantCulture, "{0}", f.Title), f.Id.ToString()));
                        }
                    }
                }

                string parent_column_lookup_id = null;
                if (str1.Count != 0)
                {
                    parent_column_lookup_id = ((SPFieldLookup)curFields[str1.ToArray()[0].Text]).LookupList.ToString();
                }
                foreach (SPField f in fields)
                {
                    if (CanFieldBeDisplayed(f))
                    {
                        if(parent_column_lookup_id != null)
                        {
                            string typename =  f.TypeDisplayName;
                            bool temp = Convert.ToString(f.GetCustomProperty(CustomDropDownList.LINK)) == Boolean.TrueString;

                            if ((typename.Equals("CustomDropDownList") && !temp) || f.Type == SPFieldType.Lookup)
                            {
                                string f_lookuplist = ((SPFieldLookup)f).LookupList.ToString();
                                f_lookuplist = f_lookuplist.TrimEnd('}');
                                f_lookuplist = f_lookuplist.TrimStart('{');
                                parent_column_lookup_id = parent_column_lookup_id.TrimEnd('}');
                                parent_column_lookup_id = parent_column_lookup_id.TrimStart('{');
                                if (parent_column_lookup_id.ToLower() == f_lookuplist.ToLower())
                                {
                                    str2.Add(new ListItem(string.Format(CultureInfo.InvariantCulture, "{0}", f.Title), f.Id.ToString()));
                                }
                            }
                        }

                        if (!f.TypeDisplayName.Equals("CustomDropDownList") && f.Type != SPFieldType.Lookup && basic_field(f))
                        {
                            str.Add(new ListItem(
                              string.Format(CultureInfo.InvariantCulture, "{0}", f.Title), f.Id.ToString()));
                        }
                    }
                }
                if (str.Count > 0)
                {
                    str.Sort(delegate(ListItem item1, ListItem item2)
                    {
                        return item1.Text.CompareTo(item2.Text);
                    });

                    listTargetColumn.Items.AddRange(str.ToArray());
                   
                    ListItem bitem = null;
                    if (!string.IsNullOrEmpty(TargetColumnId)) { bitem = listTargetColumn.Items.FindByValue(TargetColumnId); }
                    if (bitem != null) { listTargetColumn.SelectedIndex = listTargetColumn.Items.IndexOf(bitem); }
                    else { listTargetColumn.SelectedIndex = 0; } 
                    
                }
                if (str1.Count > 0)
                {
                    str1.Sort(delegate(ListItem item1, ListItem item2)
                    {
                        return item1.Text.CompareTo(item2.Text);
                    });

                    listParentColumn.Items.AddRange(str1.ToArray());

                    if (listParentColumn.SelectedIndex < 0)
                    {
                        listParentColumn.SelectedIndex = 0;
                    }

                    ListItem bitem = null;
                    if (!string.IsNullOrEmpty(TargetParentColumnId)) { bitem = listParentColumn.Items.FindByValue(TargetParentColumnId); }
                    if (bitem != null) { listParentColumn.SelectedIndex = listParentColumn.Items.IndexOf(bitem); }
                    else { listParentColumn.SelectedIndex = 0; } 
                }
                if (str2.Count > 0)
                {
                    str2.Sort(delegate(ListItem item1, ListItem item2)
                    {
                        return item1.Text.CompareTo(item2.Text);
                    });
                   
                    listLinkColumn.Items.AddRange(str2.ToArray());

                    ListItem bitem = null;
                    if (!string.IsNullOrEmpty(TargetLinkColumnId)) { bitem = listLinkColumn.Items.FindByValue(TargetLinkColumnId); }
                    if (bitem != null) { listLinkColumn.SelectedIndex = listLinkColumn.Items.IndexOf(bitem); }
                    else { listLinkColumn.SelectedIndex = 0; }
                    
                }

            }
            if (listParentColumn.Items.Count == 0 || listTargetList.SelectedValue == SPContext.Current.ListId.ToString() || !cbxLinkParent.Checked)
            {
                InitialParentColumnValues_with_false();
                cbxLinkParent.Enabled = false;
            }
            else
            {
                InitialParentColumnValues_with_true();
                cbxLinkParent.Enabled = true;
            }
        }

        private void SetTargetList(string selectedWebId, bool setTargetColumn)
        {
            listTargetList.Items.Clear();
            if (!string.IsNullOrEmpty(selectedWebId))
            {
                SPSite _site = SPControl.GetContextSite(this.Context);
                SPWeb _web = _site.OpenWeb(new Guid(selectedWebId));
                List<ListItem> str = new List<ListItem>();
                SPListCollection _listCollection = _web.Lists;
                foreach (SPList list in _listCollection)
                {
                    if (!list.Hidden)
                    {
                        str.Add(new ListItem(list.Title, list.ID.ToString()));
                    }
                }
                if (str.Count > 0)
                {
                    str.Sort(delegate(ListItem item1, ListItem item2)
                    {
                        return item1.Text.CompareTo(item2.Text);
                    });

                    SPList curList = SPContext.Current.List;
                    //str.Remove(new ListItem(curList.Title, curList.ID.ToString()));
                    listTargetList.Items.AddRange(str.ToArray());

                    ListItem bitem = null;
                    if (!string.IsNullOrEmpty(TargetListId)) { bitem = listTargetList.Items.FindByValue(TargetListId); }
                    if (bitem != null) { listTargetList.SelectedIndex = listTargetList.Items.IndexOf(bitem); }
                    else { listTargetList.SelectedIndex = 0; }

                    if (setTargetColumn)
                    {
                        SetTargetColumn(selectedWebId, listTargetList.SelectedItem.Value);
                    }

                    /*SetTargetListView(selectedWebId, listTargetList.SelectedItem.Value); */
                }
            }
        }

        private void SetTargetWeb()
        {
            listTargetWeb.Items.Clear();
            List<ListItem> str = new List<ListItem>();

            SPSite _site = SPControl.GetContextSite(this.Context);

            SPWebCollection _webCollection = _site.AllWebs;
            string contextWebId = SPControl.GetContextWeb(this.Context).ID.ToString();
            foreach (SPWeb web in _webCollection)
            {
                if (web.DoesUserHavePermissions(
                  SPBasePermissions.ViewPages | SPBasePermissions.OpenItems | SPBasePermissions.ViewListItems))
                {
                    str.Add(new ListItem(web.Title, web.ID.ToString()));
                }
            }
            if (str.Count > 0)
            {
                str.Sort(delegate(ListItem item1, ListItem item2)
                {
                    return item1.Text.CompareTo(item2.Text);
                });

                listTargetWeb.Items.AddRange(str.ToArray());
                ListItem bitem = null;
                if (!string.IsNullOrEmpty(TargetWebId)) { bitem = listTargetWeb.Items.FindByValue(TargetWebId); }
                else { bitem = listTargetWeb.Items.FindByValue(contextWebId); }
                if (bitem != null) { listTargetWeb.SelectedIndex = listTargetWeb.Items.IndexOf(bitem); }
                else { listTargetWeb.SelectedIndex = 0; }

                SetTargetList(listTargetWeb.SelectedItem.Value, true);
            }

        }

        private bool basic_field(SPField f)
        {
            if (f.InternalName.Equals("ID") || f.InternalName.Equals("Created") || f.InternalName.Equals("Author") || f.InternalName.Equals("Modified") ||
                f.InternalName.Equals("Editor") || f.InternalName.Equals("_UIVersionString") || f.InternalName.Equals("Title"))
            {
                return true;
            }
            else if (this.CanFieldBeDisplayed(f))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool CanFieldBeDisplayed(SPField f)
        {
            bool retval = false;
            if (f != null && !f.Hidden && (Array.IndexOf<string>(
              EXCLUDED_FIELDS, f.InternalName) < 0))
            {
                string inter = f.InternalName;
                switch (f.Type)
                {
                    case SPFieldType.Computed:
                        if (((SPFieldComputed)f).EnableLookup) { retval = true; }
                        break;
                    case SPFieldType.Calculated:
                        if (((SPFieldCalculated)f).OutputType == SPFieldType.Text) { retval = true; }
                        break;
                    default:
                        retval = true;
                        break;
                }
            }

            return retval;
        } 

        private void SetControlVisibility()
        {
            string referrer = this.Request.Url.AbsoluteUri;

            if (!string.IsNullOrEmpty(referrer))
            {
                if (referrer.IndexOf("_layouts/fldNew.aspx") > -1
                  || referrer.IndexOf("_layouts/FldNewEx.aspx") > -1) // adding new field
                {
                    listTargetList.Visible = true;
                    listTargetWeb.Visible = true;
                }
                else
                {
                    //lblTargetWeb.Visible = true;
                   // listTargetList.Visible = true;
                    listTargetList.Enabled = false;
                   // listTargetWeb.Visible = false;
                    listTargetWeb.Enabled = false;
                }
            }
        }
        public bool DisplayAsNewSection { get { return true; } }

        public void InitializeWithField(SPField field)
        {
            EnsureChildControls();
            CustomDropDownList _f = null;
            try { _f = field as CustomDropDownList; }
            catch { }

            if (_f != null)
            {
                // this bit only happens when field is not null
                if (!IsPostBack)
                {
                    cbxMultipleValues.Checked = _f.AllowMultipleValues;
                    cbxLinkParent.Checked = Convert.ToBoolean(_f.GetCustomProperty(CustomDropDownList.LINK));
                    cbxParentEmpty.Checked = Convert.ToBoolean(_f.GetCustomProperty(CustomDropDownList.SHOW_ALL_VALUES));
                    if (cbxLinkParent.Checked)
                    {
                        InitialParentColumnValues_with_true();
                    }
                    else
                    {
                        InitialParentColumnValues_with_false();
                    }
                    cbxAutoCompleteORFilter.Checked = Convert.ToBoolean(_f.GetCustomProperty(CustomDropDownList.AUTO_COMPLETE));

                    TargetWebId = _f.LookupWebId.ToString();
                    TargetListId = _f.LookupList;
                    TargetColumnId = _f.LookupField;
                    TargetParentColumnId = Convert.ToString(_f.GetCustomProperty(CustomDropDownList.PARENT_COLUMN));
                    TargetLinkColumnId = Convert.ToString(_f.GetCustomProperty(CustomDropDownList.LINK_COLUMN));

                    cbxAdvanceSettings.Checked = Convert.ToBoolean(_f.GetCustomProperty(CustomDropDownList.ADVANCE_SETTINGS));

                    if (cbxAdvanceSettings.Checked)
                    {
                        cblAdditionalFields.Items.Clear();
                        cblAdditionalFilters.Items.Clear();
                        InitializeFieldAdvanceSettings(TargetWebId, TargetListId);
                        InitializeFieldViews(_f, TargetWebId, TargetListId);

                        if (!string.IsNullOrEmpty(Convert.ToString(_f.GetCustomProperty(CustomDropDownList.VIEW))))
                        {
                           // ddlView.SelectedIndex = ddlView.Items.IndexOf(ddlView.Items.FindByText(Convert.ToString(_f.GetCustomProperty(CustomDropDownList.VIEW))));
                            chkSortByView.Enabled = true;
                            chkSortByView.Checked = Convert.ToBoolean(_f.GetCustomProperty(CustomDropDownList.SORT_BY_VIEW));
                        }
                        
                        chkAddingNewValues.Checked = Convert.ToBoolean(_f.GetCustomProperty(CustomDropDownList.ADDING_NEW_VALUES));

                        if (chkAddingNewValues.Checked)
                        {
                            chkUseNewForm.Enabled = true;
                            chkUseNewForm.Checked = Convert.ToBoolean(_f.GetCustomProperty(CustomDropDownList.NEW_FORM));
                        }

                        string add_fields = Convert.ToString(_f.GetCustomProperty(CustomDropDownList.ADDITIONAL_FIELDS));
                        if (!string.IsNullOrEmpty(add_fields))
                        {
                            foreach (string s in add_fields.Split(';'))
                            {
                                int index = cblAdditionalFields.Items.IndexOf(cblAdditionalFields.Items.FindByValue(s));
                                cblAdditionalFields.Items[index].Selected = true;
                            }
                        }

                        string add_filters = Convert.ToString(_f.GetCustomProperty(CustomDropDownList.ADDITIONAL_FILTERS));
                        if (!string.IsNullOrEmpty(add_filters))
                        {
                            foreach (string s in add_filters.Split(';'))
                            {
                                int index = cblAdditionalFilters.Items.IndexOf(cblAdditionalFilters.Items.FindByValue(s));
                                cblAdditionalFilters.Items[index].Selected = true;
                            }
                        }

                    }
                    else
                    {
                        cblAdditionalFields.Visible = false;
                        lbAdditionalFilters.Visible = false;
                        cblAdditionalFilters.Visible = false;
                        chkSortByView.Visible = false;
                        pnlConvertFromRegular.Visible = false;
                        ddlView.Visible = false;
                        lbView.Visible = false;
                        chkAddingNewValues.Visible = false;
                        chkUseNewForm.Visible = false;
                    }

                    if (_f.AllowMultipleValues)
                    {
                        cbxRelationshipBehavior.Enabled = false;
                    }

                    if (Convert.ToBoolean(_f.GetCustomProperty(CustomDropDownList.RELATIONSHIP_BEHAVIOR)))
                    {
                        cbxRelationshipBehavior.Checked = true;
                        rdbRestrictDelete.Enabled = true;
                        rdbCascadeDelete.Enabled = true;

                        if (Convert.ToBoolean(_f.GetCustomProperty(CustomDropDownList.RELATIONSHIP_BEHAVIOR_CASCADE)))
                        {
                            rdbRestrictDelete.Checked = false;
                            rdbCascadeDelete.Checked = true;
                        }
                        else
                        {
                            rdbRestrictDelete.Checked = true;
                            rdbCascadeDelete.Checked = false;
                        }
                    }
                    else
                    {
                        cbxRelationshipBehavior.Checked = false;
                        rdbRestrictDelete.Enabled = false;
                        rdbRestrictDelete.Checked = true;
                        rdbCascadeDelete.Enabled = false;
                        rdbCascadeDelete.Checked = false;
                    }
                }
            }
            
            // this bit must always happen, even when field is null
            if (!IsPostBack)
            {
                SetTargetWeb();
                SetControlVisibility();
            }
        }

        public void OnSaveChange(SPField field, bool isNewField)
        {
            CustomDropDownList _f = null ;
            try { _f = field as CustomDropDownList; }
            catch { }
            SPSite _site = SPControl.GetContextSite(this.Context);
            SPWeb _web = _site.OpenWeb(new Guid(listTargetWeb.SelectedItem.Value));
            _f.LookupWebId = _web.ID;
            if (listTargetList.SelectedIndex < 0)
            {
                listTargetList.SelectedIndex = 0;
            }
            
            _f.LookupList = listTargetList.SelectedItem.Value;
            _f.LookupField = listTargetColumn.SelectedItem.Value;
            _f.SetCustomProperty(CustomDropDownList.LINK, cbxLinkParent.Checked);
            _f.SetCustomProperty(CustomDropDownList.ALLOW_MULTIPLE, cbxMultipleValues.Checked);
            _f.SetCustomProperty(CustomDropDownList.SHOW_ALL_VALUES, cbxParentEmpty.Checked);
            if (cbxLinkParent.Checked && cbxLinkParent.Enabled)
            {
                //  if (listParentColumn.Items.Count != 0)
                {
                    _f.SetCustomProperty(CustomDropDownList.PARENT_COLUMN, listParentColumn.SelectedItem.Value);
                }
                /*  else
                  {
                      _f.SetCustomProperty(CustomDropDownList.PARENT_COLUMN, "");
                  }
                  if (listLinkColumn.Items.Count != 0)*/
                {
                    _f.SetCustomProperty(CustomDropDownList.LINK_COLUMN, listLinkColumn.SelectedItem.Value);
                }
                /* else
                 {
                     _f.SetCustomProperty(CustomDropDownList.LINK_COLUMN, "");
                 }*/
            }
            else
            {
                _f.SetCustomProperty(CustomDropDownList.PARENT_COLUMN, "");
                _f.SetCustomProperty(CustomDropDownList.LINK_COLUMN, "");
            }

            _f.SetCustomProperty(CustomDropDownList.AUTO_COMPLETE, cbxAutoCompleteORFilter.Checked);

            _f.SetCustomProperty(CustomDropDownList.ADVANCE_SETTINGS, cbxAdvanceSettings.Checked);

            if (cbxAdvanceSettings.Checked)
            {
                if (!string.IsNullOrEmpty(ddlView.SelectedItem.Value))
                {
                    _f.SetCustomProperty(CustomDropDownList.VIEW, ddlView.SelectedItem.Value);
                }

                _f.SetCustomProperty(CustomDropDownList.SORT_BY_VIEW, chkSortByView.Checked);
                _f.SetCustomProperty(CustomDropDownList.ADDING_NEW_VALUES, chkAddingNewValues.Checked);
                _f.SetCustomProperty(CustomDropDownList.NEW_FORM, chkUseNewForm.Checked);

                string checked_additional_fields = string.Empty;
                string unchecked_additional_fields = string.Empty;
                foreach (ListItem item in cblAdditionalFields.Items)
                {
                    if (item.Selected)
                    {
                        if (!string.IsNullOrEmpty(checked_additional_fields))
                        {
                            checked_additional_fields = checked_additional_fields + ";";
                        }
                        checked_additional_fields = checked_additional_fields + item.Value;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(unchecked_additional_fields))
                        {
                            unchecked_additional_fields = unchecked_additional_fields + ";";
                        }
                        unchecked_additional_fields = unchecked_additional_fields + item.Value;
                    }
                }
                _f.SetCustomProperty(CustomDropDownList.ADDITIONAL_FIELDS, checked_additional_fields);

                _f.EnsureAdditionalFields(checked_additional_fields, unchecked_additional_fields);
                
                string str = string.Empty;
                foreach (ListItem item in cblAdditionalFilters.Items)
                {
                    if (item.Selected)
                    {
                        if (!string.IsNullOrEmpty(str))
                        {
                            str = str + ";";
                        }
                        str = str + item.Value;
                    }
                }
                _f.SetCustomProperty(CustomDropDownList.ADDITIONAL_FILTERS, str);
            }

            _f.SetCustomProperty(CustomDropDownList.RELATIONSHIP_BEHAVIOR, cbxRelationshipBehavior.Checked);
            _f.SetCustomProperty(CustomDropDownList.RELATIONSHIP_BEHAVIOR_CASCADE, rdbCascadeDelete.Checked);
            if (!_f.AllowMultipleValues && cbxRelationshipBehavior.Checked)
            {
                if (rdbCascadeDelete.Checked)
                {
                    _f.RelationshipDeleteBehavior = SPRelationshipDeleteBehavior.Cascade;
                }
                else if (rdbRestrictDelete.Checked)
                {
                    _f.RelationshipDeleteBehavior = SPRelationshipDeleteBehavior.Restrict;
                }
                else
                {
                    _f.RelationshipDeleteBehavior = SPRelationshipDeleteBehavior.None;
                }

                _f.Indexed = true;
            }
            else
            {
                _f.RelationshipDeleteBehavior = SPRelationshipDeleteBehavior.None;
                _f.Indexed = false;
            }
            
        }

        public void InitialParentColumnValues_with_true()
        {
            cbxLinkParent.Checked = true;
            lblParentColumn.Visible = true;
            listParentColumn.Visible = true;
            lblLinkColumn.Visible = true;
            listLinkColumn.Visible = true;
            lbllistLinkColumn.Visible = true;
            cbxParentEmpty.Enabled = true;
        }

        public void InitialParentColumnValues_with_false()
        {
            cbxLinkParent.Checked = false;
            lblParentColumn.Visible = false;
            listParentColumn.Visible = false;
            lblLinkColumn.Visible = false;
            listLinkColumn.Visible = false;
            lbllistLinkColumn.Visible = false;
            cbxParentEmpty.Enabled = false;
        }

        private void InitializeFieldViews(SPField field, string webId, string listId)
        {
            SPSite _site = SPControl.GetContextSite(this.Context);
            SPWeb _web = _site.OpenWeb(new Guid(webId));
            SPList list = _web.Lists[new Guid(listId)];

            foreach (SPView view in list.Views)
            {
                if ((view.Hidden || view.PersonalView) || !view.Type.Equals("HTML"))
                {
                    continue;
                }
                ListItem item = new ListItem(view.Title, view.ID.ToString() + "|" + view.Url);
                string viewId = string.Empty;

                if (field != null)
                {
                    CustomDropDownList f = field as CustomDropDownList;
                    if (Convert.ToString(f.GetCustomProperty(CustomDropDownList.VIEW)) != string.Empty)
                    {
                        viewId = Convert.ToString(f.GetCustomProperty(CustomDropDownList.VIEW));
                    }
                }

                if (((view.ID.ToString() == viewId) || (view.ID.ToString() + "|" + view.Url == viewId)))
                {
                    item.Selected = true;
                }
                ddlView.Items.Add(item);
            }

            ddlView.Items.Insert(0, new ListItem("", ""));
            ddlView.Visible = true;
            lbView.Visible = true;
        }

        private string TargetWebId
        {
            get
            {
                object o = this.ViewState["TARGET_WEB_ID"];
                return (o != null && !string.IsNullOrEmpty(o.ToString())) ? o.ToString() : string.Empty;
            }
            set { this.ViewState["TARGET_WEB_ID"] = value; }
        }

        private string TargetListId
        {
            get
            {
                object o = this.ViewState["TARGET_LIST_ID"];
                return (o != null && !string.IsNullOrEmpty(o.ToString())) ? o.ToString() : string.Empty;
            }
            set { this.ViewState["TARGET_LIST_ID"] = value; }
        }
        private string TargetColumnId
        {
            get
            {
                object o = this.ViewState["TARGET_COLUMN_ID"];
                return (o != null && !string.IsNullOrEmpty(o.ToString())) ? o.ToString() : string.Empty;
            }
            set { this.ViewState["TARGET_COLUMN_ID"] = value; }
        }
        private string TargetParentColumnId
        {
            get
            {
                object o = this.ViewState["TARGET_PARENT_COLUMN_ID"];
                return (o != null && !string.IsNullOrEmpty(o.ToString())) ? o.ToString() : string.Empty;
            }
            set { this.ViewState["TARGET_PARENT_COLUMN_ID"] = value; }
        }
        private string TargetLinkColumnId
        {
            get
            {
                object o = this.ViewState["TARGET_LINK_COLUMN_ID"];
                return (o != null && !string.IsNullOrEmpty(o.ToString())) ? o.ToString() : string.Empty;
            }
            set { this.ViewState["TARGET_LINK_COLUMN_ID"] = value; }
        }
        
    }
}
