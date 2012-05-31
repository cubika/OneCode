'********************************* 模块头 *********************************\
' 模块名: StoryServiceLocator.vb
' 项目名: VideoStoryCreator
' 版权(c) Microsoft Corporation.
' 
' 这并不真正实现服务定位器模式。
' 但这个类封装了所有的逻辑来访问REST服务，
' UI组件不再直接依赖服务。
' 这是一种依赖注入。
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************/



Imports System.Xml.Linq
Imports System.IO
Imports System.Threading

Public Class StoryServiceLocator
    ' 云REST服务的基础地质. 改变云服务的地质如果你想在云中测试它.
    'private string _baseServiceUri = "http://127.0.0.1:81/stories";
    Private _baseServiceUri As String = "http://storycreator.cloudapp.net/stories"
    Private _storyID As String

    ' 下面的字段用来上传图片到Blob中.
    ' 我们需要等待所有的图片都被上传了，然后提交.
    Private _allPhotoCount As Integer = 0
    Private _uploadedPhotoCount As Integer = 0
    Private _uploadFailed As Boolean = False
    Private _lockObject As New Object()

    Public Event StoryUploaded As EventHandler

    Public Sub UploadStory()
        ' 建立和上传story配置文件.
        Dim storyConfig As XDocument = PersistenceHelper.SerializeStory()
        Dim webClient As New WebClient()
        AddHandler webClient.UploadStringCompleted, AddressOf UploadConfigCompleted
        webClient.UploadStringAsync(New Uri(Me._baseServiceUri), storyConfig.ToString())
    End Sub

    Private Sub UploadConfigCompleted(sender As Object, e As UploadStringCompletedEventArgs)
        If e.[Error] IsNot Nothing Then
            ' TODO: 记录错误......
            MessageBox.Show("现在连接不上服务,请稍后再试")
        Else
            Try
                ' 返回xml文件，包括Blob的SAS.
                Dim resultXDoc As XDocument = XDocument.Parse(e.Result)
                Me._storyID = resultXDoc.Root.Attribute("ID").Value
                Dim photoElements = resultXDoc.Root.Elements("Photo")

                SyncLock Me._lockObject
                    Me._allPhotoCount = photoElements.Count()
                End SyncLock

                ' 创建一个背后的线程，等待所有图片被上传.
                ' 然后提交.
                Dim thread As New System.Threading.Thread(New ThreadStart(AddressOf Me.WaitUntilAllPhotosUploaded))
                thread.Start()

                For Each photoElement In photoElements
                    Dim name As String = photoElement.Attribute("Name").Value
                    Dim blobUri As String = photoElement.Attribute("Uri").Value

                    ' Find the photo in the current story.
                    Dim photo As Photo = App.MediaCollection.Where(Function(p) p.Name = name).FirstOrDefault()
                    If photo Is Nothing Then
                        Throw New InvalidOperationException("找不到图片")
                    End If
                    If photo.ResizedImageStream Is Nothing Then
                        photo.ResizedImageStream = BitmapHelper.GetResizedImage(photo.Name)
                    End If

                    ' 找到当前story的所有图片
                    photo.ResizedImageStream.Position = 0

                    Dim policy As New RetryPolicy(blobUri)
                    policy.RequestAddress = blobUri
                    policy.Initialize = New Action(Function()
                                                       policy.Request.Method = "PUT"

                                                   End Function)
                    policy.RequestCallback = New AsyncCallback(Function(requestStreamResult)
                                                                   Dim requestStream As Stream = policy.Request.EndGetRequestStream(requestStreamResult)
                                                                   Dim buffer As Byte() = New Byte(photo.ResizedImageStream.Length - 1) {}
                                                                   photo.ResizedImageStream.Position = 0
                                                                   photo.ResizedImageStream.Read(buffer, 0, buffer.Length)
                                                                   requestStream.Write(buffer, 0, buffer.Length)
                                                                   requestStream.Close()

                                                               End Function)

                    policy.ResponseCallback = New AsyncCallback(Function(responseResult)
                                                                    Dim response As HttpWebResponse = DirectCast(policy.Request.EndGetResponse(responseResult), HttpWebResponse)
                                                                    If response.StatusCode <> HttpStatusCode.Created Then
                                                                        Throw New InvalidOperationException("上传失败")
                                                                    End If
                                                                    SyncLock Me._lockObject
                                                                        Me._uploadedPhotoCount += 1
                                                                    End SyncLock

                                                                End Function)
                    policy.MakeRequest()
                Next
            Catch
                ' TODO: 记录错误...

                SyncLock Me._lockObject
                    Me._uploadFailed = True
                End SyncLock
                MessageBox.Show("现在无法上传，请稍后再试.")
            End Try
        End If
    End Sub

    ''' <summary>
    ''' 调用后台线程上传
    ''' </summary>
    Private Sub WaitUntilAllPhotosUploaded()
        While True
            SyncLock Me._lockObject
                ' 所有图片正在上传或已经失败，请断开.
                If (Me._allPhotoCount = Me._uploadedPhotoCount) OrElse (Me._uploadFailed = True) Then
                    Exit While
                End If
            End SyncLock
            Thread.Sleep(3000)
        End While
        If Not Me._uploadFailed Then
            Dim requestUri As String = Me._baseServiceUri & "/" & Me._storyID & "?commit=true"
            Dim request As HttpWebRequest = DirectCast(HttpWebRequest.Create(requestUri), HttpWebRequest)
            request.Method = "PUT"
            request.BeginGetRequestStream(Sub(requestStreamResult)
                                              Dim requestStream As Stream = request.EndGetRequestStream(requestStreamResult)

                                              ' 没有request body.
                                              requestStream.Close()
                                              request.BeginGetResponse(Sub(responseResult)
                                                                           Dim response As HttpWebResponse = DirectCast(request.EndGetResponse(responseResult), HttpWebResponse)
                                                                           If response.StatusCode <> HttpStatusCode.NoContent Then
                                                                               Throw New InvalidOperationException("上传失败.")
                                                                           Else
                                                                               RaiseEvent StoryUploaded(Me, EventArgs.Empty)
                                                                           End If

                                                                       End Sub, Nothing)

                                          End Sub, Nothing)
        End If
    End Sub
End Class
