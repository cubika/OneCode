'*************************** Module Header ******************************\
' Module Name:  ManagedProcess.vb
' Project:	    VBEnumerateAppDomains
' Copyright (c) Microsoft Corporation.
' 
' 此类实现了一个托管进程，包含一个MDbgProcess及一个System.Diagnostics.Process，
' 同时提供了3个静态方法GetManagedProcesses，GetManagedProcessByID和
' GetAppDomainsInCurrentProcess.
'  
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**************************************************************************

Imports System.Collections
Imports System.ComponentModel
Imports System.Runtime.InteropServices
Imports System.Security.Permissions
Imports Microsoft.Samples.Debugging.CorDebug
Imports Microsoft.Samples.Debugging.MdbgEngine
Imports mscoree


<PermissionSet(SecurityAction.LinkDemand, Name:="FullTrust"),
PermissionSet(SecurityAction.InheritanceDemand, Name:="FullTrust")>
Public Class ManagedProcess
    Implements IDisposable
    Private disposed As Boolean = False

    ' 定义了一个执行可控的进程
    Private _mdbgProcess As MDbgProcess

    ''' <摘要>
    ''' 不要初始化MDbgProcess，直到它被调用时再执行初始化
    ''' </摘要>
    Private ReadOnly Property MDbgProcess() As MDbgProcess
        Get
            If _mdbgProcess Is Nothing Then
                Try

                    ' 初始化MDbgEngine对象
                    Dim engine As New MDbgEngine()

                    '给指定进程附加一个调试器
                    '返回代表此进程的MDbgProcess实例
                    If Me.LoadedRuntimes IsNot Nothing _
                        AndAlso Me.LoadedRuntimes.Count() = 1 Then
                        _mdbgProcess = engine.Attach(
                            ProcessID, LoadedRuntimes.First().GetVersionString())
                    Else
                        _mdbgProcess = engine.Attach(
                            ProcessID, CorDebugger.GetDefaultDebuggerVersion())
                    End If

                    _mdbgProcess.Go()

                Catch _COMException As COMException
                    Throw New ApplicationException(
                        String.Format(
                            "进程编号 {0} 的进程不能被附加。" _
                            & "操作被拒绝或者该进程已被附加",
                            ProcessID))
                Catch
                    Throw
                End Try

            End If

            Return _mdbgProcess
        End Get
    End Property

    Private _diagnosticsProcess As Process = Nothing

    ''' <摘要>
    ''' 通过ProcessID获得System.Diagnostics.Process
    ''' </摘要>
    Public ReadOnly Property DiagnosticsProcess() As Process
        Get
            Return _diagnosticsProcess
        End Get
    End Property

    ''' <摘要>
    ''' 进程ID
    ''' </摘要>
    Public ReadOnly Property ProcessID() As Integer
        Get
            Return DiagnosticsProcess.Id
        End Get
    End Property

    ''' <摘要>
    ''' 进程名称
    ''' </摘要>
    Public ReadOnly Property ProcessName() As String
        Get
            Return DiagnosticsProcess.ProcessName
        End Get
    End Property

    ''' <摘要>
    ''' 获得进程中加载的所有运行时
    ''' </摘要>
    Public ReadOnly Property LoadedRuntimes() As IEnumerable(Of CLRRuntimeInfo)
        Get
            Try
                Dim host As New CLRMetaHost()
                Return host.EnumerateLoadedRuntimes(ProcessID)
            Catch e1 As EntryPointNotFoundException
                Return Nothing
            End Try
        End Get
    End Property

    ''' <摘要>
    ''' 获得进程中的所有应用程序域
    ''' </摘要>
    Public ReadOnly Property AppDomains() As IEnumerable
        Get
            Dim _appDomains = MDbgProcess.CorProcess.AppDomains
            Return _appDomains
        End Get
    End Property


    Private Sub New(ByVal processID As Integer)
        Dim diagnosticsProcess As Process = Process.GetProcessById(processID)
        Me._diagnosticsProcess = diagnosticsProcess

        ' 确定指定进程为托管进程
        If Me.LoadedRuntimes Is Nothing OrElse Me.LoadedRuntimes.Count() = 0 Then
            Throw New ArgumentException("此进程为非托管进程")
        End If
    End Sub


    Private Sub New(ByVal diagnosticsProcess As Process)
        If diagnosticsProcess Is Nothing Then
            Throw New ArgumentNullException(
                "diagnosticsProcess", "System.Diagnostics.Process不能为空")
        End If
        Me._diagnosticsProcess = diagnosticsProcess
        If Me.LoadedRuntimes Is Nothing OrElse Me.LoadedRuntimes.Count() = 0 Then
            Throw New ArgumentException("此进程为非托管进程. ")
        End If
    End Sub



    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub


    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        ' 防止被多次调用
        If disposed Then
            Return
        End If

        If disposing Then
            ' 清除所有托管资源
            If _mdbgProcess IsNot Nothing Then

                ' 确保基本进程在datach前已经终止了                    
                Dim waithandler = _mdbgProcess.AsyncStop()
                waithandler.WaitOne()
                _mdbgProcess.Detach()

            End If
        End If

        disposed = True
    End Sub


    ''' <摘要>
    ''' 获得所有托管进程
    ''' </摘要>
    Public Shared Function GetManagedProcesses() As List(Of ManagedProcess)
        Dim managedProcesses As New List(Of ManagedProcess)()

        ' CLR宿主包含一个ICLRMetaHost接口，提供了一个方法，可以列举指定进程加载的所有运行时
        Dim host As New CLRMetaHost()

        Dim processes = Process.GetProcesses()

        For Each diagnosticsProcess As Process In processes
            Try

                ' 列举指定进程加载的所有运行时
                Dim runtimes = host.EnumerateLoadedRuntimes(diagnosticsProcess.Id)

                ' 如果进程加载了CLR，则被认定为托管进程
                If runtimes IsNot Nothing AndAlso runtimes.Count() > 0 Then
                    managedProcesses.Add(New ManagedProcess(diagnosticsProcess))
                End If


                ' 当文件无法找到或操作被拒绝时，EnumerateLoadedRuntimes方法会抛出Win32Exception异常
                ' 例如:目标进程是系统进程或系统闲置进程
            Catch _Win32Exception As Win32Exception

                ' 在x86平台上创建的程序试图在64位操作系统上执行64位进程时，
                ' EnumerateLoadedRuntimes方法会抛出COMException异常
            Catch _COMException As COMException

                ' 再次抛出其他异常
            Catch
                Throw
            End Try
        Next diagnosticsProcess
        Return managedProcesses
    End Function

    ''' <摘要>
    ''' 通过ID获得托管进程。
    ''' 这个方法用于从其他进程中获得应有程序域。如果想要从当前进程中获得应用程序域，
    ''' 请使用静态方法GetAppDomainsInCurrentProcess。
    ''' </摘要>
    ''' <exception cref="ArgumentException">
    ''' 当不存在相应ID的托管进程时，会抛出ArgumentException异常
    ''' </exception>
    Public Shared Function GetManagedProcessByID(ByVal processID As Integer) As ManagedProcess
        If processID = Process.GetCurrentProcess().Id Then
            Throw New ArgumentException("不能调试当前进程。")
        End If
        Return New ManagedProcess(processID)
    End Function

    ''' <摘要>
    ''' 获得当前进程中的所有应用程序域
    ''' 此方法用ICorRuntimeHost接口获得当前进程中的应用程序域的枚举
    ''' </摘要>
    Public Shared Function GetAppDomainsInCurrentProcess() As List(Of AppDomain)
        Dim appDomains = New List(Of AppDomain)()
        Dim hEnum = IntPtr.Zero

        ' CorRuntimeHostClass提供了ICorRuntimeHost接口
        Dim host = New CorRuntimeHost()

        Try
            ' 获得当前进程中的所有应用程序域
            host.EnumDomains(hEnum)
            Do
                Dim domain = New Object
                host.NextDomain(hEnum, domain)
                If domain Is Nothing Then
                    Exit Do
                End If
                appDomains.Add(TryCast(domain, AppDomain))
            Loop
        Catch
            Throw
        Finally
            host.CloseEnum(hEnum)
            Marshal.ReleaseComObject(host)
        End Try
        Return appDomains
    End Function

End Class

