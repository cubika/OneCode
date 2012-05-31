/********************************* 模块头 **********************************\
模块名:    MainPage.xaml.cs
项目:      CSSL4DataGridGroupHeaderSyle
Copyright (c) Microsoft Corporation.

DataGridGroupHeader 解决方案隐藏代码文件. 

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/


using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Data;

namespace CSSL4DataGridGroupHeaderStyle
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            PagedCollectionView pcv = new PagedCollectionView(People.GetPeople());

            // 根据 AgeGroup 对 person 分组.
            pcv.GroupDescriptions.Add(new PropertyGroupDescription("AgeGroup"));

            // 根据 Gender 对 person 分组.
            pcv.GroupDescriptions.Add(new PropertyGroupDescription("Gender"));
            
            // 将实体绑定至 DataGrid.
            PeopleList.ItemsSource = pcv;
        }

        public ObservableCollection<Style> RowGroupHeaderStyles 
        { get; set; }
    }
}
