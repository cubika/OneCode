/********************************* 模块头 *********************************\
* 模块名:   TableStoragePagingUtility.cs
* 项目名:   AzureTableStoragePaging
* 版权 (c) Microsoft Corporation.
* 
* 这是MVC模型. 
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.StorageClient;
using Microsoft.WindowsAzure;

namespace MvcWebRole.Models
{
    public class CustomersSet
    {
        public List<Customer> Customers { get; set; }
        public bool ReadyToShowUI { get; set; }
    }

    public class Customer:TableServiceEntity
    {

        public string Name { get; set; }
        public int Age { get; set; }
       
        public Customer() 
        {
            this.PartitionKey = "part1";
            this.RowKey = Guid.NewGuid().ToString();
        }

    }

    public class CustomerDataContext : TableServiceContext
    {
        public CustomerDataContext(string baseAddress, StorageCredentials credentials) :
            base(baseAddress, credentials)
        {
           
        }

        public IQueryable<Customer> Customers
        {
            get
            {
                return CreateQuery<Customer>("Customers");
            }
        }
    }
}