'************************************* 模块头 **************************************\
' 模块名:  Form2.vb
' 项目名:      VBWinFormPassValueBetweenForms
' 版权 (c) Microsoft Corporation.
' 
' 这个实例演示如何在多个窗体之间传递值.
'  
' 这里有两种通用的方法可以实现在多个窗体之间传递值:
' 
' 1. 使用一个属性.
'    在目标窗体类中创建一个公共的属性，然后我们就可以通过设置这个属性的值的方式在多个窗体中传递值.
' 
' 2. 使用一个方法.
'    在目标窗体类中创建一个公共的方法，然后我们就可以通过把值作为参数传给这个方法的形式在多个窗体中传递值.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'******************************************************************************************/

Public Class Form2
    Inherits Form

    Public Sub New()
        MyBase.New()
        InitializeComponent()

        AddHandler Load, AddressOf Me.FrmPassValueBetweenForms2_Load

    End Sub

    Private Sub FrmPassValueBetweenForms2_Load(ByVal sender As Object, ByVal e As EventArgs)
        Me.lbDisplay.Text = Me._valueToPassBetweenForms
    End Sub

    Private _valueToPassBetweenForms As String

    Public Property ValueToPassBetweenForms() As String
        Get
            Return Me._valueToPassBetweenForms
        End Get
        Set(ByVal value As String)
            Me._valueToPassBetweenForms = value
        End Set
    End Property


    Public Sub SetValueFromAnotherForm(ByVal val As String)
        Me._valueToPassBetweenForms = val
    End Sub

End Class