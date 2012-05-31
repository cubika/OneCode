'********************************* 模块头 *********************************\
' 模块名: ChooseStoryPage.xaml.vb
' 项目名: VideoStoryCreator
' 版权(c) Microsoft Corporation.
' 
' 用户打开现有短影的页面.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************/




<CLSCompliant(False)> _
Partial Public Class ChooseStoryPage
    Inherits PhoneApplicationPage

    Public Sub New()
        InitializeComponent()
    End Sub

    Protected Overrides Sub OnNavigatedTo(e As System.Windows.Navigation.NavigationEventArgs)
        Me.StoryListBox.ItemsSource = PersistenceHelper.EnumerateStories()
        MyBase.OnNavigatedTo(e)
    End Sub

    Private Sub OKButton_Click(sender As Object, e As System.Windows.RoutedEventArgs)
        If Me.StoryListBox.SelectedItem IsNot Nothing OrElse TypeOf Me.StoryListBox.SelectedItem Is String Then
            Dim storyName As String = DirectCast(Me.StoryListBox.SelectedItem, String)
            Try
                ' 清除内存数据.
                For Each photo In App.MediaCollection
                    photo.ThumbnailStream.Close()
                Next
                App.MediaCollection.Clear()
                App.CurrentStoryName = storyName
                PersistenceHelper.ReadStoryFile(storyName)
                Me.NavigationService.Navigate(New Uri("/ComposePage.xaml", UriKind.Relative))
            Catch
                MessageBox.Show("无法载入短影. 文件似乎已经损坏.")
            End Try
        End If
    End Sub

    Private Sub CancelButton_Click(sender As Object, e As System.Windows.RoutedEventArgs)
        Me.NavigationService.Navigate(New Uri("/MainPage.xaml", UriKind.Relative))
    End Sub
End Class
