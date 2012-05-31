'**************************************** 模块头 *****************************************'
' 模块名:       Message.vb
' 项目名:       VBASPNETAJAXWebChat
' 版权 (c) Microsoft Corporation
'
' 在此文件中, 我们创建了一个用来序列化到客户端的Message数据的DataContract类.
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
Public Class Message
    <DataMember()> _
    Public Property Talker() As String
        Get
            Return m_Talker
        End Get
        Private Set(ByVal value As String)
            m_Talker = value
        End Set
    End Property
    Private m_Talker As String

    <DataMember()> _
    Public Property MessageData() As String
        Get
            Return m_MessageData
        End Get
        Private Set(ByVal value As String)
            m_MessageData = value
        End Set
    End Property
    Private m_MessageData As String

    <DataMember()> _
    Public Property SendTime() As DateTime
        Get
            Return m_SendTime
        End Get
        Private Set(ByVal value As DateTime)
            m_SendTime = value
        End Set
    End Property
    Private m_SendTime As DateTime

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

    Public Sub New(ByVal message__1 As tblMessagePool, ByVal session As HttpContext)
        Talker = message__1.tblTalker.tblSession.UserAlias
        MessageData = message__1.message
        SendTime = message__1.SendTime
        IsFriend = (message__1.tblTalker.tblSession.SessionID _
                    <> session.Session.SessionID)
    End Sub
End Class

