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

namespace ASPL.SharePoint2010.CONTROLTEMPLATES.ASLP.SharePoint2010
{
    public partial class ViewPermissionUSerControl : ASPL.SharePoint2010.CONTROLTEMPLATES.ASLP.SharePoint2010.ASPLBaseUserControl
    {
        protected int ViewID
        {
            get
            {
                if (ViewState["ViewID"] == null)
                    ViewState["ViewID"] = "-1";
                return Convert.ToInt32(ViewState["ViewID"]);
            }
            set
            {
                ViewState["ViewID"] = value;
            }
        }

        protected DataTable ViewDataTable
        {
            get
            {
                if (ViewState["ViewDataTable"] != null)
                    return (DataTable)ViewState["ViewDataTable"];
                else
                    return createViewDataTable();
            }
            set
            {
                ViewState["ViewDataTable"] = value;
            }
        }

        protected DataTable createViewDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(Helper.CreateAutoRowIDColumn());
            dt.Columns.Add(Constants.ViewField.ViewID, typeof(string));
            dt.Columns.Add(Constants.ViewField.View, typeof(string));
            dt.Columns.Add(Constants.ViewField.UserGroup, typeof(string));
            dt.Columns.Add(Constants.ViewField.Permission, typeof(string));
            dt.Columns.Add(Constants.ViewField.IsDefault, typeof(int));
            dt.Columns.Add(Constants.ViewField.IsActionMenu, typeof(bool));
            dt.Columns.Add(Constants.ViewField.IsDataSheet, typeof(bool));
            dt.Columns.Add(Constants.ViewField.IsRssFeed, typeof(bool));
            dt.Columns.Add(Constants.ViewField.IsAlertMe, typeof(bool));
            return dt;

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                FillListViews();
                LoadViewSettings();
            }
        }

        protected void LoadViewSettings()
        {
            SPSecurity.RunWithElevatedPrivileges(delegate
               {
                   using (SPSite objSite = new SPSite(SPContext.Current.Web.Url))
                   {
                       using (SPWeb objWeb = objSite.OpenWeb())
                       {
                           Guid objlistID = new Guid(Request.QueryString["List"].ToString());
                           SPList list = objWeb.Lists[objlistID];

                           Views allViews = Views.LoadViews(Helper.GetConfigFile(list, Constants.ConfigFile.ViewPermissionsFile));

                           if (allViews != null)
                           {
                               DataTable viewDataTable = createViewDataTable();
                               foreach (ViewSetting v in allViews)
                               {
                                   DataRow drView = viewDataTable.NewRow();
                                   drView[Constants.ViewField.ViewID] = v.ID;
                                   drView[Constants.ViewField.View] = v.SPVName;
                                   drView[Constants.ViewField.UserGroup] = v.UserGroup;
                                   drView[Constants.ViewField.Permission] = v.Permission;
                                   drView[Constants.ViewField.IsDefault] = 0;//0?
                                   drView[Constants.ViewField.IsActionMenu] = v.HideActionsMenu;
                                   drView[Constants.ViewField.IsDataSheet] = v.HideAccessItem; //Need to Confirm
                                   drView[Constants.ViewField.IsRssFeed] = v.HideRSSItem;
                                   drView[Constants.ViewField.IsAlertMe] = v.HideAlertItem;
                                   viewDataTable.Rows.Add(drView);
                               }

                               ViewDataTable = viewDataTable;
                               grdView.DataSource = viewDataTable;
                               grdView.DataBind();
                           }

                       }
                   }
               });
        }

        protected void FillListViews()
        {
            Guid objlistID = new Guid(Request.QueryString["List"].ToString());
            using (SPSite objsite = new SPSite(SPContext.Current.Web.Url.ToString()))
            {
                using (SPWeb objwebview = objsite.OpenWeb())
                {
                    SPList objlistview = objwebview.Lists[objlistID];
                    foreach (SPView objView in objlistview.Views)
                    {
                        ListItem item = new ListItem();
                        item.Value = objView.ID.ToString();
                        item.Text = objView.Title;
                        ddlViews.Items.Add(item);

                    }
                }

            }
            ddlViews.SelectedIndex = 0;
        }

        protected void clearfields()
        {
            ddlViews.ClearSelection();
            rdoViewPermission.ClearSelection();
            peSelectUsers.Entities.Clear();
            chkShowActionsMenu.Checked = false;
            chkDisplayOpenWithAccess.Checked = false;
            chkDisplayRSS.Checked = false;
            chkDisplayAlertMe.Checked = false;
        }

        protected void cmdAddView_Click(object sender, EventArgs e)
        {
            //if (ViewID == -1)           //(ViewID == -1) ViewID>0
            //{
            DataTable viewDT = ViewDataTable;
            if (cmdAddView.Text == "Add View Permissions")
            {
                foreach (ListItem objView in ddlViews.Items)
                {
                    if (objView.Selected)
                    {
                        DataRow objDataRow = viewDT.NewRow();
                        objDataRow[Constants.ViewField.View] = objView.Text; //ddlViews.SelectedItem.Text;
                        objDataRow[Constants.ViewField.ViewID] = objView.Value;  //ddlViews.SelectedValue;
                        objDataRow[Constants.ViewField.UserGroup] = string.IsNullOrEmpty(peSelectUsers.CommaSeparatedAccounts) ? "ALL" : peSelectUsers.CommaSeparatedAccounts;
                        objDataRow[Constants.ViewField.Permission] = rdoViewPermission.SelectedValue;

                        objDataRow[Constants.ViewField.IsActionMenu] = (chkShowActionsMenu.Checked == true ? true : false);
                        objDataRow[Constants.ViewField.IsDataSheet] = (chkDisplayOpenWithAccess.Checked == true ? true : false);
                        objDataRow[Constants.ViewField.IsRssFeed] = (chkDisplayRSS.Checked == true ? true : false);
                        objDataRow[Constants.ViewField.IsAlertMe] = (chkDisplayAlertMe.Checked == true ? true : false);
                        viewDT.Rows.Add(objDataRow);
                    }
                }

            }
            else if (cmdAddView.Text == "Update View Permissions")
            {

                DataRow drSelectedRow = Helper.GetRowFromDataTable(ViewDataTable, ViewID);
                if (drSelectedRow != null)
                {
                    drSelectedRow[Constants.ViewField.View] = ddlViews.SelectedItem.Text;
                    drSelectedRow[Constants.ViewField.ViewID] = ddlViews.SelectedValue;
                    drSelectedRow[Constants.ViewField.UserGroup] = string.IsNullOrEmpty(peSelectUsers.CommaSeparatedAccounts) ? "ALL" : peSelectUsers.CommaSeparatedAccounts;
                    drSelectedRow[Constants.ViewField.Permission] = rdoViewPermission.SelectedValue;

                    drSelectedRow[Constants.ViewField.IsActionMenu] = (chkShowActionsMenu.Checked == true ? true : false);
                    drSelectedRow[Constants.ViewField.IsDataSheet] = (chkDisplayOpenWithAccess.Checked == true ? true : false);
                    drSelectedRow[Constants.ViewField.IsRssFeed] = (chkDisplayRSS.Checked == true ? true : false);
                    drSelectedRow[Constants.ViewField.IsAlertMe] = (chkDisplayAlertMe.Checked == true ? true : false);
                }
                cmdAddView.Text = "Add View Permissions";
            }
            ViewDataTable = viewDT;
            grdView.DataSource = ViewDataTable;// Helper.GetViewFromDataTable(validationConditionDataTable, ValidationID, Constants.ConditionField.ValidationRowID);
            grdView.DataBind();
            clearfields();
            //}
        }

        protected void grdTabView_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditData")
            {
                // string tab, UserGroup,ColumnGroup;
                //EditItem = 1;
                var results = from myRow in ViewDataTable.AsEnumerable()
                              where myRow.Field<int>("rowid") == Convert.ToInt32(e.CommandArgument.ToString())
                              select myRow;


                DataTable EditValue = results.AsDataView().ToTable();



                if (EditValue.Rows.Count == 1)
                {
                    System.Collections.ArrayList entityArrayList = new System.Collections.ArrayList();
                    //EditItem = 1;
                    ViewID = Convert.ToInt32(e.CommandArgument.ToString());
                    ddlViews.SelectedValue = EditValue.Rows[0]["ViewID"].ToString();

                    rdoViewPermission.SelectedValue = EditValue.Rows[0]["Permission"].ToString();

                    chkShowActionsMenu.Checked = Convert.ToBoolean(EditValue.Rows[0]["IsActionMenu"].ToString());
                    chkDisplayOpenWithAccess.Checked = Convert.ToBoolean(EditValue.Rows[0]["IsDataSheet"].ToString());
                    chkDisplayRSS.Checked = Convert.ToBoolean(EditValue.Rows[0]["IsRssFeed"].ToString());
                    chkDisplayAlertMe.Checked = Convert.ToBoolean(EditValue.Rows[0]["IsAlertMe"].ToString());

                    string[] users = EditValue.Rows[0]["UserGroup"].ToString().Split(',');

                    using (SPSite objSite = new SPSite(SPContext.Current.Web.Url.ToString()))
                    {
                        using (SPWeb objWeb = objSite.OpenWeb())
                        {
                            foreach (string user in users)
                            {
                                SPUser Tabuser = null;
                                if (user != "")
                                {
                                    try
                                    {
                                        Tabuser = objWeb.EnsureUser(user);
                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                    if (Tabuser == null)
                                    {

                                        foreach (SPGroup spg in objWeb.SiteGroups)
                                        {
                                            if (spg.Name.CompareTo(user) == 0)
                                            {
                                                PickerEntity entity = new PickerEntity();
                                                entity.Key = spg.LoginName;
                                                entityArrayList.Add(entity);
                                            }


                                        }
                                    }
                                    else
                                    {
                                        PickerEntity entity = new PickerEntity();
                                        entity.Key = Tabuser.LoginName;
                                        entityArrayList.Add(entity);
                                    }
                                }
                            }
                            if (entityArrayList.Count > 0)
                                peSelectUsers.UpdateEntities(entityArrayList);
                        }
                    }

                    cmdAddView.Text = "Update View Permissions";
                }
            }
        }

        protected void grdTabView_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var results = from myRow in ViewDataTable.AsEnumerable()
                          where myRow.Field<int>("rowid") == Convert.ToInt32(e.RowIndex.ToString())
                          select myRow;

            ViewDataTable.Rows[e.RowIndex].Delete();
            ViewDataTable.AcceptChanges();
            int count = 0;
            foreach (DataRow row in ViewDataTable.Rows)
            {
                row["RowID"] = count++;
                //  row["ID"] = Convert.ToInt32(row["RowID"].ToString()) + 1;
            }
            ViewDataTable.AcceptChanges();
            grdView.DataSource = ViewDataTable;
            grdView.DataBind();

        }

        protected void cmdOK_Click(object sender, EventArgs e)
        {
            createTabXML();
            SPUtility.Redirect(Helper.GetListSettingsURL(SPContext.Current.List), SPRedirectFlags.Default, HttpContext.Current);
        }

        protected void Cancel_Click(object sender, EventArgs e)
        {
            SPUtility.Redirect(Helper.GetListSettingsURL(SPContext.Current.List), SPRedirectFlags.Default, HttpContext.Current);
        }

        protected void createTabXML()
        {
            Views allViews = new Views();
            DataTable viewtable = ViewDataTable;
            foreach (DataRow drViewRow in viewtable.Rows)
            {
                string ViewID = drViewRow[Constants.ViewField.ViewID].ToString();
                string View = drViewRow[Constants.ViewField.View].ToString();
                string UserGroup = drViewRow[Constants.ViewField.UserGroup].ToString();
                string Permission = drViewRow[Constants.ViewField.Permission].ToString();
                bool IsActionMenu = Convert.ToBoolean(drViewRow[Constants.ViewField.IsActionMenu].ToString());
                bool IsDataSheet = Convert.ToBoolean(drViewRow[Constants.ViewField.IsDataSheet].ToString());
                bool IsRssFeed = Convert.ToBoolean(drViewRow[Constants.ViewField.IsRssFeed].ToString());
                bool IsAlertMe = Convert.ToBoolean(drViewRow[Constants.ViewField.IsAlertMe].ToString());
                int ViewRowID = Convert.ToInt32(drViewRow[Constants.RowID]);
                ViewSetting v = new ViewSetting(ViewID, ViewRowID, View, UserGroup, Permission, IsActionMenu, IsDataSheet, IsRssFeed, IsAlertMe);
                allViews.Add(v);
            }


            SPSecurity.RunWithElevatedPrivileges(delegate
            {
                using (SPSite objSite = new SPSite(SPContext.Current.Web.Url.ToString()))
                {
                    using (SPWeb objWeb = objSite.OpenWeb())
                    {
                        objWeb.AllowUnsafeUpdates = true;

                        SPList list = objWeb.Lists[new Guid(Request.QueryString["List"])];

                        string xml = allViews.ToString();
                        if (allViews.Count > 0 && Helper.IsValidXml(xml))
                        {

                            Helper.CreateConfigFile(list, Constants.ConfigFile.ViewPermissionsFile, xml);
                        }
                        else
                        {
                            Helper.DeleteConfigFile(list, Constants.ConfigFile.ViewPermissionsFile, xml);
                        }

                        objWeb.AllowUnsafeUpdates = false;
                    }
                }
            });

        }
      
    }
}
