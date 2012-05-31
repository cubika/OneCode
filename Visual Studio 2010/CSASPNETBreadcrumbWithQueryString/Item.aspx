<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Item.aspx.cs" Inherits="CSASPNETBreadcrumbWithQueryString.Item" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>项目</title>
</head>
<body>
    <form id="form1" runat="server">

    <p>这是项目页面.</p>

    <asp:SiteMapPath ID="SiteMapPath1" runat="server">
    </asp:SiteMapPath>

    <br /><br />

    <asp:Label ID="lbMsg" runat="server" Text=""></asp:Label>

    </form>
</body>
</html>
