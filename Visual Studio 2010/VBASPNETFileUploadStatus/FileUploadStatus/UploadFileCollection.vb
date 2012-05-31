'********************************** 模块头 ******************************\
' 模块名:    UploadFileCollection.vb
' 项目名:    VBASPNETFileUploadStatus
' 版权 (c) Microsoft Corporation
'
' 本项目阐述了在不使用第三方组件时实现显示上传的状态和进程
' 像ActiveX 控件,Flash 或者Silverlight.
' 
' 这个类用来收藏请求实体中上传的文件. 
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'****************************************************************************/

Public Class UploadFileCollection
    Inherits List(Of UploadFile)
End Class

