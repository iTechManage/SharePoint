using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Data;
using ASPL.Blocks;
using Microsoft.SharePoint;
using ASPL.ConfigModel;
using Microsoft.SharePoint.Utilities;
using System.Web;

namespace AdvanceSharepointListPro.CONTROLTEMPLATES
{
    public partial class FieldPermissionSettings : ASPL.SharePoint2010.CONTROLTEMPLATES.ASLP.SharePoint2010.ASPLBaseUserControl
    {
        #region ViewState properties

        protected string FieldName
        {
            get
            {
                if (ViewState["FieldName"] == null)
                    ViewState["FieldName"] = "";
                return ViewState["FieldName"].ToString();
            }
            set
            {
                ViewState["FieldName"] = value;
            }
        }

        protected int FieldPermissionID
        {
            get
            {
                if (ViewState["TPermissionID"] == null)
                    ViewState["TPermissionID"] = "0";
                return Convert.ToInt32(ViewState["TPermissionID"]);
            }
            set
            {
                ViewState["TPermissionID"] = value;
            }
        }

        protected int FieldPermissionConditionID
        {
            get
            {
                if (ViewState["TPermissionConditionID"] == null)
                    ViewState["TPermissionConditionID"] = "0";
                return Convert.ToInt32(ViewState["TPermissionConditionID"]);
            }
            set
            {
                ViewState["TPermissionConditionID"] = value;
            }
        }

        protected DataTable TabPermissionDataTable
        {
            get
            {
                if (ViewState["TabPermissionDataTable"] != null)
                    return (DataTable)ViewState["TabPermissionDataTable"];
                else
                    return createPermissionDataTable();
            }
            set
            {
                ViewState["TabPermissionDataTable"] = value;
            }
        }

        protected DataTable PermissionConditionDataTable
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
            dt.Columns.Add(Constants.ConditionField.PermissionRowID, typeof(int));
            dt.Columns.Add(Constants.ConditionField.SPFieldName, typeof(string));
            dt.Columns.Add(Constants.ConditionField.SPFieldDisplayName, typeof(string));
            dt.Columns.Add(Constants.ConditionField.SPFieldOperatorName, typeof(string));
            dt.Columns.Add(Constants.ConditionField.SPFieldOperatorID, typeof(int));
            dt.Columns.Add(Constants.ConditionField.Value, typeof(string));
            return dt;
        }

        protected DataTable createPermissionDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(Helper.CreateAutoRowIDColumn());
            dt.Columns.Add(Constants.PermissionField.SPFieldName, typeof(string));
            dt.Columns.Add(Constants.PermissionField.SPFieldDisplayName, typeof(string));
            dt.Columns.Add(Constants.PermissionField.IsDefault, typeof(string));
            dt.Columns.Add(Constants.PermissionField.PermissionID, typeof(int));
            dt.Columns.Add(Constants.PermissionField.PermissionName, typeof(string));
            dt.Columns.Add(Constants.PermissionField.SPPrinciples, typeof(string));
            dt.Columns.Add(Constants.PermissionField.SPPrinciplesOperatorID, typeof(int));
            dt.Columns.Add(Constants.PermissionField.SPPrinciplesOperatorName, typeof(string));
            dt.Columns.Add(Constants.PermissionField.OnFormNames, typeof(string));
            dt.Columns.Add(Constants.PermissionField.OnFormIDs, typeof(string));
            dt.Columns.Add(Constants.PermissionField.HasCondition, typeof(string));

            return dt;

        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
           

            if (!Page.IsPostBack)
            {
                FillListFields();
                LoadFieldSettings();
            }
        }

        protected void cmdCreateNewRule_Click(object sender, EventArgs e)
        {
            DataTable permissionDataTable = TabPermissionDataTable;
            DataTable permissionConditionDataTable = PermissionConditionDataTable;

            if (cmdCreateNewRule.Text == "Add New Permission Rule")
            {
                DataRow drPermission = permissionDataTable.NewRow();
                // Adding the tab row id for reference
                drPermission[Constants.PermissionField.SPFieldName] = cboFields.SelectedValue;
                drPermission[Constants.PermissionField.SPFieldDisplayName] = cboFields.SelectedItem.Text;
                drPermission[Constants.PermissionField.IsDefault] = false;// for time being
                drPermission[Constants.PermissionField.PermissionID] = Convert.ToInt32(cdoPermissionLevel.SelectedValue);
                drPermission[Constants.PermissionField.PermissionName] = Enums.DisplayString((Enums.PermissionLevel)Convert.ToInt32(cdoPermissionLevel.SelectedValue));
                drPermission[Constants.PermissionField.SPPrinciples] = (string.IsNullOrEmpty(peSelectUsers.CommaSeparatedAccounts) ? Constants.AllSPPrinciples : peSelectUsers.CommaSeparatedAccounts);
                drPermission[Constants.PermissionField.SPPrinciplesOperatorID] = (string.IsNullOrEmpty(peSelectUsers.CommaSeparatedAccounts) ? 0 : Convert.ToInt32(cboSPPrinciplesOperator.SelectedValue));
                drPermission[Constants.PermissionField.SPPrinciplesOperatorName] = (string.IsNullOrEmpty(peSelectUsers.CommaSeparatedAccounts) ? Enums.DisplayString((Enums.Operator)0) : Enums.DisplayString((Enums.Operator)Convert.ToInt32(cboSPPrinciplesOperator.SelectedValue)) + ":");

                // this is a critical method: keep it in synch with Permission.FormsToString() and Permission.FormsIdToString()
                string strForms = "", strFormsID = "";
                foreach (ListItem li in chkPages.Items)
                {
                    if (li.Selected)
                    {
                        strForms += Enums.DisplayString(((Enums.SPForms)Convert.ToInt32(li.Value))) + Constants.EnumValueSeparator;
                        strFormsID += li.Value + Constants.EnumValueSeparator;
                    }
                }

                strFormsID = strFormsID.TrimEnd(Constants.EnumValueSeparator.ToCharArray());
                strForms = strForms.TrimEnd(Constants.EnumValueSeparator.ToCharArray());
                drPermission[Constants.PermissionField.OnFormIDs] = strFormsID;
                drPermission[Constants.PermissionField.OnFormNames] = strForms;
                drPermission[Constants.PermissionField.HasCondition] = "";
                permissionDataTable.Rows.Add(drPermission);
            }
            else if (cmdCreateNewRule.Text == "Update Permission Rule")
            {
                DataRow drSelectedPermission = Helper.GetRowFromDataTable(permissionDataTable, FieldPermissionID);

                if (drSelectedPermission != null)
                {
                    drSelectedPermission[Constants.PermissionField.SPFieldName] = cboFields.SelectedValue;
                    drSelectedPermission[Constants.PermissionField.SPFieldDisplayName] = cboFields.SelectedItem.Text;
                    drSelectedPermission[Constants.PermissionField.IsDefault] = false;// for time being
                    drSelectedPermission[Constants.PermissionField.PermissionID] = Convert.ToInt32(cdoPermissionLevel.SelectedValue);
                    drSelectedPermission[Constants.PermissionField.PermissionName] = Enums.DisplayString((Enums.PermissionLevel)Convert.ToInt32(cdoPermissionLevel.SelectedValue));
                    drSelectedPermission[Constants.PermissionField.SPPrinciples] = (string.IsNullOrEmpty(peSelectUsers.CommaSeparatedAccounts) ? Constants.AllSPPrinciples : peSelectUsers.CommaSeparatedAccounts);
                    drSelectedPermission[Constants.PermissionField.SPPrinciplesOperatorID] = (string.IsNullOrEmpty(peSelectUsers.CommaSeparatedAccounts) ? 0 : Convert.ToInt32(cboSPPrinciplesOperator.SelectedValue));
                    drSelectedPermission[Constants.PermissionField.SPPrinciplesOperatorName] = (string.IsNullOrEmpty(peSelectUsers.CommaSeparatedAccounts) ? Enums.DisplayString((Enums.Operator)0) : Enums.DisplayString((Enums.Operator)Convert.ToInt32(cboSPPrinciplesOperator.SelectedValue)) + ":");

                    // this is a critical method: keep it in synch with Permission.FormsToString() and Permission.FormsIdToString()
                    string strForms = "", strFormsID = "";
                    foreach (ListItem li in chkPages.Items)
                    {
                        if (li.Selected)
                        {
                            strForms += Enums.DisplayString(((Enums.SPForms)Convert.ToInt32(li.Value))) + Constants.EnumValueSeparator;
                            strFormsID += li.Value + Constants.EnumValueSeparator;
                        }
                    }

                    strFormsID = strFormsID.TrimEnd(Constants.EnumValueSeparator.ToCharArray());
                    strForms = strForms.TrimEnd(Constants.EnumValueSeparator.ToCharArray());
                    drSelectedPermission[Constants.PermissionField.OnFormIDs] = strFormsID;
                    drSelectedPermission[Constants.PermissionField.OnFormNames] = strForms;

                    drSelectedPermission[Constants.PermissionField.HasCondition] = Helper.ConditionsToString(permissionConditionDataTable, FieldPermissionID, Constants.ConditionField.PermissionRowID);

                }
                cmdCreateNewRule.Text = "Add New Permission Rule";
                lnkAddCondition.Enabled = false;
            }

            TabPermissionDataTable = permissionDataTable;

            gvPermission.DataSource = permissionDataTable;
            gvPermission.DataBind();

            gvCondition.DataSource = null;
            gvCondition.DataBind();

            clearPermissionFields();
        }

        protected void gvPermission_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            DataTable permissionDataTable = TabPermissionDataTable;
            DataTable permissionConditionDataTable = PermissionConditionDataTable;
            DataRow drSelectedPermission = Helper.GetRowFromDataTable(permissionDataTable, Convert.ToInt32(e.CommandArgument));

            if (e.CommandName == "EditPermission" && drSelectedPermission != null)
            {
                DataTable conditionOfSelectedPermission = Helper.GetViewFromDataTable(permissionConditionDataTable, Convert.ToInt32(drSelectedPermission[Constants.RowID]), Constants.ConditionField.PermissionRowID).ToTable();
                gvCondition.DataSource = conditionOfSelectedPermission;
                gvCondition.DataBind();

                FieldPermissionID = Convert.ToInt32(drSelectedPermission[Constants.RowID].ToString());
                cboFields.SelectedValue = drSelectedPermission[Constants.PermissionField.SPFieldName].ToString();
                cdoPermissionLevel.SelectedValue = drSelectedPermission[Constants.PermissionField.PermissionID].ToString();
                string formIDs = drSelectedPermission[Constants.PermissionField.OnFormIDs].ToString();

                foreach (string formID in formIDs.Split(Constants.EnumValueSeparator.ToCharArray(),StringSplitOptions.RemoveEmptyEntries))
                {
                    ListItem li = chkPages.Items.FindByValue(formID);
                    if (li != null) li.Selected = true;
                }

                peSelectUsers.CommaSeparatedAccounts = (drSelectedPermission[Constants.PermissionField.SPPrinciples].ToString() == Constants.AllSPPrinciples ? "" : drSelectedPermission[Constants.PermissionField.SPPrinciples].ToString());
                cboSPPrinciplesOperator.SelectedValue = drSelectedPermission[Constants.PermissionField.SPPrinciplesOperatorID].ToString();

                cmdCreateNewRule.Text = "Update Permission Rule";
                lnkAddCondition.Enabled = true;

                gvCondition.DataSource = Helper.GetViewFromDataTable(permissionConditionDataTable, FieldPermissionID, Constants.ConditionField.PermissionRowID);
                gvCondition.DataBind();



            }
            else if (e.CommandName == "DeletePermission" && drSelectedPermission != null)
            {
                if (drSelectedPermission != null)
                {

                    foreach (DataRow drConditions in permissionConditionDataTable.Select(Constants.ConditionField.PermissionRowID + "=" + drSelectedPermission[Constants.RowID].ToString()))
                        drConditions.Delete();

                    drSelectedPermission.Delete();
                    TabPermissionDataTable = permissionDataTable;

                    gvPermission.DataSource = permissionDataTable;
                    gvPermission.DataBind();

                    gvCondition.DataSource = null;
                    gvCondition.DataBind();

                    cmdCreateNewRule.Text = "Add New Permission Rule";

                }
            }
        }

        protected void lnkAddCondition_Click(object sender, EventArgs e)
        {
            if (FieldPermissionID > 0)
            {
                DataTable permissionDataTable = TabPermissionDataTable;
                DataTable permissionConditionDataTable = PermissionConditionDataTable;

                if (lnkAddCondition.Text == "Add")
                {

                    DataRow drCondition = permissionConditionDataTable.NewRow();
                    drCondition[Constants.ConditionField.PermissionRowID] = FieldPermissionID;
                    drCondition[Constants.ConditionField.SPFieldName] = cboAllFields.SelectedValue;
                    drCondition[Constants.ConditionField.SPFieldDisplayName] = cboAllFields.SelectedItem.Text;
                    drCondition[Constants.ConditionField.SPFieldOperatorName] = Enums.DisplayString((Enums.Operator)Convert.ToInt32(cboConditionOperator.SelectedValue));
                    drCondition[Constants.ConditionField.SPFieldOperatorID] = cboConditionOperator.SelectedValue;
                    drCondition[Constants.ConditionField.Value] = Helper.ReplaceInvalidChar(txtValue.Text);
                    permissionConditionDataTable.Rows.Add(drCondition);
                }
                else if (lnkAddCondition.Text == "Update")
                {
                    DataRow drSelectedCondition = Helper.GetRowFromDataTable(PermissionConditionDataTable, FieldPermissionConditionID);

                    if (drSelectedCondition != null)
                    {
                        drSelectedCondition[Constants.ConditionField.SPFieldName] = cboAllFields.SelectedValue;
                        drSelectedCondition[Constants.ConditionField.SPFieldDisplayName] = cboAllFields.SelectedItem.Text;
                        drSelectedCondition[Constants.ConditionField.SPFieldOperatorName] = Enums.DisplayString((Enums.Operator)Convert.ToInt32(cboConditionOperator.SelectedValue));
                        drSelectedCondition[Constants.ConditionField.SPFieldOperatorID] = cboConditionOperator.SelectedValue;
                        drSelectedCondition[Constants.ConditionField.Value] = Helper.ReplaceInvalidChar(txtValue.Text);
                    }

                    lnkAddCondition.Text = "Add";
                }


                PermissionConditionDataTable = permissionConditionDataTable;
                gvCondition.DataSource = Helper.GetViewFromDataTable(permissionConditionDataTable, FieldPermissionID, Constants.ConditionField.PermissionRowID);
                gvCondition.DataBind();
            }
            else
            {
                lblErrorCondition.Text = "please select Permission to add condition";
            }

            clearConditionFields();
        }

        protected void gvCondition_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            DataTable permissionConditionDataTable = PermissionConditionDataTable;
            DataRow drSelectedCondition = Helper.GetRowFromDataTable(permissionConditionDataTable, Convert.ToInt32(e.CommandArgument));

            if (e.CommandName == "EditCondition" && drSelectedCondition != null)
            {
                FieldPermissionConditionID = Convert.ToInt32(drSelectedCondition[Constants.RowID].ToString());
                cboAllFields.SelectedValue = drSelectedCondition[Constants.ConditionField.SPFieldName].ToString();
                cboConditionOperator.SelectedValue = drSelectedCondition[Constants.ConditionField.SPFieldOperatorID].ToString();
                txtValue.Text = drSelectedCondition[Constants.ConditionField.Value].ToString();
                lnkAddCondition.Text = "Update";
            }
            else if (e.CommandName == "DeleteCondition" && drSelectedCondition != null)
            {
                drSelectedCondition.Delete();
                PermissionConditionDataTable = permissionConditionDataTable;

                gvCondition.DataSource = permissionConditionDataTable;
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
                cboFields.Items.Clear(); cboAllFields.Items.Clear();
                using (SPSite objSite = new SPSite(SPContext.Current.List.ParentWeb.Url.ToString()))
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
                            cboFields.Items.Add(item);
                        }
                    }


                }
            });
        }

        protected void LoadFieldSettings()
        {
            SPSecurity.RunWithElevatedPrivileges(delegate
            {
                using (SPSite objSite = new SPSite(SPContext.Current.Web.Url))
                {
                    using (SPWeb objWeb = objSite.OpenWeb())
                    {
                        SPList list = objWeb.Lists[new Guid(Request.QueryString["List"])];

                        FieldPermissions allFieldPermissions = FieldPermissions.LoadFieldPermissions(Helper.GetConfigFile(list, Constants.ConfigFile.FieldPermissionFile));

                        if (allFieldPermissions != null)
                        {
                            DataTable permissionDataTable = createPermissionDataTable();
                            DataTable permissionConditionDataTable = createConditionDataTable();

                            foreach (FieldPermission fp in allFieldPermissions)
                            {

                                if (!list.Fields.ContainsField(fp.OnField.SPName)) continue;

                                DataRow drPermission = permissionDataTable.NewRow();
                                // Adding the tab row id for reference
                                drPermission[Constants.PermissionField.SPFieldName] = fp.OnField.SPName;
                                drPermission[Constants.PermissionField.SPFieldDisplayName] =  list.Fields.GetFieldByInternalName(fp.OnField.SPName).Title;
                                drPermission[Constants.PermissionField.PermissionID] = (int)fp.Level;
                                drPermission[Constants.PermissionField.PermissionName] = Enums.DisplayString(fp.Level);
                                drPermission[Constants.PermissionField.SPPrinciples] = fp.ForSPPrinciples;
                                drPermission[Constants.PermissionField.SPPrinciplesOperatorID] = (int)fp.BySPPrinciplesOperator;
                                drPermission[Constants.PermissionField.SPPrinciplesOperatorName] = Enums.DisplayString(fp.BySPPrinciplesOperator);
                                drPermission[Constants.PermissionField.OnFormIDs] = fp.FormsIdToString();
                                drPermission[Constants.PermissionField.OnFormNames] = fp.FormsToString();
                                drPermission[Constants.PermissionField.HasCondition] = fp.Conditions.ConditionsToString(list);
                                permissionDataTable.Rows.Add(drPermission);


                                foreach (Condition permCondition in fp.Conditions)
                                {
                                    if (!list.Fields.ContainsField(permCondition.OnField.SPName)) continue;

                                    DataRow drCondition = permissionConditionDataTable.NewRow();
                                    drCondition[Constants.ConditionField.PermissionRowID] = drPermission[Constants.RowID];
                                    drCondition[Constants.ConditionField.SPFieldName] = permCondition.OnField.SPName;
                                    drCondition[Constants.ConditionField.SPFieldDisplayName] = list.Fields.GetFieldByInternalName(permCondition.OnField.SPName).Title;
                                    drCondition[Constants.ConditionField.SPFieldOperatorID] = (int)permCondition.ByFieldOperator;
                                    drCondition[Constants.ConditionField.SPFieldOperatorName] = Enums.DisplayString(permCondition.ByFieldOperator);
                                    drCondition[Constants.ConditionField.Value] = permCondition.Value;
                                    permissionConditionDataTable.Rows.Add(drCondition);
                                }

                                PermissionConditionDataTable = permissionConditionDataTable;

                            }

                            TabPermissionDataTable = permissionDataTable;
                            gvPermission.DataSource = permissionDataTable;
                            gvPermission.DataBind();
                        }
                    }
                }
            });
        }

        protected void clearPermissionFields()
        {
            cdoPermissionLevel.ClearSelection();
            chkPages.ClearSelection();
            peSelectUsers.CommaSeparatedAccounts = "";
            cboSPPrinciplesOperator.ClearSelection();
        }

        protected void clearConditionFields()
        {
            cboAllFields.ClearSelection();
            cboConditionOperator.ClearSelection();
            txtValue.Text = "";
        }

        protected void createTabXML()
        {
            FieldPermissions allFieldPermissions = new FieldPermissions();

            DataTable permissionDataTable = TabPermissionDataTable;
            DataTable permissionConditionDataTable = PermissionConditionDataTable;

            foreach (DataRow drPermission in permissionDataTable.Rows)
            {
                string OnField=drPermission[Constants.PermissionField.SPFieldName].ToString();
                bool IsDefault = Helper.ConvertToBool(drPermission[Constants.PermissionField.IsDefault].ToString());
                Enums.PermissionLevel permissionLevel = (Enums.PermissionLevel)(Convert.ToInt32(drPermission[Constants.PermissionField.PermissionID].ToString()));
                string OnForms = drPermission[Constants.PermissionField.OnFormIDs].ToString();
                string ForSPPrinciples = drPermission[Constants.PermissionField.SPPrinciples].ToString();
                Enums.Operator BySPPrinciplesOperator = (Enums.Operator)(Convert.ToInt32(drPermission[Constants.PermissionField.SPPrinciplesOperatorID].ToString()));
                int permissionID = Convert.ToInt32(drPermission[Constants.RowID]);

                FieldPermission perm1 = new FieldPermission(new Field(OnField), permissionLevel, FieldPermission.ParseForms(OnForms), ForSPPrinciples, BySPPrinciplesOperator);


                DataTable conditionOfSelectedPermission = Helper.GetViewFromDataTable(permissionConditionDataTable, permissionID, Constants.ConditionField.PermissionRowID).ToTable();

                if (conditionOfSelectedPermission != null && conditionOfSelectedPermission.Rows.Count > 0)
                {
                    foreach (DataRow drCondition in conditionOfSelectedPermission.Rows)
                    {
                        string OnField2 = drCondition[Constants.ConditionField.SPFieldName].ToString();
                        Enums.Operator ByFieldOperator = (Enums.Operator)Convert.ToInt32(drCondition[Constants.ConditionField.SPFieldOperatorID].ToString());
                        object Value = drCondition[Constants.ConditionField.Value].ToString();

                        perm1.Conditions.Add(new Condition(new Field(OnField2), ByFieldOperator, Value));
                    }

                }

                allFieldPermissions.Add(perm1);
            }


            SPSecurity.RunWithElevatedPrivileges(delegate
            {
                using (SPSite objSite = new SPSite(SPContext.Current.Web.Url.ToString()))
                {
                    using (SPWeb objWeb = objSite.OpenWeb())
                    {
                        SPList list = objWeb.Lists[new Guid(Request.QueryString["List"])];

                        objWeb.AllowUnsafeUpdates = true;

                        string xml = allFieldPermissions.ToString();
                        if (allFieldPermissions.Count > 0 && Helper.IsValidXml(xml))
                        {

                            Helper.CreateConfigFile(list, Constants.ConfigFile.FieldPermissionFile, xml);
                        }
                        else
                        {
                            Helper.DeleteConfigFile(list, Constants.ConfigFile.FieldPermissionFile, xml);
                        }

                        objWeb.AllowUnsafeUpdates = false;
                    }
                }
            });
            
        }
    }



}
