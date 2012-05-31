'/**************************************** 模块头 *****************************************\
'* 模块名:  RemoteUpload.vb
'* 项目名:  VBASPNETRemoteUploadAndDownload
'* 版权 (c) Microsoft Corporation.
'* 
'* WebClient 和 FtpWebRequest类都提供常用方法来发送数据到服务器URI.
'* 同时接受来自由URI定义的资源的数据.
'*
'* 当上传和下载文件时, 这些类会提交webrequest到用户输入的url.
'*
'* UploadData()方法通过HTTP或FTP发送一个数据缓冲(未编码)到以方法参数指定的资源,
'* 然后返回服务器的web响应. 相应的, DownloadData()方法请求一个HTTP
'* 或FTP下载方法到远程服务器来获得服务器的输出流.
'*
'* This source is subject to the Microsoft Public License.
'* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
'* All other rights reserved.
'*
'* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
'* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
'* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'\*****************************************************************************************/

Imports System.Net
Imports System.IO

Public MustInherit Class RemoteUpload

    Public Property FileName() As String
        Get
            Return m_FileName
        End Get
        Set(ByVal value As String)
            m_FileName = value
        End Set
    End Property
    Private m_FileName As String


    Public Property UrlString() As String
        Get
            Return m_UrlString
        End Get
        Set(ByVal value As String)
            m_UrlString = value
        End Set
    End Property
    Private m_UrlString As String


    Public Property NewFileName() As String
        Get
            Return m_NewFileName
        End Get
        Set(ByVal value As String)
            m_NewFileName = value
        End Set
    End Property
    Private m_NewFileName As String


    Public Property FileData() As Byte()
        Get
            Return m_FileData
        End Get
        Set(ByVal value As Byte())
            m_FileData = value
        End Set
    End Property
    Private m_FileData As Byte()

    Public Sub New(ByVal fileData As Byte(), ByVal fileName As String, ByVal urlString As String)
        Me.FileData = fileData
        Me.FileName = fileName
        Me.UrlString = If(urlString.EndsWith("/"), urlString, urlString & "/")
        Dim newFileName As String = DateTime.Now.ToString("yyMMddhhmmss") + DateTime.Now.Millisecond.ToString() + Path.GetExtension(Me.FileName)
        Me.UrlString = Me.UrlString & newFileName
    End Sub

    ''' <summary>
    ''' 上传文件到远程服务器
    ''' </summary>
    ''' <returns></returns>
    Public Overridable Function UploadFile() As Boolean
        Return True
    End Function

End Class

''' <summary>
''' HttpUpload 类
''' </summary>
Public Class HttpRemoteUpload
    Inherits RemoteUpload
    Public Sub New(ByVal fileData As Byte(), ByVal fileNamePath As String, ByVal urlString As String)

        MyBase.New(fileData, fileNamePath, urlString)
    End Sub

    Public Overrides Function UploadFile() As Boolean
        Dim postData As Byte()
        Try
            postData = Me.FileData
            Using client As New WebClient()
                client.Credentials = CredentialCache.DefaultCredentials
                client.Headers.Add("Content-Type", "application/x-www-form-urlencoded")
                client.UploadData(Me.UrlString, "PUT", postData)
            End Using

            Return True
        Catch ex As Exception
            Throw New Exception("上传失败", ex.InnerException)
        End Try

    End Function
End Class

''' <summary>
''' FtpUpload 类
''' </summary>
Public Class FtpRemoteUpload
    Inherits RemoteUpload
    Public Sub New(ByVal fileData As Byte(), ByVal fileNamePath As String, ByVal urlString As String)

        MyBase.New(fileData, fileNamePath, urlString)
    End Sub

    Public Overrides Function UploadFile() As Boolean
        Dim reqFTP As FtpWebRequest
        reqFTP = DirectCast(FtpWebRequest.Create(Me.UrlString), FtpWebRequest)
        reqFTP.KeepAlive = True
        reqFTP.Method = WebRequestMethods.Ftp.UploadFile
        reqFTP.UseBinary = True
        reqFTP.ContentLength = Me.FileData.Length

        Dim buffLength As Integer = 2048
        Dim buff As Byte() = New Byte(buffLength - 1) {}
        Dim ms As New MemoryStream(Me.FileData)

        Try
            Dim contenctLength As Integer
            Using strm As Stream = reqFTP.GetRequestStream()
                contenctLength = ms.Read(buff, 0, buffLength)

                While contenctLength > 0
                    strm.Write(buff, 0, contenctLength)
                    contenctLength = ms.Read(buff, 0, buffLength)
                End While

            End Using

            Return True
        Catch ex As Exception
            Throw New Exception("上传失败", ex.InnerException)
        End Try
    End Function

End Class
