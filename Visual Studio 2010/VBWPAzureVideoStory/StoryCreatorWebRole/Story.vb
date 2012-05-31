'****************************** 模块头 ******************************\
' 模块名:	Story.vb
' 项目名: StoryCreatorWebRole
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



Public Class Story
    Public Property Name() As String
        Get
            Return m_Name
        End Get
        Set(value As String)
            m_Name = value
        End Set
    End Property
    Private m_Name As String
    Public Property VideoUri() As String
        Get
            Return m_VideoUri
        End Get
        Set(value As String)
            m_VideoUri = value
        End Set
    End Property
    Private m_VideoUri As String
End Class
