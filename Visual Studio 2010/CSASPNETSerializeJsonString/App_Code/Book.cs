/****************************** 模块头 ******************************\
* 模块名:    Book.cs
* 项目名:    CSASPNETSerializeJsonString
* 版权 (c) Microsoft Corporation
*
* 本项目阐述了如何序列化Json字符串.
* 我们在客户端使用Jquery并且在服务端操作XML数据.
* 这里通过一个autocomplete控件的例子展示如何序列化Json数据.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
*
\*****************************************************************************/

using System;
using System.Web;

public class Book
{

    /// <summary>
    /// autocomplete例子需要回发可变的"id", "value"和"label".
    /// 不要改变或者删除可变的"id", "value"和"label".
    /// </summary>  
    public string id { get; set; }
    public string label { get; set; }
    public string value { get; set; }
    
    public string Author { get; set; }
    public string Genre { get; set; }
    public string Price { get; set; }
    public string Publish_date { get; set; }
    public string Description { get; set; }
}