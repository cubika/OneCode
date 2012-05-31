/******************************** 模块头 ********************************\			  
* 模块名：	Solution1.cs
* 项目名：	CSAutomatePowerPoint
* 版权 (c) Microsoft Corporation.
* 
* 解决方案1.AutomatePowerPoint 演示了自动化的Microsoft PowerPoint应用程序通过使用
* Microsoft PowerPoint主互操作程序集（PIA），和显式地将每一个COM存取器赋予一个新
* 的变量（这些变量需要在程序结束时显式的调用Marshal.FinalReleaseComObject方法来释
* 放它）。当您在使用此解决方案时，重要的是避免潜入对象式的调用，因为那会使运行时可
* 调用包装（RCW）孤立的存在于堆上，当调用Marshal.ReleaseComObject时，您将访问不到
* RCW。您需要非常小心。例如，
* 
*   PowerPoint.Presentation oPre = oPowerPoint.Presentations.Add(
*     Office.MsoTriState.msoTrue);
*  
* 调用oPowerPoint.Presentations.Add产生了一个对应于Presentations对象的RCW。如果您
* 如以上代码那样通过潜入访问这些存取器，对应于Presentations对象的RCW被创建在GC堆上，
* 但它的引用被建立在栈上并且之后便被丢弃。这样的话，便没有方法可以在
* MarshalFinalReleaseComObject中访问到这个RCW对象了。为了使这些类的RCW被释放，您需
* 要在调用方法一被弹出栈便开始强制一次垃圾回收（参见解决方案2.AutomatePowerPoint），
* 或者您需要显示的将每一个存取器对象赋给一个变量，然后释放它。
* 
*   PowerPoint.Presentations oPres = oPowerPoint.Presentations;
*   PowerPoint.Presentation oPre = oPres.Add(Office.MsoTriState.msoTrue);
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
using System.IO;
using System.Text;
using System.Reflection;

using Office = Microsoft.Office.Core;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;
using System.Runtime.InteropServices;
#endregion


namespace CSAutomatePowerPoint
{
    static class Solution1
    {
        public static void AutomatePowerPoint()
        {
            PowerPoint.Application oPowerPoint = null;
            PowerPoint.Presentations oPres = null;
            PowerPoint.Presentation oPre = null;
            PowerPoint.Slides oSlides = null;
            PowerPoint.Slide oSlide = null;
            PowerPoint.Shapes oShapes = null;
            PowerPoint.Shape oShape = null;
            PowerPoint.TextFrame oTxtFrame = null;
            PowerPoint.TextRange oTxtRange = null;

            try
            {
		// 创建一个Microsoft PowerPoint实例并使其不可见。

                oPowerPoint = new PowerPoint.Application();

		// 默认情况下PowerPoint不可见，直道你使它可见。
                //oPowerPoint.Visible = Office.MsoTriState.msoFalse;

		// 创建一个新的演示文稿。

                oPres = oPowerPoint.Presentations;
                oPre = oPres.Add(Office.MsoTriState.msoTrue);
                Console.WriteLine("一个新的演示文稿被建立");

		// 插入一个幻灯片，并为幻灯片加入一些文本。

                Console.WriteLine("插入一个幻灯片");
                oSlides = oPre.Slides;
                oSlide = oSlides.Add(1, PowerPoint.PpSlideLayout.ppLayoutText);

                Console.WriteLine("添加一些文本");
                oShapes = oSlide.Shapes;
                oShape = oShapes[1];
                oTxtFrame = oShape.TextFrame;
                oTxtRange = oTxtFrame.TextRange;
                oTxtRange.Text = "一站式代码框架";

		// 保存此演示文稿为pptx文件并将其关闭。

                Console.WriteLine("保存并退出演示文稿");

                string fileName = Path.GetDirectoryName(
                    Assembly.GetExecutingAssembly().Location) + "\\Sample1.pptx";
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
                Console.WriteLine("解决方案1.AutomatePowerPoint抛出的错误: {0}",
                    ex.Message);
            }
            finally
            {
		// 显式地调用Marshal.FinalReleaseComObject访问所有的存取器对象清除
		// 非托管的COM资源。
		// 参见http://support.microsoft.com/kb/317109

                if (oTxtRange != null)
                {
                    Marshal.FinalReleaseComObject(oTxtRange);
                    oTxtRange = null;
                }
                if (oTxtFrame != null)
                {
                    Marshal.FinalReleaseComObject(oTxtFrame);
                    oTxtFrame = null;
                }
                if (oShape != null)
                {
                    Marshal.FinalReleaseComObject(oShape);
                    oShape = null;
                }
                if (oShapes != null)
                {
                    Marshal.FinalReleaseComObject(oShapes);
                    oShapes = null;
                }
                if (oSlide != null)
                {
                    Marshal.FinalReleaseComObject(oSlide);
                    oSlide = null;
                }
                if (oSlides != null)
                {
                    Marshal.FinalReleaseComObject(oSlides);
                    oSlides = null;
                }
                if (oPre != null)
                {
                    Marshal.FinalReleaseComObject(oPre);
                    oPre = null;
                }
                if (oPres != null)
                {
                    Marshal.FinalReleaseComObject(oPres);
                    oPres = null;
                }
                if (oPowerPoint != null)
                {
                    Marshal.FinalReleaseComObject(oPowerPoint);
                    oPowerPoint = null;
                }
            }
        }
    }
}
