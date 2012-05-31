/******************************** 模块头 ***************************************\
* 模块名:	Solution2.cs
* 项目名:	CSAutomateWord
* 版权(c)   Microsoft Corporation.
* 
* Solution2.AutomateWord阐述了通过Microsoft Word主要的互用组件（PIA）自动化Word
* 应用程序，在自动化方法退出堆栈后执行垃圾收集器（此时RCW对象不再被引用）,从而
* 清除RCW并释放COM对象的过程.
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

using Word = Microsoft.Office.Interop.Word;
#endregion


namespace CSAutomateWord
{
    static class Solution2
    {
        public static void AutomateWord()
        {
            AutomateWordImpl();


           
            // 在自动化方法退出堆栈后执行垃圾收集器（此时RCW对象不再被引用）
            // 从而清除RCW并释放COM对象

            GC.Collect();
            GC.WaitForPendingFinalizers();
            

            // 为了终止程序,垃圾收集器GC必须被调用两次
            // 第一次调用将生成要终止项的相关列表
            // 第二次则是执行终止命令,此时对象将自动执行COM对象资源的释放

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private static void AutomateWordImpl()
        {
            object missing = Type.Missing;
            object oEndOfDoc = @"\endofdoc";    // 预先定义的书签
            object notTrue = false;

            try
            {
                // 创建一个Microsoft Word实例并令其不可见

                Word.Application oWord = new Word.Application();
                oWord.Visible = false;
                Console.WriteLine("Word.Application is started");

                // 创建一个新的文档

                Word.Document oDoc = oWord.Documents.Add(ref missing, ref missing,
                    ref missing, ref missing);

                // 插入段落

                Console.WriteLine("Insert a paragraph");

                Word.Paragraph oPara = oDoc.Paragraphs.Add(ref missing);
                oPara.Range.Text = "Heading 1";
                oPara.Range.Font.Bold = 1;
                oPara.Range.InsertParagraphAfter();

                // 插入表格

                Console.WriteLine("Insert a table");

                Word.Range oBookmarkRng = oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
                Word.Table oTable = oDoc.Tables.Add(oBookmarkRng, 5, 2,
                    ref missing, ref missing);
                oTable.Range.ParagraphFormat.SpaceAfter = 6;
                for (int r = 1; r <= 5; r++)
                {
                    for (int c = 1; c <= 2; c++)
                    {
                        oTable.Cell(r, c).Range.Text = "r" + r + "c" + c;
                    }
                }

                // 改变列1和列2的宽度
                oTable.Columns[1].Width = oWord.InchesToPoints(2);
                oTable.Columns[2].Width = oWord.InchesToPoints(3);

                // 将文档保存为.docx文件并关闭

                Console.WriteLine("Save and close the document");

                object fileName = Path.GetDirectoryName(
                    Assembly.GetExecutingAssembly().Location) + "\\Sample2.docx";
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
                Console.WriteLine("Solution2.AutomateWord throws the error: {0}",
                       ex.Message);
            }
        }
    }
}