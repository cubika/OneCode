===========================================================================
            CSASPNETCurrentOnlineUserList 项目概述
===========================================================================

用法:

 Membership.GetNumberOfUsersOnline方法可以获得在线用户,然而许多asp.net项目
 并没有使用membership.这个项目展示了如何不使用membership provider显示当前在线
 用户信息.

/////////////////////////////////////////////////////////////////////////////
代码.

步骤1: 从示例中浏览Login.aspx页面
你将看到让用户输入"用户名"和"真名"的两个文本框和一个用来登入的按钮.

步骤2: 在文本框中输入"用户名"和"真名"然后单击登入按钮.
如果用户没有在单击登入按钮前填写所有的文本框,页面会在按钮下报错.

步骤3: 用户登入后,页面会重定向到CurrentOnlineUserList.aspx页.
这里有一个用来显示当前在线用户的信息的gridview控件,在此之下有重定向到登出页面的超链接.

步骤4: 再次浏览Login.aspx页面(最好通过另一台计算机浏览该页),登入另一个用户.
CurrentOnlineUserList.aspx页面中的gridview会显示两条当前在线用户的信息.

步骤5: 如果你在同一台计算机上浏览Login.aspx页面.你可以在一分钟后刷新
CurrentOnlineUserList.aspx页面.剩余当前在线用户数会变成一.gridview只会显示最近登入用户数.

步骤6: 如果你浏览login.aspx页面 并在另一台计算机上登入,
请关闭其中一台上的页面,然后在一分钟后刷新另一台上的CurrentOnlineUserList.aspx页面.
你会看到gridview控件里只有一条记录.

步骤6: 你也可以在登入后尝试其他方式关闭页面.然后一分钟后刷新剩余页面,经验而言,
你也能得到在线用户列表.

/////////////////////////////////////////////////////////////////////////////
代码逻辑:

步骤1: 在Visual Studio 2010中创建一个C# ASP.NET空白Web应用程序.

步骤2: 在Visual Studio 2010中添加一个名为'UserEntity'C#类文件.
你可以在UserEntity.cs文件中看到完整版本.

步骤3: 在Visual Studio 2010中添加一个名为'DataTableForCurrentOnlineUser'C#类文件.
你可以在DataTableForCurrentOnlineUser.cs文件中看到完整版本.
它用来初始化保存当前在线用户的信息的datatable.

步骤4: 在Visual Studio 2010中添加一个名为'CheckUserOnline'C#类文件.
你可以在CheckUserOnline.cs文件中看到完整版本.它用来添加JavaScript代码到页面.
JavaScript函数可以检查用户活动时间同时发送一个请求到CheckUserOnlinePage.aspx页面.
本项目将根据最近活动时间自动删除离线用户记录

步骤5: 添加一个ASP.NET页面Login到Web 应用程序用来让用户登入.

步骤6: 如下.aspx代码添加两个文本框、三个标签控件和一个按钮到页面.

    <html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div style="text-align: center">
        <table style="width: 50%">
            <tr>
                <td>
                    <asp:Label ID="lbUserName" runat="server" Text="UserName:">
                    </asp:Label><asp:TextBox ID="tbUserName" runat="server">
                    </asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lbTrueName" runat="server" Text="TrueName:">
                    </asp:Label><asp:TextBox ID="tbTrueName" runat="server">
                    </asp:TextBox>
                </td>
            </tr>
            <tr>
                <td style="text-align: center">
                    <asp:Button ID="btnLogin" runat="server"
                     Text="Sign in" OnClick="btnLogin_Click" /><br />
                    <asp:Label ID="lbMessage" runat="server"
                     Text="Label" Visible="False" ForeColor="Red">
                     </asp:Label>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
    
步骤7: 打开C#后台代码视图编写login和检查用户输入数据值的函数.
你可以在Login.aspx.cs文件中看到完整版本.

    protected void btnLogin_Click(object sender, EventArgs e)
        {
            string _error;

            // Check the value of user's input data.
            if (check_text(out _error))
            {
                // Initialize the datatable which used to store the
                // information of current online user.
                DataTableForCurrentOnlineUser onLineTable = new DataTableForCurrentOnlineUser();

                // An instance of user's entity.
                UserEntity _user = new UserEntity();
                _user.Ticket = DateTime.Now.ToString("yyyyMMddHHmmss");
                _user.UserName = tbUserName.Text.Trim();
                _user.TrueName = tbTrueName.Text.Trim();
                _user.ClientIP = this.Request.UserHostAddress;
                _user.RoleID = "MingXuGroup";

                // Use session variable to store the ticket.
                this.Session["Ticket"] = _user.Ticket;

                // Log in.
                onLineTable.Login(_user, true);
                Response.Redirect("CurrentOnlineUserList.aspx");
            }
            else
            {
                this.lbMessage.Visible = true;
                this.lbMessage.Text = _error;
            }
        }
        public bool check_text(out string error)
        {
            error = "";
            if (this.tbUserName.Text.Trim() == "")
            {
                error = "Please enter the username";
                return false;
            }
            if (this.tbTrueName.Text.Trim() == "")
            {
                error = "Please enter the truename";
                return false;
            }
            return true;
        }

步骤8: 添加一个ASP.NET页面logout到Web 应用程序用来显示当前在线用户列表.

步骤9: 如下.aspx代码添加一个gridview控件和一个HyperLink到页面 .

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <cc1:CheckUserOnline ID="CheckUserOnline1" runat="server" />
        <table border="1" style="width: 98%; height: 100%">
            <tr>
                <td style="text-align: center">
                    Current Online User List
                </td>
            </tr>
            <tr>
                <td style="text-align: center">
                    <asp:GridView ID="gvUserList" runat="server" Width="98%">
                    </asp:GridView>
                </td>
            </tr>
            <tr>
                <td style="text-align: center">
                    <asp:HyperLink ID="hlk" runat="server" 
                    NavigateUrl="~/LogOut.aspx">sign out</asp:HyperLink>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>


步骤10: 打开C#后台代码视图编写CheckLogin函数.
你可以在CurrentOnlineUserList.aspx.cs文件中看到完整版本.

        public void CheckLogin()
        {
            string _userticket = "";
            if (Session["Ticket"] != null)
            {
                _userticket = Session["Ticket"].ToString();
            }
            if (_userticket != "")
            {
                // Initialize the datatable which used to store the information
                // of current online user.
                DataTableForCurrentOnlineUser _onlinetable = new DataTableForCurrentOnlineUser();

                // Check whether the user is online by using ticket.
                if (_onlinetable.IsOnline_byTicket(this.Session["Ticket"].ToString()))
                {
                    // Update the last active time.
                    _onlinetable.ActiveTime(Session["Ticket"].ToString());

                    // Bind the datatable which used to store the information of 
                    // current online user to gridview control.
                    gvUserList.DataSource = _onlinetable.ActiveUsers;
                    gvUserList.DataBind();
                }
                else
                {
                    // If the current User is not exist in the table,then redirect
                    // the page to LogoOut.
                    Response.Redirect("LogOut.aspx");
                }
            }
            else
            {
                Response.Redirect("Login.aspx");
            }
        }



步骤11: 添加一个ASP.NET页面logout到Web 应用程序用来让用户登出.

步骤12:添加一个如下.aspx代码所示的超连接到页面.
<asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/Login.aspx">
Sign in again</asp:HyperLink>

步骤13: 打开C#后台代码视图编写logout函数.
你可以在Logout.aspx.cs文件中找到完整版本.
 
 protected void Page_Load(object sender, EventArgs e)
        {
            // Initialize the datatable which used to store the information
            // of current online user.
            DataTableForCurrentOnlineUser _onlinetable = new DataTableForCurrentOnlineUser();
            if (this.Session["Ticket"] != null)
            {
                // Log out.
                _onlinetable.Logout(this.Session["Ticket"].ToString());
                this.Session.Clear();
            }
        }

步骤14: 向Web应用程序中增加一个CheckUserOnlinePage ASP.NET页面
作为用来检查用户是否在线的页面.

步骤15: 打开C#后台代码视图编写Check函数.
你可以在CheckUserOnlinePage.aspx.cs文件中找到完整版本.
       
        protected void Check()
        {
            string _myTicket = "";
            if (System.Web.HttpContext.Current.Session[this.SessionName] != null)
            {
                _myTicket = System.Web.HttpContext.Current.Session[this.SessionName].ToString();
            }
            if (_myTicket != "")
            {
                // Initialize the datatable which used to store the information of
                // current online user.
                DataTableForCurrentOnlineUser _onlinetable = new DataTableForCurrentOnlineUser();

                // Update the time when the page refresh or the page get a request.
                _onlinetable.RefreshTime(_myTicket);
                Response.Write("OK：" + DateTime.Now.ToString());
            }
            else
            {
                Response.Write("Sorry：" + DateTime.Now.ToString());
            }
        }


/////////////////////////////////////////////////////////////////////////////
参考资料:

MSDN: 
ASP.NET 会话状态概述
http://msdn.microsoft.com/zh-cn/library/ms178581(VS.100).aspx

MSDN:
DataTable 类
http://msdn.microsoft.com/zh-cn/library/system.data.datatable.aspx

/////////////////////////////////////////////////////////////////////////////