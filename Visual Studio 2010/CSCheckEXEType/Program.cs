/****************************** 模块头 ******************************\
* 模块名:  Program.cs
* 项目名:	    CSCheckEXEType
* 版权 (c) Microsoft Corporation.
* 
* 这个源文件用于处理输入的命令.
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
using System.IO;

namespace CSCheckEXEType
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("请键入EXE文件路径:");
                Console.WriteLine("<直接回车退出>");
                string path = Console.ReadLine();

                if (string.IsNullOrEmpty(path))
                {
                    break;
                }

                if (!File.Exists(path))
                {
                    Console.WriteLine("路径不存在!");
                    continue;
                }
                try
                {
                    ExecutableFile exeFile = new ExecutableFile(path);

                    var isConsole = exeFile.IsConsoleApplication;
                    var isDotNet = exeFile.IsDotNetAssembly;
                    

                    Console.WriteLine(string.Format(
@"控制台应用程序: {0}
.Net应用程序: {1}",
isConsole, isDotNet));

                    if (isDotNet)
                    {
                        Console.WriteLine(".NET编译运行时: " + exeFile.GetCompiledRuntimeVersion());
                        Console.WriteLine("全名: " + exeFile.GetFullDisplayName());
                        var attributes = exeFile.GetAttributes();
                        foreach (var attribute in attributes)
                        {
                            Console.WriteLine(string.Format("{0}: {1}",
                                attribute.Key,attribute.Value));
                        }
                    }
                    else
                    {
                        var is32Bit = exeFile.Is32bitImage;
                        Console.WriteLine("32位 应用程序: " + is32Bit);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                Console.WriteLine();
            }

        }
    }
}

