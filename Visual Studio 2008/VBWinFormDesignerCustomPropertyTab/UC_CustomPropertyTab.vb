'************************************* 模块头 ********************************************\
' 模块名称:	UC_CustomPropertyTab.vb
' 项目:		VBWinFormDesignerCustomPropertyTab
' 版权所有 (c) Microsoft Corporation.
' 
' 
' CustomPropertyTab 例子演示了如何在属性窗口上添加自定义的PropertyTab
' 
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'******************************************************************************************/

Imports System.ComponentModel
Imports System.Windows.Forms.PropertyGridInternal


<PropertyTab(GetType(CustomTab), PropertyTabScope.Component)> _
Public Class UC_CustomPropertyTab

    Private _testProp As String

    <Browsable(False), CustomTabDisplay(True)> _
    Public Property TestProp() As String
        Get
            Return Me._testProp
        End Get
        Set(ByVal value As String)
            Me._testProp = value
        End Set
    End Property
End Class


Public Class CustomTab
    Inherits PropertiesTab

    Public Overrides Function CanExtend(ByVal extendee As Object) As Boolean
        Return TypeOf extendee Is UC_CustomPropertyTab
    End Function

    Public Overrides Function GetProperties(ByVal context As ITypeDescriptorContext, _
                    ByVal component As Object, ByVal attrs As Attribute()) _
                    As PropertyDescriptorCollection
        Return TypeDescriptor.GetProperties(component, _
            New Attribute() {New BrowsableAttribute(False), New CustomTabDisplayAttribute(True)})
    End Function

    Public Overrides ReadOnly Property TabName() As String
        Get
            Return "Custom Tab"
        End Get
    End Property
End Class


<AttributeUsage(AttributeTargets.Property)> _
Public Class CustomTabDisplayAttribute
    Inherits Attribute

    Private _display As Boolean

    Public Sub New(ByVal display As Boolean)
        Me.display = display
    End Sub

    Public Property Display() As Boolean
        Get
            Return Me._display
        End Get
        Set(ByVal value As Boolean)
            Me._display = value
        End Set
    End Property
End Class


