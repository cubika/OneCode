/****************************** 模块头 *************************************\
* Module Name:	RestService.cs
* Project:		CSAzureServiceBusSLRest
* Copyright (c) Microsoft Corporation.
* 
* 这是WCF REST Service的服务实现.
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
using System.ServiceModel.Web;

namespace AzureServiceBusSLRest
{
	public class RestService : IRestService
	{
		public Stream DownloadFile(string fileName)
		{
			try
			{
				using (FileStream fs = File.Open(fileName, FileMode.Open, FileAccess.Read))
				{
					MemoryStream stream = new MemoryStream();
					byte[] buffer = new byte[fs.Length];
					fs.Read(buffer, 0, buffer.Length);
					stream.Write(buffer, 0, buffer.Length);
					stream.Position = 0;
					return stream;
				}
			}
			catch (IOException ex)
			{
				WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.NotFound;
				return new MemoryStream();
			}
		}

		public string UploadFile(string fileName, Stream content)
		{
            // 我们不知道HTTP请求流的长度,所以我们必须使用以下代码读出流的长度:
			int bufferSize = 4096;
			int bytesRead = 1;
			int totalBytesRead = 0;
			try
			{
				using (FileStream fileStream = File.Create(fileName))
				{
					byte[] buffer = new byte[bufferSize];
					bytesRead = content.Read(buffer, 0, bufferSize);
					while (bytesRead > 0)
					{
						fileStream.Write(buffer, 0, bytesRead);
						bytesRead = content.Read(buffer, 0, bufferSize);
						totalBytesRead += bytesRead;
					}
				}

                // 按照REST服务的最近实践，当资源被创建时，返回一个201 Created状态代码代理默认的200.
				// 这意味着Silverlight客户端必须使用HTTP栈，替代默认的浏览器HTTP栈使用服务来工作.
				return WriteResponse(HttpStatusCode.Created, "Created.");
			}
			catch (Exception ex)
			{
				return WriteResponse(HttpStatusCode.InternalServerError, ex.Message);
			}
		}

		private static string WriteResponse(HttpStatusCode statusCode, string message)
		{
			WebOperationContext.Current.OutgoingResponse.StatusCode = statusCode;
			return message;
		}

		public Stream GetClientAccessPolicy()
		{
			return this.DownloadFile("clientaccesspolicy.xml");
		}
	}
}
