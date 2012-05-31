/********************************** 模块头 ****************************************\
 * 模块名:  AvalonCommandsHelper.cs
 * 项目名称:      CSWPFIrregularShapeWindow
 * Copyright (c) Microsoft Corporation.
 *
 * AvalonCommandsHelper.cs文件定义了一个CanExecuteCommandSource方法，该方法负责是否可以运行
 * 一个特殊的命令。
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
    /// <summary>
    /// WPF 命令的辅助类
    /// </summary>
    public static class AvalonCommandsHelper
    {
        /// <summary>
        /// 当可以执行时给予一个特殊的方法
        /// </summary>
        /// <param name="commandSource">核实命令</param>
        /// <returns></returns>
        public static bool CanExecuteCommandSource(ICommandSource commandSource)
        {
            ICommand baseCommand = commandSource.Command;
            if (baseCommand == null)
                return false;


            object commandParameter = commandSource.CommandParameter;
            IInputElement commandTarget = commandSource.CommandTarget;
            RoutedCommand command = baseCommand as RoutedCommand;
            if (command == null)
                return baseCommand.CanExecute(commandParameter);
            if (commandTarget == null)
                commandTarget = commandSource as IInputElement;
            return command.CanExecute(commandParameter, commandTarget);
        }
    }
}
