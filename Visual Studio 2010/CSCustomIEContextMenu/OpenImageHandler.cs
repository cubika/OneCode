/*************************************** 模块头*****************************\
* 模块名::  OpenImageHandler.cs
* 项目名:  CSCustomIEContextMenu
* 版权 (c) Microsoft Corporation.
* 
* 类 OpenImageHandler 实现了接口 IDocHostUIHandler 的 ShowContextMenu 方法.
* 对于接口 IDocHostUIHandler 的其他方法,只返回 1,这意味着将使用默认处理程序.
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
using System.Drawing;
using System.Windows.Forms;
using CSCustomIEContextMenu.NativeMethods;
using mshtml;
using SHDocVw;

namespace CSCustomIEContextMenu
{
    class OpenImageHandler : NativeMethods.IDocHostUIHandler, IDisposable
    {

        private bool disposed = false;

        // 承载此 WebBrowser 控件的 IE 实例.
        public InternetExplorer host;

        // 自定义上下文菜单. 
        private ContextMenuStrip contextMenu;
        private ToolStripMenuItem menuItem;


        /// <summary>
        /// 初始化处理程序.
        /// </summary>
        public OpenImageHandler(InternetExplorer host)
        {
            this.host = host;

            contextMenu = new ContextMenuStrip();
            menuItem = new ToolStripMenuItem();

            menuItem.Size = new Size(180, 100);
            menuItem.Text = "在新选项卡中打开图像";
            menuItem.Click += new EventHandler(menuItem_Click);

            contextMenu.Items.Add(menuItem);
        }

        void menuItem_Click(object sender, EventArgs e)
        {

            try
            {
                (host.Document as HTMLDocument).parentWindow.open(contextMenu.Tag as string);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        #region IDocHostUIHandler

        /// <summary>
        /// 显示用于图像的自定义上下文菜单.
        /// </summary>
        /// <param name="dwID">
        /// Dword 值，它指定要显示的快捷菜单中的标识符.
        /// 请参阅 NativeMethods.CONTEXT_MENU_CONST.
        /// </param>
        /// <param name="pt">
        /// 菜单的屏幕坐标.
        /// </param>
        /// <param name="pcmdtReserved"></param>
        /// <param name="pdispReserved">
        /// 该对象在 pt 中指定的屏幕坐标处.
        /// 这使得宿主可以传递特定的对象,如锚标签和图像，以提供更多特定指定的上下文.
        /// </param>
        /// <returns>
        /// 返回 0 则意味着宿主显示其用户界面,MSHTML 不会尝试显示其用户界面.
        /// </returns>
        public int ShowContextMenu(int dwID, POINT pt, object pcmdtReserved, object pdispReserved)
        {
            if (dwID == NativeMethods.CONTEXT_MENU_CONST.CONTEXT_MENU_IMAGE)
            {
                var img = pdispReserved as IHTMLImgElement;
                if (img != null)
                {
                    contextMenu.Tag = img.src;
                    contextMenu.Show(pt.x, pt.y);
                    return 0;
                }
            }
            return 1;
        }

        public int GetHostInfo(DOCHOSTUIINFO info)
        {
            return 1;
        }

        public int ShowUI(int dwID, IOleInPlaceActiveObject activeObject, IOleCommandTarget commandTarget, IOleInPlaceFrame frame, IOleInPlaceUIWindow doc)
        {
            return 1;
        }

        public int HideUI()
        {
            return 1;
        }

        public int UpdateUI()
        {
            return 1;
        }

        public int EnableModeless(bool fEnable)
        {
            return 1;
        }

        public int OnDocWindowActivate(bool fActivate)
        {
            return 1;
        }

        public int OnFrameWindowActivate(bool fActivate)
        {
            return 1;
        }

        public int ResizeBorder(COMRECT rect, IOleInPlaceUIWindow doc, bool fFrameWindow)
        {
            return 1;
        }

        public int TranslateAccelerator(ref MSG msg, ref Guid group, int nCmdID)
        {
            return 1;
        }

        public int GetOptionKeyPath(string[] pbstrKey, int dw)
        {
            return 1;
        }

        public int GetDropTarget(IOleDropTarget pDropTarget, out IOleDropTarget ppDropTarget)
        {
            ppDropTarget = null;
            return 1;
        }

        public int GetExternal(out object ppDispatch)
        {
            ppDispatch = null;
            return 1;
        }

        public int TranslateUrl(int dwTranslate, string strURLIn, out string pstrURLOut)
        {
            pstrURLOut = string.Empty;
            return 1;
        }

        public int FilterDataObject(System.Runtime.InteropServices.ComTypes.IDataObject pDO, out System.Runtime.InteropServices.ComTypes.IDataObject ppDORet)
        {
            ppDORet = null;
            return 1;
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
                if (contextMenu != null)
                {
                    contextMenu.Dispose();
                }

                if (menuItem != null)
                {
                    menuItem.Dispose();
                }
            }
            disposed = true;
        }
    }
}
