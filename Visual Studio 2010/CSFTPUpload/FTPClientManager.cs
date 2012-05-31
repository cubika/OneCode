/****************************** 模块头******************************\
* 模块名:  FTPClientManager.cs
* 项目:	    CSFTPUpload
* Copyright (c) Microsoft Corporation.

 * FTPClientManager类提供如下特性：
 * 1.查证是否一个文件或一个目录在FTP服务器上存在。
 * 2.列出在FTP服务器子目录和文件夹中的文件
 * 3.删除FTP服务器上的文件和目录
 * 4.在FTP服务器上创建一个目录
 * 5.管理FTPUploadClient上传文件件到FTP服务器上
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE
\***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Linq;

namespace CSFTPUpload
{
    public partial class FTPClientManager
    {

        /// <summary>

        /// 连接认证到FTP服务器
        /// </summary>
        public ICredentials Credentials { get; set; }

        /// <summary>

        /// FTPClient当前路径
        /// </summary>
        public Uri Url { get; private set; }

        FTPClientManagerStatus status;

        /// <summary>

        /// 得到或设置FTPClient状态
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


                    // 执行OnStatusChanged事件
                    this.OnStatusChanged(EventArgs.Empty);

                }
            }
        }

        public event EventHandler UrlChanged;

        public event EventHandler<ErrorEventArgs> ErrorOccurred;

        public event EventHandler StatusChanged;

        public event EventHandler<FileUploadCompletedEventArgs> FileUploadCompleted;

        public event EventHandler<NewMessageEventArg> NewMessageArrived;

        /// <summary>

        ///  初始化一个FTPClient事例
        /// </summary>
        public FTPClientManager(Uri url, ICredentials credentials)
        {
            this.Credentials = credentials;


            //检查路径是否存在并且认证是否正确 假如有错误，抛出异常
            CheckFTPUrlExist(url);

            this.Url = url;

            // 设置 Status.
            this.Status = FTPClientManagerStatus.Idle;

        }

        /// <summary>
    
        /// 导航到父文件夹
        /// </summary>
        public void NavigateParent()
        {
            if (Url.AbsolutePath != "/")
            {

               
                //得到父路径
                Uri newUrl = new Uri(this.Url, "..");


                //检查路径是否存在
                CheckFTPUrlExist(newUrl);

                this.Url = newUrl;
                this.OnUrlChanged(EventArgs.Empty);
            }
        }

        /// <summary>

        /// 导航目录
        /// </summary>
        public void Naviagte(Uri newUrl)
        {

            //检查路径是否存在
            bool urlExist = VerifyFTPUrlExist(newUrl);

            this.Url = newUrl;
            this.OnUrlChanged(EventArgs.Empty);
        }

        /// <summary>

        /// 假如有路径不存在，抛出异常
        /// </summary>
        void CheckFTPUrlExist(Uri url)
        {
            bool urlExist = VerifyFTPUrlExist(url);

            if (!urlExist)
            {
                throw new ApplicationException("路径不存在");
            }
        }

        /// <summary>
       
        /// 核实路径是否存在
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

        /// 核实目录是否存在
        /// </summary>
        bool VerifyDirectoryExist(Uri url)
        {
            FtpWebRequest request = WebRequest.Create(url) as FtpWebRequest;
            
            if(this.Credentials!=null)
            {
            request.Credentials = this.Credentials;
            }
            else
            {
                request.UseDefaultCredentials = true; ;
            }

            
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

        /// 核实文件是否存在
        /// </summary>
        bool VerifyFileExist(Uri url)
        {
            FtpWebRequest request = WebRequest.Create(url) as FtpWebRequest;

            if (this.Credentials != null)
            {
                request.Credentials = this.Credentials;
            }
            else
            {
                request.UseDefaultCredentials = true; ;
            }

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

        /// 得到子目录和默认的当前路径文件
        /// </summary>
        public IEnumerable<FTPFileSystem> GetSubDirectoriesAndFiles()
        {
            return GetSubDirectoriesAndFiles(this.Url);
        }

        /// <summary>
        
        /// 得到子目录和路径中文件，在枚举中使用所有文件夹
        /// 当运行FTP LIST 协议方法得到详细的文件列表。
        /// 在一个FTP服务器上，服务器响应许多信息记录，每个记录代表一个文件
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


                //从记录字符串中找出FTP目录列表格式
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


                //关闭StreamReader对象和相关流，释放reader系统资源
                if (reader != null)
                {
                    reader.Close();
                }
            }
        }

        /// <summary>

        /// 在远程FTP服务器上创建一个文件夹子目录
        /// </summary>
        public void CreateDirectoryOnFTPServer(Uri serverPath, string subDirectoryName)
        {

            
            //创建路径为这个新子目录
            Uri subDirUrl = new Uri(serverPath, subDirectoryName);


            //检查子目录是否存在
            bool urlExist = VerifyFTPUrlExist(subDirUrl);

            if (urlExist)
            {
                return;
            }

            try
            {

                //创建一个FtpWebRequest来创建子目录
                FtpWebRequest request = WebRequest.Create(subDirUrl) as FtpWebRequest;
                request.Credentials = this.Credentials;
                request.Method = WebRequestMethods.Ftp.MakeDirectory;

                using (FtpWebResponse response = request.GetResponse() as FtpWebResponse)
                {
                    this.OnNewMessageArrived(new NewMessageEventArg
                    {
                        NewMessage = response.StatusDescription
                    });
                }
            }


                //假如文件夹没存在，创建文件夹
            catch (System.Net.WebException webEx)
            {

                FtpWebResponse ftpResponse = webEx.Response as FtpWebResponse;

                string msg = string.Format(
                                   "There is an error while creating folder {0}. "
                                   + " StatusCode: {1}  StatusDescription: {2} ",
                                   subDirUrl.ToString(),
                                   ftpResponse.StatusCode.ToString(),
                                   ftpResponse.StatusDescription);
                ApplicationException errorException
                    = new ApplicationException(msg, webEx);


                //执行带错误的ErrorOccurred事件

                ErrorEventArgs e = new ErrorEventArgs
                {
                    ErrorException = errorException
                };

                this.OnErrorOccurred(e);
            }
        }

        /// <summary>

        /// 在FTP服务器上删除选项
        /// </summary>
        public void DeleteItemsOnFTPServer(IEnumerable<FTPFileSystem> fileSystems)
        {
            if (fileSystems == null)
            {
                throw new ArgumentException("The item to delete is null!");
            }

            foreach (var fileSystem in fileSystems)
            {
                DeleteItemOnFTPServer(fileSystem);
            }

        }

        /// <summary>

        /// 在FTP服务器上删除一个选项
        /// </summary>
        public void DeleteItemOnFTPServer(FTPFileSystem fileSystem)
        {

            //检查子目录是否存在
            bool urlExist = VerifyFTPUrlExist(fileSystem.Url);

            if (!urlExist)
            {
                return;
            }

            try
            {


                //不为空的文件夹不能删除
                if (fileSystem.IsDirectory)
                {
                    var subFTPFiles = GetSubDirectoriesAndFiles(fileSystem.Url);

                    DeleteItemsOnFTPServer(subFTPFiles);
                }


                //创建一个FtpWebRequest创建子目录
                FtpWebRequest request = WebRequest.Create(fileSystem.Url) as FtpWebRequest;
                request.Credentials = this.Credentials;

                request.Method = fileSystem.IsDirectory
                    ? WebRequestMethods.Ftp.RemoveDirectory : WebRequestMethods.Ftp.DeleteFile;

                using (FtpWebResponse response = request.GetResponse() as FtpWebResponse)
                {
                    this.OnNewMessageArrived(new NewMessageEventArg
                    {
                        NewMessage = response.StatusDescription
                    });
                }
            }
            catch (System.Net.WebException webEx)
            {
                FtpWebResponse ftpResponse = webEx.Response as FtpWebResponse;

                string msg = string.Format(
                                   "There is an error while deleting {0}. "
                                   + " StatusCode: {1}  StatusDescription: {2} ",
                                   fileSystem.Url.ToString(),
                                   ftpResponse.StatusCode.ToString(),
                                   ftpResponse.StatusDescription);
                ApplicationException errorException
                    = new ApplicationException(msg, webEx);


                //有错误时执行ErrorOccurred 事件
                ErrorEventArgs e = new ErrorEventArgs
                {
                    ErrorException = errorException
                };

                this.OnErrorOccurred(e);
            }
        }

        /// <summary>

        /// 上传一个全部的本地文件夹到FTP服务器
        /// </summary>
        public void UploadFolder(DirectoryInfo localFolder, Uri serverPath,
            bool createFolderOnServer)
        {
           
            //UploadFoldersAndFiles方法将创建或重写一个默认文件夹
            if (createFolderOnServer)
            {
                UploadFoldersAndFiles(new FileSystemInfo[] { localFolder }, serverPath);
            }


                //上传文件和本地文件夹子目录
            else
            {
                UploadFoldersAndFiles(localFolder.GetFileSystemInfos(), serverPath);
            }
        }

        /// <summary>

        /// 上传本地文件夹和文件到FTP服务器
        /// </summary>
        public void UploadFoldersAndFiles(IEnumerable<FileSystemInfo> fileSystemInfos,
            Uri serverPath)
        {
            if (this.status != FTPClientManagerStatus.Idle)
            {
                throw new ApplicationException("This client is busy now.");
            }

            this.Status = FTPClientManagerStatus.Uploading;

            FTPUploadClient uploadClient = new FTPUploadClient(this);

           
            //注册事件
            uploadClient.AllFilesUploadCompleted +=
                new EventHandler(uploadClient_AllFilesUploadCompleted);
            uploadClient.FileUploadCompleted +=
                new EventHandler<FileUploadCompletedEventArgs>(uploadClient_FileUploadCompleted);

            uploadClient.UploadDirectoriesAndFiles(fileSystemInfos, serverPath);
        }


        void uploadClient_FileUploadCompleted(object sender, FileUploadCompletedEventArgs e)
        {
            this.OnFileUploadCompleted(e);
        }

        void uploadClient_AllFilesUploadCompleted(object sender, EventArgs e)
        {
            this.Status = FTPClientManagerStatus.Idle;
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

        protected virtual void OnFileUploadCompleted(FileUploadCompletedEventArgs e)
        {
            if (FileUploadCompleted != null)
            {
                FileUploadCompleted(this, e);
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
