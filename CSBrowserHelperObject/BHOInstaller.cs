/****************************** 模块头 ******************************\
*  模块名称:     BHOInstaller.cs
*  项目名称:	    CSBrowserHelperObject
*  版权：Copyright (c) Microsoft Corporation.
*  
*  BHOInstaller类 继承了 System.Configuration.Install.Installer类.
*  Install 和 Uninstall方法将会在这个应用程序安装或卸载时运行。
*  
*  这个操作必须在添加了installer自定义操作后才会生效
*  
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

namespace CSBrowserHelperObject
{
    [RunInstaller(true), ComVisible(false)]
    public partial class BHOInstaller : Installer
    {
        public BHOInstaller()
        {         
          InitializeComponent();         
        }

        /// <summary>
        ///  将在installer的自定义操作执行时被调用
        ///  注册一个工具栏扩展一个COM服务标签对象.
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
        ///将在installer的自定义操作执行时被调用
        ///注册一个工具栏扩展一个COM服务标签对象.
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
