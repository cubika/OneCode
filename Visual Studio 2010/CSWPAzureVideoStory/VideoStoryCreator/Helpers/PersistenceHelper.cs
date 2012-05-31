/********************************* 模块头 *********************************\
* 模块名: PersistenceHelper.cs
* 项目名: VideoStoryCreator
* 版权(c) Microsoft Corporation.
* 
* 序列化和反序列化story到xml的helper类.
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
using System.IO.IsolatedStorage;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Xna.Framework.Media;
using VideoStoryCreator.Models;
using VideoStoryCreator.Transitions;

namespace VideoStoryCreator
{
    public class PersistenceHelper
    {
        /// <summary>
        /// 序列化story到xml, 存储在isolated storage.
        /// </summary>
        internal static void StoreData()
        {
            if (!string.IsNullOrEmpty(App.CurrentStoryName))
            {
                IsolatedStorageFile userStore = IsolatedStorageFile.GetUserStoreForApplication();
                using (IsolatedStorageFileStream fileStream = userStore.CreateFile(App.CurrentStoryName + ".xml"))
                {
                    // 序列化tory, 并且保存.
                    SerializeStory().Save(fileStream);
                }

                // 保存当前story名称.
                using (IsolatedStorageFileStream fileStream = 
                    userStore.OpenFile("CurrentStory.txt", System.IO.FileMode.OpenOrCreate))
                {
                    using (StreamWriter writer = new StreamWriter(fileStream))
                    {
                        writer.Write(App.CurrentStoryName);
                    }
                }
            }
        }

        /// <summary>
        /// 从isolated storage读取xml文件, 反序列化为story.
        /// </summary>
        internal static void RestoreData()
        {
            IsolatedStorageFile userStore = IsolatedStorageFile.GetUserStoreForApplication();

            // 读取当前story名称.
            if (userStore.FileExists("CurrentStory.txt"))
            {
                using (IsolatedStorageFileStream fileStream = userStore.OpenFile("CurrentStory.txt", System.IO.FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        App.CurrentStoryName = reader.ReadToEnd();
                    }
                }
            }

            // 如果当前story被找到, 加载它.
            if (!string.IsNullOrEmpty(App.CurrentStoryName) && userStore.FileExists(App.CurrentStoryName + ".xml"))
            {
                ReadStoryFile(App.CurrentStoryName, userStore);
            }
        }

        /// <summary>
        /// 读取指定的story.
        /// </summary>
        /// <param name="storyName">story名称.</param>
        /// <param name="userStore">如果参数为null, 创建一个新的.</param>
        internal static void ReadStoryFile(string storyName, IsolatedStorageFile userStore = null)
        {
            if (userStore == null)
            {
                userStore = IsolatedStorageFile.GetUserStoreForApplication();
            }
            using (IsolatedStorageFileStream fileStream = userStore.OpenFile(storyName + ".xml", System.IO.FileMode.Open))
            {
                XDocument xdoc = XDocument.Load(fileStream);
                var picturesLibrary = new MediaLibrary().Pictures;

                // Load all photos.
                foreach (XElement photoElement in xdoc.Root.Elements())
                {
                    try
                    {
                        Photo photo = new Photo()
                        {
                            Name = photoElement.Attribute("Name").Value,
                        };
                        string photoDurationString = photoElement.Attribute("PhotoDuration").Value;
                        int photoDuration = int.Parse(photoDurationString);
                        photo.PhotoDuration = TimeSpan.FromSeconds(photoDuration);
                        XElement transitionElement = photoElement.Element("Transition");
                        if (transitionElement != null)
                        {
                            photo.Transition = TransitionBase.Load(photoElement.Element("Transition"));
                        }
                        Picture picture = picturesLibrary.Where(p => p.Name == photo.Name).FirstOrDefault();
                        if (picture == null)
                        {
                            // 如果找不到原文件，可能已经被删除了
                            // TODO: 我们需要记录错误吗? 我们是继续下一个图片还是抛出异常?
                            continue;
                        }
                        photo.ThumbnailStream = picture.GetThumbnail();
                        App.MediaCollection.Add(photo);
                    }
                    catch
                    {
                        // TODO: 我们需要记录错误吗? 我们是继续下一个图片还是抛出异常?
                        continue;
                    }
                }
            }
        }

        /// <summary>
        ///  序列化当前story.
        ///  我们只序列化story数据, 比如每个photo的duration.
        ///  我们不序列化下面的bitmap.
        /// </summary>
        /// <returns>当前story的XDocument对象</returns>
        internal static XDocument SerializeStory()
        {
            XDocument xdoc = new XDocument(new XElement("Story",
                new XAttribute("Name", App.CurrentStoryName),
                new XAttribute("PhotoCount", App.MediaCollection.Count)
                ));

            // 把每张图片保存为xml元素.
            foreach (Photo photo in App.MediaCollection)
            {
                XElement photoElement = new XElement("Photo");
                photoElement.Add(new XAttribute("Name", photo.Name));
                photoElement.Add(new XAttribute("PhotoDuration", photo.PhotoDuration.TotalSeconds));
                if (photo.Transition != null)
                {
                    XElement transitionElement = new XElement("Transition");
                    photo.Transition.Save(transitionElement);
                    photoElement.Add(transitionElement);
                }
                xdoc.Root.Add(photoElement);
            }
            return xdoc;
        }

        /// <summary>
        /// 列举所有保存了的story, 并且返回story名称.
        /// </summary>
        /// <returns>story列表, 没有.xml扩展名.</returns>
        internal static List<string> EnumerateStories()
        {
            IsolatedStorageFile userStore = IsolatedStorageFile.GetUserStoreForApplication();
            return (from f in userStore.GetFileNames()
                    where f.EndsWith(".xml")
                    select f.Substring(0, f.Length - 4)).ToList();
        }
    }
}
