========================================================================
    SILVERLIGHT应用程序: VBSL3SocketClient 项目概述 
========================================================================

/////////////////////////////////////////////////////////////////////////////
用法:

本项目创建一个套接字客户端示例，它能异步发送字符串消息到服务器和从服务器接收
字符串消息。


/////////////////////////////////////////////////////////////////////////////
相关项目:

VBSL3SocketClient <--> VBSL3SocketServer
项目VBSL3SocketServe是能为Silverlight套接字客户端服务的套接字服务器。 


/////////////////////////////////////////////////////////////////////////////
演示：

要运行这个套接字示例，请试试以下步骤：
1. 运行套接字服务器
	a. 用管理员身份打开VBSL3SocketServer解决方案，并编译。
	b. 运行该项目。
2. 运行silverlight套接字客户端
	a. 打开VBSL3SocketClient解决方案，并编译，运行。
	b. 当Silverlight应用程序被加载时，按照页面上显示的提示操作：
		1) 点击“连接”来连接套接字服务器。
		2) 在文本框中输入文字，然后点击“发送”按钮。
		3) 服务器将会接收和处理字符串消息，然后在1秒后回发。


/////////////////////////////////////////////////////////////////////////////
先决条件：

Visual Studio 2008 SP1 的 Silverlight 3 工具
http://www.microsoft.com/downloads/details.aspx?familyid=9442b0f2-7465-417a-88f3-5e7b5409e9dd&displaylang=en

Silverilght 3 运行时：
http://silverlight.net/getstarted/silverlight3/


/////////////////////////////////////////////////////////////////////////////
代码逻辑：

1. 怎样使用套接字同步连接套接字服务器？
	Silverlight只支持异步模式请求。连接远程套接字：
	1. 创建一个SocketAsyncEventArgs对象，设置SocketAsyncEventArgs.RemoteEndPoint 
	为套接字服务器的终端地址，注册完成事件。
    2. 创建一个套接字实例，通过已初始化的SocketAsyncEventArgs来调用Socket.ConnectAsync方法。
    3. 当SocketAsyncEventArgs.Completed被触发时，检查SocketAsyncEventArgs.SocketError的属性，
	如果它等于SocketError.Success，就意味套接字连接成功。

    此代码示例在SocketClient.vb中的"套接字异步连接"代码块中可找到。

2. 怎样从字串消息中分离出套接字字节数组？
	有几种方法可以分离消息的方法，本项目使用一个预定义字符作为分隔符。
	需要注意的是，UTF8是可变长编码的。解码时，我们需要检查和去掉该分隔符。
	
	
    此代码示例在SocketClient.vb中的"字符串解码"代码块中可找到。

3. 怎样通过套接字接收字符串？
	1. 创建一个SocketAsyncEventArgs对象，分配字节数组给SocketAsyncEventArgs.Buffer
	作为接收的缓冲器，然后注册完成事件。
	2. 通过调用Socket.ReceiveAsync方法来使用连接的套接字接收字节集。
	3. 当SocketAsyncEventArgs.Completed被触发时，检查SocketAsyncEventArgs.SocketError
	的属性，如果它等于SocketError.Success，那就意味着套接字连接成功。
    4. 使用UTF8解码器把SocketAsyncEventArgs.Buffer解码成字符串。
    
    此代码示例在SocketClient.vb中的"套接字异步接收"代码块中可找到。

4. 怎样通过套接字发送字符串？
	1. 使用UTF8编码器把字符串编码成字节数组。
	2. 创建一个SocketAsyncEventArgs对象，分配编码的字节数组给SocketAsyncEventArgs.Buffer
	作为发送的缓冲器，然后注册完成事件。
	3. 当SocketAsyncEventArgs.Completed方法被触发时，检查SocketAsyncEventArgs.SocketError
	的属性，如果它等于SocketError.Success，就意味着套接字连接成功。

    此代码示例在SocketClient.vb中的"套接字异步发送"代码块中可找到。
    
    
/////////////////////////////////////////////////////////////////////////////
参考资料：

套接字类
http://msdn.microsoft.com/en-us/library/system.net.sockets.socket.aspx


/////////////////////////////////////////////////////////////////////////////
