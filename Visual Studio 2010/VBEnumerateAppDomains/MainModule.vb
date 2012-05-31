'*************************** 模块头 **************************************\
' 模块名:    MainModule.vb
' 项目名:	 VBEnumerateAppDomains
' 版权 (c)   Microsoft Corporation.
' 
' 此文件用来获得输入指令。
' 如果应用程序带参数启动，直接执行指令并推出，否则就显示帮助字幕让用户选择指令
'  
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**************************************************************************

Imports Microsoft.Samples.Debugging.CorDebug

Module MainModule

    Sub Main(ByVal args() As String)

        ' 在当前进程中创建新的应用程序域
        AppDomain.CreateDomain("Hello world!")

        Try
            ' 如果程序没有带参数启动,则显示帮助字幕让用户选择指令
            If args.Length = 0 Then
                ' 除非用户输入了退出指令,否则应用程序不会退出.
                ' 如果指令不正确,则再次循环显示帮助字幕 
                Do
                    Console.WriteLine(
                        ControlChars.CrLf _
                        & "请选择一项命令：" & ControlChars.CrLf _
                        & "1: 显示当前进程中的应用程序域." & ControlChars.CrLf _
                        & "2: 列举所以托管进程." & ControlChars.CrLf _
                        & "3: 显示帮助字幕." & ControlChars.CrLf _
                        & "4: 退出程序." & ControlChars.CrLf _
                        & "显示指定进程的应用程序域， " _
                        & "请直接输入""PID""及进程ID" & ControlChars.CrLf _
                        & "比如PID1234.")

                    Dim cmd As String = Console.ReadLine()
                    Dim cmdID As Integer = 0
                    If Integer.TryParse(cmd, cmdID) Then
                        Select Case cmdID
                            Case 1
                                ProcessCommand("CurrentProcess")
                            Case 2
                                ProcessCommand("ListAllManagedProcesses")
                            Case 4
                                Environment.Exit(0)
                            Case Else

                                ' 下次循环再次显示帮助字幕

                        End Select
                    ElseIf cmd.StartsWith("PID", StringComparison.OrdinalIgnoreCase) Then
                        ProcessCommand(cmd)
                    End If

                Loop
            ElseIf args.Length = 1 Then
                ProcessCommand(args(0))
            End If
        Catch ex As Exception
            Console.WriteLine(ex.Message)

            ' 退出代码100表示程序没有成功运行         
            Environment.Exit(100)
        End Try
    End Sub

    Private Sub ProcessCommand(ByVal arg As String)
        ' 列举当前进程中的应用程序域
        If arg.Equals("CurrentProcess", StringComparison.OrdinalIgnoreCase) Then
            Console.WriteLine("正在列出当前进程中的应用程序域...")
            ShowAppDomainsInCurrentProcess()

            ' 列举所有托管进程
        ElseIf arg.Equals("ListAllManagedProcesses", StringComparison.OrdinalIgnoreCase) Then
            Console.WriteLine("正在列出所有托管进程...")
            ListAllManagedProcesses()

            ' 显示指定进程中的应用程序域,必须以"PID"开头
        ElseIf arg.StartsWith("PID", StringComparison.OrdinalIgnoreCase) Then
            Dim pid As Integer = 0
            Integer.TryParse(arg.Substring(3), pid)
            Console.WriteLine(String.Format("正在列出进程编号{0}中的应用程序域 ...", pid))
            ShowAppDomains(pid)

        Else
            Throw New ArgumentException("请输入有效指令。")
        End If

    End Sub
    ''' <摘要>
    ''' 显示当前进程中的所有应用程序域
    ''' </摘要> 
    Private Sub ShowAppDomainsInCurrentProcess()

        ' GetAppDomainsInCurrentProcess是托管进程类的一个静态方法
        ' 此方法用来获得当前进程中的所有应用程序域
        Dim appDomains = ManagedProcess.GetAppDomainsInCurrentProcess()

        For Each appDomain In appDomains
            Console.WriteLine("应用程序域 Id={0}, 名称={1}",
                              appDomain.Id, appDomain.FriendlyName)
        Next appDomain
    End Sub

    ''' <摘要>
    ''' 显示指定进程中的应用程序域
    ''' </摘要>
    ''' <param name="pid"> 参数进程ID</param>
    Private Sub ShowAppDomains(ByVal pid As Integer)
        If pid <= 0 Then
            Throw New ArgumentException("请输入有效指令。")
        End If

        Dim process As ManagedProcess = Nothing
        Try
            ' GetManagedProcessByID是托管进程类的一个静态方法
            ' 此方法是用来获得一个托管进程类的实例。
            ' 如果不存在相应PID的托管进程,将会抛出ArgumentException异常。
            process = ManagedProcess.GetManagedProcessByID(pid)

            For Each appDomain As CorAppDomain In process.AppDomains
                Console.WriteLine("应用程序域 Id={0}, 名称={1}", appDomain.Id, appDomain.Name)
            Next appDomain

        Catch _argumentException As ArgumentException
            Console.WriteLine(_argumentException.Message)
        Catch _applicationException As ApplicationException
            Console.WriteLine(_applicationException.Message)
        Catch ex As Exception
            Console.WriteLine("不能获得该进程. " _
                              & "  确保该进程存在，并且是托管进程。")
        Finally
            If process IsNot Nothing Then
                process.Dispose()
            End If
        End Try
    End Sub

    ''' <摘要>
    ''' 列举所有的托管进程
    ''' </摘要>
    Private Sub ListAllManagedProcesses()

        ' GetManagedProcesses是托管进程类 的一个静态方法
        ' 此方法用来获得当前机器上所有托管进程的列表
        Dim processes = ManagedProcess.GetManagedProcesses()

        For Each process In processes
            Console.WriteLine("ID={0}" & vbTab & "名称={1}",
                              process.ProcessID, process.ProcessName)
            Console.Write("加载运行时: ")
            For Each _runtime In process.LoadedRuntimes
                Console.Write(_runtime.GetVersionString() & vbTab)
            Next _runtime
            Console.WriteLine(vbLf)
        Next process

    End Sub

End Module
