'********************************* 模块头 *********************************\
' 模块名称:        ProjectFileItem.vb
' 项目 :           VBVSXSaveProject
' 版权所有（c）微软公司
' 
' 获取项目文件的信息，其中包括在本项目中的文件夹标志和用来设置选定复制文件的选项。
' 
' The source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER 
' EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF 
' MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***********************************************************************************/

Imports System.IO

Namespace Files
    Public Class ProjectFileItem
        ''' <summary>
        ''' 文件的信息
        ''' </summary>
        Public Property Fileinfo() As FileInfo

        ''' <summary>
        ''' 文件名
        ''' </summary>
        Public ReadOnly Property FileName() As String
            Get
                Return Fileinfo.Name
            End Get
        End Property

        ''' <summary>
        ''' 文件的路径
        ''' </summary>
        Public ReadOnly Property FullName() As String
            Get
                Return Fileinfo.FullName
            End Get
        End Property

        ''' <summary>
        ''' 指定的文件是否是在项目文件夹
        ''' </summary>
        Public Property IsUnderProjectFolder() As Boolean

        ''' <summary>
        ''' 指定文件是否应该被复制
        ''' </summary>
        Public Property NeedCopy() As Boolean
    End Class
End Namespace