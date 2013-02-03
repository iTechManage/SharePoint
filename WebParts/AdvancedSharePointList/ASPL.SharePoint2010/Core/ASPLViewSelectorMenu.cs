using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Specialized;
using ASPL.ConfigModel;
using ASPL.Blocks;
using System.Web.UI.HtmlControls;
using System.Globalization;
using System.Web;
using Microsoft.SharePoint.Utilities;
using System.Xml;

namespace ASPL.SharePoint2010.Core
{
    public class ASPLViewSelectorMenu : ViewSelectorMenu
    {
        private Views allViews;
        private string CurrentViewName;

        protected override void OnInit(EventArgs e)
        {
            allViews = Views.LoadViews(GetConfigFile(Constants.ConfigFile.ViewPermissionsFile));
            base.OnInit(e);
        }

        protected override void OnPreRender(EventArgs e)
        {
            try
            {
                if (allViews != null)
                {
                    CurrentViewName = SPContext.Current.ViewContext.View.ToString();
                    SPPrincipal ObjCurrentUserPrincipal = SPContext.Current.Web.CurrentUser;

                    foreach (ViewSetting objView in allViews)
                    {
                        if (objView.SPVName == CurrentViewName)
                        {
                            if (objView.Permission == "hide" &&
                                DoesUserExist(objView.UserGroup, ObjCurrentUserPrincipal))
                            {
                                SPUtility.Redirect(
                                    Constants.Resource.ViewAccessDeniedPage,
                                    SPRedirectFlags.RelativeToLayoutsPage,
                                    HttpContext.Current);
                            }
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                base.OnPreRender(e);
                Logging.Log(exp);
            }
            base.OnPreRender(e);
        }

        private bool DoesUserExist(string Username, SPPrincipal objPrincipal)
        {

            if (!String.IsNullOrEmpty(Username))
            {
                if (Username == "ALL")
                {
                    return true;
                }

                string[] objsplitUsers = Username.Split(',');
                foreach (string user in objsplitUsers)
                {
                    if (!String.IsNullOrEmpty(user) && !user.Equals(""))
                    {
                        if (user.Contains("\\"))
                        {
                            return user.ToLower() == objPrincipal.LoginName.ToLower();
                        }
                        else
                        {
                            SPGroup grp = SPContext.Current.Web.Groups[user];
                            return grp.ContainsCurrentUser;
                        }
                    }
                }
            }

            return false;
        }

        private XmlDocument GetConfigFile(string filename)
        {
            try
            {
                SPFile file = SPContext.Current.Web.GetFile(
                    SPUtility.GetFullUrl(SPContext.Current.Web.Site,
                    SPContext.Current.List.RootFolder.ServerRelativeUrl.TrimEnd('/') + "/" + filename)
                    );

                XmlDocument doc = new XmlDocument();
                doc.Load(file.OpenBinaryStream());
                return doc;
            }
            catch (Exception exp)
            {
                Logging.Log(exp);
            }

            return null;
        }
    }
}
