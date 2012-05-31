<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Default.aspx.vb" Inherits="VBASPNETAddControlDynamically._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Button ID="btnAddAddress" runat="server" Text="增加一个新地址" />
        <asp:Button ID="btnSave" runat="server" Text="保存这些地址" />
        <br />
        <br />
        <asp:Panel ID="pnlAddressContainer" runat="server">
        </asp:Panel>
        <br />
    </div>
    </form>
</body>
</html>
