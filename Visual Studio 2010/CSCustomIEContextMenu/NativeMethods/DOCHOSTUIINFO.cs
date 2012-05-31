/*************************************** 模块头*****************************\
* 模块名:  DOCHOSTUIINFO.cs
* 项目名:  CSCustomIEContextMenu
* 版权 (c) Microsoft Corporation.
* 
*　类 DOCHOSTUIINFO 被 IDocHostUIHandler::GetHostInfo 方法用来允许　MSHTML 来
*　检索有关主机用户界面要求的信息
* 
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System.Runtime.InteropServices;

namespace CSCustomIEContextMenu.NativeMethods
{
    [StructLayout(LayoutKind.Sequential)]
    [ComVisible(true)]
    public class DOCHOSTUIINFO
    {
        [MarshalAs(UnmanagedType.U4)]
        public int cbSize = Marshal.SizeOf(typeof(NativeMethods.DOCHOSTUIINFO));
        [MarshalAs(UnmanagedType.I4)]
        public int dwFlags;
        [MarshalAs(UnmanagedType.I4)]
        public int dwDoubleClick;
        [MarshalAs(UnmanagedType.I4)]
        public int dwReserved1;
        [MarshalAs(UnmanagedType.I4)]
        public int dwReserved2;
    }
}
