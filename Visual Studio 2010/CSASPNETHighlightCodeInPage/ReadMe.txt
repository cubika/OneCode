=============================================================================
            CSASPNETHighlightCodeInPage 项目 概述
=============================================================================

/////////////////////////////////////////////////////////////////////////////
用法:

有时候我们输入类似C＃或HTML代码后,需要高亮这些代码以获得一个更好的阅读体验.
这个项目演示了如何在一个页面中高亮代码.

/////////////////////////////////////////////////////////////////////////////
演示示例.

步骤1: 浏览示例中的HighlightCodePage.aspx页面,你会找到一个用来让用户选择语言代码
的DropDownList控件和一个让用户粘贴代码的TextBox控件.

步骤2: 在DropDownList控件选择一个语言类型同时粘贴代码到TextBox控件,然后单击高亮按钮.
如果用户在点击突出显示按钮之前不选择一种语言类型或不粘贴代码到TextBox控件,
页面将在突出显示按钮旁边显示错误消息.


步骤3: 在用户点击高亮按钮后,将在页面的右侧显示高亮的代码. 


/////////////////////////////////////////////////////////////////////////////
代码逻辑:

步骤1: 在Visual Studio 2010中创建一个空白C# ASP.NET Web应用程序.

步骤2: 在Visual Studio 2010新增一个C＃类文件,命名为'CodeManager'.
在这个文件中,我们使用散列表的变量来存储不同语言的代码及相关配套选项.
然后添加正则表达式样式对象到代码匹配的字符串.
你可以在CodeManager.cs文件中找到完整的代码.

步骤3: 新增用来让用户高亮显示代码的ASP.NET页面到Web应用程序.

步骤4: 如下添加一个DropDownList控件,两个标签控件,一个TextBox控件和一个
按钮控件到新建页面.

	<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link href="Styles/HighlightCode.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    <table border="1" style="height: 98%">
        <tr>
            <td style="font-size: 12px" class="style1">
                <strong>请在单击高亮按钮之前粘贴代码到文本框控件并选择语言</strong>
            </td>
            <td style="width: auto; font-size: 12px">
                <strong>结果: </strong>
            </td>
        </tr>
        <tr>
            <td class="style1">
                请选择语言:<asp:DropDownList ID="ddlLanguage" runat="server">
                    <asp:ListItem Value="-1">-请选择-</asp:ListItem>
                    <asp:ListItem Value="cs">C#</asp:ListItem>
                    <asp:ListItem Value="vb">VB.NET</asp:ListItem>
                    <asp:ListItem Value="js">JavaScript</asp:ListItem>
                    <asp:ListItem Value="vbs">VBScript</asp:ListItem>
                    <asp:ListItem Value="sql">Sql</asp:ListItem>
                    <asp:ListItem Value="css">CSS</asp:ListItem>
                    <asp:ListItem Value="html">HTML/XML</asp:ListItem>
                </asp:DropDownList>
                <br />
                请将您的代码粘贴到此处:<br />
                <asp:TextBox ID="tbCode" runat="server" TextMode="MultiLine" Height="528px" Width="710px" ></asp:TextBox>
                <br />
                <asp:Button ID="btnHighLight" runat="server" Text="高亮" OnClick="btnHighLight_Click" /><asp:Label
                    ID="lbError" runat="server" Text="Label" ForeColor="Red"></asp:Label>
            </td>
            <td>
                <div id="DivCode">
                    <asp:Label ID="lbResult" runat="server" Text=""></asp:Label>
                </div>
            </td>
        </tr>
    </table>
    </form>
</body>
</html>

步骤5: 打开页面后台C＃代码编写代码高亮的功能。
您可以在HighlightCodePage.aspx.cs文件找到完整版本.

      protected void btnHighLight_Click(object sender, EventArgs e)
        {
            string _error = string.Empty;

            // 检查用户输入的数据.
            if (CheckControlValue(this.ddlLanguage.SelectedValue, 
                this.tbCode.Text, out _error))
            {
                // 初始化根据匹配选项用来保存不同语言代码
                // 及其相关正则表达式的散列表变量.
                Hashtable _htb = CodeManager.Init();

                // 初始化合适的集合对象.
                RegExp _rg = new RegExp();
                _rg = (RegExp)_htb[this.ddlLanguage.SelectedValue];
                this.lbResult.Visible = true;
                if (this.ddlLanguage.SelectedValue != "html")
                {
                    // 在标签控件中显示高亮的代码.
                    this.lbResult.Text = CodeManager.Encode(
                        CodeManager.HighlightCode(
                        Server.HtmlEncode(this.tbCode.Text)
                        .Replace("&quot;", "\""),
                        this.ddlLanguage.SelectedValue, _rg)
                        );
                }
                else
                {
                    // 在标签控件中显示高亮的代码.
                    this.lbResult.Text = CodeManager.Encode(
                        CodeManager.HighlightHTMLCode(this.tbCode.Text, _htb)
                        );
                }
            }
            else
            {
                this.lbError.Visible = true;
                this.lbError.Text = _error;
            }
        }

步骤6: 创建一个新目录,"Style".我们需要创建一个样式表文件。
添加一个名为"HighlightCode”\"样式表文件到Style目录。
在这个文件中,定义一些用于高亮代码的样式.您可以在HighlightCode.css文件找到完整版本.


/////////////////////////////////////////////////////////////////////////////
参考资料:

MSDN: 
# struct
http://msdn.microsoft.com/zh-cn/library/ah19swz4(VS.71).aspx

MSDN:
# Hashtable 类
http://msdn.microsoft.com/zh-cn/library/system.collections.hashtable.aspx

MSDN:
# ArrayList 类
http://msdn.microsoft.com/zh-cn/library/system.collections.arraylist.aspx

MSDN:
# Regex 类
http://msdn.microsoft.com/zh-cn/library/system.text.regularexpressions.regex.aspx

MSDN:
# String.Replace 方法 
http://msdn.microsoft.com/zh-cn/library/system.string.replace.aspx

MSDN:
# GroupCollection 类
http://msdn.microsoft.com/zh-cn/library/system.text.regularexpressions.groupcollection.aspx

MSDN:
# Match 类
http://msdn.microsoft.com/zh-cn/library/system.text.regularexpressions.match.aspx


/////////////////////////////////////////////////////////////////////////////