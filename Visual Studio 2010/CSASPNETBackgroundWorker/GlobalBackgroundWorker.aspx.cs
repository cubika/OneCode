/********************************** 模块头 ***********************************\
* 模块名:        GlobalBackgroundWorker.aspx.cs
* 项目名:        CSASPNETBackgroundWorker
* 版权(c) Microsoft Corporation
*
* 此网页使用Timer控件来显示应用程序级BackgroundWorker对象进度.
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

namespace CSASPNETBackgroundWorker
{
    public partial class GlobalBackgroundWorker : System.Web.UI.Page
    {
        protected void Timer1_Tick(object sender, EventArgs e)
        {
            // 显示了应用程序级后台工作的进度。
            // 在Global.asax.cs文件Application_Start()方法创建后台工作.
            BackgroundWorker globalWorker = (BackgroundWorker)Application["worker"];
            if (globalWorker != null)
            {
                lbGlobalProgress.Text = "全局工作正在运行: "
                    + globalWorker.Progress.ToString();
            }
        }
    }
}