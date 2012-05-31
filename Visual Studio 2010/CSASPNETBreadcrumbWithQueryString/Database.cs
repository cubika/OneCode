/********************************* 模块头 *********************************\
* 模块名:             Database.cs
* 项目名:        CSASPNETBreadcrumbWithQueryString
* 版权(c) Microsoft Corporation
*
* 这是个非常简单的测试用硬编码数据库.这不是此示例的关键点.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
*
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER 
* EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF 
* MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*****************************************************************************/

using System.Collections.Generic;

namespace CSASPNETBreadcrumbWithQueryString
{
    /// <summary>
    /// 这是个非常简单的测试用硬编码数据库.
    /// </summary>
    public static class Database
    {
        public static List<string> Categories { get; set; }
        public static List<KeyValuePair<string, string>> Items { get; set; }

        static Database()
        {
            Categories = new List<string>() { "分类1", "分类2", "分类3" };
            Items = new List<KeyValuePair<string, string>>();
            Items.Add(new KeyValuePair<string, string>("分类1", "项目1"));
            Items.Add(new KeyValuePair<string, string>("分类1", "项目2"));
            Items.Add(new KeyValuePair<string, string>("分类1", "项目3"));
            Items.Add(new KeyValuePair<string, string>("分类2", "项目4"));
            Items.Add(new KeyValuePair<string, string>("分类2", "项目5"));
            Items.Add(new KeyValuePair<string, string>("分类2", "项目6"));
            Items.Add(new KeyValuePair<string, string>("分类3", "项目7"));
            Items.Add(new KeyValuePair<string, string>("分类3", "项目8"));
            Items.Add(new KeyValuePair<string, string>("分类3", "项目9"));
        }
        public static string GetCategoryByItem(string item)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i].Value == item)
                {
                    return Items[i].Key;
                }
            }
            return string.Empty;
        }
        public static List<string> GetItemsByCategory(string category)
        {
            List<string> list = new List<string>();
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i].Key == category)
                {
                    list.Add(Items[i].Value);
                }
            }
            return list;
        }
    }
}