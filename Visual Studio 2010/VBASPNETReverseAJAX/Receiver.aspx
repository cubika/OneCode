<%@ Page Language="vb" AutoEventWireup="true" CodeBehind="Receiver.aspx.vb" Inherits="VBASPNETReverseAJAX.Receiver" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">

    <!-- 登陆 -->
    <asp:Label ID="lbNotification" runat="server" ForeColor="Red" Text="请登录:"></asp:Label><br />
    <asp:TextBox ID="tbUserName" runat="server"></asp:TextBox>
    <asp:Button ID="btnLogin" runat="server" Text="点击登录" onclick="btnLogin_Click" />

    <!-- 接收信息 -->
    <asp:ScriptManager ID="ScriptManager1" runat="server" AsyncPostBackTimeout="2147483647">
        <Services>
            <asp:ServiceReference Path="~/Dispatcher.asmx" />
        </Services>
    </asp:ScriptManager>
    <script type="text/javascript">
        
        // 这个方法将会保持http请求并等待消息.
        function waitEvent() {

            VBASPNETReverseAJAX.Dispatcher.WaitMessage("<%= Session("userName") %>", 
            function (result) {
                
                displayMessage(result);

                // 保持循环.
                setTimeout(waitEvent, 0);
            }, function () {

                // 保持循环.
                setTimeout(waitEvent, 0);
            });
        }

        // 在结果面板中添加一个消息的内容.
        function displayMessage(message) {
            var panel = document.getElementById("<%= lbMessages.ClientID %>");

            panel.innerHTML += currentTime() + ": " + message + "<br />";
        }

        // 返回一个当前时间字符串.
        function currentTime() {
            var currentDate = new Date()
            return currentDate.getHours() + ":" + currentDate.getMinutes() + ":" + currentDate.getSeconds();
        }
    </script>

    <h3>消息:</h3>
    <asp:Label ID="lbMessages" runat="server" ForeColor="Red"></asp:Label>

    </form>
</body>
</html>
