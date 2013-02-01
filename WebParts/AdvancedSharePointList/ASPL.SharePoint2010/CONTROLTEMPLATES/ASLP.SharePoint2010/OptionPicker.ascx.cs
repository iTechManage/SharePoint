using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;

namespace AdvanceSharepointListPro.CONTROLTEMPLATES
{
    public partial class OptionPicker : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            fillCurrentListFields();
        }

        protected void fillCurrentListFields()
        {
            SPSecurity.RunWithElevatedPrivileges(delegate
            {
                lstAllFields.Items.Clear();
                using (SPSite objSite = new SPSite(SPContext.Current.Web.Url.ToString()))
                {
                    using (SPWeb objWeb = objSite.OpenWeb())
                    {
                        //  SPList list = objWeb.Lists[new Guid(Request.QueryString["List"])];
                        SPList list = objWeb.Lists["Tasks"];

                        foreach (SPField lstField in list.Fields)
                        {

                            if (lstField.Hidden != true)
                            {
                                if (lstField.CanBeDisplayedInEditForm == true)
                                {
                                    ListItem item = new ListItem();
                                    item.Text = lstField.Title;
                                    item.Value = lstField.Title;
                                    lstAllFields.Items.Add(item);
                                }
                            }
                        }
                    }
                }
            });
        }
    }
}
