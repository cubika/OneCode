'************************************ 模块头 **************************************\
' 模块名:      MainForm.vb
' 项目名:      VBCreateLowIntegrityProcess
' 版权 (c) Microsoft Corporation.
' 
' 这个代码示例演示了如何启动一个低完整性进程。当你点击本程序中“以低完整等级执
' 行本程序”按钮，此应用程序使用低完整性再次启动一个本程序实例。低完整性进程只
' 能在低完整性区域内写入数据，比如%USERPROFILE%\AppData\LocalLow文件夹或者注册
' 表中的HKEY_CURRENT_USER\Software\AppDataLow键值。即使当前用户的SID在自由访问
' 控制列表（discretionary access control list）中拥有写入权限，如果你想要访问一
' 个完整性高的对象，你也将会收到一个无法访问的错误。
' 
' 默认情况下，子进程继承其父进程的完整性等级。要启动一个低完整性进程，你必须使用
' CreateProcessAsUser和低完整性访问令牌启动一个新的子进程。详细信息请参考示例
' CreateLowIntegrityProcess中的相关函数。
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

Imports System.ComponentModel
Imports System.Runtime.InteropServices
Imports System.IO

#End Region


Public Class MainForm

    Private Sub MainForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles MyBase.Load
        ' 此示例必须在Windows Vista及后续支持用户账户控制的操作系统中运行。 
        If (Environment.OSVersion.Version.Major < 6) Then
            MessageBox.Show( _
                "此示例程序必须在Windows Vista及后续" & _
                "支持用户账户控制（UAC）的操作系统中运行。" & _
                "程序即将退出。")
            Close()
        End If

        Try
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
            MessageBox.Show(ex.Message, "GetProcessIntegrityLevel 错误", _
                MessageBoxButtons.OK, MessageBoxIcon.Hand)
        End Try
    End Sub


    Private Sub btnCreateLowProcess_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles btnCreateLowProcess.Click
        Try
            ' 尝试以低完整性级别为当前程序启动一个新的实例。
            CreateLowIntegrityProcess(Application.ExecutablePath)
        Catch ex As Exception
            MessageBox.Show(ex.Message, "CreateLowIntegrityProcess Error", _
                    MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub


    Private Sub btnWriteLocalAppData_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles btnWriteLocalAppData.Click
        Try
            ' 测试文件路径 %USERPROFILE%\AppData\Local\testfile.txt
            Dim filePath As String = KnownFolder.LocalAppData & "\testfile.txt"

            ' 尝试创建并写入测试文件
            Using sw As StreamWriter = New StreamWriter(filePath)
                sw.Write("VBCreateLowIntegrityProcess Test File")
            End Using

            MessageBox.Show("成功写入测试文件： " & filePath, _
                "测试结果", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MessageBox.Show(ex.Message, "写入LocalAppData失败", _
                MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub


    Private Sub btnWriteLocalAppDataLow_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles btnWriteLocalAppDataLow.Click
        Try
            ' 测试文件路径： %USERPROFILE%\AppData\LocalLow\testfile.txt
            Dim filePath As String = KnownFolder.LocalAppDataLow & "\testfile.txt"

            ' 尝试创建并写入测试文件
            Using sw As StreamWriter = New StreamWriter(filePath)
                sw.Write("VBCreateLowIntegrityProcess Test File")
            End Using

            MessageBox.Show("成功写入测试文件： " & filePath, _
                "测试结果", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MessageBox.Show(ex.Message, "写入LocalAppDataLow失败", _
                MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub


#Region "Helper Functions related to Process Integrity Level"

    ''' <summary>
    ''' 此函数以低完整性级别启动一个应用程序
    ''' </summary>
    ''' <param name="commandLine">
    ''' 需要被执行的命令行。此字符串最长为32K个字符。
    ''' </param>
    ''' <remarks>
    ''' 启动一个低完整性进程
    ''' 1) 复制当前进程的句柄，它拥有中完整性级别
    ''' 2）使用SetTokenInformation设置访问进程的完整性级别为低。
    ''' 3）使用CreateProcessAsUser及低完整性级别的访问令牌创建一个新的进程。
    ''' </remarks>
    Friend Sub CreateLowIntegrityProcess(ByVal commandLine As String)
        Dim hToken As SafeTokenHandle = Nothing
        Dim hNewToken As SafeTokenHandle = Nothing
        Dim strIntegritySid As String = "S-1-16-4096"
        Dim pIntegritySid As IntPtr = IntPtr.Zero
        Dim cbTokenInfo As Integer = 0
        Dim pTokenInfo As IntPtr = IntPtr.Zero
        Dim si As New STARTUPINFO
        Dim pi As New PROCESS_INFORMATION

        Try
            ' 打开进程的主访问令牌。
            If (Not NativeMethod.OpenProcessToken(Process.GetCurrentProcess.Handle, _
                NativeMethod.TOKEN_DUPLICATE Or NativeMethod.TOKEN_ADJUST_DEFAULT Or _
                NativeMethod.TOKEN_QUERY Or NativeMethod.TOKEN_ASSIGN_PRIMARY, _
                hToken)) Then
                Throw New Win32Exception
            End If

            ' 复制当前进程的主令牌。
            If (Not NativeMethod.DuplicateTokenEx(hToken, 0, IntPtr.Zero, _
                SECURITY_IMPERSONATION_LEVEL.SecurityImpersonation, _
                TOKEN_TYPE.TokenPrimary, hNewToken)) Then
                Throw New Win32Exception
            End If

            ' 创建低完整性SID。
            If Not NativeMethod.ConvertStringSidToSid(strIntegritySid, pIntegritySid) Then
                Throw New Win32Exception
            End If

            Dim tml As TOKEN_MANDATORY_LABEL
            tml.Label.Attributes = NativeMethod.SE_GROUP_INTEGRITY
            tml.Label.Sid = pIntegritySid

            ' 转换TOKEN_MANDATORY_LABEL结构至native内存。
            cbTokenInfo = Marshal.SizeOf(tml)
            pTokenInfo = Marshal.AllocHGlobal(cbTokenInfo)
            Marshal.StructureToPtr(tml, pTokenInfo, False)

            ' 设置访问令牌的完整性级别为低。
            If (Not NativeMethod.SetTokenInformation(hNewToken, _
                TOKEN_INFORMATION_CLASS.TokenIntegrityLevel, pTokenInfo, _
                (cbTokenInfo + NativeMethod.GetLengthSid(pIntegritySid)))) Then
                Throw New Win32Exception
            End If

            ' 以低完整性级别创建一个新进程。
            si.cb = Marshal.SizeOf(si)
            If (Not NativeMethod.CreateProcessAsUser(hNewToken, Nothing, commandLine, _
                IntPtr.Zero, IntPtr.Zero, False, 0, IntPtr.Zero, Nothing, (si), pi)) Then
                Throw New Win32Exception
            End If

        Finally
            ' 集中清理已分配的资源。 
            If (Not hToken Is Nothing) Then
                hToken.Close()
                hToken = Nothing
            End If
            If (Not hNewToken Is Nothing) Then
                hNewToken.Close()
                hNewToken = Nothing
            End If
            If (pIntegritySid <> IntPtr.Zero) Then
                Marshal.FreeHGlobal(pIntegritySid)
                pIntegritySid = IntPtr.Zero
            End If
            If (pTokenInfo <> IntPtr.Zero) Then
                Marshal.FreeHGlobal(pTokenInfo)
                pTokenInfo = IntPtr.Zero
                cbTokenInfo = 0
            End If
            If (pi.hProcess <> IntPtr.Zero) Then
                NativeMethod.CloseHandle(pi.hProcess)
                pi.hProcess = IntPtr.Zero
            End If
            If (pi.hThread <> IntPtr.Zero) Then
                NativeMethod.CloseHandle(pi.hThread)
                pi.hThread = IntPtr.Zero
            End If
        End Try
    End Sub


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
            ' 集中清理所有已分配的内存资源。
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

End Class
