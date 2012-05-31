=============================================================================
      类库 : CSShellExtContextMenuHandler 项目概述
=============================================================================

/////////////////////////////////////////////////////////////////////////////
摘要:

该c#代码示例演示如何创建一个.NET4.0框架下的上下文菜单外壳处理程序. 

上下文菜单应用程序是一个在已存在的上下文菜单中添加菜单命令的系统外壳扩展应用程序.

上下文菜单应用程序是用来关联特定的文件类型，并执行一些具体的操作. 

尽管你可以在注册表中注册增加上下文菜单的文件类型，但该处理程序可以动态地将项目添加到自定义对象的上下文菜单中.

一位CLR程序经理Jesse Kaplan解释到:在.net框架4.0之前,由于受制于每个进程的只有一个.net 运行库的 CLR 限制,
所以外壳程序的开发不被支持，详细请看：
http://social.msdn.microsoft.com/forums/en-US/netfxbcl/thread/1428326d-7950-42b4-ad94-8e962124043e.


在.NET4.0框架下，能够与任何其他运行库的过程中有多个运行时Microsoft现在可以提供支持托管外壳扩展.
CSShellExtContextMenuHandler就是这样一个托管的外壳扩展代码示例.但是，您还是不能在低于.NET4.0的任何框架下开发
托管的外壳程序。

 
该示列的上下文程序的 class ID (CLSID): 是
    {B1F1405D-94A1-4692-B72F-FC8CAF8B8700}

当你在资源管理器中右击一个.cs类型的文件时候，该程序将添加一个名叫"Display File Name (C#)"的子菜单项到上下文菜单中，
当你点击该子菜单项时候将提示该文件的完整路径.

/////////////////////////////////////////////////////////////////////////////
安装和卸载:

A. 安装

在Visual Studio 2010 \ Visual Studio Tools menu菜单下找到'Visual Studio Command Prompt (2010)'，如果您是
64位的操作系统请运行 'Visual Studio x64 Win64 Command Prompt (2010)',同时请确保您具有管理员的权限.
定位到CSShellExtContextMenuHandler.dll所在的文件夹输入以下命令:

    Regasm.exe CSShellExtContextMenuHandler.dll /codebase

如果命令执行成功的话您将收到下面的提示信息:

    "Types registered successfully"

B. 卸载

在Visual Studio 2010 \ Visual Studio Tools menu菜单下找到'Visual Studio Command Prompt (2010)'，如果您是
64位的操作系统请运行 'Visual Studio x64 Win64 Command Prompt (2010)',同时请确保您具有管理员的权限.
定位到CSShellExtContextMenuHandler.dll所在的文件夹输入以下命令:

    Regasm.exe CSShellExtContextMenuHandler.dll /unregister

如果命令执行成功的话您将收到下面的提示信息:

    "Types un-registered successfully"


/////////////////////////////////////////////////////////////////////////////
演示:

以下是使用上下文扩展库的步骤.

步骤1. 
	如果您在 Visual Studio 2010中能成功生成工程的话,您将得到一个 CSShellExtContextMenuHandler.dll的动态链接库文件
	在Visual Studio 2010 \ Visual Studio Tools menu菜单下找到'Visual Studio Command Prompt (2010)'，如果您是
	64位的操作系统请运行 'Visual Studio x64 Win64 Command Prompt (2010)',同时请确保您具有管理员的权限.
	定位到CSShellExtContextMenuHandler.dll所在的文件夹输入以下命令:

    Regasm.exe CSShellExtContextMenuHandler.dll /codebase

如果命令执行成功的话您将收到下面的提示信息:

    "Types registered successfully"

步骤2.
	在Window资源管理器中找一个.cs类型的文件 (例如在同各文件下有个文件FileContextMenuExt.cs), 
右击这个文件，您在上下文菜单中将看到"Display File Name (C#)"的菜单子项,点击这个菜单子项，
您将看到FileContextMenuExt.cpp文件的的完整物理路径.在Window资源管理器里您只能同能选择一个文件而不是多个文件.


步骤3. 
	在相同的命令提示符处, 运行以下命令:

    Regasm.exe CSShellExtContextMenuHandler.dll /unregister

用来卸载上下文菜单应用程序.


/////////////////////////////////////////////////////////////////////////////
执行:

A. 创建和设置工程

在Visual Studio 2010中, 创建一个名为"CSShellExtContextMenuHandler"的Visual C# / Windows / Class Library 工程,
在签名页上，使用强名称密钥文件对该程序集进行签名.


-----------------------------------------------------------------------------

B. 执行一个基本组件对象模型 (COM) DLL

所有扩展应用程序都是以COM对象的方式在进程内运行.
创建一个基本的.NET COM对象是很简单的事情.您只需要定义一个公共类该类的ComVisible属性设置为true,
使用Guid属性设置一个CLSID,显式实现某些COM接口.例如,

    [ClassInterface(ClassInterfaceType.None)]
    [Guid("B1F1405D-94A1-4692-B72F-FC8CAF8B8700"), ComVisible(true)]
    public class SimpleObject : ISimpleObject
    {
        ... // 实现接口
    }

您甚至不需要自己实现 IUnknown 和类工厂，因为.net 框架已经为您处理好了.

-----------------------------------------------------------------------------

C. 使用上下文菜单应用程序并关联一个文件类型

-----------
上下文应用程序的实现:

FileContextMenuExt.cs 文件定义上下文菜单处理程序. 必须继承IShellExtInit and IContextMenu 
接口. 在文件ShellExtLib.cs中你可以看到接口可以通过COMImport属性导入

    [ComImport(), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("000214e8-0000-0000-c000-000000000046")]
    internal interface IShellExtInit
    {
        void Initialize(
            IntPtr pidlFolder,
            IntPtr pDataObj,
            IntPtr /*HKEY*/ hKeyProgID);
    }

    [ComImport(), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("000214e4-0000-0000-c000-000000000046")]
    internal interface IContextMenu
    {
        [PreserveSig]
        int QueryContextMenu(
            IntPtr /*HMENU*/ hMenu,
            uint iMenu,
            uint idCmdFirst,
            uint idCmdLast,
            uint uFlags);

        void InvokeCommand(IntPtr pici);

        void GetCommandString(
            UIntPtr idCmd,
            uint uFlags,
            IntPtr pReserved,
            StringBuilder pszName,
            uint cchMax);
    }

    [ClassInterface(ClassInterfaceType.None)]
    [Guid("B1F1405D-94A1-4692-B72F-FC8CAF8B8700"), ComVisible(true)]
    public class FileContextMenuExt : IShellExtInit, IContextMenu
    {
        public void Initialize(IntPtr pidlFolder, IntPtr pDataObj, IntPtr hKeyProgID)
        {
            ...
        }
	
        public int QueryContextMenu(
            IntPtr hMenu,
            uint iMenu,
            uint idCmdFirst,
            uint idCmdLast,
            uint uFlags)
        {
            ...
        }

        public void InvokeCommand(IntPtr pici)
        {
            ...
        }

        public void GetCommandString(
            UIntPtr idCmd,
            uint uFlags,
            IntPtr pReserved,
            StringBuilder pszName,
            uint cchMax)
        {
            ...
        }
    }


COM操作使得具有最终输出参数的函数看起来是由它返回的该值，PreserveSig属性用于关闭这一特性。 
当您不设置（例如，GetCommandString 方法 PreserveSigAttributeIContextMenu）失败时候 ，将引发一个.NET 的异常。
例如 Marshal.ThrowExceptionForHR(WinError.E_FAIL) ；在 PreserveSigAttribute 应用于托管的方法的签名时，属性化方法的托管和非托管签名是相同的 （例如
QueryContextMenu 方法的 IContextMenu)。保留原始的方法，如果该成员返回多个成功的 HRESULT值签名是必要的，并且您想要检测不同的值。

只有该上下文应用程序被成功注册了，当上下文菜单显示的时候才能被实例化.

  1. IShellExtInit接口实现

  当上下文扩展COM对象被实例化时,IShellExtInit::Initialize方法将被调用. 
  IShellExtInit::Initialize  提供与在 CF_HDROP 格式中保存一个或多个文件名称的 IDataObject 对象的上下文菜单扩展。
  所选的文件和文件夹通过 IDataObject 对象，您可以枚举。如果 IShellExtInit::Initialize 返回的是S_OK 以外的其他任何值则不能使用上下文菜单扩展.
  
  在示例代码中, FileContextMenuExt::Initialize 枚举被选中的文件和文件夹. 只有一个文件被选中的时候, 这个方法将保存
  文件名并返回S_OK供后续处理.  如果没有文件或不止一个文件被选中则函数返回E_FAIL您将不能使用这个上下文应用程序.


  2.  IContextMenu接口实现:

  当IShellExtInit::Initialize返回iS_OK后, IContextMenu::QueryContextMenu这个方法将被调用用以获取子菜单项或添加子菜单项.
  QueryContextMenu方法的实现是非常简单的.上下文扩展使用InsertMenuItem或类是的函数插入子菜单项. 
  菜单标示符ID必须大于第一个菜单标示符ID且小于最后一个菜单标示符ID. QueryContextMenu必须返回可用的最大的标识符ID并加一的标识符ID. 
  指定菜单命令标识符的最佳方法是在序列中从0开始.如果上下文菜单扩展无需添加菜单项则QueryContextMenu返回0.
  
  在示例代码中, 我们插入一个 "Display File Name (C#)"子菜单项并在它的下面加一个分隔符.

  IContextMenu::GetCommandString方法被用来检索并返回子菜单项的文本, 例如，为菜单项显示帮助文本. 如果用户选中了上下文菜单中添加的子菜单,
  应用程序的IContextMenu::GetCommandString 方法将被调用来获取帮助文本并显示在资源管理器的状态栏上ANSI或Unicode的字符集都可以被使用. 示例程序只使用Unicode的uFlags参数, 
  因为自Windows 2000以后资源管理器只接受Unicode的字符集.

  当某个通过上下文菜单扩展安装的子菜单项被选中时，IContextMenu::InvokeCommand的方法在上下文菜单中执行或激发响应此方法所需的操作.

-----------
注册为某一类文件的处理程序:

上下文菜单处理程序关联的类文件或文件夹. 
文件类, 程序将被注册在.

    HKEY_CLASSES_ROOT\<File Type>\shellex\ContextMenuHandlers



上下文菜单处理程序的注册是在FileContextMenuExt方法中实现.ComRegisterFunction属性附加到该方法使基本以外的其他用户编写代码的执行
注册的 COM 类。注册调用，ShellExtReg.RegisterShellExtContextMenuHandler 方法中，ShellExtLib.cs
将该处理程序与特定文件类型相关联。如果文件的类型是以'.'开头的，它会尝试读取 HKCR\ < 文件类型 > 键的可能的默认值包含链接的文件类型的程序 ID。如果默认值不为空，使用作为文件类型的程序 ID 进行注册。

例如, 示例文件关联了 '.cs' 类型的文件. 
如果您安装了Visual Studio 2010则注册表项HKCR\.cs下就有了一个默认的文件类型'VisualStudio.cs.10.0',所以将使用'VisualStudio.cs.10.0'
来取代HKCR\.cs下的文件类型.

   HKCR
    {
        NoRemove .cs = s 'VisualStudio.cs.10.0'
        NoRemove VisualStudio.cs.10.0
        {
            NoRemove shellex
            {
                NoRemove ContextMenuHandlers
                {
                    {B1F1405D-94A1-4692-B72F-FC8CAF8B8700} = 
                        s 'CSShellExtContextMenuHandler.FileContextMenuExt'
                }
            }
        }
    }

注销的动作将在FileContextMenuExt函数中被实现并执行,类似注册的方法,ComUnregisterFunction属性附加到该方法使基本以外的其他用户编写代码的执行
注销的COM 类,执行后将删除注册表项HKCR\CLSID\{<CLSID>} 键 {<CLSID>}和HKCR\<File Type>\shellex\ContextMenuHandlers下的值.

/////////////////////////////////////////////////////////////////////////////
参考资料:

MSDN: 初始化外壳应用程序
http://msdn.microsoft.com/en-us/library/cc144105.aspx

MSDN: 创建上下文菜单应用程序
http://msdn.microsoft.com/en-us/library/bb776881.aspx

MSDN: 执行上下文菜单COM对象
http://msdn.microsoft.com/en-us/library/ms677106.aspx

MSDN: 扩展快捷菜单
http://msdn.microsoft.com/en-us/library/cc144101(VS.85).aspx

深入透析CLR : In-Process Side-by-Side
http://msdn.microsoft.com/en-us/magazine/ee819091.aspx

不要使用低于.NET 4框架编写外壳应用程序
http://blogs.msdn.com/b/oldnewthing/archive/2006/12/18/1317290.aspx
http://social.msdn.microsoft.com/forums/en-US/netfxbcl/thread/1428326d-7950-42b4-ad94-8e962124043e/
http://blogs.msdn.com/b/junfeng/archive/2005/11/18/494572.aspx


/////////////////////////////////////////////////////////////////////////////