<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="NextPage.ascx.vb" Inherits="VBASPNETPreventMultipleWindows.NextPage" %>
<script type="text/javascript">
    // 如果window.name不等于session，将会跳转到InvalidPage.
    if (window.name != "<%=GetWindowName()%>") {
        window.name = "InvalidPage";
        window.open("InvalidPage.aspx", "_self");
    }
</script>
