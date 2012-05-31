/********************************* 模块头 *********************************\
模块名:      BinaryContentResult.cs
项目名:      CSASPNETMVCFileDownload
版权 (c) Microsoft Corporation.

BinaryContentResult类封装用来(二进制格式)输出文件内容自定义ActionResult类

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CSASPNETMVCFileDownload
{
    public class BinaryContentResult : ActionResult
    {
        public BinaryContentResult()
        {
        }

        // 封装http头属性.
        public string ContentType { get; set; }
        public string FileName { get; set; }
        public byte[] Content { get; set; }

        // 设定http头和文件内容输出的代码.
        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.ClearContent();
            context.HttpContext.Response.ContentType = ContentType;

            context.HttpContext.Response.AddHeader("content-disposition", 
                "attachment; filename=" + FileName);

            context.HttpContext.Response.BinaryWrite(Content);
            context.HttpContext.Response.End();
        }
    }
}
