<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RunCmd.aspx.cs" Inherits="CSASPNETCMD.RunCmd" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>ASPNETCMD</title>
    <script type="text/javascript">
        //递交前清除tbResult的值
        function clearResult() {
            document.getElementById("tbResult").value = "";
        }
       
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <fieldset>
        <legend>请载入批处理文件: </legend>
        <asp:RegularExpressionValidator ForeColor="Red" ID="regExpValidatorForfileUpload"
            runat="server" ControlToValidate="fileUpload" ErrorMessage="请选择.bat文件"
            ValidationGroup="UploadFile" ValidationExpression=".+\.bat$" Display="Dynamic"></asp:RegularExpressionValidator>
        <asp:RequiredFieldValidator Display="Dynamic" ForeColor="Red" ID="eequiredValidatorForfileUpload"
            ControlToValidate="fileUpload" runat="server" ValidationGroup="UploadFile" ErrorMessage="请选择一个文件"></asp:RequiredFieldValidator>
        <div>
            <asp:FileUpload Width="320px" ID="fileUpload" runat="server" />
            &nbsp;
            <asp:Button ID="btnRunBatch" OnClientClick="clearResult();" ValidationGroup="UploadFile" runat="server" Text="上传并运行"
                OnClick="btnRunBatch_Click" />
        </div>
    </fieldset>
    <br />
    <fieldset>
        <legend>请输入命令行代码: </legend>
        <br />
        <asp:TextBox ID="tbCommand" TextMode="MultiLine" runat="server" Width="300px" Height="100px"></asp:TextBox>
        <asp:RequiredFieldValidator ID="requiredValidatorFortbCommand" ForeColor="Red" ControlToValidate="tbCommand"
            runat="server" ValidationGroup="CmdText" Display="Dynamic" ErrorMessage="请输入一些命令行代码"></asp:RequiredFieldValidator>
        <br />
        <asp:Button ID="btnRunCmd"  OnClientClick="clearResult();" runat="server" Text="运行命令" OnClick="btnRunCmd_Click"
            ValidationGroup="CmdText" />
    </fieldset>
    <fieldset>
        <legend>请输入验证信息</legend>
        <label>
            域名:</label>
        <asp:TextBox ID="tbDomainName" runat="server"></asp:TextBox>
        <asp:RequiredFieldValidator ID="RequiredValidatorfortbDomainName" ForeColor="Red"
            ControlToValidate="tbDomainName" runat="server" ValidationGroup="UploadFile"
            Display="Dynamic" ErrorMessage="请输入域名"></asp:RequiredFieldValidator>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" ForeColor="Red" ControlToValidate="tbDomainName"
            runat="server" ValidationGroup="CmdText" Display="Dynamic" ErrorMessage="请输入域名"></asp:RequiredFieldValidator>
        <br />
        <label>
            用户名:</label>
        <asp:TextBox ID="tbUserName" runat="server"></asp:TextBox>
        <asp:RequiredFieldValidator ID="RequiredValidatorfortbUserName" ForeColor="Red" ControlToValidate="tbUserName"
            runat="server" Display="Dynamic" ValidationGroup="UploadFile" ErrorMessage="请输入用户名"></asp:RequiredFieldValidator>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" ForeColor="Red" ControlToValidate="tbUserName"
            runat="server" Display="Dynamic" ValidationGroup="CmdText" ErrorMessage="请输入用户名"></asp:RequiredFieldValidator>
        <br />
        <label>
            密码:</label>
        <asp:TextBox ID="tbPassword" TextMode="Password" runat="server"></asp:TextBox>
        <asp:RequiredFieldValidator ID="RequiredValidatorfortbPassword" ForeColor="Red" ControlToValidate="tbPassword"
            runat="server" Display="Dynamic" ValidationGroup="UploadFile" ErrorMessage="请输入密码"></asp:RequiredFieldValidator>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" ForeColor="Red" ControlToValidate="tbPassword"
            runat="server" Display="Dynamic" ValidationGroup="CmdText" ErrorMessage="请输入密码"></asp:RequiredFieldValidator>
    </fieldset>
    <div>
        <label>
            输出:
        </label>
        <br />
        <asp:TextBox ID="tbResult" TextMode="MultiLine" runat="server" Width="500px" Height="300px"></asp:TextBox>
    </div>
    </form>
</body>
</html>
