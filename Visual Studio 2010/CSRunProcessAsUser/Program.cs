/***********************************  模块头  **************************************\
* 模块名:  Program.cs
* 项目名:  CSRunProcessAsUser
* 版权 (c) Microsoft Corporation.
*
* Program.cs是该程序的入口点. 它是自动发生的.
*
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
*
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER 
* EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF 
* MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***********************************************************************************/

using System;
using System.Windows.Forms;

namespace CSRunProcessAsUser
{
    static class Program
    {
        /// <summary>
        /// 该应用程序的主要入口点.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
