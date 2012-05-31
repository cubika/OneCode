/****************************** 模块头 ******************************\
* 模块名称:      BHOIEContextMenu.cs
* 项目名称:	    CSBrowserHelperObject
* 版权：Copyright (c) Microsoft Corporation.
* 
* BHOIEContextMenu类是一个运行在IE浏览器并提供额外服务的Browser Helper Object.
* 
* BHO 是一个能够把它自己附加到IE浏览器或资源管理器的任意新实例的动态链接库 (DLL)
* 这样的模块能够通过容器中的地址和浏览器保持联系
* 一般来说，一个地址是放在容器中间的一个中间对象并且都包含对象
* 当这个容器是IE浏览器（或者Windows资源管理器）时，对象
* 需要实现一个简单的轻量级接口IObjectWithSite. 
* 它只提供了 SetSite 和 GetSite两个方法. 
* 
* 这个类用作使IE浏览器的右键菜单失效. 它还能够提供方法把BHO注册到IE浏览器上去
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


using System;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using SHDocVw;
using mshtml;

namespace CSBrowserHelperObject
{
    /// <summary>
    /// 设置类的GUID并制定这个类为 ComVisible.
    /// BHO 必须实现接口 IObjectWithSite. 
    /// </summary>
    [ComVisible(true),
    ClassInterface(ClassInterfaceType.None),
   Guid("C42D40F0-BEBF-418D-8EA1-18D99AC2AB17")]
    public class BHOIEContextMenu : IObjectWithSite
    {
        // 当前的IE实例， 支持IE7或之后的版本, 一个IE标签就是一个IE实例
        // an IE instance.
        private InternetExplorer ieInstance;

        // 注册一个BHO, 需要在这个键下新建一个键.
        private const string BHORegistryKey =
            "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Browser Helper Objects";



        #region Com Register/UnRegister Methods
        /// <summary>
        ///当这个类注册到COM, 给 BHORegistryKey 添加一个新键
        ///让IE能够使用BHO.
        ///在64位机器上, 如果这个程序集的平台和安装器是x86,
        ///32位浏览器可以使用这个BHO.  如果这个程序集的平台和安装器是x64,
        ///64位浏览器可以使用这个BHO.
        /// </summary>
        [ComRegisterFunction]
        public static void RegisterBHO(Type t)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey(BHORegistryKey, true);
            if (key == null)
            {
                key = Registry.LocalMachine.CreateSubKey(BHORegistryKey);
            }

            // 32 个数字用连字符隔开, 包含在大括号内:
            // {00000000-0000-0000-0000-000000000000}
            string bhoKeyStr = t.GUID.ToString("B");
            
            RegistryKey bhoKey = key.OpenSubKey(bhoKeyStr, true);

            // 创建一个新键.
            if (bhoKey == null)
            {
                bhoKey = key.CreateSubKey(bhoKeyStr);
            }

            // 没有浏览器:dword = 1 阻止浏览器加载BHO
            string name = "NoExplorer";
            object value = (object)1;
            bhoKey.SetValue(name, value);
            key.Close();
            bhoKey.Close();
        }

        /// <summary>
        /// 当这个类从COM取消注册，删除这个键
        /// </summary>
        [ComUnregisterFunction]
        public static void UnregisterBHO(Type t)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey(BHORegistryKey, true);
            string guidString = t.GUID.ToString("B");
            if (key != null)
            {
                key.DeleteSubKey(guidString, false);
            }
        }
        #endregion

        #region IObjectWithSite Members
        /// <summary>
        /// 当BHO实例化或消亡时调用这个方法. 地址是实现了 InternetExplorer接口的对象
        /// </summary>
        /// <param name="site"></param>
        public void SetSite(Object site)
        {
            if (site != null)
            {
                ieInstance = (InternetExplorer)site;

                // 注册DocumentComplete 事件.
                ieInstance.DocumentComplete +=
                    new DWebBrowserEvents2_DocumentCompleteEventHandler(
                        ieInstance_DocumentComplete);
            }
        }

        /// <summary>
        /// 通过SetSite()设置从上一个地址获得并返回指定接口
        /// 典型的实现将会向指定接口查询之前存储的pUnkSite指针      
        /// </summary>
        public void GetSite(ref Guid guid, out Object ppvSite)
        {
            IntPtr punk = Marshal.GetIUnknownForObject(ieInstance);
            ppvSite = new object();
            IntPtr ppvSiteIntPtr = Marshal.GetIUnknownForObject(ppvSite);
            int hr = Marshal.QueryInterface(punk, ref guid, out ppvSiteIntPtr);
            Marshal.ThrowExceptionForHR(hr);
            Marshal.Release(punk);
            Marshal.Release(ppvSiteIntPtr);
        }
        #endregion

        #region event handler

        /// <summary>
        /// 处理 DocumentComplete 事件.
        /// </summary>
        /// <param name="pDisp">
        /// pDisp是一个实现了 InternetExplorer接口的实例.
        ///默认的, 这个实例和 ieInstance相同, 但是当页面
        ///包含多个框架时，每一个框架有它们自己的文档
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

            //在IE览器中设置文档的事件解决句柄.
            if (explorer != null)
            {
                SetHandler(explorer);
            }
        }


        void SetHandler(InternetExplorer explorer)
        {
            try
            {

                // 在IE浏览器中注册文档的 oncontextmenu 事件
                HTMLDocumentEventHelper helper =
                    new HTMLDocumentEventHelper(explorer.Document as HTMLDocument);
                helper.oncontextmenu += new HtmlEvent(oncontextmenuHandler);
            }
            catch { }
        }

        /// <summary>
        /// 处理 oncontextmenu 事件.
        /// </summary>
        /// <param name="e"></param>
        void oncontextmenuHandler(IHTMLEventObj e)
        {

            //  取消默认行为, 把事件对象的返回值属性设置为false.
            // object to false.
            e.returnValue = false;

        }

        #endregion




    }
}
