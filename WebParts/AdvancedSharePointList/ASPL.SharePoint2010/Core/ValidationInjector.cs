using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint;
using System.Web.UI;
using ASPL.ConfigModel;
using System.Text.RegularExpressions;

namespace ASPL.SharePoint2010.Core
{
    class ValidationInjector
    {
        public static BaseFieldControl GetFieldControlByName(String fieldNameToFind, SPFormContext formContext, string clientID)
        {
            foreach (Control control in formContext.FieldControlCollection)
            {
                if (control is BaseFieldControl)
                {
                    BaseFieldControl baseField = (BaseFieldControl)control;
                    String fieldName = baseField.FieldName;
                    if ((fieldName == fieldNameToFind) &&
                        (GetIteratorByFieldControl(baseField).ClientID == clientID))
                    {
                        return baseField;
                    }
                }
            }
            return null;
        }

        public static void SetValidationError(BaseFieldControl fieldControl, String errorMessage)
        {
            fieldControl.ErrorMessage = errorMessage;
            fieldControl.IsValid = false;
        }

        public static Microsoft.SharePoint.WebControls.ListFieldIterator GetIteratorByFieldControl(BaseFieldControl fieldControl)
        {
            return (Microsoft.SharePoint.WebControls.ListFieldIterator)fieldControl.Parent.Parent.Parent.Parent.Parent;
        }

        public static bool InvalidColumnValue(object fieldValue, Enums.Operator op, string valueToCompare, Type fieldValueType)
        {
            return ConditionEvaluator.MatchItemValueBasedOnOperatorAndValueType(op, valueToCompare, fieldValue, fieldValueType);
        }

        public static bool InvalidLengthValue(int length, Enums.Operator op, string lengthToCompare)
        {
            int intlengthToCompare;

            if (int.TryParse(lengthToCompare, out intlengthToCompare))
            {
                switch (op)
                {
                    case Enums.Operator.Equal:
                        return length == intlengthToCompare;
                    case Enums.Operator.NotEqual:
                        return length != intlengthToCompare; ;
                    case Enums.Operator.GreaterThan:
                        return length > intlengthToCompare;
                    case Enums.Operator.LessThan:
                        return length < intlengthToCompare;
                    default:
                        return false;
                }
            }
            else
            {
                return true;
            }
        }

        public static bool InvalidPatternValue(string value, string pattern)
        {
            return !Regex.IsMatch(value, pattern, RegexOptions.None);
        }
    }
}
