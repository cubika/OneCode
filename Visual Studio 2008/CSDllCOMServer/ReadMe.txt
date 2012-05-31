========================================================================
              库应用程序：CSDllCOMServer项目概述
========================================================================

/////////////////////////////////////////////////////////////////////////////
摘要：

这个Visual C# 示例着重于使用COM技术导出.Net Framework组件。这个允许我们为COM
开发人员编写一个.Net类型并且在非托管代码中使用此类型。

基本上，定义一个组件接口中有三个方法：

1、 显式定义类接口
[ClassInterface(ClassInterfaceType.None)] 

如果你将导出.Net类到一个COM客户端，这是一个值得推荐的方法。CSSimpleObject中的代
码演示了这个方法。

2、隐式定义类接口
[ClassInterface(ClassInterfaceType.AutoDual)] 或 
[ClassInterface(ClassInterfaceType.AutoDispatch)]。

使用ClassInterface导出一个.Net类的公共方法，这是一个通常不推荐的做法。这是由于
它违反了COM的版本控制规则。此方法不在此示例中演示。

3、使用Microsoft.VisualBasic.ComClassAttribute。

ComClassAttribute属性使编译器添加元数据，这将允许一个类作为一个COM对象被导出。
这简化了从Visual Basic中导出COM对象的过程。如果没有ComClassAttribute，你需要好
几个步骤以便从Visual Basic中创建一个COM组件。一旦一个类被标记为ComClassAttribute，
编译器会自动执行这些额外的操作。示例VBDllCOMServer演示了这个属性的用法。Visual C#
也可从中得益，只要我们在项目中引用了Microsoft.VisualBasic程序集。这个方法将不会
在示例中演示。

CSDllCOMServer导出了以下组件：

CSSimpleObject，此类的接口被显式定义。

  Program ID: CSDllCOMServer.CSSimpleObject
  CLSID_CSExplicitInterfaceObject: 4B65FE47-2F9D-37B8-B3CB-5BE4A7BC0926
  IID_ICSExplicitInterfaceObject: 32DBA9B0-BE1F-357D-827F-0196229FA0E2
  DIID_ICSExplicitInterfaceObjectEvents: 95DB823B-E204-428c-92A3-7FB29C0EC576
  LIBID_CSDllCOMServer: F0998D9A-0E79-4F67-B944-9E837F479587

  属性：
    // 包括get和set存取方法。
    float FloatProperty

  方法：
    // HelloWorld 返回一个字符串“HelloWorld”
    string HelloWorld();
    // GetProcessThreadID输出正在运行的线程ID和进程ID
    void GetProcessThreadID(out uint processId, out uint threadId);

  事件：
    // FloatPropertyChanging在新的FloatProperty属性被设置之前触发。
    // 客户端可以通过参数Cancel来取消对FloatProperty的修改。
    
    // FloatPropertyChanging
    void FloatPropertyChanging(float NewValue, ref bool Cancel);

注意：这个示例中使用的GUID是由guidgen.exe生成的（Visual Studio / Tools / 
Create GUID）。当你编写自己的COM对象时，你需要生成并使用新的GUID。

注意：使用.Net语言编写的COM组件和ActiveX控件不能被.Net应用程序以interop的形式
引用。如果你"添加引用"一个TLB或者拖拽一个ActiveX控件到你的.Net应用程序，你将
会得到一个错误提示："The ActiveX type library 'XXXXX.tlb'was exported from 
a .NET assembly and cannot be added as a reference." 正确的做法是在引用中直接
添加一个.Net组件。

/////////////////////////////////////////////////////////////////////////////
项目间关系：
（在Microsoft All-In-One Code Framework http://cfx.codeplex.com中当前项目和其他
项目的关系）

CppCOMClient -> CSDllCOMServer
CppCOMClient使用#import导入此C#组件，并且使用聪明指针（smart pointer）。

CSDllCOMServer - VBDllCOMServer
以上COM示例导出相同的属性，方法，事件，但它们是使用不同的.Net语言编写的。

CSDllCOMServer - CSCOMService
两个COM组件都是使用Visual C#编写。 CSDllCOMServer是一个进程内组件，其形式为动态
链接库（DLL）。CSCOMService是一个进程外组件，其形式为提供本地Windows启动时后台
运行的服务（EXE）。


/////////////////////////////////////////////////////////////////////////////
部署：

A、安装

Regasm.exe CSDllCOMServer.dll
此命令注册CSDllCOMServer.dll中所有COM可见（COM-visible）的类型。

B、注销

Regasm.exe /u CSDllCOMServer.dll
此命令注销了CSDllCOMServer.dll中的COM可见（COM-visible）的类型。


/////////////////////////////////////////////////////////////////////////////
创建过程：

A、创建项目

步骤1、在Visual Studio 2008中创建一个Visual C# 类库项目，并把其命名为CSDllCOMServer。

步骤2、为了使.Net程序集为可见，首先打开项目属性。点击程序集信息，选择使程序集
COM可见。此操作和在AssemblyInfo.cs的代码中添加程序集属性ComVisible是等效的。

	[assembly: ComVisible(true)]

对话框中的GUID是此组件的libid：
	[assembly: Guid("f0998d9a-0e79-4f67-b944-9e837f479587")]

其次，在项目属性的生成页面中，选择选项“为COM互操作注册”。此选项指定是否你所管理
的应用程序将导出一个COM对象，这将允许一个COM对象和你管理的应用程序进行交互。

B、添加一个显式定义的接口组件

步骤1、定义一个“public”的接口ICSExplicitInterfaceObject，此接口描述了从类的
COM接口。使用Guid属性指定其GUID（IID）：
	[Guid("32DBA9B0-BE1F-357D-827F-0196229FA0E2")]
	
此方法中，COM对象的IID是一个定值。默认情况下，.Net类所使用的接口会在IDL中变化为
双接口[InterfaceType(ComInterfaceType.InterfaceIsDual)]。这使客户端获得最佳早
绑定及晚绑定。其他选择是[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
和[InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]。

步骤2、在ICSExplicitInterfaceObject接口中，定义需导出的属性及方法的原型。不导出
的项目标记为[ComVisible(false)]。 

步骤3、定义一个“public”接口ICSExplicitInterfaceObjectEvents，此接口用于描述从类
可触发的事件。使用Guid属性指定它的GUID（Events ID）：

	[Guid("95DB823B-E204-428c-92A3-7FB29C0EC576")]
	
将此接口定义为一个IDispatch接口：

	[InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]

步骤4、在ICSExplicitInterfaceObjectEvents接口中，定义要被导出的属性和方法的原型。

步骤5、定义一个实现ICSExplicitInterfaceObject接口的“public”类CSExplicitInterfaceObject。
为它加上[ClassInterface(ClassInterfaceType.None)]属性，这将告知类库生成工具我们
不需要一个类接口。这确保了ICSExplicitInterfaceObject是一个默认的接口。另外，使用
Guid属性来指定这个类的GUID（CLSID）.

	[Guid("4B65FE47-2F9D-37B8-B3CB-5BE4A7BC0926")]

如此，COM对象的CLSID将是一个定值。最后为此类定义一个ComSourceInterface属性：

	[ComSourceInterfaces(typeof(ICSExplicitInterfaceObjectEvents))]

ComSourceInterfaces指定了一份从COM事件源导出的接口列表。

步骤6、确保CSExplicitInterfaceObject的构造函数不是一个private函数。（我们可
以添加一个public构造函数或者使用默认的构造函数）由此，COM对象可以从客户端程序
中创建。

步骤7、在CSExplicitInterfaceObject中，实现ICSExplicitInterfaceObject接口。
其中包括编写FloatProperty属性的主体部分以及HelloWorld方法，GetProcessThreadID
方法和HiddenFunction方法。

步骤8、生成项目。如果在输出文件夹里创建了一个独立的tlb文件， CSDllCOMServer.tlb，
这基本上表示此项目生成成功。

/////////////////////////////////////////////////////////////////////////////
参考资料：

Exposing .NET Framework Components to COM
http://msdn.microsoft.com/en-us/library/zsfww439.aspx

COM Interop Part 2: C# Server Tutorial
http://msdn.microsoft.com/en-us/library/aa645738.aspx

Building COM Servers in .NET By Lim Bio Liong
http://www.codeproject.com/KB/COM/BuildCOMServersInDotNet.aspx

Understanding Classic COM Interoperability With .NET Applications By Aravind
http://www.codeproject.com/KB/COM/cominterop.aspx

KB: How to develop an in-process COM component 
http://support.microsoft.com/kb/976026/en-us


/////////////////////////////////////////////////////////////////////////////