'********************************* 模块头 **********************************'
' 模块名:  HotKeyRegister.vb
' 项目名:  VBRegisterHotkey
' 版权(c)  Microsoft Corporation.
' 
' 该类引入了user32.dll的RegisterHotKey和UnregisterHotKey方法用来定义和取消系统的热键。
' 
' Application.AddMessageFilter方法被用于添加一个信息过滤器，添加的目的是为了监控窗体
' 信息，因为它们是被选到它们的目的地。在一条信息被分配之前，PreFilterMessage方法能处理
' 它，如果一条WM_HOTKEY信息被热键创建成，但是它已经被HotKeyRegister对象注册过，那么就
' 会触发一个HotKeyPressed事件。 
' 
' 该类也支持静态方法GetModifiers从KeyEventArgs的KeyData属性获得修饰符和关键词.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*************************************************************************'

Imports System.Runtime.InteropServices
Imports System.Security.Permissions


Public Class HotKeyRegister
    Implements IMessageFilter, IDisposable

    ''' <summary>
    ''' 定义一个系统的热键.
    ''' </summary>
    ''' <param name="hWnd">
    ''' 带有句柄的窗口会收到WM_HOTKEY消息，该消息是由热键产生的。 如果参数为空，
    ''' WM_HOTKEY消息将被指派到被调用的线程的消息队列中，而且在消息循环中必须被
    ''' 处理。
    ''' </param>
    ''' <param name="id">
    ''' 热键的标识符。如果hWnd为空,那么热键适合当前线程联系在一块的而不是和一个
    ''' 特别的窗体。如果一个热键利用了相同的hWnd和id参数，采取的措施看备注。
    ''' </param>
    ''' <param name="fsModifiers">
    ''' 被按下的按键要求必须是uVirtKey参数规定的按键，也是为了生成WM_HOTKEY消息。
    ''' fsModifiers参数能够结合下面的值。 
    ''' MOD_ALT     0x0001
    ''' MOD_CONTROL 0x0002
    ''' MOD_SHIFT   0x0004
    ''' MOD_WIN     0x0008
    ''' </param>
    ''' <param name="vk">热键的虚拟键控代码。</param>
    <DllImport("user32.dll", CharSet:=CharSet.Auto, SetLastError:=True)>
    Private Shared Function RegisterHotKey(ByVal hWnd As IntPtr,
                                           ByVal id As Integer,
                                           ByVal fsModifiers As KeyModifiers,
                                           ByVal vk As Keys) As Boolean
    End Function

    ''' <summary>
    ''' 通过调用线程提前取消一个热键。
    ''' </summary>
    ''' <param name="hWnd">
    ''' 含有一个句柄的窗口和热键的取消是联系在一块的，如果热键不能联系窗口，
    ''' 那么参数将为空。
    ''' </param>
    ''' <param name="id">
    ''' 热键的标识符被取消。
    ''' </param>
    <DllImport("user32.dll", CharSet:=CharSet.Auto, SetLastError:=True)>
    Private Shared Function UnregisterHotKey(ByVal hWnd As IntPtr,
                                             ByVal id As Integer) As Boolean
    End Function

    ''' <summary>
    ''' 从KeyEventArgs的KeyData属性获得编辑器和按键。
    ''' </summary>
    ''' <param name="keydata">
    ''' KeyEventArgs的KeyData属性。KeyData是一种和编辑器结合的键。
    ''' </param>
    ''' <param name="key">按下的键</param>
    Public Shared Function GetModifiers(ByVal keydata As Keys,
                                        ByRef key As Keys) As KeyModifiers

        key = keydata
        Dim modifers As KeyModifiers = KeyModifiers.None

        ' 检查keydata是否包含CTRL修饰符。
        ' Keys.Control的值是131072。
        If (keydata And Keys.Control) = Keys.Control Then
            modifers = modifers Or KeyModifiers.Control

            key = keydata Xor Keys.Control
        End If

        ' 检查keydata是否包含SHIFT修饰符。
        ' Keys.Control的值是65536。
        If (keydata And Keys.Shift) = Keys.Shift Then
            modifers = modifers Or KeyModifiers.Shift
            key = key Xor Keys.Shift
        End If

        ' 检查keydata是否包含ALT修饰符。
        ' Keys.Control的值是262144。
        If (keydata And Keys.Alt) = Keys.Alt Then
            modifers = modifers Or KeyModifiers.Alt
            key = key Xor Keys.Alt
        End If

        ' 检查是否有除了SHIFT, CTRL或者ALT按键(菜单)是被按下的。
        If key = Keys.ShiftKey OrElse key = Keys.ControlKey _
            OrElse key = Keys.Menu Then
            key = Keys.None
        End If

        Return modifers
    End Function

    ''' <summary>
    ''' 指定对象是否被处理了。
    ''' </summary>
    Private _disposed As Boolean = False

    ''' <summary>
    '''  如果你安装了Windows SDK，在WinUser.h中常量能被发现。
    '''  每个窗口都有一个标识符，0x0312意味着这条消息是一条 
    '''  WM_HOTKEY消息。
    ''' </summary>
    Private Const WM_HOTKEY As Integer = &H312

    ''' <summary>
    ''' 带一个句柄的窗口将收到热键产生的WM_HOTKEY消息。
    ''' </summary>
    Private _handle As IntPtr
    Public Property Handle() As IntPtr
        Get
            Return _handle
        End Get
        Private Set(ByVal value As IntPtr)
            _handle = value
        End Set
    End Property

    Private _id As Integer
    Public Property ID() As Integer
        Get
            Return _id
        End Get
        Private Set(ByVal value As Integer)
            _id = value
        End Set
    End Property

    Private _modifiers As KeyModifiers
    Public Property Modifiers() As KeyModifiers
        Get
            Return _modifiers
        End Get
        Private Set(ByVal value As KeyModifiers)
            _modifiers = value
        End Set
    End Property

    Private _key As Keys
    Public Property Key() As Keys
        Get
            Return _key
        End Get
        Private Set(ByVal value As Keys)
            _key = value
        End Set
    End Property


    ''' <summary>
    ''' 当热键被按下的时候触发事件。
    ''' </summary>
    Public Event HotKeyPressed As EventHandler

    Public Sub New(ByVal handle As IntPtr, ByVal id As Integer,
                   ByVal modifiers As KeyModifiers, ByVal key As Keys)
        If key = Keys.None OrElse modifiers = KeyModifiers.None Then
            Throw New ArgumentException("键或者编辑器不能为空。")
        End If

        Me.Handle = handle
        Me.ID = id
        Me.Modifiers = modifiers
        Me.Key = key

        RegisterHotKey()

        ' 添加一个消息过滤器来监督窗体信息，因为它们是由目标选择路径的。
        Application.AddMessageFilter(Me)

    End Sub


    ''' <summary>
    ''' 注册热键。
    ''' </summary>
    Private Sub RegisterHotKey()
        Dim isKeyRegisterd As Boolean = RegisterHotKey(Handle, ID, Modifiers, Key)

        ' 如果操作失败，如果线程先前已经被注册了，试着注销热键。
        If Not isKeyRegisterd Then

            ' IntPtr.Zero意思是热键被线程注册。
            UnregisterHotKey(IntPtr.Zero, ID)

            ' 试着再次注册热键。
            isKeyRegisterd = RegisterHotKey(Handle, ID, Modifiers, Key)

            ' 如果操作仍然失败，这就意味着该热键已经在另外一个线程或者进程中利用了。
            If Not isKeyRegisterd Then
                Throw New ApplicationException("热键已经被利用")
            End If
        End If
    End Sub


    ''' <summary>
    ''' 在消息被派遣之前过滤出一条消息。
    ''' </summary>
    <PermissionSetAttribute(SecurityAction.LinkDemand, Name:="FullTrust")>
    Public Function PreFilterMessage(ByRef m As Message) As Boolean _
    Implements IMessageFilter.PreFilterMessage
        '  Message的WParam属性典型的是被用来存储一小块信息。在此，它存储的是地址。
        If m.Msg = WM_HOTKEY AndAlso m.HWnd = Me.Handle _
            AndAlso m.WParam = New IntPtr(Me.ID) Then

            ' 如果它是一条WM_HOTKEY信息，引发HotKeyPressed事件。
            RaiseEvent HotKeyPressed(Me, EventArgs.Empty)

            ' 准确的去过滤消息并且阻止它被调度。
            Return True
        End If

        '  返回false去允许消息继续道下一个过滤器或者控制器。
        Return False
    End Function


    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub


    ''' <summary>
    ''' 注销热键。
    ''' </summary>
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        ' 阻止被多次调用。
        If _disposed Then
            Return
        End If

        If disposing Then

            ' 从从消息泵的应用程序中清除一个消息过滤器。
            Application.RemoveMessageFilter(Me)

            UnregisterHotKey(Handle, ID)
        End If

        _disposed = True
    End Sub

End Class
