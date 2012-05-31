'********************************* 模块头 *********************************\
' 模块名: TransitionFactory.vb
' 项目名: VideoStoryCreator
' 版权(c) Microsoft Corporation.
' 
'用于创建特效和他们的设计器的工厂类.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************/



Public Class TransitionFactory
    Private Shared _transitionNameTypes As New Dictionary(Of String, Type)()
    Private Shared _transitionNameDesigners As New Dictionary(Of String, Type)()
    Private Shared _transitionNames As New List(Of String)()

    Shared Sub New()
        _transitionNameTypes.Add("Fade Transition", GetType(FadeTransition))
        _transitionNames.Add("Fade Transition")

        _transitionNameTypes.Add("Fly In Transition", GetType(FlyInTransition))
        _transitionNameDesigners.Add("Fly In Transition", GetType(FlyInTransition_Design))

        ' 在此注册更多特效...
        _transitionNames.Add("Fly In Transition")
    End Sub

    Private Sub New()
    End Sub

    ''' <summary>
    ''' 基于名称创建特效.
    ''' </summary>
    ''' <param name="name">特效名.</param>
    ''' <returns>对象.</returns>
    Public Shared Function CreateTransition(name As String) As ITransition
        If _transitionNameTypes.ContainsKey(name) Then
            Dim transitionType As Type = _transitionNameTypes(name)
            Try
                Return TryCast(Activator.CreateInstance(transitionType), ITransition)
                ' TODO: 是否应该抛出异常或者返回空?
            Catch
            End Try
        End If
        Return Nothing
    End Function

    ''' <summary>
    ''' 当前, 我们默认创建淡入淡出特效.
    ''' </summary>
    Public Shared Function CreateDefaultTransition() As ITransition
        Return New FadeTransition()
    End Function

    ''' <summary>
    ''' 基于名称创建特效设计器.
    ''' </summary>
    ''' <param name="name">特效名.</param>
    ''' <returns>用户控件. 所有设计器必须继承自 UserControl.</returns>
    Public Shared Function CreateTransitionDesigner(name As String) As UserControl
        If _transitionNameDesigners.ContainsKey(name) AndAlso _transitionNameDesigners(name) IsNot Nothing Then
            Dim transitionDeginerType As Type = _transitionNameDesigners(name)
            Try
                Return TryCast(Activator.CreateInstance(transitionDeginerType), UserControl)
                ' Todo: 是否应该抛出异常或者返回空?
            Catch
            End Try
        End If
        Return Nothing
    End Function

    ''' <summary>
    ''' 返回特效名列表.
    ''' </summary>
    Public Shared ReadOnly Property AvailableTransitions() As List(Of String)
        Get
            Return _transitionNames
        End Get
    End Property
End Class
