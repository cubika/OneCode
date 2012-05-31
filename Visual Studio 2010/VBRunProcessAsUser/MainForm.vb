'******************************** 模块头 *********************************************'
' 模块名:  MainForm.vb
' 项目名:  VBRunProcessAsUser
' 版权 (c) Microsoft Corporation.
' 
' MainForm.vb文件是VBRunProcessAsUser主窗体的后台代码.
'
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
'
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER 
' EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF 
' MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**************************************************************************************'

Imports System.Security
Imports System.Runtime.InteropServices


Partial Public Class RunProcessAsUser
    Inherits Form


    Public Sub New()
        InitializeComponent()
    End Sub

    ''' <summary>
    ''' 用OpenFileDialog对象输入执行命令行.
    ''' </summary>
    Private Sub btnCommand_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCommand.Click
        Using ofdOpen As New OpenFileDialog()
            If ofdOpen.ShowDialog(Me) = DialogResult.OK Then
                tbCommand.Text = ofdOpen.FileName
            End If
        End Using
    End Sub

    ''' <summary>
    ''' 用System.Diagnostics命名空间的Process.Start来启动进程.
    ''' </summary>
    Private Sub btnRunCommand_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnRunCommand.Click
        Try
            If (Not String.IsNullOrEmpty(tbUserName.Text)) AndAlso _
                (Not String.IsNullOrEmpty(tbPassword.Text)) AndAlso _
                (Not String.IsNullOrEmpty(tbCommand.Text)) Then

                Dim password As SecureString = StringToSecureString(Me.tbPassword.Text.ToString())
                Dim proc As Process = Process.Start(tbCommand.Text.ToString(), _
                                                    tbUserName.Text.ToString(), _
                                                    password, _
                                                    tbDomain.Text.ToString())

                ProcessStarted(proc.Id)

                proc.EnableRaisingEvents = True
                AddHandler proc.Exited, AddressOf ProcessExited
            Else
                MessageBox.Show("请输入用户名、密码和命令")
                Return
            End If
        Catch w32e As System.ComponentModel.Win32Exception
            ProcessStartFailed(w32e.Message)
        End Try
    End Sub

    ''' <summary>
    ''' 为当前的用户名加载 UserName 变量，为当前的用户域名加载UserDomainName. 
    ''' </summary>
    Private Sub RunProccessAsMutiUser_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        tbPassword.PasswordChar = ChrW(&H25CF)
    End Sub

    ''' <summary>
    ''' 当目标进程启动时激发.
    ''' </summary>
    Private Sub ProcessStarted(ByVal processId As Integer)
        MessageBox.Show("进程 " & processId.ToString() & " 启动")
    End Sub

    ''' <summary>
    ''' 当目标进程退出时激发.
    ''' </summary>
    Private Sub ProcessExited(ByVal sender As Object, ByVal e As EventArgs)
        Dim proc As Process = TryCast(sender, Process)
        If proc IsNot Nothing Then
            MessageBox.Show("进程 " & proc.Id.ToString() & " 退出")
        End If
    End Sub

    ''' <summary>
    ''' 当目标进程启动失败时激发.
    ''' </summary>
    Private Sub ProcessStartFailed(ByVal [error] As String)
        MessageBox.Show([error], "进程启动失败", MessageBoxButtons.OK, MessageBoxIcon.Error)
    End Sub

    ''' <summary>
    ''' 演示如何弹出凭据提示以便输入用户凭据.
    ''' </summary>
    Private Sub btnCredentialUIPrompt_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCredentialUIPrompt.Click
        Try
            Using dialog As New Kerr.PromptForCredential()
                dialog.Title = "请指定用户"
                dialog.DoNotPersist = True
                dialog.ShowSaveCheckBox = False
                dialog.TargetName = Environment.MachineName
                dialog.ExpectConfirmation = True

                If DialogResult.OK = dialog.ShowDialog(Me) Then
                    tbPassword.Text = SecureStringToString(dialog.Password)
                    Dim strSplit() As String = dialog.UserName.Split("\"c)
                    If (strSplit.Length = 2) Then
                        Me.tbUserName.Text = strSplit(1)
                        Me.tbDomain.Text = strSplit(0)
                    Else
                        Me.tbUserName.Text = dialog.UserName
                    End If
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show(ex.ToString())
        End Try
    End Sub

    ''' <summary>
    ''' Helper函数.它将SecureString转换为String.
    ''' </summary>
    Shared Function SecureStringToString(ByVal secureStr As SecureString) As String
        Dim bstr As IntPtr = Marshal.SecureStringToBSTR(secureStr)
        Try
            Return Marshal.PtrToStringBSTR(bstr)
        Finally
            Marshal.FreeBSTR(bstr)
        End Try
    End Function

    ''' <summary>
    ''' Helper函数.它将String转换为SecureString.
    ''' </summary>
    Shared Function StringToSecureString(ByVal str As String) As SecureString
        Dim secureStr As New SecureString()
        Dim chars() As Char = str.ToCharArray()
        For i As Integer = 0 To chars.Length - 1
            secureStr.AppendChar(chars(i))
        Next i
        Return secureStr
    End Function
End Class

