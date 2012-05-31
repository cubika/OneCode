'********************************* 模块头 *********************************'
' 模块名:  MyCustomTypeDescriptor.vb
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
'\********************************************************************************'

Imports System.ComponentModel


Public Class MyCustomTypeDescriptor
    Inherits CustomTypeDescriptor

    Public Sub New(ByVal parent As ICustomTypeDescriptor)
        MyBase.New(parent)

    End Sub

    Public Overrides Function GetProperties(ByVal attributes() As Attribute) As PropertyDescriptorCollection
        ' 从一个类型中获取原有的PropertyDescriptorCollection集合
        Dim originalPds As PropertyDescriptorCollection = MyBase.GetProperties(attributes)
        Dim subPds As New List(Of PropertyDescriptor)

        Dim i As Integer = 0
        Do While (i < originalPds.Count)
            ' 从每一个原有的PropertyDescriptor中获取所有子属性
            Dim tempPds As PropertyDescriptorCollection = originalPds(i).GetChildProperties
            ' 如果子属性的个数大于0, 那么为每一个子属性创建一个新的 
            ' PropertyDescriptor并且把它加到subPds中
            If (tempPds.Count > 0) Then
                Dim j As Integer = 0
                Do While (j < tempPds.Count)
                    subPds.Add(New SubPropertyDescriptor(originalPds(i), tempPds(j), _
                        (originalPds(i).Name + ("_" + tempPds(j).Name))))
                    j = (j + 1)
                Loop
            End If
            i = (i + 1)
        Loop

        Dim array(originalPds.Count + subPds.Count) As PropertyDescriptor

        ' 复制原有的PropertyDescriptorCollection到一个数组中
        originalPds.CopyTo(array, 0)

        ' 复制所有的PropertyDescriptor的子属性到一个数组中
        subPds.CopyTo(array, originalPds.Count)
        Dim newPds As PropertyDescriptorCollection = New PropertyDescriptorCollection(array)

        ' 返回一个新的包含了原有属性以及他们子属性的PropertyDescriptor的PropertyDescriptorCollection集合
        Return newPds
    End Function

    Public Overrides Function GetProperties() As PropertyDescriptorCollection
        ' 从一个类型中获取原有的PropertyDescriptorCollection集合
        Dim originalPds As PropertyDescriptorCollection = MyBase.GetProperties()
        Dim subPds As New List(Of PropertyDescriptor)

        Dim i As Integer = 0
        Do While (i < originalPds.Count)
            ' 从每一个原有的PropertyDescriptor中获取所有子属性
            Dim tempPds As PropertyDescriptorCollection = originalPds(i).GetChildProperties
            ' 如果子属性的个数大于0, 那么为每一个子属性创建一个新的 
            ' PropertyDescriptor并且把它加到subPds中
            If (tempPds.Count > 0) Then
                Dim j As Integer = 0
                Do While (j < tempPds.Count)
                    subPds.Add(New SubPropertyDescriptor(originalPds(i), tempPds(j), _
                        (originalPds(i).Name + ("_" + tempPds(j).Name))))
                    j = (j + 1)
                Loop
            End If
            i = (i + 1)
        Loop

        Dim array(originalPds.Count + subPds.Count) As PropertyDescriptor

        ' 复制原有的PropertyDescriptorCollection到一个数组中
        originalPds.CopyTo(array, 0)

        ' 复制所有的PropertyDescriptor的子属性到一个数组中
        subPds.CopyTo(array, originalPds.Count)
        Dim newPds As PropertyDescriptorCollection = New PropertyDescriptorCollection(array)

        ' 返回一个新的包含了原有属性以及他们子属性的PropertyDescriptor的PropertyDescriptorCollection集合
        Return newPds
    End Function
End Class

Public Class MyTypeDescriptionProvider
    Inherits TypeDescriptionProvider

    Private td As ICustomTypeDescriptor

    Public Sub New()
        Me.New(TypeDescriptor.GetProvider(GetType(Person)))

    End Sub

    Public Sub New(ByVal parent As TypeDescriptionProvider)
        MyBase.New(parent)

    End Sub

    Public Overrides Function GetTypeDescriptor(ByVal objectType As Type, ByVal instance As Object) As ICustomTypeDescriptor
        If (td Is Nothing) Then
            td = MyBase.GetTypeDescriptor(objectType, instance)
            td = New MyCustomTypeDescriptor(td)
        End If
        Return td
    End Function
End Class

