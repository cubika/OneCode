===============================================================================
  ASP.NET 应用程序 : VBASPNETEncryptAndDecryptConfiguration 项目 概述
===============================================================================
///////////////////////////////////////////////////////////////////////////////
用法:

此示例演示如何使用RSA加密算法API的加密和解密配置节点,以保护ASP.NET Web应用程序的敏感
信息,防止拦截或劫持.

这个项目包含两个片段.第一个演示了如何使用RSA提供和RSA容器中进行加密和解密一些Web应用
程序中的词或值.第一个片段的目的是让我们知道RSA的机制概述.第二个则显示了如何使用RSA
​​配置提供加密和解密web.config中的配置节点.

///////////////////////////////////////////////////////////////////////////////
前提条件:

如果您的应用程序没有Web.config文件,请创建一个.还可以指定一些节点，如在这个web.config中
appSetting,connectSetting.

如何为 ASP.NET 应用程序创建 Web.config 文件:
http://support.microsoft.com/kb/815179/

使用 Web.config 文件:
http://msdn.microsoft.com/zh-cn/library/ms460914.aspx

/////////////////////////////////////////////////////////////////////////////

开始这个项目:

1.以管理员模式启动VBASPNETEncryptAndDecryptConfiguration.sln.


2.右击CommonEncryption.aspx或ConfigurationEncryption.aspx. 在菜单中选择
 "在浏览器中查看" 选项.

  CommonEncrytion:
    1) 在上面的文本框中输入一些值.点击这个文本框下面的“加密”按钮.
    您可以在多行文本框察看RSA加密结果字符串.
    2) 然后，您可以点击这个页面另一个名为“解密”的按钮.
    解密结果将显示在旁边的多行文本框.你会发现这个字符串是等于最初在顶部的文本框输入的值.

  ConfigurationEncryption:
    1) 在下拉列表中选择一个配置节点.
    2) 单击下面的“加密”按钮.如果加密成功,打开web.config文件,你会发现的指定节点
    被RSA数据加密和替换。
    3) 如果你要恢复此节点为纯文本.点击“解密”按钮,并再次检查web.config.

  备注: 如果您正在运行此应用程序从文件系统,当您关闭应用程序时,Visual Studio将显示
    一个对话框"文件已在编辑器外被修改.你是否希望重新加载它？"单击是并
    查看web.config.


/////////////////////////////////////////////////////////////////////////////

代码逻辑:

CommonEncrytion:

1. 创建一个CspParameters类的新实例,并传递您要呼叫的密钥容器名称到CspParameters.KeyContainerName域。
2. 创建一个新的RSACryptoServiceProvider实例,并传递CsParameter到它的构造器.
3. 创建字节数组来保存原始,加密和解密数据.
4. 通过字节数组的数据加密方法和加密字节数组得到的结果.
5. 这个字节数组数据转换为其等效的字符串,通过使用Convert.ToBase64String显示在多行文本框中.

ConfigurationEncryption:

1. 获取下拉列表的选定值决定对哪个配置节进行加密或解密.
2. 打开此Web应用程序的web.config.
3. 查找的特定节点,并使用RSAProtectedConfigurationProvider加密或解密.
4. 如果成功,在web.config中此节点将通过RSA加密,取而代之的是一些RSA的节点.


备注: 如果存储在下列配置节点中的任何敏感数据,你不能
 使用受保护配置提供程序和Aspnet_regiis.exe工具加密:

<processModel>
<runtime>
<mscorlib>
<startup>
<system.runtime.remoting>
<configProtectedData>
<satelliteassemblies>
<cryptographySettings>
<cryptoNameMapping>
<cryptoClasses>

RSAProtectedConfigurationProvider支持机器级和用户级密钥容器
存储密钥.在这个项目中,我们都使用机器级.

理解机器级和用户级RSA密钥容器:
http://msdn.microsoft.com/zh-cn/library/f5cs0acs.aspx

没有使用RSA提供的API,我们也可以使用Aspnet_regiis.exe工具来加密和解密节点.
http://msdn.microsoft.com/zh-cn/library/ms998283.aspx

/////////////////////////////////////////////////////////////////////////////

参考资料:

RSACryptoServiceProvider
http://msdn.microsoft.com/zh-cn/library/system.security.cryptography.rsacryptoserviceprovider(VS.80).aspx

CspParameters
http://msdn.microsoft.com/zh-cn/library/system.security.cryptography.cspparameters(VS.80).aspx


ConfigurationSection
http://msdn.microsoft.com/zh-cn/library/system.configuration.configurationsection.aspx

SectionInformation.ProtectSection 
http://msdn.microsoft.com/zh-cn/library/system.configuration.sectioninformation.protectsection.aspx


/////////////////////////////////////////////////////////////////////////////





