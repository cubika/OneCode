'********************************** 模块头 ******************************'
' 模块名:  tagOleMenuGroupWidths.vb
' 项目名:  VBCustomIEContextMenu
' 版权 (c) Microsoft Corporation.
' 
' 类 tagOleMenuGroupWidths 被 IOleInPlaceFrame 使用.
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

Namespace NativeMethods
    <StructLayout(LayoutKind.Sequential)>
    Public NotInheritable Class tagOleMenuGroupWidths
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=6)>
        Public widths(5) As Integer
    End Class
End Namespace
