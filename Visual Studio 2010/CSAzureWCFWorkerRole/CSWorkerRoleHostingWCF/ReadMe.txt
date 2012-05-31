========================================================================
             云服务 : CSAzureWCFWorkerRole 解决方案概述
========================================================================

/////////////////////////////////////////////////////////////////////////////
先决条件:

Microsoft Visual Studio的Windows Azure工具
http://www.microsoft.com/downloads/en/details.aspx?FamilyID=7a1089b6-4050-4307-86c4-9dadaa5ed018

/////////////////////////////////////////////////////////////////////////////
用途：

在有些情形下我们需要自托管WCF服务.如果我们想新建一个基于HTTP的服务,我们需要使
用Web Role.然而对于一个基于TCP的WCF服务，Worker Role是一个更好的选择.考虑到负
载平衡,我们需要注意逻辑和物理监听地址.这个例子的目的是提供一个方便的工作项目,
它将WCF托管在Worker Role里.

这个解决方案包括三个项目::

1. Client项目.它是使用WCF服务的客户端应用程序.
2. CloudService项目.它是一个一般的拥有一个Worker Role的云服务.
3. CSWorkerRoleHostingWCF项目.它是这个解决方案的关键项目,它阐述了怎样在Worker Role
里托管WCF.

在CSWorkerRoleHostingWCF项目中从WCF服务暴露的两个端点:
1. 一个元数据的端点
2. MyService服务合同的服务端点

两个端点都使用了TCP绑定.

/////////////////////////////////////////////////////////////////////////////
CSWorkerRoleHostingWCF项目的代码逻辑:

1. 获得当地IP地址和虚拟机的当地监听端口:

string ip = RoleEnvironment.CurrentRoleInstance.InstanceEndpoints["tcpinput"].IPEndpoint.Address.ToString();
int tcpport = RoleEnvironment.CurrentRoleInstance.InstanceEndpoints["tcpinput"].IPEndpoint.Port;
int mexport = RoleEnvironment.CurrentRoleInstance.InstanceEndpoints["mexinput"].IPEndpoint.Port;

2. 添加一个元数据TCP端点.逻辑监听端口是8001.客户端应当使用这个端口来请求元数据.
物理端口是我们在第1步中获得的mexport.

ServiceMetadataBehavior metadatabehavior = new ServiceMetadataBehavior();
host.Description.Behaviors.Add(metadatabehavior);
Binding mexBinding = MetadataExchangeBindings.CreateMexTcpBinding();
string mexlistenurl = string.Format("net.tcp://{0}:{1}/MyServiceMetaDataEndpoint", ip, mexport);
string mexendpointurl = string.Format("net.tcp://{0}:{1}/MyServiceMetaDataEndpoint", RoleEnvironment.GetConfigurationSettingValue("Domain"), 8001);
host.AddServiceEndpoint(typeof(IMetadataExchange), mexBinding, mexendpointurl, new Uri(mexlistenurl));

3. 为 MyService添加一个TCP端点.逻辑监听端口是9001.客户端应当使用这个端口来发送请求.
物理端口是我们在第1步中获得的tcpport.

string listenurl = string.Format("net.tcp://{0}:{1}/MyServiceMetaDataEndpointEndpoint", ip, tcpport);
string endpointurl = string.Format("net.tcp://{0}:{1}/MyServiceMetaDataEndpointEndpoint", RoleEnvironment.GetConfigurationSettingValue("Domain"), 9001);
host.AddServiceEndpoint(typeof(IMyService), new NetTcpBinding(SecurityMode.None), endpointurl, new Uri(listenurl));

/////////////////////////////////////////////////////////////////////////////
代码测试:

A. 在Compute Emulator中:

1. 将CloudService设为启动项目.
2. 按F5开始调试.
3. 在Client项目中运行Client.exe或者调试Client项目.

注意：如果你想新建自己的代理类,当你在Client项目中添加服务引用时你输入地元数据
端点应当是net.tcp://localhost:8001/MyServiceMetaDataEndpoint.

B. 在云服务部署之后:

1.请将CloudService项目的ServiceConfiguration.cscfg里的设置改为:

    <Setting name="Domain" value="[yourdomain.cloudapp.net]" />

2.请将Client项目的app.config里的设置改为:

<client>
            <endpoint address="net.tcp://[yourdomain.cloudapp.net]:9001/MyServiceEndpoint" binding="netTcpBinding"
                bindingConfiguration="NetTcpBinding_IMyService" contract="ServiceReference1.IMyService"
                name="NetTcpBinding_IMyService" />
</client>

注意元数据端点应该是net.tcp://[yourdomain.cloudapp.net]:8001/MyServiceMetaDataEndpoint.
    
/////////////////////////////////////////////////////////////////////////////
相关资料:

服务定义模式
http://msdn.microsoft.com/zh-cn/library/ee758711.aspx

/////////////////////////////////////////////////////////////////////////////