===================================================================      
	Windows应用程序：CSTFSWorkItemLinkInfoDetails概述     			        
===================================================================         

/////////////////////////////////////////////////////////////////////////////	                
概要：
这个例子演示了如何从一个WorkItemLinkInfo对象中去获取详细的链接信息。
详细的信息如下：
源文件：[源文件名称]==>链接类型：[链接的类型]==>目标文件：[目标文件名称]

/////////////////////////////////////////////////////////////////////////////		
前提
Team Explorer 2010.

您可以通过下面的链接下载：
http://www.microsoft.com/downloads/en/details.aspx?FamilyID=fe4f9904-0480-4c9d-a264-02fedd78ab38

////////////////////////////////////////////////////////////////////////////////	
示例：
步骤一：在Visual Studio 2010中打开这个项目。     

步骤二：建立解决方案。

步骤三：在命令行中运行CSManipulateImagesInWordDocument.exe应用程序，格式如下：
              VBTFSWorkItemLinkInfoDetails.exe <CollectionUrl> <WorkItemID>	
              CSTFSWorkItemLinkInfoDetails.exe <CollectionUrl> <WorkItemID>
             如果您的默认凭据无法连接到TFSTeamProjectCollection，
            将弹出一个对话框让你输入用户名和密码。	       

步骤四：如果指定的work item存在与之关联的work item link，您将会看到以下的信息。 
            源文件：[源文件名称]=>链接类型：[链接的类型]=>目标文件：[目标文件名称]

/////////////////////////////////////////////////////////////////////////////		
代码逻辑：

A. 连接到一个TFS Team Project Collection和WorkItemStore服务。
        ' 如果凭证失效，将启动一个UICredentialsProvider实例。
        Me.ProjectCollection = New TfsTeamProjectCollection(
            collectionUri, credential, New UICredentialsProvider())
        Me.ProjectCollection.EnsureAuthenticated()

        ' 获取WorkItemStore服务。
        Me.WorkItemStore = Me.ProjectCollection.GetService(Of WorkItemStore)()
       

B.构造WIQL。
       Private Const _queryFormat As String =
           "select * from WorkItemLinks where [Source].[System.ID] = {0}"
       Dim queryStr As String = String.Format(_queryFormat, workitemID)
       
       Dim linkQuery As New Query(Me.WorkItemStore, queryStr)

       ' 获取所有的WorkItemLinkInfo对象。

       Dim linkinfos() As WorkItemLinkInfo = linkQuery.RunLinkQuery()

C.获取详细的信息。
       Private _linkTypes As Dictionary(Of Integer, WorkItemLinkType)
       
       ' 存储ID和WorkItemLinkType的KeyValuePair的字典
       Public ReadOnly Property LinkTypes() As Dictionary(Of Integer, WorkItemLinkType)
           Get
               ' 从WorkItemStore获取所有的WorkItemLinkType 
               If _linkTypes Is Nothing Then
                   _linkTypes = New Dictionary(Of Integer, WorkItemLinkType)()
                   For Each type In Me.WorkItemStore.WorkItemLinkTypes
                       _linkTypes.Add(type.ForwardEnd.Id, type)
                   Next type
               End If
               Return _linkTypes
           End Get
       End Property

       ''' 总结
       ''' 从WorkItemLinkInfo对象中获取WorkItemLinkInfoDetails。
      			
       Public Function GetDetailsFromWorkItemLinkInfo(ByVal linkInfo As WorkItemLinkInfo) _
           As WorkItemLinkInfoDetails
       
           If Me.LinkTypes.ContainsKey(linkInfo.LinkTypeId) Then
               Dim details As New WorkItemLinkInfoDetails(
                   linkInfo,
                   Me.WorkItemStore.GetWorkItem(linkInfo.SourceId),
                   Me.WorkItemStore.GetWorkItem(linkInfo.TargetId),
                   Me.LinkTypes(linkInfo.LinkTypeId))
               Return details
           Else
               Throw New ApplicationException("无法找到WorkItemLinkType！")         		
           End If
       End Function

/////////////////////////////////////////////////////////////////////////////		
参考文献：

TfsTeamProjectCollection Constructor (Uri, ICredentials, ICredentialsProvider)
http://msdn.microsoft.com/en-us/library/ff733681.aspx

WIQL syntax for Link Query
http://blogs.msdn.com/b/team_foundation/archive/2010/07/02/wiql-syntax-for-link-query.aspx

Work Item Tracking Queries Object Model in 2010
http://blogs.msdn.com/b/team_foundation/archive/2010/06/16/work-item-tracking-queries-object-model-in-2010.aspx

/////////////////////////////////////////////////////////////////////////////