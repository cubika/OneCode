/********************************** 模块头 *********************************\
* 模块名:      Program.cs
* 项目名:      CSExeCOMServer
* 版权 (c) Microsoft Corporation.
* 
* 该程序的主要入口点。它负责在启动一个注册在可执行文件中的进程外的COM服务器
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#region Using directives
using System;
using System.Collections.Generic;
using System.Text;
#endregion


namespace CSExeCOMServer
{
    class Program
    {
        /// <summary>
        /// 程序主入口
        /// </summary>
        static void Main(string[] args)
        {
            //  运行进程外COM服务器
            ExeCOMServer.Instance.Run();
        }
    }
}
