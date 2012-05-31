/****************************** 模块头 ******************************\
* 模块名:	    Many2ManyClass.cs
* 项目名:		CSEFEntityDataModel
* 版权 (c)      Microsoft Corporation.
* 
* 本例阐明了如何通过多对多关联插入，更新，和查询这两个实体
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

namespace CSEFEntityDataModel.Many2Many
{
    class Many2ManyClass
    {
        // 测试Many2ManyClass中的所有方法
        public static void Many2ManyTest()
        {
            InsertPersonWithCourse();

            InsertPerson();

            UpdatePerson();
        }

        // 插入新的person包括新的course
        public static void InsertPersonWithCourse()
        {
            using (EFM2MEntities context = new EFM2MEntities())
            {

                Person person = new Person()
                {
                    FirstName = "Yichun",
                    LastName = "Feng"
                };

                Course course = new Course()
                {
                    CourseID = 2208,
                    Title = "UML"
                };

                person.Course.Add(course);

                context.AddToPerson(person);

                try
                {
                    Console.WriteLine("Inserting Person {0} {1} with course "
                     + "{2}.", person.FirstName, person.LastName,
                     course.Title);

                    context.SaveChanges();

                    Query();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }


        // 插入新的person到现存的course
        public static void InsertPerson()
        {
            using (EFM2MEntities context = new EFM2MEntities())
            {

                Person person = new Person()
                {
                    FirstName = "Ji",
                    LastName = "Zhou"
                };

                person.Course.Add(context.Course.FirstOrDefault());

                context.AddToPerson(person);

                try
                {
                    Console.WriteLine("Inserting Person {0} {1} .",
                        person.FirstName,
                        person.LastName);

                    context.SaveChanges();

                    Query();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }


        // 获取所有的persons和他们的courses
        public static void Query()
        {
            using (EFM2MEntities context = new EFM2MEntities())
            {
                var query = from p in context.Person.Include("Course")
                            select p;

                Console.WriteLine("Persons with their Course");

                foreach (Person p in query)
                {
                    Console.WriteLine("{0}: {1} {2}", p.PersonID,
                        p.FirstName, p.LastName);

                    foreach (Course c in p.Course)
                    {
                        Console.WriteLine("   {0}", c.Title);
                    }
                }

                Console.WriteLine();
            }
        }


        // 更新一个现存的person 
        public static void UpdatePerson()
        {
            using (EFM2MEntities context = new EFM2MEntities())
            {
                Person person = new Person();

                person.PersonID = 34;

                context.AttachTo("Person", person);

                person.FirstName = "Monica";

                Course course = new Course()
                {
                    CourseID = 2209,
                    Title = "Operating System"
                };

                person.Course.Add(course);

                try
                {
                    Console.WriteLine(
                       "Modifying Person 34's last to {0} and add course {1}"
                       + " to it", person.LastName, course.Title);

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
