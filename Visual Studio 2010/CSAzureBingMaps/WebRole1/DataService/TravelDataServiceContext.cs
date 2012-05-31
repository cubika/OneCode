/********************************* 模块头 **********************************\
* 模块名:  TravelDataServiceContext.cs
* 项目名:  AzureBingMaps
* 版权 (c) Microsoft Corporation.
* 
* 实现WCF的数据服务反射提供器.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Data.Objects;
using System.Data.Services;
using System.Linq;
using System.Web;
using AzureBingMaps.DAL;

namespace AzureBingMaps.WebRole.DataService
{    
    /// <summary>
    /// 实现WCF的数据服务反射提供器.
    /// </summary>
    public class TravelDataServiceContext : IUpdatable
    {
        private TravelModelContainer _entityFrameworkContext;

        public TravelDataServiceContext()
        {
            // 获得分区连接字符串.
            // PartitionKey代表当前用户.
            this._entityFrameworkContext = new TravelModelContainer(
                this.GetConnectionString(this.SetPartitionKey()));
        }

        /// <summary>
        /// 标准数据服务查询.
        /// </summary>
        public IQueryable<Travel> Travels
        {
            get
            {
                // 只查询指定用户数据.
                string partitionKey = this.SetPartitionKey();
                return this._entityFrameworkContext.Travels.Where(e => e.PartitionKey == partitionKey);
            }
        }

        public void AddReferenceToCollection(object targetResource, string propertyName, object resourceToBeAdded)
        {
            throw new NotImplementedException();
        }

        public void ClearChanges()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 创建一个新集合. 这个步骤不会设定属性.
        /// </summary>
        public object CreateResource(string containerName, string fullTypeName)
        {
            try
            {
                Type t = Type.GetType(fullTypeName + ", AzureBingMaps.DAL", true);
                object resource = Activator.CreateInstance(t);
                if (resource is Travel)
                {
                    this._entityFrameworkContext.Travels.AddObject((Travel)resource);
                }
                return resource;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to create resource. See the inner exception for more details.", ex);
            }
        }

        /// <summary>
        /// 删除一个集合.
        /// </summary>
        public void DeleteResource(object targetResource)
        {
            if (targetResource is Travel)
            {
                this._entityFrameworkContext.Travels.DeleteObject((Travel)targetResource);
            }
        }

        /// <summary>
        /// 获取单个集合. 被用于更新和删除.
        /// </summary>
        public object GetResource(IQueryable query, string fullTypeName)
        {
            ObjectQuery<Travel> q = query as ObjectQuery<Travel>;
            var enumerator = query.GetEnumerator();
            if (!enumerator.MoveNext())
            {
                throw new ApplicationException("Could not locate the resource.");
            }
            if (enumerator.Current == null)
            {
                throw new ApplicationException("Could not locate the resource.");
            }
            return enumerator.Current;
        }

        public object GetValue(object targetResource, string propertyName)
        {
            throw new NotImplementedException();
        }

        public void RemoveReferenceFromCollection(object targetResource, string propertyName, object resourceToBeRemoved)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 更新集合.
        /// </summary>
        public object ResetResource(object resource)
        {
            if (resource is Travel)
            {
                Travel updated = (Travel)resource;
                var original = this._entityFrameworkContext.Travels.Where(
                    t => t.PartitionKey == updated.PartitionKey && t.RowKey == updated.RowKey).FirstOrDefault();
                original.GeoLocationText = updated.GeoLocationText;
                original.Place = updated.Place;
                original.Time = updated.Time;
            }
            return resource;
        }

        public object ResolveResource(object resource)
        {
            return resource;
        }

        public void SaveChanges()
        {
            this._entityFrameworkContext.SaveChanges();
        }

        public void SetReference(object targetResource, string propertyName, object propertyValue)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 设定属性值.
        /// </summary>
        public void SetValue(object targetResource, string propertyName, object propertyValue)
        {
            try
            {

                var property = targetResource.GetType().GetProperty(propertyName);
                if (property == null)
                {
                    throw new InvalidOperationException("Invalid property: " + propertyName);
                }
                // PartitionKey表示用户身份,
                // 必须在服务器端获得e,
                // 否则客户端可能发送虚假身份.
                if (property.Name == "PartitionKey")
                {
                    string partitionKey = this.SetPartitionKey();
                    property.SetValue(targetResource, partitionKey, null);
                }
                else
                {
                    property.SetValue(targetResource, propertyValue, null);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to set value. See the inner exception for more details.", ex);
            }
        }

        /// <summary>
        /// 如果用户未登入, 使用默认分区.
        /// 否则分区键就是用户的email地址.
        /// </summary>
        private string SetPartitionKey()
        {
            string partitionKey = "defaultuser@live.com";
            string user = HttpContext.Current.Session["User"] as string;
            if (user != null)
            {
                partitionKey = user;
            }
            return partitionKey;
        }

        /// <summary>
        /// 获得分区链接字符串.
        /// 当前, 所有分区都保存在同一数据库中.
        /// 但是随着数据和用户的增加,
        /// 为了更好的性能我们可以把分区移动到其他数据库.
        /// 将来, 我们也可以使用SQL Azure federation的优异特性.
        /// </summary>
        private string GetConnectionString(string partitionKey)
        {
            return "name=TravelModelContainer";
        }
    }
}