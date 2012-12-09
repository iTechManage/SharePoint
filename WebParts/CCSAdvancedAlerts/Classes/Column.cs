using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCSAdvancedAlerts
{
    class Column
    {
        private string internalName;
        public string InternalName
        {
            get { return internalName; }
            set { internalName = value; }
        }

        private string displayName;
        public string DisplayName
        {
            get { return displayName; }
            set { displayName = value; }
        }
        
        private object afterValue;
        public object AfterValue
        {
            get { return afterValue; }
            set { afterValue = value; }
        }


        private object beforeValue;
        public object BeforeValue
        {
            get { return beforeValue; }
            set { beforeValue = value; }
        }
        


    }
}
