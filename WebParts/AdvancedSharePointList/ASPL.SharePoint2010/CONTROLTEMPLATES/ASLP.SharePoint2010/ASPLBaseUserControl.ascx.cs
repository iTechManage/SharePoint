using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using ASPL.Blocks;

namespace ASPL.SharePoint2010.CONTROLTEMPLATES.ASLP.SharePoint2010
{
    public partial class ASPLBaseUserControl : UserControl
    {
        protected override void OnInit(EventArgs e)
        {

            ValidatePageAccess();
            base.OnInit(e);
        }

        private void ValidatePageAccess()
        {
            // if user doesn't have specified permissions...
            if (!SPContext.Current.Web.DoesUserHavePermissions(SPBasePermissions.ManageWeb | SPBasePermissions.ManageLists))
            {
                SPUtility.Redirect(SPUtility.GetFullUrl(SPContext.Current.Site, SPContext.Current.Web.ServerRelativeUrl + "/_layouts/" + SPUtility.AccessDeniedPage), SPRedirectFlags.Default, System.Web.HttpContext.Current);
            }

            // if ASPL feature is activated on current web or not...
            if (SPContext.Current.SiteFeatures[new Guid(Constants.ASPLFeature.FeatureID)] == null)
            {
                SPUtility.TransferToErrorPage(string.Format("Feature: {0}({1}) is not activated in current web-site. Please activate it to access this page.", Constants.ASPLFeature.Name, Constants.ASPLFeature.FeatureID));
            }

            // check the query string for list id
            if(string.IsNullOrEmpty(Request.QueryString["List"]))
            {
                SPUtility.TransferToErrorPage("Unable to retrive list details, please try to access this page from list settings.");
            }
        }
    }
}
