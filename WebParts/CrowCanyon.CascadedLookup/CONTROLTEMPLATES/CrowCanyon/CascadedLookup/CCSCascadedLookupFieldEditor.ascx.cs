using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint;

namespace CrowCanyon.CascadedLookup
{
    public partial class CCSCascadedLookupFieldEditor : UserControl, IFieldEditor
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        CCSCascadedLookupField _ccsCascadedField = null;

        #region Events
        protected void ddlWeb_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (new EnterExitLogger("CCSCascadedLookupFieldEditor:ddlWeb_SelectedIndexChanged function"))
            {
                PopulateList(ddlWeb.SelectedItem.Value);
            }
        }

        protected void ddlList_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (new EnterExitLogger("CCSCascadedLookupFieldEditor:ddlList_SelectedIndexChanged function"))
            {
                PopulateColumns(ddlWeb.SelectedItem.Value, ddlList.SelectedItem.Value);
            }
        }

        protected void cbxLinkParent_CheckedChanged(object sender, EventArgs e)
        {
            using (new EnterExitLogger("CCSCascadedLookupFieldEditor:cbxLinkParent_CheckedChanged function"))
            {
                SetLinkedParentControl(cbxLinkParent.Checked);
            }
        }

        protected void ddlParentColumn_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (new EnterExitLogger("CCSCascadedLookupFieldEditor:ddlParentColumn_SelectedIndexChanged function"))
            {
                SetLinkColumnValue(ddlParentColumn.SelectedItem);
            }
        }

        protected void cbxAllowMultiple_CheckedChanged(object sender, EventArgs e)
        {
            using (new EnterExitLogger("CCSCascadedLookupFieldEditor:cbxAllowMultiple_CheckedChanged function"))
            {
                if (cbxAllowMultiple.Checked)
                {
                    cbxRelationshipBehavior.Enabled = false;
                    rbRestrictDelete.Enabled = false;
                    rbCascadeDelete.Enabled = false;
                }
                else
                {
                    cbxRelationshipBehavior.Enabled = true;
                }
            }
        }

        protected void cbxAdvanceSettings_CheckedChanged(object sender, EventArgs e)
        {
            using (new EnterExitLogger("CCSCascadedLookupFieldEditor:cbxAdvanceSettings_CheckedChanged function"))
            {
                AdditionSettingPane.Visible = cbxAdvanceSettings.Checked;

                if (cbxAdvanceSettings.Checked)
                {
                    string referrer = this.Request.Url.AbsoluteUri;
                    if (referrer.IndexOf("_layouts/fldNew.aspx") > -1 || referrer.IndexOf("_layouts/FldNewEx.aspx") > -1)
                    {
                        pnlConvertFromLookup.Visible = true;
                        pnlConvertToLookup.Visible = false;
                    }
                    else
                    {
                        pnlConvertFromLookup.Visible = false;
                        pnlConvertToLookup.Visible = true;
                    }
                }
                else
                {
                    pnlConvertFromLookup.Visible = false;
                    pnlConvertToLookup.Visible = false;
                }
            }
        }

        protected void ddlView_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (new EnterExitLogger("CCSCascadedLookupFieldEditor:ddlView_SelectedIndexChanged function"))
            {
                cbxSortByView.Enabled = (ddlView.SelectedIndex > 0);
            }
        }

        protected void cbxAllowNewValues_CheckedChanged(object sender, EventArgs e)
        {
            using (new EnterExitLogger("CCSCascadedLookupFieldEditor:cbxAllowNewValues_CheckedChanged function"))
            {
                cbxUseNewForm.Enabled = cbxAllowNewValues.Checked;
            }
        }

        #endregion

        #region Relationship pane Controls Event

        protected void cbxRelationshipBehavior_CheckedChanged(object sender, EventArgs e)
        {
            if (cbxRelationshipBehavior.Checked)
            {
                rbRestrictDelete.Enabled = true;
                rbCascadeDelete.Enabled = true;
            }
            else
            {
                rbRestrictDelete.Enabled = false;
                rbCascadeDelete.Enabled = false;
            }
        }

        protected void rbRestrictDelete_CheckedChanged(object sender, EventArgs e)
        {
            if (rbRestrictDelete.Checked)
            {
                rbCascadeDelete.Checked = false;
            }
        }

        protected void rbCascadeDelete_CheckedChanged(object sender, EventArgs e)
        {
            if (rbCascadeDelete.Checked)
            {
                rbRestrictDelete.Checked = false;
            }
        }

        #endregion


        #region IFieldEditor implementation

        public bool DisplayAsNewSection
        {
            get { return true; } 
        }

        public void InitializeWithField(SPField field)
        {
            using (new EnterExitLogger("CCSCascadedLookupFieldEditor:InitializeWithField function"))
            {
                try
                {
                    ErrorText.Visible = false;
                    _ccsCascadedField = field as CCSCascadedLookupField;
                    if (!IsPostBack)
                    {
                        PopulateAndSetValuesControls();

                        SetCheckboxcontrolsValue();

                        AddingNewFieldControlVisibility();

                        SetRelationShipControlsValue();

                        //if (field != null) ;
                    }
                }
                catch (Exception ex)
                {
                    Utils.LogManager.write("Exception Occurs in InitializeWithField Function. \r\nError Message: " + ex.Message + "\r\nStack Trace: " + ex.StackTrace, "error");
                    ShowErrorMessage(ex.Message);
                }
            }
        }

        public void OnSaveChange(SPField field, bool isNewField)
        {
            using (new EnterExitLogger("CCSCascadedLookupFieldEditor:OnSaveChange function"))
            {
                try
                {

                    CCSCascadedLookupField ccscascadeField = field as CCSCascadedLookupField;
                    if (ccscascadeField != null)
                    {
                        SPSecurity.RunWithElevatedPrivileges(delegate
                            {
                                using (SPWeb selWeb = SPContext.Current.Site.OpenWeb(new Guid(ddlWeb.SelectedItem.Value)))
                                {
                                    ccscascadeField.LookupWebId = selWeb.ID;
                                    Utils.LogManager.write("ccscascadeField.LookupWebId: " + selWeb.ID.ToString());
                                }
                            });

                        ccscascadeField.LookupList = (ddlList.SelectedItem != null ? ddlList.SelectedItem.Value : "");
                        Utils.LogManager.write("ccscascadeField.LookupList: " + ddlList.SelectedItem.Value);

                        ccscascadeField.LookupField = (ddlColumn.SelectedItem != null ? ddlColumn.SelectedItem.Value : "");
                        Utils.LogManager.write("ccscascadeField.LookupField: " + ddlColumn.SelectedItem.Value);

                        ccscascadeField.SourceWebID = ddlWeb.SelectedItem.Value;
                        Utils.LogManager.write("ccscascadeField.SourceWebID: " + ddlWeb.SelectedItem.Value);

                        ccscascadeField.LookupFieldListName = (ddlList.SelectedItem != null ? ddlList.SelectedItem.Value : "");
                        Utils.LogManager.write("ccscascadeField.LookupFieldListName: " + (ddlList.SelectedItem != null ? ddlList.SelectedItem.Value : ""));
                        
                        ccscascadeField.LookupFieldName = (ddlColumn.SelectedItem != null ? ddlColumn.SelectedItem.Value : "");
                        Utils.LogManager.write("ccscascadeField.LookupFieldName: " + (ddlColumn.SelectedItem != null ? ddlColumn.SelectedItem.Value : ""));
                        
                        ccscascadeField.ParentLinkedColumnName = (ddlParentColumn.SelectedItem != null ? ddlParentColumn.SelectedItem.Value : "");
                        Utils.LogManager.write("ccscascadeField.ParentLinkedColumnName: " + (ddlParentColumn.SelectedItem != null ? ddlParentColumn.SelectedItem.Value : ""));

                        ccscascadeField.AllowMultipleValues = cbxAllowMultiple.Checked;
                        Utils.LogManager.write("ccscascadeField.AllowMultipleValues: " + cbxAllowMultiple.Checked.ToString());
                        
                        ccscascadeField.AdvancedSetting = cbxAdvanceSettings.Checked;
                        Utils.LogManager.write("ccscascadeField.AdvancedSetting: " + cbxAdvanceSettings.Checked.ToString());
                        
                        ccscascadeField.View = (ddlView.SelectedItem != null ? ddlView.SelectedItem.Value : "");
                        Utils.LogManager.write("ccscascadeField.View: " + (ddlView.SelectedItem != null ? ddlView.SelectedItem.Value : ""));
                        
                        ccscascadeField.LinkToParent = cbxLinkParent.Checked;
                        Utils.LogManager.write("ccscascadeField.LinkToParent: " + cbxLinkParent.Checked.ToString());
                        
                        ccscascadeField.ShowAllOnEmpty = cbxShowallParentEmpty.Checked;
                        Utils.LogManager.write("ccscascadeField.ShowAllOnEmpty: " + cbxShowallParentEmpty.Checked.ToString());
                        
                        ccscascadeField.AllowNewEntry = cbxAllowNewValues.Checked;
                        Utils.LogManager.write("ccscascadeField.AllowNewEntry: " + cbxAllowNewValues.Checked.ToString());
                        
                        ccscascadeField.UseNewForm = cbxUseNewForm.Checked;
                        Utils.LogManager.write("ccscascadeField.UseNewForm: " + cbxUseNewForm.Checked.ToString());

                        ccscascadeField.SortByView = cbxSortByView.Checked;
                        Utils.LogManager.write("ccscascadeField.SortByView: " + cbxSortByView.Checked.ToString());
                        
                        ccscascadeField.AllowAutocomplete = false;
                        Utils.LogManager.write("ccscascadeField.AllowAutocomplete: False");

                        if (cbxRelationshipBehavior.Enabled && cbxRelationshipBehavior.Checked)
                        {
                            if (rbRestrictDelete.Enabled && rbRestrictDelete.Checked)
                            {
                                ccscascadeField.RelationshipDeleteBehavior = SPRelationshipDeleteBehavior.Restrict;
                                Utils.LogManager.write("ccscascadeField.RelationshipDeleteBehavior: Restrict");
                            }
                            else if (rbCascadeDelete.Enabled && rbCascadeDelete.Checked)
                            {
                                ccscascadeField.RelationshipDeleteBehavior = SPRelationshipDeleteBehavior.Cascade;
                                Utils.LogManager.write("ccscascadeField.RelationshipDeleteBehavior: Cascade");
                            }
                            else
                            {
                                ccscascadeField.RelationshipDeleteBehavior = SPRelationshipDeleteBehavior.None;
                                Utils.LogManager.write("ccscascadeField.RelationshipDeleteBehavior: None");
                            }
                        }
                        else
                        {
                            ccscascadeField.RelationshipDeleteBehavior = SPRelationshipDeleteBehavior.None;
                            Utils.LogManager.write("ccscascadeField.RelationshipDeleteBehavior: None");
                        }

                        ccscascadeField.AdditionalFilters = GetAdditonalFilters();
                        Utils.LogManager.write("ccscascadeField.AdditionalFilters: " + ccscascadeField.AdditionalFilters);

                        if (isNewField)
                        {
                            ccscascadeField.AdditionalFields = GetAdditonalFields();
                            Utils.LogManager.write("ccscascadeField.AdditionalFields: " + ccscascadeField.AdditionalFields);
                        }
                        else
                        {
                            ccscascadeField.AdditionalFields = "";
                            if (cblAdditionalFields.Items != null && cblAdditionalFields.Items.Count > 0)
                            {
                                foreach (ListItem li in cblAdditionalFields.Items)
                                {
                                    if (li.Selected)
                                    {
                                        if (!ccscascadeField.ParentList.Fields.ContainsField(ccscascadeField.Title + " : " + li.Text))
                                        {
                                            //create a new field
                                            Utils.LogManager.write("Creating AddintionField Name: " + ccscascadeField.Title + " : " + li.Text);
                                            string depLookUp = ccscascadeField.ParentList.Fields.AddDependentLookup(ccscascadeField.Title + " : " + li.Text, ccscascadeField.Id);
                                            SPFieldLookup fieldDepLookup = (SPFieldLookup)ccscascadeField.ParentList.Fields.GetFieldByInternalName(depLookUp);

                                            if (fieldDepLookup != null)
                                            {
                                                fieldDepLookup.LookupWebId = ccscascadeField.LookupWebId;
                                                fieldDepLookup.LookupField = li.Value;
                                                fieldDepLookup.Update();
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (ccscascadeField.ParentList.Fields.ContainsField(ccscascadeField.Title + " : " + li.Text))
                                        {
                                            //delete field if exist
                                            Utils.LogManager.write("Deleting AddintionField Name: " + ccscascadeField.Title + " : " + li.Text);
                                            ccscascadeField.ParentList.Fields.GetField(ccscascadeField.Title + " : " + li.Text).Delete();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Utils.LogManager.write("Exception Occurs in OnSaveChanges Function. \r\nError Message: " + ex.Message + "\r\nStack Trace: " + ex.StackTrace, "error");
                }
            }
        }

        #endregion

        #region Get Control Values

        private SPList GetList(string webId, string listId)
        {
            using (new EnterExitLogger("CCSCascadedLookupFieldEditor:GetList function"))
            {
                Utils.LogManager.write("Parameters webId : " + webId + ", listId: " + listId);
                SPList list = null;
                SPSecurity.RunWithElevatedPrivileges(delegate
                    {
                        using (SPWeb selWeb = SPContext.Current.Site.OpenWeb(new Guid(webId)))
                        {
                            list = selWeb.Lists[new Guid(listId)];
                        }
                    });

                return list;
            }
        }

        private List<ListItem> GetWebCollection()
        {
            using (new EnterExitLogger("CCSCascadedLookupFieldEditor:GetWebCollection function"))
            {
                List<ListItem> webList = new List<ListItem>();

                SPSecurity.RunWithElevatedPrivileges(delegate
                    {
                        using (SPSite site = new SPSite(SPContext.Current.Site.ID))
                        {
                            foreach (SPWeb web in site.AllWebs)
                            {
                                if (web.DoesUserHavePermissions(SPBasePermissions.ViewPages | SPBasePermissions.OpenItems | SPBasePermissions.ViewListItems))
                                {
                                    webList.Add(new ListItem(web.Title, web.ID.ToString()));
                                }
                            }
                        }
                    });

                if (webList.Count > 0)
                {
                    webList.Sort(delegate(ListItem item1, ListItem item2)
                    {
                        return item1.Text.CompareTo(item2.Text);
                    });
                }

                return webList;
            }
        }

        private List<ListItem> GetListCollection(string WebId)
        {
            using (new EnterExitLogger("CCSCascadedLookupFieldEditor:GetListCollection function"))
            {
                List<ListItem> spListList = new List<ListItem>();

                if (!string.IsNullOrEmpty(WebId))
                {
                    Utils.LogManager.write("Parameters WebId: " + WebId);
                    SPListCollection spListColl = null;
                    SPSecurity.RunWithElevatedPrivileges(delegate
                        {
                            using (SPWeb CurrentWeb = SPContext.Current.Site.OpenWeb(new Guid(WebId)))
                            {
                                spListColl = CurrentWeb.Lists;
                            }
                        });

                    foreach (SPList list in spListColl)
                    {
                        if (!list.Hidden)
                        {
                            spListList.Add(new ListItem(list.Title, list.ID.ToString()));
                        }
                    }

                    if (spListList.Count > 0)
                    {
                        spListList.Sort(delegate(ListItem item1, ListItem item2)
                        {
                            return item1.Text.CompareTo(item2.Text);
                        });
                    }
                }
                return spListList;
            }
        }

        private List<ListItem> GetColumnCollection(string WebId, string ListId)
        {
            using (new EnterExitLogger("CCSCascadedLookupFieldEditor:GetColumnCollection function"))
            {
                List<ListItem> columnList = new List<ListItem>();
                if (!string.IsNullOrEmpty(WebId))
                {
                    Utils.LogManager.write("Parameters WebId: " + WebId + ", ListId: " + ListId);
                    SPList SelectedList = GetList(WebId, ListId);

                    SPFieldCollection Fields = SelectedList.Fields;

                    foreach (SPField field in Fields)
                    {
                        if (!Utilities.IsLookupType(field))
                        {
                            if (Utilities.GeneralFields(field))
                            {
                                //columnList.Add(new ListItem(field.Title, field.Id.ToString()));
                                columnList.Add(new ListItem(field.Title, field.InternalName));
                            }
                        }
                    }

                    if (columnList.Count > 0)
                    {
                        columnList.Sort(delegate(ListItem item1, ListItem item2)
                        {
                            return item1.Text.CompareTo(item2.Text);
                        });

                    }
                }

                return columnList;
            }
        }

        private List<ListItem> GetParentLinkedColumnCollection(string WebId, string ListId)
        {
            List<string> TitleFieldNames = new List<string>();
            TitleFieldNames.Add("title");
            TitleFieldNames.Add("linktitlenomenu");
            TitleFieldNames.Add("linktitle");
            TitleFieldNames.Add("linktitle2");

            using (new EnterExitLogger("CCSCascadedLookupFieldEditor:GetParentLinkedColumnCollection function"))
            {
                List<ListItem> columnList = new List<ListItem>();
                if (!string.IsNullOrEmpty(WebId))
                {
                    Utils.LogManager.write("Parameters WebId: " + WebId + ", ListId: " + ListId);
                    SPList SelectedList = GetList(WebId, ListId);

                    SPList currList = SPContext.Current.List;
                    SPFieldCollection selListfields = SelectedList.Fields;
                    SPFieldCollection currListFields = currList.Fields;

                    foreach (SPField field in currListFields)
                    {
                        if (Utilities.IsLookupType(field) && Utilities.IsDisplayField(field) && !((SPFieldLookup)field).IsDependentLookup)
                        {
                            foreach (SPField selfield in selListfields)
                            {
                                if (Utilities.IsLookupType(selfield) && Utilities.IsDisplayField(selfield) && !((SPFieldLookup)selfield).IsDependentLookup)
                                { 
                                    try
                                    {
                                        Guid sellistGuid = new Guid(((SPFieldLookup)selfield).LookupList);
                                        Guid currListGuid = new Guid(((SPFieldLookup)field).LookupList);

                                        if (sellistGuid.ToString().Equals(currListGuid.ToString(), StringComparison.InvariantCultureIgnoreCase) && (((SPFieldLookup)selfield).LookupField.Equals(((SPFieldLookup)field).LookupField, StringComparison.InvariantCultureIgnoreCase) || (TitleFieldNames.Contains(((SPFieldLookup)selfield).LookupField.ToLower()) && TitleFieldNames.Contains(((SPFieldLookup)field).LookupField.ToLower()))))
                                        {
                                            string val = field.Id.ToString() + ";#" + selfield.Title + ";#" + selfield.Id.ToString();
                                            columnList.Add(new ListItem(field.Title, val));

                                            break;
                                        }
                                    }
                                    catch { }
                                }
                            }
                        }
                    }

                    if (columnList.Count > 0)
                    {
                        columnList.Sort(delegate(ListItem item1, ListItem item2)
                        {
                            return item1.Text.CompareTo(item2.Text);
                        });

                    }

                }

                return columnList;
            }
        }

        
        #endregion

        private void PopulateAndSetValuesControls()
        {
            using (new EnterExitLogger("CCSCascadedLookupFieldEditor:PopulateAndSetValuesControls function"))
            {

                string contextWebId = "";

                if (_ccsCascadedField != null)
                {
                    contextWebId = string.IsNullOrEmpty(_ccsCascadedField.SourceWebID) ? SPContext.Current.Web.ID.ToString() : _ccsCascadedField.SourceWebID;
                }
                else
                {
                    contextWebId = SPContext.Current.Web.ID.ToString();
                }

                ddlWeb.Items.Clear();
                ddlWeb.Items.AddRange(GetWebCollection().ToArray());

                ListItem li = ddlWeb.Items.FindByValue(contextWebId);
                if (li != null)
                {
                    li.Selected = true;
                }

                PopulateList(contextWebId);
            }
        }

        private void PopulateList(string webId)
        {
            using (new EnterExitLogger("CCSCascadedLookupFieldEditor:PopulateList function"))
            {
                Utils.LogManager.write("Parameters WebId: " + webId);
                ddlList.Items.Clear();
                ddlList.Items.AddRange(GetListCollection(webId).ToArray());

                if (ddlList.Items.Count > 0)
                {
                    if (_ccsCascadedField != null)
                    {
                        string selListId = string.IsNullOrEmpty(_ccsCascadedField.LookupList) ? SPContext.Current.List.ID.ToString() : _ccsCascadedField.LookupList;
                        ListItem li = ddlList.Items.FindByValue(selListId);
                        if (li != null)
                        {
                            li.Selected = true;
                        }
                        else
                        {
                            ddlList.SelectedIndex = 0;
                        }
                    }
                    else
                    {
                        ddlList.SelectedIndex = 0;
                    }
                }

                if (ddlList.SelectedItem != null)
                {
                    PopulateColumns(webId, ddlList.SelectedItem.Value);
                }
                else
                {
                    PopulateColumns(webId, null);
                }
            }
        }

        private void PopulateColumns(string webId, string listId)
        {
            using (new EnterExitLogger("CCSCascadedLookupFieldEditor:PopulateColumns function"))
            {

                ddlColumn.Items.Clear();

                List<ListItem> ParentLinkColumnlist = null;

                if (listId != null)
                {
                    Utils.LogManager.write("Parameters WebId: " + webId + ", ListId: " + listId);
                    ddlColumn.Items.AddRange(GetColumnCollection(webId, listId).ToArray());

                    if (ddlColumn.Items.Count > 0)
                    {
                        if (_ccsCascadedField != null)
                        {
                            if (!string.IsNullOrEmpty(_ccsCascadedField.LookupFieldName))
                            {
                                ListItem li = ddlColumn.Items.FindByValue(_ccsCascadedField.LookupFieldName);
                                if (li != null) li.Selected = true;
                            }
                            else
                            {
                                ListItem selLi = ddlColumn.Items.FindByText("Title");
                                if (selLi != null) selLi.Selected = true;
                            }
                        }
                        else
                        {
                            ListItem li = ddlColumn.Items.FindByText("Title");
                            if (li != null) li.Selected = true;
                        }
                    }

                    ParentLinkColumnlist = GetParentLinkedColumnCollection(webId, listId);
                }

                if (ParentLinkColumnlist == null || ParentLinkColumnlist.Count == 0)
                {
                    cbxLinkParent.Enabled = false; ;
                    SetLinkedParentControl(false);
                }
                else
                {
                    cbxLinkParent.Enabled = true;
                    SetLinkedParentControl(true);
                    ddlParentColumn.Items.Clear();
                    ddlParentColumn.Items.AddRange(ParentLinkColumnlist.ToArray());

                    if (_ccsCascadedField != null && !string.IsNullOrEmpty(_ccsCascadedField.ParentLinkedColumnName))
                    {
                        ListItem li = ddlParentColumn.Items.FindByValue(_ccsCascadedField.ParentLinkedColumnName);
                        if (li != null) li.Selected = true;
                        else ddlParentColumn.SelectedIndex = 0;
                    }
                    else
                    {
                        ddlParentColumn.SelectedIndex = 0;
                    }

                    SetLinkColumnValue(ddlParentColumn.SelectedItem);
                }

                PopulateAdditionalFields(webId, listId);

                PopulateViewDropDown(ddlWeb.SelectedItem.Value, ddlList.SelectedItem.Value);
            }
        }

        private void PopulateAdditionalFields(string webId, string listId)
        {
            using (new EnterExitLogger("CCSCascadedLookupFieldEditor:PopulateAdditionalFields function"))
            {

                cblAdditionalFields.Items.Clear();
                cblAdditionalFilters.Items.Clear();
                if (listId != null)
                {
                    Utils.LogManager.write("Parameters WebId: " + webId + ", ListId: " + listId); 
                    SPList selList = GetList(webId, listId);
                    SPFieldCollection selListFields = selList.Fields;
                    foreach (SPField selField in selListFields)
                    {
                        if (Utilities.IsDisplayField(selField))
                        {
                            if (!Utilities.IsLookupType(selField))
                            {
                                if (Utilities.GeneralFields(selField))
                                {
                                    cblAdditionalFields.Items.Add(new ListItem(selField.Title, selField.InternalName));
                                }
                            }
                            else
                            {
                                cblAdditionalFilters.Items.Add(new ListItem(selField.Title, selField.InternalName));
                            }
                        }
                    }
                }

                if (cblAdditionalFilters.Items.Count > 0)
                {
                    lbAdditionalFilters.Visible = true;
                    //cblAdditionalFilters.Visible = true;
                    cblAdditionalFilters.Visible = false;
                }
                else
                {
                    lbAdditionalFilters.Visible = false;
                    cblAdditionalFilters.Visible = false;
                }

                cblAdditionalFields.Visible = true;
            }
        }

        private void PopulateViewDropDown(string webId, string listId)
        {
            using (new EnterExitLogger("CCSCascadedLookupFieldEditor:PopulateViewDropDown function"))
            {
                ddlView.Items.Clear();

                if (listId != null)
                {
                    Utils.LogManager.write("Parameters WebId: " + webId + ", ListId: " + listId); 
                    SPList list = GetList(webId, listId);

                    foreach (SPView view in list.Views)
                    {
                        if ((view.Hidden || view.PersonalView) || !view.Type.Equals("HTML"))
                        {
                            continue;
                        }

                        ListItem item = new ListItem(view.Title, view.ID.ToString());
                        string viewId = string.Empty;

                        if (_ccsCascadedField != null)
                        {
                            if (!string.IsNullOrEmpty(_ccsCascadedField.View))
                            {
                                viewId = _ccsCascadedField.View;
                            }
                        }

                        if (((view.ID.ToString().Equals(viewId, StringComparison.InvariantCultureIgnoreCase)) || ((view.ID.ToString() + "|" + view.Url).Equals(viewId, StringComparison.InvariantCultureIgnoreCase))))
                        {
                            item.Selected = true;
                            cbxSortByView.Enabled = true;
                        }

                        ddlView.Items.Add(item);
                    }
                }

                ddlView.Items.Insert(0, new ListItem("", ""));
                ddlView.Visible = true;
                lbView.Visible = true;
            }
        }

        private void SetLinkColumnValue(ListItem parentSelecteItem)
        {
            using (new EnterExitLogger("CCSCascadedLookupFieldEditor:SetLinkColumnValue function"))
            {
                Utils.LogManager.write("Parameters parentSelecteItem: " + parentSelecteItem); 
                string[] vals = parentSelecteItem.Value.Split(new string[] { ";#" }, StringSplitOptions.None);
                if (vals != null && vals.Length == 3)
                {
                    ListItem li = new ListItem(vals[1], vals[2]);
                    if (li != null)
                    {
                        li.Selected = true;

                        ddlLinkColumn.Items.Clear();
                        ddlLinkColumn.Items.Add(li);
                    }
                }
            }
        }

        private void SetLinkedParentControl(bool flag)
        {
            using (new EnterExitLogger("CCSCascadedLookupFieldEditor:SetLinkedParentControl function"))
            {
                Utils.LogManager.write("Parameters flag: " + flag.ToString());
                //cbxLinkParent.Enabled = flag;
                cbxLinkParent.Checked = flag;

                lblParentColumn.Visible = flag;
                ddlParentColumn.Visible = flag;
                lblLinkColumn.Visible = flag;
                ddlLinkColumn.Visible = flag;

                lbllistLinkColumn.Visible = flag;
                cbxShowallParentEmpty.Enabled = flag;
            }
        }

        private void SetAdditonalFields()
        {
            using (new EnterExitLogger("CCSCascadedLookupFieldEditor:SetAdditonalFields function"))
            {
                string additionalFieldsString = _ccsCascadedField.GetAdditionalFields();

                if (_ccsCascadedField != null && !string.IsNullOrEmpty(additionalFieldsString))
                {
                    if (string.IsNullOrEmpty(additionalFieldsString))
                    {
                        return;
                    }

                    string[] ids = additionalFieldsString.Split(new String[] { ";#" }, StringSplitOptions.RemoveEmptyEntries);
                    if (ids != null && ids.Length > 0)
                    {
                        foreach (string id in ids)
                        {
                            ListItem li = cblAdditionalFields.Items.FindByValue(id);

                            if (li != null) li.Selected = true;
                        }
                    }
                }
            }
        }

        private void SetAdditonalFilters()
        {
            using (new EnterExitLogger("CCSCascadedLookupFieldEditor:SetAdditonalFilters function"))
            {
                if (_ccsCascadedField != null && !string.IsNullOrEmpty(_ccsCascadedField.AdditionalFilters))
                {
                    string[] ids = _ccsCascadedField.AdditionalFilters.Split(new String[] { ";#" }, StringSplitOptions.RemoveEmptyEntries);
                    if (ids != null && ids.Length > 0)
                    {
                        foreach (string id in ids)
                        {
                            ListItem li = cblAdditionalFilters.Items.FindByValue(id);

                            if (li != null) li.Selected = true;
                        }
                    }
                }
            }
        }

        private void SetCheckboxcontrolsValue()
        {
            using (new EnterExitLogger("CCSCascadedLookupFieldEditor:SetCheckboxcontrolsValue function"))
            {
                if (_ccsCascadedField != null)
                {
                    cbxLinkParent.Checked = _ccsCascadedField.LinkToParent;
                    cbxLinkParent_CheckedChanged(cbxLinkParent, new EventArgs());

                    cbxShowallParentEmpty.Checked = _ccsCascadedField.ShowAllOnEmpty;

                    cbxAllowMultiple.Checked = _ccsCascadedField.AllowMultipleValues;
                    cbxAllowMultiple_CheckedChanged(cbxAllowMultiple, new EventArgs());

                    cbxAdvanceSettings.Checked = _ccsCascadedField.AdvancedSetting;
                    cbxAdvanceSettings_CheckedChanged(cbxAdvanceSettings, new EventArgs());

                    cbxSortByView.Checked = _ccsCascadedField.SortByView;

                    cbxAllowNewValues.Checked = _ccsCascadedField.AllowNewEntry;
                    cbxAllowNewValues_CheckedChanged(cbxAllowNewValues, new EventArgs());

                    cbxUseNewForm.Checked = _ccsCascadedField.UseNewForm;

                    SetAdditonalFields();
                    SetAdditonalFilters();
                }
            }
        }

        private string GetAdditonalFields()
        {
            using (new EnterExitLogger("CCSCascadedLookupFieldEditor:GetAdditonalFields function"))
            {
                string val = "";
                foreach (ListItem li in cblAdditionalFields.Items)
                {
                    if (li.Selected)
                    {
                        if (string.IsNullOrEmpty(val))
                        {
                            val = li.Text + ";#" + li.Value;
                        }
                        else
                        {
                            val += ";#" + li.Text + ";#" + li.Value;
                        }
                    }
                }

                Utils.LogManager.write("AdditonalFields: " + val);
                return val;
            }
        }

        private string GetAdditonalFilters()
        {
            using (new EnterExitLogger("CCSCascadedLookupFieldEditor:GetAdditonalFilters function"))
            {
                string val = "";
                foreach (ListItem li in cblAdditionalFilters.Items)
                {
                    if (li.Selected)
                    {
                        if (string.IsNullOrEmpty(val ))
                        {
                            val = li.Value;
                        }
                        else
                        {
                            val += ";#" + li.Value;
                        }
                    }
                }
                Utils.LogManager.write("AdditonalFilters: " + val);
                return val;
            }
        }

        private void AddingNewFieldControlVisibility()
        {
            using (new EnterExitLogger("CCSCascadedLookupFieldEditor:AddingNewFieldControlVisibility function"))
            {

                string referrer = this.Request.Url.AbsoluteUri;

                if (!string.IsNullOrEmpty(referrer))
                {
                    if (referrer.IndexOf("_layouts/fldNew.aspx") > -1
                      || referrer.IndexOf("_layouts/FldNewEx.aspx") > -1) // adding new field
                    {
                        ddlWeb.Visible = true;
                        ddlList.Visible = true;

                        //populate values

                        cblAdditionalFilters.Items.Clear();


                        foreach (SPField selField in SPContext.Current.List.Fields)
                        {
                            if (Utilities.IsDisplayField(selField))
                            {
                                if (selField.Type == SPFieldType.Lookup || selField.TypeAsString.Equals("Lookup", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    if (!((SPFieldLookup)selField).IsDependentLookup)
                                    {
                                        ddlConvertFromLookup.Items.Add(new ListItem(selField.Title, selField.InternalName));
                                        ddlConvertFromLookup.SelectedIndex = 0;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        ddlWeb.Enabled = false;
                        ddlList.Enabled = false;
                    }
                }
            }
        }

        void SetRelationShipControlsValue()
        {
            using (new EnterExitLogger("CCSCascadedLookupFieldEditor:SetRelationShipControlsValue function"))
            {
                if (_ccsCascadedField != null)
                {
                    if (!_ccsCascadedField.AllowMultipleValues)
                    {
                        cbxRelationshipBehavior.Enabled = true;
                        switch (_ccsCascadedField.RelationshipDeleteBehavior)
                        {
                            case SPRelationshipDeleteBehavior.None:
                                rbRestrictDelete.Enabled = false;
                                rbCascadeDelete.Enabled = false;

                                rbRestrictDelete.Checked = false;
                                rbCascadeDelete.Checked = false;
                                break;
                            case SPRelationshipDeleteBehavior.Restrict:
                                rbRestrictDelete.Enabled = true;
                                rbCascadeDelete.Enabled = true;

                                rbRestrictDelete.Checked = true;
                                rbCascadeDelete.Checked = false;
                                break;
                            case SPRelationshipDeleteBehavior.Cascade:
                                rbRestrictDelete.Enabled = true;
                                rbCascadeDelete.Enabled = true;

                                rbRestrictDelete.Checked = false;
                                rbCascadeDelete.Checked = true;
                                break;

                        }
                    }
                    else
                    {
                        cbxRelationshipBehavior.Enabled = false;
                        rbRestrictDelete.Enabled = false;
                        rbCascadeDelete.Enabled = false;
                    }
                }
            }
        }

        void ConvertToCCSCascadedLookupField(SPFieldLookup field)
        {
            using (new EnterExitLogger("CCSCascadedLookupFieldEditor:ConvertToCCSCascadedLookupField function"))
            {
                XmlDocument doc = new XmlDocument();
                Utils.LogManager.write("Before SPFieldLookup xmlScema: " + field.SchemaXml);
                
                doc.LoadXml(field.SchemaXml);

                //Creating Attributes

                CreateAttribute(doc, "Type", "CCSCascadedLookup");

                CreateAttribute(doc, "WebId", field.LookupWebId.ToString());

                CreateAttribute(doc, "SourceWebID", field.LookupWebId.ToString());

                CreateAttribute(doc, "LookupFieldListName", field.LookupList);

                CreateAttribute(doc, "LookupFieldName", field.LookupField);

                CreateAttribute(doc, "ParentLinkedColumnName", "");

                CreateAttribute(doc, "AllowMultipleValues", "");

                CreateAttribute(doc, "AdvancedSetting", "True");

                CreateAttribute(doc, "View", "");

                CreateAttribute(doc, "LinkToParent", "False");

                CreateAttribute(doc, "ShowAllOnEmpty", "False");

                CreateAttribute(doc, "AllowNewEntry", "False");

                CreateAttribute(doc, "UseNewForm", "False");

                CreateAttribute(doc, "SortByView", "False");

                CreateAttribute(doc, "AllowAutocomplete", "False");

                CreateAttribute(doc, "AdditionalFields", "");

                CreateAttribute(doc, "AdditionalFilters", "");


                doc.DocumentElement.InnerXml = "<Customization><ArrayOfProperty>" +
                           "<Property><Name>SourceWebID</Name></Property>" +
                           "<Property><Name>LookupFieldListName</Name></Property>" +
                           "<Property><Name>LookupFieldName</Name></Property>" +
                           "<Property><Name>ParentLinkedColumnName</Name></Property>" +
                           "<Property><Name>AllowMultipleValues</Name></Property>" +
                           "<Property><Name>AdvancedSetting</Name></Property>" +
                           "<Property><Name>View</Name></Property>" +
                           "<Property><Name>LinkToParent</Name></Property>" +
                           "<Property><Name>ShowAllOnEmpty</Name></Property>" +
                           "<Property><Name>AllowNewEntry</Name></Property>" +
                           "<Property><Name>UseNewForm</Name></Property>" +
                           "<Property><Name>AdditionalFields</Name></Property>" +
                           "<Property><Name>SortByView</Name></Property>" +
                           "<Property><Name>AllowAutocomplete</Name></Property>" +
                           "<Property><Name>AdditionalFilters</Name></Property>" +
                           "</ArrayOfProperty></Customization>";


                field.SchemaXml = doc.OuterXml;

                Utils.LogManager.write("After SPFieldLookup xmlScema: " + field.SchemaXml);

                field.Update();
            }
        }

        void ConvertToSPFieldLookup(CCSCascadedLookupField field)
        {
            using (new EnterExitLogger("CCSCascadedLookupFieldEditor:ConvertToSPFieldLookup function"))
            {
                XmlDocument doc = new XmlDocument();
                Utils.LogManager.write("Before CCSCascadedLookupField xmlScema: " + field.SchemaXml);
                
                doc.LoadXml(field.SchemaXml);

                //Creating Attributes

                CreateAttribute(doc, "Type", "Lookup");

                DeleteAttribute(doc, "WebId");

                DeleteAttribute(doc, "SourceWebID");

                DeleteAttribute(doc, "LookupFieldListName");

                DeleteAttribute(doc, "LookupFieldName");

                DeleteAttribute(doc, "ParentLinkedColumnName");

                DeleteAttribute(doc, "AllowMultipleValues");

                DeleteAttribute(doc, "AdvancedSetting");

                DeleteAttribute(doc, "View");

                DeleteAttribute(doc, "LinkToParent");

                DeleteAttribute(doc, "ShowAllOnEmpty");

                DeleteAttribute(doc, "AllowNewEntry");

                DeleteAttribute(doc, "UseNewForm");

                DeleteAttribute(doc, "SortByView");

                DeleteAttribute(doc, "AllowAutocomplete");

                DeleteAttribute(doc, "AdditionalFields");

                DeleteAttribute(doc, "AdditionalFilters");


                doc.DocumentElement.InnerXml = "";


                field.SchemaXml = doc.OuterXml;
                Utils.LogManager.write("After CCSCascadedLookupField xmlScema: " + field.SchemaXml);
                
                field.Update();
            }
        }

        private void CreateAttribute(XmlDocument doc, string name, string value)
        {
            XmlAttribute attribute = doc.DocumentElement.Attributes[name];
            if (attribute == null)
            {
                attribute = doc.CreateAttribute(name);
                doc.DocumentElement.Attributes.Append(attribute);
            }
            doc.DocumentElement.Attributes[name].Value = value;
        }

        private void DeleteAttribute(XmlDocument doc, string name)
        {
            XmlAttribute attribute = doc.DocumentElement.Attributes[name];
            if (attribute != null)
            {
                doc.DocumentElement.Attributes.Remove(attribute);
            }
        }

        protected void btnConvertFromLookup_Click(object sender, EventArgs e)
        {
            if (ddlConvertFromLookup.SelectedItem != null)
            {
                SPFieldLookup field = SPContext.Current.List.Fields.GetFieldByInternalName(ddlConvertFromLookup.SelectedItem.Value) as SPFieldLookup;
                if (field != null)
                {
                    ConvertToCCSCascadedLookupField(field);
                    Page.Response.Redirect(GetFieldEditUrl(field));
                }
            }
        }


        protected void btnConvertToLookup_Click(object sender, EventArgs e)
        {
            ConvertToSPFieldLookup(_ccsCascadedField);

            Page.Response.Redirect(GetFieldEditUrl(_ccsCascadedField));
        }

        void ShowErrorMessage(string errorMessage)
        {
            using (new EnterExitLogger("CCSCascadedLookupFieldEditor:ShowErrorMessage function"))
            {
                if (ErrorText != null)
                {
                    Utils.LogManager.write("ErrorMessage : " + errorMessage);
                    ErrorText.Text = "<font color=\"red\"> Error: " + errorMessage + "</font>";
                    ErrorText.Visible = true;
                }
            }
        }

        string GetFieldEditUrl(SPField field)
        {
            return System.IO.Path.Combine(field.ParentList.ParentWeb.Url, string.Format("_layouts/FldEditEx.aspx?List={0}&Field={1}", field.ParentList.ID.ToString(), field.InternalName));
        }
    }
}
