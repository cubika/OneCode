/****************************** Module Header ******************************\
 模块:      CustomerList.cs
 项目:      CSWPFMasterDetailBinding
 版权 (c) Microsoft Corporation.
 
 此示例演示在WPF中主/细形式的数据绑定。
 
 This source is subject to the Microsoft Public License.
 See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
 All other rights reserved.
 
 THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
 EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Collections.ObjectModel;

namespace CSWPFMasterDetailBinding.Data
{
    class CustomerList
    {
        private ObservableCollection<Customer> _customers;

        public CustomerList()
        {
            _customers = new ObservableCollection<Customer>();

            // 插入客户及相关信息
            Customer c = new Customer() { ID = 1, Name = "客户1" };
            c.Orders.Add(new Order() { ID = 1, Date = new DateTime(2009, 1, 1), ShipCity = "上海" });
            c.Orders.Add(new Order() { ID = 1, Date = new DateTime(2009, 2, 1), ShipCity = "北京" });
            c.Orders.Add(new Order() { ID = 1, Date = new DateTime(2009, 11, 10), ShipCity = "广州" });
            _customers.Add(c);

            c = new Customer() { ID = 2, Name = "客户2" };
            c.Orders.Add(new Order() { ID = 1, Date = new DateTime(2009, 1, 1), ShipCity = "纽约" });
            c.Orders.Add(new Order() { ID = 1, Date = new DateTime(2009, 2, 1), ShipCity = "西雅图" });
            _customers.Add(c);

            c = new Customer() { ID = 3, Name = "客户3" };
            c.Orders.Add(new Order() { ID = 1, Date = new DateTime(2009, 1, 1), ShipCity = "厦门" });
            c.Orders.Add(new Order() { ID = 1, Date = new DateTime(2009, 2, 1), ShipCity = "深圳" });
            c.Orders.Add(new Order() { ID = 1, Date = new DateTime(2009, 11, 10), ShipCity = "天津" });
            c.Orders.Add(new Order() { ID = 1, Date = new DateTime(2009, 11, 10), ShipCity = "武汉" });
            c.Orders.Add(new Order() { ID = 1, Date = new DateTime(2009, 11, 10), ShipCity = "济南" });
            _customers.Add(c);

            c = new Customer() { ID = 4, Name = "客户4" };
            c.Orders.Add(new Order() { ID = 1, Date = new DateTime(2009, 1, 1), ShipCity = "兰州" });
            _customers.Add(c);
        }

        public ObservableCollection<Customer> Customers
        {
            get { return _customers; }
        }
    }
}
