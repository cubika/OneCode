'********************************* 模块头 *********************************\
' 模块名: FlyDirectionConverter.vb
' 项目名: VideoStoryCreator
' 版权(c) Microsoft Corporation.
' 
' 将string转化为FlyDirection, 反之亦然.
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

Public Class FlyDirectionConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As System.Globalization.CultureInfo) As Object Implements IValueConverter.Convert
        If value Is Nothing Then
            Return Nothing
        End If
        Return [Enum].Parse(GetType(FlyInTransition.FlyDirection), DirectCast(value, String), True)

    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As System.Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
        If value Is Nothing Then
            Return Nothing
        End If
        Return DirectCast(value, FlyInTransition.FlyDirection).ToString()
    End Function
End Class
