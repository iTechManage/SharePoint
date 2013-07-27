<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls"
    Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages"
    Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CCSCascadedLookupFieldEditor.ascx.cs"
    Inherits="CrowCanyon.CascadedLookup.CCSCascadedLookupFieldEditor" %>
<%@ Register TagPrefix="wssuc" TagName="InputFormControl" Src="~/_controltemplates/InputFormControl.ascx" %>
<%@ Register TagPrefix="wssuc" TagName="InputFormSection" Src="~/_controltemplates/InputFormSection.ascx" %>
<wssuc:InputFormSection runat="server" ID="FilterLookupFieldSection" Title="Special Column Settings">
    <Template_InputFormControls>
        <wssuc:InputFormControl ID="InputFormControl1" runat="server" LabelText="Specify detailed options for the filtered lookup column">
            <Template_Control>
                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                    <ContentTemplate>
                        <style type="text/css">
                            .style1
                            {
                                width: 100%;
                                height: 48px;
                            }
                            .style2
                            {
                                width: 100%;
                                height: 26px;
                            }
                            .style3
                            {
                                width: 100%;
                                height: 20px;
                            }
                        </style>
                        <div style="width: 100%; text-align: left; border-width: 0px;">
                            <table style="width: 100%; border-width: 0px; border-collapse: collapse;" cellpadding="0"
                                cellspacing="0">
                                <tr>
                                    <td class="ms-authoringcontrols" style="width: 100%; text-align: left; white-space: nowrap;
                                        padding-top: 10px;">
                                        <span>Get Information from this site:</span>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="ms-authoringcontrols" style="width: 100%; text-align: left; white-space: nowrap;">
                                        <div style="padding-left: 15px">
                                            <asp:DropDownList runat="server" ID="ddlWeb" AutoPostBack="true" OnSelectedIndexChanged="ddlWeb_SelectedIndexChanged" />
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="ms-authoringcontrols" style="width: 100%; text-align: left; white-space: nowrap;
                                        padding-top: 10px;">
                                        <span>Get Information from:</span>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="ms-authoringcontrols" style="width: 100%; text-align: left; white-space: nowrap;">
                                        <div style="padding-left: 15px">
                                            <asp:DropDownList runat="server" ID="ddlList" AutoPostBack="true" OnSelectedIndexChanged="ddlList_SelectedIndexChanged" />
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="ms-authoringcontrols" style="width: 100%; text-align: left; white-space: nowrap;
                                        padding-top: 10px;">
                                        <span>In this column</span>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="ms-authoringcontrols" style="width: 100%; text-align: left; white-space: nowrap;">
                                        <div style="padding-left: 15px">
                                            <asp:DropDownList runat="server" ID="ddlColumn" />
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="ms-authoringcontrols" style="width: 100%; text-align: left; white-space: nowrap;">
                                        <asp:CheckBox runat="server" ID="cbxLinkParent" Text="Link to parent column" Checked="true"
                                            OnCheckedChanged="cbxLinkParent_CheckedChanged" AutoPostBack="true" Style="padding-left: 15px" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="ms-authoringcontrols" style="width: 100%; text-align: left; white-space: nowrap;">
                                        <asp:CheckBox runat="server" ID="cbxShowallParentEmpty" Text="Show all values when parent is empty"
                                            Checked="false" Style="padding-left: 15px" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="ms-authoringcontrols" style="text-align: left; white-space: nowrap; padding-bottom: 10px;">
                                        <asp:Label runat="server" ID="lblParentColumn" Text="Parent Column:" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="ms-authoringcontrols" style="text-align: left; white-space: nowrap;">
                                        <div style="padding-left: 15px">
                                            <asp:DropDownList runat="server" ID="ddlParentColumn" AutoPostBack="true" OnSelectedIndexChanged="ddlParentColumn_SelectedIndexChanged" />
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="ms-authoringcontrols" style="width: 100%; text-align: left; white-space: nowrap;
                                        padding-top: 10px;">
                                        <asp:Label runat="server" ID="lblLinkColumn" Text="Link Column:" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="ms-authoringcontrols" style="width: 100%; text-align: left; white-space: nowrap;">
                                        <div style="padding-left: 15px">
                                            <asp:DropDownList runat="server" ID="ddlLinkColumn" />
                                            <asp:Label runat="server" ID="lbllistLinkColumn" Text="Source list must contain lookup column for the parent list" />
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="style3" style="text-align: left; white-space: nowrap;">
                                        <br />
                                        <asp:CheckBox runat="server" ID="cbxAllowMultiple" Text="Allow Multiple Values" Checked="false"
                                            OnCheckedChanged="cbxAllowMultiple_CheckedChanged" AutoPostBack="true" Style="padding-left: 15px" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="ms-authoringcontrols" style="width: 100%; text-align: left; white-space: nowrap;">
                                        <asp:CheckBox runat="server" ID="cbxAllowAutoComplete" Text="Allow auto-complete or filter"
                                            Checked="false" Visible="false" Style="padding-left: 15px" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="ms-authoringcontrols" style="width: 100%; text-align: left; white-space: nowrap;">
                                        <b>
                                            <br />
                                            <asp:CheckBox runat="server" ID="cbxAdvanceSettings" Text="Advance Settings" Checked="false"
                                                OnCheckedChanged="cbxAdvanceSettings_CheckedChanged" AutoPostBack="true" /></b>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Table ID="AdditionSettingPane" Visible="false" runat="server">
                                            <asp:TableRow runat="server">
                                                <asp:TableCell runat="server" class="ms-authoringcontrols" Style="width: 100%; text-align: left;
                                                    white-space: nowrap;">
                                                    <asp:Label Style="padding-left: 15px" ID="lbAdditionalFields" Text="Add a column to show each of these additional fields"
                                                        runat="server" /><br />
                                                    <div style="padding-left: 25px">
                                                        <asp:CheckBoxList ID="cblAdditionalFields" Style="padding-left: 3px" runat="server"
                                                            CssClass="ms-authoringcontrols" />
                                                    </div>
                                                    <br />
                                                </asp:TableCell></asp:TableRow>
                                            <asp:TableRow runat="server">
                                                <asp:TableCell runat="server" class="ms-authoringcontrols" Style="width: 100%; text-align: left;
                                                    white-space: nowrap;">
                                                    <br />
                                                    <asp:Label Style="padding-left: 15px" ID="lbAdditionalFilters" Text="Additional Filters"
                                                        runat="server" /><br />
                                                    <div style="padding-left: 25px">
                                                        <asp:CheckBoxList ID="cblAdditionalFilters" runat="server" CssClass="ms-authoringcontrols" />
                                                    </div>
                                                    <br />
                                                </asp:TableCell></asp:TableRow>
                                            <asp:TableRow runat="server">
                                                <asp:TableCell runat="server">
                                                    <asp:Label ID="lbView" Style="padding-left: 15px" Text="View:" runat="server" /><br />
                                                    <div style="padding-left: 25px">
                                                        <asp:DropDownList ID="ddlView" Style="padding-left: 18px" runat="server" OnSelectedIndexChanged="ddlView_SelectedIndexChanged"
                                                            AutoPostBack="true" />
                                                    </div>
                                                </asp:TableCell></asp:TableRow>
                                            <asp:TableRow runat="server">
                                                <asp:TableCell runat="server">
                                                    <asp:CheckBox ID="cbxSortByView" Style="padding-left: 25px" runat="server" Text="SortByView"
                                                        Enabled="false" Checked="false" />
                                                    <br />
                                                </asp:TableCell></asp:TableRow>
                                            <asp:TableRow runat="server">
                                                <asp:TableCell runat="server">
                                                    <br />
                                                    <asp:CheckBox ID="cbxAllowNewValues" Style="padding-left: 25px" runat="server" Text="Allow Adding New Values"
                                                        Checked="false" OnCheckedChanged="cbxAllowNewValues_CheckedChanged" AutoPostBack="true" />
                                                </asp:TableCell></asp:TableRow>
                                            <asp:TableRow runat="server">
                                                <asp:TableCell runat="server">
                                                    <asp:CheckBox ID="cbxUseNewForm" Style="padding-left: 25px" runat="server" Text="Use NEW Form"
                                                        Enabled="false" Checked="false" />
                                                </asp:TableCell></asp:TableRow>
                                        </asp:Table>
                                    </td>
                                </tr>
                            </table>
                            <asp:Panel ID="pnlConvertFromLookup" runat="server" Visible="false">
                                <table>
                                    <tr>
                                        <td class="ms-authoringcontrols" nowrap="nowrap">
                                            <asp:Label ID="lblConvertFromLookup" runat="server" Text="Convert from:" />
                                        </td>
                                        <td class="ms-authoringcontrols">
                                            <asp:DropDownList ID="ddlConvertFromLookup" runat="server" />
                                        </td>
                                        <td class="ms-authoringcontrols">
                                            <asp:Button ID="btnConvertFromLookup" runat="server" Text="Convert" CssClass="ms-ButtonHeightWidth"
                                                ValidationGroup="cfconvert" OnClick="btnConvertFromLookup_Click"  />
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                            <asp:Panel ID="pnlConvertToLookup" runat="server" Visible="false" style="width: 100%;">
                                <table style="width: 100%;">
                                    <tr>
                                        <td class="ms-authoringcontrols" style="width: 100%;">
                                            <asp:Button ID="btnConvertToLookup" runat="server" Text="Convert to Regular Lookup"
                                                ValidationGroup="cfconvert" OnClick="btnConvertToLookup_Click" OnClientClick="return confirm('Are you sure you want to convert the current Field into Regular Lookup Field?')" class="ms-ButtonHeightWidth" style="width: 100%;"/>
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </Template_Control>
        </wssuc:InputFormControl>
    </Template_InputFormControls>
</wssuc:InputFormSection>
<wssuc:InputFormSection runat="server" ID="Relationship" Title="Relationship">
    <Template_InputFormControls>
        <wssuc:InputFormControl ID="InputFormControl2" runat="server">
            <Template_Control>
                <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                    <ContentTemplate>
                        <div style="width: 100%; text-align: left; border-width: 0px;">
                            <table style="width: 100%; border-width: 0px; border-collapse: collapse;" cellpadding="0"
                                cellspacing="0">
                                <tr>
                                    <td class="ms-authoringcontrols" style="width: 100%; text-align: left; white-space: nowrap;">
                                        <asp:CheckBox runat="server" ID="cbxRelationshipBehavior" Text="Enforce Relationship Behavior"
                                            OnCheckedChanged="cbxRelationshipBehavior_CheckedChanged" AutoPostBack="true" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="ms-authoringcontrols" style="width: 100%; text-align: left; white-space: nowrap;">
                                        <asp:RadioButton runat="server" ID="rbRestrictDelete" Text="Restrict Delete" Checked="true"
                                            Enabled="false" OnCheckedChanged="rbRestrictDelete_CheckedChanged" AutoPostBack="true" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="ms-authoringcontrols" style="width: 100%; text-align: left; white-space: nowrap;">
                                        <asp:RadioButton runat="server" ID="rbCascadeDelete" Text="Cascade Delete" Enabled="false"
                                            OnCheckedChanged="rbCascadeDelete_CheckedChanged" AutoPostBack="true" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </Template_Control>
        </wssuc:InputFormControl>
    </Template_InputFormControls>
</wssuc:InputFormSection>
