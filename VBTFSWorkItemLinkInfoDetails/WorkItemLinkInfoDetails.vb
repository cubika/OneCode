'*********************************** 模块头 *******************************\
' 模块名：  WorkItemLinkInfoDetails.vb
' 项目名：  VBTFSWorkItemLinkInfoDetails
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
'**************************************************************************/

Imports System.Linq
Imports System.Text
Imports Microsoft.TeamFoundation.WorkItemTracking.Client

Public Class WorkItemLinkInfoDetails

    Private privateLinkInfo As WorkItemLinkInfo
    Public Property LinkInfo() As WorkItemLinkInfo
        Get
            Return privateLinkInfo
        End Get
        Private Set(ByVal value As WorkItemLinkInfo)
            privateLinkInfo = value
        End Set
    End Property

    Private privateSourceWorkItem As WorkItem
    Public Property SourceWorkItem() As WorkItem
        Get
            Return privateSourceWorkItem
        End Get
        Private Set(ByVal value As WorkItem)
            privateSourceWorkItem = value
        End Set
    End Property

    Private privateTargetWorkItem As WorkItem
    Public Property TargetWorkItem() As WorkItem
        Get
            Return privateTargetWorkItem
        End Get
        Private Set(ByVal value As WorkItem)
            privateTargetWorkItem = value
        End Set
    End Property

    Private privateLinkType As WorkItemLinkType
    Public Property LinkType() As WorkItemLinkType
        Get
            Return privateLinkType
        End Get
        Private Set(ByVal value As WorkItemLinkType)
            privateLinkType = value
        End Set
    End Property

    Public Sub New(ByVal linkInfo As WorkItemLinkInfo, ByVal sourceWorkItem As WorkItem, ByVal targetWorkItem As WorkItem, ByVal linkType As WorkItemLinkType)
        Me.LinkInfo = linkInfo
        Me.SourceWorkItem = sourceWorkItem
        Me.TargetWorkItem = targetWorkItem
        Me.LinkType = linkType
    End Sub

    ''' <summary>
    ''' 显示的链接格式如下：
    ''' 源文件：[源文件名称] ==> 链接类型：[链接的类型] ==> 目标文件：[目标文件名称]
    ''' </summary>
    ''' <returns></returns>
    Public Overrides Function ToString() As String
        Return String.Format("Source:{0} ==> LinkType:{1} ==> Target:{2}", SourceWorkItem.Title, LinkType.ForwardEnd.Name, TargetWorkItem.Title)
    End Function
End Class

