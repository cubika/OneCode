'****************************** 模块头 *************************************'
' 模块名:  MainModule.vb
' 项目:    VBCallNativeDllWrapper
' 版权     (c) Microsoft Corporation.
' 
' 这个代码示例展示了通过C++/CLI封装类对一个本地C++的DLL模块的导出的类和方法进行封装，
' 并且被VB.NET用。
'
'  VBCallNativeDllWrapper (.NET应用程序)
'          -->
'     CppCLINativeDllWrapper (C++/CLI封装)
'              -->
'          CppDynamicLinkLibrary (本地C++ DLL模块)
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************'

#Region "Imports directives"

Imports System.Runtime.InteropServices
Imports CppCLINativeDllWrapper

#End Region


Module MainModule

    Sub Main()

        Dim isLoaded As Boolean = False
        Const moduleName As String = "CppDynamicLinkLibrary"

        ' 检查模块是否被调用
        isLoaded = IsModuleLoaded(moduleName)
        Console.WriteLine("模块 ""{0}"" {1}被加载", moduleName, _
            If(isLoaded, "", "没有"))

        '
        ' 从模块中调用被导出函数.
        '
        Dim str As String = "HelloWorld"
        Dim length As Integer

        length = NativeMethods.GetStringLength1(str)
        Console.WriteLine("GetStringLength1(""{0}"") => {1}", str, length)

        length = NativeMethods.GetStringLength2(str)
        Console.WriteLine("GetStringLength2(""{0}"") => {1}", str, length)

        '
        ' 从模块中调用被导出回调函数
        '

        ' P/Invoke需要回调作为参数之一 
        Dim cmpFunc As CompareCallback = New CompareCallback(AddressOf CompareInts)
        Dim max As Integer = NativeMethods.Max(2, 3, cmpFunc)

        ' 确保委托的生命周期可以覆盖整个非托管代码的生命周期，否则委托将不能被垃圾回收
        ' 你也将会得到非法访问或无法访问的错误
        GC.KeepAlive(cmpFunc)
        Console.WriteLine("Max(2, 3) => {0}", max)

        '
        ' 使用从模块中导出的类.
        '

        Dim obj As New CSimpleObjectWrapper
        obj.FloatProperty = 1.2F
        Dim fProp As Single = obj.FloatProperty
        Console.WriteLine("Class: CSimpleObject::FloatProperty = {0:F2}", fProp)

        ' 你不能通过调用GetModuleHandle和FreeLibrary来卸载C++ DLL CppDynamicLinkLibrary

        ' 检查模块是否被加载.
        isLoaded = IsModuleLoaded(moduleName)
        Console.WriteLine("模块 ""{0}"" {1}被加载", moduleName, _
            If(isLoaded, "", "没有"))

    End Sub


    ''' <summary>
    ''' 这个是CppDynamicLinkLibrary.dll导出的回调Max函数
    ''' </summary>
    ''' <param name="a">第一个整数</param>
    ''' <param name="b">第二个整数</param>
    ''' <returns>
    ''' 函数返回一个正数，当a大于b，返回0，当a等于b，返回一个负数当a小于b 
    ''' </returns>
    Function CompareInts(ByVal a As Integer, ByVal b As Integer) As Integer
        Return (a - b)
    End Function


#Region "Module Related Helper Functions"

    ''' <summary>
    ''' 检查特定的模块是否被当前进程加载
    ''' </summary>
    ''' <param name="moduleName">the module name</param>
    ''' <returns>
    ''' 当前进程家在特定模块时返回真，否则返回假
    ''' </returns>
    Function IsModuleLoaded(ByVal moduleName As String) As Boolean
        ' 通过模块名获得进程中的模块
        Dim hMod As IntPtr = GetModuleHandle(moduleName)
        Return (hMod <> IntPtr.Zero)
    End Function

    <DllImport("kernel32.dll", CharSet:=CharSet.Auto, SetLastError:=True)> _
    Function GetModuleHandle(ByVal moduleName As String) As IntPtr
    End Function

#End Region

End Module
