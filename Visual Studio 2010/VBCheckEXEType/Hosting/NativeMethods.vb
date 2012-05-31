'*************************** 模块头 ******************************'
' 模块名:  NativeMethods.vb
' 项目名:	    VBCheckEXEType
' 版权 (c) Microsoft Corporation.
' 
' 这个类包装了mscoree.dll里的 CLRCreateInstance 方法. 
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

Namespace Hosting
    Friend NotInheritable Class NativeMethods
        <DllImport("mscoree.dll", CharSet:=CharSet.Auto, SetLastError:=True, PreserveSig:=False)>
        Public Shared Sub CLRCreateInstance(ByRef clsid As Guid, ByRef riid As Guid,
                                            <Out(), MarshalAs(UnmanagedType.Interface)> ByRef metahostInterface As Object)
        End Sub

        Public Shared CLSID_CLRMetaHost As New Guid("9280188D-0E8E-4867-B30C-7FA83884E8DE")

        Public Shared IID_ICLRMetaHost As New Guid("D332DB9E-B9B3-4125-8207-A14884F53216")

    End Class
End Namespace