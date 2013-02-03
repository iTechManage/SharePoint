using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASPL.ConfigModel;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using ASPL.Blocks;

namespace ASPL.SharePoint2010.Core
{
    class ConditionEvaluator
    {
        //TODO: check the values based on column type for below types..

        private static bool CheckFromListItem(string fieldName,
            Enums.Operator op, string value)
        {
            if (SPContext.Current.List.Fields.ContainsField(fieldName))
            {
                object fieldValue = SPContext.Current.ListItem[fieldName];
                Type fieldValueType =
                    SPContext.Current.List.Fields.GetFieldByInternalName(fieldName).
                    FieldValueType;

                return MatchItemValueBasedOnOperatorAndValueType(op, value,
                    fieldValue, fieldValueType);
            }
            else
            {
                return true;
            }
        }

        public static bool MatchItemValueBasedOnOperatorAndValueType(Enums.Operator op, string value, object fieldValue, Type fieldValueType)
        {
            if (fieldValue != null && !string.IsNullOrEmpty(fieldValue.ToString()))
            {
                if (fieldValueType == (typeof(SPFieldUrlValue)))
                {
                    SPFieldUrlValue fieldUrlValue = new SPFieldUrlValue(fieldValue.ToString());
                    bool isDescMatched = CompareValuesBasedOnOperator(fieldUrlValue.Description, op, value);
                    bool isUrlMatched = CompareValuesBasedOnOperator(fieldUrlValue.Url, op, value);

                    return isDescMatched || isUrlMatched;
                }
                else if (fieldValueType == (typeof(SPFieldUserValue)))
                {
                    SPFieldUserValue fieldUserValue = new SPFieldUserValue(SPContext.Current.Web, fieldValue.ToString());

                    string userLoginName = fieldUserValue.User.LoginName;
                    string userDispalyName = fieldUserValue.User.Name;

                    bool isLoginMatched = CompareValuesBasedOnOperator(userLoginName, op, value);
                    bool isDisplayNameMatched = CompareValuesBasedOnOperator(userLoginName, op, value);

                    return isLoginMatched || isDisplayNameMatched;
                }
                else if (fieldValueType == (typeof(SPFieldUserValueCollection)))
                {
                    SPFieldUserValueCollection fieldUserValueCollection = new SPFieldUserValueCollection(SPContext.Current.Web, fieldValue.ToString());
                    string userLoginNames = "";
                    string userDispalyNames = "";

                    foreach (SPFieldUserValue userValue in fieldUserValueCollection)
                    {
                        userLoginNames += userValue.LookupValue + Constants.ValueCollectionSeparator;

                        if (userValue.User != null)
                            userDispalyNames += userValue.User.Name + Constants.ValueCollectionSeparator;
                    }

                    userLoginNames = userLoginNames.TrimEnd(Constants.ValueCollectionSeparator.ToCharArray());
                    userDispalyNames = userDispalyNames.TrimEnd(Constants.ValueCollectionSeparator.ToCharArray());

                    bool isLoginMatched = CompareValuesBasedOnOperator(userLoginNames, op, value);
                    bool isDisplayNameMatched = CompareValuesBasedOnOperator(userLoginNames, op, value);

                    return isLoginMatched || isDisplayNameMatched;
                }
                else if (fieldValueType == (typeof(SPFieldLookupValue)))
                {
                    SPFieldLookupValue fieldLookupValue = new SPFieldLookupValue(fieldValue.ToString());

                    string strFieldValue = fieldLookupValue.LookupValue;
                    return CompareValuesBasedOnOperator(strFieldValue, op, value);
                }
                else if (fieldValueType == (typeof(SPFieldLookupValueCollection)))
                {
                    SPFieldLookupValueCollection fieldLookupValueCollection = new SPFieldLookupValueCollection(fieldValue.ToString());
                    string strFieldValue = "";

                    foreach (SPFieldLookupValue lookup in fieldLookupValueCollection)
                    {
                        strFieldValue += lookup.LookupValue + Constants.ValueCollectionSeparator;
                    }

                    strFieldValue = strFieldValue.TrimEnd(Constants.ValueCollectionSeparator.ToCharArray());
                    return CompareValuesBasedOnOperator(strFieldValue, op, value);
                }
                else if (fieldValueType == (typeof(DateTime)))
                {
                    DateTime sourceDT = DateTime.Parse(fieldValue.ToString());
                    DateTime targetDT = new DateTime();
                    if (DateTime.TryParse(value, out targetDT))
                    {
                        switch (op)
                        {
                            case Enums.Operator.Equal:
                                return sourceDT == targetDT;
                            case Enums.Operator.NotEqual:
                                return sourceDT != targetDT;
                            case Enums.Operator.GreaterThan:
                                return sourceDT > targetDT;
                            case Enums.Operator.GreaterThanOrEqual:
                                return sourceDT >= targetDT;
                            case Enums.Operator.LessThan:
                                return sourceDT < targetDT;
                            case Enums.Operator.LessThanOrEqual:
                                return sourceDT <= targetDT;
                            default:
                                return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (fieldValueType == (typeof(int)))
                {
                    int sourceInt = int.Parse(fieldValue.ToString());
                    int targetInt;
                    if (Int32.TryParse(value, out targetInt))
                    {
                        switch (op)
                        {
                            case Enums.Operator.Equal:
                                return sourceInt == targetInt;
                            case Enums.Operator.NotEqual:
                                return sourceInt != targetInt;
                            case Enums.Operator.GreaterThan:
                                return sourceInt > targetInt;
                            case Enums.Operator.GreaterThanOrEqual:
                                return sourceInt >= targetInt;
                            case Enums.Operator.LessThan:
                                return sourceInt < targetInt;
                            case Enums.Operator.LessThanOrEqual:
                                return sourceInt <= targetInt;
                            default:
                                return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (fieldValueType == (typeof(Boolean)))
                {
                    bool sourceBool = Boolean.Parse(fieldValue.ToString());
                    bool targetBool = false;

                    if (value.Equals("True", StringComparison.InvariantCultureIgnoreCase) || value.Equals("Yes", StringComparison.InvariantCultureIgnoreCase))
                    {
                        targetBool = true;
                    }
                    else if (value.Equals("False", StringComparison.InvariantCultureIgnoreCase) || value.Equals("No", StringComparison.InvariantCultureIgnoreCase))
                    {
                        targetBool = false;
                    }
                    else
                    {
                        return false;
                    }

                    switch (op)
                    {
                        case Enums.Operator.Equal:
                            return sourceBool == targetBool;
                        case Enums.Operator.NotEqual:
                            return sourceBool != targetBool;
                        case Enums.Operator.Contains:
                            return sourceBool == targetBool;
                        case Enums.Operator.NotContains:
                            return sourceBool != targetBool;
                        default:
                            return false;
                    }
                }
                else // default matching will be performed with string type
                {
                    string strFieldValue = fieldValue.ToString();
                    return CompareValuesBasedOnOperator(strFieldValue, op, value);
                }
            }
            else
            {
                return false;
            }
        }

        private static bool CompareValuesBasedOnOperator(string sourceValue,
            Enums.Operator op, string targetValue)
        {
            sourceValue = sourceValue.Trim();
            targetValue = targetValue.Trim();
            switch (op)
            {
                case Enums.Operator.Equal:
                    return sourceValue.Equals(targetValue, StringComparison.InvariantCultureIgnoreCase);
                case Enums.Operator.NotEqual:
                    return !sourceValue.Equals(targetValue, StringComparison.InvariantCultureIgnoreCase);
                case Enums.Operator.Contains:
                    return sourceValue.IndexOf(targetValue, StringComparison.InvariantCultureIgnoreCase) > -1;
                case Enums.Operator.NotContains:
                    return !(sourceValue.IndexOf(targetValue, StringComparison.InvariantCultureIgnoreCase) > -1);
                default:
                    return false;
            }
        }

        private static bool CheckFromUIValue(string fieldName, Enums.Operator op, string value, SPFormContext formContext, string clientID)
        {
            BaseFieldControl field = ValidationInjector.GetFieldControlByName(fieldName, formContext, clientID);

            // to manage the rich field UI text
            if (field is RichTextField)
            {
                string fieldVaue = ((RichTextField)field).HiddenInput.Value;

                switch (op)
                {
                    case Enums.Operator.Equal:
                        return fieldVaue.Equals(value);
                    case Enums.Operator.NotEqual:
                        return !fieldVaue.Equals(value);
                    case Enums.Operator.Contains:
                        return fieldVaue.Contains(value);
                    case Enums.Operator.NotContains:
                        return !fieldVaue.Contains(value);
                    default:
                        return false;
                }
            }
            else
            {
                if (field is LookupField)
                {
                    LookupField l = field as LookupField;
                    String v = l.Value.ToString();
                }

                return MatchItemValueBasedOnOperatorAndValueType(op, value, field.Value, field.Field.FieldValueType);
            }
        }

        public static bool EvaluateFromListItem(Conditions conditions)
        {
            bool result = true;
            if (SPContext.Current.FormContext.FormMode !=
                Microsoft.SharePoint.WebControls.SPControlMode.New)
            {
                foreach (Condition c in conditions)
                {
                    result = result && CheckFromListItem(c.OnField.SPName, c.ByFieldOperator, c.Value.ToString());
                }
            }

            return result;
        }

        public static bool EvaluateFromUIValue(Conditions conditions,
            SPFormContext formContext, string clientID)
        {
            bool result = true;
            if (SPContext.Current.FormContext.FormMode != Microsoft.SharePoint.WebControls.SPControlMode.Display)
            {
                foreach (Condition c in conditions)
                {
                    result = result && CheckFromUIValue(c.OnField.SPName, c.ByFieldOperator, c.Value.ToString(), formContext, clientID);
                }
            }

            return result;
        }
    }
}
