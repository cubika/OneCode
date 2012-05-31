'************************************* 模块头 ******************************\
' 模块名:    UploadProcessModule.vb
' 项目名:    VBASPNETFileUploadStatus
' 版权 (c) Microsoft Corporation
'
' 本项目阐述了在不使用第三方组件时实现显示上传的状态和进程
' 像ActiveX 控件,Flash 或者Silverlight.
' 
' 在这个例子中我们可以看到如下的特点：
' 1. 如何用HttpWorkerRequest获取用户请求实体的正文.
' 2. 如何控制服务器端读取请求数据.
' 3. 如何检索并存储上传状态.
' 
' 在这个模块的基础上,我们可以扩展它来实现如下列出的特点:
' 1. 控制多个文件上传的状态.
' 2. 控制大的文件上传到服务器并且不 
'    用服务器的缓存来存储这些文件.
' (注意: 在这个例子中我没有实现上述的特点.
'        我将会在不久的将来把它们添加进去.)
' 
' 在默认情况下IIS限制了请求内容的长度（大约28MB）,
' 如果我们想使这个模块对于大的文件也能工作,
' 请参考根目录下的readme文件.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'****************************************************************************/


Imports System.Web
Imports System.Text
Imports System.Web.Caching
Imports System.Collections
Imports System.Collections.Specialized
Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.IO
Imports System.Text.RegularExpressions


Public Class UploadProcessModule
    Implements IHttpModule
    ''' <summary>
    ''' 这是一个建立在.Net Framework 4.0基础上 
    ''' 的ASP.NET Http模块显示了无组件的文件 
    ''' 上传的状态 .
    ''' 详细信息，请参考根目录下的Readme文件.
    ''' </summary>
    Dim _cacheContainer As String = "fuFile"
    Dim _uploadedFilesFolder As String = "UploadedFiles"
    Dim _folderPath As String = ""

    Public Sub Dispose() Implements IHttpModule.Dispose

    End Sub

    Public Sub Init(ByVal context As HttpApplication) Implements IHttpModule.Init
        AddHandler context.BeginRequest, New EventHandler(AddressOf context_BeginRequest)

        ' 为上传文件核对文件夹.
        _folderPath = HttpContext.Current.Server.MapPath(_uploadedFilesFolder)
        If Not Directory.Exists(_folderPath) Then
            Directory.CreateDirectory(_folderPath)
        End If
    End Sub

    Private Sub context_BeginRequest(ByVal sender As Object, ByVal e As EventArgs)

        Dim app As HttpApplication = TryCast(sender, HttpApplication)
        Dim context As HttpContext = app.Context

        ' 我们需要HttpWorkerRequest的当前内容来处理请求数据.
        ' 要想知道HttpWorkerRequest更多更详细的内容, 
        ' 请参考根目录下的Readme文件.
        Dim provider As IServiceProvider = DirectCast(context, IServiceProvider)
        Dim request As System.Web.HttpWorkerRequest = _
            DirectCast(provider.GetService(GetType(HttpWorkerRequest)), HttpWorkerRequest)


        ' 获取当前请求的内容类型.
        Dim contentType As String = _
            request.GetKnownRequestHeader(HttpWorkerRequest.HeaderContentType)

        ' 如果我们不能获取内容类型，跳过这个模块.
        If contentType Is Nothing Then
            Return
        End If

        '   如果内容类型不是multipart/form-data,
        '   意味着没有上传请求，
        '   就可以跳过这个模块.
        If contentType.IndexOf("multipart/form-data") = -1 Then
            Return
        End If

        Dim boundary As String = contentType.Substring(contentType.IndexOf("boundary=") + 9)

        ' 获取当前请求的内容长度.
        Dim contentLength As Long
        contentLength = Convert.ToInt64( _
            request.GetKnownRequestHeader(HttpWorkerRequest.HeaderContentLength))



        ' 获取HTTP请求主体的那些
        ' 当前已经被读取的数据.
        ' 这是我们存储上传文件的第一步.
        Dim data As Byte() = request.GetPreloadedEntityBody()

        ' 创建一个管理类的实例可以
        ' 帮助过滤请求数据.
        Dim storeManager As New FileUploadDataManager(boundary)
        ' 添加预装载的数据.
        storeManager.AppendData(data)


        Dim status As UploadStatus = Nothing
        If context.Cache(_cacheContainer) Is Nothing Then
            ' 初始化UploadStatus，
            ' 它被用来存储客户状态.
            ' 把当前内容发送到status被事件使用 
            '   
            ' 初始化文件长度.
            status = New UploadStatus(context, contentLength)
            ' 当更新状态时绑定事件.

            AddHandler status.OnDataChanged, AddressOf status_OnDataChanged
        Else
            status = TryCast(context.Cache(_cacheContainer), UploadStatus)
            If status.IsFinished Then
                Return
            End If
        End If


        ' 获取留下的请求数据的长度. 
        Dim leftdata As Long = status.ContentLength - status.LoadedLength

        ' 定义一个自定义的缓存区的长度
        Dim customBufferLength As Integer = 2048

        While Not request.IsEntireEntityBodyIsPreloaded() AndAlso leftdata > 0
            ' 检查用户如果终止了上传，关闭连接.
            If status.Aborted Then
                ' 删除缓存文件.
                For Each file As UploadFile In storeManager.FilterResult
                    file.ClearCache()
                Next
                request.CloseConnection()
                Return
            End If

            ' 如果剩下的请求数据小于缓
            ' 冲区的长度，把缓冲区的
            ' 长度设置成剩余数据的长度.
            If leftdata < customBufferLength Then
                customBufferLength = CInt(leftdata)
            End If

            ' 读取自定义缓冲区的长度的请求数据
            data = New Byte(customBufferLength - 1) {}
            Dim redlen As Integer = request.ReadEntityBody(data, customBufferLength)
            If customBufferLength > redlen Then
                data = BinaryHelper.SubData(data, 0, redlen)
            End If
            ' 添加剩余数据.
            storeManager.AppendData(data)

            ' 把缓冲区的长度添加到status来更新上传status.
            status.UpdateLoadedLength(redlen)

            leftdata -= redlen
        End While

        ' 当所有的数据都被读取之后,
        ' 保存上传文件.
        For Each file As UploadFile In storeManager.FilterResult
            file.Save(Nothing)
        Next


    End Sub

    Private Sub status_OnDataChanged(ByVal sender As Object, ByVal e As UploadStatusEventArgs)
        ' 保存status class到缓存的当前内容.
        Dim state As UploadStatus = TryCast(sender, UploadStatus)
        If e.context.Cache(_cacheContainer) Is Nothing Then
            e.context.Cache.Add(_cacheContainer, _
                                state, _
                                Nothing, _
                                DateTime.Now.AddDays(1), _
                                Cache.NoSlidingExpiration, _
                                CacheItemPriority.High, _
                                Nothing)
        Else
            e.context.Cache(_cacheContainer) = state
        End If

    End Sub



End Class

