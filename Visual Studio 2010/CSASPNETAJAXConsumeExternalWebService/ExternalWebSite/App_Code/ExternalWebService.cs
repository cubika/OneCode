/**************************************** 模块头 *****************************************\
* 模块名:    ExternalWebService.cs
* 项目名:    CSASPNETAJAXConsumeExternalWebService
* 版权 (c) Microsoft Corporation
* 
* 在本文件中, 我们模拟了一个不同域的远程web服务. 
* 请确保当我们测试这个示例时web服务在线.
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
public class ExternalWebService : System.Web.Services.WebService {

    public ExternalWebService () {}

    [WebMethod]
    public DateTime GetServerTime() {
        return DateTime.Now;
    }
    
}
