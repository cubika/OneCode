/*********************************** 模块头 ***********************************\
* 模块名:  StoryDataContext.cs
* 项目名: StoryDataModel
* 版权 (c) Microsoft Corporation.
* 
* 数据表存储内容类.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System.Linq;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace StoryDataModel
{
    public class StoryDataContext : TableServiceContext
    {
        public StoryDataContext(string baseAddress, StorageCredentials credentials)
            : base(baseAddress, credentials)
        {
        }

        public IQueryable<Story> Stories
        {
            get { return this.CreateQuery<Story>("Stories"); }
        }
    }
}
