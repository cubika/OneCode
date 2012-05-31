'********************************* 模块头 *********************************\
' 模块名: MainPage.xaml.vb
' 项目名: VideoStoryCreator
' 版权(c) Microsoft Corporation.
' 
' 主页. 包含链接到其他页面的按钮.
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
Partial Public Class MainPage
    Inherits PhoneApplicationPage

    ' 构造器
    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub NewStoryButton_Click(sender As Object, e As System.Windows.RoutedEventArgs)
        If MessageBox.Show("如果您开始一个新的短影, 当前短影中未保存的信息将会丢失. 是否想要继续?", "确认", MessageBoxButton.OKCancel) = MessageBoxResult.OK Then
            Me.newStoryStoryboard.Begin()
        End If
    End Sub

    Private Sub LastStoryButton_Click(sender As Object, e As System.Windows.RoutedEventArgs)
        If String.IsNullOrEmpty(App.CurrentStoryName) Then
            MessageBox.Show("未找到当前短影.")
            Return
        End If
        Me.NavigationService.Navigate(New Uri("/ComposePage.xaml", UriKind.Relative))
    End Sub

    Private Sub PreviousStoryButton_Click(sender As Object, e As System.Windows.RoutedEventArgs)
        Me.NavigationService.Navigate(New Uri("/ChooseStoryPage.xaml", UriKind.Relative))
    End Sub

    Private Sub newStoryOKButton_Click(sender As Object, e As System.Windows.RoutedEventArgs)
        If Not String.IsNullOrEmpty(Me.newStoryNameTextBox.Text) Then
            ' 清除内存数据.
            For Each photo In App.MediaCollection
                photo.ThumbnailStream.Close()
            Next
            App.MediaCollection.Clear()
            App.CurrentStoryName = Me.newStoryNameTextBox.Text
            Me.closeNewStoryStoryboard.Begin()
            Me.NavigationService.Navigate(New Uri("/ComposePage.xaml", UriKind.Relative))
        End If
    End Sub

    Private Sub newStoryCancelButton_Click(sender As Object, e As System.Windows.RoutedEventArgs)
        Me.closeNewStoryStoryboard.Begin()
    End Sub
End Class

