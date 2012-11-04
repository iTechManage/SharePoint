using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Collections.Generic;

using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Text;
using System.Globalization;
using System.Web.UI.HtmlControls;
using System.Collections;

namespace CustomLookupField
{
    class MultipleCustomDropDownListControl : BaseFieldControl
    {
        SPFieldLookupValueCollection _fieldVals;
        List<ListItem> _availableItems = null;
        List<ListItem> _removedItems = null;

        protected SPHtmlSelect SelectCandidate;
        protected SPHtmlSelect SelectResult;
        protected HtmlButton AddButton;
        protected HtmlButton RemoveButton;
        protected GroupedItemPicker MultiLookupPicker;
        protected ListBox left_box;
        protected ListBox right_box;
        protected Button add_button;
        protected Button remove_button;
        
        protected override string DefaultTemplateName { get { return "MultipleCustomDropDownListControl"; } }

        #region OnInit and OnLoad methods
        protected override void OnInit(EventArgs e)
        {
            if (ControlMode == SPControlMode.Edit || ControlMode == SPControlMode.Display)
            {
                if (base.ListItemFieldValue != null)
                {
                    _fieldVals = base.ListItemFieldValue as SPFieldLookupValueCollection;
                }
                else { _fieldVals = new SPFieldLookupValueCollection(); }
            }
            if (ControlMode == SPControlMode.New) { _fieldVals = new SPFieldLookupValueCollection(); }
            base.OnInit(e);
            Initialize((CustomDropDownList)this.Field);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (ControlMode != SPControlMode.Display)
            {
                if (!Page.IsPostBack)
                {
                    SetValue();
                }
            }
        }
        #endregion

        #region CreateChildControls method
        protected override void CreateChildControls()
        {
            if (this.Field != null && this.ControlMode != SPControlMode.Display)
            {
                if (!this.ChildControlsCreated)
                {
                    CustomDropDownList field = this.Field as CustomDropDownList;
                    base.CreateChildControls();
                    /*
                    MultiLookupPicker = (GroupedItemPicker)TemplateContainer.FindControl("MultiLookupPicker");
                   
                    BuildAvailableItems(ref MultiLookupPicker);

                    SelectCandidate = (SPHtmlSelect)TemplateContainer.FindControl("SelectCandidate");
                    SelectResult = (SPHtmlSelect)TemplateContainer.FindControl("SelectResult");

                    AddButton = (HtmlButton)TemplateContainer.FindControl("AddButton");
                    RemoveButton = (HtmlButton)TemplateContainer.FindControl("RemoveButton");
                    */

                    left_box = (ListBox)TemplateContainer.FindControl("LeftBox");
                    if (left_box.Attributes["done"] == null)
                    {
                        BuildAvailableItems(ref left_box);
                    }
                    right_box = (ListBox)TemplateContainer.FindControl("RightBox");
                    add_button = (Button)TemplateContainer.FindControl("AddButton");
                    add_button.Click += new EventHandler(add_button_Click);
                    remove_button = (Button)TemplateContainer.FindControl("RemoveButton");
                    remove_button.Click += new EventHandler(remove_button_Click);
                }
            }
        }

        void remove_button_Click(object sender, EventArgs e)
        {
            foreach (ListItem item in _removedItems)
            {
                if (item.Selected)
                {
                    item.Selected = false;
                    left_box.Items.Add(item);
                    right_box.Items.Remove(item);
                }
            }
        }

        void add_button_Click(object sender, EventArgs e)
        {
            foreach (ListItem item in _availableItems)
            {
                if (item.Selected)
                {
                    item.Selected = false;
                    right_box.Items.Add(item);
                    left_box.Items.Remove(item);
                }
            }
        }
        #endregion

        #region BuildAvailableItems method
        private void BuildAvailableItems(ref GroupedItemPicker m)
        {
            if (_availableItems != null && _availableItems.Count != 0)
            {
                foreach (ListItem i in _availableItems)
                {
                    m.AddItem(i.Value, i.Text, string.Empty, string.Empty);
                }
            }
        }
        private void BuildAvailableItems(ref ListBox lB)
        {
            if (_availableItems != null && _availableItems.Count != 0)
            {
                foreach (ListItem i in _availableItems)
                {
                    lB.Items.Add(i); 
                }
            }
            lB.Attributes["done"] = "yes";
        }
        #endregion

        #region Value property
        public override object Value
        {
            get
            {
                EnsureChildControls();
                SPFieldLookupValueCollection _vals = new SPFieldLookupValueCollection();
                
                _availableItems.Clear();
                foreach (ListItem item in left_box.Items)
                {
                    _availableItems.Add(item);
                    if (item.Selected)
                    {
                        _vals.Add(new SPFieldLookupValue(int.Parse(item.Value), item.Text));
                    }
                }
                 
                _removedItems = new List<ListItem>();
                foreach (ListItem item in right_box.Items)
                {
                    _removedItems.Add(item);
                    _vals.Add(new SPFieldLookupValue(int.Parse(item.Value), item.Text));
                }

                return _vals;
            }
            set
            {
                EnsureChildControls();
                base.Value = value as SPFieldLookupValueCollection;
            }
        }
        #endregion

        #region Initialize method
        private void Initialize(CustomDropDownList clist)
        {
            try
            {
                SPWeb w = SPContext.Current.Site.OpenWeb(clist.LookupWebId);
                SPList list = w.Lists[new Guid(clist.LookupList)];
                
                if (list != null && list.ItemCount > 0 && list.Fields.Contains(new Guid(clist.LookupField)))
                {
                    _availableItems = new List<ListItem>();
                    foreach (SPListItem item in list.Items)
                    {
                        ListItem newItem = new ListItem(Convert.ToString(item.Fields[new Guid(clist.LookupField)].GetFieldValueAsText(item[new Guid(clist.LookupField)])), item.ID.ToString());
                        if (!this._availableItems.Contains(newItem))
                        {
                            this._availableItems.Add(newItem);
                        }
                    }
                }
            }
            catch { }
        }
        #endregion

        #region EnsureValuesAreAvailable method
        /// <summary>
        /// Ensures that previously selected values are still available
        /// when an item is being edited. This is necessary just in case
        /// the field value is not necessarily being changed.
        /// </summary>
        private void EnsureValuesAreAvailable()
        {
            if (_fieldVals != null && _fieldVals.Count > 0)
            {
                foreach (SPFieldLookupValue i in _fieldVals)
                {
                    ListItem z = _availableItems.Find(x => (x.Value.ToLower() == i.LookupId.ToString().ToLower()));
                    if (z == null)
                    {
                        _availableItems.Add(new ListItem(i.LookupValue, i.LookupId.ToString()));
                    }
                }
            }
        }
        #endregion

        #region SetValue method
        private void SetValue()
        {
            if (_fieldVals != null && _fieldVals.Count > 0)
            {
                string s = string.Empty;
                foreach (SPFieldLookupValue i in _fieldVals)
                {
                    ListItem item = new ListItem(i.LookupValue,i.LookupId.ToString());
                    right_box.Items.Add(item);
                    _availableItems.Remove(item);
                }
                left_box.Items.Clear();
                foreach (ListItem item in _availableItems)
                {
                    left_box.Items.Add(item);
                }
            }
        }
        #endregion
    }
}
