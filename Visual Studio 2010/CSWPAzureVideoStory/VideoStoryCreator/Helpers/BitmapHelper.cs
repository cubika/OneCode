/********************************* 模块头 *********************************\
* 模块名: BitmapHelper.cs
* 项目名: VideoStoryCreator
* 版权(c) Microsoft Corporation.
* 
* 重定义图片的大小的helper类.
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
using System.Windows.Media.Imaging;
using Microsoft.Xna.Framework.Media;

namespace VideoStoryCreator
{
    public static class BitmapHelper
    {
        // 目前我们使用硬编码.
        // 以后我们会支持用户定义.
        // TODO: 将所有的硬编码值移到定义的地方.
        internal static int ResizedImageWidth = 800;
        internal static int ResizedImageHeight = 600;

        /// <summary>
        /// 从XNA媒体库中取到图片.
        /// 并且重新定义到目前大小.
        /// </summary>
        /// <param name="name">图片名称</param>
        /// <returns>通过stream定义图片大小, jpeg格式.</returns>
        public static Stream GetResizedImage(string name)
        {
            MediaLibrary mediaLibrary = new MediaLibrary();
            PictureCollection pictureCollection = mediaLibrary.Pictures;

            Picture picture = pictureCollection.Where(p => p.Name == name).FirstOrDefault();
            if (picture == null)
            {
                throw new InvalidOperationException(string.Format("不能加载图片 {0}. 图片可能被删除", name));
            }
            Stream originalImageStream = picture.GetImage();
            BitmapImage bmp = new BitmapImage();
            bmp.SetSource(originalImageStream);
            WriteableBitmap originalImage = new WriteableBitmap(bmp);
            MemoryStream targetStream = new MemoryStream();
            originalImage.SaveJpeg(targetStream, ResizedImageWidth, ResizedImageHeight, 0, 100);

            // 现在图片被移到WriteableBitmap类, 原图片的stream被关闭.
            originalImageStream.Close();

            targetStream.Position = 0;
            return targetStream;
        }
    }
}
