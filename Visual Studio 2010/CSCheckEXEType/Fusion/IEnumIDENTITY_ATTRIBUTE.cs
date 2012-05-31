/**************************** 模块头 ******************************
* 模块名:  IEnumIDENTITY_ATTRIBUTE.vb
* 项目名:	    VBCheckEXEType
* 版权 (c) Microsoft Corporation.
* 
* 作为一个对代码属性的枚举服务于当前范围.
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
    [Guid("9cdaae75-246e-4b00-a26d-b9aec137a3eb")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IEnumIDENTITY_ATTRIBUTE
    {
        /// <summary>
        /// 获取下一个属性.
        /// </summary>
        /// <param name="celt">元素个数</param>
        /// <param name="attributes">将返回的属性数组.</param>
        /// <returns>下一个属性.</returns>
        [SecurityCritical]
        uint Next(
            [In] uint celt, 
            [Out, MarshalAs(UnmanagedType.LPArray)] IDENTITY_ATTRIBUTE[] rgAttributes);


        /// <summary>
        /// 将当前属性复制到缓冲区.
        /// </summary>
        /// <param name="available">可用字节数.</param>
        /// <param name="data">应将属性写入的缓冲区.</param>
        /// <returns>包含属性的缓冲区的指针.</returns>
        [SecurityCritical]
        IntPtr CurrentIntoBuffer(
            [In] IntPtr Available, 
            [Out, MarshalAs(UnmanagedType.LPArray)] byte[] Data);


        /// <summary>
        /// 跳过一些元素.
        /// </summary>
        /// <param name="celt">要跳过的元素个数.</param>
        [SecurityCritical]
        void Skip([In] uint celt);


        /// <summary>
        /// 将枚举重置到开始处.
        /// </summary>
        [SecurityCritical]
        void Reset();


        /// <summary>
        /// 克隆这个属性枚举.
        /// </summary>
        /// <returns>克隆一个 IEnumIDENTITY_ATTRIBUTE.</returns>
        [SecurityCritical]
        IEnumIDENTITY_ATTRIBUTE Clone();

    }
}
