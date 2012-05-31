/**************************************** 模块头 *****************************************\
* 模块名:  RemoteUpload.cs
* 项目名:      CSASPNETRemoteUploadAndDownload
* 版权 (c) Microsoft Corporation.
* 
* WebClient 和 FtpWebRequest类都提供常用方法来发送数据到服务器URI.
* 同时接受来自由URI定义的资源的数据.
*
* 当上传和下载文件时, 这些类会提交webrequest到用户输入的url.
*
* UploadData()方法通过HTTP或FTP发送一个数据缓冲(未编码)到以方法参数指定的资源,
* 然后返回服务器的web响应. 相应的, DownloadData()方法请求一个HTTP
* 或FTP下载方法到远程服务器来获得服务器的输出流.
*
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
*
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*****************************************************************************************/

using System;
using System.Net;
using System.IO;

namespace CSRemoteUploadAndDownload
{
    public abstract class RemoteUpload
    {

        public string FileName
        {
            get;
            set;
        }


        public string UrlString
        {
            get;
            set;
        }


        public string NewFileName
        {
            get;
            set;
        }


        public byte[] FileData
        {
            get;
            set;
        }

        public RemoteUpload(byte[] fileData, string fileName, string urlString)
        {
            this.FileData = fileData;
            this.FileName = fileName;
            this.UrlString = urlString.EndsWith("/") ? urlString : urlString + "/";
            string newFileName = DateTime.Now.ToString("yyMMddhhmmss")
                        + DateTime.Now.Millisecond.ToString()
                        + Path.GetExtension(this.FileName);
            this.UrlString = this.UrlString + newFileName;
        }

        /// <summary>
        /// 上传文件到远程服务器
        /// </summary>
        /// <returns></returns>
        public virtual bool UploadFile()
        {
            return true;
        }

    }

    /// <summary>
    /// HttpUpload 类
    /// </summary>
    public class HttpRemoteUpload : RemoteUpload
    {
        public HttpRemoteUpload(byte[] fileData, string fileNamePath, string urlString)
            : base(fileData, fileNamePath, urlString)
        {

        }

        public override bool UploadFile()
        {
            byte[] postData;
            try
            {
                postData = this.FileData;
                using (WebClient client = new WebClient())
                {
                    client.Credentials = CredentialCache.DefaultCredentials;
                    client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                    client.UploadData(this.UrlString, "PUT", postData);
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("上传失败", ex.InnerException);
            }

        }
    }

    /// <summary>
    /// FtpUpload 类
    /// </summary>
    public class FtpRemoteUpload : RemoteUpload
    {
        public FtpRemoteUpload(byte[] fileData, string fileNamePath, string urlString)
            : base(fileData, fileNamePath, urlString)
        {

        }

        public override bool UploadFile()
        {
            FtpWebRequest reqFTP;
            reqFTP = (FtpWebRequest)FtpWebRequest.Create(this.UrlString);
            reqFTP.KeepAlive = true;
            reqFTP.Method = WebRequestMethods.Ftp.UploadFile;
            reqFTP.UseBinary = true;
            reqFTP.ContentLength = this.FileData.Length;
            
            int buffLength = 2048;
            byte[] buff = new byte[buffLength];
            MemoryStream ms = new MemoryStream(this.FileData);
            
            try
            {
                int contenctLength;
                using (Stream strm = reqFTP.GetRequestStream())
                {
                    contenctLength = ms.Read(buff, 0, buffLength);
                    while (contenctLength > 0)
                    {
                        strm.Write(buff, 0, contenctLength);
                        contenctLength = ms.Read(buff, 0, buffLength);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("上传失败", ex.InnerException);
            }
        }

    }
}