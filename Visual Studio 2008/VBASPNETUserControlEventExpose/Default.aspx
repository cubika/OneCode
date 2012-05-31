<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Default.aspx.vb" Inherits="VBASPNETUserControlEventExpose._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
        <title>VBASPNETuserControlEventExpose</title>
</head>
<body>
    <form id="form1" runat="server">
   <div>
        <asp:Label ID="lblText" Text="我在主页面上." runat="server"></asp:Label>
        <asp:DropDownList ID="ddlTemp" runat="server">
            <asp:ListItem>美国</asp:ListItem>
            <asp:ListItem>中国</asp:ListItem>
            <asp:ListItem>德国</asp:ListItem>
            <asp:ListItem>日本</asp:ListItem>    
        </asp:DropDownList>
        <br />
        <br />
  
       <asp:PlaceHolder ID="PlaceHolder1" runat="server"></asp:PlaceHolder>
    </div>
    </form>
</body>
</html>
