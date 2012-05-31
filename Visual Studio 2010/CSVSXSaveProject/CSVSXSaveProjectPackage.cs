/************************************* 模块头 *********************************\
 * 模块名称:       CSVSXSaveProjectPackage.cs
 * 项目 :           CSVSXSaveProject
 * 版权所有（c）微软公司
 *
 * 这个包主要介绍了如何将菜单增加到集成开发环境中去，它提供以下功能：
 * 1 将整个项目复制到一个新的位置。
 * 2 在当前的集成开发环境中打开新的项目。
 * 
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
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using EnvDTE;
using Microsoft.VisualStudio.Shell;

namespace Microsoft.CSVSXSaveProject
{
    /// <summary>
    /// 该类实现程序集所公开的包。
    ///
    /// 一个类被认为是Visual Studio的有效的包的最低要求是实施了IVsPackage接口，并在shell中注册。
    /// 本软件包使用理软件包框架（MPF）内部定义的辅助类：它来源于从IVsPackage接口所提供的包类。 
    /// 使用在框架中定义的注册属性来定义其本身和她的shell控件。
    /// </summary>
    /// 这个属性告诉PkgDef创建工具（CreatePkgDef.exe），这个类是一个包。
    [PackageRegistration(UseManagedResourcesOnly = true)]
    // 这个属性是用来显示注册所需的信息，这个显示在Visual Studio帮助/关于对话框包中。
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    // 此属性是需要让shell知道这个包提供了哪一些菜单。
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(GuidList.guidCSVSXSaveProjectPkgString)]
    public sealed class CSVSXSaveProjectPackage : Package
    {
        /// <summary>
        /// 指定在此应用中的DTE对象。
        /// </summary>
        DTE dte;
        internal DTE DTEObject
        {
            get
            {
                if (dte == null)
                {
                    dte = this.GetService(typeof(DTE)) as DTE;
                }
                return dte;
            }
        }

        /// <summary>
        /// 为包提供默认的构造器。
        /// 在此方法中，你可以在不需要任何Visual Studio服务的情况下，放置任何的初始化代码。
        /// 因为在时，包的对象被创建，但还没有在Visual Studio环境中获得地址。所有初始化的地方位于Initialize方法。
        /// </summary>
        public CSVSXSaveProjectPackage()
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture,
                "Entering constructor for: {0}", this.ToString()));
        }

        /////////////////////////////////////////////////////////////////////////////////
        // 重写包的实现
        #region 包成员

        /// <summary>
        /// 包的初始化，这个方法在包被设置好后调用, 所以这就是一个你可以放置所有的依赖、
        /// VisualStudio提供的服务的初始化代码的地方。
        /// </summary>
        protected override void Initialize()
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture,
                "Entering Initialize() of: {0}", this.ToString()));
            base.Initialize();

            // 为我们的菜单添加命令处理程序（命令中必须存在的.vsct文件）
            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService))
                as OleMenuCommandService;
            if (null != mcs)
            {
                // 创建菜单项命令。
                CommandID menuCommandID =
                    new CommandID(GuidList.guidCSVSXSaveProjectCmdSet,
                        (int)PkgCmdIDList.cmdidCSVSXSaveProjectCommandID);
                MenuCommand menuItem =
                    new MenuCommand(MenuItemCallback, menuCommandID);
                mcs.AddCommand(menuItem);

                // 创建VSXSaveProjectCmdSet的菜单项命令。
                CommandID cSVSXSaveProjectContextCommandID =
                    new CommandID(GuidList.guidCSVSXSaveProjectContextCmdSet,
                        (int)PkgCmdIDList.cmdidCSVSXSaveProjectContextCommandID);
                OleMenuCommand cSVSXSaveProjectMenuContextCommand =
                    new OleMenuCommand(MenuItemCallback,
                        cSVSXSaveProjectContextCommandID);
                mcs.AddCommand(cSVSXSaveProjectMenuContextCommand);
            }
        }
        #endregion

        /// <summary>
        /// 此功能是用于执行命令菜单项被点击时的回调。请参阅Initialize方法，
        /// 看到菜单项是如何关联到OleMenuCommandService服务和MenuCommand类使用该功能。
        /// </summary>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            try
            {
                // 获取当前活动项目的对象。
                var proj = this.GetActiveProject();

                if (proj != null)
                {
                    // 获取项目的信息
                    var vsProj = new Files.VSProject(proj);

                    // 获取在项目中包含的文件
                    var includedFiles = vsProj.GetIncludedFiles();

                    // 获取在项目文件夹下面的文件
                    var projfolderFiles =
                        Files.ProjectFolder.GetFilesInProjectFolder(proj.FullName);

                    // 新增的其他文件，如项目文件夹下的文件，使用户可以选择他们。
                    var totalItems = new List<Files.ProjectFileItem>(includedFiles);
                    foreach (Files.ProjectFileItem fileItem in projfolderFiles)
                    {
                        if (includedFiles.Count(f => f.FullName.Equals(fileItem.FullName,
                            StringComparison.OrdinalIgnoreCase)) == 0)
                        {
                            totalItems.Add(fileItem);
                        }
                    }

                    // 显示用户接口
                    using (SaveProjectDialog dialog = new SaveProjectDialog())
                    {
                        // 显示所有文件
                        dialog.FilesItems = totalItems;
                        dialog.OriginalFolderPath = vsProj.ProjectFolder.FullName;

                        var result = dialog.ShowDialog();

                        // 打开新的项目
                        if (result == DialogResult.OK && dialog.OpenNewProject)
                        {
                            string newProjectPath = string.Format("{0}\\{1}",
                                dialog.NewFolderPath,
                                proj.FullName.Substring(vsProj.ProjectFolder.FullName.Length));

                            string cmd = string.Format("File.OpenProject \"{0}\"", newProjectPath);

                            this.DTEObject.ExecuteCommand(cmd);                      
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 获取活动项目的对象。
        /// </summary>
        internal Project GetActiveProject()
        {
            Project activeProject = null;

            // 在解决方案资源管理器获取所有项目。
            Array activeSolutionProjects =
                 this.DTEObject.ActiveSolutionProjects as Array;
            if (activeSolutionProjects != null && activeSolutionProjects.Length > 0)
            {
                // 获取活动项目。
                activeProject = activeSolutionProjects.GetValue(0) as Project;
            }
            return activeProject;
        }
    }
}

