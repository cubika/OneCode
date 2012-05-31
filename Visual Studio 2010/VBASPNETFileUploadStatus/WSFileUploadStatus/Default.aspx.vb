'**************************** 模块头 **********************************\
' 模块名:    Default.aspx.vb
' 项目名:    VBASPNETFileUploadStatus
' 版权 (c) Microsoft Corporation
'
' 本项目阐述了在不使用第三方组件时实现显示上传的状态和进程
' 像ActiveX 控件,Flash 或者Silverlight.
' 
' 在这个页面中我们为用户测试上传的状态.
' 我们在不刷新页面时使用ICallbackEventHandler来实现服务器端
' 和客户端的通信.
' 但是我们需要使用一个iframe来放置上传控件和按钮,因为上传
' 需要回发到服务器端,我们不能在同一个回发页面上调用服务器端
' 的javascript代码. 
' 所以我们用iframe来做上传的回发操作.
' 
' 想知道关于ICallbackEventHandler更多详细信息，
' 请参考根目录下的readme文件.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**************************************************************************/



Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Threading
Imports System.Web.Script.Serialization
Imports System.Text
Imports VBASPNETFileUploadStatus


Partial Public Class _Default
    Inherits System.Web.UI.Page
    Implements ICallbackEventHandler

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
        ' 为ICallbackEventHandler注册一个客户端脚本
        Dim cm As ClientScriptManager = Page.ClientScript
        Dim cbReference As String = cm.GetCallbackEventReference(Me, "arg", "ReceiveServerData", "")
        Dim callbackScript As String = "function CallServer(arg, context) {" & cbReference & "; }"
        cm.RegisterClientScriptBlock(Me.[GetType](), "CallServer", callbackScript, True)

    End Sub

    Private uploadModuleProgress As String = ""
    Public Function GetCallbackResult() As String Implements ICallbackEventHandler.GetCallbackResult
        Return uploadModuleProgress
    End Function

    Public Sub RaiseCallbackEvent(ByVal eventArgument As String) Implements ICallbackEventHandler.RaiseCallbackEvent
        If eventArgument = "Clear" Then
            ' 清除缓存的操作
            ClearCache("fuFile")
            uploadModuleProgress = "Cleared"
        End If
        If eventArgument = "Abort" Then
            ' 终止上传的操作
            AbortUpload("fuFile")
            uploadModuleProgress = "Aborted"
        End If

        Try
            Dim state As UploadStatus = TryCast(HttpContext.Current.Cache("fuFile"), UploadStatus)
            If state Is Nothing Then
                Return
            End If
            ' 我们使用JSON来向客户端发送数据,
            ' 因为它不仅简单而且容易操作.
            ' 想知道JavaScriptSerializer更多更详细的内容，
            ' 请阅读根目录下的readme文件.
            Dim jss As New JavaScriptSerializer()

            ' StringBuilder对象将会保存序列化结果.
            Dim sbUploadProgressResult As New StringBuilder()
            jss.Serialize(state, sbUploadProgressResult)

            uploadModuleProgress = sbUploadProgressResult.ToString()
        Catch err As Exception
            If err.InnerException IsNot Nothing Then
                uploadModuleProgress = "Error:" + err.InnerException.Message
            Else
                uploadModuleProgress = "Error:" + err.Message
            End If
        End Try
    End Sub

    Private Sub AbortUpload(ByVal cacheID As String)
        Dim state As UploadStatus = TryCast(HttpContext.Current.Cache(cacheID), UploadStatus)
        If state Is Nothing Then
            Return
        Else
            state.Abort()
        End If
    End Sub

    Private Sub ClearCache(ByVal cacheID As String)
        HttpContext.Current.Cache.Remove(cacheID)
    End Sub


End Class
