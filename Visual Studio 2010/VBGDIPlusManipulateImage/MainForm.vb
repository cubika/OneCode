'****************************** 模块头 ******************************'
' 模块名:  MainForm.cs
' 模块名:	  VBGDIPlusManipulateImage
' 版权(c)  Microsoft Corporation.
' 
' 这是程序的主窗体, 用来初始化用户界面以及处理事件.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**************************************************************************

Imports System.Drawing.Drawing2D

Partial Public Class MainForm
    Inherits Form
    Private _pen As Pen = Nothing

    Private _imgManipulator As ImageManipulator = Nothing

    Private _adjustment As Point = Point.Empty

    Public Sub New()
        InitializeComponent()

        ' 从本地文件加载位图.
        Dim img As New Bitmap("OneCodeIcon.png")

        ' 初始化 imgManipulator.
        _imgManipulator = New ImageManipulator(img)

        ' 向下拉框中添加所有的InterpolationMode.
        For i As Integer = 0 To 7
            cmbInterpolationMode.Items.Add(CType(i, InterpolationMode))
        Next i

        cmbInterpolationMode.SelectedIndex = 0
    End Sub

    ''' <summary>
    ''' 处理按钮 btnRotateLeft, btnRotateRight, btnFlipVertical 和 
    ''' btnFlipHorizontal 的点击事件.
    ''' </summary>
    Private Sub btnRotateFlip_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles btnFlipVertical.Click,
        btnRotateRight.Click,
        btnFlipHorizontal.Click,
        btnRotateLeft.Click

        Dim rotateFlipButton As Button = TryCast(sender, Button)

        If rotateFlipButton Is Nothing Then
            Return
        End If

        Dim rotateFlipType_Renamed As RotateFlipType = RotateFlipType.RotateNoneFlipNone

        Select Case rotateFlipButton.Name
            Case "btnRotateLeft"
                rotateFlipType_Renamed = RotateFlipType.Rotate270FlipNone
            Case "btnRotateRight"
                rotateFlipType_Renamed = RotateFlipType.Rotate90FlipNone
            Case "btnFlipVertical"
                rotateFlipType_Renamed = RotateFlipType.RotateNoneFlipY
            Case "btnFlipHorizontal"
                rotateFlipType_Renamed = RotateFlipType.RotateNoneFlipX
        End Select

        ' 旋转或翻转图片.
        _imgManipulator.RotateFlip(rotateFlipType_Renamed)

        ' 重绘 pnlImage.
        _imgManipulator.DrawControl(Me.pnlImage, _adjustment, _pen)
    End Sub

    ''' <summary>
    ''' 处理按钮 btnRotateAngle 的点击事件.
    ''' </summary>
    Private Sub btnRotateAngle_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles btnRotateAngle.Click

        Dim angle As Single = 0

        ' 验证输入值.
        Single.TryParse(tbRotateAngle.Text, angle)

        If angle > 0 AndAlso angle < 360 Then
            ' 旋转图像.
            _imgManipulator.RotateImg(angle)

            ' 重绘 pnlImage.
            _imgManipulator.DrawControl(Me.pnlImage, _adjustment, _pen)

        End If
    End Sub

    ''' <summary>
    ''' 处理按钮 btnMoveUp, btnMoveDown, btnMoveLeft 和 btnMoveRight
    ''' 的点击事件.
    ''' </summary>
    Private Sub btnMove_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles btnMoveLeft.Click,
        btnMoveUp.Click,
        btnMoveDown.Click,
        btnMoveRight.Click

        Dim moveButton As Button = TryCast(sender, Button)
        If moveButton Is Nothing Then
            Return
        End If

        Dim x As Integer = 0
        Dim y As Integer = 0

        Select Case moveButton.Name
            Case "btnMoveUp"
                y = -10
            Case "btnMoveDown"
                y = 10
            Case "btnMoveLeft"
                x = -10
            Case "btnMoveRight"
                x = 10
        End Select
        _adjustment = New Point(_adjustment.X + x, _adjustment.Y + y)

        ' 重绘 pnlImage.
        _imgManipulator.DrawControl(Me.pnlImage, _adjustment, _pen)

    End Sub


    Private Sub pnlImage_Paint(ByVal sender As Object, ByVal e As PaintEventArgs) _
        Handles pnlImage.Paint

        ' 首次绘制 pnlImage.
        _imgManipulator.DrawControl(Me.pnlImage, _adjustment, _pen)

    End Sub

    ''' <summary>
    ''' 处理按钮 btnAmplify 和 btnMicrify 的点击事件.
    ''' </summary>
    Private Sub btnAmplify_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles btnAmplify.Click, btnMicrify.Click

        Dim btnScale As Button = TryCast(sender, Button)
        If btnScale.Name = "btnAmplify" Then
            _imgManipulator.Scale(2, 2)
        Else
            _imgManipulator.Scale(0.5F, 0.5F)
        End If

        ' 重绘 pnlImage.
        _imgManipulator.DrawControl(Me.pnlImage, _adjustment, _pen)
    End Sub

    ''' <summary>
    ''' 处理按钮 btnSkewLeft 和 btnSkewRight 的点击事件.
    ''' </summary>
    Private Sub btnSkew_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSkewRight.Click, btnSkewLeft.Click
        Dim btnSkew As Button = TryCast(sender, Button)

        Select Case btnSkew.Name
            Case "btnSkewLeft"
                _imgManipulator.Skew(-10)
            Case "btnSkewRight"
                _imgManipulator.Skew(10)
        End Select

        ' 重绘 pnlImage.
        _imgManipulator.DrawControl(Me.pnlImage, _adjustment, _pen)
    End Sub


    ''' <summary>
    ''' 重置图像.
    ''' </summary>
    Private Sub btnReset_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles btnReset.Click

        _imgManipulator.Dispose()

        ' 从本地文件加载位图.
        Dim img As New Bitmap("OneCodeIcon.png")

        ' 初始化 imgManipulator.
        _imgManipulator = New ImageManipulator(img)

        ' 重绘 pnlImage.
        _imgManipulator.DrawControl(Me.pnlImage, _adjustment, _pen)

    End Sub

    ''' <summary>
    ''' 处理 chkDrawBounds 的点击事件. 
    ''' </summary>
    Private Sub chkDrawBounds_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) _
        Handles chkDrawBounds.CheckedChanged

        ' 如果 pen 不为空, 则绘制图像的边框.
        If chkDrawBounds.Checked Then
            _pen = Pens.Blue
        Else
            _pen = Nothing
        End If

        ' 重绘 pnlImage.
        _imgManipulator.DrawControl(Me.pnlImage, _adjustment, _pen)
    End Sub
End Class
