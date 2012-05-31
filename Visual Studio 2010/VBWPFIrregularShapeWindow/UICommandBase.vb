'******************************** Module Header **********************************'
' 模块名:  UICommandBase.vb
' 项目名:      VBWPFIrregularShapeWindow
' Copyright (c) Microsoft Corporation.
'
' UICommandBase.vb文件定义了一个 继承自 RoutedCommand 的 RoutedUICommand 通过实现快捷绑定到命令实例变量
'
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
'
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER 
' EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF 
' MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**********************************************************************************'

Imports System.Text

Public MustInherit Class UICommandBase
    Inherits Window

    '<summary>
    ' 最小化窗体的命令
    ' 正如你所看到的，该命令也和键盘上的F3键挂钩
    '</summary>
    Public Shared MinimizeCmd As New RoutedUICommand("MinimizeCmd", "MinimizeCmd", GetType(UICommandBase),
                                                     New InputGestureCollection(New InputGesture() {New KeyGesture(Key.F3, ModifierKeys.None, "Minimize Cmd")}))

    ''' <summary>
    ''' 最大化窗体命令
    ''' 正如你所看到的，该命令也和键盘上的F4键挂钩
    ''' </summary>
    Public Shared MaximizeCmd As New RoutedUICommand("MaximizeCmd", "MaximizeCmd", GetType(UICommandBase),
                                                     New InputGestureCollection(New InputGesture() {New KeyGesture(Key.F4, ModifierKeys.None, "Maximize Cmd")}))

    ''' <summary>
    ''' 复原窗体命令
    ''' 正如你所看到的，该命令也和键盘上的F5键挂钩
    ''' </summary>
    Public Shared RestoreCmd As New RoutedUICommand("RestoreCmd", "RestoreCmd", GetType(UICommandBase),
                                                    New InputGestureCollection(New InputGesture() {New KeyGesture(Key.F5, ModifierKeys.None, "Restore Cmd")}))

    ''' <summary>
    ''' 关闭窗体命令
    ''' 正如你所看到的，该命令也和键盘上的F6键挂钩    ''' </summary>
    Public Shared CloseCmd As New RoutedUICommand("CloseCmd", "CloseCmd", GetType(UICommandBase),
                                                  New InputGestureCollection(New InputGesture() {New KeyGesture(Key.F6, ModifierKeys.None, "Close Cmd")}))


    Public Sub New()

    End Sub

End Class
