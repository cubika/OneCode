'************************************* 模块头 **************************************\
' 模块名称:	EditorForm.vb
' 项目名称:		VBWinFormDesignerCustomEditor
' 版权所有 (c) 微软公司
' 
' 
' 这个VBWinFormDesignerCustomEditor例子说明了如何在设计阶段使用一个自定义编辑器来编辑一个特殊的属性。
' 
' 
' 这个资源受到微软公共许可的。
' 参见 http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' 保留所有其他权利。

' 这里所提供的原样的明示的或者暗示的代码和信息不受任何性质的担保，
' 但不限于那些特定用途的适销性和/或适用性的隐式担保。
'***************************************************************************'

Public Class EditorForm
    Private _subCls As SubClass = New SubClass

    Public Sub New()
        Me._subCls = New SubClass
        Me.components = Nothing
        Me.InitializeComponent()
        AddHandler MyBase.Load, New EventHandler(AddressOf Me.EditorForm_Load)
    End Sub

    Public Sub New(ByVal value As SubClass)
        Me.subCls = New SubClass
        Me.components = Nothing
        Me.InitializeComponent()
        Me._subCls = value
        AddHandler MyBase.Load, New EventHandler(AddressOf Me.EditorForm_Load)
    End Sub

    Private Sub button1_Click(ByVal sender As Object, ByVal e As EventArgs) Handles button1.Click
        Me._subCls.MyDate = Me.monthCalendar1.SelectionStart
        Me._subCls.Name = Me.textBox1.Text
        MyBase.DialogResult = DialogResult.OK
    End Sub

    Private Sub button2_Click(ByVal sender As Object, ByVal e As EventArgs) Handles button2.Click
        MyBase.DialogResult = DialogResult.Cancel
    End Sub


    Private Sub EditorForm_Load(ByVal sender As Object, ByVal e As EventArgs)
        Me.textBox1.Text = Me._subCls.Name
        Me.monthCalendar1.SelectionStart = Me._subCls.MyDate
        Me.monthCalendar1.SelectionEnd = Me._subCls.MyDate
    End Sub

    Public Property SubCls() As SubClass
        Get
            Return Me._subCls
        End Get
        Set(ByVal value As SubClass)
            Me._subCls = value
        End Set
    End Property
End Class