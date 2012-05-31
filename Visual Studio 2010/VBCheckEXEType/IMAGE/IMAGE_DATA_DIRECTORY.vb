'*************************** 模块头 ******************************'
' 模块名:  IMAGE_DATA_DIRECTORY.vb
' 项目名:	    VBCheckEXEType
' 版权 (c) Microsoft Corporation.
' 
' 表示数据目录. 
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

    Public Structure IMAGE_DATA_DIRECTORY

        ' 数据的相对虚地址
        Public VirtualAddress As UInt32

        ' 数据大小
        Public Size As UInt32

    End Structure

End Namespace


