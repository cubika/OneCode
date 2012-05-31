                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                           <%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="CSAspNetImageEditUpload._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>个人信息</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <h2>
            人员总览</h2>
        <asp:SqlDataSource ID="SqlDSPersonOverView" runat="server" 
            ConnectionString="<%$ ConnectionStrings:db_PersonsConnectionString %>" 
            SelectCommand="SELECT [Id], [PersonName] FROM [tb_personInfo]">
        </asp:SqlDataSource>
        <asp:GridView ID="gvPersonOverView" runat="server" CellPadding="4" EnableModelValidation="True"
            ForeColor="#333333" GridLines="None" Width="70%" AutoGenerateColumns="False"
            DataKeyNames="Id" DataSourceID="SqlDSPersonOverView" 
            onselectedindexchanged="gvPersonOverView_SelectedIndexChanged" 
            AllowPaging="True" AllowSorting="True">
            <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
            <Columns>
                <asp:BoundField DataField="Id" HeaderText="Id" InsertVisible="False" ReadOnly="True"
                    SortExpression="Id" />
                <asp:BoundField DataField="PersonName" HeaderText="名字" SortExpression="PersonName" />
                <asp:CommandField ShowSelectButton="True" HeaderText="单击查看详情" SelectText="详细..." />
            </Columns>
            <EditRowStyle BackColor="#999999" />
            <EmptyDataTemplate>
                无可用数据, 请依FormView的帮助插入数据...<br />
            </EmptyDataTemplate>
            <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
            <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
            <RowStyle BackColor="#F7F6F3" ForeColor="#333333" HorizontalAlign="Center" VerticalAlign="Middle" />
            <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
        </asp:GridView>
    </div>
    <h2>
        个人详细信息</h2>
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
                        个人名字:
                    </th>
                    <td colspan="2">
                        <%#Eval("PersonName") %>
                    </td>
                </tr>
                <tr>
                    <th>
                        个人图片:
                    </th>
                    <td colspan="2">
                        <img src='ImageHandler/ImageHandler.ashx?id=<%#Eval("Id") %>' width="200" alt=""
                            height="200" />
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <asp:LinkButton ID="lnkEdit" runat="server" CommandName="Edit" Text="编辑" />
                    </td>
                    <td align="center">
                        <asp:LinkButton ID="lnkDelete" runat="server" CommandName="Delete" Text="删除" OnClientClick="return confirm('Are you sure to delete it completely?');" />
                    </td>
                    <td align="center">
                        <asp:LinkButton ID="lnkNew" runat="server" CommandName="New" Text="新建" />
                    </td>
                </tr>
            </table>
        </ItemTemplate>
        <EditItemTemplate>
            <table width="100%">
                <tr>
                    <th>
                        个人名字:
                    </th>
                    <td>
                        <asp:TextBox ID="txtName" runat="server" Text=' <%#Bind("PersonName") %>' MaxLength="20" />
                        <asp:RequiredFieldValidator ID="reqName" runat="server" 
                            ControlToValidate="txtName" ErrorMessage="必须填写名字!">*</asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <th>
                        个人图片:
                    </th>
                    <td>
                        <asp:FileUpload ID="fupEditImage" runat="server" />
                        <asp:CustomValidator ID="cmvImageType" runat="server" 
                            ControlToValidate="fupEditImage" ErrorMessage="文件无效!" 
                            OnServerValidate="CustomValidator1_ServerValidate"></asp:CustomValidator>
                    </td>
                </tr>
                <tr>
                    <td align="center">gengxin
                        <asp:LinkButton ID="lnkUpdate" runat="server" CommandName="Update" Text="更新" />
                    </td>
                    <td align="center">
                        <asp:LinkButton ID="lnkCancel" runat="server" CommandName="Cancel" 
                            Text="取消" CausesValidation="False" />
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
                        个人名字:
                    </th>
                    <td>
                        <asp:TextBox ID="txtName" runat="server" MaxLength="20" Text='<%#Bind("PersonName") %>'></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtName"
                            ErrorMessage="名字无效!">*</asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <th>
                        个人图片:
                    </th>
                    <td>
                        <asp:FileUpload ID="fupInsertImage" runat="server" />
                        <asp:CustomValidator ID="cmvImageType" runat="server" ControlToValidate="fupInsertImage"
                            ErrorMessage="无效文件!" OnServerValidate="CustomValidator1_ServerValidate"></asp:CustomValidator>
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <asp:LinkButton ID="lnkInsert" runat="server" CommandName="Insert" Text="插入" />
                    </td>
                    <td align="center">
                        <asp:LinkButton ID="lnkInsertCancel" runat="server" CommandName="Cancel" 
                            Text="取消" CausesValidation="False" />
                    </td>
                </tr>
            </table>
        </InsertItemTemplate>
        <PagerStyle HorizontalAlign="Center" ForeColor="#8C4510" />
        <RowStyle BackColor="#FFF7E7" ForeColor="#8C4510" />
    </asp:FormView>
    <asp:SqlDataSource ID="SqlDSPersonDetails" runat="server" ConnectionString="<%$ ConnectionStrings:db_PersonsConnectionString %>"
        DeleteCommand="DELETE FROM tb_personInfo WHERE (Id = @Id)" InsertCommand="INSERT INTO tb_personInfo(PersonName, PersonImage, PersonImageType) VALUES (@PersonName, @PersonImage, @PersonImageType)"
        
        SelectCommand="SELECT [Id], [PersonName] FROM [tb_personInfo] where id=@id" 
        
        
        UpdateCommand="UPDATE tb_personInfo SET PersonName = @PersonName, PersonImage = @PersonImage, PersonImageType = @PersonImageType WHERE (Id = @Id)">
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
            <asp:ControlParameter Name="id" Type="Int32" ControlID="gvPersonOverView" PropertyName="SelectedValue" DefaultValue="0" />
        </SelectParameters>
    </asp:SqlDataSource>
    
    </form>
</body>
</html>
