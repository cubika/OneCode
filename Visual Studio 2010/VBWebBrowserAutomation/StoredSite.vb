'*************************** 模块头 ******************************'
' 模块名:    StoredSite.vb
' 项目名:	    VBWebBrowserAutomation
' 版权 (c) Microsoft Corporation.
' 
' 这个类代表一个站点存储的HTML元素。一个站点是以XML文件的格式被保存在StoredSites下，并且能被反序列化。
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*************************************************************************'

Imports System.IO
Imports System.Security.Permissions

Public Class StoredSite
    ''' <summary>
    ''' 主站点
    ''' </summary>
    Public Property Host() As String

    ''' <summary>
    '''  在站点中的能完全被自动化连接的超链接
    ''' </summary>
    Public Property Urls() As List(Of String)

    ''' <summary>
    ''' 能自动输入的html元素。
    ''' </summary>
    Public Property InputElements() As List(Of HtmlInputElement)

    Public Sub New()
        InputElements = New List(Of HtmlInputElement)()
    End Sub

    ''' <summary>
    '''把这实例当XML文件保存。
    ''' </summary>
    Public Sub Save()
        Dim folderPath As String = String.Format("{0}\StoredSites\",
                                                 Environment.CurrentDirectory)

        If Not Directory.Exists(folderPath) Then
            Directory.CreateDirectory(folderPath)
        End If

        Dim filepath As String = String.Format("{0}\{1}.xml", folderPath, Me.Host)

        XMLSerialization(Of StoredSite).SerializeFromObjectToXML(Me, filepath)
    End Sub

    ''' <summary>
    ''' 完成网页的自动化
    ''' </summary>
    <PermissionSetAttribute(SecurityAction.LinkDemand, Name:="FullTrust")>
    Public Sub FillWebPage(ByVal document As HtmlDocument, ByVal submit As Boolean)
        ' 网页中的提交按钮
        Dim inputSubmit As HtmlElement = Nothing

        '设置html元素的值并保存。如果这个元素是提交按钮，则指定为输入提交。
        For Each input As HtmlInputElement In Me.InputElements
            Dim element As HtmlElement = document.GetElementById(input.ID)
            If element Is Nothing Then
                Continue For
            End If
            If TypeOf input Is HtmlSubmit Then
                inputSubmit = element
            Else
                input.SetValue(element)
            End If
        Next input

        '自动点击提交按钮。
        If submit AndAlso inputSubmit IsNot Nothing Then
            inputSubmit.InvokeMember("click")
        End If
    End Sub

    ''' <summary>
    ''' 通过宿主名得到一个存储站点
    ''' </summary>
    Public Shared Function GetStoredSite(ByVal host As String) As StoredSite
        Dim storedForm As StoredSite = Nothing

        Dim folderPath As String = String.Format("{0}\StoredSites\",
                                                 Environment.CurrentDirectory)

        If Not Directory.Exists(folderPath) Then
            Directory.CreateDirectory(folderPath)
        End If

        Dim filepath As String = String.Format("{0}\{1}.xml", folderPath, host)
        If File.Exists(filepath) Then
            storedForm = XMLSerialization(Of StoredSite).DeserializeFromXMLToObject(filepath)
        End If
        Return storedForm
    End Function
End Class
