/*************************************** 模块头*****************************\
* 模块名:  BHOInstaller.cs
* 项目名:  CSCustomIEContextMenu
* 版权 (c) Microsoft Corporation.
* 
* 类CustomIEContextMenuInstaller继承自类System.Configuration.Install.Installer.
* 当该应用程序被安装或卸载时，类的安装和卸载方法将会运行.
* 
* 该操作需要添加到安装程序的自定义操作中才能生效.
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

namespace CSCustomIEContextMenu
{
    [RunInstaller(true), ComVisible(false)]
    public partial class CustomIEContextMenuInstaller : System.Configuration.Install.Installer
    {
        public CustomIEContextMenuInstaller()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 当安装程序的自定义行为执行并作为 COM 服务器注册到扩展 bandoject 的工
        /// 具栏时,该方法被调用.
        /// </summary>
        /// <param name="stateSaver"></param>     
        public override void Install(System.Collections.IDictionary stateSaver)
        {
            base.Install(stateSaver);

            OpenImageMenuExt.RegisterMenuExt();

            RegistrationServices regsrv = new RegistrationServices();
            if (!regsrv.RegisterAssembly(this.GetType().Assembly,
            AssemblyRegistrationFlags.SetCodeBase))
            {
                throw new InstallException("注册COM失败");
            }
        }

        /// <summary>
        /// 当卸载程序的自定义操作执行时，该方法被调用，并注销作为 COM 服务器扩展
        /// bandobject的 工具栏.
        /// </summary>
        /// <param name="stateSaver"></param>     
        public override void Uninstall(System.Collections.IDictionary savedState)
        {
            base.Uninstall(savedState);

            OpenImageMenuExt.UnRegisterMenuExt();

            RegistrationServices regsrv = new RegistrationServices();
            if (!regsrv.UnregisterAssembly(this.GetType().Assembly))
            {
                throw new InstallException("注销COM失败");
            }
        }
    }
}
