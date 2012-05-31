/****************************** 模块标识 ******************************\
* 模块名:  MainPage.cs
* 项目:      CSSL3Text
* 版权 (c) Microsoft Corporation.
* 
*本示例主要展示如何通过C#语言使用文本对象。
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
#endregion


namespace CSSL3Text
{
	public partial class MainPage : UserControl
	{
		public MainPage()
		{
			InitializeComponent();
		}

		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			this.CreateSimpleTextBlock();
			this.CreateComplexTextBlock();
			this.FormatText();
		}

		/// <summary>
		/// 本方法创建一个文本对象并添加到
        /// StackPanel simpleTBPlaceHolder中去.
		/// </summary>
		private void CreateSimpleTextBlock()
		{
			TextBlock simpleTextBlock = new TextBlock() { Text = "Simple TextBlock" };
			this.simpleTBPlaceHolder.Children.Add(simpleTextBlock);
		}

		/// <summary>
		/// 本方法使用多个Run以及LineBreak对象来创建文本。
        /// 本添加到StackPanel complexTBPlaceHolder中去.
		/// </summary>
		private void CreateComplexTextBlock()
		{
			TextBlock complexTextBlock = new TextBlock();
			Run paragraph1 = new Run() { Text = "Paragraph1" };
			LineBreak lineBreak = new LineBreak();
			Run paragraph2 = new Run() { Text = "Paragraph2" };
			complexTextBlock.Inlines.Add(paragraph1);
			complexTextBlock.Inlines.Add(lineBreak);
			complexTextBlock.Inlines.Add(paragraph2);
			this.complexTBPlaceHolder.Children.Add(complexTextBlock);
		}

		/// <summary>
		/// 本项目创建一个文本对象，并用外部字体，
        /// 创建完成后添加到StackPanel customizeFormatPlaceHolder中去.
		/// </summary>
		private void FormatText()
		{
			TextBlock formatTextBlock = new TextBlock();
			Run paragraph1 = new Run() { Text = "Paragraph1" };
			paragraph1.FontFamily = new FontFamily("Magnetob.ttf#Magneto");
			LineBreak lineBreak = new LineBreak();
			Run paragraph2 = new Run() { Text = "Paragraph2" };
			LinearGradientBrush brush = new LinearGradientBrush();
			brush.GradientStops.Add(new GradientStop() { Color = Colors.Blue, Offset = 0d });
			brush.GradientStops.Add(new GradientStop() { Color = Colors.Red, Offset = 1d });
			paragraph2.Foreground = brush;
			formatTextBlock.Inlines.Add(paragraph1);
			formatTextBlock.Inlines.Add(lineBreak);
			formatTextBlock.Inlines.Add(paragraph2);
			this.customizeFormatPlaceHolder.Children.Add(formatTextBlock);
		}

		/// <summary>
		/// 本方法选中文本框中的所有文字。
        /// 注意：文本框必须拥有焦点，文字选择效果才会出现
		/// </summary>
		private void selectTextButton_Click(object sender, RoutedEventArgs e)
		{
			this.targetTextBox.SelectAll();
			this.targetTextBox.Focus();
		}
	}
}
