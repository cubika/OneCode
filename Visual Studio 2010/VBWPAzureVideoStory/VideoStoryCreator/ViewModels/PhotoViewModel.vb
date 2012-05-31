'********************************* 模块头 *********************************\
' 模块名: PhotoViewModel.vb
' 项目名: VideoStoryCreator
' 版权(c) Microsoft Corporation.
' 
' 给予通信类的基类.
' 几种ViewMode关联到phone继承的类.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************/



Imports System.ComponentModel
Imports System.Windows.Media.Imaging
Imports System.IO

Public Class PhotoViewModel
    Implements INotifyPropertyChanged
    Private _name As String
    Public Property Name() As String
        Get
            Return Me._name
        End Get
        Set(value As String)
            If Me._name <> value Then
                Me._name = value
                Me.NotifyPropertyChange("Name")
            End If
        End Set
    End Property

    ' MediaStream不必支持提示变更事件.
    Public Property MediaStream() As Stream
        Get
            Return m_MediaStream
        End Get
        Set(value As Stream)
            m_MediaStream = value
        End Set
    End Property
    Private m_MediaStream As Stream

    Private _transitionDuration As Integer = 2
    Public Property TransitionDuration() As Integer
        Get
            Return Me._transitionDuration
        End Get
        Set(value As Integer)
            If Me._transitionDuration <> value Then
                Me._transitionDuration = value
                Me.NotifyPropertyChange("TransitionDuration")
            End If
        End Set
    End Property

    Private _photoDuration As Integer = 5
    Public Property PhotoDuration() As Integer
        Get
            Return Me._photoDuration
        End Get
        Set(value As Integer)
            If Me._photoDuration <> value Then
                Me._photoDuration = value
                Me.NotifyPropertyChange("PhotoDuration")
            End If
        End Set
    End Property

    Private _imageSource As BitmapImage
    Public ReadOnly Property ImageSource() As BitmapImage
        Get
            If Me.MediaStream Is Nothing Then
                Return Nothing
            End If

            If Me._imageSource Is Nothing Then
                Me._imageSource = New BitmapImage()
                Me._imageSource.SetSource(Me.MediaStream)
            End If

            Return Me._imageSource
        End Get
    End Property

    ''' <summary>
    ''' 使用相片的名称进行比较
    ''' </summary>
    Public Overrides Function Equals(obj As Object) As Boolean
        If TypeOf obj Is PhotoViewModel Then
            Return Me.Name.Equals(DirectCast(obj, PhotoViewModel).Name)
        End If
        Return False
    End Function

    Public Overrides Function GetHashCode() As Integer
        Return Me.Name.GetHashCode()
    End Function

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Protected Sub NotifyPropertyChange(propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub
End Class
