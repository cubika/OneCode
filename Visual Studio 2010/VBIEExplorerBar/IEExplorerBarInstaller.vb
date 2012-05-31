'*************************** Module Header ******************************'
' 模块名称:  IEExplorerBarInstaller.vb
' 项目:	    VBIEExplorerBar
' Copyright (c) Microsoft Corporation.
' 
' IEExplorerBarInstaller类继承了类System.Configuration.Install.Installer。Install和Uninstall
' 方法会在这个应用程序安装或卸载时运行。
' 
' 这个操作要添加到安装程序的自定义操作中才能生效。
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


<RunInstaller(True)>
Partial Public Class IEExplorerBarInstaller
    Inherits Installer
    Public Sub New()
        InitializeComponent()
    End Sub

    ''' <summary>
    ''' 当安装程序的自定义操作执行和注册浏览器栏作为COM服务器时，这个方法被调用。
    ''' </summary>
    ''' <param name="stateSaver"></param>     
    Public Overrides Sub Install(ByVal stateSaver As System.Collections.IDictionary)
        MyBase.Install(stateSaver)

        Dim regsrv As New RegistrationServices()
        If Not regsrv.RegisterAssembly(Me.GetType().Assembly, AssemblyRegistrationFlags.SetCodeBase) Then
            Throw New InstallException("Failed To Register for COM")
        End If
    End Sub

    ''' <summary>
    ''' 当安装程序的自定义操作执行和注销浏览器栏时，这个方法被调用。
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
