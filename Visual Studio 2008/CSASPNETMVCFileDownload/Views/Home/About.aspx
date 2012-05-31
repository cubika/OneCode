<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="aboutTitle" ContentPlaceHolderID="TitleContent" runat="server">
    关于我们
</asp:Content>
<asp:Content ID="aboutContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        关于</h2>
    <p>
        <ul>
            <li><b>示例名:</b> CSASPNETMVCFileDownload</li>
            <li><b>语言:</b> C#</li>
        </ul>
    </p>
</asp:Content>
