========================================================================
         ASP.NET 应用程序: CSASPNETChart 项目概述
========================================================================

/////////////////////////////////////////////////////////////////////////////
用法:

  这个项目展示了如何使用一个新的 Chart控件在WEB页面创建一个图表. 

/////////////////////////////////////////////////////////////////////////////
代码逻辑:

步骤 1. 在 Visual Studio 2010 RC 或者 Visual Web Developer 2010 中创建一个
ASP.NET的web的应用程序，并命名为 CSASPNETChart.

[NOTE] 如果没有安装 Visual Studio 2010, 你可以下载它的
express 版本, 地址：http://www.microsoft.com/express/Web/

步骤 2. 删除 Visual Studio 自动生成的默认的文件夹和文件.

Account folder
Script folder
Style folder
About.aspx file
Default.aspx file
Global.asax file
Site.Master file

步骤 3. 向网站中添加一个新的 web 页面，命名为 Default.aspx.

步骤 4. 添加一个 Chart 控件到这个页面. 你可以在工具箱的数据类型中找到它.

[注意] 当一个 Chart 控件被添加到页面的时候, 控件的注册信息会被自动的添加到页面中.

<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, 
    Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>

同样, 一个新的引用 /System.Web.DataVisualization/ 也将会被添加到这个web应用程序中.

步骤 5: 添加两个 Series Chart tag,像下面这样.

    <Series>
        <asp:Series Name="Series1">
        </asp:Series>
        <asp:Series Name="Series2">
        </asp:Series>
    </Series>

[注意] Series 集合属性存储 Series 对象, 用来储存将被显示的数据,以及数据的属性.

步骤 6: 编辑这两个 Series, 添加 ChartType 属性,使其等于 Column 和
ChartArea 属性的值，像ChartArea1.

[注意] Series ChartType 值 标示了 chart 的类型,这些会被用于展现. 
关于所有的类型, 请参考以下链接: 
http://msdn.microsoft.com/en-us/library/system.web.ui.datavisualization.charting.seriescharttype(VS.100).aspx

ChartAreas 集合属性存储了 ChartArea 对象, 它们主要使用一套轴,绘制一个或者多个图表.
你最终看到的HTML像下面这样.

<asp:Chart ID="Chart1" runat="server" Height="400px" Width="500px">
    <Series>
        <asp:Series Name="Series1" ChartType="Column" ChartArea="ChartArea1">
        </asp:Series>
        <asp:Series Name="Series2" ChartType="Column" ChartArea="ChartArea1">
        </asp:Series>
    </Series>
    <ChartAreas>
        <asp:ChartArea Name="ChartArea1">
        </asp:ChartArea>
    </ChartAreas>
</asp:Chart>

步骤 7: 在后台代码中为Chart控件创建一个数据源. 在这一步中, 请直接使用方法 CreateDataTable, 
因为这不是我们在项目中所要谈论的内容.

步骤 8: 绑定数据源到 Chart 控件.

    Chart1.Series[0].YValueMembers = "Volume1";
    Chart1.Series[1].YValueMembers = "Volume2";
    Chart1.Series[0].XValueMember = "Date";

[注意] Series.YValueMembers 属性用于获取或设置数据源的列成员的,用于绑定数据到 Y-values. 
同样,Series.YValueMembers 属性用于获取或设置数据源的列成员的,用于绑定数据到 X-values.

步骤 9: 现在,你可以运行这个页面,看到我们之前实现的效果了. :-)

/////////////////////////////////////////////////////////////////////////////
参考:

MSDN: Chart 类
http://msdn.microsoft.com/zh-cn/library/system.web.ui.datavisualization.charting.chart(VS.100).aspx

MSDN: Chart 控件入门
http://msdn.microsoft.com/zh-cn/library/dd489231(VS.100).aspx

ASP.NET: Chart 控件
http://www.asp.net/learn/aspnet-4-quick-hit-videos/video-8770.aspx (Quick Hit Videl)

/////////////////////////////////////////////////////////////////////////////