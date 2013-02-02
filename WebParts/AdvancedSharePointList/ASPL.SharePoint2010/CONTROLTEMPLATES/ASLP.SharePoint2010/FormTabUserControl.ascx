<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls"
    Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages"
    Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FormTabUserControl.ascx.cs"
    Inherits="ASPL.SharePoint2010.CONTROLTEMPLATES.FormTabUserControl" %>
<div style="margin-left: 40px">
    <asp:Literal runat="server" ID="litScripts" />
</div>
<style type="text/css">
    .iw-SectionTitle
    {
        margin: 1px;
        vertical-align: top;
        color: #525252;
        background-color: transparent;
        font-family: Tahoma;
        font-weight: 500;
        font-size: 8pt;
    }
    span.iw-SectionTitle input, span.iw-SectionTitle label
    {
        vertical-align: middle;
    }
    
    .iw-DropDown, .iw-Results, td.iw-Results
    {
        margin: 1px;
        vertical-align: top;
        background-color: transparent;
        font-family: Tahoma;
        font-size: 8pt;
    }
</style>

<script language="javascript" type="text/javascript">

    function GetFirstChild(parentNode) {

        for (var i = 0; i < parentNode.childNodes.length; i++)
            if (parentNode.childNodes[i].nodeType == 1)
                return parentNode.childNodes[i];
    }

    function GetParentByTagName(el, tagName) {
        if (el.nodeType == 1 && el.tagName.toLowerCase() == tagName.toLowerCase())
            return el;
        if (el.parentNode == null)
            return null;
        return GetParentByTagName(el.parentNode, tagName);
    }
    function iwslp_addColumns() {
        var lstAllcolumns = document.getElementById('<%=lstAllFields.ClientID %>');
        var divSelected = document.getElementById('<%=divSelectedColumns.ClientID %>');

        for (var i = 0; i < lstAllcolumns.options.length; i++) {
            if (lstAllcolumns.options[i].selected)
                iwslp_addOption(divSelected, lstAllcolumns.options[i]);
        }
    }


    function showSelectedOPtion() {
        var txtSelectedColumnDisplayName = document.getElementById('<%=hdnSelectedColumnDisplayName.ClientID %>');
        var selectedColumnDisplayNameArray = txtSelectedColumnDisplayName.value.split("]");

        var txtSelectedColumn = document.getElementById('<%=hdnSelectedColumn.ClientID %>');
        var selectedColumnArray = txtSelectedColumn.value.split("]");

        var divSelected = document.getElementById('<%=divSelectedColumns.ClientID %>');

        for (var i = 0; i < selectedColumnDisplayNameArray.length; i++) {
            if (selectedColumnDisplayNameArray[i] && selectedColumnArray[i])
                iwslp_addOptionATgridRowSelection(divSelected, txtSelectedColumn, selectedColumnArray[i], selectedColumnDisplayNameArray[i], i);
        }
    }

    // Add fields to selected tab in grid row selection

    function iwslp_addOptionATgridRowSelection(divSelected, txt, fieldIName,filedDisName, fieldCount, separator) {

        var countValues = fieldCount + 1;

        for (var i = 0; i < divSelected.childNodes.length; i++) {
            var curSelect = GetFirstChild(GetFirstChild(GetFirstChild(GetFirstChild(GetFirstChild(divSelected.childNodes[i])))));
            var newOp = new Option(countValues);
            newOp.value = countValues;
            curSelect.options.add(newOp);
        }

        var newItem = document.createElement("div");
        newItem.style.position = "relative";
        var tbl = document.createElement("table");

        tbl.cellPadding = "1";
        tbl.cellSpacing = "0";
        tbl.style.backgroundColor = "#FAFAFA";
        tbl.style.margin = "1px";
        tbl.style.borderWidth = "1px";
        tbl.style.borderColor = "#CCC";
        tbl.style.borderStyle = "solid";
        tbl.style.width = "220px";
        tbl.title = fieldIName;

        var tbody = document.createElement("tbody");
        var row = document.createElement("tr");
        var sortCell = document.createElement("td");
        sortCell.style.width = "40px";

        row.appendChild(sortCell);

        var sortSelect = document.createElement("select");
        sortCell.appendChild(sortSelect);


        for (var i = 1; i <= countValues; i++) {
            var newOp = new Option(i);
            newOp.value = i;
            sortSelect.options.add(newOp);
        }

        sortSelect.value = countValues;
        var sortHandler = function () {
            iwslp_moveColumn(txt, fieldIName, sortSelect);
        }

        sortSelect.onchange = sortHandler;

        var valueCell = document.createElement("td");
        valueCell.className = "ms-vb";
        valueCell.style.verticalAlign = "middle";
        valueCell.vAlign = "middle";
        var nodeTxt = filedDisName;
        if (nodeTxt.length > 25)
            nodeTxt = nodeTxt.substring(0, 22) + "...";
        valueCell.appendChild(document.createTextNode(nodeTxt));
        valueCell.title = filedDisName;
        row.appendChild(valueCell);

        var deleteCell = document.createElement("td");
        deleteCell.style.width = "18px";
        row.appendChild(deleteCell);

        var deleteImg = document.createElement("img");
        deleteImg.src = "/_layouts/images/delete.gif";
        var handler = function () {
            iwslp_deleteColumn(txt, fieldIName, deleteImg);
        }
        deleteImg.onclick = handler;
        deleteImg.style.cursor = "pointer";
        deleteCell.appendChild(deleteImg);

        tbody.appendChild(row);
        tbl.appendChild(tbody);
        newItem.appendChild(tbl);
        divSelected.appendChild(newItem);
    }

    function iwslp_addOption(divSelected, opt) {

        var txt = document.getElementById('<%=hdnSelectedColumn.ClientID %>');
        if (iwslp_columnInList(txt, opt.value))
            return;

        var countValues = iwslp_countColumnInList(txt) + 1;

        if (typeof (iwslp_islite) != "undefined" && iwslp_litecount <= countValues) {
            document.getElementById('<%=sslError.ClientID %>').style.display = "block";
            return;
        }

        for (var i = 0; i < divSelected.childNodes.length; i++) {
            var curSelect = GetFirstChild(GetFirstChild(GetFirstChild(GetFirstChild(GetFirstChild(divSelected.childNodes[i])))));
            var newOp = new Option(countValues);
            newOp.value = countValues;
            curSelect.options.add(newOp);
        }

        var newItem = document.createElement("div");
        newItem.style.position = "relative";
        var tbl = document.createElement("table");

        tbl.cellPadding = "1";
        tbl.cellSpacing = "0";
        tbl.style.backgroundColor = "#FAFAFA";
        tbl.style.margin = "1px";
        tbl.style.borderWidth = "1px";
        tbl.style.borderColor = "#CCC";
        tbl.style.borderStyle = "solid";
        tbl.style.width = "220px";
        tbl.title = opt.value;

        var tbody = document.createElement("tbody");
        var row = document.createElement("tr");
        var sortCell = document.createElement("td");
        sortCell.style.width = "40px";

        row.appendChild(sortCell);

        var sortSelect = document.createElement("select");
        sortCell.appendChild(sortSelect);


        for (var i = 1; i <= countValues; i++) {
            var newOp = new Option(i);
            newOp.value = i;
            sortSelect.options.add(newOp);
        }

        sortSelect.value = countValues;
        var sortHandler = function () {
            iwslp_moveColumn(txt, opt.value, sortSelect);
        }

        sortSelect.onchange = sortHandler;

        var valueCell = document.createElement("td");
        valueCell.className = "ms-vb";
        valueCell.style.verticalAlign = "middle";
        valueCell.vAlign = "middle";
        var nodeTxt = opt.text;
        if (nodeTxt.length > 25)
            nodeTxt = nodeTxt.substring(0, 22) + "...";
        valueCell.appendChild(document.createTextNode(nodeTxt));
        valueCell.title = opt.text;
        row.appendChild(valueCell);

        var deleteCell = document.createElement("td");
        deleteCell.style.width = "18px";
        row.appendChild(deleteCell);

        var deleteImg = document.createElement("img");
        deleteImg.src = "/_layouts/images/delete.gif";
        var handler = function () {
            iwslp_deleteColumn(txt, opt.value, deleteImg);
        }
        deleteImg.onclick = handler;
        deleteImg.style.cursor = "pointer";
        deleteCell.appendChild(deleteImg);

        tbody.appendChild(row);
        tbl.appendChild(tbody);
        newItem.appendChild(tbl);
        divSelected.appendChild(newItem);
        txt.value += opt.value + "]";
    }

    function iwslp_moveColumn(txt, val, select) {
        var div = GetParentByTagName(select, "div");
        var container = div.parentNode;

        var newIndex = parseInt(select.value) - 1;
        var deleteIndex = -1;
        for (var i = 0; i < container.childNodes.length; i++) {
            if (container.childNodes[i] == div) {
                deleteIndex = i;
            }
        }

        if (deleteIndex == newIndex)
            return;

        var afterOld = false;
        var afterNew = false;
        for (var i = 0; i < container.childNodes.length; i++) {
            if (i != deleteIndex) {
                if ((i > newIndex && newIndex > deleteIndex) ||
                 (i >= newIndex && newIndex < deleteIndex)) {
                    afterNew = true;
                }

                afterOld = i >= deleteIndex && deleteIndex >= 0;

                if (!(afterNew && afterOld) && (afterNew || afterOld)) {
                    var sortSelect = GetFirstChild(GetFirstChild(GetFirstChild(GetFirstChild(GetFirstChild(container.childNodes[i])))));

                    if (afterOld) {
                        sortSelect.value = parseInt(sortSelect.value) - 1;
                    }
                    if (afterNew) {
                        sortSelect.value = parseInt(sortSelect.value) + 1;
                    }
                }
            }
        }

        container.removeChild(div);
        if (newIndex == select.options.length - 1)
            container.appendChild(div);
        else {
            var refDiv = container.childNodes[newIndex];
            container.insertBefore(div, refDiv);
        }

        var items = txt.value.split("]");
        items.splice(deleteIndex, 1);
        items.splice(newIndex, 0, val);
        txt.value = items.join("]");
    }
    function iwslp_deleteColumn(txt, val, img) {
        var div = GetParentByTagName(img, "div");

        var deleteIndex;
        var container = div.parentNode;
        var afterDeleted = false;
        for (var i = 0; i < container.childNodes.length; i++) {
            if (container.childNodes[i] == div) {
                deleteIndex = i;
                afterDeleted = true;
            }
            else {
                var sortSelect = GetFirstChild(GetFirstChild(GetFirstChild(GetFirstChild(GetFirstChild(container.childNodes[i])))));
                if (afterDeleted) {
                    sortSelect.value = parseInt(sortSelect.value) - 1;
                }

                sortSelect.options.remove(sortSelect.options.length - 1);
            }
        }
        container.removeChild(div);

        var items = txt.value.split("]");
        items.splice(deleteIndex, 1);
        txt.value = items.join("]");
    }

    function iwslp_countColumnInList(txt) {
        var values = txt.value.split("]");
        return values.length - 1;
    }

    function iwslp_columnInList(txt, val) {
        return txt.value.indexOf(val + "]") >= 0;
    }


    var i = 0;
    var j = 0;

    function IWDeleteConfirm() {

    }

    function ClickAction(buttonID, rowID, hidControlRowID) {
        document.getElementById(hidControlRowID).value = rowID;
        document.getElementById(buttonID).click();
    }

    function ClickDelete(buttonID, rowID, hidDeleteRowID) {
        document.getElementById(hidDeleteRowID).value = rowID;
        document.getElementById(buttonID).click();
    }

    //sets optional section state
    function ToggleSection(imgLink, hdnID, value) {
        var img = GetFirstChild(imgLink);
        var tr = GetParentByTagName(imgLink, "tr");

        if (tr.style.display == "none")//do not show sections with hidden header
            return;

        var section = GetSiblingByTagName(tr.nextSibling, "tr");
        var hdnEl = document.getElementById(hdnID);

        var hide = value;
        if (hide == null)
            hide = section.style.display == "";
        if (hide) {

            section.style.display = "none";
            img.src = "/_layouts/images/plus.gif";
            hdnEl.value = "hidden";
        }
        else {
            section.style.display = "";
            img.src = "/_layouts/images/minus.gif";
            hdnEl.value = "visible";
        }
    }

    function GetFirstChild(parentNode) {
        for (var i = 0; i < parentNode.childNodes.length; i++)
            if (parentNode.childNodes[i].nodeType == 1)
                return parentNode.childNodes[i];
    }

    function GetSiblingByTagName(el, tagName) {
        if (el.nodeType == 1 && el.tagName.toLowerCase() == tagName.toLowerCase())
            return el;

        if (el.nextSibling == null)
            return null;

        return GetSiblingByTagName(el.nextSibling, tagName);
    }

    function GetParentByTagName(el, tagName) {
        if (el.nodeType == 1 && el.tagName.toLowerCase() == tagName.toLowerCase())
            return el;
        if (el.parentNode == null)
            return null;
        return GetParentByTagName(el.parentNode, tagName);
    }

    function SetToggleSection(linkID, hdnID) {
        var hdnEl = document.getElementById(hdnID);
        var hide = null;
        if (hdnEl.value != "")
            hide = hdnEl.value != "visible";
        if (hide != null) {
            var imgLink = document.getElementById(linkID);
            ToggleSection(imgLink, hdnID, hide);
        }
    }


    function SetAddTabClickable() {
        var txt = document.getElementById("<%=txtNewTab.ClientID%>");
        var link = document.getElementById("<%=cmdAddTab.ClientID%>");

        if (link == null)
            return;

        if (txt.value == "") {
            link.disabled = "disabled";
            link.onclick = function () { return false; };
        }
        else {
            link.disabled = false;
            link.onclick = "";
        }
    }

</script>

<div id="sslError" runat="server" style="display: none; border: #ffdf88 1px solid;
    background-color: #fff9de; padding: 8px; width: 100%">
</div>
<asp:TextBox Style="display: none" runat="server" ID="hidPermToggle" />
<asp:TextBox Style="display: none" runat="server" ID="hidGenToggle" />
<asp:TextBox Style="display: none" runat="server" ID="hidAssToggle" />
<asp:TextBox Style="display: none" runat="server" ID="hidNoPermToggle" />
<asp:HiddenField runat="server" ID="hidSAPRuleID" />
<asp:HiddenField runat="server" ID="hidCurrentFieldRules" />
<asp:HiddenField runat="server" ID="hidEditedRule" Value="-1" />
<asp:HiddenField runat="server" ID="hdnSelectedColumn" Value="" />
<asp:HiddenField runat="server" ID="hdnSelectedColumnDisplayName" Value="" />
<%-- <IW:LiteWarning runat="server" />--%>
<table style="width: 600px">
    <tr>
        <td class="ms-linksectionheader" style="padding-right: 4px; padding-left: 4px; padding-bottom: 4px;
            padding-top: 4px; white-space: nowrap" nowrap="nowrap" width="100%" colspan="2">
            <h3 class="ms-standardheader">
                <asp:Literal ID="Literal1123" Text="Tab Names" runat="server" />
            </h3>
        </td>
    </tr>
    <tr>
        <td colspan="2">
            <table width="100%" cellspacing="0" cellpadding="0">
                <tr>
                    <td>
                        <table width="100%">
                            <tr>
                                <td>
                                    <asp:GridView ID="grdTabView" runat="server" AutoGenerateColumns="False" CellPadding="4"
                                        ShowHeader="true" EnableModelValidation="True" ForeColor="#333333" GridLines="None"
                                        OnRowCommand="grdTabView_RowCommand" Width="100%" EmptyDataText="No Tabs are created yet! ">
                                        <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                                        <Columns>
                                            <asp:BoundField DataField="RowId" HeaderText="RowID" />
                                            <asp:TemplateField ControlStyle-Width="30%" HeaderText="Tab Name (click to edit)">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="lnkTab" Text='<%#Eval("Title")%>' runat="server" CommandArgument='<%#Eval("RowID")%>'
                                                        CommandName="EditTab" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="Description" HeaderText="Description" ControlStyle-Width="20%" />
                                            <asp:BoundField DataField="IsDefault" HeaderText="Is Default" ControlStyle-Width="10%" />
                                            <asp:BoundField DataField="HasPermission" HeaderText="Has Permissions" ControlStyle-Width="10%" />
                                            <asp:TemplateField>
                                                <ItemTemplate>
                                                    <asp:ImageButton ID="lnkDeleteTab" runat="server" ImageUrl="~/_layouts/ASPL.SharePoint2010/Resource/Delete.bmp"
                                                        CommandArgument='<%#Eval("RowID")%>' CommandName="DeleteTab" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField>
                                                <ItemTemplate>
                                                    <asp:ImageButton ID="lnkEditTab" runat="server" ImageUrl="~/_layouts/ASPL.SharePoint2010/Resource/Select16.bmp"
                                                        CommandArgument='<%#Eval("RowID")%>' CommandName="EditTab" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                        <EditRowStyle BackColor="#999999" />
                                        <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                                        <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                                        <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                                        <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                                        <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                                    </asp:GridView>
                                </td>
                                <td style="padding: 6px">
                                    <asp:Button ID="btnRowUP" runat="server" Text="˄" OnClick="btnRowUP_Click" />
                                    <br />
                                    <asp:Button ID="btnRowDown" runat="server" Text="˅" OnClick="btnRowDown_Click" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td style="padding-top: 6px">
                        <table cellspacing="2" cellpadding="0">
                            <tr>
                                <td class="ms-vb">
                                    <asp:Literal ID="ltrNewTab" Text="Name" runat="server" />:
                                </td>
                                <td class="ms-vb" style="width: 172px">
                                    <asp:TextBox runat="server" ID="txtNewTab" Width="160px" onkeydown="SetAddTabClickable();"
                                        onblur="SetAddTabClickable();" ValidationGroup="tabName"></asp:TextBox><asp:RequiredFieldValidator
                                            ControlToValidate="txtNewTab" runat="server" ID="rfvTabName" ValidationGroup="tabName"
                                            ErrorMessage="*" CssClass="ms-error" Display="Static" />
                                </td>
                                <td class="ms-vb" style="width: 80px">
                                    &nbsp;
                                </td>
                                <td style="width: 180px">
                                    &nbsp;
                                    <asp:CheckBox CssClass="iw-SectionTitle" ID="chkSetTabDefault" runat="server" Text="Is Default"
                                        Style="vertical-align: middle" />
                                </td>
                            </tr>
                            <tr>
                                <td valign="top" class="ms-vb">
                                    <asp:Literal ID="Literal6" Text="Description" runat="server" />
                                </td>
                                <td colspan="3" class="ms-vb">
                                    <asp:TextBox TextMode="MultiLine" runat="server" ID="txtDescription" Rows="4" Width="400px" />
                                </td>
                                <td class="ms-vb" style="vertical-align: bottom" id="tdAddTabButton">
                                    <asp:Button ID="cmdAddTab" runat="server" Text="Create" ValidationGroup="tabName"
                                        CssClass="ms-ButtonHeightWidth" OnClick="cmdAddTab_Click" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td colspan="2">
            <span class="ms-error">
                <asp:Literal runat="server" ID="lblError" EnableViewState="false"></asp:Literal></span>
        </td>
    </tr>
    <tr>
        <td class="ms-linksectionheader" style="padding-right: 4px; padding-left: 4px; padding-bottom: 4px;
            padding-top: 4px; white-space: nowrap" nowrap="nowrap" width="100%" colspan="2">
            <h3 class="ms-standardheader">
                <asp:Literal ID="Literal2" Text="Tab Fields" runat="server" />
            </h3>
        </td>
    </tr>
    <tr>
        <td class="iw-SectionTitle" colspan="2">
            <%--   <iw:optionpicker ID="iwOptionPicker" runat="server" />--%>            <%--<span style="display: none">
                <asp:TextBox runat="server" ID="txtSelectedValues" /></span>--%>
            <div id="Div1" runat="server" style="display: none; border: #ffdf88 1px solid; background-color: #fff9de;
                padding: 8px; width: 100%">
            </div>
            <table cellspacing="0" cellpadding="2" border="0">
                <tr>
                    <td class="iw-SectionTitle" scope="col" nowrap="nowrap">
                        <asp:Literal ID="Literal3" Text="Fields to select" runat="server" />
                    </td>
                    <td>
                    </td>
                    <td class="iw-SectionTitle" scope="col" nowrap="nowrap">
                        <asp:Literal ID="Literal4" Text="Selected Fields" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td valign="top" style="width: 280px">
                        <asp:ListBox runat="server" ID="lstAllFields" Width="280px" Height="255px" CssClass="iw-DropDown"
                            SelectionMode="Multiple" ondblclick="iwslp_addColumns();" Style="border-width: 1px;
                            border-style: solid; border-color: #999"></asp:ListBox>
                    </td>
                    <td valign="middle" align="center" style="width: 40px">
                        <asp:ImageButton runat="server" ID="cmdAddField" ImageUrl="/_layouts/ASPL.SharePoint2010/Resource/ico_arrow.gif" OnClientClick="iwslp_addColumns();return false;" />
                    </td>
                    <td valign="top" style="width: 250px">
                        <div style="position: relative; height: 255px; width: 250px; border: 1px solid #999;
                            overflow: auto; padding: 1px; top: 0px; left: 0px;">
                            <div style="position: absolute" runat="server" id="divSelectedColumns">
                            </div>
                        </div>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <!-- start permissions-->
    <tr id="trPermissionHeader" runat="server">
        <td class="ms-linksectionheader" style="padding-right: 4px; padding-left: 4px; padding-bottom: 4px;
            padding-top: 4px; width: 100%; white-space: nowrap;" nowrap="nowrap" width="100%"
            colspan="2">
            <h3 class="ms-standardheader">
                &nbsp;<asp:Literal ID="Literal8" Text="Tab Permissions" runat="server" />
            </h3>
        </td>
    </tr>
    <tr id="trPermissions">
        <td colspan="2">
            <table width="100%">
                <tr>
                    <td class="iw-SectionTitle" style="width: 160px">
                        <asp:Literal ID="Literal11" runat="server" Text="Permission level:" />:
                    </td>
                    <td>
                        <asp:RadioButtonList runat="server" ID="cdoPermissionLevel" RepeatDirection="Horizontal"
                            CssClass="iw-DropDown" ValidationGroup="TabPermission">
                            <asp:ListItem Value="11">Read</asp:ListItem>
                            <asp:ListItem Value="12">Write</asp:ListItem>
                            <asp:ListItem Value="13">Deny</asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:RequiredFieldValidator ID="cdoPermissionLevelRequiredFieldValidator" Display="Dynamic"
                            ControlToValidate="cdoPermissionLevel" runat="server" ErrorMessage="Please select the permission level"
                            CssClass="ms-formvalidation" ValidationGroup="TabPermission" SetFocusOnError="True" />
                    </td>
                </tr>
                <tr>
                    <td class="iw-SectionTitle" style="width: 160px">
                        <asp:Literal ID="Literal12" Text="Apply in forms:" runat="server" />:
                    </td>
                    <td>
                        <asp:CheckBoxList ValidationGroup="TabPermission" runat="server" ID="chkPages" RepeatDirection="Vertical"
                            CssClass="iw-DropDown">
                            <asp:ListItem Value="21">New item</asp:ListItem>
                            <asp:ListItem Value="22">Edit item</asp:ListItem>
                            <asp:ListItem Value="23">View item</asp:ListItem>
                        </asp:CheckBoxList>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">

                    </td>
                </tr>
                <tr>
                    <td class="iw-SectionTitle" style="width: 160px">
                        <asp:Literal ID="Literal13" Text="For User\groups:" runat="server" />:
                    </td>
                    <td>
                        <table>
                            <tr>
                                <td valign="top">
                                    <asp:DropDownList ID="cboSPPrinciplesOperator" runat="server">
                                        <asp:ListItem Text="In" Value="101" />
                                        <asp:ListItem Text="Not in" Value="102" />
                                        <asp:ListItem Text="Equal" Value="103" />
                                        <asp:ListItem Text="Not Equal" Value="104" />
                                        <asp:ListItem Text="None" Value="0" />
                                    </asp:DropDownList>
                                </td>
                                <td valign="top">
                                    <SharePoint:PeopleEditor runat="server" ID="peSelectUsers" MultiSelect="true" MaximumEntities="1"
                                        SelectionSet="User,SPGroup,SecGroup" AllowEmpty="true" Height="20px" Width="300px"
                                        AllowTypeIn="true" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td class="ms-linksectionheader" style="padding-right: 4px; padding-left: 4px; padding-bottom: 1px;
                        padding-top: 2px" nowrap width="100%" colspan="2">
                        <h3 class="ms-standardheader">
                            <asp:Literal ID="Literal5" Text="Conditions" runat="server" />
                        </h3>
                    </td>
                </tr>
                <tr>
                    <td id="tdCondition" runat="server" colspan="2">
                        <%--<asp:PlaceHolder ID="phConditions" runat="server" />--%>
                        <asp:GridView ID="gvCondition" runat="server" AutoGenerateColumns="False" CellPadding="4"
                            EnableModelValidation="True" ForeColor="#333333" GridLines="None" 
                            onrowcommand="gvCondition_RowCommand">
                            <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                            <Columns>
                                <asp:BoundField DataField="RowId" HeaderText="RowID" Visible="False" />
                                <asp:BoundField DataField="TabRowID" HeaderText="PermissionRowID" Visible="False" />
                                <asp:BoundField DataField="SPFieldDisplayName" HeaderText="Field Name" />
                                <asp:BoundField DataField="SPFieldOperatorName" HeaderText="Operator" />
                                <asp:BoundField DataField="SPFieldOperatorID" HeaderText="SPFieldOperatorID" Visible="False" />
                                <asp:BoundField DataField="Value" HeaderText="Value" />
                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <asp:ImageButton ID="lnkDeleteCondition" runat="server" ImageUrl="~/_layouts/ASPL.SharePoint2010/Resource/Delete.bmp"
                                            CommandArgument='<%#Eval("RowID")%>' CommandName="DeleteCondition" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <asp:ImageButton ID="lnkEditCondition" runat="server" ImageUrl="~/_layouts/ASPL.SharePoint2010/Resource/Select16.bmp"
                                            CommandArgument='<%#Eval("RowID")%>' CommandName="EditCondition" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <EditRowStyle BackColor="#999999" />
                            <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                            <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                            <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                            <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                            <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                        </asp:GridView>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table>
                            <tr>
                                <td>
                                    <asp:DropDownList ID="cboAllFields" runat="server">
                                    </asp:DropDownList>
                                </td>
                                <td>
                                    <asp:DropDownList ID="cboConditionOperator" runat="server">
                                        <asp:ListItem Value="103">Equal</asp:ListItem>
                                        <asp:ListItem Value="104">Not Equal</asp:ListItem>
                                        <asp:ListItem Value="105">Contains</asp:ListItem>
                                        <asp:ListItem Value="106">Not Contains</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtValue" runat="server"></asp:TextBox>
                                </td>
                                <td>
                                    <asp:LinkButton ID="lnkAddCondition" runat="server" Text="Add" OnClick="lnkAddCondition_Click"
                                        Enabled="False" ValidationGroup="Condition"></asp:LinkButton>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="4">
                                    <asp:Label ID="lblErrorCondition" runat="server"></asp:Label>
                                    <asp:RequiredFieldValidator ID="txtValueRequiredFieldValidator" runat="server" 
                                        ControlToValidate="txtValue" ErrorMessage="Add condition Value" 
                                        ValidationGroup="Condition"></asp:RequiredFieldValidator>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:Button Width="100%" runat="server" ID="cmdCreateNewRule" Text="Add New Permission Rule"
                            ValidationGroup="TabPermission" OnClick="cmdCreateNewRule_Click" Enabled="False" />
                    </td>
                </tr>
                <tr>
                    <td class="ms-linksectionheader" style="padding-right: 4px; padding-left: 4px; padding-bottom: 4px;
                        padding-top: 4px" nowrap width="100%" colspan="2">
                        <h3 class="ms-standardheader">
                            <asp:Literal ID="Literal17" Text="Current Permission rules" runat="server" />
                        </h3>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:GridView ID="gvPermission" runat="server" AutoGenerateColumns="False" CellPadding="4"
                            ForeColor="#333333" GridLines="None" OnRowCommand="gvPermission_RowCommand">
                            <AlternatingRowStyle BackColor="White" ForeColor="#284775" Width="100%" />
                            <Columns>
                                <asp:BoundField DataField="RowId" HeaderText="RowID" Visible="False" />
                                <asp:BoundField DataField="TabRowID" HeaderText="TabRowID" Visible="False" />
                                <asp:BoundField DataField="PermissionID" HeaderText="PermissionID" Visible="false" />
                                <asp:BoundField DataField="PermissionName" HeaderText="Permission Name" ControlStyle-Width="20%" />
                                <asp:TemplateField ControlStyle-Width="20%" HeaderText="For User/Group">
                                    <ItemTemplate>
                                        <asp:Label ID="lblSPPrinciples" runat="server" Text='<%#Eval("SPPrinciplesOperatorName") + "" + Eval("SPPrinciples")%>'
                                            CommandName="DeletePermission" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="OnFormNames" HeaderText="On Forms" ControlStyle-Width="30%" />
                                <asp:BoundField DataField="OnFormIDs" Visible="false" HeaderText="OnFormIDs" />
                                <asp:BoundField DataField="IsDefault" Visible="false" HeaderText="Is Default" ControlStyle-Width="15%" />
                                <asp:BoundField DataField="HasCondition" HeaderText="Condition" ControlStyle-Width="15%" />
                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <asp:ImageButton ID="lnkDeletePermission" runat="server" ImageUrl="~/_layouts/ASPL.SharePoint2010/Resource/Delete.bmp"
                                            CommandArgument='<%#Eval("RowID")%>' CommandName="DeletePermission" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <asp:ImageButton ID="lnkEditPermission" runat="server" ImageUrl="~/_layouts/ASPL.SharePoint2010/Resource/Select16.bmp"
                                            CommandArgument='<%#Eval("RowID")%>' CommandName="EditPermission" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <EditRowStyle BackColor="#999999" />
                            <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                            <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                            <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                            <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                            <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                        </asp:GridView>
                        <asp:Literal runat="server" ID="Literal23"></asp:Literal>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" class="iw-SectionTitle">
                        <%--<asp:CheckBox ID="chkReversePermissions" runat="server" Text="" />--%>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <!-- end permissions -->
    <tr>
        <td colspan="2">
            <table width="100%">
                <tr>
                    <td id="tdIWOKCancelButtons">
                        <asp:Button ID="cmdOK" runat="server" Text="Save" CssClass="ms-ButtonHeightWidth"
                            CausesValidation="false" OnClick="cmdOK_Click" Enabled="False" />
                        <asp:Button ID="Cancel" runat="server" Text="Cancel" CssClass="ms-ButtonHeightWidth"
                            CausesValidation="false" OnClick="Cancel_Click" />
                        <asp:Button ID="btnHiddenSave" CausesValidation="false" Style="display: none" runat="server" />
                    </td>
                    <td id="tdIWDevBy" class="ms-propertysheet">
                        &nbsp;
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <%--  <asp:Button ID="btnRowUP" runat="server" Text="Up" />
    <asp:Button ID="btnRowDown" runat="server" Text="Down"  />--%></table>
<script language="javascript" type="text/javascript">
    //SetToggleSection("imgLinkGeneral", "<%=hidGenToggle.ClientID %>");
    //SetToggleSection("imgLink", "<%=hidPermToggle.ClientID %>");
    //SetToggleSection("imgLinkAss", "<%=hidAssToggle.ClientID %>");
</script>
<asp:HiddenField runat="server" ID="hidDeleteRow" Value="-1" />
<asp:Literal runat="server" ID="Literal1"></asp:Literal><asp:Literal runat="server"
    ID="litRequired"></asp:Literal><script type="text/javascript">
                                       SetAddTabClickable();
    </script>
