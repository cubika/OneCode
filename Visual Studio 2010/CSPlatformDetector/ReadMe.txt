=============================================================================
            控制台程序： CSPlatformDetector 概述
=============================================================================

/////////////////////////////////////////////////////////////////////////////
摘要:

CSPlatformDetector 演示以下几个与平台检测相关的任务：

1. 检测当前操作系统的名字（例如："Microsoft Windows 7 Enterprise"）
2. 检测当前操作系统的版本（例如："Microsoft Windows NT 6.1.7600.0")
3. 确定当前的操作系统是否是64位的系统。
4. 确定当前的进程是否是64位的进程。
5. 确定任意一个进程是否运行在64位系统。



/////////////////////////////////////////////////////////////////////////////
演示:

以下的步骤详细讲解了如何使用这个例子。

步骤1. 在您成功地在Visual Studio 2010 （针对任意平台）编译后，您将会获得一个应用程序CSPlatformDetector.exe. 

步骤2. 在64位操作系统（如：windows 7 x64 终极版）的command prompt（cmd.exe）中运行这个应用程序。
      这个应用程序在command prompt中会打印出以下信息：

  当前的操作系统： Microsoft Windows 7 终极版
  版本：Microsoft Windows NT 6.1.7600.0
  当前的操作系统是64位
  当前的进程是64位
  

他表明了当前的操作系统是Microsoft Windows 7 终极版，它的版本是 6.1.7600.0.它的操作系统是一个工作站而不是一个
服务器或一个域控制器。当前的进程是64位的进程。


步骤3. 在任务管理器中，找一个在系统中运行的32位进程，并且获得他的进程ID(例如：6100）
.用把这个进程ID作为第一个参数运行CSPlatformDetector.exe，例如：

    CSPlatformDetector.exe 6100

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
Win32_OperatingSystem.Caption值的找到VC#代码.

另一种方法,您可以通过GetVersionEx, GetSystemMetrics, GetProductInfo, 和 GetNativeSystemInfo函数创建
操作系统名的字符串. The MSDN 文档"Getting the System Version" 给出了个C++ 例子:
http://msdn.microsoft.com/en-us/library/ms724429.aspx. 然而，这个解决方案对于新版本的操作系统而言太不灵活了. 

--------------------

B. 获取当前操作系统的版本.
   (例如. "Microsoft Windows NT 6.1.7600.0")

System.Environment.OSVersion property 返回一个OperatingSystem 对象， 
这个对象包含了当前平台的标识符和版本号.
http://msdn.microsoft.com/en-us/library/system.environment.osversion.aspx
http://msdn.microsoft.com/en-us/library/system.operatingsystem.aspx
您可以使用这些号码去快速确认是什么操作系统，
及某个服务包是否安装，等等. 

在示例代码中，Environment.OSVersion.VersionString 获得当前安装在操作系统中的平台
标识符，版本和服务包的连接字符串.例如, 
"Microsoft Windows NT 6.1.7600.0".

--------------------

C.确定当前的操作系统是否是64位的系统.  

Environment.Is64BitOperatingSystem 是.NET Framework 4 中的一个新属性，它
确定当前的操作系统是否是64位系统.
http://msdn.microsoft.com/en-us/library/system.environment.is64bitoperatingsystem.aspx

Environment.Is64BitOperatingSystem的执行是依据这个逻辑:

  如果运行的进程是一个64位进程，那这个操作系统一定是64位. 

  如果运行的进程是一个32位进程，那这个进程可能运行在32位操作系统也
  可能运行在64位操作系统的WOW64下.要确定是否是32位的程序运行在64位
  操作系统中，您可以使用IsWow64Process函数. 

    bool flag;
    return ((Win32Native.DoesWin32MethodExist("kernel32.dll", "IsWow64Process") 
        && Win32Native.IsWow64Process(Win32Native.GetCurrentProcess(), out flag)) 
        && flag);

--------------------

D. 确定当前的进程或任意一个在系统中运行的进程是否是64位进程. 

如果您要确定当前正在运行的进程是否是一个64位进程，
您可以使用Environment.Is64BitProcess，它是 .NET 
Framework 4的一个新属性. 
http://msdn.microsoft.com/en-us/library/system.environment.is64bitprocess.aspx

如果您要确定任意一个运行在系统中的进程是否是64位进程，
您需要确定OS bitness和此系统是否是64位系统，您可以把
目标进程的句柄当作参数传给IsWow64Process()，并调用此
函数来实现.

    static bool Is64BitProcess(IntPtr hProcess)
    {
        bool flag = false;

        if (Environment.Is64BitOperatingSystem)
        {
            // 在64位操作系统中，如果一个进程不运行在WOW64模式下，
            // 这进程一定是64位进程.
            flag = !(NativeMethods.IsWow64Process(hProcess, out flag) && flag);
        }

        return flag;
    }


/////////////////////////////////////////////////////////////////////////////
参考:

MSDN: Environment.Is64BitOperatingSystem Property 
http://msdn.microsoft.com/en-us/library/system.environment.is64bitoperatingsystem.aspx

MSDN: Environment.Is64BitProcess Property 
http://msdn.microsoft.com/en-us/library/system.environment.is64bitprocess.aspx

MSDN: Getting the System Version
http://msdn.microsoft.com/en-us/library/ms724429.aspx

How to detect programmatically whether you are running on 64-bit Windows
http://blogs.msdn.com/oldnewthing/archive/2005/02/01/364563.aspx


/////////////////////////////////////////////////////////////////////////////