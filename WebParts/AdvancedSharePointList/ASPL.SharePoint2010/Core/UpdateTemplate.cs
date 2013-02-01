using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace ASPL.SharePoint2010.Core
{
    public class UpdateTemplate : ITemplate
    {
        private string AssociatedUpdatePanelID = "";
        public string ProgressHtml = "<div id='progressDiv{0}' style='position:absolute; left:0px; top:0px; height:100%; width:100%; text-align:center; filter:alpha(opacity=70); BACKGROUND-COLOR: #ffffff;opacity:0.7'><img alt='loading' border='0' style='margin-top:80px' src='/_layouts/ASPL.SharePoint2010/Resource/loading.gif' /></div>";
        public string MoveToParentControlScript = "<script>function moveToParentTd(){try{        var progElm = document.getElementById('progressDiv{0}');var elm = progElm.parentElement;var parentTd = elm.parentElement;var moved = false;while( ( parentTd.tagName != 'DIV' || !parentTd.id.startsWith('WebPartWPQ') )&& parentTd.tagName != 'BODY' ){parentTd = parentTd.parentElement;moved = true;}if( parentTd != null )        {            if( moved )parentTd.appendChild(elm);   var jqparentTd = $(parentTd); progElm.style.top = '0px';progElm.style.left = '0px';progElm.style.height = (jqparentTd.height() + 5) + 'px'; progElm.style.width = (jqparentTd.width() + 5) + 'px';        }}catch(x){}}$(document).ready(function(){moveToParentTd();});</script>";

        private UpdateTemplate(string AssociatedUpdatePanelID)
        {
            this.AssociatedUpdatePanelID = AssociatedUpdatePanelID;
        }
        public static UpdateProgress GetUpdateProgress(string AssociatedUpdatePanelID)
        {
            return new UpdateProgress
            {
                ProgressTemplate = new UpdateTemplate(AssociatedUpdatePanelID),
                DisplayAfter = 100,
                DynamicLayout = true,
                AssociatedUpdatePanelID = AssociatedUpdatePanelID
            };
        }
        public void InstantiateIn(Control container)
        {
            container.Controls.Add(new LiteralControl((this.ProgressHtml + this.MoveToParentControlScript).Replace("{0}", this.AssociatedUpdatePanelID)));
        }
    }
}
