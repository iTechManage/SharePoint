using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.SharePoint.Utilities;
using ASPL.Blocks;

namespace ASPL.ConfigModel
{
    public class FieldValidation
    {

        internal FieldValidation(XmlNode fieldValidationNode, int valIndex)
        {
            string parentXPath = string.Format("/fieldvalidations/fieldvalidation[{0}]", valIndex);

            this.OnField = new Field(fieldValidationNode.SelectSingleNode(parentXPath + "/onfield").InnerText);

            this.Rule = (Enums.ValidationRule)Enum.Parse(typeof(Enums.ValidationRule),
                fieldValidationNode.SelectSingleNode(parentXPath + "/rule").
                    InnerText.Split(Constants.XmlElementTextSeparator.ToCharArray(),
                    StringSplitOptions.RemoveEmptyEntries)[0], true);

            this.ByRuleOperator = (Enums.Operator)Enum.Parse(typeof(Enums.Operator),
                fieldValidationNode.SelectSingleNode(parentXPath + "/rule").
                    InnerText.Split(Constants.XmlElementTextSeparator.ToCharArray(),
                    StringSplitOptions.RemoveEmptyEntries)[1], true);

            this.Value = Helper.HtmlDecode(fieldValidationNode.SelectSingleNode(parentXPath + "/rule").
                InnerText.Split(Constants.XmlElementTextSeparator.ToCharArray(),
                StringSplitOptions.RemoveEmptyEntries)[2]);

            this.ErrorMsg = Helper.HtmlDecode(fieldValidationNode.SelectSingleNode(parentXPath + "/rule").
                InnerText.Split(Constants.XmlElementTextSeparator.ToCharArray(),
                StringSplitOptions.RemoveEmptyEntries)[3]);

            this.BySPPrinciplesOperator = (Enums.Operator)Enum.Parse(typeof(Enums.Operator),
                fieldValidationNode.SelectSingleNode(parentXPath + "/forprinciples").
                    InnerText.Split(Constants.XmlElementTextSeparator.ToCharArray(),
                    StringSplitOptions.RemoveEmptyEntries)[0], true);

            this.ForSPPrinciples = fieldValidationNode.SelectSingleNode(parentXPath + "/forprinciples").
                InnerText.Split(Constants.XmlElementTextSeparator.ToCharArray(),
                StringSplitOptions.RemoveEmptyEntries)[1];

            this.Conditions =
                Conditions.LoadConditions(fieldValidationNode.SelectNodes(parentXPath + "/conditions/condition"));
        }

        public FieldValidation(Field onField, Enums.ValidationRule rule, Enums.Operator byRuleOperator, object value, string errorMsg, string forSPPrinciples, Enums.Operator bySPPrinciplesOperator)
        {
            this.OnField = onField;
            this.Rule = rule;
            this.ByRuleOperator = byRuleOperator;
            this.Value = value;
            this.ErrorMsg = errorMsg;
            this.ForSPPrinciples = forSPPrinciples;
            this.BySPPrinciplesOperator = bySPPrinciplesOperator;
            this.Conditions = new Conditions();
        }

        public Field OnField { get; set; }
        public Enums.ValidationRule Rule { get; set; }
        public Enums.Operator ByRuleOperator { get; set; }
        public object Value { get; set; }
        public string ErrorMsg { get; set; }

        public string ForSPPrinciples { get; set; }
        public Enums.Operator BySPPrinciplesOperator { get; set; }

        public bool isConditional { get { return this.Conditions.Count > 0; } }
        public Conditions Conditions { get; set; }

        public override string ToString()
        {
            return string.Format(
                @"<fieldvalidation><onfield>{0}</onfield><rule>{1}{8}{2}{8}{3}{8}{4}</rule><forprinciples>{5}{8}{6}</forprinciples>{7}</fieldvalidation>",
                this.OnField.SPName,//0
                this.Rule.ToString(),//1
                this.ByRuleOperator.ToString(),//2
                Helper.HtmlEncode(this.Value.ToString()),//3
                Helper.HtmlEncode(this.ErrorMsg),//4
                this.BySPPrinciplesOperator.ToString(),//5
                this.ForSPPrinciples,//6
                this.Conditions.ToString(),//7
                Constants.XmlElementTextSeparator//8
                );
        }
    }

    public class FieldValidations : List<FieldValidation>
    {
        public static FieldValidations LoadFieldValidations(XmlDocument xmlFieldValidations)
        {
            if (xmlFieldValidations == null) return null;
            FieldValidations fieldValidations = new FieldValidations();

            XmlNodeList xmlFieldValidationNodes = xmlFieldValidations.SelectNodes("/fieldvalidations/fieldvalidation");

            int index = 1;

            foreach (XmlNode node in xmlFieldValidationNodes)
            {
                FieldValidation f = new FieldValidation(node, index);
                fieldValidations.Add(f);
                index++;
            }

            return fieldValidations;
        }

        public override string ToString()
        {
            string str = string.Empty;
            foreach (FieldValidation item in this)
            {
                str += item.ToString();
            }

            return string.Format("<fieldvalidations>{0}</fieldvalidations>", str);
        }
    }
}
