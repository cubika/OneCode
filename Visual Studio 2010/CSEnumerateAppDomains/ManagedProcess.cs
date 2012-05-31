/****************************** 模块头*************************************\
* 模块名:    ManagedProcess.cs
* 项目名:    CSEnumerateAppDomains
* 版权(c)    Microsoft Corporation.
* 
* 此类实现了一个托管进程，包含一个MDbgProcess及一个System.Diagnostics.Process，
* 同时提供了3个静态方法GetManagedProcesses，GetManagedProcessByID和
* GetAppDomainsInCurrentProcess。
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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.Samples.Debugging.CorDebug;
using Microsoft.Samples.Debugging.MdbgEngine;
using mscoree;

namespace CSEnumerateAppDomains
{
    [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
    [PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
    public class ManagedProcess : IDisposable
    {
        private bool disposed = false;


        // 定义了一个执行可控的进程
        private MDbgProcess _mdbgProcess;


        ///<摘要>
        /// 不要初始化MDbgProcess，直到它被调用时再执行初始化
        ///</摘要>
        private MDbgProcess MDbgProcess
        {
            get
            {
                if (_mdbgProcess == null)
                {
                    try
                    {

                        // 初始化MDbgEngine对象
                        MDbgEngine engine = new MDbgEngine();


                        // 给指定进程附加一个调试器
                        // 返回代表此进程的MDbgProcess实例
                        
                        if (this.LoadedRuntimes != null && this.LoadedRuntimes.Count() == 1)
                        {
                            _mdbgProcess = engine.Attach(ProcessID, 
                                LoadedRuntimes.First().GetVersionString());
                        }
                        else
                        {
                            _mdbgProcess = engine.Attach(ProcessID, 
                                CorDebugger.GetDefaultDebuggerVersion());
                        }

                        // 如果进程不是处于同步状态，等待2秒
                        // 某些进程只能当其处于同步状态时才能被附加
                        // 参考 http://msdn.microsoft.com/en-us/library/ms404528.aspx

                        bool result = _mdbgProcess.Go().WaitOne(2000);
                        
                        if (!result)
                        {
                            throw new ApplicationException(
                                string.Format(@"进程编号 {0} 的进程不能被附加。"
                                +"操作超时。", ProcessID));
                        }

                    }
                    catch (COMException)
                    {
                        throw new ApplicationException(
                            string.Format(@"进程编号 {0} 的进程不能被附加。 "
                            + "操作被拒绝或者该进程已被附加", ProcessID));
                    }
                    catch
                    {
                        throw;
                    }

                }

                return _mdbgProcess;
            }
        }

        private System.Diagnostics.Process _diagnosticsProcess = null;

        /// <摘要>
        /// 通过ProcessID获得System.Diagnostics.Process
        /// </摘要>
        public System.Diagnostics.Process DiagnosticsProcess
        {
            get
            {
                return _diagnosticsProcess;
            }
        }

 
        /// <摘要>
        /// 进程ID
        /// </摘要>
        public int ProcessID
        {
            get
            {
                return DiagnosticsProcess.Id;
            }
        }

        /// <摘要>
        /// 进程名称
        /// </摘要>
        public string ProcessName
        {
            get
            {
                return DiagnosticsProcess.ProcessName;
            }
        }


        /// <摘要>
        /// 获得进程中加载的所有运行时
        /// </摘要>
        public IEnumerable<CLRRuntimeInfo> LoadedRuntimes
        {
            get
            {
                try
                {
                    CLRMetaHost host = new CLRMetaHost();
                    return host.EnumerateLoadedRuntimes(ProcessID);
                }
                catch (EntryPointNotFoundException)
                {
                    return null;
                }
            }
        }


        /// <摘要>
        /// 获得进程中的所有应用程序域
        /// </摘要>
        public IEnumerable AppDomains
        {
            get
            {
                var _appDomains = MDbgProcess.CorProcess.AppDomains;
                return _appDomains;
            }
        }


        private ManagedProcess(int processID)
        {
            System.Diagnostics.Process diagnosticsProcess =
                System.Diagnostics.Process.GetProcessById(processID);
            this._diagnosticsProcess = diagnosticsProcess;


            // 确定指定进程为托管进程
            if (this.LoadedRuntimes == null || this.LoadedRuntimes.Count() == 0)
            {
                throw new ArgumentException("此进程为非托管进程 ");
            }
        }


        private ManagedProcess(System.Diagnostics.Process diagnosticsProcess)
        {
            if (diagnosticsProcess == null)
            {
                throw new ArgumentNullException("diagnosticsProcess",
                    "System.Diagnostics.Process不能为空 ");
            }
            this._diagnosticsProcess = diagnosticsProcess;
            if (this.LoadedRuntimes == null || this.LoadedRuntimes.Count() == 0)
            {
                throw new ArgumentException("此进程为非托管进程。");
            }
        }



        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        protected virtual void Dispose(bool disposing)
        {

            // 防止被多次调用
            if (disposed) return;

            if (disposing)
            {

                // 清除所有托管资源
                if (_mdbgProcess != null)
                {

   
                    // 确保基本进程在datach前已经终止了
                    var waithandler = _mdbgProcess.AsyncStop();
                    waithandler.WaitOne();
                    _mdbgProcess.Detach();

                }
            }

            disposed = true;
        }


        /// <摘要>
        ///获得所有托管进程
        /// </摘要>
        public static List<ManagedProcess> GetManagedProcesses()
        {
            List<ManagedProcess> managedProcesses = new List<ManagedProcess>();

            // CLR宿主包含一个ICLRMetaHost接口，提供了一个方法，可以列举指定进程加载的所有运行时
            CLRMetaHost host = new CLRMetaHost();

            var processes = System.Diagnostics.Process.GetProcesses();

            foreach (System.Diagnostics.Process diagnosticsProcess in processes)
            {
                try
                {

                    // 列举指定进程加载的所有运行时
                    var runtimes = host.EnumerateLoadedRuntimes(diagnosticsProcess.Id);

                    // 如果进程加载了CLR，则被认定为托管进程
                    if (runtimes != null && runtimes.Count() > 0)
                    {
                        managedProcesses.Add(new ManagedProcess(diagnosticsProcess));
                    }

                }

                // 当文件无法找到或操作被拒绝时，EnumerateLoadedRuntimes方法会抛出Win32Exception异常
                // 例如:目标进程是系统进程或系统闲置进程
                catch (Win32Exception)
                { }


                // 在x86平台上创建的程序试图在64位操作系统上执行64位进程时，
                // EnumerateLoadedRuntimes方法会抛出COMException异常
                catch (COMException)
                { }


                // 再次抛出其他异常
                catch
                {
                    throw;
                }
            }
            return managedProcesses;
        }


        /// <摘要>
        /// 通过ID获得托管进程。
        /// 这个方法用于从其他进程中获得应有程序域。如果想要从当前进程中获得应用程序域，
        /// 请使用静态方法GetAppDomainsInCurrentProcess。
        /// </摘要>
        /// <exception cref="ArgumentException">
        /// 当不存在相应ID的托管进程时，会抛出ArgumentException异常
        /// </exception>
        public static ManagedProcess GetManagedProcessByID(int processID)
        {
            if (processID == System.Diagnostics.Process.GetCurrentProcess().Id)
            {
                throw new System.ArgumentException("不能调试当前进程。");
            }
            return new ManagedProcess(processID);
        }

        /// <摘要>
        /// 获得当前进程中的所有应用程序域
        /// 此方法用ICorRuntimeHost接口获得当前进程中的应用程序域的枚举
        /// </摘要>
        public static List<AppDomain> GetAppDomainsInCurrentProcess()
        {
            var appDomains = new List<AppDomain>();
            var hEnum = IntPtr.Zero;

            // CorRuntimeHostClass提供了ICorRuntimeHost接口
            var host = new CorRuntimeHost();

            try
            {

                // 获得当前进程中的所有应用程序域
                host.EnumDomains(out hEnum);
                while (true)
                {
                    object domain;
                    host.NextDomain(hEnum, out domain);
                    if (domain == null)
                        break;
                    appDomains.Add(domain as AppDomain);
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                host.CloseEnum(hEnum);
                Marshal.ReleaseComObject(host);
            }
            return appDomains;
        }

    }
}
