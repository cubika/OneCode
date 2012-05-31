<%@ Application Language="C#" %>
<script RunAt="server">
    /********************************** 模块头 ***********************************\
* 模块名:        Global.asax
* 项目名:        CSASPNETHTMLEditorExtender
* 版权(c) Microsoft Corporation
* 
* 我们并没有必要创建此文件,它只是用来捕获当我们第一次使用它时不重建整个解决方案带来的异常.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
*
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*****************************************************************************/
    void Application_Error(object sender, EventArgs e)
    {
        Exception err = Context.Server.GetLastError();
        if (err.Message.IndexOf("AjaxControlToolkit") > -1)
        {
            Context.Server.ClearError();
            Context.Response.Write(
                "<html><head><title>CSASPNETHTMLEditorExtender</title></head>" +
                "<body><p style='text-align:center'>" +
                "请重新生成整个解决方案,然后再对其进行测试!</p></body></html>");
        }
    }
</script>
