'****************************** 模块头 ******************************'
' 模块:      MainWindow.xaml.vb
' 项目:      VBWPFAnimatedImage
' 版权 (c) Microsoft Corporation.
' 
' 这个示例演示了如何展示一系列的照片，就像数字相框的图像滑入效果。
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL
' All other rights reserved.
'
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************'

Imports System.Windows.Media.Animation

''' <summary> 
''' Window1.xaml的交互逻辑
''' </summary> 
Partial Public Class MainWindow
    Inherits Window
    Public Sub New()
        InitializeComponent()
    End Sub

    Private nextImageIndex As Integer
    Private images As New List(Of BitmapImage)()

    Private Sub Window_Loaded(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' 初始化图片集合
        images.Add(New BitmapImage(New Uri("Images/image1.jpg", UriKind.Relative)))
        images.Add(New BitmapImage(New Uri("Images/image2.jpg", UriKind.Relative)))
        images.Add(New BitmapImage(New Uri("Images/image3.jpg", UriKind.Relative)))
        images.Add(New BitmapImage(New Uri("Images/image4.jpg", UriKind.Relative)))

        nextImageIndex = 2
    End Sub

    Private Sub VisbleToInvisible_Completed(ByVal sender As Object, ByVal e As EventArgs)
        ' 把myImage1的Source属性改成下一张要显示的图片，nextImageIndex加1
        Me.myImage1.Source = images(nextImageIndex)
        nextImageIndex += 1

        ' 如果nextImageIndex值超过集合中图片最大索引值，将其置0使其下一次
        ' 展示第一张图片
        If nextImageIndex = images.Count Then
            nextImageIndex = 0
        End If

        ' 获取InvisibleToVisible故事板并开始动画
        Dim sb As Storyboard = TryCast(Me.FindResource("InvisibleToVisible"), Storyboard)

        sb.Begin(Me)
    End Sub

    Private Sub InvisibleToVisible_Completed(ByVal sender As Object, ByVal e As EventArgs)
        ' 把myImage2的Source属性改成下一张要显示的图片，nextImageIndex加1
        Me.myImage2.Source = images(nextImageIndex)
        nextImageIndex += 1

        ' 如果nextImageIndex值超过集合中图片最大索引值，将其置0使其下一次
        ' 展示第一张图片
        If nextImageIndex = images.Count Then
            nextImageIndex = 0
        End If

        ' 获取VisibleToInvisible故事板并开始动画
        Dim sb As Storyboard = TryCast(Me.FindResource("VisibleToInvisible"), Storyboard)
        sb.Begin(Me)
    End Sub

End Class