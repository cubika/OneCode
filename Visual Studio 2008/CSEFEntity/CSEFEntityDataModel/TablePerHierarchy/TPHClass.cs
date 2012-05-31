/****************************** 模块头 ******************************\
* 模块名:        TPHClass.cs
* 项目名:        CSEFEntityDataModel
* 版权 (c)       Microsoft Corporation.
*
* 这个列子说明了如何创建每种层次结构一个表类型.每种类型一个表 (table-per-type)
* 模型是一种模仿继承的方式，每个实体被映射到一个存储中的单独的表.然后本例展示
* 了如何查询people类型的列表，获取Person, Student和BusinessStudent相应的属性
*
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
*
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#endregion


namespace CSEFEntityDataModel.TablePerHierarchy
{
    public static class TPHClass
    {
        // 测试TPHClass中的查询方法
        public static void TPHTest()
        {
            Query();
        }

        // 查询people类型的列表， 输出Person, Student和BusinessStudent的属性
        public static void Query()
        {
            using (EFTPHEntities context = new EFTPHEntities())
            {
                var people = from p in context.People 
                             select p;

                foreach (var p in people)
                {
                    Console.WriteLine("Student {0} {1}",
                        p.LastName,
                        p.FirstName);

                    if (p is Student)
                    {
                        Console.WriteLine("EnrollmentDate: {0}", 
                            ((Student)p).EnrollmentDate);
                    }
                    if (p is BusinessStudent)
                    {
                        Console.WriteLine("BusinessCredits: {0}", 
                            ((BusinessStudent)p).BusinessCredits);
                    }
                }

            }
        }
    }
}
