/****************************** 模块头 ******************************\
* 模块名:        One2OneClass.cs
* 项目名:        CSEFEntityDataModel
* 版权 (c)       Microsoft Corporation.
*
* 本例阐明了如何通过一对一关联插入，更新，和查询这两个实体
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


namespace CSEFEntityDataModel.One2One
{
    public static class One2OneClass
    {
        // 测试One2OneClass中的所有方法
        public static void One2OneTest()
        {
            InsertPersonWithPersonAddress();

            UpdatePerson();
        }

        // 使用personAddress插入新的person
        public static void InsertPersonWithPersonAddress()
        {
            using (EFO2OEntities context = new EFO2OEntities())
            {
                Person person = new Person() 
                {
                    FirstName = "Lingzhi", 
                    LastName ="Sun" 
                };

                // PersonAddress中的PersonID将是27因为它依赖于person.PersonID 
                PersonAddress personAddress = new PersonAddress() 
                { 
                    PersonID = 100, 
                    Address = "Shanghai", 
                    Postcode = "200021" 
                };

                // 设置navigation属性 (一对一) 
                person.PersonAddress = personAddress;

                context.AddToPerson(person);

                try
                {
                    Console.WriteLine("Inserting a person with "
                        + "person address");

                    context.SaveChanges();

                    Query();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }


        // 获取带有addresses的所有people
        public static void Query()
        {
            using (EFO2OEntities context = new EFO2OEntities())
            {
                var query = from p in context.Person.Include("PersonAddress") 
                            select p;

                Console.WriteLine("People with their addresses:");

                foreach (Person p in query)
                {
                    Console.WriteLine("{0} {1}", p.PersonID, p.LastName);

                    if (p.PersonAddress != null)
                    {
                        Console.WriteLine("   {0}", p.PersonAddress.Address);
                    }
                }

                Console.WriteLine();
            }
        }


        // 更新一个现存的person
        public static void UpdatePerson()
        {
            using (EFO2OEntities context = new EFO2OEntities())
            {
                Person person = new Person();

                person.PersonID = 1;

                context.AttachTo("Person", person);

                person.LastName = "Chen";

                // 将外键设置为空
                person.PersonAddress = null;

                try
                {
                    Console.WriteLine("Modifying Person 1's LastName to {0}"
                        + ", and PersonAddress to null", person.LastName);

                    context.SaveChanges();

                    Query();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

    }
   
}
