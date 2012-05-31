'****************************** Module Header ******************************\
' 模块名:     NextPage.ascx.vb
' 项目名:     VBASPNETPreventMultipleWindows
' 版权 (c) Microsoft Corporation
'
' 这是一个用于其它页面的用户控件。该页面将获取WinddowName值，用于检测是
' 否允许页面跳转。
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL.
' All other rights reserved.
'*****************************************************************************/

Public Class NextPage
    Inherits System.Web.UI.UserControl
    Dim InvalidPage As String = "InvalidPage"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    '/ <summary>
    '/  This method can get window name from Default.aspx
    '/ </summary>
    '/ <returns></returns>
    Public Function GetWindowName() As String
        If Not Session("WindowName") Is Nothing Then
            Dim WindowName As String = Session("WindowName").ToString()
            Return WindowName
        Else
            Return InvalidPage
        End If
    End Function

End Class