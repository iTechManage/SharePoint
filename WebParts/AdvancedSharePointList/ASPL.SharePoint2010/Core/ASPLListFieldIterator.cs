using System;
using System.Collections.Specialized;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using ASPL.Blocks;
using ASPL.ConfigModel;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebControls;

namespace ASPL.SharePoint2010.Core
{
    public class ASPLListFieldIterator : ListFieldIterator
    {
        private LiteralControl TabControl;
        protected UpdatePanel updatePanelIterator;
        protected HiddenField RequestResultTabsInfoHidden;
        protected string RequestResultTabsInfoHiddenValue;
        private HiddenField CurrentTab;
        private HiddenField hdnChangedControls;
        private HiddenField hdnLiveControls;
        private Tabs allTabs = null;
        private FieldPermissions allFieldPermissions = null;
        private FieldDefaults allFieldDefaults = null;
        protected SPFormContext _formContext = SPContext.Current.FormContext;

        protected override bool IsFieldExcluded(SPField field)
        {
            return base.IsFieldExcluded(field);
        }

        protected override void OnInit(EventArgs e)
        {
            try
            {
                if (SPContext.Current.SiteFeatures[new Guid(Constants.ASPLFeature.FeatureID)] == null)
                {
                    base.OnInit(e); return;
                }

                RendringUtil.RenderResources(this.Page.Header);

                allTabs = Tabs.LoadTabs(GetConfigFile(Constants.ConfigFile.TabSettingFile));
                allFieldPermissions = FieldPermissions.LoadFieldPermissions(GetConfigFile(Constants.ConfigFile.FieldPermissionFile));
                allFieldDefaults = FieldDefaults.LoadFieldDefaults(GetConfigFile(Constants.ConfigFile.FieldDefaultFile));


                #region Testing data
                //allTabs = new Tabs();
                //Tab t1 = new Tab(0, "Tab1", "foo");

                //t1.Fields.Add(new Field("Title"));
                //t1.Fields.Add(new Field("Predecessors"));
                //t1.Fields.Add(new Field("Priority"));
                //t1.Fields.Add(new Field("Status"));
                //t1.Permissions.Add(new TabPermission(false, Enums.PermissionLevel.Write, new List<Enums.SPForms>() { Enums.SPForms.New, Enums.SPForms.Edit }, "", Enums.Operator.None));
                //t1.Permissions.Add(new TabPermission(false, Enums.PermissionLevel.Deny, new List<Enums.SPForms>() { Enums.SPForms.New, Enums.SPForms.Edit }, "", Enums.Operator.None));



                //t1.IsFirst = true;

                //allTabs.Add(t1);


                //Tab t2 = new Tab(0, "Tab2", "foo");
                //t2.Fields.Add(new Field("PercentComplete"));
                //t2.Fields.Add(new Field("DueDate"));
                //t2.Fields.Add(new Field("Title"));
                //t2.Fields.Add(new Field("Body"));
                //t2.IsSelected = true;
                //t2.Permissions.Add(new TabPermission(false, Enums.PermissionLevel.Deny, new List<Enums.SPForms>() { Enums.SPForms.New, Enums.SPForms.Edit }, "", Enums.Operator.None));


                //TabPermission p3 = new TabPermission(false, Enums.PermissionLevel.Write, new List<Enums.SPForms>() { Enums.SPForms.New, Enums.SPForms.Edit }, "", Enums.Operator.None);
                //p3.Conditions.Add(new Condition(new Field("Title"), Enums.Operator.Contains, "Make write"));
                //t2.Permissions.Add(p3);


                //allTabs.Add(t2);


                //Tab t3 = new Tab(0, "Tab3", "foo");
                //t3.Fields.Add(new Field("PercentComplete"));
                //t3.Fields.Add(new Field("DueDate"));
                //t3.Fields.Add(new Field("Predecessors"));
                //t3.IsLast = true;

                //allTabs.Add(t3);

                //allFieldPermissions = new FieldPermissions();
                //allFieldPermissions.Add(new FieldPermission(new Field("Title"), Enums.PermissionLevel.Write, new List<Enums.SPForms>() { Enums.SPForms.New, Enums.SPForms.Edit }, "", Enums.Operator.None));
                //allFieldPermissions.Add(new FieldPermission(new Field("Predecessors"), Enums.PermissionLevel.Read, new List<Enums.SPForms>() { Enums.SPForms.New, Enums.SPForms.Edit }, "", Enums.Operator.None));


                //allFieldDefaults = new FieldDefaults();
                //allFieldDefaults.Add(new FieldDefault(new Field("Title"), "", Enums.Operator.None, "", "You cant touch me!!"));

                //allTabs = null;
                #endregion

                this.updatePanelIterator = new UpdatePanel();
                this.updatePanelIterator.ID = "updatePanelIterator";
                this.updatePanelIterator.RenderMode = UpdatePanelRenderMode.Inline;
                if (this.RequestResultTabsInfoHidden == null)
                {
                    this.RequestResultTabsInfoHidden = new HiddenField();
                }

                this.RequestResultTabsInfoHidden.ID = "RequestResultTabsInfoHidden";

                this.RequestResultTabsInfoHidden.Value = (allTabs == null ? "null" : allTabs.ToHiddenFldValue());
                this.updatePanelIterator.ContentTemplateContainer.Controls.Add(this.RequestResultTabsInfoHidden);
                Panel panel = new Panel();
                panel.Style.Add(HtmlTextWriterStyle.Position, "absolute");
                panel.Style.Add(HtmlTextWriterStyle.Width, "100%");
                this.Controls.Add(panel);
                panel.Controls.Add(this.updatePanelIterator);
                panel.Controls.Add(UpdateTemplate.GetUpdateProgress(this.updatePanelIterator.ID));
                CreateHelperControls();
                base.OnInit(e);

                // register save handler if not in display mode and form is posted back
                if ((Page.IsPostBack) && (ControlMode != SPControlMode.Display))
                {
                    _formContext.OnSaveHandler += new EventHandler(SaveHandler);
                }
            }
            catch (Exception exp)
            {
                base.OnInit(e);
                Logging.Log(exp);
            }
        }

        private XmlDocument GetConfigFile(string filename)
        {
            try
            {
                SPFile file =
                    SPContext.Current.Web.GetFile(SPUtility.GetFullUrl(SPContext.Current.Web.Site,
                    SPContext.Current.List.RootFolder.ServerRelativeUrl.TrimEnd('/') + "/" + filename));

                if (file.Exists)
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(file.OpenBinaryStream());
                    return doc;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception exp)
            {
                Logging.Log(exp); return null;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            if (SPContext.Current.SiteFeatures[new Guid(Constants.ASPLFeature.FeatureID)] == null)
            {
                base.OnLoad(e); return;
            }

            if (Page.IsPostBack)
            {
                Page.Validate();
                this.Validate();
            }
        }

        protected void SaveHandler(object sender, EventArgs e)
        {
            Page.Validate();

            if (Page.IsValid)
            {
                // do custom activities, send mail, create task, set permissions etc
                // we should save the item explicitly
                Item.Update();
            }
        }

        public void Validate()
        {
            FieldValidations allFieldVals = FieldValidations.LoadFieldValidations(GetConfigFile(Constants.ConfigFile.FieldValidationFile));

            if (base.ControlMode != SPControlMode.Display && allFieldVals != null)
            {
                #region Test data
                //allFieldVals.Add(new FieldValidation(new Field("Title"), Enums.ValidationRule.Column, Enums.Operator.Equal, "adil", "error for adil", "", Enums.Operator.None));
                //allFieldVals.Add(new FieldValidation(new Field("Status"), Enums.ValidationRule.Column, Enums.Operator.Contains, "Start", "error for start", "", Enums.Operator.None));
                //allFieldVals.Add(new FieldValidation(new Field("Title"), Enums.ValidationRule.Pattern, Enums.Operator.Equal, @"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", "not valid email", "pc\\pdadmin", Enums.Operator.Equal));
                //allFieldVals.Add(new FieldValidation(new Field("Predecessors"), Enums.ValidationRule.Column, Enums.Operator.Contains, "adil", "contains adil", "pc\\pdadmin", Enums.Operator.Equal));
                #endregion

                bool isErroredField = false;
                foreach (FieldValidation v in allFieldVals)
                {
                    if (ConditionEvaluator.EvaluateFromUIValue(v.Conditions, _formContext, ClientID) && PrincipalEvaluator.Check(v.ForSPPrinciples, v.BySPPrinciplesOperator))
                    {
                        BaseFieldControl field = ValidationInjector.GetFieldControlByName(v.OnField.SPName, _formContext, ClientID);

                        if (field != null)
                        {
                            switch (v.Rule)
                            {
                                case Enums.ValidationRule.Column:
                                    if (ValidationInjector.InvalidColumnValue(field.Value, v.ByRuleOperator, v.Value.ToString(), field.Field.FieldValueType))
                                    {
                                        ValidationInjector.SetValidationError(field, v.ErrorMsg); isErroredField = true;
                                    }
                                    break;
                                case Enums.ValidationRule.length:
                                    int length = (field.Value == null ? 0 : field.Value.ToString().Length);
                                    if (ValidationInjector.InvalidLengthValue(length, v.ByRuleOperator, v.Value.ToString()))
                                    {
                                        ValidationInjector.SetValidationError(field, v.ErrorMsg); isErroredField = true;
                                    }
                                    break;
                                case Enums.ValidationRule.Pattern:
                                    if (ValidationInjector.InvalidPatternValue((field.Value ?? "").ToString(), v.Value.ToString()))
                                    {
                                        ValidationInjector.SetValidationError(field, v.ErrorMsg); isErroredField = true;
                                    }
                                    break;
                            }
                        }

                        if (allTabs != null)
                        {
                            string tabname = allTabs.GetTabNameOfField(v.OnField.SPName);
                            if (isErroredField && !string.IsNullOrEmpty(tabname))
                            {
                                this.Page.ClientScript.RegisterStartupScript(base.GetType(), "SLFE_UpdatePanelHelper", string.Concat(new string[]
							{
								"<script type='text/javascript'>g_SLFEUpdatePanelHelper='",
								this.updatePanelIterator.ClientID,
								"';\r\ng_RequestResultTabsInfoHidden = '",
								this.RequestResultTabsInfoHidden.ClientID,
								"';\r\nSys.WebForms.PageRequestManager.getInstance().add_pageLoaded(SLFE_OnClientResponseEnded);\r\n</script>"
							}));
                                this.Page.ClientScript.RegisterStartupScript(base.GetType(), "SLFE_CallFirstSelectTab", "<script type='text/javascript'>SLFE_SelectTab('" + allTabs.GetTabNameOfField(v.OnField.SPName) + "')</script>");
                            }
                        }
                    }
                }
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (SPContext.Current.SiteFeatures[new Guid(Constants.ASPLFeature.FeatureID)] == null)
            {
                base.OnPreRender(e); return;
            }

            if (allTabs != null)
            {
                this.Page.ClientScript.RegisterStartupScript(base.GetType(), "SLFE_UpdatePanelHelper", string.Concat(new string[]
			{
				"<script type='text/javascript'>g_SLFEUpdatePanelHelper='",
				this.updatePanelIterator.ClientID,
				"';\r\ng_RequestResultTabsInfoHidden = '",
				this.RequestResultTabsInfoHidden.ClientID,
				"';\r\nSys.WebForms.PageRequestManager.getInstance().add_pageLoaded(SLFE_OnClientResponseEnded);\r\n</script>"
			}));
                this.Page.ClientScript.RegisterStartupScript(base.GetType(), "SLFE_CallFirstSelectTab", "<script type='text/javascript'>SLFE_SelectTab('" + allTabs.GetSelectedTab().Title + "')</script>");
                //base.OnPreRender(e);
            }
        }

        protected override void CreateChildControls()
        {
            if (SPContext.Current.SiteFeatures[new Guid(Constants.ASPLFeature.FeatureID)] == null)
            {
                base.CreateChildControls(); return;
            }

            this.Controls.Clear();

            try
            {
                if (this.ControlTemplate == null)
                {
                    throw new ArgumentException("Could not find ListFieldIterator control template.");
                }

                AddLiteralControl(RendringUtil.RenderTabs(allTabs));

                string allFields = string.Empty;

                for (int i = 0; i < base.Fields.Count; i++)
                {
                    SPField spField = base.Fields[i];
                    SPControlMode ctrlMode = SPControlMode.Invalid;
                    bool isFieldHidden = false;

                    // Permission matrix execution...

                    ctrlMode = PermissionHandler.Handle(spField.InternalName, this.ControlMode, allTabs, allFieldPermissions, SPContext.Current.Web.CurrentUser, out isFieldHidden);

                    if (this.ControlMode == SPControlMode.New && allFieldDefaults != null)
                        RendringUtil.SetDefault(spField, allFieldDefaults);

                    if (!this.IsFieldExcluded(spField) && !spField.Hidden && !spField.ReadOnlyField && !isFieldHidden)
                    {
                        ASPLTemplateContainer tempCon = new ASPLTemplateContainer();
                        this.Controls.Add(tempCon.Template);
                        tempCon.FieldName = spField.InternalName;
                        tempCon.ControlMode = ctrlMode;
                        this.ControlTemplate.InstantiateIn(tempCon.Template);
                        allFields += "'" + spField.InternalName + "~Show',";
                        LiteralControl templateTR = tempCon.Controls[0] as LiteralControl;
                        templateTR.Text = templateTR.Text.Replace("tr", "tr id='" + spField.InternalName + "~Show" + "'");
                    }
                }

                AddLiteralControl("<script type='text/javascript'> var allFieldsArray = new Array(" + allFields.Trim(',') + ");</script>");
            }
            catch (Exception exp)
            {
                this.Controls.Clear();
                base.CreateChildControls();
                Logging.Log(exp);
            }
        }

        protected void CreateHelperControls()
        {
            if (this.Page == null || this.Page.ClientScript.IsClientScriptBlockRegistered("FieldsIteratorBase_ScriptAndCotnrols"))
            {
                return;
            }

            this.TabControl = new LiteralControl();
            this.Controls.Add(this.TabControl);
            this.CurrentTab = new HiddenField();
            this.CurrentTab.ID = "CurrentTabFieldID";

            if (!string.IsNullOrEmpty(this.Page.Request.Form[this.UniqueID + "$CurrentTabFieldID"]))
            {
                this.CurrentTab.Value = this.Page.Request.Form[this.UniqueID + "$CurrentTabFieldID"];
            }

            try
            {
                if (!string.IsNullOrEmpty(this.Page.Request["CurrentTab"]))
                {
                    this.CurrentTab.Value = this.Page.Request["CurrentTab"];
                    NameValueCollection queryString = this.Page.Request.QueryString;
                    //ReflectionUtility.SetPropertyValue(queryString, "IsReadOnly", false);
                    queryString.Remove("CurrentTab");
                }
            }
            catch (Exception ex)
            {
                //this.logger.LogError(ex);
            }

            this.Controls.Add(this.CurrentTab);
            this.hdnChangedControls = new HiddenField();
            this.hdnChangedControls.ID = "hdnChangedControls";
            this.Controls.Add(this.hdnChangedControls);
            this.hdnLiveControls = new HiddenField();
            this.hdnLiveControls.ID = "hdnLiveControls";
            this.Controls.Add(this.hdnLiveControls);
            this.Page.ClientScript.RegisterClientScriptBlock(base.GetType(), "FieldsIteratorBase_ScriptAndCotnrols", string.Concat(new string[]
			{
				"\r\n<script type=\"text/javascript\" language=\"javascript\"> \r\nvar hdnCurrentTabFieldID = '",
				this.CurrentTab.ClientID,
				"';\r\nvar hdnChangedControlsClientID = '",
				this.hdnChangedControls.ClientID,
				"';\r\nfunction SetHiddenChangedControls(fieldName, fieldValue) {\r\n    var hdnCtrl = document.getElementById(hdnChangedControlsClientID);\r\n    if (hdnCtrl == null) return;\r\n    else hdnCtrl.value += ';' + fieldName + '|' + fieldValue;\r\n    try\r\n    {\r\n\t\t__doPostBack(g_SLFEUpdatePanelHelper, hdnChangedControlsClientID);\r\n    }\r\n    catch(e)\r\n    {\r\n        document.aspnetForm.submit();\r\n    }\r\n}\r\n</script>\r\n"
			}));
        }

        public void AddLiteralControl(string s)
        {
            this.Controls.Add(new LiteralControl(s));
        }
    }
}
