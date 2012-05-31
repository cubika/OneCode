'**************************** Module Header ******************************\
' 模块:       CustomerList.vb
' 项目:       VBWPFAutoCompleteTextBox
' 版权 (c) Microsoft Corporation.
' 
' 此示例演示在WPF中主/细形式的数据绑定。
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL 
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************/

Imports System.Collections.ObjectModel

Namespace Data
    Class CustomerList
        Private _customers As ObservableCollection(Of Customer)

        Public Sub New()
            _customers = New ObservableCollection(Of Customer)()

            ' 插入客户及相关信息
            Dim c As New Customer() With {.ID = 1, .Name = "客户1"}
            c.Orders.Add(New Order() With {.ID = 1, .Date = New DateTime(2009, 1, 1), .ShipCity = "上海"})
            c.Orders.Add(New Order() With {.ID = 1, .Date = New DateTime(2009, 2, 1), .ShipCity = "北京"})
            c.Orders.Add(New Order() With {.ID = 1, .Date = New DateTime(2009, 11, 10), .ShipCity = "广州"})
            _customers.Add(c)

            c = New Customer() With {.ID = 2, .Name = "客户2"}
            c.Orders.Add(New Order() With {.ID = 1, .Date = New DateTime(2009, 1, 1), .ShipCity = "纽约"})
            c.Orders.Add(New Order() With {.ID = 1, .Date = New DateTime(2009, 2, 1), .ShipCity = "西雅图"})
            _customers.Add(c)

            c = New Customer() With {.ID = 3, .Name = "客户3"}
            c.Orders.Add(New Order() With {.ID = 1, .Date = New DateTime(2009, 1, 1), .ShipCity = "厦门"})
            c.Orders.Add(New Order() With {.ID = 1, .Date = New DateTime(2009, 2, 1), .ShipCity = "深圳"})
            c.Orders.Add(New Order() With {.ID = 1, .Date = New DateTime(2009, 11, 10), .ShipCity = "天津"})
            c.Orders.Add(New Order() With {.ID = 1, .Date = New DateTime(2009, 11, 10), .ShipCity = "武汉"})
            c.Orders.Add(New Order() With {.ID = 1, .Date = New DateTime(2009, 11, 10), .ShipCity = "济南"})
            _customers.Add(c)

            c = New Customer() With {.ID = 4, .Name = "客户4"}
            c.Orders.Add(New Order() With {.ID = 1, .Date = New DateTime(2009, 1, 1), .ShipCity = "兰州"})
            _customers.Add(c)
        End Sub

        Public ReadOnly Property Customers() As ObservableCollection(Of Customer)
            Get
                Return _customers
            End Get
        End Property
    End Class
End Namespace