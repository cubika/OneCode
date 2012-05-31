/****************************** 模块头 ******************************\
* 模块名:  IDefinitionIdentity.cs
* 项目名:	    CSCheckEXEType
* 版权 (c) Microsoft Corporation.
* 
* 表示应用程序在当前范围内的代码的唯一签名.
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
using System.Security;

namespace CSCheckEXEType.Fusion
{
    [ComImport]
    [Guid("587bf538-4d90-4a3c-9ef1-58a200a8a9e7")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IDefinitionIdentity
    {
        [return: MarshalAs(UnmanagedType.LPWStr)]
        [SecurityCritical]
        string GetAttribute(
            [In, MarshalAs(UnmanagedType.LPWStr)] string Namespace,
            [In, MarshalAs(UnmanagedType.LPWStr)] string Name);
        
        [SecurityCritical]
        void SetAttribute(
            [In, MarshalAs(UnmanagedType.LPWStr)] string Namespace, 
            [In, MarshalAs(UnmanagedType.LPWStr)] string Name, 
            [In, MarshalAs(UnmanagedType.LPWStr)] string Value);

        [SecurityCritical]
        IEnumIDENTITY_ATTRIBUTE EnumAttributes();

        [SecurityCritical]
        IDefinitionIdentity Clone(
            [In] IntPtr cDeltas, 
            [In, MarshalAs(UnmanagedType.LPArray)]
            IDENTITY_ATTRIBUTE[] Deltas);

    }
}
