using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASPL.ConfigModel;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Web.UI.HtmlControls;
using ASPL.Blocks;
using System.Web.UI;

namespace ASPL.SharePoint2010.Core
{
    class RendringUtil
    {
        public static string RenderTabs(Tabs allTabs)
        {
            if (allTabs != null && allTabs.Count > 0)
            {

                string initHtml = @"<tr style='margin-bottom: 7px' id='TabsControl'>
                                    <td colspan=2>
                                        <script>var SLFE_TabHideEmpty ='true'</script>
                                        <ul class='ms-cui-tts' unselectable='on' style='height:65px'>
                                            <li class='ms-cui-cg ms-cui-cg-db ms-cui-cg-s' style='height:25px'style='white-space:normal;'>
                                               <ul class='ms-cui-ct-ul' id='ulTabCtrl'>";

                string endHtml = "</ul></li></ul></td></tr>";

                string tabHtml = string.Empty;

                foreach (Tab t in allTabs)
                {
                    tabHtml += RenderTab(t);
                }

                return initHtml + tabHtml + endHtml;
            }
            return "";
        }

        private static string RenderTab(Tab tab)
        {
            string cssClass = "ms-cui-tt ";
            string realCssClass = cssClass;
            if (tab.IsSelected) cssClass += "ms-cui-tt-s ";

            else if (tab.IsFirst) { cssClass += "ms-cui-ct-first "; realCssClass = cssClass; }

            else if (tab.IsLast) { cssClass += "ms-cui-ct-last "; realCssClass = cssClass; }



            string html = @"<li id='tab{0}' class='{1}' realclass='{3}' title='{2}' unselectable='on'>
                             <a class='ms-cui-tt-a' title='{2}' onclick='SLFE_SelectTab(&quot;{0}&quot;);' target='_self' href='javascript:;' unselectable='on'>
                            <span class='ms-cui-tt-span' style='white-space:normal;' unselectable='on' >{0}</span>
                            </a>
                            <script>var key=unescape('{0}').replace(/\\+/g, ' ');
                            SLFE_TabToElementIDHash[key] = 'tab{0}';</script>
                          </li>";
            return string.Format(html, tab.Title, cssClass, tab.Description, realCssClass);
        }

        public static void SetDefault(SPField field, FieldDefaults allFieldDefaults)
        {
            if (allFieldDefaults.Count > 0)
            {
                foreach (FieldDefault fd in allFieldDefaults)
                {
                    if (fd.OnField.SPName == field.InternalName)
                    {
                        if (PrincipalEvaluator.Check(fd.ForSPPrinciples,
                            fd.BySPPrinciplesOperator))
                        {
                            field.DefaultValue = fd.Value.ToString();
                            break;
                        }
                    }
                }
            }
        }

        public static void RenderResources(Control ctrlTarget)
        {
            HtmlGenericControl jsTag1 = new HtmlGenericControl("script");
            jsTag1.Attributes.Add("type", "text/javascript");
            jsTag1.Attributes.Add("language", "javascript");
            jsTag1.Attributes.Add("src", Constants.Resource.JQuery1_7_2_min);
            ctrlTarget.Page.Header.Controls.Add(jsTag1);

            HtmlGenericControl jsTag = new HtmlGenericControl("script");
            jsTag.Attributes.Add("type", "text/javascript");
            jsTag.Attributes.Add("language", "javascript");
            jsTag.Attributes.Add("src", Constants.Resource.IteratorJS);
            ctrlTarget.Page.Header.Controls.Add(jsTag);

            HtmlGenericControl cssTag = new HtmlGenericControl("link");
            cssTag.Attributes.Add("type", "text/css");
            cssTag.Attributes.Add("rel", "stylesheet");
            cssTag.Attributes.Add("href", Constants.Resource.DefaultCSS);
            ctrlTarget.Page.Header.Controls.Add(cssTag);
        }
    }
}
