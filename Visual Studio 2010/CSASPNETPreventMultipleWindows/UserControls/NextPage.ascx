<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NextPage.ascx.cs" Inherits="CSASPNETPreventMultipleWindows.NextPage" %>
<script>
    //如果window.name不等于session，将会跳转到InvalidPage. 
    if (window.name != "<%=GetWindowName()%>") {
        window.name = "InvalidPage";
        window.open("InvalidPage.aspx","_self");      
    }
</script>
