'******************************** 模块头 *********************************\
'* 模块名:   HomeController.vb
'* 项目名:   AzureTableStoragePaging
'* 版权 (c) Microsoft Corporation.
'* 
'* 这个模块控制了分页行为.
'* 
'* This source is subject to the Microsoft Public License.
'* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
'* All other rights reserved.
'* 
'* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
'* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
'* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'\**************************************************************************/
Imports Microsoft.WindowsAzure
Imports Microsoft.WindowsAzure.StorageClient
Imports MvcWebRole.Models
Imports MvcWebRole.Utilities

<HandleError(ExceptionType:=GetType(Exception), View:="Error")> _
Public Class HomeController
    Inherits Controller
    Private Shared r As Random = New Random()
    Private cloudStorageAccount As CloudStorageAccount = cloudStorageAccount.FromConfigurationSetting("SampleDataConnectionString")
    Public Function Index() As ActionResult
        Try
            Dim cloudTableClient = cloudStorageAccount.CreateCloudTableClient()
            cloudTableClient.CreateTableIfNotExist("Customers")
            Dim context = New CustomerDataContext(cloudStorageAccount.TableEndpoint.AbsoluteUri, cloudStorageAccount.Credentials)
            Dim list = context.Customers.ToList()
            ' 如果表中有实体信息则显示该表到UI层
            If list.Count() > 0 Then
                Dim provider As MVCSessionCachedDataProvider(Of Customer) = New MVCSessionCachedDataProvider(Of Customer)(Me, "provider1")
                Dim pagingUtility As TableStoragePagingUtility(Of Customer) = New TableStoragePagingUtility(Of Customer)(provider, cloudStorageAccount, context, 10, "Customers")
                'TODO: INSTANT VB TODO TASK: 表达式里的分配不被 VB.NET支持
                'ORIGINAL LINE: Return View("Index", New CustomersSet() { Customers = pagingUtility.GetCurrentOrFirstPage().ToList(), ReadyToShowUI = True });
                Return View("Index", New CustomersSet() With {.Customers = pagingUtility.GetCurrentOrFirstPage().ToList(), .ReadyToShowUI = True})
            Else
                '如果表中没有实体显示指引用户向表中添加数据的链接.
                'TODO: INSTANT VB TODO TASK: 表达式里的分配不被 VB.NET支持
                'ORIGINAL LINE: ViewResult vr = View("Index", New CustomersSet() { ReadyToShowUI = False });
                Dim vr As ViewResult = View("Index", New CustomersSet() With {.ReadyToShowUI = False})
                Return vr
            End If
        Catch ex As Exception
            Return View("Error", New HandleErrorInfo(ex, "HomeController", "Index"))
        End Try

    End Function

    Public Function AddDataToTest() As ActionResult

        Dim cloudTableClient = cloudStorageAccount.CreateCloudTableClient()
        Dim context = New CustomerDataContext(cloudStorageAccount.TableEndpoint.AbsoluteUri, cloudStorageAccount.Credentials)
        Try
            ' 向表中添加65个实体
            For i As Integer = 0 To 64
                'TODO: INSTANT VB TODO TASK: 表达式里的分配不被 VB.NET支持
                'ORIGINAL LINE: context.AddObject("Customers", New Customer() { Age = r.Next(16, 70), Name = "Customer" + i.ToString() });
                context.AddObject("Customers", New Customer() With {.Age = r.Next(16, 70), .Name = "Customer" & i.ToString()})
            Next i
            context.SaveChanges()
            Dim provider As MVCSessionCachedDataProvider(Of Customer) = New MVCSessionCachedDataProvider(Of Customer)(Me, "provider1")
            Dim pagingUtility As TableStoragePagingUtility(Of Customer) = New TableStoragePagingUtility(Of Customer)(provider, cloudStorageAccount, context, 10, "Customers")
            'TODO: INSTANT VB TODO TASK: 表达式里的分配不被 VB.NET支持
            'ORIGINAL LINE: Return View("Index", New CustomersSet() { Customers = pagingUtility.GetNextPage().ToList(), ReadyToShowUI = True });
            Return View("Index", New CustomersSet() With {.Customers = pagingUtility.GetNextPage().ToList(), .ReadyToShowUI = True})
        Catch ex As Exception
            Return View("Error", New HandleErrorInfo(ex, "HomeController", "AddDataToTest"))
        End Try


    End Function
    Public Function Previous() As ActionResult
        Dim cloudTableClient = cloudStorageAccount.CreateCloudTableClient()
        Dim context = New CustomerDataContext(cloudStorageAccount.TableEndpoint.AbsoluteUri, cloudStorageAccount.Credentials)
        Dim provider As MVCSessionCachedDataProvider(Of Customer) = New MVCSessionCachedDataProvider(Of Customer)(Me, "provider1")
        Dim pagingUtility As TableStoragePagingUtility(Of Customer) = New TableStoragePagingUtility(Of Customer)(provider, cloudStorageAccount, context, 10, "Customers")
        'TODO: INSTANT VB TODO TASK: 表达式里的分配不被 VB.NET支持
        'ORIGINAL LINE: Return View("Index", New CustomersSet() { Customers = pagingUtility.GetPreviousPage().ToList(), ReadyToShowUI=True });
        Return View("Index", New CustomersSet() With {.Customers = pagingUtility.GetPreviousPage().ToList(), .ReadyToShowUI = True})
    End Function
    Public Function [Next]() As ActionResult
        Dim cloudTableClient = cloudStorageAccount.CreateCloudTableClient()
        Dim context = New CustomerDataContext(cloudStorageAccount.TableEndpoint.AbsoluteUri, cloudStorageAccount.Credentials)
        Dim provider As MVCSessionCachedDataProvider(Of Customer) = New MVCSessionCachedDataProvider(Of Customer)(Me, "provider1")
        Dim pagingUtility As TableStoragePagingUtility(Of Customer) = New TableStoragePagingUtility(Of Customer)(provider, cloudStorageAccount, context, 10, "Customers")
        'TODO: INSTANT VB TODO TASK: 表达式里的分配不被 VB.NET支持
        'ORIGINAL LINE: Return View("Index", New CustomersSet() { Customers = pagingUtility.GetNextPage().ToList(), ReadyToShowUI=True });
        Return View("Index", New CustomersSet() With {.Customers = pagingUtility.GetNextPage().ToList(), .ReadyToShowUI = True})
    End Function
End Class

