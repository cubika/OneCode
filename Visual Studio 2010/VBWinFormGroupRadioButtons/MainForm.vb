'/************************************* 模块头  **************************************\
'* 模块名:	MainForm.vb
'* 项目 :		VBWinFormGroupRadioButtons
'* Copyright (c) Microsoft Corporation.
'* 
'* 这个例子展示了怎样在不同的容器中组织RadioButtons。
'* 
'* 关于RadioButton控件的更多信息,请看:
'* 
'*  Windows Forms RadioButton control
'*  http://msdn.microsoft.com/en-us/library/f5h102xz.aspx
'* 
'* This source is subject to the Microsoft Public License.
'* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
'* All other rights reserved.
'* 
'* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
'* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
'* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'\******************************************************************************************/

Imports System.Windows.Forms

Public Class MainForm
    ' 存储老的RadioButton
    Private radTmp As RadioButton = Nothing

    Private Sub MainForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ' 选中MainForm.Designer.cs文件中的rad1，然后这个RadioButton变为老的。
        radTmp = Me.rad1
    End Sub

    ' 在MainForm.Designer.cs文件中让4个Radiobutton使用这个方法处理它们的CheckedChanged 事件
    Private Sub radioButton_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rad3.CheckedChanged, rad2.CheckedChanged, rad1.CheckedChanged, rad4.CheckedChanged
        If (radTmp IsNot Nothing) Then
            ' 不选中老的
            radTmp.Checked = False
            radTmp = DirectCast(sender, RadioButton)

            ' 找到选中的
            If (radTmp.Checked) Then
                Me.lb.Text = radTmp.Name + "已经选中"
            End If
        End If
    End Sub
End Class