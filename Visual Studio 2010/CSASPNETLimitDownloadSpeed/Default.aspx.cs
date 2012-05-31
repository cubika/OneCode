/******************************* 模块头 ***********************************\
* 模块名:    Default.aspx.cs
* 项目名:    CSASPNETLimitDownloadSpeed
* 版权 (c) Microsoft Corporation
*
* 本项目阐述了如何通过编程来限制下载速度.  
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
using System.Web;
using System.IO;
using System.Threading;

namespace CSASPNETLimitDownloadSpeed
{

    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // 你可以增加文件的大小来得到一个更长时间的下载期. 
            // 1024 * 1024 * 1 = 1 Mb
            int length = 1024 * 1024 * 1;
            byte[] buffer = new byte[length];

            string filepath = Server.MapPath("~/bigFileSample.dat");
            using (FileStream fs = new FileStream(filepath, FileMode.Create, FileAccess.Write))
            {
                fs.Write(buffer, 0, length);
            }
        }

        protected void btnDownload_Click(object sender, EventArgs e)
        {
            string outputFileName = "bigFileSample.dat";
            string filePath = Server.MapPath("~/bigFileSample.dat");

            string value = ddlDonwloadSpeed.SelectedValue;

            // 1024 * 20 = 20 Kb/s.
            int downloadSpeed = 1024 * int.Parse(value);

            Response.Clear();

            // 调用DownloadFileWithLimitedSpeed方法来下载文件.
            try
            {
                DownloadFileWithLimitedSpeed(outputFileName, filePath, downloadSpeed);
            }
            catch (Exception ex)
            {
                Response.Write(@"<p><font color=""red"">");
                Response.Write(ex.Message);
                Response.Write(@"</font></p>");
            }
            Response.End();
        }

        public void DownloadFileWithLimitedSpeed(string fileName, string filePath, long downloadSpeed)
        {
            if (!File.Exists(filePath))
            {
                throw new Exception("Err: There is no such a file to download.");
            }

            // 获得文件的一个BinaryReader实例来下载. 
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (BinaryReader br = new BinaryReader(fs))
                {

                    Response.Buffer = false;

                    // 文件长度.
                    long fileLength = fs.Length;

                    // 包最小为 1024 = 1 Kb.
                    int pack = 1024;

                    // 初始的公式是: sleep = 1000 / (下载速度/ 包)
                    // 它等于1000.0 * 包 / 下载速度.
                    // 这里的1000.0表示1000毫秒 = 1秒. 
                    int sleep = (int)Math.Ceiling(1000.0 * pack / downloadSpeed);


                    // 设置当前响应的头部.
                    Response.AddHeader("Content-Length", fileLength.ToString());
                    Response.ContentType = "application/octet-stream";

                    string utf8EncodingFileName = HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8);
                    Response.AddHeader("Content-Disposition", "attachment;filename=" + utf8EncodingFileName);

                    // maxCount表示线程发送的文件包的总数. 
                    int maxCount = (int)Math.Ceiling(Convert.ToDouble(fileLength) / pack);

                    for (int i = 0; i < maxCount; i++)
                    {
                        if (Response.IsClientConnected)
                        {
                            Response.BinaryWrite(br.ReadBytes(pack));

                            // 在响应线程发送一个文件包以后让它进入休眠状态.
                            Thread.Sleep(sleep);
                        }
                        else
                        {
                            break;
                        }
                    }

                }
            }
        }
    }
}