'/********************************* 模块头 **********************************\
'* 模块名:  Travel.xaml.vb
'* 项目名:  AzureBingMaps
'* 版权 (c) Microsoft Corporation.
'* 
'* Travel EF实体的分部类.
'* 
'* This source is subject to the Microsoft Public License.
'* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
'* All other rights reserved.
'* 
'* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
'* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
'* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'\***************************************************************************/

Imports System.Data.Objects.DataClasses
Imports System.Data.Services
Imports System.Data.Services.Common
Imports System.IO
Imports Microsoft.SqlServer.Types

''' <summary>
''' Travel EF实体的分部类.
''' PartitionKey和RowKey都是数据服务键的部分.
''' 例如EntityState和EntityKey的属性不应被传递到客户端.
''' 二进制表达的GeoLocation同样不必被传达到客户端.
''' </summary>
<DataServiceKey(New String() {"PartitionKey", "RowKey"})> _
<IgnoreProperties(New String() {"EntityState", "EntityKey", "GeoLocation"})> _
Partial Public Class Travel
    Inherits EntityObject
    Private _geoLocationText As String

    ''' <summary>
    ''' 地理位置的文字表达, 对用户更友好.
    ''' 当Latitude和Longitude被修改时, GeoLocationText也会被修改.
    ''' 客户端可以上传一个包含Latitude/Longitude的集合, 但是不包括GeoLocationText, 因此其值可能为null.
    ''' 为避免无意识的将GeoLocaionText设为null, 我们将在设定器中检查其值.
    ''' </summary>
    Public Property GeoLocationText() As String
        Get
            Return Me._geoLocationText
        End Get
        Set(ByVal value As String)
            If Not String.IsNullOrEmpty(value) Then
                Me._geoLocationText = value
            End If
        End Set
    End Property

    ' 无论经度还是纬度变化时, GeoLocationText也一定会改变.
    ' 二进制GeoLocation不需要被修改, 因为他只能被数据库识别.
    Private _latitude As Double
    Public Property Latitude() As Double
        Get
            Return Me._latitude
        End Get
        Set(ByVal value As Double)
            Me._latitude = value
            Me.GeoLocationText = Me.LatLongToWKT(Me.Latitude, Me.Longitude)
        End Set
    End Property

    Private _longitude As Double
    Public Property Longitude() As Double
        Get
            Return Me._longitude
        End Get
        Set(ByVal value As Double)
            Me._longitude = value
            Me.GeoLocationText = Me.LatLongToWKT(Me.Latitude, Me.Longitude)
        End Set
    End Property

    ''' <summary>
    ''' 转换经度和纬度为WKT.
    ''' </summary>
    Private Function LatLongToWKT(ByVal latitude As Double, ByVal longitude As Double) As String
        Dim sqlGeography__1 As SqlGeography = SqlGeography.Point(latitude, longitude, 4326)
        Return sqlGeography__1.ToString()
    End Function

    ''' <summary>
    ''' GeoLocationText, Latitude, Longitude并不关联到数据库中的列.
    ''' Geolocation (二进制)关联到TravelView表中的GeoLocation列.
    ''' 如果GeoLocation的二进制值改变了, 相应那些值也会改变.
    ''' 这可能在查询集合时发生.
    ''' </summary>
    Private Sub OnGeoLocationChanging(ByVal value As Global.System.Byte())
        If value IsNot Nothing Then
            Using ms As New MemoryStream(value)
                Using reader As New BinaryReader(ms)
                    Dim sqlGeography As New SqlGeography()
                    sqlGeography.Read(reader)
                    Me.GeoLocationText = New String(sqlGeography.STAsText().Value)
                    Me.Latitude = sqlGeography.Lat.Value
                    Me.Longitude = sqlGeography.[Long].Value
                End Using
            End Using
        End If
    End Sub
End Class