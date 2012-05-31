'**************************************** 模块头 *****************************************\
' 模块名:  Default.aspx.vb
' 项目名:  VBASPNETImageMap
' 版权 (c) Microsoft Corporation
'
' 这个项目展示了如何使用ImageMap创建用VB.NET语言编写的太阳系行星系统的说明. 
' 当图片中的行星被单击时, 关于这个行星的简要信息将被显示在图片下面
' 同时iframe将被导航到WikiPedia上关联的页面. 
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
'
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,  
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************/


Partial Public Class _Default
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Protected Sub imgMapSolarSystem_Click(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.ImageMapEventArgs) Handles imgMapSolarSystem.Click

        ifSelectResult.Attributes.Item("src") = "http://zh.wikipedia.org/wiki/" & e.PostBackValue

        Select Case e.PostBackValue
            Case "太阳"
                '当用户单击太阳
                lbDirection.Text = "太阳是位于太阳系中心的恒星."

            Case "水星"
                lbDirection.Text = "水星是太阳系最内侧同时也是最小的行星."

            Case "金星"
                lbDirection.Text = "金星是离太阳第二近的行星."

            Case "地球"
                lbDirection.Text = "地球是太阳的第三颗行星. 同时也是我们所居住的世界, 又称为蓝星."

            Case "火星"
                lbDirection.Text = "火星是太阳系中离太阳第四近的行星."

            Case "木星"
                lbDirection.Text = "木星是太阳的第五颗行星同时也是太阳系最大的行星."

            Case "土星"
                lbDirection.Text = "土星是自太阳起第七颗行星, 也是太阳系中仅次于木星第二大的行星."

            Case "天王星"
                lbDirection.Text = "天王星是自太阳起第七颗行星, 同时也是太阳系中第三大和第四重的行星."

            Case "海王星"
                lbDirection.Text = "海王星是我们的太阳系中自太阳起第八颗行星."

            Case Else

        End Select

    End Sub
End Class