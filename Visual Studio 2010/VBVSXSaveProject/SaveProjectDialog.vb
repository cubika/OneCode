'********************************* Module Header *********************************\
' 模块名称:        SaveProjectDialog.vb
' 项目 :           VBVSXSaveProject
' 版权所有（c）微软公司
'
' 这个对话框是用来显示项目中的所有文件，以及所有在项目内的文件夹 
' 用户可以根据自己的需要来拷贝文件
' 
' The source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/default.aspx
' All other rights reserved
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER 
' EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF 
' MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***********************************************************************************/

Imports System.IO
Imports System.Windows.Forms
Imports EnvDTE
Imports Microsoft.VBVSXSaveProject.Files

Partial Public Class SaveProjectDialog
    Inherits Form
#Region "声明用于存储信息的变量。"
    ''' <summary>
    ''' 项目文件夹的路径。
    ''' </summary>
    Public Property OriginalFolderPath() As String

    ''' <summary>
    ''' 您在文件夹浏览器对话框中所选择的文件夹的路径。
    ''' </summary>
    Private _newFolderPath As String
    Public Property NewFolderPath() As String
        Get
            Return _newFolderPath
        End Get
        Private Set(ByVal value As String)
            _newFolderPath = value
        End Set
    End Property

    ''' <summary>
    ''' 项目中包含的文件，或是在项目文件夹下的文件。
    ''' </summary>
    Public Property FilesItems() As List(Of Files.ProjectFileItem)

    ''' <summary>
    ''' 指定新的项目是否应该打开。
    ''' </summary>
    Public ReadOnly Property OpenNewProject() As Boolean
        Get
            Return chkOpenProject.Checked
        End Get
    End Property

#End Region

    ''' <summary>
    ''' 构建SaveProject对话框。
    ''' </summary>
    Public Sub New()
        InitializeComponent()

        ' 将自动生成的列设置为false。
        Me.dgvFiles.AutoGenerateColumns = False
    End Sub

    ''' <summary>
    ''' 当点击按钮的时候，将项目“另存为”。
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub btnSaveAs_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSaveAs.Click
        ' 获取文件夹浏览器对话框中你想要保存的文件夹的路径。
        Using dialog As New FolderBrowserDialog()
            ' 将新建文件夹按钮设置为可用。
            dialog.ShowNewFolderButton = True

            ' 获取文件夹浏览器对话框的结果。
            Dim result = dialog.ShowDialog()

            If result = System.Windows.Forms.DialogResult.OK Then
                ' 获取文件夹路径。
                Me.NewFolderPath = dialog.SelectedPath

                ' 拷贝用户选择的文件。
                CopySelectedItems()

                ' 当按下“ok”按钮，关闭这个窗口。
                Me.DialogResult = System.Windows.Forms.DialogResult.OK
                Me.Close()
            Else
                Return
            End If
        End Using
    End Sub

    ''' <summary>
    ''' 保存项目对话框已经加载。
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub SaveProjectDialog_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        ' 将数据源和文件项目关联起来
        dgvFiles.DataSource = FilesItems

        For Each row As DataGridViewRow In dgvFiles.Rows
            Dim item As ProjectFileItem = TryCast(row.DataBoundItem, ProjectFileItem)

            row.Cells("colCopy").ReadOnly = Not item.IsUnderProjectFolder
        Next row
    End Sub

#Region "创建和复制文件。"
    ''' <summary>
    ''' 拷贝你在DataGridView中选择的项目。
    ''' </summary>
    Private Sub CopySelectedItems()
        ' 从解决方案资源管理器中获取文件的信息。
        Dim fileItems As List(Of Files.ProjectFileItem) =
            TryCast(dgvFiles.DataSource, List(Of Files.ProjectFileItem))

        ' 文件从原来的目录复制到新的路径。
        For Each fileItem In fileItems
            If fileItem.IsUnderProjectFolder AndAlso fileItem.NeedCopy Then
                ' 获取你所保存的项目文件的绝对路径。
                Dim newFile As New FileInfo(
                    String.Format("{0}\{1}",
                                  NewFolderPath,
                                  fileItem.FullName.Substring(OriginalFolderPath.Length)))

                ' 用文件全名创建新的目录。
                If Not newFile.Directory.Exists Then
                    Directory.CreateDirectory(newFile.Directory.FullName)
                End If

                ' 拷贝文件。
                fileItem.Fileinfo.CopyTo(newFile.FullName)
            End If
        Next fileItem
    End Sub

#End Region

    ''' <summary>
    ''' 取消“项目另存为”的操作。
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub btnCancel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCancel.Click
        ' 取消并关闭窗体。
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub dgvFiles_CellContentClick(sender As System.Object, e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dgvFiles.CellContentClick

    End Sub
End Class