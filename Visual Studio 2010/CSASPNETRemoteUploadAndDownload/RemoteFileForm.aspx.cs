/**************************************** 模块头 *****************************************\
* 模块名:  RemoteFileForm.aspx.cs
* 项目名:  CSASPNETRemoteUploadAndDownload
* 版权 (c) Microsoft Corporation.
* 
* 创建RemoteDownload实例instance, 输入下载文件名和服务器url地址的参数.
* 使用DownloadFile方法从远程服务器下载文件.
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
using System.IO;

namespace CSRemoteUploadAndDownload
{
    public partial class RemoteFileForm : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            RemoteUpload uploadClient = null;

            if (this.rbUploadList.SelectedIndex == 0)
            {
                uploadClient =
                    new HttpRemoteUpload(this.FileUpload.FileBytes, this.FileUpload.PostedFile.FileName, this.tbUploadUrl.Text);
            }
            else
            {
                uploadClient =
                    new FtpRemoteUpload(this.FileUpload.FileBytes, this.FileUpload.PostedFile.FileName, this.tbUploadUrl.Text);
            }

            if (uploadClient.UploadFile())
            {
                Response.Write("上传完成");
            }
            else
            {
                Response.Write("上传失败");
            }

        }

        protected void btnDownLoad_Click(object sender, EventArgs e)
        {

            RemoteDownload downloadClient = null;

            if (this.rbDownloadList.SelectedIndex == 0)
            {
                downloadClient =
                    new HttpRemoteDownload(this.tbDownloadUrl.Text, this.tbDownloadPath.Text);
            }
            else
            {
                downloadClient =
                    new FtpRemoteDownload(this.tbDownloadUrl.Text, this.tbDownloadPath.Text);

            }

            if (downloadClient.DownloadFile())
            {
                Response.Write("下载完成");
            }
            else
            {
                Response.Write(" 下载失败");
            }

        }
    }
}