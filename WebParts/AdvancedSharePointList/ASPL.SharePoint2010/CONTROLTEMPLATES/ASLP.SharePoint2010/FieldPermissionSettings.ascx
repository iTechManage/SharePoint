<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FieldPermissionSettings.ascx.cs" Inherits="AdvanceSharepointListPro.CONTROLTEMPLATES.FieldPermissionSettings" %>
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

    </style>
        

    <table style="width:600px">
        <tr>
            <td class="ms-linksectionheader" style="padding-right: 4px; padding-left: 4px; padding-bottom: 4px;
                padding-top: 4px;white-space:nowrap" nowrap="nowrap" width="100%" colspan="2">
                <h3 class="ms-standardheader">
                    <asp:Literal ID="Literal1" Text="Field Permission" runat="server" />
                </h3>
            </td>
        </tr>
        <tr>
            <td class="iw-SectionTitle" style="width: 160px">
                <asp:Literal ID="Literal2" Text="Fields" runat="server" />:
            </td>
            <td>
                <asp:DropDownList runat="server" ID="cboFields" CssClass="iw-DropDown">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="iw-SectionTitle" style="width: 160px">
                <asp:Literal ID="Literal3" runat="server" Text="Permission level" />:
            </td>
            <td>
                <asp:RadioButtonList runat="server" ID="cdoPermissionLevel" RepeatDirection="Horizontal"
                    CssClass="iw-DropDown" ValidationGroup="FieldPermission">
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
                    CssClass="ms-formvalidation" ValidationGroup="FieldPermission" SetFocusOnError="True" />       
            </td>
        </tr>
        <tr>
            <td class="iw-SectionTitle" style="width: 160px">
                <asp:Literal ID="Literal4" Text="Apply in forms" runat="server" />:
            </td>
            <td>
                <asp:CheckBoxList ValidationGroup="FieldPermission" runat="server" ID="chkPages"
                    RepeatDirection="Vertical" CssClass="iw-DropDown">
                    <asp:ListItem Value="21">New item</asp:ListItem>
                    <asp:ListItem Value="22">Edit item</asp:ListItem>
                    <asp:ListItem Value="23">View Existing item</asp:ListItem>
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
                    EnableModelValidation="True" ForeColor="#333333" GridLines="None" OnRowCommand="gvCondition_RowCommand">
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
                    ValidationGroup="FieldPermission" onclick="cmdCreateNewRule_Click" />
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
                        <asp:BoundField DataField="SPFieldDisplayName" HeaderText="Field Name" ControlStyle-Width="20%" />
                        <asp:BoundField DataField="PermissionName" HeaderText="Permission Name" ControlStyle-Width="20%" />
                        <asp:TemplateField ControlStyle-Width="20%" HeaderText="For User/Group">
                            <ItemTemplate>
                                <asp:Label ID="lblSPPrinciples" runat="server" Text='<%#Eval("SPPrinciplesOperatorName") + "" + Eval("SPPrinciples")%>'                                    />
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
                <!--<asp:CheckBox ID="chkReversePermissions" runat="server" Text="" />-->
            </td>
       </tr>
        <tr>
            <td colspan="2">
                <table width="100%">
                    <tr>
                        <td id="tdIWOKCancelButtons">
                            <asp:Button ID="cmdOK" runat="server" Text="Save" CssClass="ms-ButtonHeightWidth"
                                CausesValidation="false" OnClick="cmdOK_Click" />
                            <asp:Button ID="Cancel" runat="server" Text="Cancel" CssClass="ms-ButtonHeightWidth"
                                CausesValidation="false" OnClick="Cancel_Click" />
                            <asp:Button ID="btnHiddenSave" CausesValidation="false" Style="display: none" runat="server" />
                        </td>
                        <td id="tdIWDevBy" class="ms-propertysheet">
                          
                        </td>                    
                    </tr>
                </table>
            </td>
        </tr>        
    </table>
    <asp:Literal runat="server" ID="litScripts"></asp:Literal>