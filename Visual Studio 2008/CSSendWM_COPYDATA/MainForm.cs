/********************************** 模块头 **********************************\
模块名:  MainForm.cs
项目名:      CSSendWM_COPYDATA
版权 (c) Microsoft Corporation.

基于windows 消息 WM_COPYDATA 进程间通信(IPC) 是一种在本地机器上windows应用程序交换数据机制。

接受程序必须是一个windows应用程序。数据被传递必须不包含指针或者不能被应用程序接受的指向对象的引用。

当发送WM_COPYDATA消息时，引用数据不能被发送进程别的线程改变。 接受应用程序应该只考虑只读数据。

如果接受应用程序想要在SendMessage返回之后进入数据， 它必须拷贝数据到本地缓存。

这个代码例子示范了通过SendMessage（WM_COPYDATA）发送一个客户端数据结构（MY_STRUCT）到接受应用程序
如果数据结构传值失败，应用程序显示一个诊断错误代码。一个典型的错误代码是0x5（非法访问），它是由于用户
接口权限隔离导致的。用户接口权限隔离阻止进程发送被选择的窗口消息和其他一些用户进程APIs，这些用户进程拥有
比较高的完整性。 当接受程序（CSReceiveWM_COPYDATA）运行在一个比发送程序高的完整性时候，你将会看到一个
"SendMessage(WM_COPYDATA) failed w/err 0x00000005"错误信息。

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
using System.Security;
#endregion


namespace CSSendWM_COPYDATA
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void btnSendMessage_Click(object sender, EventArgs e)
        {
            // 找到目标窗口句柄
            IntPtr hTargetWnd = NativeMethod.FindWindow(null, "CSReceiveWM_COPYDATA");
            if (hTargetWnd == IntPtr.Zero)
            {
                MessageBox.Show("不能发现  \"CSReceiveWM_COPYDATA\" 窗口", 
                    "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 准备好用来发送的带数据的COPYDATASTRUCT（COPYDATASTRUCT）
            MyStruct myStruct;

            int nNumber;
            if (!int.TryParse(this.tbNumber.Text, out nNumber))
            {
                MessageBox.Show("无效的数值!");
                return;
            }

            myStruct.Number = nNumber;
            myStruct.Message = this.tbMessage.Text;

            // 封装托管结构到本地内存块
            int myStructSize = Marshal.SizeOf(myStruct);
            IntPtr pMyStruct = Marshal.AllocHGlobal(myStructSize);
            try
            {
                Marshal.StructureToPtr(myStruct, pMyStruct, true);

                COPYDATASTRUCT cds = new COPYDATASTRUCT();
                cds.cbData = myStructSize;
                cds.lpData = pMyStruct;

                // 通过WM_COPYDATA消息发送COPYDATASTRUCT结构到接受窗口
                // （应用程序必须用SendMessage代替PostMessage 发送WM_COPYDATA
                // 因为接受程序必须接受，而这是有保证的。）
                NativeMethod.SendMessage(hTargetWnd, WM_COPYDATA, this.Handle, ref cds);

                int result = Marshal.GetLastWin32Error();
                if (result != 0)
                {
                    MessageBox.Show(String.Format(
                        "发送消息(WM_COPYDATA) 失败 w/err 0x{0:X}", result));
                }
            }
            finally
            {
                Marshal.FreeHGlobal(pMyStruct);
            }
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
        /// 一个应用程序发送WM_COPYDATA消息传送数据到另一个应用程序
        /// </summary>
        internal const int WM_COPYDATA = 0x004A;


        /// <summary>
        ///COPYDATASTRUCT 结构包含要被传送到另一个程序的数据
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct COPYDATASTRUCT
        {
            public IntPtr dwData;       // 指定要传送的数据
            public int cbData;          // 指定数据字节大小
            public IntPtr lpData;       // 指向传送数据的指针
        }


        /// <summary>
        /// 在这个代码例子中，这个类导出 Windows APIs备用
        /// </summary>
        [SuppressUnmanagedCodeSecurity]
        internal class NativeMethod
        {
            /// <summary>
            /// 发送指定的消息到窗口或系统。SendMessage函数调用指定的窗口过程，直到窗口过程处理完消息后才返回
            /// </summary>
            /// <param name="hWnd">
            /// 窗口句柄，它的窗口过程接受消息
            /// </param>
            /// <param name="Msg">指定被发送的消息</param>
            /// <param name="wParam">
            /// 指定额外的特别的消息
            /// </param>
            /// <param name="lParam">
            /// 指定额外的特别的消息信息
            /// </param>
            /// <returns></returns>
            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern IntPtr SendMessage(IntPtr hWnd, int Msg,
                IntPtr wParam, ref COPYDATASTRUCT lParam);


            /// <summary>
            /// FindWindow函数检索顶层窗口句柄
            /// 它的类名和窗口名字匹配特定的字符
            /// 这个函数不搜索子窗口
            /// 这个函数不执行大小写敏感搜索
            /// </summary>
            /// <param name="lpClassName">Class name</param>
            /// <param name="lpWindowName">Window caption</param>
            /// <returns></returns>
            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        }

        #endregion

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
    }
}
