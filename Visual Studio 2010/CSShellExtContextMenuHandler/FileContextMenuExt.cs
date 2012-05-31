/********************************** 模块头 **********************************\
模块名:  FileContextMenuExt.cs
项目名:      CSShellExtContextMenuHandler
版权 (c) Microsoft Corporation.

FileContextMenuExt.cs定义了一个实现IShellExtInit和IContextMenu接口的上下文菜单应用程序

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER 
EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF 
MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***********************************************************************************/

#region Using directives
using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
#endregion


namespace CSShellExtContextMenuHandler
{
    [ClassInterface(ClassInterfaceType.None)]
    [Guid("B1F1405D-94A1-4692-B72F-FC8CAF8B8700"), ComVisible(true)]
    public class FileContextMenuExt : IShellExtInit, IContextMenu
    {
        // 被选中的文件名.
        private string selectedFile;

        private string menuText = "&Display File Name (C#)";
        private string verb = "csdisplay";
        private string verbCanonicalName = "CSDisplayFileName";
        private string verbHelpText = "Display File Name (C#)";
        private uint IDM_DISPLAY = 0;


        void OnVerbDisplayFileName(IntPtr hWnd)
        {
            System.Windows.Forms.MessageBox.Show(
                "被选中的文件是 \r\n\r\n" + this.selectedFile,
                "CSShellExtContextMenuHandler");
        }


        #region Shell Extension Registration

        [ComRegisterFunction()]
        public static void Register(Type t)
        {
            try
            {
                ShellExtReg.RegisterShellExtContextMenuHandler(t, ".cs",
                    "CSShellExtContextMenuHandler.FileContextMenuExt");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message); // 记录错误日志 
                throw;  // 抛出异常
            }
        }

        [ComUnregisterFunction()]
        public static void Unregister(Type t)
        {
            try
            {
                ShellExtReg.UnregisterShellExtContextMenuHandler(t, ".cs");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message); // 记录错误日志 
                throw;  // 再次抛出异常
            }
        }

        #endregion


        #region IShellExtInit Members

        /// <summary>
        /// 初始化上下文菜单处理程序
        /// </summary>
        /// <param name="pidlFolder">
        /// 指向标识文件夹的 ITEMIDLIST 结构的指针.
        /// </param>
        /// <param name="pDataObj">
        /// 一个指向 IDataObject 接口对象的指针可以用来检索对象.
        /// </param>
        /// <param name="hKeyProgID">
        /// 对象或文件夹的文件类型的注册表项.
        /// </param>
        public void Initialize(IntPtr pidlFolder, IntPtr pDataObj, IntPtr hKeyProgID)
        {
            if (pDataObj == IntPtr.Zero)
            {
                throw new ArgumentException();
            }

            FORMATETC fe = new FORMATETC();
            fe.cfFormat = (short)CLIPFORMAT.CF_HDROP;
            fe.ptd = IntPtr.Zero;
            fe.dwAspect = DVASPECT.DVASPECT_CONTENT;
            fe.lindex = -1;
            fe.tymed = TYMED.TYMED_HGLOBAL;
            STGMEDIUM stm = new STGMEDIUM();

            // pDataObj 指针指向对象. 在这示例，我们得到一个HDROP的句柄的枚举在选定的文件和文件夹

            IDataObject dataObject = (IDataObject)Marshal.GetObjectForIUnknown(pDataObj);
            dataObject.GetData(ref fe, out stm);

            try
            {
                // 取到HDROP句柄
                IntPtr hDrop = stm.unionmember;
                if (hDrop == IntPtr.Zero)
                {
                    throw new ArgumentException();
                }

                //确定在此操作中涉及多少个文件.  
                uint nFiles = NativeMethods.DragQueryFile(hDrop, UInt32.MaxValue, null, 0);

                //只有一个文件被选中时，示例代码将显示自定义上下文菜单项中. 
                if (nFiles == 1)
                {
                    //获取文件名.
                    StringBuilder fileName = new StringBuilder(260);
                    if (0 == NativeMethods.DragQueryFile(hDrop, 0, fileName,
                        fileName.Capacity))
                    {
                        Marshal.ThrowExceptionForHR(WinError.E_FAIL);
                    }
                    this.selectedFile = fileName.ToString();
                }
                else
                {
                    Marshal.ThrowExceptionForHR(WinError.E_FAIL);
                }

                // [-或者-]

                //  枚举被选中的文件和文件夹.
                //if (nFiles > 0)
                //{
                //    StringCollection selectedFiles = new StringCollection();
                //    StringBuilder fileName = new StringBuilder(260);
                //    for (uint i = 0; i < nFiles; i++)
                //    {
                //        // Get the next file name.
                //        if (0 != NativeMethods.DragQueryFile(hDrop, i, fileName,
                //            fileName.Capacity))
                //        {
                //            // Add the file name to the list.
                //            selectedFiles.Add(fileName.ToString());
                //        }
                //    }
                //
                //    // If we did not find any files we can work with, throw 
                //    // exception.
                //    if (selectedFiles.Count == 0)
                //    {
                //        Marshal.ThrowExceptionForHR(WinError.E_FAIL);
                //    }
                //}
                //else
                //{
                //    Marshal.ThrowExceptionForHR(WinError.E_FAIL);
                //}
            }
            finally
            {
                NativeMethods.ReleaseStgMedium(ref stm);
            }
        }

        #endregion


        #region IContextMenu Members

        /// <summary>
        /// 将命令添加到快捷菜单.
        /// </summary>
        /// <param name="hMenu">快捷菜单的句柄.</param>
        /// <param name="iMenu">
        /// 从零位置开始插入第一个新菜单项.
        /// </param>
        /// <param name="idCmdFirst">
        /// 菜单项 ID 指定的最小值.
        /// </param>
        /// <param name="idCmdLast">
        /// 菜单项 ID 指定的最大值.
        /// </param>
        /// <param name="uFlags">
        /// 可选参数，用来标示快捷菜单如何改变.
        /// </param>
        /// <returns>
        // 如果成功返回一个HRESULT类型的值.
        // 将代码值设置为最大的命令标识符的偏移量将被分配并且加一.
        /// </returns>
        public int QueryContextMenu(
            IntPtr hMenu,
            uint iMenu,
            uint idCmdFirst,
            uint idCmdLast,
            uint uFlags)
        {
            //  如果uFlags参数包含CMF_DEFAULTONLY，我们就可以什么都不要做了.
            if (((uint)CMF.CMF_DEFAULTONLY & uFlags) != 0)
            {
                return WinError.MAKE_HRESULT(WinError.SEVERITY_SUCCESS, 0, 0);
            }

            // 使用InsertMenu 或 InsertMenuItem添加菜单项.

            MENUITEMINFO mii = new MENUITEMINFO();
            mii.cbSize = (uint)Marshal.SizeOf(mii);
            mii.fMask = MIIM.MIIM_ID | MIIM.MIIM_TYPE | MIIM.MIIM_STATE;
            mii.wID = idCmdFirst + IDM_DISPLAY;
            mii.fType = MFT.MFT_STRING;
            mii.dwTypeData = menuText;
            mii.fState = MFS.MFS_ENABLED;
            if (!NativeMethods.InsertMenuItem(hMenu, iMenu, true, ref mii))
            {
                return Marshal.GetHRForLastWin32Error();
            }

            // 增加一个分隔符.
            MENUITEMINFO sep = new MENUITEMINFO();
            sep.cbSize = (uint)Marshal.SizeOf(sep);
            sep.fMask = MIIM.MIIM_TYPE;
            sep.fType = MFT.MFT_SEPARATOR;
            if (!NativeMethods.InsertMenuItem(hMenu, iMenu + 1, true, ref sep))
            {
                return Marshal.GetHRForLastWin32Error();
            }

            // 如果成功返回一个HRESULT类型的值.
            // 将代码值设置为最大的命令标识符的偏移量将被分配并且加一.
            return WinError.MAKE_HRESULT(WinError.SEVERITY_SUCCESS, 0,
                IDM_DISPLAY + 1);
        }

        /// <summary>
        /// 当用户点击相关联的菜单项后则调用该方法.
        /// </summary>
        /// <param name="pici">
        /// 参数pici是一个指向包含相关必须信息的CMINVOKECOMMANDINFO或CMINVOKECOMMANDINFOEX指针
        /// </param>
        public void InvokeCommand(IntPtr pici)
        {
            bool isUnicode = false;

            //检查什么类型的结构体被传递进函数，CMINVOKECOMMANDINFO还是CMINVOKECOMMANDINFOEX取决于
            //lpcmi的成员变量cbSize，虽然lpcmi在 Shlobj.h中指向CMINVOKECOMMANDINFO结构体
            //但通常lpcmi指向的是一个CMINVOKECOMMANDINFOEX结构体，CMINVOKECOMMANDINFOEX是CMINVOKECOMMANDINFO的扩展
            //可以接受Unicode字符串           
            CMINVOKECOMMANDINFO ici = (CMINVOKECOMMANDINFO)Marshal.PtrToStructure(
                pici, typeof(CMINVOKECOMMANDINFO));
            CMINVOKECOMMANDINFOEX iciex = new CMINVOKECOMMANDINFOEX();
            if (ici.cbSize == Marshal.SizeOf(typeof(CMINVOKECOMMANDINFOEX)))
            {
                if ((ici.fMask & CMIC.CMIC_MASK_UNICODE) != 0)
                {
                    isUnicode = true;
                    iciex = (CMINVOKECOMMANDINFOEX)Marshal.PtrToStructure(pici,
                        typeof(CMINVOKECOMMANDINFOEX));
                }
            }

            // 检查命令是动态的还是偏移产生的.
            // 有两种方法来确定:
            // 
            //   1) 命令是动态的 
            //   2) 命令是偏移的
            // 
            //如果在ANSI的情况下lpcmi->lpVerb或Unicode情况下的lpcmi->lpVerbW为非0,且保存了一个动态的字符串则可以判断其为动态的，
            //反之则可以确定为偏移的

            if (!isUnicode && NativeMethods.HighWord(ici.lpVerb.ToInt32()) != 0)
            {
                //上下文菜单是否支持动态的
                if (Marshal.PtrToStringAnsi(ici.lpVerb) == this.verb)
                {
                    OnVerbDisplayFileName(ici.hwnd);
                }
                else
                {
                    // 如果上下文菜单处理程序不能识别该命令, 必须返回E_FAIL,并传递到其他的能识别该命令的上下文菜单项上,并抛出异常
                    Marshal.ThrowExceptionForHR(WinError.E_FAIL);
                }
            }

            //在Unicode情况下, 不为0, 动态命令保存在 lpcmi->lpVerbW.
            else if (isUnicode && NativeMethods.HighWord(iciex.lpVerbW.ToInt32()) != 0)
            {
                // 该上下文菜单是否支持动态
                if (Marshal.PtrToStringUni(iciex.lpVerbW) == this.verb)
                {
                    OnVerbDisplayFileName(ici.hwnd);
                }
                else
                {
                    // 如果上下文菜单处理程序不能识别该命令, 必须返回E_FAIL,并传递到其他的能识别该命令的上下文菜单项上,并抛出异常
                    Marshal.ThrowExceptionForHR(WinError.E_FAIL);
                }
            }

             // 如果该命令不能通过动态确定,则可以确定为偏移的
            else
            {
                //上下文菜单是否支持偏移的
                if (NativeMethods.LowWord(ici.lpVerb.ToInt32()) == IDM_DISPLAY)
                {
                    OnVerbDisplayFileName(ici.hwnd);
                }
                else
                {
                    //如果上下文菜单处理程序不能识别该命令, 必须返回E_FAIL,并传递到其他的能识别该命令的上下文菜单项上.
                    Marshal.ThrowExceptionForHR(WinError.E_FAIL);
                }
            }
        }

        /// <summary>
        /// 获取一个快捷菜单命令包括帮助字符串，信息独立，语言规范，命令名称
        /// </summary>
        /// <param name="idCmd">菜单命令标识符偏移量.</param>
        /// <param name="uFlags">
        /// 指定要返回的信息的标志. 这个参数只能是 GCS_HELPTEXTA, GCS_HELPTEXTW, GCS_VALIDATEA, GCS_VALIDATEW, GCS_VERBA, GCS_VERBW其中之一.
        /// </param>
        /// <param name="pReserved">保留. 必须是IntPtr.Zero</param>
        /// <param name="pszName">
        /// 以null 结尾的接收字符串缓冲区的地址.
        /// </param>
        /// <param name="cchMax">
        /// 在缓冲区中接收 null 结尾的字符串的字符的大小.
        /// </param>
        public void GetCommandString(
            UIntPtr idCmd,
            uint uFlags,
            IntPtr pReserved,
            StringBuilder pszName,
            uint cchMax)
        {
            if (idCmd.ToUInt32() == IDM_DISPLAY)
            {
                switch ((GCS)uFlags)
                {
                    case GCS.GCS_VERBW:
                        if (this.verbCanonicalName.Length > cchMax - 1)
                        {
                            Marshal.ThrowExceptionForHR(WinError.STRSAFE_E_INSUFFICIENT_BUFFER);
                        }
                        else
                        {
                            pszName.Clear();
                            pszName.Append(this.verbCanonicalName);
                        }
                        break;

                    case GCS.GCS_HELPTEXTW:
                        if (this.verbHelpText.Length > cchMax - 1)
                        {
                            Marshal.ThrowExceptionForHR(WinError.STRSAFE_E_INSUFFICIENT_BUFFER);
                        }
                        else
                        {
                            pszName.Clear();
                            pszName.Append(this.verbHelpText);
                        }
                        break;
                }
            }
        }

        #endregion
    }
}