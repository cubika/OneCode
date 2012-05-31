/****************************** 模块头 ***********************************\ 
* 模块名:    TiffImageConverter.cs 
* 项目名:    CSTiffImageConverter
* 版权 (c) Microsoft Corporation. 
* 
* 该类定义了用于 TIFF 文件和 JPEG 文件相互转换的辅助方法.
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
using System.IO;
using System.Linq;
using System.Drawing.Imaging;
using System.Drawing;


namespace CSTiffImageConverter
{
    public class TiffImageConverter
    {
        /// <summary>
        /// 将 jpeg 图像转换为 tiff 图像.
        /// </summary>
        /// <param name="fileName">Tiff 图像的完整名称.</param>
        /// <returns>存放 jpeg 图像完整名称的字符串数组.</returns>
        public static string[] ConvertTiffToJpeg(string fileName)
        {
            using (Image imageFile = Image.FromFile(fileName))
            {
                FrameDimension frameDimensions = new FrameDimension(
                    imageFile.FrameDimensionsList[0]);

                // 获取 TIFF 图像中帧的数目（如果为多页）.
                int frameNum = imageFile.GetFrameCount(frameDimensions);
                string[] jpegPaths = new string[frameNum];

                for (int frame = 0; frame < frameNum; frame++)
                {
                    // 每次选择一个帧，并将另存为 jpeg 图像.
                    imageFile.SelectActiveFrame(frameDimensions, frame);
                    using (Bitmap bmp = new Bitmap(imageFile))
                    {
                        jpegPaths[frame] = String.Format("{0}\\{1}{2}.jpg", 
                            Path.GetDirectoryName(fileName),
                            Path.GetFileNameWithoutExtension(fileName), 
                            frame);
                        bmp.Save(jpegPaths[frame], ImageFormat.Jpeg);
                    }
                }

                return jpegPaths;
            }
        }

        /// <summary>
        /// 将 jpeg 图像转换为 tiff 图像.
        /// </summary>
        /// <param name="fileNames">
        /// 存有 jpeg 图像完整名称的字符串数组.
        /// </param>
        /// <param name="isMultipage">
        /// 可以创造单个的多页 tiff 文件，为 true；否则，为 false.
        /// </param>
        /// <returns>
        /// 存有 tiff 图像完整名称的字符串数组
        /// </returns>
        public static string[] ConvertJpegToTiff(string[] fileNames, bool isMultipage)
        {
            EncoderParameters encoderParams = new EncoderParameters(1);
            ImageCodecInfo tiffCodecInfo = ImageCodecInfo.GetImageEncoders()
                .First(ie => ie.MimeType == "image/tiff");

            string[] tiffPaths = null; 
            if (isMultipage)
            {
                tiffPaths = new string[1];
                Image tiffImg = null;
                try
                {
                    for (int i = 0; i < fileNames.Length; i++)
                    {
                        if (i == 0)
                        {
                            tiffPaths[i] = String.Format("{0}\\{1}.tif",
                                Path.GetDirectoryName(fileNames[i]),
                                Path.GetFileNameWithoutExtension(fileNames[i]));

                            // 初始化多页 tiff 图像的第一帧.
                            tiffImg = Image.FromFile(fileNames[i]);
                            encoderParams.Param[0] = new EncoderParameter(
                                Encoder.SaveFlag, (long)EncoderValue.MultiFrame);
                            tiffImg.Save(tiffPaths[i], tiffCodecInfo, encoderParams);
                        }
                        else
                        {
                            // 添加其他帧.
                            encoderParams.Param[0] = new EncoderParameter(
                                Encoder.SaveFlag, (long)EncoderValue.FrameDimensionPage);
                            using (Image frame = Image.FromFile(fileNames[i]))
                            {
                                tiffImg.SaveAdd(frame, encoderParams);
                            }
                        }

                        if (i == fileNames.Length - 1)
                        {
                            // 最后一帧时，刷新资源并关闭它.
                            encoderParams.Param[0] = new EncoderParameter(
                                Encoder.SaveFlag, (long)EncoderValue.Flush);
                            tiffImg.SaveAdd(encoderParams);
                        }
                    }
                }
                finally
                {
                    if (tiffImg != null)
                    {
                        tiffImg.Dispose();
                        tiffImg = null;
                    }
                }
            }
            else
            {
                tiffPaths = new string[fileNames.Length];

                for (int i = 0; i < fileNames.Length; i++)
                {
                    tiffPaths[i] = String.Format("{0}\\{1}.tif",
                        Path.GetDirectoryName(fileNames[i]),
                        Path.GetFileNameWithoutExtension(fileNames[i]));

                    // 存为单个的tiff文件.
                    using (Image tiffImg = Image.FromFile(fileNames[i]))
                    {
                        tiffImg.Save(tiffPaths[i], ImageFormat.Tiff);
                    }
                }
            }

            return tiffPaths;
        }
    }
}