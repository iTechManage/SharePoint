using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint;



namespace CrowCanyon.CascadedLookup
{
    class CCSCascadedLookupControl : BaseFieldControl
    {
        protected override string DefaultTemplateName { get { return "CCSCascadeFieldControl"; } }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }
    }
}
