/****************************** 模块头 ******************************\
* 模块名:    BlockSites.cs
* 项目名:	    CSWebBrowserAutomation
* 版权(c)  Microsoft Corporation.
* 
* 这是BlockSites类包含了不能访问站点列表。静态方法GetBlockSites将BlockList.xml转换为
*一个BlockSites实例
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

namespace CSWebBrowserAutomation
{
    public class BlockSites
    {
        public List<string> Hosts { get; set; }

        private static readonly BlockSites instance = GetBlockSites();

        public static BlockSites Instance
        {
            get
            {
                return instance;
            }
        }
       
        /// <summary>
        /// 把BlockList.xml反序列化成一个BlockSites实例。
        /// </summary>
        static private BlockSites GetBlockSites()
        {
            string path = string.Format(@"{0}\Resources\BlockList.xml",
                Environment.CurrentDirectory);
            return XMLSerialization<BlockSites>.DeserializeFromXMLToObject(path);
        }
    }
}
