<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CSASPNETMVCDataView.Models.Person>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	详细信息
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>详细信息</h2>

    <fieldset>
        <legend>Fields</legend>
        
        <div class="display-label">Id</div>
        <div class="display-field"><%= Html.Encode(Model.Id) %></div>
        
        <div class="display-label">姓名</div>
        <div class="display-field"><%= Html.Encode(Model.Name) %></div>
        
        <div class="display-label">年龄</div>
        <div class="display-field"><%= Html.Encode(Model.Age) %></div>
        
        <div class="display-label">电话</div>
        <div class="display-field"><%= Html.Encode(Model.Phone) %></div>
        
        <div class="display-label">Email</div>
        <div class="display-field"><%= Html.Encode(Model.Email) %></div>
        
    </fieldset>
    <p>
        <%= Html.ActionLink("编辑", "Edit", new { id=Model.Id }) %> |
        <%= Html.ActionLink("返回到列表", "Index") %>
    </p>

</asp:Content>

