'********************************* 模块头 *********************************\
' 模块名: FadeTransition.vb
' 项目名: VideoStoryCreator
' 版权(c) Microsoft Corporation.
' 
' 实现简单的淡入淡出特效.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************/




Public Class FadeTransition
    Inherits TransitionBase
    Private _transitionStoryboard As Storyboard
    Private _fadeAnimation As DoubleAnimationUsingKeyFrames
    Private _foregroundElement As FrameworkElement

    Public Event TransitionCompleted As EventHandler

    Private _name As String = "Fade Transition"
    Public Overrides ReadOnly Property Name() As String
        Get
            Return Me._name
        End Get
    End Property

    Public Sub New()
        Me.TransitionDuration = TimeSpan.FromSeconds(2.0)
        Me._transitionStoryboard = New Storyboard()
        Me._fadeAnimation = New DoubleAnimationUsingKeyFrames()

        ' 只需动态变更 Opacity 属性.
        Storyboard.SetTargetProperty(Me._fadeAnimation, New PropertyPath("Opacity"))
        Dim frame1 As New EasingDoubleKeyFrame() With { _
          .KeyTime = KeyTime.FromTimeSpan(TimeSpan.Zero), _
          .Value = 1.0 _
        }
        Dim frame2 As New EasingDoubleKeyFrame() With { _
          .KeyTime = KeyTime.FromTimeSpan(Me.TransitionDuration), _
          .Value = 0.0 _
        }
        Me._fadeAnimation.KeyFrames.Add(frame1)
        Me._fadeAnimation.KeyFrames.Add(frame2)
        Me._transitionStoryboard.Children.Add(_fadeAnimation)
        AddHandler Me._transitionStoryboard.Completed, AddressOf TransitionStoryboard_Completed
    End Sub

    Private Sub TransitionStoryboard_Completed(sender As Object, e As EventArgs)
        RaiseEvent TransitionCompleted(Me, EventArgs.Empty)
    End Sub

    Public Overrides Sub Begin()
        Me._transitionStoryboard.Begin()
    End Sub

    Public Overrides Sub [Stop]()
        Me._transitionStoryboard.[Stop]()
    End Sub

    Private _transitionDuration As TimeSpan
    Public Overrides Property TransitionDuration() As TimeSpan
        Get
            Return Me._transitionDuration
        End Get
        Set(value As TimeSpan)
            Me._transitionDuration = value
            If Me._fadeAnimation IsNot Nothing Then
                Me._fadeAnimation.KeyFrames(1).KeyTime = KeyTime.FromTimeSpan(Me.TransitionDuration)
            End If
        End Set
    End Property

    Public Overrides Property ForegroundElement() As FrameworkElement
        Get
            Return Me._foregroundElement
        End Get
        Set(value As FrameworkElement)
            Me._foregroundElement = value
            If value IsNot Nothing Then
                Storyboard.SetTarget(Me._fadeAnimation, value)
            End If
        End Set
    End Property

    Public Overrides Function Clone() As ITransition
        Return New FadeTransition() With { _
          .BackgroundElement = Me.BackgroundElement, _
          .ForegroundElement = Me.ForegroundElement, _
          .TransitionDuration = Me.TransitionDuration _
        }
    End Function
End Class
