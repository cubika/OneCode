/****************************** 模块头 ******************************\
 模块名:  RunningProcess.cs
 项目:      CSCheckProcessType
 版权 (c) Microsoft Corporation.
 
 这个类显示了一个运行时的进程，并判定这个进程是否是一个64位进程、托管进程、.NET进程，
 WPF进程或控制台进程。
 
 为了判定一个进程是否是一个运行于64位操作系统的64位进程，我们可以使用 Windows API中的
 IsWow64Process函数。
 
 为了判定一个进程是否是一个托管进程，我们可以检查.NET运行时执行引擎 MSCOREE.dll是否被
 加载。
 
 为了判定一个进程是否是一个托管进程，我们可以检查CLR.dll是否被加载。.NET 4.0之前的版本
 中，工作站的公共语言运行时是 MSCORWKS.DLL。而在.NET4.0版本中，这个DLL被替换成
 CLR.dll。
 
 为了判定一个进程是否是一个WPF进程，我们可以检查PresentationCore.dll是否被加载。
 
 为了判定一个进程是否是一个控制台进程，我们可以检查目标进程是否是一个控制台窗口。
 
  
 This source is subject to the Microsoft Public License.
 See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
 All other rights reserved.
 
 THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
 EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Permissions;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;
using System.ComponentModel;

namespace CSCheckProcessType
{
    [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
    public class RunningProcess
    {

        public static bool IsOSVersionSupported
        {
            get
            {
                return Environment.OSVersion.Version.Major >= 6;
            }
        }

        // System.Diagnostics.Process 实例。
        Process diagnosticsProcess;

        /// <summary>
        ///进程名称。
        /// </summary>
        public string ProcessName
        {
            get
            {
                return this.diagnosticsProcess.ProcessName;
            }
        }

        /// <summary>
        /// 进程ID。
        /// </summary>
        public int Id
        {
            get
            {
                return this.diagnosticsProcess.Id;
            }
        }

        /// <summary>
        /// 指定进程是否是一个托管的应用程序
        /// </summary>
        public bool IsManaged { get; private set; }

        /// <summary>
        /// 指定进程是否是一个.Net4.0应用程序。
        /// </summary>
        public bool IsDotNet4 { get; private set; }

        /// <summary>
        /// 指定进程是否是否是一个控制台应用程序。
        /// </summary>
        public bool IsConsole { get; private set; }

        /// <summary>
        /// 指定进程是否是一个WPF应用程序。
        /// </summary>
        public bool IsWPF { get; private set; }

        /// <summary>
        /// 指定进程是否是一个64位应用程序。
        /// </summary>
        public bool Is64BitProcess { get; private set; }

        /// <summary>
        /// 实例备注。通常是异常信息。
        /// </summary>
        public string Remarks { get; private set; }

        public RunningProcess(Process proc)
        {
            this.diagnosticsProcess = proc;

            try
            {
                CheckProcess();
            }
            catch (Exception ex)
            {
                this.Remarks = ex.Message;
            }
        }

        /// <summary>
        /// 检查进程属性。
        /// </summary>
        public void CheckProcess()
        {
            uint procID = (uint)this.diagnosticsProcess.Id;

            //用kernel32.dll附加进程控制台到Windows窗体上。
            if (NativeMethods.AttachConsole(procID))
            {
                
                // 正如之前附加进程控制台一样，使用Kernel32.dll 来获得当前进程（Windows窗体）
                //的句柄。
                IntPtr handle = NativeMethods.GetStdHandle(NativeMethods.STD_OUTPUT_HANDLE);
                uint lp = 0;
                this.IsConsole = NativeMethods.GetConsoleMode(handle, out lp);
                NativeMethods.FreeConsole();
            }

            List<string> loadedModules = this.GetLoadedModules();

            //  检查.NET运行时执行引擎MSCOREE.dll是否被加载。
            this.IsManaged = loadedModules.Count(m => m.Equals("MSCOREE.dll",
                StringComparison.OrdinalIgnoreCase)) > 0;
            if (this.IsManaged)
            {

                //检查CLR.dll是否被加载。
                this.IsDotNet4 = loadedModules.Count(m => m.Equals("CLR.dll",
                    StringComparison.OrdinalIgnoreCase)) > 0;

                // 检查PresentationCore.dll是否被加载。
                this.IsWPF = loadedModules.Count(m =>
                    m.Equals("PresentationCore.dll", StringComparison.OrdinalIgnoreCase)
                    || m.Equals("PresentationCore.ni.dll", StringComparison.OrdinalIgnoreCase)) > 0;
            }

            this.Is64BitProcess = Check64BitProcess();
        }

        /// <summary>
        /// 使用EnumProcessModulesEx函数来获取所有加载的模块。
        /// EnumProcessModulesEx函数只能运行在Windows Vista或更好版本的Windows操作系统中。
        /// </summary>
        /// <returns></returns>
        List<string> GetLoadedModules()
        {
            if (Environment.OSVersion.Version.Major < 6)
            {
                throw new ApplicationException("该应用程序必须运行在Windows Vista或更高版本的" +
                    "操作系统。");
            }

            IntPtr[] modulesHandles = new IntPtr[1024];
            int size = 0;

            var success = NativeMethods.EnumProcessModulesEx(
                this.diagnosticsProcess.Handle,
                modulesHandles,
                Marshal.SizeOf(typeof(IntPtr))*modulesHandles.Length,
                out size,
                NativeMethods.ModuleFilterFlags.LIST_MODULES_ALL);

            if (!success)
            {
                throw new Win32Exception();
            }

            List<string> moduleNames = new List<string>();

            for (int i = 0; i < modulesHandles.Length; i++)
            {
                if (modulesHandles[i] == IntPtr.Zero)
                {
                    break;
                }

                StringBuilder moduleName = new StringBuilder(1024);

                uint length = NativeMethods.GetModuleFileNameEx(
                    this.diagnosticsProcess.Handle,
                    modulesHandles[i],
                    moduleName,
                    (uint)moduleName.Capacity);

                if (length <= 0)
                {
                    throw new Win32Exception();
                }
                else
                {
                    var fileName = Path.GetFileName(moduleName.ToString());
                    moduleNames.Add(fileName);
                }
            }

            return moduleNames;
        }

        /// <summary>
        /// 判定指定的进程是否是一个64位进程。
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <returns>
        /// 如果是64位进程，返回True；否则，返回false.
        /// </returns>
        bool Check64BitProcess()
        {
            bool flag = false;

            if (Environment.Is64BitOperatingSystem)
            {

                //在64位操作系统中，如果一个进程不在Wow64模式下运行，
                //该进程一定是一个64位进程。
                flag = !(NativeMethods.IsWow64Process(this.diagnosticsProcess.Handle, out flag) && flag);
            }

            return flag;
        }
    }
}
