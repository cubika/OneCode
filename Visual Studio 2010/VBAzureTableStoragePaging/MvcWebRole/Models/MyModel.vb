'******************************** 模块头 **********************************\
'* 模块名:   MyModel.vb
'* 项目名:   AzureTableStoragePaging
'* 版权 (c) Microsoft Corporation.
'* 
'* 这是MVC模型 
'* 
'* This source is subject to the Microsoft Public License.
'* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
'* All other rights reserved.
'* 
'* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
'* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
'* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'\**************************************************************************/

Imports Microsoft.WindowsAzure.StorageClient
Imports Microsoft.WindowsAzure
Namespace Models
    Public Class CustomersSet
        Public Property Customers() As List(Of Customer)

        Public ReadyToShowUI As Boolean
    End Class
    Public Class Customer
        Inherits TableServiceEntity

        Public Property Name() As String

        Public Property Age() As Integer

        Public Sub New()
            Me.PartitionKey = "part1"
            Me.RowKey = Guid.NewGuid().ToString()
        End Sub

    End Class
    Public Class CustomerDataContext
        Inherits TableServiceContext
        Public Sub New(ByVal baseAddress As String, ByVal credentials As StorageCredentials)
            MyBase.New(baseAddress, credentials)

        End Sub

        Public ReadOnly Property Customers() As IQueryable(Of Customer)
            Get
                Return CreateQuery(Of Customer)("Customers")
            End Get
        End Property
    End Class
End Namespace