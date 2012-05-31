/****************************** 模块头 *************************************\
* Module Name:	Program.cs
* Project:		Client
* Copyright (c) Microsoft Corporation.
* 
* 该文件是检测workflow service工作良好的客户端程序.
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Microsoft.ServiceBus;

namespace Client
{
	class Program
	{
		static void Main(string[] args)
		{
			// 将{service namespace}替换为你的服务命名空间.
			EndpointAddress address = new EndpointAddress(ServiceBusEnvironment.CreateServiceUri("https", "{service namespace}", "ProcessDataWorkflowService"));
			BasicHttpRelayBinding binding = new BasicHttpRelayBinding();

			// 提供Service Bus证书.
			TransportClientEndpointBehavior sharedSecretServiceBusCredential = new TransportClientEndpointBehavior();
			sharedSecretServiceBusCredential.CredentialType = TransportClientCredentialType.SharedSecret;
			// 将{issuer name}和{issuer secret}替换为你的证书.
			sharedSecretServiceBusCredential.Credentials.SharedSecret.IssuerName = "{issuer name}";
			sharedSecretServiceBusCredential.Credentials.SharedSecret.IssuerSecret = "{issuer secret}";

			ChannelFactory<IProcessDataWorkflowServiceChannel> factory = new ChannelFactory<IProcessDataWorkflowServiceChannel>(binding, address);
			factory.Endpoint.Behaviors.Add(sharedSecretServiceBusCredential);
			factory.Open();
			IProcessDataWorkflowServiceChannel channel = factory.CreateChannel();
			channel.Open();
			Console.WriteLine("正在处理 10...");
			Console.WriteLine("服务返回: " + channel.ProcessData(new ProcessDataRequest(0)).@string);
			Console.WriteLine("正在处理 30...");
			Console.WriteLine("服务返回: " + channel.ProcessData(new ProcessDataRequest(30)).@string);
			channel.Close();
			factory.Close();
            Console.Read();
		}
	}
}
