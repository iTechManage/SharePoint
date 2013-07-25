using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint;



namespace CrowCanyon.CascadedLookup
{
    class CCSCascadedLookupControl : BaseFieldControl
    {
        #region Define Controls

        Panel SingleValuePanel = null;
        Panel MultipleValuePanel = null;
        Panel NewEntryPanel = null;

        DropDownList ddlCCSCascadeFieldControl = null;
        ListBox lbLeftBox = null;
        ListBox lbRightBox = null;
        Button btnAdd = null;
        Button btnRemove = null;

        LinkButton lnkNewEntry = null;
        TextBox txtNewEntry = null;
        LinkButton lnkAdd = null;
        LinkButton lnkCancel = null;

        HtmlInputHidden hiddenFieldType = null;
        HtmlInputHidden hParentValue = null;

        #endregion

        #region Override Properties and Methods

        protected override string DefaultTemplateName { get { return "CCSCascadedLookupControl"; } }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (this.ControlMode == SPControlMode.Display || this.ControlMode == SPControlMode.Invalid)
            {
                return;
            }


            CCSCascadedLookupField field = base.Field as CCSCascadedLookupField;

            AddeventOnParentControls();

            if (!Page.ClientScript.IsStartupScriptRegistered(this.Field.Id.ToString("n")))
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append(@"<script language='javascript'>");
                sb.Append(@"function callbackMethod" + this.Field.Id.ToString("n") + " (dialogResult, returnValue)");
                sb.Append(@"{");
                sb.Append(@"if(dialogResult == 1)");
                sb.Append(@"{");
                sb.Append(@" __doPostBack('" + this.Field.Id.ToString("n") + "', '')");
                sb.Append(@"}");
                sb.Append(@"}");
                sb.Append(@"</script>");

                Page.ClientScript.RegisterStartupScript(new object().GetType(), this.Field.Id.ToString("n"), sb.ToString());
            }

            if (!Page.IsPostBack)
            {
                PopulatingValue();

                #region Set Field Value to Control Value

                if (ItemFieldValue != null)
                {
                    if (field != null)
                    {
                        if (field.AllowMultipleValues)
                        {
                            SPFieldLookupValueCollection vals = ItemFieldValue as SPFieldLookupValueCollection;
                            if (vals != null)
                            {
                                foreach (SPFieldLookupValue val in vals)
                                {
                                    ListItem li = lbLeftBox.Items.FindByValue(val.LookupId.ToString());
                                    lbLeftBox.Items.Remove(li);
                                    lbRightBox.Items.Add(li);
                                }
                            }
                        }
                        else
                        {
                            SPFieldLookupValue singleValue = ItemFieldValue as SPFieldLookupValue;
                            if (singleValue != null && ddlCCSCascadeFieldControl.Items != null && ddlCCSCascadeFieldControl.Items.Count > 0)
                            {
                                for (int i = 0; i < ddlCCSCascadeFieldControl.Items.Count; i++)
                                {
                                    if (ddlCCSCascadeFieldControl.Items[i].Value == singleValue.LookupId.ToString())
                                    {
                                        ddlCCSCascadeFieldControl.SelectedIndex = i;
                                    }
                                }
                            }
                        }
                    }
                }

                object ParentValue = GetParentFieldValue(field);
                SPFieldLookupValue pValue = ParentValue as SPFieldLookupValue;
                if (pValue != null)
                {
                    hiddenFieldType.Value = pValue.LookupValue;
                    hParentValue.Value = pValue.LookupId.ToString();
                }

                #endregion
            }

            if (Page.Request["__EVENTTARGET"] != null)
            {
                string FieldId = field.Id.ToString("n");
                string ParentColumnId = field.GetParentColumnId();

                if (Page.Request.Params.Get("__EVENTTARGET") == FieldId || Page.Request.Params.Get("__EVENTTARGET") == "ParentField" + ParentColumnId)
                {

                    PopulatingValue();
                    
                    // Update nested Controls
                    UpdateChildLinkedControl(this.Field as CCSCascadedLookupField);
                }
                else if(Page.Request.Params.Get("__EVENTTARGET") == "ParentFieldAuto" + ParentColumnId)
                {
                    hParentValue.Value = Page.Request.Params.Get("__EVENTARGUMENT");

                    PopulatingValue(Page.Request.Params.Get("__EVENTARGUMENT"));
                    
                    // Update nested Controls
                    UpdateChildLinkedControl(this.Field as CCSCascadedLookupField);
                }
                else if (Page.Request.Params.Get("__EVENTTARGET") == ("AddItem" + FieldId))
                {
                    btnAdd_Click(btnAdd, new EventArgs());
                }
                else if (Page.Request.Params.Get("__EVENTTARGET") == ("RemoveItem" + FieldId))
                {
                    btnRemove_Click(btnRemove, new EventArgs());
                }
            }
        }

        protected override void CreateChildControls()
        {
            CCSCascadedLookupField field = base.Field as CCSCascadedLookupField;
            if (field != null && this.ControlMode != SPControlMode.Display)
            {
                base.CreateChildControls();
                if (!this.ChildControlsCreated)
                {
                    ShowControls(field);
                }
            }

            
        }

        public override object Value
        {
            get
            {
                EnsureChildControls();
                CCSCascadedLookupField field = base.Field as CCSCascadedLookupField;
                if (field.AllowMultipleValues)
                {
                    SPFieldLookupValueCollection _vals = null;
                    if (lbRightBox.Items.Count > 0)
                    {
                        _vals = new SPFieldLookupValueCollection();

                        foreach (ListItem item in lbRightBox.Items)
                        {
                            _vals.Add(new SPFieldLookupValue(int.Parse(item.Value), item.Text));
                        }
                    }

                    SetParentValueIfAutoComplete(hParentValue.Value);
                    return _vals;
                }
                else
                {
                    SPFieldLookupValue val = null;
                    if (ddlCCSCascadeFieldControl.SelectedItem != null && ddlCCSCascadeFieldControl.SelectedItem.Value != "0")
                    {
                        val = new SPFieldLookupValue(int.Parse(ddlCCSCascadeFieldControl.SelectedItem.Value), ddlCCSCascadeFieldControl.SelectedItem.Text);
                    }

                    SetParentValueIfAutoComplete(hParentValue.Value);
                    return val;
                }
            }
            set
            {
                EnsureChildControls();
                CCSCascadedLookupField field = base.Field as CCSCascadedLookupField;
                if (field.AllowMultipleValues)
                {
                    base.Value = value as SPFieldLookupValueCollection;
                }
                else
                {
                    SPFieldLookupValue val = value as SPFieldLookupValue;
                    base.Value = val;
                }
            }
        }

        #endregion

        #region Filling Data in Control

        void PopulatingValue()
        {
            CCSCascadedLookupField field = base.Field as CCSCascadedLookupField;

            object parentValue = GetParentFieldValue(field);

            PopulatingValue(parentValue);
        }

        void PopulatingValue(object parentValue)
        {
            CCSCascadedLookupField field = base.Field as CCSCascadedLookupField;

            if (field != null)
            {
                List<ListItem> poplateItemsList = new List<ListItem>();

                if (field.LinkToParent)
                {
                    if (parentValue != null)
                    {
                        SPFieldLookupValueCollection vals = parentValue as SPFieldLookupValueCollection;
                        if (vals != null)
                        {
                            foreach (SPFieldLookupValue val in vals)
                            {
                                Utilities.FetchMatchedValuesFromList(field, val.LookupId.ToString(), ref poplateItemsList);
                            }
                        }
                        else
                        {
                            SPFieldLookupValue singleValue = parentValue as SPFieldLookupValue;
                            if (singleValue != null)
                            {
                                Utilities.FetchMatchedValuesFromList(field, singleValue.LookupId.ToString(), ref poplateItemsList);
                            }
                            else
                            {

                                string stringValue = parentValue as string;
                                if (stringValue != null)
                                {
                                    Utilities.FetchMatchedValuesFromList(field, stringValue, ref poplateItemsList);
                                }
                                else
                                {
                                    int intValue = (int)parentValue;
                                    if (intValue >= 0)
                                    {
                                        Utilities.FetchMatchedValuesFromList(field, intValue.ToString(), ref poplateItemsList);
                                    }
                                }
                            }
                        }
                    }


                    if (parentValue == null && field.ShowAllOnEmpty)
                    {
                        Utilities.FetchAllValuesFromList(field, ref poplateItemsList);
                    }
                }
                else
                {
                    Utilities.FetchAllValuesFromList(field, ref poplateItemsList);
                }

                if (field.AllowMultipleValues)
                {
                    if (poplateItemsList != null && poplateItemsList.Count > 0)
                    {
                        if (lbRightBox.Items != null && lbRightBox.Items.Count > 0)
                        {
                            List<string> vals = new List<string>();
                            for (int i = lbRightBox.Items.Count - 1; i >= 0; i--)
                            {
                                if (!CheckListItemExistandRemove(lbRightBox.Items[i], ref poplateItemsList))
                                {
                                    lbRightBox.Items.RemoveAt(i);
                                }
                            }
                        }

                        for (int i = lbLeftBox.Items.Count - 1; i >= 0; i--)
                        {
                            if (!CheckListItemExistandRemove(lbLeftBox.Items[i], ref poplateItemsList))
                            {
                                lbLeftBox.Items.RemoveAt(i);
                            }
                        }

                        lbLeftBox.Items.AddRange(poplateItemsList.ToArray());
                    }
                    else
                    {
                        lbLeftBox.Items.Clear();
                        lbRightBox.Items.Clear();
                    }
                }
                else
                {
                    string selVal = ddlCCSCascadeFieldControl.SelectedValue;//cblAdditionalFields.Items.FindByValue(id);

                    ddlCCSCascadeFieldControl.Items.Clear();
                    if (this.ControlMode == SPControlMode.New || !Field.Required)
                    {
                        ddlCCSCascadeFieldControl.Items.Insert(0, new ListItem("(None)", "0"));
                    }

                    if (poplateItemsList != null && poplateItemsList.Count > 0)
                    {
                        ddlCCSCascadeFieldControl.Items.AddRange(poplateItemsList.ToArray());
                    }

                    if (ddlCCSCascadeFieldControl.Items.Count > 0)
                    {
                        ddlCCSCascadeFieldControl.SelectedIndex = 0;

                        for (int i = 0; i < ddlCCSCascadeFieldControl.Items.Count; i++)
                        {
                            if (ddlCCSCascadeFieldControl.Items[i].Value == selVal)
                            {
                                ddlCCSCascadeFieldControl.SelectedIndex = i;
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Create Control

        private void ShowControls(CCSCascadedLookupField field)
        {
            SingleValuePanel = (Panel)TemplateContainer.FindControl("SingleValuePanel");
            MultipleValuePanel = (Panel)TemplateContainer.FindControl("MultipleValuePanel");
            NewEntryPanel = (Panel)TemplateContainer.FindControl("NewEntryPanel");

            ddlCCSCascadeFieldControl = (DropDownList)TemplateContainer.FindControl("ddlCCSCascadeFieldControl");
            lbLeftBox = (ListBox)TemplateContainer.FindControl("lbLeftBox");
            lbRightBox = (ListBox)TemplateContainer.FindControl("lbRightBox");
            btnAdd = (Button)TemplateContainer.FindControl("btnAdd");
            btnRemove = (Button)TemplateContainer.FindControl("btnRemove");

            lnkNewEntry = (LinkButton)TemplateContainer.FindControl("lnkNewEntry");
            txtNewEntry = (TextBox)TemplateContainer.FindControl("txtNewEntry");
            lnkAdd = (LinkButton)TemplateContainer.FindControl("lnkAdd");
            lnkCancel = (LinkButton)TemplateContainer.FindControl("lnkCancel");

            hiddenFieldType = (HtmlInputHidden)TemplateContainer.FindControl("HiddenFieldType");
            hParentValue = (HtmlInputHidden)TemplateContainer.FindControl("hParentValue");

            if (!Page.IsPostBack)
            {
                //PopulatingValue();
            }



            if (field.AllowMultipleValues)
            {
                SingleValuePanel.Visible = false;
                MultipleValuePanel.Visible = true;

                //Add event
                lbLeftBox.Attributes.Add("ondblclick", "__doPostBack('AddItem" + this.Field.Id.ToString("n") + "','')");
                lbRightBox.Attributes.Add("ondblclick", "__doPostBack('RemoveItem" + this.Field.Id.ToString("n") + "','')");

                btnAdd.Click += new EventHandler(btnAdd_Click);
                btnRemove.Click += new EventHandler(btnRemove_Click);
            }
            else
            {
                SingleValuePanel.Visible = true;
                MultipleValuePanel.Visible = false;

                //Add event
                ddlCCSCascadeFieldControl.SelectedIndexChanged += new EventHandler(ddlCCSCascadeFieldControl_SelectedIndexChanged);
                ddlCCSCascadeFieldControl.AutoPostBack = true;
            }

            if (field.AllowNewEntry)
            {
                NewEntryPanel.Visible = true;

                lnkNewEntry.Visible = true;
                txtNewEntry.Visible = false;
                lnkAdd.Visible = false;
                lnkCancel.Visible = false;

                if (field.UseNewForm)
                {
                    using (SPWeb spWeb = SPContext.Current.Site.OpenWeb(field.LookupWebId))
                    {
                        string weburl = spWeb.Url;
                        SPList sourceList = spWeb.Lists[new Guid(field.LookupFieldListName)];
                        SPForm form = sourceList.Forms[PAGETYPE.PAGE_NEWFORM];
                        string url = form.Url;
                        url = weburl + "/" + form.Url;
                        string title = field.InternalName;

                        lnkNewEntry.OnClientClick = "javascript:SP.UI.ModalDialog.showModalDialog({ url: '" + url + "', title: '" + title + "', dialogReturnValueCallback:  callbackMethod" + this.Field.Id.ToString("n") + "});";
                    }
                }
                else
                {
                    lnkNewEntry.Click += new EventHandler(lnkNewEntry_Click);


                    lnkAdd.Click += new EventHandler(lnkAdd_Click);
                    lnkCancel.Click += new EventHandler(lnkCancel_Click);
                }
            }
            else
            {
                NewEntryPanel.Visible = false;
            }
        }

        #endregion

        #region Events

        void ParentControlDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Refresh Control Value
            PopulatingValue();

            // Update nested Controls
            UpdateChildLinkedControl(this.Field as CCSCascadedLookupField);
        }

        void lnkNewEntry_Click(object sender, EventArgs e)
        {
            txtNewEntry.Visible = true;
            lnkAdd.Visible = true;
            lnkCancel.Visible = true;
        }

        void lnkAdd_Click(object sender, EventArgs e)
        {
            CCSCascadedLookupField field = base.Field as CCSCascadedLookupField;
            if (field != null)
            {
                using (SPWeb spWeb = SPContext.Current.Site.OpenWeb(field.LookupWebId))
                {
                    SPList sourceList = spWeb.Lists[new Guid(field.LookupFieldListName)];

                    string parentValueId = "";

                    object parentValue = GetParentFieldValue(field);

                    SPFieldLookupValueCollection vals = parentValue as SPFieldLookupValueCollection;
                    if (vals != null && vals.Count > 0)
                    {
                        parentValueId = vals[0].LookupId.ToString();
                    }
                    else
                    {
                        SPFieldLookupValue singleValue = parentValue as SPFieldLookupValue;
                        if (singleValue != null)
                        {
                            parentValueId = singleValue.LookupId.ToString();
                        }
                    }


                    SPListItem item = sourceList.Items.Add();

                    //item[new Guid(field.LookupFieldName)] = txtNewEntry.Text;
                    item[field.LookupFieldName] = txtNewEntry.Text;

                    if (field.LinkToParent)
                    {
                        item[new Guid(field.GetParentLinkedColumnId())] = (string.IsNullOrEmpty(parentValueId) ? "" : parentValueId);
                    }

                    item.Update();

                    if (field.AllowMultipleValues)
                    {
                        lbRightBox.Items.Insert(lbRightBox.Items.Count, new ListItem(txtNewEntry.Text, item.ID.ToString()));
                    }
                    else
                    {
                        ddlCCSCascadeFieldControl.Items.Insert(ddlCCSCascadeFieldControl.Items.Count, new ListItem(txtNewEntry.Text, item.ID.ToString()));
                    }
                }
            }

            //-------------
            txtNewEntry.Visible = false;
            lnkAdd.Visible = false;
            lnkCancel.Visible = false;
        }

        void lnkCancel_Click(object sender, EventArgs e)
        {
            txtNewEntry.Visible = false;
            lnkAdd.Visible = false;
            lnkCancel.Visible = false;
        }

        void ddlCCSCascadeFieldControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Update nested Controls
            UpdateChildLinkedControl(this.Field as CCSCascadedLookupField);
        }

        void btnAdd_Click(object sender, EventArgs e)
        {
            if (lbLeftBox.Items.Count > 0)
            {
                for (int i = (lbLeftBox.Items.Count - 1); i >= 0; i--)
                {
                    ListItem li = lbLeftBox.Items[i];
                    if (li.Selected)
                    {
                        lbRightBox.Items.Add(new ListItem(li.Text, li.Value));
                        lbLeftBox.Items.RemoveAt(i);
                    }
                }
            }

            // Update nested Controls
            UpdateChildLinkedControl(this.Field as CCSCascadedLookupField);
        }

        void btnRemove_Click(object sender, EventArgs e)
        {
            if (lbRightBox.Items.Count > 0)
            {
                for (int i = (lbRightBox.Items.Count - 1); i >= 0; i--)
                {
                    ListItem li = lbRightBox.Items[i];
                    if (li.Selected)
                    {
                        lbLeftBox.Items.Add(new ListItem(li.Text, li.Value));
                        lbRightBox.Items.RemoveAt(i);
                    }
                }
            }

            // Update nested Controls
            UpdateChildLinkedControl(this.Field as CCSCascadedLookupField);
        }

        #endregion

        #region private functions
        
        Boolean ParentValueNullOREmpty(CCSCascadedLookupField field)
        {
            return GetParentFieldValue(field) == null;
        }

        object GetParentFieldValue(CCSCascadedLookupField field)
        {
            if (this.Page.IsPostBack)
            {
                return GetParentFieldValuePostBack(field);
            }
            else
            {
                return GetParentFieldValueStart(field);
            }
        }

        object GetParentFieldValueStart(CCSCascadedLookupField field)
        {
            if (field.LinkToParent)
            {
                string ParentColumnId = field.GetParentColumnId();
                if (!string.IsNullOrEmpty(ParentColumnId))
                {
                    SPFieldLookup fieldParent = SPContext.Current.List.Fields[new Guid(ParentColumnId)] as SPFieldLookup;
                    if (fieldParent.AllowMultipleValues)
                    {

                        SPFieldLookupValueCollection valColl = fieldParent.FieldRenderingControl.ItemFieldValue as SPFieldLookupValueCollection;
                        if (valColl != null && valColl.Count > 0)
                        {
                            return valColl;
                        }

                        string val = fieldParent.FieldRenderingControl.ItemFieldValue as string;

                        if (!string.IsNullOrEmpty(val))
                        {
                            string[] vals = val.Split(new string[] { ";#" }, StringSplitOptions.None);

                            if (vals.Length >= 2)
                            {
                                valColl = new SPFieldLookupValueCollection();
                                for (int i = 0; i <= vals.Length - 2; i = i + 2)
                                {
                                    valColl.Add(new SPFieldLookupValue(int.Parse(vals[i]), vals[i + 1]));
                                }

                                return valColl;
                            }
                        }
                    }
                    else
                    {
                        if (fieldParent.FieldRenderingControl.ItemFieldValue != null)
                        {
                            SPFieldLookupValue lookupVal = fieldParent.FieldRenderingControl.ItemFieldValue as SPFieldLookupValue;

                            if (lookupVal != null)
                            {
                                return lookupVal;
                            }
                            else
                            {
                                string val = fieldParent.FieldRenderingControl.ItemFieldValue.ToString();
                                if (!string.IsNullOrEmpty(val))
                                {
                                    string[] vals = val.Split(new string[] { ";#" }, StringSplitOptions.None);
                                    if (vals.Length == 2)
                                    {
                                        return new SPFieldLookupValue(int.Parse(vals[0]), vals[1]);
                                    }
                                    else if (vals.Length == 1)
                                    {
                                        return vals[0];
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }

        object GetParentFieldValuePostBack(CCSCascadedLookupField field)
        {
            if (field != null)
            {
                if (field.LinkToParent)
                {
                    string ParentColumnId = field.GetParentColumnId();
                    if (!string.IsNullOrEmpty(ParentColumnId))
                    {
                        SPFieldLookup fieldParent = SPContext.Current.List.Fields[new Guid(ParentColumnId)] as SPFieldLookup;

                        if (fieldParent != null)
                        {
                            List<Control> collect = new List<Control>();

                            Utilities.FindControlRecursive(Page, typeof(MultipleLookupField), ref collect);
                            if (collect.Count > 0)
                            {
                                foreach (Control ctrl in collect)
                                {
                                    if (((MultipleLookupField)ctrl).FieldName == fieldParent.InternalName)
                                    {
                                        return ((MultipleLookupField)ctrl).Value;
                                    }
                                }
                            }

                            collect.Clear();
                            Utilities.FindControlRecursive(Page, typeof(CCSCascadedLookupControl), ref collect);
                            if (collect.Count > 0)
                            {
                                foreach (Control ctrl in collect)
                                {
                                    if (((CCSCascadedLookupControl)ctrl).FieldName == fieldParent.InternalName)
                                    {
                                        return ((CCSCascadedLookupControl)ctrl).Value;
                                    }
                                }
                            }

                            collect.Clear();
                            Utilities.FindControlRecursive(Page, typeof(LookupField), ref collect);

                            if (collect.Count > 0)
                            {
                                foreach (Control ctrl in collect)
                                {
                                    if (((LookupField)ctrl).FieldName == fieldParent.InternalName)
                                    {
                                        return ((LookupField)ctrl).Value;
                                    }
                                }
                            }

                        }
                    }
                }
            }

            return null;
        }

        void AddeventOnParentControls()
        {
            CCSCascadedLookupField field = base.Field as CCSCascadedLookupField;
            if (field != null)
            {
                if (field.LinkToParent)
                {
                    string ParentColumnId = field.GetParentColumnId();
                    if (!string.IsNullOrEmpty(ParentColumnId))
                    {
                        SPFieldLookup fieldParent = SPContext.Current.List.Fields[new Guid(ParentColumnId)] as SPFieldLookup;

                        if (fieldParent != null)
                        {
                            List<Control> collect = new List<Control>();

                            Control parentControl = null;
                            Utilities.FindControlRecursive(Page, typeof(MultipleLookupField), ref collect);
                            if (collect.Count > 0)
                            {
                                foreach (Control ctrl in collect)
                                {
                                    if (((MultipleLookupField)ctrl).FieldName == fieldParent.InternalName)
                                    {
                                        parentControl = ctrl;
                                    }
                                }
                            }

                            if (parentControl == null)
                            {
                                collect.Clear();
                                Utilities.FindControlRecursive(Page, typeof(LookupField), ref collect);

                                if (collect.Count > 0)
                                {
                                    foreach (Control ctrl in collect)
                                    {
                                        if (((LookupField)ctrl).FieldName == fieldParent.InternalName)
                                        {
                                            parentControl = ctrl;
                                        }
                                    }
                                }
                            }

                            if (parentControl != null)
                            {
                                if (fieldParent.AllowMultipleValues)
                                {
                                    List<Control> childParentControls = new List<Control>();

                                    Utilities.FindControlRecursive(parentControl, typeof(SPHtmlSelect), ref childParentControls);
                                    Utilities.FindControlRecursive(parentControl, typeof(System.Web.UI.HtmlControls.HtmlButton), ref childParentControls);
                                    if (childParentControls != null && childParentControls.Count > 0)
                                    {
                                        RegisterJavaScriptOnMultipleLookupControl(childParentControls[0].ClientID, childParentControls[1].ClientID, childParentControls[2].ClientID, childParentControls[3].ClientID, "__doPostBack('ParentField" + ParentColumnId + "', '');");
                                    }
                                }
                                else
                                {
                                    List<Control> childParentControls = new List<Control>();

                                    Utilities.FindControlRecursive(parentControl, typeof(DropDownList), ref childParentControls);
                                    if (childParentControls != null && childParentControls.Count > 0)
                                    {
                                        foreach (Control ctrl in childParentControls)
                                        {
                                            ((DropDownList)ctrl).SelectedIndexChanged += new EventHandler(ParentControlDropdown_SelectedIndexChanged);
                                            ((DropDownList)ctrl).AutoPostBack = true;
                                        }
                                    }
                                    else
                                    {
                                        childParentControls.Clear();
                                        Utilities.FindControlRecursive(parentControl, typeof(TextBox), ref childParentControls);
                                        if (childParentControls != null && childParentControls.Count > 0)
                                        {
                                            foreach (Control ctrl in childParentControls)
                                            {
                                                Page.ClientScript.RegisterStartupScript(new object().GetType(), "101", "var parentFocus" + ctrl.ClientID + " = '0';");

                                                ((TextBox)ctrl).Attributes.Add("onblur", "parentFocus" + ctrl.ClientID + " = '0'; this.value = this.match;");
                                                ((TextBox)ctrl).Attributes.Add("onfocus", "parentFocus" + ctrl.ClientID + " = '1'");
                                                ((TextBox)ctrl).Attributes.Add("onpropertychange", "if(parentFocus" + ctrl.ClientID + " != '1') { if(this.match != document.getElementById('" + hiddenFieldType.ClientID + "').value) { document.getElementById('" + hiddenFieldType.ClientID + "').value = this.match;  setTimeout(__doPostBack('ParentFieldAuto" + ParentColumnId + "',document.getElementById(this.optHid).value.toString()), 0) }}");
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        void SetParentValueIfAutoComplete(object Val)
        {
            CCSCascadedLookupField field = base.Field as CCSCascadedLookupField;
            if (field != null)
            {
                if (field.LinkToParent)
                {
                    string ParentColumnId = field.GetParentColumnId();
                    if (!string.IsNullOrEmpty(ParentColumnId))
                    {
                        SPFieldLookup fieldParent = SPContext.Current.List.Fields[new Guid(ParentColumnId)] as SPFieldLookup;

                        if (fieldParent != null)
                        {
                            List<Control> collect = new List<Control>();

                            Control parentControl = null;
                            Utilities.FindControlRecursive(Page, typeof(MultipleLookupField), ref collect);
                            if (collect.Count > 0)
                            {
                                foreach (Control ctrl in collect)
                                {
                                    if (((MultipleLookupField)ctrl).FieldName == fieldParent.InternalName)
                                    {
                                        parentControl = ctrl;
                                    }
                                }
                            }

                            if (parentControl == null)
                            {
                                collect.Clear();
                                Utilities.FindControlRecursive(Page, typeof(LookupField), ref collect);

                                if (collect.Count > 0)
                                {
                                    foreach (Control ctrl in collect)
                                    {
                                        if (((LookupField)ctrl).FieldName == fieldParent.InternalName)
                                        {
                                            parentControl = ctrl;
                                        }
                                    }
                                }
                            }

                            if (parentControl != null)
                            {
                                if (!fieldParent.AllowMultipleValues)
                                {
                                    List<Control> childParentControls = new List<Control>();

                                    childParentControls.Clear();
                                    Utilities.FindControlRecursive(parentControl, typeof(TextBox), ref childParentControls);
                                    if (childParentControls != null && childParentControls.Count > 0)
                                    {
                                        fieldParent.FieldRenderingControl.ItemFieldValue = Val;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Register Javascript Event on Parent MultipleLookup field Control

        void RegisterJavaScriptOnMultipleLookupControl(string leftBox, string rightBox, string addbutton, string removeButton, string ValueString)
        {
            if (!Page.ClientScript.IsStartupScriptRegistered("Parent" + this.Field.Id.ToString("n")))
            {
                System.Text.StringBuilder sb1 = new System.Text.StringBuilder();

                sb1.Append(@"function LoadMethod" + this.Field.Id.ToString("n") + "()");
                sb1.Append(@"{");
                //sb1.Append(@"alert(document.getElementById('" + leftBox + "').getAttribute(\"ondblclick\"));");

                //leftBox
                sb1.Append(@"var str = document.getElementById('" + leftBox + "').getAttribute(\"ondblclick\");");
                sb1.Append(@"if (str.toString().indexOf(""" + ValueString + "\") < 0) {");
                sb1.Append(@"if (str.toString().indexOf(""return false"") > 0) {");
                sb1.Append(@"    document.getElementById('" + leftBox + "').setAttribute(\"ondblclick\", str.substring(0, str.toString().indexOf(\"return false\")) + \" " + ValueString + " return false\");");
                sb1.Append(@"}");
                sb1.Append(@"else {");
                sb1.Append(@"document.getElementById('" + leftBox + "').setAttribute(\"ondblclick\", str + \"" + ValueString + "\");");
                sb1.Append(@"}");
                sb1.Append(@"}");
                //sb1.Append(@"alert(document.getElementById('" + leftBox + "').getAttribute(\"ondblclick\"));");

                //rightbox
                sb1.Append(@"var str = document.getElementById('" + rightBox + "').getAttribute(\"ondblclick\");");
                sb1.Append(@"if (str.toString().indexOf(""" + ValueString + "\") < 0) {");
                sb1.Append(@"if (str.toString().indexOf(""return false"") > 0) {");
                sb1.Append(@"    document.getElementById('" + rightBox + "').setAttribute(\"ondblclick\", str.substring(0, str.toString().indexOf(\"return false\")) + \" " + ValueString + " return false\");");
                sb1.Append(@"}");
                sb1.Append(@"else {");
                sb1.Append(@"document.getElementById('" + rightBox + "').setAttribute(\"ondblclick\", str + \"" + ValueString + "\");");
                sb1.Append(@"}");
                sb1.Append(@"}");
                //sb1.Append(@"alert(document.getElementById('" + rightBox + "').getAttribute(\"ondblclick\"));");

                //addbutton
                sb1.Append(@"var str = document.getElementById('" + addbutton + "').getAttribute(\"onclick\");");
                sb1.Append(@"if (str.toString().indexOf(""" + ValueString + "\") < 0) {");
                sb1.Append(@"if (str.toString().indexOf(""return false"") > 0) {");
                sb1.Append(@"    document.getElementById('" + addbutton + "').setAttribute(\"onclick\", str.substring(0, str.toString().indexOf(\"return false\")) + \" " + ValueString + " return false\");");
                sb1.Append(@"}");
                sb1.Append(@"else {");
                sb1.Append(@"document.getElementById('" + addbutton + "').setAttribute(\"onclick\", str + \"" + ValueString + "\");");
                sb1.Append(@"}");
                sb1.Append(@"}");
                //sb1.Append(@"alert(document.getElementById('" + addbutton + "').getAttribute(\"onclick\"));");

                //removeButton
                sb1.Append(@"var str = document.getElementById('" + removeButton + "').getAttribute(\"onclick\");");
                sb1.Append(@"if (str.toString().indexOf(""" + ValueString + "\") < 0) {");
                sb1.Append(@"if (str.toString().indexOf(""return false"") > 0) {");
                sb1.Append(@"    document.getElementById('" + removeButton + "').setAttribute(\"onclick\", str.substring(0, str.toString().indexOf(\"return false\")) + \" " + ValueString + " return false\");");
                sb1.Append(@"}");
                sb1.Append(@"else {");
                sb1.Append(@"document.getElementById('" + removeButton + "').setAttribute(\"onclick\", str + \"" + ValueString + "\");");
                sb1.Append(@"}");
                sb1.Append(@"}");
                //sb1.Append(@"alert(document.getElementById('" + removeButton + "').getAttribute(\"onclick\"));");

                sb1.Append(@"}");

                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Parent" + this.Field.Id.ToString("n"), sb1.ToString(), true);

                if (!Page.ClientScript.IsStartupScriptRegistered("100"))
                {
                    Page.ClientScript.RegisterStartupScript(new object().GetType(), "100", " if (true) _spBodyOnLoadFunctionNames.push('LoadMethod" + this.Field.Id.ToString("n") + "');", true);//if (ControlMode != SPControlMode.Display)
                }
            }
        }

        #endregion

        #region Update Nested Controls Value

        public void UpdateChildLinkedControl(CCSCascadedLookupField CurrentField)
        {
            List<Control> PossibleControls = new List<Control>();
            Utilities.FindControlRecursive(this.Page, typeof(CCSCascadedLookupControl), ref PossibleControls);

            if (CurrentField.AllowMultipleValues)
            {
                List<string> vals = null;
                if (lbRightBox.Items.Count > 0)
                {
                    vals = new List<string>();
                    foreach (ListItem li in lbRightBox.Items)
                    {
                        vals.Add(li.Value);
                    }
                }

                UpdateChildLinkedControl(CurrentField, vals, ref PossibleControls);
            }
            else
            {
                if (ddlCCSCascadeFieldControl.SelectedItem != null)
                {
                    UpdateChildLinkedControl(CurrentField, ddlCCSCascadeFieldControl.SelectedItem.Value, ref PossibleControls);
                }
            }
        }

        public void UpdateChildLinkedControl(CCSCascadedLookupField CurrentField, Object CurrentControlValue, ref List<Control> AllPossibleControls)
        {
            Object childCtrlValue = null;
            foreach (Control ctrl in AllPossibleControls)
            {
                CCSCascadedLookupControl ChildControl = ctrl as CCSCascadedLookupControl;
                if (ChildControl != null)
                {
                    CCSCascadedLookupField field = ChildControl.Field as CCSCascadedLookupField;

                    if (CurrentField.Id.ToString() == field.GetParentColumnId())
                    {
                        if (field != null && field.LinkToParent)
                        {
                            string linked_column = field.GetParentLinkedColumnId();
                            List<ListItem> poplateItemsList = new List<ListItem>();
                            if (CurrentControlValue != null && CurrentControlValue.ToString() != "")
                            {
                                if (CurrentControlValue is string)
                                {
                                    Utilities.FetchMatchedValuesFromList(field, CurrentControlValue.ToString(), ref poplateItemsList);
                                }
                                else if (CurrentControlValue is List<string>)
                                {
                                    List<string> listItems = CurrentControlValue as List<string>;
                                    if (listItems.Count > 0)
                                    {
                                        foreach (string val in CurrentControlValue as List<string>)
                                        {
                                            Utilities.FetchMatchedValuesFromList(field, val, ref poplateItemsList);
                                        }
                                    }
                                    else if (field.ShowAllOnEmpty)
                                    {
                                        Utilities.FetchAllValuesFromList(field, ref poplateItemsList);
                                    }
                                }
                            }
                            else if (field.ShowAllOnEmpty)
                            {
                                Utilities.FetchAllValuesFromList(field, ref poplateItemsList);
                            }

                            //Set Child control Values
                            List<Control> ChildControls = new List<Control>();
                            if (field.AllowMultipleValues)
                            {
                                Utilities.FindControlRecursive(ChildControl, typeof(ListBox), ref ChildControls);
                                if (ChildControls != null)
                                {
                                    ListBox rightListBox = ChildControls[1] as ListBox;
                                    ListBox leftListBox = ChildControls[0] as ListBox;

                                    if (poplateItemsList != null && poplateItemsList.Count > 0)
                                    {
                                        //childListBox.Items.AddRange(poplateItemsList.ToArray());
                                        if (rightListBox.Items != null && rightListBox.Items.Count > 0)
                                        {
                                            List<string> vals = new List<string>();
                                            for (int i = rightListBox.Items.Count - 1; i >= 0; i--)
                                            {
                                                if (!CheckListItemExistandRemove(rightListBox.Items[i], ref poplateItemsList))
                                                {
                                                    rightListBox.Items.RemoveAt(i);
                                                }
                                                else
                                                {
                                                    vals.Add(rightListBox.Items[i].Value);
                                                }
                                            }

                                            if (vals.Count > 0) childCtrlValue = vals;
                                        }

                                        for (int i = leftListBox.Items.Count - 1; i >= 0; i--)
                                        {
                                            if (!CheckListItemExistandRemove(leftListBox.Items[i], ref poplateItemsList))
                                            {
                                                leftListBox.Items.RemoveAt(i);
                                            }
                                        }

                                        leftListBox.Items.AddRange(poplateItemsList.ToArray());
                                    }
                                    else
                                    {
                                        rightListBox.Items.Clear();
                                        leftListBox.Items.Clear();
                                    }
                                }
                            }
                            else
                            {
                                Utilities.FindControlRecursive(ChildControl, typeof(DropDownList), ref ChildControls);
                                if (ChildControls != null)
                                {
                                    foreach (DropDownList childListBox in ChildControls)
                                    {
                                        childListBox.Items.Clear();
                                        if (this.ControlMode == SPControlMode.New || !Field.Required)
                                            childListBox.Items.Insert(0, new ListItem("(None)", "0"));

                                        if (poplateItemsList != null && poplateItemsList.Count > 0)
                                            childListBox.Items.AddRange(poplateItemsList.ToArray());

                                        childListBox.SelectedIndex = 0;
                                    }
                                }
                            }

                            //Reset nested child controls Value
                            UpdateChildLinkedControl(field, childCtrlValue, ref AllPossibleControls);
                        }
                    }
                }
            }
        }

        bool CheckListItemExistandRemove(ListItem li, ref List<ListItem> Items)
        {
            if (Items != null && Items.Count > 0)
            {
                for (int i = Items.Count - 1; i >= 0; i--)
                {
                    if (Items[i].Value == li.Value)
                    {
                        Items.RemoveAt(i);
                        return true;
                    }
                }
            }

            return false;
        }

        #endregion
    }
}