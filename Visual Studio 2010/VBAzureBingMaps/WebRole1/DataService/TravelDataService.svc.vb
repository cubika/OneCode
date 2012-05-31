'/********************************* 模块头 **********************************\
'* 模块名:  TravelDataService.svc.vb
'* 项目名:  AzureBingMaps
'* 版权 (c) Microsoft Corporation.
'* 
'* Travel数据服务处理的代码.
'* 
'* This source is subject to the Microsoft Public License.
'* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
'* All other rights reserved.
'* 
'* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
'* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
'* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'\***************************************************************************/

Imports System.Data.Services
Imports System.Data.Services.Common
Imports System.ServiceModel
Imports System.ServiceModel.Activation
Imports System.ServiceModel.Web
Imports Microsoft.SqlServer.Types

Namespace AzureBingMaps.WebRole.DataService
    ' 建议在产品部署时将IncludeExceptionDetailInFaults设为否
    ' 但是在调试程序部署时设为真.
    <ServiceBehavior(IncludeExceptionDetailInFaults:=True, AddressFilterMode:=AddressFilterMode.Any)> _
    <AspNetCompatibilityRequirements(RequirementsMode:=AspNetCompatibilityRequirementsMode.Allowed)> _
    Public Class TravelDataService
        Inherits DataService(Of TravelDataServiceContext)
        Public Shared Sub InitializeService(ByVal config As DataServiceConfiguration)
            config.SetEntitySetAccessRule("*", EntitySetRights.All)
            config.SetServiceOperationAccessRule("*", ServiceOperationRights.All)
            config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V2
        End Sub

        ''' <summary>
        ''' 暴露WCF数据服务自定义操作.
        ''' 本示例没有使用下列代码.
        ''' </summary>
        <WebGet()> _
        Public Function DistanceBetweenPlaces(ByVal latitude1 As Double, ByVal latitude2 As Double, ByVal longitude1 As Double, ByVal longitude2 As Double) As Double
            Dim geography1 As SqlGeography = SqlGeography.Point(latitude1, longitude1, 4326)
            Dim geography2 As SqlGeography = SqlGeography.Point(latitude2, longitude2, 4326)
            Return geography1.STDistance(geography2).Value
        End Function
    End Class
End Namespace