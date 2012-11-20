<%@ Assembly Name="CCSAdvancedAlerts, Version=1.0.0.0, Culture=neutral, PublicKeyToken=d0e8c94870369eea" %>
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
    <table width="100%" border="2">
        <tr>
            <td>
                <table width="100%">
                    <tr bgcolor="#3399ff">
                        <td colspan="2">
                            <h4>
                                General Settings
                            </h4>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Title:
                        </td>
                        <td>
                            <asp:TextBox ID="txtTitle" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Site:
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlSite" runat="server" AutoPostBack="true">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            List:
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlList" runat="server" AutoPostBack ="True">
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                <table width="100%">
                    <tr bgcolor="#3399ff">
                        <td>
                            <h4>
                                Event Type
                            </h4>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:CheckBox ID="chkItemAdded" runat="server" Text="Item Added" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:CheckBox ID="chkItemUpdated" runat="server" Text="Item Updated" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:CheckBox ID="chkItemDeleted" runat="server" Text="Item Deleted" />
                        </td>
                    </tr>
                    <tr>
                        <td>
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
            <td>
                <table width = "50%">
                    <tr bgcolor="#3399ff">
                        <td colspan="2">
                            <h4>
                                Recipients
                            </h4>
                        </td>
                    </tr>
                    <tr>
                        <td width= "60px">
                            From:
                        </td>
                        <td align="left" >
                            <asp:TextBox ID="txtFrom" runat="server" Rows="2" Width="500px" TextMode="MultiLine"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td width= "60px">
                            To:
                        </td>
                        <td>
                            <asp:TextBox ID="txtTo" runat="server" Rows="2" Width="500px" TextMode="MultiLine"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td width= "60px">
                            CC:
                        </td>
                        <td>
                            <asp:TextBox ID="txtCc" runat="server" Rows="2" Width="500px" TextMode="MultiLine"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td width= "60px">
                            BCC:
                        </td>
                        <td>
                            <asp:TextBox ID="txtBcc" runat="server" Rows="2" Width="500px" TextMode="MultiLine"></asp:TextBox>
                        </td>
                    </tr>
                    <tr bgcolor="#c0c0c0">
                        <td colspan="2">
                            <table>
                                <tr>
                                    <td>
                                        Add To Recipients
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:RadioButton ID="rdCurrentUser" Text="Current User" runat="server" GroupName="rgpAddToRecipients"></asp:RadioButton>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:RadioButton ID="rdUsers" Text="Users" runat="server" GroupName="rgpAddToRecipients"></asp:RadioButton>
                                        <SharePoint:PeopleEditor ID="additionalUsers" MultiSelect="true" runat="server" PlaceButtonsUnderEntityEditor="false" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:RadioButton ID="rdUsersincolumn" Text="Users in column" runat="server" GroupName="rgpAddToRecipients"></asp:RadioButton>
                                        <asp:DropDownList ID="ddlUsersInColumn" runat="server"></asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:RadioButton ID="rdEmailAddresses" Text="E-mail addresses" runat="server" GroupName="rgpAddToRecipients"></asp:RadioButton>
                                        <asp:TextBox runat="server" ID="txtEmailAddresses" Rows="2" TextMode="MultiLine"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Button ID="btnAddTO" runat="server" Text="Add to To" />
                                        <asp:Button ID="btnAddCC" runat="server" Text="Add to CC" />
                                        <asp:Button ID="btnAddBCC" runat="server" Text="Add to Bcc" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Never send to:
                            <SharePoint:PeopleEditor ID="ppleNeverSendTo" MultiSelect="true" runat="server" PlaceButtonsUnderEntityEditor="false" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>



        <table width="100%">
            <tr bgcolor="#3399ff">
                <td>
                    <h4>
                When To Send
            </tr>
            <tr>
                <td>
                    <asp:RadioButton ID="rdImmediately" runat="server" Text="Immediately"></asp:RadioButton>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:RadioButton ID="rdDaily" runat="server" Text="Daily"></asp:RadioButton>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:RadioButton ID="rdWeekly" runat="server" Text="Weekly"></asp:RadioButton>
                </td>
            </tr>
        </table>
        
        <tr>
              <table width="100%">
                <tr bgcolor="#3399ff">
                    <td >
                        <h4>
                            Mail Templates
                        </h4>
                    </td>
                </tr>

               <tr bgcolor="#c0c0c0">
                  <td>
                        <table width="60%">
           
                          <tr>
                             <td width= "60px">
                             Name:
                            </td>
                           <td>
                             <asp:textbox id="txtMailTemplateName" width="400px" runat="server"></asp:textbox>
                            </td>
                         </tr>


                          <tr>
                              <td "60px">
                                Subject:
                              </td>
                             <td>
                              <asp:textbox id="txtMailSubject" width="400px" runat="server"></asp:textbox>
                             </td>
                         </tr>
           
           
                        <tr>
                            <td "60px">
                                Mail Body:
                            </td>
                            <td>
                                   <SharePoint:InputFormTextBox ID="BodyTextBox" runat="server" TextMode="MultiLine"
                                            RichTextMode="FullHtml" RichText="true" Rows="10" Width="400%" />
                            </td>
                        </tr>

                        <tr>
                            <td colspan="2"></td>
                        </tr>
                
                        <tr>
                            <td colspan="2">
                                <asp:checkbox id="chkIncludeUpdatedColumns" text="Include updated columns" runat="server"></asp:checkbox>
                            </td>
                        </tr>

                        <tr>
                            <td colspan="2">
                                <asp:checkbox id="chkHighlightUpdatedColumns"  text="Highlight updated columns" runat="server"></asp:checkbox>
                            </td>
                        </tr>

                        <tr>
                            <td colspan="2">
                                <asp:checkbox id="chkInsertAttachments"  text="Insert attachments" runat="server"></asp:checkbox>
                            </td>
                        </tr>


                        <tr>
                            <td colspan="2">
                                <asp:button id= "btnTemplateAdd" runat="server" text="Add" width="20%"/>
                                <asp:button id= "btnTemplateUpdate" runat="server" text="Update" width="20%"/>
                                <asp:button id= "btnTemplateCancel"  runat="server" text="Cancel" width="20%"/>
                            </td>
                        </tr>

                 </table>
                </td>
               </tr>
            </table>
        </tr>




        <tr>
            <td style="margin-left: 400px" class="style6">
                <asp:Button ID="btnsave" runat="server" Text="Save" />
                <asp:Button ID="btncancel" runat="server" Text="Cancel" />
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
