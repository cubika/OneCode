/**************************************** 模块头 *****************************************\
* 模块名:    BridgeWebService.cs
* 项目名:    CSASPNETAJAXConsumeExternalWebService
* 版权 (c) Microsoft Corporation
* 
* 在此文件中, 我们创建一个本地web服务作为桥梁调用远程web服务.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
*
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER 
* EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF 
* MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\******************************************************************************************/

using System;
using System.Web.Services;

[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[System.Web.Script.Services.ScriptService]
public class BridgeWebService : System.Web.Services.WebService
{

    public BridgeWebService() { }

    [WebMethod]
    public DateTime GetServerTime()
    {
        // 获得外部web服务实例
        ExternalWebService.ExternalWebService ews =
            new ExternalWebService.ExternalWebService();
        // 返回web服务方法结果.
        return ews.GetServerTime();
    }

}
