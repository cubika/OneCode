<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="CSASPNETBackgroundWorker.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>指定为当前用户的后台工作</title>
</head>
<body>
    <form id="form1" runat="server">

    <p><b>备注:</b><br />
    尝试使用两个使用不同会话的浏览器打开这个页面然后同时执行操作.
    你会发现每个会话有其自己的后台工作. 并不为所有用户共享.
    </p><br />

    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>

    <!-- UpdateUpanel让进度可以在不更新整个页面时更新(局部更新). -->
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            
            <!-- 用来更新进度的定时器. -->
            <asp:Timer ID="Timer1" runat="server" Interval="100" Enabled="false" ontick="Timer1_Tick"></asp:Timer>

            <!-- 用来显示进度和结果的Label -->
            <asp:Label ID="lbProgress" runat="server" Text=""></asp:Label><br />

            <!-- 输入值并单击按钮开始操作 -->
            输入一个参数: <asp:TextBox ID="txtParameter" runat="server" Text="Hello World"></asp:TextBox><br />
            <asp:Button ID="btnStart" runat="server" Text="单击开始后台工作" onclick="btnStart_Click" />

        </ContentTemplate>
    </asp:UpdatePanel>

    <br /><p><b>另一个页面: </b><br />
        这个页面显示应用程序级后台工作的当前状态.</p>
    <a href="GlobalBackgroundWorker.aspx" target="_blank">GlobalBackgroundWorker.aspx</a>

    </form>
</body>
</html>
