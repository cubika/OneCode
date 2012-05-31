<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="AddArticle.aspx.vb" Inherits="VBASPNETRssFeeds.AddArticle" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
     <asp:SqlDataSource ID="ArticleDataSource" runat="server" 
            ConnectionString="<%$ ConnectionStrings:ConnStr4Articles %>" 
            DeleteCommand="DELETE FROM [Articles] WHERE [ArticleID] = @ArticleID" 
            InsertCommand="INSERT INTO [Articles] ([Title], [Author], [Link], [Description], [PubDate]) VALUES (@Title, @Author, @Link, @Description, @PubDate)" 
            SelectCommand="SELECT * FROM [Articles]" 
            UpdateCommand="UPDATE [Articles] SET [Title] = @Title, [Author] = @Author, [Link] = @Link, [Description] = @Description, [PubDate] = @PubDate WHERE [ArticleID] = @ArticleID">
            <DeleteParameters>
                <asp:Parameter Name="ArticleID" Type="Int32" />
            </DeleteParameters>
            <UpdateParameters>
                <asp:Parameter Name="Title" Type="String" />
                <asp:Parameter Name="Author" Type="String" />
                <asp:Parameter Name="Link" Type="String" />
                <asp:Parameter Name="Description" Type="String" />
                <asp:Parameter Name="PubDate" Type="DateTime" />
                <asp:Parameter Name="ArticleID" Type="Int32" />
            </UpdateParameters>
            <InsertParameters>
                <asp:Parameter Name="Title" Type="String" />
                <asp:Parameter Name="Author" Type="String" />
                <asp:Parameter Name="Link" Type="String" />
                <asp:Parameter Name="Description" Type="String" />
                <asp:Parameter Name="PubDate" Type="DateTime" />
            </InsertParameters>
        </asp:SqlDataSource>
        <asp:FormView ID="ArticleFormView" runat="server" AllowPaging="True" 
            DataKeyNames="ArticleID" DataSourceID="ArticleDataSource">
            <EditItemTemplate>
                ArticleID:
                <asp:Label ID="ArticleIDLabel1" runat="server" 
                    Text='<%# Eval("ArticleID") %>' />
                <br />
                标题:
                <asp:TextBox ID="TitleTextBox" runat="server" Text='<%# Bind("Title") %>' />
                <br />
                作者:
                <asp:TextBox ID="AuthorTextBox" runat="server" Text='<%# Bind("Author") %>' />
                <br />
                链接:
                <asp:TextBox ID="LinkTextBox" runat="server" Text='<%# Bind("Link") %>' />
                <br />
                介绍:
                <asp:TextBox ID="DescriptionTextBox" runat="server" 
                    Text='<%# Bind("Description") %>' Height="120px" TextMode="MultiLine" 
                    Width="200px" />
                <br />
                发表日期:
                <asp:TextBox ID="PubDateTextBox" runat="server" Text='<%# Bind("PubDate") %>' />
                <br />
                <asp:LinkButton ID="UpdateButton" runat="server" CausesValidation="True" 
                    CommandName="Update" Text="更新" />
                &nbsp;<asp:LinkButton ID="UpdateCancelButton" runat="server" 
                    CausesValidation="False" CommandName="Cancel" Text="取消" />
            </EditItemTemplate>
            <InsertItemTemplate>
                标题:
                <asp:TextBox ID="TitleTextBox" runat="server" Text='<%# Bind("Title") %>' />
                <br />
                作者:
                <asp:TextBox ID="AuthorTextBox" runat="server" Text='<%# Bind("Author") %>' />
                <br />
                链接:
                <asp:TextBox ID="LinkTextBox" runat="server" Text='<%# Bind("Link") %>' />
                <br />
                介绍:
                <asp:TextBox ID="DescriptionTextBox" runat="server" 
                    Text='<%# Bind("Description") %>' Height="120px" TextMode="MultiLine" 
                    Width="200px" />
                <br />
                发表日期:
                <asp:TextBox ID="PubDateTextBox" runat="server" Text='<%# Bind("PubDate") %>' />
                <br />
                <asp:LinkButton ID="InsertButton" runat="server" CausesValidation="True" 
                    CommandName="Insert" Text="插入" />
                &nbsp;<asp:LinkButton ID="InsertCancelButton" runat="server" 
                    CausesValidation="False" CommandName="Cancel" Text="取消" />
            </InsertItemTemplate>
            <ItemTemplate>
                标题:
                <asp:Label ID="TitleLabel" runat="server" Text='<%# Bind("Title") %>' />
                <br />
                作者:
                <asp:Label ID="AuthorLabel" runat="server" Text='<%# Bind("Author") %>' />
                <br />
                链接:
                <asp:Label ID="LinkLabel" runat="server" Text='<%# Bind("Link") %>' />
                <br />
                介绍:
                <asp:Label ID="DescriptionLabel" runat="server" 
                    Text='<%# Bind("Description") %>' />
                <br />
                发表日期:
                <asp:Label ID="PubDateLabel" runat="server" Text='<%# Bind("PubDate") %>' />
                <br />
                <asp:LinkButton ID="EditButton" runat="server" CausesValidation="False" 
                    CommandName="Edit" Text="编辑" />
                &nbsp;<asp:LinkButton ID="DeleteButton" runat="server" CausesValidation="False" 
                    CommandName="Delete" Text="删除" />
                &nbsp;<asp:LinkButton ID="NewButton" runat="server" CausesValidation="False" 
                    CommandName="New" Text="新建" />
            </ItemTemplate>
        </asp:FormView>
        <br />
        <br />
    </div>
    </form>
</body>
</html>
