/****************************** 模块 标识 ******************************\
* 模块名:	Samples.svc.cs
* 项目:		CSADONETDataService
* 版权 (c)  Microsoft Corporation.
* 
* Samples.svc 展示了如何建立基于非关系型数据源的 ADO.NET Data Service。
* 非关系型数据源指的是一些存储于内存中的对象，其中储存了 All-In-One Code Framework
* 的一些信息。非关系型实体类也实现了 IUpdate 接口来允许客户端插入新的数据。
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
using System.Data.Services.Common;
using System.Reflection;
using System.Collections;
#endregion


namespace CSADONETDataService
{
    public class Samples : DataService<SampleProjects>
    {
        // 本方法只会被调用一次，用以初始化整个服务的规则。
        public static void InitializeService(IDataServiceConfiguration config)
        {
            // 设置规则，表示那些实体集合以及服务操作可见的，可更改的，等等
            config.UseVerboseErrors = true;
            config.SetEntitySetAccessRule("*", EntitySetRights.All);
        }
    }

    #region Non-relational data entity classes
    // 示例 Project 实体类，它的关键属性为 'ProjectName'。
    [DataServiceKey("ProjectName")]
    public class Project
    {
        public string ProjectName { get; set; }

        public string Owner { get; set; }

        public Category ProjectCategory { get; set; }
    }

    // 示例 Category 实体类，它的关键属性为 'CategoryName'。
    [DataServiceKey("CategoryName")]
    public class Category
    {
        public string CategoryName { get; set; }
    }

    // 示例 data 实体类。
    public class SampleProjects : IUpdatable
    {
        static List<Category> categories;
        static List<Project> projects;

        // 静态构造函数。
        static SampleProjects()
        {
            // 初始化 categories。
            categories = new List<Category>()
            {
                new Category { CategoryName = "COM"},
                new Category { CategoryName = "Data Access"},
                new Category { CategoryName = "Office"},
                new Category { CategoryName = "IPC and RPC"},
                new Category { CategoryName = "WinForm"},
                new Category { CategoryName = "Hook"}
            };

            // 初始化 projects。
            projects = new List<Project>()
            {
                new Project { ProjectName = "CSDllCOMServer", 
                    Owner = "Jialiang Ge", ProjectCategory = categories[0] },
                new Project { ProjectName = "VBDllCOMServer", 
                    Owner = "Jialiang Ge", ProjectCategory = categories[0] },
                new Project { ProjectName = "ATLDllCOMServer", 
                    Owner = "Jialiang Ge", ProjectCategory = categories[0] },
                new Project { ProjectName = "CSUseADONET", 
                    Owner = "Lingzhi Sun", ProjectCategory = categories[1] },
                new Project { ProjectName = "CppUseADONET", 
                    Owner = "Jialiang Ge", ProjectCategory = categories[1] },
                new Project { ProjectName = "CSLinqToObject", 
                    Owner = "Colbert Zhou", ProjectCategory = categories[1] },
                new Project { ProjectName = "CSLinqToSQL", 
                    Owner = "Rongchun Zhang", ProjectCategory = categories[1] },
                new Project { ProjectName = "CSOutlookUIDesigner", 
                    Owner = "Jie Wang", ProjectCategory = categories[2] },
                new Project { ProjectName = "CSOutlookRibbonXml", 
                    Owner = "Jie Wang", ProjectCategory = categories[2] },
                new Project { ProjectName = "CSAutomateExcel", 
                    Owner = "Jialiang Ge", ProjectCategory = categories[2] },
                new Project { ProjectName = "VBAutomateExcel", 
                    Owner = "Jialiang Ge", ProjectCategory = categories[2] },
                new Project { ProjectName = "CppFileMappingServer", 
                    Owner = "Hongye Sun", ProjectCategory = categories[3] },
                new Project { ProjectName = "CppFileMappingClient", 
                    Owner = "Hongye Sun", ProjectCategory = categories[3] },
                new Project { ProjectName = "CSReceiveWM_COPYDATA", 
                    Owner = "Riquel Dong", ProjectCategory = categories[3] },
                new Project { ProjectName = "CSSendWM_COPYDATA", 
                    Owner = "Riquel Dong", ProjectCategory = categories[3] },
                new Project { ProjectName = "CSWinFormGeneral", 
                    Owner = "Zhixin Ye", ProjectCategory = categories[4] },
                new Project { ProjectName = "CSWinFormDataBinding", 
                    Owner = "Zhixin Ye", ProjectCategory = categories[4] },
                new Project { ProjectName = "CSWindowsHook", 
                    Owner = "Rongchun Zhang", ProjectCategory = categories[5] }
            };
        }

        // 公共属性用以在客户端从 ADO.NET Data Service 获得 projects 信息。
        public IQueryable<Project> Projects
        {
            get { return projects.AsQueryable(); }
        }

        // 公共属性用以在客户端从 ADO.NET Data Service 获得 categories 信息。
        public IQueryable<Category> Categories
        {
            get { return categories.AsQueryable(); }
        }

        // 实现 IUpdatable 接口的方法用以添加数据。
        #region IUpdatable Members

        // 临时保存添加的对象。
        object tempObj = null;

        /// <summary>
        /// 向集合中添加给定的值。
        /// </summary>
        /// <param name="targetResource">目标对象定义了相关属性。
        /// </param>
        /// <param name="propertyName">需要更新的属性名。
        /// </param>
        /// <param name="resourceToBeAdded">需要添加的属性值。
        /// </param>
        public void AddReferenceToCollection(object targetResource, string 
            propertyName, object resourceToBeAdded)
        {
            // 获得目标对象的类型。
            Type t = targetResource.GetType();

            // 获得需要更新的属性。
            PropertyInfo pi = t.GetProperty(propertyName);
            if (pi != null)
            {
                // 获得集合中的属性值。
                IList collection = (IList)pi.GetValue(targetResource, null);

                // 将资源添加进集合中。
                collection.Add(resourceToBeAdded);
            }
        }

        /// <summary>
        /// 回滚所有未保存的操作。
        /// </summary>
        public void ClearChanges()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 创建一个所给对象的资源并附加到给定的Container上。
        /// </summary>
        /// <param name="containerName">容器名，创建的资源将会被附加到该容器上 。
        /// </param>
        /// <param name="fullTypeName">程序集限定名。
        /// </param>
        /// <returns>资源对象。
        /// </returns>
        public object CreateResource(string containerName, string 
            fullTypeName)
        {
            // 获得资源的类型。
            Type t = Type.GetType(fullTypeName, true);

            // 创建一个资源对象的实例。
            object resource = Activator.CreateInstance(t);
            
            // 返回资源对象。
            return resource;
        }

        /// <summary>
        /// 删除给定资源。
        /// </summary>
        /// <param name="targetResource">需要删除的资源对象。
        /// </param>
        public void DeleteResource(object targetResource)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获得查询指向的给定类型的资源。
        /// </summary>
        /// <param name="query">指向特定资源的查询。
        /// </param>
        /// <param name="fullTypeName">程序集限定名
        /// </param>
        /// <returns>资源对象
        /// </returns>
        public object GetResource(IQueryable query, string fullTypeName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获得给定对象相关属性的值。
        /// </summary>
        /// <param name="targetResource">定义了相关属性的目标对象。
        /// </param>
        /// <param name="propertyName">需要更新的属性名
        /// </param>
        /// <returns>给定目标资源的属性值
        /// </returns>
        public object GetValue(object targetResource, string propertyName)
        {
            // 获得目标对象类型。
            Type t = targetResource.GetType();

            // 获得属性。
            PropertyInfo pi = t.GetProperty(propertyName);
            if (pi != null)
            {
                // 返回属性值。
                return pi.GetValue(targetResource, null);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 从集合中移除给定值。
        /// </summary>
        /// <param name="targetResource">定义了相关属性的目标对象。
        /// </param>
        /// <param name="propertyName">需要更新的属性名。
        /// </param>
        /// <param name="resourceToBeRemoved">需要移除的属性值。
        /// </param>
        public void RemoveReferenceFromCollection(object targetResource, 
            string propertyName, object resourceToBeRemoved)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 重置给定资源为默认值。
        /// </summary>
        /// <param name="resource">需要被重置的资源。
        /// </param>
        /// <returns>被重置后的资源。</returns>
        public object ResetResource(object resource)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 返回被所给资源对象所代表的实际对象实例。
        /// </summary>
        /// <param name="resource">需要被用来解析的资源对象。 
        /// </param>
        /// <returns>被资源对象所代表的实际对象实例。
        /// </returns>
        public object ResolveResource(object resource)
        {
            return resource;
        }

        /// <summary>
        /// 保存所有未保存的当前操作。
        /// </summary>
        public void SaveChanges()
        {
            // 将临时对象加到集合中去。
            if (tempObj != null)
            {
                Type t = tempObj.GetType();
                if (t.Name == "Category")
                {
                    SampleProjects.categories.Add((Category)tempObj);
                }
                else if (t.Name == "Project")
                {
                    SampleProjects.projects.Add((Project)tempObj);
                }
            }
        }

        /// <summary>
        /// 设置给定目标对象相关属性的引用值。 
        /// </summary>
        /// <param name="targetResource">定义了相关属性的目标对象。 
        /// </param>
        /// <param name="propertyName">需要更新的属性名。
        /// </param>
        /// <param name="propertyValue">属性值。</param>
        public void SetReference(object targetResource, string propertyName, 
            object propertyValue)
        {
            ((IUpdatable)this).SetValue(targetResource, propertyName, 
                propertyValue);
        }

        /// <summary>
        /// 设置给定对象属性值。
        /// </summary>
        /// <param name="targetResource">定义了相关属性的目标对象。
        /// </param>
        /// <param name="propertyName">需要被更新的属性名。
        /// </param>
        /// <param name="propertyValue">属性值。</param>
        public void SetValue(object targetResource, string propertyName, 
            object propertyValue)
        {
            // 获得资源对象类型。
            Type t = targetResource.GetType();

            // 获得需要更新的属性。
            PropertyInfo pi = t.GetProperty(propertyName);
            if (pi != null)
            {
                // 设置属性值。
                pi.SetValue(targetResource, propertyValue, null);
            }

            // 将目标对象保存到临时对象中。
            tempObj = targetResource;
        }
        #endregion
    }
    #endregion
}
