using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using ASPL.ConfigModel;

namespace ASPL.SharePoint2010.Core
{
    class PermissionHandler
    {
        public static SPControlMode Handle(string spInternalName, SPControlMode formMode, Tabs tabs, FieldPermissions fPerms, SPPrincipal princ, out bool isHidden)
        {
            SPControlMode result= SPControlMode.Invalid;
            isHidden = false;

            if (tabs != null)
            {
                foreach (Tab tab in tabs)
                {
                    foreach (TabPermission tp in tab.Permissions)
                    {
                        if (ConditionEvaluator.EvaluateFromListItem(tp.Conditions) && PrincipalEvaluator.Check(tp.ForSPPrinciples, tp.BySPPrinciplesOperator))
                        {
                            if (tab.Fields.Any<Field>(f => f.SPName.Equals(spInternalName, StringComparison.InvariantCultureIgnoreCase)))
                            {
                                if (tp.OnForms.Contains(Enums.SPForms.New) && formMode == SPControlMode.New
                                        || tp.OnForms.Contains(Enums.SPForms.View) && formMode == SPControlMode.Display
                                            || tp.OnForms.Contains(Enums.SPForms.Edit) && formMode == SPControlMode.Edit)
                                {
                                    switch (tp.Level)
                                    {
                                        case Enums.PermissionLevel.Read:
                                            result = SPControlMode.Display;isHidden=false;
                                            break;

                                        case Enums.PermissionLevel.Write:

                                            if (formMode == SPControlMode.Edit)
                                                result = SPControlMode.Edit;
                                            else
                                                result = SPControlMode.New;

                                            isHidden=false;
                                            break;

                                        case Enums.PermissionLevel.Deny:
                                            isHidden = true;
                                            result = SPControlMode.Invalid;
                                            break;

                                    }
                                }
                            }
                        }
                    }
                }
            }
            //FieldPermissionCheck:
            if (fPerms != null)
            {
                foreach (FieldPermission fp in fPerms)
                {
                    if (ConditionEvaluator.EvaluateFromListItem(fp.Conditions) && PrincipalEvaluator.Check(fp.ForSPPrinciples, fp.BySPPrinciplesOperator))
                    {
                        if (fp.OnField.SPName.Equals(spInternalName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            if (fp.OnForms.Contains(Enums.SPForms.New) && formMode == SPControlMode.New
                                    || fp.OnForms.Contains(Enums.SPForms.View) && formMode == SPControlMode.Display
                                        || fp.OnForms.Contains(Enums.SPForms.Edit) && formMode == SPControlMode.Edit)
                            {
                                switch (fp.Level)
                                {
                                    case Enums.PermissionLevel.Read:
                                        result = SPControlMode.Display;isHidden=false;
                                        goto FinishPermissionCheck;

                                    case Enums.PermissionLevel.Write:

                                        if (formMode == SPControlMode.Edit)
                                            result = SPControlMode.Edit;
                                        else
                                            result = SPControlMode.New;
                                        
                                        isHidden=false;
                                        goto FinishPermissionCheck;

                                    case Enums.PermissionLevel.Deny:
                                        isHidden = true;
                                        result = SPControlMode.Invalid;
                                        break;
                                       

                                }
                            }
                        }
                    }
                }
            }
            FinishPermissionCheck:
            return result;
        }
    }
}
