/****************************** 模块头 ******************************\
* 模块名：	Program.cs
* 项目名：	CSAutomatePowerPoint
* 版权 (c) Microsoft Corporation.
* 
* CSAutomatePowerPoint案例演示了怎样使用Visual C#代码来创建一个Microsoft 
* PowerPoint实例，创建一个演示文稿，添加一个新的幻灯片，向幻灯片中加入一些文本，
* 保存演示文稿，退出Microsoft PowerPoint并进行非托管的COM资源的清理。
* 
* Office自动化是基于组件对象模型（COM）的。当你通过托管代码调用COM对象的时候，会
* 自动产生一个运行时可调用包装（RCW）对象。RCW封送了.NET应用程序和COM对象之间的
* 调用。RCW持有对COM对象的引用计数。如果RCW对象的所有引用没有被完全释放，则Office
* 的COM对象也不会退出，而可能致使Office应用程序在自动化结束后也不能退出。为了确认
* Office应用程序完全退出，案例中演示了两种解决方案。
* 
* 解决方案1.AutomatePowerPoint 演示了自动化的Microsoft PowerPoint应用程序通过使用
* Microsoft PowerPoint主互操作程序集（PIA），和显式地将每一个COM存取器赋予一个新
* 的变量（这些变量需要在程序结束时显式的调用Marshal.FinalReleaseComObject方法来释
* 放它）。
* 
* 解决方案2.AutomatePowerPoint 演示了自动化Microsoft PowerPoint应用程序通过使用
* Microsoft PowerPoint PIA，并在自动化功能函数弹出栈后（在此时的RCW对象都是不可达
* 的）就开始强制一次垃圾回收来清理RCW对象和释放COM对象。
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
#endregion


namespace CSAutomatePowerPoint
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
	    // 解决方案1.AutomatePowerPoint 演示了自动化的Microsoft PowerPoint
	    // 应用程序通过使用Microsoft PowerPoint主互操作程序集（PIA），和显
	    // 式地将每一个COM存取器赋予一个新的变量（这些变量需要在程序结束时
	    // 显式的调用Marshal.FinalReleaseComObject方法来释放它）。
            Solution1.AutomatePowerPoint();

            Console.WriteLine();

	    // 解决方案2.AutomatePowerPoint 演示了自动化Microsoft PowerPoint应
	    // 用程序通过使用Microsoft PowerPoint PIA，并在自动化功能函数弹出栈
	    // 后（在此时的RCW对象都是不可达的）就开始强制一次垃圾回收来清理RCW
	    // 对象和释放COM对象。
            Solution2.AutomatePowerPoint();
        }
    }
}