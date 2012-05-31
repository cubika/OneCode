/**************************************** 模块头 *****************************************\
* 模块名:   Employee.cs
* 项目名:   CSMVCPager
* 版权 (c) Microsoft Corporation
*
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
*
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER 
* EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF 
* MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\******************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CSASPNETMVCPager.Models
{
    public class Employee
    {
        public string Name
        {
            get;
            set;
        }
        public string Sex
        {
            get;
            set;
        }
        public int Age
        {
            get;
            set;
        }
        public string Address
        {
            get;
            set;
        }
    }
    public static class EmployeeSet
    {

        public static List<Employee> Employees
        {
            get
            {
                List<Employee> empList = new List<Employee>();
                int count = 50;
                Random ran = new Random();
                for (int i = 0; i < count; i++)
                {
                    Employee e = new Employee();
                    e.Name = "姓名" + i.ToString();
                    e.Sex = ran.Next(4) / 2 == 0 ? "男" : "女";
                    e.Age = 20 + i;
                    e.Address = "地址" + i.ToString();
                    empList.Add(e);
                }
                return empList;
            }

        }
    }
}
