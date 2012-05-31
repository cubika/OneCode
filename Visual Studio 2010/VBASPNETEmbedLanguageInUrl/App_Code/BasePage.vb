'********************************* 模块头 ***********************************\
' 模块名: BasePage.vb
' 项目名: VBASPNETEmbedLanguageInUrl
' 版权 (c) Microsoft Corporation
'
' 不同语言的网页是继承的这个类.
' BasePage类会检查请求的url语言部分和和名称部分,
' 并且设置页面的Culture和UICultrue属性.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*****************************************************************************/


Public Class BasePage
    Inherits Page
    ''' <summary>
    ''' BasePage类用来设置Page.Culture和Page.UICulture.
    ''' </summary>
    Protected Overrides Sub InitializeCulture()
        Try
            Dim language As String = RouteData.Values("language").ToString().ToLower()
            Dim pageName As String = RouteData.Values("pageName").ToString()
            Session("info") = language & "," & pageName
            Page.Culture = language
            Page.UICulture = language
        Catch generatedExceptionName As Exception
            Session("info") = "error,error"
        End Try

    End Sub


End Class
