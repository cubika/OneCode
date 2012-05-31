/*********************************** 模块头 ***********************************\
* 模块名:  Home.js
* 项目名:  StoryCreatorWebRole
* 版权 (c) Microsoft Corporation.
* 
* 主页所用JavaScript文件.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

var storyServiceUri = "stories";

$(document).ready(function () {
    $("#videoPlayer").hide(0);

    // 调用服务获取短影列表.
    $.ajax({
        type: "GET",
        url: storyServiceUri,
        success: function (data) {
            $("#videoTemplate").tmpl(data).appendTo("#videoList");
        }
    });
});

/// 播放选定影片.
function playVideo(uri) {
    var player = $("#videoPlayer")[0];
    player.src = uri;
    player.play();
    $("#placeHolderDiv").hide(0);
    $("#videoPlayer").show(0);
}