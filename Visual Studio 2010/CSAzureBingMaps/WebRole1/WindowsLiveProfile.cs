/********************************* 模块头 **********************************\
* 模块名:  WindowsLiveProfile.cs
* 项目名:  AzureBingMaps
* 版权 (c) Microsoft Corporation.
* 
* 关联Windows Live Messenger Connect profile API反应的类.
* 使用WCF syndication API.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AzureBingMaps.WebRole
{
    /// <summary>
    /// 关联Windows Live Messenger Connect profile API反应的类.
    /// 使用WCF syndication API.
    /// </summary>
    [DataContract(Namespace = "http://schemas.microsoft.com/ado/2007/08/dataservices")]
    public class WindowsLiveProfile
    {
        [DataMember]
        public List<element> Addresses { get; set; }
        [DataMember]
        public int BirthMonth { get; set; }
        [DataMember]
        public List<element> Emails { get; set; }
        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public string Gender { get; set; }
        [DataMember]
        public string LastName { get; set; }
        [DataMember]
        public string Location { get; set; }
        [DataMember]
        public List<element> PhoneNumbers { get; set; }
    }

    [DataContract(Namespace = "http://schemas.microsoft.com/ado/2007/08/dataservices")]
    public class element
    {
        [DataMember]
        public string Address { get; set; }
        [DataMember]
        public string Type { get; set; }
        [DataMember]
        public string City { get; set; }
        [DataMember]
        public string CountryRegion { get; set; }
    }
}