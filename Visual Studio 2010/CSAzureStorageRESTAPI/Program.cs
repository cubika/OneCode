/****************************** 模块头 ************************************\
* 模块名:   Program.cs
* 项目名:   CSAzureStorageRESTAPI
* 版权 (c) Microsoft Corporation.
* 
* 这个模块演示了怎样通过调用List Blob REST API来列出Blob存储的一个特定容器
* 的blob
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Collections.Specialized;
using System.Collections;
using System.Web;

namespace CSAzureStorageRESTAPI
{
    class Program
    {
        const string _bloburi = @"http://127.0.0.1:10000/devstoreaccount1";
        const string _accountname = "devstoreaccount1";
        const string _key = "Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==";
        const string _method = "GET";
       
        static void Main(string[] args)
        {
            string AccountName = _accountname;
            string AccountSharedKey = _key;
            string Address = _bloburi;
            string MessageSignature = "";
            Console.WriteLine("请输入容器名并按<ENTER>键. 它的Blob信息将被列出来:");
            // 获得容器名
            string container = Console.ReadLine();
            // 设置请求URI
            string QueryString = "?restype=container&comp=list";
            Uri requesturi = new Uri(Address + "/" + container + QueryString);
            
            // 新建HttpWebRequest对象
            HttpWebRequest Request = (HttpWebRequest)HttpWebRequest.Create(requesturi.AbsoluteUri);
            Request.Method = _method;
            Request.ContentLength = 0;
            // 添加HTTP数据头
            Request.Headers.Add("x-ms-date", DateTime.UtcNow.ToString("R"));
            Request.Headers.Add("x-ms-version", "2009-09-19");

            // 新建签名
            // 动词
            MessageSignature += "GET\n";
            // Content-Encoding内容编码
            MessageSignature += "\n";
            // Content-Language内容的语言
            MessageSignature += "\n";
            // Content-Length内容长度 
            MessageSignature += "\n";
            // Content-MD5
            MessageSignature += "\n";
            // Content-Type内容类型
            MessageSignature += "\n";
            // 日期
            MessageSignature += "\n";
            // If-Modified-Since从何时修改
            MessageSignature += "\n";
            // If-Match如果匹配
            MessageSignature += "\n";
            // If-None-Match如果不匹配
            MessageSignature += "\n";
            // If-Unmodified-Since
            MessageSignature += "\n";
            // 范围
            MessageSignature += "\n";
            // 规范化头
            MessageSignature += GetCanonicalizedHeaders(Request);
            // 规范化资源
            MessageSignature += GetCanonicalizedResourceVersion2(requesturi, AccountName);
            // 使用HMAC-SHA256标记签名
            byte[] SignatureBytes = System.Text.Encoding.UTF8.GetBytes(MessageSignature);
            System.Security.Cryptography.HMACSHA256 SHA256 = new System.Security.Cryptography.HMACSHA256(Convert.FromBase64String(AccountSharedKey));
            // 新建授权HTTP数据头的值
            String AuthorizationHeader = "SharedKey " + AccountName + ":" + Convert.ToBase64String(SHA256.ComputeHash(SignatureBytes));
            // 添加授权HTTP数据头
            Request.Headers.Add("Authorization", AuthorizationHeader);

			try
			{
                // 发送HTTP请求并获得响应
				using (HttpWebResponse response = (HttpWebResponse)Request.GetResponse())
				{
					if (response.StatusCode == HttpStatusCode.OK)
					{
						// 如果成功
						using (Stream stream = response.GetResponseStream())
						{
							using (StreamReader sr = new StreamReader(stream))
							{

								var s = sr.ReadToEnd();
								// 输出响应
								Console.WriteLine(s);
							}
						}
					}
				}
			}
			catch (WebException ex)
			{
                Console.WriteLine("出现一个错误.状态码:" + ((HttpWebResponse)ex.Response).StatusCode);
				Console.WriteLine("出错信息:");
				using (Stream stream = ex.Response.GetResponseStream())
				{
					using (StreamReader sr = new StreamReader(stream))
					{
						var s = sr.ReadToEnd();
						Console.WriteLine(s);
					}
				}
			}

            Console.ReadLine();

        }

#region Helper method/class

        static  ArrayList GetHeaderValues(NameValueCollection headers, string headerName)
        {
            ArrayList list = new ArrayList();
            string[] values = headers.GetValues(headerName);
            if (values != null)
            {
                foreach (string str in values)
                {
                    list.Add(str.TrimStart(new char[0]));
                }
            }
            return list;
        }

        static string GetCanonicalizedHeaders(HttpWebRequest request)
        {
            ArrayList list = new ArrayList();
            StringBuilder sb = new StringBuilder();
            foreach (string str in request.Headers.Keys)
            {
                if (str.ToLowerInvariant().StartsWith("x-ms-", StringComparison.Ordinal))
                {
                    list.Add(str.ToLowerInvariant());
                }
            }
            list.Sort();
            foreach (string str2 in list)
            {
                StringBuilder builder = new StringBuilder(str2);
                string str3 = ":";
                foreach (string str4 in GetHeaderValues(request.Headers, str2))
                {
                    string str5 = str4.Replace("\r\n", string.Empty);
                    builder.Append(str3);
                    builder.Append(str5);
                    str3 = ",";
                }
                sb.Append(builder.ToString());
                sb.Append("\n");
            }
            return sb.ToString();
        }

        static string GetCanonicalizedResourceVersion2(Uri address, string accountName)
        {
            StringBuilder builder = new StringBuilder("/");
            builder.Append(accountName);
            builder.Append(address.AbsolutePath);
            CanonicalizedString str = new CanonicalizedString(builder.ToString());
            NameValueCollection values = HttpUtility.ParseQueryString(address.Query);
            NameValueCollection values2 = new NameValueCollection();
            foreach (string str2 in values.Keys)
            {
                ArrayList list = new ArrayList(values.GetValues(str2));
                list.Sort();
                StringBuilder builder2 = new StringBuilder();
                foreach (object obj2 in list)
                {
                    if (builder2.Length > 0)
                    {
                        builder2.Append(",");
                    }
                    builder2.Append(obj2.ToString());
                }
                values2.Add((str2 == null) ? str2 : str2.ToLowerInvariant(), builder2.ToString());
            }
            ArrayList list2 = new ArrayList(values2.AllKeys);
            list2.Sort();
            foreach (string str3 in list2)
            {
                StringBuilder builder3 = new StringBuilder(string.Empty);
                builder3.Append(str3);
                builder3.Append(":");
                builder3.Append(values2[str3]);
                str.AppendCanonicalizedElement(builder3.ToString());
            }
            return str.Value;
        }

        internal class CanonicalizedString
         {
             // 字段
             private StringBuilder canonicalizedString = new StringBuilder();

             // 方法
             internal CanonicalizedString(string initialElement)
             {
                 this.canonicalizedString.Append(initialElement);
             }

             internal void AppendCanonicalizedElement(string element)
             {
                 this.canonicalizedString.Append("\n");
                 this.canonicalizedString.Append(element);
             }

             // 属性
             internal string Value
             {
                 get
                 {
                     return this.canonicalizedString.ToString();
                 }
             }
         }

#endregion

    }
}
