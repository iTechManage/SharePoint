<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls"
    Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdvancedAlertSettings.aspx.cs"
    Inherits="CCSAdvancedAlerts.Layouts.CCSAdvancedAlerts.AdvancedAlertSettings"
    DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
</asp:Content>
<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <table style="width: 600px">
        <tr>
            <td colspan="2" class="ms-linksectionheader" style="padding-right: 4px; padding-left: 4px;
                padding-bottom: 4px; padding-top: 4px;" nowrap="nowrap" width="576">
                <b>General Settings </b>
            </td>
        </tr>
        <tr>
            <td>
                <table width="100%">
                    <tr>
                        <td class="ms-descriptiontext" style="width: 100px" valign="top">
                            <asp:Literal ID="Literal1" runat="server" Text="Title:"></asp:Literal>
                        </td>
                        <td>
                            <asp:TextBox ID="txtTitle" runat="server" Width="70%"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="ms-descriptiontext" style="width: 100px" valign="top">
                            <asp:Literal ID="Literal2" runat="server" Text="Site:"></asp:Literal>
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlSite" runat="server" AutoPostBack="true" Width="70%">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="ms-descriptiontext" style="width: 100px" valign="top">
                            <asp:Literal ID="Literal3" runat="server" Text="List:"></asp:Literal>
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlList" runat="server" AutoPostBack="True" Width="70%">
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td class="ms-linksectionheader" style="padding-right: 4px; padding-left: 4px; padding-bottom: 4px;
                padding-top: 4px;" nowrap="nowrap" width="576">
                <b>Event Type </b>
            </td>
        </tr>
        <tr>
            <td>
                <table>
                    <tr>
                        <td class="ms-descriptiontext">
                            <asp:CheckBox ID="chkItemAdded" runat="server" Text="Item Added" />
                        </td>
                    </tr>
                    <tr>
                        <td class="ms-descriptiontext">
                            <asp:CheckBox ID="chkItemUpdated" runat="server" Text="Item Updated" />
                        </td>
                    </tr>
                    <tr>
                        <td class="ms-descriptiontext">
                            <asp:CheckBox ID="chkItemDeleted" runat="server" Text="Item Deleted" />
                        </td>
                    </tr>
                    <tr>
                        <td class="ms-descriptiontext" style="white-space: nowrap; vertical-align: top">
                            <asp:CheckBox ID="chkDateColumn" runat="server" Text="According to date in column" />
                            &nbsp;&nbsp;
                            <asp:DropDownList ID="Dropdownlist1" runat="server">
                            </asp:DropDownList>
                            <asp:Panel ID="panelDateColumn" runat="server">
                                &nbsp; &nbsp; &nbsp;<asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
                                <asp:DropDownList ID="DropDownList2" runat="server">
                                </asp:DropDownList>
                                <asp:DropDownList ID="DropDownList3" runat="server">
                                </asp:DropDownList>
                                <br />
                                &nbsp;&nbsp;&nbsp;&nbsp;
                                <asp:CheckBox ID="chkRepeat" runat="server" Text="Repeat every" />
                                &nbsp;<asp:TextBox ID="TextBox2" runat="server"></asp:TextBox>
                                <asp:DropDownList ID="DropDownList4" runat="server">
                                </asp:DropDownList>
                                <asp:DropDownList ID="DropDownList5" runat="server">
                                </asp:DropDownList>
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td colspan="2" class="ms-linksectionheader" style="padding-right: 4px; padding-left: 4px;
                padding-bottom: 4px; padding-top: 4px;" nowrap="nowrap" width="576">
                <b>Recipients </b>
            </td>
        </tr>
        <tr>
            <td valign="top" align="left">
                <table>
                    <tr>
                        <td valign="top">
                            <table width="100%">
                                <tr>
                                    <td class="ms-descriptiontext" style="white-space: nowrap; vertical-align: top">
                                        <asp:Literal ID="Literal5" runat="server" Text="To:"></asp:Literal>
                                    </td>
                                    <td class="ms-descriptiontext" style="white-space: nowrap; vertical-align: top">
                                        <asp:TextBox ID="txtTo" runat="server" Rows="2" Width="100%" TextMode="MultiLine"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="ms-descriptiontext" style="white-space: nowrap; vertical-align: top">
                                        <asp:Literal ID="Literal6" runat="server" Text="CC:"></asp:Literal>
                                    </td>
                                    <td class="ms-descriptiontext" style="white-space: nowrap; vertical-align: top">
                                        <asp:TextBox ID="txtCc" runat="server" Rows="2" Width="100%" TextMode="MultiLine"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="ms-descriptiontext" style="white-space: nowrap; vertical-align: top">
                                        <asp:Literal ID="Literal7" runat="server" Text="BCC:"></asp:Literal>
                                    </td>
                                    <td class="ms-descriptiontext" style="white-space: nowrap; vertical-align: top">
                                        <asp:TextBox ID="txtBcc" runat="server" Rows="2" Width="100%" TextMode="MultiLine"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="ms-descriptiontext" style="white-space: nowrap; vertical-align: top">
                                        <asp:Literal ID="Literal4" runat="server" Text="From:"></asp:Literal>
                                    </td>
                                    <td class="ms-descriptiontext" style="white-space: nowrap; vertical-align: top">
                                        <asp:TextBox ID="txtFrom" runat="server" Width="100%"></asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr valign="top">
                        <td valign="top">
                            <table class="ms-authoringcontrols" cellpadding="2" cellspacing="2" width="100%">
                                <tr>
                                    <td colspan="2" style="white-space: nowrap; vertical-align: top" class="ms-descriptiontext">
                                        <b>
                                            <asp:Literal ID="Literal8" runat="server" Text="Add To Recipients"></asp:Literal></b>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2" style="white-space: nowrap; vertical-align: top" class="ms-descriptiontext">
                                        <asp:RadioButton ID="rdCurrentUser" Text="Current User" runat="server" GroupName="rgpAddToRecipients">
                                        </asp:RadioButton>
                                    </td>
                                </tr>
                                <tr>
                                    <td nowrap="nowrap" style="white-space: nowrap; vertical-align: top" class="ms-descriptiontext">
                                        <asp:RadioButton ID="rdUsers" Text="Users" runat="server" GroupName="rgpAddToRecipients">
                                        </asp:RadioButton>
                                    </td>
                                    <td style="white-space: nowrap; vertical-align: top" class="ms-descriptiontext">
                                        <SharePoint:PeopleEditor ID="additionalUsers" MultiSelect="true" runat="server" PlaceButtonsUnderEntityEditor="true"
                                            Width="100%" />
                                    </td>
                                </tr>
                                <tr>
                                    <td nowrap="nowrap" style="white-space: nowrap; vertical-align: top" class="ms-descriptiontext">
                                        <asp:RadioButton ID="rdUsersincolumn" Text="Users in column" runat="server" GroupName="rgpAddToRecipients">
                                        </asp:RadioButton>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="ddlUsersInColumn" runat="server">
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td nowrap="nowrap" style="white-space: nowrap; vertical-align: top" class="ms-descriptiontext">
                                        <asp:RadioButton ID="rdEmailAddresses" Text="E-mail addresses" runat="server" GroupName="rgpAddToRecipients">
                                        </asp:RadioButton>
                                    </td>
                                    <td>
                                        <asp:TextBox runat="server" ID="txtEmailAddresses" Width="98%"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        <asp:Button ID="btnAddTO" runat="server" Text="Add to To" CssClass="ms-ButtonHeightWidth"
                                            Width="190px" />
                                        <asp:Button ID="btnAddCC" runat="server" Text="Add to CC" CssClass="ms-ButtonHeightWidth"
                                            Width="190px" />
                                        <asp:Button ID="btnAddBCC" runat="server" Text="Add to Bcc" CssClass="ms-ButtonHeightWidth"
                                            Width="190px" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td nowrap="nowrap">
                            <table>
                                <tr>
                                    <td class="ms-descriptiontext" style="white-space: nowrap; vertical-align: top">
                                        <asp:Literal ID="Literal9" runat="server" Text="Never send to:"></asp:Literal>
                                    </td>
                                    <td style="white-space: nowrap; vertical-align: top" class="ms-descriptiontext">
                                        <SharePoint:PeopleEditor ID="ppleNeverSendTo" MultiSelect="true" runat="server" PlaceButtonsUnderEntityEditor="true"
                                            Width="100%" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td class="ms-linksectionheader" style="padding-right: 4px; padding-left: 4px; padding-bottom: 4px;
                padding-top: 4px;" nowrap="nowrap" width="576">
                <b>When To Send</b>
            </td>
        </tr>
        <tr>
            <td>
                <table width="100%">
                    <tr>
                        <td class="ms-descriptiontext">
                            <asp:RadioButton ID="rdImmediately" runat="server" Text="Immediately"></asp:RadioButton>
                        </td>
                    </tr>
                    <tr>
                        <td class="ms-descriptiontext">
                            <asp:RadioButton ID="rdDaily" runat="server" Text="Daily"></asp:RadioButton>
                        </td>
                    </tr>
                    <tr>
                        <td class="ms-descriptiontext">
                            <asp:RadioButton ID="rdWeekly" runat="server" Text="Weekly"></asp:RadioButton>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td class="ms-linksectionheader" style="padding-right: 4px; padding-left: 4px; padding-bottom: 4px;
                padding-top: 4px;" nowrap="nowrap" width="576">
                <b>Conditions</b>
            </td>
        </tr>
        <tr>
            <td>
                <table>
                    <tr>
                        <td class="ms-descriptiontext">
                            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                <ContentTemplate>
                                    <asp:GridView ID="GridView1" runat="server">
                                    </asp:GridView>
                                    <SharePoint:SPGridView ID="gvConditions" runat="server" AutoGenerateColumns="false"
                                        ShowFooter="true" FooterStyle-CssClass="ms-vb2" CellPadding="1" CellSpacing="0"
                                        OnRowCommand="gvConditions_RowCommand" OnRowCancelingEdit="gvConditions_RowCancelEditing"
                                        OnRowDeleting="gvConditions_RowDeleting" OnRowUpdating="gvConditions_RowUpdating"
                                        OnRowEditing="gvConditions_RowEditing">
                                        <EmptyDataTemplate>
                                            <table cellpadding="1" cellspacing="0">
                                                <tr>
                                                    <td class="ms-vb2" width="150">
                                                        <asp:DropDownList ID="ddlConditionField" runat="server" Width="150" />
                                                    </td>
                                                    <td class="ms-vb2" width="100">
                                                        <asp:DropDownList ID="ddlConditionOperator" runat="server" Width="100" />
                                                    </td>
                                                    <td class="ms-vb2" width="160">
                                                        <asp:TextBox ID="txtConditionFieldValue" runat="server"></asp:TextBox>
                                                    </td>
                                                    <td class="ms-vb2">
                                                        &nbsp;&nbsp;<asp:LinkButton CssClass="ms-vb2" ID="btnConditionAdd" runat="server"
                                                            CommandName="EmptyDataTemplateInsert" Text="Add" ValidationGroup="selectCond" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </EmptyDataTemplate>
                                        <Columns>
                                            <asp:TemplateField HeaderText="Column Name" HeaderStyle-CssClass="ms-vh2-nofilter"
                                                ItemStyle-Width="150">
                                                <ItemTemplate>
                                                    <%# Eval("FieldName") %>
                                                </ItemTemplate>
                                                <EditItemTemplate>
                                                    <asp:DropDownList runat="server" ID="ddlConditionField" Width="150" />
                                                </EditItemTemplate>
                                                <FooterTemplate>
                                                    <asp:DropDownList runat="server" ID="ddlConditionField" Width="150" />
                                                </FooterTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Operator" HeaderStyle-CssClass="ms-vh2-nofilter" ItemStyle-Width="150">
                                                <ItemTemplate>
                                                    <%# GetValidOperatorValue(Eval("ComparisionOperator")) %>
                                                </ItemTemplate>
                                                <EditItemTemplate>
                                                    <asp:DropDownList runat="server" ID="ddlConditionOperator" Width="150" />
                                                </EditItemTemplate>
                                                <FooterTemplate>
                                                    <asp:DropDownList runat="server" ID="ddlConditionOperator" Width="150" />
                                                </FooterTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Value" HeaderStyle-CssClass="ms-vh2-nofilter" ItemStyle-Width="150">
                                                <ItemTemplate>
                                                    <%# Eval("StrValue") %>
                                                </ItemTemplate>
                                                <EditItemTemplate>
                                                    <asp:TextBox ID="txtConditionFieldValue" runat="server"></asp:TextBox>
                                                </EditItemTemplate>
                                                <FooterTemplate>
                                                    <asp:TextBox ID="txtConditionFieldValue" runat="server"></asp:TextBox>
                                                </FooterTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField ItemStyle-Width="100" HeaderStyle-CssClass="ms-vh2-nofilter">
                                                <ItemTemplate>
                                                    <span style="white-space: nowrap">
                                                        <asp:LinkButton ID="btnEdit" CssClass="ms-vb2" runat="server" CausesValidation="false"
                                                            CommandName="Edit" Text="Edit" />
                                                        <asp:LinkButton ID="btnDelete" CssClass="ms-vb2" runat="server" CausesValidation="false"
                                                            CommandName="Delete" Text="Delete" />
                                                    </span>
                                                </ItemTemplate>
                                                <EditItemTemplate>
                                                    <span style="white-space: nowrap">
                                                        <asp:LinkButton ID="btnUpdate" CssClass="ms-vb2" runat="server" CommandName="Update"
                                                            Text="Update" ValidationGroup="selectCond" />
                                                        <asp:LinkButton ID="btnCancel" CssClass="ms-vb2" runat="server" CausesValidation="false"
                                                            CommandName="Cancel" Text="Cancel" />
                                                    </span>
                                                </EditItemTemplate>
                                                <FooterTemplate>
                                                    <asp:LinkButton CssClass="ms-vb2" ID="btnConditionAdd" runat="server" CommandName="FooterInsert"
                                                        Text="Add" ValidationGroup="selectCond" /></FooterTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </SharePoint:SPGridView>
                                    <%--<asp:DropDownList ID="ddlConditionField" runat="server" AutoPostBack="true">
                                    </asp:DropDownList>
                                    <asp:DropDownList ID="ddlConditionType" runat="server" AutoPostBack="true">
                                    </asp:DropDownList>
                                    <asp:DropDownList ID="ddlConditionOperator" runat="server" AutoPostBack="true">
                                    </asp:DropDownList>
                                    <asp:TextBox ID="ddlConditionFieldValue" runat="server"></asp:TextBox>
                                    <asp:LinkButton ID="btnConditionAdd" runat="server" Text="Add" CssClass="ms-ButtonHeightWidth" />--%>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td class="ms-linksectionheader" style="padding-right: 4px; padding-left: 4px; padding-bottom: 4px;
                padding-top: 4px;" nowrap="nowrap" width="576">
                <b>Mail Templates </b>
            </td>
        </tr>
        <tr>
            <td>
                <table width="100%" class="ms-authoringcontrols" cellpadding="4" cellspacing="0">
                    <tr>
                        <td>
                            <table cellpadding="2" cellspacing="2">
                                <tr>
                                    <td colspan="2">
                                        <h3 class="ms-standardheader">
                                            <asp:Literal ID="Literal14" Text="Add or Update Mail Template" runat="server" /></h3>
                                    </td>
                                    <td>
                                    </td>
                                    <td rowspan="8" style="background-color: #FFFFFF">
                                        <img src="/_layouts/images/blank.gif" alt="" width="15" />
                                    </td>
                                    <td>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="ms-descriptiontext" style="white-space: nowrap; vertical-align: top">
                                        <asp:Literal ID="Literal10" runat="server" Text="Name:"></asp:Literal>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtMailTemplateName" Width="100%" runat="server"></asp:TextBox>
                                    </td>
                                    <td>
                                    </td>
                                    <td rowspan="8" valign="top" class="ms-authoringcontrols" style="padding: 4px; vertical-align: top">
                                        <table cellspacing="0" cellpadding="0">
                                            <tr>
                                                <td class="ms-propertysheet">
                                                    <asp:Literal ID="Literal11" runat="server" Text="Columns in list:"></asp:Literal><br />
                                                    <br />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:ListBox ID="lstPlaceHolders" runat="server" Height="270px" Style="margin-left: 1px"
                                                        Width="150px"></asp:ListBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="ms-addnew">
                                                    <img alt="" src="/_layouts/images/rect.gif" />&nbsp;
                                                    <asp:LinkButton ID="btnAddToSubject" runat="server" CssClass="ms-addnew" Text="Add to subject"></asp:LinkButton>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="ms-addnew">
                                                    <img alt="" src="/_layouts/images/rect.gif" />&nbsp;<asp:LinkButton ID="btnCopyToClipBoard"
                                                        runat="server" Text="Copy to clipboard" CssClass="ms-addnew" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="ms-descriptiontext" style="white-space: nowrap; vertical-align: top">
                                        <asp:Literal ID="Literal12" runat="server" Text="Subject:"></asp:Literal>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtMailSubject" Width="100%" runat="server"></asp:TextBox>
                                    </td>
                                    <td>
                                    </td>
                                    <td>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="ms-descriptiontext" nowrap="nowrap" style="white-space: nowrap; vertical-align: top">
                                        <asp:Literal ID="Literal13" runat="server" Text="Mail Body:"></asp:Literal>
                                    </td>
                                    <td>
                                        <SharePoint:InputFormTextBox ID="txtBody" runat="server" TextMode="MultiLine" RichTextMode="FullHtml"
                                            RichText="true" Rows="10" Width="100%" />
                                    </td>
                                    <td>
                                    </td>
                                    <td>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2" class="ms-descriptiontext">
                                        <asp:CheckBox ID="chkIncludeUpdatedColumns" Text="Include updated columns" runat="server">
                                        </asp:CheckBox>
                                    </td>
                                    <td>
                                    </td>
                                    <td>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2" class="ms-descriptiontext">
                                        <asp:CheckBox ID="chkHighlightUpdatedColumns" Text="Highlight updated columns" runat="server">
                                        </asp:CheckBox>
                                    </td>
                                    <td>
                                    </td>
                                    <td>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2" class="ms-descriptiontext">
                                        <asp:CheckBox ID="chkInsertAttachments" Text="Insert attachments" runat="server">
                                        </asp:CheckBox>
                                    </td>
                                    <td>
                                    </td>
                                    <td>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        <asp:Button ID="btnTemplateAdd" runat="server" Text="Add" Width="30%" CssClass="ms-ButtonHeightWidth" />
                                        <asp:Button ID="btnTemplateUpdate" runat="server" Text="Update" Width="30%" CssClass="ms-ButtonHeightWidth" />
                                        <asp:Button ID="btnTemplateCancel" runat="server" Text="Cancel" Width="30%" CssClass="ms-ButtonHeightWidth" />
                                    </td>
                                    <td>
                                    </td>
                                    <td>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center" valign="middle">
                <asp:Button ID="btnAlertsave" runat="server" Text="Create Alert" Width="100%" CssClass="ms-ButtonHeightWidth" /><br />
            </td>
        </tr>
        <tr>
            <td align="center" valign="middle">
                <asp:Button ID="btnUpdateAlert" runat="server" Text="Update Alert" Width="100%" CssClass="ms-ButtonHeightWidth" />
            </td>
        </tr>
        <tr>
            <td align="right" valign="bottom">
                <asp:Button ID="btnOK" runat="server" Text="OK" Width="30%" CssClass="ms-ButtonHeightWidth" />&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Button ID="btnAlertcancel" runat="server" Text="Cancel" Width="30%" CssClass="ms-ButtonHeightWidth" />
            </td>
        </tr>
        <tr>
            <td class="ms-linksectionheader" style="padding-right: 4px; padding-left: 4px; padding-bottom: 4px;
                padding-top: 4px;" nowrap="nowrap" width="576" align="right">
                <h3 class="ms-standardheader" style="text-align: right">
                    Developed by <a href="http://www.sharepoint-applications.biz" target="_blank">CrowCanyon</a></h3>
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    Alert Settings
</asp:Content>
<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea"
    runat="server">
    Alert Settings
</asp:Content>
