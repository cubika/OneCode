'******************************** 模块头 **********************************\ 
' 模块名：  TiffImageConverter.vb 
' 项目名：  VBTiffImageConverter
' 版权 (c) Microsoft Corporation. 
'
' 该类定义了用于 TIFF 文件和 JPEG 文件(s)相互转换的辅助方法.
'
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
'
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE. 
'**************************************************************************/


Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.IO


Public Class TiffImageConverter
    ''' <summary>
    ''' 将 jpeg 图像(s)转换为 tiff 图像(s).
    ''' </summary>
    ''' <param name="fileName">Tiff 图像的完整名称.</param>
    ''' <returns>存放 jpeg 图像完整名称的字符串数组.</returns>
    Public Shared Function ConvertTiffToJpeg(fileName As String) As String()
        Using imageFile As Image = Image.FromFile(fileName)
            Dim frameDimensions As New FrameDimension(imageFile.FrameDimensionsList(0))

            ' 获取 TIFF 图像中帧的数目（如果为多页）.
            Dim frameNum As Integer = imageFile.GetFrameCount(frameDimensions)
            Dim jpegPaths As String() = New String(frameNum) {}

            Dim frame As Integer = 0
            While frame < frameNum
                ' 每次选择一个帧，并将另存为 jpeg 图像.
                imageFile.SelectActiveFrame(frameDimensions, frame)
                Using bmp As New Bitmap(imageFile)
                    jpegPaths(frame) = [String].Format("{0}\{1}{2}.jpeg",
                                       Path.GetDirectoryName(fileName),
                                       Path.GetFileNameWithoutExtension(fileName), frame)
                    bmp.Save(jpegPaths(frame), ImageFormat.Jpeg)
                End Using
                System.Math.Max(System.Threading.Interlocked.Increment(frame), frame - 1)
            End While

            Return jpegPaths
        End Using
    End Function

    ''' <summary>
    ''' 将 jpeg 图像(s)转换为 tiff 图像(s).
    ''' </summary>
    ''' <param name="fileNames">
    ''' 存有 jpeg 图像完整名称的字符串数组.
    ''' </param>
    ''' <param name="isMultipage">
    ''' 可以创造单个的多页 tiff 文件,为 true；否则,为 false.
    ''' </param>
    ''' <returns>
    ''' 存有 tiff 图像完整名称的字符串数组
    ''' </returns>
    Public Shared Function ConvertJpegToTiff(fileNames As String(),
                                             isMultipage As Boolean) As String()
        Using encoderParams As New EncoderParameters(1)
            Dim tiffCodecInfo As ImageCodecInfo = ImageCodecInfo.GetImageEncoders(). _
                First(Function(ie) ie.MimeType = "image/tiff")

            Dim tiffPaths As String() = Nothing
            If isMultipage Then
                tiffPaths = New String(1) {}
                Dim tiffImg As Image = Nothing
                Try
                    Dim i As Integer = 0
                    While i < fileNames.Length
                        If i = 0 Then
                            tiffPaths(i) = [String].Format("{0}\{1}.tiff",
                                        Path.GetDirectoryName(fileNames(i)),
                                        Path.GetFileNameWithoutExtension(fileNames(i)))

                            ' 初始化多页 tiff 图像的第一帧.
                            tiffImg = Image.FromFile(fileNames(i))
                            encoderParams.Param(0) = New EncoderParameter(
                                Encoder.SaveFlag, DirectCast(EncoderValue.MultiFrame, 
                                EncoderParameterValueType))
                            tiffImg.Save(tiffPaths(i), tiffCodecInfo, encoderParams)
                        Else
                            ' 添加其他帧.
                            encoderParams.Param(0) = New EncoderParameter(
                                Encoder.SaveFlag,
                                DirectCast(EncoderValue.FrameDimensionPage, 
                                EncoderParameterValueType))
                            Using frame As Image = Image.FromFile(fileNames(i))
                                tiffImg.SaveAdd(frame, encoderParams)
                            End Using
                        End If

                        If i = fileNames.Length - 1 Then
                            ' 最后一帧时，刷新资源并关闭它.
                            encoderParams.Param(0) = New EncoderParameter(
                                Encoder.SaveFlag,
                                DirectCast(EncoderValue.Flush, EncoderParameterValueType))
                            tiffImg.SaveAdd(encoderParams)
                        End If
                        System.Math.Max(System.Threading.Interlocked.Increment(i), i - 1)
                    End While
                Finally
                    If tiffImg IsNot Nothing Then
                        tiffImg.Dispose()
                        tiffImg = Nothing
                    End If
                End Try
            Else
                tiffPaths = New String(fileNames.Length) {}

                Dim i As Integer = 0
                While i < fileNames.Length
                    tiffPaths(i) = [String].Format("{0}\{1}.tiff",
                                       Path.GetDirectoryName(fileNames(i)),
                                       Path.GetFileNameWithoutExtension(fileNames(i)))

                    ' 存为单个的tiff文件.
                    Using tiffImg As Image = Image.FromFile(fileNames(i))
                        tiffImg.Save(tiffPaths(i), ImageFormat.Tiff)
                    End Using
                    System.Math.Max(System.Threading.Interlocked.Increment(i), i - 1)
                End While
            End If

            Return tiffPaths
        End Using
    End Function
End Class