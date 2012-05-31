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
Imports EnvDTE

Namespace Files

    Public Class VSProject
        ''' <summary>
        ''' 一个项目的解决方案
        ''' </summary>
        Private privateProject As Project
        Public Property Project() As Project
            Get
                Return privateProject
            End Get
            Private Set(ByVal value As Project)
                privateProject = value
            End Set
        End Property

        ''' <summary>
        ''' 该文件夹包含的项目文件
        ''' </summary>
        Private privateProjectFolder As DirectoryInfo
        Public Property ProjectFolder() As DirectoryInfo
            Get
                Return privateProjectFolder
            End Get
            Private Set(ByVal value As DirectoryInfo)
                privateProjectFolder = value
            End Set
        End Property

        ''' <summary>
        ''' 初始化的项目和项目文件夹属性
        ''' </summary>
        Public Sub New(ByVal proj As Project)
            ' 初始化的项目对象。
            Me.Project = proj

            ' 获取当前的项目目录
            Me.ProjectFolder = New FileInfo(Project.FullName).Directory
        End Sub

        ''' <summary>
        ''' 获取包含在项目中的所有文件
        ''' </summary>
        Public Function GetIncludedFiles() As List(Of ProjectFileItem)
            Dim files = New List(Of ProjectFileItem)()

            ' 向文件列表中增加项目文件(*.csproj 或者 *.vbproj...)
            files.Add(New ProjectFileItem With _
                      {.Fileinfo = New FileInfo(Project.FullName),
                       .NeedCopy = True,
                       .IsUnderProjectFolder = True})

            '增加项目中包含的文件
            For Each item As ProjectItem In Project.ProjectItems
                GetProjectItem(item, files)
            Next item

            Return files
        End Function

        ''' <summary>
        ''' 获取包含在项目中的所有文件
        ''' </summary>
        Private Sub GetProjectItem(ByVal item As ProjectItem, ByVal files As List(Of ProjectFileItem))
            ' 获取与一个项目项相关的文件。
            ' 大部分的工程项目只包括一个文件，但有些的可能不止一个，
            ' 在Visual Basic中有两种文件形式保存.FRM（文本)和.frx（二进制）文件。
            ' 详情查看 http://msdn.microsoft.com/en-us/library/envdte.projectitem.filecount.aspx
            For i As Short = 0 To item.FileCount - 1
                If File.Exists(item.FileNames(i)) Then
                    Dim fileItem As New ProjectFileItem()

                    fileItem.Fileinfo = New FileInfo(item.FileNames(i))

                    If fileItem.FullName.StartsWith(Me.ProjectFolder.FullName,
                                                    StringComparison.OrdinalIgnoreCase) Then
                        fileItem.IsUnderProjectFolder = True
                        fileItem.NeedCopy = True
                    End If

                    files.Add(fileItem)
                End If
            Next i

            ' 获取此节点下的子节点的文件。
            For Each subItem As ProjectItem In item.ProjectItems
                GetProjectItem(subItem, files)
            Next subItem
        End Sub
    End Class
End Namespace