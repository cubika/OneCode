/********************************* 模块头 **********************************\
* 模块名:  HotKeyRegister.cs
* 项目名:  CSRegisterHotkey
* 版权(c)  Microsoft Corporation.
* 
* 该类引入了user32.dll的RegisterHotKey和UnregisterHotKey方法用来定义和取消系统的热键。
* 
* Application.AddMessageFilter方法被用于添加一个信息过滤器，添加的目的是为了监控窗体
* 信息，因为它们是被选到它们的目的地。在一条信息被分配之前，PreFilterMessage方法能处理
* 它，如果一条WM_HOTKEY信息被热键创建成，但是它已经被HotKeyRegister对象注册过，那么就
* 会触发一个HotKeyPressed事件。 
* 
* 该类也支持静态方法GetModifiers从KeyEventArgs的KeyData属性获得修饰符和关键词。
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
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Security.Permissions;

namespace CSRegisterHotkey
{
    public class HotKeyRegister : IMessageFilter, IDisposable
    {
        /// <summary>
        /// 定义一个系统的热键。
        /// </summary>
        /// <param name="hWnd">
        /// 带有句柄的窗口会收到WM_HOTKEY消息，该消息是由热键产生的。 如果参数为空，WM_HOTKEY
        /// 消息将被指派到被调用的线程的消息队列中，而且在消息循环中必须被处理。
        /// </param>
        /// <param name="id">
        /// 热键的标识符。如果hWnd为空,那么热键适合当前线程联系在一块的而不是和一个特别的窗体。
        /// </param>
        /// <param name="fsModifiers">
        /// 被按下的按键要求必须是uVirtKey参数规定的按键，也是为了生成WM_HOTKEY消息。fsModif-
        /// iers参数能够结合下面的值。
        /// MOD_ALT     0x0001
        /// MOD_CONTROL 0x0002
        /// MOD_SHIFT   0x0004
        /// MOD_WIN     0x0008
        /// </param>
        /// <param name="vk">热键的虚拟键控代码。</param>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, 
            KeyModifiers fsModifiers, Keys vk);

        /// <summary>
        /// 通过调用线程提前取消一个热键。 
        /// </summary>
        /// <param name="hWnd">
        /// 含有一个句柄的窗口和热键的取消是联系在一块的，如果热键不能联系窗口，
        /// 那么参数将为空。
        /// </param>
        /// <param name="id">
        /// 热键的标识符被取消。
        /// </param>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        /// <summary>
        /// 从KeyEventArgs的KeyData属性获得编辑器和按键。
        /// </summary>
        /// <param name="keydata">
        /// KeyEventArgs的KeyData属性。KeyData是一种和编辑器结合的键。
        /// </param>
        /// <param name="key">按下的键</param>
        public static KeyModifiers GetModifiers(Keys keydata, out Keys key)
        {
            key = keydata;
            KeyModifiers modifers = KeyModifiers.None;

            // 检查keydata是否包含CTRL修饰符。
            // Keys.Control的值是131072。
            if ((keydata & Keys.Control) == Keys.Control)
            {
                modifers |= KeyModifiers.Control;

                key = keydata ^ Keys.Control;
            }

            // 检查keydata是否包含SHIFT修饰符。
            // Keys.Control的值是65536。
            if ((keydata & Keys.Shift) == Keys.Shift)
            {
                modifers |= KeyModifiers.Shift;
                key = key ^ Keys.Shift;
            }

            // 检查keydata是否包含ALT修饰符。
            // Keys.Control的值是262144。
            if ((keydata & Keys.Alt) == Keys.Alt)
            {
                modifers |= KeyModifiers.Alt;
                key = key ^ Keys.Alt;
            }

            // 检查是否有除了SHIFT, CTRL或者ALT按键(菜单)是被按下的。
            if (key == Keys.ShiftKey || key == Keys.ControlKey || key == Keys.Menu)
            {
                key = Keys.None;
            }

            return modifers;
        }

        /// <summary>
        /// 指定对象是否被处理了。
        /// </summary>
        bool disposed = false;

        /// <summary>
        /// 如果你安装了Windows SDK，在WinUser.h中常量能被发现。
        /// 每个窗口都有一个标识符，0x0312意味着这条消息是一条 
        /// WM_HOTKEY消息。
        /// </summary>
        const int WM_HOTKEY = 0x0312;

        /// <summary>
        /// 带一个句柄的窗口将收到热键产生的WM_HOTKEY消息。
        /// </summary>
        public IntPtr Handle { get; private set; }

        /// <summary>
        /// 一个正常的应用程序能用在0x0000和0xBFFF之间的任意值作为地址，但是如果你
        /// 正在写一个Dll，那么你必须用GlobalAddAtom为你的热键来获得一个唯一的标识
        /// 符。 
        /// </summary>
        public int ID { get; private set; }

        public KeyModifiers Modifiers { get; private set; }

        public Keys Key { get; private set; }

        /// <summary>
        /// 当热键被按下的时候触发事件。
        /// </summary>
        public event EventHandler HotKeyPressed;


        public HotKeyRegister(IntPtr handle, int id, KeyModifiers modifiers, Keys key)
        {
            if (key == Keys.None || modifiers == KeyModifiers.None)
            {
                throw new ArgumentException("键或者编辑器不能为空。");
            }

            this.Handle = handle;
            this.ID = id;
            this.Modifiers = modifiers;
            this.Key = key;

            RegisterHotKey();

            // 添加一个消息过滤器来监督窗体信息，因为它们是由目标选择路径的。

            Application.AddMessageFilter(this);
        }


        /// <summary>
        /// 注册热键。
        /// </summary>
        private void RegisterHotKey()
        {
            bool isKeyRegisterd = RegisterHotKey(Handle, ID, Modifiers, Key);

            // 如果操作失败，如果线程先前已经被注册了，试着注销热键。

            if (!isKeyRegisterd)
            {
                // IntPtr.Zero意思是热键被线程注册。

                UnregisterHotKey(IntPtr.Zero, ID);

                // 试着再次注册热键。

                isKeyRegisterd = RegisterHotKey(Handle, ID, Modifiers, Key);

                // 如果操作仍然失败，这就意味着该热键已经在另外一个线程或者进程中利用了。

                if (!isKeyRegisterd)
                {
                    throw new ApplicationException("热键已经被利用");
                }
            }
        }


        /// <summary>
        /// 在消息被派遣之前过滤出一条消息。
        /// </summary>
        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public bool PreFilterMessage(ref Message m)
        {
            // Message的WParam属性典型的是被用来存储一小块信息。在此，它存储的是地址。 
           
            if (m.Msg == WM_HOTKEY
                && m.HWnd == this.Handle
                && m.WParam == (IntPtr)this.ID
                && HotKeyPressed != null)
            {
                // 如果它是一条WM_HOTKEY信息，引发HotKeyPressed事件。

                HotKeyPressed(this, EventArgs.Empty);

                // 准确的去过滤消息并且阻止它被调度。

                return true;
            }

            // 返回false去允许消息继续道下一个过滤器或者控制器。
           
            return false;
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 注销热键。
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            // 阻止被多次调用。
            if (disposed)
            {
                return;
            }

            if (disposing)
            {

                // 从从消息泵的应用程序中清除一个消息过滤器。
                Application.RemoveMessageFilter(this);

                UnregisterHotKey(Handle, ID);
            }

            disposed = true;
        }
    }
}
