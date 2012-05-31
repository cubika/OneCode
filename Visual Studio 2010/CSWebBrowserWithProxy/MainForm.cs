/****************************** Module Header ******************************\
* 模块名称:  MainForm.cs
* 项目名称:		    CSWebBrowserWithProxy
* Copyright (c) Microsoft Corporation.
*
* 这是这个应用程序的主窗口。各种初始化和事件管理都在此完成。
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
using System.Windows.Forms;

namespace CSWebBrowserWithProxy
{
    public partial class MainForm : Form
    {
        // 获取现有的代理
        InternetProxy CurrentProxy
        {
            get
            {
                if (radNoProxy.Checked)
                {
                    return InternetProxy.NoProxy;
                }
                else
                {
                    return cmbProxy.SelectedItem as InternetProxy;
                }
            }
        }

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

            // 将代理数据列表绑定到组合列表框上
            cmbProxy.DisplayMember = "ProxyName";
            cmbProxy.DataSource = InternetProxy.ProxyList;
            cmbProxy.SelectedIndex = 0;

            wbcSample.StatusTextChanged += new EventHandler(wbcSample_StatusTextChanged);

        }

        /// <summary>
        /// 处理btnNavigate_Click 事件
        /// 本方法将为WebBrowser类封装WebBrowserControl类的Navigate方法用以在如果必要的情况下设置
        /// 代理的授信头信息
        /// </summary>
        private void btnNavigate_Click(object sender, EventArgs e)
        {
            try
            {
                wbcSample.Proxy = CurrentProxy;
                wbcSample.Goto(tbUrl.Text + "?id=" + Guid.NewGuid());
            }
            catch (ArgumentException)
            {
                MessageBox.Show("请确认你的url有效！");
            }
        }

        private void wbcSample_ProgressChanged(object sender, WebBrowserProgressChangedEventArgs e)
        {
            prgBrowserProcess.Value = (int)e.CurrentProgress;
            wbcSample.StatusTextChanged += new EventHandler(wbcSample_StatusTextChanged);
        }

        void wbcSample_StatusTextChanged(object sender, EventArgs e)
        {
            lbStatus.Text = wbcSample.StatusText;
        }
    }
}
