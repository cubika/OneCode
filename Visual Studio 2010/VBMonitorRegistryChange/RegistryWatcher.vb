'******************************* 模块头 ************************************\
' 模块名:   RegistryWatcher.vb
' 项目名:   VBMonitorRegistryChange
' 版权(c)   Microsoft Corporation.
' 
' 这个类派生自ManagementEventWatcher。本类被用于
' 1. 提供所支持的节点。
' 2. 从Hive和KeyPath中构造WqlEventQuery。
' 3. 把EventArrivedEventArgs包装为RegistryKeyChangeEventArg。
'
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**************************************************************************

Imports System.Collections.ObjectModel
Imports System.Management
Imports Microsoft.Win32

Friend Class RegistryWatcher
    Inherits ManagementEventWatcher
    Implements IDisposable

    Private Shared _supportedHives As ReadOnlyCollection(Of RegistryKey) = Nothing

    ''' <summary>
    ''' 对于HKEY_CLASSES_ROOT与HKEY_CURRENT_USER节点的更改不被RegistryEvent
    ''' 或派生自它的类，例如RegistryKeyChangeEvent所支持。
    ''' </summary>
    Public Shared ReadOnly Property SupportedHives() As ReadOnlyCollection(Of RegistryKey)
        Get
            If _supportedHives Is Nothing Then
                Dim hives() As RegistryKey =
                    {
                        Registry.LocalMachine,
                        Registry.Users,
                        Registry.CurrentConfig
                    }
                _supportedHives = Array.AsReadOnly(Of RegistryKey)(hives)
            End If
            Return _supportedHives
        End Get
    End Property


    Private _hive As RegistryKey
    Public Property Hive() As RegistryKey
        Get
            Return _hive
        End Get
        Private Set(ByVal value As RegistryKey)
            _hive = value
        End Set
    End Property

    Private _keyPath As String
    Public Property KeyPath() As String
        Get
            Return _keyPath
        End Get
        Private Set(ByVal value As String)
            _keyPath = value
        End Set
    End Property

    Private _keyToMonitor As RegistryKey
    Public Property KeyToMonitor() As RegistryKey
        Get
            Return _keyToMonitor
        End Get
        Private Set(ByVal value As RegistryKey)
            _keyToMonitor = value
        End Set
    End Property

    Public Event RegistryKeyChangeEvent As EventHandler(Of RegistryKeyChangeEventArgs)

    ''' <exception cref="System.Security.SecurityException">
    ''' 当前用户没有访问监控器中项的许可时抛出。
    ''' </exception> 
    ''' <exception cref="System.ArgumentException">
    ''' 当监控器中的项不存在时抛出。
    ''' </exception> 
    Public Sub New(ByVal hive As RegistryKey, ByVal keyPath As String)
        Me.Hive = hive
        Me.KeyPath = keyPath

        ' 如果你把这个项目的平台设为x86，则在一个64位的机器上运行此项目时，当你的项路径
        ' 是HKEY_LOCAL_MACHINE\SOFTWARE时，你将会在HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node
        ' 下找到注册表项。
        Me.KeyToMonitor = hive.OpenSubKey(keyPath)

        If KeyToMonitor IsNot Nothing Then
            ' 构造查询字符串
            Dim queryString As String = String.Format("SELECT * FROM RegistryKeyChangeEvent " & ControlChars.CrLf & "                   WHERE Hive = '{0}' AND KeyPath = '{1}' ", Me.Hive.Name, Me.KeyPath)

            Dim query As New WqlEventQuery()
            query.QueryString = queryString
            query.EventClassName = "RegistryKeyChangeEvent"
            query.WithinInterval = New TimeSpan(0, 0, 0, 1)
            Me.Query = query

            AddHandler EventArrived, AddressOf RegistryWatcher_EventArrived
        Else
            Dim message As String = String.Format("注册表项 {0}\{1} 不存在。", hive.Name, keyPath)
            Throw New ArgumentException(message)

        End If
    End Sub

    Private Sub RegistryWatcher_EventArrived(ByVal sender As Object, ByVal e As EventArrivedEventArgs)

        ' 从EventArrivedEventArgs.NewEvent.Properties中获取RegistryKeyChangeEventArgs。
        Dim args As New RegistryKeyChangeEventArgs(e.NewEvent)

        ' 引发事件处理句柄。 
        RaiseEvent RegistryKeyChangeEvent(sender, args)

    End Sub

    ''' <summary>
    ''' 释放RegistryKey。
    ''' </summary>
    Public Shadows Sub Dispose() Implements IDisposable.Dispose
        MyBase.Dispose()
        If Me.KeyToMonitor IsNot Nothing Then
            Me.KeyToMonitor.Dispose()
        End If
    End Sub

End Class
