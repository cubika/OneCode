/********************************* 模块头 *********************************\
* 模块名: IsolatedStorageHelper.cs
* 项目名: VideoStoryCreator
* 版权(c) Microsoft Corporation.
* 
* 一个关于 isolated storage I/O操作的helper类.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System.IO;
using System.IO.IsolatedStorage;

namespace VideoStoryCreator
{
    public class IsolatedStorageHelper
    {
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="name">被删除图片的名称</param>
        public static void DeleteFile(string name)
        {
            IsolatedStorageFile userStore = IsolatedStorageFile.GetUserStoreForApplication();
            if (userStore.FileExists(name))
            {
                userStore.DeleteFile(name);
            }
        }

        /// <summary>
        /// 文件重命名.
        /// 首先建立一个新的文件并且从源文件拷贝内容过来.
        /// 然后删除原始文件.
        /// </summary>
        public static void RenameFile(string originalName, string newName)
        {
            IsolatedStorageFile userStore = IsolatedStorageFile.GetUserStoreForApplication();
            if (userStore.FileExists(originalName))
            {
                using (FileStream originalFileStream = userStore.OpenFile(originalName, System.IO.FileMode.Open))
                {
                    using (FileStream newFileStream = userStore.CreateFile(newName))
                    {
                        byte[] buffer = new byte[originalFileStream.Length];
                        originalFileStream.Read(buffer, 0, buffer.Length);
                        newFileStream.Write(buffer, 0, buffer.Length);
                    }
                }
                userStore.DeleteFile(originalName);
            }
        }

        /// <summary>
        /// 检查文件是否存在.
        /// </summary>
        public static bool FileExists(string name)
        {
            IsolatedStorageFile userStore = IsolatedStorageFile.GetUserStoreForApplication();
            return userStore.FileExists(name);
        }
    }
}
