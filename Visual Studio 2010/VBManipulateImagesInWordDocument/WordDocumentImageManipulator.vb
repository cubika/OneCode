'**************************** 模块头***********************************\
' 模块名:  WordDocumentImageManipulator.vb
' 项目名:      VBManipulateImagesInWordDocument
' 版权 (c) Microsoft Corporation.
'
' 这个 WordDocumentImageManipulator 类是用来在 Word 文档中
' 导出、删除、替换替换图片的
'
' 在文档中的图片数据储存为一个 ImagePart， 并且在 Drawing 元素中的 Blip 元素
' 会引用到这个 ImagePart。不同的 Blip 元素可能引用同一个 ImagePart。
' 
' 通过编辑 Blip/Drawing  元素，我们可以删除/替换在文档中的图片。
' 
'
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
'
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**************************************************************************/

Imports System.IO
Imports System.Linq
Imports DocumentFormat.OpenXml
Imports DocumentFormat.OpenXml.Drawing
Imports DocumentFormat.OpenXml.Packaging
Imports DocumentFormat.OpenXml.Wordprocessing

Public Class WordDocumentImageManipulator
    Implements IDisposable
    Private disposed As Boolean = False

    ' 当图片替换或者删除的时候发生
    Public Event ImagesChanged As EventHandler

    ' WordprocessingDocument 实例
    Private privateDocument As WordprocessingDocument
    Public Property Document() As WordprocessingDocument
        Get
            Return privateDocument
        End Get
        Private Set(ByVal value As WordprocessingDocument)
            privateDocument = value
        End Set
    End Property

    ''' <summary>
    ''' 初始化 WordDocumentImageManipulator 实例
    ''' </summary>
    ''' <param name="path">
    ''' 文档的路径
    ''' </param>
    Public Sub New(ByVal path As FileInfo)

        ' 以可编辑的形式打开 Word 文档
        Document = WordprocessingDocument.Open(path.FullName, True)

    End Sub

    ''' <summary>
    ''' 获取文档中的所有图片
    ''' 文档中的图片储存为 Blip 元素
    ''' </summary>
    ''' <returns></returns>
    Public Function GetAllImages() As IEnumerable(Of Blip)

        ' 获取文档中的 Drawing 元素
        Dim drawingElements = From run In Document.MainDocumentPart.Document.
                                  Descendants(Of DocumentFormat.OpenXml.Wordprocessing.Run)()
                              Where run.Descendants(Of Drawing)().Count() <> 0
                              Select run.Descendants(Of Drawing)().First()

        ' 获取Drawing 元素中的 Blip 元素
        Dim blipElements = From drawing In drawingElements
                           Where drawing.Descendants(Of Blip)().Count() > 0
                           Select drawing.Descendants(Of Blip)().First()

        Return blipElements
    End Function

    ''' <summary>
    ''' 从 Blip 元素中得到图片
    ''' </summary>
    Public Function GetImageInBlip(ByVal blipElement As Blip) As Image

        ' 获取被 Blip 元素引用的 ImagePart 
        Dim imagePart = TryCast(
            Document.MainDocumentPart.GetPartById(blipElement.Embed.Value), 
            ImagePart)

        If imagePart IsNot Nothing Then
            Using imageStream As Stream = imagePart.GetStream()
                Dim img As New Bitmap(imageStream)
                Return img
            End Using
        Else
            Throw New ApplicationException("不能发现图片部分：" _
                                           & blipElement.Embed.Value)
        End If
    End Function

    ''' <summary>
    ''' 把含有 Blip 元素的 Drawing 元素删除
    ''' </summary>
    ''' <param name="blipElement"></param>
    Public Sub DeleteImage(ByVal blipElement As Blip)
        Dim parent As OpenXmlElement = blipElement.Parent
        Do While parent IsNot Nothing _
            AndAlso Not (TypeOf parent Is DocumentFormat.OpenXml.Wordprocessing.Drawing)
            parent = parent.Parent
        Loop

        If parent IsNot Nothing Then
            Dim drawing_Renamed As Drawing = TryCast(parent, Drawing)
            drawing_Renamed.Parent.RemoveChild(Of Drawing)(drawing_Renamed)

            ' 触发  ImagesChanged 事件.
            Me.OnImagesChanged()

        End If
    End Sub

    ''' <summary>
    ''' 如果想替换文档中的图片
    ''' 1. 添加一个 ImagePart 到文档中
    ''' 2. 编辑 Blip 元素使它引用新的 ImagePart.
    ''' </summary>
    ''' <param name="blipElement"></param>
    ''' <param name="newImg"></param>
    Public Sub ReplaceImage(ByVal blipElement As Blip, ByVal newImg As FileInfo)
        Dim rid As String = AddImagePart(newImg)
        blipElement.Embed.Value = rid
        Me.OnImagesChanged()
    End Sub

    ''' <summary>
    ''' 添加一个 ImagePart 到文档中
    ''' </summary>
    Private Function AddImagePart(ByVal newImg As FileInfo) As String
        Dim type As ImagePartType = ImagePartType.Bmp
        Select Case newImg.Extension.ToLower()
            Case ".jpeg", ".jpg"
                type = ImagePartType.Jpeg
            Case ".png"
                type = ImagePartType.Png
            Case Else
                type = ImagePartType.Bmp
        End Select

        Dim newImgPart As ImagePart = Document.MainDocumentPart.AddImagePart(type)
        Using stream As FileStream = newImg.OpenRead()
            newImgPart.FeedData(stream)
        End Using

        Dim rId As String = Document.MainDocumentPart.GetIdOfPart(newImgPart)
        Return rId
    End Function


    ''' <summary>
    '''  触发 ImagesChanged 事件
    ''' </summary>
    Protected Overridable Sub OnImagesChanged()
        RaiseEvent ImagesChanged(Me, EventArgs.Empty)
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub

    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        ' 防止被多次调用
        If disposed Then
            Return
        End If

        If disposing Then
            If Document IsNot Nothing Then
                Document.Dispose()
            End If
            disposed = True
        End If
    End Sub
End Class
