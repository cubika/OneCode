=============================================================================
                     CSASPNETBingMaps 项目概述
=============================================================================

用法:

本项目阐述了如何在ASP.NET页面中嵌入Bing Maps并且也描述了如何通过用户输入在地图
上寻找并标记相应的位置.

/////////////////////////////////////////////////////////////////////////////
代码演示：

步骤1: 在浏览器中运行Default.aspx页，你会在页面左边找到一个地图，在右侧找到几个
输入控件.

步骤2: 你可以通过在地图上拖拽鼠标和滚动鼠标滑轮来移动地图.

步骤3: 如果你想找某一个城市，例如在地图上寻找纽约，你可以把城市的名称写入在
“Location”下方的文本框中，并点击提交按钮.

步骤4: 如果你想依据自己的习惯显示地图，你可以改变Show a Map区域内的选项，例如，
你可以改变缩放地图级别，在文本框中输入经度和纬度.

/////////////////////////////////////////////////////////////////////////////
代码逻辑:

步骤1: 在Visual Studio 2010中建立一个C# ASP.NET的空的web应用程序.

步骤2: 在应用程序中建立一个名为Default的ASP.NET页面.

步骤3: 在页面上添加一个一行两列的table.在左边放置一个Bing Maps，在右边放置
设置信息和搜索块.

步骤4: 在table左边部分添加一个名为pnlBingMap的panel控件,如果你使用了一个div层
的id也为pnlBingMap，那么页面运行的时候会以div层显示出来.

步骤5: 在table右边部分建立设置块和搜索部分.你可以根据sample或者甚至拷贝代码来
完成Html的编码工作.

步骤6: 在页面上添加一个Bing Maps的Javascript API的链接. 数字6.3代表API的版本.
在你测试这个Sample的时候它可能会改变，最新版本请参考这个链接： 
http://www.microsoft.com/maps/developers/.

<script type="text/javascript" 
	    src="http://ecn.dev.virtualearth.net/mapcontrol/mapcontrol.ashx?v=6.3" />

步骤7: 建立一个LoadMap的函数，它将在页面加载的时候触发.

	function LoadMap() {
		map = new VEMap('pnlBingMap');

		var LA = new VELatLong(34.0540, -118.2370);
		map.LoadMap(LA, 12, style, false, VEMapMode.Mode2D, true, 1);
	}

注意: VEMap类是在Bing Map的API中定义的类.它几乎包含了所有对地图的操作功能如
加载地图，改变地图设置或者在地图上添加一个图形和图钉.

步骤8: 给搜索设置内的提交按钮添加一个单击事件的函数FindLoc().

    function FindLoc() {
        var loc = document.getElementById("txtLocation").value;
        try {
            map.Find(null, loc);
        } catch (e) {
            alert(e.message);
        }
    }
		
步骤9: 在Show a Map区域内的提交按钮添加一个名为SetMap()的函数.

	function SetMap() {
		var lat = document.getElementById("txtLatitude").value;
		var lng = document.getElementById("txtLongitude").value;

		if (lng == "" | lat == "") {
			alert("You need to input both Latitude and Longitude first.");
			return;
		}

		var ddlzoom = document.getElementById("ddlZoomLevel");
		var zoom = ddlzoom.options[ddlzoom.selectedIndex].value;

		map.SetCenter(new VELatLong(lat, lng));
		map.SetMapStyle(style);
		map.SetZoomLevel(zoom);
	}

/////////////////////////////////////////////////////////////////////////////
参考信息:

Bing Maps
# Bing Maps Platform - AJAX Map Control Interactive SDK
http://www.microsoft.com/maps/isdk/ajax/

Bing Maps
# Developers Getting Start
http://www.microsoft.com/maps/developers/

MSDN:
# VEMap Class
http://msdn.microsoft.com/en-us/library/bb429586.aspx

/////////////////////////////////////////////////////////////////////////////