' **************************** Module Header ******************************\
' Module Name:  MainModule.vb
' Project:      VBXmlSerialization
' Copyright (c) Microsoft Corporation.
' 
' 此示例演示了如何将内存中的对象序列化到本地的XML文件，以及如何通过VB.Net将XML文
' 件反序列化到内存中的对象。
' 设计的MySerializableType包括整型，字符串，泛型，以及作为自定义的类型的字段和属性。
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************/

#Region "Using directives"
Imports System
Imports System.IO
Imports System.Collections.Generic
Imports System.Xml.Serialization
#End Region



Public Module MainModule
    Sub Main()
        '///////////////////////////////////////////////////////////////
        ' 将对象序列化到XML文件
        ' 

        ' 创建并初始化一个MySerializableType实例
        Dim instance As New MySerializableType()
        instance.BoolValueProperty = True
        instance.IntValueProperty = 1
        instance.StringValueProperty = "测试 String"
        instance.ListValueProperty.Add("List 项 1")
        instance.ListValueProperty.Add("List 项 2")
        instance.ListValueProperty.Add("List 项 3")
        instance.AnotherTypeValueProperty = New AnotherType()
        instance.AnotherTypeValueProperty.IntValueProperty = 2
        instance.AnotherTypeValueProperty.StringValueProperty = "测试内部元素 String"

        ' 创建序列化
        Dim serializer As New XmlSerializer(GetType(MySerializableType))

        ' 将对象序列化到XML文件
        Using streamWriter As StreamWriter = File.CreateText("VBXmlSerialization.xml")
            serializer.Serialize(streamWriter, instance)
        End Using


        '///////////////////////////////////////////////////////////////
        ' 将XML文件反序列化到一个对象的实例
        ' 

        ' 反序列化对象
        Dim deserializedInstance As MySerializableType
        Using streamReader As StreamReader = File.OpenText("VBXmlSerialization.xml")
            deserializedInstance = TryCast(serializer.Deserialize(streamReader), MySerializableType)
        End Using

        ' 对象结果输出
        Console.WriteLine("Bool值: {0}", deserializedInstance.BoolValueProperty)
        Console.WriteLine("Int值: {0}", deserializedInstance.IntValueProperty)
        Console.WriteLine("String值: {0}", deserializedInstance.StringValueProperty)
        Console.WriteLine("自定义类型元素的Int值: {0}", deserializedInstance.AnotherTypeValueProperty.IntValueProperty)
        Console.WriteLine("自定义类型元素的String值: {0}", deserializedInstance.AnotherTypeValueProperty.StringValueProperty)
        Console.WriteLine("List值: ")
        For Each obj As Object In deserializedInstance.ListValueProperty
            Console.WriteLine(obj.ToString())
        Next obj
    End Sub



    ''' <summary>
    ''' 序列化的类型声明
    ''' </summary>
    <Serializable()> _
    Public Class MySerializableType
        ' String类型字段和属性
        Private stringValue As String
        Public Property StringValueProperty() As String
            Get
                Return stringValue
            End Get
            Set(ByVal value As String)
                stringValue = value
            End Set
        End Property

        ' Bool类型字段和属性
        Private boolValue As Boolean
        Public Property BoolValueProperty() As Boolean
            Get
                Return boolValue
            End Get
            Set(ByVal value As Boolean)
                boolValue = value
            End Set
        End Property

        ' Int类型字段和属性
        Private intValue As Integer
        Public Property IntValueProperty() As Integer
            Get
                Return intValue
            End Get
            Set(ByVal value As Integer)
                intValue = value
            End Set
        End Property

        ' Another类型字段和属性
        Private anotherTypeValue As AnotherType
        Public Property AnotherTypeValueProperty() As AnotherType
            Get
                Return anotherTypeValue
            End Get
            Set(ByVal value As AnotherType)
                anotherTypeValue = value
            End Set
        End Property

        ' 泛型类型字段和属性
        Private listValue As New List(Of String)()
        Public Property ListValueProperty() As List(Of String)
            Get
                Return listValue
            End Get
            Set(ByVal value As List(Of String))
                listValue = value
            End Set
        End Property

        ' 用NonSerialized属性忽视的一个字段
        <NonSerialized()> _
        Private ignoredField As Integer = 1
    End Class

    ''' <summary>
    ''' Another类型声明
    ''' </summary>
    <Serializable()> _
    Public Class AnotherType

        Private stringValue As String
        Public Property StringValueProperty() As String
            Get
                Return stringValue
            End Get
            Set(ByVal value As String)
                stringValue = value
            End Set
        End Property


        Private intValue As Integer
        Public Property IntValueProperty() As Integer
            Get
                Return intValue
            End Get
            Set(ByVal value As Integer)
                intValue = value
            End Set
        End Property
    End Class
End Module

