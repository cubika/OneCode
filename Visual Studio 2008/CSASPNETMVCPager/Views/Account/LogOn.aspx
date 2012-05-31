<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CSASPNETMVCPager.Models.LogOnModel>" %>

<asp:Content ID="loginTitle" ContentPlaceHolderID="TitleContent" runat="server">
    登录
</asp:Content>
<asp:Content ID="loginContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        登录</h2>
    <p>
        请输入您的用户名和密码.
        如果您没有账号请先<%= Html.ActionLink("Register", "注册") %>
        .
    </p>
    <% using (Html.BeginForm())
       { %>
    <%= Html.ValidationSummary(true, "登入失败. 请修正错误并重试.")%>
    <div>
        <fieldset>
            <legend>账号信息</legend>
            <div class="editor-label">
                <%= Html.LabelFor(m => m.UserName) %>
            </div>
            <div class="editor-field">
                <%= Html.TextBoxFor(m => m.UserName) %>
                <%= Html.ValidationMessageFor(m => m.UserName) %>
            </div>
            <div class="editor-label">
                <%= Html.LabelFor(m => m.Password) %>
            </div>
            <div class="editor-field">
                <%= Html.PasswordFor(m => m.Password) %>
                <%= Html.ValidationMessageFor(m => m.Password) %>
            </div>
            <div class="editor-label">
                <%= Html.CheckBoxFor(m => m.RememberMe) %>
                <%= Html.LabelFor(m => m.RememberMe) %>
            </div>
            <p>
                <input type="submit" value="登录" />
            </p>
        </fieldset>
    </div>
    <% } %>
</asp:Content>
