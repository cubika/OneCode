'****************************** Module Header ******************************\
' 模块头:  PreviewPage.xaml.vb
' 项目名:  VideoStoryCreator
' 版权 (c) Microsoft Corporation.
' 
' 此页面允许用户预览使用DispatcherTimer和Storyboard的story.
' 实际上并不编码视频的两个Story.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************/



Imports System.Windows.Threading
Imports System.IO
Imports System.Windows.Media.Imaging

<CLSCompliant(False)> _
Partial Public Class PreviewPage
    Inherits PhoneApplicationPage
    Private _dispatcherTimer As DispatcherTimer
    Private _currentImageIndex As Integer
    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub PhoneApplicationPage_Loaded(sender As Object, e As System.Windows.RoutedEventArgs)
        Dim mediaCount As Integer = App.MediaCollection.Count
        If mediaCount < 2 Then
            Throw New InvalidOperationException("您必须至少选择两个媒体文件")
        End If

        ' 设置前后台的图片..
        If App.MediaCollection(0).ResizedImage IsNot Nothing Then
            Me.foregroundImage.Source = App.MediaCollection(0).ResizedImage
        Else
            Dim bmp As WriteableBitmap = Me.GetResizedImage(App.MediaCollection(0))
            Me.foregroundImage.Source = bmp
        End If
        If App.MediaCollection(1).ResizedImage IsNot Nothing Then
            Me.backgroundImage.Source = App.MediaCollection(1).ResizedImage
        Else
            Dim bmp As WriteableBitmap = Me.GetResizedImage(App.MediaCollection(1))
            Me.backgroundImage.Source = bmp
        End If

        ' 设置_currentImageIndex为2, 下次我们可以从App.MediaCollection[2]开始.
        Me._currentImageIndex = 2

        Me._dispatcherTimer = New DispatcherTimer()
        Me._dispatcherTimer.Interval = App.MediaCollection(0).PhotoDuration
        AddHandler Me._dispatcherTimer.Tick, AddressOf DispatcherTimer_Tick
        Me._dispatcherTimer.Start()
    End Sub

    Private Sub DispatcherTimer_Tick(sender As Object, e As EventArgs)
        Me.backgroundImage.Opacity = 1
        Dim transition As ITransition = App.MediaCollection(Me._currentImageIndex - 1).Transition

        ' 显示transition若它不为空.
        ' 这个task被委托到transition类.
        If transition IsNot Nothing Then
            transition.ForegroundElement = Me.foregroundImage
            transition.BackgroundElement = Me.backgroundImage
            AddHandler transition.TransitionCompleted, AddressOf TransitionCompleted
            transition.Begin()
            Me._dispatcherTimer.[Stop]()
        Else
            ' 没有transition，从下张图片开始.
            Me._dispatcherTimer.[Stop]()
            Me.backgroundImage.SetValue(Canvas.ZIndexProperty, 2)
            Me.foregroundImage.SetValue(Canvas.ZIndexProperty, 0)
            Me.GoToNext()
        End If
    End Sub

    Private Sub TransitionCompleted(sender As Object, e As EventArgs)
        Dim transition As ITransition = DirectCast(sender, ITransition)

        ' 如果没有被transition修改，则修改z-index.
        If Not transition.ImageZIndexModified Then
            Me.backgroundImage.SetValue(Canvas.ZIndexProperty, 2)
            Me.foregroundImage.SetValue(Canvas.ZIndexProperty, 0)
        End If
        transition.[Stop]()

        Me.GoToNext()
    End Sub

    ''' <summary>
    ''' 显示下一张图片.
    ''' </summary>
    Private Sub GoToNext()
        ' 这样的逻辑是清楚的参考切换。
        ' 当传递给过渡类，前景图像，当前图像，
        ' 而背景图像显示的新形象，
        Dim tempImg As Image = Me.foregroundImage
        Me.foregroundImage = Me.backgroundImage
        Me.backgroundImage = tempImg

        ' 更多图片可用, 动画继续.
        If _currentImageIndex < App.MediaCollection.Count Then
            If App.MediaCollection(Me._currentImageIndex).ResizedImage IsNot Nothing Then
                Me.backgroundImage.Source = App.MediaCollection(Me._currentImageIndex).ResizedImage
            Else
                Dim bmp As WriteableBitmap = Me.GetResizedImage(App.MediaCollection(Me._currentImageIndex))
                Me.backgroundImage.Source = bmp
            End If

            Me._dispatcherTimer.Interval = App.MediaCollection(_currentImageIndex).PhotoDuration
            Me._currentImageIndex += 1
            Me._dispatcherTimer.Start()
        End If
    End Sub

    ''' <summary>
    ''' 从XNA媒体库中获取的原始图像。
    ''' 并调整其大小的目标分辨率。 
    ''' 调用BitmapHelper.GetResizedImage内部。
    ''' 不同的是此方法返回WriteableBitmap的，
    ''' 而BitmapHelper.GetResizedImage返回流。
    ''' 此方法还设置Photo.ResizedImage和Photo.ResizedImageStream。
    ''' </summary>
    ''' <param name="photo">图片需要被重设</param>
    ''' <returns>一个WriteableBitmap图片需要被重新设置.</returns>
    Private Function GetResizedImage(photo As Photo) As WriteableBitmap
        Dim resizedImageStream As Stream = BitmapHelper.GetResizedImage(photo.Name)
        Dim resizedImage As New WriteableBitmap(BitmapHelper.ResizedImageWidth, BitmapHelper.ResizedImageHeight)
        resizedImage.SetSource(resizedImageStream)
        photo.ResizedImageStream = resizedImageStream
        photo.ResizedImage = resizedImage
        Return resizedImage
    End Function
End Class
