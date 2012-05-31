/******************************** 模块头 ***************************************\
* 模块名:	Solution1.cs
* 项目名:	CSAutomateWord
* 版权(c)   Microsoft Corporation.
* 
* Solution1.AutomateWord 阐述了通过Microsoft Word主要的互用组件（PIA）自动化Word
* 应用程序, 并且将每个COM访问对象分配给一个新的变量, 使用户最终可以通过调用
* Marshal.FinalReleaseComObject方法释放这些变量的过程. 使用该解决方案时,避免通过
* 层级式回溯对象模型的方法进行对象调用是十分重要的，因为这样会使运行库可调用包装
*（RCW）被孤立于堆上,以至于将无法调用Marshal.ReleaseComObject 对其进行访问. 
* 这是需要注意的地方. 例如, 
* 
*   Word.Document oDoc = oWord.Documents.Add(ref missing, ref missing,
*     ref missing, ref missing);
*  
* 调用 oWord.Documents.Add将为Documents对象创建RCW. 如果以代码所采用的层级式回溯
* 方式引用这些访问对象,文档的RCW将被创建在GC堆上,而引用则创建在栈上,然后被丢弃.
* 这样，将无法在RCW上调用MarshalFinalReleaseComObject.为了使该类型的RCW得以释放,
* 一种方法是在调用函数退出堆栈后立刻执行垃圾收集器GC（见Solution2.AutomateWord）,
* 另一种方法则是显式地将每个访问对象分配到一个变量并释放.
* 
*   Word.Documents oDocs = oWord.Documents;
*   Word.Document oDoc = oDocs.Add(ref missing, ref missing, ref missing, 
*     ref missing);
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

using Word = Microsoft.Office.Interop.Word;
using System.Runtime.InteropServices;
#endregion


namespace CSAutomateWord
{
    static class Solution1
    {
        public static void AutomateWord()
        {
            object missing = Type.Missing;
            object notTrue = false;

            Word.Application oWord = null;
            Word.Documents oDocs = null;
            Word.Document oDoc = null;
            Word.Paragraphs oParas = null;
            Word.Paragraph oPara = null;
            Word.Range oParaRng = null;
            Word.Font oFont = null;

            try
            {
                
                // 创建一个Microsoft Word实例并令其不可见

                oWord = new Word.Application();
                oWord.Visible = false;
                Console.WriteLine("Word.Application is started");

                // 创建一个新的文档

                oDocs = oWord.Documents;
                oDoc = oDocs.Add(ref missing, ref missing, ref missing, ref missing);
                Console.WriteLine("A new document is created");

                // 插入段落

                Console.WriteLine("Insert a paragraph");

                oParas = oDoc.Paragraphs;
                oPara = oParas.Add(ref missing);
                oParaRng = oPara.Range;
                oParaRng.Text = "Heading 1";
                oFont = oParaRng.Font;
                oFont.Bold = 1;
                oParaRng.InsertParagraphAfter();

                // 将文档保存为.docx文件并关闭

                Console.WriteLine("Save and close the document");

                object fileName = Path.GetDirectoryName(
                    Assembly.GetExecutingAssembly().Location) + "\\Sample1.docx";
                object fileFormat = Word.WdSaveFormat.wdFormatXMLDocument;
                oDoc.SaveAs(ref fileName, ref fileFormat, ref missing,
                    ref missing, ref missing, ref missing, ref missing,
                    ref missing, ref missing, ref missing, ref missing,
                    ref missing, ref missing, ref missing, ref missing,
                    ref missing);
                ((Word._Document)oDoc).Close(ref missing, ref missing,
                    ref missing);

                // 退出Word应用程序

                Console.WriteLine("Quit the Word application");
                ((Word._Application)oWord).Quit(ref notTrue, ref missing,
                    ref missing);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Solution1.AutomateWord throws the error: {0}",
                       ex.Message);
            }
            finally
            {
                
                // 通过在所有访问对象上显示调用Marshal.FinalReleaseComObject方法
                // 释放非托管Word COM资源
                // 见 http://support.microsoft.com/kb/317109.

                if (oFont != null)
                {
                    Marshal.FinalReleaseComObject(oFont);
                    oFont = null;
                }
                if (oParaRng != null)
                {
                    Marshal.FinalReleaseComObject(oParaRng);
                    oParaRng = null;
                }
                if (oPara != null)
                {
                    Marshal.FinalReleaseComObject(oPara);
                    oPara = null;
                }
                if (oParas != null)
                {
                    Marshal.FinalReleaseComObject(oParas);
                    oParas = null;
                }
                if (oDoc != null)
                {
                    Marshal.FinalReleaseComObject(oDoc);
                    oDoc = null;
                }
                if (oDocs != null)
                {
                    Marshal.FinalReleaseComObject(oDocs);
                    oDocs = null;
                }
                if (oWord != null)
                {
                    Marshal.FinalReleaseComObject(oWord);
                    oWord = null;
                }
            }
        }
    }
}
