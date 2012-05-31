'/********************************* 模块头 **********************************\
'* 模块名:  TravelDataServiceContext.vb
'* 项目名:  AzureBingMaps
'* 版权 (c) Microsoft Corporation.
'* 
'* 实现WCF的数据服务反射提供器.
'* 
'* This source is subject to the Microsoft Public License.
'* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
'* All other rights reserved.
'* 
'* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
'* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
'* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'\***************************************************************************/

Imports System.Data.Objects
Imports System.Data.Services
Imports System.Linq
Imports System.Web
Imports AzureBingMaps.DAL

Namespace AzureBingMaps.WebRole.DataService
    ''' <summary>
    ''' 实现WCF的数据服务反射提供器.
    ''' </summary>
    Public Class TravelDataServiceContext
        Implements IUpdatable
        Private _entityFrameworkContext As TravelModelContainer

        Public Sub New()
            ' 获得分区连接字符串.
            ' PartitionKey代表当前用户.
            Me._entityFrameworkContext = New TravelModelContainer(Me.GetConnectionString(Me.SetPartitionKey()))
        End Sub

        ''' <summary>
        ''' 标准数据服务查询.
        ''' </summary>
        Public ReadOnly Property Travels() As IQueryable(Of Travel)
            Get
                ' 只查询指定用户数据.
                Dim partitionKey As String = Me.SetPartitionKey()
                Return Me._entityFrameworkContext.Travels.Where(Function(e) e.PartitionKey = partitionKey)
            End Get
        End Property

        Public Sub AddReferenceToCollection( _
                                           ByVal targetResource As Object, _
                                           ByVal propertyName As String, _
                                           ByVal resourceToBeAdded As Object _
                                           ) Implements IUpdatable.AddReferenceToCollection
            Throw New NotImplementedException()
        End Sub

        Public Sub ClearChanges() Implements IUpdatable.ClearChanges
            Throw New NotImplementedException()
        End Sub

        ''' <summary>
        ''' 创建一个新集合. 这个步骤不会设定属性.
        ''' </summary>
        Public Function CreateResource( _
                                      ByVal containerName As String, _
                                      ByVal fullTypeName As String _
                                      ) As Object Implements IUpdatable.CreateResource
            Try
                Dim t As Type = Type.[GetType](fullTypeName & ", AzureBingMaps.DAL", True)
                Dim resource As Object = Activator.CreateInstance(t)
                If TypeOf resource Is Travel Then
                    Me._entityFrameworkContext.Travels.AddObject(DirectCast(resource, Travel))
                End If
                Return resource
            Catch ex As Exception
                Throw New InvalidOperationException(
                    "创建资源失败. 详细信息请参看内部异常.", ex)
            End Try
        End Function

        ''' <summary>
        ''' 删除一个集合.
        ''' </summary>
        Public Sub DeleteResource(ByVal targetResource As Object) Implements IUpdatable.DeleteResource
            If TypeOf targetResource Is Travel Then
                Me._entityFrameworkContext.Travels.DeleteObject(DirectCast(targetResource, Travel))
            End If
        End Sub

        ''' <summary>
        ''' 获取单个集合. 被用于更新和删除.
        ''' </summary>
        Public Function GetResource( _
                                   ByVal query As IQueryable, _
                                   ByVal fullTypeName As String _
                                   ) As Object Implements IUpdatable.GetResource
            Dim q As ObjectQuery(Of Travel) = TryCast(query, ObjectQuery(Of Travel))
            Dim enumerator = query.GetEnumerator()
            If Not enumerator.MoveNext() Then
                Throw New ApplicationException("无法定位资源.")
            End If
            If enumerator.Current Is Nothing Then
                Throw New ApplicationException("无法定位资源.")
            End If
            Return enumerator.Current
        End Function

        Public Function GetValue( _
                                ByVal targetResource As Object, _
                                ByVal propertyName As String _
                                ) As Object Implements IUpdatable.GetValue
            Throw New NotImplementedException()
        End Function

        Public Sub RemoveReferenceFromCollection( _
                                                ByVal targetResource As Object, _
                                                ByVal propertyName As String, _
                                                ByVal resourceToBeRemoved As Object _
                                                ) Implements IUpdatable.RemoveReferenceFromCollection
            Throw New NotImplementedException()
        End Sub

        ''' <summary>
        ''' 更新集合.
        ''' </summary> 
        Public Function ResetResource( _
                                     ByVal resource As Object _
                                     ) As Object Implements IUpdatable.ResetResource
            If TypeOf resource Is Travel Then
                Dim updated As Travel = DirectCast(resource, Travel)
                Dim original = Me._entityFrameworkContext.Travels.Where( _
                    Function(t) t.PartitionKey = updated.PartitionKey AndAlso t.RowKey = updated.RowKey).FirstOrDefault()
                original.GeoLocationText = updated.GeoLocationText
                original.Place = updated.Place
                original.Time = updated.Time
            End If
            Return resource
        End Function

        Public Function ResolveResource( _
                                       ByVal resource As Object _
                                       ) As Object Implements IUpdatable.ResolveResource
            Return resource
        End Function

        Public Sub SaveChanges() Implements IUpdatable.SaveChanges
            Me._entityFrameworkContext.SaveChanges()
        End Sub

        Public Sub SetReference( _
                               ByVal targetResource As Object, _
                               ByVal propertyName As String, _
                               ByVal propertyValue As Object _
                               ) Implements IUpdatable.SetReference
            Throw New NotImplementedException()
        End Sub

        ''' <summary>
        ''' 设定属性值.
        ''' </summary>
        Public Sub SetValue( _
                           ByVal targetResource As Object, _
                           ByVal propertyName As String, _
                           ByVal propertyValue As Object _
                           ) Implements IUpdatable.SetValue
            Try

                Dim [property] = targetResource.[GetType]().GetProperty(propertyName)
                If [property] Is Nothing Then
                    Throw New InvalidOperationException("无效属性: " & propertyName)
                End If

                ' PartitionKey表示用户身份,
                ' 必须在服务器端获得e,
                ' 否则客户端可能发送虚假身份..
                If [property].Name = "PartitionKey" Then
                    Dim partitionKey As String = Me.SetPartitionKey()
                    [property].SetValue(targetResource, partitionKey, Nothing)
                Else
                    [property].SetValue(targetResource, propertyValue, Nothing)
                End If
            Catch ex As Exception
                Throw New InvalidOperationException("设定值失败. 详细信息请参看内部异常.", ex)
            End Try
        End Sub

        ''' <summary>
        ''' 如果用户未登入, 使用默认分区.
        ''' 否则分区键就是用户的email地址.
        ''' </summary>
        Private Function SetPartitionKey() As String
            Dim partitionKey As String = "defaultuser@live.com"
            Dim user As String = TryCast(HttpContext.Current.Session("User"), String)
            If user IsNot Nothing Then
                partitionKey = user
            End If
            Return partitionKey
        End Function

        ''' <summary>
        ''' 获得分区链接字符串.
        ''' 当前, 所有分区都保存在同一数据库中.
        ''' 但是随着数据和用户的增加,
        ''' 为了更好的性能我们可以把分区移动到其他数据库.
        ''' 将来, 我们也可以使用SQL Azure federation的优异特性.
        ''' </summary>
        Private Function GetConnectionString(ByVal partitionKey As String) As String
            Return "name=TravelModelContainer"
        End Function
    End Class
End Namespace