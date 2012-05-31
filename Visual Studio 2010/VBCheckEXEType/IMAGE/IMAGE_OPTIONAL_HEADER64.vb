'*************************** 模块头 ******************************'
' 模块名:  IMAGE_OPTIONAL_HEADER64.vb
' 项目名:	    VBCheckEXEType
' 版权 (c) Microsoft Corporation.
' 
' 表示64位应用程序的可选头部格式. 
' 和 IMAGE_OPTIONAL_HEADER32 不同的是 64位应用程序没有BaseOfData
' 字段 并且 ImageBase/SizeOfStackReserve/SizeOfStackCommit/
' SizeOfHeapReserve/SizeOfHeapCommit 的数据类型是UInt64. 
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

    Public Structure IMAGE_OPTIONAL_HEADER64
        Public Magic As UInt16
        Public MajorLinkerVersion As Byte
        Public MinorLinkerVersion As Byte
        Public SizeOfCode As UInt32
        Public SizeOfInitializedData As UInt32
        Public SizeOfUninitializedData As UInt32
        Public AddressOfEntryPoint As UInt32
        Public BaseOfCode As UInt32
        Public ImageBase As UInt64
        Public SectionAlignment As UInt32
        Public FileAlignment As UInt32
        Public MajorOperatingSystemVersion As UInt16
        Public MinorOperatingSystemVersion As UInt16
        Public MajorImageVersion As UInt16
        Public MinorImageVersion As UInt16
        Public MajorSubsystemVersion As UInt16
        Public MinorSubsystemVersion As UInt16
        Public Win32VersionValue As UInt32
        Public SizeOfImage As UInt32
        Public SizeOfHeaders As UInt32
        Public CheckSum As UInt32
        Public Subsystem As UInt16
        Public DllCharacteristics As UInt16
        Public SizeOfStackReserve As UInt64
        Public SizeOfStackCommit As UInt64
        Public SizeOfHeapReserve As UInt64
        Public SizeOfHeapCommit As UInt64
        Public LoaderFlags As UInt32
        Public NumberOfRvaAndSizes As UInt32
        Public DataDirectory() As IMAGE_DATA_DIRECTORY
    End Structure

End Namespace
