'********************************* 模块头 *********************************\
' 模块名: ComposePage.xaml.vb
' 项目名: VideoStoryCreator
' 版权(c) Microsoft Corporation.
' 
' 生成短影的页面.
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

<CLSCompliant(False)> _
Partial Public Class ComposePage
    Inherits PhoneApplicationPage
    Private _photoDataSource As ObservableCollection(Of ComposePhotoViewModel)
    Private _viewModelBackup As ComposePhotoViewModel
    Public Sub New()
        InitializeComponent()
    End Sub

    Protected Overrides Sub OnNavigatedTo(e As System.Windows.Navigation.NavigationEventArgs)
        ' 准备数据源.
        Me._photoDataSource = New ObservableCollection(Of ComposePhotoViewModel)()
        Me.nameTextBox.Text = App.CurrentStoryName
        For Each photo As Photo In App.MediaCollection
            _photoDataSource.Add(ComposePhotoViewModel.CreateFromModel(photo))
        Next
        Me.photoListBox.ItemsSource = Me._photoDataSource

        MyBase.OnNavigatedTo(e)
    End Sub

    Protected Overrides Sub OnNavigatedFrom(e As System.Windows.Navigation.NavigationEventArgs)
        Me.UpdateModels()
        MyBase.OnNavigatedFrom(e)
    End Sub

    ''' <summary>
    ''' 绑定到试图模型的控件.
    ''' 因此我们需要显式更新所需基本模型 .
    ''' </summary>
    Private Sub UpdateModels()
        For Each viewModel As ComposePhotoViewModel In Me._photoDataSource
            viewModel.UpdateModel()
        Next
    End Sub

    Private Sub PreviewButton_Click(sender As Object, e As System.EventArgs)
        Me.NavigationService.Navigate(New Uri("/PreviewPage.xaml", UriKind.Relative))
    End Sub

    Private Sub EditPhotoButton_Click(sender As Object, e As System.EventArgs)
        If Me.photoListBox.SelectedItem IsNot Nothing AndAlso TypeOf Me.photoListBox.SelectedItem Is ComposePhotoViewModel Then
            Me.photoListBox.IsEnabled = False

            ' 备份视图模型, 因此我们可以撤销更新操作.
            Me._viewModelBackup = DirectCast(Me.photoListBox.SelectedItem, ComposePhotoViewModel).CopyTo()
            Me.ShowEditPanelStoryboard.Begin()
        End If
    End Sub

    Private Sub OKButton_Click(sender As Object, e As System.Windows.RoutedEventArgs)
        Me.photoListBox.IsEnabled = True
        Me.CloseEditPanelStoryboard.Begin()
    End Sub

    Private Sub CancelButton_Click(sender As Object, e As System.Windows.RoutedEventArgs)
        ' 恢复视图模型备份.
        If Me._viewModelBackup IsNot Nothing Then
            DirectCast(Me.photoListBox.SelectedItem, ComposePhotoViewModel).CopyFrom(Me._viewModelBackup)
        End If
        Me.photoListBox.IsEnabled = True
        Me.CloseEditPanelStoryboard.Begin()
    End Sub

    Private Sub AddPhotoButton_Click(sender As Object, e As System.EventArgs)
        Me.NavigationService.Navigate(New Uri("/ChooseMediaPage.xaml", UriKind.Relative))
    End Sub

    Private Sub RemovePhotoButton_Click(sender As Object, e As System.EventArgs)
        ' 移除选定项目, 关闭对应流.
        If Me.photoListBox.SelectedItem IsNot Nothing AndAlso TypeOf Me.photoListBox.SelectedItem Is ComposePhotoViewModel Then
            Dim photo As ComposePhotoViewModel = DirectCast(Me.photoListBox.SelectedItem, ComposePhotoViewModel)
            photo.MediaStream.Close()
            Me._photoDataSource.Remove(photo)
            photo.RemoveModel()
        End If
    End Sub

    Private Sub nameTextBox_LostFocus(sender As Object, e As System.Windows.RoutedEventArgs)
        ' 更新短影名.
        If App.CurrentStoryName <> Me.nameTextBox.Text AndAlso Me.nameTextBox.Text IsNot Nothing Then
            If IsolatedStorageHelper.FileExists(Convert.ToString(Me.nameTextBox.Text) & ".xml") Then
                If MessageBox.Show("同样名称的短影已经存在. 是否覆盖?", "Confirm", MessageBoxButton.OKCancel) = MessageBoxResult.OK Then
                    Me.RenameStory()
                Else
                    ' 回滚旧名.
                    Me.nameTextBox.Text = App.CurrentStoryName
                End If
            Else
                Me.RenameStory()
            End If
        End If
    End Sub

    ''' <summary>
    ''' 重命名短影.
    ''' </summary>
    Private Sub RenameStory()
        IsolatedStorageHelper.RenameFile(App.CurrentStoryName + ".xml", Convert.ToString(Me.nameTextBox.Text) & ".xml")
        App.CurrentStoryName = Me.nameTextBox.Text
    End Sub

    Private Sub UploadButton_Click(sender As Object, e As EventArgs)
        ' 确保我们正在使用最新的数据.
        Me.UpdateModels()
        Dim locator As New StoryServiceLocator()
        AddHandler locator.StoryUploaded, AddressOf locator_StoryUploaded
        locator.UploadStory()
    End Sub

    Private Sub locator_StoryUploaded(sender As Object, e As EventArgs)
        Me.Dispatcher.BeginInvoke(Sub()
                                      MessageBox.Show("短影已成功上传至云.")

                                  End Sub)
    End Sub

    Private Sub TransitionList_SelectionChanged(sender As Object, e As System.Windows.Controls.SelectionChangedEventArgs)
        If Me.photoListBox.SelectedItem IsNot Nothing AndAlso TypeOf Me.photoListBox.SelectedItem Is ComposePhotoViewModel Then
            ' 更新设计器以显示选定特效所需的额外的控件.
            ' UI是一个内容绑定到视图模型的TransitionDesigner属性的ContentControl.
            Dim photo As ComposePhotoViewModel = DirectCast(Me.photoListBox.SelectedItem, ComposePhotoViewModel)
            photo.TransitionDesigner = TransitionFactory.CreateTransitionDesigner(Me.transitionList.SelectedItem.ToString())
        End If
    End Sub

    Private Sub HomeButton_Click(sender As Object, e As EventArgs)
        Me.NavigationService.Navigate(New Uri("/MainPage.xaml", UriKind.Relative))
    End Sub
End Class
