using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.SharePoint.Utilities;
using ASPL.Blocks;
using Microsoft.SharePoint;

namespace ASPL.ConfigModel
{
    public class Condition
    {
        internal Condition() { }

        public Condition(Field onField, Enums.Operator byFieldOperator,object value)
        {
            this.OnField = onField;
            this.ByFieldOperator = byFieldOperator;
            this.Value = value;
        }

        public Field OnField { get; set; }
        public Enums.Operator ByFieldOperator { get; set; }
        public object Value { get; set; }

        public override string ToString()
        {
            return string.Format("<condition>{0}{3}{1}{3}{2}</condition>", this.OnField.SPName, this.ByFieldOperator.ToString(), Helper.HtmlEncode(this.Value.ToString()), Constants.XmlElementTextSeperator);
        }
    }

    public class Conditions : List<Condition>
    {
        public string ConditionsToString(SPList list)
        {
            string conditions="";
            foreach (Condition cond in this)
            {
                if (list.Fields.ContainsField(cond.OnField.SPName))
                    conditions += list.Fields.GetFieldByInternalName(cond.OnField.SPName).Title + " " + Enums.DisplayString(cond.ByFieldOperator) + " " + (cond.Value ?? "").ToString() + " AND ";
            }

            return conditions.TrimEnd(" AND ".ToCharArray());
        }
        public static Conditions LoadConditions(XmlNodeList node)
        {
            Conditions conditions = new Conditions();
            foreach (XmlNode childNode in node)
            {
                if (childNode.Name.Equals("condition", StringComparison.InvariantCultureIgnoreCase) && !string.IsNullOrEmpty(childNode.InnerText))
                {
                    Condition c = new Condition()
                    {
                        OnField = new Field(childNode.InnerText.Split(Constants.XmlElementTextSeperator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[0]),
                        ByFieldOperator = (Enums.Operator)Enum.Parse(typeof(Enums.Operator), childNode.InnerText.Split(Constants.XmlElementTextSeperator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[1], true),
                        Value = Helper.HtmlDecode(childNode.InnerText.Split(Constants.XmlElementTextSeperator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[2])
                    };
                    conditions.Add(c);               
                }
            }

            return conditions;
        }
        public override string ToString()
        {
            string str = string.Empty;
            foreach (Condition item in this)
            {
                str += item.ToString();
            }
            return string.Format("<conditions>{0}</conditions>", str);
        }
    }
}
