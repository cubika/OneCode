/****************************** 模块头 ************************************\
模块名:  CWin32WindowWrapper.cs
项目名:  CSOneNoteRibbonAddIn
版权 (c) Microsoft Corporation.

包装 Win32 HWND 手柄

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#region 引用的命名空间
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms; 
#endregion

namespace CSOneNoteRibbonAddIn
{
    internal class CWin32WindowWrapper : IWin32Window
    {
        // _windowHandle 字段
        private IntPtr _windowHandle;

        /// <summary>
        ///     CWin32WindowWrapper 构造器
        /// </summary>
        /// <param name="windowHandle">窗体句柄</param>
        public CWin32WindowWrapper(IntPtr windowHandle)
        {
            this._windowHandle = windowHandle;
        }

        // 总结:
        //     获取由实现者表示的窗口的句柄.
        //
        // 返回:
        //     由实现者表示的窗口的句柄.
        public IntPtr Handle
        {
            get
            {
                return this._windowHandle;
            }
        }
    }
}
