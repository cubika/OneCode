'*************************** 模块头 ******************************'
' 模块名:  IClrMetaHost.vb
' 项目名:	    VBCheckEXEType
' 版权 (c) Microsoft Corporation.
' 
' 
' 提供如下方法：根据公共语言运行时（CLR）的版本数字返回一个具体的版本
' 列出所有已安装的CLR.
' 列出指定进程所加载的所有运行时.
' 发现曾用于编译程序集的CLR版本.
' 用一个干净的运行时shutdown退出一个进程, 并查询传统的API binding.
' 
'
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
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports System.Security
Imports System.Text

Namespace Hosting
    <ComImport()>
    <SecurityCritical()>
    <InterfaceType(ComInterfaceType.InterfaceIsIUnknown)>
    <Guid("D332DB9E-B9B3-4125-8207-A14884F53216")>
    Public Interface IClrMetaHost
        <MethodImpl(MethodImplOptions.InternalCall, MethodCodeType:=MethodCodeType.Runtime)>
        Function GetRuntime(<[In](), MarshalAs(UnmanagedType.LPWStr)> ByVal version As String,
                            <[In](), MarshalAs(UnmanagedType.LPStruct)> ByVal interfaceId As Guid) _
                        As <MarshalAs(UnmanagedType.Interface)> Object

        <MethodImpl(MethodImplOptions.InternalCall, MethodCodeType:=MethodCodeType.Runtime)>
        Sub GetVersionFromFile(<[In](), MarshalAs(UnmanagedType.LPWStr)> ByVal filePath As String,
                               <Out(), MarshalAs(UnmanagedType.LPWStr)> ByVal buffer As StringBuilder,
                               <[In](), Out(), MarshalAs(UnmanagedType.U4)> ByRef bufferLength As UInteger)

        <MethodImpl(MethodImplOptions.InternalCall, MethodCodeType:=MethodCodeType.Runtime)>
        Function EnumerateInstalledRuntimes() As <MarshalAs(UnmanagedType.Interface)> IEnumUnknown

        <MethodImpl(MethodImplOptions.InternalCall, MethodCodeType:=MethodCodeType.Runtime)>
        Function EnumerateLoadedRuntimes(<[In]()> ByVal processHandle As IntPtr) _
        As <MarshalAs(UnmanagedType.Interface)> IEnumUnknown

        <PreserveSig(), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType:=MethodCodeType.Runtime)>
        Function Reserved01(<[In]()> ByVal reserved1 As IntPtr) As Integer
    End Interface
End Namespace