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
            PopulateList(ddlWeb.SelectedItem.Value);
        }

        protected void ddlList_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateColumns(ddlWeb.SelectedItem.Value, ddlList.SelectedItem.Value);
        }

        protected void cbxLinkParent_CheckedChanged(object sender, EventArgs e)
        {
            SetLinkedParentControl(cbxLinkParent.Checked);
        }

        protected void ddlParentColumn_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetLinkColumnValue(ddlParentColumn.SelectedItem);
        }

        protected void cbxAllowMultiple_CheckedChanged(object sender, EventArgs e)
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

        protected void cbxAdvanceSettings_CheckedChanged(object sender, EventArgs e)
        {
            AdditionSettingPane.Visible = cbxAdvanceSettings.Checked;
        }

        protected void ddlView_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbxSortByView.Enabled = (ddlView.SelectedIndex > 0);
        }

        protected void cbxAllowNewValues_CheckedChanged(object sender, EventArgs e)
        {
            cbxUseNewForm.Enabled = cbxAllowNewValues.Checked;
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
            _ccsCascadedField = field as CCSCascadedLookupField;
            if (!IsPostBack)
            {
                PopulateAndSetValuesControls();

                SetCheckboxcontrolsValue();

                AddingNewFieldControlVisibility();

                SetRelationShipControlsValue();

                if (field != null) ;
            }
        }

        public void OnSaveChange(SPField field, bool isNewField)
        {
            CCSCascadedLookupField ccscascadeField = field as CCSCascadedLookupField;
            if (ccscascadeField != null)
            {
                using (SPWeb selWeb = SPControl.GetContextSite(this.Context).OpenWeb(new Guid(ddlWeb.SelectedItem.Value)))
                {
                    ccscascadeField.LookupWebId = selWeb.ID;
                }

                ccscascadeField.LookupList = (ddlList.SelectedItem != null ? ddlList.SelectedItem.Value : "");
                ccscascadeField.LookupField = (ddlColumn.SelectedItem != null ? ddlColumn.SelectedItem.Value : "");
                
                ccscascadeField.SourceWebID = ddlWeb.SelectedItem.Value;
                ccscascadeField.LookupFieldListName = (ddlList.SelectedItem != null ? ddlList.SelectedItem.Value : "");
                ccscascadeField.LookupFieldName = (ddlColumn.SelectedItem != null ? ddlColumn.SelectedItem.Value : "");
                ccscascadeField.ParentLinkedColumnName = (ddlParentColumn.SelectedItem != null ? ddlParentColumn.SelectedItem.Value : "");

                ccscascadeField.AllowMultipleValues = cbxAllowMultiple.Checked;
                ccscascadeField.AdvancedSetting = cbxAdvanceSettings.Checked;
                ccscascadeField.View = (ddlView.SelectedItem != null ? ddlView.SelectedItem.Value : "");
                ccscascadeField.LinkToParent = cbxLinkParent.Checked;
                ccscascadeField.ShowAllOnEmpty = cbxShowallParentEmpty.Checked;
                ccscascadeField.AllowNewEntry = cbxAllowNewValues.Checked;
                ccscascadeField.UseNewForm = cbxUseNewForm.Checked;

                ccscascadeField.SortByView= cbxSortByView.Checked;
                ccscascadeField.AllowAutocomplete = false;

                if (cbxRelationshipBehavior.Enabled && cbxRelationshipBehavior.Checked)
                {
                    if(rbRestrictDelete.Enabled && rbRestrictDelete.Checked)
                        ccscascadeField.RelationshipDeleteBehavior = SPRelationshipDeleteBehavior.Restrict;
                    else if (rbCascadeDelete.Enabled && rbCascadeDelete.Checked)
                        ccscascadeField.RelationshipDeleteBehavior = SPRelationshipDeleteBehavior.Cascade;
                    else
                        ccscascadeField.RelationshipDeleteBehavior = SPRelationshipDeleteBehavior.None;
                }
                else
                {
                    ccscascadeField.RelationshipDeleteBehavior = SPRelationshipDeleteBehavior.None;
                }

                ccscascadeField.AdditionalFilters = GetAdditonalFilters();

                if (isNewField)
                {
                    ccscascadeField.AdditionalFields = GetAdditonalFields();
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
                                    ccscascadeField.ParentList.Fields.GetField(ccscascadeField.Title + " : " + li.Text).Delete();
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Get Control Values

        private SPList GetList(string webId, string listId)
        {
            SPList list = null;
            SPSite currSite = SPControl.GetContextSite(this.Context);
            using (SPWeb selWeb = currSite.OpenWeb(new Guid(webId)))
            {
                list = selWeb.Lists[new Guid(listId)];
            }

            return list;
        }
        
        private List<ListItem> GetWebCollection()
        {
            List<ListItem> webList = new List<ListItem>();

            SPSite currentSite = SPControl.GetContextSite(this.Context);

            SPWebCollection webCollection = currentSite.AllWebs;

            foreach (SPWeb web in webCollection)
            {
                if (web.DoesUserHavePermissions(SPBasePermissions.ViewPages | SPBasePermissions.OpenItems | SPBasePermissions.ViewListItems))
                {
                    webList.Add(new ListItem(web.Title, web.ID.ToString()));
                }
            }

            if (webList.Count > 0)
            {
                webList.Sort(delegate(ListItem item1, ListItem item2)
                {
                    return item1.Text.CompareTo(item2.Text);
                });
            }

            return webList;
        }

        private List<ListItem> GetListCollection(string WebId)
        {
            List<ListItem> spListList = new List<ListItem>();

            if (!string.IsNullOrEmpty(WebId))
            {
                SPSite currentSite = SPControl.GetContextSite(this.Context);
                SPListCollection spListColl = null;
                using (SPWeb CurrentWeb = currentSite.OpenWeb(new Guid(WebId)))
                {
                    spListColl = CurrentWeb.Lists;
                }

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

        private List<ListItem> GetColumnCollection(string WebId, string ListId)
        {
            List<ListItem> columnList = new List<ListItem>();
            if (!string.IsNullOrEmpty(WebId))
            {
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

        private List<ListItem> GetParentLinkedColumnCollection(string WebId, string ListId)
        {
            List<ListItem> columnList = new List<ListItem>();
            if (!string.IsNullOrEmpty(WebId))
            {
                SPList SelectedList = GetList(WebId, ListId);

                SPList currList = SPContext.Current.List;
                SPFieldCollection selListfields = SelectedList.Fields;
                SPFieldCollection currListFields = currList.Fields;

                foreach (SPField field in currListFields)
                {
                    if (Utilities.IsLookupType(field) && Utilities.IsDisplayField(field))
                    {
                        foreach (SPField selfield in selListfields)
                        {
                            if (Utilities.IsLookupType(selfield) && Utilities.IsDisplayField(selfield))
                            {
                                try
                                {
                                    if (((SPFieldLookup)selfield).LookupList.ToString() == ((SPFieldLookup)field).LookupList.ToString() && ((SPFieldLookup)selfield).LookupField.ToString() == ((SPFieldLookup)field).LookupField.ToString())
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

        
        #endregion

        private void PopulateAndSetValuesControls()
        {
            string contextWebId = "";

            if (_ccsCascadedField != null)
            {
                contextWebId = string.IsNullOrEmpty(_ccsCascadedField.SourceWebID) ? SPControl.GetContextWeb(this.Context).ID.ToString() : _ccsCascadedField.SourceWebID;
            }
            else
            {
                contextWebId = SPControl.GetContextWeb(this.Context).ID.ToString();
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

        private void PopulateList(string webId)
        {
            ddlList.Items.Clear();
            ddlList.Items.AddRange(GetListCollection(webId).ToArray());

            if (ddlList.Items.Count > 0)
            {
                if (_ccsCascadedField != null)
                {
                    string selListId = string.IsNullOrEmpty(_ccsCascadedField.LookupFieldListName) ? SPControl.GetContextWeb(this.Context).ID.ToString() : _ccsCascadedField.LookupFieldListName;
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

        private void PopulateColumns(string webId, string listId)
        {
            ddlColumn.Items.Clear();

            List<ListItem> ParentLinkColumnlist = null;

            if (listId != null)
            {
                ddlColumn.Items.AddRange(GetColumnCollection(webId, listId).ToArray());

                if (ddlColumn.Items.Count > 0)
                {
                    if (_ccsCascadedField != null)
                    {
                        if (!string.IsNullOrEmpty(_ccsCascadedField.LookupFieldName))
                        {
                            ListItem li = ddlColumn.Items.FindByValue(_ccsCascadedField.LookupFieldName);
                            if(li != null) li.Selected = true;
                        }
                        else
                        {
                            ListItem selLi =  ddlColumn.Items.FindByText("Title");
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

                if ( _ccsCascadedField != null && !string.IsNullOrEmpty(_ccsCascadedField.ParentLinkedColumnName))
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

        private void PopulateAdditionalFields(string webId, string listId)
        {
            cblAdditionalFields.Items.Clear();
            cblAdditionalFilters.Items.Clear();
            if (listId != null)
            {
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

        private void PopulateViewDropDown(string webId, string listId)
        {
            ddlView.Items.Clear();

            if (listId != null)
            {
                SPList list = GetList(webId, listId);

                foreach (SPView view in list.Views)
                {
                    if ((view.Hidden || view.PersonalView) || !view.Type.Equals("HTML"))
                    {
                        continue;
                    }

                    ListItem item = new ListItem(view.Title, view.ID.ToString() + "|" + view.Url);
                    string viewId = string.Empty;

                    if (_ccsCascadedField != null)
                    {
                        if (!string.IsNullOrEmpty(_ccsCascadedField.View))
                        {
                            viewId = _ccsCascadedField.View;
                        }
                    }

                    if (((view.ID.ToString() == viewId) || (view.ID.ToString() + "|" + view.Url == viewId)))
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

        private void SetLinkColumnValue(ListItem parentSelecteItem)
        {
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

        private void SetLinkedParentControl(bool flag)
        {
            //cbxLinkParent.Enabled = flag;
            cbxLinkParent.Checked = flag;

            lblParentColumn.Visible = flag;
            ddlParentColumn.Visible = flag;
            lblLinkColumn.Visible = flag;
            ddlLinkColumn.Visible = flag;

            lbllistLinkColumn.Visible = flag;
            cbxShowallParentEmpty.Enabled = flag;
        }

        private void SetAdditonalFields()
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
                        
                        if(li != null)  li.Selected = true;
                    }
                }
            }
        }

        private void SetAdditonalFilters()
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

        private void SetCheckboxcontrolsValue()
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

        private string GetAdditonalFields()
        {
            string val = "";
            foreach (ListItem li in cblAdditionalFields.Items)
            {
                if (li.Selected)
                {
                    if (val == "")
                    {
                        val = li.Text + ";#" + li.Value;
                    }
                    else
                    {
                        val += ";#" + li.Text + ";#" + li.Value;
                    }
                }
            }

            return val;
        }

        private string GetAdditonalFilters()
        {
            string val = "";
            foreach (ListItem li in cblAdditionalFilters.Items)
            {
                if (li.Selected)
                {
                    if (val == "")
                    {
                        val = li.Value;
                    }
                    else
                    {
                        val += ";#" + li.Value;
                    }
                }
            }

            return val;
        }

        private void AddingNewFieldControlVisibility()
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
                            if (selField.Type == SPFieldType.Lookup || selField.TypeAsString == "Lookup")
                            {
                                if (!((SPFieldLookup)selField).IsDependentLookup)
                                {
                                    ddlConvertFromLookup.Items.Add(new ListItem(selField.Title, selField.InternalName));
                                    ddlConvertFromLookup.SelectedIndex = 0;
                                }
                            }
                        }
                    }

                    pnlConvertFromLookup.Visible = true;
                    pnlConvertToLookup.Visible = false;
                }
                else
                {
                    ddlWeb.Enabled = false;
                    ddlList.Enabled = false;

                    pnlConvertFromLookup.Visible = false; 
                    pnlConvertToLookup.Visible = true;
                }
            }
        }

        void SetRelationShipControlsValue()
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

        void ConvertToCCSCascadedLookupField(SPFieldLookup field)
        {
            XmlDocument doc = new XmlDocument();
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

            field.Update();
        }

        void ConvertToSPFieldLookup(CCSCascadedLookupField field)
        {
            XmlDocument doc = new XmlDocument();
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

            field.Update();
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
                    Page.Response.Redirect(this.Page.Request.Url.ToString());
                }
            }
        }


        protected void btnConvertToLookup_Click(object sender, EventArgs e)
        {
            ConvertToSPFieldLookup(_ccsCascadedField);
            Page.Response.Redirect(this.Page.Request.Url.ToString());
        }
        
    }
}
