'****************************** 模块头 ******************************\
' 模块名:	StoryService.vb
' 项目名: StoryCreatorWebRole
' 版权 (c) Microsoft Corporation.
' 
' 使用 Web API 构建 WCF REST服务.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************/



Imports System.ServiceModel
Imports System.Net.Http
Imports System.Net
Imports System.Runtime.Serialization.Json
Imports System.IO
Imports Microsoft.WindowsAzure.StorageClient
Imports System.ServiceModel.Web
Imports System.Xml
Imports StoryDataModel

<ServiceContract()>
Public Class StoryService
    ''' <summary>
    ''' 建立一个新的story.
    ''' </summary>
    <WebInvoke(Method:="POST", UriTemplate:="")> _
    Public Function Post(request As HttpRequestMessage) As HttpResponseMessage
        '  保证Azure存储账号已就绪.
        If Global_asax.StorageAccount Is Nothing Then
            If Not Global_asax.InitializeStorage() Then
                Return Me.CreateStringResponse(HttpStatusCode.BadRequest, "服务现在不可用. 请稍后再试.")
            End If
        End If

        Try
            ' Request 内文.
            Dim docSource As XDocument = XDocument.Parse(request.Content.ReadAsString())

            ' Response 内文.
            Dim docResult As New XDocument(New XElement("Story"))


            Dim photos = docSource.Root.Elements("Photo")
            Dim photoElementsCount As Integer = photos.Count()
            If photos.Count() < 2 Then
                Return Me.CreateStringResponse(HttpStatusCode.BadRequest, "短影需要至少2张照片.")
            End If

            Dim photoCount As Integer = 0
            Try
                photoCount = Integer.Parse(docSource.Root.Attribute("PhotoCount").Value)
            Catch
                Return Me.CreateStringResponse(HttpStatusCode.BadRequest, "请求正文的格式不正确.短影所需的属性PhotoCount丢失或不正确.")
            End Try

            If photoElementsCount <> photoCount Then
                Return Me.CreateStringResponse(HttpStatusCode.BadRequest, "请求正文的格式不正确.PhotoCount不等于照片文件数.")
            End If

            Dim blobClient As New CloudBlobClient(Global_asax.StorageAccount.BlobEndpoint, Global_asax.StorageAccount.Credentials)
            Dim container As CloudBlobContainer = blobClient.GetContainerReference("videostories")

            ' 代表短影的唯一ID.
            Dim id As String = Guid.NewGuid().ToString()
            Dim configBlob As CloudBlob = container.GetBlobReference(id & ".xml")
            docResult.Root.Add(New XAttribute("ID", id))

            For Each photo As XElement In photos
                Dim name As String = photo.Attribute("Name").Value

                ' 构造SAS.  开始时间设置为 1 分钟前 ,
                ' 确保客户端能够上传blob.
                Dim blob As CloudBlob = container.GetBlobReference(id & "/" & name)
                Dim sas As String = blob.GetSharedAccessSignature(New SharedAccessPolicy() With { _
                  .Permissions = SharedAccessPermissions.Write, _
                  .SharedAccessStartTime = DateTime.Now.AddMinutes(-1.0), _
                  .SharedAccessExpiryTime = DateTime.Now.AddHours(0.5) _
                })

                ' 创建空的 blob，因此客户端可以上载至正确的 blob .
                blob.UploadText("")

                ' 修改原始配置.添加无 SAS 的 URI.
                photo.Add(New XAttribute("Uri", blob.Uri.AbsoluteUri))

                ' 将图片元素添加到响应，包括 SAS.
                Dim fullUri As String = blob.Uri.AbsoluteUri & sas
                docResult.Root.Add(New XElement("Photo", New XAttribute("Name", name), New XAttribute("Uri", fullUri)))
            Next

            ' 在 blob存储中存储配置.
            configBlob.UploadText(docSource.ToString())

            Trace.Write("短影配置已创建: " & configBlob.Uri.ToString(), "Information")

            ' 返回成功响应.
            Return Me.CreateXmlResponse(HttpStatusCode.Created, docResult.ToString())
        Catch generatedExceptionName As XmlException
            Return Me.CreateStringResponse(HttpStatusCode.BadRequest, "请求主体不是格式化好的 xml 文档.")
        Catch ex As StorageClientException
            Trace.Write("处理blob错误: " + ex.Message, "Error")
            Return Me.CreateStringResponse(HttpStatusCode.InternalServerError, "该服务目前不可用.请稍后再试.")
        End Try
    End Function

    ''' <summary>
    ''' 更新短影数据源.
    ''' 目前唯一的更新是提交 （表示我们就可以对视频进行编码）短影.
    ''' </summary>
    <WebInvoke(Method:="PUT", UriTemplate:="{id}?commit={commit}")> _
    Public Function Put(request As HttpRequestMessage, id As String, commit As System.Nullable(Of Boolean)) As HttpResponseMessage
        ' 请确保该存储帐户是准备好了.
        If Global_asax.StorageAccount Is Nothing Then
            If Not Global_asax.InitializeStorage() Then
                Return Me.CreateStringResponse(HttpStatusCode.BadRequest, "该服务目前不可用.请稍后再试.")
            End If
        End If
        If String.IsNullOrEmpty(id) Then
            Return Me.CreateStringResponse(HttpStatusCode.BadRequest, "必需的参数标识已丢失.")
        End If
        If commit Is Nothing OrElse Not commit.Value Then
            Return Me.CreateStringResponse(HttpStatusCode.BadRequest, "当前仅支持 ""commit=true"" 选项.")
        End If

        Try
            Dim blobClient As New CloudBlobClient(Global_asax.StorageAccount.BlobEndpoint, Global_asax.StorageAccount.Credentials)
            Dim container As CloudBlobContainer = blobClient.GetContainerReference("videostories")
            Dim configBlob As CloudBlob = container.GetBlobReference(id & ".xml")

            ' 我们其实不需要这些属性.我们只是检查blob是否存在.
            ' 如果blob不存在，将引发 StorageClientException，所以我们跳至 catch 块.
            configBlob.FetchAttributes()

            ' 添加到队列的作业.
            Dim queueClient As New CloudQueueClient(Global_asax.StorageAccount.QueueEndpoint, Global_asax.StorageAccount.Credentials)
            Dim queue As CloudQueue = queueClient.GetQueueReference("videostories")
            queue.AddMessage(New CloudQueueMessage(id))

            ' 返回一个空的成功消息.

            Return Me.CreateStringResponse(HttpStatusCode.NoContent, "")
        Catch ex As StorageClientException
            If ex.StatusCode = HttpStatusCode.NotFound Then
                Return Me.CreateStringResponse(HttpStatusCode.NotFound, "请求的Story不存在")
            End If

            ' 一般错误，跟踪并返回泛用消息.
            Trace.Write("处理blob错误: " + ex.Message, "Error")
            Return Me.CreateStringResponse(HttpStatusCode.InternalServerError, "该服务目前不可用.请稍后再试.")
        End Try
    End Function

    <WebGet(UriTemplate:="")> _
    Public Function [Get](request As HttpRequestMessage) As HttpResponseMessage
        ' 请确保该存储帐户是准备好了.
        If Global_asax.StorageAccount Is Nothing Then
            If Not Global_asax.InitializeStorage() Then
                Return Me.CreateStringResponse(HttpStatusCode.BadRequest, "该服务目前不可用.请稍后再试.")
            End If
        End If

        Try
            Dim storyDataContext As New StoryDataContext(Global_asax.StorageAccount.TableEndpoint.AbsoluteUri, Global_asax.StorageAccount.Credentials)

            ' 查询表存储.
            Dim query = From s In storyDataContext.Stories

            ' 将结果转换为一个简化的类，不包含分区/行键.
            Dim stories As New List(Of Story)()
            For Each s As StoryDataModel.Story In query
                stories.Add(New Story() With { _
                  .Name = s.Name, _
                  .VideoUri = s.VideoUri _
                })
            Next
            Dim jsonSerializer As New DataContractJsonSerializer(GetType(List(Of Story)))
            Using stream As New MemoryStream()
                jsonSerializer.WriteObject(stream, stories)
                stream.Position = 0
                Using reader As New StreamReader(stream)
                    Dim result As String = reader.ReadToEnd()
                    Return Me.CreateJsonResponse(HttpStatusCode.OK, result)
                End Using
            End Using
        Catch ex As StorageClientException
            If ex.StatusCode = HttpStatusCode.NotFound Then
                Return Me.CreateStringResponse(HttpStatusCode.NotFound, "请求的Story不存在")
            End If

            ' 一般错误，跟踪并返回泛用消息.
            Trace.Write("处理表服务错误: " + ex.Message, "Error")
            Return Me.CreateStringResponse(HttpStatusCode.InternalServerError, "该服务目前不可用.请稍后再试.")
        Catch ex2 As Data.EvaluateException
            Trace.Write("处理表服务错误" + ex2.Message, "Error")
            Return Me.CreateStringResponse(HttpStatusCode.InternalServerError, "该服务目前不可用.请稍后再试.")
        End Try
    End Function

    Private Function CreateStringResponse(statusCode As HttpStatusCode, body As String) As HttpResponseMessage
        Dim response As New HttpResponseMessage()
        response.StatusCode = statusCode
        response.Content = New StringContent(body)
        Return response
    End Function

    Private Function CreateXmlResponse(statusCode As HttpStatusCode, body As String) As HttpResponseMessage
        Dim response As New HttpResponseMessage()
        response.StatusCode = statusCode
        response.Content = New StringContent(body, Encoding.UTF8, "text/xml")
        Return response
    End Function

    Private Function CreateJsonResponse(statusCode As HttpStatusCode, body As String) As HttpResponseMessage
        Dim response As New HttpResponseMessage()
        response.StatusCode = statusCode
        response.Content = New StringContent(body, Encoding.UTF8, "application/json")
        Return response
    End Function
End Class
