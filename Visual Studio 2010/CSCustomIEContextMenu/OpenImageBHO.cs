/*************************************** 模块头*****************************\
* 模块名:  OpenImageBHO.cs
* 项目名:  CSCustomIEContextMenu
* 版权 (c) Microsoft Corporation.
* 
* 类 OpenImageBHO 是一个运行在IE内，提供额外服务的浏览器辅助对象(BHO).
* 
* 一个 BHO 是一个动态链接库 (DLL),它能够将自身附加到任何 IE 浏览器或 Windows 
* 资源管理器的新实例中.这种模块可以通过容器的站点与浏览器取得联系.一般情况下，
* 一个站点是放在容器和所包含的每个对象中间的一个中间对象.当容器是 IE 浏览器 
* （或 Windows 资源管理器） 时，则需要该对象实现一个更简单、 更轻的、称为 
* IObjectWithSite的接口.它仅仅提供了 SetSite 和 GetSite 两个方法.
* 
* 此类用于设置 HtmlDocument 的 IDocHostUIHandler.
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
using System.Runtime.InteropServices;
using CSCustomIEContextMenu.NativeMethods;
using Microsoft.Win32;
using SHDocVw;

namespace CSCustomIEContextMenu
{
    /// <summary>
    /// 设置该类的 GUID,指定该类为 ComVisible.
    /// 一个 BHO 必须实现接口 IObjectWithSite.
    /// </summary>
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    [Guid("AA0B1334-E7F5-4F75-A1DE-0993098AAF01")]
    public class OpenImageBHO : IObjectWithSite, IDisposable
    {
        private bool disposed = false;

        // 为了注册一个 BHO，应当在此键下创建一个新键.
        private const string BHORegistryKey =
            "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Browser Helper Objects";

        // 当前的 IE 实例. 对于 IE7 或更高版本,一个 IE 选项卡是只是一个
        // IE 的实例.
        private InternetExplorer ieInstance;

        private OpenImageHandler openImageDocHostUIHandler;


        #region Com 注册/注销 方法
        /// <summary>
        /// 当这个类被注册到 COM 时，添加一个新键到 BHORegistryKey 以便让 IE 可以使用
        /// 这个 BHO.
        /// 在 64 位机器上，如果该程序集的平台和安装程序是 x86,32 位的 IE 可以使用这
        /// 个 BHO.如果该程序集的平台和安装程序是 x64,64 位的 IE 可以使用这个 BHO.
        /// </summary>
        [ComRegisterFunction]
        public static void RegisterBHO(Type t)
        {
            // 如果该键存在， CreateSubKey 将会打开它.
            RegistryKey bhosKey = Registry.LocalMachine.CreateSubKey(
                BHORegistryKey,
                RegistryKeyPermissionCheck.ReadWriteSubTree);

            // 括在大括号中,用连字符分隔的 32 位: 
            // {00000000-0000-0000-0000-000000000000}
            string bhoKeyStr = t.GUID.ToString("B");

            // 创建一个新键.
            RegistryKey bhoKey = bhosKey.CreateSubKey(bhoKeyStr);

            // NoExplorer:dword = 1 阻止浏览器加载这个 BHO.
            bhoKey.SetValue("NoExplorer", 1);
            bhosKey.Close();
            bhoKey.Close();
        }

        /// <summary>
        /// 从 COM 注销该类时,删除该键.
        /// </summary>
        [ComUnregisterFunction]
        public static void UnregisterBHO(Type t)
        {
            RegistryKey bhosKey = Registry.LocalMachine.OpenSubKey(BHORegistryKey, true);
            string guidString = t.GUID.ToString("B");
            if (bhosKey != null)
            {
                bhosKey.DeleteSubKey(guidString, false);
            }

            bhosKey.Close();
        }


        #endregion

        #region IObjectWithSite 成员
        /// <summary>
        /// 当实例化或者销毁 BHO 时，调用该方法. 这个站点是一个
        /// 实现了 InternetExplorer 接口的对象.
        /// </summary>
        /// <param name="site"></param>
        public void SetSite(Object site)
        {
            if (site != null)
            {
                ieInstance = (InternetExplorer)site;

                openImageDocHostUIHandler = new OpenImageHandler(ieInstance);

                // 登记 DocumentComplete 事件.
                ieInstance.DocumentComplete +=
                    new DWebBrowserEvents2_DocumentCompleteEventHandler(
                        ieInstance_DocumentComplete);
            }
        }

        /// <summary>
        /// 通过设置SetSite()从最后一个站点检索并返回指定的接口.典型的实现将
        /// 会为指定的接口查询以先前存储的 pUnkSite 指针.
        /// </summary>
        public void GetSite(ref Guid guid, out Object ppvSite)
        {
            IntPtr punk = Marshal.GetIUnknownForObject(ieInstance);
            ppvSite = new object();
            IntPtr ppvSiteIntPtr = Marshal.GetIUnknownForObject(ppvSite);
            int hr = Marshal.QueryInterface(punk, ref guid, out ppvSiteIntPtr);
            Marshal.ThrowExceptionForHR(hr);
            Marshal.Release(punk);
        }
        #endregion

        #region 事件处理

        /// <summary>
        /// 处理 DocumentComplete 事件.
        /// </summary>
        /// <param name="pDisp">
        /// pDisp 是一个实现了 InternetExplorer 接口的对象. 
        /// 默认情况下,该对象与 ieInstance 相同, 但是如果页面包含多个框架,每一个框
        /// 架会有自己的文件.
        /// </param>
        void ieInstance_DocumentComplete(object pDisp, ref object URL)
        {
            string url = URL as string;

            if (string.IsNullOrEmpty(url)
                || url.Equals("about:blank", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            InternetExplorer explorer = pDisp as InternetExplorer;

            // 设置 InternetExplorer 内文件的句柄.
            if (explorer != null)
            {
                NativeMethods.ICustomDoc customDoc = (NativeMethods.ICustomDoc)explorer.Document;
                customDoc.SetUIHandler(openImageDocHostUIHandler);
            }
        }
        #endregion

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            // 防止被多次调用.
            if (disposed) return;

            if (disposing)
            {
                // 清理所有托管资源.
                if (openImageDocHostUIHandler != null)
                {
                    openImageDocHostUIHandler.Dispose();
                }
            }
            disposed = true;
        }
    }
}
