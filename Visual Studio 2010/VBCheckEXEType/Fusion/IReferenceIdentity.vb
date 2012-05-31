'*************************** 模块头 ******************************'
' 模块名:  IReferenceIdentity.vb
' 项目名:	    VBCheckEXEType
' 版权 (c) Microsoft Corporation.
' 
' 表示代码对象唯一签名的一个引用.
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
    <ComImport(), Guid("6eaf5ace-7917-4f3c-b129-e046a9704766"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)>
    Friend Interface IReferenceIdentity
        ''' <summary>
        ''' 获取一个程序集属性.
        ''' </summary>
        ''' <param name="attributeNamespace">属性命名空间.</param>
        ''' <param name="attributeName">属性名.</param>
        ''' <returns>程序集属性.</returns>
        <SecurityCritical()>
        Function GetAttribute(<[In](), MarshalAs(UnmanagedType.LPWStr)> ByVal attributeNamespace As String,
                          <[In](), MarshalAs(UnmanagedType.LPWStr)> ByVal attributeName As String) As <MarshalAs(UnmanagedType.LPWStr)> String

        ''' <summary>
        ''' 设定一个程序集属性.
        ''' </summary>
        ''' <param name="attributeNamespace">属性命名空间.</param>
        ''' <param name="attributeName">属性名.</param>
        ''' <param name="attributeValue">属性值.</param>
        <SecurityCritical()>
        Sub SetAttribute(<[In](), MarshalAs(UnmanagedType.LPWStr)> ByVal attributeNamespace As String,
                     <[In](), MarshalAs(UnmanagedType.LPWStr)> ByVal attributeName As String,
                     <[In](), MarshalAs(UnmanagedType.LPWStr)> ByVal attributeValue As String)

        ''' <summary>
        ''' 获取程序集属性的迭代器.
        ''' </summary>
        ''' <returns>程序集属性的枚举.</returns>
        <SecurityCritical()>
        Function EnumAttributes() As IEnumIDENTITY_ATTRIBUTE

        ''' <summary>
        ''' 克隆IReferenceIdentity.
        ''' </summary>
        ''' <param name="countOfDeltas">deltas数量.</param>
        ''' <param name="deltas">The deltas.</param>
        ''' <returns> IReferenceIdentity克隆体.</returns>
        <SecurityCritical()>
        Function Clone(<[In]()> ByVal countOfDeltas As IntPtr,
                   <[In](), MarshalAs(UnmanagedType.LPArray)> ByVal deltas() As IDENTITY_ATTRIBUTE) As IReferenceIdentity
    End Interface
End Namespace