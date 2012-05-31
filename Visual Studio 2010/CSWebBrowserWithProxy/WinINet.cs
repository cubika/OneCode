/****************************** Module Header ******************************\
* 模块名称:  WinINet.cs
* 项目名称:	    CSWebBrowserWithProxy
* Copyright (c) Microsoft Corporation.
* 
* 这个类是一个简单的.NET对wininet.dll的封装。它包含了2个对wininet.dll的扩展方法
* （InternetSetOption和InternetQueryOption）。这个类可以设置、激活、备份、保存对
* internet的设置。
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

namespace CSWebBrowserWithProxy
{
    public static class WinINet
    {
      

        /// <summary>
        /// 设置Internet选项
        /// </summary>
        [DllImport("wininet.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern bool InternetSetOption(
            IntPtr hInternet,
            INTERNET_OPTION dwOption,
            IntPtr lpBuffer,
            int lpdwBufferLength);

        /// <summary>
        /// 在一个指定的句柄上查询Internet选项值。这个名柄通常为0。
        /// </summary>
        [DllImport("wininet.dll", CharSet = CharSet.Ansi, SetLastError = true,
            EntryPoint = "InternetQueryOption")]
        private extern static bool InternetQueryOptionList(
            IntPtr Handle,
            INTERNET_OPTION OptionFlag,
            ref INTERNET_PER_CONN_OPTION_LIST OptionList,
            ref int size);

        /// <summary>
        /// 为局域网设定代理
        /// </summary>
        public static bool SetConnectionProxy(string proxyServer)
        {
            // 创建3个选项
            INTERNET_PER_CONN_OPTION[] Options = new INTERNET_PER_CONN_OPTION[3];

            // 设置代理标记
            Options[0] = new INTERNET_PER_CONN_OPTION();
            Options[0].dwOption = (int)INTERNET_PER_CONN_OptionEnum.INTERNET_PER_CONN_FLAGS;
            Options[0].Value.dwValue = (int)INTERNET_OPTION_PER_CONN_FLAGS.PROXY_TYPE_PROXY;

            // 设置代理名称
            Options[1] = new INTERNET_PER_CONN_OPTION();
            Options[1].dwOption =
                (int)INTERNET_PER_CONN_OptionEnum.INTERNET_PER_CONN_PROXY_SERVER;
            Options[1].Value.pszValue = Marshal.StringToHGlobalAnsi(proxyServer);

            // 设置代理口令
            Options[2] = new INTERNET_PER_CONN_OPTION();
            Options[2].dwOption =
                (int)INTERNET_PER_CONN_OptionEnum.INTERNET_PER_CONN_PROXY_BYPASS;
            Options[2].Value.pszValue = Marshal.StringToHGlobalAnsi("local");

            // 申请内存储存设置
            System.IntPtr buffer = Marshal.AllocCoTaskMem(Marshal.SizeOf(Options[0])
                + Marshal.SizeOf(Options[1]) + Marshal.SizeOf(Options[2]));

            System.IntPtr current = buffer;

            // 将数据从类的实体中转存到内存中。
            for (int i = 0; i < Options.Length; i++)
            {
                Marshal.StructureToPtr(Options[i], current, false);
                current = (System.IntPtr)((int)current + Marshal.SizeOf(Options[i]));
            }

            // 初始化INTERNET_PER_CONN_OPTION_LIST数据结构的实体。
            INTERNET_PER_CONN_OPTION_LIST option_list = new INTERNET_PER_CONN_OPTION_LIST();

            // 定义指向被申请的内存的指针。
            option_list.pOptions = buffer;

            // 返回不受管理的实体大小。
            option_list.Size = Marshal.SizeOf(option_list);

            // IntPtr为0表示局域网连接。
            option_list.Connection = IntPtr.Zero;

            option_list.OptionCount = Options.Length;
            option_list.OptionError = 0;
            int size = Marshal.SizeOf(option_list);

            // 为INTERNET_PER_CONN_OPTION_LIST实体申请内存。
            IntPtr intptrStruct = Marshal.AllocCoTaskMem(size);

            // 将数据从一个受管理的类中移到不受管理的内存中去。
            Marshal.StructureToPtr(option_list, intptrStruct, true);

            // 设定internet选项。
            bool bReturn = InternetSetOption(IntPtr.Zero,
                INTERNET_OPTION.INTERNET_OPTION_PER_CONNECTION_OPTION, intptrStruct, size);

            // 释放内存
            Marshal.FreeCoTaskMem(buffer);
            Marshal.FreeCoTaskMem(intptrStruct);

            // 当这个操作失败时抛出异常。
            if (!bReturn)
            {
                throw new ApplicationException(" Set Internet Option Failed!");
            }

            // 通知系统注册值的改变，以便注册中的句柄能使用这些设定重读代理数据。
            InternetSetOption(IntPtr.Zero, INTERNET_OPTION.INTERNET_OPTION_SETTINGS_CHANGED,
                IntPtr.Zero, 0);
            InternetSetOption(IntPtr.Zero, INTERNET_OPTION.INTERNET_OPTION_REFRESH,
                IntPtr.Zero, 0);

            return bReturn;
        }

        /// <summary>
        /// 使局域网的代理连接失效。
        /// </summary>
        /// <returns></returns>
        public static bool DisableConnectionProxy()
        {
            // 通过直接连接internet来建立连接。
            INTERNET_PER_CONN_OPTION[] Options = new INTERNET_PER_CONN_OPTION[1];
            Options[0].dwOption = (int)INTERNET_PER_CONN_OptionEnum.INTERNET_PER_CONN_FLAGS;
            Options[0].Value.dwValue = (int)INTERNET_OPTION_PER_CONN_FLAGS.PROXY_TYPE_DIRECT;

            // 将数据从一个受管理的类移到不受管理的内存中。
            System.IntPtr buffer = Marshal.AllocCoTaskMem(Marshal.SizeOf(Options[0]));
            Marshal.StructureToPtr(Options[0], buffer, false);

            // 初始化INTERNET_PER_CONN_OPTION_LIST数据结构的实体。
            INTERNET_PER_CONN_OPTION_LIST request = new INTERNET_PER_CONN_OPTION_LIST();

            // 定义指向被申请的内存的指针。
            request.pOptions = buffer;

            request.Size = Marshal.SizeOf(request);

            // IntPtr为0表示局域网连接。
            request.Connection = IntPtr.Zero;
            request.OptionCount = Options.Length;
            request.OptionError = 0;
            int size = Marshal.SizeOf(request);

            // 为INTERNET_PER_CONN_OPTION_LIST实体申请内存。
            IntPtr intptrStruct = Marshal.AllocCoTaskMem(size);

            // 将数据从一个受管理的类移到不受管理的内存中。
            Marshal.StructureToPtr(request, intptrStruct, true);

            // 设定internet选项。
            bool bReturn = InternetSetOption(IntPtr.Zero,
        INTERNET_OPTION.INTERNET_OPTION_PER_CONNECTION_OPTION, intptrStruct, size);

            // 释放内存
            Marshal.FreeCoTaskMem(buffer);
            Marshal.FreeCoTaskMem(intptrStruct);

            // 当这个操作失败时抛出异常。
            if (!bReturn)
            {
                throw new ApplicationException(" Set Internet Option Failed! ");
            }

            // 通知系统注册值的改变，以便注册中的句柄能使用这些设定重读代理数据。
            InternetSetOption(IntPtr.Zero, INTERNET_OPTION.INTERNET_OPTION_SETTINGS_CHANGED,
                IntPtr.Zero, 0);
            InternetSetOption(IntPtr.Zero, INTERNET_OPTION.INTERNET_OPTION_REFRESH,
                IntPtr.Zero, 0);

            return bReturn;
        }

        /// <summary>
        /// 备份当前局域网连接的设置，确认恢复后内存被释放。
        /// </summary>
        public static INTERNET_PER_CONN_OPTION_LIST BackupConnectionProxy()
        {

            // 查询以下选项。
            INTERNET_PER_CONN_OPTION[] Options = new INTERNET_PER_CONN_OPTION[3];

            Options[0] = new INTERNET_PER_CONN_OPTION();
            Options[0].dwOption = (int)INTERNET_PER_CONN_OptionEnum.INTERNET_PER_CONN_FLAGS;
            Options[1] = new INTERNET_PER_CONN_OPTION();
            Options[1].dwOption = (int)INTERNET_PER_CONN_OptionEnum.INTERNET_PER_CONN_PROXY_SERVER;
            Options[2] = new INTERNET_PER_CONN_OPTION();
            Options[2].dwOption = (int)INTERNET_PER_CONN_OptionEnum.INTERNET_PER_CONN_PROXY_BYPASS;

            // 为选项申请内存。
            System.IntPtr buffer = Marshal.AllocCoTaskMem(Marshal.SizeOf(Options[0])
                + Marshal.SizeOf(Options[1]) + Marshal.SizeOf(Options[2]));

            System.IntPtr current = (System.IntPtr)buffer;

            // 将数据从一个受管理的类移到不受管理的内存中。
            for (int i = 0; i < Options.Length; i++)
            {
                Marshal.StructureToPtr(Options[i], current, false);
                current = (System.IntPtr)((int)current + Marshal.SizeOf(Options[i]));
            }

            // 初始化INTERNET_PER_CONN_OPTION_LIST数据结构的实体。
            INTERNET_PER_CONN_OPTION_LIST Request = new INTERNET_PER_CONN_OPTION_LIST();

            // 定义指向被申请的内存的指针。
            Request.pOptions = buffer;

            Request.Size = Marshal.SizeOf(Request);

            // IntPtr为0表示局域网连接。
            Request.Connection = IntPtr.Zero;

            Request.OptionCount = Options.Length;
            Request.OptionError = 0;
            int size = Marshal.SizeOf(Request);

            // 查询internet设置。 
            bool result = InternetQueryOptionList(IntPtr.Zero, 
                INTERNET_OPTION.INTERNET_OPTION_PER_CONNECTION_OPTION, 
                ref Request, ref size);
            if (!result)
            {
                throw new ApplicationException(" Set Internet Option Failed! ");
            }

            return Request;
        }

        /// <summary>
        /// 恢复局域网的设置
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static bool RestoreConnectionProxy(INTERNET_PER_CONN_OPTION_LIST request)
        {
            int size = Marshal.SizeOf(request);

            // 申请内存 
            IntPtr intptrStruct = Marshal.AllocCoTaskMem(size);

            // 转换数据结构到IntPtr 
            Marshal.StructureToPtr(request, intptrStruct, true);

            // 设置internet选项。
            bool bReturn = InternetSetOption(IntPtr.Zero,
                INTERNET_OPTION.INTERNET_OPTION_PER_CONNECTION_OPTION, 
                intptrStruct, size);

            // 释放内存
            Marshal.FreeCoTaskMem(request.pOptions);
            Marshal.FreeCoTaskMem(intptrStruct);

            if (!bReturn)
            {
                throw new ApplicationException(" Set Internet Option Failed! ");
            }

            // 通知系统注册值的改变，以便注册中的句柄能使用这些设定重读代理数据。
            InternetSetOption(IntPtr.Zero, INTERNET_OPTION.INTERNET_OPTION_SETTINGS_CHANGED,
                IntPtr.Zero,0);
            InternetSetOption(IntPtr.Zero, INTERNET_OPTION.INTERNET_OPTION_REFRESH,
                IntPtr.Zero,0);


            return bReturn;
        }
    }
}
