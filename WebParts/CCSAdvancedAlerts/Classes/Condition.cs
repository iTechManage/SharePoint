using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.SharePoint;

namespace CCSAdvancedAlerts{
    class Condition
    {
        public const string ValueCollectionSeperator = ";";

        private string fieldName;
        internal string FieldName
        {
            get { return fieldName; }
            set { fieldName = value; }
        }

        private Operators comparisionOperator;
        public Operators ComparisionOperator
        {
            get { return comparisionOperator; }
            set { comparisionOperator = value; }
        }

        private string strValue;
        internal string StrValue
        {
            get { return strValue; }
            set { strValue = value; }
        }

        //private string whenToSend;
        //internal string WhenToSend
        //{
        //    get { return whenToSend; }
        //    set { whenToSend = value; }
        //}

        public Condition(string xNode)
        {
            if (!string.IsNullOrEmpty(xNode))
            {
                XmlDocument xDoc = new XmlDocument();
                xDoc.LoadXml(xNode);
                BuildConditionFromXML(xDoc.DocumentElement);
            }
        }

        public Condition(XmlNode xNode)
        {
            if(xNode != null)
            BuildConditionFromXML(xNode as XmlElement );
        }


        private void BuildConditionFromXML(XmlElement xmlElement)
        {
            try
            {
                this.FieldName = xmlElement.GetAttribute("Field");
                this.comparisionOperator = (Operators)Enum.Parse(typeof(Operators), xmlElement.GetAttribute("Operator"));
                this.strValue = xmlElement.GetAttribute("Value");
            }
            catch {  }
        }


        #region Condition Evaluation

        internal bool isValid(SPListItem item, AlertEventType eventType)
        {
            SPList list = item.ParentList;
             if (list == null)
               return false;
             SPField field = list.Fields.TryGetFieldByStaticName(this.fieldName);
             if (field != null)
             {
              return   MatchItemValueBasedOnOperatorAndValueType(item[this.fieldName], field.FieldValueType, eventType);
             }
             return false;
        }

        public bool MatchItemValueBasedOnOperatorAndValueType(object fieldValue,Type fieldValueType, AlertEventType eventType)
        {

            if (fieldValue != null && !string.IsNullOrEmpty(fieldValue.ToString()))
            {
                if (fieldValueType == (typeof(SPFieldUrlValue)))
                {
                    SPFieldUrlValue fieldUrlValue = new SPFieldUrlValue(fieldValue.ToString());
                    bool isDescMatched = CompareValuesBasedOnOperator(fieldUrlValue.Description, this.comparisionOperator,this.strValue);
                    bool isUrlMatched = CompareValuesBasedOnOperator(fieldUrlValue.Url, this.comparisionOperator, strValue);

                    return isDescMatched || isUrlMatched;
                }
                else if (fieldValueType == (typeof(SPFieldUserValue)))
                {
                    SPFieldUserValue fieldUserValue = new SPFieldUserValue(SPContext.Current.Web, fieldValue.ToString());

                    string userLoginName = fieldUserValue.User.LoginName;
                    string userDispalyName = fieldUserValue.User.Name;

                    bool isLoginMatched = CompareValuesBasedOnOperator(userLoginName, this.comparisionOperator, strValue);
                    bool isDisplayNameMatched = CompareValuesBasedOnOperator(userLoginName, this.comparisionOperator, strValue);

                    return isLoginMatched || isDisplayNameMatched;

                }
                else if (fieldValueType == (typeof(SPFieldUserValueCollection)))
                {
                    SPFieldUserValueCollection fieldUserValueCollection = new SPFieldUserValueCollection(SPContext.Current.Web, fieldValue.ToString());
                    string userLoginNames = "";
                    string userDispalyNames = "";

                    foreach (SPFieldUserValue userValue in fieldUserValueCollection)
                    {
                        userLoginNames += userValue.LookupValue + ValueCollectionSeperator;

                        if (userValue.User != null)
                            userDispalyNames += userValue.User.Name + ValueCollectionSeperator;

                    }

                    userLoginNames = userLoginNames.TrimEnd(ValueCollectionSeperator.ToCharArray());
                    userDispalyNames = userDispalyNames.TrimEnd(ValueCollectionSeperator.ToCharArray());

                    bool isLoginMatched = CompareValuesBasedOnOperator(userLoginNames, this.comparisionOperator, strValue);
                    bool isDisplayNameMatched = CompareValuesBasedOnOperator(userLoginNames, this.comparisionOperator, strValue);

                    return isLoginMatched || isDisplayNameMatched;



                }
                else if (fieldValueType == (typeof(SPFieldLookupValue)))
                {
                    SPFieldLookupValue fieldLookupValue = new SPFieldLookupValue(fieldValue.ToString());

                    string strFieldValue = fieldLookupValue.LookupValue;
                    return CompareValuesBasedOnOperator(strFieldValue, this.comparisionOperator, strValue);
                }
                else if (fieldValueType == (typeof(SPFieldLookupValueCollection)))
                {
                    SPFieldLookupValueCollection fieldLookupValueCollection = new SPFieldLookupValueCollection(fieldValue.ToString());
                    string strFieldValue = "";

                    foreach (SPFieldLookupValue lookup in fieldLookupValueCollection)
                    {
                        strFieldValue += lookup.LookupValue + ValueCollectionSeperator;
                    }

                    strFieldValue = strFieldValue.TrimEnd(ValueCollectionSeperator.ToCharArray());
                    return CompareValuesBasedOnOperator(strFieldValue, this.comparisionOperator, strValue);
                }
                else if (fieldValueType == (typeof(DateTime)))
                {
                    DateTime sourceDT = DateTime.Parse(fieldValue.ToString());
                    DateTime targetDT = new DateTime();
                    if (DateTime.TryParse(strValue, out targetDT))
                    {
                        switch (this.comparisionOperator)
                        {

                            case Operators.Eq:
                                return sourceDT == targetDT;
                            case Operators.Neq:
                                return sourceDT != targetDT;
                            case Operators.Gt:
                                return sourceDT > targetDT;
                            case Operators.Geq:
                                return sourceDT >= targetDT;
                            case Operators.Lt:
                                return sourceDT < targetDT;
                            case Operators.Leq:
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
                    if (Int32.TryParse(strValue, out targetInt))
                    {
                        switch (this.comparisionOperator)
                        {

                            case Operators.Eq:
                                return sourceInt == targetInt;
                            case Operators.Neq:
                                return sourceInt != targetInt;
                            case Operators.Gt:
                                return sourceInt > targetInt;
                            case Operators.Geq:
                                return sourceInt >= targetInt;
                            case Operators.Lt:
                                return sourceInt < targetInt;
                            case Operators.Leq:
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

                    if (strValue.Equals("True", StringComparison.InvariantCultureIgnoreCase) || strValue.Equals("Yes", StringComparison.InvariantCultureIgnoreCase))
                    {
                        targetBool = true;
                    }
                    else if (strValue.Equals("False", StringComparison.InvariantCultureIgnoreCase) || strValue.Equals("No", StringComparison.InvariantCultureIgnoreCase))
                    {
                        targetBool = false;
                    }
                    else
                    {
                        return false;
                    }

                    switch (this.comparisionOperator)
                    {

                        case Operators.Eq:
                            return sourceBool == targetBool;
                        case Operators.Neq:
                            return sourceBool != targetBool;
                        case Operators.Contains:
                            return sourceBool == targetBool;
                        case Operators.NotContains:
                            return sourceBool != targetBool;
                        default:
                            return false;
                    }
                }
                else // default matching will be performed with string type
                {
                    string strFieldValue = fieldValue.ToString();
                    return CompareValuesBasedOnOperator(strFieldValue, this.comparisionOperator, strValue);
                }
            }
            else
            {
                return false;
            }

        }

        private static bool CompareValuesBasedOnOperator(string sourceValue, Operators op, string targetValue)
        {
            sourceValue = sourceValue.Trim();
            targetValue = targetValue.Trim();
            switch (op)
            {

                case Operators.Eq:
                    return sourceValue.Equals(targetValue, StringComparison.InvariantCultureIgnoreCase);
                case Operators.Neq:
                    return !sourceValue.Equals(targetValue, StringComparison.InvariantCultureIgnoreCase);
                case Operators.Contains:
                    return sourceValue.IndexOf(targetValue, StringComparison.InvariantCultureIgnoreCase) > -1;
                case Operators.NotContains:
                    return !(sourceValue.IndexOf(targetValue, StringComparison.InvariantCultureIgnoreCase) > -1);
                default:
                    return false;
            }
        }

        #endregion Condition Evaluation
    }




}
