'/********************************* 模块头 **********************************\
'* 模块名:  MainPage.xaml.vb
'* 项目名:  AzureBingMaps
'* 版权 (c) Microsoft Corporation.
'* 
'* MainPage的后台代码.
'* 
'* This source is subject to the Microsoft Public License.
'* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
'* All other rights reserved.
'* 
'* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
'* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
'* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'\***************************************************************************/

Imports System.Collections.ObjectModel
Imports System.Data.Services.Client
Imports System.Linq
Imports System.Windows
Imports System.Windows.Controls
Imports Microsoft.Maps.MapControl
Imports SilverlightClient.GeocodeServiceReference
Imports SilverlightClient.TravelDataServiceReference

Partial Public Class MainPage
    Inherits UserControl
    ' 你的Bing Maps身份证明.
    Private _mapCredential As String = "[your credential]"
    Private _geocodeClient As New GeocodeServiceClient("CustomBinding_IGeocodeService")
    ' 因为这个Silverlight客户端和Web Role服务在同一主机上,
    ' 我们使用相对地址.
    Private _dataServiceContext As New TravelDataServiceContext( _
        New Uri("../DataService/TravelDataService.svc", UriKind.Relative))
    Private _travelItems As New ObservableCollection(Of Travel)()

    Public Sub New()
        InitializeComponent()
        ' 根据身份验证显示登入连接或欢迎信息.
        If App.IsAuthenticated Then
            Me.LoginLink.Visibility = System.Windows.Visibility.Collapsed
            Me.WelcomeTextBlock.Visibility = System.Windows.Visibility.Visible
            Me.WelcomeTextBlock.Text = App.WelcomeMessage
        Else
            Me.LoginLink.Visibility = System.Windows.Visibility.Visible
            Me.WelcomeTextBlock.Visibility = System.Windows.Visibility.Collapsed
        End If
        Me.map.CredentialsProvider = New ApplicationIdCredentialsProvider(Me._mapCredential)
        AddHandler Me._geocodeClient.ReverseGeocodeCompleted, New EventHandler( _
            Of ReverseGeocodeCompletedEventArgs)(AddressOf GeocodeClient_ReverseGeocodeCompleted)
        Me.LoginLink.NavigateUri = New Uri(Application.Current.Host.Source, "../LoginPage.aspx?returnpage=SilverlightClient.aspx")
    End Sub

    Private Sub Map_Loaded(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' 查询数据.
        Me._dataServiceContext. _
            Travels.BeginExecute(Function(result)
                                     Me._travelItems = New ObservableCollection(Of Travel) _
(Me._dataServiceContext.Travels.EndExecute(result).ToList().OrderBy(Function(t) t.Time))
                                     Me.Dispatcher.BeginInvoke(New Action(Function()
                                                                              Me.mapItems.ItemsSource = Me._travelItems
                                                                              Me.placeList.ItemsSource = Me._travelItems
                                                                              Return Nothing
                                                                          End Function))
                                     Return Nothing
                                 End Function, Nothing)
    End Sub

    Private Sub map_MouseClick(ByVal sender As Object, ByVal e As Microsoft.Maps.MapControl.MapMouseEventArgs)
        ' 调用Bing Maps Geocode服务获得最近的位置.
        Dim request As New ReverseGeocodeRequest() With { _
         .Location = map.ViewportPointToLocation(e.ViewportPoint) _
        }
        request.Credentials = New Credentials() With { _
         .Token = Me._mapCredential _
        }
        _geocodeClient.ReverseGeocodeAsync(request)
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
            ' PartitionKey代表用户.
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
            Me._travelItems.Add(travel)
            Me._dataServiceContext.AddObject("Travels", travel)
        End If
    End Sub

    Private Sub DatePicker_SelectedDateChanged(ByVal sender As Object, ByVal e As SelectionChangedEventArgs)
        Dim datePicker As DatePicker = DirectCast(sender, DatePicker)
        Dim travel As Travel = TryCast(datePicker.DataContext, Travel)
        If travel IsNot Nothing AndAlso travel.Time <> datePicker.SelectedDate.Value Then
            travel.Time = datePicker.SelectedDate.Value
            Me._dataServiceContext.UpdateObject(travel)
        End If
    End Sub

    ''' <summary>
    ''' 保存变更.
    ''' </summary>
    Private Sub SaveButton_Click(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs)
        ' 我们的数据服务提供器实现并不支持MERGE, 所以就执行一次完全更新(PUT).
        Me._dataServiceContext.BeginSaveChanges( _
            SaveChangesOptions.ReplaceOnUpdate, _
            New AsyncCallback(Function(result)
                                  Dim response = Me._dataServiceContext.EndSaveChanges(result)
                                  Return Nothing
                              End Function), Nothing)
    End Sub

    ''' <summary>
    ''' 从数据源删除项目.
    ''' </summary>
    Private Sub DeleteButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim button As HyperlinkButton = DirectCast(sender, HyperlinkButton)
        Dim travel As Travel = TryCast(button.DataContext, Travel)
        If travel IsNot Nothing Then
            Me._travelItems.Remove(travel)
            Me._dataServiceContext.DeleteObject(travel)
        End If
    End Sub
End Class