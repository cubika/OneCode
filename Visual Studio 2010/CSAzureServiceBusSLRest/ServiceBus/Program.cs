/****************************** 模块头 *************************************\
* Module Name:	Program.cs
* Project:		CSAzureServiceBusSLRest
* Copyright (c) Microsoft Corporation.
* 
* 该文件是程序主入口，托管了服务，使用Service Bus将它公开到互联网。
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
using System.Configuration;
using System.ServiceModel.Web;
using Microsoft.ServiceBus;

namespace AzureServiceBusSLRest
{
	class Program
	{
		static void Main(string[] args)
		{
			string serviceNamespace = ConfigurationManager.AppSettings["serviceNamespace"];
            // 默认情况下，所有的*RelayBindings都需要传输等级安全；因此，需要使用https.
			Uri address = ServiceBusEnvironment.CreateServiceUri("https", serviceNamespace, "");
			WebServiceHost host = new WebServiceHost(typeof(RestService), address);
			host.Open();

			Console.WriteLine("复制下面的地址到浏览器中，查看跨域策略文件:");
			Console.WriteLine(address + "clientaccesspolicy.xml");
			Console.WriteLine();
            Console.WriteLine("WCF REST服务开始监听:");
			Console.WriteLine(address + "file/");
			Console.WriteLine();
			Console.WriteLine("按[Enter]退出");
			Console.ReadLine();
			host.Close();
		}
	}
}
