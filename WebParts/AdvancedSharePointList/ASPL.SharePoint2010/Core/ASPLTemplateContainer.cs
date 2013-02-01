using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.WebControls;
using System.Web.UI;
using System.Reflection;

namespace ASPL.SharePoint2010.Core
{
    class ASPLTemplateContainer
    {
        private TemplateContainer _templateContainer = null;

        public ASPLTemplateContainer()
        {
            this._templateContainer = new TemplateContainer();
        }

        public ControlCollection Controls
        {
            get
            {
                Type targetType=_templateContainer.GetType();
                PropertyInfo propertyInfo = targetType.GetProperty("Controls", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

                return propertyInfo.GetGetMethod(true).Invoke(_templateContainer, null) as ControlCollection;
            }
        }

        public SPControlMode ControlMode
        {
            get
            {
                Type targetType = _templateContainer.GetType();
                PropertyInfo propertyInfo = targetType.GetProperty("ControlMode", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

                string ControlModeString=propertyInfo.GetGetMethod(true).Invoke(_templateContainer, null) as string;

                if (!string.IsNullOrEmpty(ControlModeString))
                {
                    return (SPControlMode)Enum.Parse(typeof(SPControlMode), ControlModeString);
                }
                else
                {
                    return SPControlMode.Invalid;
                }
            }
            set
            {
                Type targetType = _templateContainer.GetType();
                PropertyInfo propertyInfo = targetType.GetProperty("ControlMode", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

                propertyInfo.GetSetMethod(true).Invoke(_templateContainer,new object[]{ value});
            }
        }

        public string FieldName
        {
            get
            {
                Type targetType = _templateContainer.GetType();
                PropertyInfo propertyInfo = targetType.GetProperty("FieldName", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

                return propertyInfo.GetGetMethod(true).Invoke(_templateContainer, null) as string;

            }
            set
            {
                Type targetType = _templateContainer.GetType();
                PropertyInfo propertyInfo = targetType.GetProperty("FieldName", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

                propertyInfo.GetSetMethod(true).Invoke(_templateContainer, new object[] { value });
            }
        }

        public TemplateContainer Template
        {
            get { return this._templateContainer; }
        }
    }
}
