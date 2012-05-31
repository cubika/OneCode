'****************************** 模块头 ******************************\
' 模块名:	Story.vb
' 项目名:	StoryDataModel
' 版权 (c) Microsoft Corporation.
' 
' 表达短影的模型类.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************/



Imports Microsoft.WindowsAzure.StorageClient

Public Class Story
    Inherits TableServiceEntity
    Public Sub New()
    End Sub

    Public Sub New(id As String, name As String, videoUri As String)
        Me.PartitionKey = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString()
        Me.RowKey = id
        Me.Name = name
        Me.VideoUri = videoUri
    End Sub

    Public Property Name() As String
        Get
            Return m_Name
        End Get
        Set(value As String)
            m_Name = Value
        End Set
    End Property
    Private m_Name As String
    Public Property VideoUri() As String
        Get
            Return m_VideoUri
        End Get
        Set(value As String)
            m_VideoUri = Value
        End Set
    End Property
    Private m_VideoUri As String
End Class
