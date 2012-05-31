'**************************************** 模块头 *****************************************'
' 模块名:      ChatRoom.vb
' 项目名:      VBASPNETAJAXWebChat
' 版权 (c) Microsoft Corporation
'
' 在此文件中, 我们创建了一个用来序列化到客户端的ChatRoom数据的DataContract类.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
'
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'******************************************************************************************'

Imports System.Runtime.Serialization

<DataContract()> _
Public Class ChatRoom

    <DataMember()> _
    Public Property RoomID() As Guid
        Get
            Return m_RoomID
        End Get
        Private Set(ByVal value As Guid)
            m_RoomID = value
        End Set
    End Property
    Private m_RoomID As Guid


    <DataMember()> _
    Public Property RoomName() As String
        Get
            Return m_RoomName
        End Get
        Private Set(ByVal value As String)
            m_RoomName = value
        End Set
    End Property
    Private m_RoomName As String


    <DataMember()> _
    Public Property MaxUser() As Integer
        Get
            Return m_MaxUser
        End Get
        Private Set(ByVal value As Integer)
            m_MaxUser = value
        End Set
    End Property
    Private m_MaxUser As Integer


    <DataMember()> _
    Public Property CurrentUser() As Integer
        Get
            Return m_CurrentUser
        End Get
        Private Set(ByVal value As Integer)
            m_CurrentUser = value
        End Set
    End Property
    Private m_CurrentUser As Integer

    Public Sub New(ByVal id As Guid)
        Dim db As SessionDBDataContext = New SessionDBDataContext()
        Dim room = db.tblChatRooms.SingleOrDefault(Function(r) r.ChatRoomID = id)
        If room IsNot Nothing Then
            RoomID = id
            RoomName = room.ChatRoomName
            MaxUser = room.MaxUserNumber
            CurrentUser = (From t In room.tblTalkers
                         Where t.CheckOutTime Is Nothing
                         Select t).Count()
        End If
    End Sub

End Class
