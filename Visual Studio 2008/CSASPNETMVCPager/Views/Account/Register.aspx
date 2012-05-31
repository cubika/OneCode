<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CSASPNETMVCPager.Models.RegisterModel>" %>

<asp:Content ID="registerTitle" ContentPlaceHolderID="TitleContent" runat="server">
    注册
</asp:Content>
<asp:Content ID="registerContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        创建一个新账号</h2>
    <p>
        使用下列表单创建一个新账号.
    </p>
    <p>
        密码至少需要有
        <%= Html.Encode(ViewData["PasswordLength"]) %>
        个字符的长度.
    </p>
    <% using (Html.BeginForm())
       { %>
    <%= Html.ValidationSummary(true, "账号创建失败. 请修正错误并重试.")%>
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
                <%= Html.LabelFor(m => m.Email) %>
            </div>
            <div class="editor-field">
                <%= Html.TextBoxFor(m => m.Email) %>
                <%= Html.ValidationMessageFor(m => m.Email) %>
            </div>
            <div class="editor-label">
                <%= Html.LabelFor(m => m.Password) %>
            </div>
            <div class="editor-field">
                <%= Html.PasswordFor(m => m.Password) %>
                <%= Html.ValidationMessageFor(m => m.Password) %>
            </div>
            <div class="editor-label">
                <%= Html.LabelFor(m => m.ConfirmPassword) %>
            </div>
            <div class="editor-field">
                <%= Html.PasswordFor(m => m.ConfirmPassword) %>
                <%= Html.ValidationMessageFor(m => m.ConfirmPassword) %>
            </div>
            <p>
                <input type="submit" value="注册" />
            </p>
        </fieldset>
    </div>
    <% } %>
</asp:Content>
