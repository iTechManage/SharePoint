using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint;


namespace CCSAdvancedAlerts
{
    class AlertJobdefinition: SPJobDefinition
    {
       #region Constructors( SpjobDefinition Implements 3 Constructors)

        public AlertJobdefinition()
            : base()
        { }

        public AlertJobdefinition(string jobName, SPService service, SPServer server, SPJobLockType targetType)
            : base(jobName, service, server, targetType)
        { }

        public AlertJobdefinition(string jobName, SPWebApplication webApplication)
            : base(jobName, webApplication, null, SPJobLockType.Job)
        { this.Title = jobName; }

        #endregion


        #region Override methods
        public override void Execute(Guid targetInstanceId)
        {
            //Get the webapplication on which it is activated
            foreach (SPSite site in base.WebApplication.Sites)
            {

                //Get all the site collections in web application

                //Get all the root level sites in the site collections

                //check do we have Alert pro active for that site collection by checking hidden list if possible feature

                //if yes

                //get the alert from the list based on the time 

                // if we get any alerts then validate and prepare for sending email.
            }
        }
        #endregion

    }
}
