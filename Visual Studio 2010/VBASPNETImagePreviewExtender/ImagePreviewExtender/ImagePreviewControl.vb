'**************************************** 模块头 *****************************************‘
' 模块名:    ImagePreviewControl.vb
' 项目名:    VBASPNETImagePreviewExtender
' 版权 (c) Microsoft Corporation
'
' 本项目演示了如何设计一个 AJAX 扩展程序控件. 
' 在此示例中, 这是个关于图片的扩展控件.
' 使用这个扩展控件的图片最初被显示为一张缩略图,如果用户单击图片,
' 将弹出以原始尺寸显示的大图.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
'
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'******************************************************************************************'

<TargetControlType(GetType(Control))> _
Public Class ImagePreviewControl
    Inherits ExtenderControl

    ''' <summary>
    ''' 定义缩略图模式时图片所使用的css类.
    ''' </summary>
    Public Property ThumbnailCssClass() As String
        Get
            Return m_ThumbnailCssClass
        End Get
        Set(ByVal value As String)
            m_ThumbnailCssClass = value
        End Set
    End Property
    Private m_ThumbnailCssClass As String

    ''' <summary>
    ''' 返回关闭图标的资源.
    ''' </summary>
    Private ReadOnly Property closeImage() As String
        Get
            Return Page.ClientScript.GetWebResourceUrl(Me.[GetType](), "ImagePreviewExtender.Close.png")
        End Get
    End Property

    Protected Overrides Function GetScriptDescriptors(ByVal targetControl As System.Web.UI.Control) As IEnumerable(Of ScriptDescriptor)
        Dim descriptor As New ScriptBehaviorDescriptor("ImagePreviewExtender.ImagePreviewBehavior", targetControl.ClientID)
        descriptor.AddProperty("ThumbnailCssClass", ThumbnailCssClass)

        descriptor.AddProperty("closeImage", closeImage)
        Return New List(Of ScriptDescriptor) From {descriptor}
    End Function

    ' 生成脚本引用
    Protected Overrides Function GetScriptReferences() As IEnumerable(Of ScriptReference)
        Dim scriptRef As New ScriptReference("ImagePreviewExtender.ImagePreviewBehavior.js", Me.GetType().Assembly.FullName)

        Return New List(Of ScriptReference) From {scriptRef}
    End Function
End Class
