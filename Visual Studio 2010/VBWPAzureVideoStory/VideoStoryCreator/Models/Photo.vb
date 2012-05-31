'********************************* 模块头 *********************************\
' 模块名: Photo.vb
' 项目名: VideoStoryCreator
' 版权(c) Microsoft Corporation.
' 
' photo实体类.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************/



Imports System.Windows.Media.Imaging
Imports System.IO

Public Class Photo
    Public Property Name() As String
        Get
            Return m_Name
        End Get
        Set(value As String)
            m_Name = value
        End Set
    End Property
    Private m_Name As String
    Public Property ThumbnailStream() As Stream
        Get
            Return m_ThumbnailStream
        End Get
        Set(value As Stream)
            m_ThumbnailStream = value
        End Set
    End Property
    Private m_ThumbnailStream As Stream
    Public Property ResizedImage() As WriteableBitmap
        Get
            Return m_ResizedImage
        End Get
        Set(value As WriteableBitmap)
            m_ResizedImage = value
        End Set
    End Property
    Private m_ResizedImage As WriteableBitmap
    Public Property ResizedImageStream() As Stream
        Get
            Return m_ResizedImageStream
        End Get
        Set(value As Stream)
            m_ResizedImageStream = value
        End Set
    End Property
    Private m_ResizedImageStream As Stream
    Public Property Transition() As ITransition
        Get
            Return m_Transition
        End Get
        Set(value As ITransition)
            m_Transition = value
        End Set
    End Property
    Private m_Transition As ITransition
    Public Property PhotoDuration() As TimeSpan
        Get
            Return m_PhotoDuration
        End Get
        Set(value As TimeSpan)
            m_PhotoDuration = value
        End Set
    End Property
    Private m_PhotoDuration As TimeSpan

    Public Overrides Function Equals(obj As Object) As Boolean
        If TypeOf obj Is Photo Then
            Return Me.Name.Equals(DirectCast(obj, Photo).Name)
        End If
        Return False
    End Function

    Public Overrides Function GetHashCode() As Integer
        Return Me.Name.GetHashCode()
    End Function
End Class
