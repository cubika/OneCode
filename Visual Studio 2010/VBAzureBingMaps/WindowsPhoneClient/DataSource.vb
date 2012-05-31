'/********************************* 模块头 **********************************\
'* 模块名:  DataSource.vb
'* 项目名:  AzureBingMaps
'* 版权 (c) Microsoft Corporation.
'* 
'* 同时被MainPage和ListPage使用的数据源.
'* 转移调用到WCF数据服务.
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
Imports WindowsPhoneClient.AzureBingMaps.DAL

''' <summary>
''' 同时被MainPage和ListPage使用的数据源.
''' 转移调用到WCF数据服务.
''' </summary>
Public Class DataSource
    ' Windows Phone不能部署在Windows Azure.
    ' 所以我们必须用绝对地址.
    ' 本机模拟时使用 http://127.0.0.1:81/DataService/TravelDataService.svc/ . 否则, 使用你自己的Windows Azure服务地址.
    Private _dataServiceContext As New TravelDataServiceContext(New Uri("http://127.0.0.1:81/DataService/TravelDataService.svc/"))
    Private _travelItems As New ObservableCollection(Of Travel)()
    Public Event DataLoaded As EventHandler

    Public ReadOnly Property TravelItems() As ObservableCollection(Of Travel)
        Get
            Return Me._travelItems
        End Get
    End Property

    ''' <summary>
    ''' 查询数据.
    ''' </summary>
    Public Sub LoadDataAsync()
        Me._dataServiceContext.BeginExecute(Of Travel)( _
            New Uri(Me._dataServiceContext.BaseUri, "Travels"), _
            Function(result)
                Dim results = Me._dataServiceContext.EndExecute(Of Travel)(result).ToList().OrderBy(Function(t) t.Time)
                Me._travelItems = New ObservableCollection(Of Travel)()
                For Each item In results
                    Me._travelItems.Add(item)
                Next
                RaiseEvent DataLoaded(Me, EventArgs.Empty)
                Return Nothing
            End Function, Nothing)
    End Sub

    Public Sub AddToTravel(ByVal travel As Travel)
        Me._travelItems.Add(travel)
        Me._dataServiceContext.AddObject("Travels", travel)
    End Sub

    Public Sub UpdateTravel(ByVal travel As Travel)
        Me._dataServiceContext.UpdateObject(travel)
    End Sub

    Public Sub RemoveFromTravel(ByVal travel As Travel)
        Me._travelItems.Remove(travel)
        Me._dataServiceContext.DeleteObject(travel)
    End Sub

    Public Sub SaveChanges()
        ' 我们的数据服务提供器实现并不支持MERGE, 所以就执行一次完全更新(PUT).
        Me._dataServiceContext.BeginSaveChanges(SaveChangesOptions.ReplaceOnUpdate, _
                                                New AsyncCallback( _
                                                    Function(result)
                                                        Dim response = Me._dataServiceContext.EndSaveChanges(result)
                                                        Return Nothing
                                                    End Function), Nothing)
    End Sub
End Class