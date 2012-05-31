'**************************************** 模块头 **************************************\
' 模块名:  ProgrammingLanguage.vb
' 项目名:  VBEnumStringConverter
' 版权 (c) Microsoft Corporation. 
'
' 该文件定义了一个标记枚举以用于演示.
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


''' <summary>
''' Programming languages (这是用于演示的标记枚举)
''' </summary>
<Flags()> _
Enum ProgrammingLanguage
    <Description("Visual Basic")> _
    VB = &H1
    <Description("Visual C#")> _
    CS = &H2
    <Description("Visual C++")> _
    Cpp = &H4
    <Description("Javascript")> _
    JS = &H8
    ' XAML
    XAML = &H10
End Enum

