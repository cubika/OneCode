'/**************************************** 模块头 *****************************************\
'* 模块名:    Default.aspx.vb
'* 项目名:    VBASPNETImagEditUpload
'* 版权 (c) Microsoft Corporation
'*
'* 本项目演示了如何插入,编辑或更新一个图片并将其保存到Sql数据库中.
'*
'* This source is subject to the Microsoft Public License.
'* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
'* All other rights reserved.
'*
'* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
'* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
'* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'\*****************************************************************************************/

Imports System.Collections.Generic
Imports System.Web.UI.WebControls
Imports System.Data.SqlClient
Imports System.IO

Partial Public Class _Default
    Inherits System.Web.UI.Page

    '检查常用图片静态类型.
    Private Shared imgytpes As New List(Of String)() From { _
     ".BMP", _
     ".GIF", _
     ".JPG", _
     ".PNG" _
    }

    ''' <summary>
    ''' 读取所有记录到GridView.
    ''' 如果存在记录, 默认选择第一条记录显示在FormView;
    ''' 否则, 改变formview为插入模式可以使数据被插入.
    ''' </summary>
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

        If Not IsPostBack Then

            gvPersonOverView.DataBind()

            If gvPersonOverView.Rows.Count > 0 Then
                gvPersonOverView.SelectedIndex = 0
                fvPersonDetails.ChangeMode(FormViewMode.[ReadOnly])
                fvPersonDetails.DefaultMode = FormViewMode.[ReadOnly]
            Else
                fvPersonDetails.ChangeMode(FormViewMode.Insert)
                fvPersonDetails.DefaultMode = FormViewMode.Insert
            End If
        End If
    End Sub

    ''' <summary>
    ''' 验证数据是否满足图片类型.
    ''' </summary>
    Protected Sub CustomValidator1_ServerValidate(ByVal source As Object, ByVal args As ServerValidateEventArgs)
        If args.Value IsNot Nothing AndAlso args.Value <> "" Then
            args.IsValid = imgytpes.IndexOf(System.IO.Path.GetExtension(args.Value).ToUpper()) >= 0
        End If

    End Sub

    ''' <summary>
    ''' 在检查验证图片类型之后,
    ''' 通过e.Values参数关联图片类型和图片字节集合
    ''' 然后插入.
    ''' </summary>
    Protected Sub fvPersonDetails_ItemInserting(ByVal sender As Object, ByVal e As FormViewInsertEventArgs)
        Dim obj As Object = Session("insertstate")
        If obj Is Nothing OrElse CBool(obj) Then
            Dim cv As CustomValidator = TryCast(fvPersonDetails.FindControl("cmvImageType"), CustomValidator)

            cv.Validate()
            e.Cancel = Not cv.IsValid

            Dim fup As FileUpload = DirectCast(fvPersonDetails.FindControl("fupInsertImage"), FileUpload)

            If cv.IsValid AndAlso fup.PostedFile.FileName.Trim() <> "" Then
                e.Values("PersonImage") = File.ReadAllBytes(fup.PostedFile.FileName)
                e.Values("PersonImageType") = fup.PostedFile.ContentType

            End If
        Else
            e.Cancel = True
            gvPersonOverView.DataBind()
            fvPersonDetails.ChangeMode(FormViewMode.[ReadOnly])
            fvPersonDetails.DefaultMode = FormViewMode.[ReadOnly]
        End If
    End Sub

    ''' <summary>
    ''' 在检查验证图片类型之后,
    ''' 通过e.Values参数关联图片类型和图片字节集合
    ''' 然后更新.
    ''' </summary>

    Protected Sub fvPersonDetails_ItemUpdating(ByVal sender As Object, ByVal e As FormViewUpdateEventArgs)
        Dim cv As CustomValidator = TryCast(fvPersonDetails.FindControl("cmvImageType"), CustomValidator)

        cv.Validate()
        e.Cancel = Not cv.IsValid

        Dim fup As FileUpload = DirectCast(fvPersonDetails.FindControl("fupEditImage"), FileUpload)

        If cv.IsValid AndAlso fup.PostedFile.FileName.Trim() <> "" Then
            e.NewValues("PersonImage") = File.ReadAllBytes(fup.PostedFile.FileName)
            e.NewValues("PersonImageType") = fup.PostedFile.ContentType
        End If
    End Sub

    ''' <summary>
    ''' 更新成功后, 重新绑定数据,默认选择第一个.
    ''' </summary>
    Protected Sub fvPersonDetails_ItemUpdated(ByVal sender As Object, ByVal e As FormViewUpdatedEventArgs)
        gvPersonOverView.DataBind()
        gvPersonOverView.SelectedIndex = gvPersonOverView.SelectedRow.RowIndex
    End Sub

    ''' <summary>
    ''' 插入成功后, 重新绑定数据,默认选择第一个,
    ''' 改变FormView模式为只读(查看用).
    ''' </summary>
    Protected Sub fvPersonDetails_ItemInserted(ByVal sender As Object, ByVal e As FormViewInsertedEventArgs)
        gvPersonOverView.DataBind()
        gvPersonOverView.SelectedIndex = gvPersonOverView.Rows.Count - 1
        fvPersonDetails.ChangeMode(FormViewMode.[ReadOnly])
        fvPersonDetails.DefaultMode = FormViewMode.[ReadOnly]
    End Sub

    ''' <summary>
    ''' 删除成功后 , 重新绑定数据.
    ''' </summary>
    Protected Sub fvPersonDetails_ItemDeleted(ByVal sender As Object, ByVal e As FormViewDeletedEventArgs)
        gvPersonOverView.DataBind()

        If gvPersonOverView.Rows.Count > 0 Then
            Dim delindex As Integer = CInt(ViewState("delindex"))
            If delindex = 0 Then
                gvPersonOverView.SelectedIndex = 0
            ElseIf delindex = gvPersonOverView.Rows.Count Then
                gvPersonOverView.SelectedIndex = gvPersonOverView.Rows.Count - 1
            Else
                gvPersonOverView.SelectedIndex = delindex

            End If
        Else
            fvPersonDetails.ChangeMode(FormViewMode.Insert)
            fvPersonDetails.DefaultMode = FormViewMode.Insert
        End If
    End Sub

    ''' <summary>
    ''' 当GridView的SelectedRowIndex改变时在FormView中显示图片和详细信息.
    ''' </summary>
    Protected Sub gvPersonOverView_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs)
        fvPersonDetails.ChangeMode(FormViewMode.[ReadOnly])
        fvPersonDetails.DefaultMode = FormViewMode.[ReadOnly]
    End Sub

    ''' <summary>
    ''' 在ViewState中保持行索引用来Item_Deleted.
    ''' </summary>
    Protected Sub fvPersonDetails_ItemDeleting(ByVal sender As Object, ByVal e As FormViewDeleteEventArgs)
        ViewState("delindex") = gvPersonOverView.SelectedIndex
    End Sub

    ''' <summary>
    ''' 在Session中保持insertState防止刷新页面后的重复插入.
    ''' </summary>
    Protected Sub fvPersonDetails_ModeChanging(ByVal sender As Object, ByVal e As FormViewModeEventArgs)
        Session("insertstate") = (e.NewMode = FormViewMode.Insert)
    End Sub
End Class