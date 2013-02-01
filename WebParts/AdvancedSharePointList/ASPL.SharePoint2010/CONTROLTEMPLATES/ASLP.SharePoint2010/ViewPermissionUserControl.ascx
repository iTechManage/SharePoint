<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewPermissionUserControl.ascx.cs" Inherits="ASPL.SharePoint2010.CONTROLTEMPLATES.ASLP.SharePoint2010.ViewPermissionUSerControl" %>
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
            text-align: justify;
            background-color: transparent;
            font-family: Tahoma;
            font-size: 8pt;
            padding: 3px 8px 0px;
        }

    </style>
    <script type="text/javascript">
        function ClickAction(buttonID, rowID, hidControlRowID) {
            document.getElementById(hidControlRowID).value = rowID;
            document.getElementById(buttonID).click();
        }

        function SetDefaultAction(buttonID, rowID, hidControlRowID, accountID, hidControlAccountID) {
            document.getElementById(hidControlRowID).value = rowID;
            document.getElementById(hidControlAccountID).value = accountID;
            document.getElementById(buttonID).click();
        }

        function SetEnableState(chkActionsID, chk1ID, chk2ID, chk3ID) {
            var chkActions = document.getElementById(chkActionsID);
            var chk1 = document.getElementById(chk1ID);
            var chk2 = document.getElementById(chk2ID);
            var chk3 = document.getElementById(chk3ID);

            chk1.disabled = !chkActions.checked;
            chk2.disabled = !chkActions.checked;
            chk3.disabled = !chkActions.checked;
        }
    </script>
<%-- <IW:LiteWarning runat="server" />--%>

    <table style="width: 600px">
        <tr>
            <td class="ms-linksectionheader" style="padding-right: 4px; padding-left: 4px; padding-bottom: 4px;
                padding-top: 4px;white-space:nowrap" nowrap="nowrap" width="100%" colspan="2">
                <h3 class="ms-standardheader">
                    <asp:Literal ID="Literal1" 
                        Text="Views Permissions" runat="server" />
                </h3>
            </td>
        </tr>
        <tr>
            <td class="iw-SectionTitle" style="width: 160px">
                <asp:Literal ID="Literal6" Text="Views"
                    runat="server" />:
            </td>
            <td class="iw-Results">
                <asp:ListBox runat="server" ID="ddlViews" CssClass="iw-DropDown" Width="250px" Height="100px" SelectionMode="Multiple">
                </asp:ListBox>
            </td>
        </tr>
        <tr>
            <td class="iw-SectionTitle" style="width: 160px">
                <asp:Literal ID="Literal2" Text="People and Groups:" runat="server" />:
            </td>
            <td>
                <table cellpadding="0" cellspacing="0" border="0">
                    <tr>
                        <td class="iw-Results">
                            <SharePoint:PeopleEditor runat="server" ID="peSelectUsers" MultiSelect="true"
                                SelectionSet="User,SPGroup,SecGroup" AllowEmpty="false" Width="300px" AllowTypeIn="true" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td class="iw-SectionTitle" style="width: 160px">
                &nbsp;</td>
            <td>
                &nbsp;</td>
        </tr>
        <tr>
            <td class="iw-SectionTitle" style="width: 160px">
                <span id="ctl00_PlaceHolderMain_Label7" class="ms-formtoolbar ms-bold">
                Permission type:</span></td>
            <td>
                <asp:RadioButtonList ID="rdoViewPermission" runat="server" 
                    RepeatDirection="Horizontal">
                    <asp:ListItem Selected="True">show</asp:ListItem>
                    <asp:ListItem>hide</asp:ListItem>
                </asp:RadioButtonList>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Button ID="cmdAddView" runat="server" Text="Add View Permissions"
                    Width="100%" onclick="cmdAddView_Click"  />
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <%--<asp:Button runat="server" ID="cmdUpdateExistingRule" Text="Update" 
                    OnClick="cmdUpdateExistingRule_Click" Width="100%" Visible="False" />--%>
            </td>
        </tr>

        <tr>
            <td colspan="2">
                <span class="ms-error"><asp:Literal runat="server" ID="lblError" EnableViewState="false"></asp:Literal></span>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <%--<asp:Label runat="server" ID="lblInfoCheck" EnableViewState="false" CssClass="iw-SectionTitle"  Text="<%$Resources:CustomListResources,InfoCheck %>"></asp:Label>--%>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <%--<asp:Table ID="tbResults" runat="server" style="border-collapse: collapse; " CellSpacing="0"
                    CellPadding="1" Width="100%" BorderWidth="0">
                    <asp:TableHeaderRow>
                        <asp:TableHeaderCell CssClass="ms-vh2-nofilter" Scope="Column" Wrap="false" Width="10%">
                            <asp:Literal ID="Literal3" runat="server" Text="<%$Resources:CustomListResources,View %>" />
                        </asp:TableHeaderCell><asp:TableHeaderCell CssClass="ms-vh2-nofilter" Scope="Column" Wrap="false" Width="3%">
                            <asp:Literal ID="Literal4" runat="server" Text="<%$Resources:CustomListResources,UsGpCol %>" />
                        </asp:TableHeaderCell><asp:TableHeaderCell CssClass="ms-vh2-nofilter" style="text-align: right" Scope="Column" Wrap="false" Width="3%">
                        </asp:TableHeaderCell><asp:TableHeaderCell CssClass="ms-vh2-nofilter" style="text-align: right" Scope="Column" Wrap="false" Width="3%">
                        </asp:TableHeaderCell></asp:TableHeaderRow></asp:Table>--%>
                 <asp:GridView ID="grdView" runat="server" AutoGenerateColumns="False" CellPadding="4"
                                                EnableModelValidation="True" ForeColor="#333333" GridLines="None" OnRowCommand="grdTabView_RowCommand"
                                                OnRowDeleting="grdTabView_RowDeleting" EmptyDataRowText="No View Data Selected">
                                                <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                                                <Columns>
                                                    <asp:BoundField DataField="rowid" HeaderText="rowid" Visible="False" />
                                                    <asp:BoundField DataField="View" HeaderText="Views" >
                                                    <ItemStyle Width="100px" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="Permission" HeaderText="Visibility" />
                                                    <asp:BoundField DataField="UserGroup" HeaderText="User\Group">
                                                        <ItemStyle Width="100px" />
                                                    </asp:BoundField>
                                                    <asp:CommandField ButtonType="Image" ShowDeleteButton="True" DeleteImageUrl="~/_layouts/ASPL.SharePoint2010/Resource/Delete.bmp" />
                                                    <asp:TemplateField>
                                                        <ItemTemplate>
                                                            <asp:ImageButton ID="lnkvendorname" runat="server" ImageUrl="~/_layouts/ASPL.SharePoint2010/Resource/Select16.bmp"
                                                                CommandArgument='<%#Eval("rowid")%>' CommandName="EditData" />
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
                        </td></tr><asp:PlaceHolder runat="server" ID="phMenuSettings">
        <tr>
            <td colspan="2" class="iw-Results">
                <asp:CheckBox runat="server" ID="chkShowActionsMenu" Text="Display Action Menu" Checked="true" />
            </td>
        </tr>
        <tr>
            <td colspan="2" class="iw-Results">
                <asp:CheckBox runat="server" ID="chkDisplayOpenWithAccess" Text="Display Edit in Datasheet" Checked="true"/>
            </td>
        </tr>
        <tr>
            <td colspan="2" class="iw-Results">
                <asp:CheckBox runat="server" ID="chkDisplayRSS" Text="Display View RSS feed" Checked="true"/>
            </td>
        </tr>
        <tr>
            <td colspan="2" class="iw-Results">
                <asp:CheckBox runat="server" ID="chkDisplayAlertMe" Text="Display Alert me" Checked="true"/>
            </td>
        </tr>
        </asp:PlaceHolder>
        
        <tr>
            <td colspan="2">
                <table width="100%">
                    <tr>
                        <td id="tdIWOKCancelButtons">
                            <asp:Button ID="cmdOK" runat="server" Text="OK"
                                OnClick="cmdOK_Click" CssClass="ms-ButtonHeightWidth" 
                                CausesValidation="false" />
                            <asp:Button ID="Cancel" runat="server" Text="Cancel" CssClass="ms-ButtonHeightWidth"
                                CausesValidation="false" OnClick="Cancel_Click" />
                        </td>
                        <td id="tdIWDevBy" class="ms-propertysheet">
                            &nbsp;</td>
                        <td id="tdIWDevBy" class="ms-propertysheet">
                            <br />
                        </td>                    
                    </tr>
                </table>
            </td>
        </tr>        
    </table>
    <asp:HiddenField runat="server" ID="hidActionRow" Value="-1" />
    <asp:HiddenField runat="server" ID="hidEditedRule" Value="-1" />
    <asp:HiddenField runat="server" ID="hidAccountID" Value="-1" />
    <asp:Button runat="server" ID="cmdDeleteCommand" style="display:none;"  CausesValidation="false" />
    <asp:Button runat="server" ID="cmdEditCommand" style="display:none;"  CausesValidation="false" />
    <asp:Button runat="server" ID="cmdSetDefault" style="display:none;"  CausesValidation="false" />
    <asp:Literal runat="server" ID="litScripts"></asp:Literal>
    <script type="text/javascript">
//        document.getElementById("tdIWOKCancelButtons").align = languageAlign;
//        document.getElementById("tdIWOKCancelButtons").style.textAlign = languageAlign;
//        document.getElementById("tdIWDevBy").align = languageOAlign;
//        document.getElementById("tdIWDevBy").style.textAlign = languageOAlign;
    </script>