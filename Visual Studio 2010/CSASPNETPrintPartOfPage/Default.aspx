<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="CSASPNETPrintPartOfPage.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css" media="print">  
      .nonPrintable
      {
        display: none;
      }
     </style>
     <script type="text/javascript">
         function print_page() {
             window.print();
         }
     </script>
</head>
<body>
<form id="Form1" method="post" runat="server">
<center>

<font face="Verdana" size="4">ASP.NET MVC 代码实例</font><br />
<table border="0" width="90%">
<tr><td>
<p /><font face="Verdana" size="1">
    模型-视图-控制器(MVC)构建模型把应用程序分为三个主要部分:模型,视图和控制器.
 ASP.NET MVC框架在创建应用程序时提供了一种不同于ASP.NET窗体的模型. ASP.NET MVC
 框架是一个轻量级的,高度可测试的描述框架,集成了ASP.NET现有的特点，例如母版页和
 以成员为基础的身份验证.MVC框架是在System.Web.Mvc程序集中定义的. 
</font>
</td></tr>
</table>
<br />

<%=PrintImageBegin %>
<table><tr><td>
    <asp:Image ID="Image1" runat="server" ImageUrl="~/image/microsoft.jpg" /></td></tr></table>
    <%=PrintImageEnd %><%=PrintListBegin %>
<table border="0" width="90%">
<tr>
<td>标题</td>
<td>名称</td>
<td>日期</td>
</tr>
<tr>
<td>实例标题1</td>
<td>实例名称1</td>
<td>2011-03-05</td>
</tr>
<tr>
<td>实例标题2</td>
<td>实例名称2</td>
<td>2011-03-06</td>
</tr>
<tr>
<td>实例标题3</td>
<td>实例名称3</td>
<td>2011-03-07</td>
</tr>
<tr>
<td>实例标题4</td>
<td>实例名称4</td>
<td>2011-03-08</td>
</tr>
<tr>
<td>实例标题5</td>
<td>实例名称5</td>
<td>2011-03-09</td>
</tr>
<tr>
<td>实例标题6</td>
<td>实例名称6</td>
<td>2011-03-10</td>
</tr>
<tr>
<td>实例标题7</td>
<td>实例名称7</td>
<td>2011-03-11</td>
</tr>
<tr>
<td>实例标题8</td>
<td>实例名称8</td>
<td>2011-03-12</td>
</tr>
</table>
    <%=PrintListEnd%><%=PrintButtonBegin %>
<table>
<tr>
<td>
<font face="Arial" size="3" color="white">
  <asp:Button ID="btnPrint" runat="server" Text="打印这个页面" 
        onclick="btnPrint_Click"/></font>
    <asp:CheckBox ID="chkDisplayImage" runat="server" Text="显示图片" />
    <asp:CheckBox ID="chkDisplayList" runat="server" Text="显示列表" />
    <asp:CheckBox ID="chkDisplayButton" runat="server" Text="显示按钮" />
    <asp:CheckBox ID="chkDisplaySearch" runat="server" Text="显示搜索" />
</td>
</tr>
</table>
    <%=PrintButtonEnd %><%=PrintSearchBegin %>
<table>
<tr>
<td valign="top" bgcolor="#bfa57d" width="230">
<font face="Arial" size="3" color="white"><b>MVC 代码实例</b><br /><br />mvc<br /><br />
<font size="1"> 搜索代码<br /></font>
<asp:TextBox id="tbSearch" runat="server" width="100"/>
<asp:Button id="btnSearch" runat="server" height="22" Text="搜索"/>
</font>
</td>
</tr>
</table>
    <%=PrintSearchEnd %>
</center>
</form>
</body>
</html>
