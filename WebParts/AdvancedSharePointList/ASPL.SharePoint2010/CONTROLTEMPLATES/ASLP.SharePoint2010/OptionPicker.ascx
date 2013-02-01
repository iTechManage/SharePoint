<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OptionPicker.ascx.cs" Inherits="AdvanceSharepointListPro.CONTROLTEMPLATES.OptionPicker" %>

 <asp:Label ID="lblError" runat="server" EnableViewState="false" CssClass="ms-error" />
 <table cellspacing="0" cellpadding="2" border="0">
    <tr>
       <td class="iw-SectionTitle" scope="col" nowrap="nowrap">
                          <%--  <asp:Literal ID="Literal1" Text="<%$Resources:CustomListResources,FieldsInList %>"
                                runat="server" />--%>
        </td>
        <td></td>
        <td class="iw-SectionTitle" scope="col" nowrap="nowrap">
                            <%--<asp:Literal ID="Literal2" Text="<%$Resources:CustomListResources,FieldsInTab %>"
                                runat="server" />--%>
        </td>
     </tr>
     <tr>
        <td valign="top" style="width:280px"><asp:ListBox runat="server" ID="lstAllFields" Width="280px" Height="255px" CssClass="iw-DropDown"
                                SelectionMode="Multiple" ondblclick="iwslp_addColumns();" style="border-width:1px;border-style:solid;border-color:#999"></asp:ListBox></td>
        <td valign="middle" align="center" style="width:40px"><asp:ImageButton runat="server" ID="cmdAddField" ImageUrl="" OnClientClick="iwslp_addColumns();return false;" /></td>
        <td valign="top" style="width:250px"><div style="position:relative;height:255px;width:250px;border:1px solid #999;overflow:auto;padding:1px;">
            <div style="position:absolute" runat="server" id="divSelectedColumns"></div>
        </div></td>
     </tr>
 </table>
 <span style="display:none"><asp:TextBox runat="server" ID="txtSelectedValues" /></span>
<asp:Literal runat="server" id="litScripts" />
<script language="javascript" type="text/javascript">

    function GetFirstChild(parentNode) {
        debugger;
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
    function iwslp_addOption(divSelected, opt) {
        var txt = document.getElementById('<%=txtSelectedValues.ClientID %>');
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
        txt.value += opt.value + ";";
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

        var items = txt.value.split(";");
        items.splice(deleteIndex, 1);
        items.splice(newIndex, 0, val);
        txt.value = items.join(";");
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

        var items = txt.value.split(";");
        items.splice(deleteIndex, 1);
        txt.value = items.join(";");
    }

    function iwslp_countColumnInList(txt) {
        var values = txt.value.split(";");
        return values.length - 1;
    }

    function iwslp_columnInList(txt, val) {
        return txt.value.indexOf(val + ";") >= 0;
    }
    function CheckRequiredFields() {
        var message = "";
        var txt = document.getElementById('<%=txtSelectedValues.ClientID %>');
        if (requiredFields.count > 0) {
            for (var i = 0; i < requiredFields.count; i++) {
                if (txt.value.indexOf(requiredFields[i].value + ";") < 0)
                    message += requiredFields[i].text + "\r\n";
            }

            if (message == "")
                return true;
            return confirm(messageStart + message + messageEnd);
        }
    }
</script>
 <div id="sslError" runat="server" style="display:none;border: #ffdf88 1px solid;background-color: #fff9de;padding:8px;width:100%"></div>
