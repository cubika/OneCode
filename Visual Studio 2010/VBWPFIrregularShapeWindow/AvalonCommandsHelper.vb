'******************************** Module Header **********************************'
' 模块名:  AvalonCommandsHelper.vb
' Project:      VBWPFIrregularShapeWindow
' Copyright (c) Microsoft Corporation.
'
' AvalonCommandsHelper.vb文件定义了一个CanExecuteCommandSource方法，该方法负责是否可以运行
' 一个特殊的命令。
'
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
'
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER 
' EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF 
' MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*********************************************************************************'

Imports System.Text

''' <summary>
''' WPF 命令的辅助类
''' </summary>
Public NotInheritable Class AvalonCommandsHelper
    ''' <summary>
    ''' 当可以执行时给予一个特殊的方法
    ''' </summary>
    ''' <param name="commandSource">核实命令</param>
    ''' <returns></returns>
    Private Sub New()
    End Sub
    Public Shared Function CanExecuteCommandSource(ByVal commandSource As ICommandSource) As Boolean
        Dim baseCommand As ICommand = commandSource.Command
        If baseCommand Is Nothing Then
            Return False
        End If


        Dim commandParameter As Object = commandSource.CommandParameter
        Dim commandTarget As IInputElement = commandSource.CommandTarget
        Dim command As RoutedCommand = TryCast(baseCommand, RoutedCommand)
        If command Is Nothing Then
            Return baseCommand.CanExecute(commandParameter)
        End If
        If commandTarget Is Nothing Then
            commandTarget = TryCast(commandSource, IInputElement)
        End If
        Return command.CanExecute(commandParameter, commandTarget)
    End Function


End Class



