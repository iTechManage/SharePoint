using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Web.UI.WebControls;
using Microsoft.SharePoint.Utilities;
using System.Collections.Generic;
using System.Web.UI;
using System.Xml;
using System.Globalization;
using System.Linq.Expressions;

namespace CCSAdvancedAlerts.Layouts.CCSAdvancedAlerts
{


    public partial class AdvancedAlertSettings : LayoutsPageBase
    {
        private const string alertSettingsListName = "CCSAdvancedAlertsList";
        private SPList list = null;
        private bool resetControls;

        private AlertManager alertMngr;
        internal AlertManager AlertMngr
        {
            get
            {
                try
                {
                    if (this.alertMngr == null)
                    {
                        this.alertMngr = new AlertManager(SPContext.Current.Site.Url);
                    }
                }
                catch
                {
                    //Error occured while creating Alert manager
                }
                return alertMngr;
            }
        }

        private MailTemplateManager mtManager;
        internal MailTemplateManager MTManager
        {
            get
            {
                try
                {
                    if (this.mtManager == null)
                    {
                        this.mtManager = new MailTemplateManager(SPContext.Current.Site.Url);
                    }
                }
                catch
                {
                    //Error occured while creating mail template manager
                }
                return mtManager;
            }
        }

        public SPList List
        {
            get
            {
                if (this.list == null)
                {
                    if ((this.WebID == Guid.Empty) || (this.ListID == Guid.Empty))
                    {
                        return null;
                    }
                    if (this.WebID == SPContext.Current.Web.ID)
                    {
                        this.list = SPContext.Current.Web.Lists[this.ListID];
                    }
                    else
                    {
                        using (SPWeb web = SPContext.Current.Site.OpenWeb(this.WebID))
                        {
                            this.list = web.Lists[this.ListID];
                        }
                    }
                }
                return this.list;
            }
            set
            {
                this.list = value;
                if (this.list != null)
                {
                    this.WebID = this.list.ParentWeb.ID;
                    this.ListID = this.list.ID;
                }
                else
                {
                    this.WebID = Guid.Empty;
                    this.ListID = Guid.Empty;
                }
                this.resetControls = true;
                this.Conditions = null;
            }
        }

        private Guid ListID
        {
            get
            {
                if (this.ViewState["ListID"] == null)
                {
                    return Guid.Empty;
                }
                return (Guid)this.ViewState["ListID"];
            }
            set
            {
                this.ViewState["ListID"] = value;
            }
        }

        private Guid WebID
        {
            get
            {
                if (this.ViewState["WebID"] == null)
                {
                    return Guid.Empty;
                }
                return (Guid)this.ViewState["WebID"];
            }
            set
            {
                this.ViewState["WebID"] = value;
            }
        }

        protected override void CreateChildControls()
        {
            if (this.List != null)
            {
                base.CreateChildControls();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                PopulateSites();
                populateStaticDropDowns();
                FillddlUserID();
                PopulateTemplates();


                InitializeSendTimeSelectors();
                InitializeHours();
                FillHours();
                setDefaultValues();

            }

            this.btnCopyToClipBoard.OnClientClick = "CopyToClipboard(" + this.lstPlaceHolders.ClientID + ")";

            //Alert based events
            this.btnAlertsave.Click += new EventHandler(btnAlertsave_Click);
            this.ddlSite.SelectedIndexChanged += new EventHandler(ddlSite_SelectedIndexChanged);
            this.ddlList.SelectedIndexChanged += new EventHandler(ddlList_SelectedIndexChanged);

            //Recipient related
            this.btnAddTO.Click += new EventHandler(btnAddTO_Click);
            this.btnAddCC.Click += new EventHandler(btnAddCC_Click);
            this.btnAddBCC.Click += new EventHandler(btnAddBCC_Click);

            //Template related
            this.btnAddToSubject.Click += new EventHandler(btnAddToSubject_Click);
            //this.btnCopyToClipBoard.Click += new EventHandler(btnCopyToClipBoard_Click);

            this.btnTemplateAdd.Click += new EventHandler(btnTemplateAdd_Click);
            this.btnTemplateUpdate.Click += new EventHandler(btnTemplateUpdate_Click);
            this.btnTemplateCancel.Click += new EventHandler(btnTemplateCancel_Click);


            //Template Related
            this.lnkItemAddedEdit.Click += new EventHandler(lnkItemAddedEdit_Click);
            this.lnkItemAddedDelete.Click += new EventHandler(lnkItemAddedDelete_Click);

            this.lnkItemUpdateEdit.Click += new EventHandler(lnkItemUpdateEdit_Click);
            this.lnkItemUpdateDelete.Click += new EventHandler(lnkItemUpdateDelete_Click);

            this.lnkItemDeleteEdit.Click += new EventHandler(lnkItemDeleteEdit_Click);
            this.lnkItemDeleteDelete.Click += new EventHandler(lnkItemDeleteDelete_Click);

            this.linkDateTimeEdit.Click += new EventHandler(linkDateTimeEdit_Click);
            this.linkDateTimeDelete.Click += new EventHandler(linkDateTimeDelete_Click);


            //AlertType
            this.rdImmediately.CheckedChanged += new EventHandler(rdImmediately_CheckedChanged);
            this.rdImmediateBusinessdays.CheckedChanged += new EventHandler(rdImmediateBusinessdays_CheckedChanged);
            this.rdDaily.CheckedChanged += new EventHandler(rdDaily_CheckedChanged);

            //Navigate Back
            this.btnOK.Click += new EventHandler(btnOK_Click);
            this.btnAlertcancel.Click += new EventHandler(btnAlertcancel_Click);

            this.btnUpdateAlert.Click += new EventHandler(btnUpdateAlert_Click);
            this.btnTemplateUpdate.Click += new EventHandler(btnTemplateUpdate_Click);
            this.ddlUserID.SelectedIndexChanged+=new EventHandler(ddlUserID_SelectedIndexChanged);
            this.ConditionCase1.CheckedChanged += new EventHandler(ConditionCase1_CheckedChanged);
            this.ConditionCase2.CheckedChanged += new EventHandler(ConditionCase2_CheckedChanged);
            this.rdWeekly.CheckedChanged += new EventHandler(rdWeekly_CheckedChanged);
            this.rdImmediateAlways.CheckedChanged += new EventHandler(rdImmediateAlways_CheckedChanged);

        }

        void rdImmediateAlways_CheckedChanged(object sender, EventArgs e)
        {
            sendAsSingleMessage.Checked = false;
            sendAsSingleMessage.Enabled = rdImmediateBusinessdays.Checked;
            ddlAlertWeekday.Enabled = false;
            ddlAlertTime.Enabled = false;
        }

        void rdWeekly_CheckedChanged(object sender, EventArgs e)
        {
            rdImmediateAlways.Checked = false;
            rdImmediateBusinessdays.Checked = false;
            sendAsSingleMessage.Enabled = true;
            sendAsSingleMessage.Checked = false;
            ddlAlertWeekday.Enabled = true;
            ddlAlertTime.Enabled = true;
        }

        void ConditionCase2_CheckedChanged(object sender, EventArgs e)
        {
            UpdatePanel1.Visible = false;
            ConditionCase1.Checked = false;
            ConditionEditor.Visible = ConditionCase2.Checked;      
        }

        void ConditionCase1_CheckedChanged(object sender, EventArgs e)
        {
            ConditionEditor.Visible = false;
            ConditionCase2.Checked = false;
            UpdatePanel1.Visible = ConditionCase1.Checked;
        }

        #region OnStartUp

        void populateStaticDropDowns()
        {
            try
            {
                ddlPeriodType.Items.Clear();
                ddlPeriodType.Items.Add(new ListItem(PeriodType.Minutes.ToString(), PeriodType.Minutes.ToString()));
                ddlPeriodType.Items.Add(new ListItem(PeriodType.Hours.ToString(), PeriodType.Hours.ToString()));
                ddlPeriodType.Items.Add(new ListItem(PeriodType.Days.ToString(), PeriodType.Days.ToString()));
                ddlPeriodType.Items.Add(new ListItem(PeriodType.Weeks.ToString(), PeriodType.Weeks.ToString()));
                ddlPeriodType.Items.Add(new ListItem(PeriodType.Months.ToString(), PeriodType.Months.ToString()));
                ddlPeriodType.Items.Add(new ListItem(PeriodType.Years.ToString(), PeriodType.Years.ToString()));

                ddlRepeatType.Items.Clear();
                ddlRepeatType.Items.Add(new ListItem(PeriodType.Minutes.ToString(), PeriodType.Minutes.ToString()));
                ddlRepeatType.Items.Add(new ListItem(PeriodType.Hours.ToString(), PeriodType.Hours.ToString()));
                ddlRepeatType.Items.Add(new ListItem(PeriodType.Days.ToString(), PeriodType.Days.ToString()));
                ddlRepeatType.Items.Add(new ListItem(PeriodType.Weeks.ToString(), PeriodType.Weeks.ToString()));
                ddlRepeatType.Items.Add(new ListItem(PeriodType.Months.ToString(), PeriodType.Months.ToString()));
                ddlRepeatType.Items.Add(new ListItem(PeriodType.Years.ToString(), PeriodType.Years.ToString()));

                ddlPeriodPosition.Items.Clear();
                ddlPeriodPosition.Items.Add(new ListItem(PeriodPosition.After.ToString(), PeriodPosition.After.ToString()));
                ddlPeriodPosition.Items.Add(new ListItem(PeriodPosition.Before.ToString(), PeriodPosition.Before.ToString()));



            }
            catch { }
        }


        #endregion

        #region Alerts Grid View

        protected void FillddlUserID()
        {
            SPUser currentUser = SPContext.Current.Web.CurrentUser;
            this.ddlUserID.Items.Add(new ListItem(currentUser.Name, currentUser.ID.ToString()));
            if (currentUser.IsSiteAdmin)
            {
                Dictionary<string, string> allAlerOwners = AlertMngr.GetAlertOwners();
                foreach (string key in allAlerOwners.Keys)
                {
                    if (key != currentUser.ID.ToString())
                    {
                        this.ddlUserID.Items.Add(new ListItem(allAlerOwners[key],key));
                    }
                }
            }
        }

        protected void ddlUserID_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                this.gvAlerts.SelectedIndex = -1;
                this.gvAlerts.DataBind();
                PopulateTemplates();
            }
            catch
            {
                //Error ocurred getting elerts for the user
            }
        }

        protected void gvAlerts_PageIndexChanging(object sender, EventArgs e)
        {
            try
            {
                this.gvAlerts.SelectedIndex = -1;
                this.gvAlerts.DataBind();
            }
            catch
            { }
        }

        protected void gvAlerts_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                e.Cancel = true;
                int alertId = Convert.ToInt32(this.gvAlerts.DataKeys[e.RowIndex][0]);
                this.AlertMngr.DeleteAlerts(alertId.ToString(), MTManager);
                this.dsAlerts.DataBind();
                this.gvAlerts.DataBind();
            }
            catch
            {
            }
        }

        protected void gvAlerts_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {

                //Edit the existing alert
                this.btnUpdateAlert.Visible = true;
                int alertID = Convert.ToInt32(this.gvAlerts.DataKeys[this.gvAlerts.SelectedIndex][0]);
                this.FillAlert(Convert.ToString(alertID));

            }
            catch { }
        }

        #endregion

        #region Condition Grid View

        void btnOK_Click(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            this.GoBack();
        }

        internal List<Condition> Conditions
        {
            get
            {
                return (this.ViewState["Conditions"] as List<Condition>);
            }
            set
            {
                this.ViewState["Conditions"] = value;
                this.gvConditions.DataSource = value;
                this.gvConditions.DataBind();
                this.EnsureConditionInsertRow();
            }
        }

        internal string CustomEvaluationData
        {
            get
            {
                // TODO, get data from UI
                string text = this.ConditionEditor.InnerText;
                return text;
            }
            
            set
            {
                // TODO: set data in UI
                this.ConditionEditor.InnerText = value;
            }
        }

        protected string GetFieldName(string internalName)
        {
            if (this.List.Fields.ContainsField(internalName))
            {
                return this.List.Fields.GetFieldByInternalName(internalName).Title;
            }
            return "???";
        }

        private void EnsureConditionInsertRow()
        {
            List<Condition> dataSource = this.gvConditions.DataSource as List<Condition>;
            if (((dataSource == null) || (dataSource.Count == 0)) || (this.gvConditions.FooterRow == null))
            {
                this.EnsureConditionInsertRow(this.gvConditions.Controls[0].Controls[0]);
            }
            else
            {
                this.EnsureConditionInsertRow(this.gvConditions.FooterRow);
            }
        }

        private void EnsureConditionInsertRow(Control parenControl)
        {
            DropDownList ddlField = parenControl.FindControl("ddlConditionField") as DropDownList;
            DropDownList ddlWhenToCompareValue = parenControl.FindControl("ddlConditionCompareType") as DropDownList;
            DropDownList ddlOperator = parenControl.FindControl("ddlConditionOperator") as DropDownList;
            TextBox txtValue = parenControl.FindControl("txtConditionFieldValue") as TextBox;
            if (ddlOperator != null)
            {
                if (ddlOperator.Items.Count == 0)
                {
                    this.FillConditionField(ddlField, ddlOperator, txtValue);
                    this.FillOperatorField(ddlOperator);
                    this.FillWhenToCompareValue(ddlWhenToCompareValue);
                }
                else if (this.resetControls)
                {
                    this.FillConditionField(ddlField, ddlOperator, txtValue);
                }
            }
        }
    

        private void FillConditionField(DropDownList ddlField, DropDownList ddlOperator, TextBox txtValue)
        {
            ddlField.Items.Clear();
            if (this.list == null)
            {
                if (SPContext.Current.Site.RootWeb.ID.ToString() == this.ddlSite.SelectedValue)
                {
                    this.list = SPContext.Current.Site.RootWeb.Lists[new Guid(ddlList.SelectedValue)];
                }
                else
                {
                    //this.list = SPContext.Current.Site.AllWebs[new Guid(this.ddlSite.SelectedValue)].Lists[new Guid(ddlList.SelectedValue)];
                    this.list = SPContext.Current.Site.RootWeb.GetSubwebsForCurrentUser()[new Guid(this.ddlSite.SelectedValue)].Lists[new Guid(ddlList.SelectedValue)];
                }
            }

            if (this.list != null)
            {
                foreach (SPField field in this.list.Fields)
                {
                    if (field != null && !field.Hidden)
                    {
                        ListItem newFieldItem = new ListItem(field.Title, field.InternalName);
                        if (!ddlField.Items.Contains(newFieldItem) && ddlField.Items.FindByText(field.Title) == null)
                        {
                            ddlField.Items.Add(newFieldItem);
                        }
                    }
                }
            }
        }

        private void FillOperatorField(DropDownList ddlOperator)
        {
            ddlOperator.Items.Clear();
            ddlOperator.Items.Add(new ListItem("Equals", Operators.Eq.ToString()));
            ddlOperator.Items.Add(new ListItem("Not equals", Operators.Neq.ToString()));
            ddlOperator.Items.Add(new ListItem("Contains", Operators.Contains.ToString()));
            ddlOperator.Items.Add(new ListItem("Not contains", Operators.NotContains.ToString()));
            ddlOperator.Items.Add(new ListItem("Greater than", Operators.Gt.ToString()));
            ddlOperator.Items.Add(new ListItem("Greater than or equals", Operators.Geq.ToString()));
            ddlOperator.Items.Add(new ListItem("Less than", Operators.Lt.ToString()));
            ddlOperator.Items.Add(new ListItem("Less than or equals", Operators.Leq.ToString()));
            ddlOperator.Items.Add(new ListItem("Yes", Operators.Yes.ToString()));
            ddlOperator.Items.Add(new ListItem("No", Operators.No.ToString()));
        }

        private void FillWhenToCompareValue(DropDownList ddlWhenToCompare)
        {
            ddlWhenToCompare.ClearSelection();
            ddlWhenToCompare.Items.Add(new ListItem("Always", ConditionComparisionType.Always.ToString()));
            ddlWhenToCompare.Items.Add(new ListItem("After change", ConditionComparisionType.AfterChange.ToString()));
        }

        private void GoBack()
        {
            if (Context.Request["Source"] != null)
            {
                SPUtility.Redirect(Convert.ToString(Context.Request["Source"]), SPRedirectFlags.UseSource, Context);
            }
            else
            {
                string siteURL = SPContext.Current.Web.Site.Url;
                string serverRelativeURL = SPContext.Current.Web.Site.ServerRelativeUrl;
                string formURL = "";
                try
                {
                    if (Request.QueryString["Type"] == "edit")
                    {
                        formURL = SPContext.Current.List.Forms[PAGETYPE.PAGE_EDITFORM].ServerRelativeUrl;
                    }
                    else if (Request.QueryString["Type"] == "view")
                    {
                        formURL = SPContext.Current.List.Forms[PAGETYPE.PAGE_DISPLAYFORM].ServerRelativeUrl;
                    }
                    else if (Request.QueryString["Type"] == "RibbonButton" || Request.QueryString["Type"] == "EditControlBlockButton")
                    {
                        this.CloseModelDialog();
                        return;
                    }


                    if (!string.IsNullOrEmpty(serverRelativeURL) &&
                        !string.IsNullOrEmpty(siteURL) &&
                        !string.IsNullOrEmpty(formURL) &&
                        siteURL.EndsWith(serverRelativeURL) &&
                        formURL.StartsWith(serverRelativeURL))
                    {
                        siteURL = siteURL.Substring(0, siteURL.IndexOf(serverRelativeURL));
                    }

                    string url = Request.QueryString["ID"] != null ? siteURL + formURL + "?ID=" + Request.QueryString["ID"] : string.Empty;
                    if (!string.IsNullOrEmpty(url))
                    {
                        SPUtility.Redirect(url, SPRedirectFlags.Default, Context);
                    }
                    else
                    {
                        this.CloseModelDialog();
                        return;
                    }
                }
                catch { }
            }
        }

        private void CloseModelDialog()
        {
            Context.Response.Write("<script type='text/javascript'>window.frameElement.commitPopup();</script>");
            Context.Response.Flush();
            Context.Response.End();
        }

        protected void gvConditions_RowCancelEditing(object sender, GridViewCancelEditEventArgs e)
        {
            try
            {
                this.gvConditions.ShowFooter = true;
                this.gvConditions.EditIndex = -1;
                this.gvConditions.DataSource = this.Conditions;
                this.gvConditions.DataBind();
                this.EnsureConditionInsertRow();
            }
            catch
            {
            }
        }

        protected void gvConditions_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                GridViewRow footerRow = null;
                string commandName = e.CommandName;
                if (commandName == null)
                {
                    return;
                }
                if (commandName != "EmptyDataTemplateInsert")
                {
                    if (commandName != "FooterInsert")
                    {
                        return;
                    }
                }
                else
                {
                    footerRow = this.gvConditions.Controls[0].Controls[0] as GridViewRow;
                }

                if (footerRow == null)
                    footerRow = this.gvConditions.FooterRow;

                if (footerRow != null)
                {
                    DropDownList ddlField = footerRow.FindControl("ddlConditionField") as DropDownList;
                    DropDownList ddlWhen = footerRow.FindControl("ddlConditionCompareType") as DropDownList;
                    DropDownList ddlOperator = footerRow.FindControl("ddlConditionOperator") as DropDownList;
                    TextBox txtValue = footerRow.FindControl("txtConditionFieldValue") as TextBox;
                    if (((ddlField != null) && (ddlOperator != null)) && (txtValue != null))
                    {
                        this.AddUpdateCondition(ddlField, ddlWhen, ddlOperator, txtValue, -1);
                    }
                }
               
            }
            catch (Exception exception)
            {
            }
        }

        protected void gvConditions_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                List<Condition> conditions = this.Conditions;
                if ((conditions != null) && (e.RowIndex < conditions.Count))
                {
                    conditions.RemoveAt(e.RowIndex);
                    this.Conditions = conditions;
                    this.gvConditions_RowCancelEditing(null, null);
                }
            }
            catch
            {
            }
        }

        protected void gvConditions_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                this.gvConditions.ShowFooter = false;
                this.gvConditions.EditIndex = e.NewEditIndex;
                this.gvConditions.DataSource = this.Conditions;
                this.gvConditions.DataBind();
                GridViewRow parenControl = this.gvConditions.Rows[e.NewEditIndex];
                this.EnsureConditionInsertRow(parenControl);
                DropDownList ddlFields = parenControl.FindControl("ddlConditionField") as DropDownList;
                DropDownList ddlWhen = parenControl.FindControl("ddlConditionCompareType") as DropDownList;
                DropDownList ddlOps = parenControl.FindControl("ddlConditionOperator") as DropDownList;
                TextBox txtConditionValue = parenControl.FindControl("txtConditionFieldValue") as TextBox;
                //DropDownList list3 = parenControl.FindControl("ddlWhen") as DropDownList;
                ddlFields.SelectedValue = this.Conditions[e.NewEditIndex].FieldName;
                ddlWhen.SelectedValue = Convert.ToString(this.Conditions[e.NewEditIndex].ComparisionType);
                ddlOps.SelectedValue = Convert.ToString(this.Conditions[e.NewEditIndex].ComparisionOperator);
                txtConditionValue.Text = Convert.ToString(this.Conditions[e.NewEditIndex].StrValue);

                //list3.SelectedValue = this.Conditions[e.NewEditIndex].OnChange ? "AfterChange" : "Always";
            }
            catch
            {
            }
        }

        protected void gvConditions_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                GridViewRow row = this.gvConditions.Rows[this.gvConditions.EditIndex];
                this.AddUpdateCondition(row.FindControl("ddlConditionField") as DropDownList, row.FindControl("ddlConditionCompareType") as DropDownList, row.FindControl("ddlConditionOperator") as DropDownList, row.FindControl("txtConditionFieldValue") as TextBox, this.gvConditions.EditIndex);
                this.gvConditions_RowCancelEditing(sender, null);
            }
            catch
            {
            }
        }
        //protected void Conditions_SelectedIndexChanged(object sender, GridViewUpdateEventArgs e)
        //{
        //    try
        //    {
                
                
        //    }
        //    catch
        //    {
        //    }
        //}

        private void AddUpdateCondition(DropDownList ddlField, DropDownList ddlWhen, DropDownList ddlOperator, TextBox txtValue, int editIndex)
        {
            if (this.Page.IsValid)
            {
                List<Condition> conditions = this.Conditions;
                if (conditions == null)
                {
                    conditions = new List<Condition>();
                }
                Condition condition2 = new Condition();
                condition2.FieldName = ddlField.SelectedValue;
                condition2.ComparisionType = (ConditionComparisionType)Enum.Parse(typeof(ConditionComparisionType), ddlWhen.SelectedValue);
                //condition2.OnChange = ddlWhen.SelectedValue != "Always";
                condition2.ComparisionOperator = (Operators)Enum.Parse(typeof(Operators), ddlOperator.SelectedValue);
                //Creating Dictionary to store field and operator values
                //Dictionary<string, string> condValues = new Dictionary<string, string>();
                //condValues.Add(Convert.ToString(ddlField), Convert.ToString(ddlOperator));
                condition2.StrValue = txtValue.Text;
                Condition item = condition2;
                if ((editIndex == -1) || ((conditions.Count + 1) < editIndex))
                {
                    conditions.Add(item);
                }
                else
                {
                    conditions.Insert(editIndex, item);
                    conditions.RemoveAt(editIndex + 1);
                }
                this.Conditions = conditions;
                this.gvConditions.EditIndex = -1;
                //this.gvConditions.DataSource = this.Conditions;
                //this.gvConditions.DataBind();
                //this.EnsureConditionInsertRow();
            }
        }

        protected string GetValidOperatorValue(object operatorValue)
        {
            string strValue = Convert.ToString(operatorValue);
            return strValue;
        }

        private Dictionary<string, string> GetFieldCriteria(SPField field)
        {
            Dictionary<string, string> criteria = new Dictionary<string, string>();

            switch (field.Type)
            {
                case SPFieldType.Text:
                    criteria.Add("Equals", Operators.Eq.ToString());
                    criteria.Add("Not equals", Operators.Neq.ToString());
                    criteria.Add("Contains", Operators.Contains.ToString());
                    criteria.Add("Not contains", Operators.NotContains.ToString());
                    //criteria.Add(Operators.BeginsWith, Operators.BeginsWith);
                    //criteria.Add(Operators.IsNull, Operators.IsNull);
                    //criteria.Add(Operators.IsNotNull, Operators.IsNotNull);
                    break;
                case SPFieldType.Currency:
                    criteria.Add("Equals", Operators.Eq.ToString());
                    criteria.Add("Not equals", Operators.Neq.ToString());
                    criteria.Add("Greater than", Operators.Gt.ToString());
                    criteria.Add("Greater than or equals", Operators.Geq.ToString());
                    criteria.Add("Less than", Operators.Lt.ToString());
                    criteria.Add("Less than or equals", Operators.Leq.ToString());
                    //criteria.Add(CriteriaTypes.IsNull, CriteriaTypes.IsNull);
                    //criteria.Add(CriteriaTypes.IsNotNull, CriteriaTypes.IsNotNull);
                    break;
                case SPFieldType.DateTime:
                    criteria.Add("Equals", Operators.Eq.ToString());
                    criteria.Add("Not equals", Operators.Neq.ToString());
                    criteria.Add("Greater than", Operators.Gt.ToString());
                    criteria.Add("Greater than or equals", Operators.Geq.ToString());
                    criteria.Add("Less than", Operators.Lt.ToString());
                    criteria.Add("Less than or equals", Operators.Leq.ToString());
                    //criteria.Add(CriteriaTypes.IsNull, CriteriaTypes.IsNull);
                    //criteria.Add(CriteriaTypes.IsNotNull, CriteriaTypes.IsNotNull);
                    break;
                case SPFieldType.Integer:
                    criteria.Add("Equals", Operators.Eq.ToString());
                    criteria.Add("Not equals", Operators.Neq.ToString());
                    criteria.Add("Greater than", Operators.Gt.ToString());
                    criteria.Add("Greater than or equals", Operators.Geq.ToString());
                    criteria.Add("Less than", Operators.Lt.ToString());
                    criteria.Add("Less than or equals", Operators.Leq.ToString());
                    //criteria.Add(CriteriaTypes.IsNull, CriteriaTypes.IsNull);
                    //criteria.Add(CriteriaTypes.IsNotNull, CriteriaTypes.IsNotNull);
                    break;
                case SPFieldType.MultiChoice:
                    criteria.Add("Contains", Operators.Contains.ToString());
                    criteria.Add("Not contains", Operators.NotContains.ToString());
                    criteria.Add("Equals", Operators.Eq.ToString());
                    criteria.Add("Not equals", Operators.Neq.ToString());
                    //criteria.Add(CriteriaTypes.IsNull, CriteriaTypes.IsNull);
                    //criteria.Add(CriteriaTypes.IsNotNull, CriteriaTypes.IsNotNull);
                    break;
                case SPFieldType.Note:
                    criteria.Add("Contains", Operators.Contains.ToString());
                    criteria.Add("Not contains", Operators.NotContains.ToString());
                    criteria.Add("Equals", Operators.Eq.ToString());
                    criteria.Add("Not equals", Operators.Neq.ToString());
                    //criteria.Add(CriteriaTypes.BeginsWith, CriteriaTypes.BeginsWith);
                    //criteria.Add(CriteriaTypes.IsNull, CriteriaTypes.IsNull);
                    //criteria.Add(CriteriaTypes.IsNotNull, CriteriaTypes.IsNotNull);
                    break;
                case SPFieldType.Number:
                    criteria.Add("Equals", Operators.Eq.ToString());
                    criteria.Add("Not equals", Operators.Neq.ToString());
                    criteria.Add("Greater than", Operators.Gt.ToString());
                    criteria.Add("Greater than or equals", Operators.Geq.ToString());
                    criteria.Add("Less than", Operators.Lt.ToString());
                    criteria.Add("Less than or equals", Operators.Leq.ToString());
                    //criteria.Add(CriteriaTypes.IsNull, CriteriaTypes.IsNull);
                    //criteria.Add(CriteriaTypes.IsNotNull, CriteriaTypes.IsNotNull);
                    break;
                case SPFieldType.URL:
                    criteria.Add("Contains", Operators.Contains.ToString());
                    criteria.Add("Equals", Operators.Eq.ToString());
                    criteria.Add("Not equals", Operators.Neq.ToString());
                    //criteria.Add(CriteriaTypes.BeginsWith, CriteriaTypes.BeginsWith);
                    //criteria.Add(CriteriaTypes.IsNull, CriteriaTypes.IsNull);
                    //criteria.Add(CriteriaTypes.IsNotNull, CriteriaTypes.IsNotNull);
                    break;
                default:
                    criteria.Add("Equals", Operators.Eq.ToString());
                    criteria.Add("Not equals", Operators.Neq.ToString());
                    //criteria.Add(CriteriaTypes.IsNull, CriteriaTypes.IsNull);
                    //criteria.Add(CriteriaTypes.IsNotNull, CriteriaTypes.IsNotNull);
                    break;
            }

            return criteria;
        }

        #endregion

        #region Aletr related events

        void btnAddBCC_Click(object sender, EventArgs e)
        {
            AddAddress(txtBcc);
        }

        void btnAddCC_Click(object sender, EventArgs e)
        {
            AddAddress(txtCc);
        }

        void btnAddTO_Click(object sender, EventArgs e)
        {
            AddAddress(txtTo);
        }

        void AddAddress(TextBox txtAddressBox)
        {
            if (txtAddressBox != null)
            {
                string emailAddresses = string.Empty;
                if (rdCurrentUser.Checked)
                {
                    emailAddresses = SPContext.Current.Web.CurrentUser.Email;
                }
                else if (rdUsers.Checked)
                {
                    //string cC = string.Empty, ccEmail = string.Empty;
                    if (additionalUsers != null)
                    {
                        int resolvedEntitiesCount = additionalUsers.ResolvedEntities.Count;
                        if (resolvedEntitiesCount != 0)
                        {
                            for (int i = 0; i < resolvedEntitiesCount; i++)
                            {
                                try
                                {
                                    PickerEntity pEntity = (PickerEntity)additionalUsers.ResolvedEntities[i];
                                    if (pEntity != null &&
                                        !String.IsNullOrEmpty(Convert.ToString(pEntity.EntityData["Email"])))
                                    {

                                        if (!String.IsNullOrEmpty(emailAddresses))
                                        {
                                            emailAddresses = emailAddresses + ",";
                                        }
                                        emailAddresses =
                                            emailAddresses + Convert.ToString(pEntity.EntityData["Email"]);
                                    }
                                }
                                catch { }
                            }
                        }
                    }
                }


                else if (rdUsersincolumn.Checked)
                {
                    emailAddresses = ddlUsersInColumn.SelectedValue;
                }
                else if (rdEmailAddresses.Checked)
                {
                    emailAddresses = txtEmailAddresses.Text;
                }

                if (!string.IsNullOrEmpty(txtAddressBox.Text))
                {
                    txtAddressBox.Text += "," + emailAddresses;
                }
                else
                {
                    txtAddressBox.Text += emailAddresses;
                }
            }
        }

        void btnAlertsave_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtTitle.Text))
                {
                    ErrorMessageLabel2.Text = "• Missing alert title";                  
                }
                else if (!chkItemAdded.Checked && !chkItemUpdated.Checked && !chkItemDeleted.Checked && !chkDateColumn.Checked)
                {
                    ErrorMessageLabel2.Text = "• You must select at least one event type to send alerts";
                }
                else if (string.IsNullOrEmpty(txtTo.Text))
                {
                    ErrorMessageLabel2.Text = "• You must specify at least one recipient";
                }
                else if (string.IsNullOrEmpty(txtFrom.Text))
                {
                    ErrorMessageLabel2.Text = "Please enter the From Address";
                }
                else if (ddlItemAdded.SelectedItem == null || ddlItemUpdate.SelectedItem == null || ddlItemDelete.SelectedItem == null || ddlDateTime.SelectedItem == null)
                {
                    ErrorMessageLabel2.Text = " Please select atleast one template for Alerts";
                }
                else
                {
                PrepareAlert("0");       
                this.gvAlerts.DataBind();
                btnOK_Click(sender, e);
                }
            }
            catch { }
        }

        void btnUpdateAlert_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtTitle.Text))
            {
                ErrorMessageLabel2.Text = "• Missing alert title";
            }
            else if (!chkItemAdded.Checked && !chkItemUpdated.Checked && !chkItemDeleted.Checked && !chkDateColumn.Checked)
            {
                ErrorMessageLabel2.Text = "• You must select at least one event type to send alerts";
            }
            else if (string.IsNullOrEmpty(txtTo.Text))
            {
                ErrorMessageLabel2.Text = "• You must specify at least one recipient";
            }
            else if (string.IsNullOrEmpty(txtFrom.Text))
            {
                ErrorMessageLabel2.Text = "• Please enter the From Address";
            }
            else if (ddlItemAdded.SelectedItem == null || ddlItemUpdate.SelectedItem == null || ddlItemDelete.SelectedItem == null || ddlDateTime.SelectedItem == null)
            {
                ErrorMessageLabel2.Text = "• Please select atleast one template for Alerts";
            }
            this.btnUpdateAlert.Visible = false;
            PrepareAlert(this.hiddenAlertID.Text);
            btnOK_Click(sender, e);
        }

        protected void FillAlert(string alertID)
        {
            //Populate Alert
            this.rdImmediately.Checked = false;
            this.rdDaily.Checked = false;
            this.rdWeekly.Checked = false;
            pnSubImmediately.Visible = false;
            try
            {
                this.hiddenAlertID.Text = alertID;

                Alert alert = AlertMngr.GetAlertFromID(alertID, MTManager);

                //Get the General Information
                txtTitle.Text = alert.Title;
                ddlSite.SelectedValue = alert.WebId;
                PopulateLists(alert.WebId);
                ddlList.SelectedValue = alert.ListId;
                ListChanged();


                //Get Recipient Section
                txtTo.Text = alert.ToAddress;
                txtFrom.Text = alert.FromAdderss;
                txtCc.Text = alert.CcAddress;
                txtBcc.Text = alert.BccAddress;


                //Event Type
                chkItemAdded.Checked = alert.AlertType.Contains(AlertEventType.ItemAdded);
                chkItemDeleted.Checked = alert.AlertType.Contains(AlertEventType.ItemDeleted);
                chkItemUpdated.Checked = alert.AlertType.Contains(AlertEventType.ItemUpdated);
                chkDateColumn.Checked = alert.AlertType.Contains(AlertEventType.DateColumn);

                //------------------------------------------------------------------
                //this.BlockedUsers = ;
                if (this.ddlDateColumn.Items.FindByText(alert.DateColumnName) != null)
                {
                    this.ddlDateColumn.SelectedIndex = this.ddlDateColumn.Items.IndexOf(this.ddlDateColumn.Items.FindByText(alert.DateColumnName));
                }
                ddlPeriodType.SelectedValue = Convert.ToString(alert.PeriodType);
                this.ddlPeriodPosition.SelectedValue = Convert.ToString(alert.PeriodPosition);
                chkRepeat.Checked = alert.Repeat;
                ddlRepeatType.SelectedValue = Convert.ToString(alert.RepeatType);

                //when To Send
                rdDaily.Checked = (alert.SendType == SendType.Daily);
                rdImmediateAlways.Checked = (alert.SendType == SendType.ImmediateAlways);
                rdImmediateBusinessdays.Checked = (alert.SendType == SendType.ImmediateBusinessDays);
                rdWeekly.Checked = (alert.SendType == SendType.Weekly);

                if (alert.SendType==SendType.ImmediateAlways)
                {
                    rdImmediately.Checked = true;
                    pnSubImmediately.Visible = true;
                    rdImmediateAlways.Checked = true;
                    //rdImmediateBusinessdays.Checked = !rdImmediateAlways.Checked;
                    //pnImmediateBusinessDays.Visible = rdImmediateBusinessdays.Checked;
                }
                else if (alert.ImmediateBusinessDays!=null)
                {
                    sendAsSingleMessage.Enabled = true;
                    rdImmediately.Checked = true;
                    rdImmediateBusinessdays.Checked = true;
                    pnSubImmediately.Visible = true;
                    pnImmediateBusinessDays.Visible = rdImmediateBusinessdays.Checked;
                    ddlImmediateBusinessStartTime.SelectedValue = Convert.ToString(alert.BusinessStartHour);
                    ddlImmediateBusinessEndTime.SelectedValue = Convert.ToString(alert.BusinessendtHour);
                    chkImmediateSun.Checked = alert.ImmediateBusinessDays.Contains(WeekDays.Sunday);
                    chkImmediateMon.Checked = alert.ImmediateBusinessDays.Contains(WeekDays.Monday);
                    chkImmediateTue.Checked = alert.ImmediateBusinessDays.Contains(WeekDays.Tuesday);
                    chkImmediateWed.Checked = alert.ImmediateBusinessDays.Contains(WeekDays.Wednesday);
                    chkImmediateThu.Checked = alert.ImmediateBusinessDays.Contains(WeekDays.Thursday);
                    chkImmediateFri.Checked = alert.ImmediateBusinessDays.Contains(WeekDays.Friday);
                    chkImmediateSat.Checked = alert.ImmediateBusinessDays.Contains(WeekDays.Saturday);
                    sendAsSingleMessage.Checked = alert.SendAsSingleMessage.Equals(true);
                }

                else if (alert.DailyBusinessDays!=null)
                {
                    sendAsSingleMessage.Enabled = true;
                    rdDaily.Checked = true;
                    pnSubDaily.Visible = rdDaily.Checked;
                    this.ddlAlertTime.SelectedValue = Convert.ToString(alert.SendHour);
                    chkDailySun.Checked = alert.DailyBusinessDays.Contains(WeekDays.Sunday);
                    chkDailyMon.Checked = alert.DailyBusinessDays.Contains(WeekDays.Monday);
                    chkDailyTue.Checked = alert.DailyBusinessDays.Contains(WeekDays.Tuesday);
                    chkDailyThu.Checked = alert.DailyBusinessDays.Contains(WeekDays.Thursday);
                    chkDailyWed.Checked = alert.DailyBusinessDays.Contains(WeekDays.Wednesday);
                    chkDailyFri.Checked = alert.DailyBusinessDays.Contains(WeekDays.Friday);
                    chkDailySat.Checked = alert.DailyBusinessDays.Contains(WeekDays.Saturday);
                    sendAsSingleMessage.Checked = alert.SendAsSingleMessage.Equals(true);
                    ddlAlertTime.Enabled = true;
                }
                else
                {
                    rdWeekly.Checked = true;
                    if (rdWeekly.Checked)
                    {
                        sendAsSingleMessage.Enabled = true;
                        this.ddlAlertWeekday.SelectedValue = Convert.ToString(alert.SendDay);
                        this.ddlAlertTime.SelectedValue = Convert.ToString(alert.SendHour);
                        sendAsSingleMessage.Checked = alert.SendAsSingleMessage.Equals(true);
                        ddlAlertWeekday.Enabled = true;
                        ddlAlertTime.Enabled = true;
                    }

                }

                //alert.BusinessStartHour = Convert.ToInt32(xmlDoc.DocumentElement.SelectSingleNode(XMLElementNames.ImmediateBusinessHoursStart).InnerText);


               //alert.DailyBusinessDays = DesrializeDays(xmlDoc.DocumentElement.SelectSingleNode(XMLElementNames.DailyBusinessDays).InnerText);
               


                //alert.ImmediateBusinessDays = DesrializeDays(xmlDoc.DocumentElement.SelectSingleNode(XMLElementNames.ImmediateBusinessDays).InnerText);
                

                //alert.CombineAlerts = true;
                //alert.SummaryMode = true;

                txtPeriodQty.Text = Convert.ToString(alert.PeriodQty);

                txtRepeatInterval.Text = Convert.ToString(alert.RepeatInterval);

                txtRepeatCount.Text = Convert.ToString(alert.RepeatCount);

                // Evaluation Criteria
                // TODO: Confirm working
                if (alert.CustomEvaluation)
                {
                    this.ConditionCase2.Checked = true;
                    // TODO: Do we need to fire this or above statement does it
                    ConditionCase2_CheckedChanged(null, null);
                }
                else
                {
                    this.ConditionCase1.Checked = true;
                    ConditionCase1_CheckedChanged(null, null);
                }

                this.Conditions = alert.Conditions as List<Condition>;
                this.CustomEvaluationData = alert.CustomEvaluationData;

                //Populate Mail Templates
                FillSelectedTemplates(alertID);
            }
            catch { }
        }

        void FillSelectedTemplates(string alertID)
        {
            try
            {
                //1. Get Mail template instance objects
                MailTemplateUsageObject itemAddedUsageObject = MTManager.GetTemplateUsageObjectForAlert(alertID, AlertEventType.ItemAdded);
                MailTemplateUsageObject itemUpdatedUsageObject = MTManager.GetTemplateUsageObjectForAlert(alertID, AlertEventType.ItemUpdated);
                MailTemplateUsageObject itemDateUsageObject = MTManager.GetTemplateUsageObjectForAlert(alertID, AlertEventType.DateColumn);
                MailTemplateUsageObject itemDeletedUsageObject = MTManager.GetTemplateUsageObjectForAlert(alertID, AlertEventType.ItemDeleted);

                if (this.ddlItemAdded.Items.FindByValue(itemAddedUsageObject.Template.ID) != null)
                {
                    this.ddlItemAdded.SelectedIndex = this.ddlItemAdded.Items.IndexOf(this.ddlItemAdded.Items.FindByValue(itemAddedUsageObject.Template.ID));
                }

                if (this.ddlItemUpdate.Items.FindByValue(itemUpdatedUsageObject.Template.ID) != null)
                {
                    this.ddlItemUpdate.SelectedIndex = this.ddlItemUpdate.Items.IndexOf(this.ddlItemUpdate.Items.FindByValue(itemUpdatedUsageObject.Template.ID));
                }

                if (this.ddlDateTime.Items.FindByValue(itemDateUsageObject.Template.ID) != null)
                {
                    this.ddlDateTime.SelectedIndex = this.ddlDateTime.Items.IndexOf(this.ddlDateTime.Items.FindByValue(itemDateUsageObject.Template.ID));
                }

                if (this.ddlItemDelete.Items.FindByValue(itemDeletedUsageObject.Template.ID) != null)
                {
                    this.ddlItemDelete.SelectedIndex = this.ddlItemDelete.Items.IndexOf(this.ddlItemDelete.Items.FindByValue(itemDeletedUsageObject.Template.ID));
                }
                //ddlItemAdded.sele;
                //ddlItemUpdate.Items.Add(li);
                //ddlItemDelete.Items.Add(li);
                //ddlDateTime.Items.Add(li);

            }
            catch { }


        }

        Alert PrepareAlert(string alertId)
        {
            Alert alert = new Alert();
            try
            {

                //Set the alert Id if it is existing alert other wise its 0
                alert.Id = alertId;

                //Get the General Information
                alert.Title = txtTitle.Text;
                alert.WebId = ddlSite.SelectedValue;
                alert.ListId = ddlList.SelectedValue;
                // TODO
                string strItemId = Request.QueryString["ID"];
                if (string.IsNullOrEmpty(strItemId))
                {
                    strItemId = "0";
                }
                alert.ItemID = strItemId;

                //Get Recipient Section
                alert.ToAddress = txtTo.Text;
                alert.CcAddress = txtCc.Text;
                alert.BccAddress = txtBcc.Text;
                alert.FromAdderss = txtFrom.Text;
                //TODO
                //alert.BlockedUsers = 


                //Event Type
                if (chkItemAdded.Checked)
                {
                    alert.AlertType.Add(AlertEventType.ItemAdded);
                }
                if (chkItemDeleted.Checked)
                {
                    alert.AlertType.Add(AlertEventType.ItemDeleted);
                }
                if (chkItemUpdated.Checked)
                {
                    alert.AlertType.Add(AlertEventType.ItemUpdated);
                }
                if (chkDateColumn.Checked)
                {
                    alert.AlertType.Add(AlertEventType.DateColumn);
                }


                alert.DateColumnName = this.ddlDateColumn.SelectedValue;
                alert.PeriodType = (PeriodType)Enum.Parse(typeof(PeriodType), ddlPeriodType.SelectedValue);
                alert.PeriodPosition = (PeriodPosition)Enum.Parse(typeof(PeriodPosition), ddlPeriodPosition.SelectedValue); ;
                alert.Repeat = Convert.ToBoolean(chkRepeat.Checked);
                alert.RepeatType = (PeriodType)Enum.Parse(typeof(PeriodType), ddlRepeatType.SelectedValue);
                alert.ImmidiateAlways = Convert.ToBoolean(rdImmediateAlways.Checked);
                alert.ImmediateDays = Convert.ToBoolean(rdImmediateBusinessdays.Checked);
                alert.BusinessStartHour = Convert.ToInt32(ddlImmediateBusinessStartTime.SelectedValue);
                alert.BusinessendtHour = Convert.ToInt32(ddlImmediateBusinessEndTime.SelectedValue);
                alert.SendDay = Convert.ToInt32(ddlAlertWeekday.SelectedValue);
                alert.SendHour = Convert.ToInt32(ddlAlertTime.SelectedValue);
                alert.SendAsSingleMessage = Convert.ToBoolean(sendAsSingleMessage.Checked);


                //when To Send
               
                if(rdImmediateAlways.Checked)
                { alert.SendType = SendType.ImmediateAlways; }
                else if(rdImmediateBusinessdays.Checked)
                { alert.SendType = SendType.ImmediateBusinessDays; }
                else if (rdDaily.Checked)
                { alert.SendType = SendType.Daily; }
                else if (rdWeekly.Checked)
                { alert.SendType = SendType.Weekly; }

                //alert.DailyBusinessDays = DesrializeDays(xmlDoc.DocumentElement.SelectSingleNode(XMLElementNames.DailyBusinessDays).InnerText);
                alert.DailyBusinessDays = new List<WeekDays>();
                if (alert.SendType == SendType.Daily)
                {
                    //if (chkDailySun.Checked)
                    //{
                    //    alert.DailyBusinessDays.Add(WeekDays.Sunday);
                    //}
                    if (chkDailySun.Checked)
                    {
                        alert.DailyBusinessDays.Add(WeekDays.Sunday);
                    }
                    if (chkDailyMon.Checked)
                    {
                        alert.DailyBusinessDays.Add(WeekDays.Monday);
                    }
                    if (chkDailyTue.Checked)
                    {
                        alert.DailyBusinessDays.Add(WeekDays.Tuesday);
                    }
                    if (chkDailyWed.Checked)
                    {
                        alert.DailyBusinessDays.Add(WeekDays.Wednesday);
                    }
                    if (chkDailyThu.Checked)
                    {
                        alert.DailyBusinessDays.Add(WeekDays.Thursday);
                    }
                    if (chkDailyFri.Checked)
                    {
                        alert.DailyBusinessDays.Add(WeekDays.Friday);
                    }
                    if (chkDailySat.Checked)
                    {
                        alert.DailyBusinessDays.Add(WeekDays.Saturday);
                    }
                }

                //alert.ImmediateBusinessDays = DesrializeDays(xmlDoc.DocumentElement.SelectSingleNode(XMLElementNames.ImmediateBusinessDays).InnerText);
                alert.ImmediateBusinessDays = new List<WeekDays>();
                if (alert.SendType == SendType.ImmediateBusinessDays)
                {
                    if (chkImmediateSun.Checked)
                    {
                        alert.ImmediateBusinessDays.Add(WeekDays.Sunday);
                    }
                    if (chkImmediateMon.Checked)
                    {
                        alert.ImmediateBusinessDays.Add(WeekDays.Monday);
                    }
                    if (chkImmediateThu.Checked)
                    {
                        alert.ImmediateBusinessDays.Add(WeekDays.Tuesday);
                    }
                    if (chkImmediateWed.Checked)
                    {
                        alert.ImmediateBusinessDays.Add(WeekDays.Wednesday);
                    }
                    if (chkImmediateThu.Checked)
                    {
                        alert.ImmediateBusinessDays.Add(WeekDays.Thursday);
                    }
                    if (chkImmediateFri.Checked)
                    {
                        alert.ImmediateBusinessDays.Add(WeekDays.Friday);
                    }
                    if (chkImmediateSat.Checked)
                    {
                        alert.ImmediateBusinessDays.Add(WeekDays.Saturday);
                    }
                }
                //TODO
                alert.CombineAlerts = true;
                alert.SummaryMode = true;

                if (!string.IsNullOrEmpty(txtPeriodQty.Text))
                {
                    alert.PeriodQty = Convert.ToInt32(txtPeriodQty.Text);
                }
                else
                {
                    alert.PeriodQty = 0;
                }
                if (!string.IsNullOrEmpty(txtRepeatInterval.Text))
                {
                    alert.RepeatInterval = Convert.ToInt32(txtRepeatInterval.Text);
                }
                else
                {
                    alert.RepeatInterval = 0;
                }

                if (!string.IsNullOrEmpty(txtRepeatCount.Text))
                {
                    alert.RepeatCount = Convert.ToInt32(txtRepeatCount.Text);
                }
                else
                { alert.RepeatCount = 0; }


                // Conditions
                // TODO: Verify with Avinash
                alert.CustomEvaluation = this.ConditionEditor.Visible;
                alert.Conditions = this.Conditions;
                alert.CustomEvaluationData = this.CustomEvaluationData;

                //Add alert owner
                alert.Owner = SPContext.Current.Web.CurrentUser;

                //Create new alert
                int alertID = AlertManager.AddAlert(SPContext.Current.Site.RootWeb, alert);

                //Create mail template instances
                CreateMailTemplateUsageObjects(alertID);


            }
            catch { }
                return alert;
        }

        void btnAlertcancel_Click(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            this.GoBack();
        }

        #endregion

        #region Template Related events

        void btnCopyToClipBoard_Click(object sender, EventArgs e)
        {
            try
            {
                //string copyText = lstPlaceHolders.SelectedItem.Text;
                //System.Windows.Forms.Clipboard.SetText(copyText);
            }
            catch
            {
            }
            //lstPlaceHolders.SelectedItem.
        }

        void btnAddToSubject_Click(object sender, EventArgs e)
        {
            if(lstPlaceHolders.SelectedItem.Text.Equals("------------"))
            {

            }

            else if (lstPlaceHolders.SelectedItem != null)
            {
                txtMailSubject.Text += " " + "[" + lstPlaceHolders.SelectedItem.Text + "]";
            }
        }

        void btnTemplateAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtMailTemplateName.Text))
                {
                    ErrorMessageLabel2.Text = "• Missing Mail Template title";
                }
                else if (string.IsNullOrEmpty(txtMailSubject.Text))
                {
                    ErrorMessageLabel2.Text = "• Missing Mail Template Subject";
                }
                else
                {
                    AddUpdateTemplate("0");
                    this.ClearItems();
                    ErrorMessageLabel2.Text = string.Empty;
                }
            }
            catch { }
        }

        void btnTemplateUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                 if (string.IsNullOrEmpty(txtMailTemplateName.Text))
                {
                    ErrorMessageLabel2.Text = "• Missing Mail Template title";
                }
                 else if (string.IsNullOrEmpty(txtMailSubject.Text))
                 {
                     ErrorMessageLabel2.Text = "• Missing Mail Template Subject";
                 }
                 else
                 {
                     AddUpdateTemplate(this.hiddenTemplateID.Text);
                     this.ClearItems();
                     ErrorMessageLabel2.Text = string.Empty;
                 }
            }
            catch { }

        }
        void ClearItems()
        {
            txtMailTemplateName.Text = string.Empty;
            txtMailSubject.Text = string.Empty;
            txtBody.Text = string.Empty;
            chkIncludeUpdatedColumns.Checked = false;
            chkHighlightUpdatedColumns.Checked = false;
            chkInsertAttachments.Checked = false;
        }

        void AddUpdateTemplate(string templateID)
        {

            try
            {
                if (txtMailTemplateName.Text != "")
                {
                    SPList mailTemplateList = SPContext.Current.Site.RootWeb.Lists.TryGetList(ListAndFieldNames.MTListName);

                    if (mailTemplateList != null)
                    {
                        SPListItem listItem = null;
                        if (templateID != "0")
                        {
                            listItem = mailTemplateList.GetItemById(Convert.ToInt32(templateID));
                        }
                        if (listItem == null)
                        {
                            listItem = mailTemplateList.AddItem();
                        }

                        listItem["Title"] = txtMailTemplateName.Text;
                        listItem[ListAndFieldNames.MTListMailSubjectFieldName] = txtMailSubject.Text;
                        listItem[ListAndFieldNames.MTListMailBodyFieldName] = txtBody.Text;
                        listItem[ListAndFieldNames.MTListInsertUpdatedFieldsFieldName] = chkIncludeUpdatedColumns.Checked;
                        listItem[ListAndFieldNames.MTListInsertAttachmentsFieldName] = chkInsertAttachments.Checked;
                        listItem[ListAndFieldNames.MTListHighLightUpdatedFieldsFieldName] = chkHighlightUpdatedColumns.Checked;
                        listItem[ListAndFieldNames.MTListOwnerFieldName] = SPContext.Current.Web.CurrentUser;
                        listItem[ListAndFieldNames.MTUShareTemplateWithAllUsers] = chkShareTemplateWithOtherUsers.Checked;

                        listItem.Update();
                        PopulateTemplates();
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        void btnTemplateCancel_Click(object sender, EventArgs e)
        {
            this.ClearItems();
        }

        void CreateMailTemplateUsageObjects(int alertID)
        {
            try
            {
                //Delete existing usage objects and create new
                MTManager.DeleteTemplateUsageObjects(alertID.ToString());

                string[] templateIDs = new string[] { ddlItemAdded.SelectedValue, ddlItemUpdate.SelectedValue, ddlItemDelete.SelectedValue, ddlDateTime.SelectedValue };
                Dictionary<string, List<AlertEventType>> dictUsage = new Dictionary<string, List<AlertEventType>>();
                foreach (string templateID in templateIDs)
                {
                    if (!dictUsage.ContainsKey(templateID))
                    {
                        dictUsage.Add(templateID, new List<AlertEventType>());
                    }

                }

                dictUsage[ddlItemAdded.SelectedValue].Add(AlertEventType.ItemAdded);
                dictUsage[ddlItemUpdate.SelectedValue].Add(AlertEventType.ItemUpdated);
                dictUsage[ddlItemDelete.SelectedValue].Add(AlertEventType.ItemDeleted);
                dictUsage[ddlDateTime.SelectedValue].Add(AlertEventType.DateColumn);

                foreach (string key in dictUsage.Keys)
                {
                    MailTemplateUsageObject mtObject = new MailTemplateUsageObject();
                    mtObject.AlertType = dictUsage[key];
                    MailTemplate mTemplate = MTManager.GetMailtemplateByID(key);
                    mtObject.Template = mTemplate;
                    mtObject.HighLightUpdatedFields = true;
                    mtObject.InsertAttachments = true;
                    mtObject.InsertUpdatedFields = true;
                    mtObject.ShareTemplateWithAllUsers = true;
                    MTManager.AddMailTemplateUsageObject(Convert.ToString(alertID), mtObject);
                }

            }
            catch { }
        }

        void PopulateTemplates()
        {

            //Get all the templated for the current user
            SPSecurity.RunWithElevatedPrivileges(delegate
               {
                   //Dictionary<string, string> templatesByUser = MTManager.GetTemplatesByUser(Convert.ToInt32(this.ddlUserID.SelectedItem.Value));
                       Dictionary<string, string> templatesByUser = MTManager.GetTemplatesByUser(Convert.ToInt32(this.ddlUserID.SelectedItem.Value));
                       ddlItemAdded.Items.Clear();
                       ddlItemUpdate.Items.Clear();
                       ddlItemDelete.Items.Clear();
                       ddlDateTime.Items.Clear();
                       foreach (string keyId in templatesByUser.Keys)
                       {

                           ListItem li = new ListItem(templatesByUser[keyId], keyId);
                           ddlItemAdded.Items.Add(li);
                           ddlItemUpdate.Items.Add(li);
                           ddlItemDelete.Items.Add(li);
                           ddlDateTime.Items.Add(li);
                       }
               });
        }

        void FillTemplate(string templateID)
        {
            //Get the template by its id
            MailTemplate mTemplate = MTManager.GetMailtemplateByID(templateID);
            string curUser = SPContext.Current.Web.CurrentUser.ID + ";#" + SPContext.Current.Web.CurrentUser.Name;
            //SPUser curUser = SPContext.Current.Web.CurrentUser;
            if (!curUser.Equals(mTemplate.Created))
            {
                chkShareTemplateWithOtherUsers.Enabled = false;
            }
            //fill those values in to form
            txtMailTemplateName.Text = mTemplate.Name;
            txtMailSubject.Text = mTemplate.Subject;
            txtBody.Text = mTemplate.Body;
            chkIncludeUpdatedColumns.Checked = mTemplate.InsertUpdatedFields;
            chkInsertAttachments.Checked = mTemplate.InsertAttachments;
            chkHighlightUpdatedColumns.Checked = mTemplate.HighLightUpdatedFields;
            chkShareTemplateWithOtherUsers.Checked = mTemplate.ShareTemplateWithAllUsers;

        }

        void DeleteTemplate(string templateID)
        {
            MTManager.DeleteTemplateByID(templateID);
            PopulateTemplates();
        }

        void linkDateTimeDelete_Click(object sender, EventArgs e)
        {
            this.DeleteTemplate(this.ddlDateTime.SelectedValue);
        }

        void linkDateTimeEdit_Click(object sender, EventArgs e)
        {
            this.FillTemplate(this.ddlDateTime.SelectedValue);
            this.hiddenTemplateID.Text = this.ddlDateTime.SelectedValue;
        }

        void lnkItemDeleteDelete_Click(object sender, EventArgs e)
        {

            this.DeleteTemplate(this.ddlItemDelete.SelectedValue);
        }

        void lnkItemDeleteEdit_Click(object sender, EventArgs e)
        {
            this.FillTemplate(this.ddlItemDelete.SelectedValue);
            this.hiddenTemplateID.Text = this.ddlItemDelete.SelectedValue;
        }

        void lnkItemUpdateDelete_Click(object sender, EventArgs e)
        {
            this.DeleteTemplate(this.ddlItemUpdate.SelectedValue);
        }

        void lnkItemUpdateEdit_Click(object sender, EventArgs e)
        {
            this.FillTemplate(this.ddlItemUpdate.SelectedValue);
            this.hiddenTemplateID.Text = this.ddlItemUpdate.SelectedValue;
        }

        void lnkItemAddedDelete_Click(object sender, EventArgs e)
        {
            this.DeleteTemplate(this.ddlItemAdded.SelectedValue);
        }

        void lnkItemAddedEdit_Click(object sender, EventArgs e)
        {
            this.FillTemplate(this.ddlItemAdded.SelectedValue);
            this.hiddenTemplateID.Text = this.ddlItemAdded.SelectedValue;
        }

        #endregion

        #region On Change Events

        void rdDaily_CheckedChanged(object sender, EventArgs e)
        {
            rdImmediateAlways.Checked = false;
            rdImmediateBusinessdays.Checked = false;
            pnSubDaily.Visible = rdDaily.Checked;
            sendAsSingleMessage.Enabled = true;
            sendAsSingleMessage.Checked = false;
            pnSubImmediately.Visible = !rdDaily.Checked;
            ddlAlertTime.Enabled = true;
            ddlAlertWeekday.Enabled = false;
            //pnSubDaily
        }

        void rdImmediateBusinessdays_CheckedChanged(object sender, EventArgs e)
        {
            sendAsSingleMessage.Enabled = true;
            sendAsSingleMessage.Checked = false;
            ddlAlertTime.Enabled = false;
            ddlAlertWeekday.Enabled = false;
            pnImmediateBusinessDays.Visible = rdImmediateBusinessdays.Checked; 
            //pnImmediateBusinessDays
        }

        void rdImmediately_CheckedChanged(object sender, EventArgs e)
        {
            ddlAlertTime.Enabled = false;
            ddlAlertWeekday.Enabled = false;
            sendAsSingleMessage.Enabled = rdImmediateBusinessdays.Checked;
            sendAsSingleMessage.Checked = false;
            pnSubImmediately.Visible = rdImmediately.Checked;
            pnSubDaily.Visible = !rdImmediately.Checked;
            //pnSubImmediately
        }

        void ddlSite_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                this.PopulateLists(this.ddlSite.SelectedValue);
            }
            catch
            {
            }
        }

        void ddlList_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListChanged();
        }

        void PopulateSites()
        {
            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate
                {
                    using (SPSite site = new SPSite(SPContext.Current.Site.ID))
                    {
                        if (site != null)
                        {
                            SPWebCollection allWebs = site.RootWeb.GetSubwebsForCurrentUser();
                            this.ddlSite.Items.Clear();
                            this.ddlSite.Items.Add(new ListItem(site.RootWeb.Title, site.RootWeb.ID.ToString()));
                            foreach (SPWeb web in allWebs)
                            {
                                ListItem newWebItem = new ListItem(web.Title, web.ID.ToString());
                                if (!this.ddlSite.Items.Contains(newWebItem))
                                {
                                    this.ddlSite.Items.Add(newWebItem);
                                }
                                web.Dispose();
                            }

                            this.PopulateLists(this.ddlSite.SelectedValue);
                        }
                    }
                });

            }
            catch { }
        }
        //void PopulateSites()
        //{
        //    try
        //    {
        //        SPSite site = SPContext.Current.Site;
        //        if (site != null)
        //        {
        //            SPWebCollection allWebs = site.AllWebs;
        //            this.ddlSite.Items.Clear();
        //            foreach (SPWeb web in allWebs)
        //            {
        //                ListItem newWebItem = new ListItem(web.Title, web.ID.ToString());
        //                if (!this.ddlSite.Items.Contains(newWebItem))
        //                {
        //                    this.ddlSite.Items.Add(newWebItem);
        //                }

        //            }

        //            this.PopulateLists(this.ddlSite.SelectedValue);
        //        }

        //    }
        //    catch { }
        //}

        void ListChanged()
        {
            try
            {
                
                //this.list = SPContext.Current.Site.AllWebs[new Guid(this.ddlSite.SelectedValue)].Lists[new Guid(ddlList.SelectedValue)];
                if (SPContext.Current.Site.RootWeb.ID.ToString() == this.ddlSite.SelectedValue)
                {
                    this.list = SPContext.Current.Site.RootWeb.Lists[new Guid(ddlList.SelectedValue)];
                }
                else
                {
                    this.list = SPContext.Current.Site.RootWeb.GetSubwebsForCurrentUser()[new Guid(this.ddlSite.SelectedValue)].Lists[new Guid(ddlList.SelectedValue)];
                }

                ddlUsersInColumn.Items.Clear();
                ddlDateColumn.Items.Clear();
                lstPlaceHolders.Items.Clear();
                lstPlaceHolders.Items.Add("Site Link");
                lstPlaceHolders.Items.Add("Site Title");
                lstPlaceHolders.Items.Add("List Link");
                lstPlaceHolders.Items.Add("List Title");
                lstPlaceHolders.Items.Add("Item Link");
                lstPlaceHolders.Items.Add("Item Title");
                lstPlaceHolders.Items.Add("Edit Item");
                lstPlaceHolders.Items.Add("Edit Alerts");
                lstPlaceHolders.Items.Add("------------");
                if (this.list != null)
                {
                    foreach (SPField field in this.list.Fields)
                    {
                        ListItem fields = new ListItem(field.Title);
                        if (!this.lstPlaceHolders.Items.Contains(fields) && field != null && !field.Hidden)
                        {
                            if (field.Type == SPFieldType.User)
                            {
                                ListItem lItem = new ListItem(field.Title, field.InternalName);
                                ddlUsersInColumn.Items.Add(field.Title);
                            }

                            if (field.Type == SPFieldType.DateTime)
                            {
                                ListItem lItem = new ListItem(field.Title, field.InternalName);
                                ddlDateColumn.Items.Add(lItem);
                            }

                            lstPlaceHolders.Items.Add(field.Title);
                        }
                    }

                    this.Conditions = null;
                    //this.gvConditions.DataSource = this.Conditions;
                    //this.gvConditions.DataBind();
                    //this.EnsureConditionInsertRow();
                }
            }
            catch
            {
            }
        }

        void PopulateLists(string webid)
        {
            try
            {
                //SPListCollection allLists = SPContext.Current.Site.AllWebs[new Guid(webid)].Lists;
                SPListCollection allLists = null;
                if (SPContext.Current.Site.RootWeb.ID.ToString() == webid)
                {
                    allLists = SPContext.Current.Site.RootWeb.Lists;
                }
                else
                {
                     allLists = SPContext.Current.Site.RootWeb.GetSubwebsForCurrentUser()[new Guid(webid)].Lists;
                }
                this.ddlList.Items.Clear();
                if (allLists != null)
                {
                    foreach (SPList list in allLists)
                    {
                        ListItem newListItem = new ListItem(list.Title, list.ID.ToString());
                        if (!this.ddlList.Items.Contains(newListItem))
                        {
                            this.ddlList.Items.Add(newListItem);
                        }

                    }
                    ListChanged();
                }


            }
            catch
            {
            }
        }

        void FillHours()
        {
            this.ddlImmediateBusinessStartTime.Items.Clear();
            this.ddlImmediateBusinessEndTime.Items.Clear();
            DateTime today = DateTime.Today;
            for (int i = 0; i < 0x18; i++)
            {
                this.ddlImmediateBusinessStartTime.Items.Add(new ListItem(today.ToShortTimeString(), i.ToString()));
                this.ddlImmediateBusinessEndTime.Items.Add(new ListItem(today.ToShortTimeString(), i.ToString()));
                today = today.AddHours(1.0);
            }
        }

        void InitializeSendTimeSelectors()
        {
            try
            {
                ddlAlertWeekday.Items.Clear();
                DateTimeFormatInfo dateTimeFormat = SPContext.Current.Web.Locale.DateTimeFormat;
                int num = 0;
                foreach (string str in dateTimeFormat.DayNames)
                {
                    ddlAlertWeekday.Items.Add(new ListItem(str, num.ToString()));
                    num++;
                }
                InitializeHours();
            }
            catch { }
        }

        void InitializeHours()
        {
            try
            {
                this.ddlAlertTime.Items.Clear();
                for (int i = 0; i < 0x18; i++)
                {
                    if (SPContext.Current.Web.RegionalSettings.Time24)
                    {
                        this.ddlAlertTime.Items.Add(new ListItem(i.ToString(), i.ToString()));
                    }
                    else
                    {
                        string str;
                        int num2 = (i > 12) ? (i - 12) : i;
                        if (i == 0)
                        {
                            num2 = 12;
                        }
                        if (i >= 12)
                        {
                            str = SPContext.Current.Web.RegionalSettings.PM + " " + num2.ToString() + ":00";
                        }
                        else
                        {
                            str = SPContext.Current.Web.RegionalSettings.AM + " " + num2.ToString() + ":00";
                        }
                        this.ddlAlertTime.Items.Add(new ListItem(str, i.ToString()));
                    }
                }
            }
            catch { }
        }

        void setDefaultValues()
        {
            
            try
            {
                string smtpServerAddress = string.Empty;
                string curuser = string.Empty;
                this.txtPeriodQty.Text = "30";
                this.txtRepeatInterval.Text = "30";
                this.txtRepeatCount.Text = "1";
                smtpServerAddress = SPContext.Current.Site.WebApplication.OutboundMailSenderAddress;
                if (!string.IsNullOrEmpty(smtpServerAddress))
                {
                    this.txtFrom.Text= smtpServerAddress;
                }
                curuser = SPContext.Current.Web.CurrentUser.Email;
                if(!string.IsNullOrEmpty(curuser))
                {
                    this.txtTo.Text=curuser;
                }
                this.rdImmediately.Checked = true;
                this.pnSubImmediately.Visible = rdImmediately.Checked;
                this.rdImmediateAlways.Checked = true;
            }
            catch { }
        }
     

        #endregion



    }
}
