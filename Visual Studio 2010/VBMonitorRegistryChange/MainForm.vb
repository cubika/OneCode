'******************************* 模块头 ************************************\
' 模块名:   Module Name:  MainForm.vb
' 项目名:   VBMonitorRegistryChange
' 版权(c)   Microsoft Corporation.
' 
' 这是本应用程序的主窗口。用于初始化UI并处理事件。
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**************************************************************************

Imports System.Management
Imports Microsoft.Win32

Partial Public Class MainForm
    Inherits Form

    ' 当前状态
    Private isMonitoring As Boolean = False

    Private watcher As RegistryWatcher = Nothing

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub MainForm_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load

        ' 初始化cmbHives的数据源。初始化cmbHives的数据源。对于HKEY_CLASSES_ROOT和
        ' HKEY_CURRENT_USER节点的改变不被RegistryEvent或它的派生类，如RegistryKeyChangeEvent
        ' 事件所支持。 
        cmbHives.DataSource = RegistryWatcher.SupportedHives

    End Sub

    ''' <summary>
    ''' 处理btnMonitor的点击事件。
    ''' </summary>
    Private Sub btnMonitor_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnMonitor.Click

        ' 如果应用程序正在监控注册表项，则停止监控并启用编辑器。
        If isMonitoring Then
            Dim success As Boolean = StopMonitor()
            If success Then
                btnMonitor.Text = "开始监控"
                cmbHives.Enabled = True
                tbRegkeyPath.ReadOnly = False
                isMonitoring = False
                lstChanges.Items.Add(String.Format("{0} 停止监控", Date.Now))
            End If

            ' 如果应用程序是空闲的，则启动监控并禁用编辑器。
        Else
            Dim success As Boolean = StartMonitor()
            If success Then
                btnMonitor.Text = "停止监控"
                cmbHives.Enabled = False
                tbRegkeyPath.ReadOnly = True
                isMonitoring = True
                lstChanges.Items.Add(String.Format("{0} 开始监控", Date.Now))
            End If
        End If

    End Sub

    ''' <summary>
    ''' 检查被监控的项是否存在，然后启动ManagementEventWatcher来监控RegistryKeyChangeEvent事件。
    ''' </summary>
    ''' <returns>如果ManagementEventWatcher启动成功则为真。</returns>
    Private Function StartMonitor() As Boolean
        Dim hive As RegistryKey = TryCast(cmbHives.SelectedValue, RegistryKey)
        Dim keyPath = tbRegkeyPath.Text.Trim()

        Try
            watcher = New RegistryWatcher(hive, keyPath)

            ' 当被监控的项不存在时，RegistryWatcher的构造器可能会抛出一个SecurityException异常。
        Catch _ArgumentException As ArgumentException
            MessageBox.Show(_ArgumentException.Message)
            Return False

            ' 当前用户没有访问被监控项的权限时，RegistryWatcher的构造器可能会抛出
            ' 一个SecurityException异常。
        Catch _SecurityException As System.Security.SecurityException
            Dim message As String = String.Format("您没有访问项 {0}\{1} 的权限。", hive.Name, keyPath)
            MessageBox.Show(message)
            Return False
        End Try

        Try

            ' 设置用于处理变更事件的句柄。
            AddHandler watcher.RegistryKeyChangeEvent, AddressOf watcher_RegistryKeyChangeEvent

            ' 启动监听事件。
            watcher.Start()
            Return True
        Catch comException As System.Runtime.InteropServices.COMException
            MessageBox.Show("发生错误： " & comException.Message)
            Return False
        Catch managementException_Renamed As ManagementException
            MessageBox.Show("发生错误： " & managementException_Renamed.Message)
            Return False
        End Try

    End Function

    ''' <summary>
    ''' 停止监听事件。
    ''' </summary>
    ''' <returns>如果ManagementEventWatcher成功停止则为真。</returns>
    Private Function StopMonitor() As Boolean
        Try
            watcher.Stop()
            Return True
        Catch _ManagementException As ManagementException
            MessageBox.Show("发生错误： " & _ManagementException.Message)
            Return False
        Finally
            watcher.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' 处理RegistryKeyChangeEvent事件。
    ''' </summary>
    Private Sub watcher_RegistryKeyChangeEvent(ByVal sender As Object, ByVal e As RegistryKeyChangeEventArgs)
        Dim newEventMessage As String = String.Format(
            "{0} 项 {1}\{2} 发生变化",
            e.TIME_CREATED.ToLocalTime(),
            e.Hive,
            e.KeyPath)
        lstChanges.Items.Add(newEventMessage)
    End Sub

End Class
