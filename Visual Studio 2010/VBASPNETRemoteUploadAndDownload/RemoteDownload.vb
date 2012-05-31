'/**************************************** 模块头 *****************************************\
'* 模块名:  RemoteDownload.vb
'* 项目名:  VBASPNETRemoteUploadAndDownload
'* 版权 (c) Microsoft Corporation.
'* 
'* WebClient 和 FtpWebRequest 泪都提供常用方法来发送数据到服务器URI.
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

Public MustInherit Class RemoteDownload

    Public Property UrlString() As String
        Get
            Return m_UrlString
        End Get
        Set(ByVal value As String)
            m_UrlString = Value
        End Set
    End Property
    Private m_UrlString As String


    Public Property DestDir() As String
        Get
            Return m_DestDir
        End Get
        Set(ByVal value As String)
            m_DestDir = Value
        End Set
    End Property
    Private m_DestDir As String

    Public Sub New(ByVal urlString As String, ByVal destDir As String)
        Me.UrlString = urlString
        Me.DestDir = destDir
    End Sub

    '''<summary>
    '''从远程服务器下载文件
    '''</summary>
    Public Overridable Function DownloadFile() As Boolean
        Return True
    End Function
End Class

''' <summary>
''' HttpRemoteDownload 类
''' </summary>
Public Class HttpRemoteDownload
    Inherits RemoteDownload
    Public Sub New(ByVal urlString As String, ByVal descFilePath As String)

        MyBase.New(urlString, descFilePath)
    End Sub

    Public Overrides Function DownloadFile() As Boolean
        Dim fileName As String = System.IO.Path.GetFileName(Me.UrlString)
        Dim descFilePath As String = System.IO.Path.Combine(Me.DestDir, fileName)
        Try
            Dim myre As WebRequest = WebRequest.Create(Me.UrlString)
        Catch ex As Exception
            Throw New Exception("服务器上不存在对应文件", ex.InnerException)
        End Try
        Try
            Dim fileData As Byte()
            Using client As New WebClient()
                fileData = client.DownloadData(Me.UrlString)
            End Using
            Using fs As New FileStream(descFilePath, FileMode.OpenOrCreate)
                fs.Write(fileData, 0, fileData.Length)
            End Using
            Return True
        Catch ex As Exception
            Throw New Exception("下载失败", ex.InnerException)
        End Try
    End Function
End Class

''' <summary>
''' FtpDownload 类
''' </summary>
Public Class FtpRemoteDownload
    Inherits RemoteDownload
    Public Sub New(ByVal urlString As String, ByVal descFilePath As String)

        MyBase.New(urlString, descFilePath)
    End Sub

    Public Overrides Function DownloadFile() As Boolean
        Dim reqFTP As FtpWebRequest

        Dim fileName As String = System.IO.Path.GetFileName(Me.UrlString)
        Dim descFilePath As String = System.IO.Path.Combine(Me.DestDir, fileName)

        Try

            reqFTP = DirectCast(FtpWebRequest.Create(Me.UrlString), FtpWebRequest)
            reqFTP.Method = WebRequestMethods.Ftp.DownloadFile
            reqFTP.UseBinary = True

            Using outputStream As New FileStream(descFilePath, FileMode.OpenOrCreate)
                Using response As FtpWebResponse = DirectCast(reqFTP.GetResponse(), FtpWebResponse)
                    Using ftpStream As Stream = response.GetResponseStream()
                        Dim bufferSize As Integer = 2048
                        Dim readCount As Integer
                        Dim buffer As Byte() = New Byte(bufferSize - 1) {}
                        readCount = ftpStream.Read(buffer, 0, bufferSize)
                        While readCount > 0
                            outputStream.Write(buffer, 0, readCount)
                            readCount = ftpStream.Read(buffer, 0, bufferSize)
                        End While
                    End Using

                End Using
            End Using
            Return True

        Catch ex As Exception
            Throw New Exception("下载失败", ex.InnerException)
        End Try
    End Function
End Class
