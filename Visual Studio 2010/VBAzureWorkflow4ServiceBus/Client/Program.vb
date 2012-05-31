'****************************** 模块头 *************************************\
' Module Name:	Program.vb
' Project:		Client
' Copyright (c) Microsoft Corporation.
' 
' 该文件是检测workflow service工作良好的客户端程序.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************/

Imports System.ServiceModel
Imports Microsoft.ServiceBus

Module Program

	Sub Main()
        ' 将{service namespace}替换为你的服务命名空间.
		Dim address As New EndpointAddress(ServiceBusEnvironment.CreateServiceUri("https", "{service namespace}", "ProcessDataWorkflowService"))
		Dim binding As New BasicHttpRelayBinding

        ' 提供Service Bus证书.
		Dim sharedSecretServiceBusCredential As New TransportClientEndpointBehavior
		sharedSecretServiceBusCredential.CredentialType = TransportClientCredentialType.SharedSecret
        ' 将{issuer name}和{issuer secret}替换为你的证书.
		sharedSecretServiceBusCredential.Credentials.SharedSecret.IssuerName = "{issuer name}"
		sharedSecretServiceBusCredential.Credentials.SharedSecret.IssuerSecret = "{issuer secret}"
		Dim factory As New ChannelFactory(Of IProcessDataWorkflowServiceChannel)(binding, address)
		factory.Endpoint.Behaviors.Add(sharedSecretServiceBusCredential)
		factory.Open()
		Dim channel As IProcessDataWorkflowServiceChannel = factory.CreateChannel
		channel.Open()
        Console.WriteLine("正在处理 10...")
        Console.WriteLine(("服务返回: " & channel.ProcessData(New ProcessDataRequest(0)).string))
        Console.WriteLine("正在处理 30...")
        Console.WriteLine(("服务返回: " & channel.ProcessData(New ProcessDataRequest(30)).string))
		channel.Close()
        factory.Close()
        Console.Read()
	End Sub

End Module
