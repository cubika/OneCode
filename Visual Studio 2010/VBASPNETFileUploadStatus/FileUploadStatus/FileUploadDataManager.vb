'************************************ 模块头 ******************************\
' 模块名:    UploadFileCollection.vb
' 项目名:    VBASPNETFileUploadStatus
' 版权 (c) Microsoft Corporation
'
' 本项目阐述了在不使用第三方组件时实现显示上传的状态和进程
' 像ActiveX 控件,Flash 或者Silverlight.
' 
' 这个类用来过滤请求实体的文件数据并且把它们
' 存储在UploadFileCollection. 
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'****************************************************************************/

Imports System.Text
Imports System.Text.RegularExpressions

Friend Class FileUploadDataManager
    Private isFinished As Boolean = True
    Private draft As Byte() = Nothing
    Private isFile As Boolean = False

    Private ReadOnly _backSpace As Byte() = Nothing
    Private ReadOnly _doubleBackSpace As Byte() = Nothing
    Private ReadOnly _boundary As Byte() = Nothing
    Private ReadOnly _endTag As Byte() = Nothing

    Public Sub New(ByVal boundary__1 As String)
        _boundary = ASCIIEncoding.ASCII.GetBytes("--" + boundary__1)
        _backSpace = ASCIIEncoding.ASCII.GetBytes(vbCr & vbLf)
        _doubleBackSpace = ASCIIEncoding.ASCII.GetBytes(vbCr & vbLf & vbCr & vbLf)
        _endTag = ASCIIEncoding.ASCII.GetBytes("--" + boundary__1 + "--" & vbCr & vbLf)
        FilterResult = New UploadFileCollection()
        isFinished = True
        draft = Nothing
        isFile = False

    End Sub

    Public Property FilterResult() As UploadFileCollection
        Get
            Return _filterResult
        End Get
        Private Set(ByVal value As UploadFileCollection)
            _filterResult = value
        End Set
    End Property
    Private _filterResult As UploadFileCollection

    Public Sub AppendData(ByVal data As Byte())
        FilterFileDataFromBodyEntity(data)
        If data Is Nothing Then
            Return
        End If
    End Sub

    Private Sub AppendToLastEntity(ByVal data As Byte())
        If Not isFinished AndAlso isFile Then
            Dim lastFile As UploadFile = FilterResult(FilterResult.Count - 1)
            lastFile.AppendData(data)
        End If
    End Sub

    ' 过滤请求数据来获取文件的二进制数据.
    ' 这里是过滤数据的算法逻辑. 
    ' 在我们讲到算法之前将会有很多内容要 
    ' 讲（可能要一本书）.所以我只在这里开
    ' 个头，但是忽略详细的解释.
    Private Sub FilterFileDataFromBodyEntity(ByVal data As Byte())
        If data Is Nothing Then
            Return
        End If

        If draft IsNot Nothing Then
            Dim temp As Byte() =
                BinaryHelper.Combine(draft,
                                     BinaryHelper.Copy(data, 0, _boundary.Length))
            Dim entity_st As Integer =
                BinaryHelper.SequenceIndexOf(temp,
                                             _boundary,
                                             0)
            Dim entity_ed As Integer =
                BinaryHelper.SequenceIndexOf(temp,
                                             _boundary,
                                             entity_st + _boundary.Length + 2)

            If isFile AndAlso Not isFinished Then
                If entity_st = 0 Then
                    Dim header_st As Integer = entity_st + _boundary.Length + 2
                    Dim header_ed As Integer =
                        BinaryHelper.SequenceIndexOf(temp,
                                                     _doubleBackSpace, header_st)

                    Dim body_st As Integer = header_ed + 4
                    If entity_ed = -1 Then
                        AppendToLastEntity(BinaryHelper.SubData(draft, body_st))
                        draft = Nothing
                    Else
                        AppendToLastEntity(BinaryHelper.SubData(draft,
                                                                body_st,
                                                                entity_ed - body_st - 2))
                        isFinished = True
                        isFile = False
                        draft = BinaryHelper.SubData(draft, entity_ed)
                    End If
                Else
                    AppendToLastEntity(draft)
                    draft = Nothing
                End If
            End If

            ' 当需要添加新数据时，
            ' 把这两个二进制数组合并成一个.
            data = BinaryHelper.Combine(draft, data)
            draft = Nothing
        End If
        While True
            ' 找到边界
            Dim entity_st As Integer = BinaryHelper.SequenceIndexOf(data, _boundary, 0)

            ' 如果当前加载的数据包含边界
            If entity_st > -1 Then
                If isFile AndAlso Not isFinished Then
                    AppendToLastEntity(BinaryHelper.SubData(data, 0, entity_st - 2))
                    data = BinaryHelper.SubData(data, entity_st)
                    isFile = False
                    isFinished = True
                    Continue While
                End If

                Dim entity_ed As Integer = BinaryHelper.SequenceIndexOf(data,
                                                                        _boundary,
                                                                        entity_st + _boundary.Length + 2)
                Dim header_st As Integer = entity_st + _boundary.Length + 2
                Dim header_ed As Integer = BinaryHelper.SequenceIndexOf(data,
                                                                        _doubleBackSpace,
                                                                        header_st)
                Dim body_st As Integer = header_ed + 4

                If body_st < 4 Then
                    ' 如果实体的头部不完整，
                    ' 设置这个实体作为数据，
                    ' 卸载掉函数来请求更多的数据.
                    draft = data
                    Return
                Else
                    ' 如果实体的头部完整  
                    If Not isFile AndAlso isFinished Then

                        ' 把实体头部的数据转换为UTF8编码
                        Dim headerInEntity As String = ASCIIEncoding.UTF8.GetString(
                            BinaryHelper.SubData(data, header_st, header_ed - header_st))

                        ' 如果这是一个文件实体，头部包会含关键字："filename".
                        If headerInEntity.IndexOf("filename") > -1 Then
                            ' 在实体的头部使用正则表达式来 
                            ' 获取元数据的关键字值.
                            Dim detailsReg As New Regex("Content-Disposition: form-data; name=""([^""]*)"";" +
                                                        " filename=""([^""]*)""Content-Type: ([^""]*)")
                            Dim regMatch As Match =
                                detailsReg.Match(headerInEntity.Replace(vbCr & vbLf, ""))

                            Dim controlName As String = regMatch.Groups(1).Value
                            Dim clientPath As String = regMatch.Groups(2).Value
                            Dim contentType As String = regMatch.Groups(3).Value
                            If String.IsNullOrEmpty(clientPath) Then
                                isFile = False
                            Else
                                isFile = True
                                ' 为文件实体创建一个新的实例
                                Dim up As New UploadFile(clientPath, contentType)
                                FilterResult.Add(up)
                                isFinished = False
                            End If
                        Else
                            isFile = False
                        End If

                    End If
                End If
                If entity_ed > -1 Then
                    ' 如果我们在第一个边界之后又发现另一边界，
                    ' 那表示实体块在那里结束了.
                    ' 只是文件实体的时候，我们才需要在实体的
                    ' 主体里获取数据.
                    If isFile Then
                        AppendToLastEntity(BinaryHelper.SubData(data,
                                                                body_st,
                                                                entity_ed - body_st - 2))
                        isFinished = True
                        isFile = False
                    End If
                    ' 移除当前的实体处理数据并且
                    ' 循环到下一个数据.
                    data = BinaryHelper.SubData(data, entity_ed)
                    If BinaryHelper.Equals(data, _endTag) Then
                        data = Nothing
                        draft = Nothing
                        Return
                    End If
                    Continue While
                Else
                    ' 如果我们不能找到结束标记,我们
                    ' 必须要把这些数据移到草稿并
                    ' 请求添加新数据.
                    draft = data
                    Return
                End If
            Else
                ' 如果我们不能找到结束标记,我们
                ' 必须要把这些数据移到草稿并
                ' 请求添加新数据.
                draft = data
                Return

            End If
        End While


    End Sub
End Class


