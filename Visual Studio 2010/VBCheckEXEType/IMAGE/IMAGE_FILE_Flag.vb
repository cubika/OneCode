'*************************** 模块头 ******************************'
' 模块名:  IMAGE_FILE_Flag.vb
' 项目名:	    VBCheckEXEType
' 版权 (c) Microsoft Corporation.
' 
' 在 IMAGE_FILE_HEADER 文件中使用.
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

    <Flags()>
    Public Enum IMAGE_FILE_Flag
        ' 从文件中获取的重定向信息.
        IMAGE_FILE_RELOCS_STRIPPED = &H1

        ' 是可执行文件  (例如：没有为解决的外部引用).
        IMAGE_FILE_EXECUTABLE_IMAGE = &H2

        ' 从文件中获取的行数
        IMAGE_FILE_LINE_NUMS_STRIPPED = &H4

        ' 从文件中获取的本地符号.
        IMAGE_FILE_LOCAL_SYMS_STRIPPED = &H8

        ' 积极地裁剪工作集
        IMAGE_FILE_AGGRESIVE_WS_TRIM = &H10

        ' 应用程序能处理大于2GB的地址
        IMAGE_FILE_LARGE_ADDRESS_AWARE = &H20

        ' 机器字的字节数是保留的.
        IMAGE_FILE_BYTES_REVERSED_LO = &H80

        ' 32位字机器.
        IMAGE_FILE_32BIT_MACHINE = &H100

        ' 从.DBG文件中获取的调试信息
        IMAGE_FILE_DEBUG_STRIPPED = &H200

        ' 如果映像在可移动设备上，复制它，并从SWAP文件运行.
        IMAGE_FILE_REMOVABLE_RUN_FROM_SWAP = &H400

        ' 如果映像在网络上，复制它，并从SWAP文件运行.
        IMAGE_FILE_NET_RUN_FROM_SWAP = &H800

        ' 系统文件.
        IMAGE_FILE_SYSTEM = &H1000

        ' 文件是DLL.
        IMAGE_FILE_DLL = &H2000

        ' 文件只应该在UP（单处理器）上运行.
        IMAGE_FILE_UP_SYSTEM_ONLY = &H4000

        ' 机器字的字节数是保留的.
        IMAGE_FILE_BYTES_REVERSED_HI = &H8000
    End Enum

End Namespace
