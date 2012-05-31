========================================================================
	      Windows应用程序: CSProcessWatcher 概述
========================================================================

/////////////////////////////////////////////////////////////////////////////
摘要:

这个实例演示如何使用Windows Management Instrumentation(WMI)来检测进程的创建/修改/关闭事件.

/////////////////////////////////////////////////////////////////////////////
演示: 

请遵守以下所列出的步骤.

步骤 1. 在命令提示符中运行CSProcessWatcher.exe.'CSProcessWatcher.exe <进程名>'
	命令的意思是CSProcessWatcher.exe将会检测<进程名>的事件.这里的<进程名>
	是你要检测的进程的名字.如果你直接运行CSProcessWatcher.exe不添加任何参数,默认的
	<进程名>将会是"notepad.exe".

步骤 2. 打开你想要检测的进程.观察从CSProcesswatcher.exe输出的信息.


/////////////////////////////////////////////////////////////////////////////
代码逻辑:

步骤 1.  创建一个叫"CSProcessWatcher"的C#控制台应用程序.

步骤 2.  右击引用, 选择 "Add Reference ...", 然后选择
         .NET 引用 "System.Management", 点击 "确认".

步骤 3. 打开 Visual studio 2008 命令提示符, 导航到 
		你当前项目目录 ,然后输入命令:  
		
		mgmtclassgen Win32_Process /n root\cimv2 /o WMI.Win32
		
		你将会得到 Process.CS 文件. 把它添加到项目中.
		
步骤 4.  添加一个新的类 'ProcessWatcher'. 使它派生自 
         System.Management.ManagementEventWatcher. 为这个类添加三个事件:
         ProcessCreated, ProcessDeleted, ProcessModified.
         
         在类ProcessWatcher的构造函数中, 我们需要订阅基于详细事件查询的临时事件通知.
         
        private void Init(string processName)
        {
            this.Query.QueryLanguage = "WQL";
            this.Query.QueryString = string.Format(processEventQuery, processName);
            this.EventArrived += new EventArrivedEventHandler(watcher_EventArrived);
        }
        
        同时， 当我们收到一个事件通知的时候，根据事件的类型，触发相应的事件。
 
        private void watcher_EventArrived(object sender, EventArrivedEventArgs e)
        {
            string eventType = e.NewEvent.ClassPath.ClassName;
            WMI.Win32.Process proc = new WMI.Win32.Process(e.NewEvent["TargetInstance"] as ManagementBaseObject);

            switch (eventType)
            {
                case "__InstanceCreationEvent":
                    if (ProcessCreated != null)
                    {
                        ProcessCreated(proc);
                    }
                    break;

                case "__InstanceDeletionEvent":
                    if (ProcessDeleted != null)
                    {
                        ProcessDeleted(proc);
                    }
                    break;

                case "__InstanceModificationEvent":
                    if (ProcessModified != null)
                    {
                        ProcessModified(proc);
                    }
                    break;
            }
        }
        
/////////////////////////////////////////////////////////////////////////////

参考:

WQL (SQL for WMI)
http://msdn.microsoft.com/en-us/library/aa394606(v=VS.85).aspx

Win32_Process Class
http://msdn.microsoft.com/en-us/library/aa394372(v=VS.85).aspx

__InstanceOperationEvent Class
http://msdn.microsoft.com/en-us/library/aa394652(v=VS.85).aspx

Receiving a WMI Event
http://msdn.microsoft.com/en-us/library/aa393013(VS.85).aspx
/////////////////////////////////////////////////////////////////////////////


