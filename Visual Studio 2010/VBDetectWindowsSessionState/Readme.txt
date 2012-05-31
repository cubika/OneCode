================================================================================
       Windows应用程序: VBDetectWindowsSessionState 概述                        
===============================================================================
/////////////////////////////////////////////////////////////////////////////
摘要:
这个实例演示如何检查Windows当前会话状态.

Microsoft.Win32.SystemEvents包含了SessionSwitch事件. 当用户会话状态切换时,会触发SessionSwitch
事件, 如会话被锁定/解锁, 或者用户登录到一个会话. 我们可以注册这个事件来检查会话状态的变化. 

User32.dll 包含一个外部方法 OpenInputDesktop 来打开桌面接受用户的输入. 如果返回IntPtr.Zero, 
意味着方法失败, 也就是桌面被锁定.

注意:
      如果UAC弹出安全桌面, 这个方法同样会失败. 现在没有API能够区分是桌面锁定还是UAC弹出
      安全桌面.

////////////////////////////////////////////////////////////////////////////////
演示:

步骤1. 在VS2010中生成这个项目. 

步骤2. 运行VBDetectWindowsSessionState.exe.

步骤3. 选中"启动定时器每5秒检测会话状态", 你将看到 "当前状态: <状态> 时间: <时间>" 在主窗口的上部, 
	  每隔5秒列表框中也会有一条新记录. 

步骤4. 不选中"启动定时器每5秒检测会话状态". 

步骤5. 按下 Win+L 来锁定 PC, 然后再解锁. 在列表框中你将看到2条新记录, 一条是
      "<时间> SessionLock <发生了>", 另一条是"<时间> SessionUnlock <发生了>".


/////////////////////////////////////////////////////////////////////////////
代码逻辑:

1. 设计WindowsSession类. 当该类的一个对象初始化的时候, 注册SystemEvents.SessionSwitch事件.
    
2. 当SystemEvents.SessionSwitch事件发生时, 触发StateChanged事件.

3. WindowsSession类也包裹了OpenInputDesktop和CloseDesktop方法.
   如果方法OpenInputDesktop失败, 返回IntPtr.Zero, 意味着桌面被锁定.
   
4. 设计一个用户界面的主窗口. 

   主窗口会处理一个WindowsSession实例StateChanged事件. 当事件发生时, 在列表框中添加新记录.
   它同时也包含一个定时器, 可以每隔5秒检查Windows会话状态.

/////////////////////////////////////////////////////////////////////////////
参考:

SystemEvents.SessionSwitch Event
http://msdn.microsoft.com/en-us/library/microsoft.win32.systemevents.sessionswitch.aspx

OpenInputDesktop Function
http://msdn.microsoft.com/en-us/library/ms684309(VS.85).aspx

CloseDesktop Function
http://msdn.microsoft.com/en-us/library/ms682024(VS.85).aspx

Desktop Security and Access Rights
http://msdn.microsoft.com/en-us/library/ms682575(VS.85).aspx

/////////////////////////////////////////////////////////////////////////////
