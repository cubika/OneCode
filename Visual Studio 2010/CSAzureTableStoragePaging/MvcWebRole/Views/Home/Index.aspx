<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="aboutTitle" ContentPlaceHolderID="TitleContent" runat="server">
    About Us
</asp:Content>

<asp:Content ID="aboutContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Data</h2>
   <%if (ViewData.Model as MvcWebRole.Models.CustomersSet != null && ((MvcWebRole.Models.CustomersSet)ViewData.Model).ReadyToShowUI) 
    {%>
    
    <%= Html.ActionLink("上一页", "Previous", "Home")%>
    <%= Html.ActionLink("下一页", "Next", "Home")%>
     <table>     
      <tr><th>Name</th><th>Age</th></tr>
        <%for (int i = 0; i < ((MvcWebRole.Models.CustomersSet)ViewData.Model).Customers.Count; i++)
         { %>
        <tr>
        <td>
        <%=((MvcWebRole.Models.CustomersSet)ViewData.Model).Customers[i].Name%>
        </td>
        <td>
        <%=((MvcWebRole.Models.CustomersSet)ViewData.Model).Customers[i].Age%>
        </td>
        </tr>
         <%}%>
         </table>
         <%}else {%>
    <p>
        <%= Html.ActionLink("首先添加数据测试", "AddDataToTest", "Home")%>
        <%}%>
    </p>
         
</asp:Content>
