<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GlobalBackgroundWorker.aspx.cs" Inherits="CSASPNETBackgroundWorker.GlobalBackgroundWorker" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>应用程序级后台工作</title>
</head>
<body>
    <form id="form1" runat="server">

    <p><b>备注: </b><br />尝试关闭浏览器，然后10秒后再打开此页面.
    你会发现即使在浏览器关闭它仍然在工作.</p>

    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>

            <!-- 用来更新进度的定时器. -->
            <asp:Timer ID="Timer1" runat="server" Interval="1000" ontick="Timer1_Tick"></asp:Timer>

            <!-- 用来显示进度的Label -->
            <asp:Label ID="lbGlobalProgress" runat="server" Text=""></asp:Label>

        </ContentTemplate>
    </asp:UpdatePanel>

    </form>
</body>
</html>
