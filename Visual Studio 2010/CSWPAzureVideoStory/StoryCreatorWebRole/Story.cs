/*********************************** 模块头 ***********************************\
* 模块名:  Story.cs
* 项目名:  StoryCreatorWebRole
* 版权 (c) Microsoft Corporation.
* 
* 表达短影的模型类.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StoryCreatorWebRole
{
    public class Story
    {
        public string Name { get; set; }
        public string VideoUri { get; set; }
    }
}