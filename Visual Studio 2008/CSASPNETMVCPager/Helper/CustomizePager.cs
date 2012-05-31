/**************************************** 模块头 *****************************************\
* 模块名:   CustomizePager.cs
* 项目名:   CSMVCPager
* 版权 (c) Microsoft Corporation
*
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
*
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER 
* EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF 
* MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\******************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Web.Mvc;

namespace CSASPNETMVCPager.Helper
{
    public class Pager<T>
    {
        /// <summary>
        /// 决定每一页显示多少记录
        /// </summary>
        public int pageSize = 6;

        /// <summary>
        /// 实例化分页对象
        /// </summary>
        /// <param name="collection">使用ICollection接口的数据源<T></param>
        /// <param name="currentPageIndex">当前页面索引</param>
        /// <param name="requestBaseUrl">请求基本url</param>
        public Pager(ICollection<T> collection, int currentPageIndex, string requestBaseUrl)
        {
            int count = collection.Count;
            int remainder = count % pageSize;
            int quotient = count / pageSize;

            this.CurrentPageIndex = currentPageIndex;
            this.TotalPages = remainder == 0 ? quotient : quotient + 1;
            this.RequestBaseUrl = requestBaseUrl;
        }

        /// <summary>
        /// 实例化分页对象
        /// </summary>
        /// <param name="collection">使用ICollection接口的数据源<T></param>
        /// <param name="currentPageIndex">当前页面索引</param>
        /// <param name="requestBaseUrl">请求基本url</param>
        /// <param name="imgUrlForUp">前一页的图片</param>
        /// <param name="imgUrlForDown">下一页的图片</param>
        public Pager(ICollection<T> collection, int currentPageIndex, string requestBaseUrl, string imgUrlForUp, string imgUrlForDown)
        {
            int count = collection.Count;
            int remainder = count % pageSize;
            int quotient = count / pageSize;

            this.CurrentPageIndex = currentPageIndex;
            this.TotalPages = remainder == 0 ? quotient : quotient + 1;
            this.RequestBaseUrl = requestBaseUrl;
            this.ImageUrlForDown = imgUrlForDown;
            this.ImageUrlForUp = imgUrlForUp;
        }

        /// <summary>
        /// 示例化一个分页对象
        /// </summary>
        /// <param name="collection">使用ICollection接口的数据源<T></param>
        /// <param name="currentPageIndex">当前页面序号</param>
        /// <param name="requestBaseUrl">请求基本url</param>
        /// <param name="imgUrlForUp">前一页的图片</param>
        /// <param name="imgUrlForDown">下一页的图片</param>
        /// <param name="pagesSize">决定需要显示的页面最大数字</param>
        public Pager(IList<T> collection, int currentPageIndex, string requestBaseUrl, string imgUrlForUp, string imgUrlForDown, int pagesSize)
        {
            int count = collection.Count;
            int remainder = count % pageSize;
            int quotient = count / pageSize;

            this.CurrentPageIndex = currentPageIndex;
            this.TotalPages = remainder == 0 ? quotient : quotient + 1;
            this.RequestBaseUrl = requestBaseUrl;
            this.ImageUrlForDown = imgUrlForDown;
            this.ImageUrlForUp = imgUrlForUp;
            this.PagesSize = this.PagesSize > pagesSize ? this.PagesSize : pagesSize;
        }

        /// <summary>
        /// 当前页面索引
        /// </summary>
        public int CurrentPageIndex { get; private set; }

        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPages { get; private set; }

        /// <summary>
        /// 建立完整url的基本url和id值, 例 http://RequestBaseUrl/id
        /// </summary>
        public string RequestBaseUrl { get; private set; }

        /// <summary>
        /// 前一页的图片
        /// </summary>
        public string ImageUrlForUp { get; private set; }

        /// <summary>
        /// 下一页的图片
        /// </summary>
        public string ImageUrlForDown { get; private set; }

        /// <summary>
        /// 需要显示的页面最大数字
        /// </summary>
        public int PagesSize { get; private set; }
    }


    /// <summary>
    ///自定义分页工程
    /// </summary>
    public static class CustomizePager
    {
        /// <summary>
        /// 简单地为分页输入'Up' 或 'Down' 
        /// </summary>
        /// <param name="currentPageIndex">当前页面索引</param>
        /// <param name="totalPages">总页数</param>
        /// <param name="requestBaseUrl">建立完整url的基本url和id值, 例:http://baseURL/id</param>   
        public static string CreatePager(int currentPageIndex, int totalPages, string requestBaseUrl)
        {
            Validate(currentPageIndex, totalPages);
            StringBuilder sb = new StringBuilder();
            if (currentPageIndex > 0)
            {
                sb.Append(string.Format("<a href=\"{0}\">Up</a>", requestBaseUrl + "/" + (currentPageIndex - 1).ToString()));

            }
            if (currentPageIndex != totalPages - 1)
            {
                sb.Append(string.Format("<a href=\"{0}\">Down</a>", requestBaseUrl + "/" + (currentPageIndex + 1).ToString()));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 使用已传递分页图片
        /// </summary>
        /// <param name="currentPageIndex">当前页面序号</param>
        /// <param name="totalPages">总页数</param>
        /// <param name="requestBaseUrl">建立完整url的基本url和id值, 例:http://baseURL/id</param>   
        /// <param name="imageUrlForUp">前一页的图片</param>
        /// <param name="imageUrlForDown">下一页的图片</param>
        /// <returns></returns>
        public static string CreatePager(int currentPageIndex, int totalPages, string requestBaseUrl, string imageUrlForUp, string imageUrlForDown)
        {

            Validate(currentPageIndex, totalPages);
            StringBuilder sb = new StringBuilder();
            if (currentPageIndex > 0)
            {
                sb.Append(string.Format("<a href=\"{0}\"><img src={1} style=\"boder:0px;border-color: #FFFFFF\"/></a>", requestBaseUrl + "/" + (currentPageIndex - 1).ToString(), imageUrlForUp));

            }

            if (currentPageIndex != totalPages - 1)
            {
                sb.Append(string.Format("<a href=\"{0}\"><img src={1} style=\"boder:0px;border-color: #FFFFFF\"/></a>", requestBaseUrl + "/" + (currentPageIndex + 1).ToString(), imageUrlForDown));

            }

            return sb.ToString();
        }

        /// <summary>
        /// 使用阿拉伯数字和分页图片
        /// </summary>
        /// <param name="currentPageIndex">当前页面序号</param>
        /// <param name="totalPages">总页数</param>
        /// <param name="requestBaseUrl">建立完整url的基本url和id值, 例:http://baseURL/id</param>   
        /// <param name="pagesSize">显示页面数字的尺寸</param>
        /// <param name="imageUrlForUp">前一页的图片</param>
        /// <param name="imageUrlForDown">下一页的图片</param>
        /// <returns></returns>
        public static string CreatePager(int currentPageIndex, int totalPages, string requestBaseUrl, int pagesSize, string imageUrlForUp, string imageUrlForDown)
        {
            StringBuilder sb = new StringBuilder();
            Validate(currentPageIndex, totalPages, pagesSize);
            string href = "javascript:void(0)";

            // 决定是否有前一页
            if (currentPageIndex / pagesSize != 0)
            {
                href = string.Format(requestBaseUrl + "/" + ((currentPageIndex / pagesSize) * pagesSize - 1).ToString());
            }

            sb.Append(string.Format("<a href=\"{0}\"><img src=\"{1}\" style=\"boder:0px;border-color: #FFFFFF\"/></a>", href, imageUrlForUp));
            sb.Append("&nbsp;&nbsp;");

            // 需要显示的页面最大数字
            int pageIndexMaxIncrement = (currentPageIndex / pagesSize + 1) * pagesSize > totalPages ? totalPages - pagesSize * (currentPageIndex / pagesSize) : pagesSize;

            // 初始化页面序号
            int pageIndex = pagesSize * (int)(currentPageIndex / pagesSize);
            for (int i = 0; i < pageIndexMaxIncrement; i++)
            {
                if (pageIndex == currentPageIndex)
                {
                    sb.Append(string.Format("<a href='{0}' style='{2}'>{1}</a>", "javascript:void(0)", (pageIndex + 1).ToString(), "text-decoration:none;"));

                }
                else
                {
                    sb.Append(string.Format("<a href='{0}'>{1}</a>", requestBaseUrl + "/" + pageIndex.ToString(), (pageIndex + 1).ToString()));

                }
                sb.Append("&nbsp;&nbsp;");
                pageIndex++;
            }
            href = "javascript:void(0)";

            // 决定是否有下一页
            if (pageIndex < totalPages - 1)
            {
                href = string.Format(requestBaseUrl + "/" + pageIndex.ToString());
            }
            sb.Append(string.Format("<a href=\"{0}\"><img src=\"{1}\" style=\"boder:0px;border-color: #FFFFFF\"/></a>", href, imageUrlForDown));

            return sb.ToString();
        }

        /// <summary>
        /// 自定义的html辅助方法
        /// </summary>
        /// <param name="htmlHelper">HtmlHelper</param>
        /// <param name="pager">Pager</param>
        /// <returns></returns>
        public static string CreatePager<T>(this HtmlHelper htmlHelper, Pager<T> pager)
        {
            if (string.IsNullOrEmpty(pager.ImageUrlForDown.ToString()) || string.IsNullOrEmpty(pager.ImageUrlForUp.ToString()))
            {
                return CreatePager(pager.CurrentPageIndex, pager.TotalPages, pager.RequestBaseUrl);
            }
            else if (pager.PagesSize <= 1)
            {
                return CreatePager(pager.CurrentPageIndex, pager.TotalPages, pager.RequestBaseUrl, pager.ImageUrlForUp.ToString(), pager.ImageUrlForDown.ToString());
            }
            else
            {
                return CreatePager(pager.CurrentPageIndex, pager.TotalPages, pager.RequestBaseUrl, pager.PagesSize, pager.ImageUrlForUp.ToString(), pager.ImageUrlForDown.ToString());
            }
        }

        private static void Validate(int currentPageIndex, int totalPages)
        {
            if (currentPageIndex < 0)
            {
                throw new Exception("当前页面索引不能小于零");
            }
            if (totalPages < 1)
            {
                throw new Exception("总页面数不能少于");
            }
            if (currentPageIndex > totalPages - 1)
            {
                throw new Exception("当前页面索引不能大于总页面数");
            }
        }

        private static void Validate(int currentPageIndex, int totalPages, int pagesSize)
        {
            Validate(currentPageIndex, totalPages);
            if (pagesSize < 0)
            {
                throw new Exception("页面大小必须大于零");
            }
        }
    }
}
