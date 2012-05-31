'**************************** 模块头 ******************************\
' 模块名: Default.aspx.vb
' 项目名: VBASPNETDragItemInListView
' 版权 (c) Microsoft Corporation
'
' 本项目阐述了如何使用JQuery在ListView中拖放项。 
' 在本页面中，把两个xml数据文件绑定到ListView并使用项模板来显示它们。 
' 在Default.aspx页面中引用JQuery的javascript库来实现这些功能。 
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**************************************************************************/


Imports System.Xml

Public Class _Default
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' 把两个xml数据文件绑定到ListView控件上，实际上你可以把“打开”属性的值设置为0. 
        ' 这样,它将不会显示在ListView控件中。
        Dim xmlDocument As New XmlDocument()
        Using tabListView1 As New DataTable()
            tabListView1.Columns.Add("value", Type.[GetType]("System.String"))
            xmlDocument.Load(AppDomain.CurrentDomain.BaseDirectory + "/XmlFile/ListView1.xml")
            Dim xmlNodeList As XmlNodeList = xmlDocument.SelectNodes("root/data[@open='1']")
            For Each xmlNode As XmlNode In xmlNodeList
                Dim dr As DataRow = tabListView1.NewRow()
                dr("value") = xmlNode.InnerText
                tabListView1.Rows.Add(dr)
            Next
            ListView1.DataSource = tabListView1
            ListView1.DataBind()
        End Using

        Dim xmlDocument2 As New XmlDocument()
        Using tabListView2 As New DataTable()
            tabListView2.Columns.Add("value2", Type.[GetType]("System.String"))
            xmlDocument2.Load(AppDomain.CurrentDomain.BaseDirectory + "/XmlFile/ListView2.xml")
            Dim xmlNodeList2 As XmlNodeList = xmlDocument2.SelectNodes("root/data[@open='1']")
            For Each xmlNode As XmlNode In xmlNodeList2
                Dim dr As DataRow = tabListView2.NewRow()
                dr("value2") = xmlNode.InnerText
                tabListView2.Rows.Add(dr)
            Next
            ListView2.DataSource = tabListView2
            ListView2.DataBind()
        End Using

    End Sub

End Class