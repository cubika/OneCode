/****************************** 模块头 *************************************\
* 模块名:      Program.cs
* 项目名:	   CSEnumerateAppDomains
* 版权 (c)     Microsoft Corporation.
* 
* 此文件用来获得输入指令。
* 如果应用程序带参数启动，直接执行指令并推出，否则就显示帮助字幕让用户选择指令
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
using Microsoft.Samples.Debugging.CorDebug;

namespace CSEnumerateAppDomains
{
    class Program
    {
        static void Main(string[] args)
        {

            // 在当前进程中创建新的应用程序域
            AppDomain.CreateDomain("Hello world!");
          
            try
            {

                // 如果程序没有带参数启动,则显示帮助字幕让用户选择指令
                if (args.Length == 0)
                {
                    // 除非用户输入了退出指令,否则应用程序不会退出.
                    // 如果指令不正确,则再次循环显示帮助字幕
                    while (true)
                    {
                        Console.WriteLine(@"
请选择一项命令：
1: 显示当前进程中的应用程序域.
2: 列举所以托管进程.
3: 显示帮助字幕.
4: 退出程序.
显示指定进程的应用程序域，请直接输入“PID”及进程ID，比如PID1234.
");

                        string cmd = Console.ReadLine();
                        int cmdID = 0;
                        if (int.TryParse(cmd, out cmdID))
                        {
                            switch (cmdID)
                            {
                                case 1:
                                    ProcessCommand("CurrentProcess");
                                    break;
                                case 2:
                                    ProcessCommand("ListAllManagedProcesses");
                                    break;
                                case 4:
                                    Environment.Exit(0);
                                    break;
                                default:
                                    // 下次循环再次显示帮助字幕
                                    break;

                            }
                        }
                        else if (cmd.StartsWith("PID", StringComparison.OrdinalIgnoreCase))
                        {
                            ProcessCommand(cmd);
                        }

                    }
                }
                else if (args.Length == 1)
                {
                    ProcessCommand(args[0]);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                // 退出代码100表示程序没有成功运行
                Environment.Exit(100);
            }
        }

        static void ProcessCommand(string arg)
        {
            // 列举当前进程中的应用程序域
            if (arg.Equals("CurrentProcess", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("正在列出当前进程中的应用程序域...");
                ShowAppDomainsInCurrentProcess();
            }

            // 列举所有托管进程
            else if (arg.Equals("ListAllManagedProcesses", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("正在列出所有托管进程...");
                ListAllManagedProcesses();
            }

            // 显示指定进程中的应用程序域,必须以"PID"开头
            else if (arg.StartsWith("PID", StringComparison.OrdinalIgnoreCase))
            {
                int pid = 0;
                int.TryParse(arg.Substring(3), out pid);
                Console.WriteLine(string.Format(
                    "正在列出进程编号{0}中的应用程序域 ...", pid));
                ShowAppDomains(pid);
            }

            else
            {
                throw new ArgumentException("请输入有效指令。");
            }

        }

        /// <摘要>
        /// 显示当前进程中的所有应用程序域
        /// </摘要> 
        static void ShowAppDomainsInCurrentProcess()
        {
            // GetAppDomainsInCurrentProcess是托管进程类的一个静态方法
            // 此方法用来获得当前进程中的所有应用程序域
            var appDomains = ManagedProcess.GetAppDomainsInCurrentProcess();

            foreach (var appDomain in appDomains)
            {
                Console.WriteLine("应用程序域 Id={0}, 名称={1}",
                    appDomain.Id, appDomain.FriendlyName);
            }
        }
        /// <摘要>
        /// 显示指定进程中的应用程序域
        /// </摘要>
        /// <param name="pid"> 参数进程ID</param>
        static void ShowAppDomains(int pid)
        {
            if (pid <= 0)
            {
                throw new ArgumentException("请输入有效指令。");
            }

            ManagedProcess process = null;
            try
            {
                // GetManagedProcessByID是托管进程类的一个静态方法
                // 此方法是用来获得一个托管进程类的实例。
                // 如果不存在相应PID的托管进程,将会抛出ArgumentException异常。
                process = ManagedProcess.GetManagedProcessByID(pid);

                foreach (CorAppDomain appDomain in process.AppDomains)
                {
                    Console.WriteLine("应用程序域 Id={0}, 名称={1}",
                        appDomain.Id,
                        appDomain.Name);
                }

            }
            catch (ArgumentException _argumentException)
            {
                Console.WriteLine(_argumentException.Message);
            }
            catch (ApplicationException _applicationException)
            {
                Console.WriteLine(_applicationException.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.GetType());
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine("不能获得该进程。"
                   + " 确保该进程存在，并且是托管进程。");
            }
            finally
            {
                if (process != null)
                {
                    process.Dispose();
                }
            }
        }

        /// <摘要>
        /// 列举所有的托管进程
        /// </摘要>
        static void ListAllManagedProcesses()
        {
            // GetManagedProcesses是托管进程类 的一个静态方法
            // 此方法用来获得当前机器上所有托管进程的列表
            var processes = ManagedProcess.GetManagedProcesses();

            foreach (var process in processes)
            {
                Console.WriteLine("ID={0}\t名称={1}",
                     process.ProcessID, process.ProcessName);
                Console.Write("加载运行时: ");
                foreach (var runtime in process.LoadedRuntimes)
                {
                    Console.Write(runtime.GetVersionString() + "\t");
                }
                Console.WriteLine("\n");
            }

        }

    }
}
