'************************************ 模块头 *************************************
'* 模块名:	CreateTreeViewFromDataTable.vb
'* 项目名:		VBWinFormTreeViewLoad
'* 版权(c) Microsoft Corporation.
'* 
'* 该示例展示了如何从一个数据表(DataTable)载入数据，
'* 并将数据显示到一个树形视图控件(TreeView)中。
'* 
'* 更多关于 TreeView 控件的信息，请参考:
'* 
'* This source is subject to the Microsoft Public License.
'* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
'* All other rights reserved.
'* 
'* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
'* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
'* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**********************************************************************************


Imports System
Imports System.Collections.Generic
Imports System.Windows.Forms
Imports System.Data

Class CreateTreeViewFromDataTable
    ' 这个 Dictionary 对象用来标识每一个 List<TreeNode> 对象
    ' 每一个 List<TreeNode> 用来存储同一个父节点下的所有的树节点
    Private Shared dic As Dictionary(Of Integer, List(Of TreeNode))

    Public Shared Sub BuildTree(ByVal dt As DataTable, ByVal treeView As TreeView, ByVal expandAll As [Boolean], ByVal displayName As String, ByVal nodeId As String, ByVal parentId As String)
        ' 清除当前 TreeView 所有已存在的数据
        treeView.Nodes.Clear()

        dic = New Dictionary(Of Integer, List(Of TreeNode))()

        Dim node As TreeNode = Nothing

        For Each row As DataRow In dt.Rows

            ' 重新存储每一个节点数据
            node = New TreeNode(row(displayName).ToString())
            node.Tag = row(nodeId)

            ' 对于 TreeView 的根节点，DataTable 中对应的 parentId 属性值为 ""
            ' 所以当 parentId 为 "" 时，他就是根节点
            ' 否则的话，他就是一般的树节点
            If row(parentId).ToString() <> "" Then
                Dim _parentId As Integer = Convert.ToInt32(row(parentId))

                ' 如果在 Dictionary 总存在一个键值为 _parentId 的 List<TreeNode> 对象
                ' 那么我们可以把当前的树节点放入这个 List<TreeNode> 下面，作为他的子节点
                If dic.ContainsKey(_parentId) Then
                    dic(_parentId).Add(node)
                Else
                    ' 否则
                    ' 我们需要新建一个记录在 Dictionary<int, List<TreeNode>>
                    dic.Add(_parentId, New List(Of TreeNode)())

                    ' 然后，将树节点放入这个新的记录
                    dic(_parentId).Add(node)
                End If
            Else
                ' 将节点加入 TreeView 的根节点下面
                treeView.Nodes.Add(node)
            End If
        Next

        ' 在填充满所有的树节点和他的子节点之后
        ' 我们就可以在根节点下面构建整个树形结构
        SearchChildNodes(treeView.Nodes(0))

        If expandAll Then
            ' 展开 TreeView
            treeView.ExpandAll()
        End If
    End Sub
    Private Shared Sub SearchChildNodes(ByVal parentNode As TreeNode)

        If Not dic.ContainsKey(Convert.ToInt32(parentNode.Tag)) Then
            ' 如果在指定的 parentId 下没有一个集合
            ' 那么我们可以直接返回
            Return
        End If

        ' 将指定的集合添加到 TreeView 中并且作为他的 parentId 节点的子节点
        parentNode.Nodes.AddRange(dic(Convert.ToInt32(parentNode.Tag)).ToArray())

        ' 继续遍历查找所有的子节点，是否有节点还有其子节点
        For Each _parentNode As TreeNode In dic(Convert.ToInt32(parentNode.Tag)).ToArray()
            SearchChildNodes(_parentNode)
        Next
    End Sub
End Class
