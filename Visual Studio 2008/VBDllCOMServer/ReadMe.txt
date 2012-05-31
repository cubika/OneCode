========================================================================
                库应用程序：VBDllCOMServer项目概述
========================================================================

/////////////////////////////////////////////////////////////////////////////
摘要：

这个VB.NET示例着重于使用COM技术导出.Net Framework组件。这个允许我们为COM开发人员
编写一个.Net类型并且在非托管代码中使用此类型。此示例使用了ComClassAttribute属性使
编译器添加元数据，这将允许一个类作为一个COM对象被导出。这个属性简化了从Visual Basic
中导出COM对象的过程。如果没有标记为ComClassAttribute，你需要好几个步骤才能从Visual 
Basic中创建一个COM组件。一旦类被标记为ComClassAttribute，编译器会自动执行这些额外
的操作。

VBDllCOMServer导出了以下项目：

1、VBSimpleObject组件

  Program ID: VBDllCOMServer.VBSimpleObject
  CLSID_VBSimpleObject: 805303FE-B5A6-308D-9E4F-BF500978AEEB
  IID__VBSimpleObject: 90E0BCEA-7AFA-362A-A75E-6D07C1C6FC4B
  DIID___VBSimpleObject: 72D3EFB2-0D88-4BA7-A26B-8FFDB92FEBED (EventID)
  LIBID_VBDllCOMServer: A0CB2839-B70C-4035-9B11-2FF27E08B7DF
  
  属性：
    ' 包括get和set存取方法。
    FloatProperty As Single

  方法：
    ' HelloWorld 返回一个字符串“HelloWorld”
    Function HelloWorld() As String
  
    ' GetProcessThreadID输出正在运行的线程ID和进程ID
    Sub GetProcessThreadID(ByRef processId As UInteger, ByRef threadId As UInteger)

  事件：
   ' FloatPropertyChanging在新的FloatProperty属性被设置之前触发。
   ' 客户端可以通过参数Cancel来取消对FloatProperty的修改。
   
   ' FloatPropertyChanging
   Event FloatPropertyChanging(ByVal NewValue As Single, ByRef Cancel As Boolean)

注意：这个示例中使用的GUID是由guidgen.exe生成（Visual Studio / Tools / 
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

VBDllCOMServer - CSDllCOMServer
以上COM示例导出相同的属性，方法，事件，但是它们使用不同的.Net语言编写。


/////////////////////////////////////////////////////////////////////////////
生成：
当生成 VBDllCOMServer时，你需要使用管理员权限运行Visual Studio，这是由于此组件
需要在HKCR中注册。

生成后事件命令：regasm VBDllCOMServer.dll /tlb:VBDllCOMServer.tlb

/////////////////////////////////////////////////////////////////////////////
部署：

A、安装

Regasm.exe VBDllCOMServer.dll
此命令注册VBDllCOMServer.dll中所有COM可见（COM-visible）的类型。

B、注销

Regasm.exe /u VBDllCOMServer.dll
此命令注销了VBDllCOMServer.dll中的COM可见（COM-visible）的类型。

/////////////////////////////////////////////////////////////////////////////
创建过程：

A、创建项目

步骤1、在Visual Studio 2008中创建一个Visual Basic 类库项目，并把其命名为
VBDllCOMServer。删除默认的Class1.vb文件。

步骤2、为了使.Net程序集为可见，首先打开项目属性。点击程序集信息，选择使程序集
COM可见。此操作和在AssemblyInfo.cs的代码中添加程序集属性ComVisible是等效的。

	<Assembly: ComVisible(True)> 

对话框中的GUID是此组件的libid：

	<Assembly: Guid("A0CB2839-B70C-4035-9B11-2FF27E08B7DF")> 

其次，在项目属性的生成页面中，选择选项“为COM互操作注册”。此选项指定是否你所管理
的应用程序将导出一个COM对象，这将允许一个COM对象和你管理的应用程序进行交互。

B、添加VBSimpleObject组件

步骤1、添加一个public类VBSimpleObject。

步骤2、在VBSimpleObject类中定义Class ID,Interface ID, 和 Event ID:
	ClassId As String = "805303FE-B5A6-308D-9E4F-BF500978AEEB"
    InterfaceId As String = "90E0BCEA-7AFA-362A-A75E-6D07C1C6FC4B"
    EventsId As String = "72D3EFB2-0D88-4ba7-A26B-8FFDB92FEBED"

步骤3、为VBSimpleObject附加ComClassAttribute属性，并且指定其_ClassID, 
_InterfaceID, 和 _EventID为步骤2中所指定的值。

    <ComClass(VBSimpleObject.ClassId, VBSimpleObject.InterfaceId, _
        VBSimpleObject.EventsId), ComVisible(True)> _
    Public Class VBSimpleObject

C、为组件添加属性

步骤1、在VBSimpleObject类中添加一个public属性。所有public属性都会在组件中导出，
例如：
	
	Public Property FloatProperty() As Single
		Get
			Return Me.fField
		End Get
		Set(ByVal value As Single)
			Me.fField = value
		End Set
	End Property

D、为组件添加方法

步骤1、在VBSimpleObject类中添加一个public方法。所有public方法都会在组件中导出，
例如：

	Public Function HelloWorld() As String
		Return "HelloWorld"
	End Function

E、为组件添加事件

步骤1、在VBSimpleObject类中添加一个public事件，例如：
	Public Event FloatPropertyChanging(ByVal NewValue As Single, _
	                                   ByRef Cancel As Boolean)

然后在相应的位置触发事件，例如：
	Dim cancel As Boolean = False
	RaiseEvent FloatPropertyChanging(value, cancel)


/////////////////////////////////////////////////////////////////////////////
参考资料：

Exposing .NET Framework Components to COM
http://msdn.microsoft.com/en-us/library/zsfww439.aspx

Building COM Servers in .NET 
http://www.codeproject.com/KB/COM/BuildCOMServersInDotNet.aspx

MSDN: ComClassAttribute Class
http://msdn.microsoft.com/en-us/library/microsoft.visualbasic.comclassattribute.aspx

KB: How to develop an in-process COM component 
http://support.microsoft.com/kb/976026/en-us


/////////////////////////////////////////////////////////////////////////////