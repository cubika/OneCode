'************************************ 模块头 **************************************
'* 模块名:	MainModule.vb
'* 项目名:		VBProcessWatcher
'* 版权 (c) Microsoft Corporation.
'* 
'* 这个项目演示如何使用Windows Management Instrumentation(WMI)来检测进程的创建/修改/关闭事件.
'*
'* This source is subject to the Microsoft Public License.
'* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
'* All other rights reserved.
'* 
'* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
'* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
'* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*****************************************************************************************
Imports VBProcessWatcher.WMI
Imports VBProcessWatcher.VBProcessWatcher

    Module MainModule

    Dim processName As String = "notepad.exe" ' 默认进程名.

        Sub Main(ByVal args As String())
            If args.Length > 0 Then
                processName = args(0)
            End If

            Dim procWatcher As New ProcessWatcher(processName)
            AddHandler procWatcher.ProcessCreated, AddressOf procWatcher_ProcessCreated
            AddHandler procWatcher.ProcessDeleted, AddressOf procWatcher_ProcessDeleted
            AddHandler procWatcher.ProcessModified, AddressOf procWatcher_ProcessModified
            
            procWatcher.Start()

        Console.WriteLine(processName & " 正在被监控...")
        Console.WriteLine("按回车键停止监控...")

            Console.ReadLine()

            procWatcher.Stop()
        End Sub

    Private Sub procWatcher_ProcessCreated(ByVal proc As Win32.Process)
        Console.Write(vbCrLf & "进程被创建 " & vbCrLf & proc.Name & " " & proc.ProcessId & "  " & "时间:" & DateTime.Now)
    End Sub


    Private Sub procWatcher_ProcessDeleted(ByVal proc As Win32.Process)
        Console.Write(vbCrLf & "进程被关闭 " & vbCrLf & proc.Name & " " & proc.ProcessId & "  " & "时间:" & DateTime.Now)
    End Sub

    Private Sub procWatcher_ProcessModified(ByVal proc As Win32.Process)
        Console.Write(vbCrLf & "进程被修改 " & vbCrLf & proc.Name & " " & proc.ProcessId & "  " & "时间:" & DateTime.Now)
    End Sub
    End Module


