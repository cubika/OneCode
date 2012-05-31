/********************************** 模块头 **********************************\
 * 模块名:        EllipseWindow.xaml.cs
 * 项目名称:      CSWPFIrregularShapeWindow
 * Copyright (c) Microsoft Corporation.
 *
 * EllipseWindow.xaml.cs文件定义了一个EllipseWindow类，该方法负责命令绑定和命令与菜单选项
 * 或者按钮的关系.
 *
 * This source is subject to the Microsoft Public License.
 * See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
 * All other rights reserved.
 * 
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***********************************************************************************/

using System.Windows;
using System.Windows.Input;

namespace CSWPFIrregularShapeWindow
{
    /// <summary>
    /// EllipseWindow.xaml的逻辑交互
    /// </summary>
    public partial class EllipseWindow : Window
    {
        public EllipseWindow()
        {
            InitializeComponent();

            // 向下面这样添加创建一个对象所需要的代码
            CommandBinding cb = new CommandBinding();
            cb.Command = UICommandBase.CloseCmd;
            cb.Executed += new ExecutedRoutedEventHandler(CloseCmdExecuted);
            cb.CanExecute += new CanExecuteRoutedEventHandler(CloseCmdCanExecute);

            CommandBinding minb = new CommandBinding();
            minb.Command = UICommandBase.MinimizeCmd;
            minb.Executed += new ExecutedRoutedEventHandler(MinimizeCmdExecuted);
            minb.CanExecute += new CanExecuteRoutedEventHandler(MinimizeCmdCanExecute);

            CommandBinding maxb = new CommandBinding();
            maxb.Command = UICommandBase.MaximizeCmd;
            maxb.Executed += new ExecutedRoutedEventHandler(MaximizeCmdExecuted);
            maxb.CanExecute += new CanExecuteRoutedEventHandler(MaximizeCmdCanExecute);

            CommandBinding restoreb = new CommandBinding();
            restoreb.Command = UICommandBase.RestoreCmd;
            restoreb.Executed += new ExecutedRoutedEventHandler(RestoreCmdExecuted);
            restoreb.CanExecute += new CanExecuteRoutedEventHandler(RestoreCmdCanExecute);

            // 为命令对象添加设置以使其可以运行
            this.CommandBindings.Add(cb);
            this.CommandBindings.Add(minb);
            this.CommandBindings.Add(maxb);
            this.CommandBindings.Add(restoreb);

            this.mnuInvokeClose.Command = UICommandBase.CloseCmd;
            this.mnuInvokeClose.CommandTarget = btnInvokeClose;
            this.btnInvokeClose.Command = UICommandBase.CloseCmd;

            this.mnuInvokeMaximize.Command = UICommandBase.MaximizeCmd;
            this.mnuInvokeMaximize.CommandTarget = btnInvokeMaximize;
            this.btnInvokeMaximize.Command = UICommandBase.MaximizeCmd;

            this.mnuInvokeMinimize.Command = UICommandBase.MinimizeCmd;
            this.mnuInvokeMinimize.CommandTarget = btnInvokeMinimize;
            this.btnInvokeMinimize.Command = UICommandBase.MinimizeCmd;

            this.mnuInvokeRestore.Command = UICommandBase.RestoreCmd;
            this.mnuInvokeRestore.CommandTarget = btnInvokeRestore;
            this.btnInvokeRestore.Command = UICommandBase.RestoreCmd;

        }

        /// <summary>
        /// 关闭窗口行为
        /// </summary>
        private void CloseCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        ///使关闭窗体命令有效
        /// </summary>
        private void CloseCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        /// <summary>
        /// 最小化窗体
        /// </summary>
        private void MinimizeCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// 实现最小化窗体命令
        /// </summary>
        private void MinimizeCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        /// <summary>
        ///最大化窗体
        /// </summary>
        private void MaximizeCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {

            // 如果canExcute有效，那么那些触发窗体最大化的菜单选项和按钮应该是被选中的
            bool canExecute = AvalonCommandsHelper.CanExecuteCommandSource(btnInvokeRestore);
            if (canExecute == true)
            {
                this.btnInvokeRestore.Visibility = Visibility.Visible;
                this.btnInvokeMaximize.Visibility = Visibility.Hidden;
                this.mnuInvokeMaximize.IsEnabled = false;
                this.mnuInvokeRestore.IsEnabled = true;
            }
            this.WindowState = WindowState.Maximized;
        }

        /// <summary>
        /// CanExecuteRoutedEventHandler使最大化命令有效
        /// </summary>
        private void MaximizeCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        /// <summary>
        /// 还原窗体
        /// </summary>
        private void RestoreCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {

            // 如果canExcute有效，那么那些触发窗体还原的菜单选项和按钮应该是被选中的            
            bool canExecute = AvalonCommandsHelper.CanExecuteCommandSource(btnInvokeRestore);
            if (canExecute == true)
            {
                this.btnInvokeRestore.Visibility = Visibility.Hidden;
                this.btnInvokeMaximize.Visibility = Visibility.Visible;
                this.mnuInvokeMaximize.IsEnabled = true;
                this.mnuInvokeRestore.IsEnabled = false;
            }
            this.WindowState = WindowState.Normal;
        }

        /// <summary>
        /// CanExecuteRoutedEventHandler触发窗体还原命令
        /// </summary>
        private void RestoreCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        /// <summary>
        /// 代表拖动形状窗体的事件
        /// </summary>
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
