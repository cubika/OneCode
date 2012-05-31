========================================================================
                 WINDOWS 应用程序：VBExeCOMServer 项目概述
========================================================================

/////////////////////////////////////////////////////////////////////////////
用法：

VBExeCOMSever演示了一个基于本地服务可执行文件（exe）的进程外COM服务器。所有代码均
由Visual Basic .NET 编写。

VBExeCOMServer包含了以下项目：

1. VBSimpleObject

  Program ID: VBExeCOMServer.VBSimpleObject
  CLSID_VBSimpleObject: 3CCB29D4-9466-4f3c-BCB2-F5F0A62C2C3C
  IID__VBSimpleObject: 5EECE765-6416-467c-8D5E-C227F69E7EB7
  DIID___VBSimpleObjectEvents: 10C862E3-37E6-4e36-96FE-3106477235F1

  属性：
    ' With both get and set accessor methods
    ' 包括访问方法get和put 
    FloatProperty As Single


  方法：
    ' HelloWorld 返回一个字符串"HelloWorld"
    Function HelloWorld() As String
    ' GetProcessThreadID输出正在运行的进程ID和线程ID
    Sub GetProcessThreadID(ByRef processId As UInteger, 
                           ByRef threadId As UInteger)

  事件：
    
    ' FloatPropertyChanging在新的FloatProperty属性被设置之前触发。
    ' 客户端可以通过参数Cancel来取消对FloatProperty的修改。
    Event FloatPropertyChanging(ByVal NewValue As Single, 
                                ByRef Cancel As Boolean)


注意：如果你想在X64系统中发布此进程外COM服务器，您必须在生成项目时把项目属性
中的目标平台设置成X64或者X86。如果你使用默认值“Any CPU”，你将会发现当你的客户
端程序在创建COM对象时会失去响应长达2分钟，然后得到以下错误信息：

"Retrieving the COM class factory for component with CLSID {<clsid>} failed 
due to the following error: 80080005."

这是由于在X64位系统中“Any CPU”混淆了COM组件的激活路径。该可执行文件的PE头中
IMAGE_NT_HEADERS.FileHeader.Machine字段被设置成IMAGE_FILE_MACHINE_I386，
所以COM组件会认为启动的进程是32位的，而实际上所运行的进程是64位的。


/////////////////////////////////////////////////////////////////////////////
项目间关系:

VBExeCOMServer - CSExeCOMServer - ATLExeCOMServer

CSExeCOMServer, VBExeCOMServer 和 ATLExeCOMServer, 是由不同语言实现的，形式为
本地可执行文件（EXE）的进程外COM服务器。

VBExeCOMServer - VBCOMService - VBDllCOMServer

以上所有COM组件均由Visual Basic .NET实现。VBExeCOMServer是一个进程外组件，其形式
为作为本地服务器的可执行文件（EXE）。VBCOMService是一个进程外组件，其形式为提供本
地Windows启动时后台运行的服务（EXE）。 VBDllCOMServer是一个进程内组件，其形式为
动态链接库（DLL）。


/////////////////////////////////////////////////////////////////////////////
部署：

A、安装

Regasm.exe VBExeCOMServer.exe
此命令注册VBExeCOMServer.exe中所有COM可见（COM-visible）的类型。

B、移除
Regasm.exe /u VBExeCOMServer.exe
此命令注销了VBExeCOMServer.exe中的COM可见（COM-visible）类型。

注意：如果你想在X64系统中发布此进程外COM服务器，您必须在生成项目时把项目属性
中的目标平台设置成X64或者X86。如果你使用默认值“Any CPU”，你将会发现当你的客户
端程序在创建COM对象时会失去响应长达2分钟，然后得到以下错误信息：

"Retrieving the COM class factory for component with CLSID {<clsid>} failed 
due to the following error: 80080005."


/////////////////////////////////////////////////////////////////////////////
创建过程:

A、创建项目

步骤1：在Visual Studio 2008中创建一个Visual Basic Windows控制台应用程序，并把其命
名为VBExeCOMServer。

步骤2：打开项目的属性，在应用程序选项页中把输出类型改变为Windows应用程序。
这避免了当可执行文件执行时启动控制台。

B、添加COMHelper类

COMHelper提供了用于注册和注销COM服务器的helper函数。它还封装了一些在.NET环境下
使用的原生的COM API。

C、添加ExeCOMServer类

ExeCOMServer封装了VB.NET中线程外COM服务器的框架。这个类实现了单态（Singleton）设
计模式，并且它是线程安全的。执行VBExeCOMServer.Instance.Run()以启动服务器。如果
服务器正在运行，此函数将立即返回。在Run方法中，它注册了COM服务器所公开的COM类中类
组件。并且启动消息循环以等待锁定计数器回落至0。当锁定计数器为0时，它撤销注册并退
出服务器。

当一个COM对象被创建时，服务器的锁定计数器将会增加。当一个对象被释放时（被GC时），
锁定计数器会减少。为确保COM对象会被即时的垃圾回收（GC），ExeCOMServer在服务器启动
后每隔5秒钟触发一次垃圾回收。

D、添加组建VBSimpleObject

步骤1、添加一个public类VBSimpleObject。

步骤2、在VBSimpleObject类中，定义Class ID, Interface ID, 和Event ID:

	ClassId As String = "805303FE-B5A6-308D-9E4F-BF500978AEEB"
    InterfaceId As String = "90E0BCEA-7AFA-362A-A75E-6D07C1C6FC4B"
    EventsId As String = "72D3EFB2-0D88-4ba7-A26B-8FFDB92FEBED"

步骤3、在VBSimpleObject中添加ComClassAttribute， 并且指定_ClassID, 
_InterfaceID, 和 _EventID为以上定值。

    <ComClass(VBSimpleObject.ClassId, VBSimpleObject.InterfaceId, _
        VBSimpleObject.EventsId), ComVisible(True)> _
    Public Class VBSimpleObject

步骤4、声明一个ReferenceCountedObject类。这个类将负责在COM服务器的构造函数中递增
锁定计数器，并在析构函数（Finalize）中减少锁定计数器。同时VBSimpleObject类继承
ReferenceCountedObject类。

步骤5、在组建中添加一个属性（FloatProperty），两个方法（HelloWorld, 
GetProcessThreadID）和一个事件(FloatPropertyChanging)。

E、在注册表中注册VBSimpleObject。

COM服务器要求额外的注册表键和值。在使用Regasm，exe注册COM时，默认的COM注册方法只
支持形式为动态链接库的进程内服务器。为了使其注册为本地服务，我们将自定义注册过程。
这将使InprocServer32改变为LocalServer。

步骤1、在VBSimpleObject中，添加Register和Unregister函数，并且定义其属性分别为
ComRegisterFunctionAttribute和ComUnregisterFunctionAttribute。自定义的过程将会在
默认的注册过程之后被执行。Register和Unregister函数执行了COMHelper中的helper方法。

F、注册VBSimpleObject的类工厂

步骤1、为VBSimpleObject创造一个工厂类。这个类将实现IClassFactory接口。

''' <summary>
''' 为VBSimpleObject创造的一个工厂类。
''' </summary>
Friend Class VBSimpleObjectClassFactory
    Implements IClassFactory

    Public Function CreateInstance(ByVal pUnkOuter As IntPtr, ByRef riid As Guid, _
                                   <Out()> ByRef ppvObject As IntPtr) As Integer _
                                   Implements IClassFactory.CreateInstance
        ppvObject = IntPtr.Zero

        If (pUnkOuter <> IntPtr.Zero) Then
            ' 参数pUnkOuter是一个非空变量，并且此对象不支持聚合。
            Marshal.ThrowExceptionForHR(COMNative.CLASS_E_NOAGGREGATION)
        End If

        If ((riid = New Guid(VBSimpleObject.ClassId)) OrElse _
            (riid = New Guid(COMNative.GuidIUnknown))) Then
            ' 创建一个.NET对象的实例
            ppvObject = Marshal.GetComInterfaceForObject( _
            New VBSimpleObject, GetType(VBSimpleObject).GetInterface("_VBSimpleObject"))
        Else
            ' 被ppvObject所指向的对象不被此riid的接口支持。
            Marshal.ThrowExceptionForHR(COMNative.E_NOINTERFACE)
        End If

        Return 0  ' S_OK
    End Function


    Public Function LockServer(ByVal fLock As Boolean) As Integer _
    Implements IClassFactory.LockServer
        Return 0  ' S_OK
    End Function

End Class

步骤2、在服务器启动时（在ExeCOMServer的PreMessageLoop方法中），使用标准的
CoRegisterClassObject API为VBSimpleObject注册类工厂。
注意：使用PInovk CoRegisterClassObject来注册COM对象并不被支持。

	' 启动时注册VBSimpleObject类对象
	Dim hResult As Integer = COMNative.CoRegisterClassObject( _
    clsidSimpleObj, New VBSimpleObjectClassFactory, CLSCTX.LOCAL_SERVER, _
    REGCLS.SUSPENDED Or REGCLS.MULTIPLEUSE, Me._cookieSimpleObj)

步骤3，在服务器终止时（在ExeCOMServer的PostMessageLoop方法中），使用CoRevokeClassObject API
注销VBSimpleObject。

	COMNative.CoRevokeClassObject(Me._cookieSimpleObj)

G、配置和生成项目（COM本地服务器）

步骤1、打开项目的属性页，转到生成事件。

步骤2、在生成后事件命令行中输入以下命令：

	Regasm.exe "$(TargetPath)"

此命令在注册表中注册此COM可视类型（例如： VBSimpleObject）

/////////////////////////////////////////////////////////////////////////////
参考资料：

Building COM Servers in .NET 
http://www.codeproject.com/KB/COM/BuildCOMServersInDotNet.aspx


/////////////////////////////////////////////////////////////////////////////
