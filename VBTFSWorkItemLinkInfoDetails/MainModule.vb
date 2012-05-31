'*********************************** 模块头***********************************\    
' 模块名： MainModule.vb
' 项  目： VBTFSWorkItemLinkInfoDetails
' 版权 (c) Microsoft Corporation
' 
' 该应用程序的主入口。运行这个程序，使用以下命令参数：
' 
' VBTFSWorkItemLinkInfoDetails.exe <CollectionUrl> <WorkItemID>
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'****************************************************************************/

Imports System.Net
Imports Microsoft.TeamFoundation.Client
Imports Microsoft.TeamFoundation.WorkItemTracking.Client

Module MainModule
    Sub Main(ByVal args() As String)
        Try
            ' 这里有两个参数
            If args.Length = 2 Then

                ' 从arguments中获取CollectionUrl和WorkItemID。
                Dim collectionUri As New Uri(args(0))
                Dim workitemID As Integer = Integer.Parse(args(1))

                Using query As New WorkItemLinkQuery(collectionUri)

                    ' 从一个work项中获取WorkItemLinkInfoDetails列表。
                    Dim detailsList = query.GetWorkItemLinkInfos(workitemID)

                    For Each details As WorkItemLinkInfoDetails In detailsList
                        Console.WriteLine(details.ToString())
                    Next details
                End Using
            Else
                ' 运行该程序使用以下命令参数格式：
                Console.WriteLine("使用下面的命令参数使用本程序:")

                ' VBTFSWorkItemLinkInfoDetails.exe <CollectionUrl> <WorkItemID>
                Console.WriteLine("VBTFSWorkItemLinkInfoDetails.exe <CollectionUrl> <WorkItemID>")
            End If
        Catch ex As Exception
            Console.WriteLine(ex.Message)
        End Try
    End Sub
End Module
