<%@ Control Language="C#" AutoEventWireup="false" %>
<%@ Assembly Name="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c"
    Namespace="Microsoft.SharePoint.WebControls" %>
<%@ Register TagPrefix="ApplicationPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c"
    Namespace="Microsoft.SharePoint.ApplicationPages.WebControls" %>
<%@ Register TagPrefix="SPHttpUtility" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c"
    Namespace="Microsoft.SharePoint.Utilities" %>
<%@ Register TagPrefix="wssuc" TagName="ToolBar" Src="~/_controltemplates/ToolBar.ascx" %>
<%@ Register TagPrefix="wssuc" TagName="ToolBarButton" Src="~/_controltemplates/ToolBarButton.ascx" %>
<%@ Register TagPrefix="ASPL" Assembly="$SharePoint.Project.AssemblyFullName$" Namespace="ASPL.SharePoint2010.Core" %>



<SharePoint:RenderingTemplate ID="ListForm" runat="server">
    <Template>
        <span id='part1'>
            <SharePoint:InformationBar ID="InformationBar1" runat="server" />
            <div id="listFormToolBarTop">
                <wssuc:ToolBar CssClass="ms-formtoolbar" ID="toolBarTbltop" RightButtonSeparator="&amp;#160;"
                    runat="server">
                    <Template_RightButtons>
                        <SharePoint:NextPageButton ID="NextPageButton1" runat="server" />
                        <SharePoint:SaveButton ID="SaveButton1" runat="server" />
                        <SharePoint:GoBackButton ID="GoBackButton1" runat="server" />
                    </Template_RightButtons>
                </wssuc:ToolBar>
            </div>
            <SharePoint:FormToolBar ID="FormToolBar1" runat="server" />
            <SharePoint:ItemValidationFailedMessage ID="ItemValidationFailedMessage1" runat="server" />
             <table id="TableTabCtrl" cellspacing="0" cellpadding="0" style="border-collapse:collapse;" >
                <SharePoint:ChangeContentType ID="ChangeContentType1" runat="server" />
                <SharePoint:FolderFormFields ID="FolderFormFields1" runat="server" />
                <!--  initiated our iterator-->
                <ASPL:ASPLListFieldIterator ID="ASPLListFieldIterator" runat="server" />
                <!--  end:initiated our iterator-->
                <SharePoint:ApprovalStatus ID="ApprovalStatus1" runat="server" />
                <SharePoint:FormComponent ID="FormComponent1" TemplateName="AttachmentRows" runat="server" />
            </table>
            <table cellpadding="0" cellspacing="0" width="100%">
                <tr>
                    <td class="ms-formline">
                        <img src="/_layouts/images/blank.gif" width='1' height='1' alt="" />
                    </td>
                </tr>
            </table>
            <table cellpadding="0" cellspacing="0" width="100%" style="padding-top: 7px">
                <tr>
                    <td width="100%">
                        <SharePoint:ItemHiddenVersion ID="ItemHiddenVersion1" runat="server" />
                        <SharePoint:ParentInformationField ID="ParentInformationField1" runat="server" />
                        <SharePoint:InitContentType ID="InitContentType1" runat="server" />
                        <wssuc:ToolBar CssClass="ms-formtoolbar" ID="toolBarTbl" RightButtonSeparator="&amp;#160;"
                            runat="server">
                            <Template_Buttons>
                                <SharePoint:CreatedModifiedInfo ID="CreatedModifiedInfo1" runat="server" />
                            </Template_Buttons>
                            <Template_RightButtons>
                                <SharePoint:SaveButton ID="SaveButton2" runat="server" />
                                <SharePoint:GoBackButton ID="GoBackButton2" runat="server" />
                            </Template_RightButtons>
                        </wssuc:ToolBar>
                    </td>
                </tr>
            </table>
        </span>
        <SharePoint:AttachmentUpload ID="AttachmentUpload1" runat="server" />
    </Template>
</SharePoint:RenderingTemplate>
<SharePoint:RenderingTemplate ID="ViewSelector" runat="server">
    <Template>
        <table border="0" cellpadding="0" cellspacing="0" style='margin-right: 4px'>
            <tr>
                <td nowrap="nowrap" class="ms-listheaderlabel">
                    <SharePoint:EncodedLiteral ID="EncodedLiteral1" runat="server" Text="<%$Resources:wss,view_selector_view%>"
                        EncodeMethod='HtmlEncode' />&#160;
                </td>
                <td nowrap="nowrap" class="ms-viewselector" id="onetViewSelector" onmouseover="this.className='ms-viewselectorhover'"
                    onmouseout="this.className='ms-viewselector'" runat="server">
                    <ASPL:ASPLViewSelectorMenu MenuAlignment="Right" AlignToParent="true" runat="server"
                        ID="ViewSelectorMenu" />
                </td>
            </tr>
        </table>
    </Template>
</SharePoint:RenderingTemplate>

