========================================================================
	             WINDOWS 应用程序：CSExeCOMServer 项目概述
========================================================================

/////////////////////////////////////////////////////////////////////////////
用法：

CSExeCOMSever演示了一个基于本地服务可执行文件（exe）的进程外COM服务器。所有代
码均由Visual C#编写。

CSExeCOMServer包含了以下项目：

1. CSSimpleObject

  （在编写您自己的COM服务器时，请生成新的GUID）
  
  Program ID: CSExeCOMServer.CSSimpleObject
  CLSID_CSSimpleObject: DB9935C1-19C5-4ed2-ADD2-9A57E19F53A3
  IID_ICSSimpleObject: 941D219B-7601-4375-B68A-61E23A4C8425
  DIID_ICSSimpleObjectEvents: 014C067E-660D-4d20-9952-CD973CE50436

  属性：
    // 包括访问方法get和put 
    float FloatProperty

  方法：
    // HelloWorld 返回一个字符串"HelloWorld"
    string HelloWorld();
    // GetProcessThreadID输出正在运行的进程ID和线程ID
    void GetProcessThreadID(out uint processId, out uint threadId);

  事件：
    // FloatPropertyChanging在新的FloatProperty属性被设置之前触发。
    // 客户端可以通过参数Cancel来取消对FloatProperty的修改。
    void FloatPropertyChanging(float NewValue, ref bool Cancel);

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

CSExeCOMServer - VBExeCOMServer - ATLExeCOMServer

CSExeCOMServer, VBExeCOMServer 和 ATLExeCOMServer, 是由不同语言实现的，形式为
本地可执行文件（EXE）的进程外COM服务器。

CSExeCOMServer - CSCOMService - CSDllCOMServer

以上所有COM组件均由Visual C#实现。CSExeCOMServer是一个进程外组件，其形式为作为本
地服务器的可执行文件（EXE）。CSCOMService是一个进程外组件，其形式为提供本地Windows
启动时后台运行的服务（EXE）。 CSDllCOMServer是一个进程内组件，其形式为动态链接库（DLL）。


/////////////////////////////////////////////////////////////////////////////
部署：

A、安装

Regasm.exe CSExeCOMServer.exe
此命令注册CSExeCOMServer.exe中所有COM可见（COM-visible）的类型。

B、移除
Regasm.exe /u CSExeCOMServer.exe
此命令注销了CSExeCOMServer.exe中的COM可见（COM-visible）类型。

注意：如果你想在X64系统中发布此进程外COM服务器，您必须在生成项目时把项目属性
中的目标平台设置成X64或者X86。如果你使用默认值“Any CPU”，你将会发现当你的客户
端程序在创建COM对象时会失去响应长达2分钟，然后得到以下错误信息：


"Retrieving the COM class factory for component with CLSID {<clsid>} failed 
due to the following error: 80080005."


/////////////////////////////////////////////////////////////////////////////
创建过程:

A、创建项目

步骤1：在Visual Studio 2008中创建一个Visual C# Windows控制台应用程序，并把其命名
为CSExeCOMServer。

步骤2：打开项目的属性，在应用程序选项页中把输出类型改变为Windows应用程序。
这避免了当可执行文件执行时启动控制台。

B、添加COMHelper类

COMHelper提供了用于注册和注销COM服务器的helper函数。它还封装了一些在.NET环境下
使用的Native COM API。

C、添加ExeCOMServer类

ExeCOMServer封装了C#中线程外COM服务器的框架。这个类实现了单态（Singleton）设计
模式，并且它是线程安全的。执行CSExeCOMServer.Instance.Run()以启动服务器。如果
服务器正在运行，此函数将立即返回。在Run方法中，它注册了COM服务器所公开的COM类中类
组件。并且启动消息循环以等待锁定计数器回落至0。当锁定计数器为0时，它撤销注册并退
出服务器。

当一个COM对象被创建时，服务器的锁定计数器将会增加。当一个对象被释放时（被GC时），
锁定计数器会减少。为确保COM对象会被即时的垃圾回收（GC），ExeCOMServer在服务器启动后
每隔5秒钟触发一次垃圾回收。

D、添加COM可见类CSSimpleObject

步骤1、定义一个“public”的COM可见的接口ICSSimpleObject。它被用于描述从类的COM接口。
使用Guid属性来指定其GUID（IID）。

	[Guid("941D219B-7601-4375-B68A-61E23A4C8425"), ComVisible(true)]

如此，COM对象的IID将是一个定值。默认情况下，一个.NET类所使用的接口可以转化为IDL中
的双接口[InterfaceType(ComInterfaceType.InterfaceIsDual)]。将使客户端获得最佳的早
绑定和晚绑定。其他选项为 [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]和 
[InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]。

步骤2、在ICSSimpleObject接口中，定义要被导出的属性和方法的原型。

步骤3、定义一个“public”的COM可见的接口ICSSimpleObjectEvents。它被用于描述该COM
组件中导出的事件。使用Guid属性来指定其GUID（事件ID）。

	[Guid("014C067E-660D-4d20-9952-CD973CE50436"), ComVisible(true)]

将此接口定义为一个IDispatch接口：

	[InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]

步骤4、在ICSSimpleObjectEvent接口中，定义要被导出的属性和方法的原型。

步骤5、声明一个ReferenceCountedObject类。这个类将负责在COM服务器的构造函数中递增锁定
计数器，并在析构函数（Finalize）中减少锁定计数器。

步骤6、定义一个“public”的COM可见的类CSSimpleObject。这个类继承于ReferenceCountedObject
并实现了ICSSimpleObject接口。为其添加一个[ClassInterface(ClassInterfaceType.None)]
属性。这个属性告诉类型库生成工具：我们并不要求一个类接口。这也保证了ICSSimpleObject
接口是该COM组件的默认接口。此外，使用Guid属性来指定这个类的GUID（CLSID）。

	[Guid("DB9935C1-19C5-4ed2-ADD2-9A57E19F53A3"), ComVisible(true)]

如此，COM对象的CLSID将是一个定值。最后为此类定义一个ComSourceInterface属性：

	[ComSourceInterfaces(typeof(ICSSimpleObjectEvents))]

ComSourceInterfaces指定了一份从COM事件源导出的接口列表

步骤7，确保CSSimpleObject的构造函数并不是一个private函数。（我们可以添加一个public
构造函数或者使用默认的构造函数）由此，COM对象可以从客户端程序中创建。

步骤8、在CSSimpleObject中，实现ICSSimpleObject接口。其中包括编写FloatProperty属性
的主体部分以及HelloWorld方法和GetProcessThreadID方法。

E、在注册表中注册CSSimpleObject。

COM服务器要求额外的注册表键和值。在使用Regasm.exe注册COM时，默认的COM注册方法只支持
形式为动态链接库的进程内服务器。为了使其注册为本地服务，我们将自定义注册过程。这将
使InprocServer32改变为LocalServer。

步骤1、在CSSimpleObject中，添加Register和Unregister函数，并且定义其属性分别为
ComRegisterFunctionAttribute和ComUnregisterFunctionAttribute。自定义的过程将会在
默认的注册过程之后被执行。Register和Unregister函数执行了COMHelper中的helper方法。

F、注册CSSimpleObject的类工厂

步骤1、为CSSimpleObject创造一个工厂类。这个类将实现IClassFactory接口。

	/// <summary>
	/// 为CSSimpleObject创造的一个工厂类。
	/// </summary>
	internal class CSSimpleObjectClassFactory : IClassFactory
	{
		public int CreateInstance(IntPtr pUnkOuter, ref Guid riid, 
			out IntPtr ppvObject)
		{
			ppvObject = IntPtr.Zero;

			if (pUnkOuter != IntPtr.Zero)
                Marshal.ThrowExceptionForHR(COMNative.CLASS_E_NOAGGREGATION);

			if (riid == new Guid(CSSimpleObject.ClassId) || 
				riid == new Guid(COMNative.GuidIUnknown))
				// 创建一个.NET对象的实例
				ppvObject = Marshal.GetComInterfaceForObject(
					new CSSimpleObject(), typeof(ICSSimpleObject));
			else
				// 被ppvObject所指向的对象不被此riid的接口支持。
				Marshal.ThrowExceptionForHR(COMNative.E_NOINTERFACE);
			return 0;   // S_OK
		}

		public int LockServer(bool fLock)
		{
			return 0;   // S_OK
		}
	}

步骤2、在服务器启动时（在ExeCOMServer的PreMessageLoop方法中），使用标准的
CoRegisterClassObject API为CSSimpleObject注册类工厂。 
注意:PInvoke CoRegisterClassObject来注册COM对象并不被支持。

	// 启动时注册CSSimpleObject类对象
	int hResult = COMNative.CoRegisterClassObject(
		ref clsidSimpleObj,                 //注册CLSID
		new CSSimpleObjectClassFactory(),   //类工厂
		CLSCTX.LOCAL_SERVER,                //运行环境
		REGCLS.MULTIPLEUSE, 
		out _cookie);

步骤3，在服务器终止时（在ExeCOMServer的PostMessageLoop方法中），
使用CoRevokeClassObject API注销CSSimpleObject。

	COMNative.CoRevokeClassObject(_cookie);

G、配置和生成项目（COM本地服务器）

步骤1、打开项目的属性页，转到生成事件。

步骤2、在生成后事件命令行中输入以下命令：

	Regasm.exe "$(TargetPath)"

此命令在注册表中注册此COM可视类型（例如： CSSimpleObject）


/////////////////////////////////////////////////////////////////////////////
参考资料：

Building COM Servers in .NET 
http://www.codeproject.com/KB/COM/BuildCOMServersInDotNet.aspx


/////////////////////////////////////////////////////////////////////////////
