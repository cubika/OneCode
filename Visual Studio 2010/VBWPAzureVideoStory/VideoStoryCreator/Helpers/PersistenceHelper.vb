'********************************* 模块头 *********************************\
' 模块名: PersistenceHelper.vb
' 项目名: VideoStoryCreator
' 版权(c) Microsoft Corporation.
' 
' 序列化和反序列化story到xml的helper类.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************/



Imports System.IO.IsolatedStorage
Imports Microsoft.Xna.Framework.Media
Imports System.IO
Imports System.Xml.Linq


Public NotInheritable Class PersistenceHelper
    ''' <summary>
    ''' 序列化story到xml, 存储在isolated storage.
    ''' </summary>
    Friend Shared Sub StoreData()
        If Not String.IsNullOrEmpty(App.CurrentStoryName) Then
            Dim userStore As IsolatedStorageFile = IsolatedStorageFile.GetUserStoreForApplication()
            Using fileStream As IsolatedStorageFileStream = userStore.CreateFile(App.CurrentStoryName + ".xml")
                ' 序列化tory, 并且保存.
                SerializeStory().Save(fileStream)
            End Using

            ' 保存当前story名称.
            Using fileStream As IsolatedStorageFileStream = userStore.OpenFile("CurrentStory.txt", System.IO.FileMode.OpenOrCreate)
                Using writer As New StreamWriter(fileStream)
                    writer.Write(App.CurrentStoryName)
                End Using
            End Using
        End If
    End Sub

    ''' <summary>
    ''' 从isolated storage读取xml文件, 反序列化为story.
    ''' </summary>
    Friend Shared Sub RestoreData()
        Dim userStore As IsolatedStorageFile = IsolatedStorageFile.GetUserStoreForApplication()

        ' 读取当前story名称.
        If userStore.FileExists("CurrentStory.txt") Then
            Using fileStream As IsolatedStorageFileStream = userStore.OpenFile("CurrentStory.txt", System.IO.FileMode.Open)
                Using reader As New StreamReader(fileStream)
                    App.CurrentStoryName = reader.ReadToEnd()
                End Using
            End Using
        End If

        ' 如果当前story被找到, 加载它.
        If Not String.IsNullOrEmpty(App.CurrentStoryName) AndAlso userStore.FileExists(App.CurrentStoryName + ".xml") Then
            ReadStoryFile(App.CurrentStoryName, userStore)
        End If
    End Sub

    ''' <summary>
    ''' 读取指定的story.
    ''' </summary>
    ''' <param name="storyName">story名称.</param>
    ''' <param name="userStore">如果参数为null, 创建一个新的.</param>
    Friend Shared Sub ReadStoryFile(storyName As String, Optional userStore As IsolatedStorageFile = Nothing)
        If userStore Is Nothing Then
            userStore = IsolatedStorageFile.GetUserStoreForApplication()
        End If
        Using fileStream As IsolatedStorageFileStream = userStore.OpenFile(storyName & ".xml", System.IO.FileMode.Open)
            Dim xdoc As XDocument = XDocument.Load(fileStream)
            Dim mediaLibrary As New MediaLibrary()
            Dim picturesLibrary = mediaLibrary.Pictures

            ' Load all photos.
            For Each photoElement As XElement In xdoc.Root.Elements()
                Try
                    Dim photo As New Photo() With { _
                      .Name = photoElement.Attribute("Name").Value _
                    }
                    Dim photoDurationString As String = photoElement.Attribute("PhotoDuration").Value
                    Dim photoDuration As Integer = Integer.Parse(photoDurationString)
                    photo.PhotoDuration = TimeSpan.FromSeconds(photoDuration)
                    Dim transitionElement As XElement = photoElement.Element("Transition")
                    If transitionElement IsNot Nothing Then
                        photo.Transition = TransitionBase.Load(photoElement.Element("Transition"))
                    End If
                    Dim picture As Picture = picturesLibrary.Where(Function(p) p.Name = photo.Name).FirstOrDefault()
                    If picture Is Nothing Then
                        '' 如果找不到原文件，可能已经被删除了
                        '' 我们需要记录错误吗? 我们是继续下一个图片还是抛出异常?
                        Continue For
                    End If
                    photo.ThumbnailStream = picture.GetThumbnail()
                    App.MediaCollection.Add(photo)
                Catch
                    ' TODO: 我们需要记录错误吗? 我们是继续下一个图片还是抛出异常?
                    Continue For
                End Try
            Next
            mediaLibrary.Dispose()
        End Using
    End Sub

    ''' <summary>
    '''  序列化当前story.
    '''  我们只序列化story数据, 比如每个photo的duration.
    '''  我们不序列化下面的bitmap.
    ''' </summary>
    ''' <returns>当前story的XDocument对象</returns>
    Friend Shared Function SerializeStory() As XDocument
        Dim xdoc As New XDocument(New XElement("Story", New XAttribute("Name", App.CurrentStoryName), New XAttribute("PhotoCount", App.MediaCollection.Count)))

        ' 把每张图片保存为xml元素.
        For Each photo As Photo In App.MediaCollection
            Dim photoElement As New XElement("Photo")
            photoElement.Add(New XAttribute("Name", photo.Name))
            photoElement.Add(New XAttribute("PhotoDuration", photo.PhotoDuration.TotalSeconds))
            If photo.Transition IsNot Nothing Then
                Dim transitionElement As New XElement("Transition")
                photo.Transition.Save(transitionElement)
                photoElement.Add(transitionElement)
            End If
            xdoc.Root.Add(photoElement)
        Next
        Return xdoc
    End Function

    ''' <summary>
    ''' 列举所有保存了的story, 并且返回story名称.
    ''' </summary>
    ''' <returns>story列表, 没有.xml扩展名.</returns>
    Friend Shared Function EnumerateStories() As List(Of String)
        Dim userStore As IsolatedStorageFile = IsolatedStorageFile.GetUserStoreForApplication()
        Return (From f In userStore.GetFileNames() Where f.EndsWith(".xml") Select f.Substring(0, f.Length - 4)).ToList()
    End Function
End Class
