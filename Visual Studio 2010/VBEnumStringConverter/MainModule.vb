'**************************************** 模块头 **************************************\
' 模块名:  MainModule.vb
' 项目名:  VBEnumStringConverter
' 版权 (c) Microsoft Corporation. 
'
'
'
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
'
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE. 
'**************************************************************************************/


Imports System.ComponentModel


Module MainModule

    Sub Main()
        Console.WriteLine("使用 EnumConverter")
        If True Then
            Dim converter As New EnumConverter(GetType(ProgrammingLanguage))

            ' 将字符串转换为枚举.
            Dim langStr As String = "VB, CS, Cpp, XAML"
            Console.WriteLine("将字符串 ""{0}"" 转换为枚举...", langStr)
            Dim lang As ProgrammingLanguage =
                DirectCast(converter.ConvertFromString(langStr), ProgrammingLanguage)
            Console.WriteLine("完成!")

            ' 将枚举转换为字符串.
            Console.WriteLine("将枚举结果转换为字符串...")
            langStr = converter.ConvertToString(lang)
            Console.WriteLine("完成! ""{0}""", langStr)
        End If

        Console.WriteLine(vbLf & "使用 EnumDescriptionConverter")
        If True Then
            Dim converter As New EnumDescriptionConverter(GetType(ProgrammingLanguage))

            ' 将字符串转换为枚举.
            Dim langStr As String = "Visual Basic, Visual C#, Visual C++, XAML"
            Console.WriteLine("将字符串 ""{0}"" 转换为枚举...", langStr)
            Dim lang As ProgrammingLanguage =
                DirectCast(converter.ConvertFromString(langStr), ProgrammingLanguage)
            Console.WriteLine("完成!")

            ' 将枚举转换为字符串.
            Console.WriteLine("将枚举结果转换为字符串...")
            langStr = converter.ConvertToString(lang)
            Console.WriteLine("完成! ""{0}""", langStr)
        End If

        Console.ReadLine()
    End Sub

End Module

