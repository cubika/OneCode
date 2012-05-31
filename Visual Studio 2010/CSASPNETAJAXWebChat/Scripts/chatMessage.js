/**************************************** 模块头 *****************************************\
* 模块名:      chatMessage.js
* 项目名:      CSASPNETAJAXWebChat
* 版权 (c) Microsoft Corporation
*
* 在此文件中, 我们定义了一些avaScript函数用来获取消息列表,
* 用来发布一条消息,用来获取聊天者列表.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
*
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\******************************************************************************************/

// 发送消息
function SendMessage(textbox) {
    if (textbox.value != "") {
        csaspnetajaxwebchat.transition.SendMessage(textbox.value, sendMessageCallBack);
        textbox.value = "";
    }
}
function sendMessageCallBack(args) {
    if (args) {
        UpdateLocalMessage();
    }
    else {
        alert(args);
    }
}

// 更新消息列表
function UpdateLocalMessage() {
    csaspnetajaxwebchat.transition.RecieveMessage(UpdateMessageSuccessCallBack, UpdateMessageFaileCallBack);
}
function UpdateMessageFaileCallBack(args) {
    //csaspnetajaxwebchat.transition.LeaveChatRoom(null);
}
function UpdateMessageSuccessCallBack(args) {
    var container = $("#txtMessageList");
    container.html("");
    $(args).each(function (i) {
        var d = document.createElement("DIV");
        $(d)
                    .appendTo(container)
                    .addClass((this.IsFriend ? "_tlkFriend" : "_tlkMe"))
                    .end()
                    .append("<span class=\"_talker\">" + (this.IsFriend ? this.Talker : "我") + "</span>")
                    .append("<span> 在 </span>")
                    .append("<span class=\"_time\">" + this.SendTime.format("MM/dd/yyyy HH:mm:ss") + "</span>")
                    .append("<span> 说: </span><BR /> ")
                    .append("<span class=\"_msg\">" + this.MessageData + "</span>");

    });
    container.scrollTop(container[0].scrollHeight - container.height());

    setTimeout(function () { UpdateLocalMessage(); }, 2000);
}

// 更新当前聊天室发言人名单
function UpdateRoomTalkerList() {
    csaspnetajaxwebchat.transition.GetRoomTalkerList(UpdateRoomTalkerListSuccessCallBack);
}
function UpdateRoomTalkerListSuccessCallBack(args) {
    var lst = $("#lstUserList");
    lst.html("");
    $(args).each(function (i) {
        var l = document.createElement("OPTION");
        $(l)
                    .appendTo(lst)
                    .attr("text", (this.IsFriend ? this.TalkerAlias : "自己"))
                    .attr("value", this.TalkerSession)
                    .attr("title", this.TalkerSession);
    });

    setTimeout(function () { UpdateRoomTalkerList(); }, 2000);
}