'****************************** 模块头 ******************************'
' 模块名:        One2OneClass.vb
' 项目名:        VBEFEntityDataModel
' 版权 (c)       Microsoft Corporation.
'
' 本例阐明了如何通过一对一关联插入，更新，和查询这两个实体 
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


Namespace VBEFEntityDataModel.One2One
    Friend Class One2OneClass

        ' 测试One2OneClass中的所有方法
        Public Shared Sub One2OneTest()

            InsertPersonWithPersonAddress()

            UpdatePerson()

        End Sub

        ' 使用personAddress插入新的person 
        Public Shared Sub InsertPersonWithPersonAddress()

            Using context As New EFO2OEntities()

                Dim person As New Person() With _
                { _
                    .FirstName = "Lingzhi", _
                    .LastName = "Sun" _
                }

                ' PersonAddress中的PersonID将是27因为它依赖于
                ' person.PersonID
                Dim personAddress As New PersonAddress() With _
                { _
                    .PersonID = 100, _
                    .Address = "Shanghai", _
                    .Postcode = "200021" _
                }

                ' 设置navigation属性 (一对一)
                person.PersonAddress = personAddress

                context.AddToPerson(person)

                Try

                    Console.WriteLine("Inserting a person with " _
                       + "person address")

                    context.SaveChanges()

                    Query()

                Catch ex As Exception
                    Console.WriteLine(ex.Message)
                End Try
               
            End Using
        End Sub


        ' 获取带有addresses的所有people
        Public Shared Sub Query()

            Using context As New EFO2OEntities()

                Dim query = _
                    From p In context.Person.Include("PersonAddress") _
                    Select p

                Console.WriteLine("People with their addresses:")

                For Each p As Person In query

                    Console.WriteLine("{0} {1}", p.PersonID, p.LastName)

                    If Not p.PersonAddress Is Nothing Then
                        Console.WriteLine(" {0}", p.PersonAddress.Address)
                    End If

                Next

                Console.WriteLine()

            End Using
        End Sub


        ' 更新一个现存的person
        Public Shared Sub UpdatePerson()

            Using context As New EFO2OEntities()

                Dim person As New Person()

                person.PersonID = 1

                context.AttachTo("Person", person)

                person.LastName = "Chen"

                person.PersonAddress = Nothing

                Try

                    Console.WriteLine("Modifying Person 1's LastName to {0}" _
                        + ", and PersonAddress to null", person.LastName)

                    context.SaveChanges()

                    Query()

                Catch ex As Exception
                    Console.WriteLine(ex.Message)
                End Try
            End Using
        End Sub

    End Class

End Namespace
