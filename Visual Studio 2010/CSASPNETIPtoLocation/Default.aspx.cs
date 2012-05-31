/******************************** 模块头 *************************************\
* 模块名:        Default.aspx.cs
* 项目名:        CSASPNETIPtoLocation
* 版权(c) Microsoft Corporation
*
* 此项目演示了如何通过免费Webservice http://freegeoip.appspot.com/ 根据IP地址获取
* 地理位置. 
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
using System.Runtime.Serialization.Json;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace CSASPNETIPtoLocation
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string ipAddress;

            // 获取客户端IP地址.
            ipAddress = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = Request.ServerVariables["REMOTE_ADDR"];
            }

            lbIPAddress.Text = "您的IP地址是: [" + ipAddress + "].";
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string ipAddress = tbIPAddress.Text;
            string locationJson;
            LocationInfo locationInfo = null;

            // 新WebClient实例.
            using (WebClient wc = new WebClient())
            {
                // 访问 http://reegeoip.appspot.com 下载地理位置json数据
                locationJson = wc.DownloadString("http://freegeoip.appspot.com/json/" + ipAddress);
            }

            // 转换数据字符串为stream.
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();

            using (MemoryStream jsonStream = new MemoryStream(encoding.GetBytes(locationJson)))
            {
                jsonStream.Position = 0;
                try
                {
                    // 反序列化json数据获得LocationInfo对象.
                    DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(LocationInfo));
                    locationInfo = (LocationInfo)ser.ReadObject(jsonStream);
                }
                catch
                {
                    throw;
                }
            }

            if (locationInfo == null)
            {
                Response.Write("<strong>根据IP地址[" + ipAddress + "]无法找到对应位置.</strong> ");
            }
            else
            {
                if (locationInfo.status == true)
                {
                    // 输出.
                    Response.Write("<strong>IP地址:</strong> ");
                    Response.Write(locationInfo.ip + "<br />");

                    Response.Write("<strong>国家:</strong> ");
                    Response.Write(locationInfo.countryname + "<br />");

                    Response.Write("<strong>城市:</strong> ");
                    Response.Write(locationInfo.city + "<br />");
                }
            }

            lbIPAddress.Visible = false;
        }

        [Serializable]
        private class LocationInfo
        {
            public bool status = false;
            public string ip = null;
            public string countrycode = null;
            public string countryname = null;
            public string regioncode = null;
            public string regionname = null;
            public string city = null;
            public string zipcode = null;
            public string latitude = null;
            public string longitude = null;
        }
    }
}