<%@ Page Language="VB" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="aboutTitle" ContentPlaceHolderID="TitleContent" runat="server" EnableViewState="True">
    关于我们
</asp:Content>

<asp:Content ID="aboutContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>数据</h2>
   <%  If Not ViewData.Model Is Nothing AndAlso ViewData.Model.ReadyToShowUI Then
    %>
    
    <%= Html.ActionLink("上一页", "Previous", "Home")%>
    <%= Html.ActionLink("下一页", "Next", "Home")%>
     <table>     
      <tr><th>姓名</th><th>年龄</th></tr>
        <%  For i As Integer = 0 To (CType(ViewData.Model, MvcWebRole.Models.CustomersSet)).Customers.Count - 1
                 %>
        <tr>
        <td>
        <%= ViewData.Model.Customers(i).Name%>
        </td>
        <td>
        <%= ViewData.Model.Customers(i).Age%>
        </td>
        </tr>
         <% Next i%>
         </table>
         <%Else%>
    <p>
        <%= Html.ActionLink("首先添加数据测试", "AddDataToTest", "Home")%>
        <%End If%>
    </p>
         
</asp:Content>

