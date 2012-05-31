/****************************** 模块头 ******************************\
* 模块名称:  IInputObject.cs
* 项目:	    CSIEExplorerBar
* 版权： (c) Microsoft Corporation.
* 
* 公开方法，改变用户界面是否激活并为用户在Shell中输入对象过程加速器
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

using System;
using System.Runtime.InteropServices;

namespace CSIEExplorerBar.NativeMethods
{
    [ComImport]
    [Guid("68284faa-6a48-11d0-8c78-00c04fd918b4")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IInputObject
    {
        void UIActivateIO(int fActivate, ref MSG msg);
        [PreserveSig]
        int HasFocusIO();
        [PreserveSig]
        int TranslateAcceleratorIO(ref MSG msg);
    }


}
