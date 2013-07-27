<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls"
    Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages"
    Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CCSCascadedLookupFieldRender.ascx.cs"
    Inherits="CrowCanyon.CascadedLookup.CONTROLTEMPLATES.CCSCascadedLookupFieldRender" %>
<SharePoint:RenderingTemplate ID="CCSCascadedLookupControl" runat="server">
    <Template>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
            <div id="cascadeControl" runat="server">
            </div>
            <div>
                <input id="hParentValue" type="hidden" value="" runat="server" />
                <input id="HiddenFieldType" type="hidden" value="" runat="server" />
                <asp:Panel ID="SingleValuePanel" runat="server">
                    <asp:DropDownList ID="ddlCCSCascadeFieldControl" runat="server" AppendDataBoundItems="true">
                    </asp:DropDownList>
                </asp:Panel>
                <asp:Panel runat="server" ID="MultipleValuePanel">
                    <table style="width: 570px">
                        <tr>
                            <td>
                                <asp:ListBox ID="lbLeftBox" runat="server" SelectionMode="Multiple" 
                                    Style="width: 205px; min-height: 125px; max-height: 150px;"></asp:ListBox>
                            </td>
                            <td style="vertical-align: middle; text-align: center; width: 150px; height: 125px;">
                                <asp:Button class="ms-ButtonHeightWidth" runat="server" ID="btnAdd" Text="Add >"></asp:Button>
                                <br />
                                <br />
                                <asp:Button class="ms-ButtonHeightWidth" runat="server" ID="btnRemove" Text="< Remove"></asp:Button>
                            </td>
                            <td>
                                <asp:ListBox ID="lbRightBox" runat="server" SelectionMode="Multiple" 
                                    Style="width: 205px; min-height: 125px; max-height: 150px;"></asp:ListBox>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
               <asp:Panel ID="NewEntryPanel" runat="server">
                    <asp:LinkButton runat="server" ID="lnkNewEntry" Text="New Entry" />
                    &nbsp;<asp:TextBox runat="server" ID="txtNewEntry" />
                    &nbsp;<asp:LinkButton runat="server" ID="lnkAdd" Text="Add" />
                    &nbsp;<asp:LinkButton ID="lnkCancel" runat="server" Text="Cancel" />
                </asp:Panel>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>                
    </Template>
</SharePoint:RenderingTemplate>
