/**************************************** 模块头 *****************************************\
* 模块名:  DropPanelContent
* 项目名:  CSASPNETAjaxScriptControl
* 版权 (c) Microsoft Corporation
*
* DropPanel的控件层模板. 
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
*
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER 
* EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF 
* MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\******************************************************************************************/
using System.Web.UI;
using System.Web.UI.WebControls;
namespace CSASPNETAjaxScriptControl
{
    public class DropPanelContent : Panel,INamingContainer
    {
        protected override void RenderContents(HtmlTextWriter output)
        {
                base.RenderContents(output);
        }
    }
}
