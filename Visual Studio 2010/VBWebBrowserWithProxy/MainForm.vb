'*************************** Module Header ******************************'
' 模块名称:  MainForm.vb
' 项目名称:	    VBWebBrowserWithProxy
' Copyright (c) Microsoft Corporation.
' 
' 这是这个应用程序的主窗口。各种初始化和事件管理都在此完成。
' 

' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*************************************************************************'

Partial Public Class MainForm
    Inherits Form
    ' 获取现有的代理
    Private ReadOnly Property CurrentProxy() As InternetProxy
        Get
            If radNoProxy.Checked Then
                Return InternetProxy.NoProxy
            Else
                Return TryCast(cmbProxy.SelectedItem, InternetProxy)
            End If
        End Get
    End Property

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub MainForm_Load(ByVal sender As Object, ByVal e As EventArgs) _
        Handles MyBase.Load


        ' 将代理数据列表绑定到组合列表框上
        cmbProxy.DisplayMember = "ProxyName"
        cmbProxy.DataSource = InternetProxy.ProxyList
        cmbProxy.SelectedIndex = 0

        AddHandler wbcSample.StatusTextChanged, AddressOf wbcSample_StatusTextChanged

    End Sub

    ''' <summary>
    ''' 处理btnNavigate_Click 事件
    ''' 本方法将为WebBrowser类封装WebBrowserControl类的Navigate方法用以在如果必要的情况下设置
    ''' 代理的授信头信息
    ''' </summary>
    Private Sub btnNavigate_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles btnNavigate.Click
        Try
            wbcSample.Proxy = CurrentProxy
            wbcSample.Goto(tbUrl.Text & "?id=" & Guid.NewGuid().ToString())
        Catch e1 As ArgumentException
            MessageBox.Show("请确认你的url是有效的！")
        End Try
    End Sub

    Private Sub wbcSample_ProgressChanged(ByVal sender As Object, _
                                          ByVal e As WebBrowserProgressChangedEventArgs) _
                                      Handles wbcSample.ProgressChanged

        prgBrowserProcess.Value = CInt(Fix(e.CurrentProgress))
        AddHandler wbcSample.StatusTextChanged, AddressOf wbcSample_StatusTextChanged
    End Sub

    Private Sub wbcSample_StatusTextChanged(ByVal sender As Object, ByVal e As EventArgs)
        lbStatus.Text = wbcSample.StatusText
    End Sub
End Class

