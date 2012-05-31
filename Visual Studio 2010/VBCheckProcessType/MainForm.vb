'*************************** 模块头 ******************************'
' 模块名:  MainForm.vb
' 项目:      VBCheckProcessType
' 版权  (c) Microsoft Corporation.
' 
' This is the main form of this application. It is used to initialize the UI and 
' handle the events.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*************************************************************************'

Imports System.Linq


Partial Public Class MainForm
    Inherits Form
    Public Sub New()
        InitializeComponent()
        If Not RunningProcess.IsOSVersionSupported Then
            MessageBox.Show("该应用程序必须运行在Windows Vista或更高版本的操作系统。")
            btnRefresh.Enabled = False
        End If
    End Sub

    ''' <summary>
    ''' 处理btnRefresh_Click事件。
    ''' </summary>
    Private Sub btnRefresh_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles btnRefresh.Click
        DatabindGridView()
    End Sub

    ''' <summary>
    ''' 绑定DataGridView的数据源。
    ''' </summary>
    Private Sub DatabindGridView()
        Dim runningProcesses As New List(Of RunningProcess)()
        For Each proc In Process.GetProcesses().OrderBy(Function(p) p.Id)
            Dim runningProcess_Renamed As New RunningProcess(proc)
            runningProcesses.Add(runningProcess_Renamed)
        Next proc
        gvProcess.DataSource = runningProcesses
    End Sub
End Class

