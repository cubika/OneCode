/**************************************** 模块头 *****************************************\
* 模块名:      chatRoom.js
* 项目名:      VBASPNETAJAXWebChat
* 版权 (c) Microsoft Corporation
*
* 在此文件中, 我们定义了一些JavaScript函数用来显示聊天室列表,
* 用来创建一个聊天室,用来加入一个聊天室和其他人聊天.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
*
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\******************************************************************************************/

// 显示用来创建聊天室的弹出表单
function fnShowChatRoomForm() {
    // jQuery.dialog reference:
    // http://jqueryui.com/demos/dialog/
    $("#divCreateChatRoomForm").dialog({ modal: true, show: 'slide', title: '创建一个聊天室', width: 500 });
}

// 显示弹出消息框
function ShowMessageBox(Title, Message) {
    $("#DivMessage").html(Message);
    $("#DivMessage").dialog({ modal: true, title: Title });
}

// 关闭弹出消息框
function CloseMessageBox() {
    $("#DivMessage").dialog('close');
}
// 创建一个聊天室
function fuCreateChatRoom() {

    $("#divCreateChatRoomForm").dialog('close');
    ShowMessageBox("创建聊天室...", "创建聊天室...");

    vbaspnetajaxwebchat.transition.CreateChatRoom(
                $("#txtAlias").val(),
                $("#txtRoomName").val(),
                $("#txtPassword").val(),
                $("#ddlMaxUser").val(),
                ($("#chkNeedPassword").val() == "on"),
                fuCreateChatRoomOnSuccessCallBack,
                ajaxErrorCallBack
                );

}
function fuCreateChatRoomOnSuccessCallBack(args) {
    CloseMessageBox();
    fuGetRoomList();
}

// 获得聊天室列表
function fuGetRoomList() {
    ShowMessageBox("获得聊天室列表...", "获得聊天室列表...");
    vbaspnetajaxwebchat.transition.GetChatRoomList(fuGetRoolListOnSuccessCallBack, ajaxErrorCallBack);
}
function fuGetRoolListOnSuccessCallBack(args) {
    var table = $("#tblRoomList");
    table.html("");

    var TR = document.createElement("TR");
    var TH = document.createElement("TD");
    $(TR).appendTo(table);
    $(TH).appendTo(TR)
                    .html("房间ID");
    TH = document.createElement("TH");
    $(TH).appendTo(TR)
                    .html("房间名");
    TH = document.createElement("TH");
    $(TH).appendTo(TR)
                    .html("最大用户");
    TH = document.createElement("TH");
    $(TH).appendTo(TR)
                    .html("当前用户");
    TH = document.createElement("TH");
    $(TH).appendTo(TR)
                    .html("加入");

    $(args).each(function (i) {
        var tr = document.createElement("TR");
        var td = document.createElement("TD");
        $(tr).appendTo(table);
        $(td).appendTo(tr)
                    .html(this.RoomID);
        td = document.createElement("TD");
        $(td).appendTo(tr)
                    .html(this.RoomName);
        td = document.createElement("TD");
        $(td).appendTo(tr)
                    .html(this.MaxUser);
        td = document.createElement("TD");
        $(td).appendTo(tr)
                    .html("<span id='_cu_" + this.RoomID + "'>" + this.CurrentUser + "</span>");
        td = document.createElement("TD");
        $(td).appendTo(tr)
                    .html("<input type='button' value='加入' onclick=\"fnJoinChatRoom('" + args[i].RoomID + "')\" />");
    });

    CloseMessageBox();
}

// 加入一个聊天室
function fnJoinChatRoom(roomid) {
    ShowMessageBox("加入聊天室", "加入聊天室");
    vbaspnetajaxwebchat.transition.JoinChatRoom(
                roomid,
                $("#txtAlias").val(),
                fnJoinChatRoomOnSuccessCallBack);
}
function fnJoinChatRoomOnSuccessCallBack(args) {
    CloseMessageBox();
    if (args != null) {
        var chatbox = new WebChat.ChatBox();
        chatbox.open(args.RoomID, args.RoomName);
    }
    else {
        ShowMessageBox("错误", "参数错误: ChatRoomID!");
    }
}


function ajaxErrorCallBack(args) {
    $("#DivMessage").dialog("close");
}
