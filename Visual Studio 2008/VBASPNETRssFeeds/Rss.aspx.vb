'********************************* 模块头 *********************************\
' 模块名:  Rss.aspx.vb
' 项目名:  VBASPNETRssFeeds
' 版权 (c) Microsoft Corporation
'
' 这是这个示例的主页面
' 它展示了如何通过ASP.NET生成一个RSS源.
'
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
'
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************/

Imports System.Xml
Imports System.Data.SqlClient

Partial Public Class Rss
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "application/rss+xml"
        Response.ContentEncoding = Encoding.UTF8

        Dim rsswriter As New XmlTextWriter(Response.OutputStream, Encoding.UTF8)

        '开始
        WriteRssOpening(rsswriter)

        '本体
        Dim ArticlesRssTable As DataTable = GetDateSet()
        WriteRssBody(rsswriter, ArticlesRssTable)

        '结尾
        WriteRssEnding(rsswriter)

        rsswriter.Flush()
        Response.End()
    End Sub

    Private Sub WriteRssOpening(ByVal rsswriter As XmlTextWriter)
        rsswriter.WriteStartElement("rss")
        rsswriter.WriteAttributeString("version", "2.0")
        rsswriter.WriteStartElement("channel")
        rsswriter.WriteElementString("title", "VBASPNETRssFeeds")
        rsswriter.WriteElementString("link", Request.Url.Host)
        rsswriter.WriteElementString("description", "This is a sample telling how to create rss feeds for a website.")
    End Sub

    Private Sub WriteRssBody(ByVal rsswriter As XmlTextWriter, ByVal data As DataTable)
        For Each rssitem As DataRow In data.Rows
            rsswriter.WriteStartElement("item")
            rsswriter.WriteElementString("title", rssitem(1).ToString())
            rsswriter.WriteElementString("author", rssitem(2).ToString())
            rsswriter.WriteElementString("link", rssitem(3).ToString())
            rsswriter.WriteElementString("description", rssitem(4).ToString())
            rsswriter.WriteElementString("pubDate", rssitem(5).ToString())
            rsswriter.WriteEndElement()
        Next
    End Sub

    Private Sub WriteRssEnding(ByVal rsswriter As XmlTextWriter)
        rsswriter.WriteEndElement()
        rsswriter.WriteEndElement()
    End Sub

    Private Function GetDateSet() As DataTable
        Dim ArticlesRssTable As New DataTable()

        Dim strconn As String = ConfigurationManager.ConnectionStrings("ConnStr4Articles").ConnectionString
        Dim conn As New SqlConnection(strconn)
        Dim strsqlquery As String = "SELECT * FROM [Articles]"

        Dim da As New SqlDataAdapter(strsqlquery, conn)
        da.Fill(ArticlesRssTable)

        Return ArticlesRssTable
    End Function

End Class