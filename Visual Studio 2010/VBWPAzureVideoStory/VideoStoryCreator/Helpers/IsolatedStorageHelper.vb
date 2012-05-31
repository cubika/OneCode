'********************************* 模块头 *********************************\
' 模块名: IsolatedStorageHelper.vb
' 项目名: VideoStoryCreator
' 版权(c) Microsoft Corporation.
' 
' 一个关于 isolated storage I/O操作的helper类.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************/


Imports System.IO.IsolatedStorage
Imports System.IO

Public NotInheritable Class IsolatedStorageHelper
    ''' <summary>
    ''' 删除文件
    ''' </summary>
    ''' <param name="name">被删除图片的名称</param>
    Public Shared Sub DeleteFile(name As String)
        Dim userStore As IsolatedStorageFile = IsolatedStorageFile.GetUserStoreForApplication()
        If userStore.FileExists(name) Then
            userStore.DeleteFile(name)
        End If
    End Sub

    ''' <summary>
    ''' 文件重命名.
    '''首先建立一个新的文件并且从源文件拷贝内容过来.
    ''' 然后删除原始文件.
    ''' </summary>
    Public Shared Sub RenameFile(originalName As String, newName As String)
        Dim userStore As IsolatedStorageFile = IsolatedStorageFile.GetUserStoreForApplication()
        If userStore.FileExists(originalName) Then
            Using originalFileStream As FileStream = userStore.OpenFile(originalName, System.IO.FileMode.Open)
                Using newFileStream As FileStream = userStore.CreateFile(newName)
                    Dim buffer As Byte() = New Byte(originalFileStream.Length - 1) {}
                    originalFileStream.Read(buffer, 0, buffer.Length)
                    newFileStream.Write(buffer, 0, buffer.Length)
                End Using
            End Using
            userStore.DeleteFile(originalName)
        End If
    End Sub

    ''' <summary>
    '''检查文件是否存在.
    ''' </summary>
    Public Shared Function FileExists(name As String) As Boolean
        Dim userStore As IsolatedStorageFile = IsolatedStorageFile.GetUserStoreForApplication()
        Return userStore.FileExists(name)
    End Function
End Class
