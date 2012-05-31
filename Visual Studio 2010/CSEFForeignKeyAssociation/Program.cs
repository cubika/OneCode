/****************************** 模块头 ******************************\
* 模块名:	Program.cs
* 项目:		CSEFForeignKeyAssociation
* Copyright (c) Microsoft Corporation.
* 
* CSEFForeignKeyAssociation示例展示了Entity Framework(EF) 4.0的一个新特性，
* Independent Association。此示例比较了新的Foreign Key Association和Independent Association，
* 并且展示了怎样插入一个新的关联实体，通过两个关联插入已存在的实体和更新已存在实体。
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#region Using directive
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#endregion

namespace CSEFForeignKeyAssociation
{
    class Program
    {
        static void Main(string[] args)
        {
            // 测试利用Foreign Key Association的插入和更新方法。
            FKAssociation.FKAssociationClass.Test();

            // 测试Independent Association的插入和更新方法。
            IndependentAssociation.IndependentAssociationClass.Test();
        }
    }
}
