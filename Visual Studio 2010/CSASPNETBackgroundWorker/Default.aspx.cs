/********************************** 模块头 ***********************************\
* 模块名:        Default.aspx.cs
* 项目名:        CSASPNETBackgroundWorker
* 版权(c) Microsoft Corporation
*
* 此页面显示一个TextBox.当用户按下按钮时,页面创建一个BackgroundWorker对象,
* 然后通过传递TextBox输入的值运行.最后,BackgroundWorker对象是存储在会话
* 状态中德,因此即使当前的请求结束,它仍会继续运行.
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
using System.Threading;

namespace CSASPNETBackgroundWorker
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Timer1_Tick(object sender, EventArgs e)
        {
            // 显示当前操作进度.
            BackgroundWorker worker = (BackgroundWorker)Session["worker"];
            if (worker != null)
            {
                // 显示操作进度.
                lbProgress.Text = "运行中: " + worker.Progress.ToString() + "%";

                btnStart.Enabled = !worker.IsRunning;
                Timer1.Enabled = worker.IsRunning;

                // 操作完成显示结果.
                if (worker.Progress >= 100)
                {
                    lbProgress.Text = (string)worker.Result;
                }
            }
        }

        /// <summary>
        /// 当按钮单击时创建一个执行后台工作的操作.
        /// </summary>
        protected void btnStart_Click(object sender, EventArgs e)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += new BackgroundWorker.DoWorkEventHandler(worker_DoWork);
            worker.RunWorker(txtParameter.Text);

            // 这里需要会话模式为"InProc"以保持后台工作运行.
            Session["worker"] = worker;

            // 启用定时器更新操作状态.
            Timer1.Enabled = true;
        }

        /// <summary>
        /// 此方法为需要长时间完成的操作e.
        /// </summary>
        void worker_DoWork(ref int progress, 
            ref object result, params object[] arguments)
        {
            // 获得传到此操作的输入值.
            string input = string.Empty;
            if (arguments.Length > 0)
            {
                input = arguments[0].ToString();
            }

            // 需要10秒完成操作.
            for (int i = 0; i < 100; i++)
            {
                Thread.Sleep(100);

                progress += 1;
            }

            // 操作完成.
            progress = 100;
            result = "操作完成. 输入是\"" + input + "\".";
        }
    }
}