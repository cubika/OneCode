'************************** 模块头  ******************************'
' 模块名:  RunningProcess.vb
' 项目:      VBCheckProcessType
' 版权 (c) Microsoft Corporation.
' 
' 这个类显示了一个运行时的进程，并判定这个进程是否是一个64位进程、
' 托管进程、.NET进程，WPF进程或控制台进程。
' 
' 为了判定一个进程是否是一个托管进程，我们可以检查.NET运行时执行引擎 MSCOREE.dll是
' 否被加载。
' 
'  为了判定一个进程是否是一个托管进程，我们可以检查CLR.dll是否被加载。.NET 4.0之前的
' 版本中，工作站的公共语言运行时是 MSCORWKS.DLL。而在.NET4.0版本中，这个DLL被替
' 换成CLR.dll。
' 
' 为了判定一个进程是否是一个WPF进程，我们可以检查PresentationCore.dll是否被加载。
' 
'为了判定一个进程是否是一个控制台进程，我们可以检查目标进程是否是一个控制台窗口。
'
'
'This source is subject to the Microsoft Public License.
'See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
'All other rights reserved.
'
'THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
'EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
'WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'************************************************************************'

Imports System.Linq
Imports System.Security.Permissions
Imports System.Text
Imports System.Runtime.InteropServices
Imports System.IO
Imports System.ComponentModel

<PermissionSetAttribute(SecurityAction.LinkDemand, Name:="FullTrust")>
Public Class RunningProcess

    Public Shared ReadOnly Property IsOSVersionSupported() As Boolean
        Get
            Return Environment.OSVersion.Version.Major >= 6
        End Get
    End Property

    ' System.Diagnostics.Process实例。
    Private _diagnosticsProcess As Process

    ''' <summary>
    ''' 进程名称。
    ''' </summary>
    Public ReadOnly Property ProcessName() As String
        Get
            Return Me._diagnosticsProcess.ProcessName
        End Get
    End Property

    ''' <summary>
    ''' 进程ID。
    ''' </summary>
    Public ReadOnly Property Id() As Integer
        Get
            Return Me._diagnosticsProcess.Id
        End Get
    End Property

    ''' <summary>
    ''' 指定进程是否是一个托管的应用程序
    ''' </summary>
    Private _isManaged As Boolean
    Public Property IsManaged() As Boolean
        Get
            Return _isManaged
        End Get
        Private Set(ByVal value As Boolean)
            _isManaged = value
        End Set
    End Property

    ''' <summary>
    '''  指定进程是否是一个.Net4.0应用程序。
    ''' </summary>
    Private _isDotNet4 As Boolean
    Public Property IsDotNet4() As Boolean
        Get
            Return _isDotNet4
        End Get
        Private Set(ByVal value As Boolean)
            _isDotNet4 = value
        End Set
    End Property

    ''' <summary>
    ''' 指定进程是否是否是一个控制台应用程序。
    ''' </summary>
    Private _isConsole As Boolean
    Public Property IsConsole() As Boolean
        Get
            Return _isConsole
        End Get
        Private Set(ByVal value As Boolean)
            _isConsole = value
        End Set
    End Property

    ''' <summary>
    ''' 指定进程是否是一个WPF应用程序。
    ''' </summary>
    Private _isWPF As Boolean
    Public Property IsWPF() As Boolean
        Get
            Return _isWPF
        End Get
        Private Set(ByVal value As Boolean)
            _isWPF = value
        End Set
    End Property

    ''' <summary>
    ''' 指定进程是否是一个64位应用程序。
    ''' </summary>
    Private _is64BitProcess As Boolean
    Public Property Is64BitProcess() As Boolean
        Get
            Return _is64BitProcess
        End Get
        Private Set(ByVal value As Boolean)
            _is64BitProcess = value
        End Set
    End Property

    ''' <summary>
    ''' 实例备注。通常是异常信息。
    ''' </summary>
    Private _remarks As String
    Public Property Remarks() As String
        Get
            Return _remarks
        End Get
        Private Set(ByVal value As String)
            _remarks = value
        End Set
    End Property

    Public Sub New(ByVal proc As Process)
        Me._diagnosticsProcess = proc

        Try
            CheckProcess()
        Catch ex As Exception
            Me.Remarks = ex.Message
        End Try
    End Sub

    ''' <summary>
    ''' 检查进程属性。
    ''' </summary>
    Public Sub CheckProcess()
        Dim procID As UInteger = CUInt(Me._diagnosticsProcess.Id)

        ' 用kernel32.dll附加进程控制台到Windows窗体上。
        If NativeMethods.AttachConsole(procID) Then

            ' Use Kernel32.dll get the current process (windows form) std handle,
            ' as we attach the console window before.
            Dim handle As IntPtr = NativeMethods.GetStdHandle(NativeMethods.STD_OUTPUT_HANDLE)
            Dim lp As UInteger = 0
            Me.IsConsole = NativeMethods.GetConsoleMode(handle, lp)
            NativeMethods.FreeConsole()
        End If

        Dim loadedModules As List(Of String) = Me.GetLoadedModules()

        ' 检查.NET运行时执行引擎MSCOREE.dll是否被加载。
        Me.IsManaged = loadedModules.Where(
            Function(m) m.Equals("MSCOREE.dll",
                                 StringComparison.OrdinalIgnoreCase)).Count() > 0
        If Me.IsManaged Then

            ' 检查CLR.dll是否被加载。
            Me.IsDotNet4 = loadedModules.Where(
                Function(m) m.Equals("CLR.dll",
                                     StringComparison.OrdinalIgnoreCase)).Count() > 0

            ' 检查PresentationCore.dll是否被加载。
            Me.IsWPF = loadedModules.Where(
                Function(m) m.Equals("PresentationCore.dll",
                                     StringComparison.OrdinalIgnoreCase) _
                                 OrElse m.Equals("PresentationCore.ni.dll",
                                                 StringComparison.OrdinalIgnoreCase)).Count() > 0
        End If

        Me.Is64BitProcess = Check64BitProcess()
    End Sub

    ''' <summary>
    ''' 使用EnumProcessModulesEx函数来获取所有加载的模块。
    ''' EnumProcessModulesEx函数只能运行在Windows Vista或更好版本的Windows操作系统中。
    ''' </summary>
    ''' <returns></returns>
    Private Function GetLoadedModules() As List(Of String)

        If Environment.OSVersion.Version.Major < 6 Then
            Throw New ApplicationException("该应用程序必须运行在Windows Vista或更高版本" +
                                           "的操作系统。")
        End If
        
        Dim modulesHandles(1023) As IntPtr
        Dim size As Integer = 0

        Dim success As Boolean = NativeMethods.EnumProcessModulesEx(
            Me._diagnosticsProcess.Handle,
            modulesHandles,
            Marshal.SizeOf(GetType(IntPtr)) * modulesHandles.Length,
            size,
            NativeMethods.ModuleFilterFlags.LIST_MODULES_ALL)

        If Not success Then
            Throw New Win32Exception()
        End If

        Dim moduleNames As New List(Of String)()

        For i As Integer = 0 To modulesHandles.Length - 1
            If modulesHandles(i) = IntPtr.Zero Then
                Exit For
            End If

            Dim moduleName As New StringBuilder(1024)

            Dim length As UInteger = NativeMethods.GetModuleFileNameEx(
                Me._diagnosticsProcess.Handle,
                modulesHandles(i),
                moduleName,
                Convert.ToUInt32(moduleName.Capacity))

            If length <= 0 Then
                Dim code As Integer = Marshal.GetLastWin32Error()
                Marshal.ThrowExceptionForHR(code)
            Else
                Dim fileName = Path.GetFileName(moduleName.ToString())
                moduleNames.Add(fileName)
            End If
        Next i

        Return moduleNames
    End Function

    ''' <summary>
    ''' 判定指定的进程是否是一个64位进程。
    ''' </summary>
    ''' <param name="hProcess">进程句柄</param>
    ''' <returns>
    '''  如果是64位进程，返回True；否则，返回false.
    ''' </returns>
    Private Function Check64BitProcess() As Boolean
        Dim flag As Boolean = False

        If Environment.Is64BitOperatingSystem Then
            ' 在64位操作系统中，如果一个进程不在Wow64模式下运行，
            ' 该进程一定是一个64位进程。
            flag = Not (NativeMethods.IsWow64Process(
                        Me._diagnosticsProcess.Handle,
                        flag) AndAlso flag)
        End If

        Return flag
    End Function

End Class
