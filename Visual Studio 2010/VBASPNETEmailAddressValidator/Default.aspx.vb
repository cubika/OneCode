'/****************************** 模块头 ******************************\
' 模块名:    Default.aspx.vb
' 项目名:    VBASPNETEmailAddressValidator
' 版权 (c) Microsoft Corporation
'
' 本项目阐述了如何发送一封确认的邮件去检查一个邮件地址的可用性.
' 
' 在本文件中，我们创建一些控件来调用发送确认邮件的代码.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
'
'\*****************************************************************************/


Imports System.Linq
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls


Partial Public Class _Default
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
    End Sub

    ' 如果我们单击验证按钮, 它将会开始发送验证邮件.
    Protected Sub btnValidate_Click(ByVal sender As Object, ByVal e As EventArgs)
        btnSendAgain.Visible = False
        Try

            ' 实例化一个EmailValidation类，准备开始发送确认邮件.
            Dim validator As New EmailValidation(
                tbSendMail.Text, _
                Session("password").ToString(), _
                tbHost.Text, _
                chkUseSSL.Checked)

            ' 调用StartToValidateEmail方法验证邮箱地址并且发送确认邮件.
            Dim rsl As ValidateEmailResult =
                validator.StartToValidateEmail(tbValidateEmail.Text)
            Select Case rsl
                Case ValidateEmailResult.EmailStartToValidate
                    lbMessage.Text = "验证邮件已经成功发送." _
                        & "请在邮箱查收."
                    btnSendAgain.Visible = True
                    Exit Select
                Case ValidateEmailResult.EmailValidated
                    lbMessage.Text = "此邮箱已经通过验证."
                    Exit Select
                Case ValidateEmailResult.EmailValidating
                    lbMessage.Text = "此邮箱正在等待用户" _
                        & "在邮件中点击确认链接结束验证"
                    btnSendAgain.Visible = True
                    Exit Select
            End Select
        Catch err As Exception
            lbMessage.Text = "错误:" & err.Message
        End Try
    End Sub

    ' 如果我们已经发送了确认邮件，但是用户仍然没有收到 
    ' 邮件，我们可以允许他进行重发.
    Protected Sub btnSendEmailAgain_Click(ByVal sender As Object, ByVal e As EventArgs)
        Try

            ' 实例化一个EmailValidation类，准备再次发送确认邮件.
            Dim validator As New EmailValidation(
                tbSendMail.Text, _
                Session("password").ToString(), _
                tbHost.Text, _
                chkUseSSL.Checked)

            ' 使用ReSendValidationEmail方法再次发送确认邮件.
            validator.ReSendValidationEmail(tbValidateEmail.Text)
            lbMessage.Text = "邮件已经再次发送. 请再次查收."
        Catch err As Exception
            lbMessage.Text = "错误:" & err.Message
        End Try
    End Sub

    Protected Sub ValidationWizard_OnNextButtonClick(ByVal sender As Object, _
                                      ByVal e As WizardNavigationEventArgs)
        If e.CurrentStepIndex = 0 Then
            Session("password") = tbSendMailPassword.Text
        End If
    End Sub
End Class
