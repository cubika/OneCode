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

using System;
using System.Collections.Generic;
using System.IO;
using EnvDTE;

namespace Microsoft.CSVSXSaveProject.Files
{

    public class VSProject
    {
        /// <summary>
        ///一个项目的解决方案
        /// </summary>
        public Project Project { get; private set; }

        /// <summary>
        /// 该文件夹包含的项目文件
        /// </summary>
        public DirectoryInfo ProjectFolder{get; private set;}

        /// <summary>
        /// 初始化的项目和项目文件夹属性
        /// </summary>
        public VSProject(Project proj)
        {
            //  初始化的项目对象。
            this.Project = proj;

            //  获取当前的项目目录
            this.ProjectFolder = new FileInfo(Project.FullName).Directory;
        }

        /// <summary>
        /// 获取包含在项目中的所有文件
        /// </summary>
        public List<ProjectFileItem> GetIncludedFiles()
        {
            var files = new List<ProjectFileItem>();

            // 向文件列表中增加项目文件(*.csproj 或者 *.vbproj...)
            files.Add(new ProjectFileItem
            {
                Fileinfo = new FileInfo(Project.FullName),
                NeedCopy = true,
                IsUnderProjectFolder = true
            });

            // 增加项目中包含的文件
            foreach (ProjectItem item in Project.ProjectItems)
            {
                GetProjectItem(item, files);
            }

            return files;
        }

        /// <summary>
        /// 获取包含在项目中的所有文件
        /// </summary>
        void GetProjectItem(ProjectItem item, List<ProjectFileItem> files)
        {
            // 获取与一个项目项相关的文件。
            // 大部分的工程项目包括只有一个文件，但有些的可能不止一个，
            // 在Visual Basic中有两种文件形式保存.FRM（文本)和.frx（二进制）文件。
            // 敬请查看 http://msdn.microsoft.com/en-us/library/envdte.projectitem.filecount.aspx
            for (short i = 0; i < item.FileCount; i++)
            {
                if (File.Exists(item.FileNames[i]))
                {
                    ProjectFileItem fileItem = new ProjectFileItem();

                    fileItem.Fileinfo = new FileInfo(item.FileNames[i]);

                    if (fileItem.FullName.StartsWith(this.ProjectFolder.FullName,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        fileItem.IsUnderProjectFolder = true;
                        fileItem.NeedCopy = true;
                    }

                    files.Add(fileItem);
                }
            }

            // 获取此节点下的子节点的文件。
            foreach (ProjectItem subItem in item.ProjectItems)
            {
                GetProjectItem(subItem, files);
            }
        }      
    }
}
