<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls"
    Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages"
    Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FieldValidationSettings.ascx.cs"
    Inherits="ASPL.SharePoint2010.CONTROLTEMPLATES.FieldValidationSettings" %>
<script type="text/javascript" src="prototype.js"></script>
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
    .iw-DropDown
    {
        margin: 1px;
        vertical-align: top;
        text-align: justify;
        background-color: transparent;
        font-family: Tahoma;
        font-size: 8pt;
    }
    .iw-Results
    {
        vertical-align: top;
        text-align: inherit;
        background-color: transparent;
        font-family: Tahoma;
        font-size: 8pt;
        padding: 3px 8px 0px;
    }
    .iw-HighlightResult
    {
        background-color: #B0C4DE;
    }
</style>
<script type="text/javascript">
    var i = 0;
    var j = 0;



    function ClickAction(buttonID, rowID, hidControlRowID) {
        document.getElementById(hidControlRowID).value = rowID;
        document.getElementById(buttonID).click();
    }



    function FilterValidationOperators(ruleDDL, operatorsDDL, fieldDDL, patternDDL, txtValue, imgDeleteCustomExpressionClientId, imgSaveImageClientId, txtErrorMsgClientId) {
        var ruleControl = document.getElementById(ruleDDL);
        var conditionControl = document.getElementById(operatorsDDL);
        var fieldControl = document.getElementById(fieldDDL);
        var patternControl = document.getElementById(patternDDL);
        var selectedRuleValue = ruleControl.options[ruleControl.selectedIndex].value;
        var selectedFieldValue = fieldControl.options[fieldControl.selectedIndex].value;

        if (selectedRuleValue == "pattern") {
            patternControl.disabled = false;

            for (i = 0; i < fieldTypes.length; i++) {
                if (fieldTypes[i].name == "pattern") {

                    conditionControl.options.length = fieldTypes[i].operators.length
                    for (j = 0; j < fieldTypes[i].operators.length; j++) {
                        conditionControl.options[j].value = fieldTypes[i].operators[j].operation;
                        conditionControl.options[j].text = fieldTypes[i].operators[j].text;
                    }
                    break;
                }
            }
            CheckSelectedPattern(patternDDL, txtValue, imgDeleteCustomExpressionClientId, imgSaveImageClientId, txtErrorMsgClientId);
        }
        else if (selectedRuleValue == "length") {
            patternControl.disabled = true;

            for (i = 0; i < fieldTypes.length; i++) {
                if (fieldTypes[i].name == "length") {

                    conditionControl.options.length = fieldTypes[i].operators.length
                    for (j = 0; j < fieldTypes[i].operators.length; j++) {
                        conditionControl.options[j].value = fieldTypes[i].operators[j].operation;
                        conditionControl.options[j].text = fieldTypes[i].operators[j].text;
                    }
                    break;
                }
            }
            CheckSelectedPattern(patternDDL, txtValue, imgDeleteCustomExpressionClientId, imgSaveImageClientId, txtErrorMsgClientId);
        }
        else if (selectedRuleValue == "field") {
            patternControl.disabled = true;
            // searching for the right field type
            var selectedFieldType = "text";
            for (i = 0; i < item_defTypes.length; i++) {
                if (item_defTypes[i].fieldName == selectedFieldValue) {
                    selectedFieldType = item_defTypes[i].fieldType;
                    break;
                }
            }

            for (i = 0; i < item_fieldTypes.length; i++) {
                if (item_fieldTypes[i].name == selectedFieldType) {

                    if (item_fieldTypes[i].name == "yesno")
                        document.getElementById(txtValue).style.display = "none";
                    else {
                        document.getElementById(txtValue).style.display = "block";
                        CheckSelectedPattern(patternDDL, txtValue, imgDeleteCustomExpressionClientId, imgSaveImageClientId, txtErrorMsgClientId);
                    }

                    conditionControl.options.length = item_fieldTypes[i].operators.length
                    for (j = 0; j < item_fieldTypes[i].operators.length; j++) {
                        conditionControl.options[j].value = item_fieldTypes[i].operators[j].operation;
                        conditionControl.options[j].text = item_fieldTypes[i].operators[j].text;
                    }
                    break;
                }
            }
        }
    }

    function CheckSelectedPattern(ddlPatternClientID, txtPatternClientID, imgDeleteCustomExpressionClientId, imgSaveImageClientId, txtErrorMsgClientId) {
        var ddlPattern = document.getElementById(ddlPatternClientID);
        var txtPattern = document.getElementById(txtPatternClientID);
        var txtErrorMeassage = document.getElementById(txtErrorMsgClientId);
        var imgDeleteCustomExpression = document.getElementById(imgDeleteCustomExpressionClientId);
        var imgSaveImage = document.getElementById(imgSaveImageClientId);
        var trSaveCustomExpression = document.getElementById("trSaveCustomExpression");

        trSaveCustomExpression.style.display = "none";
        //txtPattern.value = "";

        if (ddlPattern.disabled) {
            txtPattern.style.display = "block";
            imgSaveImage.style.display = "none";
            imgDeleteCustomExpression.style.display = "none";
        }
        else if (ddlPattern.selectedIndex == 0) {
            txtPattern.style.display = "block";
            imgSaveImage.style.display = "block";
            imgDeleteCustomExpression.style.display = "none";
        }
        else if (ddlPattern.selectedIndex > 4) {
            txtPattern.style.display = "block";
            if (ddlPattern.value.indexOf("<<IW:INNER>>") > -1) {

                txtPattern.value = ddlPattern.value.split("<<IW:INNER>>")[0];
                txtErrorMeassage.value = ddlPattern.value.split("<<IW:INNER>>")[1];
            }
            else
                txtPattern.value = ddlPattern.value;
            imgSaveImage.style.display = "none";
            imgDeleteCustomExpression.style.display = "block";
        }
        else {
            txtPattern.style.display = "none";
            imgSaveImage.style.display = "none";
            imgDeleteCustomExpression.style.display = "none";
        }
    }

    function OpenSaveCustomExpression() {
        var trSaveCustomExpression = document.getElementById("trSaveCustomExpression");
        if (trSaveCustomExpression.style.display == "block")
            trSaveCustomExpression.style.display = "none";
        else
            trSaveCustomExpression.style.display = "block";
    }
</script>
<asp:HiddenField runat="server" ID="hidCurrentFieldRules" />
<asp:HiddenField runat="server" ID="hidEditedRule" Value="-1" />
<%--    <IW:LiteWarning runat="server" />--%>
<table style="width: 600px">
    <tr>
        <td class="ms-linksectionheader" style="padding-right: 4px; padding-left: 4px; padding-bottom: 4px;
            padding-top: 4px; white-space: nowrap" nowrap="nowrap" width="100%" colspan="2">
            <h3 class="ms-standardheader">
                <asp:Literal ID="Literal1" Text="General Settings" runat="server" />
            </h3>
        </td>
    </tr>
    <tr>
        <td class="iw-SectionTitle" style="width: 160px">
            <asp:Literal ID="Literal2" Text="Column:" runat="server" />
        </td>
        <td style="width: 100%; padding-right: 10px;">
            <asp:DropDownList runat="server" ID="cboAllFields" CssClass="iw-DropDown">
            </asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td class="ms-linksectionheader" style="padding-right: 4px; padding-left: 4px; padding-bottom: 4px;
            padding-top: 4px" nowrap width="100%" colspan="2">
            <h3 class="ms-standardheader">
                <asp:Literal ID="Literal6" Text="Validation Rule" runat="server" />
            </h3>
        </td>
    </tr>
    <tr>
        <td colspan="2" style="width: 100%;">
            <table style="border-collapse: collapse;" cellspacing="0" cellpadding="1" width="100%"
                border="0">
                <tr>
                    <th class="ms-vh2-nofilter" scope="col" nowrap>
                        <asp:Literal ID="Literal7" runat="server" Text="Rule" />
                    </th>
                    <th class="ms-vh2-nofilter" scope="col">
                        <asp:Literal ID="Literal8" runat="server" Text="Operator" />
                    </th>
                    <th class="ms-vh2-nofilter" width="140px" scope="col">
                        <asp:Literal ID="Literal9" runat="server" Text="Value" />
                    </th>
                    <th class="ms-vh2-nofilter" scope="col">
                        <asp:Literal ID="Literal10" runat="server" Text="Error Message" />
                    </th>
                </tr>
                <tr>
                    <td>
                        <asp:DropDownList runat="server" ID="cboRule" CssClass="iw-DropDown" AutoPostBack="True"
                            OnSelectedIndexChanged="cboRule_SelectedIndexChanged">
                            <asp:ListItem Value="31">Pattern</asp:ListItem>
                            <asp:ListItem Value="33">Column</asp:ListItem>
                            <asp:ListItem Value="32">Length</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td>
                        <asp:DropDownList runat="server" ID="cboRulesOperators" CssClass="iw-DropDown">
                        </asp:DropDownList>
                    </td>
                    <td style="width: 140px">
                        <asp:TextBox Width="160" runat="server" ID="txtRulesValue" />
                    </td>

                    <td>
                        &nbsp;
                        <asp:TextBox runat="server" ID="txtErrorMessage" />
                    </td>
                </tr>
                <tr id="trSaveCustomExpression" style="display: none;">
                    <td>
                        &nbsp;
                    </td>
                    <td>
                        &nbsp;
                    </td>
                    <td class="iw-Results" style="padding: 0px, 1px, 0px, 0px;">
                    </td>
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td colspan="6">
                        <asp:RequiredFieldValidator ID="txtRuleValueRequiredFieldValidator" 
                            runat="server" ControlToValidate="txtRulesValue" 
                            ErrorMessage="Add validation rule value" ValidationGroup="FieldValidation"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td colspan="6">
                        <span class="ms-error">
                            <asp:Literal runat="server" ID="lblError" EnableViewState="false"></asp:Literal></span>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td class="iw-SectionTitle" style="width: 160px">
            <asp:Literal ID="Literal123" Text="For users\groups" runat="server" />:
        </td>
        <td>
            <table>
                <tr>
                    <td>
                        <asp:DropDownList ID="cboSPPrinciplesOperator" runat="server">
                            <asp:ListItem Text="In" Value="101" />
                            <asp:ListItem Text="Not In" Value="102" />
                        </asp:DropDownList>
                    </td>
                    <td>
                        <SharePoint:PeopleEditor runat="server" ID="peSelectUsers" MultiSelect="false" MaximumEntities="1"
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
            <asp:GridView ID="gvCondition" runat="server" AutoGenerateColumns="False" CellPadding="4"
                EnableModelValidation="True" ForeColor="#333333" GridLines="None" OnRowCommand="gvCondition_RowCommand">
                <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                <Columns>
                    <asp:BoundField DataField="RowId" HeaderText="RowID" Visible="False" />
                    <asp:BoundField DataField="TabRowID" HeaderText="ValitdationRowID" Visible="False" />
                    <asp:BoundField DataField="SPFieldDisplayName" HeaderText="Field Name" />
                    <asp:BoundField DataField="SPFieldOperatorName" HeaderText="Operator" />
                    <asp:BoundField DataField="SPFieldOperatorID" HeaderText="SPFieldOperatorID" Visible="False" />
                    <asp:BoundField DataField="Value" HeaderText="Value" />                    
                    <asp:TemplateField>
                        <ItemTemplate>
                        <asp:ImageButton ID="lnkDeleteCondition" runat="server" ImageUrl="~/_layouts/ASPL.SharePoint2010/Resource/Delete.bmp"
                            CommandArgument='<%#Eval("RowID")%>' CommandName="DeleteCondition" />
                        </ItemTemplate> </asp:TemplateField>
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
                        <asp:DropDownList ID="cboAllFieldsForCondition" runat="server">
                        </asp:DropDownList>
                    </td>
                    <td>
                        <asp:DropDownList ID="cboConditionOperator" runat="server">
                            <asp:ListItem Value="103" Text="Equal"></asp:ListItem>
                            <asp:ListItem Value="104" Text="Not Equal"></asp:ListItem>
                            <asp:ListItem Value="105" Text="Contains"></asp:ListItem>
                            <asp:ListItem Value="106" Text="Not Contains"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td>
                        <asp:TextBox ID="txtValue" runat="server"></asp:TextBox>
                    </td>
                    <td>
                        <asp:LinkButton ID="lnkAddCondition" runat="server" Text="Add" 
                            OnClick="lnkAddCondition_Click" Enabled="False" 
                            ValidationGroup="Condition"></asp:LinkButton>
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
            <asp:Button Width="100%" runat="server" ID="cmdCreateNewRule" Text="Add new Validation Rule"
                OnClick="cmdCreateNewRule_Click" ValidationGroup="FieldValidation" />
        </td>
    </tr>
    <tr>
        <td colspan="2">
            &nbsp;</td>
    </tr>
    <tr>
        <td class="ms-linksectionheader" style="padding-right: 4px; padding-left: 4px; padding-bottom: 4px;
            padding-top: 4px" nowrap width="100%" colspan="2">
            <h3 class="ms-standardheader">
                <asp:Literal ID="Literal11" Text="Current validation rule" runat="server" />
            </h3>
        </td>
    </tr>
    <tr>
        <td colspan="2">
            <asp:GridView ID="gvResult" runat="server" AutoGenerateColumns="False" CellPadding="4"
                EnableModelValidation="True" ForeColor="#333333" GridLines="None" OnRowCommand="gvResult_RowCommand"   >
                <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                <Columns>
                    <asp:BoundField DataField="RowId" HeaderText="RowID" Visible="False" />
                    <asp:BoundField DataField="SPFieldDisplayName" HeaderText="Field Name" />
                    <asp:BoundField DataField="ValidationRuleID" HeaderText="SPFieldValidationRuleID" Visible="False" />
                    <asp:BoundField DataField="ValidationRuleName" HeaderText="Rule" />
                    <asp:BoundField DataField="SPFieldOperatorName" HeaderText="Operator" />
                    <asp:BoundField DataField="SPFieldOperatorID" HeaderText="SPFieldOperatorID" Visible="False" />                                      
                    <asp:BoundField DataField="Value" HeaderText="Value" />
                    <asp:BoundField DataField="ErrorMessage" HeaderText="Error Message" />
                    <asp:TemplateField ControlStyle-Width="20%" HeaderText="For User/Group">
                        <ItemTemplate>
                            <asp:Label ID="lblSPPrinciples" runat="server" Text='<%#Eval("SPPrinciplesOperatorName") + "" + Eval("SPPrinciples")%>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="HasCondition" HeaderText="Condition" ControlStyle-Width="15%" />
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
        <td style="padding: 6px">

        </td>
    </tr>
    <tr>
        <td colspan="2" class="iw-SectionTitle">
            <%--<asp:CheckBox ID="chkForceValidation" runat="server" Text="Validate empty columns" />--%>
        </td>
    </tr>
    <tr>
        <td colspan="2">
            <table width="100%">
                <tr>
                    <td id="tdIWOKCancelButtons">
                        <asp:Button ID="cmdOK" runat="server" Text="Save" 
                            CssClass="ms-ButtonHeightWidth" CausesValidation="false" OnClick="cmdOK_Click" />
                        <asp:Button ID="Cancel" runat="server" Text="Cancel" CssClass="ms-ButtonHeightWidth"
                            CausesValidation="false" OnClick="Cancel_Click" />
                    </td>
                    <td id="tdIWDevBy" class="ms-propertysheet">
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>
<asp:HiddenField runat="server" ID="hidActionRow" Value="-1" />
<asp:Button runat="server" ID="cmdDeleteCommand" Style="display: none;" CausesValidation="false" />
<asp:Button runat="server" ID="cmdEditCommand" Style="display: none;" CausesValidation="false" />
<asp:Literal runat="server" ID="litScripts"></asp:Literal>
<%--<script type="text/javascript">
        document.getElementById("tdIWOKCancelButtons").align = languageAlign;
        document.getElementById("tdIWOKCancelButtons").style.textAlign = languageAlign;
        document.getElementById("tdIWDevBy").align = languageOAlign;
        document.getElementById("tdIWDevBy").style.textAlign = languageOAlign;
    </script>--%>