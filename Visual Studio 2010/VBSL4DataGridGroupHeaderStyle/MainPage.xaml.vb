'********************************* 模块头 **********************************\
' 模块名:    MainPage.xaml.vb
' 项目:      VBSL4DataGridGroupHeaderStyle
' Copyright (c) Microsoft Corporation.
' 
' DataGridGroupHeader 解决方案隐藏代码文件. 
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************/
Imports System.Collections.ObjectModel
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Documents
Imports System.Windows.Media
Imports System.Windows.Data

Partial Public Class MainPage
    Inherits UserControl

    Public Sub New()
        InitializeComponent()
        AddHandler Me.Loaded, AddressOf MainPage_Loaded
    End Sub


    Private Sub MainPage_Loaded(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim pcv As New PagedCollectionView(People.GetPeople())

        ' 根据 AgeGroup 对 person 分组.
        pcv.GroupDescriptions.Add(New PropertyGroupDescription("AgeGroup"))

        ' 根据 Gender 对 person 分组.
        pcv.GroupDescriptions.Add(New PropertyGroupDescription("Gender"))

        ' 将实体绑定至 DataGrid.
        PeopleList.ItemsSource = pcv
    End Sub


    Public Property RowGroupHeaderStyles() As ObservableCollection(Of Style)
        Get
            Return _rowGroupHeaderStyles
        End Get
        Set(ByVal value As ObservableCollection(Of Style))
            _rowGroupHeaderStyles = value
        End Set
    End Property

    Private _rowGroupHeaderStyles As ObservableCollection(Of Style)

End Class
