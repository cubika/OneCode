/********************************** 模板头 **********************************\
模板名:  MainForm.cs
项目名:      CSReceiveWM_COPYDATA
版权 (c) Microsoft Corporation.

基于windows 消息 WM_COPYDATA 进程间通信(IPC) 是一种在本地机器上windows应用程序交换数据机制。

接受程序必须是一个windows应用程序。数据被传递必须不包含指针或者不能被应用程序接受的指向对象的引用。

当发送WM_COPYDATA消息时，引用数据不能被发送进程别的线程改变。 接受应用程序应该只考虑只读数据。

如果接受应用程序想要在SendMessage返回之后进入数据， 它必须拷贝数据到本地缓存。

这个代码例子示范了接受一个从应用程序（CSSendWM_COPYDATA）通过处理WM_COPYDATA.发送到客户数据结构（Mystruct）.

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***********************************************************************************/

#region Using directives
using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
#endregion


namespace CSReceiveWM_COPYDATA
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_COPYDATA)
            {
                // 从lParam获取COPYDATASTRUCT结构
                COPYDATASTRUCT cds = (COPYDATASTRUCT)m.GetLParam(typeof(COPYDATASTRUCT));

                // 如果大小匹配
                if (cds.cbData == Marshal.SizeOf(typeof(MyStruct)))
                {
                    // 封装非托管内存数据到MyStruct托管结构
                    MyStruct myStruct = (MyStruct)Marshal.PtrToStructure(cds.lpData, 
                        typeof(MyStruct));

                    // 显示MyStruct数据成员
                    this.lbNumber.Text = myStruct.Number.ToString();
                    this.lbMessage.Text = myStruct.Message;
                }
            }

            base.WndProc(ref m);
        }


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct MyStruct
        {
            public int Number;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string Message;
        }


        #region Native API Signatures and Types

        /// <summary>
        /// 应用程序发送WM_COPYDATA消息传递数据到另一个程序
        /// </summary>
        internal const int WM_COPYDATA = 0x004A;


        /// <summary>
        ///COPYDATASTRUCT 结构包含通过WM_COPYDATA消息要传送到另一个应用程序的数据

        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct COPYDATASTRUCT
        {
            public IntPtr dwData;       // 指定被传送的数据
            public int cbData;          // 指定数据字节大小
            public IntPtr lpData;       // 指向被传送数据的指针
        }

        #endregion

        private void lbNumber_Click(object sender, EventArgs e)
        {

        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
    }
}
