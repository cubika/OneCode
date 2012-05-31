'****************************** 模块头 ******************************'
' 模块名:	    Many2ManyClass.vb
' 项目名:		VBEFEntityDataModel
' 版权 (c)      Microsoft Corporation.
' 
' 本例阐明了如何通过多对多关联插入，更新，和查询这两个实体
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


Namespace VBEFEntityDataModel.Many2Many
    Friend Class Many2ManyClass

        ' 测试Many2ManyClass中的所有方法
        Public Shared Sub Many2ManyTest()

            InsertPersonWithCourse()

            InsertPerson()

            UpdatePerson()

        End Sub

        ' 插入新的person包括新的course
        Public Shared Sub InsertPersonWithCourse()

            Using context As New EFM2MEntities()

                Dim person As New Person() With _
                { _
                    .FirstName = "Yichun", _
                    .LastName = "Feng" _
                }

                Dim course As New Course() With _
                { _
                    .CourseID = 2211, _
                    .Title = "UML" _
                }

                person.Course.Add(course)

                context.AddToPerson(person)

                Try

                    Console.WriteLine("Inserting Person {0} {1} with course " _
                       + "{2}.", person.FirstName, person.LastName, _
                        course.Title)

                    context.SaveChanges()

                    Query()

                Catch ex As Exception
                    Console.WriteLine(ex.Message)
                End Try
            End Using
        End Sub


        ' 插入新的person到现存的course
        Public Shared Sub InsertPerson()

            Using context As New EFM2MEntities()
                Dim person As New Person() With _
                { _
                    .FirstName = "Ji", _
                    .LastName = "Zhou" _
                }

                person.Course.Add(context.Course.FirstOrDefault())

                context.AddToPerson(person)

                Try

                    Console.WriteLine("Inserting Person {0} {1} .", _
                        person.FirstName, _
                        person.LastName)

                    context.SaveChanges()

                    Query()

                Catch ex As Exception
                    Console.WriteLine(ex.Message)
                End Try
            End Using
        End Sub


        ' 获取所有的persons和他们的courses
        Public Shared Sub Query()

            Using context As New EFM2MEntities()

                Dim query = From p In context.Person.Include("Course") _
                            Select p

                Console.WriteLine("Persons with their Course")

                For Each p As Person In query

                    Console.WriteLine("{0}: {1} {2}", _
                                      p.PersonID, _
                                      p.FirstName, _
                                      p.LastName)

                    For Each c As Course In p.Course
                        Console.WriteLine(" {0}", c.Title)
                    Next
                Next

                Console.WriteLine()
            End Using
        End Sub


        ' 更新一个现存的person 
        Public Shared Sub UpdatePerson()

            Using context As New EFM2MEntities()

                Dim person As New Person()

                person.PersonID = 34

                context.AttachTo("Person", person)

                person.FirstName = "Monica"

                Dim course As New Course() With _
                { _
                    .CourseID = 2208, _
                    .Title = "Operating System" _
                }


                person.Course.Add(course)

                Try
                    Console.WriteLine( _
                        "Modifying Person 34's last to {0} and add course {1} " _
                        & "to it", person.LastName, course.Title)

                    context.SaveChanges()

                    Query()

                Catch ex As Exception
                    Console.WriteLine(ex.Message)
                End Try
            End Using
        End Sub
    End Class
End Namespace