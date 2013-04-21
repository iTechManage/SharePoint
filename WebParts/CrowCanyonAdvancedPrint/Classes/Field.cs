using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrowCanyonAdvancedPrint
{
    [Serializable]
    class Field
    {
        private string fieldName;
        public string FieldName
        {
            get { return fieldName; }
            set { fieldName = value; }
        }

    }
}
