=============================================================================
          WIN32 应用程序：CppUACSelfElevation 项目概述
=============================================================================

/////////////////////////////////////////////////////////////////////////////
摘要：

用户账户控制 （UAC）是Windows Vista及后续操作系统中的一个新安全组件。当UAC被
完全开启时，管理员的交互操作通常运行在普通用户权限下。这个示例演示了如何去检
测当前进程的权限等级，和如何通过许可验证对话框来确认并自我提升此线程的权限等
级。

/////////////////////////////////////////////////////////////////////////////
先决条件：

你必须在Windows Vista及后续操作系统中运行此示例程序。

/////////////////////////////////////////////////////////////////////////////
演示：

以下步骤演示了此UAC示例程序。

步骤1、在使用Visual Studio2008成功生成示例项目后， 你将得到一个应用程序：
CppUACSelfElevation.exe


步骤2、在一个UAC完全开启的Windows Vista或Windows 7系统中，使用受保护的管理员账
户执行此程序。此程序会显示一个对话框包含以下内容：

  用户是否是系统管理员：  是
  是否以管理员身份运行：  否
  进程权限是否被提升：    否
  完整性级别：            中
  
在“自我提升权限”按钮上有一个UAC盾状图标。

步骤3、点击“自我提升权限”按钮， 你将会看到一个许可验证对话框

  
  用户账户控制
  ---------------------------------- 
  您想允许来自未知发布者的以下程序对此计算机进行更改吗？

步骤4、点击“是”允许提升权限。之前的程序会被启动并显示以下内容

  用户是否是系统管理员：  是
  是否以管理员身份运行：  是
  进程权限是否被提升：    是
  完整性级别：            高

此时对话框中的“自我提升权限”按钮没有了之前的UAC盾状图标。这是由于此应用程序
以一个已提升的管理员运行。成功进行权限提升。如果你再次点击“自我提升权限”按钮，
此程序会告知你它已经作为管理员身份运行。

步骤5、 点击[X]关闭此应用程序。

/////////////////////////////////////////////////////////////////////////////
创建过程：


步骤1、创建一个基于对话框的Win32 VC++应用程序

新建一个Visual C++ / Win32 / Win32 项目。把其命名为CppUACSelfElevation并在应用
程序设置页中把应用程序类型设置为Windows应用程序。在程序被创建后，在项目属性/
配置属性/ C/C++ /预编译头中关闭预编译头。在资源视图中，删除所有默认Accelerator
dialog, icon, menu, 和 string table资源。这是由于这些资源在本示例中不是必须的。
接下来， 添加一个dialog资源，其ID设置为：IDD_MAINDIALOG。它作为本Windows应用程
序的主对话框。在解决方案资源管理器中，删除向导所生成的文件：stdafx.cpp, stdafx.h, 
targetver.h, CppUACSelfElevation.h, CppUACSelfElevation.ico,和small.ico。这将
简化此项目。打开CppUACSelfElevation.cpp，使用以下代码替换其内容。以下这些代码
描述了一个VC++ Win32基于对话框应用程序的基本框架。

    #include <stdio.h>
    #include <windows.h>
    #include <windowsx.h>
    #include "Resource.h"

    BOOL OnInitDialog(HWND hWnd, HWND hwndFocus, LPARAM lParam)
    {
        return TRUE;
    }
    void OnCommand(HWND hWnd, int id, HWND hwndCtl, UINT codeNotify)
    {
        switch (id)
        {
        case IDOK:
        case IDCANCEL:
            EndDialog(hWnd, 0);
            break;
        }
    }
    void OnClose(HWND hWnd)
    {
        EndDialog(hWnd, 0);
    }
    INT_PTR CALLBACK DialogProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
    {
        switch (message)
        {
            HANDLE_MSG (hWnd, WM_INITDIALOG, OnInitDialog);
            HANDLE_MSG (hWnd, WM_COMMAND, OnCommand);
            HANDLE_MSG (hWnd, WM_CLOSE, OnClose);

        default:
            return FALSE;
        }
        return 0;
    }
    int APIENTRY wWinMain(HINSTANCE hInstance,
                          HINSTANCE hPrevInstance,
                          LPWSTR    lpCmdLine,
                          int       nCmdShow)
    {
        return DialogBox(hInstance, MAKEINTRESOURCE(IDD_MAINDIALOG), NULL, DialogProc);
    }

步骤2、在主对话框中添加控件
  
  类型：Button
  ID: IDC_ELEVATE_BN
  Caption: "自我提升权限"
  
  
  类型: Static Text
  ID: IDC_INADMINGROUP_STATIC
  用法：即使在还没有为当前用户提升权限的前提下，此控件显示拥有此进程的主访问令
  牌的用户是否是本地管理员组的成员。
  
  类型: Static Text
  ID: IDC_ISRUNASADMIN_STATIC
  用法：此控件显示此程序以管理员身份运行。
  
  类型: Static Text
  ID: IDC_ISELEVATED_STATIC
  用法：此控件显示当前进程的权限是否已经被提升。令牌提升只有在Windows Vista
  及后续版本的Windows中才被支持。此控件在Windows Vista之前版的Windows中显示
  显示N/A。

  类型: Static Text
  ID: IDC_IL_STATIC
  用法：此控件显示当前进程的完整性级别。完整性级别只有在Windows Vista及后续版
  本的Windows中才被支持。此控件在Windows Vista之前版的Windows中显示显示N/A。

步骤3、在程序初始化主对话框时检查并显示当前进程的“以管理员身份运行”的状态，
权限提升信息及完整性级别。

创建以下四个辅助函数：

//
//   函数：IsUserInAdminGroup()
//
//   用途：即使在还没有为当前用户提升权限的前提下，此函数检测拥有此进程的主访
//   问令牌的用户是否是本地管理员组的成员。
//
//   返回值：如果拥有主访问令牌的用户是管理员组成员则返回TRUE，反之则返回FALSE。
//
//   异常：如果此函数出错，它抛出一个包含Win32相关错误代码的C++ DWORD异常。
//
//
//   调用示例：
//     try 
//     {
//         if (IsUserInAdminGroup())
//             wprintf (L"用户是管理员组成员\n");
//         else
//             wprintf (L"用户不是管理员组成员\n");
//     }
//     catch (DWORD dwError)
//     {
//         wprintf(L"IsUserInAdminGroup 调用失败 w/err %lu\n", dwError);
//     }
//
BOOL IsUserInAdminGroup();

// 
//   函数：IsRunAsAdmin()
//
//   用途：此函数检测当前进程是否以管理员身份运行。换而言之，此进程要求用户是
//   拥有主访问令牌的用户是管理员组成员并且已经执行了权限提升。
//
//   返回值：如果拥有主访问令牌的用户是管理员组成员且已经执行了权限提升则返回
//   TRUE，反之则返回FALSE。
//
//   异常：如果此函数出错，它抛出一个包含Win32相关错误代码的C++ DWORD异常。
//
//   调用示例：
//     try 
//     {
//         if (IsRunAsAdmin())
//             wprintf (L"进程以管理员身份运行\n");
//         else
//             wprintf (L"进程没有以管理员身份运行\n");
//     }
//     catch (DWORD dwError)
//     {
//         wprintf(L"IsRunAsAdmin 失败 w/err %lu\n", dwError);
//     }
//
BOOL IsRunAsAdmin();

//
//   函数：IsProcessElevated
//
//   用途：此函数获取当前进程的权限提升信息。它由此进程是否进行了权限提升所
//   决定。令牌权限提升只有在Windows Vista及后续版本的Windows中有效。所以在
//   Windows Vista之前的版本中执行IsProcessElevated， 它会抛出一个C++异常。
//   此函数并不适用于检测是否此进程以管理员身份运行。
//
//   返回值：如果此进程的权限已被提升，返回TRUE，反之则返回FALSE。
//
//
//   异常：如果此函数出错，它抛出一个包含Win32相关错误代码的C++ DWORD异常。
//   比如在Windows Vista之前的Windows中调用IsProcessElevated，被抛出的错误
//   代码为ERROR_INVALID_PARAMETER。
//
//   注意：TOKEN_INFORMATION_CLASS提供了TokenElevationType以便对当前进程的提升
//   类型（TokenElevationTypeDefault / TokenElevationTypeLimited /
//   TokenElevationTypeFull）进行检测。 它和TokenElevation的不同之处在于：当UAC
//   关闭时，即使当前进程已经被提升(完整性级别 == 高)，权限提升类型总是返回
//   TokenElevationTypeDefault。换而言之，以此来确认当前线程的提升类型是不安全的。
//   相对的，我们应该使用TokenElevation。
//
//   调用示例：
//     try 
//     {
//         if (IsProcessElevated())
//             wprintf (L"进程已经提升\n");
//         else
//             wprintf (L"进程尚未提升\n");
//     }
//     catch (DWORD dwError)
//     {
//         wprintf(L"IsProcessElevated 失败 w/err %lu\n", dwError);
//     }
//
BOOL IsProcessElevated();

//
//   函数： GetProcessIntegrityLevel()
//
//   用途：此函数获取当前线程的完整性级别。完整性级别只有在Windows Vista及后
//   续版本的Windows中有效。所以在Windows Vista之前的版本中执行GetProcessIntegrityLevel， 
//   它会抛出一个C++异常。
//
//   返回值：返回当前进程的完整性级别。它可能是以下某一个值。
//
//     SECURITY_MANDATORY_UNTRUSTED_RID (SID: S-1-16-0x0)
//     表示不被信任的级别。它被用于一个匿名组成员起动的进程。这时大多数
//     写入操作被禁止。
//
//     SECURITY_MANDATORY_LOW_RID (SID: S-1-16-0x1000)
//     表示低完整性级别。它被用于保护模式下的IE浏览器。这时大多数系统中对
//     象（包括文件及注册表键值）的写入操作被禁止。
//
//     SECURITY_MANDATORY_MEDIUM_RID (SID: S-1-16-0x2000)
//     表示中完整性级别。它被用于在UAC开启时启动普通应用程序。
//
//
//     SECURITY_MANDATORY_HIGH_RID (SID: S-1-16-0x3000)
//     表示高完整性级别。它被用于用户通过UAC提升权限启动一个管理员应用程序。
//     或则当UAC关闭时，管理员用户启动一个普通程序。
//
//
//     SECURITY_MANDATORY_SYSTEM_RID (SID: S-1-16-0x4000)
//     表示系统完整性级别。它被用于服务或则其他系统级别的应用程序（比如
//     Wininit, Winlogon, Smss，等等）
//
//
//   异常：如果此函数出错，它抛出一个包含Win32相关错误代码的C++ DWORD异常。
//   比如在Windows Vista之前的Windows中调用GetProcessIntegrityLevel，被抛
//   出的错误代码为ERROR_INVALID_PARAMETER。
// 
//   调用示例：
//     try 
//     {
//         DWORD dwIntegrityLevel = GetProcessIntegrityLevel();
//     }
//     catch (DWORD dwError)
//     {
//         wprintf(L"GetProcessIntegrityLevel 失败 w/err %lu\n", dwError);
//     }
//
DWORD GetProcessIntegrityLevel();

在主对话框的OnInitDialog函数中，检查并显示“以管理员身份运行”的状态，权限提升
的信息，以及当前进程的完整性级别。

    // 获取并显示即使在还没有为当前用户提升权限的前提下，拥有此进程的主访
    // 问令牌的用户是否是本地管理员组的成员。(IsUserInAdminGroup)。
    HWND hInAdminGroupLabel = GetDlgItem(hWnd, IDC_INADMINGROUP_STATIC);
    try
    {
        BOOL const fInAdminGroup = IsUserInAdminGroup();
        SetWindowText(hInAdminGroupLabel, fInAdminGroup ? L"是" : L"否");
    }
    catch (DWORD dwError)
    {
        SetWindowText(hInAdminGroupLabel, L"N/A");
        ReportError(L"IsUserInAdminGroup", dwError);
    }

    // 获取并显示是否此进程以管理员身份运行。（IsRunAsAdmin）。
    HWND hIsRunAsAdminLabel = GetDlgItem(hWnd, IDC_ISRUNASADMIN_STATIC);
    try
    {
        BOOL const fIsRunAsAdmin = IsRunAsAdmin();
        SetWindowText(hIsRunAsAdminLabel, fIsRunAsAdmin ? L"是" : L"否");
    }
    catch (DWORD dwError)
    {
        SetWindowText(hIsRunAsAdminLabel, L"N/A");
        ReportError(L"IsRunAsAdmin", dwError);
    }
    
    // 获取并显示进程权限提升信息(IsProcessElevated)和完整性级别（GetProcessIntegrityLevel）
    // 注意：这些信息在Windows Vista之前的Windows中不存在。

    HWND hIsElevatedLabel = GetDlgItem(hWnd, IDC_ISELEVATED_STATIC);
    HWND hILLabel = GetDlgItem(hWnd, IDC_IL_STATIC);

    OSVERSIONINFO osver = { sizeof(osver) };
    if (GetVersionEx(&osver) && osver.dwMajorVersion >= 6)
    {
        // 运行于Windows Vista或后续版本（主版本号 >= 6）。

        try
        {
         // 获取并显示进程权限提升信息
         
            BOOL const fIsElevated = IsProcessElevated();
            SetWindowText(hIsElevatedLabel, fIsElevated ? L"是" : L"否");

            // 如果进程尚未被提升，更新“自我提升权限”按钮以在UI中显示UAC盾形
            // 图标。宏Button_SetElevationRequiredState（在Commctrl.h中定义）用
            // 于显示或隐藏按钮上的盾形图标。你也可以通过调用SHGetStockIconInfo
            // （参量SIID_SHIELD）来获取此图标。
            
            HWND hElevateBtn = GetDlgItem(hWnd, IDC_ELEVATE_BN);
            Button_SetElevationRequiredState(hElevateBtn, !fIsElevated);
        }
        catch (DWORD dwError)
        {
            SetWindowText(hIsElevatedLabel, L"N/A");
            ReportError(L"IsProcessElevated", dwError);
        }

        try
        {
            // 获取并显示进程的完整性级别
            DWORD const dwIntegrityLevel = GetProcessIntegrityLevel();
            switch (dwIntegrityLevel)
            {
              case SECURITY_MANDATORY_UNTRUSTED_RID: SetWindowText(hILLabel, L"不信任"); break;
            case SECURITY_MANDATORY_LOW_RID: SetWindowText(hILLabel, L"低"); break;
            case SECURITY_MANDATORY_MEDIUM_RID: SetWindowText(hILLabel, L"中"); break;
            case SECURITY_MANDATORY_HIGH_RID: SetWindowText(hILLabel, L"高"); break;
            case SECURITY_MANDATORY_SYSTEM_RID: SetWindowText(hILLabel, L"系统"); break;
            default: SetWindowText(hILLabel, L"未知"); break;
            }
        }
        catch (DWORD dwError)
        {
            SetWindowText(hILLabel, L"N/A");
            ReportError(L"GetProcessIntegrityLevel", dwError);
        }
    }
    else
    {
        SetWindowText(hIsElevatedLabel, L"N/A");
        SetWindowText(hILLabel, L"N/A");
    }

步骤4、在OnCommand中响应“自我提升权限”按钮的单击事件。当用户单击按钮，如果
此进程不是以管理员身份运行，我们调用ShellExecuteEx提升权限。我们可以使用
SHELLEXECUTEINFO.lpVerb = L"runas"来重启本应用程序。

    void OnCommand(HWND hWnd, int id, HWND hwndCtl, UINT codeNotify)
    {
        switch (id)
        {
        case IDC_ELEVATE_BN:
            {
                // 检测当前进程的“以管理员身份运行”的状态
                BOOL fIsRunAsAdmin;
                try
                {
                    fIsRunAsAdmin = IsRunAsAdmin();
                }
                catch (DWORD dwError)
                {
                    ReportError(L"IsRunAsAdmin", dwError);
                    break;
                }

                // 如果此进程不是以管理员身份运行，提升权限等级。
                if (!fIsRunAsAdmin)
                {
                    wchar_t szPath[MAX_PATH];
                    if (GetModuleFileName(NULL, szPath, ARRAYSIZE(szPath)))
                    {
                        // 以管理员身份启动本程序。
                        SHELLEXECUTEINFO sei = { sizeof(sei) };
                        sei.lpVerb = L"runas";
                        sei.lpFile = szPath;
                        sei.hwnd = hWnd;
                        sei.nShow = SW_NORMAL;

                        if (!ShellExecuteEx(&sei))
                        {
                            DWORD dwError = GetLastError();
                            if (dwError == ERROR_CANCELLED)
                            {
                                // 用户拒绝提升
                                // 什么都不做......
                            }
                            else
                            {
                                ReportError(L"ShellExecuteEx", dwError);
                            }
                        }
                        else
                        {
                            EndDialog(hWnd, TRUE);  //  退出
                        }
                    }
                }
                else
                {
                    MessageBox(hWnd, L"此进程已以管理员身份运行", L"UAC", MB_OK);
                }
            }
            break;
        }
    }

步骤5、当进程启动时自动提升其权限。

如果你的应用程序总是要求管理员权限，比如在安装中，每当你的程序要求时，操作系统
会自动提示用户要求权限提升。

如果一个特殊的资源（RT_MANIFEST）被嵌入在可执行程序中，系统会查询并处理
<trustInfo>中的内容。这个示例演示了在清单文件（manifest）中的相关部分。
    <trustInfo xmlns="urn:schemas-microsoft-com:asm.v2">
       <security>
          <requestedPrivileges>
             <requestedExecutionLevel
                level="requireAdministrator"
             />
          </requestedPrivileges>
       </security>
    </trustInfo>

对于等级属性来说，我们有三个可选项。

  a) requireAdministrator 
  此应用程序必须以管理员权限启动，否则它将不能执行。

  b) highestAvailable 
  此应用程序以目前最高权限运行。如果用户登陆为一个管理员账户，这将会出现
  权限提升对话框。如果用户是一个普通用户，此程序以普通用户权限运行（没有
  权限提升对话框）

  c) asInvoker 
  使用调用此应用程序的进程相同的权限来启动这个程序。

在VC++项目中配置提升等级，我们可以打开项目属性对话框，转到连接器/清单文件， 
并选择UAC执行级别。


/////////////////////////////////////////////////////////////////////////////
参考资料：

MSDN: User Account Control
http://msdn.microsoft.com/en-us/library/aa511445.aspx

MSDN: Windows Vista Application Development Requirements for User Account 
Control Compatibility
http://msdn.microsoft.com/en-us/library/bb530410.aspx

MSDN: Windows NT Security
http://msdn.microsoft.com/en-us/library/ms995339.aspx

How to tell if the current user is in administrators group programmatically
http://blogs.msdn.com/junfeng/archive/2007/01/26/how-to-tell-if-the-current-user-is-in-administrators-group-programmatically.aspx


/////////////////////////////////////////////////////////////////////////////