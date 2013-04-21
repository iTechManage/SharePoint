<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls"
    Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PrintView.aspx.cs" Inherits="CrowCanyonAdvancedPrint.Layouts.CrowCanyon.AdvancedPrint.PrintView"
    DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
</asp:Content>
<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <script type="text/javascript">
        function printResult() {
            var printContent = document.getElementById("PRINT");
            if (printContent != null) {
                var windowUrl = 'about:blank';
                var windowName = 'Print';
                var WinPrint = window.open(windowUrl, windowName, 'left=300,top=300,right=500,bottom=500,width=1000,height=500');
                WinPrint.document.write('<' + 'html' + '><head></head><' + 'body style="background:none !important"' + '>');
                WinPrint.document.write(printContent.innerHTML);
                WinPrint.document.write('</body></html>');
                WinPrint.document.close();
                WinPrint.focus();
                WinPrint.print();
                WinPrint.close();
            }
        }
    </script>
    <SharePoint:CssLink ID="CssLink1" runat="server" />
    <asp:PlaceHolder ID="phCss" runat="server" />
    <style type="text/css">
        #mbox
        {
            background-color: #fff;
            padding: 5px;
            padding-top: 1px;
            border: 2px outset #666;
            position: absolute;
            overflow: hidden;
        }
        .ms-vh
{
    color:#000000;
    font-size:12px;
    font-family:Arial;
    border:1px solid #cccccc;
    background-color:#E0E0E0;
    font-weight:300;
    padding:4px;
}

        #mbd
        {
            font-family: sans-serif;
            font-weight: bold;
            padding-bottom: 5px;
        }
        #ol
        {
            background-image: url('/_layouts/<asp:Literal runat="server" id="ltrVersionFolder"/>images/CrowCanyon.AdvancedPrint/crowcanyonlogo.png');
            position: fixed;
            top: 0px;
            left: 0px;
            width: 100%;
            z-index: 998;
        }
        .iw-user
        {
            background-color: #E3E3E3;
            border: 1px solid #A5A5A5;
            margin: 1px;
            font-size: 12px;
        }
        .ms-formfield
{
    color:#000000;
    font-size:12px;
    font-family:Arial;
    border:1px solid #cccccc;
    background-color:#F0F0F0;
    padding:4px;
    width:60%;
    vertical-align:top;
}
.ms-standardheader
{
    font-size:1.05em;
    font-weight:500;
    font-family:Arial;
}

.ms-linksectionheader
{
    background-color:#707070;
    color:#F0F0F0;
    padding:4px;
}

.ms-formlabel
{
    color:#555555;
    font-size:12px;
    font-family:Arial;
    border:1px solid #cccccc;
    padding:4px;
    width:40%;
    vertical-align:top;
}


        table.ms-rtetoolbarmenu
        {
            width: 720px;
        }
        iframe.ms-rtelong
        {
            width: 720px;
        }
    </style>
    <table border="1" width="100%">
        <tr style="width: 100%" >
            <td>
                <table cellspacing="2" cellpadding="2" width="100%" style="background-color: #CCCCCC;
                    margin: 0px;">
                    <tr>
                        <td >
                            <asp:Label ID="labl" runat="server" Text="Template:"></asp:Label>
                            <asp:DropDownList ID="TemplatesList" runat="server" AutoPostBack="True">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:ImageButton align="Right" ID="imgPrint" runat="server" Visible="true" OnClientClick="javascript:printResult()"
                                AlternateText="Print" ImageUrl="~/_layouts/images/CrowCanyon.AdvancedPrint/printer1.png" />
                            <asp:ImageButton align="Right" ID="ImageButton1" runat="server" Visible="true" AlternateText="Print"
                                ImageUrl="~/_layouts/images/CrowCanyon.AdvancedPrint/pdf.png" />
                        </td>
                    </tr>
                </table>
                <table width="100%">
                    <tr style="width: 100%">
                        <td>
                            <asp:ListBox ID="UnvisibleListBox" Visible="false" Rows="18" runat="server" Width="100%"
                                SelectionMode="Single"></asp:ListBox>
                            <asp:Label ID="HtmlBody" runat="server"></asp:Label><br />
                            <div id="PRINT" >
                                <asp:Label ID="BdyTextBox" TextMode="MultiLine" ReadOnly="true" Rows="10" Width="99%"
                                    runat="server" />
                            </div>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    Print Page
</asp:Content>
<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea"
    runat="server">
    Print Page
</asp:Content>
