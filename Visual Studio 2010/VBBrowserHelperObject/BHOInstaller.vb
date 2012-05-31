'*************************** 模块头 ******************************'
' 模块名称:  BHOInstaller.vb
' 项目名称:  VBBrowserHelperObject
' 版权：Copyright (c) Microsoft Corporation.
' 
' BHOInstaller类 继承了 System.Configuration.Install.Installer类.
' Install 和 Uninstall方法将会在这个应用程序安装或卸载时运行。
' 
' 这个操作必须在添加了installer自定义操作后才会生效
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*************************************************************************'

Imports System.ComponentModel
Imports System.Configuration.Install
Imports System.Runtime.InteropServices

<RunInstaller(True), ComVisible(False)>
Partial Public Class BHOInstaller
    Inherits Installer
    Public Sub New()
        InitializeComponent()
    End Sub

    ''' <summary>
    ''' 将在installer的自定义操作执行时被调用
    ''' 注册一个工具栏扩展一个COM服务标签对象.
    ''' </summary>
    ''' <param name="stateSaver"></param>     
    Public Overrides Sub Install(ByVal stateSaver As System.Collections.IDictionary)
        MyBase.Install(stateSaver)


        Dim regsrv As New RegistrationServices()
        If Not regsrv.RegisterAssembly(Me.GetType().Assembly,
                                       AssemblyRegistrationFlags.SetCodeBase) Then
            Throw New InstallException("Failed To Register for COM")
        End If
    End Sub

    ''' <summary>
    ''' 将在installer的自定义操作执行时被调用
    ''' 注册一个工具栏扩展一个COM服务标签对象.
    ''' </summary>
    ''' <param name="stateSaver"></param>     
    Public Overrides Sub Uninstall(ByVal savedState As System.Collections.IDictionary)
        MyBase.Uninstall(savedState)
        Dim regsrv As New RegistrationServices()
        If Not regsrv.UnregisterAssembly(Me.GetType().Assembly) Then
            Throw New InstallException("Failed To Unregister for COM")
        End If
    End Sub

End Class
