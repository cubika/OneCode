/********************************** 模块头 ***********************************\
* 模块名:        MyItem.cs
* 项目名:        CSASPNETInheritingFromTreeNode
* 版权(c) Microsoft Corporation
*
* 这个类定义了可以用来分配给自定义树节点的自定义对象.
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
    /// <summary>
    /// 我们可以在树节点存储自定义对象.
    /// 所有在视图状态中存储的对象必须是可序列化的.
    /// </summary>
    [Serializable]
    public class MyItem
    {
        public string Title { get; set; }
    }
}