'**************************************** 模块头 *****************************************'
' 模块名:     RoomTalker.vb
' 项目名:     VBASPNETAJAXWebChat
' 版权 (c) Microsoft Corporation
'
' 在此文件中, 我们创建了一个用来序列化到客户端的聊天者数据的DataContract类.
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
Public Class RoomTalker
    <DataMember()> _
    Public Property TalkerAlias() As String
        Get
            Return m_TalkerAlias
        End Get
        Private Set(ByVal value As String)
            m_TalkerAlias = value
        End Set
    End Property
    Private m_TalkerAlias As String

    <DataMember()> _
    Public Property TalkerSession() As String
        Get
            Return m_TalkerSession
        End Get
        Private Set(ByVal value As String)
            m_TalkerSession = value
        End Set
    End Property
    Private m_TalkerSession As String

    <DataMember()> _
    Public Property TalkerIP() As String
        Get
            Return m_TalkerIP
        End Get
        Private Set(ByVal value As String)
            m_TalkerIP = value
        End Set
    End Property
    Private m_TalkerIP As String

    <DataMember()> _
    Public Property IsFriend() As Boolean
        Get
            Return m_IsFriend
        End Get
        Private Set(ByVal value As Boolean)
            m_IsFriend = value
        End Set
    End Property
    Private m_IsFriend As Boolean

    Public Sub New(ByVal talker As tblTalker, ByVal context As HttpContext)
        TalkerAlias = talker.tblSession.UserAlias
        TalkerIP = talker.tblSession.IP
        TalkerSession = talker.tblSession.SessionID
        IsFriend = (TalkerSession <> context.Session.SessionID)
    End Sub
End Class
