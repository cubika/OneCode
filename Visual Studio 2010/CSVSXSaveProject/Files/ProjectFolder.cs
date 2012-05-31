/************************************* Module Header *********************************\
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

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.CSVSXSaveProject.Files
{

    public static class ProjectFolder
    { 
        /// <summary>
        /// 获取项目文件夹中的文件
        /// </summary>
        /// <param name="projectFilePath">
        /// 项目文件路径 
        /// </param>
        public static List<ProjectFileItem> GetFilesInProjectFolder(string projectFilePath)
        {
            //  获取包括项目文件的文件夹
            FileInfo projFile = new FileInfo(projectFilePath);
            DirectoryInfo projFolder = projFile.Directory;

            if (projFolder.Exists)
            {
                // 获取项目文件夹中的所有文件的信息
                var files = projFolder.GetFiles("*", SearchOption.AllDirectories);

                return files.Select(f => new ProjectFileItem { Fileinfo = f,
                    IsUnderProjectFolder = true }).ToList();
            }
            else
            {
                //该项目文件夹不存在
                return null;
            }
        }

    }
}



