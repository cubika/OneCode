/********************************* 模块头 **********************************\
* 模块名:  DataSource.cs
* 项目名:  AzureBingMaps
* 版权 (c) Microsoft Corporation.
* 
* 同时被MainPage和ListPage使用的数据源.
* 转移调用到WCF数据服务.
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
using System.Collections.ObjectModel;
using System.Data.Services.Client;
using System.Linq;
using AzureBingMaps.DAL;

namespace WindowsPhoneClient
{
    /// <summary>
    /// 同时被MainPage和ListPage使用的数据源.
    /// 转移调用到WCF数据服务.
    /// </summary>
    public class DataSource
    {
        // Windows Phone不能部署在Windows Azure.
        // 所以我们必须用绝对地址.
        // 本机模拟时使用 http://127.0.0.1:81/DataService/TravelDataService.svc/ . 否则, 使用你自己的Windows Azure服务地址.
        private TravelDataServiceContext _dataServiceContext = new TravelDataServiceContext(new Uri("http://127.0.0.1:81/DataService/TravelDataService.svc/"));

        private ObservableCollection<Travel> _travelItems = new ObservableCollection<Travel>();
        public event EventHandler DataLoaded;

        public ObservableCollection<Travel> TravelItems
        {
            get { return this._travelItems; }
        }

        /// <summary>
        /// 查询数据.
        /// </summary>
        public void LoadDataAsync()
        {
            this._dataServiceContext.BeginExecute<Travel>(new Uri(this._dataServiceContext.BaseUri, "Travels"), result =>
            {
                var results = this._dataServiceContext.EndExecute<Travel>(result).ToList().OrderBy(t => t.Time);
                this._travelItems = new ObservableCollection<Travel>();
                foreach (var item in results)
                {
                    this._travelItems.Add(item);
                }
                if (this.DataLoaded != null)
                {
                    this.DataLoaded(this, EventArgs.Empty);
                }
            }, null);
        }

        public void AddToTravel(Travel travel)
        {
            this._travelItems.Add(travel);
            this._dataServiceContext.AddObject("Travels", travel);
        }

        public void UpdateTravel(Travel travel)
        {
            this._dataServiceContext.UpdateObject(travel);
        }

        public void RemoveFromTravel(Travel travel)
        {
            this._travelItems.Remove(travel);
            this._dataServiceContext.DeleteObject(travel);
        }

        public void SaveChanges()
        {
            // 我们的数据服务提供器实现并不支持MERGE, 所以就执行一次完全更新(PUT)..
            this._dataServiceContext.BeginSaveChanges(SaveChangesOptions.ReplaceOnUpdate, new AsyncCallback((result) =>
            {
                var response = this._dataServiceContext.EndSaveChanges(result);
            }), null);
        }
    }
}
