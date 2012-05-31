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
'******************************************************************************************/

Imports System.ComponentModel
Imports System.Drawing.Design
Imports System.Windows.Forms.Design

Public Class UC_CustomEditor
    Private _cls As SubClass

    Public Sub New()
        InitializeComponent()
        _cls = New SubClass("Name", System.DateTime.Now)
    End Sub

    <TypeConverter(GetType(ExpandableObjectConverter)), Editor(GetType(MyEditor), GetType(UITypeEditor))> _
    Public Property Cls() As SubClass
        Get
            Me.lblName.Text = Me._cls.Name
            Me.lblDateTime.Text = Me._cls.MyDate.ToString
            Return Me._cls
        End Get
        Set(ByVal value As SubClass)
            Me.lblName.Text = value.Name
            Me.lblDateTime.Text = value.MyDate.ToString
            Me._cls = value
        End Set
    End Property

End Class

Public Class SubClass
    Private _date As DateTime = DateTime.Now
    Private _name As String

    Public Sub New()
        Me._date = DateTime.Now
    End Sub

    Public Sub New(ByVal name As String, ByVal time As DateTime)
        Me._date = DateTime.Now
        Me._date = time
        Me._name = name
    End Sub

    Public Property MyDate() As DateTime
        Get
            Return Me._date
        End Get
        Set(ByVal value As DateTime)
            Me._date = value
        End Set
    End Property

    Public Property Name() As String
        Get
            Return Me._name
        End Get
        Set(ByVal value As String)
            Me._name = value
        End Set
    End Property
End Class


' 这个类展示了一个自定义UITypeEditor的使用。
' 它允许UC_CustomEditor控件的Cls属性在设计期通过使用那些在属性窗口中被唤醒的自定义的用户界面元素来改变它的值
' 这个用户界面由EditorForm类来提供。
Public Class MyEditor
    Inherits UITypeEditor

    Private editorService As IWindowsFormsEditorService = Nothing

    Public Overrides Function EditValue(ByVal context As ITypeDescriptorContext, _
                ByVal provider As IServiceProvider, ByVal value As Object) As Object
        If (Not provider Is Nothing) Then
            Me.editorService = TryCast(provider.GetService(GetType(IWindowsFormsEditorService)),  _
            IWindowsFormsEditorService)
        End If
        If (Not Me.editorService Is Nothing) Then
            Dim editorForm As New EditorForm(DirectCast(value, SubClass))
            If (Me.editorService.ShowDialog(editorForm) = DialogResult.OK) Then
                value = editorForm.SubCls
            End If
        End If
        Return value
    End Function


    Public Overrides Function GetEditStyle(ByVal context As ITypeDescriptorContext) As UITypeEditorEditStyle
        Return UITypeEditorEditStyle.Modal
    End Function

    Public Overrides Function GetPaintValueSupported(ByVal context As ITypeDescriptorContext) As Boolean
        Return True
    End Function

    Public Overrides Sub PaintValue(ByVal e As PaintValueEventArgs)
    End Sub

End Class