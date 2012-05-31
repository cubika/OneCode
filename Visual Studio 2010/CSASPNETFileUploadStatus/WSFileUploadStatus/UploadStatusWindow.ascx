<%@ Control Language="C#" AutoEventWireup="true" CodeFile="UploadStatusWindow.ascx.cs"
    Inherits="UploadStatusWindow" %>
<div id="status" class="UploadStatusWindow">
    <table cellpadding="3" cellspacing="0" border="0" align="center">
        <tr>
            <td colspan="4" style="text-align: center; color: Red; width: 550px">
                <span id="spError"></span>
            </td>
        </tr>
        <tr>
            <th>
                上传内容:
            </th>
            <td>
                <span id="spUploaded"></span>
            </td>
            <th>
                内容总长度:
            </th>
            <td>
                <span id="spTotle"></span>
            </td>
        </tr>
        <tr>
            <th>
                上传速度:
            </th>
            <td>
                <span id="spSpeed"></span>
            </td>
            <th>
                上传百分比:
            </th>
            <td>
                <span id="spPercent"></span>
            </td>
        </tr>
        <tr>
            <th>
                用时:
            </th>
            <td>
                <span id="spSpentTime"></span>
            </td>
            <th>
                预计用时:
            </th>
            <td>
                <span id="spRemainTime"></span>
            </td>
        </tr>
        <tr>
            <td colspan="4" id="ProgressBarContainer" style="text-align: center; width: 550px;">
            </td>
        </tr>
        <tr>
            <td colspan="4" style="text-align: right; width: 550px;">
                <input type="button" value="终止" onclick="AbortUpload()" />
                <input type="button" value="关闭" onclick="CloseUploadWindow()" />
            </td>
        </tr>
    </table>
</div>
