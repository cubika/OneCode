/********************************* 模块头 *********************************\
* 模块名:      NativeMethod.cs
* 项目名:      CSDllCOMServer
* 版权 (c) Microsoft Corporation.
* 
* 一些关于Native API P/Invoke的注释
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
using System.Runtime.InteropServices;
using System.Text;
#endregion


/// <summary>
/// Native 方法
/// </summary>
internal class NativeMethod
{
    /// <summary>
    /// 获取当前线程ID。
    /// </summary>
    /// <returns></returns>
    [DllImport("kernel32.dll")]
    internal static extern uint GetCurrentThreadId();

    /// <summary>
    /// 获取当前进程ID。
    /// </summary>
    [DllImport("kernel32.dll")]
    internal static extern uint GetCurrentProcessId();
}