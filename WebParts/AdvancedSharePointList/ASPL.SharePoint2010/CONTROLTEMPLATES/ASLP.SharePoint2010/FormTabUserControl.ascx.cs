using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web.UI;
using System.Linq;
using System.Web.UI.WebControls;
using ASPL.Blocks;
using ASPL.ConfigModel;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.Utilities;
using System.Web;

namespace ASPL.SharePoint2010.CONTROLTEMPLATES
{
    public partial class FormTabUserControl : ASPL.SharePoint2010.CONTROLTEMPLATES.ASLP.SharePoint2010.ASPLBaseUserControl
    {

        #region ViewState properties

        protected int TabID
        {
            get
            {
                if (ViewState["TabID"] == null)
                    ViewState["TabID"] = "-1";
                return Convert.ToInt32(ViewState["TabID"]);
            }
            set
            {
                ViewState["TabID"] = value;
            }
        }

        protected int TabPermissionID
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

         protected int TabPermissionConditionID
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

        protected DataTable TabDataTable
        {
            get
            {
                if (ViewState["TabDataTable"] != null)
                    return (DataTable)ViewState["TabDataTable"];
                else
                    return createTabDataTable();
            }
            set
            {
                ViewState["TabDataTable"] = value;
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

        protected DataTable createTabDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(Helper.CreateAutoRowIDColumn());
            dt.Columns.Add(Constants.TabField.Index, typeof(int));
            dt.Columns.Add(Constants.TabField.Title, typeof(string));
            dt.Columns.Add(Constants.TabField.IsDefault, typeof(bool));
            dt.Columns.Add(Constants.TabField.Description, typeof(string));
            dt.Columns.Add(Constants.TabField.FieldToString, typeof(string));
            dt.Columns.Add(Constants.TabField.FieldDisplayNameToString, typeof(string));
            dt.Columns.Add(Constants.TabField.HasPermission, typeof(bool));
            return dt;

        }

        protected DataTable createPermissionDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(Helper.CreateAutoRowIDColumn());
            dt.Columns.Add(Constants.PermissionField.TabRowID, typeof(int));
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
                LoadTabSettings();
            }

            if (TabDataTable.Rows.Count > 1)
            {
                btnRowDown.Visible = true;
                btnRowUP.Visible = true;
            }
            else
            {
                btnRowDown.Visible = false;
                btnRowUP.Visible = false;
            }

            if (!string.IsNullOrEmpty(hdnSelectedColumn.Value))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ShowColumn", "Javascript:showSelectedOPtion();", true);
            }
        }

        protected void btnRowUP_Click(object sender, EventArgs e)
        {
            if (TabID > 0)
            {

                DataTable tabsDataTable = TabDataTable;
                DataRow drSelectedTab = Helper.GetRowFromDataTable(tabsDataTable, TabID);

                if (drSelectedTab != null)
                {

                    int drSelectedTabIndex = tabsDataTable.Rows.IndexOf(drSelectedTab);

                    if (--drSelectedTabIndex < 0)
                    {
                        drSelectedTabIndex = tabsDataTable.Rows.Count - 1;
                    }

                    DataRow drTarget = tabsDataTable.Rows[drSelectedTabIndex];


                    DataRow drTemp=tabsDataTable.NewRow();
                    drTemp.ItemArray = drTarget.ItemArray;

                    drTarget.ItemArray = drSelectedTab.ItemArray;
                    drSelectedTab.ItemArray = drTemp.ItemArray;


                    grdTabView.DataSource = tabsDataTable;
                    grdTabView.DataBind();

                    TabDataTable = tabsDataTable;

                }
                
            }
        }

        protected void btnRowDown_Click(object sender, EventArgs e)
        {
            if (TabID > 0)
            {

                DataTable tabsDataTable = TabDataTable;
                DataRow drSelectedTab = Helper.GetRowFromDataTable(tabsDataTable, TabID);

                if (drSelectedTab != null)
                {

                    int drSelectedTabIndex = tabsDataTable.Rows.IndexOf(drSelectedTab);

                    if (++drSelectedTabIndex > tabsDataTable.Rows.Count - 1)
                    {
                        drSelectedTabIndex = 0;
                    }

                    DataRow drTarget = tabsDataTable.Rows[drSelectedTabIndex];


                    DataRow drTemp = tabsDataTable.NewRow();
                    drTemp.ItemArray = drTarget.ItemArray;

                    drTarget.ItemArray = drSelectedTab.ItemArray;
                    drSelectedTab.ItemArray = drTemp.ItemArray;


                    grdTabView.DataSource = tabsDataTable;
                    grdTabView.DataBind();

                    TabDataTable = tabsDataTable;

                }

            }
        }

        protected void cmdAddTab_Click(object sender, EventArgs e)
        {
            DataTable tabsDataTable = TabDataTable;
            DataTable permissionDataTable = TabPermissionDataTable;
            DataTable permissionConditionDataTable = PermissionConditionDataTable;

            // settings all the tabs undefault if new/edit tabs is set to default
            if (chkSetTabDefault.Checked)
            {
                foreach (DataRow dr in tabsDataTable.Rows)
                {
                    dr[Constants.TabField.IsDefault] = false;
                }
            }

            if (cmdAddTab.Text == "Create") // creating new tab
            {
                DataRow drNewTab = tabsDataTable.NewRow();
                drNewTab[Constants.TabField.Index] = 0;
                drNewTab[Constants.TabField.Title] = Helper.ReplaceInvalidChar(txtNewTab.Text);
                drNewTab[Constants.TabField.IsDefault] = chkSetTabDefault.Checked;
                drNewTab[Constants.TabField.Description] = Helper.ReplaceInvalidChar(txtDescription.Text);
                drNewTab[Constants.TabField.FieldToString] = hdnSelectedColumn.Value;
                drNewTab[Constants.TabField.FieldDisplayNameToString] = Helper.GetFieldDisplayNames(SPContext.Current.List,hdnSelectedColumn.Value);
                drNewTab[Constants.TabField.HasPermission] = false;
                tabsDataTable.Rows.Add(drNewTab);

            }
            else if (cmdAddTab.Text == "Save Changes") // while editing the tab
            {

                DataRow drEditTab = Helper.GetRowFromDataTable(tabsDataTable, TabID);

                if (drEditTab != null)
                {
                    drEditTab[Constants.TabField.Title] = Helper.ReplaceInvalidChar(txtNewTab.Text);
                    drEditTab[Constants.TabField.Description] = Helper.ReplaceInvalidChar(txtDescription.Text);
                    drEditTab[Constants.TabField.FieldDisplayNameToString] = Helper.GetFieldDisplayNames(SPContext.Current.List, hdnSelectedColumn.Value);
                    drEditTab[Constants.TabField.FieldToString] = hdnSelectedColumn.Value;
                    drEditTab[Constants.TabField.IsDefault] = chkSetTabDefault.Checked;
                    drEditTab[Constants.TabField.HasPermission] = Helper.GetViewFromDataTable(permissionDataTable, TabID, Constants.PermissionField.TabRowID).ToTable().Rows.Count > 0; ;
                    TabDataTable.AcceptChanges();


                }
                cmdAddTab.Text = "Create";
                cmdCreateNewRule.Enabled = false;
                btnRowDown.Enabled = false;
                btnRowUP.Enabled = false;
                cmdCreateNewRule.Text = "Add New Permission Rule";
            }

            TabID = -1;

            cmdOK.Enabled = true;

            TabDataTable = tabsDataTable;
            grdTabView.DataSource = tabsDataTable;
            grdTabView.DataBind();

            gvPermission.DataSource = null;
            gvPermission.DataBind();

            gvCondition.DataSource = null;
            gvCondition.DataBind();

            clearTabFields();
        }

        protected void grdTabView_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            DataTable tabsDataTable = TabDataTable;
            DataTable permissionDataTable = TabPermissionDataTable;
            DataTable permissionConditionDataTable = PermissionConditionDataTable;
            DataRow drSelectedTab = Helper.GetRowFromDataTable(tabsDataTable, Convert.ToInt32(e.CommandArgument));

            if (e.CommandName == "EditTab" && drSelectedTab!=null)
            {
                DataTable permissionOfSelectedTab = Helper.GetViewFromDataTable(permissionDataTable, Convert.ToInt32(drSelectedTab[Constants.RowID]), Constants.PermissionField.TabRowID).ToTable();
                gvPermission.DataSource = permissionOfSelectedTab;
                gvPermission.DataBind();

                TabID = Convert.ToInt32(drSelectedTab[Constants.RowID].ToString());
                txtNewTab.Text = drSelectedTab[Constants.TabField.Title].ToString();
                txtDescription.Text = drSelectedTab[Constants.TabField.Description].ToString();
                chkSetTabDefault.Checked = Helper.ConvertToBool(drSelectedTab[Constants.TabField.IsDefault].ToString());
                hdnSelectedColumnDisplayName.Value= drSelectedTab[Constants.TabField.FieldDisplayNameToString].ToString();
                hdnSelectedColumn.Value = drSelectedTab[Constants.TabField.FieldToString].ToString();
                if (!string.IsNullOrEmpty(hdnSelectedColumn.Value))
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ShowColumn", "Javascript:showSelectedOPtion();", true);
                cmdAddTab.Text="Save Changes";
                cmdCreateNewRule.Enabled = true;

                gvPermission.DataSource = Helper.GetViewFromDataTable(permissionDataTable,TabID,Constants.PermissionField.TabRowID);
                gvPermission.DataBind();

                gvCondition.DataSource = null;
                gvCondition.DataBind();

                cmdOK.Enabled = false;
                btnRowDown.Enabled = true;
                btnRowUP.Enabled = true;
                
            }
            else if (e.CommandName == "DeleteTab" && drSelectedTab!=null)
            {


                foreach (DataRow drPermissions in permissionDataTable.Select(Constants.PermissionField.TabRowID+"="+drSelectedTab[Constants.RowID].ToString()))
                {

                    foreach (DataRow drConditions in permissionConditionDataTable.Select(Constants.ConditionField.PermissionRowID + "=" + drPermissions[Constants.RowID].ToString()))
                        drConditions.Delete();

                    drPermissions.Delete();


                }

                drSelectedTab.Delete();
                grdTabView.DataSource = tabsDataTable;
                grdTabView.DataBind();
                TabDataTable = tabsDataTable;

                TabPermissionDataTable = permissionDataTable;
                gvPermission.DataSource = null;
                gvPermission.DataBind();

                PermissionConditionDataTable = permissionConditionDataTable;
                gvCondition.DataSource = null;
                gvCondition.DataBind();

                cmdOK.Enabled = true;
                cmdCreateNewRule.Enabled = false;
                cmdCreateNewRule.Text = "Add New Permission Rule";

            }

           
        }

        protected void cmdCreateNewRule_Click(object sender, EventArgs e)
        {
            if (TabID > 0)
            {
                DataTable permissionDataTable = TabPermissionDataTable;
                DataTable permissionConditionDataTable = PermissionConditionDataTable;

                if (cmdCreateNewRule.Text == "Add New Permission Rule")
                {
                    DataRow drPermission = permissionDataTable.NewRow();
                    // Adding the tab row id for reference
                    drPermission[Constants.PermissionField.TabRowID] = TabID;
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
                    DataRow drSelectedPermission = Helper.GetRowFromDataTable(permissionDataTable, TabPermissionID);

                    if (drSelectedPermission != null)
                    {
                        drSelectedPermission[Constants.PermissionField.IsDefault] = false;// for time being
                        drSelectedPermission[Constants.PermissionField.PermissionID] = Convert.ToInt32(cdoPermissionLevel.SelectedValue);
                        drSelectedPermission[Constants.PermissionField.PermissionName] = Enums.DisplayString((Enums.PermissionLevel)Convert.ToInt32(cdoPermissionLevel.SelectedValue));
                        drSelectedPermission[Constants.PermissionField.SPPrinciples] = (string.IsNullOrEmpty(peSelectUsers.CommaSeparatedAccounts) ? Constants.AllSPPrinciples: peSelectUsers.CommaSeparatedAccounts);
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

                        drSelectedPermission[Constants.PermissionField.HasCondition]=Helper.ConditionsToString(permissionConditionDataTable,TabPermissionID,Constants.ConditionField.PermissionRowID);

                    }
                    cmdCreateNewRule.Text ="Add New Permission Rule";
                    lnkAddCondition.Enabled = false;
                }

                TabPermissionDataTable = permissionDataTable;

                gvPermission.DataSource = Helper.GetViewFromDataTable(permissionDataTable, TabID, Constants.PermissionField.TabRowID); ;
                gvPermission.DataBind();

                gvCondition.DataSource = null;
                gvCondition.DataBind();
            }
            else
            {
                lblError.Text = "Please Select Tab to add permissions";
            }
          
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

                TabPermissionID = Convert.ToInt32(drSelectedPermission[Constants.RowID].ToString());

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

                gvCondition.DataSource = Helper.GetViewFromDataTable(permissionConditionDataTable, TabPermissionID, Constants.ConditionField.PermissionRowID);
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

                    DataTable permissionOfSelectedTab = Helper.GetViewFromDataTable(permissionDataTable, TabID, Constants.PermissionField.TabRowID).ToTable();
                    gvPermission.DataSource = permissionOfSelectedTab;
                    gvPermission.DataBind();

                    gvCondition.DataSource = null;
                    gvCondition.DataBind();

                }
            }
        }

        protected void lnkAddCondition_Click(object sender, EventArgs e)
        {
            if (TabPermissionID > 0)
            {
                DataTable permissionDataTable = TabPermissionDataTable;
                DataTable permissionConditionDataTable = PermissionConditionDataTable;

                if (lnkAddCondition.Text == "Add")
                {

                    DataRow drCondition = permissionConditionDataTable.NewRow();
                    drCondition[Constants.ConditionField.PermissionRowID] = TabPermissionID;
                    drCondition[Constants.ConditionField.SPFieldName] = cboAllFields.SelectedValue;
                    drCondition[Constants.ConditionField.SPFieldDisplayName] = cboAllFields.SelectedItem.Text;
                    drCondition[Constants.ConditionField.SPFieldOperatorName] = Enums.DisplayString((Enums.Operator)Convert.ToInt32(cboConditionOperator.SelectedValue));
                    drCondition[Constants.ConditionField.SPFieldOperatorID] = cboConditionOperator.SelectedValue;
                    drCondition[Constants.ConditionField.Value] = Helper.ReplaceInvalidChar( txtValue.Text);
                    permissionConditionDataTable.Rows.Add(drCondition);
                }
                else if (lnkAddCondition.Text == "Update")
                {
                    DataRow drSelectedCondition = Helper.GetRowFromDataTable(PermissionConditionDataTable, TabPermissionConditionID);

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
                gvCondition.DataSource = Helper.GetViewFromDataTable(permissionConditionDataTable,TabPermissionID,Constants.ConditionField.PermissionRowID);
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
                TabPermissionConditionID = Convert.ToInt32(drSelectedCondition[Constants.RowID].ToString());
                cboAllFields.SelectedValue=drSelectedCondition[Constants.ConditionField.SPFieldName].ToString();
                cboConditionOperator.SelectedValue=drSelectedCondition[Constants.ConditionField.SPFieldOperatorID].ToString();
                txtValue.Text=drSelectedCondition[Constants.ConditionField.Value].ToString();
                lnkAddCondition.Text = "Update";
            }
            else if (e.CommandName == "DeleteCondition" && drSelectedCondition != null)
            {
                drSelectedCondition.Delete();
                PermissionConditionDataTable = permissionConditionDataTable;

                DataTable conditionOfSelectedPerm = Helper.GetViewFromDataTable(permissionConditionDataTable, TabPermissionID, Constants.ConditionField.PermissionRowID).ToTable();
                gvCondition.DataSource = conditionOfSelectedPerm;
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
            SPUtility.Redirect(Helper.GetListSettingsURL(SPContext.Current.List),SPRedirectFlags.Default, HttpContext.Current);   
        }

        protected void FillListFields()
        {
            SPSecurity.RunWithElevatedPrivileges(delegate
            {
                lstAllFields.Items.Clear(); cboAllFields.Items.Clear();
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
                            lstAllFields.Items.Add(item);
                            cboAllFields.Items.Add(item);

                        }

                    }


                }
            });
        }

        protected void LoadTabSettings()
        {
            SPSecurity.RunWithElevatedPrivileges(delegate
            {
                using (SPSite objSite = new SPSite(SPContext.Current.Web.Url))
                {
                    using (SPWeb objWeb = objSite.OpenWeb())
                    {
                        SPList list = objWeb.Lists[new Guid(Request.QueryString["List"])];

                        Tabs allTabs = Tabs.LoadTabs(Helper.GetConfigFile(list, Constants.ConfigFile.TabSettingFile));

                        if (allTabs != null)
                        {
                            DataTable tabsDataTable = createTabDataTable();
                            DataTable permissionDataTable = createPermissionDataTable();
                            DataTable permissionConditionDataTable = createConditionDataTable();

                            foreach (Tab tab in allTabs)
                            {
                                DataRow drTab = tabsDataTable.NewRow();
                                drTab[Constants.TabField.Index] = tab.Index;
                                drTab[Constants.TabField.Title] = tab.Title;
                                drTab[Constants.TabField.IsDefault] = tab.IsSelected;
                                drTab[Constants.TabField.Description] = tab.Description;
                                drTab[Constants.TabField.FieldToString] = tab.CommaSeperatedFields;
                                drTab[Constants.TabField.FieldDisplayNameToString] = Helper.GetFieldDisplayNames(list,tab.CommaSeperatedFields);
                                drTab[Constants.TabField.HasPermission] = (tab.Permissions.Count > 0);
                                tabsDataTable.Rows.Add(drTab);

                                foreach (TabPermission tabPermission in tab.Permissions)
                                {
                                    DataRow drPermission = permissionDataTable.NewRow();
                                    // Adding the tab row id for reference
                                    drPermission[Constants.PermissionField.TabRowID] = drTab[Constants.RowID];
                                    drPermission[Constants.PermissionField.IsDefault] = tabPermission.IsDefault;
                                    drPermission[Constants.PermissionField.PermissionID] = (int)tabPermission.Level;
                                    drPermission[Constants.PermissionField.PermissionName] = Enums.DisplayString(tabPermission.Level);
                                    drPermission[Constants.PermissionField.SPPrinciples] = tabPermission.ForSPPrinciples;
                                    drPermission[Constants.PermissionField.SPPrinciplesOperatorID] = (int)tabPermission.BySPPrinciplesOperator;
                                    drPermission[Constants.PermissionField.SPPrinciplesOperatorName] = Enums.DisplayString(tabPermission.BySPPrinciplesOperator);
                                    drPermission[Constants.PermissionField.OnFormIDs] = tabPermission.FormsIdToString();
                                    drPermission[Constants.PermissionField.OnFormNames] = tabPermission.FormsToString();
                                    drPermission[Constants.PermissionField.HasCondition] = tabPermission.Conditions.ConditionsToString(list);
                                    permissionDataTable.Rows.Add(drPermission);


                                    foreach (Condition permCondition in tabPermission.Conditions)
                                    {
                                        if (!list.Fields.ContainsField(permCondition.OnField.SPName)) continue;

                                        DataRow drCondition = permissionConditionDataTable.NewRow();
                                        drCondition[Constants.ConditionField.PermissionRowID] = drPermission[Constants.RowID];
                                        drCondition[Constants.ConditionField.SPFieldName] = permCondition.OnField.SPName;
                                        drCondition[Constants.ConditionField.SPFieldDisplayName] =  list.Fields.GetFieldByInternalName(permCondition.OnField.SPName).Title;
                                        drCondition[Constants.ConditionField.SPFieldOperatorID] = (int)permCondition.ByFieldOperator;
                                        drCondition[Constants.ConditionField.SPFieldOperatorName] = Enums.DisplayString(permCondition.ByFieldOperator);
                                        drCondition[Constants.ConditionField.Value] = permCondition.Value;
                                        permissionConditionDataTable.Rows.Add(drCondition);
                                    }

                                    PermissionConditionDataTable = permissionConditionDataTable;
                                }

                                TabPermissionDataTable = permissionDataTable;

                            }


                            TabDataTable = tabsDataTable;
                            grdTabView.DataSource = tabsDataTable;
                            grdTabView.DataBind();
                        }
                    }
                }
            });
        }

        protected void clearTabFields()
        {
            txtNewTab.Text = "";
            txtDescription.Text = "";
            lblError.Text = "";
           //txtSelectedValues.Text = "";
            hdnSelectedColumn.Value = string.Empty;
            hdnSelectedColumnDisplayName.Value = string.Empty;
            chkSetTabDefault.Checked = false;
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
            bool isDefault = false;
            Tabs allTabs = new Tabs();

            DataTable tabsDataTable = TabDataTable;
            DataTable permissionDataTable = TabPermissionDataTable;
            DataTable permissionConditionDataTable = PermissionConditionDataTable;

            foreach (DataRow drTab in tabsDataTable.Rows)
            {
                ushort index = Convert.ToUInt16(drTab[Constants.TabField.Index]);
                string title =  drTab[Constants.TabField.Title].ToString();
                string desc =  drTab[Constants.TabField.Description].ToString();
                bool isTabDefault = Helper.ConvertToBool( drTab[Constants.TabField.IsDefault].ToString());
                int tabID = Convert.ToInt32(drTab[Constants.RowID]);

                Tab t1 = new Tab(index, title, desc);
                t1.CommaSeperatedFields = drTab[Constants.TabField.FieldToString].ToString();
                t1.IsSelected = isTabDefault;

                DataTable permissionOfSelectedTab = Helper.GetViewFromDataTable(permissionDataTable, tabID, Constants.PermissionField.TabRowID).ToTable();

                if (permissionOfSelectedTab != null && permissionOfSelectedTab.Rows.Count > 0)
                {
                    foreach (DataRow drPermission in permissionOfSelectedTab.Rows)
                    {
                        bool IsDefault = Helper.ConvertToBool(drPermission[Constants.PermissionField.IsDefault].ToString());
                        Enums.PermissionLevel permissionLevel = (Enums.PermissionLevel)(Convert.ToInt32(drPermission[Constants.PermissionField.PermissionID].ToString()));
                        string OnForms = drPermission[Constants.PermissionField.OnFormIDs].ToString();
                        string ForSPPrinciples = drPermission[Constants.PermissionField.SPPrinciples].ToString();
                        Enums.Operator BySPPrinciplesOperator = (Enums.Operator)(Convert.ToInt32(drPermission[Constants.PermissionField.SPPrinciplesOperatorID].ToString()));
                        int permissionID = Convert.ToInt32(drPermission[Constants.RowID]);

                        TabPermission perm1 = new TabPermission(isDefault, permissionLevel, TabPermission.ParseForms(OnForms), ForSPPrinciples, BySPPrinciplesOperator);


                        DataTable conditionOfSelectedPermission = Helper.GetViewFromDataTable(permissionConditionDataTable, permissionID, Constants.ConditionField.PermissionRowID).ToTable();

                        if (conditionOfSelectedPermission != null && conditionOfSelectedPermission.Rows.Count > 0)
                        {
                            foreach (DataRow drCondition in conditionOfSelectedPermission.Rows)
                            {
                                string OnField = drCondition[Constants.ConditionField.SPFieldName].ToString();
                                Enums.Operator ByFieldOperator = (Enums.Operator)Convert.ToInt32(drCondition[Constants.ConditionField.SPFieldOperatorID].ToString());
                                object Value = drCondition[Constants.ConditionField.Value].ToString();

                                perm1.Conditions.Add(new Condition(new Field(OnField), ByFieldOperator, Value));
                            }

                        }

                        t1.Permissions.Add(perm1);
                    }
                }

                allTabs.Add(t1);

            }

            SPSecurity.RunWithElevatedPrivileges(delegate
            {
                using (SPSite objSite = new SPSite(SPContext.Current.Web.Url.ToString()))
                {
                    using (SPWeb objWeb = objSite.OpenWeb())
                    {
                        SPList list = objWeb.Lists[new Guid(Request.QueryString["List"])];

                        objWeb.AllowUnsafeUpdates = true;
                        string xml = allTabs.ToString();
                        if (allTabs.Count > 0 && Helper.IsValidXml(xml))
                        {

                            Helper.CreateConfigFile(list, Constants.ConfigFile.TabSettingFile, xml);
                        }
                        else
                        {
                            Helper.DeleteConfigFile(list, Constants.ConfigFile.TabSettingFile, xml);
                        }

                        objWeb.AllowUnsafeUpdates = false;
                    }
                }
            });
                
        }

    }
}
