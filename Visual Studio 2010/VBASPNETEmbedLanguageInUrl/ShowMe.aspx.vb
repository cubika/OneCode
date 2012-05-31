'********************************* 模块头 ***********************************\
' 模块名: ShowMe.aspx.vb
' 项目名: VBASPNETEmbedLanguageInUrl
' 版权 (c) Microsoft Corporation
'
' 本项目阐述了如何在URL中插入语言编码,例如：
' http://domain/en-us/ShowMe.aspx. 页面会根据语言
' 编码呈现不同的内容,在例子中使用url-路径和源文件 
' 来本地化页面的内容.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*****************************************************************************/



Public Class _Default
    Inherits BasePage
    ''' <summary>
    ''' 用一种确定的语言加载页面.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim elementArray As String() = Session("info").ToString().Split(","c)
        Dim language As String = elementArray(0)
        Dim pageName As String = elementArray(1)
        If language = "error" Then
            Response.Write("url地址错误:请在Default.aspx页面重新启动应用程序.")
            Return
        End If
        Dim xmlPath As String = Server.MapPath("~/XmlFolder/Language.xml")
        Dim strTitle As String = String.Empty
        Dim strText As String = String.Empty
        Dim strElement As String = String.Empty
        Dim flag As Boolean = False

        ' 加载xml数据.
        Dim xmlLoad As New XmlLoad()
        xmlLoad.XmlLoadMethod(language, strTitle, strText, strElement, flag)

        ' 如果特定的语言不存在,返回这个网页的英文版.
        If flag = True Then
            language = "en-us"
            Response.Write("没有该语言的资源,将使用英文网页.")
            xmlLoad.XmlLoadMethod(language, strTitle, strText, strElement, flag)
        End If
        lbTitleContent.Text = strTitle
        lbTextContent.Text = strText
        lbTimeContent.Text = DateTime.Now.ToLongDateString()
        lbCommentContent.Text = strElement
    End Sub

End Class