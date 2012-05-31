'/********************************* 模块头 **********************************\
'* 模块名:  MainPage.xaml.vb
'* 项目名:  AzureBingMaps
'* 版权 (c) Microsoft Corporation.
'* 
'* MainPage后台代码.
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
Imports System.Windows.Input
Imports WindowsPhoneClient.AzureBingMaps.DAL
Imports Microsoft.Phone.Controls
Imports Microsoft.Phone.Controls.Maps
Imports WindowsPhoneClient.GeocodeServiceReference

Partial Public Class MainPage
    Inherits PhoneApplicationPage
    ' 你的Bing Maps身份证明.
    Private _mapCredential As String = "[your credential]"
    Private _geocodeClient As New GeocodeServiceClient()
    Private clickedPoint As Point

    ' 构造器
    Public Sub New()
        InitializeComponent()

        ' 设定listbox控件数据内容为示例数据
        DataContext = App.ViewModel
        AddHandler Me.Loaded, New RoutedEventHandler(AddressOf MainPage_Loaded)
        Me.map.CredentialsProvider = New ApplicationIdCredentialsProvider(Me._mapCredential)
        AddHandler Me._geocodeClient.ReverseGeocodeCompleted, _
            New EventHandler(Of ReverseGeocodeCompletedEventArgs)(AddressOf GeocodeClient_ReverseGeocodeCompleted)
    End Sub

    ' ViewModel项目载入数据
    Private Sub MainPage_Loaded(ByVal sender As Object, ByVal e As RoutedEventArgs)
        If Not App.ViewModel.IsDataLoaded Then
            App.ViewModel.LoadData()
        End If
        Me.mapItems.ItemsSource = App.DataSource.TravelItems
    End Sub

    Private Sub Map_Loaded(ByVal sender As Object, ByVal e As RoutedEventArgs)
        AddHandler App.DataSource.DataLoaded, New EventHandler(AddressOf DataSource_DataLoaded)
    End Sub

    Private Sub DataSource_DataLoaded(ByVal sender As Object, ByVal e As EventArgs)
        Me.Dispatcher.BeginInvoke(New Action(Function()
                                                 Me.mapItems.ItemsSource = App.DataSource.TravelItems
                                                 Return Nothing
                                             End Function))
    End Sub

    ''' <summary>
    ''' Bing Maps Geocode服务的回调方法.
    ''' </summary>
    Private Sub GeocodeClient_ReverseGeocodeCompleted(ByVal sender As Object, ByVal e As ReverseGeocodeCompletedEventArgs)
        If e.[Error] IsNot Nothing Then
            MessageBox.Show(e.[Error].Message)
        ElseIf e.Result.Results.Count > 0 Then
            ' 我们只关心第一个结果.
            Dim result = e.Result.Results(0)

            ' TPartitionKey代表用户.
            ' 然而, 客户端可以如下演示般发送一个假身份.
            ' 为了防止客户端使用假身份,
            ' 我们的服务一直在服务器端查询真实的身份.
            ' Latitude/Longitude通过服务获得地址,
            ' 因此可能不是正巧为所点击的地址.
            Dim travel As New Travel() With { _
              .PartitionKey = "fake@live.com", _
              .RowKey = Guid.NewGuid(), _
              .Place = result.DisplayName, _
              .Time = DateTime.Now, _
              .Latitude = result.Locations(0).Latitude, _
              .Longitude = result.Locations(0).Longitude _
            }
            ' 添加到ObservableCollection.
            App.DataSource.AddToTravel(travel)
        End If
    End Sub

    ''' <summary>
    ''' Windows Phone map控件并不支持Click,
    ''' 因此我们必须捕获MouseLeftButtonDown/Up.
    ''' 这些事件将在用户触摸屏幕时触发.
    ''' </summary>
    Private Sub map_MouseLeftButtonDown(ByVal sender As Object, ByVal e As MouseButtonEventArgs)
        Me.clickedPoint = e.GetPosition(Me.map)
    End Sub

    Private Sub map_MouseLeftButtonUp(ByVal sender As Object, ByVal e As MouseButtonEventArgs)
        ' 检查手指是否移动.
        Dim clickedPoint As Point = e.GetPosition(Me.map)
        If Math.Abs(clickedPoint.X - Me.clickedPoint.X) < 5 _
            AndAlso Math.Abs(clickedPoint.Y - Me.clickedPoint.Y) < 5 Then
            ' 调用Bing Maps Geocode服务获得最近的位置.
            Dim request As New ReverseGeocodeRequest() With { _
              .Location = map.ViewportPointToLocation(e.GetPosition(Me.map)) _
            }
            request.Credentials = New Microsoft.Phone.Controls.Maps.Credentials() With { _
              .ApplicationId = Me._mapCredential _
            }
            _geocodeClient.ReverseGeocodeAsync(request)
        End If
    End Sub

    ''' <summary>
    ''' 应用程序bar事件句柄: 导航到ListPage.
    ''' </summary>
    Private Sub ApplicationBarIconButton_Click(ByVal sender As Object, ByVal e As EventArgs)
        Me.NavigationService.Navigate(New Uri("/ListPage.xaml", UriKind.Relative))
    End Sub
End Class