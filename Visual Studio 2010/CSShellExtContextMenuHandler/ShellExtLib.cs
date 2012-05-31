/********************************** 模块头**********************************\
模块名:      ShellExtLib.cs
项目名:      CSShellExtContextMenuHandler
版权 (c) Microsoft Corporation.

该文件声明导入命令行程序接口： IShellExtInit 和 IContextMenu，
实现用于注册和注销壳上下文的帮助器函数
处理程序菜单，并定义 Win32 枚举、 结构、 consts 和所使用的功能代码示例


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
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
#endregion


namespace CSShellExtContextMenuHandler
{
    #region Shell Interfaces

    [ComImport(), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("000214e8-0000-0000-c000-000000000046")]
    internal interface IShellExtInit
    {
        void Initialize(
            IntPtr /*LPCITEMIDLIST*/ pidlFolder,
            IntPtr /*LPDATAOBJECT*/ pDataObj,
            IntPtr /*HKEY*/ hKeyProgID);
    }


    [ComImport(), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("000214e4-0000-0000-c000-000000000046")]
    internal interface IContextMenu
    {
        [PreserveSig]
        int QueryContextMenu(
            IntPtr /*HMENU*/ hMenu,
            uint iMenu,
            uint idCmdFirst,
            uint idCmdLast,
            uint uFlags);

        void InvokeCommand(IntPtr pici);

        void GetCommandString(
            UIntPtr idCmd,
            uint uFlags,
            IntPtr pReserved,
            StringBuilder pszName,
            uint cchMax);
    }

    #endregion


    #region Shell Registration

    internal class ShellExtReg
    {
        /// <summary>
        /// 注册上下文菜单处理程序.
        /// </summary>
        /// <param name="t">COM class</param>
        /// <param name="fileType">
        /// 注册上下文菜单处理程序关联的文件类型. 比如 '*' 表示所有文件; '.txt' 表示文本文件 .该参数不能是null值和空字符串. 
        /// </param>
        /// <param name="friendlyName">组件的别名</param>
        public static void RegisterShellExtContextMenuHandler(
            Type t, string fileType, string friendlyName)
        {
            if (string.IsNullOrEmpty(fileType))
            {
                throw new ArgumentException("文件类型不能为null或空字符串");
            }

            //如果文件的类型是以'.'开头的，然后在注册表的HKCR\<File Type>下获取该文件类型的对应的Program ID来关联该文件
            if (fileType.StartsWith("."))
            {
                using (RegistryKey key = Registry.ClassesRoot.OpenSubKey(fileType))
                {
                    if (key != null)
                    {
                        // 如果该键存在且不为空, 使用ProgID 为该文件类型
                        string defaultVal = key.GetValue(null) as string;
                        if (!string.IsNullOrEmpty(defaultVal))
                        {
                            fileType = defaultVal;
                        }
                    }
                }
            }

            // 创建键HKCR\<File Type>\shellex\ContextMenuHandlers\{<CLSID>}
            string keyName = string.Format(@"{0}\shellex\ContextMenuHandlers\{1}",
                fileType, t.GUID.ToString("B"));
            using (RegistryKey key = Registry.ClassesRoot.CreateSubKey(keyName))
            {
                // 为该键设置默认值.
                if (key != null && !string.IsNullOrEmpty(friendlyName))
                {
                    key.SetValue(null, friendlyName);
                }
            }
        }

        /// <summary>
        /// 反注册上下文菜单处理程序.
        /// </summary>
        /// <param name="t">COM类</param>
        /// <param name="fileType">
        /// 注册上下文菜单处理程序关联的文件类型. 比如 '*' 表示所有文件; '.txt' 表示文本文件 .该参数不能是null值和空字符串. 
        /// </param>
        public static void UnregisterShellExtContextMenuHandler(
            Type t, string fileType)
        {
            if (string.IsNullOrEmpty(fileType))
            {
                throw new ArgumentException("fileType must not be null or empty");
            }

            //如果文件的类型是以'.'开头的，然后在注册表的HKCR\<File Type>下获取该文件类型的对应的Program ID来关联该文件
            if (fileType.StartsWith("."))
            {
                using (RegistryKey key = Registry.ClassesRoot.OpenSubKey(fileType))
                {
                    if (key != null)
                    {
                        // 如果该键存在且不为空, 使用ProgID 为该文件类型
                        string defaultVal = key.GetValue(null) as string;
                        if (!string.IsNullOrEmpty(defaultVal))
                        {
                            fileType = defaultVal;
                        }
                    }
                }
            }

            // 删除HKCR\<File Type>\shellex\ContextMenuHandlers\{<CLSID>}键.
            string keyName = string.Format(@"{0}\shellex\ContextMenuHandlers\{1}",
                fileType, t.GUID.ToString("B"));
            Registry.ClassesRoot.DeleteSubKeyTree(keyName, false);
        }
    }

    #endregion


    #region Enums & Structs

    internal enum GCS : uint
    {
        GCS_VERBA = 0x00000000,
        GCS_HELPTEXTA = 0x00000001,
        GCS_VALIDATEA = 0x00000002,
        GCS_VERBW = 0x00000004,
        GCS_HELPTEXTW = 0x00000005,
        GCS_VALIDATEW = 0x00000006,
        GCS_VERBICONW = 0x00000014,
        GCS_UNICODE = 0x00000004
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct CMINVOKECOMMANDINFO
    {
        public uint cbSize;
        public CMIC fMask;
        public IntPtr hwnd;
        public IntPtr lpVerb;
        [MarshalAs(UnmanagedType.LPStr)]
        public string parameters;
        [MarshalAs(UnmanagedType.LPStr)]
        public string directory;
        public int nShow;
        public uint dwHotKey;
        public IntPtr hIcon;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct CMINVOKECOMMANDINFOEX
    {
        public uint cbSize;
        public CMIC fMask;
        public IntPtr hwnd;
        [MarshalAs(UnmanagedType.LPStr)]
        public string verb;
        [MarshalAs(UnmanagedType.LPStr)]
        public string parameters;
        [MarshalAs(UnmanagedType.LPStr)]
        public string directory;
        public int nShow;
        public uint dwHotKey;
        public IntPtr hIcon;
        [MarshalAs(UnmanagedType.LPStr)]
        public string title;
        public IntPtr lpVerbW;
        public string parametersW;
        public string directoryW;
        public string titleW;
        POINT ptInvoke;
    }

    [Flags]
    internal enum CMIC : uint
    {
        CMIC_MASK_ICON = 0x00000010,
        CMIC_MASK_HOTKEY = 0x00000020,
        CMIC_MASK_NOASYNC = 0x00000100,
        CMIC_MASK_FLAG_NO_UI = 0x00000400,
        CMIC_MASK_UNICODE = 0x00004000,
        CMIC_MASK_NO_CONSOLE = 0x00008000,
        CMIC_MASK_ASYNCOK = 0x00100000,
        CMIC_MASK_NOZONECHECKS = 0x00800000,
        CMIC_MASK_FLAG_LOG_USAGE = 0x04000000,
        CMIC_MASK_SHIFT_DOWN = 0x10000000,
        CMIC_MASK_PTINVOKE = 0x20000000,
        CMIC_MASK_CONTROL_DOWN = 0x40000000
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct POINT
    {
        public int X;
        public int Y;
    }

    internal enum CLIPFORMAT : uint
    {
        CF_TEXT = 1,
        CF_BITMAP = 2,
        CF_METAFILEPICT = 3,
        CF_SYLK = 4,
        CF_DIF = 5,
        CF_TIFF = 6,
        CF_OEMTEXT = 7,
        CF_DIB = 8,
        CF_PALETTE = 9,
        CF_PENDATA = 10,
        CF_RIFF = 11,
        CF_WAVE = 12,
        CF_UNICODETEXT = 13,
        CF_ENHMETAFILE = 14,
        CF_HDROP = 15,
        CF_LOCALE = 16,
        CF_MAX = 17,

        CF_OWNERDISPLAY = 0x0080,
        CF_DSPTEXT = 0x0081,
        CF_DSPBITMAP = 0x0082,
        CF_DSPMETAFILEPICT = 0x0083,
        CF_DSPENHMETAFILE = 0x008E,

        CF_PRIVATEFIRST = 0x0200,
        CF_PRIVATELAST = 0x02FF,

        CF_GDIOBJFIRST = 0x0300,
        CF_GDIOBJLAST = 0x03FF
    }

    [Flags]
    internal enum CMF : uint
    {
        CMF_NORMAL = 0x00000000,
        CMF_DEFAULTONLY = 0x00000001,
        CMF_VERBSONLY = 0x00000002,
        CMF_EXPLORE = 0x00000004,
        CMF_NOVERBS = 0x00000008,
        CMF_CANRENAME = 0x00000010,
        CMF_NODEFAULT = 0x00000020,
        CMF_INCLUDESTATIC = 0x00000040,
        CMF_ITEMMENU = 0x00000080,
        CMF_EXTENDEDVERBS = 0x00000100,
        CMF_DISABLEDVERBS = 0x00000200,
        CMF_ASYNCVERBSTATE = 0x00000400,
        CMF_OPTIMIZEFORINVOKE = 0x00000800,
        CMF_SYNCCASCADEMENU = 0x00001000,
        CMF_DONOTPICKDEFAULT = 0x00002000,
        CMF_RESERVED = 0xFFFF0000
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct MENUITEMINFO
    {
        public uint cbSize;
        public MIIM fMask;
        public MFT fType;
        public MFS fState;
        public uint wID;
        public IntPtr hSubMenu;
        public IntPtr hbmpChecked;
        public IntPtr hbmpUnchecked;
        public UIntPtr dwItemData;
        public string dwTypeData;
        public uint cch;
        public IntPtr hbmpItem;
    }

    [Flags]
    internal enum MIIM : uint
    {
        MIIM_STATE = 0x00000001,
        MIIM_ID = 0x00000002,
        MIIM_SUBMENU = 0x00000004,
        MIIM_CHECKMARKS = 0x00000008,
        MIIM_TYPE = 0x00000010,
        MIIM_DATA = 0x00000020,
        MIIM_STRING = 0x00000040,
        MIIM_BITMAP = 0x00000080,
        MIIM_FTYPE = 0x00000100
    }

    internal enum MFT : uint
    {
        MFT_STRING = 0x00000000,
        MFT_BITMAP = 0x00000004,
        MFT_MENUBARBREAK = 0x00000020,
        MFT_MENUBREAK = 0x00000040,
        MFT_OWNERDRAW = 0x00000100,
        MFT_RADIOCHECK = 0x00000200,
        MFT_SEPARATOR = 0x00000800,
        MFT_RIGHTORDER = 0x00002000,
        MFT_RIGHTJUSTIFY = 0x00004000
    }

    internal enum MFS : uint
    {
        MFS_ENABLED = 0x00000000,
        MFS_UNCHECKED = 0x00000000,
        MFS_UNHILITE = 0x00000000,
        MFS_GRAYED = 0x00000003,
        MFS_DISABLED = 0x00000003,
        MFS_CHECKED = 0x00000008,
        MFS_HILITE = 0x00000080,
        MFS_DEFAULT = 0x00001000
    }

    #endregion


    internal class NativeMethods
    {
        /// <summary>
        /// 检索成功的拖放操作中的已删除文件的名称.
        /// </summary>
        /// <param name="hDrop">
        /// 结构，其中包含已删除的文件的文件名称的标识符.
        /// </param>
        /// <param name="iFile">
        /// 要查询文件的索引. 如果此参数的值是 0xFFFFFFFF,DragQueryFile 返回删除的文件的计数. 
        /// </param>
        /// <param name="pszFile">
        /// 返回接收的已删除的文件的文件名缓冲区的地址.
        /// </param>
        /// <param name="cch">
        /// pszFile 缓冲区的大小.
        /// </param>
        /// <returns>一个非零值指示成功调用.</returns>
        [DllImport("shell32", CharSet = CharSet.Unicode)]
        public static extern uint DragQueryFile(
            IntPtr hDrop,
            uint iFile,
            StringBuilder pszFile,
            int cch);

        /// <summary>
        /// 释放指定的存储空间.
        /// </summary>
        /// <param name="pmedium">
        /// 对要释放的存储空间的引用.
        /// </param>
        [DllImport("ole32.dll", CharSet = CharSet.Unicode)]
        public static extern void ReleaseStgMedium(ref STGMEDIUM pmedium);

        /// <summary>
        /// 在菜单特定位置插入一个新的菜单
        /// </summary>
        /// <param name="hMenu">
        /// 新插入的菜单项的句柄. 
        /// </param>
        /// <param name="uItem">
        /// 新插入菜单项的前一个菜单项目的地址或位置.
        /// </param>
        /// <param name="fByPosition">
        /// 菜单项的标识符 
        /// </param>
        /// <param name="mii">
        /// 包含新的菜单项的 MENUITEMINFO 结构信息的引用
        /// </param>
        /// <returns>
        /// 如果执行成功将返回True.
        /// </returns>
        [DllImport("user32", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool InsertMenuItem(
            IntPtr hMenu,
            uint uItem,
            [MarshalAs(UnmanagedType.Bool)]bool fByPosition,
            ref MENUITEMINFO mii);


        public static int HighWord(int number)
        {
            return ((number & 0x80000000) == 0x80000000) ?
                (number >> 16) : ((number >> 16) & 0xffff);
        }

        public static int LowWord(int number)
        {
            return number & 0xffff;
        }
    }

    internal static class WinError
    {
        public const int S_OK = 0x0000;
        public const int S_FALSE = 0x0001;
        public const int E_FAIL = -2147467259;
        public const int E_INVALIDARG = -2147024809;
        public const int E_OUTOFMEMORY = -2147024882;
        public const int STRSAFE_E_INSUFFICIENT_BUFFER = -2147024774;

        public const uint SEVERITY_SUCCESS = 0;
        public const uint SEVERITY_ERROR = 1;

        /// <summary>
        /// 返回HRESULT.
        /// </summary>
        /// <param name="sev">严重度</param>
        /// <param name="fac">The facility to be used</param>
        /// <param name="code">错误代码</param>
        /// <returns>通过上面三个值构造一个HRESULT类型返回值</returns>
        public static int MAKE_HRESULT(uint sev, uint fac, uint code)
        {
            return (int)((sev << 31) | (fac << 16) | code);
        }
    }
}