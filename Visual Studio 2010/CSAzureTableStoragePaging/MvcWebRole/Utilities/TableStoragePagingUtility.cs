/********************************* 模块头 *********************************\
* 模块名:   TableStoragePagingUtility.cs
* 项目名:   AzureTableStoragePaging
* 版权 (c) Microsoft Corporation.
* 
* 这个类能被其它应用程序重复利用.如果你想再利用这些代码,你需要做的是实现自定
* 义的ICachedDataProvider<T>类来存储TableStoragePagingUtility<T>所需要的数据.
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
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using System.Web.Mvc;

namespace MvcWebRole.Utilities
{
    public class TableStoragePagingUtility<T>
    {
        CloudStorageAccount _cloudStorageAccount;
        ICachedDataProvider<T> _provider;
        TableServiceContext _tableServiceContext;
        private ResultContinuation RC { get { return _provider.GetResultContinuation(); } set { _provider.SetResultContinuation(value); } }
        string _entitySetName;
        public int CurrentPageIndex { get { return _provider.GetCurrentIndex(); } private set { _provider.SetCurrentIndex(value); } }
        public int PageSize { get; private set; }
        public TableStoragePagingUtility(ICachedDataProvider<T> provider, CloudStorageAccount cloudStorageAccount, TableServiceContext tableServiceContext, int pageSize, string entitySetName)
        {
            this._provider = provider;
            this._cloudStorageAccount = cloudStorageAccount;
            this._entitySetName = entitySetName;
            this._tableServiceContext = tableServiceContext;
            if (pageSize <= 0) { throw new IndexOutOfRangeException("pageSize out of range"); }
            this.PageSize = pageSize;

        }
        /// <summary>
        /// 下一个页面
        /// </summary>
        /// <returns>下一个页面.如果当前页面是最后一页,返回最后一页.</returns>
        public IEnumerable<T> GetNextPage()
        {
            // 获取缓存数据
            var cachedData = this._provider.GetCachedData();
            int pageCount = 0;
            if (cachedData != null)
            {
                pageCount = Convert.ToInt32(Math.Ceiling((double)cachedData.Count() / (double)this.PageSize));
            }
            //  如果存储表中仍然有实体可读并且当前页是最后一个页面,
            //  请求表存储获取新数据.
            if (!this._provider.HasReachedEnd && CurrentPageIndex == pageCount - 1)
            {
                var q = this._tableServiceContext.CreateQuery<T>(this._entitySetName).Take(PageSize).AsTableServiceQuery();
                IAsyncResult r;
                r = q.BeginExecuteSegmented(RC, (ar) => { }, q);
                r.AsyncWaitHandle.WaitOne();
                ResultSegment<T> result = q.EndExecuteSegmented(r);
                var results = result.Results;
                this._provider.AddDataToCache(results);
                // 如果有实体返回我们需要增加页面数
                if (results.Count() > 0)
                {
                    pageCount++;
                }
                RC = result.ContinuationToken;
                // 如果返回记号为空意味着表中不再有实体了
                if (result.ContinuationToken == null)
                {
                    this._provider.HasReachedEnd = true;
                }
            }
            CurrentPageIndex = CurrentPageIndex + 1 < pageCount ? CurrentPageIndex + 1 : pageCount - 1;
            if (cachedData == null)
            {
                cachedData = this._provider.GetCachedData();
            }
            return cachedData.Skip((this.CurrentPageIndex) * this.PageSize).Take(this.PageSize);

        }
        /// <summary>
        /// 获得前一个页面
        /// </summary>
        /// <returns>前一个页面.如果当前页是第一页，返回第一页.如果缓存中没有数据,返回空集 /></returns>
        public IEnumerable<T> GetPreviousPage()
        {

            var cachedData = this._provider.GetCachedData();
            if (cachedData != null && cachedData.Count() > 0)
            {
                CurrentPageIndex = CurrentPageIndex - 1 < 0 ? 0 : CurrentPageIndex - 1;
                return cachedData.Skip(this.CurrentPageIndex * this.PageSize).Take(this.PageSize);
            }
            else { return new List<T>(); }

        }
        /// <summary>
        /// 如果有实体的缓存,返回当前页,否则检索表存储并返回第一个页面.
        /// </summary>
        /// <returns>如果有实体缓存，返回当前页.
        /// 如果没有数据缓存，第一个页面将会从表存储中重新取回并被返回..</returns>
        public IEnumerable<T> GetCurrentOrFirstPage()
        {
            var cachedData = this._provider.GetCachedData();
            if (cachedData != null && cachedData.Count() > 0)
            {
                return cachedData.Skip(this.CurrentPageIndex * this.PageSize).Take(this.PageSize);
            }
            else
            {
                return GetNextPage();
            }
        }
    }

    /// <summary>
    /// 这个类实现这个接口必须负责缓存存储，包括数据、ResultContinuation和HasReachedEnd标记.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICachedDataProvider<T>
    {
        /// <summary>
        /// 返回所有缓存数据
        /// </summary>
        /// <returns></returns>
        IEnumerable<T> GetCachedData();
        /// <summary>
        /// 把数据保存到缓存
        /// </summary>
        /// <param name="data">这个供应者的用户希望添加到缓存的数据</param>
        void AddDataToCache(IEnumerable<T> data);
        /// <summary>
        /// 保存当前索引
        /// </summary>
        /// <param name="index">这个供应者的用户发送的当前页面索引r</param>
        void SetCurrentIndex(int index);
        /// <summary>
        /// 获取当前索引
        /// </summary>
        /// <returns>保存在缓存的当前页面索引</returns>
        int GetCurrentIndex();
        /// <summary>
        /// 设置连续记号
        /// </summary>
        /// <param name="rc">这个供应者的用户发送的ResultContinuation</param>
        void SetResultContinuation(ResultContinuation rc);
        /// <summary>
        /// 获取连续记号
        /// </summary>
        /// <returns>缓存中保存的ResultContinuation</returns>
        ResultContinuation GetResultContinuation();
        /// <summary>
        /// 一个标记告诉这个供应者的用户他是否可以充分信赖缓存而不用从表存储中获取新数据.
        /// </summary>
        bool HasReachedEnd { get; set; }
    }

    /// <summary>
    ///  实现ICachedDataProvider<T>的一个例子为MVC应用程序把数据缓存在Session中.
    ///  因为它的实现使用MVC的Session,这个类的用户需要被告知不要在其它的Session变量中使用这个类的保留关键字
    /// （例如一个以"currentindex"开始的变量.如果他们不得不使用它，他们可以定义一个特殊的ID来区分它们
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MVCSessionCachedDataProvider<T> : ICachedDataProvider<T>
    {
        HttpSessionStateBase _session;
        string _id;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="c">为这个参数大体地定义这个</param>
        /// <param name="id">缓存提供者的id,你需要使用相同的id进入同一个的高速缓存</param>
        public MVCSessionCachedDataProvider(Controller c, string id)
        {
            _session = c.Session;
            _id = id;
            // 初始化currentindex
            if (_session["currentindex" + this._id] == null) { _session["currentindex" + this._id] = -1; }
            // 初始化hasreachedend标记指出是否不需要检索新数据
            if (_session["hasreachedend" + this._id] == null) { _session["hasreachedend" + this._id] = false; }
        }
        public IEnumerable<T> GetCachedData()
        {
            return _session["inmemorycacheddata" + this._id] as List<T>;
        }
        public void AddDataToCache(IEnumerable<T> data)
        {
            var inmemorycacheddata = _session["inmemorycacheddata" + this._id] as List<T>;
            if (inmemorycacheddata == null)
            {
                inmemorycacheddata = new List<T>();
            }
            inmemorycacheddata.AddRange(data);
            _session["inmemorycacheddata" + this._id] = inmemorycacheddata;
        }
        public void SetCurrentIndex(int index)
        {
            _session["currentindex" + this._id] = index;
        }
        public int GetCurrentIndex()
        {
            return Convert.ToInt32(_session["currentindex" + this._id]);
        }
        public ResultContinuation GetResultContinuation()
        {
            return _session["resultcontinuation" + this._id] as ResultContinuation;
        }
        public void SetResultContinuation(ResultContinuation rc)
        {
            _session["resultcontinuation" + this._id] = rc;
        }

        public bool HasReachedEnd
        {
            get
            {
                return Convert.ToBoolean(_session["hasreachedend" + this._id]);
            }
            set
            {
                _session["hasreachedend" + this._id] = value;
            }
        }


    }

}