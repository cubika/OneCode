/********************************** 模块头**********************************\
模块名称:  ShellExtLib.cs
项目名称:      CSShellExtInfotipHandler
版权 (c) Microsoft Corporation.

本模块声明了 Shell interfaces 接口： IQueryInfo 和 
执行注册和取消注册一个shell infotip事件处理函数

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER 
EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF 
MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***********************************************************************************/

using System;
using Microsoft.Win32;
using System.Runtime.InteropServices;


namespace CSShellExtInfotipHandler
{
    #region Shell Interfaces

    [ComImport(), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("00021500-0000-0000-c000-000000000046")]
    internal interface IQueryInfo
    {
        // GetInfoTip原始的签名是 
        // HRESULT GetInfoTip(DWORD dwFlags, [out] PWSTR *ppwszTip);
        // 根据需求,执行这个方法的应用程序必须为ppwszTip（CoTaskMemAlloc的一个对象）分配内存空间
        // 当不在使用本应用程序的时候，要用CoTaskMemFree方法释放内存空间 
        // 在这个例子中，首先设置PreserveSig的值为false（在com组件中的默认值）作为输出参数 ppwszTip的返回值
        //  同时指定一个字符串最为LPWStr的返回值。
        // 在CLR的interop层，CoTaskMemAlloc 方法会分配内存空间 并且为.net 字符串指定内存。
        [return: MarshalAs(UnmanagedType.LPWStr)]
        string GetInfoTip(uint dwFlags);

        int GetInfoFlags();
    }

    #endregion


    #region Shell Registration

    internal class ShellExtReg
    {
        /// <summary>
        /// 注册shell信息提示处理事件
        /// </summary>
        /// <param name="t">COM class</param>
        /// <param name="fileType">
        ///  文件类型如：“*” 代表所有类型，".txt" 代表所有.txt 文件
        /// 参数不能为null 或者 空字符串
        /// 
        /// </param>
        /// <remarks>
        /// 执行本方法是会产生以下注册码.
        ///
        ///   HKCR
        ///   {
        ///      NoRemove &lt;File Type&gt;
        ///      {
        ///          NoRemove shellex
        ///          {
        ///              {00021500-0000-0000-C000-000000000046} = s '{&lt;CLSID&gt;}'
        ///          }
        ///      }
        ///   }
        /// </remarks>
        public static void RegisterShellExtInfotipHandler(Type t, string fileType)
        {
            if (string.IsNullOrEmpty(fileType))
            {
                throw new ArgumentException("文件类型不能为空");
            }

            // 如果文件类型以“.”开始，就试着去找包含ProgID的HKCR\<FIle Type>的默认值。
            if (fileType.StartsWith("."))
            {
                using (RegistryKey key = Registry.ClassesRoot.OpenSubKey(fileType))
                {
                    if (key != null)
                    {
                        // 如果key存在并且默认值不空，就用ProgID作为文件的类型
                        string defaultVal = key.GetValue(null) as string;
                        if (!string.IsNullOrEmpty(defaultVal))
                        {
                            fileType = defaultVal;
                        }
                    }
                }
            }

            // 产生注册码，并设置它的默认值作为处理事件的CLSID
            // HKCR\<File Type>\shellex\{00021500-0000-0000-C000-000000000046}, and 
            
            string keyName = string.Format(
                @"{0}\shellex\{{00021500-0000-0000-C000-000000000046}}", fileType);
            using (RegistryKey key = Registry.ClassesRoot.CreateSubKey(keyName))
            {
                // Set the default value of the key.
                if (key != null)
                {
                    key.SetValue(null, t.GUID.ToString("B"));
                }
            }
        }

        /// <summary>
        /// 取消注册shell 信息提示处理事件.
        /// </summary>
        /// <param name="fileType">
        ///  文件类型如：“*” 代表所有类型，".txt" 代表所有.txt 文件
        /// 参数不能为null 或者 空字符串 
        /// </param>
        /// <remarks>
        /// 执行本方法会删除以下注册码
        /// HKCR\&lt;File Type&gt;\shellex\{00021500-0000-0000-C000-000000000046}.
        /// </remarks>
        public static void UnregisterShellExtInfotipHandler(string fileType)
        {
            if (string.IsNullOrEmpty(fileType))
            {
                throw new ArgumentException("文件类型不能为空");
            }

            // 如果文件类型以“.”开始，就试着去找包含ProgID的HKCR\<FIle Type>的默认值。
            if (fileType.StartsWith("."))
            {
                using (RegistryKey key = Registry.ClassesRoot.OpenSubKey(fileType))
                {
                    if (key != null)
                    {
                        // 如果key存在并且默认值不空，就用ProgID作为文件的类型
                        string defaultVal = key.GetValue(null) as string;
                        if (!string.IsNullOrEmpty(defaultVal))
                        {
                            fileType = defaultVal;
                        }
                    }
                }
            }

            //删除以下注册码：
            // HKCR\<File Type>\shellex\{00021500-0000-0000-C000-000000000046}.
            string keyName = string.Format(
                @"{0}\shellex\{{00021500-0000-0000-C000-000000000046}}", fileType);
            Registry.ClassesRoot.DeleteSubKeyTree(keyName, false);
        }
    }

    #endregion
}