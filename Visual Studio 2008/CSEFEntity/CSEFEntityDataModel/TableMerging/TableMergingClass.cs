/****************************** 模块头 ******************************\
* 模块名:        TableMergingClass.cs
* 项目名:        CSEFEntityDataModel
* 版权 (c)       Microsoft Corporation.
*
* 这个列子说明了如何将多个表合并为一个实体并且从两张表中查询字段
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


namespace CSEFEntityDataModel.TableMerging
{
    public static class TableMergingClass
    {
        // 测试TableMergingClass中的所有方法
        public static void TableMergingTest()
        {
            Query();
        }

        // 查询合并后的表中的第一个Person
        public static void Query()
        {
            using (EFTblMergeEntities context = new EFTblMergeEntities())
            {
                Person person = (context.Person).First();

                Console.WriteLine("{0}   \n{1} {2}   \n{3}",
                    person.PersonID, 
                    person.FirstName, 
                    person.LastName, 
                    person.Address);
            }
        }
    }

}
