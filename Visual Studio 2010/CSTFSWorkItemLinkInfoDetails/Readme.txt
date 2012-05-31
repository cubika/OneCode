===============================================================	
	Windows应用程序：CSTFSWorkItemLinkInfoDetails概述		
===============================================================	

/////////////////////////////////////////////////////////////////////////////
概要：
该例子演示了如何从一个WorkItemLinkInfo对象中去获取详细的链接信息。
详细的信息如下：
源文件：[源文件名称]=>链接类型：[链接的类型]=>目标文件：[目标文件名称]			

/////////////////////////////////////////////////////////////////////////////
前提
Team Explorer 2010.

您可以通过下面的链接下载：
http://www.microsoft.com/downloads/en/details.aspx?FamilyID=fe4f9904-0480-4c9d-a264-02fedd78ab38


////////////////////////////////////////////////////////////////////////////////
示例：

步骤一：在Visual Studio 2010中打开这个项目。

步骤二：build解决方案。

步骤三：在命令行中运行CSManipulateImagesInWordDocument.exe应用程序。
        格式如下：CSTFSWorkItemLinkInfoDetails.exe <CollectionUrl> <WorkItemID>
        如果您的默认凭据无法连接到TFS Team Project Collection，将弹出一个对话框让您输入用户名和密码。	

步骤四：如果指定的work item存在与之相关联的work item link，您将会看到以下的信息。			             
        源文件：[源文件名称]=>链接类型：[链接的类型]=>目标文件：[目标文件名称]		
/////////////////////////////////////////////////////////////////////////////
代码逻辑：							
		
A. 连接到一个TFS Team Project Collection和WorkItem Store服务。
       如果凭证失效，将启动一个UICredentialsProvider实例。
       this.ProjectCollection =
           new TfsTeamProjectCollection(collectionUri, credential, new UICredentialsProvider());
       this.ProjectCollection.EnsureAuthenticated();
       
       获取WorkItemStore服务。
       this.WorkItemStore = this.ProjectCollection.GetService<WorkItemStore>();
       
B. 构造WIQL。
       const string queryFormat =
           "select * from WorkItemLinks where [Source].[System.ID] = {0}";
       string queryStr = string.Format(queryFormat, workitemID);
       
       Query linkQuery = new Query(this.WorkItemStore, queryStr);
       
       获取所有的WorkItemLinkInfo对象。
       WorkItemLinkInfo[] linkinfos = linkQuery.RunLinkQuery();

C. 获取详细的信息。
        public Dictionary<int, WorkItemLinkType> LinkTypes
        {
            get
            {
                从WorkItemStore获取所有WorkItemLink类型
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
   
        总结：
        从WorkItemLinkInfo对象中获取WorkItemLinkInfoDetails
       
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
                throw new ApplicationException("无法找到WorkItemLinkType！");  		
            }
        }

////////////////////////////////////////////////////////////////////////////
参考文献：

TfsTeamProjectCollection Constructor (Uri, ICredentials, ICredentialsProvider)
http://msdn.microsoft.com/en-us/library/ff733681.aspx

WIQL syntax for Link Query
http://blogs.msdn.com/b/team_foundation/archive/2010/07/02/wiql-syntax-for-link-query.aspx

Work Item Tracking Queries Object Model in 2010
http://blogs.msdn.com/b/team_foundation/archive/2010/06/16/work-item-tracking-queries-object-model-in-2010.aspx

////////////////////////////////////////////////////////////////////////////