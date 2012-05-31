/********************************* 模块头 *********************************\
* 模块名:    UploadFile.cs
* 项目名:    CSASPNETFileUploadStatus
* 版权 (c) Microsoft Corporation
*
* 本项目阐述了在不使用第三方组件时实现显示上传的状态和进程
* 像ActiveX 控件,Flash 或者Silverlight.
* 
* 这个类用来在请求实体中上传文件. 
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*****************************************************************************/

using System;
using System.IO;
using System.Web;


namespace CSASPNETFileUploadStatus
{
    public class UploadFile
    {
        private string cachePath = null;
        private int cacheLength = 1024 * 1024 * 5;

        public string FileName { get; private set; }
        public string ContentType { get; private set; }
        public string ClientPath { get; private set; }
        private byte[] Data { get; set; }

        private readonly string _defaultUploadFolder = "UploadedFiles";

        public UploadFile(string clientPath, string contentType)
        {
            Data = new byte[0];
            ClientPath = clientPath;
            ContentType = contentType;
            cachePath = HttpContext.Current.Server.MapPath("uploadcaching") +
                        "\\" + Guid.NewGuid().ToString();
            FileName = new FileInfo(clientPath).Name;
            FileInfo cache_file = new FileInfo(cachePath);
            if (!cache_file.Directory.Exists)
            {
                cache_file.Directory.Create();
            }
        }

        // 对于大文件我们需要读取并存储部分数据.
        // 并且这种方法可以被用来把部分数据块合并起来.
        internal void AppendData(byte[] data)
        {
            this.Data = BinaryHelper.Combine(this.Data, data);
            if (this.Data.Length > cacheLength)
            {
                CacheData();
            }
        }

        // 为了释放内存，我们可以存储 
        // 那些已读进磁盘的数据.
        private void CacheData()
        {
            if (this.Data != null && this.Data.Length > 0)
            {

                using (FileStream fs = new FileStream(
                     cachePath, FileMode.Append, FileAccess.Write))
                {
                    fs.Write(Data, 0, Data.Length);
                    this.Data = new byte[0];
                }
            }
        }

        // 清除模板文件.
        internal void ClearCache()
        {
            if (File.Exists(cachePath))
            {
                File.Delete(cachePath);
            }
        }


        // 用正确的存储路径来存储上传文件.
        public void Save(string path)
        {
            if (this.Data.Length > 0)
            {
                CacheData();
            }

            if (String.IsNullOrEmpty(path))
            {
                path = HttpContext.Current.Server.MapPath(
                    _defaultUploadFolder + "\\" + FileName);
            }

            // 把缓存文件移到正确的路径.
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            if (!new FileInfo(path).Directory.Exists)
            {
                new FileInfo(path).Directory.Create();
            }
            File.Move(cachePath, path);
        }

    }
}
