/****************************** Module Header ******************************\
* 模块名称:  ImageListExplorerBar.cs
* 项目:	    CSIEExplorerBar
* Copyright (c) Microsoft Corporation.
* 
* ImageListExplorerBar类继承了类System.Windows.Forms.UserControl，并实现了
* IObjectWithSite、IDeskBand、IDockingWindow、IOleWindow和IInputObject接口.
* 
* 想要在IE中添加一个浏览器栏目，你需要做以下几步：
* 
* 1. 为ComVisible 类创建一个有效的GUID。
* 
* 2. 实现IObjectWithSite、IDeskBand、IDockingWindow、IOleWindow和IInputObject接口。
*    
* 3. 将这个程序集注册到COM中。
* 
* 4.  创建一个新的密钥，并用你创建的Explorer Bar的类型的类别标识符 (CATID)作为这个密钥的名称
* 可能会是以下值中的一个： 
*    {00021494-0000-0000-C000-000000000046} 定义一个横向的浏览器栏。 
*    {00021493-0000-0000-C000-000000000046} 定义一个垂直的浏览器栏。
*    
*    结果类似于：
*
*    HKEY_CLASSES_ROOT\CLSID\<Your GUID>\Implemented Categories\{00021494-0000-0000-C000-000000000046}
*    或  
*    HKEY_CLASSES_ROOT\CLSID\<Your GUID>\Implemented Categories\{00021493-0000-0000-C000-000000000046}
*    
* 5. 删除以下的注册表项，因为这些注册表项用于缓存 ExplorerBar 的枚举。
* 
*    HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Discardable\PostSetup\
*    Component Categories\{00021493-0000-0000-C000-000000000046}\Enum
*    或
*    HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Discardable\PostSetup\
*    Component Categories\{00021494-0000-0000-C000-000000000046}\Enum
*
*
* 6. 在注册表中设置浏览器栏的大小。
* 
*    HKEY_LOCAL_MACHINE\Software\Microsoft\Internet Explorer\Explorer Bars\<Your GUID>\BarSize
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
using System.Windows.Forms;
using Microsoft.Win32;
using SHDocVw;

namespace CSIEExplorerBar
{
    [ComVisible(true)]
    [Guid("5802D092-1784-4908-8CDB-99B6842D353F")]
    public partial class ImageListExplorerBar :
        UserControl,
        NativeMethods.IObjectWithSite,
        NativeMethods.IDeskBand,
        NativeMethods.IDockingWindow,
        NativeMethods.IOleWindow,
        NativeMethods.IInputObject
    {

        // 浏览器栏的标题。
        private const string imageListExplorerBarTitle =
            "图片列举浏览器栏";

        // 定义一个垂直的浏览器栏。 
        private const string verticalExplorerBarCATID =
            "{00021493-0000-0000-C000-000000000046}";

        // IInputObjectSite对象。
        private NativeMethods.IInputObjectSite site;

        // 浏览器栏实例。
        private InternetExplorer explorer;

        public ImageListExplorerBar()
        {
            InitializeComponent();
        }

        #region NativeMethods.IObjectWithSite

        public void SetSite(object pUnkSite)
        {

            // 释放之前的COM对象。
            if (this.site != null)
            {
                Marshal.ReleaseComObject(this.site);
            }
            if (this.explorer != null)
            {
                Marshal.ReleaseComObject(this.explorer);
                this.explorer = null;
            }

            // pUnkSite是一个实现IOleWindowSite对象的指针。
            this.site = pUnkSite as NativeMethods.IInputObjectSite;

            if (this.site != null)
            {

                // site实现了IServiceProvider接口并可以被用来获取InternetExplorer实例。
                var provider = this.site as NativeMethods._IServiceProvider;
                Guid guid = new Guid("{0002DF05-0000-0000-C000-000000000046}");
                Guid riid = new Guid("{00000000-0000-0000-C000-000000000046}");
                try
                {
                    object webBrowser;
                    provider.QueryService(ref guid, ref riid, out webBrowser);
                    this.explorer = webBrowser as InternetExplorer;
                }
                catch (COMException)
                {
                }
            }

        }


        public void GetSite(ref Guid riid, out object ppvSite)
        {
            ppvSite = this.site;
        }

        #endregion

        #region NativeMethods.IDeskBand

        /// <summary>
        /// 获取一个band对象的信息。
        /// </summary>
        public void GetBandInfo(uint dwBandID, uint dwViewMode,
            ref NativeMethods.DESKBANDINFO pdbi)
        {
            pdbi.wszTitle = ImageListExplorerBar.imageListExplorerBarTitle;
            pdbi.ptActual.X = base.Size.Width;
            pdbi.ptActual.Y = base.Size.Height;
            pdbi.ptMaxSize.X = -1;
            pdbi.ptMaxSize.Y = -1;
            pdbi.ptMinSize.X = -1;
            pdbi.ptMinSize.Y = -1;
            pdbi.ptIntegral.X = -1;
            pdbi.ptIntegral.Y = -1;
            pdbi.dwModeFlags = NativeMethods.DBIM.NORMAL 
                | NativeMethods.DBIM.VARIABLEHEIGHT;
        }

        public void ShowDW(bool fShow)
        {
            if (fShow)
                this.Show();
            else
                this.Hide();
        }

        public void CloseDW(uint dwReserved)
        {
            this.Dispose(true);
        }

        public void ResizeBorderDW(IntPtr prcBorder, object punkToolbarSite, bool fReserved)
        {
        }

        public void GetWindow(out IntPtr hwnd)
        {
            hwnd = this.Handle;
        }

        public void ContextSensitiveHelp(bool fEnterMode)
        {
        }

        #endregion


        #region NativeMethods.IInputObject

        /// <summary>
        /// 用户界面激活或停用对象。
        /// </summary>
        /// <param name="fActivate">
        /// 表示对象是否是激活或停用。如果值为非零，则对象是激活的，如果值为零，则对象是停用的。
        /// </param>
        public void UIActivateIO(int fActivate, ref NativeMethods.MSG msg)
        {
            if (fActivate != 0)
            {
                Control nextControl = base.GetNextControl(this, true);
                if (Control.ModifierKeys == Keys.Shift)
                {
                    nextControl = base.GetNextControl(nextControl, false);
                }
                if (nextControl != null)
                {
                    nextControl.Select();
                }
                base.Focus();
            }

        }

        public int HasFocusIO()
        {
            if (!base.ContainsFocus)
            {
                return 1;
            }
            return 0;
        }

        /// <summary>
        /// 启用对象来处理键盘快捷方式。
        /// </summary>
        public int TranslateAcceleratorIO(ref NativeMethods.MSG msg)
        {
            if ((msg.message == 256) && ((msg.wParam == 9) || (msg.wParam == 117)))
                if (base.SelectNextControl(
                    base.ActiveControl,
                    Control.ModifierKeys != Keys.Shift,
                    true,
                    true,
                    false))
                {
                    return 0;
                }
            return 1;

        }
        #endregion

        #region ComRegister Functions

        /// <summary>
        /// 当派生类被注册为一个COM服务器时被调用。
        /// </summary>
        [ComRegisterFunctionAttribute]
        public static void Register(Type t)
        {

            // 为浏览器栏添加类别标示符并设置其他一些值。
            RegistryKey clsidkey =
                Registry.ClassesRoot.CreateSubKey(@"CLSID\" + t.GUID.ToString("B"));
            clsidkey.SetValue(null, ImageListExplorerBar.imageListExplorerBarTitle);
            clsidkey.SetValue("MenuText", ImageListExplorerBar.imageListExplorerBarTitle);
            clsidkey.SetValue("HelpText", "See Readme.txt for detailed help!");
            clsidkey.CreateSubKey("Implemented Categories")
                .CreateSubKey(ImageListExplorerBar.verticalExplorerBarCATID);
            clsidkey.Close();

            // 设置栏的大小。
            string explorerBarKeyPath =
@"SOFTWARE\Microsoft\Internet Explorer\Explorer Bars\" + t.GUID.ToString("B");
            RegistryKey explorerBarKey =
                Registry.LocalMachine.CreateSubKey(explorerBarKeyPath);
            explorerBarKey.SetValue("BarSize",
                new byte[] { 06, 01, 00, 00, 00, 00, 00, 00 },
                RegistryValueKind.Binary);
            explorerBarKey.Close();


            Registry.CurrentUser.CreateSubKey(explorerBarKeyPath).SetValue("BarSize", new byte[] { 06, 01, 00, 00, 00, 00, 00, 00 }, RegistryValueKind.Binary);

            // 删除缓存。
            try
            {
                string catPath =
@"Software\Microsoft\Windows\CurrentVersion\Explorer\Discardable\PostSetup\Component Categories\"
+ ImageListExplorerBar.verticalExplorerBarCATID;

                Registry.CurrentUser.OpenSubKey(catPath, true).DeleteSubKey("Enum");
            }
            catch { }

            try
            {
                string catPath =
@"Software\Microsoft\Windows\CurrentVersion\Explorer\Discardable\PostSetup\Component Categories64\"
+ ImageListExplorerBar.verticalExplorerBarCATID;

                Registry.CurrentUser.OpenSubKey(catPath, true).DeleteSubKey("Enum");
            }
            catch { }
        }

        /// <summary>
        /// 当派生类以一个COM服务器注销时被调用。
        /// </summary>
        [ComUnregisterFunctionAttribute]
        public static void Unregister(Type t)
        {
            string clsidkeypath = t.GUID.ToString("B");
            Registry.ClassesRoot.OpenSubKey("CLSID", true).DeleteSubKeyTree(clsidkeypath);

            string explorerBarsKeyPath =
@"SOFTWARE\Microsoft\Internet Explorer\Explorer Bars";

            Registry.LocalMachine.OpenSubKey(explorerBarsKeyPath, true).DeleteSubKey(t.GUID.ToString("B"));
            Registry.CurrentUser.OpenSubKey(explorerBarsKeyPath, true).DeleteSubKey(t.GUID.ToString("B"));


        }


        #endregion

        private void btnGetImg_Click(object sender, EventArgs e)
        {
            lstImg.Items.Clear();

            mshtml.HTMLDocument doc = explorer.Document as mshtml.HTMLDocument;

            var imgs = doc.getElementsByTagName("img");

            foreach (var img in imgs)
            {
                mshtml.IHTMLImgElement imgElement = img as mshtml.IHTMLImgElement;
                if (imgElement != null && !lstImg.Items.Contains(imgElement.src))
                {
                    lstImg.Items.Add(imgElement.src);
                }
            }
        }

        private void lstImg_DoubleClick(object sender, EventArgs e)
        {
            if (lstImg.SelectedItem != null)
            {
                string url = lstImg.SelectedItem as string;
                mshtml.HTMLDocument doc = explorer.Document as mshtml.HTMLDocument;
                doc.parentWindow.open(url);
            }
        }
    }
}
