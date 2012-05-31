<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DefaultPage.ascx.cs" Inherits="CSASPNETPreventMultipleWindows.DefaultPage" %>
<script>
    //首先,window.name为空("")就会进入这个if分支 
    if (window.name == "") {
        window.name = "Default";
        window.open("Default.aspx", "_self");       
    }
    //其次，window.name改变为“Default”会进入这个if分支 
    else if (window.name == "Default") {
        //设置window的特征 
        var WindowFeatures = 'width=800,height=600';
        window.open("Main.aspx", "<%=GetWindowName() %>");
        window.opener = top;
        window.close();
    }
    else if (window.name == "InvalidPage") {
        window.close();
    }
    else {
        window.name = "InvalidPage";
        window.open("InvalidPage.aspx", "_self");
    }
</script>
