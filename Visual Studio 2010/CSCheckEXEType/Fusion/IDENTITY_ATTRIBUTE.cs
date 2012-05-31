/****************************** 模块头 ******************************\
* 模块名:  IDENTITY_ATTRIBUTE.cs
* 项目名:	    CSCheckEXEType
* 版权 (c) Microsoft Corporation.
* 
* 程序集属性. 包括关于 IReferenceIdentity 的数据.  
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

namespace CSCheckEXEType.Fusion
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct IDENTITY_ATTRIBUTE
    {
        [MarshalAs(UnmanagedType.LPWStr)]
        public string Namespace;

        [MarshalAs(UnmanagedType.LPWStr)]
        public string Name;

        [MarshalAs(UnmanagedType.LPWStr)]
        public string Value;

    }
}
