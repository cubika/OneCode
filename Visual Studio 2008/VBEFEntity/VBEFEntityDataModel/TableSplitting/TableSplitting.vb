'****************************** 模块头 ******************************'
' 模块名:        TableMergingClass.vb
' 项目名:        VBEFEntityDataModel
' 版权 (c)       Microsoft Corporation.
'
' 本例说明了如何将一个表拆分为两个实体，并且展示了怎样向这两个实体插入记录和
' 查询结果
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


Namespace VBEFEntityDataModel.TableSplitting
    Friend Class TableSplittingClass

        ' 插入和查询PersonWithPersonDetail
        Public Shared Sub TableSplittingTest()
            InsertQueryPersonWithPersonDetail()
        End Sub

        ' 插入和查询PersonWithPersonDetail
        Public Shared Sub InsertQueryPersonWithPersonDetail()

            ' 创建新的Person实体
            Dim person As New Person()
            person.FirstName = "Liao"
            person.LastName = "Typot"

            ' 创建新的PersonDetail实体
            Dim personDetail As New PersonDetail()
            personDetail.PersonCategory = 0
            personDetail.HireDate = System.DateTime.Now

            ' 将person中的PersonDetail属性设置成此PersonDetail
            person.PersonDetail = personDetail

            Using context As New EFTblSplitEntitie()

                context.AddToPerson(person)

                Console.Out.WriteLine("Saving person {0}.", person.PersonID)

                ' 请注意personDetail.PersonID和person.PersonID是相同的，这就
                ' 是我们为什么喜欢Entity Framework的原因.
                Console.Out.WriteLine("Saving person detail {0}." & vbLf, _
                                      personDetail.PersonID)

                context.SaveChanges()

            End Using

            Using context As New EFTblSplitEntitie()

                ' 取得刚插入的person
                Dim person2 As Person = (From p In context.Person _
                    Where p.PersonID = person.PersonID _
                    Select p).FirstOrDefault()

                Console.Out.WriteLine( _
                    "Retrieved person {0} with person detail '{1}'.", _
                    person2.PersonID, _
                    person2.PersonDetail)

                person.PersonDetailReference.Load()

                Console.Out.WriteLine( _
                    "Retrieved hiredate for person detail {0}.", _
                    person2.PersonDetail.HireDate)

            End Using
        End Sub
    End Class
End Namespace
