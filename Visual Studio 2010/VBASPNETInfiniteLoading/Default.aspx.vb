'********************************** 模块头 **********************************\
' 模块名:    Default.aspx.vb
' 项目名:    VBASPNETInfiniteLoading
' 版权 (c) Microsoft Corporation
'
' 本项目阐述了如何使用AJAX技术实现向下滚动来加载新页面的内容.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*****************************************************************************/

Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.Services
Imports System.Data
Imports System.Text

Public Class _Default
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    <System.Web.Services.WebMethod()>
    Public Shared Function Foo() As String
        Dim getPostsText As New StringBuilder()
        Using ds As New DataSet()
            ds.ReadXml(HttpContext.Current.Server.MapPath("App_Data/books.xml"))
            Dim dv As DataView = ds.Tables(0).DefaultView

            For Each myDataRow As DataRowView In dv
                getPostsText.AppendFormat("<p>作者: {0}</br>", myDataRow("author"))
                getPostsText.AppendFormat("种类: {0}</br>", myDataRow("genre"))
                getPostsText.AppendFormat("价格: {0}</br>", myDataRow("price"))
                getPostsText.AppendFormat("出版时间: {0}</br>", myDataRow("publish_date"))
                getPostsText.AppendFormat("简介: {0}</br></p>", myDataRow("description"))
            Next

            getPostsText.AppendFormat("<div style='height:15px;'></div>")
        End Using
        Return getPostsText.ToString()
    End Function


End Class