========================================================================
       ASP.NET APPLICATION : VBASPNETImageMap 项目 概述
========================================================================

/////////////////////////////////////////////////////////////////////////////
用途:

  这个项目展示了如何使用ImageMap创建用VB.NET语言编写的太阳系行星系统的说明. 
  当图片中的行星被单击时, 关于这个行星的简要信息将被显示在图片下面
  同时iframe将被导航到WikiPedia上关联的页面. 

/////////////////////////////////////////////////////////////////////////////
代码逻辑:

步骤1: 在Visual Studio 2008 / Visual Web Developer中新建一个
Visual Basic ASP.NET Web应用程序将其命名为VBASPNETImageMap.

步骤2: 在Default页面中新建一个ImageMap控件然后将他的ID属性改为
imgMapSolarSystem.

[备注] ImageMap可包含若干自定义热点区. 当用户点击热点区, 
控件既可以生成一个到服务器的回发也可以导航到一个指定的URL. 
这个演示生成一个到服务器的回发并运行基于ImageMap控件上被单击的热点区特定代码. 
作为展示了导航到数个不同的URL的例子, 请参考下列代码并注意RectangleHotSpot标记中的
NavigateUrl属性:


<asp:ImageMap ID="ImageMap4Navigate" ImageUrl="image.jpg" runat="Server">
    <asp:RectangleHotSpot HotSpotMode="Navigate" 
                          NavigateUrl="navigate1.htm" 
                          AlternateText="Button 1"
                          Top="20" Left="200" 
                          Bottom="100" Right="300">
    </asp:RectangleHotSpot>
</asp:ImageMap> 

步骤3: 请复制下列代码并粘贴至页面, 即form标记之间. 

        <asp:ImageMap ID="imgMapSolarSystem" runat="server" 
                      ImageUrl="~/solarsystem.jpg" HotSpotMode="PostBack">
        
            <asp:RectangleHotSpot PostBackValue="太阳"
                                  AlternateText="太阳"
                                  Top="0" Left="0" Right="110" Bottom="258" 
                                  HotSpotMode="PostBack" />
                                  
            <asp:CircleHotSpot PostBackValue="水星"
                               AlternateText="水星"
                               X="189" Y="172" Radius="3" 
                               HotSpotMode="PostBack" />
                               
            <asp:CircleHotSpot PostBackValue="金星"
                               AlternateText="金星"
                               X="227" Y="172" Radius="10" 
                               HotSpotMode="PostBack" />
            
            <asp:CircleHotSpot PostBackValue="地球" 
                               AlternateText="地球"
                               X="277" Y="172" Radius="10" 
                               HotSpotMode="PostBack" />
            
            <asp:CircleHotSpot PostBackValue="火星" 
                               AlternateText="火星"
                               X="324" Y="172" Radius="8" 
                               HotSpotMode="PostBack" />
            
            <asp:CircleHotSpot PostBackValue="木星" 
                               AlternateText="木星"
                               X="410" Y="172" Radius="55" 
                               HotSpotMode="PostBack" />
            
            <asp:PolygonHotSpot PostBackValue="土星"
                            AlternateText="土星"
                            Coordinates="492,235,
                                         471,228,
                                         522,179,
                                         540,133,
                                         581,126,
                                         593,134,
                                         657,110,
                                         660,126,
                                         615,167,
                                         608,203,
                                         563,219,
                                         542,214"
                            HotSpotMode="PostBack" />
                                
            <asp:CircleHotSpot PostBackValue="天王星" 
                               AlternateText="天王星"
                               X="667" Y="172" Radius="21" 
                               HotSpotMode="PostBack" />
            
            <asp:CircleHotSpot PostBackValue="海王星" 
                               AlternateText="海王星"
                               X="736" Y="172" Radius="18" 
                               HotSpotMode="PostBack" />
            
        </asp:ImageMap>

[备注] 在ImageMap控件中有三种热点. 他们分别是
RectangleHotSpot, CircleHotSpot和PolygonHotSpot. 正如名称所表示的, 
RectangleHotSpot定义了矩形热点区. CircleHotSpots定义了圆形的热点区
PolygonHotSpot用来定义不规则形状的热点区.
要定义RectangleHotSpot对象的区域时, 使用Left, Top, Right
和Bottom属性表示区域本身的坐标. 
要定义一个CircleHotSpot对象的区域时, 设定X和Y为圆心的坐标. 
然后设定Radius属性圆心到圆周的距离. 
要定义一个PolygonHotSpot对象的区域时, 
将Coordinates属性设定为一个表示指定PolygonHotSpot对象的每一个顶点坐标的字符串. 
多边形顶点即多边形两条边的交点. 
关于这三个热点的更多说明, 请参照参考资料部分的链接.

步骤4: 在页面发的设计视图中双击ImageMap控件以Visual Basic .NET语言打开Code-Behind页面. 

步骤5: 定位到代码中imgMapSolarSystem_Click事件句柄然后根据PostBackValue的值
使用Select Case语句创建不同的行为.

[备注] 使热点生成到服务器的回发, 设置HotSpotMode属性为Postback 
然后使用PostBackValue属性指定区域名. 
这个名字在回发出现时将会传递到ImageMapEventArgs事件. 


/////////////////////////////////////////////////////////////////////////////
参考资料:

MSDN: ImageMap 类
http://msdn.microsoft.com/zh-cn/library/system.web.ui.webcontrols.imagemap.aspx

MSDN: RectangleHotSpot 类
http://msdn.microsoft.com/zh-cn/library/system.web.ui.webcontrols.rectanglehotspot.aspx

MSDN: CircleHotSpot 类
http://msdn.microsoft.com/zh-cn/library/system.web.ui.webcontrols.circlehotspot.aspx

MSDN: PolygonHotSpot 类
http://msdn.microsoft.com/zh-cn/library/system.web.ui.webcontrols.polygonhotspot.aspx

/////////////////////////////////////////////////////////////////////////////