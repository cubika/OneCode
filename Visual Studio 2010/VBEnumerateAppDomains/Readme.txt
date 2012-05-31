================================================================================
	   控制台应用程序： 控制台枚举应用程序域 项目概述                        
================================================================================
/////////////////////////////////////////////////////////////////////////////
摘要:

VBEnumerateAppDomains案例阐述了如何使用hosting API和debugging API枚举托管进程及应用程序域。
使用的这些API是非托管的，但是都包含在MdbgCore.dll和mscoree.dll中。

通过检查一个进程在加载CLR时是否使用的是hosting API来判断此进程是否为托管进程。
如果需要枚举进程中的应用程序域，可以使用debugging API给进程附加一个调试器。

注意点：
1.因为无法调试自己的进程，所以如果想要枚举当前进程中的应用程序域，可以使用ICorRuntimeHost接口。
2.在64位操作系统上枚举x86托管进程必须将应用程序运行平台设置成x86。
3.有一些进程不能被附加：
  3.1 一些已经被附加的进程，比如*.exe.vshost。
  3.2 一些进程没有在同步状态下。某些附加操作要求进程处于同步状态下，参考
	  http://msdn.microsoft.com/en-us/library/ms404528.aspx
   
////////////////////////////////////////////////////////////////////////////////

演示：

步骤1. 在Visual Studio 2010中建立示例项目。

步骤2. 运行VBEnumerateAppDomains.exe，此应用将显示以下帮助字幕。

	   请选择一项命令：
	   1: 显示当前进程中的应用程序域.
	   2: 列举所以托管进程.
	   3: 显示帮助字幕.
	   4: 退出程序.
	   显示指定进程的应用程序域，请直接输入“PID”及进程ID，比如PID1234.

步骤3. 输入1和回车键，将显示当前进程中的所有应用程序域。

步骤4. 输入2和回车键，将显示所有正在运行的托管进程。

步骤5. 输入PIDxxxx（xxxx是可以通过步骤4显示的进程ID，比如PID1234），将显示指定进程中的所有
应用程序域。

可以使用以下代码在测试程序中创建一个应用程序域。
var newDomain = AppDomain.CreateDomain("Hello World!");

/////////////////////////////////////////////////////////////////////////////
创建过称:

步骤1. 将mdbgcore.dll复制到_External_Dependencies文件夹中，并把此程序集添加到项目引用中去。
	   
	   此程序集是Windows SDK中的一部分。安装了VS2010后，此程序集将出现在
	   C:\Program Files\Microsoft SDKs\Windows\v7.0A\Bin\ (32 bit OS)或
	   C:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\Bin\x64(64 bit OS)路径下。
	   也可通过以下链接下载最新的Windows SDK：
	   http://www.microsoft.com/downloads/en/details.aspx?FamilyID=35AEDA01-421D-4BA5-B44B-543DC8C33A20

步骤2. 添加一个COM引用: Common Language Runtime Execution Engine 2.4 Library。

步骤3. 此程序支持2种方式启动运行。一种按照演示运行，另一种通过指令，比如：
	   VBEnumerateAppDomains.exe CurrentProcess
       VBEnumerateAppDomains.exe ListAllManagedProcesses
       VBEnumerateAppDomains.exe PID1234

步骤4. 使用mscoree.ICorRuntimeHost显示当前进程中的应用程序域。

步骤5. 使用CLRMetaHost枚举进程已加载的运行时。如果一个进程加载了一次或者多次CLR，就被认为是托管进程。

步骤6. 使用MDbgEngine给指定进程附加一个调试器，并枚举应用程序域。

/////////////////////////////////////////////////////////////////////////////
参考资料:
http://msdn.microsoft.com/en-us/library/ms164320.aspx
http://msdn.microsoft.com/en-us/library/dd233134.aspx
http://msdn.microsoft.com/en-us/library/ms404520.aspx
/////////////////////////////////////////////////////////////////////////////