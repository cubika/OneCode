/********************************* 模块头 *********************************\
* 模块名：WorkItemLinkInfoDetails.cs
* 项目名：CSTFSWorkItemLinkInfoDetails
* 版权 (c) Microsoft Corporation.
* 
* WorkItemLinkInfoentry类的详细内容。
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace CSTFSWorkItemLinkInfoDetails
{
    public class WorkItemLinkInfoDetails
    {

        public WorkItemLinkInfo LinkInfo { get; private set; }

        public WorkItem SourceWorkItem { get; private set; }

        public WorkItem TargetWorkItem { get; private set; }

        public WorkItemLinkType LinkType { get; private set; }

        public WorkItemLinkInfoDetails(WorkItemLinkInfo linkInfo, WorkItem sourceWorkItem,
            WorkItem targetWorkItem,WorkItemLinkType linkType)
        {
            this.LinkInfo = linkInfo;
            this.SourceWorkItem = sourceWorkItem;
            this.TargetWorkItem = targetWorkItem;
            this.LinkType = linkType;
        }

        /// <summary>
        /// 显示的链接格式如下：
        /// 源文件：[源文件名称] ==> 链接类型：[链接的类型] ==> 目标文件：[目标文件名称]
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format(
                "Source:{0} ==> LinkType:{1} ==> Target:{2}",
                SourceWorkItem.Title,
                LinkType.ForwardEnd.Name, 
                TargetWorkItem.Title);
        }      
    }
}
