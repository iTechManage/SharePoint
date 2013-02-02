using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel;

namespace ASPL.ConfigModel
{
    public static class Enums
    {
        public enum PermissionLevel
        {
            [DescriptionAttribute("Read Only")]
            Read = 11,
            [DescriptionAttribute("Write")]
            Write = 12,
            [DescriptionAttribute("Hidden")]
            Deny = 13
        };

        [Flags]
        public enum SPForms
        {
            [DescriptionAttribute("New Form")]
            New = 21,
            [DescriptionAttribute("Edit Form")]
            Edit = 22,
            [DescriptionAttribute("View Form")]
            View = 23
        };


        public enum ValidationRule
        {
            [DescriptionAttribute("Pattern")]
            Pattern = 31,
            [DescriptionAttribute("length")]
            length = 32,
            [DescriptionAttribute("Column")]
            Column = 33,
            [DescriptionAttribute("Invalid")]
            Invalid = 30
        };

        public enum Operator
        {
            [DescriptionAttribute("In")]
            In = 101,
            [DescriptionAttribute("Not in")]
            NotIn = 102,
            [DescriptionAttribute("Equal")]
            Equal = 103,
            [DescriptionAttribute("Not equal")]
            NotEqual = 104,
            [DescriptionAttribute("Contains")]
            Contains = 105,
            [DescriptionAttribute("Not contains")]
            NotContains = 106,
            [DescriptionAttribute("Greater than")]
            GreaterThan = 107,
            [DescriptionAttribute("Greater than or Equal to")]
            GreaterThanOrEqual = 110,
            [DescriptionAttribute("Less than")]
            LessThan = 108,
            [DescriptionAttribute("Less than or Equal to")]
            LessThanOrEqual = 111,
            [DescriptionAttribute(" ")]
            None = 0
        };

        public static string DisplayString(this Enum value)
        {
            //Using reflection to get the field info
            FieldInfo info = value.GetType().GetField(value.ToString());

            if (info != null)
            {
                //Get the Description Attributes
                DescriptionAttribute[] attributes = (DescriptionAttribute[])info.GetCustomAttributes(
                    typeof(DescriptionAttribute), false);

                //Only capture the description attribute if it is a concrete result (i.e. 1 entry)
                if (attributes != null && attributes.Length == 1)
                {
                    return attributes[0].Description;
                }
            }

            return value.ToString();
        }

        public static PermissionLevel ParsePermissionLevel(string value)
        {
            return (PermissionLevel)Enum.Parse(typeof(PermissionLevel), value, true);
        }

        public static SPForms ParseSPForms(string value)
        {
            return (SPForms)Enum.Parse(typeof(SPForms), value, true);
        }

        public static ValidationRule ParseValidationRule(string value)
        {
            return (ValidationRule)Enum.Parse(typeof(ValidationRule), value, true);
        }

        public static Operator ParseOperator(string value)
        {
            return (Enums.Operator)Enum.Parse(typeof(Enums.Operator), value, true);
        }
    }
}
