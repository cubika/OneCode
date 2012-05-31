'****************************** 模块头 *************************************'
' 模块名:   MainForm.vb
' 项目名:	VBWebBrowserSuppressError
' 版权(c)   Microsoft Corporation.
' 
' 这是这个应用程序的主窗体.它是用来初始化界面并处理事件的.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************'

Partial Public Class MainForm
    Inherits Form

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub MainForm_Load(ByVal sender As Object, ByVal e As EventArgs) _
        Handles MyBase.Load
        ' 初始化浏览器的ScriptErrorsSuppressed和HtmlElementErrorsSuppressed属性.
        wbcSample.ScriptErrorsSuppressed = chkSuppressAllDialog.Checked
        wbcSample.HtmlElementErrorsSuppressed = chkSuppressHtmlElementError.Checked

        ' 给Web浏览器控件的链接错误事件添加一个处理程序.
        AddHandler wbcSample.NavigateError, AddressOf wbcSample_NavigateError

        ' 获取键值HKEY_CURRENT_USER\Software\Microsoft\Internet Explorer\Main中的
        ' DisableJITDebugger的当前值
        chkSuppressJITDebugger.Checked =
            VBWebBrowserSuppressError.WebBrowserEx.JITDebuggerDisabled

        AddHandler chkSuppressJITDebugger.CheckedChanged,
            AddressOf chkSuppressJITDebugger_CheckedChanged
    End Sub


    ''' <summary>
    ''' 处理btnNavigate_Click事件.
    ''' 如果tbUrl的内容不为空,则会链接到填写的url,否则链接到内建的Error.htm页面.
    ''' </summary>
    Private Sub btnNavigate_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles btnNavigate.Click
        Try
            If Not String.IsNullOrEmpty(tbUrl.Text.Trim()) Then
                wbcSample.Navigate(tbUrl.Text)
            Else
                wbcSample.Navigate(Environment.CurrentDirectory & "\HTMLPages\Error.htm")
            End If
        Catch e1 As ArgumentException
            MessageBox.Show("请确保您输入了有效的url.")
        End Try
    End Sub


    ''' <summary>
    ''' 如果chkSuppressJITDebugger.Checked的值改变,则启用或禁用Web浏览器的JIT调试器.
    ''' </summary>
    Private Sub chkSuppressJITDebugger_CheckedChanged(ByVal sender As Object,
                                                     ByVal e As EventArgs)
        WebBrowserEx.JITDebuggerDisabled = chkSuppressJITDebugger.Checked
        MessageBox.Show("您必须重启应用程序来启用或禁用脚本调试器.")
    End Sub

    ''' <summary>
    ''' 如果chkSuppressHtmlElementError.Checked的值改变,则设置Web浏览器属性
    ''' HtmlElementErrorsSuppressed的值.
    ''' </summary>
    Private Sub chkSuppressHtmlElementError_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) _
                                                 Handles chkSuppressHtmlElementError.CheckedChanged
        wbcSample.HtmlElementErrorsSuppressed = chkSuppressHtmlElementError.Checked
    End Sub

    ''' <summary>
    ''' 如果chkSuppressAllDialog.Checked的值改变,则设置Web浏览器属性
    ''' ScriptErrorsSuppressed的值
    ''' </summary>
    Private Sub chkSuppressAllDialog_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) _
                                                Handles chkSuppressAllDialog.CheckedChanged
        wbcSample.ScriptErrorsSuppressed = chkSuppressAllDialog.Checked
    End Sub

    ''' <summary>
    ''' 处理链接错误.
    ''' </summary>
    Private Sub wbcSample_NavigateError(ByVal sender As Object,
                                        ByVal e As WebBrowserNavigateErrorEventArgs)

        ' 如果http状态代码是404,则链接到内建的404.htm页面.
        If chkSuppressNavigationError.Checked AndAlso e.StatusCode = 404 Then
            wbcSample.Navigate(String.Format("{0}\HTMLPages\404.htm",
                                             Environment.CurrentDirectory))
        End If
    End Sub

End Class

