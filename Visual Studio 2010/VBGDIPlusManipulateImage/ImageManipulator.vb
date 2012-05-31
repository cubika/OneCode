'*************************** 模块头 ******************************'
' 模块名:  ImageManipulator.vb
' 项目名:	  VBGDIPlusManipulateImage
' 版权 (c) Microsoft Corporation.
' 
' 类 ImageManipulator 使用 GDI+ 来操作图像. 它提供了公共方法来拉伸, 旋转, 翻转以及
' 倾斜图像, 支持任意角度旋转. 这个类同时提供方法在 System.Windows.Forms.Control 的
' 子类对象上绘制图像.
' 
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*************************************************************************'

Imports System.Drawing.Drawing2D

Public Class ImageManipulator
    Implements IDisposable

    ' 指示资源是否已经被释放.
    Private _disposed As Boolean = False

    ''' <summary>
    '''  需处理的图像.
    ''' </summary>
    Private _image As Bitmap
    Public Property Image() As Bitmap
        Get
            Return _image
        End Get
        Private Set(ByVal value As Bitmap)
            _image = value
        End Set
    End Property

    ''' <summary>
    ''' 图像的实际大小. 当旋转, 倾斜图像的时候, 图像的实际大小是不变的.
    ''' </summary>
    Private _contentSize As Size
    Public Property ContentSize() As Size
        Get
            Return _contentSize
        End Get
        Private Set(ByVal value As Size)
            _contentSize = value
        End Set
    End Property

    ''' <summary>
    ''' 已旋转的角度. 0~360.
    ''' </summary>
    Private _rotatedAngle As Single
    Public Property RotatedAngle() As Single
        Get
            Return _rotatedAngle
        End Get
        Private Set(ByVal value As Single)
            _rotatedAngle = value
        End Set
    End Property

    ''' <summary>
    ''' 已倾斜的单位.
    ''' </summary>
    Private _skewedUnit As Integer
    Public Property SkewedUnit() As Integer
        Get
            Return _skewedUnit
        End Get
        Private Set(ByVal value As Integer)
            _skewedUnit = value
        End Set
    End Property

    Public Sub New(ByVal img As Bitmap)
        If img Is Nothing Then
            Throw New ArgumentNullException("图像不能为空.")
        End If

        ' 初始化属性值.
        Me.Image = img
        Me.ContentSize = img.Size
        RotatedAngle = 0
    End Sub

    ''' <summary>
    ''' 重新设置图像的大小.
    ''' 这个方法使用指定的尺寸创建一个新的Bitmap实例.
    ''' </summary>
    Public Sub ResizeImage(ByVal newsize As Size)
        Dim newImg As Bitmap = Nothing
        Try

            ' 使用指定的尺寸创建一个新的Bitmap实例.
            newImg = New Bitmap(Image, newsize)

            ' 释放原始图像.
            Me.Image.Dispose()

            ' 设置 this.Image 为新的位图.
            Me.Image = newImg

            Me.ContentSize = Me.Image.Size
        Catch

            ' 如果有任何异常, 释放新位图.
            If newImg IsNot Nothing Then
                newImg.Dispose()
            End If

            Throw
        End Try
    End Sub

    ''' <summary>
    ''' 使用指定的系数拉伸图像.
    ''' 步骤:
    ''' 1 计算新的尺寸.
    ''' 2 使用指定系数初始化一个矩阵. 这个矩阵用于图像的转换.
    ''' 3 将原始图片绘制到新位图上, 然后转化就会生效.
    ''' </summary>
    Public Sub Scale(ByVal xFactor As Single, ByVal yFactor As Single)
        Dim newImg As Bitmap = Nothing
        Dim g As Graphics = Nothing
        Dim mtrx As Matrix = Nothing

        Try
            ' 使用指定的尺寸创建一个新的Bitmap实例.
            newImg = New Bitmap(Convert.ToInt32(Me.Image.Size.Width * xFactor),
                                Convert.ToInt32(Me.Image.Size.Height * yFactor))

            ' 从位图获得Graphics对象.
            g = Graphics.FromImage(newImg)

            ' 设置 Interpolation Mode. 
            g.InterpolationMode = InterpolationMode.HighQualityBilinear

            ' 使用指定系数初始化一个矩阵. 这个矩阵用于图像的转换.
            mtrx = New Matrix(xFactor, 0, 0, yFactor, 0, 0)

            ' 使用矩阵转换图像.
            g.MultiplyTransform(mtrx, MatrixOrder.Append)

            ' 将原始图片绘制到新位图上, 然后转化就会生效.
            g.DrawImage(Me.Image, 0, 0)

            ' 释放原始图像.
            Me.Image.Dispose()

            ' 设置 this.Image 为新的位图.
            Me.Image = newImg

            Me.ContentSize = Me.Image.Size
        Catch

            ' 如果有任何异常, 释放新位图.
            If newImg IsNot Nothing Then
                newImg.Dispose()
            End If

            Throw
        Finally

            ' 释放 Graphics 和 Matrix 对象.
            If g IsNot Nothing Then
                g.Dispose()
            End If

            If mtrx IsNot Nothing Then
                mtrx.Dispose()
            End If
        End Try
    End Sub

    ''' <summary>
    ''' 选择, 翻转图像.
    ''' </summary>
    ''' <param name="type">
    '''  一个 System.Drawing.RotateFlipType 的成员指示旋转或翻转的类型.
    ''' </param>
    Public Sub RotateFlip(ByVal type As RotateFlipType)
        ' 选择, 翻转图像.
        Me.Image.RotateFlip(type)

        ' 计算旋转的角度. 
        Select Case type
            ' Rotate180FlipXY 等价于 RotateNoneFlipNone;
            Case RotateFlipType.RotateNoneFlipNone

                ' Rotate270FlipXY 等价于 Rotate90FlipNone;
            Case RotateFlipType.Rotate90FlipNone
                Me.RotatedAngle += 90

                ' RotateNoneFlipXY 等价于 Rotate180FlipNone;
            Case RotateFlipType.Rotate180FlipNone
                Me.RotatedAngle += 180

                ' Rotate90FlipXY 等价于 Rotate270FlipNone;
            Case RotateFlipType.Rotate270FlipNone
                Me.RotatedAngle += 270

                ' Rotate180FlipY 等价于 RotateNoneFlipX;
            Case RotateFlipType.RotateNoneFlipX
                Me.RotatedAngle = 180 - Me.RotatedAngle

                ' Rotate270FlipY 等价于 Rotate90FlipX;
            Case RotateFlipType.Rotate90FlipX
                Me.RotatedAngle = 90 - Me.RotatedAngle

                ' Rotate180FlipX 等价于 RotateNoneFlipY;
            Case RotateFlipType.RotateNoneFlipY
                Me.RotatedAngle = 360 - Me.RotatedAngle

                ' Rotate270FlipX 等价于 Rotate90FlipY;
            Case RotateFlipType.Rotate90FlipY
                Me.RotatedAngle = 270 - Me.RotatedAngle

            Case Else
        End Select

        ' 旋转的角度是0~360度.
        If RotatedAngle >= 360 Then
            RotatedAngle -= 360
        End If
        If RotatedAngle < 0 Then
            RotatedAngle += 360
        End If
    End Sub

    ''' <summary>
    ''' 旋转图像至任意角度.
    ''' 步骤:
    ''' 1 计算实际需要的大小.
    ''' 2 将图像的中心点移至(0,0).
    ''' 3 旋转指定角度.
    ''' 4 将图像的中心点移至新图像的中心.
    ''' </summary>
    Public Sub RotateImg(ByVal angle As Single)
        Dim necessaryRectangle As RectangleF = RectangleF.Empty

        ' 计算实际需要的大小.
        Using path As New GraphicsPath()
            path.AddRectangle(
                New RectangleF(0.0F, 0.0F, ContentSize.Width, ContentSize.Height))
            Using mtrx As New Matrix()
                Dim totalAngle As Single = angle + Me.RotatedAngle
                Do While totalAngle >= 360
                    totalAngle -= 360
                Loop
                Me.RotatedAngle = totalAngle
                mtrx.Rotate(RotatedAngle)

                ' 实际需要的大小.
                necessaryRectangle = path.GetBounds(mtrx)
            End Using
        End Using

        Dim newImg As Bitmap = Nothing
        Dim g As Graphics = Nothing

        Try

            ' 使用指定的尺寸创建一个新的Bitmap实例.
            newImg = New Bitmap(Convert.ToInt32(necessaryRectangle.Width),
                                Convert.ToInt32(necessaryRectangle.Height))

            ' 从位图获得Graphics对象.
            g = Graphics.FromImage(newImg)

            ' 将图像的中心点移至(0,0).
            g.TranslateTransform(-Me.Image.Width \ 2, -Me.Image.Height \ 2)

            ' 旋转指定角度.
            g.RotateTransform(angle, MatrixOrder.Append)

            ' 将图像的中心点移至新图像的中心.
            g.TranslateTransform(newImg.Width \ 2, newImg.Height \ 2, MatrixOrder.Append)

            g.InterpolationMode = InterpolationMode.HighQualityBicubic

            g.DrawImage(Me.Image, 0, 0)

            ' 释放原始图像.
            Me.Image.Dispose()

            ' 设置 this.Image 为新的位图.
            Me.Image = newImg

        Catch

            ' 如果有任何异常, 释放新位图.
            If newImg IsNot Nothing Then
                newImg.Dispose()
            End If

            Throw
        Finally

            ' Dispose the Graphics
            If g IsNot Nothing Then
                g.Dispose()
            End If

        End Try
    End Sub


    ''' <summary>
    ''' 通过指定原始图像的左上角、右上角和左下角的目标点可旋转、反射和扭曲图像。 这三个目标点确定
    ''' 将原始矩形图像映射为平行四边形的仿射变换。
    ''' 例如, 原始的图像所在的矩形是 {[0,0], [100,0], [100,50],[50, 0]}, 倾斜 -10个单元后,
    ''' 结果是 {[-10,0], [90,0], [100,50],[50, 0]}. 
    ''' </summary>
    ''' <param name="unit">
    ''' 倾斜的单元.
    ''' </param>
    Public Sub Skew(ByVal unit As Integer)
        Dim necessaryRectangle As RectangleF = RectangleF.Empty
        Dim totalUnit As Integer = 0

        ' 计算实际需要的大小.
        Using path As New GraphicsPath()
            Dim newPoints() As Point = Nothing

            totalUnit = SkewedUnit + unit

            newPoints = New Point() {New Point(totalUnit, 0), New Point(totalUnit + Me.ContentSize.Width, 0), New Point(Me.ContentSize.Width, Me.ContentSize.Height), New Point(0, Me.ContentSize.Height)}
            path.AddLines(newPoints)
            necessaryRectangle = path.GetBounds()
        End Using


        Dim newImg As Bitmap = Nothing
        Dim g As Graphics = Nothing

        Try

            ' 使用指定的尺寸创建一个新的Bitmap实例.
            newImg = New Bitmap(Convert.ToInt32(necessaryRectangle.Width), Convert.ToInt32(necessaryRectangle.Height))

            ' 从位图获得Graphics对象.
            g = Graphics.FromImage(newImg)

            ' 移动图像.
            If totalUnit <= 0 AndAlso SkewedUnit <= 0 Then
                g.TranslateTransform(-unit, 0, MatrixOrder.Append)
            End If

            g.InterpolationMode = InterpolationMode.HighQualityBilinear

            Dim destinationPoints() As Point = {New Point(unit, 0), New Point(unit + Me.Image.Width, 0), New Point(0, Me.Image.Height)}

            g.DrawImage(Me.Image, destinationPoints)

            ' 释放原始图像.
            Me.Image.Dispose()

            ' 设置 this.Image 为新的位图.
            Me.Image = newImg

            SkewedUnit = totalUnit
        Catch

            ' 如果有任何异常, 释放新位图.
            If newImg IsNot Nothing Then
                newImg.Dispose()
            End If

            Throw
        Finally

            ' 释放 Graphics 对象
            If g IsNot Nothing Then
                g.Dispose()
            End If

        End Try

    End Sub

    ''' <summary>
    '''将图像绘制在控件上.
    ''' </summary>
    Public Sub DrawControl(ByVal control_Renamed As Control,
                           ByVal adjust As Point, ByVal penToDrawBounds As Pen)

        ' 为控件创建Graphics对象.
        Dim graphicsForPanel As Graphics = control_Renamed.CreateGraphics()

        ' 清理控件的图像, 并设置背景颜色.
        graphicsForPanel.Clear(SystemColors.ControlDark)

        ' 将图像绘制在控件的中心.
        Dim p As New Point(Convert.ToInt32((control_Renamed.Width - Me.Image.Size.Width) / 2),
                           Convert.ToInt32((control_Renamed.Height - Me.Image.Size.Height) / 2))

        ' 调整位置.
        graphicsForPanel.TranslateTransform(adjust.X, adjust.Y)

        graphicsForPanel.DrawImage(Me.Image, p)

        ' 绘制边框.
        If penToDrawBounds IsNot Nothing Then

            Dim unit = GraphicsUnit.Pixel
            Dim rec = Me.Image.GetBounds(unit)

            graphicsForPanel.DrawRectangle(penToDrawBounds,
                                           rec.X + p.X,
                                           rec.Y + p.Y,
                                           rec.Width,
                                           rec.Height)
            graphicsForPanel.DrawLine(penToDrawBounds,
                                      rec.X + p.X,
                                      rec.Y + p.Y,
                                      rec.X + p.X + rec.Width,
                                      rec.Y + p.Y + rec.Height)

            graphicsForPanel.DrawLine(penToDrawBounds,
                                      rec.X + p.X + rec.Width,
                                      rec.Y + p.Y,
                                      rec.X + p.X,
                                      rec.Y + p.Y + rec.Height)
        End If
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        Me.Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub

    Protected Overridable Sub Dispose(ByVal disposing As Boolean)

        If _disposed Then
            Return
        End If

        If disposing Then
            ' 释放托管资源.
            If Me.Image IsNot Nothing Then
                Me.Image.Dispose()
            End If

        End If
        _disposed = True
    End Sub
End Class
