================================================================================
         Windows应用程序: VBCheckProcessType 概述                        
===============================================================================

/////////////////////////////////////////////////////////////////////////////
摘要:
这个示例演示如何在运行时获取进程类型信息，包括进程是否是64位进程、
托管进程、.NET进程、WPF进程和控制台进程。

注意：
        该应用程序必须运行在Windows Vista或者更高版本的Windows系统上， 因
为EnumProcessModulesEx函数只存在于这些版本的Windows系统中。
////////////////////////////////////////////////////////////////////////////////
演示：

步骤1. 运行 VBCheckProcessType.exe.

步骤2. 点击刷新按钮就能在DataGridView中看到所有的进程。

/////////////////////////////////////////////////////////////////////////////
代码逻辑:

1. 添加NativeMethods类来存放必要的Windows API（Kernel32.dll, psapi.dll),包括：
   GetConsoleMode
   GetStdHandle
   AttachConsole
   FreeConsole
   IsWow64Process
   EnumProcessModulesEx
   GetModuleFileNameEx


2. 设计RunningProcess类来存放一个System.Diagnostics.Process实例并检查进程类型。

  为了判定一个进程是否是一个运行于64位系统上的64位进程，我们可以使用Windows API中的
  IsWow64Process函数。
  
  为了判定一个进程是否是一个托管进程，我们可以检查.NET运行时执行引擎MSCOREE.dll是否
  被加载。

  为了判定一个进程是否是一个托管进程，我们可以检查CLR.dll是否被加载。 在.NET4.0之前的
  版本中，工作站的公共语言运行时叫做MSCORWKS.DLL，而在.NET 4.0中，这个dll被替换成了
  CLR.dll。
   
  为了判定一个进程是否是一个WPF进程，我们可以检查PresentationCore.dll是否被加载。

  为了判定一个进程是否是一个控制台进程，我们可以检查目标进程是否是一个控制台窗体。
   
3. 设计主界面MainForm来显示所有正在运行的进程类型。

/////////////////////////////////////////////////////////////////////////////
参考:

GetConsoleMode Function
http://msdn.microsoft.com/en-us/library/ms683167(VS.85).aspx

GetStdHandle Function
http://msdn.microsoft.com/en-us/library/ms683231(VS.85).aspx

AttachConsole Function
http://msdn.microsoft.com/en-us/library/ms681952(VS.85).aspx

FreeConsole Function
http://msdn.microsoft.com/en-us/library/ms683150(VS.85).aspx

Determine Whether a Program Is a Console or GUI Application
http://www.devx.com/tips/Tip/33584

EnumProcessModulesEx Function
http://msdn.microsoft.com/en-us/library/ms682633(VS.85).aspx

GetModuleFileNameEx Function
http://msdn.microsoft.com/en-us/library/ms683198(VS.85).aspx

IsWow64Process Function
http://msdn.microsoft.com/en-us/library/ms684139(VS.85).aspx
/////////////////////////////////////////////////////////////////////////////
