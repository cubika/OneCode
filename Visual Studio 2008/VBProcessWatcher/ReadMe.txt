========================================================================
	      Windows应用程序: VBProcessWatcher 概述
========================================================================

/////////////////////////////////////////////////////////////////////////////
演示: 

请遵守以下所列出的步骤.

步骤 1. 在命令提示符中运行VBProcesswatcher.exe.'VBProcesswatcher.exe <进程名>'
	命令的意思是VBProcesswatcher.exe将会检测<进程名>的事件.这里的<进程名>
	是你要检测的进程的名字.如果你直接运行VBProcesswatcher.exe不添加任何参数,默认的
	<进程名>将会是"notepad.exe".

步骤 2. 打开你想要检测的进程.观察从VBProcesswatcher.exe输出的信息.


/////////////////////////////////////////////////////////////////////////////
代码逻辑:

步骤 1.  创建一个叫"VBProcesswatcher"的Visual Basic控制台应用程序.

步骤 2.  右击引用, 选择 "Add Reference ...", 然后选择
         .NET 引用 "System.Management", 点击 "确认".

步骤 3. 打开 Visual studio 2008 命令提示符, 导航到 
		你当前项目目录 ,然后输入命令:  
		
		mgmtclassgen Win32_Process /n root\cimv2 /o WMI.Win32 /l VB
		
		你将会得到 Process.VB 文件. 把它添加到项目中.
		
步骤 4.  添加一个新的类 'ProcessWatcher'. 使它派生自 
         System.Management.ManagementEventWatcher. 为这个类添加三个事件:
         ProcessCreated, ProcessDeleted, ProcessModified.
         
        在类ProcessWatcher的构造函数中, 我们需要订阅基于详细事件查询的临时事件通知.
         
         Private Sub Init(ByVal processName As String)
             Me.Query.QueryLanguage = "WQL"
             Me.Query.QueryString = String.Format(WMI_OPER_EVENT_QUERY, processName)
             AddHandler Me.EventArrived, AddressOf Me.watcher_EventArrived
         End Sub
        
        同时， 当我们收到一个事件通知的时候，根据事件的类型，触发相应的事件。
        
         Private Sub watcher_EventArrived(ByVal sender As Object, ByVal e As EventArrivedEventArgs)
            Dim eventType As String = e.NewEvent.ClassPath.ClassName
            Dim proc As New Win32.Process(TryCast(e.NewEvent("TargetInstance"), ManagementBaseObject))

            Select Case eventType
                Case "__InstanceCreationEvent"
                    RaiseEvent ProcessCreated(proc)
                    Exit Select

                Case "__InstanceDeletionEvent"
                    RaiseEvent ProcessDeleted(proc)
                    Exit Select

                Case "__InstanceModificationEvent"
                    RaiseEvent ProcessModified(proc)
                    Exit Select
            End Select
        End Sub
        
        
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


