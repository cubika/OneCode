'****************************** Module Header ******************************\
' 模块名:    Global.asax.vb
' 项目名:        VBASPNETShareSessionBetweenSubDomainsSite1
' Copyright (c) Microsoft Corporation
'
' VBASPNETShareSessionBetweenSubDomains示例演示了如何配置
' 一个SessionState服务器,然后创建一个SharedSessionModule模块实现
' 子域之间ASP.NET Web应用程序的交流.
'
' Global.asax 中的代码只是当你没有为SQL server添加会话状态(Session State)支持时给出相应的
' 提示。
'
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
'
'*****************************************************************************/

Public Class Global_asax
    Inherits System.Web.HttpApplication
    Sub Application_Error(ByVal sender As Object, ByVal e As EventArgs)
        Response.Clear()
        Response.Write("在你运行该示例之前，请运行下面的命令：<br />")
        Response.Write("""<b>C:\Windows\Microsoft.NET\Framework\v4.0.30319\aspnet_regsql.exe -S localhost\sqlexpress -E -ssadd</b>""<br />")
        Response.Write("目的是为了给SQL Server 添加对会话状态的支持.<br /><br />")
        Response.Write("想要取消对会话状态的支持，请参考ReadMe文件.")
        Response.End()
    End Sub
End Class