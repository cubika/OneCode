'********************************* 模块头 *********************************\
' 模块名: ChooseMediaPage.xaml.vb
' 项目名: VideoStoryCreator
' 版权(c) Microsoft Corporation.
' 
' 这个页面允许用户选择图片.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************/



Imports System.Collections.ObjectModel
Imports Microsoft.Xna.Framework.Media
Imports System.IO

<CLSCompliant(False)> _
Partial Public Class ChooseMediaPage
    Inherits PhoneApplicationPage
    Private _photoDataSource As ObservableCollection(Of ChoosePhotoViewModel)
    Private _selectedPhotos As List(Of ChoosePhotoViewModel)
    Private _currentPage As Integer = 0

    ''' <summary>
    ''' TODO: 设计考虑：目前硬编码20。
    ''' 如果我们将所有的硬编码值到一个地方，
    ''' 所以在未来，它将会更容易地支持用户配置？
    ''' </summary>
    Private _pageSize As Integer = 20

    Public Sub New()
        InitializeComponent()

        Me._photoDataSource = New ObservableCollection(Of ChoosePhotoViewModel)()
        Me._selectedPhotos = New List(Of ChoosePhotoViewModel)()
        Me.GetPicturesForCurrentPage()
        Me.MediaListBox.ItemsSource = _photoDataSource
    End Sub


    Private Sub GetPicturesForCurrentPage()
        ' Page值不能小于0.
        If Me._currentPage < 0 Then
            Me._currentPage = 0
            Return
        End If

        Dim mediaLibrary As New MediaLibrary()

        ' 存储选择.
        Dim pageCount As Integer = mediaLibrary.Pictures.Count / Me._pageSize
        If Me._currentPage > pageCount Then
            Me._currentPage = pageCount
            Return
        End If

        ' Store the selection.
        Me.StoreSelection()

        ' 数据源清空
        Me._photoDataSource.Clear()

        Dim picturesOnCurrentPage = mediaLibrary.Pictures.Skip(Me._currentPage * Me._pageSize).Take(Me._pageSize)
        For Each picture In picturesOnCurrentPage
            Dim pictureStream As Stream = picture.GetThumbnail()
            Dim viewModel As New ChoosePhotoViewModel() With { _
              .Name = picture.Name, _
              .MediaStream = pictureStream _
            }

            ' 在PhotoViewModel, 我们重写photo名称
            ' 如果图片重名，将会返回true
            If Me._selectedPhotos.Contains(viewModel) Then
                viewModel.IsSelected = True
            End If

            _photoDataSource.Add(viewModel)
        Next
    End Sub

    ''' <summary>
    ''' 当前页面上选中的照片添加到选定的照片列表，
    ''' 并关闭未选中的照片。
    ''' </summary>
    Private Sub StoreSelection()
        For Each photo As ChoosePhotoViewModel In Me._photoDataSource
            If photo.IsSelected AndAlso Not Me._selectedPhotos.Contains(photo) Then
                Me._selectedPhotos.Add(photo)
            Else
                ' 如果图片没有关闭，关闭Stream流
                photo.MediaStream.Close()
            End If
        Next
    End Sub

    Private Sub OKButton_Click(sender As Object, e As System.EventArgs)
        Me.StoreSelection()

        ' 添加所有选中的图片到App.MediaCollection.
        For Each photo As ChoosePhotoViewModel In Me._selectedPhotos
            Dim photoModel As New Photo() With { _
              .Name = photo.Name, _
              .ThumbnailStream = photo.MediaStream, _
              .PhotoDuration = TimeSpan.FromSeconds(5.0), _
              .Transition = TransitionFactory.CreateDefaultTransition() _
            }
            If Not App.MediaCollection.Contains(photoModel) Then
                App.MediaCollection.Add(photoModel)
            End If
        Next

        ' 回到调用页面
        Me.NavigationService.GoBack()
    End Sub

    Private Sub CancelButton_Click(sender As Object, e As System.EventArgs)
        ' 关掉thumbnail streams.
        For Each photo As ChoosePhotoViewModel In Me._photoDataSource
            photo.MediaStream.Close()
        Next

        ' 回到调用页面.
        Me.NavigationService.GoBack()
    End Sub

    Private Sub PreviousPageButton_Click(sender As Object, e As System.EventArgs)
        Me._currentPage -= 1
        Me.GetPicturesForCurrentPage()
    End Sub

    Private Sub NextPageButton_Click(sender As Object, e As System.EventArgs)
        Me._currentPage += 1
        Me.GetPicturesForCurrentPage()
    End Sub
End Class
