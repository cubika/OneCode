'********************************* 模块头 **********************************'
' 模块名:    COMHelper.vb
' 项目名:      VBExeCOMServer
' 版权 (c) Microsoft Corporation.
' 
' COMHelper提供了用于注册和注销COM服务器的helper函数。它还封装了一些在.NET环境下
' 使用的Native COM API。
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

Imports Microsoft.Win32
Imports System.Runtime.InteropServices
Imports System.Reflection

#End Region


Friend Class COMHelper

    ''' <summary>
    ''' 注册组件为本地服务器
    ''' </summary>
    ''' <param name="t"></param>
    Public Shared Sub RegasmRegisterLocalServer(ByVal t As Type)
        COMHelper.GuardNullType(t, "t")  ' 检查参数

        ' 打开组件的CLSID键值
        Using keyCLSID As RegistryKey = Registry.ClassesRoot.OpenSubKey( _
        "CLSID\" & t.GUID.ToString("B"), True)

       
            ' 在注册之后移除自动生成的InprocServer32键值
            ' (REGASM将其设置在这里，但是我们要制作进程外的服务器组件)
            keyCLSID.DeleteSubKeyTree("InprocServer32")

            ' 在CLSID键中创建"LocalServer32"
            Using subkey As RegistryKey = keyCLSID.CreateSubKey("LocalServer32")
                subkey.SetValue("", Assembly.GetExecutingAssembly.Location, _
                                RegistryValueKind.String)
            End Using
        End Using
    End Sub


    ''' <summary>

    ''' 注销组件
    ''' </summary>
    ''' <param name="t"></param>
    Public Shared Sub RegasmUnregisterLocalServer(ByVal t As Type)
        COMHelper.GuardNullType(t, "t")  '检查参数

        ' 删除组件的CLSID键
        Registry.ClassesRoot.DeleteSubKeyTree(("CLSID\" & t.GUID.ToString("B")))
    End Sub


    Private Shared Sub GuardNullType(ByVal t As Type, ByVal param As String)
        If (t Is Nothing) Then
            Throw New ArgumentException("The CLR type must be specified.", param)
        End If
    End Sub

End Class


Friend Class COMNative

    ''' <summary>
    ''' CoInitializeEx()用于设置每个线程的单元模式。
    ''' </summary>
    ''' <param name="pvReserved">必须为空（NULL）</param>
    ''' <param name="dwCoInit">
    ''' 该线程的并发模式和初始化选项
    ''' </param>
    ''' <returns></returns>
    <DllImport("ole32.dll")> _
    Public Shared Function CoInitializeEx(ByVal pvReserved As IntPtr, ByVal dwCoInit As UInt32) As Integer
    End Function


    ''' <summary>
    ''' CoUninitialize() 用于卸载一个COM线程。
    ''' </summary>
    ''' <remarks></remarks>
    <DllImport("ole32.dll")> _
    Public Shared Sub CoUninitialize()
    End Sub


    ''' <summary>
    ''' 如果注册了一个拥有OLE的EXE类对象，它就可以被其他程序连接。EXE对象程序需要
    ''' 在启动时执行CoRegisterClassObject。并且它仍然需要组册其内部对象以供相同的
    ''' EXE程序或这个程序使用的其他代码块，例如动态链接库（DLL）。
    ''' </summary>
    ''' <param name="rclsid">用于注册的CLSID</param>
    ''' <param name="pUnk">
    ''' 指向可以被发布的类对象上的IUnknow接口。
    ''' </param>
    ''' <param name="dwClsContext">
    ''' 可执行代码的主体内容。
    ''' </param>
    ''' <param name="flags">
    ''' 如何连接至类对象。
    ''' </param>
    ''' <param name="lpdwRegister">
    ''' 指向一个用于确认该类对象已经被注册的值。 
    ''' </param>
    ''' <returns></returns>
    ''' <remarks>
    ''' 在CoRegisterClassObject中使用P/Invoke注册COM对象是不被支持的。
    ''' </remarks>
    <DllImport("ole32.dll")> _
    Public Shared Function CoRegisterClassObject( _
    ByRef rclsid As Guid, <MarshalAs(UnmanagedType.Interface)> ByVal pUnk As IClassFactory, _
    ByVal dwClsContext As CLSCTX, ByVal flags As REGCLS, <Out()> ByRef lpdwRegister As UInt32) _
    As Integer
    End Function


    ''' <summary>
    ''' 注意：OLE是一个类对象。在此之前它和CoRegisterClassObject函数一起注册。
    ''' 现在已经不需要再使用了。
    ''' </summary>
    ''' <param name="dwRegister">
    ''' CoRegisterClassObject函数之前所返回的标识变量
    ''' </param>
    ''' <returns></returns>
    <DllImport("ole32.dll")> _
    Public Shared Function CoRevokeClassObject(ByVal dwRegister As UInt32) As UInt32
    End Function


    ''' <summary>
    ''' 一个服务器可以注册多个类对象时，此函数被执行。其用于通知SCM关于所有的已注
    ''' 册类和允许所有这些类对象的激活请求。
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' 此服务器可以使用CoResumeClassObjects而注册多个类对象。 在第一次执行CoRegisterClassObject
    ''' 后，为每一个服务器支持的CLSID指定REGCLS_LOCAL_SERVER或REGCLS_SUSPENDED。
    ''' 这个函数使OLE通知SCM所有的已注册类以及在服务器进程中开始激活请求。
    ''' 
    ''' 
    ''' 此方法中，无论多少个已注册的CLSID，对SCM仅使用一次执行代码， 这大大缩短了
    ''' 总体注册的时间和服务器启动时间。另外一个好处是，如果服务器有多重单元，即不
    ''' 同的已注册CLSID在不同的单元中，或则是无限制的线程服务器， 在服务器执行
    ''' CoResumeClassObjects之前不会有新的激活请求。这使得服务器在不得不处理激活和
    ''' 关闭请求之前注册所有其CLSID和正常启动。
    ''' </remarks>
    <DllImport("ole32.dll")> _
    Public Shared Function CoResumeClassObjects() As Integer
    End Function


    ''' <summary>
    ''' IClassFactory的接口ID
    ''' </summary>
    ''' <remarks></remarks>
    Public Const IID_IClassFactory As String = "00000001-0000-0000-C000-000000000046"

    ''' <summary>
    ''' IDispatch的接口ID
    ''' </summary>
    ''' <remarks></remarks>
    Public Const IID_IDispatch As String = "00020400-0000-0000-C000-000000000046"

    ''' <summary>
    ''' IUnknown的接口ID
    ''' </summary>
    ''' <remarks></remarks>
    Public Const IID_IUnknown As String = "00000000-0000-0000-C000-000000000046"

    ''' <summary>
    ''' 此类不支持聚合（aggregation）或则远程操作此类对象
    ''' </summary>
    ''' <remarks></remarks>
    Public Const CLASS_E_NOAGGREGATION As Integer = &H80040110

    ''' <summary>
    ''' 没所支持的接口
    ''' </summary>
    ''' <remarks></remarks>
    Public Const E_NOINTERFACE As Integer = &H80004002

End Class


''' <summary>
''' 所有在系统注册表中注册的和拥有你所分配CLSID的类必须实现这个接口。
''' 这样这个类的对象才能被创建。
''' http://msdn.microsoft.com/en-us/library/ms694364.aspx
''' </summary>
<ComImport(), ComVisible(False), _
InterfaceType(ComInterfaceType.InterfaceIsIUnknown), _
Guid("00000001-0000-0000-C000-000000000046")> _
Friend Interface IClassFactory

    ''' <summary>
    ''' 创建一个卸载对象.
    ''' </summary>
    ''' <param name="pUnkOuter"></param>
    ''' <param name="riid">
    ''' 此引用变量用于确认这个接口被用于和新创建的对象间的通讯。如果pUnkOuter为
    ''' NULL， 这个变量为初始化接口时所频繁使用的IID。
    ''' </param>
    ''' <param name="ppvObject">
    ''' 用于接收于riid中请求的接口指针的指针变量的地址。
    ''' </param>
    ''' <returns>S_OK为成功。</returns>
    <PreserveSig()> _
    Function CreateInstance(ByVal pUnkOuter As IntPtr, ByRef riid As Guid, _
                            <Out()> ByRef ppvObject As IntPtr) As Integer


    ''' <summary>
    ''' 锁定内存中已打开的对象应用程序。
    ''' </summary>
    ''' <param name="fLock">
    ''' 如果为TRUE，增加锁定计数器；
    ''' 如果为FALSE，减少锁定计数器。
    ''' </param>
    ''' <returns>S_OK为成功。</returns>
    <PreserveSig()> _
    Function LockServer(ByVal fLock As Boolean) As Integer

End Interface


''' <summary>
''' 在CLSCTX枚举中的值是用于在在执行激活时确认在一个对象中那一段可执行代码被执行。
''' 这些值还被用于执行CoRegisterClassObject时确认在一个有效的类对象中请求构造实例
''' 的可执行代码块。
''' </summary>
<Flags()> _
Friend Enum CLSCTX As UInt32
    INPROC_SERVER = &H1
    INPROC_HANDLER = &H2
    LOCAL_SERVER = &H4
    INPROC_SERVER16 = &H8
    REMOTE_SERVER = &H10
    INPROC_HANDLER16 = &H20
    RESERVED1 = &H40
    RESERVED2 = &H80
    RESERVED3 = &H100
    RESERVED4 = &H200
    NO_CODE_DOWNLOAD = &H400
    RESERVED5 = &H800
    NO_CUSTOM_MARSHAL = &H1000
    ENABLE_CODE_DOWNLOAD = &H2000
    NO_FAILURE_LOG = &H4000
    DISABLE_AAA = &H8000
    ENABLE_AAA = &H10000
    FROM_DEFAULT_CONTEXT = &H20000
    ACTIVATE_32_BIT_SERVER = &H40000
    ACTIVATE_64_BIT_SERVER = &H80000
End Enum


''' <summary>
''' 在REGCLS枚举中定义了用于CoRegisterClassObject中控制连接类型的值。
''' 这个值用于确定如何连接到一个类对象的连接方法。
''' </summary>
''' <remarks></remarks>
<Flags()> _
Friend Enum REGCLS As UInt32
    SINGLEUSE = 0
    MULTIPLEUSE = 1
    MULTI_SEPARATE = 2
    SUSPENDED = 4
    SURROGATE = 8
End Enum