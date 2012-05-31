'*************************** 模块头 ******************************'
' 模块名:  IDefinitionIdentity.vb
' 项目名:	    VBCheckEXEType
' 版权 (c) Microsoft Corporation.
' 
' 表示应用程序在当前范围内的代码的唯一签名. 
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

Imports System
Imports System.Runtime.InteropServices
Imports System.Security

Namespace Fusion

    <ComImport()>
    <Guid("587bf538-4d90-4a3c-9ef1-58a200a8a9e7")>
    <InterfaceType(ComInterfaceType.InterfaceIsIUnknown)>
    Friend Interface IDefinitionIdentity

        <SecurityCritical()>
        Function GetAttribute(<[In](), MarshalAs(UnmanagedType.LPWStr)> ByVal [Namespace] As String,
                              <[In](), MarshalAs(UnmanagedType.LPWStr)> ByVal Name As String) _
                          As <MarshalAs(UnmanagedType.LPWStr)> String

        <SecurityCritical()>
        Sub SetAttribute(<[In](), MarshalAs(UnmanagedType.LPWStr)> ByVal [Namespace] As String,
                         <[In](), MarshalAs(UnmanagedType.LPWStr)> ByVal Name As String,
                         <[In](), MarshalAs(UnmanagedType.LPWStr)> ByVal Value As String)

        <SecurityCritical()>
        Function EnumAttributes() As IEnumIDENTITY_ATTRIBUTE

        <SecurityCritical()>
        Function Clone(<[In]()> ByVal cDeltas As IntPtr,
                       <[In](), MarshalAs(UnmanagedType.LPArray)> ByVal Deltas() As IDENTITY_ATTRIBUTE) _
                   As IDefinitionIdentity

    End Interface
End Namespace