'******************************** 模块头 *********************************\
'* 模块名:   WorkerRole.vb
'* 项目名:   VBWorkerRoleHostingWCF
'* 版权 (c) Microsoft Corporation.
'* 
'* 这个Worker Role托管了一个WCF服务,暴露了两个tcp端点.一个是为元数据,另一个
'* 是为MyService.
'* 
'* This source is subject to the Microsoft Public License.
'* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
'* All other rights reserved.
'* 
'* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
'* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
'* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'\**************************************************************************

Imports System.Net
Imports System.Threading
Imports Microsoft.WindowsAzure.Diagnostics
Imports Microsoft.WindowsAzure.ServiceRuntime
Imports System.ServiceModel
Imports System.ServiceModel.Description
Imports System.ServiceModel.Channels

Public Class WorkerRole
    Inherits RoleEntryPoint

    Public Overrides Sub Run()
        ' 这是一个实现Worker的例子.用你的逻辑替换.

        ' Trace.WriteLine("WorkerRoleHostingWCF entry point called", "Information");

        ' 主要逻辑

        Using host As ServiceHost = New ServiceHost(GetType(MyService))

            Dim ip As String = RoleEnvironment.CurrentRoleInstance.InstanceEndpoints("tcpinput").IPEndpoint.Address.ToString()
            Dim tcpport As Integer = RoleEnvironment.CurrentRoleInstance.InstanceEndpoints("tcpinput").IPEndpoint.Port
            Dim mexport As Integer = RoleEnvironment.CurrentRoleInstance.InstanceEndpoints("mexinput").IPEndpoint.Port

            ' 为客户端代理服务器添加一个metadatabehavior
            ' 元数据通过net.tcp暴露
            Dim metadatabehavior As ServiceMetadataBehavior = New ServiceMetadataBehavior()
            host.Description.Behaviors.Add(metadatabehavior)
            Dim mexBinding As Binding = MetadataExchangeBindings.CreateMexTcpBinding()
            Dim mexlistenurl As String = String.Format("net.tcp://{0}:{1}/MyServiceMetaDataEndpoint", ip, mexport)
            Dim mexendpointurl As String = String.Format("net.tcp://{0}:{1}/MyServiceMetaDataEndpoint", RoleEnvironment.GetConfigurationSettingValue("Domain"), 8001)
            host.AddServiceEndpoint(GetType(IMetadataExchange), mexBinding, mexendpointurl, New Uri(mexlistenurl))

            ' 为MyService添加端点
            Dim listenurl As String = String.Format("net.tcp://{0}:{1}/MyServiceEndpoint", ip, tcpport)
            Dim endpointurl As String = String.Format("net.tcp://{0}:{1}/MyServiceEndpoint", RoleEnvironment.GetConfigurationSettingValue("Domain"), 9001)
            host.AddServiceEndpoint(GetType(IMyService), New NetTcpBinding(SecurityMode.None), endpointurl, New Uri(listenurl))
            host.Open()

            Do
                Thread.Sleep(1800000)
                'Trace.WriteLine("Working", "Information");
            Loop
        End Using
    End Sub



    Public Overrides Function OnStart() As Boolean

        ' 设置最大的并发连接数
        ServicePointManager.DefaultConnectionLimit = 12

        ' 为了避免潜在的错误关闭诊断程序.
        ' DiagnosticMonitor.Start("DiagnosticsConnectionString");

        ' 关于处理配置的变化请参考MSDN,网址为 http://go.microsoft.com/fwlink/?LinkId=166357.
        AddHandler RoleEnvironment.Changing, AddressOf RoleEnvironmentChanging

        Return MyBase.OnStart()

    End Function

    Private Sub RoleEnvironmentChanging(ByVal sender As Object, ByVal e As RoleEnvironmentChangingEventArgs)

        ' 如果一个配置环境发生变化
        If (e.Changes.Any(Function(change) TypeOf change Is RoleEnvironmentConfigurationSettingChange)) Then
            ' 将e.Cancel设为true重启这个角色实例
            e.Cancel = True
        End If

    End Sub

End Class
