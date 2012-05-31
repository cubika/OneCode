<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CSASPNETPageValidationServerSide.aspx.cs"
    Inherits="CSASPNETPageValidation.CSASPNETPageValidationServerSide" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <p>
            <b>服务器端验证控件示例</b></p>
        <p>
            <asp:Label ID="lbl_username" runat="server" Text="您当前用户名:"></asp:Label>
            <asp:TextBox ID="tb_username" runat="server"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="tb_username"
                Display="Dynamic" EnableClientScript="False" OnLoad="RequiredFieldValidator1_Load"></asp:RequiredFieldValidator>
            <asp:CustomValidator ID="CustomValidator1" ControlToValidate="tb_username" runat="server"
                ErrorMessage="用户名至少有6个字符." Display="Dynamic"
                OnServerValidate="CustomValidator1_ServerValidate"></asp:CustomValidator>
        </p>
        <p>
            <asp:Button ID="btn_submit" runat="server" Text="递交" />
        </p>
    </div>
    </form>
</body>
</html>
