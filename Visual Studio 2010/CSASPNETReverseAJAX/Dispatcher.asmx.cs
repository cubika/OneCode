/****************************** 模块头 ******************************\
* 模块名:    Dispatcher.asmx.cs
* 项目名:    CSASPNETReverseAJAX
* 版权 (c) Microsoft Corporation
*
* 这个网络服务设计用来被Ajax的客户端调用.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
*
\********************************************************************/

using System.Web.Services;

namespace CSASPNETReverseAJAX
{
    /// <summary>
    /// 这个网络服务包含帮助分发事件到客户端的方法.
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
    public class Dispatcher : System.Web.Services.WebService
    {
        /// <summary>
        /// 调度新的消息事件.
        /// </summary>
        /// <param name="userName">登陆的用户名</param>
        /// <returns>消息的内容</returns>
        [WebMethod]
        public string WaitMessage(string userName)
        {
            return ClientAdapter.Instance.GetMessage(userName);
        }
    }
}
