========================================================================
                   VBASPNETAJAXWebChat 概述
========================================================================

/////////////////////////////////////////////////////////////////////////////
用法:

这个项目演示了如何设计一个简单的AJAX web 聊天应用程序. 
我们在客户端使用jQuery, ASP.NET AJAX并在服务器端使用Linq to SQL.
在这个示例中,我们可以创建一个聊天室并邀请其他人加入这个房间开始聊天.

/////////////////////////////////////////////////////////////////////////////
代码实现. 

直接打开VBASPNETAJAXWebChat.sln,展开WebChat web应用程序节点并按F5开始测试应用程序.

如果你想进行进一步测试, 请遵照下列演示步骤.

步骤 1: 按F5打开default.aspx.

步骤 2: 默认状况下, 我们可以看到列表里有两个聊天室,你可以单击下列按钮,"创建聊天室",
创建你自己的聊天室.在这个按钮之前,可以看到一个文本框,我们可以在加入聊天室之前输入昵称.

步骤 3: 单击任何行末尾的"加入"按钮. 你会看到一个弹出的聊天室窗口.

步骤 4: 打开一个新浏览器安同样的步骤以另一个用户的身份加入同一个聊天室.

步骤 5: 当我们输入一条消息, 我们可以看到两个聊天室窗口都会显示这条消息.

/////////////////////////////////////////////////////////////////////////////
代码逻辑:

步骤 1.  创建一个ASP.NET Web 应用程序.在本示例中它是"WebChat".

步骤 2.  右击App_Data目录, 单击添加 -> 新项目 ->
         SQL Server 数据库. 在本示例中它是"SessionDB.mdf".

步骤 3.  打开数据库文件创建四个表.
         * tblChatRoom: 存储聊天室数据.
         * tblMessagePool: 临时存储聊天室消息数据.
         * tblSession: 存储用户session数据.
         * tblTalker: 存储在聊天室中用户数据.
         这些表的详细列名, 请参考本示例中的SessionDB.mdf.

步骤 4.  创建一个新目录, "Data". 右击目录单击
         添加 -> 新项目 -> Linq to SQL 类.
		 (如果你找不到这个模板, 请单击左侧树视图的Data节点.) 
		 在本示例中它是SessionDB.dbml.

步骤 5.  打开SessionDB.dbml双击SessionDB.mdf, 你会在Server Explorer看到数据库. 
		 展开SessionDB.mdf,展开表文件夹,选中四张表,把它们全拖动到SessionDB.dbml.

步骤 6.  创建一个新目录, "Logic". 我们需要创建些类文件.
         * ChatManager.vb: 我们需要些静态方法通过Linq控制数据库中的数据.
		 * ChatRoom.vb: 这是通过WCF服务发送聊天室数据到客户端的DataContract.
         * Message.vb: 这是通过WCF服务发送消息数据到客户端的DataContract.
		 * RoomTalker.vb: 这是通过WCF服务发送聊天室中聊天者数据到客户端的DataContract.
		关于这些类的详情, 请参照本示例中的四个对应文件.

步骤 7.  创建一个新目录, "Services". 右击目录单击
         添加 -> 新项目 -> 支持 AJAX 的 WCF 服务. 在本示例中它是
		 Transition.svc.
		 添加这两个属性到类以保证session 被允许.
		 [CODE]
		 [ServiceContract(Namespace = "http://VBASPNETAJAXWebChat", SessionMode = SessionMode.Allowed)]
         [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
		 [/CODE]
		 在此文件中, 我们创建些可以用来在客户端执行的WCF服务方法.
		 关于这些类的详情, 请参照本示例中的Transition.svc.

步骤 8.  创建一个新目录, "Scripts". 右击目录单击
         添加 -> 新项目 -> Jscript 文件.我们需要创建些 js文件从客户端调用WCF服务. 
		 这里有些本示例的页面逻辑代码;可以根据用户需求来定义.
		 ASP.NET AJAX 允许我们添加一些服务引用. 
		 因此ScriptManager会相对于脚本自动生成客户端服务. 
		 然后我们只需要如同在服务器端一般调用服务方法. 
		 比如,我们在Transition.svc中调用LeaveChatRoom,可以写如下JavaScript方法:
		 [CODE]
		 vbaspnetajaxwebchat.transition.LeaveChatRoom(RoomID,SuccessCallBack,FailCallBack);
		 [/CODE]
		 * vbaspnetajaxwebchat是本应用程序的命名空间.
		 * transition是服务名.
		 * LeaveChatRoom是方法名.
		 因为这个方法有一个参数, RoomID表示这个参数, 如果我们的方法
		 有两个或者更多参数,只要在SuccessCallBack之前写入他们.
		 * SuccessCallBack是当服务调用成功时被触发的方法.
		 * FailCallBack是当服务调用失败时被触发的方法.
		 关于这些脚本方法的具体信息, 请参照本示例中的文件.(chatbox.js, chatMessage.js, chatRoom.js)

步骤 9.  打开Default.aspx,(如果不存在Default.aspx,就创建一个.)
         创建一个ScriptManager控件并如下添加一个服务引用和脚本引用.
		 [CODE]
		  <asp:ScriptManager ID="ScriptManager1" runat="server">
            <Services>
                <asp:ServiceReference Path="~/Services/Transition.svc" />
            </Services>
            <Scripts>
                <asp:ScriptReference Path="~/Scripts/chatbox.js" />
            </Scripts>
         </asp:ScriptManager>
		 [/CODE]
		 In the Head block, add some js and css references from the CDN.
		 [CODE]
		 <script type="text/javascript" src="http://ajax.microsoft.com/ajax/jQuery/jquery-1.4.2.min.js"></script>
         <script type="text/javascript" src="http://ajax.microsoft.com/ajax/jquery.ui/1.8.5/jquery-ui.min.js"></script>
         <script type="text/javascript" src="scripts/chatRoom.js"></script>
	     <link rel="Stylesheet" type="text/css" href="http://ajax.microsoft.com/ajax/jquery.ui/1.8.5/themes/dark-hive/jquery-ui.css" />
	     [/CODE]
		 我们使用这些引用使这个示例更易于编写同时看上去更好.
		 这里还有些其他的UI修饰,请参照本示例中的Default.aspx.
		 关于CDN的详情, 请参阅本ReadMe文件末尾所附的参考资料所列的链接.

步骤 10. 创建一个新web页面. 在本示例中它是"ChatBox.aspx". 
         在这个页面中, 我们创建一些用来发送和接收消息的UI控件. 
		 欲知详情, 请参照本示例中的ChatBox.aspx.

步骤 11. 一切都已完成, 测试这个应用程序希望你能笑颜绽开.
   



/////////////////////////////////////////////////////////////////////////////
参考资料:

MSDN: 如何：创建支持 AJAX 的 WCF 服务和访问该服务的 ASP.NET 客户端
http://msdn.microsoft.com/zh-cn/library/bb924552.aspx

MSDN: LINQ to SQL: .NET Language-Integrated Query for Relational Data
http://msdn.microsoft.com/zh-cn/library/bb425822.aspx

MSDN: JavaScript 和 .NET 中的 JavaScript Object Notation (JSON) 简介
http://msdn.microsoft.com/zh-cn/library/bb299886.aspx

MSDN: 浏览丰富客户端脚本与 jQuery
http://msdn.microsoft.com/zh-cn/magazine/dd453033.aspx

ASP.NET: Microsoft Ajax Content Delivery Network(Microsoft Ajax CDN)
http://www.asp.net/ajaxlibrary/cdn.ashx

/////////////////////////////////////////////////////////////////////////////