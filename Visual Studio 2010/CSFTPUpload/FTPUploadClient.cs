/****************************** 模块头******************************\
* 模块名:  FTPUploadClient.cs
* 项目:	    CSFTPUpload
* Copyright (c) Microsoft Corporation.

* 这个类被使用上传一个文件到FTP服务器，当上传开始，将要在一个后台线程上上传文件
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
using System.Threading;
using System.IO;
using System.Net;

namespace CSFTPUpload
{
    public partial class FTPClientManager
    {

        public class FTPUploadClient
        {
            // 2M bytes.
            public const int MaxCacheSize = 2097152;

            // 2K bytes.
            public const int BufferSize = 2048;

            FTPClientManager manager;

            public event EventHandler<FileUploadCompletedEventArgs>
                FileUploadCompleted;

            public event EventHandler AllFilesUploadCompleted;

            public FTPUploadClient(FTPClientManager manager)
            {
                if (manager == null)
                {
                    throw new ArgumentNullException("FTPClientManager不能为空.");
                }

                this.manager = manager;
            }

            /// <summary>
            /// 上传文件，目录和其子目录
            /// </summary>
            public void UploadDirectoriesAndFiles(IEnumerable<FileSystemInfo> fileSysInfos,
                Uri serverPath)
            {
                if (fileSysInfos == null)
                {
                    throw new ArgumentNullException(
                        "文件上传不能为空.");
                }

                //创建一个线程上传数据。
                ParameterizedThreadStart threadStart =
                    new ParameterizedThreadStart(StartUploadDirectoriesAndFiles);
                Thread uploadThread = new Thread(threadStart);
                uploadThread.IsBackground = true;
                uploadThread.Start(new object[] { fileSysInfos, serverPath });
            }

            /// <summary>

            /// 上传文件，目录和其子目录
            /// </summary>
            void StartUploadDirectoriesAndFiles(object state)
            {
                var paras = state as object[];

                IEnumerable<FileSystemInfo> fileSysInfos =
                    paras[0] as IEnumerable<FileSystemInfo>;
                Uri serverPath = paras[1] as Uri;

                foreach (var fileSys in fileSysInfos)
                {
                    UploadDirectoryOrFile(fileSys, serverPath);
                }

                this.OnAllFilesUploadCompleted(EventArgs.Empty);
            }

            /// <summary>

            /// 上传一个单独的文件和目录
            /// </summary>
            void UploadDirectoryOrFile(FileSystemInfo fileSystem, Uri serverPath)
            {


                //直接上传文件
                if (fileSystem is FileInfo)
                {
                    UploadFile(fileSystem as FileInfo, serverPath);
                }

                    //上传一个目录
                else
                {

                    //构造子目录路径
                    Uri subDirectoryPath = new Uri(serverPath, fileSystem.Name + "/");

                    this.manager.CreateDirectoryOnFTPServer(serverPath, fileSystem.Name + "/");

                    //得到子目录和文件
                    var subDirectoriesAndFiles = (fileSystem as DirectoryInfo)
                        .GetFileSystemInfos();

                    //上传文件夹中的文件和子目录
                    foreach (var subFile in subDirectoriesAndFiles)
                    {
                        UploadDirectoryOrFile(subFile, subDirectoryPath);
                    }
                }
            }

            /// <summary>
            /// 直接上传一个单独文件
            /// </summary>
            void UploadFile(FileInfo file, Uri serverPath)
            {
                if (file == null)
                {
                    throw new ArgumentNullException(" 上传文件为空. ");
                }

                Uri destPath = new Uri(serverPath, file.Name);


                //创建一个上传文件的请求
                FtpWebRequest request = WebRequest.Create(destPath) as FtpWebRequest;

                request.Credentials = this.manager.Credentials;

                // 上传文件.
                request.Method = WebRequestMethods.Ftp.UploadFile;

                FtpWebResponse response = null;
                Stream requestStream = null;
                FileStream localFileStream = null;

                try
                {

                    //从服务器检索响应
                    response = request.GetResponse() as FtpWebResponse;

                    //打开读取本地文件
                    localFileStream = file.OpenRead();

                    //检索要求流
                    requestStream = request.GetRequestStream();

                    int bytesSize = 0;
                    byte[] uploadBuffer = new byte[FTPUploadClient.BufferSize];

                    while (true)
                    {

                        //读取本地文件的缓冲
                        bytesSize = localFileStream.Read(uploadBuffer, 0,
                          uploadBuffer.Length);

                        if (bytesSize == 0)
                        {
                            break;
                        }
                        else
                        {

                            //在请求流中写缓冲。
                            requestStream.Write(uploadBuffer, 0, bytesSize);

                        }
                    }

                    var fileUploadCompletedEventArgs = new FileUploadCompletedEventArgs
                    {

                        LocalFile = file,
                        ServerPath = destPath
                    };

                    this.OnFileUploadCompleted(fileUploadCompletedEventArgs);

                }
                catch (System.Net.WebException webEx)
                {
                    FtpWebResponse ftpResponse = webEx.Response as FtpWebResponse;

                    string msg = string.Format(
                                       "当上传{0}有个错误. "
                                       + " StatusCode: {1}  StatusDescription: {2} ",
                                       file.FullName,
                                       ftpResponse.StatusCode.ToString(),
                                       ftpResponse.StatusDescription);
                    ApplicationException errorException
                        = new ApplicationException(msg, webEx);

                    //执行带错误的 ErrorOccurred事件
                    ErrorEventArgs e = new ErrorEventArgs
                    {
                        ErrorException = errorException
                    };

                    this.manager.OnErrorOccurred(e);
                }
                catch (Exception ex)
                {
                    string msg = string.Format(
                                       "当上传 {0}有个错误. "
                                       + " See InnerException for detailed error. ",
                                       file.FullName);
                    ApplicationException errorException
                        = new ApplicationException(msg, ex);

                    //激发ErrorOccurred事件
                    ErrorEventArgs e = new ErrorEventArgs
                    {
                        ErrorException = errorException
                    };

                    this.manager.OnErrorOccurred(e);
                }
                finally
                {
                    if (response != null)
                    {
                        response.Close();
                    }

                    if (requestStream != null)
                    {
                        requestStream.Close();
                    }

                    if (localFileStream != null)
                    {
                        localFileStream.Close();
                    }
                }
            }

            protected virtual void OnFileUploadCompleted(FileUploadCompletedEventArgs e)
            {

                if (FileUploadCompleted != null)
                {
                    FileUploadCompleted(this, e);
                }
            }

            protected virtual void OnAllFilesUploadCompleted(EventArgs e)
            {
                if (AllFilesUploadCompleted != null)
                {
                    AllFilesUploadCompleted(this, e);
                }
            }
        }
    }
}
