/************************************* 模块头 *********************************\
 * 模块名称:         ProjectFileItem.cs
 * 项目 :            CSVSXSaveProject
 * 版权所有（c）微软公司
 * 
 * 获取项目文件的信息，其中包括在本项目中的文件夹标志和用来设置选定复制文件的选项。
 * 
 * The source is subject to the Microsoft Public License.
 * See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
 * All other rights reserved
 * 
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER 
 * EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF 
 * MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*************************************************************************************/

using System.IO;

namespace Microsoft.CSVSXSaveProject.Files
{
    public class ProjectFileItem
    {
        /// <summary>
        /// 文件的信息
        /// </summary>
        public FileInfo Fileinfo { get; set; }

        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName 
        {
            get
            {
                return Fileinfo.Name;
            }
        }

        /// <summary>
        /// 文件的路径
        /// </summary>
        public string FullName
        {
            get
            {
                return Fileinfo.FullName;
            }
        }

        /// <summary>
        /// 指定的文件是否是在项目文件夹
        /// </summary>
        public bool IsUnderProjectFolder { get; set; }

        /// <summary>
        /// 指定文件是否应该被复制
        /// </summary>
        public bool NeedCopy { get; set; }
    }
}
