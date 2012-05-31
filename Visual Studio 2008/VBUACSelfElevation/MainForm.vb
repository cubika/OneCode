'********************************* 模块头 ***********************************\
' 模块名:      MainForm.vb
' 项目名:      VBUACSelfElevation
' 版权 (c) Microsoft Corporation.
' 
' 用户账户控制 （UAC）是Windows Vista及后续操作系统中的一个新安全组件。当UAC被
' 完全开启时，管理员的交互操作通常运行在普通用户权限下。这个示例演示了如何去检
' 测当前进程的权限等级，和如何通过许可验证对话框来确认并自我提升此线程的权限等
' 级。
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER 
' EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF 
' MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***********************************************************************************/

#Region "Imports directives"

Imports Microsoft.Win32.SafeHandles
Imports System.Runtime.InteropServices
Imports System.Security.Principal
Imports System.ComponentModel

#End Region


Public Class MainForm


#Region "Helper Functions for Admin Privileges and Elevation Status"

    ''' <summary>
    ''' 即使在还没有为当前用户提升权限的前提下，此函数检测拥有此进程的主访
    ''' 问令牌的用户是否是本地管理员组的成员。
    ''' </summary>
    ''' <returns>
    ''' 如果拥有主访问令牌的用户是管理员组成员则返回TRUE，反之则返回FALSE。
    ''' </returns>
    ''' <exception cref="System.ComponentModel.Win32Exception">
    ''' 如果任何原生的Windows API函数出错，此函数会抛出一个包含最后错误代码的Win32Exception。
    ''' </exception>
    Friend Function IsUserInAdminGroup() As Boolean
        Dim fInAdminGroup As Boolean = False
        Dim hToken As SafeTokenHandle = Nothing
        Dim hTokenToCheck As SafeTokenHandle = Nothing
        Dim pElevationType As IntPtr = IntPtr.Zero
        Dim pLinkedToken As IntPtr = IntPtr.Zero
        Dim cbSize As Integer = 0

        Try
            ' 打开此进程的主访问令牌（使用TOKEN_QUERY | TOKEN_DUPLICATE）.
            If (Not NativeMethod.OpenProcessToken(Process.GetCurrentProcess.Handle, _
                NativeMethod.TOKEN_QUERY Or NativeMethod.TOKEN_DUPLICATE, hToken)) Then
                Throw New Win32Exception
            End If

            ' 检测是否此系统是Windows Vista或后续版本（主版本号 >= 6）。这是由于自
            ' Windows Vista开始支持链接令牌（LINKED TOKEN），而之前的版本不支持。
            '（主版本 < 6） 
            If (Environment.OSVersion.Version.Major >= 6) Then
                ' 运行于Windows Vista 或后续版本（主版本 >= 6）.
                ' 检测令牌类型：受限（limited）,（已提升）elevated, 
                ' 或者默认（default） 

                ' 为提升类别信息对象分配内存
                cbSize = 4  ' TOKEN_ELEVATION_TYPE的大小
                pElevationType = Marshal.AllocHGlobal(cbSize)
                If (pElevationType = IntPtr.Zero) Then
                    Throw New Win32Exception
                End If

                ' 获取令牌提升类别信息.
                If (Not NativeMethod.GetTokenInformation(hToken, _
                    TOKEN_INFORMATION_CLASS.TokenElevationType, _
                    pElevationType, cbSize, cbSize)) Then
                    Throw New Win32Exception
                End If

                ' 转换TOKEN_ELEVATION_TYPE枚举类型（从原生到.Net）
                Dim elevType As TOKEN_ELEVATION_TYPE = Marshal.ReadInt32(pElevationType)

                ' 如果为受限的，获取相关联的已提升令牌以便今后使用。
                If (elevType = TOKEN_ELEVATION_TYPE.TokenElevationTypeLimited) Then
                    ' 为连接令牌分配内存
                    cbSize = IntPtr.Size
                    pLinkedToken = Marshal.AllocHGlobal(cbSize)
                    If (pLinkedToken = IntPtr.Zero) Then
                        Throw New Win32Exception
                    End If

                    ' 获取连接令牌
                    If (Not NativeMethod.GetTokenInformation(hToken, _
                        TOKEN_INFORMATION_CLASS.TokenLinkedToken, _
                        pLinkedToken, cbSize, cbSize)) Then
                        Throw New Win32Exception
                    End If

                    ' 转换连接令牌的值（从原生到.Net）
                    Dim hLinkedToken As IntPtr = Marshal.ReadIntPtr(pLinkedToken)
                    hTokenToCheck = New SafeTokenHandle(hLinkedToken)
                End If
            End If

            ' CheckTokenMembership要求一个伪装令牌。如果我们仅得到一个链接令牌，那
            ' 它就是一个伪装令牌。如果我们没有得到一个关联式令牌，我们需要复制当前
            ' 令牌以便得到一个伪装令牌。
            If (hTokenToCheck Is Nothing) Then
                If (Not NativeMethod.DuplicateToken(hToken, _
                    SECURITY_IMPERSONATION_LEVEL.SecurityIdentification, _
                    hTokenToCheck)) Then
                    Throw New Win32Exception
                End If
            End If

            ' 检测是否被检测的令牌包含管理员组SID
            Dim id As New WindowsIdentity(hTokenToCheck.DangerousGetHandle)
            Dim principal As New WindowsPrincipal(id)
            fInAdminGroup = principal.IsInRole(WindowsBuiltInRole.Administrator)

        Finally
            ' 集中清理所有已分配的内存资源
            If (Not hToken Is Nothing) Then
                hToken.Close()
                hToken = Nothing
            End If
            If (Not hTokenToCheck Is Nothing) Then
                hTokenToCheck.Close()
                hTokenToCheck = Nothing
            End If
            If (pElevationType <> IntPtr.Zero) Then
                Marshal.FreeHGlobal(pElevationType)
                pElevationType = IntPtr.Zero
            End If
            If (pLinkedToken <> IntPtr.Zero) Then
                Marshal.FreeHGlobal(pLinkedToken)
                pLinkedToken = IntPtr.Zero
            End If
        End Try

        Return fInAdminGroup
    End Function


    ''' <summary>
    ''' 此函数检测当前进程是否以管理员身份运行。换而言之，此进程要求用户是
    ''' 拥有主访问令牌的用户是管理员组成员并且已经执行了权限提升。
    ''' </summary>
    ''' <returns>
    ''' 如果拥有主访问令牌的用户是管理员组成员且已经执行了权限提升则返回
    ''' TRUE，反之则返回FALSE。
    ''' </returns>
    Friend Function IsRunAsAdmin() As Boolean
        Dim principal As New WindowsPrincipal(WindowsIdentity.GetCurrent)
        Return principal.IsInRole(WindowsBuiltInRole.Administrator)
    End Function


    ''' <summary>
    ''' 此函数获取当前进程的权限提升信息。它由此进程是否进行了权限提升所
    ''' 决定。令牌权限提升只有在Windows Vista及后续版本的Windows中有效。所以在
    ''' Windows Vista之前的版本中执行IsProcessElevated， 它会抛出一个C++异常。
    ''' 此函数并不适用于检测是否此进程以管理员身份运行。
    ''' </summary>
    ''' <returns>
    ''' 如果此进程的权限已被提升，返回TRUE，反之则返回FALSE。
    ''' </returns>
    ''' <exception cref="System.ComponentModel.Win32Exception">
    ''' 如果任何原生的Windows API函数出错，此函数会抛出一个包含最后错误代码的Win32Exception。
    ''' </exception>
    ''' <remarks>
    ''' TOKEN_INFORMATION_CLASS提供了TokenElevationType以便对当前进程的提升
    ''' 类型（TokenElevationTypeDefault / TokenElevationTypeLimited /
    ''' TokenElevationTypeFull）进行检测。 它和TokenElevation的不同之处在于：当UAC
    ''' 关闭时，即使当前进程已经被提升(完整性级别 == 高)，权限提升类型总是返回
    ''' TokenElevationTypeDefault。换而言之，以此来确认当前线程的提升类型是不安全的。
    ''' 相对的，我们应该使用TokenElevation。
    ''' </remarks>
    Friend Function IsProcessElevated() As Boolean
        Dim fIsElevated As Boolean = False
        Dim hToken As SafeTokenHandle = Nothing
        Dim cbTokenElevation As Integer = 0
        Dim pTokenElevation As IntPtr = IntPtr.Zero

        Try
            ' 使用TOKEN_QUERY打开进程主访问令牌
            If (Not NativeMethod.OpenProcessToken(Process.GetCurrentProcess.Handle, _
                NativeMethod.TOKEN_QUERY, hToken)) Then
                Throw New Win32Exception
            End If

            ' 为提升信息分配内存
            cbTokenElevation = Marshal.SizeOf(GetType(TOKEN_ELEVATION))
            pTokenElevation = Marshal.AllocHGlobal(cbTokenElevation)
            If (pTokenElevation = IntPtr.Zero) Then
                Throw New Win32Exception
            End If

            ' 获取令牌提升信息
            If (Not NativeMethod.GetTokenInformation(hToken, _
                TOKEN_INFORMATION_CLASS.TokenElevation, _
                pTokenElevation, cbTokenElevation, cbTokenElevation)) Then
                ' 当进程运行于Windows Vista之前的系统中，GetTokenInformation返回
                ' FALSE和错误码ERROR_INVALID_PARAMETER。这是由于这些操作系统不支
                ' 持TokenElevation。
                Throw New Win32Exception
            End If

            ' 转换TOKEN_ELEVATION结构（从原生到.Net）
            Dim elevation As TOKEN_ELEVATION = Marshal.PtrToStructure( _
            pTokenElevation, GetType(TOKEN_ELEVATION))

            ' 如果令牌权限已经被提升，TOKEN_ELEVATION.TokenIsElevated是一个非0值
            ' 反之则为0
            fIsElevated = (elevation.TokenIsElevated <> 0)

        Finally
            ' 集中清理所有已分配的内存资源
            If (Not hToken Is Nothing) Then
                hToken.Close()
                hToken = Nothing
            End If
            If (pTokenElevation <> IntPtr.Zero) Then
                Marshal.FreeHGlobal(pTokenElevation)
                pTokenElevation = IntPtr.Zero
                cbTokenElevation = 0
            End If
        End Try

        Return fIsElevated
    End Function


    ''' <summary>
    ''' 此函数获取当前线程的完整性级别。完整性级别只有在Windows Vista及后
    ''' 续版本的Windows中有效。所以在Windows Vista之前的版本中执行GetProcessIntegrityLevel， 
    ''' 它会抛出一个C++异常。
    ''' </summary>
    ''' <returns>
    '''
    ''' 返回当前进程的完整性级别。它可能是以下某一个值。
    '''
    '''     SECURITY_MANDATORY_UNTRUSTED_RID (SID: S-1-16-0x0)
    '''     表示不被信任的级别。它被用于一个匿名组成员起动的进程。这时大多数
    '''     写入操作被禁止。
    '''
    '''     SECURITY_MANDATORY_LOW_RID (SID: S-1-16-0x1000)
    '''     表示低完整性级别。它被用于保护模式下的IE浏览器。这时大多数系统中对
    '''     象（包括文件及注册表键值）的写入操作被禁止。
    '''
    '''     SECURITY_MANDATORY_MEDIUM_RID (SID: S-1-16-0x2000)
    '''     表示中完整性级别。它被用于在UAC开启时启动普通应用程序。
    '''
    '''
    '''     SECURITY_MANDATORY_HIGH_RID (SID: S-1-16-0x3000)
    '''     表示高完整性级别。它被用于用户通过UAC提升权限启动一个管理员应用程序。
    '''     或则当UAC关闭时，管理员用户启动一个普通程序。
    '''
    '''
    '''     SECURITY_MANDATORY_SYSTEM_RID (SID: S-1-16-0x4000)
    '''     表示系统完整性级别。它被用于服务或则其他系统级别的应用程序（比如
    '''     Wininit, Winlogon, Smss，等等）
    ''' 
    ''' </returns>
    ''' <exception cref="System.ComponentModel.Win32Exception">
    ''' 如果任何原生的Windows API函数出错，此函数会抛出一个包含最后错误代码的Win32Exception。
    ''' </exception>
    Friend Function GetProcessIntegrityLevel() As Integer
        Dim IL As Integer = -1
        Dim hToken As SafeTokenHandle = Nothing
        Dim cbTokenIL As Integer = 0
        Dim pTokenIL As IntPtr = IntPtr.Zero

        Try
            ' 使用TOKEN_QUERY打开线程的主访问令牌。
            If (Not NativeMethod.OpenProcessToken(Process.GetCurrentProcess.Handle, _
                NativeMethod.TOKEN_QUERY, hToken)) Then
                Throw New Win32Exception
            End If

            ' 然后我们必须查询令牌完整性级别信息的大小。注意：我们预期得到一个FALSE结果及错误
            ' ERROR_INSUFFICIENT_BUFFER， 这是由于我们在GetTokenInformation输入一个
            ' 空缓冲。同时，在cbTokenIL中我们会得知完整性级别信息的大小。
            If (Not NativeMethod.GetTokenInformation(hToken, _
                TOKEN_INFORMATION_CLASS.TokenIntegrityLevel, _
                IntPtr.Zero, 0, cbTokenIL)) Then
                Dim err As Integer = Marshal.GetLastWin32Error
                If (err <> NativeMethod.ERROR_INSUFFICIENT_BUFFER) Then
                    ' 当进程运行于Windows Vista之前的系统中，GetTokenInformation返回
                    ' FALSE和错误码ERROR_INVALID_PARAMETER。这是由于这些操作系统不支
                    ' 持TokenElevation。
                    Throw New Win32Exception(err)
                End If
            End If

            ' 现在我们为完整性级别信息分配一个缓存。
            pTokenIL = Marshal.AllocHGlobal(cbTokenIL)
            If (pTokenIL = IntPtr.Zero) Then
                Throw New Win32Exception
            End If

            ' 现在我们需要再次查询完整性级别信息。如果在第一次调用GetTokenInformation
            ' 和本次之间一个管理员把当前账户加到另外一个组中，此次调用会失败。
            If (Not NativeMethod.GetTokenInformation(hToken, _
                TOKEN_INFORMATION_CLASS.TokenIntegrityLevel, _
                pTokenIL, cbTokenIL, cbTokenIL)) Then
                Throw New Win32Exception
            End If

            ' 转换TOKEN_MANDATORY_LABEL结构（从原生到.Net）
            Dim tokenIL As TOKEN_MANDATORY_LABEL = Marshal.PtrToStructure( _
            pTokenIL, GetType(TOKEN_MANDATORY_LABEL))

            ' 完整性级别SID为S-1-16-0xXXXX形式。（例如：S-1-16-0x1000表示为低完整性
            ' 级别的SID）。而且有且仅有一个次级授权信息。
            Dim pIL As IntPtr = NativeMethod.GetSidSubAuthority(tokenIL.Label.Sid, 0)
            IL = Marshal.ReadInt32(pIL)

        Finally
            ' 集中清理所有已分配的内存资源
            If (Not hToken Is Nothing) Then
                hToken.Close()
                hToken = Nothing
            End If
            If (pTokenIL <> IntPtr.Zero) Then
                Marshal.FreeHGlobal(pTokenIL)
                pTokenIL = IntPtr.Zero
                cbTokenIL = 0
            End If
        End Try

        Return IL
    End Function

#End Region


    Private Sub MainForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles MyBase.Load

        '' 获取并显示即使在还没有为当前用户提升权限的前提下，拥有此进程的主访
        '' 问令牌的用户是否是本地管理员组的成员。(IsUserInAdminGroup)。
        Try
            Dim fInAdminGroup As Boolean = Me.IsUserInAdminGroup
            Me.lbInAdminGroup.Text = IIf(fInAdminGroup, "是", "否")
        Catch ex As Exception
            Me.lbInAdminGroup.Text = "N/A"
            MessageBox.Show(ex.Message, "在IsUserInAdminGroup中发生一个错误", _
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

        ' 获取并显示是否此进程以管理员身份运行。（IsRunAsAdmin）。

        Try
            Dim fIsRunAsAdmin As Boolean = Me.IsRunAsAdmin
            Me.lbIsRunAsAdmin.Text = IIf(fIsRunAsAdmin, "是", "否")
        Catch ex As Exception
            Me.lbIsRunAsAdmin.Text = "N/A"
            MessageBox.Show(ex.Message, "在IsRunAsAdmin中发生一个错误", _
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
        
        ' 获取并显示进程权限提升信息(IsProcessElevated)和完整性级别（GetProcessIntegrityLevel）
        ' 注意：这些信息在Windows Vista之前的Windows中不存在。    

        If (Environment.OSVersion.Version.Major >= 6) Then
            ' 运行于Windows Vista或后续版本（主版本号 >= 6）。

            Try
                ' 获取并显示进程权限提升信息
                Dim fIsElevated As Boolean = Me.IsProcessElevated
                Me.lbIsElevated.Text = IIf(fIsElevated, "是", "否")

                ' 如果进程尚未被提升，更新“自我提升权限”按钮以在UI中显示UAC盾形
                ' 图标。
                Me.btnElevate.FlatStyle = FlatStyle.System
                NativeMethod.SendMessage(Me.btnElevate.Handle, NativeMethod.BCM_SETSHIELD, _
                                         0, IIf(fIsElevated, IntPtr.Zero, New IntPtr(1)))
            Catch ex As Exception
                Me.lbIsElevated.Text = "N/A"
                MessageBox.Show(ex.Message, "An error occurred in IsProcessElevated", _
                                MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try

            Try
                ' 获取并显示进程的完整性级别
                Dim IL As Integer = Me.GetProcessIntegrityLevel
                Select Case IL
                    Case NativeMethod.SECURITY_MANDATORY_UNTRUSTED_RID
                        Me.lbIntegrityLevel.Text = "不信任"
                    Case NativeMethod.SECURITY_MANDATORY_LOW_RID
                        Me.lbIntegrityLevel.Text = "低"
                    Case NativeMethod.SECURITY_MANDATORY_MEDIUM_RID
                        Me.lbIntegrityLevel.Text = "中"
                    Case NativeMethod.SECURITY_MANDATORY_HIGH_RID
                        Me.lbIntegrityLevel.Text = "高"
                    Case NativeMethod.SECURITY_MANDATORY_SYSTEM_RID
                        Me.lbIntegrityLevel.Text = "系统"
                    Case Else
                        Me.lbIntegrityLevel.Text = "未知"
                End Select
            Catch ex As Exception
                Me.lbIntegrityLevel.Text = "N/A"
                MessageBox.Show(ex.Message, "An error occurred in GetProcessIntegrityLevel!", _
                                MessageBoxButtons.OK, MessageBoxIcon.Hand)
            End Try

        Else
            Me.lbIsElevated.Text = "N/A"
            Me.lbIntegrityLevel.Text = "N/A"
        End If

    End Sub


    Private Sub btnElevate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles btnElevate.Click
        ' 检查当前进程是否以管理员权限运行，如果不是则提升。
        If (Not Me.IsRunAsAdmin) Then
            ' 以管理员身份重启本程序
            Dim proc As New ProcessStartInfo
            proc.UseShellExecute = True
            proc.WorkingDirectory = Environment.CurrentDirectory
            proc.FileName = Application.ExecutablePath
            proc.Verb = "runas"

            Try
                Process.Start(proc)
            Catch
                ' 用户拒绝提升
                ' 什么都不做并直接返回......
                Return
            End Try

            Application.Exit()  '退出
        Else
            MessageBox.Show("此进程已以管理员身份运行", "UAC")
        End If
    End Sub

End Class
