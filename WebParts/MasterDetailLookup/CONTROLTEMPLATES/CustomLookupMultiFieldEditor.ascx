<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CustomLookupMultiFieldEditor.ascx.cs" Inherits="CustomLookupField.CONTROLTEMPLATES.CustomLookupMultiFieldEditor" %>
<SharePoint:RenderingTemplate ID="MultipleCustomDropDownListControl" runat="server">
  <Template>
  <div style="width: 100%; text-align: left; border-width: 0px;">
          <table style="width: 100%; border-width: 0px; border-collapse: collapse;" cellpadding="0" cellspacing="0">
          <tr>
                <td class="ms-authoringcontrols" style="width: 34%; text-align: left; white-space: nowrap;">
                  <asp:ListBox ID="LeftBox" runat="server" SelectionMode="Multiple" style="width: 100%;"></asp:ListBox>
                </td>
                <td class="ms-authoringcontrols" style="width: 32%; text-align: center; white-space: nowrap;">
                <asp:Button Runat="server" ID="AddButton" Text="Add" style="width: 95%;"></asp:Button>
                <br />
                <br />
                <asp:Button Runat="server" ID="RemoveButton" Text="Remove" style="width: 95%;"></asp:Button>
                </td>
                <td class="ms-authoringcontrols" style="width: 33%; text-align: left; white-space: nowrap;">
                  <asp:ListBox ID="RightBox" runat="server" SelectionMode="Multiple" style="width: 100%;"></asp:ListBox>
                </td>
           </tr>
          </table>
          </div>
    
  </Template>
</SharePoint:RenderingTemplate>

