/********************************** 模块头 **********************************\
 * 模块名:   UICommandBase.cs
 * 项目名称:      CSWPFIrregularShapeWindow
 * Copyright (c) Microsoft Corporation.
 *
 * UICommandBase.cs文件定义了一个 继承自 RoutedCommand 的 RoutedUICommand 通过实现快捷绑定到命令实例变量
 *
 * This source is subject to the Microsoft Public License.
 * See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
 * All other rights reserved.
 * 
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***********************************************************************************/

using System.Windows.Input;
using System.Windows;


namespace CSWPFIrregularShapeWindow
{
    public abstract class UICommandBase : Window
    {
        
        //<summary>
        // 最小化窗体的命令
        // 正如你所看到的，该命令也和键盘上的F3键挂钩
        //</summary>
        public static RoutedUICommand MinimizeCmd = new RoutedUICommand("MinimizeCmd",
                "MinimizeCmd", typeof(UICommandBase), new InputGestureCollection(
                    new InputGesture[] { new KeyGesture(Key.F3, ModifierKeys.None, "Minimize Cmd") }
                    ));

        /// <summary>
        /// 最大化窗体命令
        /// 正如你所看到的，该命令也和键盘上的F4键挂钩
        /// </summary>
        public static RoutedUICommand MaximizeCmd = new RoutedUICommand("MaximizeCmd",
                "MaximizeCmd", typeof(UICommandBase), new InputGestureCollection(
                    new InputGesture[] { new KeyGesture(Key.F4, ModifierKeys.None, "Maximize Cmd") }
                    ));

        /// <summary>
        /// 复原窗体命令
        /// 正如你所看到的，该命令也和键盘上的F5键挂钩
        /// </summary>
        public static RoutedUICommand RestoreCmd = new RoutedUICommand("RestoreCmd",
                "RestoreCmd", typeof(UICommandBase), new InputGestureCollection(
                    new InputGesture[] { new KeyGesture(Key.F5, ModifierKeys.None, "Restore Cmd") }
                    ));

        /// <summary>
        /// 关闭窗体命令
        /// 正如你所看到的，该命令也和键盘上的F6键挂钩
        /// </summary>
        public static RoutedUICommand CloseCmd = new RoutedUICommand("CloseCmd",
                "CloseCmd", typeof(UICommandBase), new InputGestureCollection(
                    new InputGesture[] { new KeyGesture(Key.F6, ModifierKeys.None, "Close Cmd") }
                    ));


        public UICommandBase()
        {
                      
        }

    }
}
