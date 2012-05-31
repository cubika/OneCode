/********************************* 模块头 **********************************\ 
* 模块名:    Program.cs 
* 项目名:    CSEnumStringConverter
* 版权 (c) Microsoft Corporation. 
*
*
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.

* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE. 
\***************************************************************************/

using System;
using System.ComponentModel;


namespace CSEnumStringConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("使用 EnumConverter ");
            {
                EnumConverter converter = new EnumConverter(typeof(ProgrammingLanguage));

                // 将字符串转换为枚举.
                string langStr = "CS, Cpp, XAML";
                Console.WriteLine("将字符串 \"{0}\" 转换为枚举...", langStr);
                ProgrammingLanguage lang = (ProgrammingLanguage)converter.ConvertFromString(langStr);
                Console.WriteLine("完成!");

                // 将枚举转换为字符串.
                Console.WriteLine("将枚举结果转换为字符串...");
                langStr = converter.ConvertToString(lang);
                Console.WriteLine("完成! \"{0}\"", langStr);
            }

            Console.WriteLine("\n使用 EnumDescriptionConverter ");
            {
                EnumDescriptionConverter converter = new EnumDescriptionConverter(
                    typeof(ProgrammingLanguage));

                // 将枚举转换为字符串.
                string langStr = "Visual C#, Visual C++, XAML";
                Console.WriteLine("将字符串 \"{0}\" 转换为枚举...", langStr);
                ProgrammingLanguage lang = (ProgrammingLanguage)converter.ConvertFromString(langStr);
                Console.WriteLine("完成!");

                // 将枚举转换为字符串.
                Console.WriteLine("将枚举结果转换为字符串...");
                langStr = converter.ConvertToString(lang);
                Console.WriteLine("完成! \"{0}\"", langStr);
            }


            Console.ReadLine();
        }
    }
}