'******************************** 模块头 **********************************\
' 模块名:     Default.aspx.vb
' 项目名:     VBASPNETShowSpinnerImage
' 版权(c) Microsoft Corporation
'
' 本页面是用于从XML文件中检索数据,并包含了PopupProgeress用户控件。 
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL.
' All other rights reserved.
'
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'****************************************************************************



Imports System.Xml

Public Class _Default
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Protected Sub btnRefresh_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnRefresh.Click
        ' 在这里我们使用Thread.sleep()代码暂停线程10秒模仿
        ' 一个昂贵、耗时的操作检索数据（如连接网络
        ' 数据库检索海量数据）
        ' 所以在实际的应用中,您可以删除此行。
        System.Threading.Thread.Sleep(10000)

        ' 从XML文件中检索数据作为示例数据。
        Dim xmlDocument As New XmlDocument()
        xmlDocument.Load(AppDomain.CurrentDomain.BaseDirectory + "XMLFile/XMLData.xml")
        Dim tabXML As New DataTable()
        Dim columnName As New DataColumn("Name", Type.[GetType]("System.String"))
        Dim columnAge As New DataColumn("Age", Type.[GetType]("System.Int32"))
        Dim columnCountry As New DataColumn("Country", Type.[GetType]("System.String"))
        Dim columnComment As New DataColumn("Comment", Type.[GetType]("System.String"))
        tabXML.Columns.Add(columnName)
        tabXML.Columns.Add(columnAge)
        tabXML.Columns.Add(columnCountry)
        tabXML.Columns.Add(columnComment)
        Dim nodeList As XmlNodeList = xmlDocument.SelectNodes("Root/Person")
        For Each node As XmlNode In nodeList
            Dim row As DataRow = tabXML.NewRow()
            row("Name") = node.SelectSingleNode("Name").InnerText
            row("Age") = node.SelectSingleNode("Age").InnerText
            row("Country") = node.SelectSingleNode("Country").InnerText
            row("Comment") = node.SelectSingleNode("Comment").InnerText
            tabXML.Rows.Add(row)
        Next
        gvwXMLData.DataSource = tabXML
        gvwXMLData.DataBind()

    End Sub
End Class