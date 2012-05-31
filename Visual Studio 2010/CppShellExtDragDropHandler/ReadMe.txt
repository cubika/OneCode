=============================================================================
    动态链接库 : CppShellExtDragDropHandler项目概述
=============================================================================

/////////////////////////////////////////////////////////////////////////////
概要:

该示例代码演示了用C++创建一个Shell鼠标拖动的处理。

当用户右键点击一个Shell对象来拖动该对象时，当用户试图想删除放置的对象时一个上下
文菜单被显示，一个鼠标拖动处理程序是一个上下文菜单处理程序，可以将项目添加到这
个上下文菜单。


这个鼠标拖动处理程序的例子具有Class ID(CLSID): 
    {F574437A-F944-4350-B7E9-95B6C7008029}

它在上下文菜单添加了一个菜单项“创建硬链接”。当你右键单击一个文件并拖动这个文件到
一个目录或一个驱动器或桌面，上下文菜单将作为“创建硬链接”的菜单项被显示。通过点击
菜单项，处理程序将在这个拖动位置创建一个拖动文件的硬链接。该链接的名称是“以<源文
件名>的硬链接”。(一个硬链接是一个文件的文件系统的代表，其中该文件在相同的体积内超
过一个引用单一文件的路径，文件的任何改变被显示在引用文件的硬链接的应用程序中，关于
硬链接的更多信息，参考MSDN文档：http://msdn.microsoft.com/en-us/library/aa365006.aspx.）


/////////////////////////////////////////////////////////////////////////////
安装和移除:

A. 安装

如果你要使用Windows系统的一个X64的Shell扩展，请使用项目
配置（http://msdn.microsoft.com/en-us/library/9yb4317s.aspx把Visual C++项目配置64位平台，
只有64位扩展的DLL可以被加载到64位Windows Shell程序中。。如果扩展被加载的是32位Windows系统，
你可以使用默认Win32项目配置，以生成该项目。


在命令提示符下以管理员身份运行，导航到包括生成的CppShellExtDragDropHandler.dll 的文件夹并
输入命令：

    Regsvr32.exe CppShellExtDragDropHandler.dll

这个鼠标拖拽处理程序将被成功注册，如果看到一个消息框的信息是：

    "DllRegisterServer in CppShellExtDragDropHandler.dll succeeded."

B. 移除

在命令提示符下以管理员身份运行，导航到包括生成的CppShellExtDragDropHandler.dll 的文件夹并
输入命令：

    Regsvr32.exe /u CppShellExtDragDropHandler.dll

这个鼠标拖拽处理程序将被成功移除，如果看到一个消息框的信息是：

    "DllUnregisterServer in CppShellExtDragDropHandler.dll succeeded."


/////////////////////////////////////////////////////////////////////////////
演示:

下面的步骤是鼠标拖拽处理程序代码示例的演示。

步骤1. 如果你要使用Windows系统的一个X64的Shell扩展，请使用项目
配置（http://msdn.microsoft.com/en-us/library/9yb4317s.aspx把Visual C++项目配置64位平台，
只有64位扩展的DLL可以被加载到64位Windows Shell程序中。。如果扩展被加载的是32位Windows系统，
你可以使用默认Win32项目配置，以生成该项目。

步骤2.然后你在Visual Studio 2010成功的生成了这个项目示例，你讲获得一个DLL文件
CppShellExtDragDropHandler.dll。作为管理员的身份启动一个命令提示符，导航到包括生成
的CppShellExtDragDropHandler.dll 的文件夹并输入命令：

 
    Regsvr32.exe CppShellExtDragDropHandler.dll

这个鼠标拖拽处理程序将被成功注册，如果看到一个消息框的信息是：

    "DllRegisterServer in CppShellExtDragDropHandler.dll succeeded."

步骤3. 在资源管理器中查找一个文件（如这个样本的文件夹中的ReadMe.txt，在相似的大小中右击这个
文件并且拖动它到一个目录。一个上下文菜单将被作为“硬链接”的菜单项被显示。通过点击这个菜单项，
这个处理应用程序将在拖放的位置中的拖拽的那个文件上创建一个硬链接。这个链接的名字是“硬链接<资
源文件名>”（如“硬链接的ReadMe.txt”）.文件的任何改变被显示在引用文件的硬链接的应用程序中，如果
在拖放的位置中有一个相同的文件名，你将看见一个错误的信息“在这已经有一个相同的文件名。”通过Windows Shell 提示。


步骤4. 在相同的命令提示符下，运行命令

    Regsvr32.exe /u CppShellExtDragDropHandler.dll

为了注销鼠标拖动处理程序的Shell。


/////////////////////////////////////////////////////////////////////////////
实施:

A. 创建和配置这个项目

在 Visual Studio 2010中, 创建一个 Visual C++ / Win32 / Win32 的项目名为
"CppShellExtDragDropHandler". 在Win32应用程序向导的 "应用程序设置" 页中，选择类型为"DLL“的应用
程序并且查看”空项目“选项，然后你点击完成按钮，一个空的Win32 DLL 项目被创建好了，

-----------------------------------------------------------------------------

B. 实现基本组件对象模型（COM）的DLL

Shell扩展处理程序都在进程内的COM对象的DLL中实现的。
在DLL中（和从这导出它们）创建一个基本的COM包括实施的DllGetClassObject, DllCanUnloadNow, 
DllRegisterServer,和 DllUnregisterServer in (and exporting them from) the 
DLL, 添加一个IUnknown接口的基本实现的COM类，对于你的COM类准备类工厂，相关的文件在此示例代码中

  dllmain.cpp - 实现 DllMain 和这个DllGetClassObject, DllCanUnloadNow, 
    DllRegisterServer, DllUnregisterServer 的功能对于COM DLL是必要的 

  GlobalExportFunctions.def -通过module-definition文件从DLL中导出DllGetClassObject, DllCanUnloadNow, 
    DllRegisterServer, DllUnregisterServer. 你必须通过.def文件的链接在项目的属性的属性页中通过配置这
个Module Definition File属性

  Reg.h/cpp - 在注册表中定义COM组件的注册或者注销的功能的重复使用
    RegisterInprocServer, UnregisterInprocServer

  FileDragDropExt.h/cpp - 定义COM类. 在这个文件中你能发现IUnknown接口的基本实现

  ClassFactory.h/cpp - 对于COM类定义类工厂. 

-----------------------------------------------------------------------------

C. 实现鼠标拖动的处理和注册.

-----------
实现鼠标拖动处理:

一个鼠标拖动处理是一个上下文菜单的处理，这个上下文菜单处理可以在上下文菜单中添加多个项 . 实现
基本鼠标拖放处理程序和常规上下文菜单处理程序相同

这个FileDragDropExt.h/cpp文件定义一个鼠标拖放处理程序. 这个鼠标拖放处理程序必须实
现IShellExtInit 和 IContextMenu接口。 

    class FileDragDropExt : IShellExtInit, IContextMenu
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

  1.实现IShellExtInit

在鼠标拖放扩展COM对象被实例化后，IShellExtInit::Initialize方法被调用，在方法的参数之间，pidlFolder是
文件夹的PIDL，这个文件夹是在文件被放的地方。你可以通过调用SHGetPathFromIDList从PIDL中获得目录
  

  pDataObj是一个IDataObject接口指针通过它我们可以检索到被拖文件的名字。
  
从IShellExtInit::Initialize中如果有其它任何S_OK的值被返回，这个鼠标菜单项将不会被使用。


 在代码示例中.这个FileDragDropExt：：Initialize方法检索
  目标目录并保存在成员变量的目录路径中： m_szTargetDir
  
    //获得这个文件被放的目录.
    if (!SHGetPathFromIDList(pidlFolder, this->m_szTargetDir))
    {
		return E_FAIL;
    }

  它还列举了选定的文件和文件夹。如果只*一*文件被
  拖动，并且该文件不是目录要考虑硬链接不工作
  ，该方法存储这个文件名供以后使用，并返回文件名
  S_OK。
.
  2. 实现 IContextMenu

  IShellExtInit::Initialize returns S_OK之后，IContextMenu::QueryContextMenu 将被调用来获取菜单项或者
鼠标拖动的扩展项将被添加。QueryContextMenu 是简单易懂得。用InsertMenuItem或者相似的方法来添加鼠标拖动
的扩展的菜单项。菜单命令标识符必须大于或者等于idCmdFirst并且必须小于idCmdLast。QueryContextMenu必须返
回最大的数字标识符添加到菜单后加一。最好的分配菜单命令的方式是以0开始并且建立在序列中。如果鼠标拖放扩
展不需要添加任何项到菜单中，它应该简单的从QueryContextMenu返回0.在这个示例代码中，我们插入这个菜单项“在
这里创建硬连接”。
IContextMenu::GetCommandString 被调用来检索菜单项原来的数据，例如在菜单项中显示的帮助文本。在这个鼠标拖
放操作例子中，对于命令我们不需要帮助字符串或者变量字符串，所以我们简单的从GetCommandString返回E_NOTIMP。
当通过选择扩展安装菜单项时IContextMenu::InvokeCommand被调用。处理程序在响应“在这里创建硬连接”命令时会在拖
动的文件和新文件之间的放的位置建立硬连接。


-----------
注册处理程序:

鼠标处理程序通常注册在下面的子项中：

    HKEY_CLASSES_ROOT\Directory\shellex\DragDropHandlers\

在某些情况下，你需要在HKCR\Folder下注册它用来在桌面上处理鼠标的drop操作，并且在根目录中
HKCR\Drive来处理鼠标drop操作。

鼠标拖放处理程序的注册是实现在 dllmain.cpp的DllRegisterServer函数中。 DllRegisterServer首先在Reg.h/cpp中调
用 DllRegisterServer函数来注册COM组件。接下来， 它调用RegisterShellExtDragDropHandler在HKEY_CLASSES_ROOT\Directory
\shellex\DragDropHandlers\，HKEY_CLASSES_ROOT\Folder\shellex\DragDropHandlers\和HKEY_CLASSES_ROOT\Drive\shellex\
DragDropHandlers\下注册鼠标拖放处理程序，   

下面的键值是添加在示例处理的注册进程中
 

    HKCR
    {
        NoRemove CLSID
        {
            ForceRemove {F574437A-F944-4350-B7E9-95B6C7008029} = 
                s 'CppShellExtDragDropHandler.FileDragDropExt Class'
            {
                InprocServer32 = s '<Path of CppShellExtDragDropHandler.DLL file>'
                {
                    val ThreadingModel = s 'Apartment'
                }
            }
        }
        NoRemove Directory
        {
            NoRemove shellex
            {
                NoRemove DragDropHandlers
                {
                    {F574437A-F944-4350-B7E9-95B6C7008029} = 
                        s 'CppShellExtDragDropHandler.FileDragDropExt'
                }
            }
        }
        NoRemove Folder
        {
            NoRemove shellex
            {
                NoRemove DragDropHandlers
                {
                    {F574437A-F944-4350-B7E9-95B6C7008029} = 
                        s 'CppShellExtDragDropHandler.FileDragDropExt'
                }
            }
        }
        NoRemove Drive
        {
            NoRemove shellex
            {
                NoRemove DragDropHandlers
                {
                    {F574437A-F944-4350-B7E9-95B6C7008029} = 
                        s 'CppShellExtDragDropHandler.FileDragDropExt'
                }
            }
        }
    }

注销是实现在dllmain.cpp的DllUnregisterServer函数中。 它移走了在HKCR\Directory\shellex\ContextMenuHandlers, 
HKCR\Folder\shellex\ContextMenuHandlers和HKCR\Drive\shellex\ContextMenuHandlers下的HKCR\CLSID\{<CLSID>} 键和 {<CLSID>} 键



/////////////////////////////////////////////////////////////////////////////
参考:

MSDN: 创建鼠标拖放处理程序
http://msdn.microsoft.com/en-us/library/bb776881.aspx#dragdrop

The Complete Idiot's Guide to Writing Shell Extensions - 第四部分
http://www.codeproject.com/KB/shell/shellextguide4.aspx


/////////////////////////////////////////////////////////////////////////////