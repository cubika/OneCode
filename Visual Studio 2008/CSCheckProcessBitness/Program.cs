/****************************** 模块头 ******************************\
* 模块名:  Program.cs
* 项目名:  CSCheckProcessBitness
* 版权 (c) Microsoft Corporation.
* 
* 这个实例代码演示了如何确定一个给定的进程是64位的还是32位的 
*  
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/


#region Using directives
using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
#endregion


namespace CSCheckProcessBitness
{
    class Program
    {
        #region 本地方法

        const Int32 PROCESS_QUERY_INFORMATION = 0x0400;


        /// <summary>
        /// 打开一个已经存在的本地进程对象.
        /// </summary>
        /// <param name="dwDesiredAccess">
        /// 访问进程对象.. 
        /// 这种访问权是核对该进程的安全描述符 
        /// 这个参数可以提供是一个或多个进程访问的权利。
        /// </param>
        /// <param name="bInheritHandle">
        /// 如果这个值是true,被该进程创建的进程会继承这个句柄.
        /// 否则，不会继承这个句柄.
        /// </param>
        /// <param name="dwProcessId">
        /// 标识要打开的本地进程
        /// </param>
        /// <returns>如果函数运行成功，返回值是一个指定进程打开的句柄. 
        /// 如果运行失败，返回一个NULL.
        /// </returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr OpenProcess(Int32 dwDesiredAccess,
            [MarshalAs(UnmanagedType.Bool)]bool bInheritHandle, 
            Int32 dwProcessId);


        /// <summary>
        /// 检索模块句柄指定的模块.
        /// </summary>
        /// <param name="moduleName">
        /// 已经加载了的模块的名称(一个.dll或者.exe文件)
        /// </param>
        /// <returns>
        /// 如果函数成功运行，返回值是一个指定模块的句柄.
        /// 如果函数运行失败，返回值是NULL.
        /// </returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr GetModuleHandle(string moduleName);


        /// <summary>
        ///  检索导出函数或指定的动态链接库（DLL）的变量的地址.
        /// </summary>
        /// <param name="hModule">
        /// 一个包含该函数或变量的DLL模块句柄.
        /// </param>
        /// <param name="procName">
        ///  函数或变量名，或函数的序号值.
        /// </param>
        /// <returns>
        /// 如果函数成功运行，返回值是导出的函数或变量的地址.
        /// 如果函数运行失败，返回值是NULL.
        /// </returns>
        [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
        static extern IntPtr GetProcAddress(IntPtr hModule,
            [MarshalAs(UnmanagedType.LPStr)]string procName);


        /// <summary>
        /// 确定指定的进程是否在WOW64下运行.
        /// </summary>
        /// <param name="hProcess">
        /// 该进程的句柄.
        /// </param>
        /// <param name="wow64Process">
        /// 如果该进程在WOW64下运行，一个指针的值设置为TRUE. 
        /// 如果该进程在32位windows下运行，这个值设置为FALSE. 
        /// 如果这个进程是一个运行在64位Windows上的64位应用程序，该值也设置为FALSE.
        /// </param>
        /// <returns>
        /// 如果函数成功运行，返回值是非零的.
        /// 如果函数运行失败，返回值是0.
        /// </returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsWow64Process(IntPtr hProcess, out bool wow64Process);


        #endregion

        public static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                //如果指定一个进程的ID作为参数.
                Int32 processId = 0;

                if (Int32.TryParse(args[0], out processId))
                {
                    IntPtr hProcess = OpenProcess(PROCESS_QUERY_INFORMATION,
                        false,
                        processId);

                    if (hProcess != IntPtr.Zero)
                    {
                        if (Is64BitProcess(hProcess))
                        {
                            Console.WriteLine("该进程是一个64位进程.");
                        }
                        else
                        {
                            Console.WriteLine("该进程是一个32位进程.");
                        }
                    }
                    else
                    {
                        Int32 errorCode = Marshal.GetLastWin32Error();
                        Console.WriteLine(
                            "错误发生在打开进程{0},错误代码是:{1}",
                            processId.ToString(), errorCode.ToString());
                    }
                }
                else
                {
                    Console.WriteLine(
                        "检测到不可用的参数，请输入一个进程Id.");
                }
            }
            else
            {
                // 如果没有指定进程ID，则使用当前进程的ID.
                IntPtr hProcess = Process.GetCurrentProcess().Handle;
                if (Is64BitProcess(hProcess))
                {
                    Console.WriteLine("当前进程是64位的.");
                }
                else
                {
                    Console.WriteLine("当前进程是32位的.");
                }
            }
            Console.Read();
        }


        /// <summary>
        ///  该函数判断指定的进程是否是64位的.
        /// </summary>
        /// <param name="hProcess">该进程的句柄</param>
        /// <returns>
        ///  如果指定的进程是64位的，函数返回TRUE;
        ///  否则返回FALSE.
        /// </returns>
        static bool Is64BitProcess(IntPtr hProcess)
        {
            bool flag = false;

            if (Is64BitOperatingSystem())
            {
                //在64位操作系统，如果一个进程是没有在WOW64模式下运行，这个进程一定是一个64位的.
                flag = !(IsWow64Process(hProcess, out flag) && flag);
            }

            return flag;
        }


        /// <summary>
        /// 该函数判断当前操作系统是否是64位操作系统.
        /// </summary>
        /// <returns>
        /// 如果操作系统是64位，该函数返回true，否则返回false
        /// </returns>
        static bool Is64BitOperatingSystem()
        {
            if (IntPtr.Size == 8)  //64位程序只能运行在windows x64里
            {
                return true;
            }
            else  // 32位程序运行在32位和64位windows下.
            {
                //检测是否当前进程是一个运行在64位系统上的32位进程。
                bool flag;
                return (DoesWin32MethodExist("kernel32.dll", "IsWow64Process") &&
                    (IsWow64Process(Process.GetCurrentProcess().Handle, 
                        out flag) && flag));
            }
        }


        /// <summary>
        /// 该函数确定是否方法存在于一个特定的模块导出表. 
        /// </summary>
        /// <param name="moduleName">模块的名称</param>
        /// <param name="methodName">方法的名称</param>
        /// <returns>
        /// 如果由methodName指定的的方法存在于模块名指定的模块导出表，该函数返回true.
        /// </returns>
        static bool DoesWin32MethodExist(string moduleName, string methodName)
        {
            IntPtr moduleHandle = GetModuleHandle(moduleName);
            if (moduleHandle == IntPtr.Zero)
            {
                return false;
            }
            return (GetProcAddress(moduleHandle, methodName) != IntPtr.Zero);
        }

    }
}
