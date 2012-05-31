/****************************** 模块头 ******************************\
* 模块名:  IReferenceIdentity.cs
* 项目名:	    CSCheckEXEType
* 版权 (c) Microsoft Corporation.
* 
* 表示代码对象唯一签名的一个引用.
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
    [Guid("6eaf5ace-7917-4f3c-b129-e046a9704766")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IReferenceIdentity
    {
        /// <summary>
        /// 获取一个程序集属性.
        /// </summary>
        /// <param name="attributeNamespace">属性命名空间.</param>
        /// <param name="attributeName">属性名.</param>
        /// <returns>程序集属性.</returns>
        [return: MarshalAs(UnmanagedType.LPWStr)]
        [SecurityCritical]
        string GetAttribute(
            [In, MarshalAs(UnmanagedType.LPWStr)] string attributeNamespace,
            [In, MarshalAs(UnmanagedType.LPWStr)] string attributeName);

        /// <summary>
        /// 设定一个程序集属性.
        /// </summary>
        /// <param name="attributeNamespace">属性命名空间.</param>
        /// <param name="attributeName">属性名.</param>
        /// <param name="attributeValue">属性值.</param>
        [SecurityCritical]

        void SetAttribute(
            [In, MarshalAs(UnmanagedType.LPWStr)] string attributeNamespace,
            [In, MarshalAs(UnmanagedType.LPWStr)] string attributeName,
            [In, MarshalAs(UnmanagedType.LPWStr)] string attributeValue);

        /// <summary>
        /// 获取程序集属性的迭代器.
        /// </summary>
        /// <returns>程序集属性的枚举.</returns>
        [SecurityCritical]
        IEnumIDENTITY_ATTRIBUTE EnumAttributes();

        /// <summary>
        /// 克隆 IReferenceIdentity.
        /// </summary>
        /// <param name="countOfDeltas">deltas数量.</param>
        /// <param name="deltas">The deltas.</param>
        /// <returns>IReferenceIdentity克隆体.</returns>
        [SecurityCritical]
        IReferenceIdentity Clone(
            [In] IntPtr countOfDeltas,
            [In, MarshalAs(UnmanagedType.LPArray)] IDENTITY_ATTRIBUTE[] deltas);
    }
}
