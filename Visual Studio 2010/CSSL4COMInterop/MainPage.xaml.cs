/****************************** 模块头*************************************\
* 模块名:   MainPage.xaml.cs
* 项目名:   CSSL4COMInterop
* 版权 (c) Microsoft Corporation.
* 
* Silverlight COM 后台代码文件交互操作.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Runtime.InteropServices.Automation;
using System.Collections;
using System.Threading;

namespace CSSL4COMInterop
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            // 新建实体列表
            var list = new List<PersonEntity>();
            var rand = new Random();
            for (int i = 0; i < 9; i++)
                list.Add(new PersonEntity
                {
                    Name = "姓名:" + i,
                    Age = rand.Next(20),
                    Gender = (i % 2 == 0 ? "男" : "女"),
                });

            // 将实体列表绑定到数据表格.
            dataGrid1.ItemsSource = list;
        }


        bool _isprint;
        // 更新 "print directly" 状态.
        private void CheckBox_StateChanged(object sender, RoutedEventArgs e)
        {
            var state = ((CheckBox)sender).IsChecked;
            if (state.HasValue && state.Value)
                _isprint = true;
            else
                _isprint = false;
        }

        // 将数据导出到记事本.
        private void TextExport_Click(object sender, RoutedEventArgs e)
        {
            // 检查是否允许使用AutomationFactory.
            if (!AutomationFactory.IsAvailable)
            {
                MessageBox.Show("这个函数要求silverlight应用程序在受信任的OOB模式下运行.");
            }
            else
            {
                // 使用shell打开记事本应用程序.
                using (dynamic shell = AutomationFactory.CreateObject("WScript.Shell"))
                {
                    shell.Run(@"%windir%\notepad", 5);
                    Thread.Sleep(100);

                    shell.SendKeys("Name{Tab}Age{Tab}Gender{Enter}");
                    foreach (PersonEntity item in dataGrid1.ItemsSource as IEnumerable)
                        shell.SendKeys(item.Name + "{Tab}" + item.Age + "{Tab}" + item.Gender + "{Enter}");
                }
            }
        }

        // 将数据导出到Word.
        private void WordExport_Click(object sender, RoutedEventArgs e)
        {
            // 检查是否允许使用AutomationFactory.
            if (!AutomationFactory.IsAvailable)
            {
                MessageBox.Show("这个函数要求silverlight应用程序在受信任的OOB模式下运行.");
            }
            else
            {
                // 创建Word自动控制对象.
                dynamic word = AutomationFactory.CreateObject("Word.Application");
                word.Visible = true;

                // 新建一个Word文件.
                dynamic doc = word.Documents.Add();

                // 写标题
                dynamic range1 = doc.Paragraphs[1].Range;
                range1.Text = "Silverlight4 Word自动控制示例\n";
                range1.Font.Size = 24;
                range1.Font.Bold = true;

                var list = dataGrid1.ItemsSource as List<PersonEntity>;

                dynamic range2 = doc.Paragraphs[2].Range;
                range2.Font.Size = 12;
                range2.Font.Bold = false;

                // 创建表
                doc.Tables.Add(range2, list.Count+1, 3, null, null);

                dynamic cell = doc.Tables[1].Cell(1, 1);
                cell.Range.Text = "姓名";
                cell.Range.Font.Bold = true;

                cell = doc.Tables[1].Cell(1, 2);
                cell.Range.Text = "年龄";
                cell.Range.Font.Bold = true;

                cell = doc.Tables[1].Cell(1, 3);
                cell.Range.Text = "性别";
                cell.Range.Font.Bold = true;

                // 在表格中填写数据
                for (int i = 0; i < list.Count; i++)
                {
                    cell = doc.Tables[1].Cell(i + 2, 1);
                    cell.Range.Text = list[i].Name;

                    cell = doc.Tables[1].Cell(i + 2, 2);
                    cell.Range.Text = list[i].Age;

                    cell = doc.Tables[1].Cell(i + 2, 3);
                    cell.Range.Text = list[i].Gender;
                }

                if (_isprint)
                {
                    // 不需要预览，直接打印Word.
                    doc.PrintOut();
                }

            }
        }
    }
    
}
