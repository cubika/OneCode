'****************************** 模块头 ******************************\
' 模块名:    Dispatcher.asmx.vb
' 项目名:    VBASPNETReverseAJAX
' 版权 (c) Microsoft Corporation
'
' 这个网络服务设计用来被Ajax的客户端调用.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
'
'********************************************************************/

Imports System.Web.Services

''' <summary>
''' 这个网络服务包含帮助分发事件到客户端的方法.
''' </summary>
<System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")> _
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<System.ComponentModel.ToolboxItem(False)> _
Public Class Dispatcher
    Inherits System.Web.Services.WebService

    ''' <summary>
    ''' 调度新的消息事件.
    ''' </summary>
    ''' <param name="userName">登陆的用户名</param>
    ''' <returns>消息的内容</returns>
    <WebMethod()> _
    Public Function WaitMessage(ByVal userName As String) As String
        Return ClientAdapter.Instance.GetMessage(userName)
    End Function

End Class