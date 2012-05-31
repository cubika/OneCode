'/**************************************** 模块头 *****************************************\
'* 模块名:    CheckUserOnline.vb
'* 项目名:    VBASPNETCurrentOnlineUserList
'* 版权 (c) Microsoft Corporation
'*
'* 本类用来添加JavaScript代码到页面.JavaScript函数可以检查用户活动时间
'* 同时发送一个请求到CheckUserOnlinePage.aspx页面.
'* 本项目将根据最近活动时间自动删除离线用户记录.
'*
'* This source is subject to the Microsoft Public License.
'* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
'* All other rights reserved.
'*
'* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
'* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
'* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'\*****************************************************************************************/

Imports System.ComponentModel

<Description("CheckUserOnline"), DefaultProperty(""), ToolboxData("<{0}:CheckUserOnline runat=server />")> _
Public Class CheckUserOnline
    Inherits System.Web.UI.WebControls.PlaceHolder
    ''' <summary>
    ''' 刷新时间间隔,默认值为25.
    ''' </summary>
    Public Overridable Property RefreshTime() As Integer
        Get
            Dim _obj1 As Object = Me.ViewState("RefreshTime")
            If _obj1 IsNot Nothing Then
                Return Integer.Parse(DirectCast(_obj1, String).Trim())
            End If
            Return 25
        End Get
        Set(ByVal value As Integer)
            Me.ViewState("RefreshTime") = value
        End Set
    End Property
    Protected Overrides Sub Render(ByVal writer As HtmlTextWriter)
        ' 自web.config通过xmlhttp获得来访地址.
        Dim _refreshUrl As String = DirectCast(ConfigurationManager.AppSettings("refreshUrl"), String)
        Dim _scriptString As String = " <script language=""JavaScript"">"
        _scriptString += writer.NewLine
        _scriptString += "   window.attachEvent(""onload"", " & Me.ClientID
        _scriptString += "_postRefresh);" & writer.NewLine
        _scriptString += "   var " & Me.ClientID & "_xmlhttp=null;"
        _scriptString += writer.NewLine
        _scriptString += "   function " & Me.ClientID & "_postRefresh(){"
        _scriptString += writer.NewLine
        _scriptString += "    var " & Me.ClientID
        _scriptString += "_xmlhttp = new ActiveXObject(""Msxml2.XMLHTTP"");"
        _scriptString += writer.NewLine
        _scriptString += "    " & Me.ClientID
        _scriptString += "_xmlhttp.Open(""POST"", """ & _refreshUrl & """, false);"
        _scriptString += writer.NewLine
        _scriptString += "    " & Me.ClientID & "_xmlhttp.Send();"
        _scriptString += writer.NewLine
        _scriptString += "    var refreshStr= " & Me.ClientID
        _scriptString += "_xmlhttp.responseText;"
        _scriptString += writer.NewLine

        _scriptString += "    try {"
        _scriptString += writer.NewLine
        _scriptString += "     var refreshStr2=refreshStr;"
        _scriptString += writer.NewLine
        _scriptString += "    }"
        _scriptString += writer.NewLine
        _scriptString += "    catch(e) {}"
        _scriptString += writer.NewLine
        _scriptString += "    setTimeout("""
        _scriptString += Me.ClientID & "_postRefresh()"","
        _scriptString += Me.RefreshTime.ToString() & "000);"
        _scriptString += writer.NewLine
        _scriptString += "   }" & writer.NewLine
        _scriptString += "<"
        _scriptString += "/"
        _scriptString += "script>" & writer.NewLine
        writer.Write(writer.NewLine)
        writer.Write(_scriptString)
        writer.Write(writer.NewLine)
        MyBase.Render(writer)
    End Sub
End Class

