/******************************** 模块头 ********************************\				  
* 模块名：	Solution2.cs
* 项目名：	CSAutomatePowerPoint
* 版权 (c) Microsoft Corporation.
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
\*******************************************************************************/

#region Using directives
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;

using Office = Microsoft.Office.Core;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;
#endregion


namespace CSAutomatePowerPoint
{
    static class Solution2
    {
        public static void AutomatePowerPoint()
        {
            AutomatePowerPointImpl();


	    // 在自动化功能函数弹出栈后（在此时的RCW对象都是不可达的）就开始
	    // 强制一次垃圾回收来清理RCW对象和释放COM对象。

            GC.Collect();
            GC.WaitForPendingFinalizers();
	    // 为了使终结器（Finalizers）被调用，需要两次调用GC
	    // 第一次调用，它只简单的列出需要被终结的对象，
	    // 第二次调用，已经终结化了。
	    // 只有在那时，对象才会自动执行它们的ReleaseComObject方法。
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private static void AutomatePowerPointImpl()
        {
            try
            {
		// 创建一个Microsoft PowerPoint实例并使其不可见。

                PowerPoint.Application oPowerPoint = new PowerPoint.Application();
		// 默认情况下PowerPoint不可见，直道你使它可见。
                //oPowerPoint.Visible = Office.MsoTriState.msoFalse;

		// 创建一个新的演示文稿。

                PowerPoint.Presentation oPre = oPowerPoint.Presentations.Add(
                    Microsoft.Office.Core.MsoTriState.msoTrue);
                Console.WriteLine("一个新的演示文稿被建立");

		// 插入一个幻灯片，并为幻灯片加入一些文本。

                Console.WriteLine("插入一个幻灯片");
                PowerPoint.Slide oSlide = oPre.Slides.Add(1,
                    PowerPoint.PpSlideLayout.ppLayoutText);

                Console.WriteLine("添加一些文本");
                oSlide.Shapes[1].TextFrame.TextRange.Text =
                    "一站式代码框架";

		// 保存此演示文稿为pptx文件并将其关闭。

                Console.WriteLine("保存并退出演示文稿");

                string fileName = Path.GetDirectoryName(
                    Assembly.GetExecutingAssembly().Location) + "\\Sample2.pptx";
                oPre.SaveAs(fileName,
                    PowerPoint.PpSaveAsFileType.ppSaveAsOpenXMLPresentation,
                    Office.MsoTriState.msoTriStateMixed);
                oPre.Close();

		// 退出PowerPoint应用程序

                Console.WriteLine("退出PowerPoint应用程序");
                oPowerPoint.Quit();
            }
            catch (Exception ex)
            {
                Console.WriteLine("解决方案2.AutomatePowerPoint抛出的错误: {0}",
                    ex.Message);
            }
        }
    }
}
