<%@ Assembly Name="CCSAdvancedAlerts, Version=1.0.0.0, Culture=neutral, PublicKeyToken=d0e8c94870369eea" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdvancedAlertSettings.aspx.cs" Inherits="CCSAdvancedAlerts.Layouts.CCSAdvancedAlerts.AdvancedAlertSettings" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">


<head>
    <style type="text/css">
        .style1
        {
            width: 621px;
        }
        .style2
        {
            width: 617px;
        }
        .style3
        {
            width: 613px;
        }
        .style4
        {
            width: 118px;
        }
    </style>
</head>
<table width="100%" border="2" cellpadding="5" cellspacing="5">
    <tr>
        <td colspan="2">
            <table width="100%">
                <tr bgcolor ="#3399ff">
                    <td colspan="2">
                        <h4>
                            General Settings
                        </h4>
                    </td>
                </tr>
                <tr>
                    <td class="style1">
                        Title:
                    </td>
                    <td>
                        <asp:textbox id="txtTitle" runat="server" style="" width="225px"></asp:textbox>
                    </td>
                </tr>
                
                <tr>
                    <td class="style1">
                        Site:
                    </td>
                    <td>
                        <asp:dropdownlist id="ddlSite" runat="server" width="223px" AutoPostBack= "true"></asp:dropdownlist>
                    </td>
                </tr>
                <tr>
                    <td class="style1">
                        List:
                    </td>
                    <td>
                        <asp:dropdownlist id="ddlList" runat="server" width="224px"></asp:dropdownlist>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td class="style4">
            <table width="60%">
                <tr bgcolor ="#3399ff">
                    <td colspan="2" class="style2">
                        <h4>
                            Recipients
                        </h4>
                    </td>
                </tr>
                <tr>
                    <td class="style3">
                        To:
                    </td>
                    <td>
                        <asp:textbox id="txtTo" runat="server" style="margin-left: 1px" width="223px"></asp:textbox>
                    </td>
                </tr>
                <tr>
                    <td class="style3">
                        CC:
                    </td>
                    <td>
                        <asp:textbox id="txtCc" runat="server" width="223px"></asp:textbox>
                    </td>
                </tr>
                <tr>
                    <td class="style3">
                        BCC:
                    </td>
                    <td>
                        <asp:textbox id="txtBcc" runat="server" width="223px"></asp:textbox>
                    </td>
                </tr>
                <tr>
                    <td class="style3">
                        From:
                    </td>
                    <td>
                        <asp:textbox id="txtFrom" runat="server" width="222px"></asp:textbox>
                    </td>
                </tr>
            </table>
        </td>
        <td>
            <table width="60%">
                <tr bgcolor ="#3399ff">
                    <td>
                        <h4>
                            Event Type</h4>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:checkbox id="chkItemAdded" runat="server" text="Item Added" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:checkbox id="chkItemUpdated" runat="server" text="Item Updated" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:checkbox id="chkItemDeleted" runat="server" text="Item Deleted" />
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
    <td colspan ="2" style="margin-left: 400px">
        <asp:button ID="Button1" runat="server" text="Save"/>
        <asp:button ID="Button2" runat="server" text="Cancel"/>
    </td>
    </tr>
</table>





</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
Alert Settings
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
Alert Settings
</asp:Content>
