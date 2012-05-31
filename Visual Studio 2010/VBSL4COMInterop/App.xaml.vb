 Partial Public Class App
    Inherits Application

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub Application_Startup(ByVal o As Object, ByVal e As StartupEventArgs) Handles Me.Startup
        Me.RootVisual = New MainPage()
    End Sub

    Private Sub Application_Exit(ByVal o As Object, ByVal e As EventArgs) Handles Me.Exit

    End Sub

    Private Sub Application_UnhandledException(ByVal sender As Object, ByVal e As ApplicationUnhandledExceptionEventArgs) Handles Me.UnhandledException

        ' 如果app在调试程序之外运行，这会导致使用浏览器的异常机制报告异常.在IE状态栏里将它显示为
        ' 一个黄色的警告图标，在Firefox里显示脚本错误.

        If Not System.Diagnostics.Debugger.IsAttached Then

            ' 注意: 这会允许应用程序在抛出异常但是没有处理之后继续运行. 
            ' 为了生成应用程序这个错误处理应该被那种能够向网站报告错误并停止运行的错误处理方式所代替.
            e.Handled = True
            Deployment.Current.Dispatcher.BeginInvoke(New Action(Of ApplicationUnhandledExceptionEventArgs)(AddressOf ReportErrorToDOM), e)
        End If
    End Sub

    Private Sub ReportErrorToDOM(ByVal e As ApplicationUnhandledExceptionEventArgs)

        Try
            Dim errorMsg As String = e.ExceptionObject.Message + e.ExceptionObject.StackTrace
            errorMsg = errorMsg.Replace(""""c, "'"c).Replace(ChrW(13) & ChrW(10), "\n")

            System.Windows.Browser.HtmlPage.Window.Eval("throw new Error(""Silverlight应用程序中未处理的错误" + errorMsg + """);")
        Catch

        End Try
    End Sub

End Class
