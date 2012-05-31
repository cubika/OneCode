/********************************** 模块头 ***********************************\
* 模块名:        H1.cs
* 项目名:        CSASPNETHTMLEditorExtender
* 版权(c) Microsoft Corporation
* 
* 这个类实例化工具栏按钮. 我们需要在这个类中嵌入些WebResources.
* 
* Ajax控件工具包包含一套可以供您使用建立高度敏感和交互式AJAX的ASP.NET Web窗体
* 应用的丰富控件.我们可以从此链接开始学习和下载AjaxControlkit:
* http://www.asp.net/ajaxlibrary/act.ashx
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
*
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*****************************************************************************/

using System;
using AjaxControlToolkit;
using AjaxControlToolkit.HTMLEditor.ToolbarButton;
using System.Web.UI;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;


// 以webresource向项目嵌入图片和js文件,
// 然后他们应当被编译到程序集.
#region [ Resources ]
[assembly: WebResource("HTMLEditorExtender.Images.ed_format_h1_n.gif",
                        "image/gif")]
[assembly: WebResource("HTMLEditorExtender.Images.ed_format_h1_a.gif",
                        "image/gif")]
[assembly: WebResource("HTMLEditorExtender.ToolBar_buttons.H1.pre.js",
                        "application/x-javascript")]

#endregion


namespace HTMLEditorExtender.ToolBar_buttons
{
    [ToolboxItem(false)]
    [ParseChildren(true)]
    [PersistChildren(false)]
    [RequiredScript(typeof(CommonToolkitScripts))]
    [ClientScriptResource("Sys.Extended.UI.HTMLEditor.ToolbarButton.H1",
                            "HTMLEditorExtender.ToolBar_buttons.H1.pre.js")]
    [SuppressMessage("Microsoft.Maintainability",
                        "CA1501:AvoidExcessiveInheritance")]
    public class H1 : EditorToggleButton
    {
        protected override void OnPreRender(EventArgs e)
        {
            // 从嵌入资源设定工具栏按钮的默认外观.
            NormalSrc = Page.ClientScript.GetWebResourceUrl(this.GetType(),
                "HTMLEditorExtender.Images.ed_format_h1_n.gif");

            // 设定当你按下按钮时的工具栏按钮外观.
            DownSrc = Page.ClientScript.GetWebResourceUrl(this.GetType(),
                "HTMLEditorExtender.Images.ed_format_h1_a.gif");

            // 从嵌入资源设定工具栏按钮的默认外观特效.
            ActiveSrc = DownSrc;
            base.OnPreRender(e);
        }
    }
}
