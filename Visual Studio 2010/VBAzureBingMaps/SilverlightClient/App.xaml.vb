'/********************************* 模块头 **********************************\
'* 模块名:  App.xaml.vb
'* 项目名:  AzureBingMaps
'* 版权 (c) Microsoft Corporation.
'* 
'* App后台代码.
'* 
'* This source is subject to the Microsoft Public License.
'* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
'* All other rights reserved.
'* 
'* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
'* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
'* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'\***************************************************************************/

Imports System.Windows

Partial Public Class App
    Inherits Application
    Friend Shared IsAuthenticated As Boolean = False
    Friend Shared WelcomeMessage As String = Nothing

    Public Sub New()
        AddHandler Me.Startup, AddressOf Me.Application_Startup
        AddHandler Me.[Exit], AddressOf Me.Application_Exit
        AddHandler Me.UnhandledException, AddressOf Me.Application_UnhandledException

        InitializeComponent()
    End Sub

    Private Sub Application_Startup(ByVal sender As Object, ByVal e As StartupEventArgs)
        IsAuthenticated = Boolean.Parse(e.InitParams("IsAuthenticated"))
        If IsAuthenticated Then
            WelcomeMessage = e.InitParams("WelcomeMessage")
        End If
        Me.RootVisual = New MainPage()
    End Sub

    Private Sub Application_Exit(ByVal sender As Object, ByVal e As EventArgs)

    End Sub

    Private Sub Application_UnhandledException(ByVal sender As Object, ByVal e As ApplicationUnhandledExceptionEventArgs)
        ' 如果程序运行在调试器外部异常报告将使用浏览器的异常处理机制. 
        ' 在IE的场合将会在状态栏显示一个黄色的警告图标
        ' Firefox将显示一个脚本错误.
        If Not System.Diagnostics.Debugger.IsAttached Then

            ' 备注: 这会使应用程序在异常已被抛出但未被捕捉时继续运行. 
            ' 产品级应用程序的场合这个错误处理必须被替换为报告错误到网站并中止应用程序.
            e.Handled = True
            Deployment.Current.Dispatcher.BeginInvoke(Function()
                                                          ReportErrorToDOM(e)
                                                          Return Nothing
                                                      End Function)
        End If
		End Sub

    Private Sub ReportErrorToDOM(ByVal e As ApplicationUnhandledExceptionEventArgs)
        Try
            Dim errorMsg As String = e.ExceptionObject.Message + e.ExceptionObject.StackTrace
            errorMsg = errorMsg.Replace(""""c, "'"c).Replace(vbCr & vbLf, "\n")

            System.Windows.Browser.HtmlPage.Window.Eval("throw new Error(""Unhandled Error in Silverlight Application " & errorMsg & """);")
        Catch generatedExceptionName As Exception
        End Try
    End Sub
End Class