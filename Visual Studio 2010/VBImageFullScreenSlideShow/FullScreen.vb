'************************************ 模块头 **************************************'
' 模块名:  FullScreen.vb
' 项目名:  VBImageFullScreenSlideShow
' 版权 (c) Microsoft Corporation.
'
' 该类定义了两个辅助方法，用于使Windows窗体进入全屏模式和离开全屏模式. 
'
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER 
' EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF 
' MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**********************************************************************************'

Public Class FullScreen

    Private winState As FormWindowState
    Private brdStyle As FormBorderStyle
    Private topMost As Boolean
    Private bounds As Rectangle

    Public Sub New()
        IsFullScreen = False
    End Sub

    Public Property IsFullScreen() As Boolean

    ''' <summary>
    ''' 将窗口最大化至全屏.
    ''' </summary>
    Public Sub EnterFullScreen(ByVal targetForm As Form)
        If Not IsFullScreen Then
            Save(targetForm) ' 保存原始的窗体状态.

            targetForm.WindowState = FormWindowState.Maximized
            targetForm.FormBorderStyle = FormBorderStyle.None
            targetForm.TopMost = True
            targetForm.Bounds = Screen.GetBounds(targetForm)

            IsFullScreen = True
        End If
    End Sub

    ''' <summary>
    ''' 保存当前的窗口状态.
    ''' </summary>
    Private Sub Save(ByVal targetForm As Form)
        winState = targetForm.WindowState
        brdStyle = targetForm.FormBorderStyle
        topMost = targetForm.TopMost
        bounds = targetForm.Bounds
    End Sub

    ''' <summary>
    ''' 离开全屏模式,回到原始的窗口状态.
    ''' </summary>
    Public Sub LeaveFullScreen(ByVal targetForm As Form)
        If IsFullScreen Then
            ' 回到原始的窗口状态.
            targetForm.WindowState = winState
            targetForm.FormBorderStyle = brdStyle
            targetForm.TopMost = topMost
            targetForm.Bounds = bounds

            IsFullScreen = False
        End If
    End Sub
End Class
