/****************************** 模块头 ******************************\
* 模块名称:  IEExplorerBarInstaller.cs
* 项目:	    CSIEExplorerBar
* Copyright (c) Microsoft Corporation.
* 
* IEExplorerBarInstaller类继承了类System.Configuration.Install.Installer。Install和Uninstall
* 方法会在这个应用程序安装或卸载时运行。
* 
* 这个操作要添加到安装程序的自定义操作中才能生效。
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System.ComponentModel;
using System.Configuration.Install;
using System.Runtime.InteropServices;


namespace CSIEExplorerBar
{
    [RunInstaller(true)]
    public partial class IEExplorerBarInstaller : Installer
    {
        public IEExplorerBarInstaller()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 当安装程序的自定义操作执行和注册浏览器栏作为COM服务器时，这个方法被调用。
        /// </summary>
        /// <param name="stateSaver"></param>     
        public override void Install(System.Collections.IDictionary stateSaver)
        {
            base.Install(stateSaver);

            RegistrationServices regsrv = new RegistrationServices();
            if (!regsrv.RegisterAssembly(this.GetType().Assembly,
            AssemblyRegistrationFlags.SetCodeBase))
            {
                throw new InstallException("Failed To Register for COM");
            }
        }

        /// <summary>
        /// 当安装程序的自定义操作执行和注销浏览器栏时，这个方法被调用。
        /// </summary>
        /// <param name="stateSaver"></param>     
        public override void Uninstall(System.Collections.IDictionary savedState)
        {
            base.Uninstall(savedState);

            RegistrationServices regsrv = new RegistrationServices();
            if (!regsrv.UnregisterAssembly(this.GetType().Assembly))
            {
                throw new InstallException("Failed To Unregister for COM");
            }
        }
    }
}
