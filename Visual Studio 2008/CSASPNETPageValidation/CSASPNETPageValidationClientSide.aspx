<%--/****************************** Module Header ******************************\
* Module Name:    CSASPNETPageValidationClientSide.aspx
* Project:        CSASPNETPageValidation
* Copyright (c) Microsoft Corporation.
*
* The CSASPNETPageValidationClientSide sample demonstrates the 
* ASP.NET validation controls to validate the value at the client side.
* 
* All other rights reserved.
*
* History:
* * 7/30/2009 11:00 AM Zong-Qing Li Created
\***************************************************************************/--%>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CSASPNETPageValidationClientSide.aspx.cs"
    Inherits="CSASPNETPageValidation.CSASPNETPageValidationClientSide" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>

    <script type="text/javascript">
        function ClientValidate(source, clientside_arguments) {
            //测试值是否有6个字符
            if (clientside_arguments.Value.length >= 6) {
                clientside_arguments.IsValid = true;
            }
            else { clientside_arguments.IsValid = false };
        }
    </script>

</head>
<body>
    <form id="form1" runat="server">
    <div>
        <p>
            <b>客户端验证控件示例</b></p>
        <p>
            <asp:Label ID="lbl_email" runat="server" Text="您当前的电子邮件地址:"></asp:Label>
            <asp:TextBox ID="tb_email" runat="server"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="tb_email"
                ErrorMessage="必填项目不能为空." Display="Dynamic"></asp:RequiredFieldValidator>
            <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ErrorMessage="无效的电子邮件地址."
                ControlToValidate="tb_email" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"
                Display="Dynamic"></asp:RegularExpressionValidator>
        </p>
        <p>
            <asp:Label ID="lbl_pwd" runat="server" Text="选择一个密码:"></asp:Label>
            <asp:TextBox ID="tb_pwd" runat="server" TextMode="Password"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" ControlToValidate="tb_pwd"
                runat="server" ErrorMessage="必填项目不能为空." Display="Dynamic"></asp:RequiredFieldValidator>
            <asp:CustomValidator ID="CustomValidator1" ClientValidationFunction="ClientValidate"
                ControlToValidate="tb_pwd" runat="server" ErrorMessage="密码至少有6个字符."
                Display="Dynamic"></asp:CustomValidator>
        </p>
        <p>
            <asp:Label ID="lbl_repwd" runat="server" Text="再输入一次密码:"></asp:Label>
            <asp:TextBox ID="tb_repwd" runat="server" TextMode="Password"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator3" ControlToValidate="tb_repwd"
                runat="server" ErrorMessage="必填项目不能为空." Display="Dynamic"></asp:RequiredFieldValidator>
        </p>
        <p>
            <asp:CompareValidator ID="CompareValidator1" runat="server" ErrorMessage="密码不相符."
                ControlToCompare="tb_repwd" ControlToValidate="tb_pwd"></asp:CompareValidator>
        </p>
        <p>
            <asp:ValidationSummary ID="ValidationSummary1" runat="server" />
        </p>
        <p>
            <asp:Button ID="btn_submit" runat="server" Text="递交" />
        </p>
    </div>
    </form>
</body>
</html>
