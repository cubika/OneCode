'********************************* 模块头 *********************************\
' 模块名:           CommonEncryption.aspx.vb
' 项目名:      VBASPNETEncryptAndDecryptConfiguration
' 版权(c) Microsoft Corporation.
' 
' 此示例演示如何使用RSA加密算法API的加密和解密配置节点,以保护ASP.NET Web应用程序的敏感
' 信息,防止拦截或劫持.
'
' 这个项目包含两个片段.第一个演示了如何使用RSA提供和RSA容器中进行加密和解密一些Web应用
' 程序中的词或值.第一个片段的目的是让我们知道RSA的机制概述.第二个则显示了如何使用RSA
' 配置提供加密和解密web.config中的配置节点..
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
'
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'\***************************************************************************/

Imports System.Security.Cryptography

Public Class CommonEncryption_aspx
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        AddKeyUpEventOnTextControl()
        AddHandler btnEncrypt.PreRender, AddressOf btnEncrypt_PreRender
        AddHandler btnDecrypt.PreRender, AddressOf btnDecrypt_PreRender
    End Sub

    Private Sub RSAEncryption()
        Dim param As New CspParameters()
        param.KeyContainerName = "MyKeyContainer"
        Using rsa As New RSACryptoServiceProvider(param)
            Dim plaintext As String = Me.tbData.Text
            Dim plaindata As Byte() = System.Text.Encoding.[Default].GetBytes(plaintext)
            Dim encryptdata As Byte() = rsa.Encrypt(plaindata, False)
            Dim encryptstring As String = Convert.ToBase64String(encryptdata)
            Me.tbEncryptData.Text = encryptstring
        End Using
    End Sub

    Private Sub RSADecryption()
        Dim param As New CspParameters()
        param.KeyContainerName = "MyKeyContainer"
        Using rsa As New RSACryptoServiceProvider(param)
            Dim encryptdata As Byte() = Convert.FromBase64String(Me.tbEncryptData.Text)
            Dim decryptdata As Byte() = rsa.Decrypt(encryptdata, False)
            Dim plaindata As String = System.Text.Encoding.[Default].GetString(decryptdata)
            Me.tbDecryptData.Text = plaindata
        End Using
    End Sub

    Private Sub AddKeyUpEventOnTextControl()
        Dim script As String = String.Format("function PressFn(sender) {{" & vbCr & vbLf & " document.getElementById('{0}').disabled = sender.value == '' ? true : false;" & vbCr & vbLf & " }}", btnEncrypt.ClientID)
        tbData.Attributes("onkeyup") = "PressFn(this)"
        Page.ClientScript.RegisterStartupScript(Me.[GetType](), "DataKeyUp", script, True)
    End Sub

    Private Sub EnableDecryptButton()
        btnDecrypt.Enabled = If(Me.tbEncryptData.Text <> String.Empty, True, False)
    End Sub

    Private Sub EnableEncryptButton()
        btnEncrypt.Enabled = If(Me.tbData.Text <> String.Empty, True, False)
    End Sub

    Protected Sub Encrypt_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnEncrypt.Click
        RSAEncryption()
    End Sub

    Protected Sub Decrypt_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnDecrypt.Click
        RSADecryption()
    End Sub

    Private Sub btnDecrypt_PreRender(ByVal sender As Object, ByVal e As EventArgs)
        EnableDecryptButton()
    End Sub

    Private Sub btnEncrypt_PreRender(ByVal sender As Object, ByVal e As EventArgs)
        EnableEncryptButton()
    End Sub
End Class