'**************************************** 模块头 *****************************************'
'模块名:      BatchRunner.vb
'项目名:      VBASPNETCMD
'版权 (c) Microsoft Corporation.
'
'Batch Runner类反映了如何处理批处理文件.
'
'This source is subject to the Microsoft Public License.
'See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
'All other rights reserved.
'
'THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
'EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
'WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'\*****************************************************************************************'

#Region "Using directives"
Imports System.Security
#End Region

Public Class BatchRunner
#Region "fields"
    Private m_domainName As String
    Private m_userName As String
    Private m_password As String
#End Region

#Region "properties"
    ''' <summary>
    ''' 设置和获取验证用户的DomainName
    ''' </summary>
    Public Property DomainName() As String
        Get
            Return m_domainName
        End Get
        Set(ByVal value As String)
            m_domainName = Value
        End Set
    End Property
    ''' <summary>
    ''' 设置和获取验证用户的UserName
    ''' </summary>
    Public Property UserName() As String
        Get
            Return m_userName
        End Get
        Set(ByVal value As String)
            m_userName = Value
        End Set
    End Property
    ''' <summary>
    ''' 设置和获取验证用户的Password
    ''' </summary>
    Public Property Password() As String
        Get
            Return m_password
        End Get
        Set(ByVal value As String)
            m_password = Value
        End Set
    End Property

#End Region

#Region "constructor"
    ''' <summary>
    ''' 默认构造器
    ''' </summary>
    Public Sub New()
    End Sub

    ''' <summary>
    ''' 验证信息的构造器
    ''' </summary>
    ''' <param name="domain">验证用户的DomainName</param>
    ''' <param name="username">验证用户的username</param>
    ''' <param name="password">验证用户的password</param>
    Public Sub New(ByVal domain As String, ByVal username As String, ByVal password As String)
        Me.m_domainName = domain
        Me.m_userName = username
        Me.m_password = password
    End Sub

#End Region

    ''' <summary>
    ''' 执行批处理文件
    ''' </summary>
    ''' <param name="fileName">执行的批处理文件的全名</param>
    ''' <returns>批处理文件的执行结果</returns>
    Public Function ExecuteBatch(ByVal fileName As String) As String
        If Not fileName.ToLower().EndsWith(".bat") Then
            Throw New ArgumentException("the file's suffix must be .bat")
        End If
        Dim retOutPut As String = "No Output!"

        Try
            Dim psi As System.Diagnostics.ProcessStartInfo = GenerateProcessInfo(fileName)

            Dim processBatch As System.Diagnostics.Process = System.Diagnostics.Process.Start(psi)
            Dim myOutput As System.IO.StreamReader = processBatch.StandardOutput

            processBatch.WaitForExit()
            If processBatch.HasExited Then
                retOutPut = myOutput.ReadToEnd()
            End If
        Catch winException As System.ComponentModel.Win32Exception
            If winException.Message.Contains("bad password") Then
                retOutPut = winException.Message
            Else
                retOutPut = "Win32Exception occured"
                ' 记录异常信息
            End If
        Catch exception As System.Exception
            ' 记录异常信息
            retOutPut = exception.Message
        End Try
        Return retOutPut
    End Function

    ''' <summary>
    ''' 通过(domainName,userName,password)字段生成ProcessInfo
    ''' </summary>
    ''' <param name="fileName">执行的批处理文件的全名</param>
    ''' <returns></returns>
    Private Function GenerateProcessInfo(ByVal fileName As String) As System.Diagnostics.ProcessStartInfo
        Dim psi As New System.Diagnostics.ProcessStartInfo(fileName)
        psi.RedirectStandardOutput = True
        psi.CreateNoWindow = True
        psi.UseShellExecute = False
        If Not String.IsNullOrEmpty(m_domainName) Then
            psi.Domain = m_domainName
        End If
        If Not String.IsNullOrEmpty(m_userName) Then
            psi.UserName = m_userName
        End If
        If Not String.IsNullOrEmpty(m_password) Then
            Dim sstring As New SecureString()
            For Each c As Char In m_password
                sstring.AppendChar(c)
            Next
            psi.Password = sstring
        End If
        Return psi
    End Function

End Class