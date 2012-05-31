=============================================================================
      控制台应用程序 : CSCallNativeDllWrapper 项目概述
=============================================================================

/////////////////////////////////////////////////////////////////////////////
摘要:

这个代码示例展示了通过C++/CLI封装类对一个本地C++的DLL模块的导出的类和方法进行封装，
并且被Visual C#代码调用。

  CSCallNativeDllWrapper (.NET应用程序)
          -->
      CppCLINativeDllWrapper (C++/CLI封装)
              -->
          CppDynamicLinkLibrary (本地C++ DLL模块)


/////////////////////////////////////////////////////////////////////////////
示例:

以下步骤演示了.NET-本地内部调用实例.

步骤1. 在Visual Studio 2008中，当你成功的编译完CppDynamicLinkLibrary，CppCLINativeDllWrapper
以及CSCallNativeDllWrapper实例项目后,无论是针对Win32平台或x64平台, 你将会得到应用程序：CSCallNativeDllWrapper.exe 和
两个DLL文件：CppCLINativeDllWrapper.dll and CppDynamicLinkLibrary.dll。它们之间的关系是：
CSCallNativeDllWrapper.exe引用CppCLINativeDllWrapper.dll，而CppCLINativeDllWrapper.dll又引用
CppDynamicLinkLibrary.dll 的导出类和方法。

步骤2. 命令框运行CSCallNativeDllWrapper.程序在控制台输出以下信息： 

    模块 "CppDynamicLinkLibrary" 被加载
    GetStringLength1("HelloWorld") => 10
    GetStringLength2("HelloWorld") => 10
    Max(2, 3) => 3
    Class: CSimpleObject::FloatProperty = 1.20
    模块 "CppDynamicLinkLibrary" 被加载

这个信息显示了CSCallNativeDllWrapper成功地加载了CppDynamicLinkLibrary.dll，并且调用被本地模块
导出的方法(GetStringLength1, GetStringLength2, Max)和类(CSimpleObject)。

注意: 如果你运行调试程序在没有安装Visual Studio 2008的系统上，你也许会收到以下错误提示。

    无法处理的信息: System.IO.FileLoadException: 无法加载文件或程序集'CppCLINativeDllWrapper, Version=1.0.0.0, 
    Culture=neutral, PublicKeyToken=null' 或者其中的一个依赖项. 程序无法打开因为程序配置不正确。
    重新安装可能会解决这个问题。(Exception from HRESULT: 0x800736B1)
    文件名: 'CppCLINativeDllWrapper, Version=1.0.0.0, Culture=neutral, 
    PublicKeyToken=null' ---> System.Runtime.InteropServices.COMException 
    (0x800736B1):  程序无法打开因为程序配置不正确。重新安装可能会解决这个问题。
    (Exception from HRESULT: 0x800736B1)在CSCallNativeDllWrapper.Program.Main(String[] args)

这个情况是由于CppCLINativeDllWrapper和CppDynamicLinkLibrary的编译调试是依赖与Visual Studio 2008
安装一起捆绑的开发环境CRT。你必须运行发布版本的的实例项目在没有开发环境的电脑上。


/////////////////////////////////////////////////////////////////////////////
实例关系:
(当前实例和其他实例之间的关系在Microsoft All-In-One Code Framework http://1code.codeplex.com)

CSCallNativeDllWrapper -> CppCLINativeDllWrapper -> CppDynamicLinkLibrary
C#实例应用程序CSCallNativeDllWrapper调用被C++/CLI封装类CppCLINativeDllWrapper的本地C++DLL模块
CppDynamicLinkLibrary中被导出的方法和类。

/////////////////////////////////////////////////////////////////////////////
演示:

步骤1. 在C#实例应用程序中添加C++/CLI封装类CppCLINativeDllWrapper的引用。CppCLINativeDllWrapper
封装了本地DLL CppDynamicLinkLibrary导出的方法和类。

步骤2. 调用.NET类CSimpleObjectWrapper和NativeMethods，通过CppCLINativeDllWrapper间接访问被本地C++ DLL
CppDynamicLinkLibrary导出的类和方法。例如： 

    CSimpleObjectWrapper obj = new CSimpleObjectWrapper();
    obj.FloatProperty = 1.2F;
    float fProp = obj.FloatProperty;
    Console.WriteLine("Class: CSimpleObject::FloatProperty = {0:F2}", fProp);


/////////////////////////////////////////////////////////////////////////////
参考:

How to: Wrap Native Class for Use by C#
http://msdn.microsoft.com/en-us/library/ms235281.aspx


/////////////////////////////////////////////////////////////////////////////