/********************************* 模块头 **********************************\
* 模块名:  Travel.xaml.cs
* 项目名:  AzureBingMaps
* 版权 (c) Microsoft Corporation.
* 
* Travel EF实体的分部类.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System.Data.Objects.DataClasses;
using System.Data.Services;
using System.Data.Services.Common;
using System.IO;
using Microsoft.SqlServer.Types;

namespace AzureBingMaps.DAL
{
    /// <summary>
    /// Travel EF实体的分部类.
    /// PartitionKey和RowKey都是数据服务键的部分.
    /// 例如EntityState和EntityKey的属性不应被传递到客户端.
    /// 二进制表达的GeoLocation同样不必被传达到客户端.
    /// </summary>
    [DataServiceKey(new string[] { "PartitionKey", "RowKey" })]
    [IgnoreProperties(new string[] { "EntityState", "EntityKey", "GeoLocation" })]
    public partial class Travel : EntityObject
    {
        private string _geoLocationText;

        /// <summary>
        /// 地理位置的文字表达, 对用户更友好.
        /// 当Latitude和Longitude被修改时, GeoLocationText也会被修改.
        /// 客户端可以上传一个包含Latitude/Longitude的集合, 但是不包括GeoLocationText, 因此其值可能为null.
        /// 为避免无意识的将GeoLocaionText设为null, 我们将在设定器中检查其值.
        /// </summary>
        public string GeoLocationText
        {
            get { return this._geoLocationText; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    this._geoLocationText = value;
                }
            }
        }

        // 无论经度还是纬度变化时, GeoLocationText也一定会改变.
        // 二进制GeoLocation不需要被修改, 因为他只能被数据库识别.
        private double _latitude;
        public double Latitude
        {
            get { return this._latitude; }
            set
            {
                this._latitude = value;
                this.GeoLocationText = this.LatLongToWKT(this.Latitude, this.Longitude);
            }
        }

        private double _longitude;
        public double Longitude
        {
            get { return this._longitude; }
            set
            {
                this._longitude = value;
                this.GeoLocationText = this.LatLongToWKT(this.Latitude, this.Longitude);
            }
        }

        /// <summary>
        /// 转换经度和纬度为WKT.
        /// </summary>
        private string LatLongToWKT(double latitude, double longitude)
        {
            SqlGeography sqlGeography = SqlGeography.Point(latitude, longitude, 4326);            
            return sqlGeography.ToString();
        }

        /// <summary>
        /// GeoLocationText, Latitude, Longitude并不关联到数据库中的列.
        /// Geolocation (二进制)关联到TravelView表中的GeoLocation列.
        /// 如果GeoLocation的二进制值改变了, 相应那些值也会改变.
        /// 这可能在查询集合时发生.
        /// </summary>
        partial void OnGeoLocationChanging(global::System.Byte[] value)
        {
            if (value != null)
            {
                using (MemoryStream ms = new MemoryStream(value))
                {
                    using (BinaryReader reader = new BinaryReader(ms))
                    {
                        SqlGeography sqlGeography = new SqlGeography();
                        sqlGeography.Read(reader);
                        this.GeoLocationText = new string(sqlGeography.STAsText().Value);
                        this.Latitude = sqlGeography.Lat.Value;
                        this.Longitude = sqlGeography.Long.Value;
                    }
                }
            }
        }
    }
}
