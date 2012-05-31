/****************************** 模块头 ******************************\
* 模块名:  AutoLogin.aspx.cs
* 项目名:  CSASPNETAutoLogin
* 版权 (c) Microsoft Corporation.
* 
* 这个页面首先请求Login.aspx并获取__VIEWSTATE和__EVENTVALIDATION的字段.
* 然后我们可以设置post数据字符串,像__VIEWSTATE, __EVENTVALIDATION, 用户
* 名,密码和自动登录按钮的id参数.
* 我们使用webrequest用post的方式发布这些数据到login.aspx来登录该网站.
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
using System.Net;
using System.Web;
using System.Text;


namespace CSASPNETAutoLogin
{
    public partial class AutoLogin : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Response.Clear();
        }

        protected void autoLogin_Click(object sender, EventArgs e)
        {
            string url = HttpContext.Current.Request.Url.AbsoluteUri.ToString().Replace("AutoLogin", "Login");
            CookieContainer myCookieContainer = new CookieContainer();
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.CookieContainer = myCookieContainer;
            request.Method = "GET";
            request.KeepAlive = false;

            HttpWebResponse response = request.GetResponse() as HttpWebResponse;

            System.IO.Stream responseStream = response.GetResponseStream();
            System.IO.StreamReader reader = new System.IO.StreamReader(responseStream, Encoding.UTF8);
            string srcString = reader.ReadToEnd();

            // 获取页面的ViewState                
            string viewStateFlag = "id=\"__VIEWSTATE\" value=\"";
            int i = srcString.IndexOf(viewStateFlag) + viewStateFlag.Length;
            int j = srcString.IndexOf("\"", i);
            string viewState = srcString.Substring(i, j - i);

            // 获取页面的EventValidation                
            string eventValidationFlag = "id=\"__EVENTVALIDATION\" value=\"";
            i = srcString.IndexOf(eventValidationFlag) + eventValidationFlag.Length;
            j = srcString.IndexOf("\"", i);
            string eventValidation = srcString.Substring(i, j - i);

            string submitButton = "LoginButton";

            // 用户名和密码
            string userName = btnUserName.Text;
            string password = btnPassword.Text;

            // 把文本转换成url编码字符串
            viewState = System.Web.HttpUtility.UrlEncode(viewState);
            eventValidation = System.Web.HttpUtility.UrlEncode(eventValidation);
            submitButton = System.Web.HttpUtility.UrlEncode(submitButton);

            // 合并多个将会提交的字符串数据
            string formatString =
                     "UserName={0}&Password={1}&loginButton={2}&__VIEWSTATE={3}&__EVENTVALIDATION={4}";
            string postString =
                     string.Format(formatString, userName, password, submitButton, viewState, eventValidation);

            // 把提交的字符串数据转换成字节数组
            byte[] postData = Encoding.ASCII.GetBytes(postString);

            // 设置请求参数
            request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "POST";
            request.Referer = url;
            request.KeepAlive = false;
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; WOW64; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; InfoPath.2; CIBA)";
            request.ContentType = "application/x-www-form-urlencoded";
            request.CookieContainer = myCookieContainer;
            System.Net.Cookie ck = new System.Net.Cookie("TestCookie1", "Value of test cookie");
            ck.Domain = request.RequestUri.Host;
            request.CookieContainer.Add(ck);
            request.CookieContainer.Add(response.Cookies);

            request.ContentLength = postData.Length;

            // 提交请求数据
            System.IO.Stream outputStream = request.GetRequestStream();
            request.AllowAutoRedirect = true;
            outputStream.Write(postData, 0, postData.Length);
            outputStream.Close();

            
            // 获取返回数据
            response = request.GetResponse() as HttpWebResponse;
            responseStream = response.GetResponseStream();
            reader = new System.IO.StreamReader(responseStream, Encoding.UTF8);
            srcString = reader.ReadToEnd();
            Response.Write(srcString);
            Response.End();
        }
    }
}
