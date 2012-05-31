========================================================================
         ASP.NET 应用程序 VBASPNETImageEditUpload 项目概述
========================================================================

/////////////////////////////////////////////////////////////////////////////
用法:

  这个项目显示了如何向数据库插入,编辑和更新Image类型字段.

/////////////////////////////////////////////////////////////////////////////
代码逻辑:

步骤1. 在Visual Studio 2010/Visual Web Developer 2010中创建
一个VB ASP.NET Web应用程序命名为VBASPNETImagEditUpload.

[备注: 你可以从此下载免费的Web Developer:
 http://www.microsoft.com/express/Web/ ]

步骤2. 删除下列由Visual Studio自动生成的默认目录和文件.

Account folder
Script folder
Style folder
About.aspx file
Default.aspx file
Global.asax file
Site.Master file

步骤3. 添加一个web表单页面到网站命名为Default.aspx.
然后创建名为"App_Data"的目录,接着在其中创建一个文本文件,
复制下列代码并保存, 重命名为"SQLScript.sql".

[备注] 你应该编写一个sql脚本来创建数据库和数据表
同时完成实验, 代码如下:

use master
if exists (select [name] from sysdatabases where [name]
='db_Persons')
drop database db_Persons
create database db_Persons
go

--Open the database and create a table called "tb_personInfo"

use db_Persons
go
create table tb_personInfo
(
	Id int primary key identity(1,1),
	PersonName varchar(20)not null,
	PersonImage image,
	PersonImageType varchar(20)
)

然后打开你的SqlExpress或Sql Server management studio,登入并拖放文本文档到此, 
按F5创建包括数据表的数据库. 你可以自http://www.microsoft.com/express/Database/ 
下载免费的SqlExpress.

步骤4. 创建一个名为"DefaultImage"的目录,用来放置新注册用户默认图片或更新的地方.

步骤5. 创建另一个名为"ImageHandler"的目录, 这将用来读取显示一张保存在指定表
"db_Persons"中的图片所用字节集合.
你的代码将类似于如下:

 Public Class ImageHandler
    Implements System.Web.IHttpHandler

    Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest


        Using cmd As New SqlCommand()

            cmd.Connection = New SqlConnection(
                ConfigurationManager.ConnectionStrings("db_PersonsConnectionString").ConnectionString)
            cmd.Connection.Open()
            cmd.CommandText = "select PersonImage,PersonImageType from tb_personInfo where id=" +
                context.Request.QueryString("id")

            Dim reader As SqlDataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection _
                                                            Or CommandBehavior.SingleRow)
            If (reader.Read) Then
                Dim imgbytes() As Byte = Nothing
                Dim imgtype As String = Nothing

                If (reader.GetValue(0) IsNot DBNull.Value) Then
                    imgbytes = CType(reader.GetValue(0), Byte())
                    imgtype = reader.GetString(1)
                Else
                    imgbytes = File.ReadAllBytes(
                        context.Server.MapPath("~/DefaultImage/DefaultImage.JPG"))
                    imgtype = "image/pjpeg"
                End If
                context.Response.ContentType = imgtype
                context.Response.BinaryWrite(imgbytes)
            End If

            reader.Close()
            context.Response.End()
        End Using
    End Sub

    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property
End Class

步骤6: 拖放一个GridView控件到Default.aspx页面中,然后捡起绑定到SqlDataSource. 
你的代码最终看上去像如下式样:

 <asp:SqlDataSource ID="SqlDSPersonOverView" runat="server" 
            ConnectionString="<%$ ConnectionStrings:db_PersonsConnectionString %>" 
            SelectCommand="SELECT [Id], [PersonName] FROM [tb_personInfo]">
        </asp:SqlDataSource>
        <asp:GridView ID="gvPersonOverView" runat="server" CellPadding="4" 
		EnableModelValidation="True" ForeColor="#333333" GridLines="None" Width="70%" 
		AutoGenerateColumns="False" DataKeyNames="Id" DataSourceID="SqlDSPersonOverView" 
            onselectedindexchanged="gvPersonOverView_SelectedIndexChanged">
            <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
            <Columns>
                <asp:BoundField DataField="Id" HeaderText="Id" InsertVisible="False"
				 ReadOnly="True" SortExpression="Id" />
                <asp:BoundField DataField="PersonName" HeaderText="PersonName" 
				SortExpression="PersonName" />
                <asp:CommandField ShowSelectButton="True" HeaderText="Click to see Details" 
				SelectText="Details..." />
            </Columns>
            <EditRowStyle BackColor="#999999" />
            <EmptyDataTemplate>
                No Data Available, Please Insert data with the help of the FormView...<br />
            </EmptyDataTemplate>
            <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
            <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
            <RowStyle BackColor="#F7F6F3" ForeColor="#333333" HorizontalAlign="Center"
			 VerticalAlign="Middle" />
            <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
        </asp:GridView>

[备注] 你可以:
1) 从"数据"面板拖放一个SqlDataSource,或者简单单击GridView的右箭头,
选择SqlDataSource作为GridView的数据源.
2) 然后根据向导, 绑定数据库"db_Persons"到SqlDataSource.
3) 最后绑定SqlDataSource到GridView.
4) 在web.config中定义你的Sql连接字符串, 即"connectionStrings" 节点
如有必要(如果你不知道如何写这个字符串, 请切换到SqlDataSource
查看属性面板, 复制那里的ConnectionString).

步骤7: 拖放FormView到Default.aspx页面,如下所述绑定到另一个使用同一张表的SqlDataSource, 
定义其如下:

<asp:FormView ID="fvPersonDetails" runat="server" Width="50%"
        DataSourceID="SqlDSPersonDetails" EnableModelValidation="True" DataKeyNames="Id"
        DataMember="DefaultView" OnItemInserting="fvPersonDetails_ItemInserting" 
        onitemupdating="fvPersonDetails_ItemUpdating" BackColor="#DEBA84" 
        BorderColor="#DEBA84" BorderStyle="None" BorderWidth="1px" CellPadding="3" 
        CellSpacing="2" GridLines="Both" 
        onitemupdated="fvPersonDetails_ItemUpdated" 
        onitemdeleted="fvPersonDetails_ItemDeleted" 
        onitemdeleting="fvPersonDetails_ItemDeleting" 
        oniteminserted="fvPersonDetails_ItemInserted" 
        onmodechanging="fvPersonDetails_ModeChanging">
        <ItemTemplate>
            <table width="100%">
                <tr>
                    <th>
                        Person Name:
                    </th>
                    <td colspan="2">
                        <%#Eval("PersonName") %>
                    </td>
                </tr>
                <tr>
                    <th>
                        Person Image:
                    </th>
                    <td colspan="2">
                        <img src='ImageHandler/ImageHandler.ashx?id=<%#Eval("Id") %>' width="200" alt=""
                            height="200" />
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <asp:LinkButton ID="lnkEdit" runat="server" CommandName="Edit" Text="Edit" />
                    </td>
                    <td align="center">
                        <asp:LinkButton ID="lnkDelete" runat="server" CommandName="Delete" Text="Delete" 
						OnClientClick="return confirm('Are you sure to delete it completely?');" />
                    </td>
                    <td align="center">
                        <asp:LinkButton ID="lnkNew" runat="server" CommandName="New" Text="New" />
                    </td>
                </tr>
            </table>
        </ItemTemplate>
        <EditItemTemplate>
            <table width="100%">
                <tr>
                    <th>
                        Person Name:
                    </th>
                    <td>
                        <asp:TextBox ID="txtName" runat="server" Text=' <%#Bind("PersonName") %>' 
						MaxLength="20" />
                        <asp:RequiredFieldValidator ID="reqName" runat="server" 
                            ControlToValidate="txtName" ErrorMessage="Name is required!">
							*
							</asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <th>
                        Person Image:
                    </th>
                    <td>
                        <asp:FileUpload ID="fupEditImage" runat="server" />
                        <asp:CustomValidator ID="cmvImageType" runat="server" 
                            ControlToValidate="fupEditImage" ErrorMessage="File is invalid!" 
                            OnServerValidate="CustomValidator1_ServerValidate">
							</asp:CustomValidator>
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <asp:LinkButton ID="lnkUpdate" runat="server" CommandName="Update" 
						Text="Update" />
                    </td>
                    <td align="center">
                        <asp:LinkButton ID="lnkCancel" runat="server" CommandName="Cancel" 
                            Text="Cancel" CausesValidation="False" />
                    </td>
                </tr>
            </table>
        </EditItemTemplate>
        <EditRowStyle BackColor="#738A9C" Font-Bold="True" ForeColor="White" />
        <FooterStyle BackColor="#F7DFB5" ForeColor="#8C4510" />
        <HeaderStyle BackColor="#A55129" Font-Bold="True" ForeColor="White" />
        <InsertItemTemplate>
            <table width="100%">
                <tr>
                    <th>
                        Person Name:
                    </th>
                    <td>
                        <asp:TextBox ID="txtName" runat="server" MaxLength="20" 
						Text='<%#Bind("PersonName") %>'></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
						ControlToValidate="txtName" ErrorMessage="Name is required!">
						*
						</asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <th>
                        Person Image:
                    </th>
                    <td>
                        <asp:FileUpload ID="fupInsertImage" runat="server" />
                        <asp:CustomValidator ID="cmvImageType" runat="server" 
						ControlToValidate="fupInsertImage" ErrorMessage="File is invalid!" 
						OnServerValidate="CustomValidator1_ServerValidate">
						</asp:CustomValidator>
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <asp:LinkButton ID="lnkInsert" runat="server" CommandName="Insert"
						 Text="Insert" />
                    </td>
                    <td align="center">
                        <asp:LinkButton ID="lnkInsertCancel" runat="server" CommandName="Cancel" 
                            Text="Cancel" CausesValidation="False" />
                    </td>
                </tr>
            </table>
        </InsertItemTemplate>
        <PagerStyle HorizontalAlign="Center" ForeColor="#8C4510" />
        <RowStyle BackColor="#FFF7E7" ForeColor="#8C4510" />
    </asp:FormView>

    <asp:SqlDataSource ID="SqlDSPersonDetails" runat="server" 
	ConnectionString="<%$ ConnectionStrings:db_PersonsConnectionString %>"
        DeleteCommand="DELETE FROM tb_personInfo WHERE (Id = @Id)" 
		InsertCommand="INSERT INTO tb_personInfo(PersonName, PersonImage, PersonImageType) 
		VALUES (@PersonName, @PersonImage, @PersonImageType)"
        SelectCommand="SELECT [Id], [PersonName] FROM [tb_personInfo] where id=@id" 
        UpdateCommand="UPDATE tb_personInfo SET PersonName = @PersonName,
		 PersonImage = @PersonImage, PersonImageType = @PersonImageType WHERE (Id = @Id)">
        <DeleteParameters>
            <asp:Parameter Name="Id" Type="Int32" />
        </DeleteParameters>
        <InsertParameters>
            <asp:Parameter Name="PersonName" Type="String" />
            <asp:Parameter Name="PersonImage" DbType="Binary" ConvertEmptyStringToNull="true" />
            <asp:Parameter Name="PersonImageType" Type="String" ConvertEmptyStringToNull="true" />
        </InsertParameters>
        <UpdateParameters>
            <asp:Parameter Name="PersonName" Type="String" />
            <asp:Parameter Name="PersonImage" DbType="Binary" ConvertEmptyStringToNull="true" />
            <asp:Parameter Name="PersonImageType" Type="String" ConvertEmptyStringToNull="true" />
            <asp:Parameter Name="Id" Type="Int32" />
        </UpdateParameters>
        <SelectParameters>
            <asp:ControlParameter Name="id" Type="Int32" ControlID="gvPersonOverView" 
			PropertyName="SelectedValue" DefaultValue="0" />
        </SelectParameters>
    </asp:SqlDataSource>

步骤8: 切换到设计模式,双击空白处,在Page_Load事件中
请编写如下代码:

 Partial Public Class _Default
    Inherits System.Web.UI.Page
    ''' <summary>
	''' Static types of common images for checking.
    ''' </summary>
    Private Shared imgytpes As New List(Of String)() From { _
     ".BMP", _
     ".GIF", _
     ".JPG", _
     ".PNG" _
    }

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

        If Not IsPostBack Then

            gvPersonOverView.DataBind()

            If gvPersonOverView.Rows.Count > 0 Then
                gvPersonOverView.SelectedIndex = 0
                fvPersonDetails.ChangeMode(FormViewMode.[ReadOnly])
                fvPersonDetails.DefaultMode = FormViewMode.[ReadOnly]
            Else
                fvPersonDetails.ChangeMode(FormViewMode.Insert)
                fvPersonDetails.DefaultMode = FormViewMode.Insert
            End If
        End If
    End Sub

  
步骤9: 现在你可以转到gridview,切换到事件面板,双击名为"SelectIndexChanged"
的事件,然后照此编写:

 Protected Sub gvPersonOverView_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs)
        fvPersonDetails.ChangeMode(FormViewMode.[ReadOnly])
        fvPersonDetails.DefaultMode = FormViewMode.[ReadOnly]
 End Sub


[备注] 你的其他控件的事件可以依照步骤9添加, 关于具体代码和信息, 
请查看Default.aspx.vb.

/////////////////////////////////////////////////////////////////////////////
参考资料:

ASP.NET QuickStart Torturial:
http://quickstarts.asp.net/QuickStartv20/aspnet/doc/ctrlref/data/gridview.aspx

MSDN: Serving Dynamic Content with HTTP Handlers
http://msdn.microsoft.com/zh-cn/library/ms972953.aspx

/////////////////////////////////////////////////////////////////////////////