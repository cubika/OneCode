'******************************** 模块头 **********************************\
' 模块名:      NativeMethod.vb
' 项目名:      VBUACSelfElevation
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
''' WELL_KNOWN_SID_TYPE枚举定义一组常用的安全标识符（SID）。
''' 程序可以传递这些值到CreateWellKnownSid函数，以便为此列表创建一个SID。
''' </summary>
Friend Enum WELL_KNOWN_SID_TYPE
    WinNullSid = 0
    WinWorldSid = 1
    WinLocalSid = 2
    WinCreatorOwnerSid = 3
    WinCreatorGroupSid = 4
    WinCreatorOwnerServerSid = 5
    WinCreatorGroupServerSid = 6
    WinNtAuthoritySid = 7
    WinDialupSid = 8
    WinNetworkSid = 9
    WinBatchSid = 10
    WinInteractiveSid = 11
    WinServiceSid = 12
    WinAnonymousSid = 13
    WinProxySid = 14
    WinEnterpriseControllersSid = 15
    WinSelfSid = 16
    WinAuthenticatedUserSid = 17
    WinRestrictedCodeSid = 18
    WinTerminalServerSid = 19
    WinRemoteLogonIdSid = 20
    WinLogonIdsSid = 21
    WinLocalSystemSid = 22
    WinLocalServiceSid = 23
    WinNetworkServiceSid = 24
    WinBuiltinDomainSid = 25
    WinBuiltinAdministratorsSid = 26
    WinBuiltinUsersSid = 27
    WinBuiltinGuestsSid = 28
    WinBuiltinPowerUsersSid = 29
    WinBuiltinAccountOperatorsSid = 30
    WinBuiltinSystemOperatorsSid = 31
    WinBuiltinPrintOperatorsSid = 32
    WinBuiltinBackupOperatorsSid = 33
    WinBuiltinReplicatorSid = 34
    WinBuiltinPreWindows2000CompatibleAccessSid = 35
    WinBuiltinRemoteDesktopUsersSid = 36
    WinBuiltinNetworkConfigurationOperatorsSid = 37
    WinAccountAdministratorSid = 38
    WinAccountGuestSid = 39
    WinAccountKrbtgtSid = 40
    WinAccountDomainAdminsSid = 41
    WinAccountDomainUsersSid = 42
    WinAccountDomainGuestsSid = 43
    WinAccountComputersSid = 44
    WinAccountControllersSid = 45
    WinAccountCertAdminsSid = 46
    WinAccountSchemaAdminsSid = 47
    WinAccountEnterpriseAdminsSid = 48
    WinAccountPolicyAdminsSid = 49
    WinAccountRasAndIasServersSid = 50
    WinNTLMAuthenticationSid = 51
    WinDigestAuthenticationSid = 52
    WinSChannelAuthenticationSid = 53
    WinThisOrganizationSid = 54
    WinOtherOrganizationSid = 55
    WinBuiltinIncomingForestTrustBuildersSid = 56
    WinBuiltinPerfMonitoringUsersSid = 57
    WinBuiltinPerfLoggingUsersSid = 58
    WinBuiltinAuthorizationAccessSid = 59
    WinBuiltinTerminalServerLicenseServersSid = 60
    WinBuiltinDCOMUsersSid = 61
    WinBuiltinIUsersSid = 62
    WinIUserSid = 63
    WinBuiltinCryptoOperatorsSid = 64
    WinUntrustedLabelSid = 65
    WinLowLabelSid = 66
    WinMediumLabelSid = 67
    WinHighLabelSid = 68
    WinSystemLabelSid = 69
    WinWriteRestrictedCodeSid = 70
    WinCreatorOwnerRightsSid = 71
    WinCacheablePrincipalsGroupSid = 72
    WinNonCacheablePrincipalsGroupSid = 73
    WinEnterpriseReadonlyControllersSid = 74
    WinAccountReadonlyControllersSid = 75
    WinBuiltinEventLogReadersGroup = 76
    WinNewEnterpriseReadonlyControllersSid = 77
    WinBuiltinCertSvcDComAccessGroup = 78
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
''' 在调用GetTokenInformation获得令牌或者调用SetTokenInformation设置令牌时，
''' TOKEN_ELEVATION_TYPE枚举定义了对令牌权限提升等级的类型
''' </summary>
Friend Enum TOKEN_ELEVATION_TYPE
    TokenElevationTypeDefault = 1
    TokenElevationTypeFull
    TokenElevationTypeLimited
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
''' 这个结构体检测了是否一个令牌已经权限提升
''' </summary>
<StructLayout(LayoutKind.Sequential)> _
Friend Structure TOKEN_ELEVATION
    Public TokenIsElevated As Integer
End Structure


''' <summary>
''' 这个结构描述了令牌的完整性等级。
''' </summary>
<StructLayout(LayoutKind.Sequential)> _
Friend Structure TOKEN_MANDATORY_LABEL
    Public Label As SID_AND_ATTRIBUTES
End Structure


''' <summary>
''' 用于令牌句柄的包装类
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

    <DllImport("kernel32.dll", CharSet:=CharSet.Auto, SetLastError:=True)> _
    Friend Shared Function CloseHandle(ByVal handle As IntPtr) As Boolean
    End Function

    Protected Overrides Function ReleaseHandle() As Boolean
        Return SafeTokenHandle.CloseHandle(MyBase.handle)
    End Function

End Class


Friend Class NativeMethod

    ' 定义令牌的访问权限

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
    Public Const TOKEN_ALL_ACCESS As UInt32 = (STANDARD_RIGHTS_REQUIRED Or _
        TOKEN_ASSIGN_PRIMARY Or TOKEN_DUPLICATE Or TOKEN_IMPERSONATE Or _
        TOKEN_QUERY Or TOKEN_QUERY_SOURCE Or TOKEN_ADJUST_PRIVILEGES Or _
        TOKEN_ADJUST_GROUPS Or TOKEN_ADJUST_DEFAULT Or TOKEN_ADJUST_SESSIONID)


    Public Const ERROR_INSUFFICIENT_BUFFER As Int32 = 122


    ' 完整性级别

    Public Const SECURITY_MANDATORY_UNTRUSTED_RID As Integer = 0
    Public Const SECURITY_MANDATORY_LOW_RID As Integer = &H1000
    Public Const SECURITY_MANDATORY_MEDIUM_RID As Integer = &H2000
    Public Const SECURITY_MANDATORY_HIGH_RID As Integer = &H3000
    Public Const SECURITY_MANDATORY_SYSTEM_RID As Integer = &H4000


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
    ''' 此函数创建一个新的从当前令牌复制的访问令牌。
    ''' </summary>
    ''' <param name="ExistingTokenHandle">
    ''' 以TOKEN_DUPLICATE打开的访问令牌的句柄。
    ''' </param>
    ''' <param name="ImpersonationLevel">
    ''' 指定一个安全身份模拟级别的枚举类型。此类型将应用于新建的令牌。
    ''' </param>
    ''' <param name="DuplicateTokenHandle">
    ''' 返回复制令牌的句柄。
    ''' </param>
    ''' <returns></returns>
    <DllImport("advapi32.dll", CharSet:=CharSet.Auto, SetLastError:=True)> _
    Public Shared Function DuplicateToken( _
        ByVal ExistingTokenHandle As SafeTokenHandle, _
        ByVal ImpersonationLevel As SECURITY_IMPERSONATION_LEVEL, _
        <Out()> ByRef DuplicateTokenHandle As SafeTokenHandle) _
        As Boolean
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
    ''' 为一个用于设置提升请求状态按钮或链接显示提升图标
    ''' </summary>
    ''' <remarks></remarks>
    Public Const BCM_SETSHIELD As UInt32 = &H160C


    ''' <summary>
    ''' 向一个或多个窗口发送指定的消息。 这个函数调用指定窗体的回调函数。
    ''' 在窗体尚未处理此消息之前，它不会被返回。
    ''' </summary>
    ''' <param name="hWnd">
    ''' 接收及处理此消息的窗体的句柄。
    ''' </param>
    ''' <param name="Msg">发送的消息</param>
    ''' <param name="wParam">
    ''' 消息指定的额外的信息
    ''' </param>
    ''' <param name="lParam">
    ''' 消息指定的额外的信息
    ''' </param>
    ''' <returns></returns>
    <DllImport("user32", CharSet:=CharSet.Auto, SetLastError:=True)> _
    Public Shared Function SendMessage( _
        ByVal hWnd As IntPtr, _
        ByVal Msg As UInt32, _
        ByVal wParam As Integer, _
        ByVal lParam As IntPtr) _
        As Integer
    End Function


    ''' <summary>
    ''' 此函数返回一个在安全标识符中的指定的次级授权的指针。 它的值是一个
    ''' 相对标识符。
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

End Class
