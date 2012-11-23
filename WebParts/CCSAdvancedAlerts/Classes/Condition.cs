using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCSAdvancedAlerts{
    class Condition
    {
        private string fieldName;

        internal string FieldName
        {
            get { return fieldName; }
            set { fieldName = value; }
        }
        private string operatorType;

        internal string OperatorType
        {
            get { return operatorType; }
            set { operatorType = value; }
        }
        private string fieldValue;

        internal string FieldValue
        {
            get { return fieldValue; }
            set { fieldValue = value; }
        }
        private string whenToSend;

        internal string WhenToSend
        {
            get { return whenToSend; }
            set { whenToSend = value; }
        }

    }
}
