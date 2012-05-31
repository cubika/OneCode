/****************************** 模块 标识 ******************************\
* 模块名:	IUpdatableLinqToSQL.cs
* 项目:		CSADONETDataService
* 版权 (c)  Microsoft Corporation.
* 
* IUpdatableLinqToSQL.cs 是 ADO.NET Data Service IUpdatable 接口基于LinqToSQL
* 数据源的具体实现。它包含了一些 partial Linq to SQL entity classes 来设置DataServiceKey属性。 
* ADO.NET Data Service IUpdatable 接口基于 LinqToSQL 的具体实现代码
* 可以从下方的地址中下载到。
* http://code.msdn.microsoft.com/IUpdateableLinqToSql
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#region Using directives
using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data.Services;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Data.Services.Common;
#endregion


namespace CSADONETDataService.LinqToSQL
{
    #region DataContext partial class
    public partial class SchoolLinqToSQLDataContext : IUpdatable
    {
        /// <summary>
        /// 创建给定类型的资源，并将它附属在给定的容器上 
        /// </summary>
        /// <param name="containerName">容器名，资源需要被添加在该容器上 
        /// </param>
        /// <param name="fullTypeName"> 程序集限定名
        /// </param>
        /// <returns>一个给定类型的资源对象以及它所从属的Container信息 
        /// </returns>
        public object CreateResource(string containerName, 
            string fullTypeName)
        {
            Type t = Type.GetType(fullTypeName, true);
            ITable table = GetTable(t);
            object resource = Activator.CreateInstance(t);
            table.InsertOnSubmit(resource);
            return resource;
        }

        /// <summary>
        /// 获得查询指向的给定类型的资源
        /// </summary>
        /// <param name="query">查询指向一个特定的资源
        /// </param>
        /// <param name="fullTypeName">程序集限定名
        /// </param>
        /// <returns>一个给定类型的资源对象以及引用它的查询
        /// </returns>
        public object GetResource(IQueryable query, string fullTypeName)
        {
            object resource = query.Cast<object>().SingleOrDefault();

            // 对 Delegate 类型来说,程序集限定名可以为null
            if (fullTypeName != null && resource.GetType().FullName != 
                fullTypeName)
                throw new Exception("未知的对象");
            return resource;
        }


        /// <summary>
        /// 重置给定的资源为默认值
        /// </summary>
        /// <param name="resource">需要重置的资源对象
        /// </param>
        /// <returns>重置为默认值后的资源对象</returns>
        public object ResetResource(object resource)
        {
            Type t = resource.GetType();
            MetaTable table = Mapping.GetTable(t);
            object dummyResource = Activator.CreateInstance(t);
            foreach (var member in table.RowType.DataMembers)
            {
                if (!member.IsPrimaryKey && !member.IsDeferred &&
                    !member.IsAssociation && !member.IsDbGenerated)
                {
                    object defaultValue = member.MemberAccessor.
                        GetBoxedValue(dummyResource);
                    member.MemberAccessor.SetBoxedValue(ref resource, 
                        defaultValue);
                }
            }
            return resource;
        }

        /// <summary>
        /// 设置目标对象相关属性的值
        /// </summary>
        /// <param name="targetResource">定义了相关属性的对象目标 
        /// </param>
        /// <param name="propertyName">需要被更新的属性名
        /// </param>
        /// <param name="propertyValue">属性值</param>
        public void SetValue(object targetResource, string propertyName, 
            object propertyValue)
        {
            MetaTable table = Mapping.GetTable(targetResource.GetType());
            MetaDataMember member = table.RowType.DataMembers.Single(
                x => x.Name == propertyName);
            member.MemberAccessor.SetBoxedValue(ref targetResource, 
                propertyValue);
        }

        /// <summary>
        /// 从目标对象上获得相关属性的值
        /// </summary>
        /// <param name="targetResource">定义了相关属性的目标对象
        /// </param>
        /// <param name="propertyName">需要被更新的属性名
        /// </param>
        /// <returns>目标对象的属性值
        /// </returns>
        public object GetValue(object targetResource, string propertyName)
        {
            MetaTable table = Mapping.GetTable(targetResource.GetType());
            MetaDataMember member = table.RowType.DataMembers.Single(
                x => x.Name == propertyName);
            return member.MemberAccessor.GetBoxedValue(targetResource);
        }

        /// <summary>
        /// 设置给定对象的相关属性值
        /// </summary>
        /// <param name="targetResource">定义了相关属性的目标对象 
        /// </param>
        /// <param name="propertyName">需要被更新的属性名
        /// </param>
        /// <param name="propertyValue">属性值</param>
        public void SetReference(object targetResource, string propertyName, 
            object propertyValue)
        {
            ((IUpdatable)this).SetValue(targetResource, propertyName, 
                propertyValue);
        }

        /// <summary>
        /// 将值添加到集合中去
        /// </summary>
        /// <param name="targetResource">定义了相关属性的目标对象 
        /// </param>
        /// <param name="propertyName">需要被更新的属性名
        /// </param>
        /// <param name="resourceToBeAdded">需要被添加的属性值
        /// </param>
        public void AddReferenceToCollection(object targetResource, string 
            propertyName, object resourceToBeAdded)
        {
            PropertyInfo pi = targetResource.GetType().GetProperty(
                propertyName);
            if (pi == null)
                throw new Exception("无法找到该属性");
            IList collection = (IList)pi.GetValue(targetResource, null);
            collection.Add(resourceToBeAdded);
        }

        /// <summary>
        /// 从集合中删除值
        /// </summary>
        /// <param name="targetResource">定义了相关属性的目标对象 
        /// </param>
        /// <param name="propertyName">需要被更新的属性名
        /// </param>
        /// <param name="resourceToBeRemoved">需要被删除的属性值
        /// </param>
        public void RemoveReferenceFromCollection(object targetResource, 
            string propertyName, object resourceToBeRemoved)
        {
            PropertyInfo pi = targetResource.GetType().GetProperty(
                propertyName);
            if (pi == null)
                throw new Exception("无法找到该属性");
            IList collection = (IList)pi.GetValue(targetResource, null);
            collection.Remove(resourceToBeRemoved);
        }

        /// <summary>
        /// 删除所给的资源
        /// </summary>
        /// <param name="targetResource">需要删除的资源
        /// </param>
        public void DeleteResource(object targetResource)
        {
            ITable table = GetTable(targetResource.GetType());
            table.DeleteOnSubmit(targetResource);
        }

        /// <summary>
        /// 保存所有未保存的操作
        /// </summary>
        public void SaveChanges()
        {
            SubmitChanges();
        }

        /// <summary>
        /// 返回资源对象所代表的实际类型的对象
        /// </summary>
        /// <param name="resource">需要被获得的资源对象实例 
        /// </param>
        /// <returns>被资源对象所代表实际对象的实例
        /// </returns>
        public object ResolveResource(object resource)
        {
            return resource;
        }

        /// <summary>
        /// 回滚所有未保存的记录。
        /// </summary>
        public void ClearChanges()
        {
            // 如果你不希望抛出任何的异常，可以删除下行代码
            throw new NotSupportedException();
        }
    }
    #endregion

    #region Linq to SQL entity partial classes
    // 把 Course 类的关键属性设置成 'CourseID'
    [DataServiceKey("CourseID")]
    public partial class Course
    { }

    // 把 CourseGrade 类的关键属性设置为 'EnrollmentID'
    [DataServiceKey("EnrollmentID")]
    public partial class CourseGrade
    { }

    // 把 CourseInstructor 类的关键属性设置为 'CourseID' and 'PersonID'
    [DataServiceKey("CourseID", "PersonID")]
    public partial class CourseInstructor
    { }

    // 把 Person 类的关键属性设置为 'PersonID'
    [DataServiceKey("PersonID")]
    public partial class Person
    { }
    #endregion
}