'/**************************************** 模块头 *****************************************\
'* 模块名:    BridgeWebService.vb
'* 项目名:    VBASPNETAJAXConsumeExternalWebService
'* 版权 (c) Microsoft Corporation
'* 
'* 在此文件中, 我们创建一个本地web服务作为桥梁调用远程web服务.
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
 Public Class BridgeWebService
    Inherits System.Web.Services.WebService

    <WebMethod()> _
    Public Function GetServerTime() As DateTime
        ' 获得外部web服务实例
        Dim ews As ExternalWebService.ExternalWebService = New ExternalWebService.ExternalWebService()
        ' 返回web服务方法结果.
        Return ews.GetServerTime()
    End Function

End Class