'********************************** 模块头********************************\   
' 模块名： WorkItemLinkQuery.vb
' 项目名： VBTFSWorkItemLinkInfoDetails
' 版权 (c) Microsoft Corporation
' 
' WorkItemLinkInfoentry类的详细内容。
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*************************************************************************/ 

Imports System.Net
Imports Microsoft.TeamFoundation.Client
Imports Microsoft.TeamFoundation.WorkItemTracking.Client

Public Class WorkItemLinkQuery
    Implements IDisposable

    ' 查询语句格式
    Private Const _queryFormat As String =
        "select * from WorkItemLinks where [Source].[System.ID] = {0}"

    Private _disposed As Boolean = False

    Private _linkTypes As Dictionary(Of Integer, WorkItemLinkType)

    ' 存储ID和WorkItemLinkType KeyValuePair的字典
    Public ReadOnly Property LinkTypes() As Dictionary(Of Integer, WorkItemLinkType)
        Get
            ' 从WorkItemStore中获取所有的WorkItemLinkType
            If _linkTypes Is Nothing Then
                _linkTypes = New Dictionary(Of Integer, WorkItemLinkType)()
                For Each type In Me.WorkItemStore.WorkItemLinkTypes
                    _linkTypes.Add(type.ForwardEnd.Id, type)
                Next type
            End If
            Return _linkTypes
        End Get
    End Property

    ''' <summary>
    ''' TFS Team Project Collection。
    ''' </summary>
    Private _projectCollection As TfsTeamProjectCollection
    Public Property ProjectCollection() As TfsTeamProjectCollection
        Get
            Return _projectCollection
        End Get
        Private Set(ByVal value As TfsTeamProjectCollection)
            _projectCollection = value
        End Set
    End Property

    ''' <summary>
    ''' Team Project Collection的WorkItemStore.                                   
    ''' </summary>
    Private _workItemStore As WorkItemStore
    Public Property WorkItemStore() As WorkItemStore
        Get
            Return _workItemStore
        End Get
        Private Set(ByVal value As WorkItemStore)
            _workItemStore = value
        End Set
    End Property

    ''' <summary>
    ''' 使用默认的credentials初始化实例。
    ''' </summary>
    Public Sub New(ByVal collectionUri As Uri)
        Me.New(collectionUri, CredentialCache.DefaultCredentials)
    End Sub

    ''' <summary>
    ''' 初始化实例。
    ''' </summary>
    Public Sub New(ByVal collectionUri As Uri, ByVal credential As ICredentials)
        If collectionUri Is Nothing Then
            Throw New ArgumentNullException("collectionUrl")
        End If

        ' //如果凭证失效，将启动一个UICredentialsProvider实例。
        Me.ProjectCollection = New TfsTeamProjectCollection(
            collectionUri, credential, New UICredentialsProvider())
        Me.ProjectCollection.EnsureAuthenticated()

        ' 获取WorkItemStore服务。
        Me.WorkItemStore = Me.ProjectCollection.GetService(Of WorkItemStore)()
    End Sub

    ''' <summary>
    ''' 从一个work项中获取WorkItemLinkInfoDetails。
    ''' </summary>
    Public Function GetWorkItemLinkInfos(ByVal workitemID As Integer) _
        As IEnumerable(Of WorkItemLinkInfoDetails)

        ' 构造WIQL。
        Dim queryStr As String = String.Format(_queryFormat, workitemID)

        Dim linkQuery As New Query(Me.WorkItemStore, queryStr)

        ' 获取所有的WorkItemLinkInfo对象。
        Dim linkinfos() As WorkItemLinkInfo = linkQuery.RunLinkQuery()

        ' 定义一个WorkItemLinkInfoDetails类型的泛型变量。从WorkItemLinkInfo对象中获取WorkItemLinkInfoDetails。
        Dim detailsList As New List(Of WorkItemLinkInfoDetails)()
        For Each linkinfo In linkinfos
            If linkinfo.LinkTypeId <> 0 Then
                Dim details As WorkItemLinkInfoDetails =
                    GetDetailsFromWorkItemLinkInfo(linkinfo)
                Console.WriteLine(details.ToString())
            End If
        Next linkinfo
        Return detailsList
    End Function

    ''' <summary>
    ''' 从WorkItemLinkInfo对象中获取WorkItemLinkInfoDetails。
    ''' </summary>
    Public Function GetDetailsFromWorkItemLinkInfo(ByVal linkInfo As WorkItemLinkInfo) _
        As WorkItemLinkInfoDetails

        If Me.LinkTypes.ContainsKey(linkInfo.LinkTypeId) Then
            Dim details As New WorkItemLinkInfoDetails(
                linkInfo,
                Me.WorkItemStore.GetWorkItem(linkInfo.SourceId),
                Me.WorkItemStore.GetWorkItem(linkInfo.TargetId),
                Me.LinkTypes(linkInfo.LinkTypeId))
            Return details
        Else
            Throw New ApplicationException("Cannot find WorkItemLinkType!")
        End If
    End Function

    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub

    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        ' 避免被处理多次。
        If _disposed Then
            Return
        End If

        If disposing Then
            If Me.ProjectCollection IsNot Nothing Then
                Me.ProjectCollection.Dispose()
            End If
            _disposed = True
        End If
    End Sub

End Class
