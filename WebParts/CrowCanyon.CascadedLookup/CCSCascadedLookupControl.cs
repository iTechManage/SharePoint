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

        Literal ErrorText = null;

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
            using (new EnterExitLogger("CCSCascadedLookupControl:OnLoad function"))
            {
                try
                {
                    base.OnLoad(e);

                    if (this.ControlMode == SPControlMode.Display || this.ControlMode == SPControlMode.Invalid)
                    {
                        return;
                    }

                    Utils.LogManager.write("Registering javascript file 'CCSCascadeLookup.js' file");
                    Page.ClientScript.RegisterClientScriptInclude("1000", "/_layouts/CrowCanyon.CascadedLookup/CCSCascadeLookup.js");

                    CCSCascadedLookupField field = base.Field as CCSCascadedLookupField;
                    Utils.LogManager.write("Field : " + field.Title);

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
                        Utils.LogManager.write("Registered callbackMethod for creating New entry for Field : " + field.Title);
                    }


                    PopulatingValue();
                    Utils.LogManager.write("Controls Value Populated");

                    #region Set Field Value to Control Value

                    if (!Page.IsPostBack)
                    {
                        Utils.LogManager.write("Setting the Controls and hidden field Value ");
                        hFieldValue.Value = "";
                        if (ItemFieldValue != null)
                        {
                            SetControlsValue(ItemFieldValue);
                            hFieldValue.Value = ItemFieldValue.ToString();
                        }
                    }
                    else
                    {
                        Utils.LogManager.write("Setting the Controls Value ");
                        Utils.LogManager.write("");
                        if (this.Value != null)
                        {
                            SetControlsValue(this.Value);
                        }
                    }

                    #endregion
                }
                catch (Exception ex)
                {
                    Utils.LogManager.write("Exception Occurs in OnLoad Function. \r\nError Message: " + ex.Message + "\r\nStack Trace: " + ex.StackTrace, "error" );
                    ShowErrorMessage(ex.Message);
                }
            }
        }

        protected override void CreateChildControls()
        {
            using (new EnterExitLogger("CCSCascadedLookupControl:CreateChildControls function"))
            {
                try
                {
                    CCSCascadedLookupField field = base.Field as CCSCascadedLookupField;
                    if (field != null && this.ControlMode != SPControlMode.Display)
                    {
                        base.CreateChildControls();
                        Utils.LogManager.write("Child controls Created: " + this.ChildControlsCreated.ToString());
                        
                        if (!this.ChildControlsCreated)
                        {
                            ShowControls(field);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Utils.LogManager.write("Exception Occurs in CreateChildControls Function. \r\nError Message: " + ex.Message + "\r\nStack Trace: " + ex.StackTrace, "error");
                    ShowErrorMessage(ex.Message);
                }
            }
        }

        public override object Value
        {
            get
            {
                using (new EnterExitLogger("CCSCascadedLookupControl:Value Get Property"))
                {
                    try
                    {
                        EnsureChildControls();

                        if (string.IsNullOrEmpty(hFieldValue.Value)) { return null; }

                        CCSCascadedLookupField field = base.Field as CCSCascadedLookupField;
                        if (field.AllowMultipleValues)
                        {
                            SPFieldLookupValueCollection vals = new SPFieldLookupValueCollection(hFieldValue.Value);

                            Utils.LogManager.write("Return vals : " + (vals != null ? vals.ToString() : "Null"));
                            return vals;
                        }
                        else
                        {
                            SPFieldLookupValue val = new SPFieldLookupValue(hFieldValue.Value);
                            Utils.LogManager.write("Return val : " + (val != null ? val.ToString() : "Null"));
                            return val;
                        }
                    }
                    catch (Exception ex)
                    {
                        Utils.LogManager.write("Exception Occurs in Value Get Property. \r\nError Message: " + ex.Message + "\r\nStack Trace: " + ex.StackTrace, "error");
                        ShowErrorMessage(ex.Message);
                        Utils.LogManager.write("Return Null");
                        return null;
                    }
                }
            }
            set
            {
                using (new EnterExitLogger("CCSCascadedLookupControl:Value Set Property"))
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
        }

        #endregion

        #region Filling Data in Control

        void PopulatingValue()
        {
            using (new EnterExitLogger("CCSCascadedLookupControl:PopulatingValue function"))
            {
                CCSCascadedLookupField field = base.Field as CCSCascadedLookupField;

                if (field != null)
                {
                    List<ListItem> poplateItemsList = new List<ListItem>();

                    if (field.LinkToParent)
                    {
                        object parentValue = GetParentFieldValue(field);

                        Utils.LogManager.write("Function PopulatingValue : Field : " + field.Title + ", Link to Parent : true");
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
                        Utils.LogManager.write("Function PopulatingValue : Field : " + field.Title + ", Link to Parent : false");
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
        }

        #endregion

        #region Create Control

        private void ShowControls(CCSCascadedLookupField field)
        {
            using (new EnterExitLogger("CCSCascadedLookupControl:ShowControls function"))
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

                ErrorText = (Literal)TemplateContainer.FindControl("ErrorText");

                Utils.LogManager.write("Initilized All Controls");
                ErrorText.Visible = false;

                if (field.AllowMultipleValues)
                {
                    Utils.LogManager.write("Field: " + field.Title + ", Allow Multiple Values: true");
                    SingleValuePanel.Visible = false;
                    MultipleValuePanel.Visible = true;

                    lbLeftBox.Multiple = true;
                    lbRightBox.Multiple = true;

                    Utils.LogManager.write("Adding Events on controls");
                    
                    lbLeftBox.Attributes.Add("ondblclick", "Listbox_MoveAcross('" + lbLeftBox.ClientID + "','" + lbRightBox.ClientID + "'); SetValueFromListBox('" + hFieldValue.ClientID + "','" + lbRightBox.ClientID + "'); UpdateMyChildControls('" + lbLeftBox.ClientID + ";#" + lbRightBox.ClientID + "');");
                    lbRightBox.Attributes.Add("ondblclick", "Listbox_MoveAcross('" + lbRightBox.ClientID + "','" + lbLeftBox.ClientID + "'); SetValueFromListBox('" + hFieldValue.ClientID + "','" + lbRightBox.ClientID + "'); UpdateMyChildControls('" + lbLeftBox.ClientID + ";#" + lbRightBox.ClientID + "');");

                    btnAdd.Attributes.Add("onclick", "Listbox_MoveAcross('" + lbLeftBox.ClientID + "','" + lbRightBox.ClientID + "'); SetValueFromListBox('" + hFieldValue.ClientID + "','" + lbRightBox.ClientID + "'); UpdateMyChildControls('" + lbLeftBox.ClientID + ";#" + lbRightBox.ClientID + "'); return false;");
                    btnRemove.Attributes.Add("onclick", "Listbox_MoveAcross('" + lbRightBox.ClientID + "','" + lbLeftBox.ClientID + "'); SetValueFromListBox('" + hFieldValue.ClientID + "','" + lbRightBox.ClientID + "'); UpdateMyChildControls('" + lbLeftBox.ClientID + ";#" + lbRightBox.ClientID + "'); return false;");

                }
                else
                {
                    Utils.LogManager.write("Field: " + field.Title + ", Allow Multiple Values: false");
                    SingleValuePanel.Visible = true;
                    MultipleValuePanel.Visible = false;

                    Utils.LogManager.write("Adding Event on control");
                    ddlCCSCascadeFieldControl.Attributes.Add("onchange", "SetValueFromDropDown('" + hFieldValue.ClientID + "','" + ddlCCSCascadeFieldControl.ClientID + "'); UpdateMyChildControls('" + ddlCCSCascadeFieldControl.ClientID + "');");
                }

                if (field.AllowNewEntry)
                {
                    Utils.LogManager.write("Field: " + field.Title + ", Allow New Entry: true");
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
                        Utils.LogManager.write("Field: " + field.Title + ", Use New Form: true");
                        SPSecurity.RunWithElevatedPrivileges(delegate
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
                            });
                    }
                    else
                    {
                        Utils.LogManager.write("Field: " + field.Title + ", Use New Form: false");
                        lnkNewEntry.OnClientClick = "document.getElementById('" + txtNewEntry.ClientID + "').value = ''; document.getElementById('" + txtNewEntry.ClientID + "').style.display=''; document.getElementById('" + lnkAdd.ClientID + "').style.display=''; document.getElementById('" + lnkCancel.ClientID + "').style.display=''; return false;";

                        lnkAdd.OnClientClick = "createListItem('" + GetControlId(field) + "','" + txtNewEntry.ClientID + "'); document.getElementById('" + txtNewEntry.ClientID + "').value = ''; document.getElementById('" + txtNewEntry.ClientID + "').style.display='none'; document.getElementById('" + lnkAdd.ClientID + "').style.display='none'; document.getElementById('" + lnkCancel.ClientID + "').style.display='none'; return false;";
                        lnkCancel.OnClientClick = "document.getElementById('" + txtNewEntry.ClientID + "').value = ''; document.getElementById('" + txtNewEntry.ClientID + "').style.display='none'; document.getElementById('" + lnkAdd.ClientID + "').style.display='none'; document.getElementById('" + lnkCancel.ClientID + "').style.display='none'; return false;";
                    }
                }
                else
                {
                    Utils.LogManager.write("Field: " + field.Title + ", Allow New Entry: false");
                    NewEntryPanel.Visible = false;
                }
            }
        }

        #endregion

       
        #region private functions
        
        object GetParentFieldValue(CCSCascadedLookupField field)
        {
            using (new EnterExitLogger("CCSCascadedLookupControl:GetParentFieldValue function"))
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
        }

        object GetParentFieldValueStart(CCSCascadedLookupField field)
        {
            using (new EnterExitLogger("CCSCascadedLookupControl:GetParentFieldValueStart function"))
            {
                if (field.LinkToParent)
                {
                    string ParentColumnId = field.GetParentColumnId();
                    if (!string.IsNullOrEmpty(ParentColumnId))
                    {
                        SPFieldLookup fieldParent = SPContext.Current.List.Fields[new Guid(ParentColumnId)] as SPFieldLookup;
                        if (fieldParent.AllowMultipleValues)
                        {
                            Utils.LogManager.write("Parent Field : " + fieldParent.Title + ", Allow Multiple Values: true");
                            SPFieldLookupValueCollection valColl = fieldParent.FieldRenderingControl.ItemFieldValue as SPFieldLookupValueCollection;
                            if (valColl != null && valColl.Count > 0)
                            {
                                Utils.LogManager.write("Return Parent Field : " + fieldParent.Title + ", Value: " + valColl.ToString());
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

                                    Utils.LogManager.write("Return Parent Field : " + fieldParent.Title + ", Value: " + valColl.ToString());
                                    return valColl;
                                }
                            }
                        }
                        else
                        {
                            Utils.LogManager.write("Parent Field : " + fieldParent.Title + ", Allow Multiple Values: false");
                            
                            if (fieldParent.FieldRenderingControl.ItemFieldValue != null)
                            {
                                SPFieldLookupValue lookupVal = fieldParent.FieldRenderingControl.ItemFieldValue as SPFieldLookupValue;

                                if (lookupVal != null)
                                {
                                    Utils.LogManager.write("Return Parent Field : " + fieldParent.Title + ", Value: " + lookupVal.ToString());
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
                                            Utils.LogManager.write("Return Parent Field : " + fieldParent.Title + ", Value: " + vals[0]);
                                            return new SPFieldLookupValue(int.Parse(vals[0]), vals[1]);
                                        }
                                        else if (vals.Length == 1)
                                        {
                                            Utils.LogManager.write("Return Parent Field : " + fieldParent.Title + ", Value: " + vals[0]);
                                            return vals[0];
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                Utils.LogManager.write("Return Value: Null");
                return null;
            }
        }

        object GetParentFieldValuePostBack(CCSCascadedLookupField field)
        {
            using (new EnterExitLogger("CCSCascadedLookupControl:GetParentFieldValuePostBack function"))
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
                                        if (((MultipleLookupField)ctrl).FieldName.Equals(fieldParent.InternalName, StringComparison.InvariantCultureIgnoreCase))
                                        {
                                            Utils.LogManager.write("Return Parent Field : " + fieldParent.Title + ", MultipleLookupField Value: " + (((MultipleLookupField)ctrl).Value != null ? ((MultipleLookupField)ctrl).Value : "NULL"));
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
                                        if (((CCSCascadedLookupControl)ctrl).FieldName.Equals(fieldParent.InternalName, StringComparison.InvariantCultureIgnoreCase))
                                        {
                                            Utils.LogManager.write("Return Parent Field : " + fieldParent.Title + ", CCSCascadedLookupControl Value: " + (((CCSCascadedLookupControl)ctrl).Value != null ? ((CCSCascadedLookupControl)ctrl).Value : "NULL"));
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
                                        if (((LookupField)ctrl).FieldName.Equals(fieldParent.InternalName, StringComparison.InvariantCultureIgnoreCase))
                                        {
                                            Utils.LogManager.write("Return Parent Field : " + fieldParent.Title + ", LookupField Value: " + (((LookupField)ctrl).Value != null ? ((LookupField)ctrl).Value : "NULL"));
                                            return ((LookupField)ctrl).Value;
                                        }
                                    }
                                }

                            }
                        }
                    }
                }

                Utils.LogManager.write("Return Value: Null");
                return null;
            }
        }

        void AddClientsideEvents()
        {
            using (new EnterExitLogger("CCSCascadedLookupControl:AddClientsideEvents function"))
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
                                        if (((MultipleLookupField)ctrl).FieldName.Equals(fieldParent.InternalName, StringComparison.InvariantCultureIgnoreCase))
                                        {
                                            Utils.LogManager.write("Get the Parent Control Type is MultipleLookupField");
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
                                            if (((LookupField)ctrl).FieldName.Equals(fieldParent.InternalName, StringComparison.InvariantCultureIgnoreCase))
                                            {
                                                Utils.LogManager.write("Get the Parent Control Type is LookupField");
                                                parentControl = ctrl;
                                            }
                                        }
                                    }
                                }

                                if (parentControl != null)
                                {
                                    if (fieldParent.AllowMultipleValues)
                                    {
                                        Utils.LogManager.write("Parent Control Type is MultipleLookupField");
                                        List<Control> childParentControls = new List<Control>();

                                        Utilities.FindControlRecursive(parentControl, typeof(SPHtmlSelect), ref childParentControls);
                                        Utilities.FindControlRecursive(parentControl, typeof(System.Web.UI.HtmlControls.HtmlButton), ref childParentControls);
                                        if (childParentControls != null && childParentControls.Count > 0)
                                        {
                                            Utils.LogManager.write("Added event to Parent Control Listbox");
                                            RegisterJavaScriptOnMultipleLookupControl(fieldParent.Id.ToString("n"), childParentControls[0].ClientID, childParentControls[1].ClientID, childParentControls[2].ClientID, childParentControls[3].ClientID, "UpdateMyChildControls('" + childParentControls[0].ClientID + ";#" + childParentControls[1].ClientID + "');");

                                            Utils.LogManager.write("AddCascadedControlScript Parent Control is Listbox");
                                            RegisterAddCascadedControlScript(field, childParentControls[0].ClientID + ";#" + childParentControls[1].ClientID, "2");
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        Utils.LogManager.write("Parent Control Type is LookupField"); 
                                        List<Control> childParentControls = new List<Control>();

                                        Utilities.FindControlRecursive(parentControl, typeof(DropDownList), ref childParentControls);
                                        if (childParentControls != null && childParentControls.Count > 0)
                                        {
                                            foreach (Control ctrl in childParentControls)
                                            {
                                                Utils.LogManager.write("Added event to Parent Control dropdown");
                                                ((DropDownList)ctrl).Attributes.Add("onchange", "UpdateMyChildControls('" + ctrl.ClientID + "');");

                                                Utils.LogManager.write("AddCascadedControlScript Parent Control is dropdown");
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
                                                Utils.LogManager.write("Added event to Parent Control autoComplet");
                                                Page.ClientScript.RegisterStartupScript(new object().GetType(), "101", "var parentFocus" + childParentControls[0].ClientID + " = '0';", true);

                                                ((TextBox)childParentControls[0]).Attributes.Add("onblur", "parentFocus" + childParentControls[0].ClientID + " = '0'; UpdateMyChildControls('" + childParentControls[0].ClientID + "');");
                                                ((TextBox)childParentControls[0]).Attributes.Add("onfocus", "parentFocus" + childParentControls[0].ClientID + " = '1'");
                                                ((TextBox)childParentControls[0]).Attributes.Add("onpropertychange", "if(parentFocus" + childParentControls[0].ClientID + " != '1') { UpdateMyChildControls('" + childParentControls[0].ClientID + "'); }");

                                                Utils.LogManager.write("AddCascadedControlScript Parent Control is autoComplet");
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
                                            if (((CCSCascadedLookupControl)ctrl).FieldName.Equals(fieldParent.InternalName, StringComparison.InvariantCultureIgnoreCase))
                                            {
                                                Utils.LogManager.write("Get the Parent Control Type is CCSCascadedLookupControl");
                                                parentControl = ctrl;
                                            }
                                        }
                                    }
                                }

                                if (parentControl != null)
                                {
                                    Utils.LogManager.write("Parent Control Type is CCSCascadedLookupControl"); 
                                    if (fieldParent.AllowMultipleValues)
                                    {
                                        List<Control> childParentControls = new List<Control>();

                                        Utilities.FindControlRecursive(parentControl, typeof(HtmlSelect), ref childParentControls);
                                        if (childParentControls != null && childParentControls.Count == 3)
                                        {
                                            Utils.LogManager.write("AddCascadedControlScript Parent Control is MultiSelect Control"); 
                                            RegisterAddCascadedControlScript(field, childParentControls[1].ClientID + ";#" + childParentControls[2].ClientID, "2");
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        List<Control> childParentControls = new List<Control>();

                                        Utilities.FindControlRecursive(parentControl, typeof(HtmlSelect), ref childParentControls);
                                        if (childParentControls != null && childParentControls.Count > 0)
                                        {
                                            Utils.LogManager.write("AddCascadedControlScript Parent Control is DropDown"); 
                                            RegisterAddCascadedControlScript(field, childParentControls[0].ClientID, "0");
                                            return;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    Utils.LogManager.write("AddCascadedControlScript Parent Control Null");
                    RegisterAddCascadedControlScript(field, "", "-1");
                }
            }
        }

        #endregion

        #region Register Javascript Event on Parent MultipleLookup field Control

        void RegisterJavaScriptOnMultipleLookupControl(string ParentFieldId, string leftBox, string rightBox, string addbutton, string removeButton, string ValueString)
        {
            using (new EnterExitLogger("CCSCascadedLookupControl:RegisterJavaScriptOnMultipleLookupControl function"))
            {
                Utils.LogManager.write("IsStartupScriptRegistered: Parent" + ParentFieldId);
                if (!Page.ClientScript.IsStartupScriptRegistered("Parent" + ParentFieldId))
                {
                    System.Text.StringBuilder sb1 = new System.Text.StringBuilder();

                    Utils.LogManager.write("javascript Function Name: LoadMethod" + ParentFieldId + "()");
                    sb1.Append(@"function LoadMethod" + ParentFieldId + "()");
                    sb1.Append(@"{");
                    //sb1.Append(@"alert(document.getElementById('" + leftBox + "').getAttribute(\"ondblclick\"));");

                    //leftBox
                    Utils.LogManager.write("Adding event on LeftBox");
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
                    Utils.LogManager.write("Adding event on rightbox");
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
                    Utils.LogManager.write("Adding event on addbutton");
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
                    Utils.LogManager.write("Adding event on removeButton");
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

                    Utils.LogManager.write("Register the the client Script block");
                    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Parent" + ParentFieldId, sb1.ToString(), true);

                    if (!Page.ClientScript.IsStartupScriptRegistered("100"))
                    {
                        Utils.LogManager.write("Register the the function on body load");
                        Page.ClientScript.RegisterStartupScript(new object().GetType(), "100", " if (true) {_spBodyOnLoadFunctionNames.push('LoadMethod" + ParentFieldId + "');}", true);
                    }
                }
            }
        }

        #endregion

        void RegisterAddCascadedControlScript(CCSCascadedLookupField field, string parentControlId, string ParentControlType)
        {
            using (new EnterExitLogger("CCSCascadedLookupControl:RegisterAddCascadedControlScript function"))
            {
                string webUrl = "";
                string LookuplistName = "";
                string ParentLinkedFieldName = "";
                string LookupFieldName = "";
                string ViewWhereString = "";
                string ViewOrderString = "";

                string controlId = GetControlId(field);

                Utilities.GetParametersValue(field, out webUrl, out LookuplistName, out ParentLinkedFieldName, out LookupFieldName, out ViewWhereString, out ViewOrderString);

                Utils.LogManager.write("RegisterAddCascadedControlScript: new AddCascadedControl('" + controlId + "', " + field.AllowMultipleValues.ToString().ToLower() + ", " + field.ShowAllOnEmpty.ToString().ToLower() + ", '" + hFieldValue.ClientID + "','" + parentControlId + "', " + ParentControlType + ", '" + webUrl + "', '" + LookuplistName + "', '" + ParentLinkedFieldName + "', '" + LookupFieldName + "', '" + ViewWhereString + "', '" + ViewOrderString + "');");
                        
                //function AddCascadedControl(controlId, isAllowMultiple, allValuesOnEmpty, parentControlId, parentControlType, webUrl, lookupListName, linkedParentfield, lookupTargetField, viewWhereString, viewOrderString) {
                string AddCascadeControlScript = "new AddCascadedControl('" + controlId + "', " + field.AllowMultipleValues.ToString().ToLower() + ", " + field.ShowAllOnEmpty.ToString().ToLower() + ", '" + hFieldValue.ClientID + "','" + parentControlId + "', " + ParentControlType + ", '" + webUrl + "', '" + LookuplistName + "', '" + ParentLinkedFieldName + "', '" + LookupFieldName + "', '" + ViewWhereString + "', '" + ViewOrderString + "');";

                Utils.LogManager.write("Register the script");
                Page.ClientScript.RegisterStartupScript(new object().GetType(), "ChildControl" + this.Field.Id.ToString("n"), "if(true){ " + AddCascadeControlScript + "}", true);
            }

        }

        string GetControlId(CCSCascadedLookupField field)
        {
            using (new EnterExitLogger("CCSCascadedLookupControl:GetControlId function"))
            {
                if (field.AllowMultipleValues)
                {
                    Utils.LogManager.write("Field: " + field.Title + ", ControlId: " + lbLeftBox.ClientID + ";#" + lbRightBox.ClientID); 
                    return lbLeftBox.ClientID + ";#" + lbRightBox.ClientID;
                }
                else
                {
                    Utils.LogManager.write("Field: " + field.Title + ", ControlId: " + ddlCCSCascadeFieldControl.ClientID);
                    return ddlCCSCascadeFieldControl.ClientID;
                }
            }
        }

        void SetControlsValue(object ControlValue)
        {
            using (new EnterExitLogger("CCSCascadedLookupControl:SetControlsValue function"))
            {
                CCSCascadedLookupField field = base.Field as CCSCascadedLookupField;

                if (ControlValue != null)
                {
                    Utils.LogManager.write("Field: " + field.Title + ", Controls Value : " + ControlValue.ToString());
                    
                    if (field != null)
                    {
                        if (field.AllowMultipleValues)
                        {
                            SPFieldLookupValueCollection vals = ControlValue as SPFieldLookupValueCollection;
                            if (vals != null && lbLeftBox.Items != null && lbLeftBox.Items.Count > 0)
                            {
                                foreach (SPFieldLookupValue val in vals)
                                {
                                    ListItem li = lbLeftBox.Items.FindByValue(val.LookupId.ToString());
                                    if (li != null)
                                    {
                                        Utils.LogManager.write("Selected values: " + li.Text); 
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
                                    if (ddlCCSCascadeFieldControl.Items[i].Value.Equals(singleValue.LookupId.ToString(), StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        Utils.LogManager.write("Selected index : " + i.ToString());
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

        void ShowErrorMessage(string errorMessage)
        {
            using (new EnterExitLogger("CCSCascadedLookupControl:ShowErrorMessage function"))
            {
                Utils.LogManager.write("ErrorMessage : " + errorMessage);
                ErrorText = ErrorText != null ? ErrorText : (Literal)TemplateContainer.FindControl("ErrorText");
                SingleValuePanel = SingleValuePanel != null ? SingleValuePanel : (Panel)TemplateContainer.FindControl("SingleValuePanel");
                MultipleValuePanel = MultipleValuePanel != null ? MultipleValuePanel : (Panel)TemplateContainer.FindControl("MultipleValuePanel");
                NewEntryPanel = NewEntryPanel != null ? NewEntryPanel : (Panel)TemplateContainer.FindControl("NewEntryPanel");

                if (ErrorText != null)
                {
                    ErrorText.Text = "<font color=\"red\">Error: " + errorMessage + "</font>";

                    ErrorText.Visible = true;
                    SingleValuePanel.Visible = false;
                    MultipleValuePanel.Visible = false;
                    NewEntryPanel.Visible = false;
                }
            }
        }
    }
}