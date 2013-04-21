using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Web.UI;
using Microsoft.SharePoint.Utilities;
using System.Text;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using CrowCanyonAdvancedPrint.Classes;
using System.Xml;
using System.IO;
using System.Web.UI.WebControls.WebParts;
using System.Web;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;

namespace CrowCanyonAdvancedPrint.Layouts.CrowCanyon.AdvancedPrint
{
    public partial class PrintPage : LayoutsPageBase
    {
        protected System.Collections.Generic.List<SPListItem> listItems;

        SPList currentList = null;
        SPListItem currentItem = null;
        private string smtpServer = string.Empty;
        private string Blankrow = "- Blank Row -", blank = string.Empty, section = string.Empty;
        string[] items;
        
        protected void Page_Load(object sender, EventArgs e)
        {
           
            if (Request.QueryString["items"] != null && Request.QueryString["source"] != null &&
             Request.QueryString["SiteUrl"] != null)
            {
                string source = Request.QueryString["source"];
                items = Request.QueryString["items"].ToString().Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                string siteURL = Request.QueryString["SiteUrl"];

                try
                {
                    if (Context.Request["source"] != null && Context.Request["source"] != "")
                        currentList = SPContext.Current.Web.Lists[new Guid(Context.Request["source"])];
                }
                catch
                {
                    return;
                }
                try
                {
                    if (!string.IsNullOrEmpty(Request.QueryString["SiteUrl"]))
                    {
                        using (SPSite targetSite = new SPSite(Request.QueryString["SiteUrl"]))
                        {
                            if (targetSite != null)
                            {
                                using (SPWeb targetWeb = targetSite.OpenWeb())
                                {
                                    if (!string.IsNullOrEmpty(Request.QueryString["source"]))
                                    {
                                        currentItem = targetWeb.Lists[new Guid(Request.QueryString["source"])].GetItemById(Convert.ToInt32(items[0]));
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(Request.QueryString["source"]))
                        {
                            currentItem = SPContext.Current.Web.Lists[new Guid(Request.QueryString["source"])].GetItemById(Convert.ToInt32(items[0]));
                        }
                    }
                }
                catch
                {
                    
                    return;
                }

                ((LiteralControl)Page.Master.Controls[0].FindControl("PlaceHolderPageTitleInTitleArea").Controls[0]).Text += ": " + currentItem.Title;

                if (!Page.IsPostBack)
                    PopulatePage();
                this.TemplatesList.SelectedIndexChanged += new EventHandler(TemplatesList_SelectedIndexChanged);
                this.ImageButton1.Click += new ImageClickEventHandler(ImageButton1_Click);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Type"]) &&
                   (Request.QueryString["Type"] == "RibbonButton" ||
                   Request.QueryString["Type"] == "EditControlBlockButton") &&
                   !string.IsNullOrEmpty(Request.QueryString["SiteUrl"]))
            {
                using (SPSite targetSite = new SPSite(Request.QueryString["SiteUrl"]))
                {
                    if (targetSite != null)
                    {
                        using (SPWeb targetWeb = targetSite.OpenWeb())
                        {
                            if (!string.IsNullOrEmpty(Request.QueryString["List"]) && !string.IsNullOrEmpty(Request.QueryString["ID"]))
                            {
                                currentItem = targetWeb.Lists[new Guid(Request.QueryString["List"])].GetItemById(Convert.ToInt32(Request.QueryString["ID"]));
                            }
                        }
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(Request.QueryString["List"]) && !string.IsNullOrEmpty(Request.QueryString["ID"]))
                {
                    currentItem = SPContext.Current.Web.Lists[new Guid(Request.QueryString["List"])].GetItemById(Convert.ToInt32(Request.QueryString["ID"]));
                }
            }

            ((LiteralControl)Page.Master.Controls[0].FindControl("PlaceHolderPageTitleInTitleArea").Controls[0]).Text += ": " + currentItem.Title;

            if (!Page.IsPostBack)
                PopulatePage();
            this.TemplatesList.SelectedIndexChanged += new EventHandler(TemplatesList_SelectedIndexChanged);
            this.ImageButton1.Click += new ImageClickEventHandler(ImageButton1_Click);
            this.DropDownList1.SelectedIndexChanged += new EventHandler(DropDownList1_SelectedIndexChanged);
        }

       protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DropDownList1.SelectedValue.Equals("List"))
            {
                this.PopulatePage();
            }
            else
            {
                this.PopulatePages();
            }
        }
        void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {
            this.PdfExport();
        }
        protected void PopulatePages()
        {
            BdyTextBox.Text = string.Empty;
            string fieldName = string.Empty, fld2 = string.Empty;
            string fldValues = string.Empty, printHead = string.Empty, printFoot = string.Empty, temp = string.Empty, op2 = string.Empty;
            foreach (String strSelItemsID in items)
            {
               int selItemID=0;
                if(Int32.TryParse(strSelItemsID, out selItemID))
                {
                    SPListItem SelItem = currentList.GetItemById(selItemID);
                try
                {
                    XmlDocument xmlDoc = Helper.GetConfigFile(currentList, Constants.ConfigFile.PrintSettingsFile);
                    XmlNode rootNode = null;
                    if (xmlDoc == null)
                    {
                        rootNode = xmlDoc.CreateElement("Templates");                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                              
                        xmlDoc.AppendChild(rootNode);
                    }
                    else
                    {
                        rootNode = xmlDoc.DocumentElement;
                    }
                    foreach (XmlNode printNode in rootNode.ChildNodes)
                    {
                        string templateTitle = printNode.Attributes[Constants.ActionField.printTitle].Value;
                        string printHeader = printNode.Attributes[Constants.ActionField.printHeader].Value;
                        string printFooter = printNode.Attributes[Constants.ActionField.printFooter].Value;
                        if (!string.IsNullOrEmpty(templateTitle))
                        {
                            CCSTemplate action = new CCSTemplate();
                            XmlNode expressionsNode = printNode.FirstChild;
                            if (templateTitle.Equals(TemplatesList.SelectedItem.Text))
                            {
                                foreach (XmlNode expressionNode in expressionsNode.ChildNodes)
                                {
                                    if (expressionNode.Name == Constants.Field.fldNodeName)
                                    {
                                        try
                                        {
                                            fieldName = expressionNode.Attributes[Constants.Field.fldFieldName].Value;
                                            if (fieldName.Equals(Blankrow))
                                            {
                                                fld2 = "";
                                            }
                                            else if (fieldName.StartsWith("- Section:"))
                                            {
                                                fld2 = fieldName;
                                            }
                                            else
                                            {
                                                fld2 = currentList.Fields.GetField(fieldName).Title;
                                            }
                                            //fldValues = currentItem.GetFormattedValue(fieldName);
                                            if (string.IsNullOrEmpty(fld2))
                                            {

                                                fld2 = "&nbsp;";
                                                fldValues = string.Empty;
                                            }

                                            else if (fld2.Contains("- Section"))
                                            {
                                                fldValues = string.Empty;
                                            }
                                            else
                                            {
                                                fldValues = Convert.ToString(SelItem[fieldName]);
                                            }
                                        }
                                        catch { }
                                    }
                                    if (!string.IsNullOrEmpty(fldValues))
                                    {
                                        if (fldValues.Equals("0;#"))
                                        {
                                            fldValues = string.Empty;
                                        }
                                        else if (fldValues.Contains(";#"))
                                        {
                                            fldValues = fldValues.Substring(fldValues.IndexOf(";#") + 2);
                                        }

                                    }
                                    if (string.IsNullOrEmpty(fld2) || fld2 == "&nbsp;")
                                    {
                                        temp += "<tr>" + "<td colspan='2'>" + "&nbsp" + "</td>" + "</tr>";
                                    }
                                    else if (fld2.Contains("- Section"))
                                    {
                                        fld2 = fld2.Substring(fieldName.IndexOf("Section:") + 8);
                                        fld2 = fld2.Substring(0, fld2.IndexOf(" "));
                                        temp += "<tr>" + "<td colspan='2' class='ms-linksectionheader'>" + "<span class='ms-standardheader'>" + fld2 + "</span>" + "</td>" + "</tr>";
                                    }
                                    else
                                    {
                                        temp += "<tr>" + "<td class='ms-formlabel'>" + fld2 + "</td>" + "<td class='ms-formfield'>" + fldValues + "</td>" + "</tr>";
                                    }
                                    //style=\" color:#000000; font-size:12px; font-family:Arial; border:1px solid #cccccc; background-color:#E0E0E0; font-weight:300; padding:4px\">" + fld2 + "</td>" + "<td class='ms-vb2' style=\" color:#000000; font-size:12px; font-family:Arial; border:1px solid #cccccc; padding:4px; background-color: #ffffff\"
                                    printHead = printHeader;
                                    printFoot = printFooter;
                                  
                                }
                            }

                        }
                    } 
                }
                catch { }
                }
                op2 = printHead + "<br>" + "<table class='iw-formtbl' border='1' style=\"border:1px solid #cccccc;margin-top:10px;margin-bottom:10px;border-collapse:collapse\" width='100%'>" + temp + "</table>" + "<br>" + printFoot + "<br>" + "<br>" + "<br>" + "<br>";
                BdyTextBox.Text += op2;
                op2 = string.Empty;
                temp = string.Empty;
            }
        }
  
       
        protected void TemplatesList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DropDownList1.SelectedValue.Equals("Item"))
            {
                this.PopulatePages();
            }
            else
            {
                string fieldName = string.Empty;
                string fldValues = string.Empty;
                string temp = string.Empty;
                string fld2 = string.Empty, printHead = string.Empty, printFoot = string.Empty;

                if (items.Length == 1)
                {
                    if (!string.IsNullOrEmpty(this.TemplatesList.SelectedValue))
                    {
                        XmlDocument xmlDoc = Helper.GetConfigFile(currentList, Constants.ConfigFile.PrintSettingsFile);
                        XmlNode rootNode = null;
                        if (xmlDoc == null)
                        {
                            rootNode = xmlDoc.CreateElement("Templates");
                            xmlDoc.AppendChild(rootNode);
                        }
                        else
                        {
                            rootNode = xmlDoc.DocumentElement;
                        }
                        foreach (XmlNode printNode in rootNode.ChildNodes)
                        {
                            string templateTitle = printNode.Attributes[Constants.ActionField.printTitle].Value;
                            string printHeader = printNode.Attributes[Constants.ActionField.printHeader].Value;
                            string printFooter = printNode.Attributes[Constants.ActionField.printFooter].Value;
                            if (!string.IsNullOrEmpty(templateTitle))
                            {
                                CCSTemplate action = new CCSTemplate();
                                XmlNode expressionsNode = printNode.FirstChild;
                                if (templateTitle.Equals(TemplatesList.SelectedItem.Text))
                                {
                                    foreach (XmlNode expressionNode in expressionsNode.ChildNodes)
                                    {
                                        if (expressionNode.Name == Constants.Field.fldNodeName)
                                        {
                                            try
                                            {
                                                fieldName = expressionNode.Attributes[Constants.Field.fldFieldName].Value;
                                                if (fieldName.Equals(Blankrow))
                                                {
                                                    fld2 = "";
                                                }
                                                else if (fieldName.StartsWith("- Section:"))
                                                {
                                                    fld2 = fieldName;
                                                }
                                                else
                                                {
                                                    fld2 = currentList.Fields.GetField(fieldName).Title;
                                                }
                                                //fldValues = currentItem.GetFormattedValue(fieldName);
                                                if (string.IsNullOrEmpty(fld2))
                                                {

                                                    fld2 = "&nbsp;";
                                                    fldValues = string.Empty;
                                                }

                                                else if (fld2.Contains("- Section"))
                                                {
                                                    fldValues = string.Empty;
                                                }
                                                else
                                                {

                                                    fieldName = expressionNode.Attributes[Constants.Field.fldFieldName].Value;
                                                    fld2 = currentList.Fields.GetField(fieldName).Title;
                                                    //fldValues = currentItem.GetFormattedValue(fieldName);
                                                    fldValues = Convert.ToString(currentItem[Convert.ToString(fieldName)]);
                                                }
                                            }
                                            catch { }
                                        }
                                        if (!string.IsNullOrEmpty(fldValues))
                                        {
                                            if (fldValues.Equals("0;#"))
                                            {
                                                fldValues = string.Empty;
                                            }
                                            else if (fldValues.Contains(";#"))
                                            {
                                                fldValues = fldValues.Substring(fldValues.IndexOf(";#") + 2);
                                            }

                                        }
                                        if (string.IsNullOrEmpty(fld2) || fld2 == "&nbsp;")
                                        {
                                            temp += "<tr>" + "<td colspan='2'>" + "&nbsp" + "</td>" + "</tr>";
                                        }
                                        else if (fld2.Contains("- Section"))
                                        {
                                            fld2 = fld2.Substring(fieldName.IndexOf("Section:") + 8);
                                            fld2 = fld2.Substring(0, fld2.IndexOf(" "));
                                            temp += "<tr>" + "<td colspan='2' class='ms-linksectionheader'>" + "<span class='ms-standardheader'>" + fld2 + "</span>" + "</td>" + "</tr>";
                                        }
                                        else
                                        {
                                            temp += "<tr>" + "<td class='ms-formlabel'>" + fld2 + "</td>" + "<td class='ms-formfield'>" + fldValues + "</td>" + "</tr>";
                                        }
                                        //style=\" color:#000000; font-size:12px; font-family:Arial; border:1px solid #cccccc; background-color:#E0E0E0; font-weight:300; padding:4px\">" + fld2 + "</td>" + "<td class='ms-vb2' style=\" color:#000000; font-size:12px; font-family:Arial; border:1px solid #cccccc; padding:4px; background-color: #ffffff\"
                                        printHead = printHeader;
                                        printFoot = printFooter;
                                        string op2 = printHead + "<br>" + "<table border='1' style=\"border:1px solid #cccccc;margin-top:10px;margin-bottom:10px;border-collapse:collapse\" width='100%'>" + temp + "</table>" + "<br>" + printFoot;
                                        BdyTextBox.Text = op2;
                                    }
                                }
                            }
                        }
                    }
                    else if (TemplatesList.SelectedItem.Text.Equals("- None -"))
                    {
                        //this.TemplatesList.Items.Clear();
                        this.PopulatePage();
                    }
                }
                else
                {
                    string fieldName3 = string.Empty, fld3 = string.Empty, fldValues3 = string.Empty, textVal3 = string.Empty, textVal = string.Empty;
                    if (!string.IsNullOrEmpty(this.TemplatesList.SelectedValue))
                    {

                        XmlDocument xmlDoc = Helper.GetConfigFile(currentList, Constants.ConfigFile.PrintSettingsFile);
                        XmlNode rootNode = null;
                        if (xmlDoc == null)
                        {
                            rootNode = xmlDoc.CreateElement("Templates");
                            xmlDoc.AppendChild(rootNode);
                        }
                        else
                        {
                            rootNode = xmlDoc.DocumentElement;
                        }
                        foreach (XmlNode printNode in rootNode.ChildNodes)
                        {
                            string templateTitle = printNode.Attributes[Constants.ActionField.printTitle].Value;
                            string printHeader = printNode.Attributes[Constants.ActionField.printHeader].Value;
                            string printFooter = printNode.Attributes[Constants.ActionField.printFooter].Value;
                            if (!string.IsNullOrEmpty(templateTitle))
                            {
                                CCSTemplate action = new CCSTemplate();
                                XmlNode expressionsNode = printNode.FirstChild;
                                if (templateTitle.Equals(TemplatesList.SelectedItem.Text))
                                {
                                    foreach (XmlNode expressionNode in expressionsNode.ChildNodes)
                                    {
                                        if (expressionNode.Name == Constants.Field.fldNodeName)
                                        {
                                            try
                                            {
                                                fieldName3 = expressionNode.Attributes[Constants.Field.fldFieldName].Value;
                                                fld2 = currentList.Fields.GetField(fieldName3).Title;
                                            }
                                            catch { }
                                        }

                                        textVal3 += "<td class='ms-vh'>" + fld2 + "</td>";
                                    }
                                }
                            }
                        }
                    }
                    listItems = new System.Collections.Generic.List<SPListItem>();

                    for (int i = 0; i < items.Length; i++)
                    {
                        SPListItem currentListItem = currentList.GetItemById(int.Parse(items[i]));
                        listItems.Add(currentListItem);

                    }

                    foreach (SPListItem selItem in listItems)
                    {
                        textVal += "<tr>";
                        if (!string.IsNullOrEmpty(this.TemplatesList.SelectedValue))
                        {

                            XmlDocument xmlDoc = Helper.GetConfigFile(currentList, Constants.ConfigFile.PrintSettingsFile);
                            XmlNode rootNode = null;
                            if (xmlDoc == null)
                            {
                                rootNode = xmlDoc.CreateElement("Templates");
                                xmlDoc.AppendChild(rootNode);
                            }
                            else
                            {
                                rootNode = xmlDoc.DocumentElement;
                            }
                            foreach (XmlNode printNode in rootNode.ChildNodes)
                            {
                                string templateTitle = printNode.Attributes[Constants.ActionField.printTitle].Value;
                                string printHeader = printNode.Attributes[Constants.ActionField.printHeader].Value;
                                string printFooter = printNode.Attributes[Constants.ActionField.printFooter].Value;
                                if (!string.IsNullOrEmpty(templateTitle))
                                {
                                    CCSTemplate action = new CCSTemplate();
                                    XmlNode expressionsNode = printNode.FirstChild;
                                    if (templateTitle.Equals(TemplatesList.SelectedItem.Text))
                                    {
                                        foreach (XmlNode expressionNode in expressionsNode.ChildNodes)
                                        {
                                            if (expressionNode.Name == Constants.Field.fldNodeName)
                                            {
                                                try
                                                {
                                                    fieldName3 = expressionNode.Attributes[Constants.Field.fldFieldName].Value;
                                                    fld3 = currentList.Fields.GetField(fieldName3).Title;
                                                    fldValues3 = Convert.ToString(selItem[Convert.ToString(fld3)]);

                                                }
                                                catch { }

                                                if (!string.IsNullOrEmpty(fldValues3))
                                                {
                                                    if (fldValues3.Equals("0;#"))
                                                    {
                                                        fldValues3 = string.Empty;
                                                    }
                                                    else if (fldValues3.Contains(";#"))
                                                    {
                                                        fldValues3 = fldValues3.Substring(fldValues3.IndexOf(";#") + 2);
                                                    }

                                                }
                                            }
                                            printHead = printHeader;
                                            printFoot = printFooter;
                                            textVal += "<td class='ms-vb2'>" + fldValues3 + "</td>";

                                        }
                                    }
                                }

                            }
                        }

                        textVal += "</tr>";
                    }
                    BdyTextBox.Text = printHead + "<br>" + "<table border='1' style=\"border:1px solid #cccccc;margin-top:10px;margin-bottom:10px;border-collapse:collapse\" width='100%'>" + "<tr>" + textVal3 + "</tr>" + textVal + "</table>" + "<br>" + printFoot;
                }

            }

        }
        public void GetTemplateName()
        {
            
            XmlDocument xmlDoc = Helper.GetConfigFile(currentList, Constants.ConfigFile.PrintSettingsFile);
            XmlNode rootNode = null;
            if (xmlDoc == null)
            {
                rootNode = xmlDoc.CreateElement("Templates");
                xmlDoc.AppendChild(rootNode);
            }
            else
            {
                rootNode = xmlDoc.DocumentElement;
            }
            if (items.Length == 1)
            {
                System.Web.UI.WebControls.ListItem emptyColumnItem = new System.Web.UI.WebControls.ListItem("- None -", string.Empty);
                if (!this.TemplatesList.Items.Contains(emptyColumnItem))
                    this.TemplatesList.Items.Add(emptyColumnItem);
            }
            foreach (XmlNode printNode in rootNode.ChildNodes)
            {
                string templateTitle = printNode.Attributes[Constants.ActionField.printTitle].Value;
                if (!string.IsNullOrEmpty(templateTitle) && !this.TemplatesList.Items.Contains(new System.Web.UI.WebControls.ListItem(templateTitle)))
                    this.TemplatesList.Items.Add(templateTitle);
            }

        }
        protected void PopulatePage()
        {
          
            try
            {

                string textVal = string.Empty, textVal2 = string.Empty;
                string val = string.Empty, printHead = string.Empty, PrintFoot = string.Empty;
           
                this.GetTemplateName();
                
               
                if (items.Length == 1)
                {
                    this.lbl2.Enabled = false;
                    this.DropDownList1.Enabled = false;
                    try
                    {
                        XmlDocument xmlDoc = Helper.GetConfigFile(currentList, Constants.ConfigFile.PrintSettingsFile);
                        XmlNode rootNode = null;
                        if (xmlDoc == null)
                        {
                            rootNode = xmlDoc.CreateElement("Templates");
                            xmlDoc.AppendChild(rootNode);
                        }
                        else
                        {
                            rootNode = xmlDoc.DocumentElement;
                        }
                        foreach (XmlNode printNode in rootNode.ChildNodes)
                        {
                            string templateTitle = printNode.Attributes[Constants.ActionField.printTitle].Value;
                            string printHeader = printNode.Attributes[Constants.ActionField.printHeader].Value;
                            string printFooter = printNode.Attributes[Constants.ActionField.printFooter].Value;


                            foreach (SPField field in currentList.Fields)
                            {

                                try
                                {
                                    if (field != null && !field.Hidden)// && !field.ReadOnlyField
                                    {
                                        System.Web.UI.WebControls.ListItem newFieldItem = new System.Web.UI.WebControls.ListItem(field.Title, field.InternalName);
                                        if (!this.UnvisibleListBox.Items.Contains(newFieldItem) && this.UnvisibleListBox.Items.FindByText(field.Title) == null)
                                            this.UnvisibleListBox.Items.Add(newFieldItem);

                                        //AllFieldsListBox.Items.Add(field.Title);
                                    }
                                }
                                catch { }
                            }


                            for (int i = 0; i < UnvisibleListBox.Items.Count; i++)
                            {
                                try
                                {

                                    val = Convert.ToString(currentItem[Convert.ToString(UnvisibleListBox.Items[i])]);

                                    if (!string.IsNullOrEmpty(val))
                                    {
                                        if (val.Equals("0;#"))
                                        {
                                            val = string.Empty;
                                        }
                                        else if (val.Contains(";#"))
                                        {
                                            val = val.Substring(val.IndexOf(";#") + 2);
                                        }

                                    }
                                }
                                catch { }



                                textVal += "<tr>" + "<td class='ms-formlabel'>" + Convert.ToString(UnvisibleListBox.Items[i]) + "</td>" + "<td class='ms-formfield'>" + val + "</td>" + "</tr>";
                                printHead = printHeader;
                                PrintFoot = printFooter;
                            }
                        }
                    }
                    catch { }
                    BdyTextBox.Text = printHead + "<table border='1' style=\"border:1px solid #cccccc;margin-top:10px;margin-bottom:10px;border-collapse:collapse\" width='100%'>" + textVal + "</table>" + PrintFoot;
                }

                else
                {

                    string fieldName2 = string.Empty, fld2 = string.Empty, fldValues2 = string.Empty;
                    if (!string.IsNullOrEmpty(this.TemplatesList.SelectedValue))
                    {

                        XmlDocument xmlDoc = Helper.GetConfigFile(currentList, Constants.ConfigFile.PrintSettingsFile);
                        XmlNode rootNode = null;
                        if (xmlDoc == null)
                        {
                            rootNode = xmlDoc.CreateElement("Templates");
                            xmlDoc.AppendChild(rootNode);
                        }
                        else
                        {
                            rootNode = xmlDoc.DocumentElement;
                        }
                        foreach (XmlNode printNode in rootNode.ChildNodes)
                        {
                            string templateTitle = printNode.Attributes[Constants.ActionField.printTitle].Value;
                            string printHeader = printNode.Attributes[Constants.ActionField.printHeader].Value;
                            string printFooter = printNode.Attributes[Constants.ActionField.printFooter].Value;
                            if (!string.IsNullOrEmpty(templateTitle))
                            {
                                CCSTemplate action = new CCSTemplate();
                                XmlNode expressionsNode = printNode.FirstChild;
                                if (templateTitle.Equals(TemplatesList.SelectedItem.Text))
                                {
                                    foreach (XmlNode expressionNode in expressionsNode.ChildNodes)
                                    {
                                        if (expressionNode.Name == Constants.Field.fldNodeName)
                                        {
                                            try
                                            {
                                                fieldName2 = expressionNode.Attributes[Constants.Field.fldFieldName].Value;
                                                fld2 = currentList.Fields.GetField(fieldName2).Title;
                                            }
                                            catch { }
                                        }
                                        textVal2 += "<td class='ms-vh'>" + fld2 + "</td>";
                                    }
                                }
                            }
                        }
                    }
                    listItems = new System.Collections.Generic.List<SPListItem>();

                    for (int i = 0; i < items.Length; i++)
                    {
                        SPListItem currentListItem = currentList.GetItemById(int.Parse(items[i]));
                        listItems.Add(currentListItem);

                    }

                    foreach (SPListItem selItem in listItems)
                    {
                        textVal += "<tr>";
                        if (!string.IsNullOrEmpty(this.TemplatesList.SelectedValue))
                        {

                            XmlDocument xmlDoc = Helper.GetConfigFile(currentList, Constants.ConfigFile.PrintSettingsFile);
                            XmlNode rootNode = null;
                            if (xmlDoc == null)
                            {
                                rootNode = xmlDoc.CreateElement("Templates");
                                xmlDoc.AppendChild(rootNode);
                            }
                            else
                            {
                                rootNode = xmlDoc.DocumentElement;
                            }
                            foreach (XmlNode printNode in rootNode.ChildNodes)
                            {
                                string templateTitle = printNode.Attributes[Constants.ActionField.printTitle].Value;
                                string printHeader = printNode.Attributes[Constants.ActionField.printHeader].Value;
                                string printFooter = printNode.Attributes[Constants.ActionField.printFooter].Value;
                                if (!string.IsNullOrEmpty(templateTitle))
                                {
                                    CCSTemplate action = new CCSTemplate();
                                    XmlNode expressionsNode = printNode.FirstChild;
                                    if (templateTitle.Equals(TemplatesList.SelectedItem.Text))
                                    {
                                        foreach (XmlNode expressionNode in expressionsNode.ChildNodes)
                                        {
                                            if (expressionNode.Name == Constants.Field.fldNodeName)
                                            {
                                                try
                                                {
                                                    fieldName2 = expressionNode.Attributes[Constants.Field.fldFieldName].Value;
                                                    fld2 = currentList.Fields.GetField(fieldName2).Title;
                                                    fldValues2 = Convert.ToString(selItem[Convert.ToString(fieldName2)]);
                                                }
                                                catch { }

                                                if (!string.IsNullOrEmpty(fldValues2))
                                                {
                                                    if (fldValues2.Equals("0;#"))
                                                    {
                                                        fldValues2 = string.Empty;
                                                    }
                                                    else if (fldValues2.Contains(";#"))
                                                    {
                                                        fldValues2 = fldValues2.Substring(fldValues2.IndexOf(";#") + 2);
                                                    }

                                                }
                                            }

                                            textVal += "<td class='ms-vb2'>" + fldValues2 + "</td>";
                                            printHead = printHeader;
                                            PrintFoot = printFooter;

                                        }
                                    }
                                }

                            }
                        }

                        textVal += "</tr>";
                    }
                    BdyTextBox.Text = printHead + "<table border='1' style=\"border:1px solid #cccccc;margin-top:10px;margin-bottom:10px;border-collapse:collapse\" width='100%'>" + "<tr >" + textVal2 + "</tr>" + textVal + "</table>" + PrintFoot;
                }


            }
            catch { }


        }
        protected void PdfExport()
        {
            this.PopulatePage();
            System.IO.MemoryStream mstream = createPDF();
            byte[] byteArray = mstream.ToArray();
            string exportTitle = "Pdf";
            mstream.Flush();
            mstream.Close();
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.AddHeader("content-disposition", "attachment;filename=" + exportTitle + " " + currentList + ".pdf");
            HttpContext.Current.Response.Charset = "";
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Current.Response.ContentType = "application/octet-stream";
            HttpContext.Current.Response.BinaryWrite(byteArray);
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();
          
        }
        private MemoryStream createPDF()
        {

            string html = BdyTextBox.Text;

            MemoryStream msOutput = new MemoryStream();

            TextReader reader = new StringReader(html);

            Document document = new Document(PageSize.A4, 30, 30, 30, 30);

            PdfWriter writer = PdfWriter.GetInstance(document, msOutput);

            HTMLWorker worker = new HTMLWorker(document);

            document.Open();

            worker.StartDocument();

            worker.Parse(reader);

            worker.EndDocument();

            worker.Close();

            document.Close();


            return msOutput;
        }


    }


}
