'*************************** 模块头 ******************************'
' 模块名:  IEnumIDENTITY_ATTRIBUTE.vb
' 项目名:	    VBCheckEXEType
' 版权 (c) Microsoft Corporation.
' 
' 作为一个对代码属性的枚举服务于当前范围.
' 
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*************************************************************************'

Imports System.Runtime.InteropServices
Imports System.Security

Namespace Fusion
    <ComImport()>
    <Guid("9cdaae75-246e-4b00-a26d-b9aec137a3eb")>
    <InterfaceType(ComInterfaceType.InterfaceIsIUnknown)>
    Friend Interface IEnumIDENTITY_ATTRIBUTE
        ''' <summary>
        '''  获取下一个属性.
        ''' </summary>
        ''' <param name="celt">元素个数</param>
        ''' <param name="rgAttributes">将返回的属性数组.</param>
        ''' <returns>下一个属性.</returns>
        <SecurityCritical()>
        Function [Next](<[In]()> ByVal celt As UInteger,
                    <Out(), MarshalAs(UnmanagedType.LPArray)> ByVal rgAttributes() As IDENTITY_ATTRIBUTE) As UInteger


        ''' <summary>
        ''' 将当前属性复制到缓冲区
        ''' </summary>
        ''' <param name="available">可用字节数.</param>
        ''' <param name="data">应将属性写入的缓冲区.</param>
        ''' <returns>包含属性的缓冲区的指针.</returns>
        <SecurityCritical()>
        Function CurrentIntoBuffer(<[In]()> ByVal Available As IntPtr,
                               <Out(), MarshalAs(UnmanagedType.LPArray)> ByVal Data() As Byte) As IntPtr


        ''' <summary>
        ''' 跳过一些元素.
        ''' </summary>
        ''' <param name="celt">要跳过的元素个数.</param>
        <SecurityCritical()>
        Sub Skip(<[In]()> ByVal celt As UInteger)


        ''' <summary>
        ''' 将枚举重置到开始处.
        ''' </summary>
        <SecurityCritical()>
        Sub Reset()


        ''' <summary>
        ''' 克隆这个属性枚举.
        ''' </summary>
        ''' <returns>克隆一个 IEnumIDENTITY_ATTRIBUTE.</returns>
        <SecurityCritical()>
        Function Clone() As IEnumIDENTITY_ATTRIBUTE

    End Interface
End Namespace


