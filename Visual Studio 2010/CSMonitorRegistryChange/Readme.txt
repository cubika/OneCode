================================================================================
       Windows应用程序: CSMonitorRegistryChange 项目概述                        
===============================================================================
/////////////////////////////////////////////////////////////////////////////
摘要:
CSMonitorRegistryChange案例阐述了如何使用WMI事件监控注册表项变更事件。当下面的操作
发生时，变更事件将会被引发。
1 重命名或删除项。
2 添加，重命名或删除一个项下面的子项。
3 添加，重命名或删除一个项的值。

这个WMI事件不会返回改变了的值或类型。它只是报告这里有了一个变更。你可以从事件中获得的
属性有Hive，KeyPath，SECURITY_DESCRIPTOR以及TIME_CREATED。

////////////////////////////////////////////////////////////////////////////////
演示:
步骤1 在Visual Studio 2010中生成示例项目。

步骤2 在comboBox中选择一个节点“HKEY_LOCAL_MACHINE”，然后在文本框中输入项的路径
“SOFTWARE\\Microsoft”。
注意，你需要在注册表路径中使用双斜线 “\\”。

步骤3 点击 “启动监视器” 按钮。

步骤4 在运行命令中输入“Regedit”来打开注册表编辑器。

步骤5 在注册表编辑器中，导航到HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft。右键点击这一
项并创建一个新的项。你会在列表中看到一个新的子项目“The key HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft changed”。


/////////////////////////////////////////////////////////////////////////////
代码逻辑:

首先，初始化包含有所有支持节点的combobox组件cmbHives。只有 HKEY_LOCAL_MACHINE，
HKEY_USERS，以及HKEY_CURRENT_CONFIG是被RegistryEvent或它的派生类，例如RegistryKeyChangeEvent
所支持的。

第二步，当用户输入了项路径并点击启动监视器按钮，会创建一个新的RegistryWatcher实例。RegistryWatcher
的构造器将会检查项是否存在或用户是否拥有进入项的许可，然后构造一个WqlEventQuery。

第三步，创建一个句柄来监听RegistryWatcher的RegistryKeyChangeEvent。

最后，当注册表变更事件到达后，在一个listbox中显示通知。


/////////////////////////////////////////////////////////////////////////////
参考资料:
http://msdn.microsoft.com/en-us/library/aa393040(VS.85).aspx
http://msdn.microsoft.com/en-us/library/aa392388(VS.85).aspx
http://www.codeproject.com/KB/system/WMI_RegistryMonitor.aspx
/////////////////////////////////////////////////////////////////////////////