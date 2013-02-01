using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Data;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using ASPL.ConfigModel;
using System.Text;
using System.Xml;
using System.IO;
using ASPL.Blocks;
using Microsoft.SharePoint.Utilities;
using System.Web;

namespace ASPL.SharePoint2010.CONTROLTEMPLATES
{
    public partial class FieldValidationSettings : ASPL.SharePoint2010.CONTROLTEMPLATES.ASLP.SharePoint2010.ASPLBaseUserControl
    {
        #region ViewState properties
        protected int ValidationID
        {
            get
            {
                if (ViewState["ValidationID"] == null)
                    ViewState["ValidationID"] = "-1";
                return Convert.ToInt32(ViewState["ValidationID"]);
            }
            set
            {
                ViewState["ValidationID"] = value;
            }
        }

        protected int ValidationConditionID
        {
            get
            {
                if (ViewState["ValidationConditionID"] == null)
                    ViewState["ValidationConditionID"] = "-1";
                return Convert.ToInt32(ViewState["ValidationConditionID"]);
            }
            set
            {
                ViewState["ValidationConditionID"] = value;
            }
        }

        protected DataTable ValidationDataTable
        {
            get
            {
                if (ViewState["ValidationDataTable"] != null)
                    return (DataTable)ViewState["ValidationDataTable"];
                else
                    return createValidationDataTable();
            }
            set
            {
                ViewState["ValidationDataTable"] = value;
            }
        }

        protected DataTable ValidationConditionDataTable
        {
            get
            {
                if (ViewState["dtPermissionConditionDataTable"] != null)
                    return (DataTable)ViewState["dtPermissionConditionDataTable"];
                else
                    return createConditionDataTable();
            }
            set
            {
                ViewState["dtPermissionConditionDataTable"] = value;
            }
        }

        protected DataTable createConditionDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(Helper.CreateAutoRowIDColumn());
            dt.Columns.Add(Constants.ConditionField.ValidationRowID, typeof(int));
            dt.Columns.Add(Constants.ConditionField.SPFieldName, typeof(string));
            dt.Columns.Add(Constants.ConditionField.SPFieldDisplayName, typeof(string));
            dt.Columns.Add(Constants.ConditionField.SPFieldOperatorName, typeof(string));
            dt.Columns.Add(Constants.ConditionField.SPFieldOperatorID, typeof(int));
            dt.Columns.Add(Constants.ConditionField.Value, typeof(string));            
            return dt;
        }

        private DataTable createValidationDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(Helper.CreateAutoRowIDColumn());
            dt.Columns.Add(Constants.ValidationField.ValidationFieldRowID, typeof(int));
            dt.Columns.Add(Constants.ValidationField.SPFieldName, typeof(string));
            dt.Columns.Add(Constants.ValidationField.SPFieldDisplayName, typeof(string));
            dt.Columns.Add(Constants.ValidationField.ValidationRuleName, typeof(string));
            dt.Columns.Add(Constants.ValidationField.ValidationRuleID, typeof(string));
            dt.Columns.Add(Constants.ValidationField.SPFieldOperatorName, typeof(string));
            dt.Columns.Add(Constants.ValidationField.SPFieldOperatorID, typeof(string));
            dt.Columns.Add(Constants.ValidationField.Value, typeof(string));
            dt.Columns.Add(Constants.ValidationField.ErrorMessage, typeof(string));
            dt.Columns.Add(Constants.ValidationField.SPPrinciples, typeof(string));
            dt.Columns.Add(Constants.ValidationField.SPPrinciplesOperatorID, typeof(string));
            dt.Columns.Add(Constants.ValidationField.SPPrinciplesOperatorName, typeof(string));
            dt.Columns.Add(Constants.ValidationField.HasCondition, typeof(string));
            return dt;

        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                FillOperatorsFor(Convert.ToInt32(cboRule.SelectedValue));
                FillListFields();
                LoadValidationSettings();
            }

        }
        
        protected void cboRule_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillOperatorsFor(Convert.ToInt32(cboRule.SelectedValue));

        }

        protected void cmdCreateNewRule_Click(object sender, EventArgs e)
        {
            DataTable validationDataTable = ValidationDataTable;
            DataTable validationConditionDataTable = ValidationConditionDataTable;
            if (cmdCreateNewRule.Text == "Add new Validation Rule")
            {
                DataRow drValidationRule = validationDataTable.NewRow();
                drValidationRule[Constants.ValidationField.SPFieldName] = cboAllFields.SelectedValue;
                drValidationRule[Constants.ValidationField.SPFieldDisplayName] = cboAllFields.SelectedItem.Text;
                drValidationRule[Constants.ValidationField.SPFieldOperatorID] = cboRulesOperators.SelectedValue;
                drValidationRule[Constants.ValidationField.SPFieldOperatorName] = Enums.DisplayString((Enums.Operator)Convert.ToInt32(cboRulesOperators.SelectedValue));
                drValidationRule[Constants.ValidationField.ValidationRuleID] = cboRule.SelectedValue;
                drValidationRule[Constants.ValidationField.ValidationRuleName] = Enums.DisplayString((Enums.ValidationRule)Convert.ToInt32(cboRule.SelectedValue));
                drValidationRule[Constants.ValidationField.Value] = Helper.ReplaceInvalidChar( txtRulesValue.Text);
                drValidationRule[Constants.ValidationField.ErrorMessage] = Helper.ReplaceInvalidChar(string.IsNullOrEmpty(txtErrorMessage.Text) ? "Invalid Field" : txtErrorMessage.Text);
                drValidationRule[Constants.ValidationField.SPPrinciples] = (string.IsNullOrEmpty(peSelectUsers.CommaSeparatedAccounts) ? Constants.AllSPPrinciples : peSelectUsers.CommaSeparatedAccounts);
                drValidationRule[Constants.ValidationField.SPPrinciplesOperatorID] = (string.IsNullOrEmpty(peSelectUsers.CommaSeparatedAccounts) ? 0 : Convert.ToInt32(cboSPPrinciplesOperator.SelectedValue));
                drValidationRule[Constants.ValidationField.SPPrinciplesOperatorName] = (string.IsNullOrEmpty(peSelectUsers.CommaSeparatedAccounts) ? Enums.DisplayString((Enums.Operator)0) : Enums.DisplayString((Enums.Operator)Convert.ToInt32(cboSPPrinciplesOperator.SelectedValue)) + ":");
                drValidationRule[Constants.ValidationField.HasCondition] = "";
                
                validationDataTable.Rows.Add(drValidationRule);
            }

            else if (cmdCreateNewRule.Text == "Update Validation Rule")
                {
                    DataRow drSelectedValidation = Helper.GetRowFromDataTable(validationDataTable, ValidationID);
                    if (drSelectedValidation != null)
                    {
                        drSelectedValidation[Constants.ValidationField.SPFieldName] = cboAllFields.SelectedValue;
                        drSelectedValidation[Constants.ValidationField.SPFieldDisplayName] = cboAllFields.SelectedItem.Text;
                        drSelectedValidation[Constants.ValidationField.SPFieldOperatorID] = cboRulesOperators.SelectedValue;
                        drSelectedValidation[Constants.ValidationField.SPFieldOperatorName] = Enums.DisplayString((Enums.Operator)Convert.ToInt32(cboRulesOperators.SelectedValue)); 
                        drSelectedValidation[Constants.ValidationField.ValidationRuleID] = cboRule.SelectedValue;
                        drSelectedValidation[Constants.ValidationField.ValidationRuleName] = Enums.DisplayString((Enums.ValidationRule)Convert.ToInt32(cboRule.SelectedValue));
                        drSelectedValidation[Constants.ValidationField.Value] = Helper.ReplaceInvalidChar(txtRulesValue.Text);
                        drSelectedValidation[Constants.ValidationField.ErrorMessage] = Helper.ReplaceInvalidChar(string.IsNullOrEmpty(txtErrorMessage.Text) ? "Invalid Field" : txtErrorMessage.Text);
                        drSelectedValidation[Constants.ValidationField.SPPrinciples] = (string.IsNullOrEmpty(peSelectUsers.CommaSeparatedAccounts) ? Constants.AllSPPrinciples : peSelectUsers.CommaSeparatedAccounts);
                        drSelectedValidation[Constants.ValidationField.SPPrinciplesOperatorID] = (string.IsNullOrEmpty(peSelectUsers.CommaSeparatedAccounts) ? 0 : Convert.ToInt32(cboSPPrinciplesOperator.SelectedValue));
                        drSelectedValidation[Constants.ValidationField.SPPrinciplesOperatorName] = (string.IsNullOrEmpty(peSelectUsers.CommaSeparatedAccounts) ? Enums.DisplayString((Enums.Operator)0) : Enums.DisplayString((Enums.Operator)Convert.ToInt32(cboSPPrinciplesOperator.SelectedValue)) + ":");
                        drSelectedValidation[Constants.ValidationField.HasCondition] = Helper.ConditionsToString(validationConditionDataTable, ValidationID, Constants.ConditionField.ValidationRowID);
                 
                    }
                    cmdCreateNewRule.Text = "Add new Validation Rule";
                    lnkAddCondition.Enabled = false;                
                }

            ValidationDataTable = validationDataTable;
            gvResult.DataSource = validationDataTable;
            gvResult.DataBind();

            gvCondition.DataSource = null;
            gvCondition.DataBind();

            clearValidationField();


        }
            
        protected void gvResult_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            DataTable validationDataTable = ValidationDataTable;
            DataTable validationConditionDataTable = ValidationConditionDataTable;
            DataRow drSelectedCondition = Helper.GetRowFromDataTable(validationDataTable, Convert.ToInt32(e.CommandArgument));

            if (e.CommandName == "EditCondition" && drSelectedCondition != null)
            {
                DataTable conditionOfSelectedPermission = Helper.GetViewFromDataTable(validationConditionDataTable, Convert.ToInt32(drSelectedCondition[Constants.RowID]), Constants.ConditionField.ValidationRowID).ToTable();
                gvCondition.DataSource = conditionOfSelectedPermission;
                gvCondition.DataBind();

                ValidationID = Convert.ToInt32(drSelectedCondition[Constants.RowID].ToString());

                cboAllFields.SelectedValue = drSelectedCondition[Constants.ValidationField.SPFieldName].ToString();
                cboRule.SelectedValue = drSelectedCondition[Constants.ValidationField.ValidationRuleID].ToString();
                FillOperatorsFor(Convert.ToInt32(cboRule.SelectedValue));
                cboRulesOperators.SelectedValue = drSelectedCondition[Constants.ValidationField.SPFieldOperatorID].ToString();
                txtRulesValue.Text = drSelectedCondition[Constants.ValidationField.Value].ToString();
                txtErrorMessage.Text = drSelectedCondition[Constants.ValidationField.ErrorMessage].ToString();          
                peSelectUsers.CommaSeparatedAccounts = (drSelectedCondition[Constants.ValidationField.SPPrinciples].ToString() == Constants.AllSPPrinciples ? "" : drSelectedCondition[Constants.PermissionField.SPPrinciples].ToString());
                cboSPPrinciplesOperator.SelectedValue = drSelectedCondition[Constants.ValidationField.SPPrinciplesOperatorID].ToString();

                cmdCreateNewRule.Text = "Update Validation Rule";
                lnkAddCondition.Enabled = true;

                gvCondition.DataSource = Helper.GetViewFromDataTable(validationConditionDataTable, ValidationID, Constants.ConditionField.ValidationRowID);
                gvCondition.DataBind();



            }
            else if (e.CommandName == "DeleteCondition" && drSelectedCondition != null)
            {
                foreach (DataRow drConditions in validationConditionDataTable.Select(Constants.ConditionField.ValidationRowID + "=" + drSelectedCondition[Constants.RowID].ToString()))
                    drConditions.Delete();

                drSelectedCondition.Delete();
                ValidationDataTable = validationDataTable;

                gvResult.DataSource = validationDataTable;
                gvResult.DataBind();

                gvCondition.DataSource = null;
                gvCondition.DataBind();

                cmdCreateNewRule.Text = "Add new Validation Rule";
                lnkAddCondition.Enabled = false;
            }
        }

        protected void lnkAddCondition_Click(object sender, EventArgs e)
        {
            if (ValidationID > 0)
            {
                DataTable validationConditionDataTable = ValidationConditionDataTable;

                if (lnkAddCondition.Text == "Add")
                {

                    DataRow drCondition = validationConditionDataTable.NewRow();
                    drCondition[Constants.ConditionField.ValidationRowID] = ValidationID;
                    drCondition[Constants.ConditionField.SPFieldName] = cboAllFieldsForCondition.SelectedValue;
                    drCondition[Constants.ConditionField.SPFieldDisplayName] = cboAllFieldsForCondition.SelectedItem.Text;
                    drCondition[Constants.ConditionField.SPFieldOperatorName] = Enums.DisplayString((Enums.Operator)Convert.ToInt32(cboConditionOperator.SelectedValue));
                    drCondition[Constants.ConditionField.SPFieldOperatorID] = cboConditionOperator.SelectedValue;
                    drCondition[Constants.ConditionField.Value] = Helper.ReplaceInvalidChar(txtValue.Text);
                    validationConditionDataTable.Rows.Add(drCondition);
                }
                else if (lnkAddCondition.Text == "Update")
                {
                    DataRow drSelectedCondition = Helper.GetRowFromDataTable(ValidationConditionDataTable, ValidationConditionID);

                    if (drSelectedCondition != null)
                    {
                        drSelectedCondition[Constants.ConditionField.SPFieldName] = cboAllFieldsForCondition.SelectedValue;
                        drSelectedCondition[Constants.ConditionField.SPFieldDisplayName] = cboAllFieldsForCondition.SelectedItem.Text;
                        drSelectedCondition[Constants.ConditionField.SPFieldOperatorName] = Enums.DisplayString((Enums.Operator)Convert.ToInt32(cboConditionOperator.SelectedValue));
                        drSelectedCondition[Constants.ConditionField.SPFieldOperatorID] = cboConditionOperator.SelectedValue;
                        drSelectedCondition[Constants.ConditionField.Value] = Helper.ReplaceInvalidChar(txtValue.Text);
                    }

                    lnkAddCondition.Text = "Add";
                }


                ValidationConditionDataTable = validationConditionDataTable;
                gvCondition.DataSource = Helper.GetViewFromDataTable(validationConditionDataTable, ValidationID, Constants.ConditionField.ValidationRowID);
                gvCondition.DataBind();
            }
            else
            {
                lblErrorCondition.Text = "Please select Validation Rule to add condition";
            }

            clearConditionFields();
        }

        protected void gvCondition_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            DataTable validationConditionDataTable = ValidationConditionDataTable;
            DataRow drSelectedCondition = Helper.GetRowFromDataTable(validationConditionDataTable, Convert.ToInt32(e.CommandArgument));

            if (e.CommandName == "EditCondition" && drSelectedCondition != null)
            {
                ValidationConditionID = Convert.ToInt32(drSelectedCondition[Constants.RowID].ToString());
                cboAllFieldsForCondition.SelectedValue = drSelectedCondition[Constants.ConditionField.SPFieldName].ToString();
                cboConditionOperator.SelectedValue = drSelectedCondition[Constants.ConditionField.SPFieldOperatorID].ToString();
                txtValue.Text = drSelectedCondition[Constants.ConditionField.Value].ToString();
                lnkAddCondition.Text = "Update";
            }
            else if (e.CommandName == "DeleteCondition" && drSelectedCondition != null)
            {
                drSelectedCondition.Delete();
                ValidationConditionDataTable = validationConditionDataTable;

                gvCondition.DataSource = validationConditionDataTable;
                gvCondition.DataBind();

                clearConditionFields();
                lnkAddCondition.Text = "Add";
            }
        }

        protected void cmdOK_Click(object sender, EventArgs e)
        {
            createTabXML();

            // Below to redirect to the list root folder
            //SPUtility.Redirect(SPContext.Current.List.RootFolder.ServerRelativeUrl, SPRedirectFlags.Default, HttpContext.Current);


            SPUtility.Redirect(Helper.GetListSettingsURL(SPContext.Current.List), SPRedirectFlags.Default, HttpContext.Current);
        }

        protected void Cancel_Click(object sender, EventArgs e)
        {


            SPUtility.Redirect(Helper.GetListSettingsURL(SPContext.Current.List), SPRedirectFlags.Default, HttpContext.Current);


        }

        protected void FillListFields()
        {
            SPSecurity.RunWithElevatedPrivileges(delegate
            {
                cboAllFields.Items.Clear();
                cboAllFieldsForCondition.Items.Clear();
                using (SPSite objSite = new SPSite(SPContext.Current.Web.Url.ToString()))
                {
                    using (SPWeb objWeb = objSite.OpenWeb())
                    {
                        SPList list = objWeb.Lists[new Guid(Request.QueryString["List"])];

                        var orderedFields = Helper.GetOrderedListField(list);

                        foreach (SPField lstField in orderedFields)
                        {
                            ListItem item = new ListItem();
                            item.Text = lstField.Title;
                            item.Value = lstField.InternalName;
                            cboAllFields.Items.Add(item);
                            cboAllFieldsForCondition.Items.Add(item);
                        }
                    }
                }
            });
        }

        private void LoadValidationSettings()
        {
            SPSecurity.RunWithElevatedPrivileges(delegate
            {
                using (SPSite objSite = new SPSite(SPContext.Current.Web.Url))
                {
                    using (SPWeb objWeb = objSite.OpenWeb())
                    {
                        SPList list = objWeb.Lists[new Guid(Request.QueryString["List"])];

                        FieldValidations allFieldsValidation = FieldValidations.LoadFieldValidations(Helper.GetConfigFile(list, Constants.ConfigFile.FieldValidationFile));
                        if (allFieldsValidation != null)
                        {
                            DataTable validationDataTable = createValidationDataTable();
                            DataTable valConditionDataTable = createConditionDataTable();

                            foreach (FieldValidation fieldValidation in allFieldsValidation)
                            {
                                if (!list.Fields.ContainsField(fieldValidation.OnField.SPName)) continue;

                                DataRow drValidation = validationDataTable.NewRow();
                                drValidation[Constants.ValidationField.SPFieldName] = fieldValidation.OnField.SPName;
                                drValidation[Constants.ValidationField.SPFieldDisplayName] =  list.Fields.GetFieldByInternalName(fieldValidation.OnField.SPName).Title;
                                drValidation[Constants.ValidationField.ValidationRuleID] = (int)fieldValidation.Rule;
                                drValidation[Constants.ValidationField.ValidationRuleName] = Enums.DisplayString(fieldValidation.Rule);
                                drValidation[Constants.ValidationField.SPFieldOperatorID] = (int)fieldValidation.ByRuleOperator;
                                drValidation[Constants.ValidationField.SPFieldOperatorName] = Enums.DisplayString(fieldValidation.ByRuleOperator);
                                drValidation[Constants.ValidationField.Value] = fieldValidation.Value;
                                drValidation[Constants.ValidationField.SPPrinciplesOperatorID] = (int)fieldValidation.BySPPrinciplesOperator;
                                drValidation[Constants.ValidationField.SPPrinciplesOperatorName] = Enums.DisplayString(fieldValidation.BySPPrinciplesOperator);
                                drValidation[Constants.ValidationField.ErrorMessage] = fieldValidation.ErrorMsg;
                                drValidation[Constants.ValidationField.SPPrinciples] = fieldValidation.ForSPPrinciples;
                                drValidation[Constants.ValidationField.HasCondition] = fieldValidation.Conditions.Count > 0 ? fieldValidation.Conditions.ConditionsToString(list) : "";
                                validationDataTable.Rows.Add(drValidation);

                                foreach (Condition permCondition in fieldValidation.Conditions)
                                {
                                    if (!list.Fields.ContainsField(permCondition.OnField.SPName)) continue;

                                    DataRow drCondition = valConditionDataTable.NewRow();
                                    drCondition[Constants.ConditionField.ValidationRowID] = drValidation[Constants.RowID];
                                    drCondition[Constants.ConditionField.SPFieldName] = permCondition.OnField.SPName;
                                    drCondition[Constants.ConditionField.SPFieldDisplayName] = list.Fields.GetFieldByInternalName(permCondition.OnField.SPName).Title;
                                    drCondition[Constants.ConditionField.SPFieldOperatorID] = (int)permCondition.ByFieldOperator;
                                    drCondition[Constants.ConditionField.SPFieldOperatorName] = Enums.DisplayString(permCondition.ByFieldOperator);
                                    drCondition[Constants.ConditionField.Value] = permCondition.Value;
                                    valConditionDataTable.Rows.Add(drCondition);
                                }

                                ValidationConditionDataTable = valConditionDataTable;
                            }

                            ValidationDataTable = validationDataTable;
                            gvResult.DataSource = validationDataTable;
                            gvResult.DataBind();

                        }
                    }
                }
            });

        }

        protected void FillOperatorsFor(int ruleId)
        {
            cboRulesOperators.Items.Clear();

            ListItem lst = new ListItem();
            lst.Value = ((int)Enums.Operator.Equal).ToString();
            lst.Text = Enums.DisplayString(Enums.Operator.Equal);
            cboRulesOperators.Items.Add(lst);

            if (ruleId == (int)Enums.ValidationRule.Column)
            {
                lst = new ListItem();
                lst.Value = ((int)Enums.Operator.NotEqual).ToString();
                lst.Text = Enums.DisplayString(Enums.Operator.NotEqual);
                cboRulesOperators.Items.Add(lst);

                lst = new ListItem();
                lst.Value = ((int)Enums.Operator.Contains).ToString();
                lst.Text = Enums.DisplayString(Enums.Operator.Contains);
                cboRulesOperators.Items.Add(lst);

                lst = new ListItem();
                lst.Value = ((int)Enums.Operator.NotContains).ToString();
                lst.Text = Enums.DisplayString(Enums.Operator.NotContains);
                cboRulesOperators.Items.Add(lst);

            }
            else if (ruleId == (int)Enums.ValidationRule.length)
            {

                lst = new ListItem();
                lst.Value = ((int)Enums.Operator.NotEqual).ToString();
                lst.Text = Enums.DisplayString(Enums.Operator.NotEqual);


                lst = new ListItem();
                lst.Value = ((int)Enums.Operator.GreaterThan).ToString();
                lst.Text = Enums.DisplayString(Enums.Operator.GreaterThan);
                cboRulesOperators.Items.Add(lst);

                lst = new ListItem();
                lst.Value = ((int)Enums.Operator.LessThan).ToString();
                lst.Text = Enums.DisplayString(Enums.Operator.LessThan);
                cboRulesOperators.Items.Add(lst);

            }
        }

        private void clearValidationField()
        {
            cboAllFields.ClearSelection();
            cboRule.ClearSelection();
            cboRulesOperators.ClearSelection();
            txtRulesValue.Text = "";
            txtErrorMessage.Text = "";
        }

        protected void clearConditionFields()
        {
            cboAllFieldsForCondition.ClearSelection();
            cboConditionOperator.ClearSelection();
            txtValue.Text = "";
        }

        protected void createTabXML()
        {
            
            FieldValidations allFieldsValidations = new FieldValidations();

            DataTable validationDataTable = ValidationDataTable;
            DataTable validationConditionDataTable = ValidationConditionDataTable;

            foreach (DataRow drValidationRow in validationDataTable.Rows)
            {

                string FieldName = drValidationRow[Constants.ValidationField.SPFieldName].ToString();
                Enums.ValidationRule ValidationRule = (Enums.ValidationRule)(Convert.ToInt32(drValidationRow[Constants.ValidationField.ValidationRuleID].ToString()));
                Enums.Operator FieldOperatorID = (Enums.Operator)(Convert.ToInt32(drValidationRow[Constants.ValidationField.SPFieldOperatorID].ToString()));
                string ValidationValue = drValidationRow[Constants.ValidationField.Value].ToString();
                string ErrorMessage = drValidationRow[Constants.ValidationField.ErrorMessage].ToString();
                Enums.Operator BySPPrincipalOperator = (Enums.Operator)(Convert.ToInt32(drValidationRow[Constants.ValidationField.SPPrinciplesOperatorID].ToString()));
                string ForSPPrinciples = drValidationRow[Constants.ValidationField.SPPrinciples].ToString();

                FieldValidation objValidation = new FieldValidation(new Field(FieldName), ValidationRule, FieldOperatorID, ValidationValue, ErrorMessage, ForSPPrinciples, BySPPrincipalOperator);


                int validationID = Convert.ToInt32(drValidationRow[Constants.RowID]);
                DataTable conditionOfSelectedValidation = Helper.GetViewFromDataTable(validationConditionDataTable, validationID, Constants.ConditionField.ValidationRowID).ToTable();

                if (conditionOfSelectedValidation != null && conditionOfSelectedValidation.Rows.Count > 0)
                {
                    foreach (DataRow drCondition in conditionOfSelectedValidation.Rows)
                    {
                        string OnField2 = drCondition[Constants.ConditionField.SPFieldName].ToString();
                        Enums.Operator ByFieldOperator = (Enums.Operator)Convert.ToInt32(drCondition[Constants.ConditionField.SPFieldOperatorID].ToString());
                        object Value = drCondition[Constants.ConditionField.Value].ToString();
                        objValidation.Conditions.Add(new Condition(new Field(OnField2), ByFieldOperator, Value));
                    }

                }

                allFieldsValidations.Add(objValidation);
            }


            SPSecurity.RunWithElevatedPrivileges(delegate
            {
                using (SPSite objSite = new SPSite(SPContext.Current.Web.Url.ToString()))
                {
                    using (SPWeb objWeb = objSite.OpenWeb())
                    {

                        SPList list = objWeb.Lists[new Guid(Request.QueryString["List"])];
                        objWeb.AllowUnsafeUpdates = true;


                        string xml = allFieldsValidations.ToString();
                        if (allFieldsValidations.Count > 0 && Helper.IsValidXml(xml))
                        {

                            Helper.CreateConfigFile(list, Constants.ConfigFile.FieldValidationFile, xml);
                        }
                        else
                        {
                            Helper.DeleteConfigFile(list, Constants.ConfigFile.FieldValidationFile, xml);
                        }

                        objWeb.AllowUnsafeUpdates = false;
                    }
                }
            });
            
        }

    }
}
