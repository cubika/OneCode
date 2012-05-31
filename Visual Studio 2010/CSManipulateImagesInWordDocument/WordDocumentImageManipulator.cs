/****************************** 模块头******************************\
模块名:  WordDocumentImageManipulator.cs
项目名:      CSManipulateImagesInWordDocument
版权 (c) Microsoft Corporation.

这个 WordDocumentImageManipulator 类是用来在 Word 文档中
导出、删除、替换替换图片的

在文档中的图片数据储存为一个 ImagePart， 并且在 Drawing 元素中的 Blip 元素
会引用到这个 ImagePart。不同的 Blip 元素可能引用同一个 ImagePart。

通过编辑 Blip/Drawing  元素，我们可以删除/替换在文档中的图片。

 
This source is subject to the Microsoft Public License.
See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq; 
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace CSManipulateImagesInWordDocument
{
    public class WordDocumentImageManipulator : IDisposable
    {
        bool disposed = false;

        // 当图片替换或者删除的时候发生
        public event EventHandler ImagesChanged;

        // WordprocessingDocument 实例
        public WordprocessingDocument Document { get; private set; }

        /// <summary>
        /// 初始化 WordDocumentImageManipulator 实例
        /// </summary>
        /// <param name="path">
        /// 文档的路径
        /// </param>
        public WordDocumentImageManipulator(FileInfo path)
        {

            // 以可编辑的形式打开 Word 文档
            Document = WordprocessingDocument.Open(path.FullName, true);
        
        }

        /// <summary>
        /// 获取文档中的所有图片
        /// 文档中的图片储存为 Blip 元素
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Blip> GetAllImages()
        {

            // 获取文档中的 Drawing 元素
            var drawingElements = from run in Document.MainDocumentPart.Document.Descendants<DocumentFormat.OpenXml.Wordprocessing.Run>()
                                  where run.Descendants<Drawing>().Count() != 0
                                  select run.Descendants<Drawing>().First();

            // 获取Drawing 元素中的 Blip 元素
            var blipElements = from drawing in drawingElements
                               where drawing.Descendants<Blip>().Count() > 0
                               select drawing.Descendants<Blip>().First();           

            return blipElements;
        }

        /// <summary>
        /// 从 Blip 元素中得到图片
        /// </summary>
        public Image GetImageInBlip(Blip blipElement)
        {

            // 获取被 Blip 元素引用的 ImagePart 
            var imagePart = Document.MainDocumentPart.GetPartById(blipElement.Embed.Value)
                as ImagePart;

            if (imagePart != null)
            {              
                using (Stream imageStream = imagePart.GetStream())
                {
                    Bitmap img = new Bitmap(imageStream);
                    return img;
                }
            }
            else
            {
                throw new ApplicationException("不能发现图片部分：" 
                    + blipElement.Embed.Value);
            }
        }

        /// <summary>
        /// 把含有 Blip 元素的 Drawing 元素删除
        /// </summary>
        /// <param name="blipElement"></param>
        public void DeleteImage(Blip blipElement)
        {
            OpenXmlElement parent = blipElement.Parent;
            while (parent != null && 
                !(parent is DocumentFormat.OpenXml.Wordprocessing.Drawing))
            {
                parent = parent.Parent;
            }

            if (parent != null)
            {
                Drawing drawing = parent as Drawing;
                drawing.Parent.RemoveChild<Drawing>(drawing);

                // 触发  ImagesChanged 事件.
                this.OnImagesChanged();

            }
        }

        /// <summary>
        /// 如果想替换文档中的图片
        /// 1. 添加一个 ImagePart 到文档中
        /// 2. 编辑 Blip 元素使它引用新的 ImagePart.
        /// </summary>
        /// <param name="blipElement"></param>
        /// <param name="newImg"></param>
        public void ReplaceImage(Blip blipElement, FileInfo newImg)
        {
            string rid = AddImagePart(newImg);
            blipElement.Embed.Value = rid;
            this.OnImagesChanged();
        }

        /// <summary>
        /// 添加一个 ImagePart 到文档中
        /// </summary>
        string AddImagePart(FileInfo newImg)
        {
            ImagePartType type = ImagePartType.Bmp ;
            switch(newImg.Extension.ToLower())
            {
                case ".jpeg":
                case ".jpg":
                    type = ImagePartType.Jpeg;
                    break;
                case ".png":
                    type = ImagePartType.Png;
                    break;
                default:
                    type = ImagePartType.Bmp;
                    break;
            }

            ImagePart newImgPart = Document.MainDocumentPart.AddImagePart(type);
            using (FileStream stream = newImg.OpenRead())
            {
                newImgPart.FeedData(stream);
            }

            string rId = Document.MainDocumentPart.GetIdOfPart(newImgPart);
            return rId;
        }


        /// <summary>
        ///  触发 ImagesChanged 事件
        /// </summary>
        protected virtual void OnImagesChanged()
        {
            if (this.ImagesChanged != null)
            {
                this.ImagesChanged(this, EventArgs.Empty);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // 防止被多次调用
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                if (Document != null)
                {
                    Document.Dispose();
                }
                disposed = true;
            }
        }
    }
}
