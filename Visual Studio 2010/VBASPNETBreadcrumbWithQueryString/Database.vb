'********************************* 模块头 *********************************\
' 模块名:             Database.vb
' 项目名:        VBASPNETBreadcrumbWithQueryString
' 版权(c) Microsoft Corporation
'
' 这是个非常简单的测试用硬编码数据库.这不是此示例的关键点.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
'
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER 
' EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF 
' MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*****************************************************************************/

''' <summary>
''' 这是个非常简单的测试用硬编码数据库.
''' </summary>
Public NotInheritable Class Database
    Private Sub New()
    End Sub

    Public Shared Property Categories As List(Of String)
    Public Shared Property Items As List(Of KeyValuePair(Of String, String))

    Shared Sub New()
        Categories = New List(Of String)() From { _
         "分类1", _
         "分类2", _
         "分类3" _
        }
        Items = New List(Of KeyValuePair(Of String, String))()
        Items.Add(New KeyValuePair(Of String, String)("分类1", "项目1"))
        Items.Add(New KeyValuePair(Of String, String)("分类1", "项目2"))
        Items.Add(New KeyValuePair(Of String, String)("分类1", "项目3"))
        Items.Add(New KeyValuePair(Of String, String)("分类2", "项目4"))
        Items.Add(New KeyValuePair(Of String, String)("分类2", "项目5"))
        Items.Add(New KeyValuePair(Of String, String)("分类2", "项目6"))
        Items.Add(New KeyValuePair(Of String, String)("分类3", "项目7"))
        Items.Add(New KeyValuePair(Of String, String)("分类3", "项目8"))
        Items.Add(New KeyValuePair(Of String, String)("分类3", "项目9"))
    End Sub

    Public Shared Function GetCategoryByItem(ByVal item As String) As String
        For i As Integer = 0 To Items.Count - 1
            If Items(i).Value = item Then
                Return Items(i).Key
            End If
        Next
        Return String.Empty
    End Function

    Public Shared Function GetItemsByCategory(ByVal category As String) As List(Of String)
        Dim list As New List(Of String)()
        For i As Integer = 0 To Items.Count - 1
            If Items(i).Key = category Then
                list.Add(Items(i).Value)
            End If
        Next
        Return list
    End Function

End Class