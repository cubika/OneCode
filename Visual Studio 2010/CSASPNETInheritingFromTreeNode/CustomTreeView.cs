/********************************** 模块头 ***********************************\
* 模块名:        CustomTreeView.cs
* 项目名:        CSASPNETInheritingFromTreeNode
* 版权(c) Microsoft Corporation
*
* 这个文件定义了一个树节点包含Tag属性的CustomTreeView控件.Tag属性可用于存储自定义对象.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
*
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*****************************************************************************/

using System.Web.UI.WebControls;


namespace CSASPNETInheritingFromTreeNode
{
    public class CustomTreeView : TreeView
    {
        /// <summary>
        /// 返回TreeNode类的新实例.CreateNode是一个辅助方法.
        /// </summary>
        protected override TreeNode CreateNode()
        {
            return new CustomTreeNode(this, false);
        }
    }

    public class CustomTreeNode : TreeNode
    {
        /// <summary>
        /// 获取或设置包含在想关树节点中的对象数据.
        /// </summary>
        public object Tag { get; set; }

        public CustomTreeNode() : base()
        {
        }

        public CustomTreeNode(TreeView owner, bool isRoot) : base(owner, isRoot)
        {
        }

        /// <summary>
        /// 从上一个页面请求还原使用SaveViewState方法保存的视图状态信息.
        /// </summary>
        /// <param name="state">
        /// 表示要恢复控件状态的对象. 
        /// </param>
        protected override void LoadViewState(object state)
        {
            object[] arrState = state as object[];

            this.Tag = arrState[0];
            base.LoadViewState(arrState[1]);
        }

        /// <summary>
        /// 保存任何自页面回发到服务器起视图状态已发生变化的服务器控件.
        /// </summary>
        /// <returns>
        /// 返回服务器控件的当前视图状态.
        /// 如果没有关联到控件的视图状态,此方法返回null.
        /// </returns>
        protected override object SaveViewState()
        {
            object[] arrState = new object[2];
            arrState[1] = base.SaveViewState();
            arrState[0] = this.Tag;

            return arrState;
        }
    }
}
