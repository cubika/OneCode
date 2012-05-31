=============================================================================
        APPLICATION : CSCreateLowIntegrityProcess Project Overview
        应用程序：CSCreateLowIntegrityProcess项目概述
=============================================================================

/////////////////////////////////////////////////////////////////////////////
摘要：

这个代码示例演示了如何启动一个低完整性进程。当你点击本程序中“以低完整等级执
行本程序”按钮，此应用程序使用低完整性再次启动一个本程序实例。低完整性进程只
能在低完整性区域内写入数据，比如%USERPROFILE%\AppData\LocalLow文件夹或者注册
表中的HKEY_CURRENT_USER\Software\AppDataLow键值。即使当前用户的SID在自由访问
控制列表（discretionary access control list）中拥有写入权限，如果你想要访问一
个完整性高的对象，你也将会收到一个无法访问的错误。

默认情况下，子进程继承其父进程的完整性等级。要启动一个低完整性进程，你必须使用
CreateProcessAsUser和低完整性访问令牌启动一个新的子进程。详细信息请参考示例
CreateLowIntegrityProcess中的相关函数。


/////////////////////////////////////////////////////////////////////////////
先决条件：

你必须在Windows Vista及后续版本的操作系统中执行此示例。

/////////////////////////////////////////////////////////////////////////////
演示：

以下步骤演示了此低完整性进程示例。

步骤1、在Visual Studio 2008中成功生成此项目后，你将得到以下应用程序：
CSCreateLowIntegrityProcess.exe. 

步骤2、在Windows Vista或Windows 7中，当UAC完全开启的情况下，以普通用户运行此
应用程序，此应用程序在主对话框中会显示以下内容：


  当前完整度级别：中

  [以低完整级别执行本程序]

  测试：

  [写入LocalAppData文件夹]
  [写入LocalAppDataLow文件夹]


当前进程运行于中完整性级别。如果你点击[写入LocalAppData文件夹]或者[写入
LocalAppDataLow文件夹]按钮，你会得到一个文件写入成功的消息框。

步骤3、点击 [以低完整级别执行本程序]按钮。一个新的CSCreateLowIntegrityProcess
应用程序实例被启动，但是此时它运行于低完整性级别。

  当前完整度级别：低
  
  [以低完整级别执行本程序]
  
  测试：
  [写入LocalAppData文件夹]
  [写入LocalAppDataLow文件夹]
  
此时如果你单击[写入LocalAppData文件夹]按钮，你会得到一个0x80070005错误(Access Denied)，
并得到以下信息：

  禁止访问'%USERPROFILE%\AppData\Local\testfile.txt'

这表示低完整性进程禁止写入LocalAppData文件夹（%USERPROFILE%\AppData\Local\）．
这是由于此文件夹要求更高的完整性级别．

  accesschk "%USERPROFILE%\AppData\Local" -d
  %USERPROFILE%\AppData\Local
    Medium Mandatory Level (Default) [No-Write-Up]
    RW NT AUTHORITY\SYSTEM
    RW BUILTIN\Administrators
    RW [MachineName]\[UserName]

然而，当你点击[写入LocalAppDataLow文件夹]按钮，低完整性级别进程能够写入
LocalAppDataLow文件夹（%USERPROFILE%\AppData\LocalLow\）。这是由于此文件
夹拥有与进程相同的完整性级别。


  accesschk "%USERPROFILE%\AppData\LocalLow" -d
  %USERPROFILE%\AppData\LocalLow
    Low Mandatory Level [No-Write-Up]
    RW NT AUTHORITY\SYSTEM
    RW BUILTIN\Administrators
    RW [MachineName]\[UserName]

步骤4、关闭程序及清理应用程序在%USERPROFILE%\AppData\Local\和 
%USERPROFILE%\AppData\LocalLow\中所创建的测试文件（testfile.txt）。


/////////////////////////////////////////////////////////////////////////////
程序逻辑：

A. 以低完整性启动一个进程

默认情况下，子进程继承其父进程的完整性等级。要启动一个低完整性进程，你必须使用
CreateProcessAsUser和低完整性访问令牌启动一个新的子进程。

启动一个低完整性进程

  1) P/Invok OpenProcessToken和DuplicateTokenEx复制当前进程的句柄，它拥有中完
     整性级别

  2）P/Invok SetTokenInformation设置访问进程的完整性级别为低。
  
  3）P/Invoke CreateProcessAsUser及低完整性级别的访问令牌创建一个新的进程。


CreateProcessAsUser更新了新的子进程的安全标示符和访问令牌的安全标示符为低完整
性级别。示例中的CreateLowIntegrityProcess函数演示了此过程。

B. 检测当前进程的完整性级别

示例中的GetProcessIntegrityLevel函数演示了如何获取当前进程的完整性级别。

  1）P/Invoke OpenProcessToken以TOKEN_QUERY开启此线程的主访问令牌。
  
  2）P/Invoke GetTokenInformation从主访问令牌中获取TokenIntegrityLevel信息。



/////////////////////////////////////////////////////////////////////////////
参考资料：

MSDN: Designing Applications to Run at a Low Integrity Level
http://msdn.microsoft.com/en-us/library/bb625960.aspx

MSDN: Understanding and Working in Protected Mode Internet Explorer
http://msdn.microsoft.com/en-us/library/bb250462(VS.85).aspx


/////////////////////////////////////////////////////////////////////////////