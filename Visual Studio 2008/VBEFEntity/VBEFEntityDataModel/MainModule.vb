'****************************** Module Header ******************************'
' 模块名称:    MainModule.vb
' 工程:        VBEFEntityDataModel
' 版权 (c) Microsoft Corporation.
'
' VBEFEntityDataModel示例通过多种方式说明了如何使用EDM。 其中包括多对多关系、
' 一对多关系、一对一 关系、表的合并、表的拆分、每个层次结构一个表继承和每种
' 类型一个表继承。 在本例中您将看到针对实体的插入、更新和查询操作。
' 
'
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
'
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************'


#Region "Imports directives"
Imports VBEFEntityDataModel.VBEFEntityDataModel.TableSplitting
Imports VBEFEntityDataModel.VBEFEntityDataModel.TablePerType
Imports VBEFEntityDataModel.VBEFEntityDataModel.TablePerHierarchy
Imports VBEFEntityDataModel.VBEFEntityDataModel.TableMerging
Imports VBEFEntityDataModel.VBEFEntityDataModel.One2One
Imports VBEFEntityDataModel.VBEFEntityDataModel.One2Many
Imports VBEFEntityDataModel.VBEFEntityDataModel.Many2Many
#End Region


Module MainModule
    Sub Main()

        Console.WriteLine("Many-to-Many test:")

        '通过多对多关联对实体进行插入，更新和查询操作
        Many2ManyClass.Many2ManyTest()

        Console.WriteLine()

        Console.WriteLine("One-to-Many test:")

        '通过一对多关联对实体进行插入，更新和查询操作
        One2ManyClass.One2ManyTest()

        Console.WriteLine()

        Console.WriteLine("One-to-One test:")

        ' 通过一对一关联对实体进行插入，更新和查询操作
        One2OneClass.One2OneTest()

        Console.WriteLine()

        Console.WriteLine("Table merging test:")

        ' 执行查询操作来合并实体
        TableMergingClass.TableMergingTest()

        Console.WriteLine()

        Console.WriteLine("Table splitting test:")

        ' 在单独的表中执行插入和查询操作
        TableSplittingClass.TableSplittingTest()

        Console.WriteLine()

        Console.WriteLine("Table-Per-Hierarchy inheritance test:")

        ' 在TablePerType中对实体进行查询操作
        TPTClass.TPTTest()

        Console.WriteLine()

        Console.WriteLine("Table-Per-Type inheritance test:")

        '在TablePerHierarchy中对实体进行查询操作
        TPHClass.TPHTest()

        Console.ReadLine()
    End Sub

End Module
