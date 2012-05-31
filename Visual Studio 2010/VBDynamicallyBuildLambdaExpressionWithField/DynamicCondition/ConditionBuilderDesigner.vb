'******************************** 模块头 *************************************************'
' 模块名:  ConditionBuilderDesigner.vb
' 项目名:  VBDynamicallyBuildLambdaExpressionWithField
' 版权 (c) Microsoft Corporation.
' 
' ConditionBuilderDesigner.vb file 文件定义了一些集合，来演示如何与其他控件相结合.
'
'
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER 
' EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF 
' MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**********************************************************************************'

Imports System.ComponentModel
Imports System.ComponentModel.Design
Imports System.Windows.Forms.Design

' 设计器，用于为 ConditionBuilderDesigner 显示智能标签.
Friend Class ConditionBuilderDesigner
    Inherits ControlDesigner
    Private actions As New DesignerActionListCollection()

    ''' <summary>
    ''' 重写属性，用于集成 PropertyGrid 控件.
    ''' </summary>
    Public Overrides ReadOnly Property ActionLists() As DesignerActionListCollection
        Get
            If actions.Count = 0 Then
                actions.Add(New ConditionBuilderActionList(Me.Component))
            End If
            Return actions
        End Get
    End Property

    ''' <summary>
    ''' 为类型提供基类.这些类型定义了一系列用于创建智能标记面板的项目.   
    ''' </summary>
    Public Class ConditionBuilderActionList
        Inherits DesignerActionList
        Private cBuilder As ConditionBuilder
        Public Sub New(ByVal component As IComponent)
            MyBase.New(component)
            cBuilder = CType(component, ConditionBuilder)
        End Sub

        ''' <summary>
        ''' Lines 属性.
        ''' </summary>
        Public Property Lines() As Integer
            Get
                Return cBuilder.Lines
            End Get
            Set(ByVal value As Integer)
                GetPropertyByName("Lines").SetValue(cBuilder, value)
            End Set
        End Property

        ''' <summary>
        ''' OperatorType属性.
        ''' </summary>
        Public Property OperatorType() As ConditionBuilder.Compare
            Get
                Return cBuilder.OperatorType
            End Get
            Set(ByVal value As ConditionBuilder.Compare)
                GetPropertyByName("OperatorType").SetValue(cBuilder, value)
            End Set
        End Property

        ''' <summary>
        ''' Box 属性.
        ''' </summary>
        Private Function GetPropertyByName(ByVal propName As String) As PropertyDescriptor
            Dim prop = TypeDescriptor.GetProperties(cBuilder)(propName)
            If prop Is Nothing Then
                Throw New ArgumentException("Invalid Property.", propName)
            End If
            Return prop
        End Function

        ''' <summary>
        ''' 创建将在智能标签中显示的元素.
        ''' </summary>
        Public Overrides Function GetSortedActionItems() As System.ComponentModel.Design.DesignerActionItemCollection
            Dim items As New DesignerActionItemCollection()
            items.Add(New DesignerActionHeaderItem("Appearance"))
            items.Add(New DesignerActionPropertyItem("Lines", "Number of Lines:", "Appearance", "Sets the number of lines in the ConditionBuilder"))
            items.Add(New DesignerActionPropertyItem("OperatorType", "Default Operator:", "Appearance", "Default operator to use"))
            Return items
        End Function
    End Class
End Class
