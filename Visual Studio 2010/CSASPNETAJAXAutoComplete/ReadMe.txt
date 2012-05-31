========================================================================
         ASP.NET 应用程序 : CSASPNETAJAXAutoComplete 项目概述
========================================================================

/////////////////////////////////////////////////////////////////////////////
用法:

  这个项目说明了如何在文本框中输入文字时使用AutoCompleteExtender在文字显示中加上前缀.

/////////////////////////////////////////////////////////////////////////////
代码逻辑:

步骤1. 在Visual Studio 2010中创建一个C# ASP.NET应用程序命名为CSASPNETAJAXAutoComplete.

步骤2. 删除下列由Visual Studio自动生成的默认目录和文件.

Account 文件夹
Script 文件夹
Style 文件夹
About.aspx 文件
Default.aspx 文件
Global.asax 文件
Site.Master 文件

步骤3. 添加一个web表单页面到网站并且命名为Default.aspx.

步骤4. 在开始的页面添加一个ToolkitScriptManager空间. 然后在这个页面添加一个TextBox控件和
一个AutoCompleteExtender控件.你可以在工具箱的Ajax Control Toolkit分类下找到AutoCompleteExtender.

[备注] 当一个ToolkitScriptManager被添加到该页面, 有些的注册信息将会自动的被添加到相同的页面.

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" 
TagPrefix="asp" %>

更多关于如何添加Ajax Control Toolkit的详细信息, 请参考:
http://www.asp.net/ajaxlibrary/act.ashx.

步骤5: 添加一个web服务到网站并也命名为Searcher.asmx. 
然后在Searcher.cs中添加一个WebMethod.

[WebMethod]
public string[] HelloWorld(string prefixText, int count) 
    {
        if (count == 0)
        {
            count = 10;
        }

        if (prefixText.Equals("xyz"))
        {
            return new string[0];
        }

        Random random = new Random();
        List<string> items = new List<string>(count);
        char c1;
        char c2;
        char c3;
        for (int i = 0; i < count; i++)
        {
            c1 = (char)random.Next(65, 90);
            c2 = (char)random.Next(97, 122);
            c3 = (char)random.Next(97, 122);
            items.Add(prefixText + c1 + c2 + c3);
        }

        return items.ToArray();
    }

[备注] 当一个web服务被添加到应用程序的时候，一个相同名称的cs文件
会自动被加载到App_Code文件夹中.更多关于web服务的详细信息
请参考:http://msdn.microsoft.com/zh-cn/library/t745kdsh.aspx.

步骤6: 为AutoCompleteExtender设置一些相应的属性.

<asp:AutoCompleteExtender ID="AutoCompleteExtender1" runat="server" 
TargetControlID="txtSearch" ServicePath="~/Searcher.asmx" 
ServiceMethod="HelloWorld" MinimumPrefixLength="1" CompletionSetCount="10">
</asp:AutoCompleteExtender>

[备注] 更多关于AutoCompleteExtender属性的详细信息, 请参考:
http://www.asp.net/ajaxlibrary/act_AutoComplete.ashx.

步骤7: 现在, 你可以运行页面看到我们之前所做出的成就 :)

/////////////////////////////////////////////////////////////////////////////
参考资料:

AutoComplete 教程:
http://www.asp.net/ajaxlibrary/act_AutoComplete.ashx

AutoComplete 示例:
http://www.asp.net/ajax/ajaxcontroltoolkit/Samples/AutoComplete/AutoComplete.aspx

/////////////////////////////////////////////////////////////////////////////