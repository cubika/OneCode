/****************************** 模块头 ******************************\
* 模块名:        TPTClass.cs
* 项目名:        CSEFEntityDataModel
* 版权 (c)       Microsoft Corporation.
*
* 这个列子说明了如何创建每种类型一个表类型. 不同之处在于所有的实体均来源于一
* 个单一表由discriminator列来进行区分, 它还展示了如何在上下文中查询所有的学生
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


namespace CSEFEntityDataModel.TablePerType
{
    public static class TPTClass
    {
        // 测试TPTClass中的查询方法
        public static void TPTTest()
        {
            Query();
        }

        // 查询上下文中的所有学生
        public static void Query()
        {
            using (EFTPTEntities context = new EFTPTEntities())
            {
                var people = from p in context.People.OfType<Student>()
                             select p;
                
                foreach (var s in people)
                {
                    Console.WriteLine("{0} {1} Degree: {2}",
                        s.LastName,
                        s.FirstName,
                        s.Degree);
                }
            }
        }
    }
}
