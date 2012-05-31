<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="CSASPNETImageMap._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <p>
            请在图片中选择将iframe导航至对应的行星</p>
        <asp:ImageMap ID="imgMapSolarSystem" runat="server" ImageUrl="~/solarsystem.jpg"
            HotSpotMode="PostBack" OnClick="imgMapSolarSystem_Click">
            <asp:RectangleHotSpot PostBackValue="太阳" AlternateText="太阳" Top="0" Left="0" Right="110"
                Bottom="258" HotSpotMode="PostBack" />
            <asp:CircleHotSpot PostBackValue="水星" AlternateText="水星" X="189" Y="172"
                Radius="3" HotSpotMode="PostBack" />
            <asp:CircleHotSpot PostBackValue="金星" AlternateText="金星" X="227" Y="172" Radius="10"
                HotSpotMode="PostBack" />
            <asp:CircleHotSpot PostBackValue="地球" AlternateText="地球" X="277" Y="172" Radius="10"
                HotSpotMode="PostBack" />
            <asp:CircleHotSpot PostBackValue="火星" AlternateText="火星" X="324" Y="172" Radius="8"
                HotSpotMode="PostBack" />
            <asp:CircleHotSpot PostBackValue="木星" AlternateText="木星" X="410" Y="172"
                Radius="55" HotSpotMode="PostBack" />
            <asp:PolygonHotSpot PostBackValue="土星" AlternateText="土星" Coordinates="492,235,471,228,522,179,540,133,581,126,593,134,657,110,660,126,615,167,608,203,563,219,542,214"
                HotSpotMode="PostBack" />
            <asp:CircleHotSpot PostBackValue="天王星" AlternateText="天王星" X="667" Y="172"
                Radius="21" HotSpotMode="PostBack" />
            <asp:CircleHotSpot PostBackValue="海王星" AlternateText="海王星" X="736" Y="172"
                Radius="18" HotSpotMode="PostBack" />
        </asp:ImageMap>
        <p>
            <asp:Label ID="lbDirection" runat="server"></asp:Label>
        </p>
        <iframe id="ifSelectResult" runat="server" width="895" height="500" src="http://zh.wikipedia.org/zh-cn/%E5%A4%AA%E9%98%B3%E7%B3%BB">
        </iframe>
    </div>
    </form>
</body>
</html>
