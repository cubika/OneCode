'*************************** Module Header ******************************'
' 模块名称:  DBIM.vb
' 项目:	    VBIEExplorerBar
' Copyright (c) Microsoft Corporation.
' 
' 用于DESKBANDINFO结构 
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


Namespace NativeMethods
    <Flags()>
    Public Enum DBIM
        NORMAL = 0
        MINSIZE = &H1
        MAXSIZE = &H2
        INTEGRAL = &H4
        VARIABLEHEIGHT = &H8
        TITLE = &H10
        MODEFLAGS = &H20
        BKCOLOR = &H40
        USECHEVRON = &H80
        BREAK = &H100
        ADDTOFRONT = &H200
        TOPALIGN = &H400
    End Enum


End Namespace
