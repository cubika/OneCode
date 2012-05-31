/****************************** 模块 标识 ******************************\
* 模块名:	SchoolLinqToEntities.svc.cs
* 项目:		CSADONETDataService
* 版权 (c)  Microsoft Corporation.
* 
* SchoolLinqToEntities.svc 展示了如何建立基于 ADO.NET Entity Data Model 的
* ADO.NET Data Service。The ADO.NET Entity Data Model 连接一个由 SQLServe2005DB 
* 部署的 SQL Server 数据库。它包含了下列数据表:Person, Course, CourseGrade, 
* 以及 CourseInstructor. 本服务暴露了服务操作 CoursesByPersonID 
* 来通过 PersonID 值获得教导员的课程。另一个服务操作为 GetPersonUpdateException 
* 用来获得 Person 对象的更新错误。一个查询拦截器来过滤 Course 记录，
* 一个更新拦截器用以检查新添加的或者更改过的 Person 对象的 PersonCategory 值。
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
using CSADONETDataService.LinqToEntities;
using System.Linq.Expressions;
#endregion


namespace CSADONETDataService
{
    public class SchoolLinqToEntities : DataService<SQLServer2005DBEntities>
    {
        // 本方法只被调用一次来初始化服务的全局规则。
        public static void InitializeService(IDataServiceConfiguration config)
        {
            // 设定规则，表明那些实体集合和服务操作是可见的，可以更新的，等等。
            config.UseVerboseErrors = true;
            config.SetEntitySetAccessRule("*", EntitySetRights.All);
            config.SetServiceOperationAccessRule("CoursesByPersonID", 
                ServiceOperationRights.All);
            config.SetServiceOperationAccessRule("GetPersonUpdateException",
                ServiceOperationRights.All);
        }

        /// <summary>
        /// 重写 HandleException 方法来抛出 400 Bad Request 错误到客户端。
        /// </summary>
        /// <param name="args">HandleException 方法的参数</param>
        protected override void HandleException(HandleExceptionArgs args)
        {
            // 检查是否内部异常为空 null。
            if (args.Exception.InnerException != null)
            {
                // 把内部异常转化为 DataServiceException
                DataServiceException ex = args.Exception.InnerException as
                    DataServiceException;

                // 检查是否内部异常的类型为 DataServiceException 
                // 并且 状态代码为 400(Bad Request) 
                if (ex != null && ex.StatusCode == 400)
                {
                    // 将 DataServiceException 返回给客户端。
                    args.Exception = ex;
                }
            }

            base.HandleException(args);
        }

        /// <summary>
        /// 用以获得 Person 对象更新错误信息的服务操作。
        /// </summary>
        /// <returns>一个 IQueryable 集合，包含了一些 Person 对象。
        /// </returns>
        [WebGet]
        public IQueryable<Person> GetPersonUpdateException()
        {
            // 将 Person 对象更新错误信息抛出并返回到客户端。
            throw new DataServiceException(400, 
                "可用的 PersonCategory 值为 1" + 
                "(学生) 或 2(教员)."); ;
        }

        /// <summary>
        /// 用以通过主键 PersonID 获得指导者课程列表的服务操作。
        /// </summary>
        /// <param name="ID">主键 PersonID。</param>
        /// <returns>包含了 Course 对象的 IQueryable 集合。
        /// </returns>
        [WebGet]
        public IQueryable<Course> CoursesByPersonID(int ID)
        {
            // 检查是否 PersonID 是可用的并且是教导员的 ID。
            if (this.CurrentDataSource.Person.Any(i => i.PersonID == ID &&
                i.PersonCategory == 2))
            {
                // 获得教导员的课程列表。
                var courses = from p in this.CurrentDataSource.Person
                              where p.PersonID == ID
                              select p.Course;

                // 返回查询结果。
                return courses.First().AsQueryable();
            }
            else
            {
                // 抛出 DataServiceException 异常如果 PersonID 不可用，
                // 或者它是一个学员的 ID
                throw new DataServiceException(400, "Person ID 不正确" +
                    " 或他不是一个教员。");
            }
        }


        /// <summary>
        /// 一个查询拦截器用以过滤 Course 数据，
        /// 返回 CourseID 大于 4000 的数据。
        /// </summary>
        /// <returns>lambda 表达式用来过滤 Course 数据。
        /// </returns>
        [QueryInterceptor("Course")]
        public Expression<Func<Course, bool>> QueryCourse()
        {
            // lambda 表达式用来过滤 Course 数据。
            return c => c.CourseID > 4000;
        }


        /// <summary>
        /// 一个更新拦截器用以检查新添加的或者更改过的 Person 对象的 PersonCategory 值。
        /// </summary>
        /// <param name="p">添加的或更改的 Person 对象。</param>
        /// <param name="operation">更新操作。</param>
        [ChangeInterceptor("Person")]
        public void OnChangePerson(Person p, UpdateOperations operation)
        {
            // 检查更新操作是更改还是添加。
            if (operation == UpdateOperations.Add || 
                operation == UpdateOperations.Change)
            {
                // 检查 PersonCategory 值是否可用。
                if (p.PersonCategory != 1 && p.PersonCategory != 2)
                {
                    // 抛出 DataServieException 异常。
                    throw new DataServiceException(400,
                        "PersonCategory 的合法值为 1" + 
                        "(学生) 或 2(教员).");
                }
            }
        }
    }
}
