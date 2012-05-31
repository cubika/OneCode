=============================================================================
              VBASPNETLimitDownloadSpeed Project 项目概述
=============================================================================

用法:

 本项目阐述了如何通过编程来限制下载速度. 

/////////////////////////////////////////////////////////////////////////////
注意:

请注意IIS7有一个叫限制比特率的扩展功能, 只要简单的设置几个选项,
就可以为我们实现这个功能. 要知道限制比特率的更多信息, 请参 
考: http://www.iis.net/download/BitRateThrottling

如果你在Windows Server 2008中使用的不是IIS7,我们强烈建议您尽快的  
升级您的服务器. 如果你已经在使用IIS7, 请使用限制比特率的功能来代替
本代码实例中演示的限制下载速度. 谢谢. 

/////////////////////////////////////////////////////////////////////////////
代码演示.

步骤1: 把实例中的Default.aspx在浏览器中查看, 并在下拉列表中 
		选择一个受限的速度.

步骤2: 单击下载按钮来下载文件.

步骤3: 你可以发现下载速度被限制在你选择的那个速度了.

/////////////////////////////////////////////////////////////////////////////
代码逻辑:

步骤1: 在Visual Studio 2010中创建一个VB.NET的空应用程序. 

步骤2: 在应用程序中添加一个Default页面. 

步骤3: 在后台添加这两个命名空间. 

	using System.IO;
	using System.Threading;
	
注意: System.IO是在FileStream和BinaryReader类还有一些枚举类型中用到.
System.Threading是在Thread.Sleep()方法中用到. 

步骤4: 操纵Page_Load的事件句柄, 并编写代码创建一个临时的大文件来测试下载. 

    Protected Sub Page_Load(ByVal sender As Object, 
                            ByVal e As System.EventArgs) 
                            Handles Me.Load
        Dim length As Integer = 1024 * 1024 * 1
        Dim buffer As Byte() = New Byte(length - 1) {}

        Dim filepath As String = Server.MapPath("~/bigFileSample.dat")
        Using fs As New FileStream(filepath, 
                                   FileMode.Create, 
                                   FileAccess.Write)
            fs.Write(buffer, 0, length)
        End Using
    End Sub
    
步骤5: 在页面中添加一个下拉列表控件和一个按钮控件. 

    <asp:DropDownList ID="ddlDownloadSpeed" runat="server">
		<asp:ListItem Value="20">20 Kb/s</asp:ListItem>
		<asp:ListItem Value="50">50 Kb/s</asp:ListItem>
		<asp:ListItem Value="80">80 Kb/s</asp:ListItem>
    </asp:DropDownList>
    <asp:Button ID="btnDownload" runat="server" Text="下载" />

步骤6: 编写DownloadFileWithLimitedSpeed()方法.请参考"注意"来理解这个方法. 

    Public Sub DownloadFileWithLimitedSpeed(ByVal fileName As String, 
                                            ByVal filePath As String, 
                                            ByVal downloadSpeed As Long)
        '...
	End Sub
    
注意: 这个方法是这个实例的关键.在这个方法中实现限制下载速度功能的基本原理是, 
强制响应线程在每次发送文件包的时候休眠一个适当的时间. 

举个最简单的例子,如果文件包的大小是1Kb,我们可以使响应线程在发送每个文件包后 
休眠1秒,这样我们就可以控制下载速度为1Kb/s.要获得不同的下载速度,我们只需要 
使线程休眠的时间更短或更长.

很明显,在这里决定休眠时间是最重要的事情. 

比如说如果文件包是1Kb,并且我们需要把下载速度限制在50Kb/s,线程需要休眠多久呢？
答案是20毫秒. 50Kb/s意味着我们每秒需要发送50个文件包.由于每秒等于1000毫秒,我
们可以把它们分成50个部分并且每一部分是20毫秒.  20 ms * 50 =  1000 ms = 1 s.

所以,休眠时间的公式应该是：sleep = 1000 / (下载速度/包) 

根据上面的例子, 下载速度 = 1024 * 50 和 包 = 1024 我们得到:
sleep = 1000 / (50 * 1024 / 1024) = 1000 / 50 = 20.

为了更好的理解,我们可以认为这个结果(1000 / sleep)是线程在每一秒应当发送文件包 
的次数. 像sleep = 20，这表示线程每秒发送1000 / 20 = 50个文件包. 所以,下载速度
等于每秒50文件包.  

更多信息: 你可能发现有些时候下载速度没有准确的符合选择的值,甚至是它达到一个更高速度.  
这是由于IO消耗引起的.在每秒钟更高的速度表示比较低的速度有更短的休眠时间,也意味着
下面的代码被执行了更多次.

    If Response.IsClientConnected Then
        Response.BinaryWrite(br.ReadBytes(pack))
        Thread.Sleep(sleep)
    End If
                        
然而,代码本身的执行也会花费一些时间,可能是1或者2毫秒.如果休眠时间是100毫秒 
或者更多时不会太明显.我们知道100毫秒和101毫秒没有很大的区别.但是,如果休眠 
时间是10毫秒或更少,这个误差会对下载速度产生很大影响. 

步骤7: 添加Button.Click事件句柄来调用这个方法.

    Protected Sub btnDownload_Click(ByVal sender As Object, 
                                    ByVal e As EventArgs) 
                                    Handles btnDownload.Click
                                    
        Dim outputFileName As String = "bigFileSample.dat"
        Dim filePath As String = Server.MapPath("~/bigFileSample.dat")

        Dim value As String = ddlDownloadSpeed.SelectedValue

        Dim downloadSpeed As Integer = 1024 * Integer.Parse(value)
        Response.Clear()

        Try
            DownloadFileWithLimitedSpeed(outputFileName, 
                                         filePath, 
                                         downloadSpeed)
        Catch ex As Exception
            Response.Write(ex.Message)
        End Try
        Response.End()
    End Sub


/////////////////////////////////////////////////////////////////////////////
参考文献:

MSDN:
# HttpResponse.AddHeader 方法
http://msdn.microsoft.com/zh-cn/library/system.web.httpresponse.addheader.aspx

# HttpResponse.BinaryWrite 方法
http://msdn.microsoft.com/zh-cn/library/system.web.httpresponse.binarywrite.aspx

# Thread.Sleep 方法 
http://msdn.microsoft.com/zh-cn/library/d00bd51t.aspx

/////////////////////////////////////////////////////////////////////////////