'********************************* 模块头 *********************************\
' 模块名: TransitionConverter.vb
' 项目名: VideoStoryCreator
' 版权(c) Microsoft Corporation.
' 
' 将transition转为string，反之亦然.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************/



Imports System.Windows.Data

Public Class TransitionConverter
    Implements IValueConverter
    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As System.Globalization.CultureInfo) As Object Implements IValueConverter.Convert

        If value Is Nothing Then
            ' Default to fade transition.
            Return "Fade Transition"
        End If
        Return DirectCast(value, ITransition).Name
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As System.Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
        If value Is Nothing Then
            Return TransitionFactory.CreateTransition("Fade Transition")
        End If
        Return TransitionFactory.CreateTransition(DirectCast(value, String))
    End Function
End Class
