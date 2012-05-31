'******************************** 模块头 **********************************\ 
' 模块名：  MainForm.vb 
' 项目名：  VBTiffImageConverter
' 版权 (c)) Microsoft Corporation. 
'
' 该实例演示了如果实现JPEG图像与TIFF图像之间的相互转换. 此示例还允许从所选的 
' JPEG图像创建一个多页 TIFF 图像.
'
' TIFF (原本代表的是标签图像文件格式），是一种灵活的、 适应性强的文件格式，可
' 用来处理单个文件中的图像和数据，通过文件头标签（大小、定义、图像数据的安排、
' 应用图像压缩）来定义图像几何.例如，一个TIFF 文件可以是一个容器来容纳压缩（有
' 损）JPEG 和（无损） PackBits 压缩图像.TIFF 文件也可以包括基于矢量的剪切路径
' (轮廓、裁剪部分 、图像帧）. 
'
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
'
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE. 
'**************************************************************************/

Imports System
Imports System.Windows.Forms


Public Class MainForm

    Private Sub btnConvertToTiff_Click(sender As System.Object, e As System.EventArgs) Handles btnConvertToTiff.Click


        dlgOpenFileDialog.Multiselect = True
        dlgOpenFileDialog.Filter = "Image files (.jpg, .jpeg)|*.jpg;*.jpeg"

        If dlgOpenFileDialog.ShowDialog() = DialogResult.OK Then
            Try
                TiffImageConverter.ConvertJpegToTiff(dlgOpenFileDialog.FileNames, chkIsMultipage.Checked)
                MessageBox.Show("图像转换完成.")
            Catch ex As Exception
                MessageBox.Show("出现错误: " + ex.Message, "错误")
            End Try
        End If

    End Sub

    Private Sub btnConvertToJpeg_Click(sender As System.Object, e As System.EventArgs) Handles btnConvertToJpeg.Click

        dlgOpenFileDialog.Multiselect = False
        dlgOpenFileDialog.Filter = "Image files (.tif, .tiff)|*.tif;*.tiff"

        If dlgOpenFileDialog.ShowDialog() = DialogResult.OK Then
            Try
                TiffImageConverter.ConvertTiffToJpeg(dlgOpenFileDialog.FileName)
                MessageBox.Show("图像转换完成.")
            Catch ex As Exception
                MessageBox.Show("出现错误: " + ex.Message, "错误")
            End Try
        End If
    End Sub

End Class