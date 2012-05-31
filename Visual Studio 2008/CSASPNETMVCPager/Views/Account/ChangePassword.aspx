<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CSASPNETMVCPager.Models.ChangePasswordModel>" %>

<asp:Content ID="changePasswordTitle" ContentPlaceHolderID="TitleContent" runat="server">
    修改密码
</asp:Content>
<asp:Content ID="changePasswordContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        修改密码</h2>
    <p>
        使用下列表单改变你的密码.
    </p>
    <p>
        新密码至少需要有
        <%= Html.Encode(ViewData["PasswordLength"]) %>
        个字符的长度.
    </p>
    <% using (Html.BeginForm())
       { %>
    <%= Html.ValidationSummary(true, "密码修改失败. 请修正错误并重试.") %>
    <div>
        <fieldset>
            <legend>帐号信息</legend>
            <div class="editor-label">
                <%= Html.LabelFor(m => m.OldPassword) %>
            </div>
            <div class="editor-field">
                <%= Html.PasswordFor(m => m.OldPassword) %>
                <%= Html.ValidationMessageFor(m => m.OldPassword) %>
            </div>
            <div class="editor-label">
                <%= Html.LabelFor(m => m.NewPassword) %>
            </div>
            <div class="editor-field">
                <%= Html.PasswordFor(m => m.NewPassword) %>
                <%= Html.ValidationMessageFor(m => m.NewPassword) %>
            </div>
            <div class="editor-label">
                <%= Html.LabelFor(m => m.ConfirmPassword) %>
            </div>
            <div class="editor-field">
                <%= Html.PasswordFor(m => m.ConfirmPassword) %>
                <%= Html.ValidationMessageFor(m => m.ConfirmPassword) %>
            </div>
            <p>
                <input type="submit" value="修改密码" />
            </p>
        </fieldset>
    </div>
    <% } %>
</asp:Content>
