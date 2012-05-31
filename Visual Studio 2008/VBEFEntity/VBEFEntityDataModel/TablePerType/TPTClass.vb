'****************************** 模块头 ******************************'
' 模块名:        TPTClass.vb
' 项目名:        VBEFEntityDataModel
' 版权 (c)       Microsoft Corporation.
'
' 这个列子说明了如何创建每种类型一个表类型. 不同之处在于所有的实体均来源于
' 一个单一表,由discriminator列来进行区分, 它还展示了如何在上下文中查询所有
' 的学生  
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


Namespace VBEFEntityDataModel.TablePerType
    Friend Class TPTClass

        ' 测试TPTClass中的查询方法
        Public Shared Sub TPTTest()
            Query()
        End Sub

        ' 查询上下文中的所有学生 
        Public Shared Sub Query()
            Using context As New EFTPTEntities()
                Dim people = From p In context.People.OfType(Of Student)() _
                    Select p

                For Each s In people
                    Console.WriteLine("{0} {1} Degree: {2}", _
                                      s.LastName, _
                                      s.FirstName, _
                                      s.Degree)
                Next
            End Using
        End Sub
    End Class
End Namespace
