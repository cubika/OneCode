=============================================================================
    动态连接库 : CppShellExtContextMenuHandler 项目概述
=============================================================================

/////////////////////////////////////////////////////////////////////////////
摘要: 

该代码示例演示如何创建一个C++上下文菜单处理程序. 

上下文菜单应用程序是一个在已存在的上下文菜单中添加命令的系统外壳扩展应用程序.

上下文菜单应用程序是用来关联特定的文件类型，并执行一些具体的操作. 

尽管你可以在注册表中注册增加上下文菜单的文件类型，该处理程序可以动态地将项目添加到自定义对象的上下文菜单中.

示列的上下文程序的 class ID (CLSID): 
    {BFD98515-CD74-48A4-98E2-13D209E3EE4F}

当你在资源管理器中右击一个.cpp类型的文件时候，该程序将添加一个名叫"Display File Name (C++)"的子菜单项到上下文菜单中，
当你点击该子菜单项时候将提示该文件的完整路径.


/////////////////////////////////////////////////////////////////////////////
安装和卸载:

A. 安装

如果您要在64位Windows系统中使用命令行程序扩展名, 请参照 (http://msdn.microsoft.com/en-us/library/9yb4317s.aspx) 的方法
设置您的Visual C++工程生成的对象为64位,只有64位的扩展动态链接库才能在64位的Window外壳程序上运行.如果您将在32位的Window系统上
运行，则保持默认的编译设置不变.


以管理员的身份定位到CppShellExtContextMenuHandler.dll所在的文件夹输入以下命令:

    Regsvr32.exe CppShellExtContextMenuHandler.dll

如果命令执行成功的话您将收到下面的提示信息:

    "DllRegisterServer in CppShellExtContextMenuHandler.dll succeeded."

B. 卸载

以管理员的身份定位到CppShellExtContextMenuHandler.dll所在的文件夹输入以下命令:

    Regsvr32.exe /u CppShellExtContextMenuHandler.dll

如果命令执行成功的话您将收到下面的提示信息:

    "DllUnregisterServer in CppShellExtContextMenuHandler.dll succeeded."


/////////////////////////////////////////////////////////////////////////////
演示:

以下是使用上下文扩展库的步骤.

步骤1. 

	如果您要在64位Windows系统中使用命令行程序扩展名, 请参照 (http://msdn.microsoft.com/en-us/library/9yb4317s.aspx) 的方法
设置您的Visual C++工程生成的对象为64位,只有64位的扩展动态链接库才能在64位的Window外壳程序上运行.如果您将在32位的Window系统上
运行，则保持默认的编译设置不变.

步骤2. 

	如果您在 Visual Studio 2010中能成功生成的话,您将得到一个 CppShellExtContextMenuHandler.dll的动态链接库文件，
以管理员的身份定位到CppShellExtContextMenuHandler.dll所在的文件夹输入以下命令:

    Regsvr32.exe CppShellExtContextMenuHandler.dll

如果命令执行成功的话您将收到下面的提示信息:

    "DllRegisterServer in CppShellExtContextMenuHandler.dll succeeded."

步骤3. 
	
	在Window资源管理器中找一个文件 (例如在同各文件下有个文件FileContextMenuExt.cpp), 
右击这个文件，您在上下文菜单中将看到"Display File Name (C++)"的菜单子项,点击这个菜单子项，
您将看到FileContextMenuExt.cpp文件的的完全物理路径.
在Window资源管理器里您只能同能选择一个文件而不是多个文件.

步骤4. 
	在相同的命令提示符处, 运行以下命令

    Regsvr32.exe /u CppShellExtContextMenuHandler.dll

用来卸载上下文菜单处理程序.


/////////////////////////////////////////////////////////////////////////////
执行:

A. 创建和设置工程

在Visual Studio 2010中, 创建一个名为"CppShellExtContextMenuHandler"的Visual C++ / Win32 / Win32 工程.
在 "Application Settings"向导页, 选择应用程序的类型为 "DLL" 同时选择 "Empty project" 选项. 然后点击完成按钮,
这样一个空的Win32 DLL的工程就被创建成功了.

-----------------------------------------------------------------------------

B. 执行一个基本组件对象模型 (COM) DLL

所有扩展应用程序都是以COM对象的方式在进程内运行.
创建一个基本的COM对象，必须实现DllGetClassObject,DllCanUnloadNow,DllRegisterServer,和DllUnregisterServer方法，
增加一个实现了IUnknown的接口的COM类.相关示列代码文件:

  dllmain.cpp - 实现DllMain ,DllGetClassObject, DllCanUnloadNow,DllRegisterServer, DllUnregisterServer 这些COM DLL必须的基本函数.   

  GlobalExportFunctions.def - 导出DllGetClassObject, DllCanUnloadNow, DllRegisterServer, DllUnregisterServer这些函数的Dll模块定义文件.

  Reg.h/cpp - 重复使用的 RegisterInprocServer, UnregisterInprocServer函数定义文件

  FileContextMenuExt.h/cpp - COM类定义文件.这个文件包含了IUnknown接口实现 .

  ClassFactory.h/cpp -COM类的工厂类. 

-----------------------------------------------------------------------------

C. 使用上下文菜单应用程序并关联一个文件类型

-----------
上下文应用程序的实现:

FileContextMenuExt.h/cpp 定义了一个上下文应用程序. 必须继承IShellExtInit and IContextMenu 
接口. 只有该上下文应用程序被注册了当上下文菜单显示的时候才能被实例化.


    class FileContextMenuExt : IShellExtInit, IContextMenu
    {
    public:
        // IShellExtInit
        IFACEMETHODIMP Initialize(LPCITEMIDLIST pidlFolder, LPDATAOBJECT 
            pDataObj, HKEY hKeyProgID);

        // IContextMenu
        IFACEMETHODIMP QueryContextMenu(HMENU hMenu, UINT indexMenu, 
            UINT idCmdFirst, UINT idCmdLast, UINT uFlags);
        IFACEMETHODIMP InvokeCommand(LPCMINVOKECOMMANDINFO pici);
        IFACEMETHODIMP GetCommandString(UINT_PTR idCommand, UINT uFlags, 
            UINT *pwReserved, LPSTR pszName, UINT cchMax);
    };
	
  1. IShellExtInit接口实现:

  当上下文扩展COM对象被实例化时,IShellExtInit::Initialize方法将被调用. 
  IShellExtInit::Initialize  提供与在 CF_HDROP 格式中保存一个或多个文件名称的 IDataObject 对象的上下文菜单扩展。
  所选的文件和文件夹通过 IDataObject 对象，您可以枚举。如果 IShellExtInit::Initialize 返回的是S_OK 以外的其他任何值则不能使用上下文菜单扩展

  在示例代码中, FileContextMenuExt::Initialize 枚举被选中的文件和文件夹. 只有一个文件被选中的时候, 这个方法将保存
  文件名并返回S_OK供后续处理.  如果没有文件或不止一个文件被选中则函数返回E_FAIL您将不能使用这个上下文应用程序.

  2. IContextMenu接口实现:

  当IShellExtInit::Initialize返回iS_OK后, IContextMenu::QueryContextMenu这个方法将被调用用以获取子菜单项或添加子菜单项.
  QueryContextMenu方法的实现是非常简单的.上下文扩展使用InsertMenuItem或类是的函数插入子菜单项. 
  菜单标示符ID必须大于第一个菜单标示符ID且小于最后一个菜单标示符ID. QueryContextMenu必须返回可用的最大的标识符ID并加一的标识符ID. 
  指定菜单命令标识符的最佳方法是在序列中从0开始.如果上下文菜单扩展无需添加菜单项则QueryContextMenu返回0.

  在示例代码中, 我们插入一个 "Display File Name (C++)"子菜单项并在它的下面加一个分隔符.

  IContextMenu::GetCommandString方法被用来检索并返回子菜单项的文本, 例如，为菜单项显示帮助文本. 如果用户选中了上下文菜单中添加的子菜单,
  应用程序的IContextMenu::GetCommandString 方法将被调用来获取帮助文本并显示在资源管理器的状态栏上ANSI或Unicode的字符集都可以被使用. 示例程序只使用Unicode的uFlags参数, 
  因为自Windows 2000以后资源管理器只接受Unicode的字符集.

  当某个通过上下文菜单扩展安装的子菜单项被选中时，IContextMenu::InvokeCommand的方法在上下文菜单中执行或激发响应此方法所需的操作。

-----------
注册为某一类文件的处理程序:

上下文菜单处理程序关联的类文件或文件夹. 
文件类, 程序将被注册在.

    HKEY_CLASSES_ROOT\<File Type>\shellex\ContextMenuHandlers注册表下的子项

上下文应用程序将在dllmain.cpp文件中的DllRegisterServer函数中被注册.
DllRegisterServer函数首先调用Reg.h/cpp中的RegisterInprocServer方法来注册COM组件
然后, 调用RegisterShellExtContextMenuHandler方法关联文件类型. 如果文件的类型是以'.'开头的，
然后在注册表的HKCR\<File Type>下获取该文件类型的对应的Program ID来关联该文件，如果默认的值不为空，则使用该Program ID来完成注册。


例如, 示例文件关联了 '.cpp' 类型的文件. 
如果您安装了Visual Studio 2010则注册表项HKCR\.cpp下就有了一个默认的文件类型'VisualStudio.cpp.10.0',所以将使用'VisualStudio.cpp.10.0'
来取代HKCR\.cpp下的文件类型.

    HKCR
    {
        NoRemove CLSID
        {
            ForceRemove {BFD98515-CD74-48A4-98E2-13D209E3EE4F} = 
                s 'CppShellExtContextMenuHandler.FileContextMenuExt Class'
            {
                InprocServer32 = s '<Path of CppShellExtContextMenuHandler.DLL file>'
                {
                    val ThreadingModel = s 'Apartment'
                }
            }
        }
        NoRemove .cpp = s 'VisualStudio.cpp.10.0'
        NoRemove VisualStudio.cpp.10.0
        {
            NoRemove shellex
            {
                NoRemove ContextMenuHandlers
                {
                    {BFD98515-CD74-48A4-98E2-13D209E3EE4F} = 
                        s 'CppShellExtContextMenuHandler.FileContextMenuExt'
                }
            }
        }
    }


注销的动作将在dllmain.cpp文件中的DllUnregisterServer函数中被实现并执行.
执行后将删除注册表项HKCR\CLSID\{<CLSID>} 键 {<CLSID>}和HKCR\<File Type>\shellex\ContextMenuHandlers下的值。


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

Idiot's的非常完整的外壳程序向导
http://www.codeproject.com/KB/shell/shellextguide1.aspx
http://www.codeproject.com/KB/shell/shellextguide2.aspx
http://www.codeproject.com/KB/shell/shellextguide7.aspx

如何使用上下文菜单的子菜单
http://www.codeproject.com/KB/shell/ctxextsubmenu.aspx


/////////////////////////////////////////////////////////////////////////////