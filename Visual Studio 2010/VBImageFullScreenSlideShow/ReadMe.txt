=============================================================================
		   Windows 应用程序: VBImageFullScreenSlideShow 概述
=============================================================================

/////////////////////////////////////////////////////////////////////////////
摘要:

该实例演示如何在 Windows 窗体应用程序中显示图像幻灯片。它也演示了如何在全屏模式下以
幻灯片形式放映图像。


/////////////////////////////////////////////////////////////////////////////
演示:

步骤1. 在 Visual Studio 2010中生成并运行该示例项目.

步骤2. 准备一些图像文件.点击“打开文件...”按钮，选择包含图像文件的路径. 

步骤3. 点击"上一张"按钮和"下一张"按钮使图像文件按顺序显示

步骤4. 左键单击"设置"按钮, 为Timer控件选择显示图像文件的时间间隔，以便定期显示图像.
	   最后，左键单击"开始幻灯片放映"按钮，使图像文件一张接一张地显示

步骤5. 左键单击"全屏"按钮，在全屏模式下显示图像.按"ESC"键退出全屏模式.


/////////////////////////////////////////////////////////////////////////////
实现:

1. 当用户选择图像文件的根文件夹时, 该实例用基于堆栈的枚举方法，枚举文件夹中的
   图像文件.此方法在这篇MSDN文章中有所诠释： 
   http://msdn.microsoft.com/en-us/library/bb513869.aspx
   该实例没有使用
        Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)
   来枚举文件，因为当用户对根文件中的某一个目录或文件没有访问权限时，它将中断.

       Public Shared Function GetFiles(ByVal path As String, ByVal searchPattern As String) As String()
        Dim patterns() As String = searchPattern.Split(";"c)
        Dim files As New List(Of String)()
        For Each filter As String In patterns
            ' 遍历目录树 ,忽视 DirectoryNotFoundException  
            ' 或者 UnauthorizedAccessException 异常. 
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

2. 该实例在PictureBox显示图像.

    ''' <summary>
    ''' 在PictureBox显示图像..
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

   一个定时器(timer)用于自动地以幻灯片形式放映这些图像.

    ''' <summary>
    ''' 定期显示下一张图像.
    ''' </summary>
    Private Sub timer_Tick(ByVal sender As Object, ByVal e As EventArgs) Handles timer.Tick
        ShowNextImage()
    End Sub

3. 该实例提供了一个辅助类'FullScreen',以便在全屏模式下以幻灯片形式放映图像. 
   FullScreen.cs包括两个公共方法:
   
        EnterFullScreen - 用来使Windows窗体在全屏模式下显示.
        LeaveFullScreen - 用来使Windows窗体回到原始状态.

    ''' <summary>
    ''' 将窗口最大化至全屏.
    ''' </summary>
    Public Sub EnterFullScreen(ByVal targetForm As Form)
        If Not IsFullScreen Then
            Save(targetForm) ' 保存原始的窗体状态.

            targetForm.WindowState = FormWindowState.Maximized
            targetForm.FormBorderStyle = FormBorderStyle.None
            targetForm.TopMost = True
            targetForm.Bounds = Screen.GetBounds(targetForm)

            IsFullScreen = True
        End If
    End Sub
	
    ''' <summary>
    ''' 离开全屏模式,回到原始的窗体状态.
    ''' </summary>
    Public Sub LeaveFullScreen(ByVal targetForm As Form)
        If IsFullScreen Then
            ' 回到原始的窗体状态.
            targetForm.WindowState = winState
            targetForm.FormBorderStyle = brdStyle
            targetForm.TopMost = topMost
            targetForm.Bounds = bounds

            IsFullScreen = False
        End If
    End Sub


/////////////////////////////////////////////////////////////////////////////
参考:

How to: Iterate Through a Directory Tree (C# Programming Guide)
http://msdn.microsoft.com/en-us/library/bb513869.aspx

Screen.GetBounds Method 
http://msdn.microsoft.com/en-us/library/system.windows.forms.screen.getbounds.aspx


/////////////////////////////////////////////////////////////////////////////
