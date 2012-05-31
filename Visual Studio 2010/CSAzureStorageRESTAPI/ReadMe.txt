========================================================================
    AZURESTORAGE : CSAzureStorageRESTAPI 解决方案概述
========================================================================

/////////////////////////////////////////////////////////////////////////////
用途:

有时我们需要使用原始的REST API而不是使用SDK. i.e提供的StorageClient类.StorageClient
类不使用视图插入一个实体到表存储、用其它的编程语言写"StorageClient"库等等.这个示例
展示了怎样使用List Blobs API来产生一个HTTP消息.你可以再利用这个代码添加认证报头来调
用其它REST APIs.

/////////////////////////////////////////////////////////////////////////////
先决条件:

Microsoft Visual Studio的Windows Azure工具
http://www.microsoft.com/downloads/en/details.aspx?FamilyID=7a1089b6-4050-4307-86c4-9dadaa5ed018

/////////////////////////////////////////////////////////////////////////////
代码测试:

A. 确保Storage Emulator正在运行.

B. 开始调试.

C. 在Blob存储中输入一个容器的名称并按<ENTER>键.

D. 观察输出，它列出了那个容器中的所有Blob信息.

/////////////////////////////////////////////////////////////////////////////
相关资料:

存储模拟器和Windows Azure存储服务之间的不同点
http://msdn.microsoft.com/zh-cn/library/gg433135.aspx

列表Blob
http://msdn.microsoft.com/zh-cn/library/dd135734.aspx

认证方案
http://msdn.microsoft.com/zh-cn/library/dd179428.aspx

/////////////////////////////////////////////////////////////////////////////