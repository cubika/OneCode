'****************************** 模块头 ******************************'
' 模块名:    TPHClass.vb
' 项目名:        VBEFEntityDataModel
' 版权 (c) Microsoft Corporation.
'
' 这个列子说明了如何创建每种层次结构一个表类型.每种类型一个表 (table-per-type)
' 模型是一种模仿继承的方式，每个实体被映射到一个存储中的单独的表.然后本例展示
' 了如何查询people类型的列表，获取Person, Student和BusinessStudent相应的属性
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


Namespace VBEFEntityDataModel.TablePerHierarchy

    Friend Class TPHClass

        ' 测试TPHClass中的查询方法
        Public Shared Sub TPHTest()
            Query()
        End Sub

        ' 查询people类型的列表， 输出Person, Student和BusinessStudent的属性
        Public Shared Sub Query()
            Using context As New EFTPHEntities()

                Dim people = From p In context.PersonSet _
                    Select p

                For Each p In people
                    Console.WriteLine("Student {0} {1}", p.LastName, p.FirstName)

                    If TypeOf p Is Student Then
                        Console.WriteLine("EnrollmentDate: {0}", _
                            DirectCast(p, Student).EnrollmentDate)
                    End If

                    If TypeOf p Is BusinessStudent Then
                        Console.WriteLine("BusinessCredits: {0}", _
                            DirectCast(p, BusinessStudent).BusinessCredits)
                    End If

                Next
            End Using
        End Sub
    End Class
End Namespace
