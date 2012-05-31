/********************************* 模块头 **********************************\
* 模块名:  HtmlClient.js
* 项目名:  AzureBingMaps
* 版权 (c) Microsoft Corporation.
* 
* HTML客户端JavaScript代码.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

// 您的Bing Maps身份证明.
var mapCredential = '[your credential]';
var map;
var mouseDownLocation;
// 因为HTML客户端和Web Role服务同一主机,
// 我们使用相对地址.
var dataServiceUri = "DataService/TravelDataService.svc";
var items = new Array();
var addedItemIDs = new Array();
var modifiedItemIDs = new Array();
var deletedItemIDs = new Array();
var selectedItem;
var tempID = 0;

// Microsoft.Maps的捷径命名空间.
var Bing = Microsoft.Maps;

$(document).ready(LoadMap);

function LoadMap() {
    map = new Bing.Map($('#MainMap')[0],
    {
        credentials: mapCredential,
        center: new Bing.Location(31, 121),
        zoom: 4
    });
    // 不能使用单击,因为它会在摇动地图时触发.
    // Bing.Events.addHandler(map, 'click', Map_OnMouseDown);
    Bing.Events.addHandler(map, 'mousedown', Map_OnMouseDown);
    Bing.Events.addHandler(map, 'mouseup', Map_OnMouseUp);

    // 查询数据服务获得数据.
    $.ajax(
	{
	    type: 'GET',
	    url: dataServiceUri + '/Travels?$orderby=Time',
	    dataType: 'json',
	    success: LoadDataCompleted,
	    error: function () {
	        $('#ErrorInforamtion').text('获取数据发生错误. 请稍后再次尝试.');
	    }
	});
}

function LoadDataCompleted(result) {
    $(result['d']).each(function () {
        // 去除'/', 只显示数据部分.
        this.Time = eval('new ' + this.Time.replace(/\//g, ''));
        // 增加一个图钉.
        var latLong = new Bing.Location(this.Latitude, this.Longitude);
        var pushpin = new Bing.Pushpin(latLong);
        pushpin.title = this.Place;
        AttachPushpinEvents(pushpin)
        map.entities.push(pushpin);
        items.push({ PartitionKey: this.PartitionKey, RowKey: this.RowKey, Value: this, Pushpin: pushpin });
    });
    ApplyTemplate();
}

function ApplyTemplate() {
    // 删除旧数据.
    $('#TravelList').children().remove();
    // 应用jQuery模板到ul.
    $('#travelTemplate').tmpl(items).appendTo('#TravelList');
    // UI不能通过模板实现的额外变换.
    $.each($('.DatePicker'), function (index, item) {
        $(item).datepicker({ onSelect: DateChanged, dateFormat: 'yy-mm-dd' });
    });
}

function Map_OnMouseDown(e) {
    mouseDownLocation = new Bing.Point(e.pageX, e.pageY);
}

function Map_OnMouseUp(e) {
    var pixel = new Bing.Point(e.pageX, e.pageY);
    // 如果用户没有在筛选地图时只增加一个图钉.
    if (mouseDownLocation != null && mouseDownLocation.x == pixel.x && mouseDownLocation.y == pixel.y) {
        var latLong = map.tryPixelToLocation(pixel, Bing.PixelReference.page);
        // 调用Location REST服务获得单击处信息.
        $.ajax(
		{
		    url: 'http://dev.virtualearth.net/REST/v1/Locations/' + latLong.latitude + ',' + latLong.longitude + '?o=json&jsonp=LocationCallback&key=' + mapCredential,
		    dataType: 'jsonp',
		    jsonp: 'LocationCallback',
		    success: LocationCallback
		});
    }
}

/// Location REST服务的回调函数.
function LocationCallback(result) {
    if (result.resourceSets.length > 0) {
        var resourceSet = result.resourceSets[0];
        if (resourceSet.resources.length > 0) {
            var resource = resourceSet.resources[0];

            // 为RowKey生成临时GUID.
            var id = tempID.toString();
            var id2 = '';
            for (var i = 0; i < 12 - id.length; i++) {
                id2 += '0';
            }
            id2 += id;
            id = '00000000-0000-0000-0000-' + id2;
            tempID++;
            var time = new Date();
            time.setDate(time.getDate() + 1);

            // 增加一个图钉.
            var pushpin = new Bing.Pushpin(new Bing.Location(resource.point.coordinates[0], resource.point.coordinates[1]));
            pushpin.title = resource.name;
            AttachPushpinEvents(pushpin)
            map.entities.push(pushpin);
            var item =
            {
                PartitionKey: "[UserName]",
                RowKey: id,
                Place: resource.name,
                Latitude: resource.point.coordinates[0],
                Longitude: resource.point.coordinates[1],
                Time: time
            };
            items.push({ PartitionKey: item.PartitionKey, RowKey: item.RowKey, Value: item, Pushpin: pushpin });
            addedItemIDs.push({ PartitionKey: item.PartitionKey, RowKey: item.RowKey });
            // jQuery模板并不支持集合通知. 所以我们必须再次套用模板.
            ApplyTemplate();
        }
    }
}

// 关联图钉的鼠标事件.
function AttachPushpinEvents(pushpin) {
    Bing.Events.addHandler(pushpin, 'mouseover', Pushpin_MouseOver);
    Bing.Events.addHandler(pushpin, 'mouseout', Pushpin_MouseOut);
}

// 当鼠标悬浮于图钉上, 显示信息框.
function Pushpin_MouseOver(e) {
    $('#PushpinText').text(e.target.title);
    var pushpinPopup = $('#PushpinPopup');    
    var pixel = map.tryLocationToPixel(e.target.getLocation(), Bing.PixelReference.control);
    pushpinPopup.css('left', pixel.x - 100);
    pushpinPopup.css('top', pixel.y);
    pushpinPopup.animate({ opacity: 1 });
}

// 当鼠标离开图钉, 隐藏信息框.
function Pushpin_MouseOut() {
    $('#PushpinPopup').animate({ opacity: 0 })
}

// 此版本示例中不会用到下列代码.
function SelectItem(sender) {
    var partitionKey = $(sender).find(':input[type=hidden]')[0].value;
    var rowKey = $(sender).find(':input[type=hidden]')[1].value;
    selectedItem = SearchItems(partitionKey, rowKey);
    var ul = $(sender.currentTarget).parent();
    ul.children().removeClass('SelectedItem');
    $(sender.currentTarget).addClass('SelectedItem');
}

function DateChanged(dateText, datePicker) {
    var li = datePicker.input.parent().parent();
    var partitionKey = li.find(':input[type=hidden]')[0].value;
    var rowKey = li.find(':input[type=hidden]')[1].value;
    var item = SearchItems(partitionKey, rowKey);
    item.Value.Time = $(this).datepicker('getDate');
    // 如果该项目不是最近创建的，将它添加到已修改的名单.
    if (!IsItemInArray(addedItemIDs, partitionKey, rowKey, false)) {
        modifiedItemIDs.push({ PartitionKey: partitionKey, RowKey: rowKey });
    }
}

function DeleteItem(sender) {
    var li = $(sender).parent().parent();
    var partitionKey = li.find(':input[type=hidden]')[0].value;
    var rowKey = li.find(':input[type=hidden]')[1].value;
    // 如果该项目不是最近创建的，将它添加到已删除的名单.否则从已添加列表中移除.
    if (!IsItemInArray(addedItemIDs, partitionKey, rowKey, true)) {
        deletedItemIDs.push({ PartitionKey: partitionKey, RowKey: rowKey });
    }
    var item = SearchItems(partitionKey, rowKey, true);
    map.entities.remove(item.Pushpin);
    li.detach();
}

// 调用服务保存数据.
function SaveListButton_Click() {
    $(addedItemIDs).each(PostToDS);
    $(modifiedItemIDs).each(PutToDS);
    $(deletedItemIDs).each(DeleteFromDS);
    addedItemIDs = new Array();
    modifiedItemIDs = new Array();
    deletedItemIDs = new Array();
}

// 插入: 执行一个POST请求.
function PostToDS() {
    var item = SearchItems(this.PartitionKey, this.RowKey);
    $.ajax(
    {
        type: 'POST',
        url: dataServiceUri + '/Travels',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify(item.Value),
        datatype: 'json',
        error: function () {
            $('#ErrorInforamtion').text('获取数据发生错误. 请稍后再次尝试.');
        }
    });
}

// 更新: 执行一个PUT请求.
function PutToDS() {
    var item = SearchItems(this.PartitionKey, this.RowKey);
    $.ajax(
    {
        type: 'PUT',
        url: dataServiceUri + "/Travels(PartitionKey='" + this.PartitionKey + "',RowKey=guid'" + this.RowKey + "')",
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify(item.Value),
        datatype: 'json',
        error: function () {
            $('#ErrorInforamtion').text('获取数据发生错误. 请稍后再次尝试.');
        }
    });
}

// 删除: 执行一个DELETE请求.
function DeleteFromDS() {
    $.ajax(
    {
        type: 'DELETE',
        url: dataServiceUri + "/Travels(PartitionKey='" + this.PartitionKey + "',RowKey=guid'" + this.RowKey + "')",
        contentType: 'application/json; charset=utf-8',
        datatype: 'json',
        error: function () {
            $('#ErrorInforamtion').text('获取数据发生错误. 请稍后再次尝试.');
        }
    });
}

// 根据主键在列表中查找项目的功能函数.
// 如有需要删除项目.
function SearchItems(partitionKey, rowKey, remove) {
    for (var i = 0; i < items.length; i++) {
        var item = items[i];
        if (item.PartitionKey == partitionKey && item.RowKey == rowKey) {
            if (remove) {
                items.splice(i, 1);
            }
            return item;
        }
    }
    return null;
}

// 查看项目是否在列表中的功能函数.
// 如果需要删除项目.
function IsItemInArray(array, partitionKey, rowKey, remove) {
    for (var i = 0; i < array.length; i++) {
        var item = array[i];
        if (item.PartitionKey == partitionKey && item.RowKey == rowKey) {
            if (remove) {
                array.splice(i, 1);
            }
            return true;
        }
    }
    return false;
}

function formatDate(date) {
    return $.datepicker.formatDate('yy-mm-dd', date);
}