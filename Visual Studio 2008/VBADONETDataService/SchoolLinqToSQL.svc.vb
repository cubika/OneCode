'****************************** 模块 标识 ******************************'
' 模块名:	SchoolLinqToSQL.svc.vb
' 项目:		VBADONETDataService
' 版权  (c) Microsoft Corporation.
' 
' SchoolLinqToSQL.svc 展示如何基于LinqToSql Data Classes 建立 
' ADO.NET Data Service.LinqToSql Data Class 连接一个由 SQLServe2005DB 
' 部署的 SQL Server 数据库。它包含了下列数据表:Person, Course, CourseGrade, 
' 以及 CourseInstructor.服务暴露了一个服务操作 SearchCourses,该操作使客户端
' 可以使用 SQL 语句来检索 Course 对象
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
Imports VBADONETDataService.LinqToSQL
#End Region


Public Class SchoolLinqToSQL
    Inherits DataService(Of SchoolLinqToSQLDataContext)

    ' 本方法只被调用一次来初始化服务的全局规则。.
    Public Shared Sub InitializeService(ByVal config As  _
                                        IDataServiceConfiguration)
        ' 设定规则，表明那些实体集合和服务操作是可见的，可以更新的，等等。
        config.UseVerboseErrors = True
        config.SetEntitySetAccessRule("*", EntitySetRights.All)
        config.SetServiceOperationAccessRule("SearchCourses", _
                                             ServiceOperationRights.All)
    End Sub

    ''' <summary>
    ''' 本操作使客户端
    ''' 可以使用 SQL 语句来检索 Course 对象
    ''' </summary>
    ''' <param name="searchText">要执行的 SQL 查询语句</param>
    ''' <returns>包含了Course 记录的 IQueryable 集合。
    ''' </returns>
    <WebGet()> _
    Public Function SearchCourses(ByVal searchText As String) As  _
    IQueryable(Of Course)

        ' 调用 DataContext.ExecuteQuery 来执行查询语句
        Return Me.CurrentDataSource.ExecuteQuery(Of Course) _
        (searchText).AsQueryable()

    End Function
End Class
