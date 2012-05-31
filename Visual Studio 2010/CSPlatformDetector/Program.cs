/******************************** 模块头 ********************************\
模块名:  Program.cs
项目名:      CSPlatformDetector
版权 (c) Microsoft Corporation.

CSPlatformDetector  演示以下几个与平台检测相关的任务：

' 1. 检测当前操作系统的名字（例如："Microsoft Windows 7 企业版"）
' 2. 检测当前操作系统的版本（例如："Microsoft Windows NT 6.1.7600.0")
' 3. 确定当前的操作系统是否是64位的系统。
' 4. 确定当前的进程是否是64位的进程。
' 5. 确定任意一个进程是否运行在64位系统。
' 

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*******************************************************************************/

using System;
using System.Runtime.InteropServices;
using System.Management;


namespace CSPlatformDetector
{
    class Program
    {
        static void Main(string[] args)
        {
            // 打印当前操作系统的名字.
            Console.WriteLine("当前的操作系统是: " + GetOSName());

            //  打印当前操作系统的版本字符串.
            Console.WriteLine("版本: " + Environment.OSVersion.VersionString);

            // 确定当前操作系统是否是64位.
            Console.WriteLine("当前的操作系统 {0}64位",
                Environment.Is64BitOperatingSystem ? "" : "不是 ");

            // 确定当前的进程是否是64位的. 
            Console.WriteLine("当前的进程 {0}64位",
                Environment.Is64BitProcess ? "" : "不是 ");

            // 确定运行在系统中的任意一个进程是否是64位进程.
            if (args.Length > 0)
            {
                // 如果一个进程ID在命令行中被指定，则获取进程ID，并打开进程句柄.
                int processId = 0;
                if (int.TryParse(args[0], out processId))
                {
                    IntPtr hProcess = NativeMethods.OpenProcess(
                        NativeMethods.PROCESS_QUERY_INFORMATION, false, processId);
                    if (hProcess != IntPtr.Zero)
                    {
                        try
                        {
                            // 检测指定进程是否是64位.
                            bool is64bitProc = Is64BitProcess(hProcess);
                            Console.WriteLine("进程 {0}  {1}64位",
                                processId.ToString(), is64bitProc ? "" : "不是 ");
                        }
                        finally
                        {
                            NativeMethods.CloseHandle(hProcess);
                        }
                    }
                    else
                    {
                        int errorCode = Marshal.GetLastWin32Error();
                        Console.WriteLine("打开进程({0})失败 w/err 0x{1:X}",
                            processId.ToString(), errorCode.ToString());
                    }
                }
                else
                {
                    Console.WriteLine("无效的进程 ID: {0}", processId.ToString());
                }
            }
        }


        /// <summary>
        /// 获得当前运行的操作系统的名字.例如，
        /// "Microsoft Windows 7 企业版".
        /// </summary>
        /// <returns>当前运行的操作系统的名字</returns>
        static string GetOSName()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                "root\\CIMV2", "SELECT Caption FROM Win32_OperatingSystem");

            foreach (ManagementObject queryObj in searcher.Get())
            {
                return queryObj["Caption"] as string;
            }

            return null;
        }


        /// <summary>
        /// 确定一个特定的进程是否是64位进程.
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <returns>
        /// 如果此进程是64位返回true；否则返回false.
        /// </returns>
        static bool Is64BitProcess(IntPtr hProcess)
        {
            bool flag = false;

            if (Environment.Is64BitOperatingSystem)
            {
                //  在一个64位的操作系统中，如果一个进程不是运行在Wow64模式下，
                //  这个进程就一定是一个64位进程.
                flag = !(NativeMethods.IsWow64Process(hProcess, out flag) && flag);
            }

            return flag;
        }
    }
}