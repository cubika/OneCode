<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<CSASPNETMVCPager.Models.Employee>>" %>

<%@ Import Namespace="CSASPNETMVCPager.Helper" %>
<%@ Import Namespace="CSASPNETMVCPager.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    索引
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        索引</h2>
    <table>
        <tr>
            <th>
            </th>
            <th>
                姓名
            </th>
            <th>
                性别
            </th>
            <th>
                年龄
            </th>
            <th>
                地址
            </th>
        </tr>
        <% foreach (var item in Model)
           { %>
        <tr>
            <td>
              <%--  <%= Html.ActionLink("Edit", "Edit", new { /* id=item.PrimaryKey */ }) %>
                |
                <%= Html.ActionLink("Details", "Details", new { /* id=item.PrimaryKey */ })%>
                |
                <%= Html.ActionLink("Delete", "Delete", new { /* id=item.PrimaryKey */ })%>--%>
            </td>
            <td>
                <%= Html.Encode(item.Name) %>
            </td>
            <td>
                <%= Html.Encode(item.Sex) %>
            </td>
            <td>
                <%= Html.Encode(item.Age) %>
            </td>
            <td>
                <%= Html.Encode(item.Address) %>
            </td>
        </tr>
        <% } %>
        <tr>
            <td colspan="5" align="right">
                <%=Html.CreatePager<Employee>(ViewData["pager"] as Pager<Employee>)%>
            </td>
        </tr>
    </table>
  
</asp:Content>
