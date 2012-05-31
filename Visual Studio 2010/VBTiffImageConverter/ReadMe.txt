========================================================================================
       Windows 应用程序: VBTiffImageConverter 项目概述
========================================================================================

////////////////////////////////////////////////////////////////////////////////////////
摘要: 

该实例演示了如果实现JPEG图像与TIFF图像之间的相互转换.此示例还允许从所选的 JPEG 
图像创建一个多页 TIFF 图像.

TIFF (原本代表的是标签图像文件格式），是一种灵活的、 适应性强的文件格式，可用来
处理单个文件中的图像和数据，通过文件头标签（大小、定义、图像数据的安排、应用图
像压缩）来定义图像几何.例如，一个TIFF 文件可以是一个容器来容纳压缩 （有损）JPEG 
和（无损） PackBits 压缩图像.TIFF 文件也可以包括基于矢量的剪切路径(轮廓、裁剪部
分 、图像帧）.


////////////////////////////////////////////////////////////////////////////////////////
演示:

下列步骤一步步演示了TIFF图像转换器实例.

步骤 1: 在Visual Studio 2010 中生成并运行该实例解决方案.

步骤 2: 如果要创建多页 tiff 文件,选中"选中以创建多页 tiff （单个） 文件"复选框.

步骤 3: 将图像格式从 Jpeg 转换为 Tiff.
        
		步骤 3_1. 点击"转换为 Tiff"按钮来浏览 jpeg 图像.(支持多选）
		
		步骤 3_2. 选择 jpeg 图像后，请单击"确定"，将启动转换过程.

步骤 4: 将图像格式从 Jpeg 转换为 Tiff.

        步骤 4_1. 单击"转换为 Jpeg"按钮来浏览 tiff 图像.

		步骤 4_2. 选择 tiff 图像中后，请单击"确定",将启动转换过程.


/////////////////////////////////////////////////////////////////////////////////////////
实现:

A. 将 TIFF 转换为 JPEG.
(请参阅: TiffImageConverter.ConvertTiffToJpeg)

基本的代码逻辑，如下所示：

  1. 加载 TIFF 图像.
  2. 获取 TIFF 图像中帧的数目.
  3. 选择每一帧，并将其保存为新的 jpg 图像文件.

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

B. 将 JPEG(s) 转换为 TIFF.
(请参阅: TiffImageConverter.ConvertJpegToTiff)

基本的代码逻辑，如下所示：

  1. 如果用户选中"选中以创建多页 tiff （单个） 文件"复选框.

    1) 使用选定的第一个 jpeg 文件,初始化的多页 tiff 的第一帧.
    2) 从剩下的 jpeg 文件添加更多的帧. 
    3) 最后一帧时，刷新资源并关闭它.

  2.如果用户没有选中"选中以创建多页 tiff （单个） 文件"复选框.

    1) 加载的每个 jpeg 文件.
    2) 将它保存为单帧 tiff 文件.


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

                            ' 初始化多页 tiff 的第一帧.
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

/////////////////////////////////////////////////////////////////////////////////////////
参考:

Tagged Image File Format
http://en.wikipedia.org/wiki/Tagged_Image_File_Format


/////////////////////////////////////////////////////////////////////////////////////////