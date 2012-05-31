'****************************** 模块头 ******************************\
' 模块名:	MainModule.vb
' 项目:		VBEFForeignKeyAssociation
' Copyright (c) Microsoft Corporation.
' 
' VBEFForeignKeyAssociation示例展示了Entity Framework(EF) 4.0的一个新特性，
' Foreign Key Association。此示例比较了新的Foreign Key Association和Independent Association，
' 并且展示了怎样插入一个新的关联实体，通过两个关联插入已存在的实体和更新已存在实体。
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************/
#Region "Imports directive"
Imports VBEFForeignKeyAssociation.VBEFForeignKeyAssociation
#End Region

Module MainModule

    Sub Main()
        ' 测试利用Foreign Key Association的插入和更新方法。
        FKAssociation.FKAssociationClass.Test()

        ' 测试Independent Association的插入和更新方法。
        IndependentAssociation.IndependentAssociationClass.Test()
    End Sub

End Module
