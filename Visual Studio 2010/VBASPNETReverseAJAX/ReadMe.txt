==============================================================================
 ASP.NET 应用程序 : VBASPNETReverseAJAX 项目概述
==============================================================================

//////////////////////////////////////////////////////////////////////////////
总结:

反向的Ajax也被理解为：Ajax的Comet模式,Ajax推送,双向网络和HTTP服务器推送.  
这个技术保持一个HTTP的请求来允许服务器推送数据到一个浏览器, 
而不用在一个单独的时间请求服务器.

这个例子展示了如何在ASP.NET Ajax中使用这个技术.

//////////////////////////////////////////////////////////////////////////////
代码演示:

步骤1. 在一个或多个窗口中打开Receiver.aspx页面(不是浏览器标签). 
	用不同的用户名登陆.

步骤2. 打开Sender.aspx页面.输入接收端名字和消息内容.
   点击发送按钮.

//////////////////////////////////////////////////////////////////////////////
代码逻辑:

步骤1. Message类表示信息的实体.

步骤2. Client类表示一个可以从服务器接收消息的客户端. 
   服务器也可以发送消息到客户端.这个类包含两个私 
   有成员和两个公用方法. 
   
    Public Class Client
        Private messageEvent As New ManualResetEvent(False)
        Private messageQueue As New Queue(Of Message)()

        Public Sub EnqueueMessage(ByVal message As Message)
            SyncLock messageQueue
                messageQueue.Enqueue(message)
                messageEvent.[Set]()
            End SyncLock
        End Sub
        Public Function DequeueMessage() As Message
            messageEvent.WaitOne()
            SyncLock messageQueue
                If messageQueue.Count = 1 Then
                    messageEvent.Reset()
                End If
                Return messageQueue.Dequeue()
            End SyncLock
        End Function
    End Class

   EnqueueMessage方法被设计用来为发送方发送消息到客户端 
   并且DequeueMessage被设计用来为接收端接收消息.

步骤3. ClientAdapter类用来管理多个用户.表示层可以很容易得调 
   用它的方法来发送和接收消息.

步骤4. Ajax调用Dispatcher.asmx网络服务来接收消息.

步骤5. Receiver.aspx页面包含一个JavaScript函数waitEvent.当该 
   函数被调用,它将保持请求直到有一个新的消息.然后它将会
   立即保持下一个请求.

    function waitEvent() {

        VBASPNETReverseAJAX.Dispatcher.WaitMessage("<%= Session("userName") %>", 
        function (result) {
            displayMessage(result);
            setTimeout(waitEvent, 0);
        }, function () {
            setTimeout(waitEvent, 0);
        });
    }

//////////////////////////////////////////////////////////////////////////////
参考信息:

ManualResetEvent Class
http://msdn.microsoft.com/zh-cn/library/system.threading.manualresetevent.aspx

ScriptManager Control Overview
http://msdn.microsoft.com/zh-cn/library/bb398863.aspx

Web Services in ASP.NET AJAX
http://msdn.microsoft.com/zh-cn/library/bb398785(v=VS.90).aspx

//////////////////////////////////////////////////////////////////////////////