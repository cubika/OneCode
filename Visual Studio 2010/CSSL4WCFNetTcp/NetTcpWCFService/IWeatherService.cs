/****************************** 模块头 ******************************\
* 模块名:                IWeatherService.cs
* 项目名:                NetTcpWCFService
* 版权 (c) Microsoft Corporation.
* 
* Weather Service 服务协议。
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
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace NetTcpWCFService
{
    [ServiceContract(CallbackContract=typeof(IWeatherServiceCallback))]
    public interface IWeatherService
    {
        [OperationContract(IsOneWay = true)]
        void Subscribe();

        [OperationContract(IsOneWay = true)]
        void UnSubscribe();
    }

    public interface IWeatherServiceCallback
    {
        [OperationContract(IsOneWay=true)]
        void WeatherReport(string report);
    }
}
