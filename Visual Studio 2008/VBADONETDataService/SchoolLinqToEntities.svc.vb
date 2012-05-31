'****************************** 模块 标识 ******************************'
' 模块名:	SchoolLinqToEntities.svc.vb
' 项目:		VBADONETDataService
' 版权 (c)  Microsoft Corporation.
' 
' SchoolLinqToEntities.svc 展示了如何建立基于 ADO.NET Entity Data Model 的
' ADO.NET Data Service。The ADO.NET Entity Data Model 连接一个由 SQLServe2005DB 
' 部署的 SQL Server 数据库。它包含了下列数据表:Person, Course, CourseGrade, 
' 以及 CourseInstructor. 本服务暴露了服务操作 CoursesByPersonID 
' 来通过 PersonID 值获得教导员的课程。另一个服务操作为 GetPersonUpdateException 
' 用来获得 Person 对象的更新错误。一个查询拦截器来过滤 Course 记录，
' 一个更新拦截器用以检查新添加的或者更改过的 Person 对象的 PersonCategory 值。
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

#Region "Imports directive"
Imports System.Data.Services
Imports System.Linq
Imports System.ServiceModel.Web
Imports VBADONETDataService.LinqToEntities
Imports System.Linq.Expressions
#End Region


Public Class SchoolLinqToEntities
    Inherits DataService(Of SQLServer2005DBEntities)

    ' 本方法只被调用一次来初始化服务的全局规则。
    Public Shared Sub InitializeService(ByVal config As  _
                                        IDataServiceConfiguration)
        ' 设定规则，表明那些实体集合和服务操作是可见的，可以更新的，等等。
        config.UseVerboseErrors = True
        config.SetEntitySetAccessRule("*", EntitySetRights.All)
        config.SetServiceOperationAccessRule("CoursesByPersonID", _
                                             ServiceOperationRights.All)
        config.SetServiceOperationAccessRule("GetPersonUpdateException", _
                                             ServiceOperationRights.All)
    End Sub

    ''' <summary>
    ''' 用以通过主键 PersonID 获得指导者课程列表的服务操作。
    ''' </summary>
    ''' <param name="ID">主键 PersonID。</param>
    ''' <returns>包含了 Course 对象的 IQueryable 集合。
    ''' </returns>
    <WebGet()> _
    Public Function CoursesByPersonID(ByVal ID As Integer) As IQueryable( _
    Of Course)

        ' 检查是否 PersonID 是可用的并且是教导员的 ID。
        If Me.CurrentDataSource.Person.Any(Function(i) i.PersonID = ID And _
                                               i.PersonCategory = 2) Then
            '  获得教导员的课程列表。
            Dim courses = From p In Me.CurrentDataSource.Person _
                          Where p.PersonID = ID _
                          Select p.Course

            ' 返回查询结果。
            Return courses.First().AsQueryable()
        Else
            ' 抛出 DataServiceException 异常如果 PersonID 不可用，
            ' 或者它是一个学员的 ID
            Throw New DataServiceException(400, _
                "Person ID 不正确或他不是一个教员。")
        End If
    End Function

    ''' <summary>
    ''' 用以获得 Person 对象更新错误信息的服务操作。
    ''' </summary>
    ''' <returns>一个 IQueryable 集合，包含了一些 Person 对象。
    ''' </returns>
    <WebGet()> _
    Public Function GetPersonUpdateException() As IQueryable(Of Person)
        Throw New DataServiceException(400, _
            "可用的 PersonCategory 值为 1" & _
            "(学生) 或 2(教员).")
    End Function

    ''' <summary>
    ''' 重写 HandleException 方法来抛出 400 Bad Request 错误到客户端。
    ''' </summary>
    ''' <param name="args">HandleException 方法的参数</param>
    Protected Overrides Sub HandleException(ByVal args As  _
                                System.Data.Services.HandleExceptionArgs)

        ' 检查是否内部异常为空 null。
        If args.Exception.InnerException IsNot Nothing Then

            ' 检查内部错误类型是否为 dataServiceException
            If TypeOf args.Exception.InnerException Is  _
            DataServiceException Then

                ' 把内部异常转化为 DataServiceException.
                Dim ex As DataServiceException = _
                DirectCast(args.Exception.InnerException, DataServiceException)

                ' 将 DataServiceException 返回给客户端。
                args.Exception = ex

            End If
        End If

        MyBase.HandleException(args)
    End Sub

    ''' <summary>
    ''' 一个查询拦截器用以过滤 Course 数据，
    ''' 返回 CourseID 大于 4000 的数据。
    ''' </summary>
    ''' <returns>lambda 表达式用来过滤 Course 数据。
    ''' </returns>
    <QueryInterceptor("Course")> _
    Public Function QueryCourse() As Expression(Of Func(Of Course, Boolean))

        '  lambda 表达式用来过滤 Course 数据。
        Return Function(c) c.CourseID > 4000

    End Function

    ''' <summary>
    ''' 一个更新拦截器用以检查新添加的或者更改过的 Person 对象的 PersonCategory 值。
    ''' </summary>
    ''' <param name="p">添加的或更改的 Person 对象。</param>
    ''' <param name="operation">更新操作.</param>
    <ChangeInterceptor("Person")> _
    Public Sub OnChangePerson(ByVal p As Person, ByVal operation As  _
                              UpdateOperations)

        ' 检查更新操作是更改还是添加。
        If operation = UpdateOperations.Add Or operation = _
        UpdateOperations.Change Then

            ' 检查 PersonCategory 值是否可用。
            If Not p.PersonCategory = 1 And Not p.PersonCategory = 2 Then

                ' 抛出 DataServieException 异常。
                Throw New DataServiceException(400, _
                    "PersonCategory 的合法值为 1" & _
                    "(学生) 或 2(教员).")
            End If
        End If
    End Sub
End Class
