<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="CSASPNETEmailAddresseValidator.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>CSASPNETEmailAddressValidator</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Wizard ID="ValidationWizard" runat="server" Width="700px" ActiveStepIndex="0"
            BackColor="#F7F6F3" BorderColor="#CCCCCC" BorderStyle="Solid" BorderWidth="1px"
            Font-Names="Verdana" Font-Size="0.8em" 
            OnNextButtonClick="ValidationWizard_OnNextButtonClick" 
            onfinishbuttonclick="ValidationWizard_FinishButtonClick" 
            FinishCompleteButtonText="完成" FinishPreviousButtonText="上一步" 
            StartNextButtonText="下一步" StepPreviousButtonText="上一步">
            <HeaderStyle BackColor="#5D7B9D" BorderStyle="Solid" Font-Bold="True" Font-Size="0.9em"
                ForeColor="White" HorizontalAlign="Left" />
            <HeaderTemplate>
                <h2>
                    使用验证邮件来验证邮箱地址的可用性</h2>
            </HeaderTemplate>
            <NavigationButtonStyle BackColor="#FFFBFF" BorderColor="#CCCCCC" BorderStyle="Solid"
                BorderWidth="1px" Font-Names="Verdana" Font-Size="0.8em" ForeColor="#284775" />
            <SideBarButtonStyle BorderWidth="0px" Font-Names="Verdana" ForeColor="White" />
            <SideBarStyle BackColor="#7C6F57" BorderWidth="0px" Font-Size="0.9em" VerticalAlign="Top" />
            <StepStyle BorderWidth="0px" ForeColor="#5D7B9D" />
            <WizardSteps>
                <asp:WizardStep ID="WizardStep1" runat="server" Title="登录SMTP服务器">
                    <table>
                        <tr>
                            <td align="right" style="width: 200px;">
                                发送邮箱主机:
                            </td>
                            <td style="width: 300px;">
                                <asp:TextBox ID="tbHost" runat="server" Text="smtp.live.com"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td align="right" style="width: 200px;">
                                发送邮箱地址:
                            </td>
                            <td style="width: 300px;">
                                <asp:TextBox ID="tbSendMail" runat="server" Text="@hotmail.com"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td align="right">
                                发送邮箱密码:
                            </td>
                            <td>
                                <asp:TextBox ID="tbSendMailPassword" TextMode="Password" runat="server"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td align="right">
                                是否使用SSL:
                            </td>
                            <td>
                                <asp:CheckBox ID="chkUseSSL" Checked="true" runat="server" 
                                    OnCheckedChanged="chkUseSSL_CheckedChanged" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                描述:<br />这里有一个SMPTP服务器和你将会用来发送验证邮件的邮箱地址.
                            </td>
                        </tr>
                    </table>
                </asp:WizardStep>
                <asp:WizardStep ID="WizardStep2" runat="server" Title="邮箱验证">
                    <table width="100%">
                        <tr>
                            <td>
                                <asp:Label runat="server" ID="lbMessage"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                需要验证的邮箱地址:<br />
                                <asp:TextBox runat="server" ID="tbValidateEmail" Width="100%"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Button runat="server" ID="btnValidate" Text="开始验证邮箱地址"
                                    OnClick="btnValidate_Click" />
                                <asp:Button runat="server" ID="btnSendAgain" Visible="false" Text="再发一封邮件到你的邮箱"
                                    OnClick="btnSendEmailAgain_Click" />
                            </td>
                        </tr>
                    </table>
                </asp:WizardStep>
            </WizardSteps>
        </asp:Wizard>
    </div>
    </form>
</body>
</html>
