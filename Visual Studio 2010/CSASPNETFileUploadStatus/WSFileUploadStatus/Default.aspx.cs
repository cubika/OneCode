/****************************** 模块头 ******************************\
* 模块名:    Default.aspx.cs
* 项目名:    CSASPNETFileUploadStatus
* 版权 (c) Microsoft Corporation
*
* 本项目阐述了在不使用第三方组件时实现显示上传的状态和进程 
* 像ActiveX 控件,Flash 或者Silverlight.
* 
* 在这个页面中我们为用户测试上传的状态.
* 我们在不刷新页面时使用ICallbackEventHandler来实现服务器端
* 和客户端的通信.
* 但是我们需要使用一个iframe来放置上传控件和按钮,因为上传
* 需要回发到服务器端,我们不能在同一个回发页面上调用服务器端
* 的javascript代码. 
* 所以我们用iframe来做上传的回发操作.
* 
* 想知道关于ICallbackEventHandler更多详细信息，
* 请参考根目录下的readme文件.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*****************************************************************************/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Threading;
using System.Web.Script.Serialization;
using System.Text;
using CSASPNETFileUploadStatus;

public partial class _Default : System.Web.UI.Page, ICallbackEventHandler
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //为ICallbackEventHandler注册一个客户端脚本 
        ClientScriptManager cm = Page.ClientScript;
        String cbReference = cm.GetCallbackEventReference(this, "arg",
            "ReceiveServerData", "");
        String callbackScript = "function CallServer(arg, context) {" +
            cbReference + "; }";
        if (!cm.IsClientScriptBlockRegistered(this.GetType(), "CallServer"))
        {
            cm.RegisterClientScriptBlock(this.GetType(),
                "CallServer", callbackScript, true);
        }

    }
    private string uploadModuleProgress = "";
    public string GetCallbackResult()
    {
        return uploadModuleProgress;
    }

    public void RaiseCallbackEvent(string eventArgument)
    {
        if (eventArgument == "Clear")
        {
            // 清除缓存的操作
            ClearCache("fuFile");
            uploadModuleProgress = "Cleared";
        }
        if (eventArgument == "Abort")
        {
            // 终止上传的操作
            AbortUpload("fuFile");
            uploadModuleProgress = "Aborted";
        }


        try
        {
            UploadStatus status =
                HttpContext.Current.Cache["fuFile"] as UploadStatus;
            if (status == null)
            {
                return;
            }
            // 我们使用JSON来向客户端发送数据,
            // 因为它不仅简单而且容易操作.
            // 想知道JavaScriptSerializer更多更详细的内容，
            // 请阅读根目录下的readme文件.
            JavaScriptSerializer jss = new JavaScriptSerializer();

            // StringBuilder对象将会保存序列化结果.
            StringBuilder sbUploadProgressResult = new StringBuilder();
            jss.Serialize(status, sbUploadProgressResult);

            uploadModuleProgress = sbUploadProgressResult.ToString();
        }
        catch (Exception err)
        {
            if (err.InnerException != null)
                uploadModuleProgress = "错误:" + err.InnerException.Message;
            else
                uploadModuleProgress = "错误:" + err.Message;
        }
    }

    private void AbortUpload(string cacheID)
    {
        UploadStatus status = HttpContext.Current.Cache[cacheID] as UploadStatus;
        if (status == null)
        {
            return;
        }
        else
        {
            status.Abort();
        }

    }

    private void ClearCache(string cacheID)
    {
        HttpContext.Current.Cache.Remove(cacheID);
    }


}