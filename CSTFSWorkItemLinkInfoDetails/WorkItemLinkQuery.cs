/*************************************模块头**************************************\
* 模块名：WorkItemLinkQuery.cs
* 项  目：CSTFSWorkItemLinkInfoDetails
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
\********************************************************************************/

using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace CSTFSWorkItemLinkInfoDetails
{
    public class WorkItemLinkQuery : IDisposable
    {

        // 查询语句格式
        const string queryFormat =
            "select * from WorkItemLinks where [Source].[System.ID] = {0}";

        bool disposed = false;
      
        Dictionary<int, WorkItemLinkType> linkTypes;

        // 存储ID和WorkItemLinkType KeyValuePair的字典。
        public Dictionary<int, WorkItemLinkType> LinkTypes
        {
            get
            {
                // 从WorkItemStore中获取所有的WorkItemLinkType。
                if (linkTypes == null)
                {
                    linkTypes = new Dictionary<int, WorkItemLinkType>();
                    foreach (var type in this.WorkItemStore.WorkItemLinkTypes)
                    {
                        linkTypes.Add(type.ForwardEnd.Id, type);
                    }
                }
                return linkTypes;
            }
        }

        /// <summary>
        /// TFS Team Project Collection.                                        
        /// </summary>
        public TfsTeamProjectCollection ProjectCollection { get; private set; }

        /// <summary>
        /// Team Project Collection的WorkItemStore
        /// </summary>
        public WorkItemStore WorkItemStore { get; private set; }

        /// <summary>
        /// 使用默认的凭证初始化实例。
        /// </summary>
        public WorkItemLinkQuery(Uri collectionUri)
            : this(collectionUri, CredentialCache.DefaultCredentials)
        { }

        /// <summary>
        /// 初始化实例。
        /// </summary>
        public WorkItemLinkQuery(Uri collectionUri, ICredentials credential)
        {
            if (collectionUri == null)
            {
                throw new ArgumentNullException("collectionUrl");
            }

            // 如果凭证失效，将启动一个UICredentialsProvider实例。
            this.ProjectCollection =
                    new TfsTeamProjectCollection(collectionUri, credential, new UICredentialsProvider());
            this.ProjectCollection.EnsureAuthenticated();

            // 获取WorkItemStore服务。
            this.WorkItemStore = this.ProjectCollection.GetService<WorkItemStore>();
        }

        /// <summary>
        /// 从一个work项中获取WorkItemLinkInfoDetails。
        /// </summary>
        public IEnumerable<WorkItemLinkInfoDetails> GetWorkItemLinkInfos(int workitemID)
        {

            // 构造WIQL。
            string queryStr = string.Format(queryFormat, workitemID);

            Query linkQuery = new Query(this.WorkItemStore, queryStr);

            // 获取所有的WorkItemLinkInfo对象。
            WorkItemLinkInfo[] linkinfos = linkQuery.RunLinkQuery();

            // 定义一个WorkItemLinkInfoDetails类型的泛型变量。从WorkItemLinkInfo对象中获取WorkItemLinkInfoDetails。
            List<WorkItemLinkInfoDetails> detailsList = new List<WorkItemLinkInfoDetails>();
            foreach (var linkinfo in linkinfos)
            {
                if (linkinfo.LinkTypeId != 0)
                {
                    WorkItemLinkInfoDetails details = GetDetailsFromWorkItemLinkInfo(linkinfo);
                    Console.WriteLine(details.ToString());
                }
            }
            return detailsList;
        }

        /// <summary>
        /// 从WorkItemLinkInfo对象中获取WorkItemLinkInfoDetails。
        /// </summary>
        public WorkItemLinkInfoDetails GetDetailsFromWorkItemLinkInfo(WorkItemLinkInfo linkInfo)
        {
            if (linkInfo == null)
            {
                throw new ArgumentNullException("linkInfo");
            }

            if (this.LinkTypes.ContainsKey(linkInfo.LinkTypeId))
            {
                WorkItemLinkInfoDetails details = new WorkItemLinkInfoDetails(
                   linkInfo,
                   this.WorkItemStore.GetWorkItem(linkInfo.SourceId),
                   this.WorkItemStore.GetWorkItem(linkInfo.TargetId),
                   this.LinkTypes[linkInfo.LinkTypeId]);
                return details;
            }
            else
            {
                throw new ApplicationException("Cannot find WorkItemLinkType!");
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // 避免被处理多次。
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                if (this.ProjectCollection != null)
                {
                    this.ProjectCollection.Dispose();
                }
                disposed = true;
            }
        }

    }
}
