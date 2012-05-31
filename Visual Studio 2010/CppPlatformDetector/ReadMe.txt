=============================================================================
            控制台程序：  CppPlatformDetector  概述
=============================================================================

/////////////////////////////////////////////////////////////////////////////
摘要:

 CppPlatformDetector  演示以下几个与平台检测相关的任务：

1. 检测当前操作系统的名字（例如："Microsoft Windows 7 Enterprise"）
2. 检测当前操作系统的版本（例如："Microsoft Windows NT 6.1.7600.0")
3. 确定当前的操作系统是否是64位的系统。
4. 确定当前的进程是否是64位的进程。
5. 确定任意一个进程是否运行在64位系统。



/////////////////////////////////////////////////////////////////////////////
演示:

以下的步骤详细讲解了如何使用这个例子。

步骤1. 在您成功地在Visual Studio 2010 （针对任意平台）编译后，您将会获得一个应用程序CppPlatformDetector.exe. 

步骤2. 在64位操作系统（如：windows 7 x64 终极版）的command prompt（cmd.exe）中运行这个应用程序。这个应用程序
      在command prompt中会打印出以下信息：

  当前的操作系统： Microsoft Windows 7 终极版
  版本：Microsoft Windows NT 6.1.7600.0
  当前的操作系统是64位
  当前的进程是64位
  

这表明了当前的操作系统是Microsoft Windows 7 终极版，它的版本是 6.1.7600.0.它的操作系统是一个工作站而不是一个
服务器或一个域控制器。当前的进程是64位的进程。


步骤3. 在任务管理器中，找一个在系统中运行的32位进程，并且获得他的进程ID(例如：6100）.
      用把这个进程ID作为第一个参数运行CppPlatformDetector.exe，例如：

    CppPlatformDetector.exe 6100

这个应用程序将会输出:

  ...
  进程6100不是64位的
 

它表明指定进程不是个64位进程.


/////////////////////////////////////////////////////////////////////////////
代码逻辑:

A. 获取当前操作系统的名字. 
   (例如. "Microsoft Windows 7 企业版")

操作系统的名字（例如. "Microsoft Windows 7 终极版"）可以通过Win32_OperatingSystem WMI类的标题属性获得
(http://msdn.microsoft.com/en-us/library/aa394239.aspx).您可以在 GetOSName 方法中找到需要
Win32_OperatingSystem.Caption值的找到VC++代码.

另一种方法,您可以通过GetVersionEx, GetSystemMetrics, GetProductInfo, 和 GetNativeSystemInfo函数创建
操作系统名的字符串. The MSDN 文档"Getting the System Version" 给出了个例子:
http://msdn.microsoft.com/en-us/library/ms724429.aspx. 然而，这个解决方案对于新版本的操作系统而言太不灵活了. 

--------------------
B. 获取当前操作系统的版本.
   (例如. "Microsoft Windows NT 6.1.7600.0")

由GetVersionEx输出的OSVERSIONINFOEX结构包含了最大和最小的版本号（dwMajorVersion, dwMinorVersion),
编译号（dwBuildNumber),及最新的服务包的最大,最小版本号(wServicePackMajor, wServicePackMinor). 您
可以这些号码去快速确认是什么操作系统，及某个服务包是否安装，等等. OSVERSIONINFOEX.wProductType能够
说明一个操作系统是否是一个工作台或一个服务器或一个域控制器.

在PlatformDetector.h/cpp中的GetOSVersionString函数获得当前安装在操作系统中的平台标识符，版本和服务
包的连接字符串，例如, "Microsoft Windows NT 6.1.7600.0 Workstation".
--------------------
C.确定当前的操作系统是否是64位的系统.  

	//
    //   函数: Is64BitOS()
    //
    //   目的: 这个函数确定当前的操作系统是否是64位的系统.
    //
    //   返回值:如果操作系统是64位的返回TRUE；否则，返回FALSE.
    //
    BOOL Is64BitOS()
	

  如果运行的进程是一个64位进程，那这个操作系统一定是64位. 

  #if defined(_WIN64)
    return TRUE;   //  64位程序只运行在Win64

您可以使用IsWow64Process函数来编程检测您的32位程序是否是运行在64位操作系统中. 

#elif defined(_WIN32)
    // 32位程序运行在32位和64位窗口
    BOOL f64bitOS = FALSE;
    return (SafeIsWow64Process(GetCurrentProcess(), &f64bitOS) && f64bitOS);

SafeIsWow64Process是IsWow64Process API的包装.它用来确定特定的进程是否是运行在
Wow64下.在带SP2的Windows XP系统和带SP1的Window Server 2003之前并不存在
IsWow64Process.为了和不支持IsWow64Process的操作系统兼容，我们可以调用
GetProcAddress来检测 IsWow64Process是否在Kernel32.dll中执行.如果
GetProcAddress成功,那么可以成功地动态调用IsWow64Process.否则,WOW64不出现. 

--------------------

D.  确定当前的进程或任意一个在系统中运行的进程是否是64位进程. 

如果您要确定当前正在运行的进程是否是一个64位进程，您可
以使用VC++预编译符号,这些在编译阶段就被设置的符号只是
简单的返回 true/false.

    BOOL Is64BitProcess(void)
    {
    #if defined(_WIN64)
        return TRUE;   // 64-bit program
    #else
        return FALSE;
    #endif
    }

如果您要确定任意一个运行在系统中的进程是否是64位进程，
您需要确定OS bitness和此系统是否是64位系统，您可以把
目标进程的句柄当作参数传给IsWow64Process()，并调用此
函数来实现.

    BOOL Is64BitProcess(HANDLE hProcess)
    {
	    BOOL f64bitProc = FALSE;

        if (Is64BitOS())
	    {
		    /// 在64位操作系统中，如果一个进程不运行在WOW64模式下，
            // 这进程一定是64位进程.
            f64bitProc = !(SafeIsWow64Process(hProcess, &f64bitProc) && f64bitProc);
	    }

	    return f64bitProc;
    }


/////////////////////////////////////////////////////////////////////////////
参考:

MSDN: Getting the System Version
http://msdn.microsoft.com/en-us/library/ms724429.aspx

How to detect programmatically whether you are running on 64-bit Windows
http://blogs.msdn.com/oldnewthing/archive/2005/02/01/364563.aspx


/////////////////////////////////////////////////////////////////////////////