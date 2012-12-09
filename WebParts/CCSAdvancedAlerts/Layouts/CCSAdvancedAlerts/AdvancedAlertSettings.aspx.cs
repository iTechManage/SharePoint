using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Web.UI.WebControls;

namespace CCSAdvancedAlerts.Layouts.CCSAdvancedAlerts
{


    public partial class AdvancedAlertSettings : LayoutsPageBase
    {
        private const string alertSettingsListName = "CCSAdvancedAlertsList";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                PopulateSites();
            }

            //Alert based events
            this.btnAlertsave.Click += new EventHandler(btnAlertsave_Click);
            this.ddlSite.SelectedIndexChanged += new EventHandler(ddlSite_SelectedIndexChanged);
            this.ddlList.SelectedIndexChanged += new EventHandler(ddlList_SelectedIndexChanged);
            
            //Recipient related
            this.btnAddTO.Click += new EventHandler(btnAddTO_Click);
            this.btnAddCC.Click += new EventHandler(btnAddCC_Click);
            this.btnAddBCC.Click += new EventHandler(btnAddBCC_Click);


            //Template related
            this.btnAddToSubject.Click +=new EventHandler(btnAddToSubject_Click);
            this.btnCopyToClipBoard.Click += new EventHandler(btnCopyToClipBoard_Click);

            this.btnTemplateAdd.Click  +=new EventHandler(btnTemplateAdd_Click);
            this.btnTemplateUpdate.Click +=new EventHandler(btnTemplateUpdate_Click);
            this.btnTemplateCancel.Click += new EventHandler(btnTemplateCancel_Click);

        
        }

        
        #region Aletr related events

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

        private void PopulateSites()
        {
            try
            {
                SPSite site = SPContext.Current.Site;
                if (site != null)
                {
                    SPWebCollection allWebs = site.AllWebs;
                    foreach (SPWeb web in allWebs)
                    {
                        ListItem newWebItem = new ListItem(web.Title, web.ID.ToString());
                        if (!this.ddlSite.Items.Contains(newWebItem))
                        {
                            this.ddlSite.Items.Add(newWebItem);
                        }
 
                    }

                    this.PopulateLists(this.ddlSite.SelectedValue);
                }

            }
            catch 
            {
            }
        }

        private void PopulateLists(string webid)
        {
            try
            {
                SPListCollection allLists = SPContext.Current.Site.AllWebs[new Guid(webid)].Lists;
                if (allLists != null)
                {
                    foreach (SPList  list in allLists)
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

        void ddlList_SelectedIndexChanged(object sender, EventArgs e)
        {
          
            //rdUsersincolumn
            ListChanged();
        }

        void ListChanged()
        {
            try
            {
                SPList list = SPContext.Current.Site.AllWebs[new Guid(this.ddlSite.SelectedValue)].Lists[new Guid(ddlList.SelectedValue)];

                if (list != null)
                {
                    foreach (SPField field in list.Fields)
                    {
                        if (field.Type == SPFieldType.User)
                        {
                            ddlUsersInColumn.Items.Add(field.Title);
                        }

                        lstPlaceHolders.Items.Add(field.Title);
                    }

                }
            }
            catch
            {
            }
        }

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

                }
                else if (rdEmailAddresses.Checked)
                {
                    emailAddresses = emailAddresses + "," + txtEmailAddresses;
                }

                txtAddressBox.Text += emailAddresses; 
            }
        }

        void btnAlertsave_Click(object sender, EventArgs e)
        {
            try
            {
                /// Basic information we are saving for Alert in Alert listing List
                //Title  Single line of text  
                //WebID  Single line of text  
                //ListID  Single line of text  
                //ItemID  Single line of text  
                //WhenToSend  Choice  
                //DetailInfo  Multiple lines of text  
                //Owner  Person or Group  
                //EventType  Choice 

                Alert alert = new Alert();

                //Get the General Information
                alert.Title = txtTitle.Text;
                alert.WebId = ddlSite.SelectedValue;
                alert.listId = ddlList.SelectedValue;

                
                //Get Recipient Section
                alert.ToAddress = txtTo.Text;
                alert.FromAdderss = txtFrom.Text;
                alert.CcAddress = txtCc.Text;
                alert.BccAddress = txtBcc.Text;


                //Event Type
                if (chkItemAdded.Checked) {
                    alert.AlertType.Add(AlertEventType.ItemAdded); }
                if(chkItemDeleted.Checked){
                    alert.AlertType.Add(AlertEventType.ItemDeleted); }
                if(chkItemUpdated.Checked){
                    alert.AlertType.Add(AlertEventType.ItemUpdated); }
                if(chkDateColumn.Checked){
                    alert.AlertType.Add(AlertEventType.DateColumn); }


                //when To Send
                if (rdDaily.Checked)
                { alert.SendType = SendType.Daily; }
                else if (rdImmediately.Checked)
                { alert.SendType = SendType.Immediate; }
                else if(rdWeekly.Checked)
                { alert.SendType = SendType.Weekely; }


                //TODO Conditions


                //Create new alert
                if(AlertManager.AddAlert(SPContext.Current.Site.RootWeb,alert))
                {
                    //Successfully added
                }

            }
            catch { }

            


           
        }
        #endregion 


        #region Template Related events


        void btnCopyToClipBoard_Click(object sender, EventArgs e)
        {
            try
            {
                string copyText = lstPlaceHolders.SelectedItem.Text;
                System.Windows.Forms.Clipboard.SetText(copyText);
            }
            catch
            {
            }
            //lstPlaceHolders.SelectedItem.
        }

        void btnAddToSubject_Click(object sender, EventArgs e)
        {
            txtMailSubject.Text += " " + "[" + lstPlaceHolders.SelectedItem.Text + "]";
        }
        
        void btnTemplateAdd_Click(object sender, EventArgs e)
        {
            try
            {
                SPList mailTemplateList = SPContext.Current.Site.RootWeb.Lists.TryGetList(ListAndFieldNames.MTListName);

                if (mailTemplateList != null)
                {
                    SPListItem listItem = mailTemplateList.AddItem();
                    listItem["Title"] = txtMailTemplateName.Text;
                    listItem[ListAndFieldNames.MTListMailSubjectFieldName] = txtMailSubject.Text;
                    listItem[ListAndFieldNames.MTListMailBodyFieldName] = txtBody.Text;
                    listItem[ListAndFieldNames.MTListInsertUpdatedFieldsFieldName] = chkIncludeUpdatedColumns.Checked;
                    listItem[ListAndFieldNames.MTListInsertAttachmentsFieldName] = chkInsertAttachments.Checked;
                    listItem[ListAndFieldNames.MTListHighLightUpdatedFieldsFieldName] = chkHighlightUpdatedColumns.Checked;
                    listItem[ListAndFieldNames.MTListOwnerFieldName] = SPContext.Current.Web.CurrentUser;


                    listItem.Update();
                }

            }
            catch { }
        }
        void btnTemplateUpdate_Click(object sender, EventArgs e)
        {


        }
        void btnTemplateCancel_Click(object sender, EventArgs e)
        {

        }
       

        #endregion
    }
}
