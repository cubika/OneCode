<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    列表
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        所有可供下载文件:</h2>
    <table style="width: 100%">
        <thead>
            <td>
                文件名
            </td>
            <td>
                大小(字节)
            </td>
            <td style="width: 40">
            </td>
        </thead>
        <%
            var fileList = (List<System.IO.FileInfo>)Model;
            foreach (var file in fileList)
            {
        %>
        <tr>
            <td>
                <%= Html.ActionLink(file.Name,"Download",new {Action="Download", fn=file})  %>
            </td>
            <td>
                <%=file.Length %>
            </td>
            <td>
                <a href='<%= ResolveUrl("~/File/Download/"+ file.Name) %>'>
                    <img width="30" height="30" src='<%= ResolveUrl("~/images/download-icon.gif") %>' />
                </a>
            </td>
        </tr>
        <%} %>
    </table>
</asp:Content>
