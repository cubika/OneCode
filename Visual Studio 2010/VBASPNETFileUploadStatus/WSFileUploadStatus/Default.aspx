<%@ Page Language="VB" AutoEventWireup="true" CodeFile="Default.aspx.vb" Inherits="_Default" %>

<%@ Register Src="UploadStatusWindow.ascx" TagName="UploadStatusWindow" TagPrefix="jerryControl" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>VBASPNETFileUploadStatus</title>
    <script language="javascript" type="text/javascript" src="scripts/progressbar.js"></script>
     <link rel="Stylesheet" href="styles/UIstyle.css" type="text/css" />
       <script type="text/javascript">
           var UploadState = 0; //0=>准备就绪, 1=>上传, 2=>完成, 3=>终止, 4=>错误
           var bar = new ProgressBar();
           var tryReCall = 0;
           function ReceiveServerData(arg, context) {
               if (arg == "Cleared" || arg == "Aborted") {
                   return;
               }
               if (arg == "") {
                   if (tryReCall < 100) {
                       tryReCall++;
                       CallServer();
                       return;
                   }
                   else {
                       tryReCall = 0;
                       arg = "Error:超时!";
                   }
               }
               if (arg.indexOf("Error") > -1) {
                   $("spError").innerHTML = arg;
                   ReloadFrame();
                   UploadState = 0;
                   return false;
               }

               var json = eval("(" + arg + ")");
               if (json == undefined) {
                   return;
               }

               $("spUploaded").innerHTML = json.LoadedLengthString;
               $("spTotle").innerHTML = json.ContentLengthString;
               $("spSpeed").innerHTML = json.UploadSpeedString;
               $("spPercent").innerHTML = json.LoadedPersentage + "%";
               $("spSpentTime").innerHTML = json.SpendTimeString;
               $("spRemainTime").innerHTML = json.LeftTimeString;
               bar.SetProgress(json.LoadedPersentage);

               if (json.IsFinished) {
                   UploadState = 2;
               }
               if (json.Aborted) {
                   UploadState = 3;
               }
               switch (UploadState) {
                   case 0:
                       $("spError").innerHTML = "";
                       break;
                   case 1:
                       $("spError").innerHTML = "文件正在上传";
                       setTimeout(function () { CallServer(); }, 500);
                       break;
                   case 2:
                       $("spError").innerHTML = "文件已经上传";
                       ReloadFrame();
                       CallServer("Clear");
                       break;
                   case 3:
                       $("spError").innerHTML = "用户终止";
                       ReloadFrame();
                       CallServer("Clear");
                       break;
               }


           }
           function StartGetStatus() {
               $("status").style.display = "block";
               $("status").style.width = (window.outerWidth == null ? document.documentElement.offsetWidth : window.outerWidth) + "px";
               $("status").style.height = (window.outerWidth == null ? document.documentElement.offsetHeight : window.outerHeight) + "px";
               UploadState = 1;
               CallServer();
           }

           function AbortUpload() {
               if (UploadState == 1) {
                   CallServer("Abort");
               }
           }
           function CloseUploadWindow() {
               if (UploadState != 1) {
                   UploadState = 0;
                   tryReCall = 0;
                   $("status").style.display = "none";
               }
           }
           function $(id) {
               return document.getElementById(id);
           }
           function ReloadFrame() {
               tryReCall = 0;
               $("UploadControlContainer").src = "UploadControls.aspx";
           }
           window.onload = function () {
               bar.Init("ProgressBarContainer", "progress");
           }
    </script>
   
</head>
<body>
    <form id="form1" runat="server">
    <iframe src="UploadControls.aspx" id="UploadControlContainer" frameborder="0" runat="server"
        width="400" height="80" scrolling="no"></iframe>
    <!--我们创建一个用户控件来支持显示详细状态的弹出窗口 -->
    <jerryControl:UploadStatusWindow ID="UploadStatusWindow1" runat="server" />
    </form>
</body>
</html>
