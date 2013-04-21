using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Xml;
using CrowCanyonAdvancedPrint.Classes;
using System.Web;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;

namespace CrowCanyonAdvancedPrint.Layouts.CrowCanyon.AdvancedPrint
{
    public partial class PrintView : LayoutsPageBase
    {
        protected System.Collections.Generic.List<SPListItem> listItems;
        SPListItem currentItem = null;
        private string Blankrow = "- Blank Row -", blank = string.Empty, section = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
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
        
            }
        void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {
            this.PdfExport();
        }
        void TemplatesList_SelectedIndexChanged(object sender, EventArgs e)
        {
            string fieldName = string.Empty;
            string fldValues = string.Empty;
            string temp = string.Empty;
            string fld2 = string.Empty;
           string printHead = string.Empty, PrintFoot = string.Empty;
           if (TemplatesList.SelectedItem.Text.Equals("- None -"))
           {
               //this.TemplatesList.Items.Clear();
               this.PopulatePage();
           }
           else if (!string.IsNullOrEmpty(this.TemplatesList.SelectedValue))
            {
                XmlDocument xmlDoc = Helper.GetConfigFile(SPContext.Current.List, Constants.ConfigFile.PrintSettingsFile);
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
                                        else if(fieldName.StartsWith("- Section:"))
                                        {
                                            fld2 = fieldName;
                                            }
                                            
                                        else
                                        {
                                            fld2 = SPContext.Current.List.Fields.GetField(fieldName).Title;
                                        }

                                        if (string.IsNullOrEmpty(fld2))
                                        {
                                            fld2 = "&nbsp;";
                                            fldValues = string.Empty;
                                        }
                                        else if(fld2.Contains("- Section"))
                                        {
                                            fldValues=string.Empty;
                                        }
                                        else
                                        {
                                            fldValues = Convert.ToString(currentItem[Convert.ToString(fld2)]);
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
                                    temp += "<tr>" + "<td colspan='2'>"+"&nbsp"+"</td>"+"</tr>";
                                }
                                else if(fld2.Contains("- Section"))
                                {
                                    fld2 = fld2.Substring(fieldName.IndexOf("Section:") + 8);
                                    fld2 = fld2.Substring(0, fld2.IndexOf(" "));
                                    temp+= "<tr>" +"<td colspan='2' class='ms-linksectionheader'>"+"<span class='ms-standardheader'>"+fld2+"</span>"+"</td>"+"</tr>";
                                }


                                else
                                {
                                    temp += "<tr>" + "<td class='ms-formlabel'>" + fld2 + "</td>" + "<td class='ms-formfield' bgcolor='#F0F0F0'>" + fldValues + "</td>" + "</tr>";
                                }
                                printHead = printHeader;
                                PrintFoot = printFooter;
                                string op2 =printHead+"<table border='1' style=\"border:1px solid #cccccc;margin-top:10px;margin-bottom:10px;border-collapse:collapse\" width='100%'>" + temp + "</table>"+PrintFoot;
                                BdyTextBox.Text = op2;
                            }
                        }
                    }
                }
                
            }
        }        
        public void GetTemplateName()
        {

            XmlDocument xmlDoc = Helper.GetConfigFile(SPContext.Current.List, Constants.ConfigFile.PrintSettingsFile);
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

            System.Web.UI.WebControls.ListItem emptyColumnItem = new System.Web.UI.WebControls.ListItem("- None -", string.Empty);
            if (!this.TemplatesList.Items.Contains(emptyColumnItem))
                this.TemplatesList.Items.Add(emptyColumnItem);
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
                string val = string.Empty, printHead = string.Empty, PrintFoot = string.Empty, blank = string.Empty;
                this.GetTemplateName();

                try
                {
                    XmlDocument xmlDoc = Helper.GetConfigFile(SPContext.Current.List, Constants.ConfigFile.PrintSettingsFile);
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

                        foreach (SPField field in SPContext.Current.List.Fields)
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



                            textVal += "<tr >" + "<td style='font-family : Arial' class='ms-formlabel'>" + Convert.ToString(UnvisibleListBox.Items[i]) + "</td>" + "<td style='font-family : Arial' bgcolor='F0F0F0' class='ms-formfield'>" + val + "</td>" + "</tr>";
                            printHead = printHeader;
                            PrintFoot = printFooter;
                        }
                    }
                }
                catch { }
                BdyTextBox.Text = "<br>" + "<table border='1' style=\"border:1px solid #cccccc;margin-top:10px;margin-bottom:10px;border-collapse:collapse\" width='100%'>" + textVal + "</table>" + "<br>";
            }

            catch { }

        }
        protected void PdfExport()
        {
            System.IO.MemoryStream mstream = createPDF();
            byte[] byteArray = mstream.ToArray();
            string exportTitle = "Pdf";
            mstream.Flush();
            mstream.Close();
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.AddHeader("content-disposition", "attachment;filename=" + exportTitle + " " + SPContext.Current.List + ".pdf");
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
