'******************************** 模块头 *************************************************'
' 模块名:  SimulateVal.vb
' 项目名:  VBDynamicallyBuildLambdaExpressionWithField
' 版权 (c) Microsoft Corporation.
' 
' SimulateVal.vb 文件定义了处理DateTime和Boolean的不同函数.
'
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
'
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER 
' EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF 
' MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*****************************************************************************************'

Public NotInheritable Class SimulateVal
    Private Sub New()
    End Sub
    Public Shared Function Val(ByVal expression As String) As Double
        If expression Is Nothing Then
            Return 0
        End If
        ' 先尝试整个字符串，然后逐渐处理较小的子字符串，
        ' 以便模拟VB中“Val”的行为.
        ' 它忽视了一个可识别值后面的尾随字符.
        For size As Integer = expression.Length To 1 Step -1
            Dim testDouble As Double
            If Double.TryParse(expression.Substring(0, size), testDouble) Then
                Return testDouble
            End If
        Next size

        ' 没有识别到任何值，所以返回0.
        Return 0
    End Function

    Public Shared Function Val(ByVal expression As Object) As Double
        If expression Is Nothing Then
            Return 0
        End If

        Dim testDouble As Double
        If Double.TryParse(expression.ToString(), testDouble) Then
            Return testDouble
        End If

        Dim testBool As Boolean

        If Boolean.TryParse(expression.ToString(), testBool) Then
            Return If(testBool, -1, 0)
        End If

        Dim testDate As Date
        If Date.TryParse(expression.ToString(), testDate) Then
            Return testDate.Day
        End If
        ' 没有识别到任何值，所以返回0.
        Return 0

    End Function

    ''' <summary>
    ''' 将字符转换为字符串.
    ''' </summary>
    Public Shared Function Val(ByVal expression As Char) As Integer
        Dim testInt As Integer
        If Integer.TryParse(expression.ToString(), testInt) Then
            Return testInt
        Else
            Return 0
        End If
    End Function
End Class