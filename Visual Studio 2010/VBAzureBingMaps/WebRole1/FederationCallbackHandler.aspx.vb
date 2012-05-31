'/********************************* 模块头 **********************************\
'* 模块名:  FederationCallbackHandler.aspx.vb
'* 项目名:  AzureBingMaps
'* 版权 (c) Microsoft Corporation.
'* 
'* 回调句柄. 配置ACS和Messenger Connect在用户登入后重定向到此页面.
'* 
'* This source is subject to the Microsoft Public License.
'* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
'* All other rights reserved.
'* 
'* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
'* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
'* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'\***************************************************************************/

Imports System.Linq
Imports System.Net
Imports System.ServiceModel.Syndication
Imports System.Threading
Imports System.Web
Imports System.Xml
Imports Microsoft.IdentityModel.Claims

Partial Public Class FederationCallbackHandler
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
        ' 从会话中获得的返回页面，重定向到此页面.
        Dim returnPage As String = "HtmlClient.aspx"
        If Session("ReturnPage") IsNot Nothing Then
            returnPage = DirectCast(Session("ReturnPage"), String)
        End If

        ' 解析wl_internalState cookie,
        ' 展开Windows Live Messenger Connect Profile API相关信息.
        ' 用户没有尝试使用Live ID登入时wl_internalState可以为空.
        If Response.Cookies("wl_internalState") IsNot Nothing Then
            Dim accessToken As String = Me.ExtractWindowsLiveInternalState("wl_accessToken")
            Dim cid As String = Me.ExtractWindowsLiveInternalState("wl_cid")
            Dim uri As String = "http://apis.live.net/V4.1/cid-" & cid & "/Profiles"

            ' 如果LiveID登入失败wl_internalState可能失效.
            If Not String.IsNullOrEmpty(accessToken) AndAlso Not String.IsNullOrEmpty(cid) Then

                ' 生成一个到profile API的请求.
                Dim request As HttpWebRequest = DirectCast(HttpWebRequest.Create(uri), HttpWebRequest)
                request.Headers("Authorization") = accessToken
                Dim response As HttpWebResponse = DirectCast(request.GetResponse(), HttpWebResponse)
                If response.StatusCode = HttpStatusCode.OK Then

                    ' 使用WCF Syndication API解析回复.
                    Dim xmlReader As XmlReader = XmlReader.Create(response.GetResponseStream())
                    Dim feed As SyndicationFeed = SyndicationFeed.Load(xmlReader)
                    Dim entry = feed.Items.FirstOrDefault()
                    If entry IsNot Nothing Then
                        Dim content = TryCast(entry.Content, XmlSyndicationContent)
                        If content IsNot Nothing Then

                            ' WindowsLiveProfile是关联profile API反应的类.
                            Dim profile = content.ReadContent(Of WindowsLiveProfile)()
                            Dim liveID = profile.Emails.Where(Function(m) String.Equals(m.Type, "WindowsLiveID")).FirstOrDefault()

                            ' 如果profile API成功,
                            ' 我们就可以获得用户的LiveID.
                            ' LiveID将代表用户身份.
                            ' 我们保存用户身份到会话.
                            If liveID IsNot Nothing Then
                                Session("User") = liveID.Address
                            End If
                        End If
                    End If
                    xmlReader.Close()
                End If
            End If
        End If

        ' 下列代码处理通过WIF的ACS.
        Dim principal = TryCast(Thread.CurrentPrincipal, IClaimsPrincipal)
        If principal IsNot Nothing AndAlso principal.Identities.Count > 0 Then
            Dim identity = principal.Identities(0)

            ' 查询email声明.
            Dim query = From c In identity.Claims Where c.ClaimType = ClaimTypes.Email Select c
            Dim emailClaim = query.FirstOrDefault()
            If emailClaim IsNot Nothing Then

                ' 保存用户身份到会话.
                Session("User") = emailClaim.Value
            End If
        End If
        ' 重定向用户到返回页面.
        Response.Redirect(returnPage)
    End Sub

    ''' <summary>
    ''' 自wl_internalState cookie提取Windows Live Messenger Connect Profile API信息.
    ''' cookie包含若干信息
    ''' 比如cid和访问标识.
    ''' </summary>
    ''' <param name="key">需提取的数据.</param>
    ''' <returns>数据值.</returns>
    Private Function ExtractWindowsLiveInternalState(ByVal key As String) As String
        Dim result As String = Request.Cookies("wl_internalState").Value
        Try
            result = HttpUtility.UrlDecode(result)
            result = result.Substring(result.IndexOf(key))
            result = result.Substring(key.Length + 3, result.IndexOf(","c) - key.Length - 4)

            ' 如果LiveID登入失败wl_internalState可能失效.
            ' 在此场合, 我们返回null.
        Catch
            result = Nothing
        End Try
        Return result
    End Function
End Class