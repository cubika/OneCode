'****************************** 模块头 ******************************'
' 模块名:                 MainPage.xaml.vb
' 项目名:                     VBSL3IsolatedStorage
' 版权 (c) Microsoft Corporation.
' 
' 独立存储器示例后台文件
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************'

Imports System.Windows.Data
Imports System.Windows.Media.Imaging
Imports System.Collections.ObjectModel
Imports System.IO
Imports System.ComponentModel
Imports System.IO.IsolatedStorage

Partial Public Class MainPage
    Inherits UserControl
    Public Sub New()
        InitializeComponent()
        AddHandler Loaded, AddressOf MainPage_Loaded
    End Sub

    ' 初始化应用程序
    Private _isoroot As IsoDirectory
    Private _isofile As IsolatedStorageFile
    Private Sub MainPage_Loaded(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' 加载独立存储器文件
        _isofile = IsolatedStorageFile.GetUserStoreForApplication()

        ' 显示独立存储器文件信息
        RefreshAvailableSize()

        ' 加载独立存储器树目录查看模型
        _isoroot = LoadIsoRoot()

        ' 显示树目录
        Dim roots = New ObservableCollection(Of IsoFile)()
        roots.Add(_isoroot)
        tvIsolatedStorage.ItemsSource = roots

        ' 从IsolatedStorageSettings取得上次登陆时间
        If IsolatedStorageSettings.ApplicationSettings.Contains("lastLogin") Then
            Dim [date] = IsolatedStorageSettings.ApplicationSettings("lastLogin").ToString()
            tbAppInfo.Text = "上次本应用程序运行时间为: " & [date]
        Else
            tbAppInfo.Text = "没有上次本应用程序运行时间。"
        End If
        ' 保存登陆时间到IsolatedStorageSettings
        IsolatedStorageSettings.ApplicationSettings("lastLogin") = DateTime.Now

        ' 更新操作面板
        UpdateOperationPanel()
    End Sub

#Region "独立存储器树目录查看模型方法"

    ' Helper 方法: 取得父目录
    Private Function GetParentDir(ByVal root As IsoDirectory, ByVal child As IsoFile) As IsoDirectory
        If String.IsNullOrEmpty(child.FilePath) Then
            Return Nothing
        Else
            Dim dirs As String() = child.FilePath.Split("/"c)
            Dim cur As IsoDirectory = root
            For i As Integer = 1 To dirs.Length - 2
                Dim [next] As IsoDirectory = TryCast(cur.Children.FirstOrDefault(Function(dir) dir.FileName = dirs(i)), IsoDirectory)
                If [next] IsNot Nothing Then
                    cur = [next]
                Else
                    Return Nothing
                End If
            Next
            Return cur
        End If
    End Function

    ' 加载独立存储器查看模型
    Private Function LoadIsoRoot() As IsoDirectory
        Dim root = New IsoDirectory("Root", Nothing)
        AddFileToDirectory(root, _isofile)
        Return root
    End Function

    ' 用递归方法增加目录/文件
    Private Sub AddFileToDirectory(ByVal dir As IsoDirectory, ByVal isf As IsolatedStorageFile)
        Dim childrendir As String(), childrenfile As String()
        If String.IsNullOrEmpty(dir.FilePath) Then
            childrendir = isf.GetDirectoryNames()
            childrenfile = isf.GetFileNames()
        Else
            childrendir = isf.GetDirectoryNames(dir.FilePath & "/")
            childrenfile = isf.GetFileNames(dir.FilePath & "/")
        End If

        ' 增加目录实体
        For Each dirname In childrendir
            Dim childdir = New IsoDirectory(dirname, (dir.FilePath & "/") + dirname)
            AddFileToDirectory(childdir, isf)
            dir.Children.Add(childdir)
        Next

        ' 增加文件实体
        For Each filename In childrenfile
            dir.Children.Add(New IsoFile(filename, (dir.FilePath & "/") + filename))
        Next
    End Sub
#End Region

#Region "在worker线程中复制流"

    ' 创建workerthread来复制流
    Private Sub CopyStream(ByVal from As Stream, ByVal [to] As Stream)

        Dim bworker As New BackgroundWorker()
        bworker.WorkerReportsProgress = True
        AddHandler bworker.DoWork, AddressOf bworker_DoWork
        AddHandler bworker.ProgressChanged, AddressOf bworker_ProgressChanged
        AddHandler bworker.RunWorkerCompleted, AddressOf bworker_RunWorkerCompleted

        bworker.RunWorkerAsync(New Stream() {from, [to]})

        ' 显示“复制”面板
        gdDisable.Visibility = Visibility.Visible
        spCopyPanel.Visibility = Visibility.Visible
        gdPlayerPanel.Visibility = Visibility.Collapsed
    End Sub

    ' 处理工作完成事件
    Private Sub bworker_RunWorkerCompleted(ByVal sender As Object, ByVal e As RunWorkerCompletedEventArgs)
        ' 关闭“复制”面板
        gdDisable.Visibility = Visibility.Collapsed

        If e.[Error] IsNot Nothing Then
            MessageBox.Show(e.[Error].Message)
        End If
    End Sub

    ' 显示进度
    Private Sub bworker_ProgressChanged(ByVal sender As Object, ByVal e As ProgressChangedEventArgs)
        pbCopyProgress.Value = e.ProgressPercentage
    End Sub

    ' 在workerthread线程中复制流
    Private Sub bworker_DoWork(ByVal sender As Object, ByVal e As DoWorkEventArgs)
        Dim param = TryCast(e.Argument, Stream())

        Dim buffer As Byte() = New Byte(65535) {}
        Dim pos As Integer = 0
        Dim progress As Integer = -1
        While True
            Dim icount As Integer = param(0).Read(buffer, pos, buffer.Length)
            param(1).Write(buffer, 0, icount)
            If icount < buffer.Length Then
                Exit While
            End If

            Dim curprogress As Integer = CInt((param(1).Length * 100 / param(0).Length))
            If curprogress > progress Then
                progress = curprogress
                DirectCast(sender, BackgroundWorker).ReportProgress(progress)
            End If
        End While

        ' 在用户界面线程中关闭线程
        Dispatcher.BeginInvoke(New MyDelegate(AddressOf CloseStream), param(0), param(1))
    End Sub

    Delegate Sub MyDelegate(ByVal stream1 As Stream, ByVal stream2 As Stream)
    Private Sub CloseStream(ByVal stream1 As Stream, ByVal stream2 As Stream)
        stream1.Close()
        stream2.Close()
        RefreshAvailableSize()
    End Sub

#End Region

#Region "树目录视图和操作按钮事件处理器"

    Private Sub RefreshAvailableSize()
        tbQuotaAvailable.Text = String.Format("当前存储器空间大小是: {0}KB, {1}KB可用。 这个配额能通过用户操作增加，例如鼠标点击操作。", _isofile.Quota / 1024, _isofile.AvailableFreeSpace / 1024)
    End Sub

    ' 更新操作面板
    Private Sub UpdateOperationPanel()
        Dim item = tvIsolatedStorage.SelectedItem
        If item Is Nothing Then
            spOperationPanel.Visibility = Visibility.Collapsed
        Else
            spOperationPanel.Visibility = Visibility.Visible
            If TypeOf item Is IsoDirectory Then
                bnAddDir.Visibility = Visibility.Visible
                bnAddFile.Visibility = Visibility.Visible
                bnDelete.Visibility = Visibility.Visible
                bnSave.Visibility = Visibility.Collapsed
                bnPlay.Visibility = Visibility.Collapsed
            ElseIf TypeOf item Is IsoFile Then
                bnAddDir.Visibility = Visibility.Collapsed
                bnAddFile.Visibility = Visibility.Collapsed
                bnDelete.Visibility = Visibility.Visible
                bnSave.Visibility = Visibility.Visible
                bnPlay.Visibility = Visibility.Visible
            End If
        End If
    End Sub

    ' 增加空间配额
    Private Sub bnIncreaseQuota_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Try
            If Not _isofile.IncreaseQuotaTo(_isofile.Quota + 1024 * 1024 * 10) Then
                MessageBox.Show("增加空间配额失败。")
            End If
        Catch ex As ArgumentException
            MessageBox.Show(ex.Message)
        End Try

        RefreshAvailableSize()
    End Sub

    ' 增加目录
    Private Sub bnAddDir_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim item = TryCast(tvIsolatedStorage.SelectedItem, IsoDirectory)
        If item IsNot Nothing Then
            Dim newfoldename As String = "Folder_" & Guid.NewGuid().ToString()
            Dim newfolderpath As String = (item.FilePath & "/") + newfoldename

            Try
                ' 检查是否目录已经存在
                If _isofile.DirectoryExists(newfolderpath) Then
                    MessageBox.Show("目录已经存在:" & newfolderpath)
                Else
                    _isofile.CreateDirectory(newfolderpath)
                    item.Children.Add(New IsoDirectory(newfoldename, newfolderpath))
                End If
            Catch ex As PathTooLongException
                MessageBox.Show("由于路径长度限制，目录深度设置为应该小于4。")
            Catch ex As Exception
                MessageBox.Show("增加目录失败。" & vbLf & "详细: " & ex.Message)
            End Try
        End If
    End Sub

    ' 增加文件
    Private Sub bnAddFile_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim overrideflag As Boolean = False
        Dim selecteddir = TryCast(tvIsolatedStorage.SelectedItem, IsoDirectory)
        If selecteddir Is Nothing Then
            Exit Sub
        End If

        Dim ofd As New OpenFileDialog()
        Dim result = ofd.ShowDialog()
        If result.HasValue AndAlso result.Value Then
            Dim filename As String = ofd.File.Name
            Dim filepath As String = (selecteddir.FilePath & "/") + filename
            Dim file As New IsoFile(filename, filepath)

            Try
                ' 检查文件名是否和目录名一样
                If _isofile.GetDirectoryNames(filepath).Length > 0 Then
                    MessageBox.Show(String.Format("File name {0} not allowed", filename))
                    Exit Sub
                    ' 检查文件是否已经存在
                ElseIf _isofile.GetFileNames(filepath).Length > 0 Then
                    ' 显示提示框，问是否覆盖文件
                    Dim mbresult = MessageBox.Show(String.Format("覆盖当前文件: {0} ?", filename), "override warning", MessageBoxButton.OKCancel)
                    If mbresult <> MessageBoxResult.OK Then
                        Exit Sub
                    Else
                        overrideflag = True
                    End If
                End If
            Catch ex As PathTooLongException
                MessageBox.Show("增加文件失败。文件路径太长。")
                Exit Sub
            Catch ex As Exception
                MessageBox.Show(ex.Message)
                Exit Sub
            End Try

            ' 检查是否有足够空间
            If _isofile.AvailableFreeSpace < ofd.File.Length Then
                MessageBox.Show("独立存储器空间不够。")
                Exit Sub
            End If

            Dim isostream As Stream = Nothing
            Dim filestream As Stream = Nothing
            Try
                ' 创建isolatedstorage流
                isostream = _isofile.CreateFile(filepath)
                ' 打开文件流
                filestream = ofd.File.OpenRead()
                ' 复制
                ' 注意：这里不会捕捉复制过程中的异常。
                CopyStream(filestream, isostream)

                ' 检查覆盖
                If overrideflag = False Then
                    selecteddir.Children.Add(file)
                End If
            Catch ex As Exception
                If isostream IsNot Nothing Then
                    isostream.Close()
                End If
                If filestream IsNot Nothing Then
                    filestream.Close()
                End If
                MessageBox.Show(ex.Message)
            End Try
        End If
    End Sub

    ' 删除
    Private Sub bnDelete_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim item = TryCast(tvIsolatedStorage.SelectedItem, IsoFile)
        If item IsNot Nothing Then
            ' 根目录
            If String.IsNullOrEmpty(item.FilePath) Then
                MessageBox.Show("不能删除根目录")
                Exit Sub
            End If

            Try
                If TypeOf item Is IsoDirectory Then
                    _isofile.DeleteDirectory(item.FilePath)
                Else
                    _isofile.DeleteFile(item.FilePath)
                End If
                Dim isodirparent = GetParentDir(_isoroot, item)
                If isodirparent IsNot Nothing Then
                    isodirparent.Children.Remove(item)
                End If
            Catch ex As Exception
                MessageBox.Show(ex.Message)
            End Try
        End If

        RefreshAvailableSize()
    End Sub

    ' 保存到本地
    Private Sub bnSave_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim item = TryCast(tvIsolatedStorage.SelectedItem, IsoFile)
        If item IsNot Nothing Then
            Try
                Dim sfd1 As New SaveFileDialog()

                ' 设置文件过滤
                Dim substr = item.FileName.Split("."c)
                If substr.Length >= 2 Then
                    Dim defaultstr As String = "*." & substr(substr.Length - 1)
                    sfd1.Filter = String.Format("({0})|{1}|(*.*)|*.*", defaultstr, defaultstr)
                Else
                    sfd1.Filter = "(*.*)|*.*"
                End If

                ' 显示保存对话框
                Dim result = sfd1.ShowDialog()

                If result.HasValue AndAlso result.Value Then
                    ' 打开isolatedstorage流
                    Dim filestream = sfd1.OpenFile()
                    ' 创建文件流
                    Dim isostream = _isofile.OpenFile(item.FilePath, FileMode.Open, FileAccess.Read)
                    ' 复制
                    CopyStream(isostream, filestream)
                End If
            Catch ex As Exception
                MessageBox.Show(ex.Message)
            End Try
        End If
    End Sub

    ' 关闭播放面板
    Private Sub bnClosePlayer_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        gdDisable.Visibility = Visibility.Collapsed
        mePlayer.[Stop]()
        mePlayer.Source = Nothing

        If currentplaystream IsNot Nothing Then
            currentplaystream.Close()

        End If
    End Sub

    ' 播放
    Private currentplaystream As Stream = Nothing
    Private Sub bnPlay_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim item = TryCast(tvIsolatedStorage.SelectedItem, IsoFile)
        If item IsNot Nothing Then
            Try
                Dim stream = _isofile.OpenFile(item.FilePath, FileMode.Open, FileAccess.Read)

                ' 显示播放面板
                gdDisable.Visibility = Visibility.Visible
                spCopyPanel.Visibility = Visibility.Collapsed
                gdPlayerPanel.Visibility = Visibility.Visible

                mePlayer.SetSource(stream)
                currentplaystream = stream
            Catch ex As Exception
                MessageBox.Show(ex.Message)
            End Try
        End If
    End Sub

    ' 当选择树目录的子项改变时，刷新操作面板
    Private Sub TreeView_SelectedItemChanged(ByVal sender As Object, ByVal e As RoutedPropertyChangedEventArgs(Of Object))
        UpdateOperationPanel()
    End Sub
#End Region

End Class


' Isolatedstoarge文件对象
Public Class IsoFile
    Private _FilePath As String
    Public Property FilePath() As String
        Get
            Return _FilePath
        End Get
        Set(ByVal value As String)
            _FilePath = value
        End Set
    End Property
    Private _FileName As String
    Public Property FileName() As String
        Get
            Return _FileName
        End Get
        Set(ByVal value As String)
            _FileName = value
        End Set
    End Property

    Private _ContentStream As Stream
    Public Property ContentStream() As Stream
        Get
            Return _ContentStream
        End Get
        Private Set(ByVal value As Stream)
            _ContentStream = value
        End Set
    End Property
    Public Sub New(ByVal strFilename As String, ByVal strPath As String)
        FileName = strFilename
        FilePath = strPath
    End Sub
End Class

' Isolatedstorage目录对象
Public Class IsoDirectory
    Inherits IsoFile
    Private _Children As ObservableCollection(Of IsoFile)
    Public Property Children() As ObservableCollection(Of IsoFile)
        Get
            Return _Children
        End Get
        Private Set(ByVal value As ObservableCollection(Of IsoFile))
            _Children = value
        End Set
    End Property
    Public Sub New(ByVal strFilename As String, ByVal strPath As String)
        MyBase.New(strFilename, strPath)
        Children = New ObservableCollection(Of IsoFile)()
    End Sub
End Class

' 图像转换器
' 根据实体类型，返回不同的图像
Public Class ImageConverter
    Implements IValueConverter

    Public Function Convert(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert
        If TypeOf value Is IsoDirectory Then
            Return New BitmapImage(New Uri("/Images/dir.png", UriKind.Relative))
        End If
        Return New BitmapImage(New Uri("/Images/File.png", UriKind.Relative))
    End Function

    Public Function ConvertBack(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.ConvertBack
        Throw New NotImplementedException
    End Function
End Class
