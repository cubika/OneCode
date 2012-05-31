'*************************** 模块头 ******************************
' 模块名:  NativeMethods.vb
' 项目:      VBCheckProcessType
' 版权 (c) Microsoft Corporation.
' 
' 这个类导入了kernel32.dll 和 psapi.dll的方法。
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*************************************************************************

Imports System.Runtime.InteropServices

Friend NotInheritable Class NativeMethods

    ''' <summary>
    ''' 标准的输出设备。首先，这是一个活动的控制台。
    '''输出缓冲：CONOUT$。
    ''' </summary>
    Friend Const STD_OUTPUT_HANDLE As Integer = -11

    ''' <summary>
    ''' 获取当前控制台输入缓冲的输入模式或者当前控制台输出缓冲的输出模式。
    ''' </summary>
    ''' <param name="hConsoleHandle">
    ''' 控制台输入输出缓冲句柄
    ''' </param>
    ''' <param name="lpMode">
    '''一个指针变量用于获取当前特定的缓冲模式。
    ''' </param>
    <DllImport("kernel32.dll", CharSet:=CharSet.Auto, SetLastError:=True)>
    Friend Shared Function GetConsoleMode(ByVal hConsoleHandle As IntPtr,
                                          <Out()> ByRef lpMode As UInteger) As Boolean
    End Function

    ''' <summary>
    ''' 获取特定的标准设备（标准输入、输出或错误）的句柄。
    ''' </summary>
    ''' <param name="nStdHandle">
    ''' 标准设备。这个参数可以是下面的某一个值：
    ''' STD_INPUT_HANDLE  -10
    ''' STD_OUTPUT_HANDLE -11
    ''' STD_ERROR_HANDLE  -12
    ''' </param>
    <DllImport("kernel32.dll", CharSet:=CharSet.Auto, SetLastError:=True)>
    Friend Shared Function GetStdHandle(ByVal nStdHandle As Integer) As IntPtr
    End Function

    ''' <summary>
    ''' 附加一个进程到特定进程的控制台。
    ''' </summary>
    ''' <param name="dwProcessId">
    ''' 被使用的控制台进程标识。
    ''' </param>
    <DllImport("kernel32.dll", CharSet:=CharSet.Auto, SetLastError:=True)>
    Friend Shared Function AttachConsole(ByVal dwProcessId As UInteger) As Boolean
    End Function

    ''' <summary>
    '''  从控制台分离进程。
    ''' </summary>
    ''' <returns></returns>
    <DllImport("kernel32.dll", CharSet:=CharSet.Auto, SetLastError:=True)>
    Friend Shared Function FreeConsole() As Boolean
    End Function

    ''' <summary>
    ''' 判定特定的进程是否运行在WOW64模式。
    ''' </summary>
    ''' <param name="hProcess">
    '''  一个进程句柄。该进程必须有PROCESS_QUERY_INFORMATION或者
    ''' PROCESS_QUERY_LIMITED_INFORMATION的访问权限。
    ''' System.Diagnostics.Process类的句柄属性。
    ''' 当PROCESS_QUERY_INFORMATION被请求的时候，
    '''Process类将用dwDesiredAccess = 0x1F0FFF来打开进程。
    ''' </param>
    ''' <param name="wow64Process">
    ''' True：进程在WOW64下运行。
    ''' </param>
    ''' <returns>
    ''' 如果函数执行成功，将返回非0值。
    ''' 如果函数执行失败，返回0。
    ''' 为了获取扩展的错误信息，需要执行GetLastError。
    ''' </returns>
    <DllImport("kernel32.dll", CharSet:=CharSet.Auto, SetLastError:=True)>
    Friend Shared Function IsWow64Process(ByVal hProcess As IntPtr,
                                          <Out()> ByRef wow64Process As Boolean) _
                                      As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    ''' <summary>
    '''  在符合过滤后的进程中，获取每个模块的句柄。
    ''' </summary>
    ''' <param name="hProcess">
    ''' 一个进程的句柄。
    ''' </param>
    ''' <param name="lphModule">
    '''  获取模块句柄列表的数组。
    ''' </param>
    ''' <param name="cb">
    '''  lphModule数组的大小，以字节表示。
    ''' </param>
    ''' <param name="lpcbNeeded ">
    ''' 储存lphModule数组中所有句柄的字节数。
    ''' </param>
    ''' <param name="dwFilterFlag">
    '''  过滤条件。这个参数可以是下面值中的一个值：
    ''' </param>
    ''' <returns>
    ''' 如果函数执行成功，将返回非0。
    '''如果函数执行失败，将返回0。
    ''' 为了获取扩展的错误信息，需要执行GetLastError。
    ''' </returns>
    <DllImport("psapi.dll", CharSet:=CharSet.Auto, SetLastError:=True)>
    Friend Shared Function EnumProcessModulesEx(<[In]()> ByVal hProcess As IntPtr,
                                                <Out()> ByVal lphModule() As IntPtr,
                                                <[In]()> ByVal cb As Integer,
                                                <Out()> ByRef lpcbNeeded As Integer,
                                                <[In]()> ByVal dwFilterFlag As ModuleFilterFlags) _
                                            As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    ''' <summary>
    '''  获取包含特定模块文件的有效路径。
    ''' </summary>
    ''' <param name="hProcess">
    ''' 包含模块的进程句柄。
    ''' </param>
    ''' <param name="hModule">
    '''模块句柄。
    '''  如果参数为null, GetModuleFileNameEx将返回在hProcess中特定进程的可执行文件路径。
    ''' </param>
    ''' <param name="lpFilename">
    ''' 指向缓冲的指针，用于获取模块的有效路径。
    ''' </param>
    ''' <param name="nSize">
    ''' 特性，lpFilename缓冲的大小。
    ''' </param>
    ''' <returns>
    ''' 如果函数执行成功，返回值将指定复制到缓冲的字符串长度。
    ''' 如果函数执行失败，返回值是0。
    ''' 为了获取扩展的错误信息，需要执行GetLastError。
    ''' </returns>
    <DllImport("psapi.dll", CharSet:=CharSet.Auto, SetLastError:=True)>
    Friend Shared Function GetModuleFileNameEx(<[In]()> ByVal hProcess As IntPtr,
                                               <[In]()> ByVal hModule As IntPtr,
                                               <Out(), MarshalAs(UnmanagedType.LPTStr)> ByVal lpFilename As System.Text.StringBuilder,
                                               ByVal nSize As UInteger) As UInteger
    End Function

    <Flags()>
    Friend Enum ModuleFilterFlags
        '  // 32位模块列表。
        LIST_MODULES_32BIT = &H1

        ' 64位模块列表。
        LIST_MODULES_64BIT = &H2

        ' 所有模块列表。
        LIST_MODULES_ALL = &H3

        ' 使用默认行为。
        LIST_MODULES_DEFAULT = &H0
    End Enum

End Class
