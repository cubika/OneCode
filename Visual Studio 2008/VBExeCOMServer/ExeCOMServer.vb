'********************************* 模块头 **********************************'
' 模块名:      ExeCOMServer.vb
' 项目名:      VBExeCOMServer
' 版权 (c) Microsoft Corporation.
' 
' ExeCOMServer封装了VB.NET中线程外COM服务器的框架。这个类实现了单态（Singleton）
' 设计模式，并且它是线程安全的。执行VBExeCOMServer.Instance.Run()以启动服务器。
' 如果服务器正在运行，此函数将立即返回。在Run方法中，它注册了COM服务器所公开的COM
' 类中类组件。并且启动消息循环以等待锁定计数器回落至0。当锁定计数器为0时，它撤销
' 注册并退出服务器。
' 
' 当一个COM对象被创建时，服务器的锁定计数器将会增加。当一个对象被释放时（被GC时），
' 锁定计数器会减少。为确保COM对象会被即时的垃圾回收（GC），ExeCOMServer在服务器启
' 动后每隔5秒钟触发一次垃圾回收。
'
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************'

#Region "Imports directives"

Imports System.Threading

#End Region


Friend NotInheritable Class ExeCOMServer

#Region "Singleton Pattern"

    Private Sub New()
    End Sub

    Private Shared _instance As ExeCOMServer = New ExeCOMServer
    Public Shared ReadOnly Property Instance() As ExeCOMServer
        Get
            Return ExeCOMServer._instance
        End Get
    End Property

#End Region

  
    Private syncRoot As Object = New Object  ' 锁定中的线程同步
    Private _bRunning As Boolean = False  ' 是否服务器正在运行

    ' 运行消息循环的线程ID
    Private _nMainThreadID As UInt32 = 0

    ' 服务器中的锁定计数器（记录已激活的COM对象数目
    Private _nLockCnt As Integer = 0

    ' 用于每5秒触发GC的定时器
    Private _gcTimer As Timer

    ''' <summary>
    ''' 这个方法实现了在COM服务器启动后每5秒触发一次GC。
    ''' </summary>
    ''' <param name="stateInfo"></param>
    Private Shared Sub GarbageCollect(ByVal stateInfo As Object)
        GC.Collect()  ' GC
    End Sub

    Private _cookieSimpleObj As UInt32


    ''' <summary>
    ''' PreMessageLoop负责注册COM类工厂，并且初始化COM服务器的关键成员变量
    '''（比如_nMainThreadID和_nLockCnt）。
    ''' </summary>
    Private Sub PreMessageLoop()

        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        ' 注册COM类工厂
        ' 

        Dim clsidSimpleObj As New Guid(VBSimpleObject.ClassId)

        ' 注册CSSimpleObject类对象 
        Dim hResult As Integer = COMNative.CoRegisterClassObject( _
        clsidSimpleObj, New VBSimpleObjectClassFactory, CLSCTX.LOCAL_SERVER, _
        REGCLS.SUSPENDED Or REGCLS.MULTIPLEUSE, Me._cookieSimpleObj)
        If (hResult <> 0) Then
            Throw New ApplicationException( _
            "CoRegisterClassObject failed w/err 0x" & hResult.ToString("X"))
        End If

        ' 注册其他类对象
        ' ...

        ' 通知SCM所有的已注册类以及在服务器进程中开始激活请求。
        hResult = COMNative.CoResumeClassObjects
        If (hResult <> 0) Then
            ' 在失败时撤销 CSSimpleObject的注册
            If (Me._cookieSimpleObj <> 0) Then
                COMNative.CoRevokeClassObject(Me._cookieSimpleObj)
            End If

            ' 撤销其他类的注册
            ' ...

            Throw New ApplicationException( _
            "CoResumeClassObjects failed w/err 0x" & hResult.ToString("X"))
        End If


        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        ' 初始化成员变量
        ' 

        ' 记录COM服务器当前运行中的线程ID。这样，服务器可以知道哪里可以发送
        ' WM_QUIT消息，以便退出消息循环。
        Me._nMainThreadID = NativeMethod.GetCurrentThreadId

        ' 记录在服务器中激活的COM对象的数目。当_nLockCnt为0时，服务器可以关闭。
        Me._nLockCnt = 0

        ' 启动GC计时器以触发每5秒一次的GC。
        Me._gcTimer = New Timer( _
        New TimerCallback(AddressOf ExeCOMServer.GarbageCollect), Nothing, _
        5000, 5000)

    End Sub


    ''' <summary>
    ''' RunMessageLoop运行标准的消息循环。该消息循环再收到WM_QUIT时退出。
    ''' </summary>
    Private Sub RunMessageLoop()
        Dim msg As MSG
        Do While NativeMethod.GetMessage(msg, IntPtr.Zero, 0, 0)
            NativeMethod.TranslateMessage((msg))
            NativeMethod.DispatchMessage((msg))
        Loop
    End Sub


    ''' <summary>
    ''' PostMessageLoop被用于撤销对服务器可见的COM类并且执行清理。
    ''' </summary>
    Private Sub PostMessageLoop()

        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        ' 撤销COM类的注册
        ' 

        ' 撤销CSSimpleObject的注册
        If (Me._cookieSimpleObj <> 0) Then
            COMNative.CoRevokeClassObject(Me._cookieSimpleObj)
        End If

        ' 撤销其他类
        ' ...


        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        ' 执行清理
        ' 

        ' 关闭GC计时器
        If (Not Me._gcTimer Is Nothing) Then
            Me._gcTimer.Dispose()
        End If

        ' 等待线程结束
        Thread.Sleep(1000)

    End Sub


    ''' <summary>
    ''' 运行COM服务器。如果服务器正在运行，此函数直接返回。
    ''' </summary>
    ''' <remarks>这个方法是线程安全的</remarks>
    Public Sub Run()
        SyncLock Me.syncRoot  ' 确认线程安全
            ' 如果服务器正在运行，直接返回
            If Me._bRunning Then
                Return
            End If
            ' 用于确认服务器是否正在运行
            Me._bRunning = True
        End SyncLock

        Try
            ' 执行PreMessageLoop以执行初始化成员变量和注册类工厂。
            Me.PreMessageLoop()

            ' 执行消息循环。
            Me.RunMessageLoop()

            ' 执行PostMessageLoop以便撤销注销
            Me.PostMessageLoop()
        Finally
            Me._bRunning = False
        End Try
    End Sub


    ''' <summary>
    ''' 增加锁定计数
    ''' </summary>
    ''' <returns>在数值增加后返回新的锁定计数器t</returns>
    ''' <remarks>此方法是线程安全的</remarks>
    Public Function Lock() As Integer
        Return Interlocked.Increment(Me._nLockCnt)
    End Function


    ''' <summary>
    ''' 减少锁定计数。当锁定计时器的值为0时，发送WM_QUIT消息，以便关闭COM服务器
    ''' </summary>
    ''' <returns>在数值减少后返回新的锁定计数器</returns>
    Public Function Unlock() As Integer
        Dim nRet As Integer = Interlocked.Decrement(Me._nLockCnt)

        ' 档计数为0时，终止服务器。
        If (nRet = 0) Then
            ' 为主线程发送一个WM_QUIT消息
            NativeMethod.PostThreadMessage( _
            _nMainThreadID, NativeMethod.WM_QUIT, UIntPtr.Zero, IntPtr.Zero)
        End If
        Return nRet
    End Function


    ''' <summary>
    ''' 返回当前锁定计数器
    ''' </summary>
    ''' <returns></returns>
    Public Function GetLockCount() As Integer
        Return Me._nLockCnt
    End Function

End Class