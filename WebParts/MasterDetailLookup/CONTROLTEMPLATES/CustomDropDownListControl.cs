using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint;
using System.Web.UI.WebControls;
using System.Runtime.InteropServices;
using System.Web.UI;
using System.Globalization;
using System.Web.UI.HtmlControls;
using System.Web;
using CustomLookupField.CONTROLTEMPLATES;
using System.Xml;

namespace CustomLookupField
{
    public sealed class CustomDropDownListControl : BaseFieldControl
    {
        SPFieldLookupValue _fieldVal;
        List<ListItem> _availableItems = null;
        DropDownList _customisedList;
        TextBox _auto_completion_box;
        LinkButton _new_element;
        TextBox _new_value;
        LinkButton _add_entry;
        LinkButton _cancel_button;
        HtmlTable _table;

        List<ListItem> _ItemsList_Multi_NoLink = null;

        SPFieldLookupValueCollection _fieldVals;
        List<ListItem> _removedItems = null;

        protected ListBox left_box;
        protected ListBox right_box;
        protected Button add_button;
        protected Button remove_button;

        protected override string DefaultTemplateName { get { return "CustomDropDownListControl"; } }

        protected override void OnInit(EventArgs e)
        {
            CustomDropDownList field = base.Field as CustomDropDownList;
            if (field.AllowMultipleValues)
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
                Initialize_multi_value((CustomDropDownList)this.Field);
            }
            else
            {
                if (ControlMode == SPControlMode.Edit || ControlMode == SPControlMode.Display)
                {
                    if (base.ListItemFieldValue != null)
                    {
                        _fieldVal = base.ListItemFieldValue as SPFieldLookupValue;
                    }
                    else { _fieldVal = new SPFieldLookupValue(); }
                }
                if (ControlMode == SPControlMode.New) { _fieldVal = new SPFieldLookupValue(); }
                base.OnInit(e);
                Initialize((CustomDropDownList)base.Field);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (ControlMode != SPControlMode.Display)
            {
                if (!Page.ClientScript.IsStartupScriptRegistered(this.Field.Id.ToString("n")))
                {
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    sb.Append(@"<script language='javascript'>");
                    sb.Append(@"function callbackMethod" + this.Field.Id.ToString("n") + " (dialogResult, returnValue)");
                    sb.Append(@"{");
                    sb.Append(@"if(dialogResult == 1)");
                    sb.Append(@"{");
                    sb.Append(@" __doPostBack('" + this.Field.Id.ToString("n") + "', '')");
                    sb.Append(@"}");
                    sb.Append(@"}");
                    sb.Append(@"</script>");

                    Page.ClientScript.RegisterStartupScript(this.GetType(), this.Field.Id.ToString("n"), sb.ToString());
                }

                if (!Page.IsPostBack)
                {
                    SetValue();
                }
                else if (ControlMode == SPControlMode.Edit || ControlMode == SPControlMode.New)
                {
                    CustomDropDownList field = base.Field as CustomDropDownList;
                    bool has_link = Convert.ToString(field.GetCustomProperty(CustomDropDownList.LINK)) == Boolean.TrueString;
                    if (field.AllowMultipleValues)
                    {
                        if (has_link)
                        {
                            List<ListItem> FielditemList = GetCurrentLinkedFieldValue();
                            if (FielditemList != null)
                            {
                                foreach (ListItem item in FielditemList)
                                {
                                    if (!(left_box.Items.Contains(item) || right_box.Items.Contains(item)))
                                        left_box.Items.Insert(left_box.Items.Count, item);
                                }
                            }
                        }
                        else
                        {
                            if (_ItemsList_Multi_NoLink != null)
                            {
                                foreach (ListItem item in _ItemsList_Multi_NoLink)
                                {
                                    if (!(left_box.Items.Contains(item) || right_box.Items.Contains(item)))
                                        left_box.Items.Insert(left_box.Items.Count, item);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (!has_link)
                        {
                            if (_availableItems != null && _availableItems.Count != 0)
                            {
                                bool update = false, flag = Page.Request["__EVENTTARGET"] != null && this.Field.Id.ToString("n") == Page.Request.Params.Get("__EVENTTARGET");
                                int Max = -1;
                                foreach (ListItem li in _availableItems)
                                {
                                    if (!_customisedList.Items.Contains(li))
                                    {
                                        int index = _customisedList.Items.Count;
                                        _customisedList.Items.Insert(index, li);

                                        if (flag && Max < Convert.ToInt32(li.Value))
                                        {
                                            Max = Convert.ToInt32(li.Value);
                                            _customisedList.SelectedIndex = index;
                                            update = true;
                                        }
                                    }
                                }

                                if (update)
                                {
                                    l_SelectedIndexChanged(_customisedList, e);
                                }
                            }
                        }
                        else
                        {
                            List<ListItem> FielditemList = GetCurrentLinkedFieldValue();
                            if (FielditemList != null)
                            {
                                bool update = false, flag = Page.Request["__EVENTTARGET"] != null && this.Field.Id.ToString("n") == Page.Request.Params.Get("__EVENTTARGET");
                                int Max = -1;
                                foreach (ListItem li in FielditemList)
                                {
                                    if (!_customisedList.Items.Contains(li))
                                    {
                                        int index = _customisedList.Items.Count;
                                        _customisedList.Items.Insert(index, li);


                                        if (flag && Max < Convert.ToInt32(li.Value))
                                        {
                                            Max = Convert.ToInt32(li.Value);
                                            _customisedList.SelectedIndex = index;
                                            update = true;
                                        }
                                    }
                                }
                                if (update)
                                {
                                    l_SelectedIndexChanged(_customisedList, e);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void Initialize(CustomDropDownList clist)
        {
            try
            {
                SPWeb w = SPContext.Current.Site.OpenWeb(clist.LookupWebId);
                SPList list = w.Lists[new Guid(clist.LookupList)];

                if (list != null && list.ItemCount > 0 && list.Fields.Contains(new Guid(clist.LookupField)))
                {
                    SPListItemCollection Items = null;
                    string viewId = string.Empty;

                    if (Convert.ToString(clist.GetCustomProperty(CustomDropDownList.VIEW)) != string.Empty)
                    {
                        viewId = Convert.ToString(clist.GetCustomProperty(CustomDropDownList.VIEW));
                        viewId = viewId.Substring(0, viewId.IndexOf('|'));
                        SPView view = list.GetView(new Guid(viewId));
                        string view_query = view.Query;
                        string view_order_query = string.Empty;
                        SPQuery query = new SPQuery();
                        query.ViewAttributes = "Scope=\"RecursiveAll\"";
                        bool use_view_order = Convert.ToBoolean(clist.GetCustomProperty(CustomDropDownList.SORT_BY_VIEW));
                        if (use_view_order)
                        {
                            string xml = string.Format("<Query>{0}</Query>", view.Query);
                            XmlDocument document = new XmlDocument();
                            document.LoadXml(xml);
                            XmlNode node = document.DocumentElement.SelectSingleNode("OrderBy");
                            if (node == null || string.IsNullOrEmpty(node.InnerXml))
                            {
                                //do nothing
                            }
                            else
                            {
                                view_order_query = node.InnerXml;
                            }

                            view_order_query = string.Format("<OrderBy>{0}</OrderBy>", view_order_query);
                        }
                        if (view_query.Contains("<Where>"))
                        {
                            int start_index = view_query.IndexOf("<Where>") + "<Where>".Length;
                            int length = view_query.IndexOf("</Where>") - start_index;
                            view_query = view_query.Substring(start_index, length);
                            query.Query = "<Where>" + view_query + "</Where>" + view_order_query;
                            Items = list.GetItems(query);
                        }
                        else
                        {
                            query.Query = view_order_query;
                            Items = list.GetItems(query);
                        }
                    }
                    else
                    {
                        Items = list.Items;
                    }

                    _availableItems = new List<ListItem>();
                    foreach (SPListItem item in Items)
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

        private void Initialize_multi_value(CustomDropDownList clist)
        {
            try
            {
                SPWeb w = SPContext.Current.Site.OpenWeb(clist.LookupWebId);
                SPList list = w.Lists[new Guid(clist.LookupList)];

                if (list != null && list.ItemCount > 0 && list.Fields.Contains(new Guid(clist.LookupField)))
                {
                    SPListItemCollection Items = null;
                    string viewId = string.Empty;

                    if (Convert.ToString(clist.GetCustomProperty(CustomDropDownList.VIEW)) != string.Empty)
                    {
                        viewId = Convert.ToString(clist.GetCustomProperty(CustomDropDownList.VIEW));
                        viewId = viewId.Substring(0, viewId.IndexOf('|'));
                        SPView view = list.GetView(new Guid(viewId));
                        string view_query = view.Query;
                        string view_order_query = string.Empty;
                        SPQuery query = new SPQuery();
                        query.ViewAttributes = "Scope=\"RecursiveAll\"";
                        bool use_view_order = Convert.ToBoolean(clist.GetCustomProperty(CustomDropDownList.SORT_BY_VIEW));
                        if (use_view_order)
                        {
                            string xml = string.Format("<Query>{0}</Query>", view.Query);
                            XmlDocument document = new XmlDocument();
                            document.LoadXml(xml);
                            XmlNode node = document.DocumentElement.SelectSingleNode("OrderBy");
                            if (node == null || string.IsNullOrEmpty(node.InnerXml))
                            {
                                //do nothing
                            }
                            else
                            {
                                view_order_query = node.InnerXml;
                            }

                            view_order_query = string.Format("<OrderBy>{0}</OrderBy>", view_order_query);
                        }
                        if (view_query.Contains("<Where>"))
                        {
                            int start_index = view_query.IndexOf("<Where>") + "<Where>".Length;
                            int length = view_query.IndexOf("</Where>") - start_index;
                            view_query = view_query.Substring(start_index, length);
                            query.Query = "<Where>" + view_query + "</Where>" + view_order_query;
                            Items = list.GetItems(query);
                        }
                        else
                        {
                            query.Query = view_order_query;
                            Items = list.GetItems(query);
                        }
                    }
                    else
                    {
                        Items = list.Items;
                    }

                    _availableItems = new List<ListItem>();
                    _ItemsList_Multi_NoLink = new List<ListItem>();
                    foreach (SPListItem item in Items)
                    {
                        ListItem newItem = new ListItem(Convert.ToString(item.Fields[new Guid(clist.LookupField)].GetFieldValueAsText(item[new Guid(clist.LookupField)])), item.ID.ToString());
                        if (!this._availableItems.Contains(newItem))
                        {
                            this._availableItems.Add(newItem);
                            _ItemsList_Multi_NoLink.Add(newItem);
                        }
                    }
                }
            }
            catch { }
        }

        private List<ListItem> get_filtered_items(List<ListItem> item_collection, string text)
        {
            List<ListItem> filtered_item = new List<ListItem>();
            foreach (ListItem item in item_collection)
            {
                string item_val = item.Text;

                if (item_val.Contains(text))
                {
                    if (!filtered_item.Contains(item))
                    {
                        filtered_item.Add(item);
                    }
                }
            }
            return filtered_item;
        }

        protected override void CreateChildControls()
        {
            CustomDropDownList field = base.Field as CustomDropDownList;
            if (field.AllowMultipleValues)
            {
                if (this.ControlMode != SPControlMode.Display)
                {
                    base.CreateChildControls();
                    if (!this.ChildControlsCreated)
                    {
                        if (base.ControlMode == SPControlMode.Edit)
                        {
                            _table = (HtmlTable)TemplateContainer.FindControl("MultiColumnTable");
                            _table.Visible = true;
                            //fill_controls();
                            fill_controls_for_multi();
                            left_box = (ListBox)TemplateContainer.FindControl("LeftBox");
                            left_box.Visible = true;
                            // BuildAvailableItems(ref left_box);

                            right_box = (ListBox)TemplateContainer.FindControl("RightBox");
                            right_box.Visible = true;
                            add_button = (Button)TemplateContainer.FindControl("AddButton");
                            add_button.Visible = true;
                            add_button.Click += new EventHandler(add_button_Click);
                            remove_button = (Button)TemplateContainer.FindControl("RemoveButton");
                            remove_button.Visible = true;
                            remove_button.Click += new EventHandler(remove_button_Click);
                            ListItemCollection coll = right_box.Items;
                        }
                        else
                        {
                            CreateStandardSelect_for_multi();
                        }
                    }
                }
            }
            else
            {
                if (base.ControlMode != SPControlMode.Display)
                {
                    base.CreateChildControls();
                    if (!this.ChildControlsCreated)
                    {
                        if (base.ControlMode == SPControlMode.Edit)
                        {
                            fill_controls();
                        }
                        else
                        {
                            CreateStandardSelect();
                        }
                    }
                }
            }
        }

        void RenderMethod1(HtmlTextWriter output, object t)
        {
            string s = "ss";
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
            List<Control> list_box_controls = new List<Control>();
            List<Control> dropdownlist_controls = new List<Control>();
            FindControlRecursive(this.Page, typeof(ListBox), ref list_box_controls);
            FindControlRecursive(this.Page, typeof(DropDownList), ref dropdownlist_controls);

            SPList list = SPContext.Current.List;
            SPFieldCollection fields = list.Fields;
            //ListItemCollection parent_field_right_box_items = right_box.Items;
            //string parent_field_selected_value = string.Empty;

            List<Control> PossibleControls = new List<Control>();
            FindControlRecursive(this.Page, typeof(CustomLookupField.CustomDropDownListControl), ref PossibleControls);

            List<string> SelectValues = new List<string>();
            if (right_box.Items != null && right_box.Items.Count > 0)
            {
                foreach (ListItem li in right_box.Items)
                {
                    SelectValues.Add(li.Value);
                }
            }
            else
            {
                SelectValues = null;
            }

            UpdateChildLinkedControl(base.Field, SelectValues, ref PossibleControls);
            //Update_child_controls(fields, base.Field, parent_field_right_box_items, parent_field_selected_value, list_box_controls, dropdownlist_controls);
            //l_SelectedIndexChanged(sender, e);
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
            //List<Control> list_box_controls = new List<Control>();
            //List<Control> dropdownlist_controls = new List<Control>();
            //FindControlRecursive(this.Page, typeof(ListBox), ref list_box_controls);
            //FindControlRecursive(this.Page, typeof(DropDownList), ref dropdownlist_controls);

            //SPList list = SPContext.Current.List;
            //SPFieldCollection fields = list.Fields;
            //ListItemCollection parent_field_right_box_items = right_box.Items;
            //string parent_field_selected_value = string.Empty;

            //Update_child_controls(fields, base.Field, parent_field_right_box_items, parent_field_selected_value, list_box_controls, dropdownlist_controls);
            //l_SelectedIndexChanged(sender, e);

            List<Control> PossibleControls = new List<Control>();
            FindControlRecursive(this.Page, typeof(CustomLookupField.CustomDropDownListControl), ref PossibleControls);

            List<string> SelectValues = new List<string>();
            if (right_box.Items != null && right_box.Items.Count > 0)
            {
                foreach (ListItem li in right_box.Items)
                {
                    SelectValues.Add(li.Value);
                }
            }
            else
            {
                SelectValues = null;
            }

            UpdateChildLinkedControl(base.Field, SelectValues, ref PossibleControls);
        }

        private void fill_controls()
        {
            CustomDropDownList field = base.Field as CustomDropDownList;

            bool auto_completion_option_enabled = Convert.ToString(field.GetCustomProperty(CustomDropDownList.AUTO_COMPLETE)) == Boolean.TrueString;
            if (auto_completion_option_enabled)
            {
                _auto_completion_box = (TextBox)TemplateContainer.FindControl("auto_complete");
                _auto_completion_box.Visible = true;
                _auto_completion_box.Text = _fieldVal.LookupValue;
                _auto_completion_box.TextChanged += new EventHandler(_auto_completion_box_TextChanged);
                _auto_completion_box.AutoPostBack = true;

            }
            _customisedList = (DropDownList)TemplateContainer.FindControl("customList");
            _customisedList.Visible = true;

            bool has_link = Convert.ToString(field.GetCustomProperty(CustomDropDownList.LINK)) == Boolean.TrueString;

            if (has_child(field.Id.ToString()))
            {
                _customisedList.SelectedIndexChanged += new EventHandler(l_SelectedIndexChanged);
                _customisedList.AutoPostBack = true;
            }

            _customisedList.ID = "Lookup";
            _customisedList.ToolTip = string.Format(CultureInfo.InvariantCulture, "{0}", Field.InternalName);

            if (!has_link || (Convert.ToBoolean(field.GetCustomProperty(CustomDropDownList.SHOW_ALL_VALUES)) && ParentValueNullOREmpty()))
            {
                Initialize((CustomDropDownList)base.Field);
                _customisedList.Items.Clear();
                // _customisedList.AutoPostBack = true;
                if (!Field.Required) _customisedList.Items.Insert(0, new ListItem("(None)", "0"));
                _customisedList.Items.AddRange(_availableItems.ToArray());
                _customisedList.SelectedIndex = _customisedList.Items.IndexOf(_customisedList.Items.FindByText(_fieldVal.LookupValue));
                base.Field.SetCustomProperty("Items", _fieldVal.LookupId);

            }
            else
            {
                SPFieldCollection field_coll = this.Fields;
                string selected_items = string.Empty;
                foreach (SPField f in field_coll)
                {
                    if (f.Id.ToString().Equals(Convert.ToString(field.GetProperty(CustomDropDownList.PARENT_COLUMN))))
                    {
                        selected_items = GetFieldValue(f);
                        break;
                    }
                }

                List<ListItem> Items_List = new List<ListItem>();
                string linked_column = field.GetProperty(CustomDropDownList.LINK_COLUMN);

                foreach (string sel_value in selected_items.Split(':'))
                {
                    Helper.get_matched_items(field, sel_value, linked_column, ref Items_List);
                }

                //Helper.get_matched_items(field, sel_value, linked_column, ref item_list);

                _customisedList.Items.Clear();
                if (!Field.Required) _customisedList.Items.Insert(0, new ListItem("(None)", "0"));
                if (Items_List != null && Items_List.Count != 0)
                {
                    _customisedList.Items.AddRange(Items_List.ToArray());
                    //   _customisedList.AutoPostBack = true;

                    _customisedList.SelectedIndex = _customisedList.Items.IndexOf(_customisedList.Items.FindByText(_fieldVal.LookupValue));
                }

                base.Field.SetCustomProperty("Items", _fieldVal.LookupId);
            }

            if (field.GetProperty(CustomDropDownList.PARENT_COLUMN) == null)
            {
                _customisedList.Attributes.Add("parentColumnId", "");
            }
            else
            {
                _customisedList.Attributes.Add("parentColumnId", field.GetProperty(CustomDropDownList.PARENT_COLUMN));
            }
            _customisedList.Attributes.Add("linkColumnId", field.GetProperty(CustomDropDownList.LINK_COLUMN));
            _customisedList.Attributes.Add("columnId", field.Id.ToString());

            if (field.GetCustomProperty(CustomDropDownList.ADDING_NEW_VALUES) != string.Empty)
            {
                if (Convert.ToBoolean(field.GetCustomProperty(CustomDropDownList.ADDING_NEW_VALUES)))
                {
                    _new_element = (LinkButton)TemplateContainer.FindControl("lbAddNew");
                    _new_element.Visible = true;
                    if (Convert.ToBoolean(field.GetCustomProperty(CustomDropDownList.NEW_FORM)))
                    {
                        SPWeb w = SPContext.Current.Site.OpenWeb(field.LookupWebId);
                        string weburl = w.Url;
                        SPList sourceList = w.Lists[new Guid(field.LookupList)];
                        SPForm form = sourceList.Forms[PAGETYPE.PAGE_NEWFORM];
                        string url = form.Url;
                        url = weburl + "/" + form.Url;
                        string title = field.InternalName;
                        //_new_element.OnClientClick = "javascript:SP.UI.ModalDialog.showModalDialog({ url: '" + url + "', title: '" + title + "', dialogReturnValueCallback: RefreshOnDialogClose});";
                        _new_element.OnClientClick = "javascript:SP.UI.ModalDialog.showModalDialog({ url: '" + url + "', title: '" + title + "', dialogReturnValueCallback:  callbackMethod" + this.Field.Id.ToString("n") + "});";
                    }
                    else
                    {
                        _new_element.Click += new EventHandler(_new_element_Click);
                        _add_entry = (LinkButton)TemplateContainer.FindControl("lbAddEntry");
                        _add_entry.Click += new EventHandler(_add_entry_Click);
                        _cancel_button = (LinkButton)TemplateContainer.FindControl("lbCancel");
                        _cancel_button.Click += new EventHandler(_cancel_button_Click);
                    }
                }
            }
        }

        private void fill_controls_for_multi()
        {
            CustomDropDownList field = base.Field as CustomDropDownList;

            bool auto_completion_option_enabled = Convert.ToString(field.GetCustomProperty(CustomDropDownList.AUTO_COMPLETE)) == Boolean.TrueString;
            if (auto_completion_option_enabled)
            {
                _auto_completion_box = (TextBox)TemplateContainer.FindControl("auto_complete");
                _auto_completion_box.Visible = true;
                _auto_completion_box.Text = _fieldVal.LookupValue;
                _auto_completion_box.TextChanged += new EventHandler(_auto_completion_box_TextChanged);
                _auto_completion_box.AutoPostBack = true;

            }

            _table = (HtmlTable)TemplateContainer.FindControl("MultiColumnTable");
            _table.Visible = true;
            left_box = (ListBox)TemplateContainer.FindControl("LeftBox");
            left_box.Visible = true;
            right_box = (ListBox)TemplateContainer.FindControl("RightBox");
            right_box.Visible = true;
            add_button = (Button)TemplateContainer.FindControl("AddButton");
            add_button.Visible = true;
            add_button.Click += new EventHandler(add_button_Click);
            remove_button = (Button)TemplateContainer.FindControl("RemoveButton");
            remove_button.Visible = true;
            remove_button.Click += new EventHandler(remove_button_Click);

            bool has_link = Convert.ToString(field.GetCustomProperty(CustomDropDownList.LINK)) == Boolean.TrueString;

            //_customisedList.ID = "Lookup";
            left_box.ToolTip = string.Format(CultureInfo.InvariantCulture, "{0}", Field.InternalName);

            if (!has_link || (Convert.ToBoolean(field.GetCustomProperty(CustomDropDownList.SHOW_ALL_VALUES)) && ParentValueNullOREmpty()))
            {
                // Initialize_multi_value((CustomDropDownList)base.Field);
                // BuildAvailableItems(ref left_box);
                string s = string.Empty;
                foreach (SPFieldLookupValue i in _fieldVals)
                {
                    s = s + i.LookupId.ToString() + ":";

                }
                base.Field.SetCustomProperty("Items", s);
            }
            else
            {
                SPFieldCollection field_coll = this.Fields;
                string selected_items = string.Empty;
                foreach (SPField f in field_coll)
                {
                    if (f.Id.ToString().Equals(Convert.ToString(field.GetProperty(CustomDropDownList.PARENT_COLUMN))))
                    {
                        selected_items = GetFieldValue(f);
                        break;
                    }
                }

                List<ListItem> item_list = new List<ListItem>();
                string linked_column = field.GetProperty(CustomDropDownList.LINK_COLUMN);

                foreach (string sel_value in selected_items.Split(':'))
                {
                    Helper.get_matched_items(field, sel_value, linked_column, ref item_list);
                }

                _availableItems.Clear();
                if (item_list != null && item_list.Count != 0)
                {
                    _availableItems.AddRange(item_list.ToArray());
                    foreach (SPFieldLookupValue i in _fieldVals)
                    {
                        ListItem item = new ListItem(i.LookupValue, i.LookupId.ToString());
                        left_box.Items.Remove(item);
                        _availableItems.Remove(item);
                    }

                    //_customisedList.AutoPostBack = true;

                    //_customisedList.SelectedIndex = _customisedList.Items.IndexOf(_customisedList.Items.FindByText(_fieldVal.LookupValue));
                }

                string s = string.Empty;
                foreach (SPFieldLookupValue i in _fieldVals)
                {
                    s = s + i.LookupId.ToString() + ":";

                }
                base.Field.SetCustomProperty("Items", s);
                s = string.Empty;
                foreach (ListItem it in right_box.Items)
                {
                    s = s + it.Value + ":";
                }
                base.Field.SetCustomProperty("RightboxItems", s);
            }

            if (field.GetProperty(CustomDropDownList.PARENT_COLUMN) == null)
            {
                left_box.Attributes.Add("parentColumnId", "");
                right_box.Attributes.Add("parentColumnId", "");
                right_box.Attributes.Add("columnId", field.Id.ToString());
                right_box.Attributes.Add("side", "");
            }
            else
            {
                left_box.Attributes.Add("parentColumnId", field.GetProperty(CustomDropDownList.PARENT_COLUMN));
                right_box.Attributes.Add("parentColumnId", field.GetProperty(CustomDropDownList.PARENT_COLUMN));
                right_box.Attributes.Add("columnId", field.Id.ToString());
                right_box.Attributes.Add("side", "right");

            }
            left_box.Attributes.Add("linkColumnId", field.GetProperty(CustomDropDownList.LINK_COLUMN));
            left_box.Attributes.Add("columnId", field.Id.ToString());

            if (field.GetCustomProperty(CustomDropDownList.ADDING_NEW_VALUES).ToString() != string.Empty)
            {
                if (Convert.ToBoolean(field.GetCustomProperty(CustomDropDownList.ADDING_NEW_VALUES)))
                {
                    _new_element = (LinkButton)TemplateContainer.FindControl("lbAddNew");
                    _new_element.Visible = true;
                    if (Convert.ToBoolean(field.GetCustomProperty(CustomDropDownList.NEW_FORM)))
                    {
                        SPWeb w = SPContext.Current.Site.OpenWeb(field.LookupWebId);
                        string weburl = w.Url;
                        SPList sourceList = w.Lists[new Guid(field.LookupList)];
                        SPForm form = sourceList.Forms[PAGETYPE.PAGE_NEWFORM];
                        string url = form.Url;
                        url = weburl + "/" + form.Url;
                        string title = field.InternalName;
                        //_new_element.OnClientClick = "javascript:SP.UI.ModalDialog.showModalDialog({ url: '" + url + "', title: '" + title + "', dialogReturnValueCallback: RefreshOnDialogClose});";
                        _new_element.OnClientClick = "javascript:SP.UI.ModalDialog.showModalDialog({ url: '" + url + "', title: '" + title + "', dialogReturnValueCallback:  callbackMethod" + this.Field.Id.ToString("n") + "});";
                    }
                    else
                    {
                        _new_element.Click += new EventHandler(_new_element_Click);
                        _add_entry = (LinkButton)TemplateContainer.FindControl("lbAddEntry");
                        _add_entry.Click += new EventHandler(_add_entry_Click);
                        _cancel_button = (LinkButton)TemplateContainer.FindControl("lbCancel");
                        _cancel_button.Click += new EventHandler(_cancel_button_Click);
                    }
                }
            }
        }

        void _new_element_Click(object sender, EventArgs e)
        {
            _new_value = (TextBox)TemplateContainer.FindControl("txtNewEntry");
            _new_value.Visible = true;
            _add_entry = (LinkButton)TemplateContainer.FindControl("lbAddEntry");
            _add_entry.Visible = true;
            _add_entry.Click += new EventHandler(_add_entry_Click);

            _cancel_button = (LinkButton)TemplateContainer.FindControl("lbCancel");
            _cancel_button.Visible = true;
            _cancel_button.Click += new EventHandler(_cancel_button_Click);
        }

        void _cancel_button_Click(object sender, EventArgs e)
        {
            _new_value = (TextBox)TemplateContainer.FindControl("txtNewEntry");
            _new_value.Visible = false;
            _new_value.Text = string.Empty;
            _add_entry = (LinkButton)TemplateContainer.FindControl("lbAddEntry");
            _add_entry.Visible = false;
            _cancel_button = (LinkButton)TemplateContainer.FindControl("lbCancel");
            _cancel_button.Visible = false;
        }

        void _add_entry_Click(object sender, EventArgs e)
        {
            CustomDropDownList field = base.Field as CustomDropDownList;
            if (field != null)
            {
                SPWeb w = SPContext.Current.Site.OpenWeb(field.LookupWebId);
                SPList sourceList = w.Lists[new Guid(field.LookupList)];


                _new_value = (TextBox)TemplateContainer.FindControl("txtNewEntry");

                if (_new_value != null)
                {
                    string selected_items = string.Empty;
                    string parent_field_id = Convert.ToString(field.GetCustomProperty(CustomDropDownList.PARENT_COLUMN));
                    Get_parent_selected_values(parent_field_id, ref selected_items);

                    foreach (string s in selected_items.Split(':'))
                    {
                        if (s != "")
                        {
                            SPListItem item = sourceList.Items.Add();

                            item[new Guid(field.LookupField)] = _new_value.Text;

                            item[new Guid(Convert.ToString(field.GetProperty(CustomDropDownList.LINK_COLUMN)))] = s;

                            item.Update();

                            if (field.AllowMultipleValues)
                            {
                                right_box.Items.Insert(right_box.Items.Count, new ListItem(_new_value.Text, item.ID.ToString()));
                            }
                            else
                            {
                                _customisedList.Items.Insert(_customisedList.Items.Count, new ListItem(_new_value.Text, item.ID.ToString()));
                            }
                        }
                    }
                    if (selected_items == string.Empty)
                    {
                        SPListItem item = sourceList.Items.Add();

                        item[new Guid(field.LookupField)] = _new_value.Text;

                        if (!string.IsNullOrEmpty(Convert.ToString(field.GetProperty(CustomDropDownList.LINK_COLUMN))))
                        {
                            item[new Guid(Convert.ToString(field.GetProperty(CustomDropDownList.LINK_COLUMN)))] = "";
                        }
                        item.Update();

                        if (field.AllowMultipleValues)
                        {
                            right_box.Items.Insert(right_box.Items.Count, new ListItem(_new_value.Text, item.ID.ToString()));
                        }
                        else
                        {
                            _customisedList.Items.Insert(_customisedList.Items.Count, new ListItem(_new_value.Text, item.ID.ToString()));
                        }
                    }
                }

            }
            _new_value = (TextBox)TemplateContainer.FindControl("txtNewEntry");
            _new_value.Visible = false;
            _new_value.Text = string.Empty;
            _add_entry = (LinkButton)TemplateContainer.FindControl("lbAddEntry");
            _add_entry.Visible = false;
            _cancel_button = (LinkButton)TemplateContainer.FindControl("lbCancel");
            _cancel_button.Visible = false;
        }

        private void CreateStandardSelect()
        {
            EnsureChildControls();
            CustomDropDownList field = base.Field as CustomDropDownList;

            bool has_link = Convert.ToString(field.GetCustomProperty(CustomDropDownList.LINK)) == Boolean.TrueString;
            bool parent_empty = Convert.ToString(field.GetCustomProperty(CustomDropDownList.SHOW_ALL_VALUES)) == Boolean.TrueString;
            bool auto_completion_option_enabled = Convert.ToString(field.GetCustomProperty(CustomDropDownList.AUTO_COMPLETE)) == Boolean.TrueString;
            string parent_column = Convert.ToString(field.GetCustomProperty(CustomDropDownList.PARENT_COLUMN));
            string link_column = Convert.ToString(field.GetCustomProperty(CustomDropDownList.LINK_COLUMN));

            _customisedList = (DropDownList)TemplateContainer.FindControl("customList");
            _customisedList.Visible = true;

            if (auto_completion_option_enabled)
            {
                _auto_completion_box = (TextBox)TemplateContainer.FindControl("auto_complete");
                _auto_completion_box.Visible = true;
                _auto_completion_box.Text = "";
                _auto_completion_box.TextChanged += new EventHandler(_auto_completion_box_TextChanged);
                _auto_completion_box.AutoPostBack = true;
            }

            _customisedList.Attributes.Add("siteId", field.LookupWebId.ToString());
            _customisedList.Attributes.Add("listId", field.LookupList.ToString());
            _customisedList.Attributes.Add("fieldId", field.LookupField.ToString());
            if (field.GetProperty(CustomDropDownList.PARENT_COLUMN) == null)
            {
                _customisedList.Attributes.Add("parentColumnId", "");
            }
            else
            {
                _customisedList.Attributes.Add("parentColumnId", field.GetProperty(CustomDropDownList.PARENT_COLUMN));
            }
            _customisedList.Attributes.Add("linkColumnId", field.GetProperty(CustomDropDownList.LINK_COLUMN));
            _customisedList.Attributes.Add("columnId", field.Id.ToString());

            if (has_child(field.Id.ToString()))
            {
                _customisedList.SelectedIndexChanged += new EventHandler(l_SelectedIndexChanged);
                _customisedList.AutoPostBack = true;
            }
            _customisedList.ID = "Lookup";
            _customisedList.ToolTip = string.Format(CultureInfo.InvariantCulture, "{0}", Field.InternalName);

            if (!has_link || parent_empty)
            {
                Initialize((CustomDropDownList)base.Field);
                if (_availableItems != null && _availableItems.Count != 0)
                {
                    _customisedList.Items.Clear();
                    //    _customisedList.AutoPostBack = true;
                    _customisedList.Items.AddRange(_availableItems.ToArray());
                }
            }
            if (!Field.Required) { _customisedList.Items.Insert(0, new ListItem("(None)", "0")); }

            if (field.GetCustomProperty(CustomDropDownList.ADDING_NEW_VALUES) != string.Empty)
            {
                if (Convert.ToBoolean(field.GetCustomProperty(CustomDropDownList.ADDING_NEW_VALUES)))
                {
                    _new_element = (LinkButton)TemplateContainer.FindControl("lbAddNew");
                    _new_element.Visible = true;
                    if (Convert.ToBoolean(field.GetCustomProperty(CustomDropDownList.NEW_FORM)))
                    {
                        SPWeb w = SPContext.Current.Site.OpenWeb(field.LookupWebId);
                        string weburl = w.Url;
                        SPList sourceList = w.Lists[new Guid(field.LookupList)];
                        SPForm form = sourceList.Forms[PAGETYPE.PAGE_NEWFORM];
                        string url = form.Url;
                        url = weburl + "/" + form.Url;
                        string title = field.InternalName;
                        //_new_element.OnClientClick = "javascript:SP.UI.ModalDialog.showModalDialog({ url: '" + url + "', title: '" + title + "' , dialogReturnValueCallback: RefreshOnDialogClose}); return false";
                        _new_element.OnClientClick = "javascript:SP.UI.ModalDialog.showModalDialog({ url: '" + url + "', title: '" + title + "', dialogReturnValueCallback:  callbackMethod" + this.Field.Id.ToString("n") + "});";
                    }
                    else
                    {
                        _new_element.Click += new EventHandler(_new_element_Click);
                        _add_entry = (LinkButton)TemplateContainer.FindControl("lbAddEntry");
                        _add_entry.Click += new EventHandler(_add_entry_Click);
                        _cancel_button = (LinkButton)TemplateContainer.FindControl("lbCancel");
                        _cancel_button.Click += new EventHandler(_cancel_button_Click);
                    }
                }
            }
        }

        private void CreateStandardSelect_for_multi()
        {
            EnsureChildControls();
            CustomDropDownList field = base.Field as CustomDropDownList;

            bool has_link = Convert.ToString(field.GetCustomProperty(CustomDropDownList.LINK)) == Boolean.TrueString;
            bool parent_empty = Convert.ToString(field.GetCustomProperty(CustomDropDownList.SHOW_ALL_VALUES)) == Boolean.TrueString;
            bool auto_completion_option_enabled = Convert.ToString(field.GetCustomProperty(CustomDropDownList.AUTO_COMPLETE)) == Boolean.TrueString;
            string parent_column = Convert.ToString(field.GetCustomProperty(CustomDropDownList.PARENT_COLUMN));
            string link_column = Convert.ToString(field.GetCustomProperty(CustomDropDownList.LINK_COLUMN));

            _table = (HtmlTable)TemplateContainer.FindControl("MultiColumnTable");
            _table.Visible = true;
            left_box = (ListBox)TemplateContainer.FindControl("LeftBox");
            left_box.Visible = true;
            right_box = (ListBox)TemplateContainer.FindControl("RightBox");
            right_box.Visible = true;
            add_button = (Button)TemplateContainer.FindControl("AddButton");
            add_button.Visible = true;
            add_button.Click += new EventHandler(add_button_Click);
            remove_button = (Button)TemplateContainer.FindControl("RemoveButton");
            remove_button.Visible = true;
            remove_button.Click += new EventHandler(remove_button_Click);

            if (auto_completion_option_enabled)
            {
                _auto_completion_box = (TextBox)TemplateContainer.FindControl("auto_complete");
                _auto_completion_box.Visible = true;
                _auto_completion_box.Text = "";
                _auto_completion_box.TextChanged += new EventHandler(_auto_completion_box_TextChanged);
                _auto_completion_box.AutoPostBack = true;
            }

            left_box.Attributes.Add("siteId", field.LookupWebId.ToString());
            left_box.Attributes.Add("listId", field.LookupList.ToString());
            left_box.Attributes.Add("fieldId", field.LookupField.ToString());
            if (field.GetProperty(CustomDropDownList.PARENT_COLUMN) == null)
            {
                left_box.Attributes.Add("parentColumnId", "");
                right_box.Attributes.Add("parentColumnId", "");
                right_box.Attributes.Add("columnId", field.Id.ToString());
                right_box.Attributes.Add("side", "");
            }
            else
            {
                left_box.Attributes.Add("parentColumnId", field.GetProperty(CustomDropDownList.PARENT_COLUMN));
                right_box.Attributes.Add("parentColumnId", field.GetProperty(CustomDropDownList.PARENT_COLUMN));
                right_box.Attributes.Add("columnId", field.Id.ToString());
                right_box.Attributes.Add("side", "right");
            }
            left_box.Attributes.Add("linkColumnId", field.GetProperty(CustomDropDownList.LINK_COLUMN));

            //_customisedList.ID = "Lookup";
            left_box.ToolTip = string.Format(CultureInfo.InvariantCulture, "{0}", Field.InternalName);

            if (!has_link || parent_empty)
            {
                Initialize_multi_value((CustomDropDownList)base.Field);
                BuildAvailableItems(ref left_box);
            }
            else
            {
                foreach (ListItem item in right_box.Items)
                {
                    left_box.Items.Remove(item);
                }
            }
            // if (!Field.Required) { left_box.Items.Insert(0, new ListItem("(None)", "0")); }

            if (field.GetCustomProperty(CustomDropDownList.ADDING_NEW_VALUES) != string.Empty)
            {
                if (Convert.ToBoolean(field.GetCustomProperty(CustomDropDownList.ADDING_NEW_VALUES)))
                {
                    _new_element = (LinkButton)TemplateContainer.FindControl("lbAddNew");
                    _new_element.Visible = true;
                    if (Convert.ToBoolean(field.GetCustomProperty(CustomDropDownList.NEW_FORM)))
                    {
                        SPWeb w = SPContext.Current.Site.OpenWeb(field.LookupWebId);
                        string weburl = w.Url;
                        SPList sourceList = w.Lists[new Guid(field.LookupList)];
                        SPForm form = sourceList.Forms[PAGETYPE.PAGE_NEWFORM];
                        string url = form.Url;
                        url = weburl + "/" + form.Url;
                        string title = field.InternalName;
                        //_new_element.OnClientClick = "javascript:SP.UI.ModalDialog.showModalDialog({ url: '" + url + "', title: '" + title + "' , dialogReturnValueCallback: RefreshOnDialogClose}); return false";
                        _new_element.OnClientClick = "javascript:SP.UI.ModalDialog.showModalDialog({ url: '" + url + "', title: '" + title + "', dialogReturnValueCallback:  callbackMethod" + this.Field.Id.ToString("n") + "});";
                    }
                    else
                    {
                        _new_element.Click += new EventHandler(_new_element_Click);
                        _add_entry = (LinkButton)TemplateContainer.FindControl("lbAddEntry");
                        _add_entry.Click += new EventHandler(_add_entry_Click);
                        _cancel_button = (LinkButton)TemplateContainer.FindControl("lbCancel");
                        _cancel_button.Click += new EventHandler(_cancel_button_Click);
                    }
                }
            }
        }

        void _auto_completion_box_TextChanged(object sender, EventArgs e)
        {
            CustomDropDownList cddlist = (CustomDropDownList)base.Field;
            List<ListItem> item_list = new List<ListItem>();

            bool has_link = Convert.ToString(cddlist.GetCustomProperty(CustomDropDownList.LINK)) == Boolean.TrueString;
            if (has_link)
            {
                string sel_value = _customisedList.Attributes["parent_selected_value"].ToString();

                string linked_column = _customisedList.Attributes["linkColumnId"].ToString();

                Helper.get_matched_items(cddlist, sel_value, linked_column, ref item_list);
            }
            else
            {
                Initialize(cddlist);
                item_list = _availableItems;
            }
            List<ListItem> filtered_item = get_filtered_items(item_list, _auto_completion_box.Text.ToUpper());
            _customisedList.Items.Clear();
            _customisedList.Items.Insert(0, new ListItem("(None)", "0"));
            _customisedList.Items.AddRange(filtered_item.ToArray());
            _customisedList.AutoPostBack = true;
            _customisedList.SelectedIndexChanged += new EventHandler(l_SelectedIndexChanged);
        }

        void l_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_auto_completion_box != null)
            {
                _auto_completion_box.Text = _customisedList.SelectedItem.Text;
            }

            //CustomDropDownList field = base.Field as CustomDropDownList;
            //SPList list = SPContext.Current.List;
            //SPFieldCollection fields = list.Fields;

            //List<Control> list_box_controls = new List<Control>();
            //List<Control> dropdownlist_controls = new List<Control>();
            //FindControlRecursive(this.Page, typeof(ListBox), ref list_box_controls);
            //FindControlRecursive(this.Page, typeof(DropDownList), ref dropdownlist_controls);

            //ListItemCollection parent_field_right_box_items = null;
            //string parent_field_selected_value = _customisedList.SelectedItem.Value;

            //Update_child_controls(fields, base.Field, parent_field_right_box_items, parent_field_selected_value, list_box_controls, dropdownlist_controls);

            //

            List<Control> PossibleControls = new List<Control>();
            FindControlRecursive(this.Page, typeof(CustomLookupField.CustomDropDownListControl), ref PossibleControls);

            string SelectValue = _customisedList.SelectedItem.Value;
            
            UpdateChildLinkedControl(base.Field, SelectValue, ref PossibleControls);
            
            /*
            List<Control> control_list = new List<Control>();

            FindControlRecursive(this.Page, typeof(DropDownList), ref control_list);

            foreach (SPField f in fields)
            {
                if (field.Id.ToString() == f.GetProperty(CustomDropDownList.PARENT_COLUMN))
                {

                    foreach (Control c in control_list)
                    {
                        DropDownList l = (DropDownList)c;

                        if (l.Attributes["parentColumnId"] == null)
                        {
                            continue;
                        }
                        if (l.Attributes["parentColumnId"].ToString() == f.GetProperty(CustomDropDownList.PARENT_COLUMN))
                        {
                            string sel_value = string.Empty;

                            List<ListItem> item_list = new List<ListItem>();

                            string linked_column = f.GetProperty(CustomDropDownList.LINK_COLUMN);

                            CustomDropDownList custddl = f as CustomDropDownList;
                            string selected_items = string.Empty;
                            if (field.AllowMultipleValues)
                            {
                                foreach (ListItem it in right_box.Items)
                                {
                                    sel_value = it.Value;
                                    selected_items = selected_items + it.Value + ":";
                                    Helper.get_matched_items(custddl, sel_value, linked_column, ref item_list);
                                }
                            }
                            else
                            {
                                sel_value = _customisedList.SelectedItem.Value;
                                selected_items = sel_value;
                                Helper.get_matched_items(custddl, sel_value, linked_column, ref item_list);
                            }


                            if (item_list != null && item_list.Count != 0)
                            {
                                l.Items.Clear();
                                l.Items.Insert(0, new ListItem("(None)", "0"));
                                l.Items.AddRange(item_list.ToArray());
                                l.SelectedIndexChanged += new EventHandler(l_SelectedIndexChanged);
                                l.AutoPostBack = true;
                                l.Attributes.Add("parent_selected_value", selected_items);
                                custddl.SetCustomProperty(CustomDropDownList.PARENT_SELECTED_VALUES, selected_items);
                                custddl.Update();
                            }
                            else
                            {
                                if (Convert.ToBoolean(custddl.GetCustomProperty(CustomDropDownList.SHOW_ALL_VALUES)))
                                {
                                    Initialize(custddl);
                                    item_list = _availableItems;
                                    l.Items.Clear();
                                    l.Items.Insert(0, new ListItem("(None)", "0"));
                                    l.Items.AddRange(item_list.ToArray());
                                    l.SelectedIndexChanged += new EventHandler(l_SelectedIndexChanged);
                                    l.AutoPostBack = true;
                                    l.Attributes.Add("parent_selected_value", selected_items);
                                    custddl.SetCustomProperty(CustomDropDownList.PARENT_SELECTED_VALUES, selected_items);
                                    custddl.Update();
                                }
                            }
                        }
                    }
                }
            }

            control_list.Clear();
            FindControlRecursive(this.Page, typeof(ListBox), ref control_list);
            foreach (SPField f in fields)
            {
                if (field.Id.ToString() == f.GetProperty(CustomDropDownList.PARENT_COLUMN))
                {

                    foreach (Control c in control_list)
                    {
                        ListBox l = (ListBox)c;

                        if (l.Attributes["parentColumnId"] == null)
                        {
                            continue;
                        }
                        Boolean is_left_box;
                        if (l.Attributes["side"] == null)
                        {
                            is_left_box = true;
                        }
                        else
                        {
                            if (l.Attributes["side"].ToString().Equals("right"))
                            {
                                is_left_box = false;
                            }
                            else
                            {
                                is_left_box = true;
                            }
                        }
                        ListItemCollection parent_field_right_box_items = new ListItemCollection();
                        string parent_field_selected_value = string.Empty;
                        if ((l.Attributes["parentColumnId"].ToString() == f.GetProperty(CustomDropDownList.PARENT_COLUMN)) && is_left_box)
                        {
                            string sel_value = string.Empty;

                            List<ListItem> item_list = new List<ListItem>();

                            string linked_column = f.GetProperty(CustomDropDownList.LINK_COLUMN);

                            CustomDropDownList custddl = f as CustomDropDownList;

                            string selected_items = string.Empty;
                            if (field.AllowMultipleValues)
                            {
                                foreach (ListItem it in right_box.Items)
                                {
                                    sel_value = it.Value;
                                    selected_items = selected_items + it.Value + ":";
                                    Helper.get_matched_items(custddl, sel_value, linked_column, ref item_list);
                                }
                            }
                            else
                            {
                                sel_value = _customisedList.SelectedItem.Value;
                                selected_items = sel_value;
                                Helper.get_matched_items(custddl, sel_value, linked_column, ref item_list);
                            }

                            foreach (Control c1 in control_list)
                            {
                                ListBox l1 = (ListBox)c1;

                                if (l1.Attributes["parentColumnId"] == null)
                                {
                                    continue;
                                }
                                Boolean is_right_box;
                                if (l1.Attributes["side"] == null)
                                {
                                    is_right_box = false;
                                }
                                else
                                {
                                    if (l1.Attributes["side"].ToString().Equals("right"))
                                    {
                                        is_right_box = true;
                                    }
                                    else
                                    {
                                        is_right_box = false;
                                    }
                                }
                                if ((l1.Attributes["parentColumnId"].ToString() == l.Attributes["parentColumnId"].ToString()) && is_right_box)
                                {
                                    ListItemCollection item_collection = new ListItemCollection();
                                    foreach (ListItem rightbox_item in l1.Items)
                                    {
                                        if (!item_list.Contains(rightbox_item))
                                        {
                                            item_collection.Add(rightbox_item);
                                        }
                                    }
                                    foreach (ListItem item in item_collection)
                                    {
                                        l1.Items.Remove(item);
                                    }
                                    foreach (ListItem rightbox_item in l1.Items)
                                    {
                                        item_list.Remove(rightbox_item);
                                    }
                                    parent_field_right_box_items = l1.Items;
                                }
                            }

                            if (item_list != null && item_list.Count != 0)
                            {
                                l.Items.Clear();
                                l.Items.AddRange(item_list.ToArray());

                                l.Attributes.Add("parent_selected_value", selected_items);
                                custddl.SetCustomProperty(CustomDropDownList.PARENT_SELECTED_VALUES, selected_items);
                                custddl.Update();
                            }
                            else
                            {
                                if (Convert.ToBoolean(custddl.GetCustomProperty(CustomDropDownList.SHOW_ALL_VALUES)))
                                {
                                    Initialize_multi_value(custddl);
                                    item_list = _availableItems;
                                    l.Items.Clear();
                                    l.Items.AddRange(item_list.ToArray());
                                    l.Attributes.Add("parent_selected_value", selected_items);
                                    custddl.SetCustomProperty(CustomDropDownList.PARENT_SELECTED_VALUES, selected_items);
                                    custddl.Update();
                                }
                            }
            
                            //Cleaning all child controls
                            List<Control> list_box_controls = new List<Control>();
                            List<Control> dropdownlist_controls = new List<Control>();
                            FindControlRecursive(this.Page, typeof(ListBox), ref list_box_controls);
                            FindControlRecursive(this.Page, typeof(DropDownList), ref dropdownlist_controls);
                                                                           
                            Update_child_controls(fields, f, parent_field_right_box_items, parent_field_selected_value, list_box_controls, dropdownlist_controls);
                            
                        }
                    }
                }
            }*/
        }

        public override object Value
        {
            get
            {
                EnsureChildControls();
                CustomDropDownList cusdrop = base.Field as CustomDropDownList;
                if (cusdrop.AllowMultipleValues)
                {
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

                    this.SetAdditionalFields(_vals);
                    return _vals;
                }
                else
                {
                    string s = string.Empty;
                    foreach (ListItem item in _customisedList.Items)
                    {
                        s = s + item.Text + ":" + item.Value + ",";

                    }
                    base.Field.SetCustomProperty("Items", s);
                    if (_customisedList.SelectedItem != null)
                        this.SetAdditionalFields(_customisedList.SelectedItem.Value);
                    return _customisedList.SelectedValue;
                }
            }

            set
            {
                EnsureChildControls();
                CustomDropDownList cusdrop = base.Field as CustomDropDownList;
                if (cusdrop.AllowMultipleValues)
                {
                    base.Value = value as SPFieldLookupValueCollection;
                }
                else
                {
                    _customisedList.SelectedValue = _fieldVal.LookupValue;
                    base.Value = _fieldVal.LookupValue;
                }
            }
        }

        public static void FindControlRecursive(Control Root, Type type, ref List<Control> collect)
        {
            if (Root.GetType() == type) { collect.Add(Root); }

            foreach (Control Ctl in Root.Controls)
            {
                FindControlRecursive(Ctl, type, ref collect);
            }
        }

        private void SetAdditionalFields(SPFieldLookupValueCollection col)
        {
            CustomDropDownList field = base.Field as CustomDropDownList;
            string fields = string.Empty;

            if (field.GetCustomProperty(CustomDropDownList.ADDITIONAL_FIELDS) != string.Empty)
            {
                fields = Convert.ToString(field.GetCustomProperty(CustomDropDownList.ADDITIONAL_FIELDS));
            }

            if ((field != null) && !string.IsNullOrEmpty(fields))
            {
                SPWeb w = SPContext.Current.Site.OpenWeb(field.LookupWebId);
                SPList sourceList = w.Lists[new Guid(field.LookupList)];
                SPList currentList = SPContext.Current.List;

                foreach (string str in fields.Split(';'))
                {
                    string str2 = field.ToString() + ":" + sourceList.Fields[new Guid(str)].Title;
                    if (str2.Length > 0x20)
                    {
                        str2 = str2.Substring(0, 0x20);
                    }

                    if (sourceList.Fields.Contains(new Guid(str)))
                    {
                        SPFieldLookupValueCollection values = new SPFieldLookupValueCollection();
                        SPFieldLookup fieldByInternalName = currentList.Fields[str2] as SPFieldLookup;
                        if (fieldByInternalName != null)
                        {
                            foreach (SPFieldLookupValue value2 in col)
                            {
                                if (value2.LookupId != 0)
                                {
                                    int c = sourceList.Items.Count;
                                    SPListItem itemById = null;
                                    for (int i = 0; i < sourceList.Items.Count; i++)
                                    {
                                        if (sourceList.Items[i].ID == value2.LookupId)
                                        {
                                            itemById = sourceList.Items[i];
                                        }
                                    }

                                    if (itemById[new Guid(str)] == null)
                                    {
                                        values.Add(new SPFieldLookupValue(value2.LookupId, string.Empty));
                                    }
                                    else
                                    {
                                        string fieldValueAsText = itemById.Fields[new Guid(str)].GetFieldValueAsText(itemById[new Guid(str)]);
                                        values.Add(new SPFieldLookupValue(value2.LookupId, fieldValueAsText));
                                    }
                                }
                            }
                            if (fieldByInternalName.AllowMultipleValues)
                            {
                                base.Item[str2] = values;
                            }
                            else if (values.Count > 0)
                            {
                                base.Item[str2] = values[0].ToString();
                            }
                            else
                            {
                                base.Item[str2] = null;
                            }
                        }
                    }

                }
            }
        }

        private void SetAdditionalFields(string value)
        {
            SPFieldLookupValueCollection col = new SPFieldLookupValueCollection();
            col.Add(new SPFieldLookupValue(value));
            this.SetAdditionalFields(col);
        }

        private void SetValue()
        {
            CustomDropDownList field = base.Field as CustomDropDownList;
            if (field.AllowMultipleValues)
            {
                string str = field.GetProperty(CustomDropDownList.PARENT_COLUMN);
                if (!string.IsNullOrEmpty(str) && Convert.ToString(SPContext.Current.ListItem[new Guid(str)]) == "" && !Convert.ToBoolean(field.GetCustomProperty(CustomDropDownList.SHOW_ALL_VALUES)))
                {
                    return;
                }

                if (_availableItems != null && _fieldVals != null && _fieldVals.Count >= 0)
                {
                    right_box.Items.Clear();
                    foreach (SPFieldLookupValue i in _fieldVals)
                    {
                        ListItem item = new ListItem(i.LookupValue, i.LookupId.ToString());
                        right_box.Items.Add(item);
                        _availableItems.Remove(item);
                    }

                    left_box.Items.Clear();
                    foreach (ListItem item in _availableItems)
                    {
                        left_box.Items.Add(item);
                    }
                }
                else if (Convert.ToBoolean(field.GetCustomProperty(CustomDropDownList.SHOW_ALL_VALUES)))
                {
                    left_box.Items.Clear();
                    foreach (ListItem item in _availableItems)
                    {
                        left_box.Items.Add(item);
                    }
                }
            }
            else
            {
                if (_fieldVal != null)
                {
                    _customisedList.SelectedIndex = _customisedList.Items.IndexOf(new ListItem(_fieldVal.LookupValue, _fieldVal.LookupId.ToString()));
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
        }

        public void Update_child_controls(SPFieldCollection fields, SPField parent_field, ListItemCollection parent_field_right_box_items, String parent_field_selected_value, List<Control> list_box_controls, List<Control> dropdownlist_controls)
        {
            CustomDropDownList parent_field_ccddl = parent_field as CustomDropDownList;
            foreach (SPField child_field in fields)
            {
                if (parent_field.Id.ToString() == child_field.GetProperty(CustomDropDownList.PARENT_COLUMN))
                {
                    foreach (Control list_box_control in list_box_controls)
                    {
                        ListBox list_box_left = (ListBox)list_box_control;

                        if (list_box_left.Attributes["parentColumnId"] == null)
                        {
                            continue;
                        }
                        Boolean is_left_box;
                        if (list_box_left.Attributes["side"] == null)
                        {
                            is_left_box = true;
                        }
                        else
                        {
                            if (list_box_left.Attributes["side"].ToString().Equals("right"))
                            {
                                list_box_left.Items.Clear();
                                is_left_box = false;
                            }
                            else
                            {
                                is_left_box = true;
                            }
                        }
                        if ((list_box_left.Attributes["parentColumnId"].ToString() == child_field.GetProperty(CustomDropDownList.PARENT_COLUMN)) && is_left_box)
                        {
                            string sel_value = string.Empty;

                            List<ListItem> item_list = new List<ListItem>();

                            string linked_column = child_field.GetProperty(CustomDropDownList.LINK_COLUMN);

                            CustomDropDownList custddl = child_field as CustomDropDownList;

                            string selected_items = string.Empty;
                            if (parent_field_ccddl.AllowMultipleValues)
                            {
                                foreach (ListItem it in parent_field_right_box_items)
                                {
                                    sel_value = it.Value;
                                    selected_items = selected_items + it.Value + ":";
                                    Helper.get_matched_items(custddl, sel_value, linked_column, ref item_list);
                                }
                                foreach (Control list_box_control1 in list_box_controls)
                                {
                                    ListBox list_box_right = (ListBox)list_box_control1;

                                    if (list_box_right.Attributes["parentColumnId"] == null)
                                    {
                                        continue;
                                    }
                                    Boolean is_right_box;
                                    if (list_box_right.Attributes["side"] == null)
                                    {
                                        is_right_box = false;
                                    }
                                    else
                                    {
                                        if (list_box_right.Attributes["side"].ToString().Equals("right"))
                                        {
                                            is_right_box = true;
                                        }
                                        else
                                        {
                                            is_right_box = false;
                                        }
                                    }
                                    if ((list_box_right.Attributes["parentColumnId"].ToString() == list_box_left.Attributes["parentColumnId"].ToString()) && is_right_box)
                                    {
                                        ListItemCollection item_collection = new ListItemCollection();
                                        foreach (ListItem rightbox_item in list_box_right.Items)
                                        {
                                            if (!item_list.Contains(rightbox_item))
                                            {
                                                item_collection.Add(rightbox_item);
                                            }
                                        }
                                        foreach (ListItem item in item_collection)
                                        {
                                            list_box_right.Items.Remove(item);
                                        }
                                        foreach (ListItem rightbox_item in list_box_right.Items)
                                        {
                                            item_list.Remove(rightbox_item);
                                        }
                                        parent_field_right_box_items = list_box_right.Items;
                                    }
                                }
                            }
                            else
                            {
                                sel_value = parent_field_selected_value;
                                selected_items = sel_value;
                                Helper.get_matched_items(custddl, sel_value, linked_column, ref item_list);
                            }

                            list_box_left.Items.Clear();
                            if (item_list != null && item_list.Count != 0)
                            {
                                list_box_left.Items.AddRange(item_list.ToArray());
                                //  list_box_left.AutoPostBack = true;
                                list_box_left.Attributes.Add("parent_selected_value", selected_items);
                            }
                            else
                            {
                                if (Convert.ToBoolean(custddl.GetCustomProperty(CustomDropDownList.SHOW_ALL_VALUES)) && (string.IsNullOrEmpty(parent_field_selected_value) || parent_field_selected_value == "0"))
                                {
                                    Initialize_multi_value(custddl);
                                    item_list = _availableItems;
                                    list_box_left.Items.AddRange(item_list.ToArray());
                                    //     list_box_left.AutoPostBack = true;
                                    list_box_left.Attributes.Add("parent_selected_value", selected_items);
                                }
                            }
                            Update_child_controls(fields, child_field, parent_field_right_box_items, parent_field_selected_value, list_box_controls, dropdownlist_controls);
                        }
                    }
                    foreach (Control dropdownlist_control in dropdownlist_controls)
                    {
                        DropDownList ddl = (DropDownList)dropdownlist_control;

                        if (ddl.Attributes["parentColumnId"] == null)
                        {
                            continue;
                        }
                        if (ddl.Attributes["parentColumnId"].ToString() == child_field.GetProperty(CustomDropDownList.PARENT_COLUMN))
                        {
                            string sel_value = string.Empty;

                            List<ListItem> item_list = new List<ListItem>();

                            string linked_column = child_field.GetProperty(CustomDropDownList.LINK_COLUMN);

                            CustomDropDownList custddl = child_field as CustomDropDownList;
                            string selected_items = string.Empty;
                            if (parent_field_ccddl.AllowMultipleValues)
                            {
                                foreach (ListItem it in parent_field_right_box_items)
                                {
                                    sel_value = it.Value;
                                    selected_items = selected_items + it.Value + ":";
                                    Helper.get_matched_items(custddl, sel_value, linked_column, ref item_list);
                                }
                            }
                            else
                            {
                                sel_value = parent_field_selected_value;
                                selected_items = sel_value;
                                Helper.get_matched_items(custddl, sel_value, linked_column, ref item_list);
                            }

                            ddl.Items.Clear();
                            ddl.Items.Insert(0, new ListItem("(None)", "0"));
                            if (item_list != null && item_list.Count != 0)
                            {
                                ddl.Items.AddRange(item_list.ToArray());
                                //  ddl.SelectedIndexChanged += new EventHandler(l_SelectedIndexChanged);
                                //   ddl.AutoPostBack = true;
                                ddl.Attributes.Add("parent_selected_value", selected_items);
                            }
                            else
                            {
                                if (Convert.ToBoolean(custddl.GetCustomProperty(CustomDropDownList.SHOW_ALL_VALUES)) && (string.IsNullOrEmpty(parent_field_selected_value) || parent_field_selected_value == "0"))
                                {
                                    Initialize(custddl);
                                    item_list = _availableItems;

                                    ddl.Items.AddRange(item_list.ToArray());
                                    // ddl.SelectedIndexChanged += new EventHandler(l_SelectedIndexChanged);
                                    // ddl.AutoPostBack = true;
                                    ddl.Attributes.Add("parent_selected_value", selected_items);
                                }
                            }
                            Update_child_controls(fields, child_field, parent_field_right_box_items, parent_field_selected_value, list_box_controls, dropdownlist_controls);
                        }
                    }
                }
            }
        }

        public void Get_parent_selected_values(string parent_field_id, ref string selected_items)
        {
            CustomDropDownList field = base.Field as CustomDropDownList;
            SPList list = SPContext.Current.List;
            SPFieldCollection fields = list.Fields;

            foreach (SPField parent_field in fields)
            {
                if (parent_field.Id.ToString() == parent_field_id)
                {
                    CustomDropDownList parent_field_ccddl = parent_field as CustomDropDownList;
                    if (parent_field_ccddl.AllowMultipleValues)
                    {
                        List<Control> controls = new List<Control>();
                        FindControlRecursive(this.Page, typeof(ListBox), ref controls);
                        foreach (Control list_box_control in controls)
                        {
                            ListBox list_box = (ListBox)list_box_control;

                            if (list_box.Attributes["columnId"] == null)
                            {
                                continue;
                            }
                            Boolean is_right_box;
                            if (list_box.Attributes["side"] == null)
                            {
                                is_right_box = false;
                            }
                            else
                            {
                                if (list_box.Attributes["side"].ToString().Equals("right"))
                                {
                                    is_right_box = true;
                                }
                                else
                                {
                                    is_right_box = false;
                                }
                            }

                            if ((list_box.Attributes["columnId"].ToString() == parent_field_id) && is_right_box)
                            {
                                foreach (ListItem it in list_box.Items)
                                {
                                    selected_items = selected_items + it.Value + ":";
                                }
                            }
                        }
                    }
                    else
                    {
                        List<Control> controls = new List<Control>();
                        FindControlRecursive(this.Page, typeof(DropDownList), ref controls);
                        foreach (Control dropdownlist_control in controls)
                        {
                            DropDownList ddl = (DropDownList)dropdownlist_control;

                            if (ddl.Attributes["columnId"] == null)
                            {
                                continue;
                            }
                            if (ddl.Attributes["columnId"].ToString() == parent_field_id)
                            {
                                selected_items = ddl.SelectedItem.Value;
                            }
                        }
                    }
                }
            }
        }

        public bool has_child(string field_id)
        {
            SPList list = SPContext.Current.List;
            SPFieldCollection fields = list.Fields;
            foreach (SPField field in fields)
            {
                if (field_id == field.GetProperty(CustomDropDownList.PARENT_COLUMN))
                {
                    return true;
                }
            }
            return false;
        }

        public List<ListItem> GetCurrentLinkedFieldValue()
        {
            List<ListItem> returnListITems = new List<System.Web.UI.WebControls.ListItem>();
            CustomDropDownList field = base.Field as CustomDropDownList;
            string linked_column = field.GetProperty(CustomDropDownList.LINK_COLUMN);
            bool has_link = Convert.ToString(field.GetCustomProperty(CustomDropDownList.LINK)) == Boolean.TrueString;
            if (has_link)
            {
                //---------------------------
                //Iterate Controls
                List<Control> Collect = new List<Control>();
                FindControlRecursive(this.Page, typeof(CustomLookupField.CustomDropDownListControl), ref Collect);

                if (Collect.Count > 0)
                {
                    CustomDropDownList field11 = base.Field as CustomDropDownList;
                    string str = field11.GetProperty(CustomDropDownList.PARENT_COLUMN);

                    if (!string.IsNullOrEmpty(str))
                    {
                        SPField fldPArent = SPContext.Current.List.Fields[new Guid(field11.GetProperty(CustomDropDownList.PARENT_COLUMN))];
                        if (fldPArent.FieldRenderingControl.ControlMode == SPControlMode.Edit && fldPArent != null && fldPArent.FieldRenderingControl != null)
                        {
                            foreach (Control ctrl in Collect)
                            {
                                CustomDropDownListControl LookupFieldControl = ctrl as CustomDropDownListControl;
                                if (LookupFieldControl != null && LookupFieldControl.FieldName == fldPArent.FieldRenderingControl.FieldName)
                                {
                                    SPFieldLookupValue val = LookupFieldControl.Value as SPFieldLookupValue;

                                    if (val != null)
                                    {
                                        Helper.get_matched_items(field, val.LookupId.ToString(), linked_column, ref returnListITems);
                                    }
                                    else
                                    {
                                        SPFieldLookupValueCollection valColl = LookupFieldControl.Value as SPFieldLookupValueCollection;

                                        if (valColl != null && valColl.Count > 0)
                                        {
                                            foreach (SPFieldLookupValue val1 in valColl)
                                            {
                                                if (val1 != null)
                                                {
                                                    Helper.get_matched_items(field, val1.LookupId.ToString(), linked_column, ref returnListITems);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            //--------------------------------------------------------------------
            //List<ListItem> returnListITems = new List<System.Web.UI.WebControls.ListItem>();
            //CustomDropDownList field = base.Field as CustomDropDownList;
            //string linked_column = field.GetProperty(CustomDropDownList.LINK_COLUMN);
            //bool has_link = Convert.ToString(field.GetCustomProperty(CustomDropDownList.LINK)) == Boolean.TrueString;
            //if (has_link)
            //{
            //    string str = field.GetProperty(CustomDropDownList.PARENT_COLUMN);
            //    if (!string.IsNullOrEmpty(str))
            //    {
            //        SPField fldPArent = SPContext.Current.List.Fields[new Guid(field.GetProperty(CustomDropDownList.PARENT_COLUMN))];

            //        SPFieldLookupValue val = fldPArent.FieldRenderingControl.ItemFieldValue as SPFieldLookupValue;

            //        if (val != null)
            //        {
            //            Helper.get_matched_items(field, val.LookupId.ToString(), linked_column, ref returnListITems);
            //        }
            //        else
            //        {
            //            SPFieldLookupValueCollection valColl = fldPArent.FieldRenderingControl.ItemFieldValue as SPFieldLookupValueCollection;

            //            if (valColl != null && valColl.Count > 0)
            //            {
            //                foreach (SPFieldLookupValue val1 in valColl)
            //                {
            //                    if (val1 != null)
            //                    {
            //                        Helper.get_matched_items(field, val1.LookupId.ToString(), linked_column, ref returnListITems);
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}
            return returnListITems;
        }

        Boolean ParentValueNullOREmpty()
        {
            CustomDropDownList field = base.Field as CustomDropDownList;
            string linked_column = field.GetProperty(CustomDropDownList.LINK_COLUMN);
            bool has_link = Convert.ToString(field.GetCustomProperty(CustomDropDownList.LINK)) == Boolean.TrueString;
            if (has_link)
            {
                string str = field.GetProperty(CustomDropDownList.PARENT_COLUMN);
                if (!string.IsNullOrEmpty(str))
                {
                    SPField fldPArent = SPContext.Current.List.Fields[new Guid(field.GetProperty(CustomDropDownList.PARENT_COLUMN))];

                    SPFieldLookupValue val = fldPArent.FieldRenderingControl.ItemFieldValue as SPFieldLookupValue;

                    if (val != null)
                    {
                        return false;
                    }
                    else
                    {
                        SPFieldLookupValueCollection valColl = fldPArent.FieldRenderingControl.ItemFieldValue as SPFieldLookupValueCollection;
                        if (valColl != null && valColl.Count > 0)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public void UpdateChildLinkedControl(SPField CurrentField, Object CurrentControlValue, ref List<Control> AllPossibleControls)
        {
            Object childCtrlValue = null;
            foreach (Control ctrl in AllPossibleControls)
            {
                CustomDropDownListControl ChildControl = ctrl as CustomDropDownListControl;
                if (ChildControl != null)
                {
                    if (CurrentField.Id.ToString() == ChildControl.Field.GetProperty(CustomDropDownList.PARENT_COLUMN))
                    {
                        CustomDropDownList field = ChildControl.Field as CustomDropDownList;
                        if (field != null && (Convert.ToString(field.GetCustomProperty(CustomDropDownList.LINK)) == Boolean.TrueString))
                        {
                            string linked_column = field.GetProperty(CustomDropDownList.LINK_COLUMN);
                            List<ListItem> poplateItemsList = new List<ListItem>();
                            if (CurrentControlValue != null && CurrentControlValue.ToString() != "")
                            {
                                if (CurrentControlValue is string)
                                {
                                    Helper.FetchMatchedValuesFromList(field, CurrentControlValue.ToString(), ref poplateItemsList);
                                }
                                else if (CurrentControlValue is List<string>)                                
                                {
                                    List<string> listItems = CurrentControlValue as List<string>;
                                    if (listItems.Count > 0)
                                    {
                                        foreach (string val in CurrentControlValue as List<string>)
                                        {
                                            Helper.FetchMatchedValuesFromList(field, val, ref poplateItemsList);
                                        }
                                    }
                                    else if (Convert.ToBoolean(field.GetCustomProperty(CustomDropDownList.SHOW_ALL_VALUES)))
                                    {
                                        Helper.FetchAllValueFromLinkedList(field, ref poplateItemsList);
                                    }
                                }
                            }
                            else if (Convert.ToBoolean(field.GetCustomProperty(CustomDropDownList.SHOW_ALL_VALUES)))
                            {
                                Helper.FetchAllValueFromLinkedList(field, ref poplateItemsList);
                            }

                            //Set Child control Values
                            List<Control> ChildControls = new List<Control>();
                            if (field.AllowMultipleValues)
                            {
                                FindControlRecursive(ctrl, typeof(ListBox), ref ChildControls);
                                if (ChildControls != null)
                                {
                                    ListBox rightListBox = null;
                                    ListBox leftListBox = null;

                                    foreach (ListBox childListBox in ChildControls)
                                    {
                                        if (childListBox.Attributes["side"] != null && childListBox.Attributes["side"].ToString().Equals("right"))
                                        {
                                            rightListBox = childListBox;
                                        }
                                        else
                                        {
                                            leftListBox = childListBox;
                                        }
                                    }

                                    if (poplateItemsList != null && poplateItemsList.Count > 0)
                                    {
                                        //childListBox.Items.AddRange(poplateItemsList.ToArray());
                                        if (rightListBox.Items != null && rightListBox.Items.Count > 0)
                                        {
                                            List<string> vals = new List<string>();
                                            for (int i = rightListBox.Items.Count - 1; i >= 0; i--)
                                            {
                                                if (!CheckListItemExistandRemove(rightListBox.Items[i], ref poplateItemsList))
                                                {
                                                    rightListBox.Items.RemoveAt(i);
                                                }
                                                else
                                                {
                                                    vals.Add(rightListBox.Items[i].Value);
                                                }
                                            }

                                            if (vals.Count > 0) childCtrlValue = vals;
                                        }

                                        for (int i = leftListBox.Items.Count - 1; i >= 0; i--)
                                        {
                                            if (!CheckListItemExistandRemove(leftListBox.Items[i], ref poplateItemsList))
                                            {
                                                leftListBox.Items.RemoveAt(i);
                                            }
                                        }

                                        leftListBox.Items.AddRange(poplateItemsList.ToArray());
                                    }
                                    else
                                    {
                                        rightListBox.Items.Clear();
                                        leftListBox.Items.Clear();
                                    }

                                }
                            }
                            else
                            {
                                FindControlRecursive(ctrl, typeof(DropDownList), ref ChildControls);
                                if (ChildControls != null)
                                {
                                    foreach (DropDownList childListBox in ChildControls)
                                    {
                                        childListBox.Items.Clear();
                                        if (this.ControlMode == SPControlMode.New || !Field.Required) childListBox.Items.Insert(0, new ListItem("(None)", "0"));
                                        if(poplateItemsList != null && poplateItemsList.Count > 0)
                                            childListBox.Items.AddRange(poplateItemsList.ToArray());
                                        childListBox.SelectedIndex = 0;
                                    }
                                }
                            }

                            //Reset nested child controls Value
                            UpdateChildLinkedControl(field, childCtrlValue, ref AllPossibleControls);
                        }
                    }                
                }
            }
        }

        bool CheckListItemExistandRemove(ListItem li, ref List<ListItem> Items)
        {
            if (Items != null && Items.Count > 0)
            {
                for (int i = Items.Count - 1; i >= 0; i--)
                {
                    if (Items[i].Value == li.Value)
                    {
                        Items.RemoveAt(i);
                        return true;
                    }
                }
            }

            return false;
        }

        string GetFieldValue(SPField field)
        {
            string val = "";
            if (field != null)
            {
                 val = Convert.ToString(field.GetCustomProperty("Items"));
                if (string.IsNullOrEmpty(val))
                {
                    SPFieldLookupValue spVal = field.FieldRenderingControl.ItemFieldValue as SPFieldLookupValue;

                    if (spVal != null)
                    {
                        return spVal.LookupId.ToString();
                    }
                    else
                    {
                        SPFieldLookupValueCollection valColl = field.FieldRenderingControl.ItemFieldValue as SPFieldLookupValueCollection;
                        if (valColl != null && valColl.Count > 0)
                        {
                            foreach (SPFieldLookupValue v in valColl)
                            {
                                val += v.LookupId.ToString() + ":";
                            }
                        }
                    }
                }
            }
            return val;
        }

    }
}