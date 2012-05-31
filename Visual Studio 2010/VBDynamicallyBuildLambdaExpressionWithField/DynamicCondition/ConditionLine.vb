'******************************** 模块头 *************************************************'
' 模块名:  ConditionLine.vb
' 项目名:  VBDynamicallyBuildLambdaExpressionWithField
' 版权 (c) Microsoft Corporation.
'
' The ConditionLine.vb 文件定义了一些子条件连接运算符和一些属性框.
'
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
'
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER 
' EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF 
' MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*************************************************************************************************'
Imports System.Reflection

Partial Friend Class ConditionLine

    Friend Sub New()
        InitializeComponent()
    End Sub

    ''' <summary>
    ''' DataType 属性.
    ''' </summary>
    Private _dataType As Type
    Public Property DataType() As Type
        Get
            Return _dataType
        End Get
        Set(ByVal value As Type)
            _dataType = value
        End Set
    End Property

    ''' <summary>
    ''' DataSource属性.
    ''' </summary>
    Private _dataSource() As PropertyInfo
    Public Property DataSource() As PropertyInfo()
        Get
            Return _dataSource
        End Get
        Set(ByVal value As PropertyInfo())
            _dataSource = value
            cmbColumn.DataSource = value
            cmbColumn.DisplayMember = "Name"
        End Set
    End Property

    ''' <summary>
    ''' 条件比较运算符.
    ''' </summary>
    Public Property OperatorType() As DynamicQuery.Condition.Compare
        Get
            Return (If(lb.Text = "AND", DynamicQuery.Condition.Compare.And, DynamicQuery.Condition.Compare.Or))
        End Get
        Set(ByVal value As DynamicQuery.Condition.Compare)
            If value <> DynamicQuery.Condition.Compare.And And value <> DynamicQuery.Condition.Compare.Or Then
                Throw New ArgumentException("操作符必须是 ""And"" 或者 ""Or""")
            End If
            lb.Text = value.ToString().ToUpper()
        End Set
    End Property

    ''' <summary>
    ''' 返回 Condition(Of T），它表示了存储在 UserControl 控件中的条件.
    ''' </summary>
    Public Function GetCondition(Of T)(ByVal dataSrc As T) As DynamicCondition.DynamicQuery.Condition(Of T)

        Dim pType = (CType(cmbColumn.SelectedItem, PropertyInfo)).PropertyType

        '  CheckType 确保 T 和 T? 被同样对待.
        If CheckType(Of Boolean)(pType) Then
            Return MakeCond(dataSrc, pType, chkValue.Checked)

        ElseIf CheckType(Of Date)(pType) Then
            Return MakeCond(dataSrc, pType, dtpValue.Value)
        ElseIf CheckType(Of Char)(pType) Then
            Return MakeCond(dataSrc, pType, Convert.ToChar(tbValue.Text))
        ElseIf CheckType(Of Long)(pType) Then
            Return MakeCond(dataSrc, pType, Convert.ToInt64(tbValue.Text))
        ElseIf CheckType(Of Short)(pType) Then
            Return MakeCond(dataSrc, pType, Convert.ToInt16(tbValue.Text))
        ElseIf CheckType(Of ULong)(pType) Then
            Return MakeCond(dataSrc, pType, Convert.ToUInt64(tbValue.Text))
        ElseIf CheckType(Of UShort)(pType) Then
            Return MakeCond(dataSrc, pType, Convert.ToUInt16(tbValue.Text))
        ElseIf CheckType(Of Single)(pType) Then
            Return MakeCond(dataSrc, pType, Convert.ToSingle(tbValue.Text))
        ElseIf CheckType(Of Double)(pType) Then
            Return MakeCond(dataSrc, pType, Convert.ToDouble(tbValue.Text))
        ElseIf CheckType(Of Decimal)(pType) Then
            Return MakeCond(dataSrc, pType, Convert.ToDecimal(tbValue.Text))
        ElseIf CheckType(Of Integer)(pType) Then
            Return MakeCond(dataSrc, pType, Convert.ToInt32(SimulateVal.Val(tbValue.Text)))
        ElseIf CheckType(Of UInteger)(pType) Then
            Return MakeCond(dataSrc, pType, Convert.ToUInt32(tbValue.Text))

            ' 这只能是字符串，因为我们筛选了添加到符合框中的类型.
        Else
            Return MakeCond(dataSrc, pType, tbValue.Text)
        End If
    End Function


    Public Shared typeList As List(Of Type)

    ''' <summary>
    ''' where 关键字后面的协议.
    ''' </summary>
    Public Shared Function GetSupportedTypes() As List(Of Type)
        If typeList Is Nothing Then
            typeList = New List(Of Type)()
            typeList.AddRange(New Type() {GetType(Date), GetType(Date?), GetType(Char), GetType(Char?),
                                          GetType(Long), GetType(Long?), GetType(Short), GetType(Short?),
                                          GetType(ULong), GetType(ULong?), GetType(UShort), GetType(UShort?),
                                          GetType(Single), GetType(Single?), GetType(Double), GetType(Double?),
                                          GetType(Decimal), GetType(Decimal?), GetType(Boolean), GetType(Boolean?),
                                          GetType(Integer), GetType(Integer?), GetType(UInteger), GetType(UInteger?),
                                          GetType(String)})
        End If

        Return typeList
    End Function

    ''' <summary>
    ''' 组合条件.  
    ''' </summary>
    Private Sub ConditionLine_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        cmbOperator.DisplayMember = "Name"
        cmbOperator.ValueMember = "Value"
        Dim opList = MakeList(New With {Key .Name = "等于", Key .Value = DynamicQuery.Condition.Compare.Equal},
                              New With {Key .Name = "不等于", Key .Value = DynamicQuery.Condition.Compare.NotEqual},
                              New With {Key .Name = ">", Key .Value = DynamicQuery.Condition.Compare.GreaterThan},
                              New With {Key .Name = ">=", Key .Value = DynamicQuery.Condition.Compare.GreaterThanOrEqual},
                              New With {Key .Name = "<", Key .Value = DynamicQuery.Condition.Compare.LessThan},
                              New With {Key .Name = "<=", Key .Value = DynamicQuery.Condition.Compare.LessThanOrEqual},
                              New With {Key .Name = "Like", Key .Value = DynamicQuery.Condition.Compare.Like})
        cmbOperator.DataSource = opList
    End Sub

    ''' <summary>
    ''' 当获得用户选取的属性时，选择用于演示的控件.
    ''' </summary>
    Private Sub cboColumn_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles cmbColumn.SelectedIndexChanged

        ' 获取用户选取的属性的基础类型. 
        Dim propType = (CType(cmbColumn.SelectedItem, PropertyInfo)).PropertyType

        ' 为属性类型展示合适的控件（CheckBox/TextBox/DateTimePicker).
        If CheckType(Of Boolean)(propType) Then
            SetVisibility(True, False, False)

        ElseIf CheckType(Of Date)(propType) Then
            SetVisibility(False, True, False)
        Else
            SetVisibility(False, False, True)
        End If
    End Sub

    ''' <summary>
    ''' 设置控件的可见性
    ''' </summary>
    Private Sub SetVisibility(ByVal chkBox As Boolean, ByVal datePicker As Boolean, ByVal txtBox As Boolean)
        chkValue.Visible = chkBox
        tbValue.Visible = txtBox
        dtpValue.Visible = datePicker
    End Sub

    ''' <summary>
    ''' AND与OR间的切换.
    ''' </summary>
    Private Sub lblOperator_Click(ByVal sender As Object, ByVal e As EventArgs) Handles lb.Click
        lb.Text = (If(lb.Text = "AND", "OR", "AND"))
    End Sub

    ''' <summary>
    '''  MakeCond 运算符. 
    ''' </summary>
    Private Function MakeCond(Of T, S)(ByVal dataSource As T, ByVal propType As Type, ByVal value As S) As DynamicQuery.Condition(Of T)
        Dim dataSourceType As IEnumerable(Of T) = Nothing
        Return DynamicCondition.DynamicQuery.Condition.Create(Of T)(dataSourceType, cmbColumn.Text,
                                                                    CType(cmbOperator.SelectedValue, 
                                                                        DynamicQuery.Condition.Compare),
                                                                    value, propType)
    End Function

    ''' <summary>
    ''' 当 proType 是 T 类型或者 Nullable（Of T) 时，返回true.
    ''' </summary>
    Private Shared Function CheckType(Of T As Structure)(ByVal propType As Type) As Boolean
        Return (propType.Equals(GetType(T)) Or propType.Equals(GetType(T?)))
    End Function

    ''' <summary>
    ''' 将参数列表转换到 IEnumerable(Of T) (其中T在本例中是匿名类型)中.
    ''' </summary>
    Private Shared Function MakeList(Of T)(ByVal ParamArray items() As T) As IEnumerable(Of T)
        Return items
    End Function
End Class
