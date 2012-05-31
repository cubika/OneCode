/****************************** 模块头 ******************************\
* 模块名:  TabbedWebBrowserContainer.cs
* 项目名:	    CSTabbedWebBrowser
* 版权 (c) Microsoft Corporation.
* 
* 这是一个UserControl并且包含一个 System.Windows.Forms.TabControl。
* 在用户界面，TabControl 不支持创建/关闭一个标签。
* 所以UserControl 提供一个方法去创建/关闭一个标签。 
* 
* 向Tabcontrol中添加一个新的TabPage时，WebBrowserTabPage类型继承System.Windows.Forms.TabPage，
* 并且UerControl会显示出System.Windows.Forms.WebBrowser类中的 向前，后退和刷新方法。
* 在Internet Explorer中NewWindow 事件被触发时，它将会创建一个标签并打开标签。
* 
* 在WebBorwser中“在新建选项卡中打开”默认状态下是被禁用的。
* 你可以向键值 HKCU\Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_TABBED_BROWSING中，
* 添加一个*.exe=1(*表示该进程的名字)
*
* 当应用程序重新启动后，这个菜单才会生效。
* 参见： http://msdn.microsoft.com/en-us/library/ms537636(VS.85).aspx
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
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Security.Permissions;


namespace CSTabbedWebBrowser
{
    public partial class TabbedWebBrowserContainer : UserControl
    {
        /// <summary>
        /// 一个静态属性来获取或设置WebBrowser中的“在新建选项卡中打开”的下拉菜单是否启用。
        /// </summary>
        public static bool IsTabEnabled
        {
            [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
            get
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(
                    @"Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_TABBED_BROWSING"))
                {
                    if (key != null)
                    {
                        string processName = Process.GetCurrentProcess().ProcessName + ".exe";
                        int keyValue = (int)key.GetValue(processName, 0);
                        return keyValue == 1;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
            set
            {
                using (RegistryKey key = Registry.CurrentUser.CreateSubKey(
                       @"Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_TABBED_BROWSING"))
                {
                    string processName = Process.GetCurrentProcess().ProcessName + ".exe";
                    int keyValue = (int)key.GetValue(processName, 0);

                    bool isEnabled = keyValue == 1;
                    if (isEnabled != value)
                    {
                        key.SetValue(processName, value ? 1 : 0);
                    }
                }
            }
        }

        /// <summary>
        /// 标签控件的选择标签。
        /// </summary>
        public WebBrowserTabPage ActiveTab
        {
            get
            {
                if (tabControl.SelectedTab != null)
                {
                    return tabControl.SelectedTab as WebBrowserTabPage;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 此控件至少含有一个标签。
        /// </summary>
        public bool CanCloseActivePage
        {
            get
            {
                return tabControl.TabPages.Count > 1;
            }
        }


        public TabbedWebBrowserContainer()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 创建一个新的标签，当空间载入时，导航栏设置为“about:blank”。
        /// </summary>
        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            NewTab("about:blank");
        }

        /// <summary>
        /// 操作ACTIVETAB下的WEB浏览器控制这个URL。
        /// </summary>
        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public void Navigate(string url)
        {
            if (this.ActiveTab != null)
            {
                this.ActiveTab.WebBrowser.Navigate(url);
            }
        }

        /// <summary>
        /// 创建一个新的 WebBrowserTabPage 实例，把它添加到标签控件中，然后签署它的NewWindow事件。
        /// </summary>
        /// <returns></returns>
        WebBrowserTabPage CreateTabPage()
        {
            WebBrowserTabPage tab = new WebBrowserTabPage();
            tab.NewWindow += new EventHandler<WebBrowserNewWindowEventArgs>(tab_NewWindow);
            this.tabControl.TabPages.Add(tab);
            this.tabControl.SelectedTab = tab;
            return tab;
        }

        /// <summary>
        /// 创建一个WebBrowserTabPage，然后进入到URL中。
        /// </summary>
        /// <param name="url"></param>
        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public void NewTab(string url)
        {
            CreateTabPage();
            Navigate(url);
        }

        /// <summary>
        /// 关闭激活的标签.
        /// </summary>
        public void CloseActiveTab()
        {
            // 此控件至少含有一个标签.
            if (CanCloseActivePage)
            {
                var tabToClose = this.ActiveTab;
                this.tabControl.TabPages.Remove(tabToClose);
            }
        }

        /// <summary>
        /// 处理WebBrowserTabPage的NewWindow 事件。
        /// 当WebBrowser中 NewWindow 事件被触发时，在nternet Explorer中药创建一个标签并且打开主页. 
        /// </summary>
        void tab_NewWindow(object sender, WebBrowserNewWindowEventArgs e)
        {
            if (TabbedWebBrowserContainer.IsTabEnabled)
            {
                NewTab(e.Url);
                e.Cancel = true;
            }
        }

        /// <summary>
        /// 在ActiveTab中显示WebBrowser 控件的后退方法
        /// </summary>
        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public void GoBack()
        {
            this.ActiveTab.WebBrowser.GoBack();
        }

        /// <summary>
        /// 在ActiveTab中显示WebBrowser 控件的向前方法。
        /// </summary>
        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public void GoForward()
        {
            this.ActiveTab.WebBrowser.GoForward();
        }

        /// <summary>
        /// 在ActiveTab中显示WebBrowser 控件的刷新方法。
        /// </summary>
        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public void RefreshWebBrowser()
        {
            this.ActiveTab.WebBrowser.Refresh();
        }

       
    }
}
