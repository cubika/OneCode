'********************************* 模块头 *********************************'
' 模块名:  Person.vb
' 项目名:      VBWinFormBindToNestedProperties
' 版权 (c) Microsoft Corporation.
' 
' 这个实例演示了如何将数据源中的嵌套的属性绑定到DataGridView控件中的列上.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*********************************************************************************'

Imports System.ComponentModel

' 在Person类上增加一个TypeDescriptionProviderAttribute属性
<TypeDescriptionProvider(GetType(MyTypeDescriptionProvider))> _
Public Class Person
    Private _id As String
    Private _name As String
    Private _homeAddr As Address

    Public Property ID() As String
        Get
            Return _id
        End Get
        Set(ByVal value As String)
            _id = value
        End Set
    End Property

    Public Property Name() As String
        Get
            Return _name
        End Get
        Set(ByVal value As String)
            _name = value
        End Set
    End Property

    Public Property HomeAddr() As Address
        Get
            Return _homeAddr
        End Get
        Set(ByVal value As Address)
            _homeAddr = value
        End Set
    End Property
End Class

Public Class Address
    Private _cityname As String
    Private _postcode As String

    Public Property Cityname() As String
        Get
            Return _cityname
        End Get
        Set(ByVal value As String)
            _cityname = value
        End Set
    End Property

    Public Property Postcode() As String
        Get
            Return _postcode
        End Get
        Set(ByVal value As String)
            _postcode = value
        End Set
    End Property

End Class