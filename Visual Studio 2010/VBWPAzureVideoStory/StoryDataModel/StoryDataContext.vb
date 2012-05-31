'****************************** 模块头 ******************************\
' 模块名:	StoryDataContext.vb
' 项目名:		StoryDataModel
' 版权 (c) Microsoft Corporation.
' 
' 数据表存储内容类.
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
Imports Microsoft.WindowsAzure

Public Class StoryDataContext
    Inherits TableServiceContext

    Public Sub New(baseAddress As String, credentials As StorageCredentials)
        MyBase.New(baseAddress, credentials)
    End Sub

    Public ReadOnly Property Stories() As IQueryable(Of Story)
        Get
            Return Me.CreateQuery(Of Story)("Stories")
        End Get
    End Property
End Class
