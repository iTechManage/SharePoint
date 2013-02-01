using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASPL.ConfigModel;
using Microsoft.SharePoint;
using ASPL.Blocks;

namespace ASPL.SharePoint2010.Core
{
    class PrincipalEvaluator
    {
        public static bool Check(string userName, Enums.Operator op)
        {
            if (string.IsNullOrEmpty(userName) || userName.Equals(Constants.AllSPPrinciples, StringComparison.InvariantCultureIgnoreCase)) return true;

            if (userName.Contains("\\"))
            {
                switch (op)
                {
                    case Enums.Operator.In:
                        return userName.ToLower() == SPContext.Current.Web.CurrentUser.LoginName.ToLower();
                    case Enums.Operator.Equal:
                        return userName.ToLower() == SPContext.Current.Web.CurrentUser.LoginName.ToLower();
                    case Enums.Operator.NotEqual:
                        return userName.ToLower() != SPContext.Current.Web.CurrentUser.LoginName.ToLower();
                    case Enums.Operator.NotIn :
                        return userName.ToLower() != SPContext.Current.Web.CurrentUser.LoginName.ToLower();
                    default: return true;
                }
            }
            else
            {
                if (SPContext.Current.Web.Groups.OfType<SPGroup>().Count(g => g.Name.Equals(userName, StringComparison.InvariantCultureIgnoreCase)) <= 0) return false;

                SPGroup grp = SPContext.Current.Web.Groups[userName];

                switch (op)
                {
                    case Enums.Operator.In  :
                        return grp.ContainsCurrentUser;
                    case Enums.Operator.Equal:
                        return grp.ContainsCurrentUser;
                    case Enums.Operator.NotIn :
                        return !grp.ContainsCurrentUser;
                    case Enums.Operator.NotEqual:
                        return !grp.ContainsCurrentUser;
                    default: return true;
                }

            }


        }
    }
}
