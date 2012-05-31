'/**************************************** 模块头 *****************************************\
'* 模块名:  RemoteFileForm.aspx.vb
'* 项目名:  VBASPNETRemoteUploadAndDownload
'* 版权 (c) Microsoft Corporation.
'* 
'* 创建RemoteDownload实例instance, 输入下载文件名和服务器url地址的参数.
'* 使用DownloadFile方法从远程服务器下载文件.
'*
'* This source is subject to the Microsoft Public License.
'* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
'* All other rights reserved.
'*
'* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
'* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
'* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'\*****************************************************************************************/

Public Class RemoteFileForm
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Protected Sub btnUpload_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim uploadClient As RemoteUpload = Nothing
        If Me.rbUploadList.SelectedIndex = 0 Then
            uploadClient = New HttpRemoteUpload(Me.FileUpload.FileBytes, Me.FileUpload.PostedFile.FileName, Me.tbUploadUrl.Text)
        Else
            uploadClient = New FtpRemoteUpload(Me.FileUpload.FileBytes, Me.FileUpload.PostedFile.FileName, Me.tbUploadUrl.Text)
        End If

        If uploadClient.UploadFile() Then
            Response.Write("上传完成")
        Else
            Response.Write("上传失败")
        End If
    End Sub

    Protected Sub btnDownLoad_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim downloadClient As RemoteDownload = Nothing
        If Me.rbDownloadList.SelectedIndex = 0 Then
            downloadClient = New HttpRemoteDownload(Me.tbDownloadUrl.Text, Me.tbDownloadPath.Text)
        Else
            downloadClient = New FtpRemoteDownload(Me.tbDownloadUrl.Text, Me.tbDownloadPath.Text)
        End If

        If downloadClient.DownloadFile() Then
            Response.Write("下载完成")
        Else
            Response.Write("下载失败")
        End If
    End Sub

End Class