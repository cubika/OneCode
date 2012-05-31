========================================================================
          CSASPNETAJAXConsumeExternalWebService 概述
========================================================================

/////////////////////////////////////////////////////////////////////////////
用法:

这个项目演示了如何从别的域调用一个外部Web服务.

/////////////////////////////////////////////////////////////////////////////
演示这个示例. 

请遵循下列演示步骤.

步骤 1: 打开CSASPNETAJAXConsumeExternalWebService.sln.

步骤 2: 展开ExternalWebSite然后右击ExternalWebService.asmx 
        再单击"在浏览器中查看". 这个步骤非常重要, 
        它模拟打开一个外部web服务.

步骤 3: 展开TestWebSite然后右击default.aspx再单击"在浏览器中查看".

步骤 4: 你将看到一个空白面板和一个按钮.单击按钮.你将看到面板显示"请稍等...",  
	稍等一秒左右,你将看到来自服务器的时间和日期.

/////////////////////////////////////////////////////////////////////////////
代码逻辑:

步骤 1.  在Visual Studio 2010或Visual Web Developer 2010中创建一个
	C# "ASP.NET空白Web站点".变更最后的文件夹名为ExternalWebSite.

步骤 2.  增加一个新"Web服务"项目.我们叫它ExternalWebService.asmx.

步骤 3.  打开App_Code中的ExternalWebService.asmx.cs.

步骤 4.  取消类名前下列行的注释标记.
         [CODE]
         [System.Web.Script.Services.ScriptService]
         [/CODE]

步骤 5.  如下编写一个新的Web方法并保存文件.
         [CODE]
         [WebMethod]
         public DateTime GetServerTime() {
              return DateTime.Now;
         }
         [/CODE]

步骤 6.  打开ExternalWebService.asmx在浏览器中查看页面.
         复制导航栏中的URL地址.
         
步骤 7.  增加一个C# "ASP.NET空白Web站点".变更最后的文件夹名为TestWebSite.
         
步骤 8.  在解决方案资源管理器单击新站点然后查看菜单栏.
		单击站点-> 添加Web引用...

步骤 9.  粘贴我们从步骤6得到的URL地址到"URL" textbox.
         变更"Web引用名称"为"ExternalWebService". 单击"添加引用".

步骤 10. 增加一个新"Web服务"项目.我们叫它BridgeWebService.asmx

步骤 11. 在App_Code中打开BridgeWebService.asmx.cs. 重复步骤4.

步骤 12. 编写下列用来调用外部web服务代码并保存文件.
         [CODE]
         [WebMethod]
         public DateTime GetServerTime()
         {
             // Get an instance of the external web service
             ExternalWebService.ExternalWebService ews = new ExternalWebService.ExternalWebService();
             // Return the result from the web service method.
             return ews.GetServerTime();
         }
         [/CODE]

步骤 13. 创建一个新的"Web页面"项.变更其名字为Default.aspx.

步骤 14. 向页面添加一个ScriptManager控件. 
		还有一个地址为本地桥梁web服务的服务引用:BridgeWebService.asmx.

步骤 15. 创建一个用来显示结果的DIV和用来调用服务的按钮.
         [CODE]
         <div id="Result" 
              style="width: 100%; height: 100px; background-color: Black; color: White">
         </div>
         <input type="button" 
                value="Get Server Time From External Web Service"
                onclick="GetServerDateTime()" />
         [/CODE]

步骤 16. 创建调用Web服务的Javascript函数.
         [CODE]
         <script type="text/javascript">
         // This function is used to call the service by Ajax Extension.
         function GetServerDateTime() {
             $get("Result").innerHTML = "Please wait a moment...";
             BridgeWebService.GetServerTime(onSuccess, onFailed);
         }
         // This function will be executed when get a response 
         // from the service.
         function onSuccess(result) {
             $get("Result").innerHTML = "Server DateTime is : " + result.toLocaleString();
         }

         // This function will be executed when get an exception
         // from the service.
         function onFailed(args) {
             alert("Server Return Error:\n" +
             "Message:" + args.get_message() + "\n" +
             "Status Code:" + args.get_statusCode() + "\n" +
             "Exception Type:" + args.get_exceptionType());
         }
         </script>
         [/CODE]
         当我们向ScriptManager添加ServiceReference,我们将看到智能信息效果.

步骤 17. 测试Default.aspx我们将从服务器得到时间和日期.

PS
如果我们想通过ScriptManager的服务引用使用不同域的一个AJAX-enabled WCF服务,
其步骤与我们上面讨论的相同.
只要创建一个本地的AJAX-enabled WCF作为调用远程的桥梁.
/////////////////////////////////////////////////////////////////////////////
参考资料:

MSDN: ServiceReference 类
http://msdn.microsoft.com/zh-cn/library/system.web.ui.servicereference.aspx

MSDN: ASP.NET Web 服务
http://msdn.microsoft.com/zh-cn/library/t745kdsh.aspx

/////////////////////////////////////////////////////////////////////////////
