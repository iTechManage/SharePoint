using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace CrowCanyonAdvancedPrint.Classes
{
    [Serializable]
    class CCSTemplate
    {
        private string title;
        public string Title
        {
            get { return this.title; }
            set { this.title = value; }
        }
        private string header;
        public string Header
        {
            get { return this.header; }
            set { this.header = value; }
        }
        private string footer;
        public string Footer
        {
            get { return this.footer; }
            set { this.footer = value; }
        }

        private string id;
        public string Id
        {
            get { return this.id; }
            set { this.id = value; }
        }

        private IList<Field> fields;
        internal IList<Field> Fields
        {
            get
            {
                if (this.fields == null)
                {
                    this.fields = new List<Field>();
                }
                return this.fields;
            }
            set
            {
                this.fields = value;
            }
        }
    }
}
