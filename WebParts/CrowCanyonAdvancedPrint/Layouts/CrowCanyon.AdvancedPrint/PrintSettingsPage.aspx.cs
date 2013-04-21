using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Xml;
using CrowCanyonAdvancedPrint.Classes;

namespace CrowCanyonAdvancedPrint.Layouts.CrowCanyon.AdvancedPrint
{
    public partial class PrintSettingsPage : LayoutsPageBase
    {
        private string blankrow = "- Blank Row -";
        internal List<CCSTemplate> Templates
        {
            get
            {
                return (this.ViewState["Templates"] as List<CCSTemplate>);
            }
            set
            {
                this.ViewState["Templates"] = value;
                this.gvTemplates.DataSource = value;
                this.gvTemplates.DataBind();
            }
        }
        SPList currentList = null;
   
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Context.Request["List"] != null && Context.Request["List"] != "")
                    currentList = SPContext.Current.Web.Lists[new Guid(Context.Request["List"])];
            }
            catch
            {
                return;
            }

            ((LiteralControl)Page.Master.Controls[0].FindControl("PlaceHolderPageTitleInTitleArea").Controls[0]).Text += ": " + currentList.Title;

            if (!Page.IsPostBack)
                PopulatePage();
        }
        protected void PopulatePage()
        {
           
            TooListBox.Items.Clear();
            XmlDocument xmlDoc = Helper.GetConfigFile(SPContext.Current.List, Constants.ConfigFile.PrintSettingsFile);

            if (xmlDoc != null)
            {
                this.Templates = Helper.DeSerializeActionFromXML(xmlDoc);
            }

            if (currentList != null)
            {
                this.AllFieldsListBox.Items.Add(this.blankrow);
                foreach (SPField field in currentList.Fields)
                {

                    try
                    {
                        if (field != null && !field.Hidden)// && !field.ReadOnlyField
                        {
                            ListItem newFieldItem = new ListItem(field.Title, field.InternalName);
                            if (!this.AllFieldsListBox.Items.Contains(newFieldItem) && this.AllFieldsListBox.Items.FindByText(field.Title) == null)
                                this.AllFieldsListBox.Items.Add(newFieldItem);
                            
                            //AllFieldsListBox.Items.Add(field.Title);
                        }
                        
                    }


                    catch { }

                }
               
                this.gvTemplates.DataSource = this.Templates;
                this.gvTemplates.DataBind();

            }
        }
            //try
            //{
            //    if (TooListBox != null)
            //    {
            //        //string toLBValue = Convert.ToString(
            //            //this.GetWebPropertyValue(TooListBox.ID));
            //        if (!string.IsNullOrEmpty(toLBValue))
            //        {
            //            string[] toLBItemValues = null;
            //            try
            //            {
            //                toLBItemValues = toLBValue.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            //            }
            //            catch (Exception ex)
            //            {
            //            }
            //            if (toLBItemValues != null && toLBItemValues.Length > 0)
            //            {
            //                foreach (string itemValue in toLBItemValues)
            //                {
            //                    try
            //                    {
            //                        TooListBox.ClearSelection();
            //                        TooListBox.Items.Add(itemValue);
            //                        AllFieldsListBox.Items.Remove(itemValue);
            //                    }
            //                    catch (Exception ex)
            //                    {
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}

            //catch (System.Exception e)
            //{
            //}
     
        //protected string GetWebPropertyValue(string key)
        //{
        //    String webPropertyValue = String.Empty;
        //    String webPropertyKey = Context.Request["List"].Replace(
        //        "{", "").Replace("}", "").Replace("-", "") + "_" + key;
        //    try
        //    {
        //        SPWeb web = SPContext.Current.Web;
        //        SPSecurity.RunWithElevatedPrivileges(delegate()
        //        {
        //            if (web != null && web.Properties.ContainsKey(webPropertyKey))
        //            {
        //                webPropertyValue = web.Properties[webPropertyKey];
        //            }
        //        });
        //    }
        //    catch (System.Exception e)
        //    {
        //        if (this.ErrorLabel != null)
        //        {
        //            if (!String.IsNullOrEmpty(this.ErrorLabel.Text))
        //            {
        //                this.ErrorLabel.Text += "<br>";
        //            }
        //            this.ErrorLabel.Text += key + " value not found. Message: " + e.Message;
        //        }
        //    }
        //    finally { }
        //    return webPropertyValue;
        //}
        protected void AddFieldToListBox(object sender, EventArgs e)
        {
            if (AllFieldsListBox != null && AllFieldsListBox.SelectedItem != null)
            {
                if (AllFieldsListBox.SelectedItem.Text.Equals(this.blankrow))
                {
                    TooListBox.Items.Add(this.blankrow);
                }
                else
                {

                    if (!TooListBox.Items.Contains(AllFieldsListBox.SelectedItem))
                    {
                        TooListBox.ClearSelection();
                        TooListBox.Items.Add(AllFieldsListBox.SelectedItem);
                        AllFieldsListBox.Items.Remove(AllFieldsListBox.SelectedItem);

                    }
                }
            }
        }
        protected void RemoveFieldFromList(object sender, EventArgs e)
        {
            if (TooListBox != null &&
                TooListBox.SelectedItem != null)
            {
                AllFieldsListBox.ClearSelection();
                AllFieldsListBox.Items.Add(TooListBox.SelectedItem);
                TooListBox.Items.Remove(TooListBox.SelectedItem);
            }
        }
        protected void AddSectionField(object sender, EventArgs e)
        {
            if (TxtSection != null &&
                TxtSection.Text != null)
            {
                TooListBox.ClearSelection();
                TooListBox.Items.Add("- Section:" + TxtSection.Text + " -");
                TxtSection.Text = String.Empty;
            }
            }
        protected void gvActions_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                List<CCSTemplate> ccsprint = this.Templates;
                if ((ccsprint != null) && (e.RowIndex < ccsprint.Count))
                {
                    CCSTemplate actionToDelete = ccsprint[e.RowIndex];
                    string actionIdToDelete = actionToDelete.Id;
                    Helper.DeleteAction(actionIdToDelete, SPContext.Current.List);

                    ccsprint.RemoveAt(e.RowIndex);
                    this.Templates = ccsprint;

                    //Helper.AddAction(newAction, SPContext.Current.List);

                    this.gvTemplates.DataSource = this.Templates;
                    this.gvTemplates.DataBind();
                }
            }
            catch
            {
            }
        }
        protected void gvActions_SelectedIndexChanged(object sender, EventArgs e)
        {
            //this.PopulatePage();  
            try
                {
                    CCSTemplate selectedAction = this.Templates[this.gvTemplates.SelectedIndex];
                    this.TemplateTitle.Text = selectedAction.Title;
                    this.RichtextBox.Text = selectedAction.Header;
                    this.RichtextBox2.Text = selectedAction.Footer;
                    foreach (Field field in selectedAction.Fields)
                    {
                        if(field.FieldName.Equals(blankrow))
                        {
                            this.TooListBox.Items.Add(field.FieldName);
                        }
                        else if(field.FieldName.StartsWith("- Section"))
                        {
                            this.TooListBox.Items.Add(field.FieldName);
                        }
                        else
                        {
                        string fldinternalName=field.FieldName;
                        if (currentList.Fields.ContainsField(fldinternalName))
                        {
                            try
                            {
                                string fldDisplayName = currentList.Fields.GetField(fldinternalName).Title;
                                this.TooListBox.Items.Add(fldDisplayName);
                            }
                            catch { }
                        }
                        }
                    }
                }
                catch
                {
                }         
        }
        protected void SaveButton_Clicked(object sender, EventArgs e)
        {
            if (this.TooListBox != null && this.TooListBox.Items.Count > 0)
            {
                List<Field> fields = new List<Field>();
                foreach (ListItem item in this.TooListBox.Items)
                {
                    Field newField = new Field();
                    newField.FieldName = item.Value;
                    fields.Add(newField);
                }
                if (this.gvTemplates.SelectedIndex != -1)
                {
                    CCSTemplate selectedAction = this.Templates[this.gvTemplates.SelectedIndex];
                    selectedAction.Title = this.TemplateTitle.Text.Trim();
                    selectedAction.Header = this.RichtextBox.Text.Trim();
                    selectedAction.Footer = this.RichtextBox2.Text.Trim();
                    selectedAction.Fields = fields;
                    selectedAction.Id = selectedAction.Id;

                    Helper.UpdateTemplate(selectedAction, SPContext.Current.List);
                }
                else
                {

                    CCSTemplate newAction = new CCSTemplate();
                    newAction.Title = this.TemplateTitle.Text.Trim();
                    newAction.Header = this.RichtextBox.Text.Trim();
                    newAction.Footer = this.RichtextBox2.Text.Trim();
                    newAction.Id = Guid.NewGuid().ToString();
                    newAction.Fields = fields;
                    Helper.AddTemplate(newAction, SPContext.Current.List);
                }
                btnCancel_Click(sender, e);

            }
            if (this.MessageLabel != null)
            {
                this.MessageLabel.Text = "";
            }
            if (this.ErrorLabel != null)
            {
                this.ErrorLabel.Text = "";
            }
            MessageLabel.Text += " Settings saved successfully.";
 
  
        }
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            this.TemplateTitle.Text = string.Empty;
            this.TooListBox.Items.Clear();
            this.RichtextBox.Text = string.Empty;
            this.RichtextBox2.Text = string.Empty;
            this.gvTemplates.SelectedIndex = -1;
            this.PopulatePage();
        }
    }
}


