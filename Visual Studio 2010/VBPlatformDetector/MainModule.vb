'****************************** 模块头 ******************************\
' 模块名:  MainModule.vb
' 项目名:      VBPlatformDetector
' 版权 (c) Microsoft Corporation.
' 
' VBPlatformDetector 演示以下几个与平台检测相关的任务：

' 1. 检测当前操作系统的名字（例如："Microsoft Windows 7 Enterprise"）
' 2. 检测当前操作系统的版本（例如："Microsoft Windows NT 6.1.7600.0")
' 3. 确定当前的操作系统是否是64位的系统。
' 4. 确定当前的进程是否是64位的进程。
' 5. 确定任意一个进程是否运行在64位系统。
' 
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************/

Imports System.Management
Imports System.Runtime.InteropServices


Module MainModule

    Sub Main(ByVal args As String())

        ' 打印当前操作系统的名字.
        Console.WriteLine("当前的操作系统是: " & GetOSName())

        ' 打印当前操作系统的版本字符串.
        Console.WriteLine("版本: " & Environment.OSVersion.VersionString)

        ' 确定当前操作系统是否是64位.
        Console.WriteLine("当前的操作系统{0}64位", _
            If(Environment.Is64BitOperatingSystem, "", "不是 "))

        ' 确定当前的进程是否是64位的. 
        Console.WriteLine("当前的进程 {0}64-bit", _
            If(Environment.Is64BitProcess, "", "不是 "))

        ' 确定运行在系统中的任意一个进程是否是64位进程.
        If (args.Length > 0) Then
            ' 如果一个进程ID在命令行中被指定，则获取进程ID，并打开进程句柄.
            Dim processId As Integer = 0
            If Integer.TryParse(args(0), processId) Then
                Dim hProcess As IntPtr = NativeMethods.OpenProcess( _
                    NativeMethods.PROCESS_QUERY_INFORMATION, False, processId)
                If (hProcess <> IntPtr.Zero) Then
                    Try
                        ' 检测指定进程是否是64位.
                        Dim is64bitProc As Boolean = Is64BitProcess(hProcess)
                        Console.WriteLine("进程 {0}  {1}64位", _
                            processId.ToString, If(is64bitProc, "", "不是 "))
                    Finally
                        NativeMethods.CloseHandle(hProcess)
                    End Try
                Else
                    Dim errorCode As Integer = Marshal.GetLastWin32Error
                    Console.WriteLine("打开进程({0}) 失败 w/err 0x{1:X}", _
                        processId.ToString, errorCode.ToString)
                End If
            Else
                Console.WriteLine("无效的进程 ID: {0}", processId.ToString)
            End If
        End If

    End Sub


    ''' <summary>
    ''' 获得当前运行的操作系统的名字.例如，
    ''' "Microsoft Windows 7 企业版".
    ''' </summary>
    ''' <returns>当前运行的操作系统的名字</returns>
    Function GetOSName() As String
        Dim searcher As New ManagementObjectSearcher("root\CIMV2", _
            "SELECT Caption FROM Win32_OperatingSystem")
        For Each queryObj As ManagementObject In searcher.Get()
            Return TryCast(queryObj.Item("Caption"), String)
        Next
        Return Nothing
    End Function


    ''' <summary>
    ''' 确定一个特定的进程是否是64位进程.
    ''' </summary>
    ''' <param name="hProcess">进程句柄</param>
    ''' <returns>
    ''' 如果此进程是64位返回true；否则返回false.
    ''' </returns>
    Function Is64BitProcess(ByVal hProcess As IntPtr) As Boolean
        Dim flag As Boolean = False
        If Environment.Is64BitOperatingSystem Then
            ' 在一个64位的操作系统中，如果一个进程不是运行在Wow64模式下，
            ' 这个进程就一定是一个64位进程.
            flag = Not (NativeMethods.IsWow64Process(hProcess, flag) AndAlso flag)
        End If
        Return flag
    End Function

End Module