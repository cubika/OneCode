'*********************************** 模块头 ******************************\
' 模块名:    UploadFile.vb
' 项目名:    VBASPNETFileUploadStatus
' 版权 (c) Microsoft Corporation
'
' 本项目阐述了在不使用第三方组件时实现显示上传的状态和进程
' 像ActiveX 控件,Flash 或者Silverlight.
' 
' 这个类用来在请求实体中上传文件. 
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'****************************************************************************/


Imports System
Imports System.IO
Imports System.Web


Public Class UploadFile
    Private _cachePath As String = Nothing
    Private _cacheLength As Integer = 1024 * 1024 * 5

    Public Property FileName() As String

    Public Property ContentType() As String

    Public Property ClientPath() As String

    Private Property Data() As Byte()

    Private ReadOnly _defaultUploadFolder As String = "UploadedFiles"

    Public Sub New(ByVal clientPath__1 As String,
                   ByVal contentType__2 As String)
        Data = New Byte(-1) {}
        ClientPath = clientPath__1
        ContentType = contentType__2
        _cachePath = HttpContext.Current.Server.MapPath(
            "uploadcaching") + "\" + Guid.NewGuid().ToString()
        FileName = New FileInfo(clientPath__1).Name
        Dim cache_file As New FileInfo(_cachePath)
        If Not cache_file.Directory.Exists Then
            cache_file.Directory.Create()
        End If
    End Sub

    ' 对于大文件我们需要读取并存储部分数据.
    ' 并且这种方法可以被用来把部分数据块合并起来.
    Friend Sub AppendData(ByVal data As Byte())
        Me.Data = BinaryHelper.Combine(Me.Data, data)
        If Me.Data.Length > _cacheLength Then
            CacheData()
        End If
    End Sub

    ' 为了释放内存，我们可以存储
    ' 那些已读进磁盘的数据.
    Private Sub CacheData()
        If Me.Data IsNot Nothing AndAlso Me.Data.Length > 0 Then

            Using fs As New FileStream(_cachePath,
                                       FileMode.Append,
                                       FileAccess.Write)
                fs.Write(Data, 0, Data.Length)
                Me.Data = New Byte(-1) {}
            End Using
        End If
    End Sub

    ' 清除模板文件.
    Friend Sub ClearCache()
        If File.Exists(_cachePath) Then
            File.Delete(_cachePath)
        End If
    End Sub


    ' 用正确的存储路径来存储上传文件.
    Public Sub Save(ByVal path As String)
        If Me.Data.Length > 0 Then
            CacheData()
        End If

        If [String].IsNullOrEmpty(path) Then
            path = HttpContext.Current.Server.MapPath(
                _defaultUploadFolder + "\" + FileName)
        End If
        If File.Exists(path) Then
            File.Delete(path)
        End If

        Dim dir = New FileInfo(path).Directory
        If Not dir.Exists Then
            dir.Create()
        End If

        ' 把缓存文件移到正确的路径.
        File.Move(_cachePath, path)
    End Sub

End Class


