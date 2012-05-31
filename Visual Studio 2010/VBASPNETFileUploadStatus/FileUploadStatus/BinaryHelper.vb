'************************************ 模块头 ******************************\
' 模块名:    BinaryHelper.vb
' 项目名:    VBASPNETFileUploadStatus
' 版权 (c) Microsoft Corporation
'
' 本项目阐述了在不使用第三方组件时实现显示上传的状态和进程
' 像ActiveX 控件,Flash 或者Silverlight.
' 
' 这个类是用来过滤二进制数据进而获得文件数据. 
' 所有这些静态的方法对于二进制数据的处理都是有帮助的.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'****************************************************************************/

Friend Class BinaryHelper

    ' 从一个原始的二进制数组中复制部分数据.
    Public Shared Function Copy(ByVal source As Byte(),
                                ByVal index As Integer,
                                ByVal length As Integer) As Byte()
        Dim result As Byte() = New Byte(length - 1) {}
        Array.ConstrainedCopy(source, index, result, 0, length)
        Return result
    End Function

    ' 把两个二进制数组合并在一起.
    Public Shared Function Combine(ByVal a As Byte(), ByVal b As Byte()) As Byte()
        If a Is Nothing AndAlso b Is Nothing Then
            Return Nothing
        ElseIf a Is Nothing OrElse b Is Nothing Then
            Return If(a, b)
        End If
        Dim newData As Byte() = New Byte(a.Length + (b.Length - 1)) {}
        Array.ConstrainedCopy(a, 0, newData, 0, a.Length)
        Array.ConstrainedCopy(b, 0, newData, a.Length, b.Length)
        Return newData

    End Function

    ' 检查两个二进制数组在同一个索引
    ' 里是否有相同的数据 .
    Public Overloads Shared Function Equals(ByVal source As Byte(),
                                            ByVal compare As Byte()) As Boolean
        If source.Length <> compare.Length Then
            Return False
        End If
        If SequenceIndexOf(source, compare, 0) <> 0 Then
            Return False
        End If
        Return True
    End Function

    ' 在二进制数组里获取部分数据.
    Public Shared Function SubData(ByVal source As Byte(),
                                   ByVal startIndex As Integer) As Byte()
        Dim result As Byte() = New Byte(source.Length - startIndex - 1) {}
        Array.ConstrainedCopy(source, startIndex, result, 0, result.Length)
        Return result
    End Function

    ' 在二进制数组里获取部分数据.
    Public Shared Function SubData(ByVal source As Byte(),
                                   ByVal startIndex As Integer,
                                   ByVal length As Integer) As Byte()
        Dim result As Byte() = New Byte(length - 1) {}
        Array.ConstrainedCopy(source, startIndex, result, 0, length)
        Return result
    End Function

    ' 在原数组里所有数据和位置都和另
    ' 一个数组相同的时候获取原数组索引.
    Public Shared Function SequenceIndexOf(ByVal source As Byte(),
                                           ByVal compare As Byte()) As Integer
        Return SequenceIndexOf(source, compare, 0)
    End Function
    Public Shared Function SequenceIndexOf(ByVal source As Byte(),
                                           ByVal compare As Byte(),
                                           ByVal startIndex As Integer) As Integer
        Dim result As Integer = -1
        Dim sourceLen As Integer = source.Length
        Dim compareLen As Integer = compare.Length
        If startIndex < 0 Then
            Return -1
        End If

        For i As Integer = startIndex To sourceLen - compareLen
            If source(i) = compare(0) AndAlso _
                source(i + compareLen - 1) = compare(compareLen - 1) Then
                Dim t As Integer = 0
                For j As Integer = 0 To compare.Length - 1
                    t += 1
                    If compare(j) <> source(i + j) Then
                        Exit For
                    End If
                Next
                If t = compareLen Then
                    result = i
                    Exit For
                End If
            End If
        Next
        Return result
    End Function

End Class
