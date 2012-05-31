=============================================================================
        WINDOWS APPLICATION : VBReceiveWM_COPYDATA Project Overview
=============================================================================

/////////////////////////////////////////////////////////////////////////////
Summary:

基于windows 消息 WM_COPYDATA 进程间通信(IPC) 是一种在本地机器上windows应用程序交换数据机制。

接受程序必须是一个windows应用程序。数据被传递必须不包含指针或者不能被应用程序接受的指向对象的引用。

当发送WM_COPYDATA消息时，引用数据不能被发送进程别的线程改变。 接受应用程序应该只考虑只读数据。

如果接受应用程序想要在SendMessage返回之后进入数据， 它必须拷贝数据到本地缓存。

这个代码例子示范了通过处理WM_COPYDATA消息从发送程序（VBSendWM_COPYDATA）接受一个客户数据结构（MYStruct）


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
VBSendWM_COPYDATA 发送数据到 VBReceiveWM_COPYDATA 通过 WM_COPYDATA 消息


/////////////////////////////////////////////////////////////////////////////
代码逻辑:

1. 在窗口Form里面重写WndProc方法

    Protected Overrides Sub WndProc(ByRef m As System.Windows.Forms.Message)
    End Sub

2. 在窗口过程WndProc处理WM_COPYDATA消息

    If (m.Msg = WM_COPYDATA) Then
    End If
        
3. 从WM_COPYDATA消息的lParam里得到COPYDATASTRUCT结构，然后再从COPYDATASTRUCT.lpData
   获取到数据（Mystruct对象）
    ' Get the COPYDATASTRUCT struct from lParam.
    Dim cds As COPYDATASTRUCT = m.GetLParam(GetType(COPYDATASTRUCT))

    ' If the size matches
    If (cds.cbData = Marshal.SizeOf(GetType(MyStruct))) Then
        ' Marshal the data from the unmanaged memory block to a MyStruct 
        ' managed struct.
        Dim myStruct As MyStruct = Marshal.PtrToStructure(cds.lpData, _
            GetType(MyStruct))
    End If

4. 在Form里显示数据。


/////////////////////////////////////////////////////////////////////////////
参考文献:

WM_COPYDATA Message
http://msdn.microsoft.com/en-us/library/ms649011.aspx


/////////////////////////////////////////////////////////////////////////////
