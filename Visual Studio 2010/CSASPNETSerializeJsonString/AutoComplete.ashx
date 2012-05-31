/****************************** 模块头 ******************************\
* 模块名:   AutoComplete.ashx
* 项目名:   CSASPNETSerializeJsonString
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

<%@ WebHandler Language="C#" Class="AutoComplete" %> 
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Web;
using System.Web.Script.Serialization;

public class AutoComplete : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        //  在autocomplete控件中查询字符串'term'. 默认的情况下，将把 
        //  查询'term'字符串的结果发送到后端页面.
        string searchText = context.Request.QueryString["term"];

        Collection<Book> books = new Collection<Book>();

        DataSet ds = new DataSet();
        ds.ReadXml(HttpContext.Current.Server.MapPath("App_Data/books.xml"));
        DataView dv = ds.Tables[0].DefaultView;
        dv.RowFilter = String.Format("title like '{0}*'", searchText.Replace("'", "''"));

        Book book;
        foreach (DataRowView myDataRow in dv)
        {
            book = new Book();
            book.id = myDataRow["id"].ToString();
            book.value = book.label = myDataRow["title"].ToString();
            book.Author = myDataRow["author"].ToString();
            book.Genre = myDataRow["genre"].ToString();
            book.Price = myDataRow["price"].ToString();
            book.Publish_date = myDataRow["publish_date"].ToString();
            book.Description = myDataRow["description"].ToString();
            books.Add(book);
        }

        JavaScriptSerializer serializer = new JavaScriptSerializer();

        string jsonString = serializer.Serialize(books);

        context.Response.Write(jsonString);
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }
}

