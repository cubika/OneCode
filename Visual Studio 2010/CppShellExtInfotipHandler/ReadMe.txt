=============================================================================
    　　　　　 动态链接库 : CppShellExtInfotipHandler项目描述
=============================================================================

/////////////////////////////////////////////////////////////////////////////
摘要:

该代码块演示了如何用C++创建一个Shell 信息提示处理程序。信息提示处理程序是一
个Shell扩展处理程序，当鼠标悬停在文件上时，会弹出文本提示信息。这是一个非常灵
活的信息定制提示处理程序。另外的一种方法是指定一个固定的字符串或者文件的
属性列表显示提示信息（详细信息请点击以下链接：
 http://msdn.microsoft.com/en-us/library/cc144067.aspx)


 例子中的 信息提示处理程序的 CLSID是： 
    {A67511FE-371A-498D-9372-A27FDA58BE60}

该例子是显示一个.cpp文件的定制的提示信息。
当你的鼠标悬停在一个.cpp文件上时，你会看到以下文本信息：
    File: <File path, e.g. D:\Test.cpp>
    Lines: <Line number, e.g. 123 or N/A>
    -  通过CppShellExtInfotipHandler显示信息提示信息
    


/////////////////////////////////////////////////////////////////////////////
安装和移除：
A. 安装

如果你想在X64windows操作系统上使用shell扩展，请设置Visual C++的项目在
64-bit平台上运行（详情：http://msdn.microsoft.com/en-us/library/9yb4317s.aspx)。
只有在64-bit平台上能加载64-bit扩展程序的DLL。

如果你想在32位windows操作平台上加载这个扩展程序的话，你可以使用默认配置来建立这个项目。

以管理员身份运行命令提示窗口，浏览到包含CppShellExtInfotipHandler.dll的文件夹，并输入以下命令：

    Regsvr32.exe CppShellExtInfotipHandler.dll

	信息提示处理程序被成功注册，如果你看到一个写有以下信息的对话框：
 

    "DllRegisterServer in CppShellExtInfotipHandler.dll succeeded."

B. 移除

以管理员身份运行命令提示窗口，浏览到包含CppShellExtInfotipHandler.dll的文件夹，并输入以下命令：


    Regsvr32.exe /u CppShellExtInfotipHandler.dll

信息提示处理程序被成功移除，如果你看到一个写有以下信息的对话框：

    "DllUnregisterServer in CppShellExtInfotipHandler.dll succeeded."


/////////////////////////////////////////////////////////////////////////////
演示:

 第一步、

如果你想在X64windows操作系统上使用shell扩展程序的haunted，请设置Visual C++的项目在
64-bit平台上运行（详情：http://msdn.microsoft.com/en-us/library/9yb4317s.aspx)。
只有在64-bit平台上能加载64-bit扩展程序的DLL。

如果你想在32位windows操作平台上加载这个扩展程序的话，你可以使用默认配置来建立这个项目。

第二步、在VIsual Studio 2010 你成功建立了例子中的项目，你会看到这个DLL文件：CppShellExtInfotipHandler.dll
       以管理员身份运行命令提示窗口，浏览到包含CppShellExtInfotipHandler.dll的文件夹，并输入以下命令（不要关闭）：


    Regsvr32.exe CppShellExtInfotipHandler.dll

信息提示处理程序被成功注册，如果你看到一个写有以下信息的对话框：

    "DllRegisterServer in CppShellExtInfotipHandler.dll succeeded."

第三步、在windows Explorer中找到一个.cpp文件（如FileInfotipExt.cpp）
	    然后把鼠标悬停在文件上面，你会看到以下文本信息：
 
    File: <File path, e.g. D:\CppShellExtInfotipHandler\FileInfotipExt.cpp>
    Lines: <Line number, e.g. 123 or N/A>
    - 通过CppShellExtInfotipHandler显示提示信息
第四步、在用一个命令提示窗口，输入以下命令，移除信息提示处理程序
 
    Regsvr32.exe /u CppShellExtInfotipHandler.dll

 

/////////////////////////////////////////////////////////////////////////////
代码逻辑:

A. 创建并配置项目

在Visual Studio 2010，创建一个Visual C++ / Win32 / Win32项目，名字为：
"CppShellExtInfotipHandler".在win32应用程序的向导页的"Application Settings"页，
选择应用程序的类型为DLL，勾选”Empty project"选项。然后使用默认配置，知道点击完成按钮。
 

-----------------------------------------------------------------------------

B. 执行基于  Component Object Model (COM) DLL

shell 扩展处理程序在com 中都是 dll。 所以 创建一个包含DllGetClassObject,
 DllCanUnloadNow, DllRegisterServer, and DllUnregisterServercom 组件，
 添加一个基于IUnknown接口的COM类，为你自己的COM类做好准备。有关文件的代码例子如下：
 
  dllmain.cpp - 实现一个ＣＯＭ　ＤＬＬ需要的 DllMain and the DllGetClassObject, DllCanUnloadNow, 
    DllRegisterServer, DllUnregisterServer 函数方法。

  GlobalExportFunctions.def - 通过module-definition文件调用 DllGetClassObject, DllCanUnloadNow, 
    DllRegisterServer, DllUnregisterServer 函数方法。你需要配置模块定义属性才能使用．ｄｅｆ　文件。
	在项目属性页面配置：　Property  Pages / Linker / Input 　属性页
	 　

  Reg.h/cpp - 定义几个辅助函数: 
    RegisterInprocServer, UnregisterInprocServer

  FileInfotipExt.h/cpp - 定义ＣＯＭ类.在这个文件中你可以找到实现ＩＵｎｋｎｏｗｎ接口的 方法.

  ClassFactory.h/cpp -为ＣＯＭ　类定义了类库。

-----------------------------------------------------------------------------

C.　添加一个类，实现信息提示处理程序，并写进注册表

-----------
实现信息提示处理程序:

在 FileInfotipExt.h/cpp 文件中定义了一个信息提示处理方法.该方法必须实现ＩＱｕｅｒｙＩｎｆｏ接口，
才能创建一个提示文本。并且初始化时要实现 IPersistFile 接口。
　

    class FileInfotipExt : IPersistFile, IQueryInfo
    {
    public:
        // IPersistFile
        IFACEMETHODIMP GetClassID(CLSID *pClassID);
        IFACEMETHODIMP IsDirty(void);
        IFACEMETHODIMP Load(LPCOLESTR pszFileName, DWORD dwMode);
        IFACEMETHODIMP Save(LPCOLESTR pszFileName, BOOL fRemember);
        IFACEMETHODIMP SaveCompleted(LPCOLESTR pszFileName);
	    IFACEMETHODIMP GetCurFile(LPOLESTR *ppszFileName);

        // IQueryInfo
        IFACEMETHODIMP GetInfoTip(DWORD dwFlags, LPWSTR *ppwszTip);
        IFACEMETHODIMP GetInfoFlags(DWORD *pdwFlags);
    };
	
  1. 实现 IPersistFile

  ｓｈｅｌｌ先调用IPersistFile的Ｌｏａｄ方法，并当鼠标悬停在文件按名字上面时
  调用它的加载方法。
　 
  IPersistFile::Load 打开指定文件，并且初始化想要的数据。
  在这个代码例子中，我们保存文件的绝对路径。 
  　

    IFACEMETHODIMP FileInfotipExt::Load(LPCOLESTR pszFileName, DWORD dwMode)
    {
        // pszFileName包含被打开文件的绝对路径。
        return StringCchCopy(
            m_szSelectedFile, 
            ARRAYSIZE(m_szSelectedFile), 
            pszFileName);
    }

  2. 实现 IQueryInfo

  在IPersistFile查询之后，ｓｈｅｌｌ　调用IQueryInfo的GetInfoTip方法。GetInfoTip
  有一个可输出LPWSTR *类型的参数ppwszTip，它接收ｔｏｏｌ　ｔｒｉｐ　的内存地址。
  请注意：　必须调用CoTaskMemAlloc为*ppwszTip分配内存空间。　ｓｈｅｌｌ会调用
  After IPersistFile is queried, the Shell queries the IQueryInfo interface 
  and the GetInfoTip method is called. GetInfoTip has an out parameter CoTaskMemFree
  去释放内存，当信息提示不在需要的时候。
 　
 在这段代码中，信息提示文本信息包含文件路径和文件代码行数。

  　

    const int cch = MAX_PATH + 512;
    *ppwszTip = static_cast<LPWSTR>(CoTaskMemAlloc(cch * sizeof(wchar_t)));
    if (*ppwszTip == NULL)
    {
        return E_OUTOFMEMORY;
    }

    // 先准备信息提示文本infotip. 在这段代码中，信息提示文本信息包含文件路径和文件代码行数。
    wchar_t szLineNum[50];
    ...

    HRESULT hr = StringCchPrintf(*ppwszTip, cch, 
        L"File: %s\nLines: %s\n- Infotip displayed by CppShellExtInfotipHandler", 
        m_szSelectedFile, szLineNum);
    if (FAILED(hr))
    {
        CoTaskMemFree(*ppwszTip);
    }

The IQueryInfo::GetInfoFlags 方法目前没用到，我们简单的让它返回E_NOTIMPL。

-----------
为一个类显示提示信息:

为一个类文件显示提示信息。该处理方法添加了下面的注册项：
　
    HKEY_CLASSES_ROOT\<File Type>\shellex\{00021500-0000-0000-C000-000000000046}

注册信息提示信息是实现dllmain.cpp里的DllRegisterServer方法。DllRegisterServer首
先调用Reg.h/cpp的RegisterInprocServer方法，注册一个ＣＯＭ组件。然后调用RegisterShellExtInfotipHandler
处理文件类型。如果以‘．’开始的文件类型，那么该方法就试着读取它的包含ＩＤ（文件类型相关）。如果默认值不为空
就用ＩＤ作为文件类型来处理。
　例如：　这段代码是处理＇．ｃｐｐ＇文件类型的，HKCR\.cpp的默认值是‘VisualStudio.cpp.10.0’，
因此我们就用HKCR\VisualStudio.cpp.10.0　代替HKCR\.cpp。
　在本例子中，当代码执行时，一下的注册项和注册码将会被添加到注册表中。
　

    HKCR
    {
        NoRemove CLSID
        {
            ForceRemove {A67511FE-371A-498D-9372-A27FDA58BE60} = 
                s 'CppShellExtInfotipHandler.FileInfotipExt Class'
            {
                InprocServer32 = s '<Path of CppShellExtInfotipHandler.DLL file>'
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
                {00021500-0000-0000-C000-000000000046} = 
                    s '{A67511FE-371A-498D-9372-A27FDA58BE60}'
            }
        }
    }

删除信息提示信息是实现dllmain.cpp里的DllＵｎRegisterServer方法。
它删除了注册项 HKCR\CLSID\{<CLSID>} 和
HKCR\<File Type>\shellex\{00021500-0000-0000-C000-000000000046} .


/////////////////////////////////////////////////////////////////////////////
参考资料:
http://msdn.microsoft.com/en-us/library/cc144105.aspx
http://msdn.microsoft.com/en-us/library/bb761359.aspx
http://www.codeproject.com/KB/shell/ShellExtGuide3.aspx



/////////////////////////////////////////////////////////////////////////////