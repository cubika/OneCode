/****************************** 模块头 *************************************\
* Module Name:	MainPage.xaml.cs
* Project:		CSAzureServiceBusSLRest
* Copyright (c) Microsoft Corporation.
* 
* 这是Silverlight客户端.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.IO;
using System.Net;
using System.Net.Browser;
using System.Windows;
using System.Windows.Controls;

namespace Client
{
	public partial class MainPage : UserControl
	{
        // 更改为你的命名空间.
		private const string ServiceNamespace = "[namespace]";

		public MainPage()
		{
			InitializeComponent();
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
            // 切换到客户端HTTP栈，利用功能（例如状态代码）
			HttpWebRequest.RegisterPrefix("http://", WebRequestCreator.ClientHttp);
			HttpWebRequest.RegisterPrefix("https://", WebRequestCreator.ClientHttp);
		}

		private void UploadButton_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			if (ofd.ShowDialog().Value)
			{
				this.FileNameTextBox.Text = ofd.File.Name;
				FileStream fs = ofd.File.OpenRead();

                // 为了使示例可以运行大量文件，请配置webHttpRelayBinding的max***属性。
				// 为了使代码更易懂，我们使用默认设置.
				// WCF配置将在一个单独的WCF示例中演示.
				if (fs.Length >= 65535)
				{
					MessageBox.Show("文件过大，无法使用默认WCF设置发送.");
					return;
				}
				this.infoTextBlock.Text = "上传文件. 请等待...";
				this.infoTextBlock.Visibility = Visibility.Visible;

                // 利用HTTP POST调用WCF REST服务.
				HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("https://" + ServiceNamespace + ".servicebus.windows.net/file/" + ofd.File.Name);
				request.Method = "POST";
				request.ContentType = "application/octet-stream";
				request.BeginGetRequestStream(result1 =>
				{
					Stream requestStream = request.EndGetRequestStream(result1);
					byte[] buffer = new byte[fs.Length];
					fs.Read(buffer, 0, buffer.Length);
					requestStream.Write(buffer, 0, buffer.Length);
					requestStream.Close();
					request.BeginGetResponse(result2 =>
					{
						HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(result2);
						using (Stream responseStream = response.GetResponseStream())
						{
							if (response.StatusCode == HttpStatusCode.Created)
							{
								this.Dispatcher.BeginInvoke(() =>
								{
									this.infoTextBlock.Visibility = Visibility.Collapsed;
									MessageBox.Show("上传成功.");
								});
							}
						}
					}, null);
				}, null);
			}
		}

		private void DownloadButton_Click(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrEmpty(this.FileNameTextBox.Text))
			{
				MessageBox.Show("请输入文件名.");
				return;
			}
			SaveFileDialog sfd = new SaveFileDialog();
			if (sfd.ShowDialog().Value)
			{
				this.infoTextBlock.Text = "下载文件. 请等待...";
				this.infoTextBlock.Visibility = Visibility.Visible;
				Stream stream = sfd.OpenFile();
                // 利用HTTP GET调用WCF REST服务.
				WebClient webClient = new WebClient();
				webClient.OpenReadCompleted += new OpenReadCompletedEventHandler(webClient_OpenReadCompleted);
				webClient.OpenReadAsync(new Uri("https://" + ServiceNamespace + ".servicebus.windows.net/file/" + this.FileNameTextBox.Text), stream);
			}
		}

		void webClient_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
		{
			try
			{
				Stream fileStream = (Stream)e.UserState;
				byte[] buffer = new byte[e.Result.Length];
				e.Result.Read(buffer, 0, buffer.Length);
				fileStream.Write(buffer, 0, buffer.Length);
				fileStream.Close();
				this.infoTextBlock.Visibility = Visibility.Collapsed;
				MessageBox.Show("文件已下载. 请检查已保存文件.");
			}
			catch
			{
				this.infoTextBlock.Visibility = Visibility.Collapsed;
				MessageBox.Show("文件下载失败. 请检查服务器上是否存在该文件.");
			}
		}
	}
}
