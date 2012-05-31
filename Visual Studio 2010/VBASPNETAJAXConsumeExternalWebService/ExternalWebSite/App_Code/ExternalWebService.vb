'/**************************************** 模块头 *****************************************\
'* 模块名:    ExternalWebService.vb
'* 项目名:    VBASPNETAJAXConsumeExternalWebService
'* 版权 (c) Microsoft Corporation
'* 
'* 在本文件中, 我们模拟了一个不同域的远程web服务. 
'* 请确保当我们测试这个示例时web服务在线.
'* 
'* This source is subject to the Microsoft Public License.
'* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
'* All other rights reserved.
'*
'* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER 
'* EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF 
'* MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'\******************************************************************************************/

Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols


<System.Web.Script.Services.ScriptService()> _
<WebService(Namespace:="http://tempuri.org/")> _
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
 Public Class ExternalWebService
    Inherits System.Web.Services.WebService

    <WebMethod()> _
    Public Function GetServerTime() As DateTime
        Return DateTime.Now
    End Function

End Class