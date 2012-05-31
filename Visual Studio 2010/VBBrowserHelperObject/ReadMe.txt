================================================================================
       Windows 应用程序：VBBrowserHelperObject 概述                        
===============================================================================
/////////////////////////////////////////////////////////////////////////////

摘要：
这个样例展示了如何去创建和部署一个Browser Helper Object. 一个Browser Helper Object
在IE浏览器中运行并能够提供额外的服务，这个BHO样例用来使IE浏览器的右键菜单失效。

想要在IE浏览器上加一个BHO，这个类需要一个class ID注册到COM（CLSID，在这个例子中，
{C42D40F0-BEBF-418D-8EA1-18D99AC2AB17}），还要在
"HKLM\Software\Microsoft\Windows\CurrentVersion\Explorer\Browser Helper Objects"
增加一个键值。

注意：
1. 在64位计算机上，32位的IE要使用
     "HKLM\Software\Wow6432Node\Microsoft\Windows\CurrentVersion\Explorer\Browser Helper Objects"

2. 在Windows Server 2008或者Windows Server 2008 R2上，IE浏览器的安全配置（IE ESC）
加强至默认，这意味着这个BHO将不能被IE浏览器加载。你必须关闭IE ESC。

/////////////////////////////////////////////////////////////////////////////
安装和卸载：

--------------------------------------
在开发环境下

A.安装

x86和x64操作系统中的32位IE浏览器，以管理员身份运行
Microsoft Visual Studio 2010 \ Visual Studio Tools菜单下的的
“Visual Studio Command Prompt (2010)”。x64操作系统下的64位IE浏览器，运行 
Visual Studio x64 Win64 Command Prompt(2010).

找到包含VBBrowserHelperObject.dll的文件夹并输入指令：

    Regasm.exe VBBrowserHelperObject.dll /codebase

如果控制台显示出以下信息说明BHO已经成功注册：

    "Types registered successfully"

B.卸载

x86和x64操作系统的32位IE浏览器，以管理员身份运行
Microsoft Visual Studio 2010 \ Visual Studio Tools菜单下的 
'Visual Studio Command Prompt (2010)'.x64操作系统下的64位浏览器，运行
Visual Studio x64 Win64 Command Prompt (2010).

找到包含VBBrowserHelperObject.dll的文件夹并输入指令：

    Regasm.exe VBBrowserHelperObject.dll /unregister

如果控制台显示出以下信息说明BHO已经成功注册：

    "Types un-registered successfully"

--------------------------------------
在部署环境下

A.安装

x86和x64操作系统下的32位浏览器，安装VBBrowserHelperObjectSetup(x86).msi，
这是VBBrowserHelperObjectSetup(x86)安装项目的输出。

x64操作系统下的64位IE浏览器，安装VBBrowserHelperObjectSetup(x64).msi，
这是VBBrowserHelperObjectSetup(x64)安装项目的输出。

B.卸载

x86和x64操作系统下的32位浏览器，卸载VBBrowserHelperObjectSetup(x86).msi， 
这是VBBrowserHelperObjectSetup(x86)安装项目的输出。

x64操作系统下的64位IE浏览器，卸载VBBrowserHelperObjectSetup(x64).msi，
这是VBBrowserHelperObjectSetup(x64)安装项目的输出。


////////////////////////////////////////////////////////////////////////////////
例子：

步骤一：用VS2010打开这个项目把解决方案平台设置为x86.确认VBBrowserHelperObject 和
VBBrowserHelperObjectSetup(x86)都在配置管理器中被选中。
               
注意：如果你想在64位IE浏览器中使用这个BHO，把平台设置为x64，并选中select VBBrowserHelperObject 
和VBBrowserHelperObjectSetup(x64) 去创建。

步骤二：创建解决方案。

步骤三：在解决方案管理器中的VBBrowserHelperObjectSetup(x86)项目上点击右键选择“安装”。
安装完成后，打开32位IE浏览器，点击工具=>插件管理，在插件管理对话框中，你可以找到
"VBBrowserHelperObject.BHOIEContextMenu"项，确定他已经可以使用。你可能需要重新启动IE浏览器来使它生效。

注意：你还可以使用“安装和卸载”部分的注册表命令。

步骤四：打开IE浏览器访问http://www.microsoft.com/.当页面加载完成后，右键点击页面你会发现右键菜单失效了。


/////////////////////////////////////////////////////////////////////////////
实现：

A. 创建一个项目并添加引用

在Visual Studio2010中，创建一个Visual Basic / Windows / Class Library 项目，命名为 "VBBrowserHelperObject".

在解决方案管理其中点击右键选择“添加引用”。添加COM下的"Microsoft HTML Object Library" 和"Microsoft Internet Controls" .

-----------------------------------------------------------------------------

B.实现一个基本的Component Object Model(COM) DLL

一个Browser Helper Object是一个被实现为一个DLL的COM对象。做一个基本的.NET COM模块非常简单。
你只需要定义一个'public'类并把它的ComVisible特性设置为true，用Guid特性来识别它的CLSID，并显式实现COM接口。例如，

    [ClassInterface(ClassInterfaceType.None)]
    [Guid("69FA02A4-19BE-4C49-8D8F-E284E6B01363"), ComVisible(true)]
    public class SimpleObject : ISimpleObject
    {
        ... // Implements the interface
    }

你甚至不用自己去实现IUnknown和类工厂，.NET Framework帮你解决了这一切。

-----------------------------------------------------------------------------

C.实现BHO并把它注册到某个文件类

-----------
实现之前的句柄：

BHOIEContextMenu.vb文件定义了BHO.

GuidAttribute特性是BHOIEContextMenu类上用来指定CLSID的。当你写了自己的句柄时，你必须工具菜单中
的“Create GUID”工具为你的BHO类创建一个新的CLSID，并在Guid特性中指定它的值。

    ...
<ComVisible(True), ClassInterface(ClassInterfaceType.None),
Guid("C42D40F0-BEBF-418D-8EA1-18D99AC2AB17")>
Public Class BHOIEContextMenu 
...

这个类也实现了IObjectWithSite接口。在SetSite方法中，你可以得到一个实现了InternetExplorer接口的实例。

Public Sub SetSite(ByVal site As Object) Implements IObjectWithSite.SetSite
        If site IsNot Nothing Then
            ieInstance = CType(site, InternetExplorer)

            ' Register the DocumentComplete event.
            AddHandler ieInstance.DocumentComplete, AddressOf ieInstance_DocumentComplete
        End If
    End Sub

    
    Public Sub GetSite(ByRef guid_Renamed As Guid,
                       <System.Runtime.InteropServices.Out()> ByRef ppvSite As Object) _
                   Implements IObjectWithSite.GetSite
        Dim punk As IntPtr = Marshal.GetIUnknownForObject(ieInstance)
        ppvSite = New Object()
        Dim ppvSiteIntPtr As IntPtr = Marshal.GetIUnknownForObject(ppvSite)
        Dim hr As Integer = Marshal.QueryInterface(punk, guid_Renamed, ppvSiteIntPtr)
        Marshal.ThrowExceptionForHR(hr)
        Marshal.Release(ppvSiteIntPtr)
        Marshal.Release(punk)
    End Sub


-----------
注册BHO

BHO中的注册和发注册逻辑也在这个类中实现，要注册一个BHO，需要在下面这个键下创建一个新键：

HKLM\Software\Microsoft\Windows\CurrentVersion\Explorer\Browser Helper Objects


-----------------------------------------------------------------------------

D.用setup project部署BHO.

（1）在BHOIEContextMenu中，增加一个安装类（在这个例子中被命名为BHOInstaller ）去在设置中
定义用户自定义行为。这个类继承了System.Configuration.Install.Installer.当用户安装或卸载
组件时，我们利用用户自定义行为去注册或反注册现在这个托管程序集中的可见COM类。

    <RunInstaller(True), ComVisible(False)>
    Partial Public Class BHOInstaller
    Inherits Installer
    Public Sub New()
        InitializeComponent()
    End Sub

    Public Overrides Sub Install(ByVal stateSaver As System.Collections.IDictionary)
        MyBase.Install(stateSaver)


        Dim regsrv As New RegistrationServices()
        If Not regsrv.RegisterAssembly(Me.GetType().Assembly,
                                       AssemblyRegistrationFlags.SetCodeBase) Then
            Throw New InstallException("Failed To Register for COM")
        End If
    End Sub
 
    Public Overrides Sub Uninstall(ByVal savedState As System.Collections.IDictionary)
        MyBase.Uninstall(savedState)
        Dim regsrv As New RegistrationServices()
        If Not regsrv.UnregisterAssembly(Me.GetType().Assembly) Then
            Throw New InstallException("Failed To Unregister for COM")
        End If
    End Sub

End Class

在重写的Install方法中，我们使用了RegistrationServices. RegisterAssembly注册了现在这个托管
程序集中的类使之支持从COM创建。这个方法也调用了有ComRegisterFunctionAttribute特性的静态方法去执行用户COM注册流程。
这个调用相当于在开发环境中运行了以下指令：

    "Regasm.exe VBBrowserHelperObject.dll /codebase"

（2）添加一个部署项目，在文件菜单中，选择添加，然后点击新项目。在添加新项目对话框中，展开其他
项目类型节点，展开设置和部署项目，点击Visual Studio Installer，然后点击设置项目。在名称文
本框中，输入VBBrowserHelperObjectSetup(x86).点击确定创建项目。在设置项目的属性对话框中，
确认TargetPlatform 属性设置为x86,。这个设置项目要被部署在x64和x86操作系统的32位IE浏览器上。

右键点击设置项目，选择添加/项目输出...
在添加项目输出组对话框中，VBBrowserHelperObject会出现在项目列表中。选择主输出并点击确认。
VS会检测VBBrowserHelperObject的依赖，包括.NET Framework4.0（客户端配置文件）。

再次右键点击设置项目，选择浏览 / 自定义操作。
在自定义操作编辑器中，右键点击自定义操作根节点。在操作菜单中，点击添加自定义操作。在项目对话
框的选择项中，双击应用程序文件夹。从VBBrowserHelperObject选择主输出。这是添加了我们在
VBBrowserHelperObject的BHOInstaller中定义的自定义操作。

创建配置项目。如果创建成功，你会得到一个.msi文件和一个.exe文件。你可以把它分配给你的用户去
安装和卸载这个BHO.

（3）为x64操作系统中的64位IE浏览器部署BHO，你必须要创建一个新的配置项目
（例如：在这个例子中是VBBrowserHelperObjectSetup(x64)），设置它的目标平台属性位x64.

虽然目标平台属性是x64，本地打包为.msi文件依旧是32位可执行文件。Visual Studio把32位版本的
InstallUtilLib.dll以InsutallUtil嵌入了二进制表。所以自定义操作会运行在32位模式下，这是这
个代码样例中不希望看到的。想要解决这种情况保证用户自定义行为运行在64位模式下，你需要将
InstallUtilLib.dll合适的位导入二进制表使InstallUtil记录，或者，如果你有或者将要有32位托
管自定义操作，将它以一条新纪录的形式添加到二进制表中，调整自定义操作表让64位托管自定义操作使
用64位二进制表记录。这篇博客文章介绍了如何利用Orca手动去做： 
http://blogs.msdn.com/b/heaths/archive/2006/02/01/64-bit-managed-custom-actions-with-visual-studio.aspx

在这个代码示例中，我们用了一个编译好的javascript（Fix64bitInstallUtilLib.js）文件来自动
修改了InstallUtil.你可以在 VBBrowserHelperObjectSetup(x64)项目文件夹中找到这个文件。
配置脚本让他运行在编译后的事件中，你要在解决方案管理器中选择 VBBrowserHelperObjectSetup(x64)项目，
在属性窗口找到PostBuildEvent属性，指定它的值为：

"$(ProjectDir)Fix64bitInstallUtilLib.js" "$(BuiltOuputPath)" "$(ProjectDir)"

剩下的步骤重复（2），添加项目输出，设置自定义操作，配置先决条件，创建设置项目。


/////////////////////////////////////////////////////////////////////////////
诊断程序:

想要调试BHO，你可以附加到iexplorer.exe

 
/////////////////////////////////////////////////////////////////////////////
参考:

Browser Helper Objects: 你想要的浏览器
http://msdn.microsoft.com/en-us/library/ms976373.aspx

宿主和复用
http://msdn.microsoft.com/en-us/library/aa752038(VS.85).aspx

IHTMLDocument2 接口
http://msdn.microsoft.com/en-us/library/aa752574(VS.85).aspx

BHO中鼠标事件的解决问题
http://social.msdn.microsoft.com/Forums/en/ieextensiondevelopment/thread/1ea154a5-5802-460c-9941-30f14b36d0a4

/////////////////////////////////////////////////////////////////////////////

