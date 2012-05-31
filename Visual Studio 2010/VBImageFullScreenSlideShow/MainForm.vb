'************************************ 模块头 ********************************************'
' 模块名: MainForm.vb
' 项目名: VBImageFullScreenSlideShow
' 版权 (c) Microsoft Corporation.
'
' 该实例演示了如何在全屏模式中显示幻灯片，如何修改放映图像的时间间隔.
'
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
'
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER 
' EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF 
' MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'****************************************************************************************'

Imports System.IO

Partial Public Class MainForm
    Inherits Form
    Private imageFiles() As String = Nothing

    ' 图像索引.
    Private selected As Integer = 0
    Private begin As Integer = 0
    Private [end] As Integer = 0

    Private fullScreenHelper As New FullScreen()

    Public Sub New()
        InitializeComponent()

        Me.btnPrevious.Enabled = False
        Me.btnNext.Enabled = False
        Me.btnImageSlideShow.Enabled = False
    End Sub

    ''' <summary>
    ''' 选择图像文件夹.
    ''' </summary>
    Private Sub btnOpenFolder_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnOpenFolder.Click
        If Me.imageFolderBrowserDlg.ShowDialog() = DialogResult.OK Then
            Me.imageFiles = GetFiles(Me.imageFolderBrowserDlg.SelectedPath, _
                                     "*.jpg;*.jpeg;*.png;*.bmp;*.tif;*.tiff;*.gif")

            Me.selected = 0
            Me.begin = 0
            Me.end = imageFiles.Length

            If Me.imageFiles.Length = 0 Then
                MessageBox.Show("找不到任何图像")

                Me.btnPrevious.Enabled = False
                Me.btnNext.Enabled = False
                Me.btnImageSlideShow.Enabled = False
            Else
                ShowImage(imageFiles(selected), pictureBox)

                Me.btnPrevious.Enabled = True
                Me.btnNext.Enabled = True
                Me.btnImageSlideShow.Enabled = True
            End If
        End If
    End Sub

    Public Shared Function GetFiles(ByVal path As String, ByVal searchPattern As String) As String()
        Dim patterns() As String = searchPattern.Split(";"c)
        Dim files As New List(Of String)()
        For Each filter As String In patterns
            ' 遍历目录树 ,忽视 DirectoryNotFoundException 
            '  或者 UnauthorizedAccessException 异常.
            ' http://msdn.microsoft.com/en-us/library/bb513869.aspx

            ' 用于保存文件中待检查的子文件夹名称的数据结构.
            Dim dirs As New Stack(Of String)(20)

            If Not Directory.Exists(path) Then
                Throw New ArgumentException()
            End If
            dirs.Push(path)

            Do While dirs.Count > 0
                Dim currentDir As String = dirs.Pop()
                Dim subDirs() As String
                Try
                    subDirs = Directory.GetDirectories(currentDir)
                    ' 如果我们没有一个文件夹或者文件的访问权限,一个  
                    ' UnauthorizedAccessException 异常将抛出.
                    ' 它可能允许忽视异常并且继续枚举剩下的文件和文件夹,或者不可能.
                    ' 有时候也可能引起DirectoryNotFound异常（但可能性不大）.
                    ' 当调用Directory.Exists之后, currentDir被另一个应用程序或线
                    ' 程删除时,该异常将发生.
                    ' 选择捕捉哪一个异常，完全取决于你打算执行的特定任务，和你对此 
                    ' 代码将运行的系统有多少确定性.
                Catch e As UnauthorizedAccessException
                    Continue Do
                Catch e As DirectoryNotFoundException
                    Continue Do
                End Try

                Try
                    files.AddRange(Directory.GetFiles(currentDir, filter))
                Catch e As UnauthorizedAccessException
                    Continue Do
                Catch e As DirectoryNotFoundException
                    Continue Do
                End Try

                ' 将子目录推入栈以便遍历.
                ' 这也可能在处理文件前发生.
                For Each str As String In subDirs
                    dirs.Push(str)
                Next str
            Loop
        Next filter

        Return files.ToArray()
    End Function

    ''' <summary>
    ''' 点击"上一张"按钮导航至上一张图像. 
    ''' </summary>
    Private Sub btnPrevious_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnPrevious.Click
        If Me.imageFiles Is Nothing OrElse Me.imageFiles.Length = 0 Then
            MessageBox.Show("请选择要以幻灯片形式放映的图像!")
            Return
        End If
        ShowPrevImage()
    End Sub

    ''' <summary>
    ''' 点击"下一张"按钮导航至下一张图像. 
    ''' </summary>
    Private Sub btnNext_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnNext.Click
        If Me.imageFiles Is Nothing OrElse Me.imageFiles.Length = 0 Then
            MessageBox.Show("请选择要以幻灯片形式放映的图像!")
            Return
        End If
        ShowNextImage()
    End Sub

    ''' <summary>
    ''' 以幻灯片形式放映图像.
    ''' </summary>
    Private Sub btnImageSlideShow_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnImageSlideShow.Click
        If Me.imageFiles Is Nothing OrElse Me.imageFiles.Length = 0 Then
            MessageBox.Show("请选择要以幻灯片形式放映的图像!")
            Return
        End If

        If timer.Enabled = True Then
            Me.timer.Enabled = False
            Me.btnOpenFolder.Enabled = True
            Me.btnImageSlideShow.Text = "开始幻灯片放映"
        Else
            Me.timer.Enabled = True
            Me.btnOpenFolder.Enabled = False
            Me.btnImageSlideShow.Text = "停止幻灯片放映"
        End If
    End Sub

    ''' <summary>
    ''' 定期显示下一张图像.
    ''' </summary>
    Private Sub timer_Tick(ByVal sender As Object, ByVal e As EventArgs) Handles timer.Tick
        ShowNextImage()
    End Sub

    ''' <summary>
    ''' 显示子窗体来更改Timer控件的设置. 
    ''' </summary>
    Private Sub btnSetting_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSetting.Click
        Dim frmSettings As New Setting(timer)
        frmSettings.ShowDialog()
    End Sub

    ''' <summary>
    ''' 进入或者离开全屏模式.
    ''' </summary>
    Private Sub btnFullScreen_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnFullScreen.Click
        If Not Me.fullScreenHelper.IsFullScreen Then
            ' 隐藏按钮，最大化幻灯片放映面板. 
            Me.gbButtons.Visible = False
            Me.pnlSlideShow.Dock = DockStyle.Fill

            fullScreenHelper.EnterFullScreen(Me)
        End If
    End Sub

    ''' <summary>
    ''' 响应"ESC"按键.
    ''' </summary>
    Protected Overrides Function ProcessCmdKey(ByRef msg As Message, ByVal keyData As Keys) As Boolean
        If keyData = Keys.Escape Then
            If Me.fullScreenHelper.IsFullScreen Then
                ' 显示按钮,还原幻灯片放映面板.
                Me.gbButtons.Visible = True
                Me.pnlSlideShow.Dock = DockStyle.Top

                fullScreenHelper.LeaveFullScreen(Me)
            End If
            Return True
        Else
            Return MyBase.ProcessCmdKey(msg, keyData)
        End If
    End Function

    ''' <summary>
    ''' 在PictureBox显示图像.
    ''' </summary>
    Public Shared Sub ShowImage(ByVal path As String, ByVal pct As PictureBox)
        pct.ImageLocation = path
    End Sub

    ''' <summary>
    ''' 显示上一张图像.
    ''' </summary>
    Private Sub ShowPrevImage()
        Me.selected -= 1
        ShowImage(Me.imageFiles((Me.selected) Mod Me.imageFiles.Length), Me.pictureBox)
    End Sub

    ''' <summary>
    ''' 显示下一张图像.
    ''' </summary>
    Private Sub ShowNextImage()
        Me.selected += 1
        ShowImage(Me.imageFiles((Me.selected) Mod Me.imageFiles.Length), Me.pictureBox)
    End Sub
End Class

