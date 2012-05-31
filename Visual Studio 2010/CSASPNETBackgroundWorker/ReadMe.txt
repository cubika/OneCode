==============================================================================
 ASP.NET 应用程序 : CSASPNETBackgroundWorker 项目概述
==============================================================================

//////////////////////////////////////////////////////////////////////////////
总结:

有时我们做一个需要较长时间才能完成操作.直到操作完成前,它将停止响应和显示空白的页面.
在这种情况下,我们希望操作在后台运行,并且我们要在页面上显示正在运行的操作的进度.
因此，用户可以知道操作在运行以及其进度.

另一方面,我们要安排一些操作(发送电子邮件/报告等).
我们希望这些操作可以在特定时间运行. 

该项目创建一个名为“BackgroundWorker的“实现这些目标的类.
创建一个名为“Default.aspx的“运行长时间操作的页面。
创建一个后台工作在应用程序启动时执行时间表,
使用“GlobalBackgroundWorker.aspx“页,检查进度.


//////////////////////////////////////////////////////////////////////////////
演示示例:

1. 基于会话的后台工作.
    a. 打开Default.aspx.然后单击按钮,在后台运行操作.
    b. 在两个浏览器中打开Default.aspx,然后在同一时间单击按钮.
       你会看到两个独立的后台工作在运行.

2. 应用程序级后台工作.
    a. 打开GlobalBackgroundWorker.aspx,你会看到后台工作正在运行.
    b. 关闭浏览器,等待10秒钟,然后再打开网页.
       你会看到即使我们关闭浏览器后台工作仍在运行.

//////////////////////////////////////////////////////////////////////////////
代码逻辑:

1. 打开名为“BackgroundWorker“的类.你会看到它在独立线程启动一个操作(方法).

    _innerThread = new Thread(() =>
    {
        _progress = 0;
        DoWork.Invoke(ref _progress, ref _result, arguments);
        _progress = 100;
    });
    _innerThread.Start();

2. 在名为“Default.aspx的“的页面中.使用UpdatePanel实现部分更新.
   使用Timer控件来更新操作的进度.

    <!-- UpdateUpanel let the progress can be updated without updating the whole page (partial update). -->
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            
            <!-- The timer which used to update the progress. -->
            <asp:Timer ID="Timer1" runat="server" Interval="100" Enabled="false" ontick="Timer1_Tick"></asp:Timer>

            <!-- The Label which used to display the progress and the result -->
            <asp:Label ID="lbProgress" runat="server" Text=""></asp:Label><br />

            <!-- Start the operation by inputting value and clicking the button -->
            Input a parameter: <asp:TextBox ID="txtParameter" runat="server" Text="Hello World"></asp:TextBox><br />
            <asp:Button ID="btnStart" runat="server" Text="Click to Start the Background Worker" onclick="btnStart_Click" />

        </ContentTemplate>
    </asp:UpdatePanel>

    btnStart_Click()方法用于处理按钮的Click事件.
	它创建后台工作,并将其保存到会话状态.
    因此，后台工作必须绑定到当前的会话.

        BackgroundWorker worker = new BackgroundWorker();
        worker.DoWork += new BackgroundWorker.DoWorkEventHandler(worker_DoWork);
        worker.RunWorker(txtParameter.Text);

        // It needs Session Mode is "InProc"
        // to keep the Background Worker working.
        Session["worker"] = worker;

3. 在Global类,你会发现Application_Start()方法创建一个后台工作,然后将其保存到
   应用程序状态.	因此,后台工作将继续在后台运行,它是由所有用户共享.
        BackgroundWorker worker = new BackgroundWorker();
        worker.DoWork += new BackgroundWorker.DoWorkEventHandler(worker_DoWork);
        worker.RunWorker(null);

        // This Background Worker is Applicatoin Level,
        // so it will keep working and it is shared by all users.
        Application["worker"] = worker;

//////////////////////////////////////////////////////////////////////////////
参考资料:

使用线程和线程处理
http://msdn.microsoft.com/zh-cn/library/e1dx6b2h.aspx

UpdatePanel 控件概述
http://msdn.microsoft.com/zh-cn/library/bb386454.aspx

Timer 控件概述
http://msdn.microsoft.com/zh-cn/library/bb398865.aspx

事件（C# 编程指南）
http://msdn.microsoft.com/zh-cn/library/awbftdfh.aspx