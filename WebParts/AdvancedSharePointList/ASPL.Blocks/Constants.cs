using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASPL.Blocks
{
    public static class Constants
    {

        public const string RowID="RowID";
        public const string FieldToStringSeparator = "]";
        public const string EnumValueSeparator = "|";
        public const string AllSPPrinciples = "All";
        public const string ValueCollectionSeparator = ";";
        public const string XmlElementTextSeparator = "^|";
        
        public static string[] InvalidChar = new string[]{";#","^|"};

        public static class ASPLFeature
        {
            public static string Name = "Advanced SharePoint List Pro";
            public static string FeatureID = "c3d5d648-0f3c-4123-9322-539c91020dc1"; 
        }


        public static class Resource
        {
            public static string DefaultCSS = "/_layouts/ASPL.SharePoint2010/Resource/ASPLDefault.css";
            public static string IteratorJS = "/_layouts/ASPL.SharePoint2010/Resource/ASPLscripts.js";
            public static string JQuery1_7_2_min = "/_layouts/ASPL.SharePoint2010/Resource/jquery-1.7.2.min.js";
            public static string ViewAccessDeniedPage = "ASPL.SharePoint2010/AccessDeniedView.aspx";
        }

        public static class ConfigFile
        {
            public const string TabSettingFile = "ASLPTabSettings.xml";
            public const string FieldPermissionFile = "ASLPFieldPermissions.xml";
            public const string FieldValidationFile = "ASLPFieldValidations.xml";
            public const string FieldDefaultFile = "ASLPFieldDefaults.xml";
            public const string ViewPermissionsFile = "ASLPViewPermissions.xml";
        }

        public static class TabField
        {
            public const string Index = "Index";
            public const string Title = "Title";
            public const string Description = "Description";
            public const string FieldToString = "FieldToString";
            public const string FieldDisplayNameToString = "FieldDisplayNameToString";
            public const string HasPermission = "HasPermission";
            public const string IsDefault = "IsDefault";
        }

        public static class PermissionField
        {
            public const string SPFieldName = "SPFieldName";
            public const string SPFieldDisplayName = "SPFieldDisplayName";
            public const string TabRowID = "TabRowID";
            public const string IsDefault = "IsDefault";
            public const string PermissionName = "PermissionName";
            public const string PermissionID = "PermissionID";
            public const string SPPrinciples = "SPPrinciples";
            public const string SPPrinciplesOperatorID = "SPPrinciplesOperatorID";
            public const string SPPrinciplesOperatorName = "SPPrinciplesOperatorName";
            public const string OnFormNames = "OnFormNames";
            public const string OnFormIDs = "OnFormIDs";
            public const string HasCondition = "HasCondition";
            
        }

        public static class ConditionField
        {
            public const string PermissionRowID = "PermissionRowID";
            public const string ValidationRowID = "ValidationRowID";
            public const string SPFieldName = "SPFieldName";
            public const string SPFieldDisplayName = "SPFieldDisplayName";
            public const string SPFieldOperatorName = "SPFieldOperatorName";
            public const string SPFieldOperatorID = "SPFieldOperatorID";
            public const string Value = "Value";
        }

        public static class ValidationField
        {
            public const string ValidationFieldRowID = "ValidationFieldRowID";
            public const string SPFieldName = "SPFieldName";
            public const string SPFieldDisplayName = "SPFieldDisplayName";
            public const string ValidationRuleName = "ValidationRuleName";
            public const string ValidationRuleID = "ValidationRuleID";
            public const string SPFieldOperatorName = "SPFieldOperatorName";
            public const string SPFieldOperatorID = "SPFieldOperatorID";
            public const string Value = "Value";
            public const string ErrorMessage = "ErrorMessage";
            public const string SPPrinciples = "SPPrinciples";
            public const string SPPrinciplesOperatorID = "SPPrinciplesOperatorID";
            public const string SPPrinciplesOperatorName = "SPPrinciplesOperatorName";
            public const string HasCondition = "HasCondition";
        }

        public static class ViewField
        {

            public const string ViewID = "ViewID";
            public const string View = "View";
            public const string UserGroup = "UserGroup";
            public const string Permission = "Permission";
            public const string IsDefault = "IsDefault";
            public const string IsActionMenu = "IsActionMenu";
            public const string IsDataSheet = "IsDataSheet";
            public const string IsRssFeed = "IsRssFeed";
            public const string IsAlertMe = "IsAlertMe";

        }
    }
}
