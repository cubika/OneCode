'*************************** 模块头 ******************************'
' 模块名:  IMAGE_FILE_HEADER.vb
' 项目名:	    VBCheckEXEType
' 版权 (c) Microsoft Corporation.
' 
' 表示 COFF 头部格式. 
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

    Public Structure IMAGE_FILE_HEADER
        ''' <summary>
        ''' 该可执行文件的目标CPU. 
        ''' 一般取值为: IMAGE_FILE_MACHINE_I386    0x014c // Intel 386
        '''             IMAGE_FILE_MACHINE_IA64    0x0200 // Intel 64
        '''             IMAGE_FILE_MACHINE_AMD64   0x8664 // AMD 64 
        ''' </summary>
        Public Machine As UInt16

        ''' <summary>
        ''' 显示在节表中有多少节. 节表
        ''' 紧跟在 IMAGE_NT_HEADERS 后.
        ''' </summary>
        Public NumberOfSections As UInt16

        ''' <summary>
        ''' 显示文件创建的时间. 这个值是指从格林威治标准时间（GMT） 
        ''' 1970年1月1号到现在的秒数. 这个值
        ''' 比文件系统的日期/时间更准确的表示文件是何时创建的
        ''' </summary>
        Public TimeDateStamp As UInt32

        ''' <summary>
        ''' COFF符号表的文件偏移量, 在微软详细说明书中的5.4节有描述.
        ''' COFF 符号表是比较罕见的PE文件,
        ''' 因为现在有较新的调试格式来处理. 在Visual Studio .NET之前, 
        ''' 一个COFF s符号表可有指定的链接开关创建：/DEBUGTYPE:COFF. 
        ''' 几乎总会在 OBJ 文件中找到COFF 符号表. 
        ''' 如果没有符号表存在，设置为0.
        ''' </summary>
        Public PointerToSymbolTable As UInt32

        ''' <summary>
        ''' 如果COFF符号表存在，那么该表中会有大量的符号. COFF 符号是一个 
        ''' 大小固定的数据结构, 并且这个字段需要找到COFF符号的结尾. 
        ''' 紧随COFF符号的是一个字符串表，
        ''' 该表存着更长的符号名.
        ''' </summary>
        Public NumberOfSymbols As UInt32

        ''' <summary>
        ''' 这是跟在 IMAGE_FILE_HEADER 后面的可选数据的大小. 在PE文件中,
        ''' 这个数据就是 IMAGE_OPTIONAL_HEADER. 其大小是否相同依赖于 
        ''' 它是32位文件还是64位文件. 对32位的PE文件来说, 这个字段的值常常是 
        ''' 224. 对于64位的PE32+的文件来说, 它通常是 240. 但是, 他们的值都是最小值,
        ''' 并且更大的值是可以出现的.
        ''' </summary>
        Public SizeOfOptionalHeader As UInt16

        ''' <summary>
        ''' 表明文件属性的一组位标识. 
        ''' </summary>
        Public Characteristics As IMAGE_FILE_Flag

    End Structure

End Namespace
