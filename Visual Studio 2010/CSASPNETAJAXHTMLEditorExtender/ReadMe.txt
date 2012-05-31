========================================================================
                 CSASPNETHTMLEditorExtender 概述
========================================================================

/////////////////////////////////////////////////////////////////////////////
用法:

此项目演示了如何在Ajax Control Toolkit 4.1.40412.中向HTMLEditor工具栏添加一个
自定义按钮.

/////////////////////////////////////////////////////////////////////////////
示例演示. 

请遵循下列演示步骤.

步骤 1: 打开CSASPNETHTMLEditorExtender.sln.

步骤 2: [此步骤非常重要!]重新生成解决方案.

步骤 3: 展开TestWebSite并右击Default.aspx
        点击“在浏览器中查看“. 

步骤 4: 在Editor输入写文字.

步骤 5: 选泽部分输入的文字.

步骤 6: 单击顶部工具栏最后的"H1"按钮.

步骤 7: 你会看到所选的文本已格式化为H1的风格.
        如果您单击工具栏从底部第二个按钮,你会看到所选的文本已被标签<H1>包围.

/////////////////////////////////////////////////////////////////////////////
代码逻辑:

步骤 1: 从下列链接下载AjaxControlToolkit:
        http://www.asp.net/ajaxlibrary/act.ashx

步骤 2.  在Visual Studio 2010或Visual Web Developer 2010中创建C# "Class Library" 项目. 
		改名为HTMLEditorExtender.

步骤 3.  添加下列引用:
         AjaxControlToolkit (version 4.1.40412.0)
         System.Web 
         System.Web.Extensions

步骤 4.  创建两个新文件夹, 称为Images和ToolBar_buttons.

步骤 5.  我们需要两个图标按钮,一个用于激活的按钮,一个用于未激活按钮.
         我创建了两个图像,一个叫ed_format_h1_a.gif,另一个是ed_format_h1_n.gif.
         添加两个图像到Image文件夹.

步骤 6.  在VS解决方案浏览器中选择两个图像。
         右击它们，并选择属性。
         你可以找到生成操作，
         生成操作设置为嵌入资源.
		 
步骤 7.  在ToolBar_buttons文件夹中,创建一个js文件,称为H1.pre.js.
         
步骤 8.  在H1.pre.js编写的注册H1按钮客户端功能的JavaScript函数.
         我们可以在示例文件H1.pre.js找到完整函数.
         
步骤 9.  重复步骤 5, 执行生成操作嵌入资源H1.pre.js

步骤 10. 在ToolBar_buttons文件夹中创建一个类文件,称为H1.cs.

步骤 11. 编写为按钮注册服务器端类代码.参照示例文件H1.cs.

步骤 12. 在项目根目录创建一个新类, 称为MyEditor.cs.
         编写类似下列代码生成一个扩展Editor.
         [CODE]
         namespace HTMLEditorExtender   
         {   
             public class MyEditor : Editor   
             {   
                 protected override void FillTopToolbar()   
                 {   
                     base.FillTopToolbar();   
                     TopToolbar.Buttons.Add(new H1());   
                 }   
             }   
         }  
         [/CODE]

步骤 13. 生成项目.

步骤 14. 创建一个新的C# "Web Site", 改变最后的文件夹名为TestWebSite.

步骤 15. 添加HTMLEditorExtender类项目引用.

步骤 16. 创建测试页面. 然后添加下列两个注册声明.
         [CODE]
         <%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
         <%@ Register Assembly="HTMLEditorExtender" Namespace="HTMLEditorExtender" TagPrefix="asp" %>
         [/CODE]

步骤 17. 在页面创建一个ToolScriptManager和一个MyEditor.
         [CODE]
         <asp:ToolkitScriptManager runat="server" ID="ToolkitScriptManager1"></asp:ToolkitScriptManager>
         <asp:MyEditor runat="server" ID="MyEditor1" />
         [/CODE]

步骤 18. 测试Default.aspx.

/////////////////////////////////////////////////////////////////////////////
参考资料:

HTMLEditor Tutorials
http://www.asp.net/ajaxlibrary/act_HTMLEditor.ashx