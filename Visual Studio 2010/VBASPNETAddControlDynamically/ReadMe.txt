===========================================================================
            VBASPNETAddControlDynamically 项目概述
===========================================================================

用法:

 这个项目演示了如何向ASP.NET页面动态添加控件. 它虚构了客户需要输入多于一个且
 无上限的地址信息的情景. 因此我们使用按钮添加新地址TextBox.当用户输入完所有地址, 
 我们也使用按钮在数据库更新这些信息, 在本示例中运行为显示这些地址.

/////////////////////////////////////////////////////////////////////////////
示例演示方法.

步骤1: 浏览示例的Default.aspx页面你会找到标为"增加一个新地址"
和"保存这些地址"的两个按钮.

步骤2: 单击"增加一个新地址" 按钮在这两个按钮后面添加一个区域. 
包括一个Lable,一个TextBox和一个"检查"按钮.

步骤3: 在TextBox中输入些东西然后再一次单击"增加一个新地址"按钮
在第一个区域后面再添加一个.

步骤4: 输入些字符到第二个TextBox然后试着单击"检查"按钮. 
会弹出一个消息框显示"检查"按钮旁边的TextBox中的值.

步骤5: 单击"保存这些地址"按钮. 
现在,已输入地址的列表会被显示在页面上部.

步骤6: 单击"增加一个新地址"按钮重复这个过程. 
在这个示例里你可以添加任意多的区域.

/////////////////////////////////////////////////////////////////////////////
代码逻辑:

步骤1: 在Visual Studio 2010中创建一个空白的VB ASP.NET Web应用程序.

步骤2: 向其中添加一个ASP.NET页面作为示例页面.

步骤3: 依下列HTML代码向页面添加两个按钮和一个面板.

	<asp:Button ID="btnAddAddress" runat="server" Text="Add a New Address" />
	<asp:Button ID="btnSave" runat="server" Text="Save These Addresses" />
	<br />
	<br />
	<asp:Panel ID="pnlAddressContainer" runat="server">
	</asp:Panel>
	<br />

步骤4: 打开VB后台代码视图编写添加组件的函数.
你可以在Default.aspx.vb文件中找到完整版本.

    Protected Sub AddAdress(ByVal id As String)
        Dim lb As New Label()
        lb.Text = "Address" & id & ": "

        Dim tb As New TextBox()
        tb.ID = "TextBox" & id
		tb.Text = Request.Form(tb.ID)

        Dim btn As New Button()
        btn.Text = "Check"
        btn.ID = "Button" & id

        AddHandler btn.Click, AddressOf ClickEvent

        pnlAddressContainer.Controls.Add(lb)
        pnlAddressContainer.Controls.Add(tb)
        pnlAddressContainer.Controls.Add(btn)
    End Sub

步骤5: 编写"检查"按钮单击事件相关函数. 这个函数的部分代码需要些技巧才能使JavaScript
在ASP.NET页面中生效.

    Protected Sub ClickEvent(ByVal sender As Object, ByVal e As EventArgs)
        Dim btn As Button = TryCast(sender, Button)
        Dim tb As TextBox = TryCast(pnlAddressContainer.FindControl(btn.ID.Replace("Button", "TextBox")), TextBox)
        Dim address As String = If(tb.Text = "", "Empty", tb.Text)

        Dim sb As New System.Text.StringBuilder()
        sb.Append("<script type=""text/javascript"">")
        sb.Append("alert(""Address" & btn.ID.Replace("Button", "") & " is " & address & "."");")
        sb.Append("</script>")

        If Not ClientScript.IsClientScriptBlockRegistered(Me.GetType(), "AlertClick") Then
            ClientScript.RegisterClientScriptBlock(Me.GetType(), "AlertClick", sb.ToString())
        End If
    End Sub

步骤6: 编辑"增加一个新地址" 按钮的单击事件句柄调用上述函数添加地址组件.

/////////////////////////////////////////////////////////////////////////////
参考资料:

ASP.NET Forum: 
# Why do dynamic controls disappear on postback and not raise events?
http://forums.asp.net/t/1186195.aspx

MSDN:
# ASP.NET 页生命周期概述
http://msdn.microsoft.com/zh-cn/library/ms178472.aspx

/////////////////////////////////////////////////////////////////////////////