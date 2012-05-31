/****************************** 模块头 **************************************\
* 模块名:  MainForm.cs
* 项目名:  CSMonitorRegistryChange
* 版权(c)  Microsoft Corporation.
* 
* 这是本应用程序的主窗口。用于初始化UI并处理事件。
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
using System.Management;
using System.Windows.Forms;
using Microsoft.Win32;

namespace CSMonitorRegistryChange
{
    public partial class MainForm : Form
    {

        #region Fields

        // 当前状态
        bool isMonitoring = false;

        RegistryWatcher watcher = null;

        #endregion

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

            // 初始化cmbHives的数据源。对于HKEY_CLASSES_ROOT和HKEY_CURRENT_USER节点
            // 的改变不被RegistryEvent或它的派生类，如RegistryKeyChangeEvent事件所支持。   
            cmbHives.DataSource = RegistryWatcher.SupportedHives;

        }

        /// <summary>
        /// 处理btnMonitor的点击事件。
        /// </summary>
        private void btnMonitor_Click(object sender, EventArgs e)
        {

            // 如果应用程序正在监控注册表项，则停止监控并启用编辑器。
            if (isMonitoring)
            {
                bool success = StopMonitor();
                if (success)
                {
                    btnMonitor.Text = "开始监控";
                    cmbHives.Enabled = true;
                    tbRegkeyPath.ReadOnly = false;
                    isMonitoring = false;
                    lstChanges.Items.Add(string.Format("{0} 停止监控", DateTime.Now));
                }
            }

            // 如果应用程序是空闲的，则启动监控并禁用编辑器。
            else
            {
                bool success = StartMonitor();
                if (success)
                {
                    btnMonitor.Text = "停止监控";
                    cmbHives.Enabled = false;
                    tbRegkeyPath.ReadOnly = true;
                    isMonitoring = true;
                    lstChanges.Items.Add(string.Format("{0} 开始监控", DateTime.Now));
                }
            }

        }

        /// <summary>
        /// 检查被监控的项是否存在，然后启动ManagementEventWatcher来监控
        /// RegistryKeyChangeEvent事件。
        /// </summary>
        /// <returns>如果ManagementEventWatcher启动成功则为真。</returns>
        bool StartMonitor()
        {
            RegistryKey hive = cmbHives.SelectedValue as RegistryKey;
            var keyPath = tbRegkeyPath.Text.Trim();
            
            try
            {
                watcher = new RegistryWatcher(hive, keyPath);
            }

            // 当被监控的项不存在时，RegistryWatcher的构造器可能会抛出一个SecurityException异常。
            catch (System.ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }

            // 当前用户没有访问被监控项的权限时，RegistryWatcher的构造器可能会抛出一个
            // SecurityException异常。
            catch (System.Security.SecurityException)
            {
                string message = string.Format(
                    @"您没有访问项 {0}\{1} 的权限。",
                    hive.Name,
                    keyPath);
                MessageBox.Show(message);
                return false;
            }
                      
            try
            {
                          
                // 设置用于处理变更事件的句柄。
                watcher.RegistryKeyChangeEvent += new EventHandler<RegistryKeyChangeEventArgs>(
                    watcher_RegistryKeyChangeEvent);

                // 启动监听事件。
                watcher.Start();
                return true;
            }
            catch (System.Runtime.InteropServices.COMException comException)
            {
                MessageBox.Show("发生错误： " + comException.Message);
                return false;
            }
            catch (ManagementException managementException)
            {
                MessageBox.Show("发生错误： " + managementException.Message);
                return false;
            }

        }
     
        /// <summary>
        /// 停止监听事件。
        /// </summary>
        /// <returns>如果ManagementEventWatcher成功停止则为真。</returns>
        bool StopMonitor()
        {
            try
            {
                watcher.Stop();
                return true;
            }
            catch (ManagementException managementException)
            {
                MessageBox.Show("发生错误： " + managementException.Message);
                return false;
            }
            finally
            {
                watcher.Dispose();
            }
        }

        /// <summary>
        /// 处理RegistryKeyChangeEvent事件。
        /// </summary>
        void watcher_RegistryKeyChangeEvent(object sender, RegistryKeyChangeEventArgs e)
        {         
            string newEventMessage = string.Format(@"{0} 项 {1}\{2} 发生变化",
                e.TIME_CREATED.ToLocalTime(), e.Hive, e.KeyPath);
            lstChanges.Items.Add(newEventMessage);
        }
    }
}
