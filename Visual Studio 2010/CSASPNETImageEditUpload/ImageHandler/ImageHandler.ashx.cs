/**************************************** 模块头 *****************************************\
* 模块名:    ImageHandler.ashx
* 项目名:    CSASPNETImagEditUpload
* 版权 (c) Microsoft Corporation
*
* 这是一个常用从特定数据库中根据指定Id记录读取字节集合的http-handler.
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
using System.Collections.Generic;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace CSAspNetImageEditUpload.ImageHandler
{
    /// <summary>
    /// 连接数据库DataBase,
    /// 如果指定记录存在图片,作为图片类型读取指定行的图片字节集合,并输出.
    /// 否则, 输出默认图片作为替代 .
    /// </summary>
    public class ImageHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = new SqlConnection(
                    ConfigurationManager.ConnectionStrings["db_PersonsConnectionString"]
                    .ConnectionString);
                cmd.Connection.Open();
                cmd.CommandText = "select PersonImage,PersonImageType from tb_personInfo"+
               " where id=" + context.Request.QueryString["id"];
                
                SqlDataReader reader = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection 
                    | System.Data.CommandBehavior.SingleRow);

                if (reader.Read())
                {
                    byte[] imgbytes=null;
                    string imgtype=null;

                    if (reader.GetValue(0) != DBNull.Value)
                    {
                       imgbytes = (byte[])reader.GetValue(0);
                       imgtype = reader.GetString(1);

                        // 如果是bmp,因为格式不同先转换成jpg然后显示.

                        if (imgtype.Equals("image/bmp",StringComparison.OrdinalIgnoreCase))
                        {
                            using (MemoryStream ms = new MemoryStream(imgbytes))
                            {
                                using (Bitmap bm = new Bitmap(Image.FromStream(ms)))
                                {
                                    bm.Save(context.Response.OutputStream, ImageFormat.Jpeg);
                                }
                            }
                        }
                        else
                        {
                            context.Response.ContentType = imgtype;
                            context.Response.BinaryWrite(imgbytes);
                        }
                        
                    }
                    else
                    {
                        imgbytes = File.ReadAllBytes(context.Server.MapPath
                            ("~/DefaultImage/DefaultImage.JPG"));
                        imgtype = "image/pjpeg";
                        context.Response.ContentType = imgtype;
                        context.Response.BinaryWrite(imgbytes);
                    }
                }
                reader.Close();
                context.Response.End();
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}