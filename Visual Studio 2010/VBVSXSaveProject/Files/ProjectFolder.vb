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

    Public NotInheritable Class ProjectFolder
        ''' <summary>
        ''' 获取项目文件夹中的文件。
        ''' </summary>
        ''' <param name="projectFilePath">
        ''' 项目文件路径
        ''' </param>
        Private Sub New()
        End Sub
        Public Shared Function GetFilesInProjectFolder(ByVal projectFilePath As String) As List(Of ProjectFileItem)
            ' 获取包括项目文件的文件夹
            Dim projFile As New FileInfo(projectFilePath)
            Dim projFolder As DirectoryInfo = projFile.Directory

            If projFolder.Exists Then
                ' 获取项目文件夹中的所有文件的信息
                Dim files = projFolder.GetFiles("*", SearchOption.AllDirectories)

                Return files.Select(Function(f) New ProjectFileItem With _
                                                {.Fileinfo = f,
                                                 .IsUnderProjectFolder = True}).ToList()
            Else
                ' 该项目文件夹不存在
                Return Nothing
            End If
        End Function

    End Class
End Namespace