'************************************ 模块头 ****************************************'
' 模块名: Settings.vb
' 项目名: VBImageFullScreenSlideShow
' 版权 (c) Microsoft Corporation.
'
' 这段代码为Timer控件设置了时间间隔.
'
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
'
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER 
' EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF 
' MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***********************************************************************************'

Partial Public Class Setting
    Inherits Form
    Private _timr As Timer

    ''' <summary>
    ''' 为引入控件引用地址自定义构造函数.
    ''' </summary>
    Public Sub New(ByRef timr As Timer)
        InitializeComponent()
        _timr = timr
        Me.dtpInternal.Value = timr.Interval
    End Sub

    ''' <summary>
    ''' 取消操作.
    ''' </summary>
    Private Sub btnCancel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCancel.Click
        Me.Close()
    End Sub

    ''' <summary>
    ''' 保存Timer控件的时间间隔，关闭子窗体.
    ''' </summary>
    Private Sub btnConfirm_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnConfirm.Click
        _timr.Interval = Integer.Parse(Me.dtpInternal.Value.ToString())
        Me.Close()
    End Sub

End Class

