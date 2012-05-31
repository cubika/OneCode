'****************************** 模块头 *************************************\
' Module Name:	MainPage.xaml.vb
' Project:		VBAzureServiceBusSLRest
' Copyright (c) Microsoft Corporation.
' 
' 这是Silverlight客户端.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************/

Imports System.Net.Browser
Imports System.IO

Partial Public Class MainPage
	Inherits UserControl

    ' 更改为你的命名空间.
    Private Const ServiceNamespace As String = "[namespace]"

	Private request As HttpWebRequest
	Private fs As FileStream

	Public Sub New()
		InitializeComponent()
	End Sub

	Private Sub UserControl_Loaded(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' 切换到客户端HTTP栈，利用功能（例如状态代码）
		WebRequest.RegisterPrefix("http://", WebRequestCreator.ClientHttp)
		WebRequest.RegisterPrefix("https://", WebRequestCreator.ClientHttp)
	End Sub

	Private Sub UploadButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
		Dim ofd As New OpenFileDialog
		If ofd.ShowDialog.Value Then
			Me.FileNameTextBox.Text = ofd.File.Name
			Me.fs = ofd.File.OpenRead

            ' 为了使示例可以运行大量文件，请配置webHttpRelayBinding的max***属性。
            ' 为了使代码更易懂，我们使用默认设置.
            ' WCF配置将在一个单独的WCF示例中演示.
			If (fs.Length >= 65535) Then
                MessageBox.Show("文件过大，无法使用默认WCF设置发送.")
			Else
                Me.infoTextBlock.Text = "上传文件. 请等待..."
				Me.infoTextBlock.Visibility = Visibility.Visible

                ' 利用HTTP POST调用WCF REST服务.
				Me.request = DirectCast(WebRequest.Create(("https://" & ServiceNamespace & ".servicebus.windows.net/file/" & ofd.File.Name)), HttpWebRequest)
				request.Method = "POST"
				request.ContentType = "application/octet-stream"
				request.BeginGetRequestStream(AddressOf BeginGetRequestStream_Complete, Nothing)
			End If
        End If
    End Sub

	' VB 9 doesn't support anonymous lambda expressions, so we have to write a seperate callback method...
	Sub BeginGetRequestStream_Complete(ByVal result1 As IAsyncResult)
		Dim requestStream As Stream = request.EndGetRequestStream(result1)
		Dim buffer As Byte() = New Byte(fs.Length - 1) {}
		fs.Read(buffer, 0, buffer.Length)
		requestStream.Write(buffer, 0, buffer.Length)
		requestStream.Close()
		request.BeginGetResponse(AddressOf BeginGetResponse_Complete, Nothing)
	End Sub

	' VB 9 doesn't support anonymous lambda expressions, so we have to write a seperate callback method...
	Sub BeginGetResponse_Complete(ByVal result2 As IAsyncResult)
		Dim response As HttpWebResponse = DirectCast(request.EndGetResponse(result2), HttpWebResponse)
		Using response.GetResponseStream
			If (response.StatusCode = HttpStatusCode.Created) Then
				Me.Dispatcher.BeginInvoke(AddressOf Dispatcher_BeginInvoke_Complete)
			End If
		End Using
	End Sub

	' VB 9 doesn't support anonymous lambda expressions, so we have to write a seperate callback method...
	Sub Dispatcher_BeginInvoke_Complete()
		Me.infoTextBlock.Visibility = Visibility.Collapsed
        MessageBox.Show("上传成功.")
	End Sub

	Private Sub DownloadButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
		If String.IsNullOrEmpty(Me.FileNameTextBox.Text) Then
            MessageBox.Show("请输入文件名.")
		Else
			Dim sfd As New SaveFileDialog
			If sfd.ShowDialog.Value Then
                Me.infoTextBlock.Text = "下载文件. 请等待..."
				Me.infoTextBlock.Visibility = Visibility.Visible
				Dim stream As Stream = sfd.OpenFile
                ' 利用HTTP GET调用WCF REST服务.
				Dim webClient As New WebClient
				AddHandler webClient.OpenReadCompleted, New OpenReadCompletedEventHandler(AddressOf Me.webClient_OpenReadCompleted)
				webClient.OpenReadAsync(New Uri(("https://" & ServiceNamespace & ".servicebus.windows.net/file/" & Me.FileNameTextBox.Text)), stream)
			End If
		End If
	End Sub

	Private Sub webClient_OpenReadCompleted(ByVal sender As Object, ByVal e As OpenReadCompletedEventArgs)
		Try
			Dim fileStream As Stream = DirectCast(e.UserState, Stream)
			Dim buffer As Byte() = New Byte(e.Result.Length - 1) {}
			e.Result.Read(buffer, 0, buffer.Length)
			fileStream.Write(buffer, 0, buffer.Length)
			fileStream.Close()
			Me.infoTextBlock.Visibility = Visibility.Collapsed
            MessageBox.Show("文件已下载. 请检查已保存文件.")
		Catch
			Me.infoTextBlock.Visibility = Visibility.Collapsed
            MessageBox.Show("文件下载失败. 请检查服务器上是否存在该文件.")
		End Try
	End Sub
End Class