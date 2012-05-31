'**************************************** 模块头 *****************************************'
' 模块名:    Default.aspx.vb
' 项目名:    VBASPNETAddControlDynamically
' 版权 (c) Microsoft Corporation
'
' 这个项目演示了如何向ASP.NET页面动态添加控件. 它虚构了客户需要输入多于一个且
' 无上限的地址信息的情景. 因此我们使用按钮添加新地址TextBox.当用户输入完所有地址, 
' 我们也使用按钮在数据库更新这些信息, 在本示例中运行为显示这些地址.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
'
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'******************************************************************************************'

Public Class _Default
    Inherits System.Web.UI.Page

    Protected Sub Page_PreLoad(ByVal sender As Object, ByVal e As EventArgs) Handles Me.PreLoad
        ' 使用ViewState储存地址总数.
        If ViewState("AddressCount") Is Nothing Then
            ViewState("AddressCount") = 0
        End If

        ' 获得地址总数.
        Dim addresscount As Integer = CInt(ViewState("AddressCount"))

        ' 递归添加地址输入组件.
        For i As Integer = 0 To addresscount - 1
            AddAdress((i + 1).ToString())
        Next
    End Sub

    Protected Sub btnAddAddress_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnAddAddress.Click
        If ViewState("AddressCount") IsNot Nothing Then
            Dim btncount As Integer = CInt(ViewState("AddressCount"))

            ' 添加一个新组件到pnlAddressContainer.
            AddAdress((btncount + 1).ToString())
            ViewState("AddressCount") = btncount + 1
        Else
            Response.Write("出错了")
            Response.End()
        End If
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSave.Click
        Dim addresscount As Integer = CInt(ViewState("AddressCount"))

        ' 在页面上显示所有地址.
        ' 这里模仿我们将这些地址写入数据库.
        For i As Integer = 0 To addresscount - 1
            Dim tb As TextBox = TryCast(pnlAddressContainer.FindControl("TextBox" & (i + 1).ToString()), TextBox)
            Dim address As String = If(tb.Text = "", "空白", tb.Text)
            Response.Write("地址" & (i + 1).ToString() & " 是 " & address & ".<br />")
        Next

        ' 清除ViewState.
        ViewState("AddressCount") = Nothing
    End Sub

    Protected Sub AddAdress(ByVal id As String)
        ' 用以显示地址编号的Label.
        Dim lb As New Label()
        lb.Text = "地址" & id & ": "

        ' 用以输入地址的TextBox.
        Dim tb As New TextBox()
        tb.ID = "TextBox" & id

        If id <> "1" Then
            ' 可以尝试不带判定条件下的这句代码.
            ' 单击Save按钮后会有很奇怪的行为.
            tb.Text = Request.Form(tb.ID)
        End If

        ' 用以检查地址的Button.
        ' 同时演示如何绑定事件到一个动态控件上.
        Dim btn As New Button()
        btn.Text = "检查"
        btn.ID = "Button" & id

        ' 使用AddressOf语句绑定事件.
        AddHandler btn.Click, AddressOf ClickEvent

        Dim lt As New Literal() With {.Text = "<br />"}

        ' 作为一个组件添加这些控件到pnlAddressContainer.
        pnlAddressContainer.Controls.Add(lb)
        pnlAddressContainer.Controls.Add(tb)
        pnlAddressContainer.Controls.Add(btn)
        pnlAddressContainer.Controls.Add(lt)

    End Sub

    Protected Sub ClickEvent(ByVal sender As Object, ByVal e As EventArgs)
        ' 从sender获得按钮实例.
        Dim btn As Button = TryCast(sender, Button)

        ' 通过FindControl()方法获得TextBox实例和它的值.
        Dim tb As TextBox = TryCast(pnlAddressContainer.FindControl(btn.ID.Replace("Button", "TextBox")), TextBox)

        Dim address As String = If(tb.Text = "", "空白", tb.Text)

        ' 弹出一个消息框显示对应的地址.
        Dim sb As New System.Text.StringBuilder()
        sb.Append("<script type=""text/javascript"">")
        sb.Append("alert(""地址" & btn.ID.Replace("Button", "") & " is " & address & "."");")
        sb.Append("</script>")

        If Not ClientScript.IsClientScriptBlockRegistered(Me.GetType(), "AlertClick") Then
            ClientScript.RegisterClientScriptBlock(Me.GetType(), "AlertClick", sb.ToString())
        End If
    End Sub


End Class