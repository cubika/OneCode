/****************************** 模块头 ******************************\
* 模块名:  FTPClientManager.cs
* 项目名:	    CSFTPDownload
* 版权(c)  Microsoft Corporation.

* 这个类FTPClientManager有以下特点：
* 1.验证在FTP服务上是否存在一个文件和目录。
* 2.列出FTP服务上的子目录和一个文件夹的文件。
* 3.删除FTP服务上的文件和目录。
* 4.在FTP服务上创建一个目录
* 5. 管理FTPDownloadClient从FTP服务下载的文件。
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
using System.IO;
using System.Net;

namespace CSFTPDownload
{
    public partial class FTPClientManager
    {
       

        /// <summary>
        /// 连接FTP服务器的凭据.
        /// </summary>
        public ICredentials Credentials { get; set; }

        /// <summary>
        ///  这个FTPClient当前的URL.
        /// </summary>
        public Uri Url { get; private set; }

        FTPClientManagerStatus status;

        /// <summary>
        /// 获得或者设置这个FTPClient的状态.
        /// </summary>
        public FTPClientManagerStatus Status
        {
            get
            {
                return status;
            }

            private set
            {
                if (status != value)
                {
                    status = value;

                    //提出一个OnStatusChanged事件.
                    this.OnStatusChanged(EventArgs.Empty);

                }
            }
        }

        public event EventHandler UrlChanged;

        public event EventHandler<ErrorEventArgs> ErrorOccurred;

        public event EventHandler StatusChanged;

        public event EventHandler<FileDownloadCompletedEventArgs> FileDownloadCompleted;

        public event EventHandler<NewMessageEventArg> NewMessageArrived;

        /// <summary>
        ///  初始化一个FTPClient实例.
        /// </summary>
        public FTPClientManager(Uri url, ICredentials credentials)
        {
            this.Credentials = credentials;

            // 检查URL是否存在和凭据是否正确.
            // 如果有一个错误，将会抛出异常.
            CheckFTPUrlExist(url);

            this.Url = url;

            // 设置状态
            this.Status = FTPClientManagerStatus.Idle;

        }

        /// <summary>
        /// 导航到上一级目录.
        /// </summary>
        public void NavigateParent()
        {
            if (Url.AbsolutePath != "/")
            {

                //获得上一级目录的URL.
                Uri newUrl = new Uri(this.Url, "..");

                //检查URL是否存在.
                CheckFTPUrlExist(newUrl);

                this.Url = newUrl;
                this.OnUrlChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// 导航一个URL.
        /// </summary>
        public void Naviagte(Uri newUrl)
        {
            //检查URL是否存在.
            bool urlExist = VerifyFTPUrlExist(newUrl);

            this.Url = newUrl;
            this.OnUrlChanged(EventArgs.Empty);
        }

        /// <summary>
        /// 如果这个URL不存在的话, 将抛出一个异常.
        /// </summary>
        void CheckFTPUrlExist(Uri url)
        {
            bool urlExist = VerifyFTPUrlExist(url);

            if (!urlExist)
            {
                throw new ApplicationException("The url does not exist");
            }
        }

        /// <summary>
        /// 验证URL是否存在.
        /// </summary>
        bool VerifyFTPUrlExist(Uri url)
        {
            bool urlExist = false;

            if (url.IsFile)
            {
                urlExist = VerifyFileExist(url);
            }
            else
            {
                urlExist = VerifyDirectoryExist(url);
            }

            return urlExist;
        }

        /// <summary>
        /// 验证是否存在目录.
        /// </summary>
        bool VerifyDirectoryExist(Uri url)
        {
            FtpWebRequest request = WebRequest.Create(url) as FtpWebRequest;
            request.Credentials = this.Credentials;
            request.Method = WebRequestMethods.Ftp.ListDirectory;

            FtpWebResponse response = null;

            try
            {
                response = request.GetResponse() as FtpWebResponse;

                return response.StatusCode == FtpStatusCode.DataAlreadyOpen;
            }
            catch (System.Net.WebException webEx)
            {
                FtpWebResponse ftpResponse = webEx.Response as FtpWebResponse;

                if (ftpResponse.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                {
                    return false;
                }

                throw;
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
            }
        }

        /// <summary>
        /// 验证文件是否存在.
        /// </summary>
        bool VerifyFileExist(Uri url)
        {
            FtpWebRequest request = WebRequest.Create(url) as FtpWebRequest;
            request.Credentials = this.Credentials;
            request.Method = WebRequestMethods.Ftp.GetFileSize;

            FtpWebResponse response = null;

            try
            {
                response = request.GetResponse() as FtpWebResponse;

                return response.StatusCode == FtpStatusCode.FileStatus;
            }
            catch (System.Net.WebException webEx)
            {
                FtpWebResponse ftpResponse = webEx.Response as FtpWebResponse;

                if (ftpResponse.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                {
                    return false;
                }

                throw;
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
            }
        }

        /// <summary>
        ///通过dafault获得当前Url的子目录和文件。
        /// </summary>
        public IEnumerable<FTPFileSystem> GetSubDirectoriesAndFiles()
        {
            return GetSubDirectoriesAndFiles(this.Url);
        }

        /// <summary>
        /// 获得Url的子目录和文件. 对于所有的文件夹他将使用枚举.
         
        /// 在一个FTP 服务上，当运行FTP LIST 协议方法来获得一个文件的详细列表时，
        /// 这个服务将响应一些信息的记录。每一个记录代表一个文件。
        /// </summary>
        public IEnumerable<FTPFileSystem> GetSubDirectoriesAndFiles(Uri url)
        {
            FtpWebRequest request = WebRequest.Create(url) as FtpWebRequest;

            request.Credentials = this.Credentials;

            request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;


            FtpWebResponse response = null;
            Stream responseStream = null;
            StreamReader reader = null;
            try
            {
                response = request.GetResponse() as FtpWebResponse;

                this.OnNewMessageArrived(new NewMessageEventArg
                {
                    NewMessage = response.StatusDescription
                });

                responseStream = response.GetResponseStream();
                reader = new StreamReader(responseStream);

                List<FTPFileSystem> subDirs = new List<FTPFileSystem>();

                string subDir = reader.ReadLine();

                //在记录的字符串中找到FTP目录列表类型.
                FTPDirectoryListingStyle style = FTPDirectoryListingStyle.MSDOS;
                if (!string.IsNullOrEmpty(subDir))
                {
                    style = FTPFileSystem.GetDirectoryListingStyle(subDir);
                }
                while (!string.IsNullOrEmpty(subDir))
                {
                    subDirs.Add(FTPFileSystem.ParseRecordString(url, subDir, style));

                    subDir = reader.ReadLine();
                }
                return subDirs;
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }

                // 关闭StreamReader对象和底层流,并且释放与reader有关的任何资源。
              
                if (reader != null)
                {
                    reader.Close();
                }
            }
        }

        /// <summary>
        /// 下载文件，目录和它们的子目录.
        /// </summary>
        public void DownloadDirectoriesAndFiles(IEnumerable<FTPFileSystem> files, string localPath)
        {
            if (this.status != FTPClientManagerStatus.Idle)
            {
                throw new ApplicationException("This client is busy now.");
            }

            this.Status = FTPClientManagerStatus.Downloading;

            FTPDownloadClient downloadClient = new FTPDownloadClient(this);
            downloadClient.FileDownloadCompleted += new EventHandler<FileDownloadCompletedEventArgs>(downloadClient_FileDownloadCompleted);
            downloadClient.AllFilesDownloadCompleted += new EventHandler(downloadClient_AllFilesDownloadCompleted);
            downloadClient.DownloadDirectoriesAndFiles(files,localPath);
            
        }

        void downloadClient_AllFilesDownloadCompleted(object sender, EventArgs e)
        {
            this.Status = FTPClientManagerStatus.Idle;
        }

        void downloadClient_FileDownloadCompleted(object sender, FileDownloadCompletedEventArgs e)
        {
            this.OnFileDownloadCompleted(e);
        }
      
        protected virtual void OnUrlChanged(EventArgs e)
        {
            if (UrlChanged != null)
            {
                UrlChanged(this, e);
            }
        }

        protected virtual void OnStatusChanged(EventArgs e)
        {
            if (StatusChanged != null)
            {
                StatusChanged(this, e);
            }
        }

        protected virtual void OnFileDownloadCompleted(FileDownloadCompletedEventArgs e)
        {
            if (FileDownloadCompleted != null)
            {
                FileDownloadCompleted(this, e);
            }
        }

        protected virtual void OnErrorOccurred(ErrorEventArgs e)
        {
            this.Status = FTPClientManagerStatus.Idle;

            if (ErrorOccurred != null)
            {
                ErrorOccurred(this, e);
            }
        }

        protected virtual void OnNewMessageArrived(NewMessageEventArg e)
        {
            if (NewMessageArrived != null)
            {
                NewMessageArrived(this, e);
            }
        }
    }
}
