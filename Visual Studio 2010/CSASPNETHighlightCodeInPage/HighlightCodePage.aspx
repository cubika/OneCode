<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HighlightCodePage.aspx.cs"
    Inherits="CSASPNETHighlightCodeInPage.HighlightCodePage" ValidateRequest="false" %>
   
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link href="Styles/HighlightCode.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .style1
        {
            width: 4%;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <table border="1" style="height: 98%">
        <tr>
            <td style="font-size: 12px" class="style1">
                <strong>请在单击高亮按钮之前粘贴代码到文本框控件并选择语言</strong>
            </td>
            <td style="width: auto; font-size: 12px">
                <strong>结果: </strong>
            </td>
        </tr>
        <tr>
            <td class="style1">
                请选择语言:<asp:DropDownList ID="ddlLanguage" runat="server">
                    <asp:ListItem Value="-1">-请选择-</asp:ListItem>
                    <asp:ListItem Value="cs">C#</asp:ListItem>
                    <asp:ListItem Value="vb">VB.NET</asp:ListItem>
                    <asp:ListItem Value="js">JavaScript</asp:ListItem>
                    <asp:ListItem Value="vbs">VBScript</asp:ListItem>
                    <asp:ListItem Value="sql">Sql</asp:ListItem>
                    <asp:ListItem Value="css">CSS</asp:ListItem>
                    <asp:ListItem Value="html">HTML/XML</asp:ListItem>
                </asp:DropDownList>
                <br />
                请将您的代码粘贴到此处:<br />
                <asp:TextBox ID="tbCode" runat="server" TextMode="MultiLine" Height="528px" Width="710px" ></asp:TextBox>
                <br />
                <asp:Button ID="btnHighLight" runat="server" Text="高亮" OnClick="btnHighLight_Click" /><asp:Label
                    ID="lbError" runat="server" Text="Label" ForeColor="Red"></asp:Label>
            </td>
            <td>
                <div id="DivCode">
                    <asp:Label ID="lbResult" runat="server" Text=""></asp:Label>
                </div>
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
