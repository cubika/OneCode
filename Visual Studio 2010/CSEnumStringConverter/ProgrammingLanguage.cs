/********************************* 模块头 **********************************\ 
* 模块名:    ProgrammingLanguage.cs 
* 项目名:    CSEnumStringConverter
* 版权(c) Microsoft Corporation. 
*
* 该文件定义了一个标记枚举以用于演示.
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
using System.ComponentModel;


namespace CSEnumStringConverter
{
    /// <summary>
    /// Programming languages (这是用于演示的标记枚举)
    /// </summary>
    [Flags]
    enum ProgrammingLanguage
    {
        [Description("Visual C#")]
        CS = 0x1,
        [Description("Visual Basic")]
        VB = 0x2, 
        [Description("Visual C++")]
        Cpp = 0x4,
        [Description("Javascript")]
        JS = 0x8,
        // XAML
        XAML = 0x10
    }
}
