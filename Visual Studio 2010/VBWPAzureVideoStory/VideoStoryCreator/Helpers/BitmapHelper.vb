'********************************* 模块头 *********************************\
'模块名: BitmapHelper.vb
' 项目名: VideoStoryCreator
' 版权(c) Microsoft Corporation.
' 
' 重定义图片的大小的helper类.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************/




Imports System.IO
Imports Microsoft.Xna.Framework.Media
Imports System.Windows.Media.Imaging

Public NotInheritable Class BitmapHelper
    ' 目前我们使用硬编码.
    ' 以后我们会支持用户定义.
    ' TODO: 将所有的硬编码值移到定义的地方.
    Friend Shared ResizedImageWidth As Integer = 800
    Friend Shared ResizedImageHeight As Integer = 600

    ''' <summary>
    ''' 从XNA媒体库中取到图片.
    ''' 并且重新定义到目前大小
    ''' </summary>
    ''' <param name="name">图片名称</param>
    ''' <returns>通过stream定义图片大小, jpeg格式.</returns>
    Public Shared Function GetResizedImage(name As String) As Stream
        Dim mediaLibrary As New MediaLibrary()
        Dim pictureCollection As PictureCollection = mediaLibrary.Pictures

        Dim picture As Picture = pictureCollection.Where(Function(p) p.Name = name).FirstOrDefault()
        If picture Is Nothing Then
            Throw New InvalidOperationException(String.Format("不能加载图片 {0}. 图片可能被删除", name))
        End If
        Dim originalImageStream As Stream = picture.GetImage()
        Dim bmp As New BitmapImage()
        bmp.SetSource(originalImageStream)
        Dim originalImage As New WriteableBitmap(bmp)
        Dim targetStream As New MemoryStream()
        originalImage.SaveJpeg(targetStream, ResizedImageWidth, ResizedImageHeight, 0, 100)

        ' 现在图片被移到WriteableBitmap类, 原图片的stream被关闭.
        originalImageStream.Close()

        targetStream.Position = 0
        Return targetStream
    End Function
End Class
