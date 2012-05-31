/********************************* 模块头 **********************************\
* 模块名:  FederationCallbackHandler.aspx.cs
* 项目名:  AzureBingMaps
* 版权 (c) Microsoft Corporation.
* 
* 回调句柄. 配置ACS和Messenger Connect在用户登入后重定向到此页面.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Threading;
using System.Web;
using System.Xml;
using Microsoft.IdentityModel.Claims;

namespace AzureBingMaps.WebRole
{
    public partial class FederationCallbackHandler : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // 从会话中获得的返回页面，重定向到此页面.
            string returnPage = "HtmlClient.aspx";
            if (Session["ReturnPage"] != null)
            {
                returnPage = (string)Session["ReturnPage"];
            }

            // 解析wl_internalState cookie,
            // 展开Windows Live Messenger Connect Profile API相关信息.
            // 用户没有尝试使用Live ID登入时wl_internalState可以为空.
            if (Response.Cookies["wl_internalState"] != null)
            {
                string accessToken = this.ExtractWindowsLiveInternalState("wl_accessToken");
                string cid = this.ExtractWindowsLiveInternalState("wl_cid");
                string uri = "http://apis.live.net/V4.1/cid-" + cid + "/Profiles";

                // 如果LiveID登入失败wl_internalState可能失效.
                if (!string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(cid))
                {
                    // 生成一个到profile API的请求.
                    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);
                    request.Headers["Authorization"] = accessToken;
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        // 使用WCF Syndication API解析回复.
                        XmlReader xmlReader = XmlReader.Create(response.GetResponseStream());
                        SyndicationFeed feed = SyndicationFeed.Load(xmlReader);
                        var entry = feed.Items.FirstOrDefault();
                        if (entry != null)
                        {
                            var content = entry.Content as XmlSyndicationContent;
                            if (content != null)
                            {
                                // WindowsLiveProfile是关联profile API反应的类.
                                var profile = content.ReadContent<WindowsLiveProfile>();
                                var liveID = profile.Emails.Where(m =>
                                    string.Equals(m.Type, "WindowsLiveID")).FirstOrDefault();

                                // 如果profile API成功,
                                // 我们就可以获得用户的LiveID.
                                // LiveID将代表用户身份.
                                // 我们保存用户身份到会话.
                                if (liveID != null)
                                {
                                    Session["User"] = liveID.Address;
                                }
                            }
                        }
                        xmlReader.Close();
                    }
                }
            }

            // 下列代码处理通过WIF的ACS.
            var principal = Thread.CurrentPrincipal as IClaimsPrincipal;
            if (principal != null && principal.Identities.Count > 0)
            {
                var identity = principal.Identities[0];

                // 查询email声明.
                var query = from c in identity.Claims where c.ClaimType == ClaimTypes.Email select c;
                var emailClaim = query.FirstOrDefault();
                if (emailClaim != null)
                {
                    // 保存用户身份到会话.
                    Session["User"] = emailClaim.Value;
                }
            }
            // 重定向用户到返回页面.
            Response.Redirect(returnPage);
        }

        /// <summary>
        /// 自wl_internalState cookie提取Windows Live Messenger Connect Profile API信息.
        /// cookie包含若干信息
        /// 比如cid和访问标识.
        /// </summary>
        /// <param name="key">需提取的数据.</param>
        /// <returns>数据值.</returns>
        private string ExtractWindowsLiveInternalState(string key)
        {
            string result = Request.Cookies["wl_internalState"].Value;
            try
            {
                result = HttpUtility.UrlDecode(result);
                result = result.Substring(result.IndexOf(key));
                result = result.Substring(key.Length + 3,
                    result.IndexOf(',') - key.Length - 4);
            }
            // 如果LiveID登入失败wl_internalState可能失效.
            // 在此场合, 我们返回null.
            catch
            {
                result = null;
            }
            return result;
        }
    }
}