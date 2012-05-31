'/********************************* 模块头 **********************************\
'* 模块名:  ListPage.xaml.vb
'* 项目名:  AzureBingMaps
'* 版权 (c) Microsoft Corporation.
'* 
'* ListPage后台代码.
'* 
'* This source is subject to the Microsoft Public License.
'* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
'* All other rights reserved.
'* 
'* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
'* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
'* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'\***************************************************************************/


Imports System.Windows
Imports System.Windows.Controls
Imports WindowsPhoneClient.AzureBingMaps.DAL
Imports Microsoft.Phone.Controls

Partial Public Class ListPage
    Inherits PhoneApplicationPage
    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub PhoneApplicationPage_Loaded(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Me.placeList.ItemsSource = App.DataSource.TravelItems
    End Sub

    ''' <summary>
    ''' 当DatePicker的值变更时, 更新数据源.
    ''' </summary>
    Private Sub DatePicker_ValueChanged(ByVal sender As Object, ByVal e As DateTimeValueChangedEventArgs)
        Dim datePicker As DatePicker = DirectCast(sender, DatePicker)
        Dim travel As Travel = TryCast(datePicker.DataContext, Travel)
        If travel IsNot Nothing AndAlso travel.Time <> datePicker.Value Then
            travel.Time = datePicker.Value.Value
            App.DataSource.UpdateTravel(travel)
        End If
    End Sub

    ''' <summary>
    ''' 从数据源删除项目.
    ''' </summary>
    Private Sub DeleteButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim button As HyperlinkButton = DirectCast(sender, HyperlinkButton)
        Dim travel As Travel = TryCast(button.DataContext, Travel)
        If travel IsNot Nothing Then
            App.DataSource.RemoveFromTravel(travel)
        End If
    End Sub

    ''' <summary>
    ''' 保存变更.
    ''' </summary>
    Private Sub SaveButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        App.DataSource.SaveChanges()
    End Sub

    ''' <summary>
    ''' 应用程序bar事件句柄: 导航到MainPage.
    ''' </summary>
    Private Sub ApplicationBarIconButton_Click(ByVal sender As Object, ByVal e As EventArgs)
        Me.NavigationService.Navigate(New Uri("/MainPage.xaml", UriKind.Relative))
    End Sub
End Class