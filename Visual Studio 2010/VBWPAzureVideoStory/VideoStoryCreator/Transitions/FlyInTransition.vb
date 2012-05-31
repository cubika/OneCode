'********************************* 模块头 *********************************\
' 模块名: FlyInTransition_Design.vb
' 项目名: VideoStoryCreator
' 版权(c) Microsoft Corporation.
' 
' 提供用来指定飞入特效的额外设计界面.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************/



Imports System.Xml.Linq

Public Class FlyInTransition
    Inherits TransitionBase
    Private _transitionStoryboard As Storyboard
    Private _flyAnimation As DoubleAnimationUsingKeyFrames
    Private _direction As FlyDirection
    Private _backgroundElement As FrameworkElement
    Private _foregroundElement As FrameworkElement
    Public Event TransitionCompleted As EventHandler

    Public Sub New()
        Me.TransitionDuration = TimeSpan.FromSeconds(2.0)
        Me._transitionStoryboard = New Storyboard()
        Me._flyAnimation = New DoubleAnimationUsingKeyFrames()
        Me._transitionStoryboard.Children.Add(_flyAnimation)
        Me.Direction = FlyDirection.Left
        AddHandler Me._transitionStoryboard.Completed, AddressOf TransitionStoryboard_Completed
    End Sub

    Public Overrides ReadOnly Property Name() As String
        Get
            Return "Fly In Transition"
        End Get
    End Property

    Public Property Direction() As FlyDirection
        Get
            Return Me._direction
        End Get
        Set(value As FlyDirection)
            Me._direction = value

            ' Reset the key frames.
            Me._flyAnimation.KeyFrames.Clear()
            Dim frame1 As EasingDoubleKeyFrame = Nothing
            Dim frame2 As EasingDoubleKeyFrame = Nothing

            ' If BackgroundElement is null, we'll add the key frames later.
            If Me.BackgroundElement IsNot Nothing Then
                frame1 = New EasingDoubleKeyFrame() With { _
                  .KeyTime = KeyTime.FromTimeSpan(TimeSpan.Zero) _
                }
                frame2 = New EasingDoubleKeyFrame() With { _
                  .KeyTime = KeyTime.FromTimeSpan(Me.TransitionDuration) _
                }
            End If

            ' Configure the property to animate depending on the fly direction.
            If value = FlyDirection.Left OrElse value = FlyDirection.Right Then
                Storyboard.SetTargetProperty(Me._flyAnimation, New PropertyPath("RenderTransform.(TranslateTransform.X)"))
                If Me.BackgroundElement IsNot Nothing Then
                    frame1.Value = If((value = FlyDirection.Left), -Me.BackgroundElement.ActualWidth, Me.BackgroundElement.ActualWidth)
                    frame2.Value = 0
                End If
            Else
                Storyboard.SetTargetProperty(Me._flyAnimation, New PropertyPath("RenderTransform.(TranslateTransform.Y)"))
                If Me.BackgroundElement IsNot Nothing Then
                    frame1.Value = If((value = FlyDirection.Up), Me.BackgroundElement.ActualHeight, -Me.BackgroundElement.ActualHeight)
                    frame2.Value = 0
                End If
            End If

            If Me.BackgroundElement IsNot Nothing Then
                Me._flyAnimation.KeyFrames.Add(frame1)
                Me._flyAnimation.KeyFrames.Add(frame2)
            End If
        End Set
    End Property

    Public Overrides Property BackgroundElement() As FrameworkElement
        Get
            Return Me._backgroundElement
        End Get
        Set(value As FrameworkElement)
            If value IsNot Nothing Then
                Me._backgroundElement = value
                Me._backgroundElement.SetValue(Canvas.ZIndexProperty, 2)
                Storyboard.SetTarget(Me._flyAnimation, value)
                Me._backgroundElement.RenderTransform = New TranslateTransform()

                ' Reset the key frames.
                Me._transitionStoryboard.[Stop]()
                Me._flyAnimation.KeyFrames.Clear()
                Dim frame1 As New EasingDoubleKeyFrame() With { _
                  .KeyTime = KeyTime.FromTimeSpan(TimeSpan.Zero) _
                }


                Dim frame2 As New EasingDoubleKeyFrame() With { _
                  .KeyTime = KeyTime.FromTimeSpan(Me.TransitionDuration) _
                }

                ' Configure the property to animate depending on the fly direction.
                If Me.Direction = FlyDirection.Left OrElse Me.Direction = FlyDirection.Right Then
                    Storyboard.SetTargetProperty(Me._flyAnimation, New PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)"))
                    frame1.Value = If((Me.Direction = FlyDirection.Left), -value.ActualWidth, value.ActualWidth)
                    frame2.Value = 0
                Else
                    Storyboard.SetTargetProperty(Me._flyAnimation, New PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"))
                    frame1.Value = If((Me.Direction = FlyDirection.Up), -value.ActualHeight, value.ActualHeight)
                    frame2.Value = 0
                End If

                Me._flyAnimation.KeyFrames.Add(frame1)
                Me._flyAnimation.KeyFrames.Add(frame2)
            End If
        End Set
    End Property

    Public Overrides Property ForegroundElement() As FrameworkElement
        Get
            Return Me._foregroundElement
        End Get
        Set(value As FrameworkElement)
            If value IsNot Nothing Then
                Me._foregroundElement = value
                If value IsNot Nothing Then
                    Me._foregroundElement.SetValue(Canvas.ZIndexProperty, 0)
                End If
            End If
        End Set
    End Property

    Public Overrides ReadOnly Property ImageZIndexModified() As Boolean
        Get
            ' This transition altered the background/foreground images' z-index.
            ' So we must return true.
            Return True
        End Get
    End Property

    Public Overrides Sub Begin()
        Me._transitionStoryboard.Begin()
    End Sub

    Public Overrides Sub [Stop]()
        Me._transitionStoryboard.[Stop]()
    End Sub

    Private Sub TransitionStoryboard_Completed(sender As Object, e As EventArgs)
        ' Reset the render transform as it is no longer needed.
        Me.BackgroundElement.RenderTransform = Nothing
        RaiseEvent TransitionCompleted(Me, EventArgs.Empty)
    End Sub

    ''' <summary>
    ''' Serialize additional properties specific to this transition.
    ''' Namely, the FlyDirection property.
    ''' </summary>
    Public Overrides Sub Save(transitionElement As XElement)
        transitionElement.Add(New XAttribute("Direction", Me.Direction))
        MyBase.Save(transitionElement)
    End Sub

    ''' <summary>
    ''' Desterilize additional properties specific to this transition.
    ''' Namely, the FlyDirection property.
    ''' </summary>
    Protected Overrides Sub LoadChild(transitionElement As XElement)
        Try
            Me.Direction = CType([Enum].Parse(GetType(FlyDirection), transitionElement.Attribute("Direction").Value, True), FlyDirection)
            ' TODO: Is it safe to just ignore the exception, and use the default value?
        Catch
        End Try
        MyBase.LoadChild(transitionElement)
    End Sub

    Public Overrides Function Clone() As ITransition
        Return New FlyInTransition() With { _
          .BackgroundElement = Me.BackgroundElement, _
          .ForegroundElement = Me.ForegroundElement, _
          .TransitionDuration = Me.TransitionDuration, _
          .Direction = Me.Direction _
        }
    End Function

    Public Enum FlyDirection
        Up
        Down
        Left
        Right
    End Enum
End Class
