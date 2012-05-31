'/**************************************** 模块头 *****************************************\
'* 模块名:    DataTableForCurrentOnlineUser.vb
'* 项目名:    VBASPNETCurrentOnlineUserList
'* 版权 (c) Microsoft Corporation
'*
'* 这个类用来初始化保存当前在线用户的信息的datatable.
'*
'* This source is subject to the Microsoft Public License.
'* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
'* All other rights reserved.
'*
'* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
'* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
'* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'\*****************************************************************************************/



Public Class DataTableForCurrentOnlineUser
    Private Shared _activeusers As DataTable
    Private _activeTimeout As Integer
    Private _refreshTimeout As Integer
    ''' <summary>
    ''' 初始化UserOnlineTable.
    ''' </summary> 
    Private Sub UsersTableFormat()
        If _activeusers Is Nothing Then
            _activeusers = New DataTable("ActiveUsers")
            Dim myDataColumn As DataColumn
            Dim mystringtype As System.Type
            mystringtype = System.Type.[GetType]("System.String")
            Dim mytimetype As System.Type
            mytimetype = System.Type.[GetType]("System.DateTime")
            myDataColumn = New DataColumn("Ticket", mystringtype)
            _activeusers.Columns.Add(myDataColumn)
            myDataColumn = New DataColumn("UserName", mystringtype)
            _activeusers.Columns.Add(myDataColumn)
            myDataColumn = New DataColumn("TrueName", mystringtype)
            _activeusers.Columns.Add(myDataColumn)
            myDataColumn = New DataColumn("RoleID", mystringtype)
            _activeusers.Columns.Add(myDataColumn)
            myDataColumn = New DataColumn("RefreshTime", mytimetype)
            _activeusers.Columns.Add(myDataColumn)
            myDataColumn = New DataColumn("ActiveTime", mytimetype)
            _activeusers.Columns.Add(myDataColumn)
            myDataColumn = New DataColumn("ClientIP", mystringtype)
            _activeusers.Columns.Add(myDataColumn)
        End If
    End Sub

    Public Sub New()
        ' 初始化保存当前在线用户的信息的datatable.
        UsersTableFormat()

        ' 初始化用户活动时间(分钟).
        Try
            _activeTimeout = Integer.Parse(ConfigurationManager.AppSettings("ActiveTimeout"))
        Catch
            _activeTimeout = 60
        End Try

        ' 初始化刷新时间(分钟).
        Try
            _refreshTimeout = Integer.Parse(ConfigurationManager.AppSettings("RefreshTimeout"))
        Catch
            _refreshTimeout = 1
        End Try
    End Sub
    Public ReadOnly Property ActiveUsers() As DataTable
        Get
            Return _activeusers.Copy()
        End Get
    End Property

    ''' <summary>
    ''' 登录方法.
    ''' </summary>
    Public Sub Login(ByVal user As UserEntity, ByVal singleLogin As Boolean)
        ' 清除离线用户记录.
        DelTimeOut()
        If singleLogin Then
            ' 让已登录用户登出.
            Me.Logout(user.UserName, False)
        End If
        Dim _myrow As DataRow
        Try
            _myrow = _activeusers.NewRow()
            _myrow("Ticket") = user.Ticket.Trim()
            _myrow("UserName") = user.UserName.Trim()
            _myrow("TrueName") = "" & user.TrueName.Trim()
            _myrow("RoleID") = "" & user.RoleID.Trim()
            _myrow("ActiveTime") = DateTime.Now
            _myrow("RefreshTime") = DateTime.Now
            _myrow("ClientIP") = user.ClientIP.Trim()
            _activeusers.Rows.Add(_myrow)
        Catch
            Throw
        End Try
        _activeusers.AcceptChanges()

    End Sub

    ''' <summary>
    ''' 用户登出,取决于ticket或username.
    ''' </summary> 
    Private Sub Logout(ByVal strUserKey As String, ByVal byTicket As Boolean)
        ' 清除离线用户记录.
        DelTimeOut()
        strUserKey = strUserKey.Trim()
        Dim _strExpr As String
        _strExpr = If(byTicket, "Ticket='" & strUserKey & "'", "UserName='" & strUserKey & "'")
        Dim _curuser As DataRow()
        _curuser = _activeusers.[Select](_strExpr)
        If _curuser.Length > 0 Then
            For i As Integer = 0 To _curuser.Length - 1
                _curuser(i).Delete()
            Next
        End If
        _activeusers.AcceptChanges()
    End Sub

    ''' <summary>
    ''' 根据ticket登出用户.
    ''' </summary>
    ''' <param name="strTicket">用户的ticket</param>
    Public Sub Logout(ByVal strTicket As String)
        Me.Logout(strTicket, True)
    End Sub

    ''' <summary>
    ''' 清除离线用户记录.
    ''' </summary>
    Private Function DelTimeOut() As Boolean
        Dim _strExpr As String
        _strExpr = "ActiveTime < '" & DateTime.Now.AddMinutes(0 - _activeTimeout) & "'or RefreshTime < '" & DateTime.Now.AddMinutes(0 - _refreshTimeout) & "'"
        Dim _curuser As DataRow()
        _curuser = _activeusers.[Select](_strExpr)
        If _curuser.Length > 0 Then
            For i As Integer = 0 To _curuser.Length - 1
                _curuser(i).Delete()
            Next
        End If
        _activeusers.AcceptChanges()
        Return True
    End Function

    ''' <summary>
    ''' 更新用户上次活动时间.
    ''' </summary>
    Public Sub ActiveTime(ByVal strTicket As String)
        DelTimeOut()
        Dim _strExpr As String
        _strExpr = "Ticket='" & strTicket & "'"
        Dim _curuser As DataRow()
        _curuser = _activeusers.[Select](_strExpr)
        If _curuser.Length > 0 Then
            For i As Integer = 0 To _curuser.Length - 1
                _curuser(i)("ActiveTime") = DateTime.Now
                _curuser(i)("RefreshTime") = DateTime.Now
            Next
        End If
        _activeusers.AcceptChanges()
    End Sub

    ''' <summary>
    ''' 当页面更新或获得请求时刷新时间.
    ''' </summary>
    Public Sub RefreshTime(ByVal strTicket As String)
        DelTimeOut()
        Dim _strExpr As String
        _strExpr = "Ticket='" & strTicket & "'"
        Dim _curuser As DataRow()
        _curuser = _activeusers.[Select](_strExpr)
        If _curuser.Length > 0 Then
            For i As Integer = 0 To _curuser.Length - 1
                _curuser(i)("RefreshTime") = DateTime.Now
            Next
        End If
        _activeusers.AcceptChanges()
    End Sub

    Private Function SingleUser(ByVal strUserKey As String, ByVal byTicket As Boolean) As UserEntity
        strUserKey = strUserKey.Trim()
        Dim _strExpr As String
        Dim user As New UserEntity()
        _strExpr = If(byTicket, "Ticket='" & strUserKey & "'", "UserName='" & strUserKey & "'")
        Dim _curuser As DataRow()
        _curuser = _activeusers.[Select](_strExpr)
        If _curuser.Length > 0 Then
            user.Ticket = DirectCast(_curuser(0)("Ticket"), String)
            user.UserName = DirectCast(_curuser(0)("UserName"), String)
            user.TrueName = DirectCast(_curuser(0)("TrueName"), String)
            user.RoleID = DirectCast(_curuser(0)("RoleID"), String)
            user.ActiveTime = CType(_curuser(0)("ActiveTime"), DateTime)
            user.RefreshTime = CType(_curuser(0)("RefreshTime"), DateTime)
            user.ClientIP = DirectCast(_curuser(0)("ClientIP"), String)
        Else
            user.UserName = ""
        End If
        Return user
    End Function

    ''' <summary>
    ''' 依据ticket搜索用户.
    ''' </summary>
    Public Function SingleUser_byTicket(ByVal strTicket As String) As UserEntity
        Return Me.SingleUser(strTicket, True)
    End Function

    ''' <summary>
    ''' 依据username搜索用户.
    ''' </summary>
    Public Function SingleUser_byUserName(ByVal strUserName As String) As UserEntity
        Return Me.SingleUser(strUserName, False)
    End Function

    ''' <summary>
    ''' 通过ticket检查用户是否在线.
    ''' </summary>
    Public Function IsOnline_byTicket(ByVal strTicket As String) As Boolean
        Return CBool(Me.SingleUser(strTicket, True).UserName <> "")
    End Function

    ''' <summary>
    ''' 通过username检查用户是否在线.
    ''' </summary>
    Public Function IsOnline_byUserName(ByVal strUserName As String) As Boolean
        Return CBool(Me.SingleUser(strUserName, False).UserName <> "")
    End Function
End Class

