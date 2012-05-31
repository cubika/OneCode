'******************************* 模块头 ************************************\
' 模块名:   RegistryKeyChangeEventArgs.vb
' 项目名:   VBMonitorRegistryChange
' 版权(c)   Microsoft Corporation.
' 
' 这个类派生自EventArgs。用于将ManagementBaseObject包装为EventArrivedEventArgs。
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**************************************************************************

Imports System.Management

Friend Class RegistryKeyChangeEventArgs
    Inherits EventArgs

    Public Property Hive() As String
    Public Property KeyPath() As String
    Public Property SECURITY_DESCRIPTOR() As UInteger()
    Public Property TIME_CREATED() As Date

    Public Sub New(ByVal arrivedEvent As ManagementBaseObject)

        ' 类RegistryKeyChangeEvent有以下属性：Hive，KeyPath，SECURITY_DESCRIPTOR
        ' 以及TIME_CREATED。这些属性可以从arrivedEvent.Properties中得到。
        Me.Hive = TryCast(arrivedEvent.Properties("Hive").Value, String)
        Me.KeyPath = TryCast(arrivedEvent.Properties("KeyPath").Value, String)

        ' TIME_CREATED属性是一个代表了事件被创建时间的独特的值。
        ' 它表示了从1601年1月1日之后到现在的，以100纳秒为间隔的一个64位FILETIME值。
        ' 这个值使用了协调世界时间(UTC)格式。
        Me.TIME_CREATED = New Date(
            CLng(Fix(CULng(arrivedEvent.Properties("TIME_CREATED").Value))),
            DateTimeKind.Utc).AddYears(1600)
    End Sub
End Class
