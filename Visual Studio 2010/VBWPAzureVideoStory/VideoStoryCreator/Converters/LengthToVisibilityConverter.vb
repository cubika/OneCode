'********************************* 模块头 *********************************\
'模块名: LengthToVisibilityConverter.vb
' 项目名: VideoStoryCreator
' 版权(c) Microsoft Corporation.
' 
' 将整型转为Visibility.
' 返回Collapsed，仅当integer大于0.
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
''' <summary>
''' 一个converter. 值必须为整型.
''' 如果值大于0, 返回visible. 其他情况返回collapsed.
''' </summary>
''' <remarks></remarks>
Public Class LengthToVisibilityConverter
    Implements IValueConverter
    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As System.Globalization.CultureInfo) As Object Implements IValueConverter.Convert
        If TypeOf value Is Integer Then
            Dim length As Integer = CInt(value)
            If length <= 0 Then
                Return Visibility.Visible
            End If
        End If
        Return Visibility.Collapsed
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As System.Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException()
    End Function
End Class
