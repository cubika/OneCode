'***************************** 模块头 *************************************\
' 模块名:   TestForm.vb
' 项目名:  VBOneNoteRibbonAddIn
' 版权 (c) Microsoft Corporation.
'
' 用于在测试中打开的窗体.
'
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
'
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**************************************************************************/
Imports OneNote = Microsoft.Office.Interop.OneNote
Imports System.Runtime.InteropServices
Imports System.Xml.Linq
Imports System.Linq
<ComVisible(False)> _
Partial Public Class TestForm
    Inherits Form
    Private oneNoteApp As OneNote.Application = Nothing

    Public Sub New(ByVal oneNote As OneNote.Application)
        oneNoteApp = oneNote
        InitializeComponent()
    End Sub
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles btnGetPageTitle.Click

        MessageBox.Show(GetPageTitle())
    End Sub
    ''' <summary>
    ''' 获取页面的标题
    ''' </summary>
    ''' <returns>string</returns>
    Private Function GetPageTitle() As String
        Dim pageXmlOut As String = GetActivePageContent()
        Dim doc = XDocument.Parse(pageXmlOut)
        Dim pageTitle As String = ""
        pageTitle = doc.Descendants().FirstOrDefault().Attribute("ID").NextAttribute.Value

        Return pageTitle
    End Function

    ''' <summary>
    ''' 获取当前页面的内容和输出 xml 字符串.
    ''' </summary>
    ''' <returns>string</returns>
    Private Function GetActivePageContent() As String
        Dim activeObjectID As String = Me.GetActiveObjectID(_ObjectType.Page)
        Dim pageXmlOut As String = ""
        oneNoteApp.GetPageContent(activeObjectID, pageXmlOut)

        Return pageXmlOut
    End Function

    ''' <summary>
    ''' 获取当前页面的 ID.
    ''' </summary>
    ''' <param name="obj">_Object Type</param>
    ''' <returns>当前页面的 ID.</returns>
    Private Function GetActiveObjectID(ByVal obj As _ObjectType) As String
        Dim currentPageId As String = ""
        Dim count As UInteger = oneNoteApp.Windows.Count
        For Each window As OneNote.Window In oneNoteApp.Windows
            If window.Active Then
                Select Case obj
                    Case _ObjectType.Notebook
                        currentPageId = window.CurrentNotebookId
                        Exit Select
                    Case _ObjectType.Section
                        currentPageId = window.CurrentSectionId
                        Exit Select
                    Case _ObjectType.SectionGroup
                        currentPageId = window.CurrentSectionGroupId
                        Exit Select
                End Select

                currentPageId = window.CurrentPageId
            End If
        Next

        Return currentPageId

    End Function

    ''' <summary>
    ''' 嵌套的类型
    ''' </summary>
    Private Enum _ObjectType
        Notebook
        Section
        SectionGroup
        Page
        SelectedPages
        PageObject
    End Enum
End Class