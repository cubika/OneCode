'**************************** 模块头 *************************************\
'  模块名:  MainForm.vb
'  项目名:      VBManipulateImagesInWordDocument
'  版权 (c) Microsoft Corporation.
' 
'  这是应用程序的主窗口
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

Imports System.Drawing.Imaging
Imports System.IO
Imports DocumentFormat.OpenXml.Drawing

Partial Public Class MainForm
    Inherits Form
    Private documentManipulator As WordDocumentImageManipulator

    Public Sub New()
        InitializeComponent()
    End Sub

    ''' <summary>
    ''' 处理 btnOpenFile 点击事件.
    ''' </summary>
    Private Sub btnOpenFile_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles btnOpenFile.Click

        ' 打开一个OpenFileDialog实例
        Using dialog As New OpenFileDialog()
            dialog.Filter = "Word 文档 (*.docx)|*.docx"

            Dim result = dialog.ShowDialog()
            If result = DialogResult.OK Then
                Try
                    lstImage.Items.Clear()
                    If picView.Image IsNot Nothing Then
                        picView.Image.Dispose()
                    End If
                    picView.Image = Nothing
                    lbFileName.Text = String.Empty


                    ' 初始化一个WordDocumentImageManipulator 实例.
                    OpenWordDocument(dialog.FileName)

                    ' 更新 lstImage listbox.
                    UpdateImageList()

                    lbFileName.Text = dialog.FileName
                Catch ex As Exception
                    MessageBox.Show(ex.Message)
                End Try
            End If
        End Using
    End Sub

    ''' <summary>
    ''' 初始化一个WordDocumentImageManipulator实例.
    ''' </summary>
    Private Sub OpenWordDocument(ByVal filepath As String)
        If String.IsNullOrEmpty(filepath) OrElse (Not System.IO.File.Exists(filepath)) Then
            Throw New ArgumentException("文件路径")
        End If

        Dim file As New FileInfo(filepath)

        ' 释放之前实例的资源
        If documentManipulator IsNot Nothing Then
            documentManipulator.Dispose()
        End If

        documentManipulator = New WordDocumentImageManipulator(file)

        ' 注册 ImagesChanged事件.
        AddHandler documentManipulator.ImagesChanged, AddressOf documentManipulator_ImagesChanged

    End Sub

    ''' <summary>
    ''' 更新 lstImage listbox.
    ''' </summary>
    Private Sub UpdateImageList()
        If picView.Image IsNot Nothing Then
            picView.Image.Dispose()
        End If
        picView.Image = Nothing

        lstImage.Items.Clear()

        ' 显示Blip元素的 "嵌入"属性. 这个属性是ImagePart 的ID的引用       
        lstImage.DisplayMember = "嵌入"
        For Each blip In documentManipulator.GetAllImages()
            lstImage.Items.Add(blip)
        Next blip
    End Sub

    ''' <summary>
    ''' 处理 ImagesChanged 事件.
    ''' </summary>
    Private Sub documentManipulator_ImagesChanged(ByVal sender As Object, ByVal e As EventArgs)
        UpdateImageList()
    End Sub

    ''' <summary>
    ''' 处理 lstImage SelectedIndexChanged event 来显示 picView 中的图片        
    ''' </summary>
    Private Sub lstImage_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) _
        Handles lstImage.SelectedIndexChanged

        Dim imgBlip = TryCast(lstImage.SelectedItem, Blip)
        If imgBlip Is Nothing Then
            Return
        End If

        ' 释放在picView中之前的图片的资源
        If picView.Image IsNot Nothing Then
            picView.Image.Dispose()
            picView.Image = Nothing
        End If

        Try
            Dim newImg = documentManipulator.GetImageInBlip(imgBlip)
            picView.Image = New Bitmap(newImg)
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try

    End Sub

    ''' <summary>
    ''' 处理 btnDelete click 事件.
    ''' </summary>
    Private Sub btnDelete_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles btnDelete.Click
        If lstImage.SelectedItem IsNot Nothing Then
            Dim result = MessageBox.Show("你想删除这张图片吗？",
                                         "删除图片",
                                         MessageBoxButtons.YesNo,
                                         MessageBoxIcon.Question)

            If result = DialogResult.Yes Then
                Try
                    documentManipulator.DeleteImage(TryCast(lstImage.SelectedItem, Blip))
                Catch ex As Exception
                    MessageBox.Show(ex.Message)
                End Try
            End If
        End If
    End Sub

    ''' <summary>
    ''' 处理 btnReplace Click 事件.
    ''' </summary>
    Private Sub btnReplace_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles btnReplace.Click
        If lstImage.SelectedItem IsNot Nothing Then
            Using dialog As New OpenFileDialog()
                dialog.Filter = "图片文件 (*.jpeg;*.jpg;*.png)|*.jpeg;*.jpg;*.png"

                Dim result = dialog.ShowDialog()
                If result = DialogResult.OK Then
                    Dim confirm = MessageBox.Show("你想替换这张图片吗？",
                                                  "替换图片",
                                                  MessageBoxButtons.YesNo,
                                                  MessageBoxIcon.Question)
                    If confirm = DialogResult.Yes Then
                        Try
                            documentManipulator.ReplaceImage(
                                TryCast(lstImage.SelectedItem, Blip),
                                New FileInfo(dialog.FileName))
                        Catch ex As Exception
                            MessageBox.Show(ex.Message)
                        End Try
                    End If

                End If
            End Using
        End If
    End Sub

    ''' <summary>
    ''' 处理 btnExport Click 事件.
    ''' </summary>
    Private Sub btnExport_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles btnExport.Click
        If lstImage.SelectedItem IsNot Nothing _
            AndAlso picView.Image IsNot Nothing Then
            Using dialog As New SaveFileDialog()
                dialog.Filter = "图片文件 (*.jpeg)|*.jpeg"
                Dim result = dialog.ShowDialog()
                If result = DialogResult.OK Then
                    picView.Image.Save(dialog.FileName, ImageFormat.Jpeg)
                End If
            End Using
        End If
    End Sub
End Class