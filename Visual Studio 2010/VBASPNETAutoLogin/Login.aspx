<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Login.aspx.vb" Inherits="VBASPNETAutoLogin.Login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Login Page</title>
         <style type="text/css">
        .style1
        {
            width: 29%;
        }
        .style2
        {
            width: 636px;
        }
        .style3
        {
            width: 390px;
        }
        </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
     <table cellspacing="0" cellpadding="1" border="0" id="Login1" style="border-collapse:collapse;">

	<tr>

		<td><table cellpadding="0" border="0">

			<tr>

				<td align="center" colspan="2">登录</td>

			</tr><tr>

				<td align="right"><label for="UserName">用户名:</label></td><td><input name="UserName" type="text" id="UserName" /></td>

			</tr><tr>

				<td align="right"><label for="Password">密码:</label></td><td><input name="Password" type="password" id="Password" /></td>

			</tr><tr>
				<td align="right" colspan="2"><asp:Button ID="LoginButton" runat="server" 
                        Text="登录" 
                        style="height: 26px; width: 56px" /></td>
			</tr>

		</table>
		
		</td>

	</tr>

</table>
    </div>
    </form>
</body>
</html>
 