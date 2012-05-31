/********************************* 模块偷 *********************************\
模块名:  Program.cs
项目:    CSCallNativeDllWrapper
版权		 (c) Microsoft Corporation.

这个代码示例展示了通过C++/CLI封装类对一个本地C++的DLL模块的导出的类和方法进行封装，
并且被Visual C#代码调用。

  CSCallNativeDllWrapper (.NET应用程序)
          -->
      CppCLINativeDllWrapper (C++/CLI封装)
              -->
          CppDynamicLinkLibrary (本地C++ DLL模块)

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Runtime.InteropServices;
using CppCLINativeDllWrapper;


namespace CSCallNativeDllWrapper
{
    class Program
    {
        static void Main(string[] args)
        {
            bool isLoaded = false;
            const string moduleName = "CppDynamicLinkLibrary";

            // 检查模块是否被加载
            isLoaded = IsModuleLoaded(moduleName);
            Console.WriteLine("模块 \"{0}\" {1}被加载", moduleName, isLoaded ? "" : "没有");

            // 从模块中调用被导出的函数
            {
                string str = "HelloWorld";
                int length;

                length = NativeMethods.GetStringLength1(str);
                Console.WriteLine("GetStringLength1(\"{0}\") => {1}", str, length);

                length = NativeMethods.GetStringLength2(str);
                Console.WriteLine("GetStringLength2(\"{0}\") => {1}", str, length);
            }

            // 从模块中调用被导出的回调函数
            {
                CompareCallback cmpFunc = new CompareCallback(CompareInts);
                int max = NativeMethods.Max(2, 3, cmpFunc);

                // 确保该委托的实例的生命周期涵盖整个非托管代码 
                // 否则，该委托将无法使用，在垃圾回收后。
                // 你可能会收到访问冲突或非法指令错误 
                GC.KeepAlive(cmpFunc);
                Console.WriteLine("Max(2, 3) => {0}", max);
            }

            // 从模块中使用被导出类.
            {
                CSimpleObjectWrapper obj = new CSimpleObjectWrapper();
                obj.FloatProperty = 1.2F;
                float fProp = obj.FloatProperty;
                Console.WriteLine("Class: CSimpleObject::FloatProperty = {0:F2}", fProp);
            }

            // 你不通过调用GetModuleHandle 和 FreeLibrary
            // 能卸载C++ DLL CppDynamicLinkLibrary 
            
            // 检查模块是否被加载。
            isLoaded = IsModuleLoaded(moduleName);
            Console.WriteLine("模块 \"{0}\" {1}被加载", moduleName, isLoaded ? "" : "没有");
        }


        /// <summary>
        /// 这是从DLL CppDynamicLinkLibrary.dll中返回最大值的回调函数 
        /// </summary>
        /// <param name="a">the first integer</param>
        /// <param name="b">the second integer</param>
        /// <returns>
        /// 该函数返回一个正数，如果 a > b, 返回 0 如果 a 
        /// 等于 b, 返回一个负数，如果 if a < b.
        /// </returns>
        static int CompareInts(int a, int b)
        {
            return (a - b);
        }


        #region Module Related Helper Functions

        /// <summary>
        /// 检查特定的模块是否被当前进程加载
        /// </summary>
        /// <param name="moduleName">the module name</param>
        /// <returns>
        /// 函数返回一个真，如果特定模块被当前进程加载。如果没有加载则函数返回一个否
        /// </returns>
        static bool IsModuleLoaded(string moduleName)
        {
            // Get the module in the process according to the module name根据模块在进程中得到的模块名.
            IntPtr hMod = GetModuleHandle(moduleName);
            return (hMod != IntPtr.Zero);
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern IntPtr GetModuleHandle(string moduleName);

        #endregion
    }
}