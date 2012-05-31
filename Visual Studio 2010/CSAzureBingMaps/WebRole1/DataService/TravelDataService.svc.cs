/********************************* 模块头 **********************************\
* 模块名:  TravelDataService.svc.cs
* 项目名:  AzureBingMaps
* 版权 (c) Microsoft Corporation.
* 
* Travel数据服务处理的代码.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System.Data.Services;
using System.Data.Services.Common;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using Microsoft.SqlServer.Types;

namespace AzureBingMaps.WebRole.DataService
{
    // 建议在产品部署时将IncludeExceptionDetailInFaults设为否
    // 但是在调试程序部署时设为真.
    [ServiceBehavior(IncludeExceptionDetailInFaults = true, AddressFilterMode = AddressFilterMode.Any)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class TravelDataService : DataService<TravelDataServiceContext>
    {
        public static void InitializeService(DataServiceConfiguration config)
        {
            config.SetEntitySetAccessRule("*", EntitySetRights.All);
            config.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
            config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V2;
        }

        /// <summary>
        /// 暴露WCF数据服务自定义操作.
        /// 本示例没有使用下列代码.
        /// </summary>
        [WebGet]
        public double DistanceBetweenPlaces(double latitude1, double latitude2, double longitude1, double longitude2)
        {
            SqlGeography geography1 = SqlGeography.Point(latitude1, longitude1, 4326);
            SqlGeography geography2 = SqlGeography.Point(latitude2, longitude2, 4326);
            return geography1.STDistance(geography2).Value;
        }
    }
}
