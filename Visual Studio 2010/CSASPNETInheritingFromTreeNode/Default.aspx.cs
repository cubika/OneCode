/********************************** 模块头 ***********************************\
* 模块名:        Default.aspx.cs
* 项目名:        CSASPNETInheritingFromTreeNode
* 版权(c) Microsoft Corporation
*
* 此页显示如何到/从CustomTreeView控件关联/获取自定义对象.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
*
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*****************************************************************************/

using System;


namespace CSASPNETInheritingFromTreeNode
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // 在TreeView 控件中显示10个树节点.
                for (int i = 0; i < 10; i++)
                {
                    CustomTreeNode treeNode = new CustomTreeNode();

                    // 关联自定义对象到树节点.
                    MyItem item = new MyItem();
                    item.Title = "对象 " + i.ToString();
                    treeNode.Tag = item;

                    treeNode.Value = i.ToString();
                    treeNode.Text = "节点 " + i.ToString();

                    CustomTreeView1.Nodes.Add(treeNode);
                }
            }
        }
        protected void CustomTreeView1_SelectedNodeChanged(object sender, EventArgs e)
        {
            // 获取保存在树节点钟的对象.
            if (CustomTreeView1.SelectedNode != null)
            {
                CustomTreeNode treeNode = (CustomTreeNode)CustomTreeView1.SelectedNode;
                MyItem item = (MyItem)treeNode.Tag;

                lbMessage.Text = string.Format("选择的对象是: {0}", item.Title);
            }
        }
    }
}