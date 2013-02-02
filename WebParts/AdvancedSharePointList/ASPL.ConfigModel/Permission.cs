using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using ASPL.Blocks;

namespace ASPL.ConfigModel
{

    public class PermissionBase
    {
        internal PermissionBase() { }

        public PermissionBase(Enums.PermissionLevel level, List<Enums.SPForms> onForms,
            string forSPPrinciples, Enums.Operator bySPPrinciplesOperator)
        {
            this.Level = level;

            this.OnForms = onForms;
            this.ForSPPrinciples = forSPPrinciples;
            this.BySPPrinciplesOperator = bySPPrinciplesOperator;

            this.Conditions = new Conditions();
        }

        public Enums.PermissionLevel Level { get; set; }
        public List<Enums.SPForms> OnForms { get; set; }

        public string ForSPPrinciples { get; set; }
        public Enums.Operator BySPPrinciplesOperator { get; set; }

        public bool IsConditional { get { return this.Conditions.Count > 0; } }
        public Conditions Conditions { get; set; }

        public static List<Enums.SPForms> ParseForms(string forms)
        {
            List<Enums.SPForms> OnForms = new List<Enums.SPForms>();
            foreach (string f in
                forms.Split(Constants.EnumValueSeparator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
            {
                if (!string.IsNullOrEmpty(f))
                {
                    OnForms.Add((Enums.SPForms)Convert.ToInt32(f));
                }
            }

            return OnForms;
        }

        public void LoadForms(string forms)
        {
            this.OnForms = new List<Enums.SPForms>();
            foreach (string f in
                forms.Split(Constants.EnumValueSeparator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
            {
                if (!string.IsNullOrEmpty(f))
                {
                    this.OnForms.Add((Enums.SPForms)Convert.ToInt32(f));
                }
            }
        }

        public string FormsToString()
        {
            string value = "";

            foreach (Enums.SPForms f in this.OnForms)
            {
                value += Enums.DisplayString(f) + Constants.EnumValueSeparator;
            }

            return value.Trim(Constants.EnumValueSeparator.ToCharArray());

        }

        public static string FormsToString(List<Enums.SPForms> onForms)
        {
            string value = "";

            foreach (Enums.SPForms f in onForms)
            {
                value += Enums.DisplayString(f) + Constants.EnumValueSeparator;
            }

            return value.Trim(Constants.EnumValueSeparator.ToCharArray());
        }

        public string FormsIdToString()
        {
            string value = "";

            foreach (Enums.SPForms f in this.OnForms)
            {
                value += ((int)f).ToString() + Constants.EnumValueSeparator;
            }

            return value.Trim(Constants.EnumValueSeparator.ToCharArray());

        }

        public static string FormsIdToString(List<Enums.SPForms> onForms)
        {
            string value = "";

            foreach (Enums.SPForms f in onForms)
            {
                value += ((int)f).ToString() + Constants.EnumValueSeparator;
            }

            return value.Trim(Constants.EnumValueSeparator.ToCharArray());
        }
    }

    public class FieldPermission : PermissionBase
    {
        internal FieldPermission(XmlNode fieldPermissionNode, int permIndex)
        {
            string parentXPath = string.Format("/fieldpermissions/fieldpermission[{0}]", permIndex);

            this.OnField = new Field(fieldPermissionNode.SelectSingleNode(parentXPath + "/onfield").InnerText);

            this.Level =
                (Enums.PermissionLevel)Enum.Parse(typeof(Enums.PermissionLevel),
                fieldPermissionNode.SelectSingleNode(parentXPath + "/level").InnerText, true);

            this.OnForms =
                PermissionBase.ParseForms(fieldPermissionNode.SelectSingleNode(parentXPath + "/onforms").InnerText);

            this.BySPPrinciplesOperator =
                (Enums.Operator)Enum.Parse(typeof(Enums.Operator),
                fieldPermissionNode.SelectSingleNode(parentXPath + "/forprinciples").
                    InnerText.Split(Constants.XmlElementTextSeparator.ToCharArray(),
                    StringSplitOptions.RemoveEmptyEntries)[0], true);

            this.ForSPPrinciples = fieldPermissionNode.SelectSingleNode(parentXPath + "/forprinciples").
                InnerText.Split(Constants.XmlElementTextSeparator.ToCharArray(),
                StringSplitOptions.RemoveEmptyEntries)[1];

            this.Conditions =
                Conditions.LoadConditions(fieldPermissionNode.SelectNodes(parentXPath + "/conditions/condition"));
        }

        public FieldPermission(Field OnField, Enums.PermissionLevel level, List<Enums.SPForms> onForms, string forSPPrinciples, Enums.Operator bySPPrinciplesOperator)
            : base(level, onForms, forSPPrinciples, bySPPrinciplesOperator)
        {
            this.OnField = OnField;
        }

        public Field OnField { get; set; }

        public override string ToString()
        {
            return string.Format(
                "<fieldpermission><onfield>{0}</onfield><level>{1}</level><onforms>{2}</onforms><forprinciples>{3}{6}{4}</forprinciples>{5}</fieldpermission>",
                this.OnField.SPName,//0
                this.Level.ToString(),//1
                this.FormsIdToString(),//2
                this.BySPPrinciplesOperator.ToString(),//3
                this.ForSPPrinciples,//4
                this.Conditions.ToString(),//5
                Constants.XmlElementTextSeparator//6
                );
        }
    }

    public class FieldPermissions : List<FieldPermission>
    {
        public FieldPermission IsForAllFields
        {
            get
            {
                foreach (FieldPermission f in this)
                {
                    if (f.OnField.SPName.Equals("*"))
                    {
                        return f;
                    }
                }

                return null;
            }
        }

        public static FieldPermissions LoadFieldPermissions(XmlDocument xmlFieldPermissions)
        {
            if (xmlFieldPermissions == null) return null;

            FieldPermissions fieldPermissions = new FieldPermissions();

            XmlNodeList xmlFieldPermissionNodes = xmlFieldPermissions.SelectNodes("/fieldpermissions/fieldpermission");

            int index = 1;

            foreach (XmlNode node in xmlFieldPermissionNodes)
            {
                FieldPermission f = new FieldPermission(node, index);
                fieldPermissions.Add(f);
                index++;
            }

            return fieldPermissions;
        }

        public override string ToString()
        {
            string str = string.Empty;
            foreach (FieldPermission item in this)
            {
                str += item.ToString();
            }

            return string.Format("<fieldpermissions>{0}</fieldpermissions>", str);
        }
    }

    public class TabPermission : PermissionBase
    {

        internal TabPermission(XmlNode tabPermissionNode, int tabIndex, int permIndex)
        {
            string parentXPath = string.Format("/tabs/tab[{0}]/permissions/permission[{1}]", tabIndex, permIndex);

            this.IsDefault =
                Helper.ConvertToBool(tabPermissionNode.SelectSingleNode(parentXPath + "/isdefault").InnerText);

            this.Level =
                (Enums.PermissionLevel)Enum.Parse(typeof(Enums.PermissionLevel),
                tabPermissionNode.SelectSingleNode(parentXPath + "/level").InnerText, true);

            this.OnForms =
                TabPermission.ParseForms(tabPermissionNode.SelectSingleNode(parentXPath + "/onforms").InnerText);

            this.BySPPrinciplesOperator = (Enums.Operator)Enum.Parse(typeof(Enums.Operator),
                tabPermissionNode.SelectSingleNode(parentXPath + "/forprinciples").
                    InnerText.Split(Constants.XmlElementTextSeparator.ToCharArray(),
                    StringSplitOptions.RemoveEmptyEntries)[0], true);

            this.ForSPPrinciples =
                tabPermissionNode.SelectSingleNode(parentXPath + "/forprinciples").
                InnerText.Split(Constants.XmlElementTextSeparator.ToCharArray(),
                StringSplitOptions.RemoveEmptyEntries)[1];

            this.Conditions =
                Conditions.LoadConditions(tabPermissionNode.SelectNodes(parentXPath + "/conditions/condition"));
        }

        public TabPermission(bool isDefault, Enums.PermissionLevel level,
            List<Enums.SPForms> onForms, string forSPPrinciples, Enums.Operator bySPPrinciplesOperator)
            : base(level, onForms, forSPPrinciples, bySPPrinciplesOperator)
        {
            this.IsDefault = isDefault;
        }

        public bool IsDefault { get; set; }

        public override string ToString()
        {
            return string.Format(
                "<permission><isdefault>{0}</isdefault><level>{1}</level><onforms>{2}</onforms><forprinciples>{3}{6}{4}</forprinciples>{5}</permission>",
                this.IsDefault.ToString(),//0
                this.Level.ToString(),//1
                this.FormsIdToString(),//2
                this.BySPPrinciplesOperator.ToString(),//3
                this.ForSPPrinciples,//4
                this.Conditions.ToString(),//5
                Constants.XmlElementTextSeparator//6
                );
        }
    }

    public class TabPermissions : List<TabPermission>
    {
        public override string ToString()
        {
            string str = string.Empty;
            foreach (TabPermission item in this)
            {
                str += item.ToString();
            }

            return string.Format("<permissions>{0}</permissions>", str);
        }
    }
}
