'/****************************** 模块头 ******************************\
'* 模块名:    Book.vb
'* 项目名:    VBASPNETSerializeJsonString
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

Imports System
Imports System.Web


Public Class Book

    'autocomplete例子需要回发可变的"id", "value"和"label".
    '不要改变或者删除可变的"id", "value"和"label". 
    Public Property id() As String
    Public Property label() As String
    Public Property value() As String

    Public Property Author() As String
    Public Property Genre() As String
    Public Property Price() As String
    Public Property Publish_date() As String
    Public Property Description() As String
End Class
