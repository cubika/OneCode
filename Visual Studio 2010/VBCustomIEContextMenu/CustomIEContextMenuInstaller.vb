'********************************** 模块头 ******************************'
' 模块名:  BHOInstaller.vb
' 项目名:  VBCustomIEContextMenu
' 版权 (c) Microsoft Corporation.
' 
' 类CustomIEContextMenuInstaller继承自类System.Configuration.Install.Installer..
' 当该应用程序被安装或卸载时，类的安装和卸载方法将会运行.
' 
' 该操作需要添加到安装程序的自定义操作中才能生效.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*************************************************************************

Imports System.ComponentModel
Imports System.Configuration.Install
Imports System.Runtime.InteropServices

<RunInstaller(True), ComVisible(False)>
Partial Public Class CustomIEContextMenuInstaller
    Inherits System.Configuration.Install.Installer
    Public Sub New()
        InitializeComponent()
    End Sub

    ''' <summary>
    ''' 当安装程序的自定义行为执行并作为 COM 服务器注册到扩展 bandoject 的工
    ''' 具栏时,该方法被调用.
    ''' </summary>
    ''' <param name="stateSaver"></param>     
    Public Overrides Sub Install(ByVal stateSaver As System.Collections.IDictionary)
        MyBase.Install(stateSaver)

        OpenImageMenuExt.RegisterMenuExt()

        Dim regsrv As New RegistrationServices()
        If Not regsrv.RegisterAssembly(Me.GetType().Assembly,
                                       AssemblyRegistrationFlags.SetCodeBase) Then
            Throw New InstallException("注册COM失败")
        End If
    End Sub

    ''' <summary>
    ''' 当卸载程序的自定义操作执行,并注销作为 COM 服务器扩展
    ''' bandobject的 工具栏,该方法被调用，
    ''' </summary>
    ''' <param name="stateSaver"></param>     
    Public Overrides Sub Uninstall(ByVal savedState As System.Collections.IDictionary)
        MyBase.Uninstall(savedState)

        OpenImageMenuExt.UnRegisterMenuExt()

        Dim regsrv As New RegistrationServices()
        If Not regsrv.UnregisterAssembly(Me.GetType().Assembly) Then
            Throw New InstallException("注销COM失败")
        End If
    End Sub
End Class
