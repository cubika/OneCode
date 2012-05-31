'********************************* 模块头 *********************************\
' 模块名: ChoosePhotoViewModel.vb
' 项目名: VideoStoryCreator
' 版权(c) Microsoft Corporation.
' 
' 选中图片的ViewModel类.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************/



Public Class ChoosePhotoViewModel
    Inherits PhotoViewModel

    Public Property IsSelected() As Boolean
        Get
            Return m_IsSelected
        End Get
        Set(value As Boolean)
            m_IsSelected = Value
        End Set
    End Property
    Private m_IsSelected As Boolean
End Class
