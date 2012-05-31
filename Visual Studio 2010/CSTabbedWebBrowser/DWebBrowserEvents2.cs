/****************************** 模块头 ******************************\
* 模块名:  DWebBrowserEvents2.cs
* 项目名:	    CSTabbedWebBrowser
* 版权 (c) Microsoft Corporation.
* 
* DWebBrowserEvents2接口指定一个事件接收器，一个应用程序必须从一个WebBrowser 
* 控件或从Windows Internet Explorer 应用程序中实现接收事件通知。 
* 事件通知包含NewWindow3事件，并且将会在应用程序中使用。
* 
* 要获得完整的DWebBrowserEvents2事件列表。
* 参见：http://msdn.microsoft.com/en-us/library/aa768283(VS.85).aspx
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

namespace CSTabbedWebBrowser
{
    [ComImport, TypeLibType(TypeLibTypeFlags.FHidden),
    InterfaceType(ComInterfaceType.InterfaceIsIDispatch),
    Guid("34A715A0-6587-11D0-924A-0020AFC7AC4D")]
    public interface DWebBrowserEvents2
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ppDisp">
        /// 一个接口指针, 可选择的, 接收一个新的WebBrowser对象的IDispatch接口指针
        /// 或是一个InternetExplorer对象的IDispatch接口指针
        /// 
        /// </param>
        /// <param name="Cancel">
        /// value 是否决定取消当前的导航
        /// </param>
        /// <param name="dwFlags">
        /// NWMF枚举的标识，与new Windows 相关
        /// 参见： http://msdn.microsoft.com/en-us/library/bb762518(VS.85).aspx.
        /// </param>
        /// <param name="bstrUrlContext">
        /// 这个页面的URL是打开新的窗口。
        /// </param>
        /// <param name="bstrUrl">在新窗口中打开URL.</param>
        [DispId(0x111)]
        void NewWindow3(
            [In, Out, MarshalAs(UnmanagedType.IDispatch)] ref object ppDisp,
            [In, Out] ref bool Cancel,
            [In] uint dwFlags,
            [In, MarshalAs(UnmanagedType.BStr)] string bstrUrlContext,
            [In, MarshalAs(UnmanagedType.BStr)] string bstrUrl);
    }
}
