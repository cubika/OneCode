<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Sender.aspx.cs" Inherits="CSASPNETReverseAJAX.Sender" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">

    <asp:Label ID="lbNotification" runat="server" ForeColor="Red"></asp:Label><br /><br />

    接收端名字:<br />
    <asp:TextBox ID="tbRecipientName" runat="server" Width="100px"></asp:TextBox><br />

    消息:<br />
    <asp:TextBox ID="tbMessageContent" runat="server" Width="300px"></asp:TextBox><br />

    <asp:Button ID="btnSend" runat="server" Text="点击发送" onclick="btnSend_Click" />

    </form>
</body>
</html>
