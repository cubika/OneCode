'********************************* 模块头 *********************************\
' 模块名: NullToBoolConverter.vb
' 项目名: VideoStoryCreator
' 版权(c) Microsoft Corporation.
' 
' 将object转为boolean.
' 如果object为空, 返回false.
' 其他情况返回true.
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
''' 一个converter. 如果值为空, 返回false. 其他情况下返回true.
''' </summary>
''' <remarks></remarks>
Public Class NullToBoolConverter
    Implements IValueConverter
    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As System.Globalization.CultureInfo) As Object Implements IValueConverter.Convert
        Return (value IsNot Nothing)
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As System.Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException()
    End Function
End Class
