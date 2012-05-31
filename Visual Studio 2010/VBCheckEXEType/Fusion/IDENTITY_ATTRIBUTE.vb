'*************************** 模块头 ******************************'
' 模块名:  IDENTITY_ATTRIBUTE.vb
' 项目名:	    VBCheckEXEType
' 版权 (c) Microsoft Corporation.
' 
' 程序集属性. 包括关于 IReferenceIdentity 的数据. 
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

Namespace Fusion
    <StructLayout(LayoutKind.Sequential)>
    Friend Structure IDENTITY_ATTRIBUTE
        <MarshalAs(UnmanagedType.LPWStr)>
        Public [Namespace] As String
        <MarshalAs(UnmanagedType.LPWStr)>
        Public Name As String
        <MarshalAs(UnmanagedType.LPWStr)>
        Public Value As String
    End Structure
End Namespace


