'******************************** Module Header ***********************************'
' 模块名:  WPFIrregularShapeWindow.xaml.vb
' 项目名:      VBWPFIrregularShapeWindow
' Copyright (c) Microsoft Corporation.
'
' WPFIrregularShapeWindow.xaml.vb 文件定义了一个继承自 Window 类 的 MainWindow 子类，其职责是
' 显示5个不规则窗体.
'
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
'
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER 
' EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF 
' MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**********************************************************************************'

Imports System.Text

''' <summary>
''' MainWindow.xaml的交互逻辑
''' </summary>
Partial Public Class WPFIrregularShapeWindow
    Inherits Window
    Public AdditionHeight As Integer
    Public AdditionWidth As Integer

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub Window_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)

        ' 为子窗体的宽和高生成随机数目的是避免子窗体互相重叠.
        Dim ro As New Random()
        AdditionHeight = ro.Next(500)
        AdditionWidth = ro.Next(500)
        Dim btn As Button = TryCast(e.Source, Button)
        If btn IsNot Nothing Then
            Select Case TryCast(btn.Tag, String)
                Case "Ellipse Window"
                    Dim ellipseWindow_Renamed As New EllipseWindow()

                    ' 显示椭圆子窗体
                    ellipseWindow_Renamed.Left = Me.Left + Me.Width + AdditionWidth
                    ellipseWindow_Renamed.Top = Me.Top + Me.Height + AdditionHeight

                    ellipseWindow_Renamed.Show()
                Case "Rounded Corners Window"
                    Dim roundedCornersWindow_Renamed As New RoundedCornersWindow()

                    ' 显示圆角子窗体
                    roundedCornersWindow_Renamed.Left = Me.Left + Me.Width + AdditionWidth
                    roundedCornersWindow_Renamed.Top = Me.Top + Me.Height + AdditionHeight

                    roundedCornersWindow_Renamed.Show()
                Case "Triangle Corners Window"
                    Dim triangleCornersWindow_Renamed As New TriangleCornersWindow()

                    ' 显示三角形子窗体
                    triangleCornersWindow_Renamed.Left = Me.Left + Me.Width + AdditionWidth
                    triangleCornersWindow_Renamed.Top = Me.Top + AdditionHeight

                    triangleCornersWindow_Renamed.Show()
                Case "Popup Corners Window"
                    Dim popupCornersWindow_Renamed As New PopupCornersWindow()

                    ' 显示对话子窗体
                    popupCornersWindow_Renamed.Left = Me.Left + AdditionWidth
                    popupCornersWindow_Renamed.Top = Me.Top + AdditionHeight

                    popupCornersWindow_Renamed.Show()
                Case "Picture Based Windows"
                    Dim picturebasedWindows As New PictureBasedWindow()

                    ' 显示图片背景子窗体
                    picturebasedWindows.Left = Me.Left + AdditionWidth
                    picturebasedWindows.Top = Me.Top + AdditionHeight

                    picturebasedWindows.Show()

                Case Else
            End Select
        End If
    End Sub


End Class

