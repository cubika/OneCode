/********************************* 模块头 **********************************\
* 模块名:  TravelModelContainer.cs
* 项目名:  AzureBingMaps
* 版权 (c) Microsoft Corporation.
* 
* 对象内容的分部类.
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
using System.Data;
using System.Data.Objects;
using System.Reflection;

namespace AzureBingMaps.DAL
{
    /// <summary>
    /// 对象内容的分部类.
    /// </summary>
    public partial class TravelModelContainer : ObjectContext
    {
        public override int SaveChanges(SaveOptions options)
        {
            int returnValue = 0;
            // 因为我们不调用base.SaveChanges, 我们必须手动关闭链接.
            // 否则我们将留下许多打开的链接, 最终导致链接瓶颈.
            // Entity Framework提供了base.SaveChanges内部使用的EnsureConnection和ReleaseConnection.
            // 这些是内部方法, 所以我们必须使用反射调用它们.
            var EnsureConnectionMethod = typeof(ObjectContext).GetMethod(
                "EnsureConnection", BindingFlags.Instance | BindingFlags.NonPublic);
            EnsureConnectionMethod.Invoke(this, null);
            // 使用ObjectStateManager.GetObjectStateEntries完成增加,修改,和删除集合.
            foreach (ObjectStateEntry ose in this.ObjectStateManager.GetObjectStateEntries(EntityState.Added))
            { 
                Travel travel = ose.Entity as Travel;
                if (travel != null)
                {
                    RetryPolicy retryPolicy = new RetryPolicy();
                    retryPolicy.Task = new Action(() =>
                    {
                        this.InsertIntoTravel(travel.PartitionKey,
                            travel.Place, travel.GeoLocationText, travel.Time);
                    });
                    retryPolicy.Execute();
                    returnValue++;
                }
            }
            foreach (ObjectStateEntry ose in this.ObjectStateManager.GetObjectStateEntries(EntityState.Modified))
            {
                Travel travel = ose.Entity as Travel;
                if (travel != null)
                {
                    RetryPolicy retryPolicy = new RetryPolicy();
                    retryPolicy.Task = new Action(() =>
                    {
                        this.UpdateTravel(travel.PartitionKey,
                            travel.RowKey, travel.Place, travel.GeoLocationText, travel.Time);
                    });
                    retryPolicy.Execute();
                    returnValue++;
                }
            }
            foreach (ObjectStateEntry ose in this.ObjectStateManager.GetObjectStateEntries(EntityState.Deleted))
            {
                Travel travel = ose.Entity as Travel;
                if (travel != null)
                {
                    RetryPolicy retryPolicy = new RetryPolicy();
                    retryPolicy.Task = new Action(() =>
                    {
                        this.DeleteFromTravel(travel.PartitionKey, travel.RowKey);
                    });
                    retryPolicy.Execute();
                    returnValue++;
                }
            }
            var ReleaseConnectionMethod = typeof(ObjectContext).
                GetMethod("ReleaseConnection", BindingFlags.Instance | BindingFlags.NonPublic);
            ReleaseConnectionMethod.Invoke(this, null);
            return returnValue;
        }
    }
}
