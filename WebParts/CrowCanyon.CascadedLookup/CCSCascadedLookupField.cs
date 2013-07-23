using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Reflection;
using System.Web.UI.WebControls;

using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;

namespace CrowCanyon.CascadedLookup
{
    class CCSCascadedLookupField : SPFieldLookup
    {
        
       #region Constructors

        public CCSCascadedLookupField(SPFieldCollection fields, string fieldName)
            : base(fields, fieldName)
        {
        }

        public CCSCascadedLookupField(SPFieldCollection fields, string typeName, string displayName)
            : base(fields, typeName, displayName)
        {
        }

        #endregion

        #region Override methods

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

        public override void OnUpdated()
        {
            base.OnUpdated();

            if (AdditionalFieldControlItems != null && AdditionalFieldControlItems.Count > 0)
            {
                foreach (ListItem li in AdditionalFieldControlItems)
                {
                    if (li.Selected)
                    {
                        if (!this.ParentList.Fields.ContainsField(this.Title + " : " + li.Text))
                        {
                            //create a new field
                            string depLookUp = this.ParentList.Fields.AddDependentLookup(this.Title + " : " + li.Text, this.Id);
                            SPFieldLookup fieldDepLookup = (SPFieldLookup)this.ParentList.Fields.GetFieldByInternalName(depLookUp);

                            if (fieldDepLookup != null)
                            {
                                fieldDepLookup.LookupWebId = this.LookupWebId;
                                fieldDepLookup.LookupField = li.Value;
                                fieldDepLookup.Update();
                            }
                        }
                    }
                    else
                    {
                        if (this.ParentList.Fields.ContainsField(this.Title + " : " + li.Text))
                        {
                            //delete field if exist
                            this.ParentList.Fields.GetField(this.Title + " : " + li.Text).Delete();
                        }
                    }
                }
            }
        }

        public string GetAdditionalFields()
        {
            string additionalFieldsString = "";
            if (this != null && this.ParentList != null)
            {
                for(int i=0; i < this.ParentList.Fields.Count; i++)
                {
                    SPFieldLookup field = this.ParentList.Fields[i] as SPFieldLookup;
                    if(field != null && field.IsDependentLookup && field.PrimaryFieldId != null && field.PrimaryFieldId == this.Id.ToString())
                    {
                        additionalFieldsString = additionalFieldsString + ";#" + field.LookupField; 
                    }
                }
            }

            return additionalFieldsString;
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

        public override Microsoft.SharePoint.WebControls.BaseFieldControl FieldRenderingControl
        {
            [SharePointPermission(System.Security.Permissions.SecurityAction.LinkDemand, ObjectModel = true)]
            get
            {
                Microsoft.SharePoint.WebControls.BaseFieldControl ccsCascadedLookupControl = new CCSCascadedLookupControl();
                ccsCascadedLookupControl.FieldName = this.InternalName;

                return ccsCascadedLookupControl;
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

        #endregion

        #region Field Propertires
        
        /// <summary>
        /// <Field Name="SourceWebID" DisplayName="Source web ID" Type="Text" Hidden="TRUE"/>
        /// </summary>
        public string SourceWebID
        {
            get
            {
                return (string)this.GetCustomProperty("SourceWebID");
            }
            set
            {
                this.SetCustomProperty("SourceWebID", value);
            }
        }

        /// <summary>
        /// <Field Name="LookupFieldListName" DisplayName="Lookup Field list" Type="Text" Hidden="TRUE"/>
        /// </summary>
        public string LookupFieldListName
        {
            get
            {
                return (string)this.GetCustomProperty("LookupFieldListName");
            }
            set
            {
                this.SetCustomProperty("LookupFieldListName", value);
            }
        }

        /// <summary>
        /// <Field Name="LookupFieldName" DisplayName="Loorup column" Type="Text" Hidden="TRUE"/>
        /// </summary>
        public string LookupFieldName
        {
            get
            {
                return (string)this.GetCustomProperty("LookupFieldName");
            }
            set
            {
                this.SetCustomProperty("LookupFieldName", value);
            }
        }

        /// <summary>
        ///  <Field Name="ParentLinkedColumnName" DisplayName="Parent Linked column" Type="Text" Hidden="TRUE"/>
        /// </summary>
        public string ParentLinkedColumnName
        {
            get
            {
                return (string)this.GetCustomProperty("ParentLinkedColumnName");
            }
            set
            {
                this.SetCustomProperty("ParentLinkedColumnName", value);
            }
        }

        /// <summary>
        /// <Field Name="AllowMultipleValues" DisplayName="Allow Multiple Values" Type="Text" Hidden="TRUE"/>
        /// </summary>
        public override Boolean AllowMultipleValues
        {
            get
            {
                string val = (string)this.GetCustomProperty("AllowMultipleValues");
                if (!string.IsNullOrEmpty(val) && val.ToLower().Equals("mult"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            set
            {
                if (value)
                {
                    this.SetCustomProperty("AllowMultipleValues", "mult");
                }
                else
                {
                    this.SetCustomProperty("AllowMultipleValues", "");
                }
            }
        }

        /// <summary>
        /// <Field Name="AdvancedSetting" DisplayName="Advanced Setting" Type="Boolean" Hidden="TRUE"/>
        /// </summary>
        public Boolean AdvancedSetting
        {
            get
            {
                return ConvertToBool(this.GetCustomProperty("AdvancedSetting"));
            }
            set
            {
                this.SetCustomProperty("AdvancedSetting", value.ToString());
            }
        }

        /// <summary>
        /// <Field Name="View" DisplayName="View" Type="Text" Hidden="TRUE"/>
        /// </summary>
        public string View
        {
            get
            {
                return (string)this.GetCustomProperty("View");
            }
            set
            {
                this.SetCustomProperty("View", value);
            }
        }

        /// <summary>
        /// <Field Name="LinkToParent" DisplayName="Link to Parent" Type="Boolean" Hidden="TRUE"/>
        /// </summary>
        public Boolean LinkToParent
        {
            get
            {
                return ConvertToBool(this.GetCustomProperty("LinkToParent"));
            }
            set
            {
                this.SetCustomProperty("LinkToParent", value.ToString());
            }
        }

        /// <summary>
        ///<Field Name="ShowAllOnEmpty" DisplayName="Show all on empty parent" Type="Boolean" Hidden="TRUE"/>
        /// </summary>
        public Boolean ShowAllOnEmpty
        {
            get
            {
                return ConvertToBool(this.GetCustomProperty("ShowAllOnEmpty"));
            }
            set
            {
                this.SetCustomProperty("ShowAllOnEmpty", value.ToString());
            }
        }

        /// <summary>
        /// <Field Name="AllowNewEntry" DisplayName="Allow adding values" Type="Boolean" Hidden="TRUE"/>
        /// </summary>
        public Boolean AllowNewEntry
        {
            get
            {
                return ConvertToBool(this.GetCustomProperty("AllowNewEntry"));
            }
            set
            {
                this.SetCustomProperty("AllowNewEntry", value.ToString());
            }
        }

        /// <summary>
        /// <Field Name="UseNewForm" DisplayName="Use New form" Type="Boolean" Hidden="TRUE"/>
        /// </summary>
        public Boolean UseNewForm
        {
            get
            {
                return ConvertToBool(this.GetCustomProperty("UseNewForm"));
            }
            set
            {
                this.SetCustomProperty("UseNewForm", value.ToString());
            }
        }

        /// <summary>
        /// <Field Name="AdditionalFields" DisplayName="Additional Fields" Type="Text" Hidden="TRUE"/>
        /// </summary>
        public string AdditionalFields
        {
            get
            {
                return (string)this.GetCustomProperty("AdditionalFields");
            }
            set
            {
                this.SetCustomProperty("AdditionalFields", value);
            }
        }

        /// <summary>
        /// <Field Name="SortByView" DisplayName="Sort By View" Type="Boolean" Hidden="TRUE"/>
        /// </summary>
        public Boolean SortByView
        {
            get
            {
                return ConvertToBool(this.GetCustomProperty("SortByView"));
            }
            set
            {
                this.SetCustomProperty("SortByView", value.ToString());
            }
        }

        /// <summary>
        /// <Field Name="AllowAutocomplete" DisplayName="Allow autocomplete" Type="Boolean" Hidden="TRUE"/>
        /// </summary>
        public Boolean AllowAutocomplete
        {
            get
            {
                return ConvertToBool(this.GetCustomProperty("AllowAutocomplete"));
            }
            set
            {
                this.SetCustomProperty("AllowAutocomplete", value.ToString());
            }
        }

        /// <summary>
        /// <Field Name="AdditionalFilters" DisplayName="AdditionalFilters" Type="Text" Hidden="TRUE"/>
        /// </summary>
        public string AdditionalFilters
        {
            get
            {
                return (string)this.GetCustomProperty("AdditionalFilters");
            }
            set
            {
                this.SetCustomProperty("AdditionalFilters", value);
            }
        }

        public System.Web.UI.WebControls.ListItemCollection AdditionalFieldControlItems
        {
            get;
            set;
        }

        #endregion

        public string GetParentColumnId()
        {
            if (!string.IsNullOrEmpty(ParentLinkedColumnName))
            {
                string[] vals = ParentLinkedColumnName.Split(new string[] { ";#" }, StringSplitOptions.None);
                if (vals != null && vals.Length == 3)
                {
                    return vals[0];
                }
            }
            return "";
        }

        public string GetParentLinkedColumnId()
        {
            if (!string.IsNullOrEmpty(ParentLinkedColumnName))
            {
                string[] vals = ParentLinkedColumnName.Split(new string[] { ";#" }, StringSplitOptions.None);
                if (vals != null && vals.Length == 3)
                {
                    return vals[2];
                }
            }
            return "";
        }


        Boolean ConvertToBool(object obj)
        {
            if (obj != null)
            {
                return Convert.ToBoolean(obj);
            }

            return false;
        }

        public new void SetCustomProperty(string attribute, object value)
        {
            Type type = typeof(CCSCascadedLookupField);
            MethodInfo mi = type.GetMethod("SetFieldAttributeValue", BindingFlags.Instance | BindingFlags.NonPublic);
            mi.Invoke(this, new object[] { attribute, value });
        }

        public new object GetCustomProperty(string attribute)
        {
            Type type = typeof(CCSCascadedLookupField);
            MethodInfo mi = type.GetMethod("GetFieldAttributeValue", BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[] { typeof(String) }, null);
            object obj = mi.Invoke(this, new object[] { attribute });

            return obj == null ? "" : obj; 
        }
    }
}
