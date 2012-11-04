using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SharePoint;
using System.Xml;
using System.Reflection;
using Microsoft.SharePoint.WebControls;
using System.Web.UI.WebControls;
using System.Web;
using Microsoft.SharePoint.ApplicationPages;
using System.Collections.Specialized;
using System.Linq;
using System.Xml.Linq;
using System.Globalization;
using System.Security.Permissions;
using Microsoft.SharePoint.Security;

namespace CustomLookupField
{
    [SharePointPermission(SecurityAction.Demand, ObjectModel = true)]
    class CustomDropDownList : SPFieldLookup
    {
        public const string LINK = "link_to_parent";
        public const string ALLOW_MULTIPLE = "allow_multiple_values";
        public const string PARENT_COLUMN = "parent_column";
        public const string LINK_COLUMN = "link_column";
        public const string AUTO_COMPLETE = "auto_completion";
        public const string ADVANCE_SETTINGS = "advance_settings";
        public const string ADDITIONAL_FIELDS = "additional_fields";
        public const string UNCHECKED_ADDITIONAL_FIELDS = "unchecked_additional_fields";
        public const string ADDITIONAL_FILTERS = "additional_filters";
        public const string VIEW = "view";
        public const string SORT_BY_VIEW = "sort_by_view";
        public const string ADDING_NEW_VALUES = "adding_new_values";
        public const string NEW_FORM = "new_form";
        public const string SHOW_ALL_VALUES = "show_all_values";
        public const string RELATIONSHIP_BEHAVIOR = "fields_relationship_behavior";
        public const string RELATIONSHIP_BEHAVIOR_CASCADE = "cascade_delete";
        public const string PARENT_SELECTED_VALUES = "parent_selected_values";
        
        public CustomDropDownList(SPFieldCollection fields, string fieldName)
         : base(fields, fieldName) {
        }

        public CustomDropDownList(SPFieldCollection fields, string typeName, string displayName)
         : base(fields, typeName, displayName) {
        }

        public override BaseFieldControl FieldRenderingControl
        {
            [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
            get
            {
                BaseFieldControl fieldControl = null;
                /*if (this.AllowMultipleValues)
                {
                    fieldControl = new MultipleCustomDropDownListControl();
                }
                else*/
                {
                    fieldControl = new CustomDropDownListControl();
                }
                fieldControl.FieldName = this.InternalName;

                return fieldControl;
            }
        } 
   
        public override void OnAdded(SPAddFieldOptions op)
        {
            base.OnAdded(op);
            Update();
        }
    
        public override void Update()
        {
           if (this.AllowMultipleValues)
           {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(base.SchemaXml);
                EnsureAttribute(doc, "Mult", "TRUE");
                base.SchemaXml = doc.OuterXml;
           }
           
           base.Update();
        }

        private void EnsureAttribute(XmlDocument doc, string name, string value)
        {
            XmlAttribute attribute = doc.DocumentElement.Attributes[name];
            if (attribute == null)
            {
                attribute = doc.CreateAttribute(name);
                doc.DocumentElement.Attributes.Append(attribute);
            }
            doc.DocumentElement.Attributes[name].Value = value;
        } 

        internal void SetFieldAttribute(string attribute, string value)
        {
            //Hack: Invokes an internal method from the base class
            Type baseType = typeof(CustomDropDownList);
            MethodInfo mi = baseType.GetMethod("SetFieldAttributeValue", BindingFlags.Instance | BindingFlags.NonPublic);
            mi.Invoke(this, new object[] { attribute, value});
        }

        internal object GetFieldAttribute(string attribute)
        {
            //Hack: Invokes an internal method from the base class
            Type baseType = typeof(CustomDropDownList);
            MethodInfo mi = baseType.GetMethod("GetFieldAttributeValue", BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[] { typeof(String) }, null);
            object obj = mi.Invoke(this, new object[] { attribute });

            if (obj == null)
                return "";
            else
                return obj;

        }

        public new void SetCustomProperty(string name, object value)
        {
            
             if (name.Equals("link_to_parent") || name.Equals("allow_multiple_values"))
             {
                 SetFieldAttribute(name, (Boolean)value ? Boolean.TrueString : Boolean.FalseString);
             }
             else
             {
                 SetFieldAttribute(name, Convert.ToString(value));
             }
           
             base.SetCustomProperty(name, value);  
        }

        public new object GetCustomProperty(string name)
        {
            if (name.Equals("Items"))
            {
                return base.GetCustomProperty(name);
            }

            return GetFieldAttribute(name);
           
        }

        public override bool AllowMultipleValues
        {
            get
            {
                //return base.AllowMultipleValues;
                return ( Convert.ToString(this.GetCustomProperty(CustomDropDownList.ALLOW_MULTIPLE)) == Boolean.TrueString);
            }
            set
            {
                base.AllowMultipleValues = value;
            }
        }

        public override Type FieldValueType
        {
            get
            {
                if (this.AllowMultipleValues)
                {
                    return typeof(SPFieldLookupValueCollection);
                }
                return typeof(SPFieldLookupValue);
            }
        }

        public override object GetFieldValue(string value)
        {
            return base.GetFieldValue(value);
        }

        internal void EnsureAdditionalFields(string fields, string unchecked_fields)
        {
            
            base.ParentList.Update();
                
            SPWeb w = SPContext.Current.Site.OpenWeb(this.LookupWebId);
            SPList list = w.Lists[new Guid(this.LookupList)];
                   
            if (fields.Length > 0)
            {
                foreach (string str2 in fields.Split(';'))
                {
                    string str3 = this.Title + ":" + list.Fields[new Guid(str2)].Title;
                    if (str3.Length > 0x20)
                    {
                        str3 = str3.Substring(0, 0x20);
                    }
                        
                    SPList list3 = base.ParentList;
                    if (!list3.Fields.ContainsField(str3))
                    {
                        list3.Fields.AddLookup(str3, new Guid(this.LookupList), false);
                    }
                    SPFieldLookup lookup2 = (SPFieldLookup)list3.Fields[str3];
                    lookup2.Title = str3;
                    lookup2.LookupField = str2;
                    lookup2.LookupWebId = this.LookupWebId;
                    lookup2.ReadOnlyField = false;
                    lookup2.UnlimitedLengthInDocumentLibrary = base.UnlimitedLengthInDocumentLibrary;
                    lookup2.AllowMultipleValues = this.AllowMultipleValues;
                    lookup2.ShowInDisplayForm = true;
                    lookup2.ShowInListSettings = true;
                    lookup2.ShowInViewForms = true;
                    lookup2.ShowInNewForm = false;
                    lookup2.ShowInEditForm = false; 
                    lookup2.Update();
                }
            }
            if (unchecked_fields.Length > 0)
            {
                foreach (string str2 in unchecked_fields.Split(';'))
                {
                    string str3 = this.Title + ":" + list.Fields[new Guid(str2)].Title;
                    if (str3.Length > 0x20)
                    {
                        str3 = str3.Substring(0, 0x20);
                    }

                    SPList list3 = base.ParentList;
                    if (list3.Fields.ContainsField(str3))
                    {
                        list3.Fields[str3].Delete();
                    }
                }
            }
        }

        public override void OnDeleting()
        {
                base.OnDeleting();
                string fields = string.Empty;

                if (this.GetCustomProperty(CustomDropDownList.ADDITIONAL_FIELDS) != null)
                {
                    fields = Convert.ToString(this.GetCustomProperty(CustomDropDownList.ADDITIONAL_FIELDS));
                }

                if ((base.ParentList != null) && !string.IsNullOrEmpty(fields))
                {
                    SPWeb w = SPContext.Current.Site.OpenWeb(this.LookupWebId);
                    SPList sourceList = w.Lists[new Guid(this.LookupList)];
                    SPList currentList = SPContext.Current.List;

                    foreach (string str in fields.Split(';'))
                    {
                        string str2 = this.InternalName + ":" + sourceList.Fields[new Guid(str)].Title;
                        if (str2.Length > 0x20)
                        {
                            str2 = str2.Substring(0, 0x20);
                        }
                        if (currentList.Fields.ContainsField(str2))
                        {
                            SPField fieldByInternalName = currentList.Fields[str2] as SPFieldLookup;
                            fieldByInternalName.ReadOnlyField = false;
                            fieldByInternalName.Update();
                            base.ParentList.Fields.Delete(str2);
                        }
                    }
                }
            }
       
 
    }
}
