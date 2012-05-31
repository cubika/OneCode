'******************************** 模块头 **********************************\
' 模块名:      NativeMethod.vb
' 项目名:      VBCreateLowIntegrityProcess
' 版权 (c) Microsoft Corporation.
' 
' 一些在此项目中使用的关于Native API P/Invoke注释
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************/

#Region "Imports directives"

Imports System.Runtime.InteropServices
Imports Microsoft.Win32.SafeHandles

#End Region


''' <summary>
''' TOKEN_INFORMATION_CLASS枚举类型定义了获取或设定访问令牌时使用的信息
''' </summary>
Friend Enum TOKEN_INFORMATION_CLASS
    TokenUser = 1
    TokenGroups
    TokenPrivileges
    TokenOwner
    TokenPrimaryGroup
    TokenDefaultDacl
    TokenSource
    TokenType
    TokenImpersonationLevel
    TokenStatistics
    TokenRestrictedSids
    TokenSessionId
    TokenGroupsAndPrivileges
    TokenSessionReference
    TokenSandBoxInert
    TokenAuditPolicy
    TokenOrigin
    TokenElevationType
    TokenLinkedToken
    TokenElevation
    TokenHasRestrictions
    TokenAccessInformation
    TokenVirtualizationAllowed
    TokenVirtualizationEnabled
    TokenIntegrityLevel
    TokenUIAccess
    TokenMandatoryPolicy
    TokenLogonSid
    MaxTokenInfoClass
End Enum


''' <summary>
''' SECURITY_IMPERSONATION_LEVEL枚举定义了安全身份模拟级别。身份模拟级别
''' 定义了一个服务器进程可以扮演一个客户端进程的度量机制。
''' </summary>
Friend Enum SECURITY_IMPERSONATION_LEVEL
    SecurityAnonymous = 0
    SecurityIdentification
    SecurityImpersonation
    SecurityDelegation
End Enum


''' <summary>
''' TOKEN_TYPE枚举类型定义了主令牌及模拟令牌
''' </summary>
''' <remarks></remarks>
Friend Enum TOKEN_TYPE
    TokenPrimary = 1
    TokenImpersonation
End Enum


''' <summary>
''' 这个结构体描述了一个安全标识符（SID）和它的属性。SID被用于描述
''' 唯一的用户或组
''' </summary>
<StructLayout(LayoutKind.Sequential)> _
Friend Structure SID_AND_ATTRIBUTES
    Public Sid As IntPtr
    Public Attributes As UInteger
End Structure


''' <summary>
''' 这个结构描述了令牌的完整性等级。
''' </summary>
<StructLayout(LayoutKind.Sequential)> _
Friend Structure TOKEN_MANDATORY_LABEL
    Public Label As SID_AND_ATTRIBUTES
End Structure


''' <summary>
''' 创建时主窗口时定义的窗体，桌面，标准句柄以及表现形式。
''' </summary>
''' <remarks></remarks>
<StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Unicode)> _
Friend Structure STARTUPINFO
    Public cb As Integer
    Public lpReserved As String
    Public lpDesktop As String
    Public lpTitle As String
    Public dwX As Integer
    Public dwY As Integer
    Public dwXSize As Integer
    Public dwYSize As Integer
    Public dwXCountChars As Integer
    Public dwYCountChars As Integer
    Public dwFillAttribute As Integer
    Public dwFlags As Integer
    Public wShowWindow As Short
    Public cbReserved2 As Short
    Public lpReserved2 As IntPtr
    Public hStdInput As IntPtr
    Public hStdOutput As IntPtr
    Public hStdError As IntPtr
End Structure


''' <summary>
''' 包括新创建的进程和主线程的信息
''' </summary>
''' <remarks></remarks>
<StructLayout(LayoutKind.Sequential)> _
Friend Structure PROCESS_INFORMATION
    Public hProcess As IntPtr
    Public hThread As IntPtr
    Public dwProcessId As Integer
    Public dwThreadId As Integer
End Structure


''' <summary>
''' 表示一个令牌句柄的wrapper类
''' </summary>
Friend Class SafeTokenHandle
    Inherits SafeHandleZeroOrMinusOneIsInvalid

    Private Sub New()
        MyBase.New(True)
    End Sub

    Friend Sub New(ByVal handle As IntPtr)
        MyBase.New(True)
        MyBase.SetHandle(handle)
    End Sub

    Protected Overrides Function ReleaseHandle() As Boolean
        Return NativeMethod.CloseHandle(MyBase.handle)
    End Function

End Class


Friend Class NativeMethod

    ' 令牌所指定的访问权限

    Public Const STANDARD_RIGHTS_REQUIRED As UInt32 = &HF0000
    Public Const STANDARD_RIGHTS_READ As UInt32 = &H20000
    Public Const TOKEN_ASSIGN_PRIMARY As UInt32 = 1
    Public Const TOKEN_DUPLICATE As UInt32 = 2
    Public Const TOKEN_IMPERSONATE As UInt32 = 4
    Public Const TOKEN_QUERY As UInt32 = 8
    Public Const TOKEN_QUERY_SOURCE As UInt32 = &H10
    Public Const TOKEN_ADJUST_PRIVILEGES As UInt32 = &H20
    Public Const TOKEN_ADJUST_GROUPS As UInt32 = &H40
    Public Const TOKEN_ADJUST_DEFAULT As UInt32 = &H80
    Public Const TOKEN_ADJUST_SESSIONID As UInt32 = &H100
    Public Const TOKEN_READ As UInt32 = (STANDARD_RIGHTS_READ Or TOKEN_QUERY)
    Public Const TOKEN_ALL_ACCESS As UInt32 = ( _
    STANDARD_RIGHTS_REQUIRED Or TOKEN_ASSIGN_PRIMARY Or TOKEN_DUPLICATE Or _
    TOKEN_IMPERSONATE Or TOKEN_QUERY Or TOKEN_QUERY_SOURCE Or _
    TOKEN_ADJUST_PRIVILEGES Or TOKEN_ADJUST_GROUPS Or TOKEN_ADJUST_DEFAULT Or _
    TOKEN_ADJUST_SESSIONID)


    Public Const ERROR_INSUFFICIENT_BUFFER As Int32 = 122


    ' 完整性级别

    Public Const SECURITY_MANDATORY_UNTRUSTED_RID As Integer = 0
    Public Const SECURITY_MANDATORY_LOW_RID As Integer = &H1000
    Public Const SECURITY_MANDATORY_MEDIUM_RID As Integer = &H2000
    Public Const SECURITY_MANDATORY_HIGH_RID As Integer = &H3000
    Public Const SECURITY_MANDATORY_SYSTEM_RID As Integer = &H4000


    ' 组相关的SID信息

    Public Const SE_GROUP_MANDATORY As UInt32 = 1
    Public Const SE_GROUP_ENABLED_BY_DEFAULT As UInt32 = 2
    Public Const SE_GROUP_ENABLED As UInt32 = 4
    Public Const SE_GROUP_OWNER As UInt32 = 8
    Public Const SE_GROUP_USE_FOR_DENY_ONLY As UInt32 = &H10
    Public Const SE_GROUP_INTEGRITY As UInt32 = &H20
    Public Const SE_GROUP_INTEGRITY_ENABLED As UInt32 = &H40
    Public Const SE_GROUP_LOGON_ID As UInt32 = &HC0000000UI
    Public Const SE_GROUP_RESOURCE As UInt32 = &H20000000
    Public Const SE_GROUP_VALID_ATTRIBUTES As UInt32 = (SE_GROUP_MANDATORY Or _
        SE_GROUP_ENABLED_BY_DEFAULT Or SE_GROUP_ENABLED Or SE_GROUP_OWNER Or _
        SE_GROUP_USE_FOR_DENY_ONLY Or SE_GROUP_LOGON_ID Or SE_GROUP_RESOURCE Or _
        SE_GROUP_INTEGRITY Or SE_GROUP_INTEGRITY_ENABLED)


    ''' <summary>
    ''' 这个函数打开一个进程所属的访问令牌。
    ''' </summary>
    ''' <param name="hProcess">
    ''' 已打开的访问令牌的线程的句柄。
    ''' </param>
    ''' <param name="desiredAccess">
    ''' 指定一个访问标记。它用于表示访问令牌的访问请求类型。
    ''' </param>
    ''' <param name="hToken">
    ''' 此函数返回时输出一个新开的访问令牌的句柄。
    ''' </param>
    ''' <returns></returns>
    <DllImport("advapi32", CharSet:=CharSet.Auto, SetLastError:=True)> _
    Public Shared Function OpenProcessToken( _
        ByVal hProcess As IntPtr, _
        ByVal desiredAccess As UInt32, _
        <Out()> ByRef hToken As SafeTokenHandle) _
        As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function


    ''' <summary>
    ''' DuplicateTokenEx函数以现有令牌创建一个新的访问令牌。这个函数既可以
    ''' 创建一个主访问令牌也可以创建一个模拟令牌
    ''' </summary>
    ''' <param name="hExistingToken">
    ''' 以TOKEN_DUPLICATE打开的访问令牌的句柄。
    ''' </param>
    ''' <param name="desiredAccess">
    ''' 指定新令牌的访问权限
    ''' </param>
    ''' <param name="pTokenAttributes">
    ''' 一个指向SECURITY_ATTRIBUTES的指针，其为新令牌指定一个安全标示符并
    ''' 指出其子进程时候可以继承令牌。如果lpTokenAttributes为空，此令牌获
    ''' 得一个默认的安全标示符并且此句柄不能被继承。
    ''' </param>
    ''' <param name="ImpersonationLevel">
    ''' 指定一个安全身份模拟级别的枚举类型。此类型将应用于新建的令牌。
    ''' </param>
    ''' <param name="TokenType">
    ''' TokenPrimary - 新令牌是一个主令牌。你可以在CreateProcessAsUser函数
    ''' 中使用此令牌。
    ''' TokenImpersonation - 新令牌是一个模拟令牌。
    ''' </param>
    ''' <param name="hNewToken">
    ''' 获得一个新令牌。
    ''' </param>
    ''' <returns></returns>
    <DllImport("advapi32", CharSet:=CharSet.Auto, SetLastError:=True)> _
    Public Shared Function DuplicateTokenEx( _
        ByVal hExistingToken As SafeTokenHandle, _
        ByVal desiredAccess As UInt32, _
        ByVal pTokenAttributes As IntPtr, _
        ByVal ImpersonationLevel As SECURITY_IMPERSONATION_LEVEL, _
        ByVal TokenType As TOKEN_TYPE, _
        <Out()> ByRef hNewToken As SafeTokenHandle) _
        As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function


    ''' <summary>
    ''' 这个函数用于获取一个访问令牌的信息。调用此函数的进程必须拥有适当的
    ''' 权限才能得到此信息。
    ''' </summary>
    ''' <param name="hToken">
    ''' 获取信息的访问进程的句柄
    ''' </param>
    ''' <param name="tokenInfoClass">
    ''' 指定一个TOKEN_INFORMATION_CLASS枚举中的一个值。它将被用于指定获取
    ''' 信息的种类
    ''' </param>
    ''' <param name="pTokenInfo">
    ''' 用于接收信息的缓存的指针
    ''' </param>
    ''' <param name="tokenInfoLength">
    ''' 指定TokenInformation缓存的大小（以字节计算）
    ''' </param>
    ''' <param name="returnLength">
    ''' 用于记录TokenInformation缓存接收到字节数的变量的指针
    ''' </param>
    ''' <returns></returns>
    <DllImport("advapi32.dll", CharSet:=CharSet.Auto, SetLastError:=True)> _
    Public Shared Function GetTokenInformation( _
        ByVal hToken As SafeTokenHandle, _
        ByVal tokenInfoClass As TOKEN_INFORMATION_CLASS, _
        ByVal pTokenInfo As IntPtr, _
        ByVal tokenInfoLength As Integer, _
        <Out()> ByRef returnLength As Integer) _
        As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function


    ''' <summary>
    ''' 此函数为一个指定的访问令牌设定了不同的信息类型。此信息用于替换
    ''' 现有信息。调用进程必须拥有合适的访问权限以便设置此信息
    ''' </summary>
    ''' <param name="hToken">
    ''' 用于设置信息的访问令牌的句柄。
    ''' </param>
    ''' <param name="tokenInfoClass">
    ''' TOKEN_INFORMATION_CLASS枚举中的一个值。它指定了此函数所指定的
    ''' 信息类型。 
    ''' </param>
    ''' <param name="pTokenInfo">
    ''' 一个指向设置到此访问令牌的信息缓存的指针 
    ''' </param>
    ''' <param name="tokenInfoLength">
    ''' 指定TokenInformation缓存的长度（以字节表示）
    ''' </param>
    ''' <returns></returns>
    <DllImport("advapi32.dll", CharSet:=CharSet.Auto, SetLastError:=True)> _
    Public Shared Function SetTokenInformation( _
        ByVal hToken As SafeTokenHandle, _
        ByVal tokenInfoClass As TOKEN_INFORMATION_CLASS, _
        ByVal pTokenInfo As IntPtr, _
        ByVal tokenInfoLength As Integer) _
        As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function


    ''' <summary>
    ''' 此函数返回一个在安全标识符（SID）中的指定的次级授权的指针。 它的
    ''' 值是一个相对标识符（RID）。
    ''' </summary>
    ''' <param name="pSid">
    ''' 一个指向用于返回次级授权的SID结构体的指针。
    ''' </param>
    ''' <param name="nSubAuthority">
    ''' 用于指定一个索引值。该索引值用于指定在次级授权列表中需要返回的元素
    ''' </param>
    ''' <returns>
    ''' 如果函数调用成功，返回值是一个指向次级授权的指针。可以调用GetLastError获取
    ''' 额外详细的错误信息。如果此函数调用失败，返回值则没有定义。如果指定的SID结构
    ''' 不合法或则次级授权的索引值越界，则此函数调用失败。
    ''' </returns>
    <DllImport("advapi32.dll", CharSet:=CharSet.Auto, SetLastError:=True)> _
    Public Shared Function GetSidSubAuthority( _
        ByVal pSid As IntPtr, _
        ByVal nSubAuthority As UInt32) _
        As IntPtr
    End Function


    ''' <summary>
    ''' 函数ConvertStringSidToSid转换一个字符串形式的安全标示符到一个合法
    ''' 的安全标示符。你可以使用此函数从ConvertSidToStringSid函数转换的字
    ''' 符串形式的标示符中获得一个SID
    ''' </summary>
    ''' <param name="StringSid">用于转换的字符串形式的安全标示符</param>
    ''' <param name="Sid">
    ''' 指向一个用于接收成功转换的SID变量的指针。调用LocalFree函数以便释放
    ''' 返回的缓存
    ''' </param>
    ''' <returns></returns>
    <DllImport("advapi32.dll", CharSet:=CharSet.Auto, SetLastError:=True)> _
    Public Shared Function ConvertStringSidToSid( _
        ByVal StringSid As String, _
        <Out()> ByRef Sid As IntPtr) _
        As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function


    ''' <summary>
    ''' 此函数返回一个合法的安全标示符的长度（以字节计算）
    ''' </summary>
    ''' <param name="pSID">
    ''' 一个指向返回的SID结构的指针
    ''' </param>
    ''' <returns>
    ''' 如果此SID结构是合法的，返回值则为结构的长度（以字符计算）
    ''' </returns>
    <DllImport("advapi32.dll", CharSet:=CharSet.Auto, SetLastError:=True)> _
    Public Shared Function GetLengthSid(ByVal pSID As IntPtr) As Integer
    End Function


    ''' <summary>
    ''' 创建一个新进程及其主线程。新的进程使用指定的用户安全令牌。
    ''' </summary>
    ''' <param name="hToken">
    ''' 主用户令牌的句柄
    ''' </param>
    ''' <param name="applicationName">
    ''' 需要被执行的模块的名字
    ''' </param>
    ''' <param name="commandLine">
    ''' 需要执行的命令行。此字符串的最大长度为32K个字符
    ''' </param>
    ''' <param name="pProcessAttributes">
    ''' 一个指向SECURITY_ATTRIBUTES结构体的指针。他为新的进程指定了一个
    ''' 安全标示符，并且指明是否子进程能够继承此进程的句柄。
    ''' </param>
    ''' <param name="pThreadAttributes">
    ''' 一个指向SECURITY_ATTRIBUTES结构体的指针。他为新的线程指定了一个
    ''' 安全标示符，并且指明是否子线程能够继承此进程的句柄。
    ''' </param>
    ''' <param name="bInheritHandles">
    ''' 如果这个变量为ture， 当前调用进程中每一个可继承的句柄都将被新进程
    ''' 继承。反之，这些句柄则不被继承。
    ''' </param>
    ''' <param name="dwCreationFlags">
    ''' 此标识控制了创建新进程的优先等级。
    ''' </param>
    ''' <param name="pEnvironment">
    ''' 指向一个新进程的环境设定的指针。
    ''' </param>
    ''' <param name="currentDirectory">
    ''' 进程当前文件夹的完整路径。
    ''' </param>
    ''' <param name="startupInfo">
    ''' 表示STARTUPINFO结构体
    ''' </param>
    ''' <param name="processInformation">
    ''' 输出一个PROCESS_INFORMATION结构体。 它包含了新进程的具体信息。
    ''' </param>
    ''' <returns></returns>
    <DllImport("advapi32.dll", CharSet:=CharSet.Auto, SetLastError:=True)> _
    Public Shared Function CreateProcessAsUser( _
        ByVal hToken As SafeTokenHandle, _
        ByVal applicationName As String, _
        ByVal commandLine As String, _
        ByVal pProcessAttributes As IntPtr, _
        ByVal pThreadAttributes As IntPtr, _
        ByVal bInheritHandles As Boolean, _
        ByVal dwCreationFlags As UInt32, _
        ByVal pEnvironment As IntPtr, _
        ByVal currentDirectory As String, _
        ByRef startupInfo As STARTUPINFO, _
        <Out()> ByRef processInformation As PROCESS_INFORMATION) _
        As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function


    ''' <summary>
    ''' 关闭一个已打开的句柄。
    ''' </summary>
    ''' <param name="handle">已打开的对象的合法句柄</param>
    ''' <returns></returns>
    <DllImport("kernel32.dll", CharSet:=CharSet.Auto, SetLastError:=True)> _
    Public Shared Function CloseHandle( _
        ByVal handle As IntPtr) _
        As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

End Class


''' <summary>
''' 已知文件夹路径
''' </summary>
''' <remarks></remarks>
Friend Class KnownFolder

    Private Shared ReadOnly LocalAppDataGuid As Guid = New Guid( _
        "F1B32785-6FBA-4FCF-9D55-7B8E7F157091")
    Public Shared ReadOnly Property LocalAppData() As String
        Get
            Return SHGetKnownFolderPath(KnownFolder.LocalAppDataGuid)
        End Get
    End Property

    Private Shared ReadOnly LocalAppDataLowGuid As Guid = New Guid( _
        "A520A1A4-1780-4FF6-BD18-167343C5AF16")
    Public Shared ReadOnly Property LocalAppDataLow() As String
        Get
            Return SHGetKnownFolderPath(KnownFolder.LocalAppDataLowGuid)
        End Get
    End Property


    ''' <summary>
    ''' 通过文件夹的KNOWNFOLDERID获得已知文件夹的完整路径。
    ''' </summary>
    ''' <param name="rfid">
    ''' 一个指定文件夹KNOWNFOLDERID的引用。
    ''' </param>
    ''' <returns></returns>
    Public Shared Function SHGetKnownFolderPath(ByVal rfid As Guid) As String
        Dim pPath As IntPtr = IntPtr.Zero
        Dim path As String = Nothing
        Try
            Dim hr As Integer = SHGetKnownFolderPath(rfid, 0, IntPtr.Zero, pPath)
            If (hr <> 0) Then
                Throw Marshal.GetExceptionForHR(hr)
            End If
            path = Marshal.PtrToStringUni(pPath)
        Finally
            If (pPath <> IntPtr.Zero) Then
                Marshal.FreeCoTaskMem(pPath)
                pPath = IntPtr.Zero
            End If
        End Try
        Return path
    End Function


    ''' <summary>
    ''' 通过文件夹的KNOWNFOLDERID获取已知文件夹的完整路径。
    ''' </summary>
    ''' <param name="rfid">
    ''' 一个指定文件夹KNOWNFOLDERID的引用。
    ''' </param>
    ''' <param name="dwFlags">
    ''' 特殊获取选项
    ''' </param>
    ''' <param name="hToken">
    ''' 一个作为指定用户的访问令牌。如果此参数为空，此函数以当前用户查询
    ''' 已知文件夹信息。
    ''' </param>
    ''' <param name="pszPath">
    ''' 当此方法返回时，它包含了一个指向已知文件夹路径的字符串的指针。调用此
    ''' 函数的进程需要负责在其不被使用时调用CoTaskMemFree释放此资源。
    ''' </param>
    ''' <returns>HRESULT</returns>
    <DllImport("shell32.dll", CharSet:=CharSet.Auto, SetLastError:=True)> _
    Private Shared Function SHGetKnownFolderPath( _
        <MarshalAs(UnmanagedType.LPStruct)> ByVal rfid As Guid, _
        ByVal dwFlags As UInt32, _
        ByVal hToken As IntPtr, _
        <Out()> ByRef pszPath As IntPtr) _
        As Integer
    End Function

End Class