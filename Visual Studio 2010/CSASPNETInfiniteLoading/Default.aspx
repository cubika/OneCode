<%@ Page Title="Home Page" Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs"
    Inherits="CSASPNETInfiniteLoading._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Infinite loading</title>
    <link rel="stylesheet" href="Styles/Site.css" type="text/css" />
    <script type="text/javascript" src="Scripts/jquery-1.4.1.min.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {

            function lastPostFunc() {
                $('#divPostsLoader').html('<img src="images/bigLoader.gif">');

                //向服务器发送一个查询来显示新的内容. 
                $.ajax({
                    type: "POST",
                    url: "Default.aspx/Foo",
                    data: "{}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data) {

                        if (data != "") {
                            $('.divLoadData:last').after(data.d);
                        }
                        $('#divPostsLoader').empty();
                    }

                })
            };

            //向下滚动,当滚动条滚动到底部的时候会关联到下面的函数,并会触发lastPostFunc函数. 
            $(window).scroll(function () {
                if ($(window).scrollTop() == $(document).height() - $(window).height()) {
                    lastPostFunc();
                }
            });

        });
    </script>
</head>
<body>
    <form id="Form1" runat="server">
    <div style="height: 900px">
        <h1>
            只要向下滚动就可以看到正在加载的新的内容... </h1>
    </div>
    <div class="divLoadData">
    </div>
    <div id="divPostsLoader">
    </div>
    </form>
</body>
</html>
