'*********************************** 模块头 ***********************************\
' 模块名:  WorkerRole.vb
' 项目名:  VideoEncodingWorkerRole
' 版权 (c) Microsoft Corporation.
' 
' Worker Role对视频进行编码.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************/



Imports System
Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.Linq
Imports System.Net
Imports System.Threading
Imports Microsoft.WindowsAzure
Imports Microsoft.WindowsAzure.Diagnostics
Imports Microsoft.WindowsAzure.ServiceRuntime
Imports Microsoft.WindowsAzure.StorageClient
Imports System.IO
Imports StoryDataModel

Public Class WorkerRole
    Inherits RoleEntryPoint
    Private _storageAccount As CloudStorageAccount
    Private _container As CloudBlobContainer
    Private _queue As CloudQueue
    Public Overrides Sub Run()
        ' 这是worker实现示例.请替换为您的逻辑.
        Trace.WriteLine("VideoEncodingWorkerRole入口点调用", "Information")

        ' 获取本地存储的路径 .
        Dim localStorageRoot As String = RoleEnvironment.GetLocalResource("PhotoStore").RootPath
        Dim diagnosticsRoot As String = RoleEnvironment.GetLocalResource("DiagnosticStore").RootPath
        While True
            Try
                ' 从队列中得到一条消息.
                Dim message As CloudQueueMessage = Me._queue.GetMessage(TimeSpan.FromMinutes(20.0))
                If message IsNot Nothing Then
                    Dim storyID As String = message.AsString

                    ' 短影无法编码...
                    If message.DequeueCount > 3 Then
                        Trace.Write("短影 " & storyID & "已尝试多次均未能被处理.", "Error")
                        Me._queue.DeleteMessage(message)
                    Else
                        Trace.Write("开始处理短影 " & storyID & ".", "Information")

                        Dim storyFolderPath As String = Path.Combine(localStorageRoot, storyID)
                        Directory.CreateDirectory(storyFolderPath)

                        ' 下载Xml 配置文件.
                        Dim blob As CloudBlob = Me._container.GetBlobReference(storyID & ".xml")
                        Dim config As String = blob.DownloadText()
                        Dim xdoc As XDocument = XDocument.Parse(config)
                        Dim storyName As String = xdoc.Root.Attribute("Name").Value
                        For Each photo In xdoc.Root.Elements("Photo")
                            ' 下载照片，并将其保存在本地存储.
                            Dim photoBlobUri As String = photo.Attribute("Uri").Value
                            Dim photoBlob As CloudBlob = Me._container.GetBlobReference(photoBlobUri)
                            Dim localPhotoFilePath As String = Path.Combine(localStorageRoot, storyID, photo.Attribute("Name").Value)
                            photoBlob.DownloadToFile(localPhotoFilePath)

                            ' 修改图片的名称，包括完整路径 .
                            photo.Attribute("Name").Value = localPhotoFilePath
                        Next

                        ' 保存配置文件到本地存储.
                        Dim configFilePath As String = Path.Combine(localStorageRoot, storyID & ".xml")
                        xdoc.Save(configFilePath, SaveOptions.None)

                        ' 编码视频.
                        Dim outputFilePath As String = Path.Combine(localStorageRoot, storyID & ".mp4")
                        Dim logFilePath As String = Path.Combine(diagnosticsRoot, storyID & ".log")
                        Dim hr As Integer = NativeMethods.EncoderVideo(configFilePath, outputFilePath, logFilePath)
                        If hr <> 0 Then
                            Trace.Write("编码短影" & storyID & "时出错. 非托管代码返回的HRESULT:: " & hr & ".", "Error")
                        Else
                            ' 上传结果视频至blob.
                            Dim blobName As String = storyID & "/"
                            blobName += If(String.IsNullOrEmpty(storyName), storyID, storyName)
                            blobName += ".mp4"
                            Dim outputBlob As CloudBlob = Me._container.GetBlobReference(blobName)
                            outputBlob.UploadFile(outputFilePath)
                            outputBlob.Properties.ContentType = "video/mp4"
                            outputBlob.SetProperties()

                            Dim storyDataContext As New StoryDataContext(Me._storageAccount.TableEndpoint.AbsoluteUri, Me._storageAccount.Credentials)
                            storyDataContext.AddObject("Stories", New Story(storyID, storyName, outputBlob.Uri.AbsoluteUri))
                            storyDataContext.SaveChanges()

                            ' 删除本地文件.
                            File.Delete(configFilePath)
                            File.Delete(outputFilePath)
                            For Each fileName As String In Directory.GetFiles(storyFolderPath)
                                File.Delete(fileName)
                            Next
                            Directory.Delete(storyFolderPath)

                            Me._queue.DeleteMessage(message)

                            ' TODO: 我们应该从 blob删除短影的配置文件和照片?

                            Trace.Write("短影 " & storyID & "成功编码.", "Information")
                        End If
                    End If
                End If
            Catch ex As Exception
                Trace.Write("处理短影错误: " + ex.Message)
            End Try

            Thread.Sleep(10000)
        End While
    End Sub

    Public Overrides Function OnStart() As Boolean

        ' 设置的最大并发连接数
        ServicePointManager.DefaultConnectionLimit = 12

        ' 有关处理配置更改信息
        ' 请参阅 MSDN 主题在  http://go.microsoft.com/fwlink/?LinkId=166357.

        ' 如果没有被初始化，初始化存储.
        CloudStorageAccount.SetConfigurationSettingPublisher(Function(configName, configSetter)
                                                                 configSetter(RoleEnvironment.GetConfigurationSettingValue(configName))

                                                             End Function)
        Me._storageAccount = CloudStorageAccount.FromConfigurationSetting("DataConnectionString")
        Dim blobClient As New CloudBlobClient(Me._storageAccount.BlobEndpoint, Me._storageAccount.Credentials)
        Me._container = blobClient.GetContainerReference("videostories")
        Me._container.CreateIfNotExist()
        Me._container.SetPermissions(New BlobContainerPermissions() With { _
          .PublicAccess = BlobContainerPublicAccessType.Blob _
        })
        Dim queueClient As New CloudQueueClient(Me._storageAccount.QueueEndpoint, Me._storageAccount.Credentials)
        Me._queue = queueClient.GetQueueReference("videostories")
        Me._queue.CreateIfNotExist()
        Dim tableClient As New CloudTableClient(Me._storageAccount.TableEndpoint.AbsoluteUri, Me._storageAccount.Credentials)
        tableClient.CreateTableIfNotExist("Stories")

        ' 配置诊断程序.
        Dim config = DiagnosticMonitor.GetDefaultInitialConfiguration()
        config.Logs.ScheduledTransferPeriod = TimeSpan.FromMinutes(10.0)
        config.WindowsEventLog.ScheduledTransferPeriod = TimeSpan.FromMinutes(10.0)
        config.Directories.ScheduledTransferPeriod = TimeSpan.FromMinutes(10.0)
        config.Directories.DataSources.Add(New DirectoryConfiguration() With { _
          .Path = RoleEnvironment.GetLocalResource("DiagnosticStore").RootPath, _
          .Container = "videostorydiagnosticstore", _
          .DirectoryQuotaInMB = 200 _
        })
        config.PerformanceCounters.ScheduledTransferPeriod = TimeSpan.FromMinutes(10.0)
        Microsoft.WindowsAzure.Diagnostics.CrashDumps.EnableCollection(True)
        DiagnosticMonitor.Start("Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString", config)
        AddHandler RoleEnvironment.Changing, AddressOf RoleEnvironment_Changing

        Return MyBase.OnStart()

    End Function

    Private Sub RoleEnvironment_Changing(sender As Object, e As RoleEnvironmentChangingEventArgs)
        If e.Changes.OfType(Of RoleEnvironmentConfigurationSettingChange)().Count() > 0 Then
            e.Cancel = True
        End If
    End Sub
End Class
