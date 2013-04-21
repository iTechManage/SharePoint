<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls"
    Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PrintSettingsPage.aspx.cs"
    Inherits="CrowCanyonAdvancedPrint.Layouts.CrowCanyon.AdvancedPrint.PrintSettingsPage"
    DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
</asp:Content>
<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <script type="text/javascript">
        function ToggleSection(imgLink, hdnID, value) {
            var img = imgLink.firstChild;
            var tr = GetParentByTagName(imgLink, "tr");

            var section1 = GetSiblingByTagName(tr.nextSibling, "tr");
            var section2 = GetSiblingByTagName(tr.nextSibling.nextSibling, "tr");
            var section3 = GetSiblingByTagName(tr.nextSibling.nextSibling.nextSibling, "tr");
            var hiddenElement = document.getElementById(hdnID);
            var hide = value;
            if (hide == null) {
                hide = section1.style.display == "";
            }
            if (hide) {
                section1.style.display = "none";
                section2.style.display = "none";
                if (section3 != null) {
                    section3.style.display = "none";
                }
                img.src = "/_layouts/images/plus.gif";
                hiddenElement.value = "hide";
            }
            else {
                section1.style.display = "";
                section2.style.display = "";
                if (section3 != null) {
                    section3.style.display = "";
                }
                img.src = "/_layouts/images/minus.gif";
                hiddenElement.value = "show";
            }
        }

        function GetSiblingByTagName(el, tagName) {
            if (el.nodeType == 1 && el.tagName.toLowerCase() == tagName.toLowerCase())
                return el;

            if (el.nextSibling == null)
                return null;

            return GetSiblingByTagName(el.nextSibling, tagName);
        }

        function GetParentByTagName(el, tagName) {
            if (el.nodeType == 1 && el.tagName.toLowerCase() == tagName.toLowerCase())
                return el;
            if (el.parentNode == null)
                return null;
            return GetParentByTagName(el.parentNode, tagName);
        }
        function iwspp_addSection() {
            var txtSection = document.getElementById('TxtSection');
            var divSelected = document.getElementById('SelectedColumn');

            if (txtSection.value != "") {
                var option = new Option('- Section: ' + TxtSection.value + ' -');
                option.value = "[[Section:" + TxtSection.value + "]]";
                TooListBox.Items.Add(option);
                return false;
            }
        }


        //        function SetTabVisibility()
        //        {
        //            SetToggleSection("imgToLink","ctl00_PlaceHolderMain_HiddenToTab");
        //            SetToggleSection("imgSubLink","ctl00_PlaceHolderMain_HiddenSubTab");
        //            SetToggleSection("imgAdvLink","ctl00_PlaceHolderMain_HiddenAdvTab");
        //        }

        function SetToggleSection(linkID, hdnID) {
            var hdnEl = document.getElementById(hdnID);
            var hide = null;
            if (hdnEl.value != "")
                hide = hdnEl.value != "show";
            if (hide != null) {
                var imgLink = document.getElementById(linkID);
                ToggleSection(imgLink, hdnID, hide);
            }
        }
        //        window.onload=SetTabVisibility;
        window.setTimeout(function () {
            SetToggleSection("imgToLink", "ctl00_PlaceHolderMain_HiddenToTab");
            SetToggleSection("imgSubLink", "ctl00_PlaceHolderMain_HiddenSubTab");
            SetToggleSection("imgAdvLink", "ctl00_PlaceHolderMain_HiddenAdvTab");
        }, 300);
    </script>
    <table border="0" cellspacing="1" cellpadding="1" width="50%">
        <tr style="display: none">
            <td>
                <asp:HiddenField Value="show" ID="HiddenToTab" runat="server"></asp:HiddenField>
                <asp:HiddenField Value="hide" ID="HiddenSubTab" runat="server"></asp:HiddenField>
                <asp:HiddenField Value="hide" ID="HiddenAdvTab" runat="server"></asp:HiddenField>
            </td>
        </tr>
        <tr>
            <td class="ms-descriptiontext ms-inputformdescription">
                <asp:Label ID="MessageLabel" runat="server" ForeColor="Red"></asp:Label>
                <asp:Label ID="ErrorLabel" runat="server"></asp:Label><br />
                <br />
            </td>
        </tr>
        <tr>
            <td class="ms-linksectionheader" style="padding-right: 4px; padding-left: 4px; padding-bottom: 4px;
                padding-top: 4px" nowrap width="100%">
                <h3 class="ms-standardheader">
                    &nbsp;Print Templates
                </h3>
            </td>
        </tr>
        <tr>
            <td class="ms-descriptiontext">
                <SharePoint:SPGridView ID="gvTemplates" DataKeyNames="ID" runat="server" AutoGenerateColumns="false"
                    OnSelectedIndexChanged="gvActions_SelectedIndexChanged" OnRowDeleting="gvActions_RowDeleting"
                    EmptyDataRowStyle-CssClass="ms-vb" EmptyDataText="No data to show in this view.">
                    <Columns>
                        <asp:BoundField HeaderText="Template Name" ItemStyle-Width="100" HeaderStyle-CssClass="ms-vh2-nofilter"
                            DataField="Title" />
                        <asp:TemplateField HeaderStyle-CssClass="ms-vh2-nofilter">
                            <ItemTemplate>
                                <asp:LinkButton ID="lbDeleteAction" runat="server" CausesValidation="False" CommandName="Delete"
                                    Text="Delete" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:CommandField HeaderStyle-CssClass="ms-vh2-nofilter" SelectText="Edit" ButtonType="Link"
                            ShowCancelButton="false" ShowEditButton="false" ShowDeleteButton="false" ShowSelectButton="true" />
                    </Columns>
                </SharePoint:SPGridView>
            </td>
        </tr>
        <tr>
            <td class="ms-linksectionheader" style="padding-right: 4px; padding-left: 4px; padding-bottom: 4px;
                padding-top: 4px" nowrap width="100%">
                <h3 class="ms-standardheader">
                    &nbsp;Create/Edit Template
                </h3>
            </td>
        </tr>
        <tr>
        </tr>
        <tr>
        </tr>
        <tr>
            <td class="ms-linksectionheader" style="padding-right: 4px; padding-left: 4px; padding-bottom: 4px;
                padding-top: 4px" nowrap width="100%" colspan="2">
                <h3 class="ms-standardheader">
                    <a href="javascript:void(0)" id="imgToLink" onclick="ToggleSection(this,'ctl00_PlaceHolderMain_HiddenToTab');return false">
                        <img src="/_layouts/images/minus.gif" alt="" border="0" /></a>&nbsp;Print Settings
                </h3>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="Label1" runat="server" Text="&nbsp;&nbsp;Title:&nbsp;&nbsp;&nbsp;&nbsp;"></asp:Label>
                <asp:TextBox ID="TemplateTitle" runat="server"></asp:TextBox>
                </td>
                </tr>
        <tr id="TR1">
            <td class="ms-descriptiontext ms-inputformdescription" style="width: auto">
                Left side ListBox contains the available fields. Added fields in right side ListBox
                will be printed.<br />
            </td>
        </tr>
        <tr style="width: 100%" id="TR2">
            <td align="center">
                <br />
                <table border="0" cellspacing="0" cellpadding="0" width="70%">
                    <tr>
                        <td colspan="3" align="left">
                            <b>Select the Fields to Print:</b>
                            <br />
                            <br />
                        </td>
                    </tr>
                    <tr>
                        <th class="ms-vh2-nofilter" scope="col" nowrap>
                            Fields in List
                        </th>
                        <th class="ms-vh2-nofilter" scope="col" nowrap>
                        </th>
                        <th class="ms-vh2-nofilter" scope="col" nowrap>
                            Fields To be printed
                        </th>
                    </tr>
                    <tr>
                        <td style="width: 40%">
                            <asp:ListBox ID="AllFieldsListBox" Enabled="true" Rows="18" runat="server" Width="100%"
                                SelectionMode="Single"></asp:ListBox>
                            <br />
                        </td>
                        <td style="width: 10%; text-align: center">
                            <asp:ImageButton ID="AddImageButton" runat="server" ImageUrl="/_layouts/images/PLNEXT1.GIF"
                                OnClick="AddFieldToListBox" /><br />
                            <br />
                            <asp:ImageButton ID="RemoveImageButton" runat="server" ImageUrl="/_layouts/images/PLPREV1.GIF"
                                OnClick="RemoveFieldFromList" /><br />
                            <br />
                        </td>
                        <td style="width: 40%">
                            <asp:ListBox ID="TooListBox" Enabled="true" Rows="18" runat="server" Width="100%"
                                SelectionMode="Single" />
                            <br />
                        </td>
                        </tr>
                         </table>
                         <table align="left">
                        <tr>
                        <td></td><td></td><td></td><td></td> <td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td> <td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td>
                        <td class="ms-descriptiontext">
                 Section:
            </td>
           <td>
   
          <asp:TextBox style="Width:200%" ID="TxtSection" Enabled="true" runat="server" ></asp:TextBox>
          </td>
          <td></td><td></td><td></td> <td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td>
          <td>
          <asp:Button ID="BtnSection" style="width:100%" OnClick="AddSectionField" Enabled="true" runat="server" Text="Add" />
          </td>

            </tr>
            </table>
               
                <tr>
                    <td class="ms-linksectionheader" style="padding-right: 4px; padding-left: 4px; padding-bottom: 4px;
                        padding-top: 4px" nowrap width="100%" colspan="2">
                        <h3 class="ms-standardheader">
                            <a href="javascript:void(0)" id="A1" onclick="ToggleSection(this,'ctl00_PlaceHolderMain_HiddenToTab');return false">
                                <img src="/_layouts/images/minus.gif" alt="" border="0" /></a>&nbsp;Display
                        </h3>
                    </td>
                </tr>
                <tr style="width: 100%" id="TR3">
            <td align="center">
                <br />
                <table border="0" cellspacing="0" cellpadding="0" width="70%">
                <tr>              
                <td align="left">
                <asp:Label ID="Label2" runat="server" Text="Header:&nbsp;&nbsp;"></asp:Label>
                <br />
                    </td>
                    
                    <tr>
                    <td align="center">
                        <SharePoint:InputFormTextBox ID="RichtextBox" runat="server" TextMode="MultiLine"
                            RichTextMode="FullHtml" RichText="true" Rows="10" Width="100%" />
                    </td>
                    </tr> 
                    <td align="left">
                <asp:Label ID="Label3" runat="server" Text="Footer:&nbsp;&nbsp;"></asp:Label><br />
                    </td>
                    <tr>
                    <td align="center">
                        <SharePoint:InputFormTextBox ID="RichtextBox2" runat="server" TextMode="MultiLine"
                            RichTextMode="FullHtml" RichText="true" Rows="10" Width="100%" />
                    </td>
                    </tr>
                    </tr>
                    </tr>
                    </table>
                    
                <tr>
                    <td align="Middle">
                        <table>
                            <tr>
                                <td>
                                    <asp:Button ID="SaveButton" Text="Save" Enabled="true" CssClass="ms-ButtonHeightWidth"
                                        OnClick="SaveButton_Clicked" runat="server" />
                                    <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="ms-ButtonHeightWidth"
                                        OnClick="btnCancel_Click" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="right">
                        <table>
                            <tr>
                                <td>
                                    <SharePoint:GoBackButton runat="server" ID="GoBackButton" ControlMode="Display" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <table border="0" cellspacing="1" cellpadding="1" width="50%">
                    <tr>
                        <td class="ms-linksectionheader ms-descriptiontext ms-inputformdescription" style="padding-right: 4px;
                            padding-left: 4px; padding-bottom: 4px; padding-top: 4px" nowrap width="100%"
                            align="right">
                            <h3 class="ms-standardheader" style="text-align: right">
                                Developed by <a href="http://www.sharepoint-applications.com" target="_blank">CrowCanyon</a>
                            </h3>
                        </td>
                    </tr>
                </table>
    </table>
</asp:Content>
<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    CrowCanyon Print Settings Page
</asp:Content>
<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea"
    runat="server">
    CrowCanyon Print Settings Page
</asp:Content>
