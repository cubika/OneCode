'/********************************** 模块头 ***********************************\
' 模块名:        Default.aspx.vb
' 项目名:        VBASPNETShareSessionBetweenSubDomainsSite2
' 版权(c) Microsoft Corporation
' 
' 此页面是用来设定值到会话和读取会话值测试
' 会话的值是否已被网站1改变.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
'
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'\*****************************************************************************/

Public Class _Default
    Inherits System.Web.UI.Page

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        ' Web站点2显示会话.
        lbMsg.Text = DirectCast(Session("MySession"), String)
    End Sub

    Protected Sub btnSetSession_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSetSession.Click
        ' Web站点2设定会话.
        Session("MySession") = "来自站点2的会话内容."
    End Sub
End Class