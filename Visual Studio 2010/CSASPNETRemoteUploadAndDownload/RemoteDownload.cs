/**************************************** 模块头 *****************************************\
* 模块名:  RemoteDownload.cs
* 项目名:      CSASPNETRemoteUploadAndDownload
* 版权 (c) Microsoft Corporation.
* 
* WebClient 和 FtpWebRequest 泪都提供常用方法来发送数据到服务器URI.
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
    public abstract class RemoteDownload
    {

        public string UrlString
        {
            get;
            set;
        }


        public string DestDir
        {
            get;
            set;
        }

        public RemoteDownload(string urlString, string destDir)
        {
            this.UrlString = urlString;
            this.DestDir = destDir;
        }

        ///<summary>
        ///从远程服务器下载文件
        ///</summary>
        public virtual bool DownloadFile()
        {
            return true;
        }
    }

    /// <summary>
    /// HttpRemoteDownload 类
    /// </summary>
    public class HttpRemoteDownload : RemoteDownload
    {
        public HttpRemoteDownload(string urlString, string descFilePath)
            : base(urlString, descFilePath)
        {

        }

        public override bool DownloadFile()
        {
            string fileName = System.IO.Path.GetFileName(this.UrlString);
            string descFilePath =
                System.IO.Path.Combine(this.DestDir, fileName);
            try
            {
                WebRequest myre = WebRequest.Create(this.UrlString);
            }
            catch(Exception ex)
            {
                throw new Exception("服务器上不存在对应文件", ex.InnerException);
            }
            try
            {
                byte[] fileData;
                using (WebClient client = new WebClient())
                {
                    fileData = client.DownloadData(this.UrlString);
                }
                using (FileStream fs =
                      new FileStream(descFilePath, FileMode.OpenOrCreate))
                {
                    fs.Write(fileData, 0, fileData.Length);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("下载失败", ex.InnerException);
            }
        }
    }

    /// <summary>
    /// FtpDownload 类
    /// </summary>
    public class FtpRemoteDownload : RemoteDownload
    {
        public FtpRemoteDownload(string urlString, string descFilePath)
            : base(urlString, descFilePath)
        {

        }

        public override bool DownloadFile()
        {
            FtpWebRequest reqFTP;

            string fileName = System.IO.Path.GetFileName(this.UrlString);
            string descFilePath =
                System.IO.Path.Combine(this.DestDir, fileName);

            try
            {

                reqFTP = (FtpWebRequest)FtpWebRequest.Create(this.UrlString);
                reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;
                reqFTP.UseBinary = true;

                using (FileStream outputStream = new FileStream(descFilePath, FileMode.OpenOrCreate))
                {
                    using (FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse())
                    {
                        using (Stream ftpStream = response.GetResponseStream())
                        {
                            int bufferSize = 2048;
                            int readCount;
                            byte[] buffer = new byte[bufferSize];
                            readCount = ftpStream.Read(buffer, 0, bufferSize);
                            while (readCount > 0)
                            {
                                outputStream.Write(buffer, 0, readCount);
                                readCount = ftpStream.Read(buffer, 0, bufferSize);
                            }
                        }
                    }

                }
                return true;
            }

            catch (Exception ex)
            {
                throw new Exception("下载失败", ex.InnerException);
            }
        }
    }
}