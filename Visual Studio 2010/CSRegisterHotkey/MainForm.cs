/********************************* 模块头 **********************************\
* 模块名:  MainForm.cs
* 项目名:      CSRegisterHotkey
* 版权(c)  Microsoft Corporation.
* 
* 这是这个应用程序的主窗体.它是用来初始化界面并处理事件的.
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

namespace CSRegisterHotkey
{
    public partial class MainForm : Form
    {
        HotKeyRegister hotKeyToRegister = null;

        Keys registerKey = Keys.None;

        KeyModifiers registerModifiers = KeyModifiers.None;

        public MainForm()
        {
            InitializeComponent();
        }


        /// <summary>
        /// 处理tbHotKey的KeyDown，事件处理中要检查按键。
        /// 按键必须是包含Ctrl，Shift或者Alt键的，比如Ctrl+Alt+T，HotKeyRegister.GetMo-
        ///difiers方法能够检查是否“T”按键被按了。
        /// </summary>
        private void tbHotKey_KeyDown(object sender, KeyEventArgs e)
        {
            // 按键事件不应该被发送到底层控制。
            e.SuppressKeyPress = true;

            // 检查修饰键是否被按下。
            if (e.Modifiers != Keys.None)
            {
                Keys key = Keys.None;
                KeyModifiers modifiers = HotKeyRegister.GetModifiers(e.KeyData, out key);

                // 如果按下的按键是有效的。
                if (key != Keys.None)
                {
                    this.registerKey = key;
                    this.registerModifiers = modifiers;

                    // 在文本框中显示按下的按键。
                    tbHotKey.Text = string.Format("{0}+{1}",
                        this.registerModifiers, this.registerKey);

                    // 启动按钮。
                    btnRegister.Enabled = true;
                }
            }
        }


        /// <summary>
        /// 处理btnRegister的Click事件。
        /// </summary>
        private void btnRegister_Click(object sender, EventArgs e)
        {
            try
            {
                // 注册热键。
                hotKeyToRegister = new HotKeyRegister(this.Handle, 100,
                    this.registerModifiers, this.registerKey);

                // 注册热键事件。
                hotKeyToRegister.HotKeyPressed += new EventHandler(HotKeyPressed);

                // 更新界面。
                btnRegister.Enabled = false;
                tbHotKey.Enabled = false;
                btnUnregister.Enabled = true;
            }
            catch (ArgumentException argumentException)
            {
                MessageBox.Show(argumentException.Message);
            }
            catch (ApplicationException applicationException)
            {
                MessageBox.Show(applicationException.Message);
            }
        }

        /// <summary>
        /// 如果HotKeyPressed事件被触发的话，显示一个消息框。
        /// </summary>
        void HotKeyPressed(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.WindowState = FormWindowState.Normal;
            }
            this.Activate();
        }


        /// <summary>
        /// 处理btnUnregister的Click事件。
        /// </summary>
        private void btnUnregister_Click(object sender, EventArgs e)
        {
            // 处理hotKeyToRegister。
            if (hotKeyToRegister != null)
            {
                hotKeyToRegister.Dispose();
                hotKeyToRegister = null;
            }

            // 更新界面。
            tbHotKey.Enabled = true;
            btnRegister.Enabled = true;
            btnUnregister.Enabled = false;
        }


        /// <summary>
        /// 当窗体关闭的时候处理hotKeyToRegister。
        /// </summary>
        protected override void OnClosed(EventArgs e)
        {
            if (hotKeyToRegister != null)
            {
                hotKeyToRegister.Dispose();
                hotKeyToRegister = null;
            }

            base.OnClosed(e);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
    }
}
