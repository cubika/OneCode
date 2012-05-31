'*************************** 模块头 ******************************'
' 模块名:  IMAGE_DATA_DIRECTORY_Values.vb
' 项目名:	    VBCheckEXEType
' 版权 (c) Microsoft Corporation.
' 
' 数据目录是大小为16的结构体数组. 每一个数组元素都
' 预先定义了它的含义. 常量 IMAGE_DIRECTORY_ENTRY_ xxx 
' 表示数组目录的索引(从 0 到 15).
' 
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*************************************************************************'

Namespace IMAGE

    Public Class IMAGE_DATA_DIRECTORY_Values
        ' 导出目录
        Public Const IMAGE_DIRECTORY_ENTRY_EXPORT As Integer = 0

        ' 导入目录
        Public Const IMAGE_DIRECTORY_ENTRY_IMPORT As Integer = 1

        ' 资源目录
        Public Const IMAGE_DIRECTORY_ENTRY_RESOURCE As Integer = 2

        ' 异常目录
        Public Const IMAGE_DIRECTORY_ENTRY_EXCEPTION As Integer = 3

        ' 安全目录
        Public Const IMAGE_DIRECTORY_ENTRY_SECURITY As Integer = 4

        ' 基础重定向表
        Public Const IMAGE_DIRECTORY_ENTRY_BASERELOC As Integer = 5

        ' 调试目录
        Public Const IMAGE_DIRECTORY_ENTRY_DEBUG As Integer = 6

        ' 体系的具体数据
        Public Const IMAGE_DIRECTORY_ENTRY_ARCHITECTURE As Integer = 7

        ' 全局指针的相对虚地址
        Public Const IMAGE_DIRECTORY_ENTRY_GLOBALPTR As Integer = 8

        ' 线程本地存储目录
        Public Const IMAGE_DIRECTORY_ENTRY_TLS As Integer = 9

        ' 加载配置目录
        Public Const IMAGE_DIRECTORY_ENTRY_LOAD_CONFIG As Integer = 10

        ' 头部的绑定导入目录
        Public Const IMAGE_DIRECTORY_ENTRY_BOUND_IMPORT As Integer = 11

        ' 导入地址表
        Public Const IMAGE_DIRECTORY_ENTRY_IAT As Integer = 12

        ' 延迟加载导入描述符
        Public Const IMAGE_DIRECTORY_ENTRY_DELAY_IMPORT As Integer = 13

        ' COM 运行时描述符
        Public Const IMAGE_DIRECTORY_ENTRY_COM_DESCRIPTOR As Integer = 14

        Public Property Values() As IMAGE_DATA_DIRECTORY()

        Public Sub New()
            Values = New IMAGE_DATA_DIRECTORY(15) {}
        End Sub
    End Class

End Namespace

