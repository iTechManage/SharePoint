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

        HtmlSelect ddlCCSCascadeFieldControl = null;
        HtmlSelect lbLeftBox = null;
        HtmlSelect lbRightBox = null;
        Button btnAdd = null;
        Button btnRemove = null;

        LinkButton lnkNewEntry = null;
        TextBox txtNewEntry = null;
        LinkButton lnkAdd = null;
        LinkButton lnkCancel = null;

        HtmlInputHidden hFieldValue = null;
        #endregion

        #region Override Properties and Methods

        protected override string DefaultTemplateName { get { return "CCSCascadedLookupControl"; } }

        protected override void OnInit(EventArgs e)
        {
            //if (Microsoft.SharePoint.Administration.SPFarm.Local.Properties["CCScascadeId"] == null)
            //{s
            //    System.Web.HttpContext.Current.Items["FormDigestValidated"] = true;
            //    SPSecurity.RunWithElevatedPrivileges(delegate
            //    {
            //        SPContext.Current.Site.WebApplication.Farm.Properties.Add("CCScascadeId", "test1234");
            //        SPContext.Current.Site.WebApplication.Farm.Update();
            //    });
            //}
            //object v = SPContext.Current.Site.WebApplication.Farm.Properties["CCScascadeId"];
            base.OnInit(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (this.ControlMode == SPControlMode.Display || this.ControlMode == SPControlMode.Invalid)
            {
                return;
            }

            Page.ClientScript.RegisterClientScriptInclude("1000", "/_layouts/CrowCanyon.CascadedLookup/CCSCascadeLookup.js");

            CCSCascadedLookupField field = base.Field as CCSCascadedLookupField;

            AddClientsideEvents();

            if (!Page.ClientScript.IsStartupScriptRegistered(this.Field.Id.ToString("n")))
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append(@"<script language='javascript'>");
                sb.Append(@"function callbackMethod" + this.Field.Id.ToString("n") + " (dialogResult, returnValue)");
                sb.Append(@"{");
                sb.Append(@"if(dialogResult == 1)");
                sb.Append(@"{");
                sb.Append(@" UpdateMyControls('" + GetControlId(field) + "', '');");
                sb.Append(@"}");
                sb.Append(@"}");
                sb.Append(@"</script>");

                Page.ClientScript.RegisterStartupScript(new object().GetType(), this.Field.Id.ToString("n"), sb.ToString());
            }


            PopulatingValue();

            #region Set Field Value to Control Value

            if (!Page.IsPostBack)
            {

                hFieldValue.Value = "";
                if (ItemFieldValue != null)
                {
                    SetControlsValue(ItemFieldValue);
                    hFieldValue.Value = ItemFieldValue.ToString();
                }

                
            }
            else
            {
                if (this.Value != null)
                {
                    SetControlsValue(this.Value);
                }
            }

            #endregion
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

                if (string.IsNullOrEmpty(hFieldValue.Value)) { return null; }

                CCSCascadedLookupField field = base.Field as CCSCascadedLookupField;
                if (field.AllowMultipleValues)
                {
                    SPFieldLookupValueCollection vals = new SPFieldLookupValueCollection(hFieldValue.Value);
                    
                    return vals;
                }
                else
                {
                    SPFieldLookupValue val = new SPFieldLookupValue(hFieldValue.Value);
                    
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
                            if (vals.Count > 0)
                            {
                                foreach (SPFieldLookupValue val in vals)
                                {
                                    Utilities.FetchMatchedValuesFromList(field, val.LookupId.ToString(), ref poplateItemsList);
                                }
                            }
                            else
                            {
                                parentValue = null;
                            }
                        }
                        else
                        {
                            SPFieldLookupValue singleValue = parentValue as SPFieldLookupValue;
                            if (singleValue != null)
                            {
                                if (singleValue.LookupId > 0)
                                {
                                    Utilities.FetchMatchedValuesFromList(field, singleValue.LookupId.ToString(), ref poplateItemsList);
                                }
                                else
                                {
                                    parentValue = null;
                                }
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
                        lbLeftBox.Items.Clear();
                        lbRightBox.Items.Clear();

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

            ddlCCSCascadeFieldControl = (HtmlSelect)TemplateContainer.FindControl("ddlCCSCascadeFieldControl");
            lbLeftBox = (HtmlSelect)TemplateContainer.FindControl("lbLeftBox");
            lbRightBox = (HtmlSelect)TemplateContainer.FindControl("lbRightBox");
            btnAdd = (Button)TemplateContainer.FindControl("btnAdd");
            btnRemove = (Button)TemplateContainer.FindControl("btnRemove");

            lnkNewEntry = (LinkButton)TemplateContainer.FindControl("lnkNewEntry");
            txtNewEntry = (TextBox)TemplateContainer.FindControl("txtNewEntry");
            lnkAdd = (LinkButton)TemplateContainer.FindControl("lnkAdd");
            lnkCancel = (LinkButton)TemplateContainer.FindControl("lnkCancel");

            hFieldValue = (HtmlInputHidden)TemplateContainer.FindControl("hFieldValue");

            lbLeftBox.EnableViewState = true;
            lbRightBox.EnableViewState = true;
            ddlCCSCascadeFieldControl.EnableViewState = true;
                    
                
            if (field.AllowMultipleValues)
            {
                SingleValuePanel.Visible = false;
                MultipleValuePanel.Visible = true;

                lbLeftBox.Multiple = true;
                lbRightBox.Multiple = true;

                lbLeftBox.Attributes.Add("ondblclick", "Listbox_MoveAcross('" + lbLeftBox.ClientID + "','" + lbRightBox.ClientID + "'); SetValueFromListBox('" + hFieldValue.ClientID + "','" + lbRightBox.ClientID + "'); UpdateMyChildControls('" + lbLeftBox.ClientID + ";#" + lbRightBox.ClientID + "');");
                lbRightBox.Attributes.Add("ondblclick", "Listbox_MoveAcross('" + lbRightBox.ClientID + "','" + lbLeftBox.ClientID + "'); SetValueFromListBox('" + hFieldValue.ClientID + "','" + lbRightBox.ClientID + "'); UpdateMyChildControls('" + lbLeftBox.ClientID + ";#" + lbRightBox.ClientID + "');");

                btnAdd.Attributes.Add("onclick", "Listbox_MoveAcross('" + lbLeftBox.ClientID + "','" + lbRightBox.ClientID + "'); SetValueFromListBox('" + hFieldValue.ClientID + "','" + lbRightBox.ClientID + "'); UpdateMyChildControls('" + lbLeftBox.ClientID + ";#" + lbRightBox.ClientID + "'); return false;");
                btnRemove.Attributes.Add("onclick", "Listbox_MoveAcross('" + lbRightBox.ClientID + "','" + lbLeftBox.ClientID + "'); SetValueFromListBox('" + hFieldValue.ClientID + "','" + lbRightBox.ClientID + "'); UpdateMyChildControls('" + lbLeftBox.ClientID + ";#" + lbRightBox.ClientID + "'); return false;");

            }
            else
            {
                SingleValuePanel.Visible = true;
                MultipleValuePanel.Visible = false;

                ddlCCSCascadeFieldControl.Attributes.Add("onchange", "SetValueFromDropDown('" + hFieldValue.ClientID + "','" + ddlCCSCascadeFieldControl.ClientID + "'); UpdateMyChildControls('" + ddlCCSCascadeFieldControl.ClientID + "');");
            }

            if (field.AllowNewEntry)
            {
                NewEntryPanel.Visible = true;

                lnkNewEntry.Visible = true;
                txtNewEntry.Visible = true;
                lnkAdd.Visible = true;
                lnkCancel.Visible = true;

                txtNewEntry.Style.Add(HtmlTextWriterStyle.Display, "none");
                lnkAdd.Style.Add(HtmlTextWriterStyle.Display, "none");
                lnkCancel.Style.Add(HtmlTextWriterStyle.Display, "none");

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
                    lnkNewEntry.OnClientClick = "document.getElementById('" + txtNewEntry.ClientID + "').value = ''; document.getElementById('" + txtNewEntry.ClientID + "').style.display=''; document.getElementById('" + lnkAdd.ClientID + "').style.display=''; document.getElementById('" + lnkCancel.ClientID + "').style.display=''; return false;";
                
                    lnkAdd.OnClientClick = "createListItem('" + GetControlId(field) + "','" + txtNewEntry.ClientID + "'); document.getElementById('" + txtNewEntry.ClientID + "').value = ''; document.getElementById('" + txtNewEntry.ClientID + "').style.display='none'; document.getElementById('" + lnkAdd.ClientID + "').style.display='none'; document.getElementById('" + lnkCancel.ClientID + "').style.display='none'; return false;";
                    lnkCancel.OnClientClick = "document.getElementById('" + txtNewEntry.ClientID + "').value = ''; document.getElementById('" + txtNewEntry.ClientID + "').style.display='none'; document.getElementById('" + lnkAdd.ClientID + "').style.display='none'; document.getElementById('" + lnkCancel.ClientID + "').style.display='none'; return false;";
                }
            }
            else
            {
                NewEntryPanel.Visible = false;
            }
        }

        #endregion

       
        #region private functions
        
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

        void AddClientsideEvents()
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
                                        RegisterJavaScriptOnMultipleLookupControl(fieldParent.Id.ToString("n"), childParentControls[0].ClientID, childParentControls[1].ClientID, childParentControls[2].ClientID, childParentControls[3].ClientID, "UpdateMyChildControls('" + childParentControls[0].ClientID+ ";#"+ childParentControls[1].ClientID + "');");

                                        RegisterAddCascadedControlScript(field, childParentControls[0].ClientID + ";#" + childParentControls[1].ClientID, "2");
                                        return;
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
                                            ((DropDownList)ctrl).Attributes.Add("onchange", "UpdateMyChildControls('" + ctrl.ClientID + "');");

                                            RegisterAddCascadedControlScript(field, ctrl.ClientID, "0");
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        childParentControls.Clear();
                                        Utilities.FindControlRecursive(parentControl, typeof(TextBox), ref childParentControls);
                                        if (childParentControls != null && childParentControls.Count > 0)
                                        {
                                            Page.ClientScript.RegisterStartupScript(new object().GetType(), "101", "var parentFocus" + childParentControls[0].ClientID + " = '0';", true);

                                            ((TextBox)childParentControls[0]).Attributes.Add("onblur", "parentFocus" + childParentControls[0].ClientID + " = '0'; UpdateMyChildControls('" + childParentControls[0].ClientID + "');");
                                            ((TextBox)childParentControls[0]).Attributes.Add("onfocus", "parentFocus" + childParentControls[0].ClientID + " = '1'");
                                            ((TextBox)childParentControls[0]).Attributes.Add("onpropertychange", "if(parentFocus" + childParentControls[0].ClientID + " != '1') { UpdateMyChildControls('" + childParentControls[0].ClientID + "'); }");

                                            RegisterAddCascadedControlScript(field, childParentControls[0].ClientID, "1");
                                            return;
                                        }
                                    }
                                }
                            }


                            if (parentControl == null)
                            {
                                collect.Clear();
                                Utilities.FindControlRecursive(Page, typeof(CCSCascadedLookupControl), ref collect);

                                if (collect.Count > 0)
                                {
                                    foreach (Control ctrl in collect)
                                    {
                                        if (((CCSCascadedLookupControl)ctrl).FieldName == fieldParent.InternalName)
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

                                    Utilities.FindControlRecursive(parentControl, typeof(HtmlSelect), ref childParentControls);
                                    if (childParentControls != null && childParentControls.Count == 2)
                                    {
                                        RegisterAddCascadedControlScript(field, childParentControls[0].ClientID + ";#" + childParentControls[1].ClientID, "0");
                                        return;
                                    }
                                }
                                else
                                {
                                    List<Control> childParentControls = new List<Control>();

                                    Utilities.FindControlRecursive(parentControl, typeof(HtmlSelect), ref childParentControls);
                                    if (childParentControls != null && childParentControls.Count > 0)
                                    {
                                        RegisterAddCascadedControlScript(field, childParentControls[0].ClientID, "0");
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }

                RegisterAddCascadedControlScript(field, "", "-1");
            }
        }

        #endregion

        #region Register Javascript Event on Parent MultipleLookup field Control

        void RegisterJavaScriptOnMultipleLookupControl(string ParentFieldId, string leftBox, string rightBox, string addbutton, string removeButton, string ValueString)
        {
            if (!Page.ClientScript.IsStartupScriptRegistered("Parent" + ParentFieldId))
            {
                System.Text.StringBuilder sb1 = new System.Text.StringBuilder();

                sb1.Append(@"function LoadMethod" + ParentFieldId + "()");
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

                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Parent" + ParentFieldId, sb1.ToString(), true);

                if (!Page.ClientScript.IsStartupScriptRegistered("100"))
                {
                    Page.ClientScript.RegisterStartupScript(new object().GetType(), "100", " if (true) {_spBodyOnLoadFunctionNames.push('LoadMethod" + ParentFieldId + "');}", true);//if (ControlMode != SPControlMode.Display)
                }
            }
        }

        #endregion

        void RegisterAddCascadedControlScript(CCSCascadedLookupField field, string parentControlId, string ParentControlType)
        {
            string webUrl = "";
            string LookuplistName = "";
            string ParentLinkedFieldName = "";
            string LookupFieldName = "";
            string ViewWhereString = "";
            string ViewOrderString = "";

            string controlId = GetControlId(field);

            Utilities.GetParametersValue(field, out webUrl, out LookuplistName, out ParentLinkedFieldName, out LookupFieldName, out ViewWhereString, out ViewOrderString);

            //function AddCascadedControl(controlId, isAllowMultiple, allValuesOnEmpty, parentControlId, parentControlType, webUrl, lookupListName, linkedParentfield, lookupTargetField, viewWhereString, viewOrderString) {
            string AddCascadeControlScript = "new AddCascadedControl('" + controlId + "', " + field.AllowMultipleValues.ToString().ToLower() + ", " + field.ShowAllOnEmpty.ToString().ToLower() + ", '" + hFieldValue.ClientID + "','" + parentControlId + "', " + ParentControlType + ", '" + webUrl + "', '" + LookuplistName + "', '" + ParentLinkedFieldName + "', '" + LookupFieldName + "', '" + ViewWhereString + "', '" + ViewOrderString + "');";
            
            Page.ClientScript.RegisterStartupScript(new object().GetType(), "ChildControl" + this.Field.Id.ToString("n"), "if(true){ " + AddCascadeControlScript + "}", true);
        }

        string GetControlId(CCSCascadedLookupField field)
        {
            if (field.AllowMultipleValues)
            {
                return lbLeftBox.ClientID + ";#" + lbRightBox.ClientID;
            }
            else
            {
                return ddlCCSCascadeFieldControl.ClientID;
            }
        }

        void SetControlsValue(object ControlValue)
        {
            CCSCascadedLookupField field = base.Field as CCSCascadedLookupField;

            if (ControlValue != null)
            {
                if (field != null)
                {
                    if (field.AllowMultipleValues)
                    {
                        SPFieldLookupValueCollection vals = ControlValue as SPFieldLookupValueCollection;
                        if (vals != null)
                        {
                            foreach (SPFieldLookupValue val in vals)
                            {
                                ListItem li = lbLeftBox.Items.FindByValue(val.LookupId.ToString());
                                if (li != null)
                                {
                                    lbLeftBox.Items.Remove(li);
                                    lbRightBox.Items.Add(li);
                                }
                            }
                        }
                    }
                    else
                    {
                        SPFieldLookupValue singleValue = ControlValue as SPFieldLookupValue;
                        if (singleValue != null && ddlCCSCascadeFieldControl.Items != null && ddlCCSCascadeFieldControl.Items.Count > 0)
                        {
                            for (int i = 0; i < ddlCCSCascadeFieldControl.Items.Count; i++)
                            {
                                if (ddlCCSCascadeFieldControl.Items[i].Value == singleValue.LookupId.ToString())
                                {
                                    ddlCCSCascadeFieldControl.SelectedIndex = i;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}