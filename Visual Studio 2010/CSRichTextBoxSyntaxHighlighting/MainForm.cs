/****************************** 模块头 ******************************\
* 模块名:  MainForm.cs
* 项目名:	    CSRichTextBoxSyntaxHighlighting 
* 版权 (c) Microsoft Corporation.
* 
* 这是应用程序的Main窗口。他是用来初始化UI界面和处理事件。
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
using System.Drawing;
using System.Windows.Forms;

namespace CSRichTextBoxSyntaxHighlighting 
{
    public partial class MainForm : Form
    {

        public MainForm()
        {
            InitializeComponent();

            // 初始化 XMLViewerSettings.
            XMLViewerSettings viewerSetting = new XMLViewerSettings
            {
                AttributeKey = Color.Red,
                AttributeValue = Color.Blue,
                Tag = Color.Blue,
                Element = Color.DarkRed,
                Value = Color.Black,
            };

            viewer.Settings = viewerSetting;

        }

        /// <summary>
        /// 处理按钮的 "btnProcess"单击事件.
        /// </summary>
        private void btnProcess_Click(object sender, EventArgs e)
        {
            try
            {
                viewer.Process(true);
            }
            catch (ApplicationException appException)
            {
                MessageBox.Show(appException.Message, "应用程序异常");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "异常");
            }

        }

        private void viewer_TextChanged(object sender, EventArgs e)
        {

        }

    }
}
