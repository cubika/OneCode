/****************************** 模块头 ******************************\
* 模块名:        TableSplittingClass.cs
* 项目名:        CSEFEntityDataModel
* 版权 (c)       Microsoft Corporation.
*
* 本例说明了如何将一个表拆分为两个实体，并且展示了怎样向这两个实体插入记录和
* 查询结果
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


namespace CSEFEntityDataModel.TableSplitting
{
    public static class TableSplittingClass
    {
        public static void TableSplittingTest()
        {
            InsertQueryPersonWithPersonDetail();
        }


        // 插入和查询PersonWithPersonDetail
        public static void InsertQueryPersonWithPersonDetail()
        {
            // 创建新的Person实体
            Person person = new Person();
            person.FirstName = "Lopez";
            person.LastName = "Typot";

            // 创建新的PersonDetail实体
            PersonDetail personDetail = new PersonDetail();
            personDetail.PersonCategory = 0;
            personDetail.HireDate = System.DateTime.Now;

            // 将person中的PersonDetail属性设置成此PersonDetail
            person.PersonDetail = personDetail;

            // 插入PersonWithPersonDetail
            using (EFTblSplitEntities context = new EFTblSplitEntities())
            {
                context.AddToPerson(person);

                Console.WriteLine("Saving person {0} {1}", person.FirstName, 
                    person.LastName);

                // 请注意personDetail.PersonID和person.PersonID是相同的，这就
                // 是我们为什么喜欢Entity Framework的原因. 
                Console.WriteLine("Saving person detail with the same " + 
                    "personID.\n");

                context.SaveChanges();
            }

            // 查询 PersonWithPersonDetail
            using (EFTblSplitEntities context = new EFTblSplitEntities())
            {
                // 获取刚插入的person
                Person person2 = (from p in context.Person
                                  where p.PersonID == person.PersonID
                                  select p).FirstOrDefault();

                Console.WriteLine(
                    "Retrieved person {0} with person detail",
                    person2.PersonID,
                    person2.PersonDetail);

                person2.PersonDetailReference.Load();

                Console.WriteLine(
                    "Retrieved hiredate for person detail {0}",
                    person2.PersonDetail.HireDate);
            }
        }
    }
}
