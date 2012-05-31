/**************************************** 模块头 *****************************************\
* 模块名:      chatbox.js
* 项目名:      CSASPNETAJAXWebChat
* 版权 (c) Microsoft Corporation
*
* 在此文件中, 我们定义了一些JavaScript函数来打开一个用以加载一个聊天室的弹出窗口.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
*
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\******************************************************************************************/

Type.registerNamespace('WebChat');
WebChat.ChatBox = function () {
    WebChat.ChatBox.initializeBase(this);
    this._title = "";
    this._iframe = null;
    this._element = null;
    this._roomid = null;

}
var lockBox = new Array();
WebChat.ChatBox.prototype = {
    get_title: function () {
        return this._title;
    },
    set_title: function (val) {
        this._title = val;
    },
    get_roomid: function () {
        return this._roomid;
    },
    set_roomid: function (val) {
        this._roomid = val;
    },
    open: function (RoomId, RoomName) {
        if ($.inArray(RoomId, lockBox) == -1) {
            this._roomid = RoomId;
            this._title = RoomName;
            this._element = document.createElement("DIV");
            this._element.style.display = "none";
            this._iframe = document.createElement("IFRAME");
            this._iframe.src = "chatbox.aspx?i=" + RoomId;
            this._iframe.style.width = "400px";
            this._iframe.frameBorder = 0;
            this._iframe.style.height = "260px";
            this._iframe.scrolling = "no";
            this._element.appendChild(this._iframe);
            document.body.appendChild(this._element);
            var obj = this;
            $(this._element).dialog({ show: 'slide', title: this._title, width: 430, beforeClose: function () { obj.quit(obj._roomid); } });
            lockBox.push(RoomId);
            $addHandler(window, "unload", function () {
                obj.quit(obj._roomid);
            });
            $("#_cu_" + RoomId).html(Number($("#_cu_" + RoomId).html()) + 1);
        }
        else {
            ShowMessageBox("异常", "你已加入这个聊天室");
        }
    },
    quit: function (roomid) {
        ShowMessageBox("离开聊天室", "请稍等...");
        csaspnetajaxwebchat.transition.LeaveChatRoom(roomid, function () {
            lockBox = $.map(lockBox, function (n) {
                return n != roomid ? n : null;
            });

            $("#_cu_" + roomid).html(Number($("#_cu_" + roomid).html()) - 1);
            CloseMessageBox();
        });

    }
}

