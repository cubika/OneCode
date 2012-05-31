/********************************** 模块头 ***********************************\
* 模块名:        Global.asax.cs
* 项目名:        CSASPNETBackgroundWorker
* 版权(c) Microsoft Corporation
*
* 当应用程序启动时，Application_Start()方法将被调用。
* 在Application_Start()方法中,它创建一个BackgroundWorker对象，
* 然后在应用程序状态保存起来.因此worker_DoWork()将继续执行,直到应用程序结束.
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
    public class Global : System.Web.HttpApplication
    {
        /// <summary>
        /// 当应用程序开始时创建实行操作的后台工作.
        /// </summary>
        protected void Application_Start(object sender, EventArgs e)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += new BackgroundWorker.DoWorkEventHandler(worker_DoWork);
            worker.RunWorker(null);

            // 后台工作是应用程序级的,
            // 它将继续工作并被所有用户共享.
            Application["worker"] = worker;
        }

        /// <summary>
        /// 操作将运行到结束.
        /// </summary>
        void worker_DoWork(ref int progress, 
            ref object _result, params object[] arguments)
        {
            // 结束前每秒执行一次操作.
            while (true)
            {
                Thread.Sleep(1000);

                // 此语句每秒运行一次.
                progress++;

                // 其他你想运行的逻辑.
                // 你可以通过检查DateTime.Now运行些计划任务.
            }
        }
    }
}