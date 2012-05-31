/**************************************** 模块头 *****************************************\
* 模块名:      ImagePreviewControl.cs
* 项目名:        CSASPNETImagePreviewExtender
* 版权 (c) Microsoft Corporation
*
* 本项目演示了如何设计一个 AJAX 扩展程序控件. 
* 在此示例中, 这是个关于图片的扩展控件.
* 使用这个扩展控件的图片最初被显示为一张缩略图,如果用户单击图片,
* 将弹出以原始尺寸显示的大图.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
*
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\******************************************************************************************/
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;


namespace ImagePreviewExtender
{

    [TargetControlType(typeof(Control))]
    public class ImagePreviewControl : ExtenderControl
    {
        /// <summary>
        /// 定义缩略图模式时图片所使用的css类.
        /// </summary>
        public string ThumbnailCssClass { get; set; }

        /// <summary>
        /// 返回关闭图标的资源.
        /// </summary>
        private string closeImage
        {
            get
            {
                return Page.ClientScript.GetWebResourceUrl(this.GetType(),
                                        "ImagePreviewExtender.Close.png");
            }
        }

        protected override IEnumerable<ScriptDescriptor>
                GetScriptDescriptors(System.Web.UI.Control targetControl)
        {
            ScriptBehaviorDescriptor descriptor =
                new ScriptBehaviorDescriptor(
                    "ImagePreviewExtender.ImagePreviewBehavior",
                    targetControl.ClientID);

            descriptor.AddProperty("ThumbnailCssClass", ThumbnailCssClass);

            descriptor.AddProperty("closeImage", closeImage);
            yield return descriptor;
        }

        // 生成脚本引用
        protected override IEnumerable<ScriptReference>
                GetScriptReferences()
        {
            yield return new ScriptReference(
                "ImagePreviewExtender.ImagePreviewBehavior.js",
                this.GetType().Assembly.FullName);
        }
    }
}