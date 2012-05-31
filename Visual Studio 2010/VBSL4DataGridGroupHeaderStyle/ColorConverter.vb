'********************************* 模块头 **********************************\
' 模块名:    ColorConverter.vb
' 项目:      VBSL4DataGridGroupHeaderStyle
' Copyright (c) Microsoft Corporation.
' 
' 根据 Group Name 改变 GroupHeader 的背景.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************/
Imports System.Windows.Media
Imports System.Windows.Data

Public Class ColorConverter
    Implements IValueConverter

    Public Function Convert(ByVal value As Object, ByVal targetType As Type, _
    ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object _
    Implements IValueConverter.Convert
        If value.Equals("儿童") Then
            Return New SolidColorBrush(Colors.Yellow)
        End If
        If value.Equals("成人") Then
            Return New SolidColorBrush(Colors.Orange)
        Else
            Return New SolidColorBrush(Colors.Gray)
        End If
    End Function

    Public Function ConvertBack(ByVal value As Object, ByVal targetType As Type, _
    ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object _
    Implements IValueConverter.ConvertBack
        Throw New InvalidOperationException("Converter cannot convert back.")
    End Function

End Class


