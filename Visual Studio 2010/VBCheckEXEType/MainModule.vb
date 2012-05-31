'*************************** 模块头 ******************************'
' 模块名:  MainModule.vb
' 项目名:	    VBCheckEXEType
' 版权 (c) Microsoft Corporation.
' 
' 这个源文件用于处理输入的命令.
' 
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**************************************************************************

Imports System.IO

Module MainModule

    Sub Main()
        Do
            Console.WriteLine("请键入EXE文件路径:")
            Console.WriteLine("<直接回车退出>")
            Dim path As String = Console.ReadLine()

            If String.IsNullOrEmpty(path) Then
                Exit Do
            End If

            If Not File.Exists(path) Then
                Console.WriteLine("路径不存在!")
                Continue Do
            End If
            Try
                Dim exeFile As New ExecutableFile(path)

                Dim isConsole = exeFile.IsConsoleApplication
                Dim isDotNet = exeFile.IsDotNetAssembly


                Console.WriteLine(String.Format("控制台应用程序: {0}" _
                                                & ControlChars.CrLf _
                                                & ".Net应用程序: {1}",
                                                isConsole,
                                                isDotNet))

                If isDotNet Then
                    Console.WriteLine(".NET编译运行时: " & exeFile.GetCompiledRuntimeVersion())
                    Console.WriteLine("全名: " & exeFile.GetFullDisplayName())
                    Dim attributes = exeFile.GetAttributes()
                    For Each attribute In attributes
                        Console.WriteLine(String.Format("{0}: {1}", attribute.Key, attribute.Value))
                    Next attribute
                Else
                    Dim is32Bit = exeFile.Is32bitImage
                    Console.WriteLine("32位 应用程序: " & is32Bit)
                End If
            Catch ex As Exception
                Console.WriteLine(ex.Message)
            End Try
            Console.WriteLine()
        Loop
    End Sub

End Module
