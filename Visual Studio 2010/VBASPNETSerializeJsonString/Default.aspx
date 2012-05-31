<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Default.aspx.vb" Inherits="VBASPNETSerializeJsonString._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>AutoComplete Example -- serialize JSON string and return search result</title>
    <link rel="stylesheet" href="Styles/jquery-ui.css" type="text/css" media="all" />
    <link rel="stylesheet" href="Styles/site.css" type="text/css" />
    <script type="text/javascript" src="Scripts/jquery.min.js"></script>
    <script type="text/javascript" src="Scripts/jquery-ui.min.js"></script>
   <script type="text/javascript">
       $(function () {
           $('#<%= tbBookName.ClientID %>').autocomplete({
               source: "AutoComplete.ashx",
               select: function (event, ui) {
                   $(".author").text(ui.item.Author);
                   $(".genre").text(ui.item.Genre);
                   $(".price").text(ui.item.Price);
                   $(".publish_date").text(ui.item.Publish_date);
                   $(".description").text(ui.item.Description);
               }
           });
       });
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <h3>
        AutoComplete实例 -- 
        实例化JSON字符串并返回查询结果
    </h3>
    <div class="search_bar">
        查询书籍:
        <asp:TextBox ID="tbBookName" runat="server" />
        (尝试使用简单的例子，比如: 'Midnight Rain', 'XML Developer's Guide')</div>
    <div class="search_response">
        <p>
            作者: <span class="author"></span>
        </p>
        <p>
            流派: <span class="genre"></span>
        </p>
        <p>
            单价: <span class="price"></span>
        </p>
        <p>
            出版日期: <span class="publish_date"></span>
        </p>
        <p>
            简介: <span class="description"></span>
        </p>
    </div>
    </form>
</body>
</html>
