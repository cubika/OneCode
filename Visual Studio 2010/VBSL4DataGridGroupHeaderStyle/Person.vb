'********************************* 模块头 **********************************\
' 模块名:    Person.vb
' 项目:      VBSL4DataGridGroupHeaderStyle
' Copyright (c) Microsoft Corporation.
' 
'定义 Person 实体.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************/

Imports System.Collections.Generic

Public Class Person
    Public Property FirstName() As String
        Get
            Return m_FirstName
        End Get
        Set(ByVal value As String)
            m_FirstName = value
        End Set
    End Property
    Private m_FirstName As String
    Public Property LastName() As String
        Get
            Return m_LastName
        End Get
        Set(ByVal value As String)
            m_LastName = value
        End Set
    End Property
    Private m_LastName As String
    Public Property Gender() As String
        Get
            Return m_Gender
        End Get
        Set(ByVal value As String)
            m_Gender = value
        End Set
    End Property
    Private m_Gender As String
    Public Property AgeGroup() As String
        Get
            Return m_AgeGroup
        End Get
        Set(ByVal value As String)
            m_AgeGroup = value
        End Set
    End Property
    Private m_AgeGroup As String
End Class

Public Class People
    Public Shared Function GetPeople() As List(Of Person)
        Dim people As New List(Of Person)()
        people.Add(New Person() With {.FirstName = "Tom", .LastName = "Smith", .Gender = "M", .AgeGroup = "成人"})
        people.Add(New Person() With {.FirstName = "Emma", .LastName = "Smith", .Gender = "F", .AgeGroup = "成人"})
        people.Add(New Person() With {.FirstName = "Jacob", .LastName = "Smith", .Gender = "M", .AgeGroup = "成人"})
        people.Add(New Person() With {.FirstName = "Joshua", .LastName = "Smith", .Gender = "M", .AgeGroup = "儿童"})
        people.Add(New Person() With {.FirstName = "Michael", .LastName = "Smith", .Gender = "M", .AgeGroup = "儿童"})
        people.Add(New Person() With {.FirstName = "Emily", .LastName = "Smith", .Gender = "F", .AgeGroup = "儿童"})
        Return people
    End Function
End Class

