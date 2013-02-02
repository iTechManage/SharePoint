using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.SharePoint.Utilities;
using ASPL.Blocks;

namespace ASPL.ConfigModel
{
    public class FieldDefault
    {
        public Field OnField { get; set; }
        public string ForSPPrinciples { get; set; }
        public Enums.Operator BySPPrinciplesOperator { get; set; }
        public string SPContentType { get; set; }
        public object Value { get; set; }

        internal FieldDefault(XmlNode fieldDefaultNode, int defaultValueIndex)
        {
            string parentXPath = string.Format("/fielddefaults/default[{0}]", defaultValueIndex);
            this.OnField = new Field(fieldDefaultNode.SelectSingleNode(parentXPath + "/onfield").InnerText);
            this.Value = Helper.HtmlDecode(fieldDefaultNode.SelectSingleNode(parentXPath + "/value").InnerText);

            this.BySPPrinciplesOperator =
                (Enums.Operator)Enum.Parse(
                typeof(Enums.Operator),
                fieldDefaultNode.SelectSingleNode(parentXPath + "/forprinciples").
                InnerText.Split(Constants.XmlElementTextSeparator.ToCharArray(),
                StringSplitOptions.RemoveEmptyEntries)[0],
                true);

            this.ForSPPrinciples =
                fieldDefaultNode.SelectSingleNode(parentXPath + "/forprinciples").InnerText.Split(
                Constants.XmlElementTextSeparator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[1];

            this.SPContentType = fieldDefaultNode.SelectSingleNode(parentXPath + "/ctype").InnerText;
        }

        public FieldDefault(Field onField, string forSPPrinciples,
            Enums.Operator bySPPrinciplesOperator, string spContentType, object value)
        {
            this.OnField = onField;
            this.ForSPPrinciples = forSPPrinciples;
            this.BySPPrinciplesOperator = bySPPrinciplesOperator;
            this.SPContentType = spContentType;
            this.Value = value;
        }

        public override string ToString()
        {
            return string.Format(
                "<default><onfield>{0}</onfield><forprinciples>{1}{5}{2}</forprinciples><value>{3}</value><ctype>{4}</ctype></default>",
                            this.OnField.SPName,//0
                            this.BySPPrinciplesOperator,//1
                            this.ForSPPrinciples,//2
                            Helper.HtmlEncode(this.Value.ToString()),//3
                            this.SPContentType,//4
                            Constants.XmlElementTextSeparator//5
                            );
        }
    }

    public class FieldDefaults : List<FieldDefault>
    {

        public static FieldDefaults LoadFieldDefaults(XmlDocument xmlFieldDefaults)
        {
            if (xmlFieldDefaults == null) return null;

            FieldDefaults fieldDefaults = new FieldDefaults();

            XmlNodeList xmlFieldValidationNodes = xmlFieldDefaults.SelectNodes("/fielddefaults/default");

            int defValindex = 1;
            foreach (XmlNode node in xmlFieldValidationNodes)
            {
                FieldDefault f = new FieldDefault(node, defValindex);
                fieldDefaults.Add(f);
                defValindex++;
            }

            return fieldDefaults;
        }

        public override string ToString()
        {
            string str = string.Empty;
            foreach (FieldDefault item in this)
            {
                str += item.ToString();
            }

            return string.Format("<fielddefaults>{0}</fielddefaults>", str);
        }
    }
}
