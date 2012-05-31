'********************************** 模块头 ********************************\
' 模块名: Default.aspx.vb
' 项目名: VBASPNETStripHtmlCode
' 版权 (c) Microsoft Corporation
'
' 这个页面从SourcePage.aspx中搜索完整的html代码.
' 用户可以获取和解析html代码的许多部分,像:纯文本, 
' 图片,链接,脚本代码,等等.
' 这个实例代码可以用在许多应用程序中.例如： 
' 搜索引擎,搜索引擎需要检查网页的简短信息,像标题,
' 纯文本,图片等等.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*****************************************************************************/



Imports System.Net
Imports System.IO

Public Class _Default
    Inherits System.Web.UI.Page
    Dim strUrl As String = [String].Empty
    Dim strWholeHtml As String = String.Empty
    Const MsgPageRetrieveFailed As String = "对不起,网页运行失败！"
    Dim flgPageRetrieved As Boolean = True

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        strUrl = Me.Page.Request.Url.ToString().Replace("Default", "SourcePage")
        tbResult.Text = String.Empty
    End Sub

    Protected Sub btnRetrieveAll_Click(ByVal sender As Object, ByVal e As EventArgs)
        strWholeHtml = Me.GetWholeHtmlCode(strUrl)
        If flgPageRetrieved Then
            tbResult.Text = strWholeHtml
        Else
            tbResult.Text = MsgPageRetrieveFailed
        End If
    End Sub

    ''' <summary>
    ''' 用WebRequest和WebRespond从SourcePage.aspx中检索完整的html代码
    ''' 我们把html代码的格式转换为uft-8.
    ''' </summary>
    ''' <param name="url"></param>
    ''' <returns></returns>
    Public Function GetWholeHtmlCode(ByVal url As String) As String
        Dim strHtml As String = String.Empty
        Dim strReader As StreamReader = Nothing
        Dim wrpContent As HttpWebResponse = Nothing
        Try
            Dim wrqContent As HttpWebRequest = DirectCast(WebRequest.Create(strUrl), HttpWebRequest)
            wrqContent.Timeout = 300000
            wrpContent = DirectCast(wrqContent.GetResponse(), HttpWebResponse)
            If wrpContent.StatusCode <> HttpStatusCode.OK Then
                flgPageRetrieved = False
                strHtml = MsgPageRetrieveFailed
            End If
            If wrpContent IsNot Nothing Then
                strReader = New StreamReader(wrpContent.GetResponseStream(), Encoding.GetEncoding("utf-8"))
                strHtml = strReader.ReadToEnd()
            End If
        Catch e As Exception
            flgPageRetrieved = False
            strHtml = e.Message
        Finally
            If strReader IsNot Nothing Then
                strReader.Close()
            End If
            If wrpContent IsNot Nothing Then
                wrpContent.Close()
            End If
        End Try
        Return strHtml
    End Function

    ''' <summary>
    ''' 从html代码里搜索纯文本,这个纯文本只包括html的
    ''' Body标记.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Protected Sub btnRetrievePureText_Click(ByVal sender As Object, ByVal e As EventArgs)
        strWholeHtml = Me.GetWholeHtmlCode(strUrl)
        If flgPageRetrieved Then
            Dim strRegexScript As String = "(?m)<body[^>]*>(\w|\W)*?</body[^>]*>"
            Dim strRegex As String = "<[^>]*>"
            Dim strMatchScript As String = String.Empty
            Dim matchText As Match = Regex.Match(strWholeHtml, strRegexScript, RegexOptions.IgnoreCase)
            strMatchScript = matchText.Groups(0).Value
            Dim strPureText As String = Regex.Replace(strMatchScript, strRegex, String.Empty, RegexOptions.IgnoreCase)
            tbResult.Text = strPureText
        Else
            tbResult.Text = MsgPageRetrieveFailed
        End If
    End Sub

    ''' <summary>
    ''' 从html代码中检索脚本代码.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Protected Sub btnRetrieveSriptCode_Click(ByVal sender As Object, ByVal e As EventArgs)
        strWholeHtml = Me.GetWholeHtmlCode(strUrl)
        If flgPageRetrieved Then
            Dim strRegexScript As String = "(?m)<script[^>]*>(\w|\W)*?</script[^>]*>"
            Dim strRegex As String = "<[^>]*>"
            Dim strMatchScript As String = String.Empty
            Dim matchList As MatchCollection = Regex.Matches(strWholeHtml, strRegexScript, RegexOptions.IgnoreCase)
            Dim strbScriptList As New StringBuilder()
            For Each matchSingleScript As Match In matchList
                Dim strSingleScriptText As String = Regex.Replace(matchSingleScript.Value, strRegex, String.Empty, RegexOptions.IgnoreCase)
                strbScriptList.Append(strSingleScriptText & vbCr & vbLf)
            Next
            tbResult.Text = strbScriptList.ToString()
        Else
            tbResult.Text = MsgPageRetrieveFailed
        End If
    End Sub

    ''' <summary>
    ''' 从html代码中检索图片信息.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Protected Sub btnRetrieveImage_Click(ByVal sender As Object, ByVal e As EventArgs)
        strWholeHtml = Me.GetWholeHtmlCode(strUrl)
        If flgPageRetrieved Then
            Dim strRegexImg As String = "(?is)<img.*?>"
            Dim matchList As MatchCollection = Regex.Matches(strWholeHtml, strRegexImg, RegexOptions.IgnoreCase)
            Dim strbImageList As New StringBuilder()

            For Each matchSingleImage As Match In matchList
                strbImageList.Append(matchSingleImage.Value + vbCr & vbLf)
            Next
            tbResult.Text = strbImageList.ToString()
        Else
            tbResult.Text = MsgPageRetrieveFailed
        End If
    End Sub

    ''' <summary>
    ''' 从html代码中检索链接.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Protected Sub btnRetrievelink_Click(ByVal sender As Object, ByVal e As EventArgs)
        strWholeHtml = Me.GetWholeHtmlCode(strUrl)
        If flgPageRetrieved Then
            Dim strRegexLink As String = "(?is)<a .*?>"
            Dim matchList As MatchCollection = Regex.Matches(strWholeHtml, strRegexLink, RegexOptions.IgnoreCase)
            Dim strbLinkList As New StringBuilder()

            For Each matchSingleLink As Match In matchList
                strbLinkList.Append(matchSingleLink.Value + vbCr & vbLf)
            Next
            tbResult.Text = strbLinkList.ToString()
        Else
            tbResult.Text = MsgPageRetrieveFailed
        End If
    End Sub

End Class