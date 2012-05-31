/********************************** 模块头 ******************************************\
* 模块名:  Settings.cs
* 项目名:  CSImageFullScreenSlideShow
* 版权 (c) Microsoft Corporation.
*
* 这段代码为Timer控件设置了时间间隔.
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

namespace CSImageFullScreenSlideShow
{
    public partial class Settings : Form
    {
        Timer _timr;
       
        /// <summary>
        /// 为引入控件引用地址自定义构造函数.
        /// </summary>
        public Settings(ref Timer timr)
        {
            InitializeComponent();
            _timr = timr;
            this.dtpInternal.Value = timr.Interval;
        }

        /// <summary>
        /// 取消操作.
        /// </summary>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 保存Timer控件的时间间隔，关闭子窗体.
        /// </summary>
        private void btnConfirm_Click(object sender, EventArgs e)
        {
            _timr.Interval = int.Parse(this.dtpInternal.Value.ToString());
            this.Close();
        }

    

    }
}
