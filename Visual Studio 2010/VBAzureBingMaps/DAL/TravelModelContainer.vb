'/********************************* 模块头 **********************************\
'* 模块名:  TravelModelContainer.vb
'* 项目名:  AzureBingMaps
'* 版权 (c) Microsoft Corporation.
'* 
'* 对象内容的分部类.
'* 
'* This source is subject to the Microsoft Public License.
'* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
'* All other rights reserved.
'* 
'* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
'* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
'* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'\***************************************************************************/

Imports System.Data
Imports System.Data.Objects
Imports System.Reflection

''' <summary>
''' 对象内容的分部类.
''' </summary>
Partial Public Class TravelModelContainer
    Inherits ObjectContext

    Public Overrides Function SaveChanges(ByVal options As SaveOptions) As Integer
        Dim returnValue As Integer = 0
        ' 因为我们不调用base.SaveChanges, 我们必须手动关闭链接.
        ' 否则我们将留下许多打开的链接, 最终导致链接瓶颈.
        ' Entity Framework提供了base.SaveChanges内部使用的EnsureConnection和ReleaseConnection.
        ' 这些是内部方法, 所以我们必须使用反射调用它们.
        Dim EnsureConnectionMethod = GetType(ObjectContext).GetMethod("EnsureConnection", BindingFlags.Instance Or BindingFlags.NonPublic)
        EnsureConnectionMethod.Invoke(Me, Nothing)
        ' 使用ObjectStateManager.GetObjectStateEntries完成增加,修改,和删除集合.
        For Each ose As ObjectStateEntry In Me.ObjectStateManager.GetObjectStateEntries(EntityState.Added)
            Dim travel As Travel = TryCast(ose.Entity, Travel)
            If travel IsNot Nothing Then
                Dim retryPolicy As New RetryPolicy()
                retryPolicy.Task = New Action(Function()
                                                  Me.InsertIntoTravel(travel.PartitionKey, travel.Place, travel.GeoLocationText, travel.Time)
                                                  Return Nothing
                                              End Function)
                retryPolicy.Execute()
                returnValue += 1
            End If
        Next
        For Each ose As ObjectStateEntry In Me.ObjectStateManager.GetObjectStateEntries(EntityState.Modified)
            Dim travel As Travel = TryCast(ose.Entity, Travel)
            If travel IsNot Nothing Then
                Dim retryPolicy As New RetryPolicy()
                retryPolicy.Task = New Action(Function()
                                                  Me.UpdateTravel(travel.PartitionKey, travel.RowKey, travel.Place, travel.GeoLocationText, travel.Time)
                                                  Return Nothing
                                              End Function)
                retryPolicy.Execute()
                returnValue += 1
            End If
        Next
        For Each ose As ObjectStateEntry In Me.ObjectStateManager.GetObjectStateEntries(EntityState.Deleted)
            Dim travel As Travel = TryCast(ose.Entity, Travel)
            If travel IsNot Nothing Then
                Dim retryPolicy As New RetryPolicy()
                retryPolicy.Task = New Action(Function()
                                                  Me.DeleteFromTravel(travel.PartitionKey, travel.RowKey)
                                                  Return Nothing
                                              End Function)
                retryPolicy.Execute()
                returnValue += 1
            End If
        Next
        Dim ReleaseConnectionMethod = GetType(ObjectContext).GetMethod("ReleaseConnection", BindingFlags.Instance Or BindingFlags.NonPublic)
        ReleaseConnectionMethod.Invoke(Me, Nothing)
        Return returnValue
    End Function
End Class