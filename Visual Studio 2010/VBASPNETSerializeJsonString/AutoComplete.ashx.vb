'/****************************** 模块头 ******************************\
'* 模块名:   AutoComplete.ashx.vb
'* 项目名:   VBASPNETSerializeJsonString
'* 版权 (c) Microsoft Corporation
'*
'* 本项目阐述了如何序列化Json字符串.
'* 我们在客户端使用Jquery并且在服务端操作XML数据.
'* 这里通过一个autocomplete控件的例子展示如何序列化Json数据.
'* 
'* This source is subject to the Microsoft Public License.
'* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
'* All other rights reserved.
'*
'\*****************************************************************************/

Imports System.Web
Imports System.Web.Services
Imports System.Web.Script.Serialization
Imports System.Data


Public Class AutoComplete
    Implements System.Web.IHttpHandler

    Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest

        '  在autocomplete控件中查询字符串'term'. 默认的情况下，将把 
        '  查询'term'字符串的结果发送到后端页面.
        Dim searchText As String = context.Request.QueryString("term")
        Dim books As Collection = New Collection

        Dim ds As New DataSet()
        ds.ReadXml(HttpContext.Current.Server.MapPath("App_Data/books.xml"))
        Dim dv As DataView = ds.Tables(0).DefaultView
        dv.RowFilter = [String].Format("title like '{0}*'", searchText.Replace("'", "''"))

        Dim book As Book
        For Each myDataRow As DataRowView In dv
            book = New Book()
            book.id = myDataRow("id").ToString()
            book.value = myDataRow("title").ToString()
            book.label = myDataRow("title").ToString()
            book.Author = myDataRow("author").ToString()
            book.Genre = myDataRow("genre").ToString()
            book.Price = myDataRow("price").ToString()
            book.Publish_date = myDataRow("publish_date").ToString()
            book.Description = myDataRow("description").ToString()
            books.Add(book)
        Next

        Dim serializer As JavaScriptSerializer = New JavaScriptSerializer
        Dim jsonString As String = serializer.Serialize(books)
        context.Response.Write(jsonString)

    End Sub

    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property


End Class