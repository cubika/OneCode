/****************************************模块头 ***************************************\
* 模块名:  CreateTreeViewFromDataTable.cs
* 项目名:	 CSWinFormTreeViewLoad
* 版权(c) Microsoft Corporation.
* 
* 该示例展示了如何从一个数据表(DataTable)载入数据，
* 并将数据显示到一个树形视图控件(TreeView)中。
* 
* 更多关于 TreeView 控件的信息，请参考:
* 
*  Windows Forms TreeView 控件
*  http://msdn.microsoft.com/zh-cn/library/ch6etkw4.aspx
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\**************************************************************************************/

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Data;

namespace CSWinFormTreeViewLoad
{
    class CreateTreeViewFromDataTable
    {
        // 这个 Dictionary 对象用来标识每一个 List<TreeNode> 对象
        // 每一个 List<TreeNode> 用来存储同一个父节点下的所有的树节点
        private static Dictionary<int, List<TreeNode>> dic;

        public static void BuildTree(DataTable dt, TreeView treeView, Boolean expandAll,
            string displayName, string nodeId, string parentId)
        {
            // 清除当前 TreeView 所有已存在的数据
            treeView.Nodes.Clear();

            dic = new Dictionary<int, List<TreeNode>>();

            TreeNode node = null;

            foreach (DataRow row in dt.Rows)
            {
                // 重新存储每一个节点数据
                node = new TreeNode(row[displayName].ToString());
                node.Tag = row[nodeId];

                // 对于 TreeView 的根节点，DataTable 中对应的 parentId 属性值为 ""
                // 所以当 parentId 为 "" 时，他就是根节点
                // 否则的话，他就是一般的树节点
                if (row[parentId] != DBNull.Value)
                {
                    int _parentId = Convert.ToInt32(row[parentId]);

                    // 如果在 Dictionary 总存在一个键值为 _parentId 的 List<TreeNode> 对象
                    // 那么我们可以把当前的树节点放入这个 List<TreeNode> 下面，作为他的子节点
                    if (dic.ContainsKey(_parentId))
                    {
                        dic[_parentId].Add(node);
                    }
                    else
                    {
                        // 否则
                        // 我们需要新建一个记录在 Dictionary<int, List<TreeNode>>
                        dic.Add(_parentId, new List<TreeNode>());

                        // 然后，将树节点放入这个新的记录
                        dic[_parentId].Add(node);
                    }
                }
                else
                {
                    // 将节点加入 TreeView 的根节点下面
                    treeView.Nodes.Add(node);
                }
            }

            // 在填充满所有的树节点和他的子节点之后
            // 我们就可以在根节点下面构建整个树形结构
            SearchChildNodes(treeView.Nodes[0]);

            if (expandAll)
            {
                // 展开 TreeView
                treeView.ExpandAll();
            }
        }
        private static void SearchChildNodes(TreeNode parentNode)
        {
            if (!dic.ContainsKey(Convert.ToInt32(parentNode.Tag)))
            {
                // 如果在指定的 parentId 下没有一个集合
                // 那么我们可以直接返回
                return;
            }

            // 将指定的集合添加到 TreeView 中并且作为他的 parentId 节点的子节点
            parentNode.Nodes.AddRange(dic[Convert.ToInt32(parentNode.Tag)].ToArray());

            // 继续遍历查找所有的子节点，是否有节点还有其子节点
            foreach (TreeNode _parentNode in dic[Convert.ToInt32(parentNode.Tag)].ToArray())
            {
                SearchChildNodes(_parentNode);
            }
        }
    }
}
