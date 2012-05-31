'****************************** 模块头 ******************************'
' 模块名:        TableMergingClass.vb
' 项目名:        VBEFEntityDataModel
' 版权 (c)       Microsoft Corporation.
'
' 这个列子说明了如何将多个表合并为一个实体并且从两张表中查询字段
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
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
#End Region


Namespace VBEFEntityDataModel.TableMerging
    Friend Class TableMergingClass

        ' 测试TableMergingClass中的所有方法
        Public Shared Sub TableMergingTest()
            Query()
        End Sub

        ' 查询合并后的表中的第一个Person 
        Public Shared Sub Query()
            Using context As New EFTblMergeEntities()

                Dim person As Person = (context.Person).First()

                Console.WriteLine("{0} " & vbLf & "{1} {2} " & vbLf & "{3}", _
                                  person.PersonID, _
                                  person.FirstName, _
                                  person.LastName, _
                                  person.Address)
            End Using
        End Sub
    End Class

End Namespace
