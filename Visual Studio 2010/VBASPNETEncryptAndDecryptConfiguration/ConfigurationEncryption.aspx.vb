'********************************* 模块头 *********************************\
' 模块名:           ConfigurationEncryption.aspx.vb
' 项目名:      VBASPNETEncryptAndDecryptConfiguration
' 版权(c) Microsoft Corporation.
' 
'  此示例演示如何使用RSA加密算法API的加密和解密配置节点,以保护ASP.NET Web应用程序的敏感
' 信息,防止拦截或劫持.
'
' 这个项目包含两个片段.第一个演示了如何使用RSA提供和RSA容器中进行加密和解密一些Web应用
' 程序中的词或值.第一个片段的目的是让我们知道RSA的机制概述.第二个则显示了如何使用RSA
' 配置提供加密和解密web.config中的配置节点.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
'
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'\***************************************************************************/

Imports System.Web.Configuration

Public Class ConfigurationEncryption_aspx
    Inherits System.Web.UI.Page

    Private Const provider As String = "RSAProtectedConfigurationProvider"
    '使用RSA提供器加密配置节点

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Protected Sub btnEncrypt_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnEncrypt.Click
        If String.IsNullOrEmpty(Me.ddlSection.SelectedValue) Then
            Response.Write("请选择一个配置节点")
            Return
        End If

        Dim sectionString As String = Me.ddlSection.SelectedValue

        Dim config As System.Configuration.Configuration = WebConfigurationManager.OpenWebConfiguration(Request.ApplicationPath)
        Dim section As ConfigurationSection = config.GetSection(sectionString)
        If section IsNot Nothing Then
            section.SectionInformation.ProtectSection(provider)
            config.Save()
            Response.Write("加密成功,请检查配置文件.")
        End If
    End Sub

    Protected Sub btnDecrypt_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnDecrypt.Click
        Dim sectionString As String = Me.ddlSection.SelectedValue

        Dim config As System.Configuration.Configuration = WebConfigurationManager.OpenWebConfiguration(Request.ApplicationPath)
        Dim section As ConfigurationSection = config.GetSection(sectionString)
        If section IsNot Nothing AndAlso section.SectionInformation.IsProtected Then
            section.SectionInformation.UnprotectSection()
            config.Save()
            Response.Write("解密成功,请检查配置文件.")
        End If
    End Sub
End Class