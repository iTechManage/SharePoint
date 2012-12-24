<%@ Assembly Name="CustomLookupField, Version=1.0.0.0, Culture=neutral, PublicKeyToken=9b7a99f1f2e462c2" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CustomLookupFieldEditor.ascx.cs" Inherits="CustomLookupField.CONTROLTEMPLATES.CustomLookupFieldEditor" %>
<%@ Register TagPrefix="wssuc" TagName="InputFormControl" src="~/_controltemplates/InputFormControl.ascx" %>
<%@ Register TagPrefix="wssuc" TagName="InputFormSection" src="~/_controltemplates/InputFormSection.ascx" %>
<wssuc:InputFormSection runat="server" id="FilterLookupFieldSection" Title="Special Column Settings">
  <template_inputformcontrols>
    <wssuc:InputFormControl ID="InputFormControl1" runat="server" LabelText="Specify detailed options for the filtered lookup column">
      <Template_Control>
          <div style="width: 100%; text-align: left; border-width: 0px;">
          <table style="width: 100%; border-width: 0px; border-collapse: collapse;" cellpadding="0" cellspacing="0">
              <tr>
                <td class="ms-authoringcontrols" style="width: 100%; text-align: left; white-space: nowrap; padding-top: 10px;">
                  <span>Get Information from this site:</span>
                </td>
              </tr>
              <tr>
                <td class="ms-authoringcontrols" style="width: 100%; text-align: left; white-space: nowrap;">
                  <asp:DropDownList runat="server" ID="listTargetWeb" AutoPostBack="true" OnSelectedIndexChanged="SelectedTargetWebChanged" />
                </td>
              </tr>
              <tr>
                <td class="ms-authoringcontrols" style="width: 100%; text-align: left; white-space: nowrap; padding-top: 10px;">
                  <span>Get Information from:</span>
                </td>
              </tr>
              <tr>
                <td class="ms-authoringcontrols" style="width: 100%; text-align: left; white-space: nowrap;">
                  <asp:DropDownList runat="server" ID="listTargetList" AutoPostBack="true" OnSelectedIndexChanged="SelectedTargetListChanged" />
                </td>
              </tr>
              <tr>
                <td class="ms-authoringcontrols" style="width: 100%; text-align: left; white-space: nowrap; padding-top: 10px;">
                  <span>In this column</span>
                </td>
              </tr>
              <tr>
                <td class="ms-authoringcontrols" style="width: 100%; text-align: left; white-space: nowrap;">
                  <asp:DropDownList runat="server" ID="listTargetColumn" />
                </td>
              </tr>
              <tr>
                <td class="ms-authoringcontrols" style="width: 100%; text-align: left; white-space: nowrap;">
                  <asp:CheckBox runat="server" ID="cbxLinkParent" Text="Link to parent column" Checked="true" OnCheckedChanged="LinkParentColumnChanged" AutoPostBack="true" />
                </td>
              </tr>
              <tr>
                <td class="ms-authoringcontrols" style="width: 100%; text-align: left; white-space: nowrap;">
                  <asp:CheckBox runat="server" ID="cbxParentEmpty" Text="Show all values when parent is empty" Checked="false"/>
                </td>
              </tr>
              <tr>
                <td class="ms-authoringcontrols" style="width: 100%; text-align: left; white-space: nowrap; padding-top: 10px;">
                  <asp:Label runat="server" ID="lblParentColumn" Text="Parent Column:" />
                </td>
              </tr>
              <tr>
                <td class="ms-authoringcontrols" style="width: 100%; text-align: left; white-space: nowrap;">
                  <asp:DropDownList runat="server" ID="listParentColumn" AutoPostBack="true" OnSelectedIndexChanged="SelectedParentColumnChanged"/>
                </td>
              </tr>
              <tr>
                <td class="ms-authoringcontrols" style="width: 100%; text-align: left; white-space: nowrap; padding-top: 10px;">
                  <asp:Label runat="server" ID="lblLinkColumn" Text="Link Column:" />
                </td>
              </tr>
              <tr>
                <td class="ms-authoringcontrols" style="width: 100%; text-align: left; white-space: nowrap;">
                  <asp:DropDownList runat="server" ID="listLinkColumn" />
                  <asp:Label runat="server" ID="lbllistLinkColumn" Text="Source list must contain lookup column for the parent list" />
                </td>
              </tr>
              <tr>
                <td class="ms-authoringcontrols" style="width: 100%; text-align: left; white-space: nowrap;">
                  <asp:CheckBox runat="server" ID="cbxMultipleValues" Text="Allow Multiple Values" Checked="false" OnCheckedChanged="Allow_multiple_values_changed" AutoPostBack="true"/>
                </td>
              </tr>
              <tr>
                <td class="ms-authoringcontrols" style="width: 100%; text-align: left; white-space: nowrap;">
                  <asp:CheckBox runat="server" ID="cbxAutoCompleteORFilter" Text="Allow auto-complete or filter" Checked="false" Visible="false"/>
                </td>
              </tr>
              <tr>
                <td colspan="2" class="ms-authoringcontrols" style="width: 100%; text-align: left; white-space: nowrap;">
                 <b><asp:CheckBox runat="server" ID="cbxAdvanceSettings" Text="Advance Settings" Checked="false" OnCheckedChanged="OptedforAdvanceSettings" AutoPostBack="true"/></b>
                </td>
              </tr>
              <tr>  
                <td class="ms-authoringcontrols" style="width: 100%; text-align: left; white-space: nowrap;">
                  <asp:CheckBoxList ID="cblAdditionalFields" runat="server" CssClass="ms-authoringcontrols"  Visible="false" /><br />
                </td>
              </tr>
              <tr>
                <td class="ms-authoringcontrols" style="width: 100%; text-align: left; white-space: nowrap;">
                  <asp:Label ID="lbAdditionalFilters" Text="Additional Filters" runat="server" Visible="false"/><br />
                  <asp:CheckBoxList ID="cblAdditionalFilters" runat="server" CssClass="ms-authoringcontrols"  Visible="false" /><br />
                </td>
              </tr>
              <tr>
                <td>
                <asp:Label ID="lbView" Text="View:" runat="server" Visible="false"/><br />
			    <asp:DropDownList id="ddlView" runat="server" Visible="false" OnSelectedIndexChanged="SelectedViewChanged" AutoPostBack ="true"/>
                <br />
                </td>
               </tr>
               <tr>
                <td>
                <asp:CheckBox ID="chkSortByView" runat="server" Text="SortByView" Enabled = "false" Visible="false" Checked="false"/>
                <br />
                </td>
              </tr>
              <tr>
                <td>
                <asp:CheckBox ID="chkAddingNewValues" runat="server" Text="Allow Adding New Values" Checked="false" OnCheckedChanged="SelectedAddNewValues" Visible="false" AutoPostBack="true" />
                </td>
              </tr>
              <tr>
                <td>
                <asp:CheckBox ID="chkUseNewForm" runat="server" Text="Use NEW Form" Enabled = "false" Visible="false" Checked="false"/>
                </td>
              </tr>          
          </table>
          <asp:Panel ID="pnlConvertFromRegular" runat="server" Visible="false">
             <table>
                 <tr>
                    <td class="ms-authoringcontrols" nowrap="nowrap"><asp:Label ID="lblConvertFromRegular" runat="server" Text="Convert from:" /></td>
                    <td class="ms-authoringcontrols"><asp:DropDownList ID="ddlConvertFromRegular" runat="server" /></td>
                    <td class="ms-authoringcontrols"><asp:Button ID="btnConvertFromRegular" runat="server" Text="Convert" CssClass="ms-ButtonHeightWidth" ValidationGroup="cfconvert" /></td>
                 </tr>
             </table>
             </asp:Panel>
          </div>
        </Template_Control>
    </wssuc:InputFormControl>
   </template_inputformcontrols>
</wssuc:InputFormSection>
<wssuc:InputFormSection runat="server" id="Relationship" Title="Relationship">
  <template_inputformcontrols>
    <wssuc:InputFormControl ID="InputFormControl2" runat="server" >
      <Template_Control>
          <div style="width: 100%; text-align: left; border-width: 0px;">
          <table style="width: 100%; border-width: 0px; border-collapse: collapse;" cellpadding="0" cellspacing="0">
              <tr>
                <td class="ms-authoringcontrols" style="width: 100%; text-align: left; white-space: nowrap;">
                  <asp:CheckBox runat="server" ID="cbxRelationshipBehavior" Text="Enforce Relationship Behavior" OnCheckedChanged="Relationship_behavior_changed" AutoPostBack="true"/>
                </td>
              </tr>
              <tr>
                <td class="ms-authoringcontrols" style="width: 100%; text-align: left; white-space: nowrap;">
                  <asp:RadioButton runat="server" ID="rdbRestrictDelete" Text="Restrict Delete" Checked="true" Enabled="false" OnCheckedChanged="Restrict_delete_behavior_changed" AutoPostBack="true"/>
                </td>
              </tr>
              <tr>
                <td class="ms-authoringcontrols" style="width: 100%; text-align: left; white-space: nowrap;">
                  <asp:RadioButton runat="server" ID="rdbCascadeDelete" Text="Cascade Delete" Enabled="false"  OnCheckedChanged="Cascade_delete_behavior_changed" AutoPostBack="true"/>
                </td>
              </tr>
          </table>
          </div>
        </Template_Control>
    </wssuc:InputFormControl>
   </template_inputformcontrols>
</wssuc:InputFormSection>

<SharePoint:RenderingTemplate ID="CustomDropDownListControl" runat="server">
<Template>
<asp:UpdatePanel ID="UpdatePanel1" runat="server" >
<ContentTemplate><asp:TextBox ID="auto_complete" runat="server" Visible="false">
</asp:TextBox>
<asp:DropDownList ID="customList" runat="server" AppendDataBoundItems="true" Visible="false">
</asp:DropDownList>
    <table style="width: 570px; border-width: 0px; border-collapse: collapse;" cellpadding="0" cellspacing="0">
        <tr>
        <td style="width=210px">
            <asp:ListBox ID="LeftBox" runat="server" SelectionMode="Multiple" Visible="false" style="width: 205px; min-height: 125px; max-height: 150px;"></asp:ListBox>
        </td>
        <td style="padding-left: 10px">
                    </td>
        <td align="center" valign="middle" style="width=150px;">
            <asp:Button class="ms-ButtonHeightWidth"  Runat="server" ID="AddButton" Text="Add >" Visible="false"></asp:Button>
            <br />
            <br />
            <asp:Button class="ms-ButtonHeightWidth" Runat="server" ID="RemoveButton" Text="< Remove" Visible="false"></asp:Button>
        </td>
        <td style="padding-left: 10px">
                    </td>
        <td style="width=210px;">
            <asp:ListBox ID="RightBox" runat="server" SelectionMode="Multiple" Visible="false" style="width: 205px;  min-height: 125px; max-height: 150px;"></asp:ListBox>
        </td>
        </tr>
    </table>
<asp:LinkButton runat="server" ID="lbAddNew" Text="New Element" Visible="false"/> 
<asp:TextBox runat="server" ID="txtNewEntry" Visible ="false" />
<asp:LinkButton runat="server" ID="lbAddEntry" Text="Add" Visible ="false"/>
<asp:LinkButton ID="lbCancel" runat="server" Text="Cancel" Visible="false"/>
    </ContentTemplate></asp:UpdatePanel>
</Template>
</SharePoint:RenderingTemplate>
