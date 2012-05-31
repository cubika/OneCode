'****************************** 模块头 *************************************\
' Module Name:	ServiceRegistrySettingsElement.vb
' Project:		VBAZWorkflow4ServiceBus
' Copyright (c) Microsoft Corporation.
' 
' 该类允许你在web/app.config中配置ServiceRegistrySettings.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************/

Imports System.ServiceModel.Configuration
Imports Microsoft.ServiceBus

''' <summary>
''' 该类允许你在web/app.config中配置ServiceRegistrySettings.
''' </summary>
Public Class ServiceRegistrySettingsElement
	Inherits BehaviorExtensionElement

	Public Overrides ReadOnly Property BehaviorType As Type
		Get
			Return GetType(ServiceRegistrySettings)
		End Get
	End Property

	Protected Overrides Function CreateBehavior() As Object
		Return New ServiceRegistrySettings() With {.DiscoveryMode = Me.DiscoveryMode, .DisplayName = Me.DisplayName}
	End Function

	<ConfigurationProperty("discoveryMode", DefaultValue:=DiscoveryType.Private)>
	Public Property DiscoveryMode As DiscoveryType
		Get
			Return Me("discoveryMode")
		End Get
		Set(ByVal value As DiscoveryType)
			Me("discoveryMode") = value
		End Set
	End Property

	<ConfigurationProperty("displayName")>
	Public Property DisplayName As String
		Get
			Return Me("displayName")
		End Get
		Set(ByVal value As String)
			Me("displayName") = value
		End Set
	End Property
End Class
