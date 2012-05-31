/*************************************** 模块头*****************************\
* 模块名:  OpenImageMenuExt.cs
* 项目名:  CSCustomIEContextMenu
* 版权 (c) Microsoft Corporation.
* 
* 当该程序集被注册 / 注销时, 用类 OpenImageMenuExt 来在注册表中添加 / 移除菜单.
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
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace CSCustomIEContextMenu
{
    public class OpenImageMenuExt
    {
        private const string IEMenuExtRegistryKey =
            "Software\\Microsoft\\Internet Explorer\\MenuExt";

        public static void RegisterMenuExt()
        {

            // 如果该键存在， CreateSubKey 将会打开它.
            RegistryKey ieMenuExtKey = Registry.CurrentUser.CreateSubKey(
                IEMenuExtRegistryKey + "\\在新选项卡中打开图像");


            // 获取 Resource\OpenImage.htm 的路径.
            FileInfo fileIofo = new FileInfo(Assembly.GetExecutingAssembly().Location);
            string path = fileIofo.Directory.FullName + "\\Resource\\OpenImage.htm";

            // 设置项的默认值为该路径.
            ieMenuExtKey.SetValue(string.Empty, path);

            // 设置值的名称.
            ieMenuExtKey.SetValue("Name", "Open_Image");

            // 设置上下文的值，通过使用一个由下列值逻辑或组成的位掩,以指示您的项目将
            // 在标准上下文菜单中显示哪一种文本:
            // Default 0x1 
            // Images 0x2 
            // Controls 0x4 
            // Tables 0x8 
            // Text selection 0x10 
            // Anchor 0x20 
            ieMenuExtKey.SetValue("Contexts", 0x2);

            ieMenuExtKey.Close();
        }

        public static void UnRegisterMenuExt()
        {

            RegistryKey ieMenuExtskey = Registry.CurrentUser.OpenSubKey(
                IEMenuExtRegistryKey, true);

            if (ieMenuExtskey != null)
            {
                ieMenuExtskey.DeleteSubKey("在新选项卡中打开图像", false);
            }

            ieMenuExtskey.Close();
        }
    }
}
