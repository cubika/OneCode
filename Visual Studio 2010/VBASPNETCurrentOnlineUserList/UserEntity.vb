'/**************************************** 模块头 *****************************************\
'* 模块名:    UserEntity.vb
'* 项目名:    VBASPNETCurrentOnlineUserList
'* 版权 (c) Microsoft Corporation
'*
'* 本类用以设定用户身份. 
'*
'* This source is subject to the Microsoft Public License.
'* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
'* All other rights reserved.
'*
'* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
'* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
'* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'\*****************************************************************************************/

Public Class UserEntity
    Public Sub New()
    End Sub
    ' Ticket.
    Private _ticket As String

    ' UserName.
    Private _username As String

    ' TrueName.
    Private _truename As String

    ' Role.
    Private _roleid As String

    ' 页面最后刷新时间.
    Private _refreshtime As DateTime

    ' 用户最后活动时间.
    Private _activetime As DateTime

    ' 用户Ip地址.
    Private _clientip As String

    Public Property Ticket() As String
        Get
            Return _ticket
        End Get
        Set(ByVal value As String)
            _ticket = value
        End Set
    End Property
    Public Property UserName() As String
        Get
            Return _username
        End Get
        Set(ByVal value As String)
            _username = value
        End Set
    End Property
    Public Property TrueName() As String
        Get
            Return _truename
        End Get
        Set(ByVal value As String)
            _truename = value
        End Set
    End Property
    Public Property RoleID() As String
        Get
            Return _roleid
        End Get
        Set(ByVal value As String)
            _roleid = value
        End Set
    End Property
    Public Property RefreshTime() As DateTime
        Get
            Return _refreshtime
        End Get
        Set(ByVal value As DateTime)
            _refreshtime = value
        End Set
    End Property
    Public Property ActiveTime() As DateTime
        Get
            Return _activetime
        End Get
        Set(ByVal value As DateTime)
            _activetime = value
        End Set
    End Property
    Public Property ClientIP() As String
        Get
            Return _clientip
        End Get
        Set(ByVal value As String)
            _clientip = value
        End Set
    End Property
End Class

