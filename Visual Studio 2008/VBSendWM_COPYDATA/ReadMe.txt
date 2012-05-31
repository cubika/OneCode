=============================================================================
         WINDOWS 应用程序 : VBSendWM_COPYDATA 项目概述
=============================================================================

/////////////////////////////////////////////////////////////////////////////
概要:

基于windows 消息 WM_COPYDATA 进程间通信(IPC) 是一种在本地机器上windows应用程序交换数据机制。

接受程序必须是一个windows应用程序。数据被传递必须不包含指针或者不能被应用程序接受的指向对象的引用。

当发送WM_COPYDATA消息时，引用数据不能被发送进程别的线程改变。 接受应用程序应该只考虑只读数据。

如果接受应用程序想要在SendMessage返回之后进入数据， 它必须拷贝数据到本地缓存。

这个代码例子示范了通过SendMessage（WM_COPYDATA）发送一个客户端数据结构（MY_STRUCT）到接受应用程序
如果数据结构传值失败，应用程序显示一个诊断错误代码。一个典型的错误代码是0x5（非法访问），它是由于用户
接口权限隔离导致的。用户接口权限隔离阻止进程发送被选择的窗口消息和其他一些用户进程APIs，这些用户进程拥有
比较高的完整性。 当接受程序（VBReceiveWM_COPYDATA）运行在一个比发送程序高的完整性时候，你将会看到一个
"SendMessage(WM_COPYDATA) failed w/err 0x00000005"错误信息。


/////////////////////////////////////////////////////////////////////////////
演示:

下面的步骤贯穿整个WM_COPYDATA例子演示。

步骤1. 当你成功的在vs2008编译了VBSendWM_COPYDATA和VBReceiveWM_COPYDATA例子后，你将会得到
VBSendWM_COPYDATA.exe以及VBReceiveWM_COPYDATA.exe.

步骤2： 运行VBSendWM_COPYDATA.exe以及VBReceiveWM_COPYDATA.exe程序， 在VBSendWM_COPYDATA程序中
输入数字和消息
	数字： 123456
	消息： 你好，世界

点击发送按钮。 数字和消息将会被发送到VBReceiveWM_COPYDATA通过 WM_COPYDATA消息， 当VBReceiveWM_COPYDATA

收到消息后， 应用程序解压它们并且把它们显示在窗口上。



/////////////////////////////////////////////////////////////////////////////
与本例相关:
(当前例子和其他例子在微软 All-In-One Code Framework http://1code.codeplex.com)

VBSendWM_COPYDATA -> VBReceiveWM_COPYDATA
VBSendWM_COPYDATA 发送数据到 VBReceiveWM_COPYDATA 通过 WM_COPYDATA 消息。


/////////////////////////////////////////////////////////////////////////////
代码逻辑:

1. 找到接受窗口的句柄（FindWindow）

2. 准备好用来发送的带数据的COPYDATASTRUCT（COPYDATASTRUCT）


3. 通过WM_COPYDATA消息发送COPYDATASTRUCT结构到接受窗口。


/////////////////////////////////////////////////////////////////////////////
参考文献:

WM_COPYDATA Message
http://msdn.microsoft.com/en-us/library/ms649011(VS.85).aspx


/////////////////////////////////////////////////////////////////////////////