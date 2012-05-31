'****************************** Module Header ******************************\
' Module Name:	FlyInTransition_Design.vb
' Project:		VideoStoryCreator
' Copyright (c) Microsoft Corporation.
' 
' Provides additional design surface for the fly in transition.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************/



Partial Public Class FlyInTransition_Design
    Inherits UserControl

    Public Sub New()
        InitializeComponent()
        Dim directions As New List(Of FlyInTransition.FlyDirection)() From { _
         FlyInTransition.FlyDirection.Left, _
         FlyInTransition.FlyDirection.Right, _
         FlyInTransition.FlyDirection.Up, _
         FlyInTransition.FlyDirection.Down _
        }
        Me.directionList.ItemsSource = directions
    End Sub
End Class
