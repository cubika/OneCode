/****************************** 模块 标识 ******************************\
* 模块名:	SchoolLinqToSQL.svc.cs
* 项目:		CSADONETDataService
* 版权 (c)  Microsoft Corporation.
* 
* SchoolLinqToSQL.svc 展示如何基于LinqToSql Data Classes 建立 
* ADO.NET Data Service.LinqToSql Data Class 连接一个由 SQLServe2005DB 
* 部署的 SQL Server 数据库。它包含了下列数据表:Person, Course, CourseGrade, 
* 以及 CourseInstructor.服务暴露了一个服务操作 SearchCourses,该操作使客户端
* 可以使用 SQL 语句来检索 Course 对象
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#region Using directive
using System;
using System.Collections.Generic;
using System.Data.Services;
using System.Linq;
using System.ServiceModel.Web;
using System.Web;
using CSADONETDataService.LinqToSQL;
#endregion


namespace CSADONETDataService
{
    public class SchoolLinqToSQL : DataService<SchoolLinqToSQLDataContext>
    {
        // 本方法只被调用一次来初始化服务的全局规则。
        public static void InitializeService(IDataServiceConfiguration config)
        {
            // 设定规则，表明那些实体集合和服务操作是可见的，可以更新的，等等。
            config.UseVerboseErrors = true;
            config.SetEntitySetAccessRule("*", EntitySetRights.All);
            config.SetServiceOperationAccessRule("SearchCourses", 
                ServiceOperationRights.All);
        }

        /// <summary>
        /// 本操作使客户端
        /// 可以使用 SQL 语句来检索 Course 对象
        /// </summary>
        /// <param name="searchText">要执行的 SQL 查询语句</param>
        /// <returns>包含了Course 记录的 IQueryable 集合。
        /// </returns>
        [WebGet]
        public IQueryable<Course> SearchCourses(string searchText)
        {
            // 调用 DataContext.ExecuteQuery 来执行查询语句
            return this.CurrentDataSource.ExecuteQuery<Course>(searchText).
                AsQueryable();
        }
    }
}
