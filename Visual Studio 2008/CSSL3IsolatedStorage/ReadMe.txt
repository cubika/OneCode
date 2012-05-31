========================================================================
    SILVERLIGHT应用程序 : CSSL3IsolatedStorage项目概述
========================================================================

/////////////////////////////////////////////////////////////////////////////
用法:

本项目创建一个独立存储器浏览器。使用这个浏览器，用户能查看应用程序独立存储器
的虚拟文件结构，也提供如下的文件管理功能：

    创建目录
    上传本地文件到独立存储器
    打开和播放储存在独立存储器中的媒体流
    删除目录/文件
    增加独立存储器的磁盘配额
    从独立存储器保存文件到本地
    使用独立存储器设置IsolatedStorageSettings存储/加载配置
    
    
/////////////////////////////////////////////////////////////////////////////
先决条件:

Visual Studio 2008 SP1 的Silverlight3工具
http://www.microsoft.com/downloads/details.aspx?familyid=9442b0f2-7465-417a-88f3-5e7b5409e9dd&displaylang=en

Silverilght 3 运行时:
http://silverlight.net/getstarted/silverlight3


/////////////////////////////////////////////////////////////////////////////
演示:

要运行本示例, 请尝试如下步骤:
   1. 打开CSSL3IsolatedStorage项目, 编译并运行.
   2. 在应用程序用户界面中第三段显示，上次打开应用程序的时间。
	这个信息是保存在IsolatedStorageSettings中。
   3. 按下"增加10MB空间" 按钮, 应用程序存储器空间就增加10MB。
   4. 在树目录控件中，可以增加，删除和打开文件。
   它将会使用应用程序接口操作存储在独立存储器中的文件。


/////////////////////////////////////////////////////////////////////////////
代码逻辑:

1: 独立存储器查看模型是怎样工作的？
    1. 为独立虚拟文件和目录定义实体。
    2. 在树目录中，使用HierarchicalDataTemplate绑定数据到树目录结点。
    3. 当应用程序开始运行，遍历独立存储器，使用定义的实体创建树目录查看模型
	然后设为树目录的子项数据源itemssource.

2: 怎样上传文件到独立存储？
    1. 使用OpenFileDialog取得本地可读文件流。
    2. 取得应用程序的IsolatedStorageFile, 然后使用CreateFile方法取得一个
	可写的IsolatedStorageStream.
    3. 复制字节从文件流到独立存储器流。
    4. 关闭流。
    
    注意： 复制文件流非常耗时，在后台用另外的线程使用BackgroundWorker运行它，
	会取得更好效果。 关于BackgroundWorker的详情, 请查看
       http://msdn.microsoft.com/en-us/library/cc221403(VS.95).aspx

3. 怎样打开和释放独立存储器中的媒体流？
    1. 打开独立存储器流。
       IsolatedStorageFile _isofile = IsolatedStorageFile.GetUserStoreForApplication();
       var stream = _isofile.OpenFile(item.FilePath, FileMode.Open, FileAccess.Read);
       
    2. 设置给MediaElement, 播放.
       mePlayer.SetSource(stream);

4. 怎样使用IsolatedStorageSetting存储配置？
    IsolatedStorageSettings是存储在独立存储器和由Silverlight维护的目录。
	是存储配置的好地方。
    
    为了被应用程序使用把数据存储到IsolatedStorageSetting， 使用这个：
        solatedStorageSettings.ApplicationSettings["keyname"] = data;

    如果需要数据被整个站点使用，使用
        solatedStorageSettings.SiteSettings["keyname"] = data;
     
5. 在应用程序中，为什么目录深度应该小于4？
	独立存储器限制目录名应该小于248个字符，	完整文件路径应该小于260个字符。
	如果创建目录深度更大的话，路径长度就会超过限制。


/////////////////////////////////////////////////////////////////////////////
参考资料:

独立存储器
http://msdn.microsoft.com/en-us/library/bdts8hk0(VS.95).aspx


/////////////////////////////////////////////////////////////////////////////
